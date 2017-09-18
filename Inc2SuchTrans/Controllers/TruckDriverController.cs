using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inc2SuchTrans.Models;
using Inc2SuchTrans.BLL;
using Inc2SuchTrans.CustomFilters;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Data.Entity;

namespace Inc2SuchTrans.Controllers
{
    public class TruckDriverController : BaseController
    {
        TruckDriverLogic logic = new TruckDriverLogic();
        STLogisticsEntities db = new STLogisticsEntities();

        // GET: TruckDriver
        [AuthLog(Roles = "Admin")]
        public ActionResult Index()
        {
            try
            {
                List<TruckDriver> result = logic.returnAllDrivers();
                if (result != null)
                {
                    return View(result);
                }
                else
                {
                    Danger("Could not retrieve any results");
                    return View();
                }
            }
            catch (Exception e)
            {
                Danger("Oops! Something went wrong.. <br> Error: " + e.Message + "<br> Message: " + e.StackTrace);
                return View();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //[HttpPost]
        //public ActionResult Create()
        //{
        //    return View();
        //}


        public ActionResult updateStatus(int? id)
        {
            try
            {
                TruckDriver truckDriver = db.TruckDriver.Find(id);
                return View(truckDriver);
            }
            catch (Exception e)
            {
                Danger("Oops!! Something went wrong.. Please contact support. <br> Error: " + e.Message + "<br> StackTrace: " + e.StackTrace);
                return View();
            }

        }

        [HttpPost]
        public ActionResult updateStatus([Bind(Include = "DriverID,EmpID,Avail,PriorityStatus")] TruckDriver truckDriver)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(truckDriver).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    Danger("Oops!! Something went wrong.. Please contact support. <br> Error: " + e.Message + "<br> StackTrace: " + e.StackTrace);
                    return View();
                }

            }
            else
            {
                return View();
            }
        }
    }
}