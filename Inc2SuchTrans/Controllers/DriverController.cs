using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inc2SuchTrans.Models;
using Inc2SuchTrans.BLL;
using Inc2SuchTrans.CustomFilters;
using System.Data.Entity;

namespace Inc2SuchTrans.Controllers
{
    public class DriverController : BaseController
    {
        STLogisticsEntities db = new STLogisticsEntities();
        // GET: Driver
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AuthLog(Roles = "Admin, Driver")]
        public ActionResult SearchDelivery()
        {
            //if (String.IsNullOrEmpty(refkey))
            //{
            //    Information("Please Enter A Delivery Reference Key");
            //    return View();

            //else
            //{
            //    try
            //    {
            //        Delivery del = db.Delivery.Where(x => x.DeliveryRef == refkey).FirstOrDefault();
            //        if (del != null)
            //        {
            //            return RedirectToAction("DeliveryDetails", new { id = del.DelID });
            //        }
            //        else
            //        {
            //            Danger("Invalid Reference Key. Please make sure you have a valid key");
            //            return View();
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        Danger("Oops!! Something went wrong.. Please contact support. <br> Error: " + e.Message + "<br> StackTrace: " + e.StackTrace);
            //        return View();
            //    }
            //}

            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="refkey"></param>
        /// <returns></returns>
        [AuthLog(Roles = "Admin, Driver")]
        public ActionResult DeliveryDetails(string refkey)
        {
            if (String.IsNullOrEmpty(refkey))
            {
                try
                {
                    Information("Please Enter A Delivery Reference Key");
                    return RedirectToAction("SearchForDelivery");
                }
                catch (Exception e)
                {
                    Danger("Oops! Something went wrong..<br> Please contact support");
                    return View();
                    throw e;
                }
            }
            else
            {
                try
                {
                    Delivery del = db.Delivery.Where(x => x.DeliveryRef == refkey).SingleOrDefault();
                    if (del != null)
                    {
                        return View(del);
                    }
                    else
                    {
                        Danger("The reference key you have entered is not valid.. <br> Please make sure you have a valid reference key or contact support");
                        return RedirectToAction("SearchForDelivery");
                    }
                }
                catch (Exception e)
                {
                    Danger("Oops! Something went wrong..<br> Please contact support");
                    return View();
                    throw e;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthLog(Roles = "Admin, Driver")]
        public ActionResult Edit(int id)
        {
            try
            {
                Delivery del = db.Delivery.Find(id);
                if (del != null)
                {
                    return View(del);
                }
                else
                {
                    Danger("Error retrieving Delivery");
                    return View();
                }
            }
            catch (Exception e)
            {
                Danger("Oops!! Something went wrong.. Please contact support. <br> Error: " + e.Message + "<br> StackTrace: " + e.StackTrace);
                return View();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthLog(Roles = "Admin, Driver")]
        [HttpPost]
        public ActionResult Edit([Bind(Include = "DelID,CustomerID,CurrentDate,DeliveryDate,PickUpArea,DropOffArea,DropOffAdd,CargoSize,CargoWeight,TotalCost,Paid,DeliveryRef,DeliveryStatus")] Delivery delivery)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(delivery).State = EntityState.Modified;
                    Deliveryjob dj = db.Deliveryjob.Where(x => x.DelID == delivery.DelID).FirstOrDefault();
                    if (dj != null)
                    {
                        dj.JobStatus = delivery.DeliveryStatus;
                        db.Entry(dj).State = EntityState.Modified;
                        if (dj.JobStatus.ToLower() == "delivered")
                        {
                            JobQueue jq = db.JobQueue.Where(x => x.JobID == dj.JobID).FirstOrDefault();
                            {
                                if (jq != null)
                                {
                                    db.JobQueue.Attach(jq);
                                    db.JobQueue.Remove(jq);
                                }
                            }
                        }
                    }
                    db.SaveChanges();
                    return RedirectToAction("DeliveryJobs", "Admin");
                }
                catch (Exception e)
                {
                    Danger("Oops!! Something went wrong.. Please contact support. <br> Error: " + e.Message + "<br> StackTrace: " + e.StackTrace);
                    return View();
                }
            }
            return View(delivery);
        }
    }
}