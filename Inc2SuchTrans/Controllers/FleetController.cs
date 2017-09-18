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
    public class FleetController : BaseController
    {
        FleetLogic logic = new FleetLogic();
        STLogisticsEntities db = new STLogisticsEntities();

        // GET: Fleet
        [AuthLog(Roles = "Admin")]
        public ActionResult Index()
        {
            try
            {
                List<Fleet> result = logic.returnAllTrucks();
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
        [AuthLog(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AuthLog(Roles = "Admin")]
        [HttpPost]
        public ActionResult Create(string TruckNumberPlate, string Availability, string PriorityStatus)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Fleet f = new Fleet();
                    f.TruckNumberPlate = TruckNumberPlate;
                    f.Availability = Convert.ToBoolean(Availability);
                    f.PriorityStatus = Convert.ToBoolean(PriorityStatus);
                    logic.addTruck(f);
                    return View("Index");
                }
                else
                {
                    Danger("Oops! Something went wrong.. <br> ModelState is invalid");
                    return View();
                }
            }
            catch (Exception e)
            {
                Danger("Oops! Something went wrong.. <br> Error: " + e.Message + "<br>Message: " + e.StackTrace);
                return View();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult updateStatus(int? id)
        {
            try
            {
                Fleet truck = db.Fleet.Find(id);
                return View(truck);
            }
            catch (Exception e)
            {
                Danger("Oops!! Something went wrong.. Please contact support. <br> Error: " + e.Message + "<br> StackTrace: " + e.StackTrace);
                return View();
            }

        }

        [HttpPost]
        public ActionResult updateStatus([Bind(Include = "TruckID,TruckNumberPlate,Availability,PriorityStatus")] Fleet truck)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(truck).State = EntityState.Modified;
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