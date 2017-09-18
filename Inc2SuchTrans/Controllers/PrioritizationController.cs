using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inc2SuchTrans.BLL;
using Inc2SuchTrans.Models;
using Inc2SuchTrans.CustomFilters;
using System.Data.Entity;

namespace Inc2SuchTrans.Controllers
{
    public class PrioritizationController : BaseController
    {
        DeliveryJobLogic djlogic = new DeliveryJobLogic();
        STLogisticsEntities db = new STLogisticsEntities();
        JobQueueLogic jqlogic = new JobQueueLogic();

        // GET: Prioritization
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AuthLog(Roles = "Admin")]
        public ActionResult PrioritizeDelivery()
        {
            try
            {
                List<Deliveryjob> result = djlogic.getAllDeliveryJobs();
                return View(result);
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
        /// <returns></returns>
        [AuthLog(Roles = "Admin")]
        public ActionResult UpdatePriority(int id)
        {
            try
            {
                Deliveryjob dj = db.Deliveryjob.Find(id);
                ViewBag.DelID = dj.DelID;
                ViewBag.DriverID = new SelectList(db.TruckDriver, "DriverID", "DriverID", dj.DriverID);
                ViewBag.TruckID = new SelectList(db.Fleet, "TruckId", "TruckNumberPlate", dj.TruckID);
                return View(dj);
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
        [AuthLog(Roles = "Admin")]
        [HttpPost]
        public ActionResult UpdatePriority([Bind(Include = "JobID,DelID,TruckID,DriverID,JobStatus,PortDelay")] Deliveryjob dj)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(dj).State = EntityState.Modified;
                    db.SaveChanges();

                    JobQueue jq = jqlogic.searchItem(dj.JobID);
                    jq.PriorityScore = 3;
                    jqlogic.updateJobQueue(jq);
                    return RedirectToAction("ScheduleJobs", "Admin");
                }
                catch (Exception e)
                {
                    Danger("Oops!! Something went wrong.. Please contact support. <br> Error: " + e.Message + "<br> StackTrace: " + e.StackTrace);
                    return View();
                }
            }
            else
            {
                ViewBag.DelID = new SelectList(db.Delivery, "DelID", "PickUpArea", dj.DelID);
                ViewBag.DriverID = new SelectList(db.TruckDriver, "DriverID", "DriverID", dj.DriverID);
                ViewBag.TruckID = new SelectList(db.Fleet, "TruckId", "TruckNumberPlate", dj.TruckID);
                return View(dj);
            }
        }




        //try
        //{
        //    //Deliveryjob dj = db.Deliveryjob.Find(id);
        //    //dj.JobStatus = JobStatus;
        //    djlogic.updateDetails(dj);

        //    //JobQueue jq = jqlogic.searchItem(id);
        //    //jq.PriorityScore = 3;
        //    //jqlogic.updateJobQueue(jq);
        //    return RedirectToAction("JobQueue", "Admin");
        //}
        //catch (Exception e)
        //{
        //    Danger("Oops!! Something went wrong.. Please contact support. <br> Error: " + e.Message + "<br> StackTrace: " + e.StackTrace);
        //    return View();
        //}



        //Automated prioritization after initiated by admin
        public ActionResult PrioritizeJobs()
        {
            return View();
        }

        //Manually inputted prioritization done by admin (Driver and fleet assigned to delivery job if a breakdown occurs) //Could include third party 

        //Walk in bookings (Need to manage walk in bookings and prioritization)

        //Testing google map autocomplete
        public ActionResult WalkInBooking()
        {
            return View();
        }

        //[HttpGet]
        //public ActionResult WalkInBooking()
        //{
        //    return View();
        //}
    }
}