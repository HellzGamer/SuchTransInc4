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
    public class BreakdownController : BaseController
    {
        BreakdownLogic blogic = new BreakdownLogic();
        DeliveryJobLogic djlogic = new DeliveryJobLogic();
        FleetLogic flogic = new FleetLogic();
        TruckDriverLogic drivlogic = new TruckDriverLogic();
        STLogisticsEntities db = new STLogisticsEntities();
        // GET: Breakdown
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AuthLog(Roles = "Admin")]
        public ActionResult BreakdownList()
        {
            try
            {
                List<Breakdowns> result = blogic.getAllBreakDowns();
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
        public ActionResult SearchTruck()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AuthLog(Roles = "Admin")]
        public ActionResult Details(string TruckNumberPlate)
        {
            if (String.IsNullOrEmpty(TruckNumberPlate))
            {
                try
                {
                    Information("Please enter truck registration number");
                    return View();
                }
                catch (Exception e)
                {
                    Danger("Oops!! Something went wrong.. Please contact support. <br> Error: " + e.Message + "<br> StackTrace: " + e.StackTrace);
                    return View();
                }
            }
            else
            {
                try
                {
                    Fleet fleet = db.Fleet.Where(x => x.TruckNumberPlate == TruckNumberPlate).FirstOrDefault();
                    if (fleet != null)
                    {
                        return View(fleet);
                    }
                    else
                    {
                        Danger("Not a valid registration number.. <br>Please enter a valid registration number or contact support");
                        return View("SearchTruck");
                    }
                }
                catch (Exception e)
                {
                    Danger("Oops!! Something went wrong.. Please contact support. <br> Error: " + e.Message + "<br> StackTrace: " + e.StackTrace);
                    return View();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AuthLog(Roles = "Admin")]
        public ActionResult EnterBreakdownDetails()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AuthLog(Roles = "Admin")]
        [HttpPost]
        public ActionResult EnterBreakdownDetails(/*string TruckNumberPlate,*/ int id, string BreakdownDescription)
        {
            //if (String.IsNullOrEmpty(TruckNumberPlate))
            //{
            //    Danger("Please enter a correct truck number plate");
            //    return View();
            //}
            if (String.IsNullOrEmpty(BreakdownDescription))
            {
                Danger("Please enter a breakdown description");
                return View();
            }

            try
            {
                //Enters the breakdown details which includes: Truck number plate, Breakdown Description.. (Location will be added next increment with the app)
                //int truckId;
                Breakdowns b = new Breakdowns();
                //b.TruckNumberPlate = TruckNumberPlate;
                b.BreakdownDescription = BreakdownDescription;

                blogic.addBreakDown(b);

                //Searches for the truckId which will be used to find the job that this truck is currently doing
                //truckId = flogic.truckId(TruckNumberPlate);

                //Searches for the delivery job that this truck is currently doing
                Deliveryjob dj = new Deliveryjob();
                dj = djlogic.findJobByTruck(id);

                if (dj != null)
                {
                    //Changes this delivery jobs status to "Breakdown" 
                    TruckDriver td = drivlogic.searchDriver(dj.DriverID);

                    if (td != null)
                    {
                        td.Avail = true;
                        drivlogic.updateStatus(td);
                    }


                    dj.TruckID = null;
                    dj.DriverID = null;
                    dj.JobStatus = "Breakdown";
                    djlogic.updateDetails(dj);


                }
                else
                {
                    Fleet truck = db.Fleet.Find(id);
                    if (truck != null)
                    {
                        truck.Availability = false;
                        db.Entry(truck).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    Information("This specific truck currently does not have a delivery job");
                    return RedirectToAction("DeliveryJobs", "Admin");
                }

                //Returns to the list of breakdowns
                Success("Breakdown Details have been added.<br> Please note that the the fleet and driver associated with this delivery have now been removed. <br> Please reassign driver and fleet.");
                return RedirectToAction("DeliveryJobs", "Admin");
            }
            catch (Exception e)
            {
                Danger("Oops!! Something went wrong.. Please contact support. <br> Error: " + e.Message + "<br> StackTrace: " + e.StackTrace);
                return View();
            }
        }
    }

}