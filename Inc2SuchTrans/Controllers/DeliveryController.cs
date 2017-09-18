using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inc2SuchTrans.BLL;
using Inc2SuchTrans.Models;
using System.Globalization;
using Microsoft.AspNet.Identity;
using System.Text;
using Inc2SuchTrans.CustomFilters;
using System.Net.Mail;
using System.IO;
using HiQPdf;
using System.Data.Entity;

namespace Inc2SuchTrans.Controllers
{
    public class DeliveryController : BaseController
    {
        DeliveryLogic logic = new DeliveryLogic();
        QuoteLogic qlogic = new QuoteLogic();
        CustomerLogic custLogic = new CustomerLogic();
        CargoLogic clogic = new CargoLogic();
        AreaRateLogic arlogic = new AreaRateLogic();
        DeliveryJobLogic djlogic = new DeliveryJobLogic();
        ArchiveLogic arclogic = new ArchiveLogic();
        TruckDriverLogic tdlogic = new TruckDriverLogic();
        FleetLogic flogic = new FleetLogic();

        //byte[] pdfBuffer;

        STLogisticsEntities db = new STLogisticsEntities();

        // GET: Delivery
        public ActionResult Index()
        {
            try
            {
                List<Delivery> del = logic.returnAllDeliveries();
                return View(del);
            }
            catch (Exception e)
            {
                Danger("Unfortunately something went wrong.. <br> Could not retrieve list, Please contact support");
                return View();
                throw e;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthLog(Roles = "Admin")]
        public ActionResult UpdateDeliveryJobStatus(int id)
        {
            try
            {
                Deliveryjob dj = db.Deliveryjob.Find(id);
                return View(dj);
            }
            catch (Exception e)
            {
                Danger("Unfortunately something went wrong.. <br>Please contact support <br>Error: " + e.Message + "<br> StackTrace: " + e.StackTrace);
                return View();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthLog(Roles = "Admin")]
        public ActionResult UpdateDeliveryJobStatus([Bind(Include = "JobID,DelID,TruckID,DriverID,JobStatus,PortDelay")] Deliveryjob dj)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(dj).State = EntityState.Modified;
                    //db.SaveChanges();

                    Delivery del = db.Delivery.Where(x => x.DelID == dj.DelID).FirstOrDefault();
                    del.DeliveryStatus = dj.JobStatus;
                    db.Entry(del).State = EntityState.Modified;

                    if (dj.JobStatus.ToLower() == "delivered")
                    {
                        JobQueue jq = db.JobQueue.Where(x => x.JobID == dj.JobID).FirstOrDefault();
                        if (jq != null)
                        {
                            db.JobQueue.Attach(jq);
                            db.JobQueue.Remove(jq);

                            TruckDriver td = db.TruckDriver.Where(x => x.DriverID == dj.DriverID).FirstOrDefault();
                            Fleet truck = db.Fleet.Where(x => x.TruckId == dj.TruckID).FirstOrDefault();

                            td.Avail = true;
                            truck.Availability = true;

                            db.Entry(td).State = EntityState.Modified;
                            db.Entry(truck).State = EntityState.Modified;
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
            else
            {
                ViewBag.DelID = new SelectList(db.Delivery, "DelID", "PickUpArea", dj.DelID);
                ViewBag.DriverID = new SelectList(db.TruckDriver, "DriverID", "DriverID", dj.DriverID);
                ViewBag.TruckID = new SelectList(db.Fleet, "TruckId", "TruckNumberPlate", dj.TruckID);
                return View(dj);
            }
        }

        //TODO : Selected - RED and CHANGE TO label will have drop down next to it.
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AuthLog(Roles = "Customer")]
        [HttpGet]
        public ActionResult CreateFromEmail()
        {
            IEnumerable<SelectListItem> dropOff = db.AreaRates.Select(c => new SelectListItem
            {
                Value = c.Area,
                Text = c.Area
            });

            IEnumerable<SelectListItem> pickUp = db.Cargo.Select(c => new SelectListItem
            {
                Value = c.PickUpArea,
                Text = c.PickUpArea
            });
            ViewBag.DropOffArea = dropOff;
            ViewBag.PickUpArea = pickUp;
            return View();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="DeliveryDate"></param>
        /// <param name="PickUpArea"></param>
        /// <param name="PickUpAdd"></param>
        /// <param name="DropOffArea"></param>
        /// <param name="DropOffAdd"></param>
        /// <param name="CargoSize"></param>
        /// <param name="CargoWeight"></param>
        /// <returns></returns>
        [AuthLog(Roles = "Customer")]
        [HttpPost]
        public ActionResult CreateFromEmail(string userId, string DeliveryDate, string PickUpArea, string DropOffArea, string DropOffAdd, string CargoSize, string CargoWeight)
        {
            if (String.IsNullOrEmpty(DeliveryDate))
            {
                Danger("Please Enter A Delivery Date");
                IEnumerable<SelectListItem> dropOff = db.AreaRates.Select(c => new SelectListItem
                {
                    Value = c.Area,
                    Text = c.Area
                });

                IEnumerable<SelectListItem> pickUp = db.Cargo.Select(c => new SelectListItem
                {
                    Value = c.PickUpArea,
                    Text = c.PickUpArea
                });
                ViewBag.DropOffArea = dropOff;
                ViewBag.PickUpArea = pickUp;
                return View();
            }
            if (String.IsNullOrEmpty(PickUpArea))
            {
                Danger("Please Enter A Delivery Date");
                IEnumerable<SelectListItem> dropOff = db.AreaRates.Select(c => new SelectListItem
                {
                    Value = c.Area,
                    Text = c.Area
                });

                IEnumerable<SelectListItem> pickUp = db.Cargo.Select(c => new SelectListItem
                {
                    Value = c.PickUpArea,
                    Text = c.PickUpArea
                });
                ViewBag.DropOffArea = dropOff;
                ViewBag.PickUpArea = pickUp;
                return View();
            }
            if (String.IsNullOrEmpty(DropOffArea))
            {
                Danger("Please Enter A Delivery Date");
                IEnumerable<SelectListItem> dropOff = db.AreaRates.Select(c => new SelectListItem
                {
                    Value = c.Area,
                    Text = c.Area
                });

                IEnumerable<SelectListItem> pickUp = db.Cargo.Select(c => new SelectListItem
                {
                    Value = c.PickUpArea,
                    Text = c.PickUpArea
                });
                ViewBag.DropOffArea = dropOff;
                ViewBag.PickUpArea = pickUp;
                return View();
            }
            if (String.IsNullOrEmpty(DropOffAdd))
            {
                Danger("Please Enter A Delivery Date");
                IEnumerable<SelectListItem> dropOff = db.AreaRates.Select(c => new SelectListItem
                {
                    Value = c.Area,
                    Text = c.Area
                });

                IEnumerable<SelectListItem> pickUp = db.Cargo.Select(c => new SelectListItem
                {
                    Value = c.PickUpArea,
                    Text = c.PickUpArea
                });
                ViewBag.DropOffArea = dropOff;
                ViewBag.PickUpArea = pickUp;
                return View();
            }
            if (String.IsNullOrEmpty(CargoSize))
            {
                Danger("Please Enter A Delivery Date");
                IEnumerable<SelectListItem> dropOff = db.AreaRates.Select(c => new SelectListItem
                {
                    Value = c.Area,
                    Text = c.Area
                });

                IEnumerable<SelectListItem> pickUp = db.Cargo.Select(c => new SelectListItem
                {
                    Value = c.PickUpArea,
                    Text = c.PickUpArea
                });
                ViewBag.DropOffArea = dropOff;
                ViewBag.PickUpArea = pickUp;
                return View();
            }
            if (String.IsNullOrEmpty(CargoWeight))
            {
                Danger("Please Enter A Delivery Date");
                IEnumerable<SelectListItem> dropOff = db.AreaRates.Select(c => new SelectListItem
                {
                    Value = c.Area,
                    Text = c.Area
                });

                IEnumerable<SelectListItem> pickUp = db.Cargo.Select(c => new SelectListItem
                {
                    Value = c.PickUpArea,
                    Text = c.PickUpArea
                });
                ViewBag.DropOffArea = dropOff;
                ViewBag.PickUpArea = pickUp;
                return View();
            }

            DateTime DDate = DateTime.Parse(DeliveryDate);

            if (ModelState.IsValid)
            {
                try
                {
                    decimal weight;
                    int size;
                    double temp = Double.Parse(CargoWeight, CultureInfo.InvariantCulture);
                    temp = Math.Round(temp, 2);
                    decimal.TryParse(temp.ToString("0.00"), out weight);
                    int.TryParse(CargoSize, out size);

                    string dateref = DDate.ToString().Substring(16, 2);

                    Delivery del = new Delivery();
                    del.CustomerID = custLogic.getCurrentUserId(User.Identity.GetUserId()); ;
                    del.CurrentDate = System.DateTime.Now;
                    del.DeliveryDate = Convert.ToDateTime(DeliveryDate);
                    del.PickUpArea = PickUpArea;
                    del.DropOffArea = DropOffArea;
                    del.DropOffAdd = DropOffAdd;
                    del.CargoSize = size;
                    del.CargoWeight = weight;
                    del.TotalCost = logic.determineTotalCost(PickUpArea, DropOffArea, size, weight);
                    del.Paid = false;
                    del.DeliveryRef = (dateref + del.CustomerID + del.PickUpArea.Substring(1, 1) + dateref + del.DropOffArea.Substring(3, 1) + "st").Replace(" ", "");
                    del.DeliveryStatus = "Waiting";

                    logic.addDelivery(del);
                    Deliveryjob dj = new Deliveryjob();
                    dj.DelID = del.DelID;
                    dj.JobStatus = "Waiting";
                    dj.TruckID = null;
                    dj.DriverID = null;
                    dj.PortDelay = false;
                    djlogic.addDeliveryJob(dj);

                    return RedirectToAction("YourDeliveries", new { id = del.CustomerID });
                }
                catch (Exception e)
                {
                    Danger("Error: " + e.Message);
                    IEnumerable<SelectListItem> dropOff = db.AreaRates.Select(c => new SelectListItem
                    {
                        Value = c.Area,
                        Text = c.Area
                    });

                    IEnumerable<SelectListItem> pickUp = db.Cargo.Select(c => new SelectListItem
                    {
                        Value = c.PickUpArea,
                        Text = c.PickUpArea
                    });
                    ViewBag.DropOffArea = dropOff;
                    ViewBag.PickUpArea = pickUp;
                    return View();
                }
            }
            else
            {
                Danger("Oops! Something went wrong.. <br> Please contact support..");
                IEnumerable<SelectListItem> dropOff = db.AreaRates.Select(c => new SelectListItem
                {
                    Value = c.Area,
                    Text = c.Area
                });

                IEnumerable<SelectListItem> pickUp = db.Cargo.Select(c => new SelectListItem
                {
                    Value = c.PickUpArea,
                    Text = c.PickUpArea
                });
                ViewBag.DropOffArea = dropOff;
                ViewBag.PickUpArea = pickUp;
                return View();
            }

        }


        //TODO : Selected - RED and CHANGE TO label will have drop down next to it.
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AuthLog(Roles = "Customer")]
        [HttpGet]
        public ActionResult CreateFromSite()
        {
            IEnumerable<SelectListItem> dropOff = db.AreaRates.Select(c => new SelectListItem
            {
                Value = c.Area,
                Text = c.Area
            });

            IEnumerable<SelectListItem> pickUp = db.Cargo.Select(c => new SelectListItem
            {
                Value = c.PickUpArea,
                Text = c.PickUpArea
            });
            ViewBag.DropOffArea = dropOff;
            ViewBag.PickUpArea = pickUp;
            return View();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="DeliveryDate"></param>
        /// <param name="PickUpArea"></param>
        /// <param name="PickUpAdd"></param>
        /// <param name="DropOffArea"></param>
        /// <param name="DropOffAdd"></param>
        /// <param name="CargoSize"></param>
        /// <param name="CargoWeight"></param>
        /// <returns></returns>
        [AuthLog(Roles = "Customer")]
        [HttpPost]
        public ActionResult CreateFromSite(string userId, string DeliveryDate, string PickUpArea, string DropOffArea, string DropOffAdd, string CargoSize, string CargoWeight)
        {
            if (String.IsNullOrEmpty(DeliveryDate))
            {
                Danger("Please Enter A Delivery Date");
                IEnumerable<SelectListItem> dropOff = db.AreaRates.Select(c => new SelectListItem
                {
                    Value = c.Area,
                    Text = c.Area
                });

                IEnumerable<SelectListItem> pickUp = db.Cargo.Select(c => new SelectListItem
                {
                    Value = c.PickUpArea,
                    Text = c.PickUpArea
                });
                ViewBag.DropOffArea = dropOff;
                ViewBag.PickUpArea = pickUp;
                return View();
            }
            if (String.IsNullOrEmpty(PickUpArea))
            {
                Danger("Please Enter A Delivery Date");
                IEnumerable<SelectListItem> dropOff = db.AreaRates.Select(c => new SelectListItem
                {
                    Value = c.Area,
                    Text = c.Area
                });

                IEnumerable<SelectListItem> pickUp = db.Cargo.Select(c => new SelectListItem
                {
                    Value = c.PickUpArea,
                    Text = c.PickUpArea
                });
                ViewBag.DropOffArea = dropOff;
                ViewBag.PickUpArea = pickUp;
                return View();
            }
            if (String.IsNullOrEmpty(DropOffArea))
            {
                Danger("Please Enter A Delivery Date");
                IEnumerable<SelectListItem> dropOff = db.AreaRates.Select(c => new SelectListItem
                {
                    Value = c.Area,
                    Text = c.Area
                });

                IEnumerable<SelectListItem> pickUp = db.Cargo.Select(c => new SelectListItem
                {
                    Value = c.PickUpArea,
                    Text = c.PickUpArea
                });
                ViewBag.DropOffArea = dropOff;
                ViewBag.PickUpArea = pickUp;
                return View();
            }
            if (String.IsNullOrEmpty(DropOffAdd))
            {
                Danger("Please Enter A Delivery Date");
                IEnumerable<SelectListItem> dropOff = db.AreaRates.Select(c => new SelectListItem
                {
                    Value = c.Area,
                    Text = c.Area
                });

                IEnumerable<SelectListItem> pickUp = db.Cargo.Select(c => new SelectListItem
                {
                    Value = c.PickUpArea,
                    Text = c.PickUpArea
                });
                ViewBag.DropOffArea = dropOff;
                ViewBag.PickUpArea = pickUp;
                return View();
            }
            if (String.IsNullOrEmpty(CargoSize))
            {
                Danger("Please Enter A Delivery Date");
                IEnumerable<SelectListItem> dropOff = db.AreaRates.Select(c => new SelectListItem
                {
                    Value = c.Area,
                    Text = c.Area
                });

                IEnumerable<SelectListItem> pickUp = db.Cargo.Select(c => new SelectListItem
                {
                    Value = c.PickUpArea,
                    Text = c.PickUpArea
                });
                ViewBag.DropOffArea = dropOff;
                ViewBag.PickUpArea = pickUp;
                return View();
            }
            if (String.IsNullOrEmpty(CargoWeight))
            {
                Danger("Please Enter A Delivery Date");
                IEnumerable<SelectListItem> dropOff = db.AreaRates.Select(c => new SelectListItem
                {
                    Value = c.Area,
                    Text = c.Area
                });

                IEnumerable<SelectListItem> pickUp = db.Cargo.Select(c => new SelectListItem
                {
                    Value = c.PickUpArea,
                    Text = c.PickUpArea
                });
                ViewBag.DropOffArea = dropOff;
                ViewBag.PickUpArea = pickUp;
                return View();
            }

            DateTime DDate = DateTime.Parse(DeliveryDate);

            if (ModelState.IsValid)
            {
                try
                {
                    decimal weight;
                    int size;
                    double temp = Double.Parse(CargoWeight, CultureInfo.InvariantCulture);
                    temp = Math.Round(temp, 2);
                    decimal.TryParse(temp.ToString("0.00"), out weight);
                    int.TryParse(CargoSize, out size);
                    string dateref = System.DateTime.Now.ToString().Substring(17, 2);

                    Delivery del = new Delivery();
                    del.CustomerID = custLogic.getCurrentUserId(User.Identity.GetUserId()); ;
                    del.CurrentDate = System.DateTime.Now;
                    del.DeliveryDate = Convert.ToDateTime(DeliveryDate);
                    del.PickUpArea = PickUpArea;
                    del.DropOffArea = DropOffArea;
                    del.DropOffAdd = DropOffAdd;
                    del.CargoSize = size;
                    del.CargoWeight = weight;
                    del.TotalCost = logic.determineTotalCost(PickUpArea, DropOffArea, size, weight);
                    del.DeliveryRef = (dateref + del.CustomerID + del.PickUpArea.Substring(1, 1) + dateref + del.DropOffArea.Substring(3, 1) + "st").Replace(" ", "");
                    del.Paid = false;
                    del.DeliveryStatus = "Waiting";

                    logic.addDelivery(del);

                    Deliveryjob dj = new Deliveryjob();
                    dj.DelID = del.DelID;
                    dj.JobStatus = "Waiting";
                    dj.TruckID = null;
                    dj.DriverID = null;
                    dj.PortDelay = false;
                    djlogic.addDeliveryJob(dj);

                    return RedirectToAction("YourDeliveries", new { id = del.CustomerID });
                }
                catch (Exception e)
                {
                    Danger("Error: " + e.Message);
                    IEnumerable<SelectListItem> dropOff = db.AreaRates.Select(c => new SelectListItem
                    {
                        Value = c.Area,
                        Text = c.Area
                    });

                    IEnumerable<SelectListItem> pickUp = db.Cargo.Select(c => new SelectListItem
                    {
                        Value = c.PickUpArea,
                        Text = c.PickUpArea
                    });
                    ViewBag.DropOffArea = dropOff;
                    ViewBag.PickUpArea = pickUp;
                    return View();
                }
            }
            else
            {
                Danger("Oops! Something went wrong.. <br> Please contact support..");
                IEnumerable<SelectListItem> dropOff = db.AreaRates.Select(c => new SelectListItem
                {
                    Value = c.Area,
                    Text = c.Area
                });

                IEnumerable<SelectListItem> pickUp = db.Cargo.Select(c => new SelectListItem
                {
                    Value = c.PickUpArea,
                    Text = c.PickUpArea
                });
                ViewBag.DropOffArea = dropOff;
                ViewBag.PickUpArea = pickUp;
                return View();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthLog(Roles = "Customer")]
        public ActionResult YourDeliveries(string refkey)
        {
            if (!String.IsNullOrEmpty(refkey))
            {
                try
                {
                    List<Delivery> result = db.Delivery.ToList().Where(x => x.DeliveryRef == refkey).ToList();
                    return View(result);
                }
                catch (Exception e)
                {
                    Danger("Oops! Something went wrong.. <br> Please contact support");
                    List<Delivery> yourDeliveries = logic.searchCustomerDeliveries(User.Identity.GetUserId());
                    return View(yourDeliveries);
                    throw e;
                }
            }

            else
            {
                try
                {
                    List<Delivery> yourDeliveries = logic.searchCustomerDeliveries(User.Identity.GetUserId());
                    return View(yourDeliveries);
                }
                catch (Exception e)
                {
                    Danger("Oops! Something went wrong.. <br> Please contact support..");
                    return View();
                    throw e;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AuthLog(Roles = "Customer")]
        public ActionResult SearchForDelivery()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="refkey"></param>
        /// <returns></returns>
        [AuthLog(Roles = "Customer")]
        public ActionResult DeliveryStatus(string refkey)
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
        [AuthLog(Roles = "Customer")]
        public ActionResult TrackDelivery(int id)
        {
            Delivery d = logic.searchDelivery(id);
            ViewBag.DelID = Convert.ToString(d.DelID);
            ViewBag.PickUp = d.PickUpArea;
            ViewBag.DropOff = d.DropOffAdd;
            ViewBag.CustName = d.Customer.CustomerName;
            ViewBag.CustSurname = d.Customer.CustomerSurname;
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthLog(Roles = "Customer")]
        public ActionResult Details(int id)
        {
            try
            {
                Delivery del = logic.searchDelivery(id);
                return View(del);
            }
            catch (Exception e)
            {
                Danger("Oops! Something went wrong.. <br> Please contact support");
                return RedirectToAction("YourDeliveries");

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[AuthLog(Roles = "Customer")]
        public ActionResult PaperworkDetails(int? id)
        {
            try
            {
                ErrorUtil.InsertError("PaperworkDetails", "So far so good,About to search Delivery logic");
                Delivery del = logic.searchDelivery(id);

                string url = "Delivery/PaperworkDetails/" + id.ToString();
                // get the HTML code of this view
                string htmlToConvert = RenderViewAsString("PaperworkDetails", del);

                // the base URL to resolve relative images and css
                String thisPageUrl = this.ControllerContext.HttpContext.Request.Url.AbsoluteUri;
                String baseUrl = thisPageUrl.Substring(0, thisPageUrl.Length - url.Length);

                // instantiate the HiQPdf HTML to PDF converter
                HtmlToPdf htmlToPdfConverter = new HtmlToPdf();

                // hide the button in the created PDF
                htmlToPdfConverter.HiddenHtmlElements = new string[] { "#convertThisPageButtonDiv" };

                // render the HTML code as PDF in memory
                byte[] pdfBuffer = htmlToPdfConverter.ConvertHtmlToMemory(htmlToConvert, baseUrl);


                // send the PDF file to browser
                FileResult fileResult = new FileContentResult(pdfBuffer, "application/pdf");
                fileResult.FileDownloadName = "Delivery Paperwork.pdf";

                return fileResult;
                //return View(del);
            }
            catch (Exception e)
            {
                Danger("Oops! Something went wrong.. <br> Please contact support with error: " + e.Message + " " + e.StackTrace);
                return RedirectToAction("YourDeliveries");

            }
        }

        public string RenderViewAsString(string viewName, object model)
        {
            // create a string writer to receive the HTML code
            StringWriter stringWriter = new StringWriter();

            // get the view to render
            ViewEngineResult viewResult = ViewEngines.Engines.FindView(ControllerContext, viewName, null);
            // create a context to render a view based on a model
            ViewContext viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    new ViewDataDictionary(model),
                    new TempDataDictionary(),
                    stringWriter
                    );

            // render the view to a HTML code
            viewResult.View.Render(viewContext, stringWriter);

            // return the HTML code
            return stringWriter.ToString();
        }

        /// <summary>
        /// Delivery paperwork in pdf form
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[AuthLog(Roles = "Customer")]
        public ActionResult DeliveryPDF(int? id)
        {
            try
            {
                Delivery del = logic.searchDelivery(id);
                return View(del);
            }
            catch (Exception e)
            {
                ErrorUtil.InsertError(e.Message, e.StackTrace);
                Danger("Oops! Something went wrong.. <br> Please contact support with error: " + e.Message + " " + e.StackTrace);

                return RedirectToAction("Details", new { id = id });

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthLog(Roles = "Customer")]
        public ActionResult EmailPaperwork(int? id)
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="_objModelMail"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EmailPaperwork(int? id, MailModel _objModelMail)
        {
            try
            {
                Delivery del = logic.searchDelivery(id);

                string url = "Delivery/DeliveryPDF/" + id.ToString();
                // get the HTML code of this view
                string htmlToConvert = RenderViewAsString("DeliveryPDF", del);

                // the base URL to resolve relative images and css
                String thisPageUrl = this.ControllerContext.HttpContext.Request.Url.AbsoluteUri;
                String baseUrl = thisPageUrl.Substring(0, thisPageUrl.Length - url.Length);

                // instantiate the HiQPdf HTML to PDF converter
                HtmlToPdf htmlToPdfConverter = new HtmlToPdf();

                // hide the button in the created PDF
                htmlToPdfConverter.HiddenHtmlElements = new string[] { "#convertThisPageButtonDiv" };

                // render the HTML code as PDF in memory
                byte[] pdfBuffer = htmlToPdfConverter.ConvertHtmlToMemory(htmlToConvert, baseUrl);


                if (ModelState.IsValid)
                {
                    Attachment att = new Attachment(new MemoryStream(pdfBuffer), "Delivery Paperwork.pdf");
                    MailMessage mail = new MailMessage();
                    mail.To.Add(_objModelMail.To);
                    mail.From = new MailAddress("suchtranslogistics@gmail.com");
                    mail.Subject = "Your Delivery Paperwork";
                    string Body = "Please find attached your requested delivery paperwork";
                    mail.Body = Body;
                    mail.Attachments.Add(att);
                    mail.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new System.Net.NetworkCredential
                    ("suchtranslogistics@gmail.com", "Suchtrans@123");// Enter seders User name and password  
                    smtp.EnableSsl = true;
                    smtp.Send(mail);

                    Success("Your delivery paperwork has been emailed successfully! <br> Please make sure you have received it.");
                    return View();
                }
                else
                {
                    Danger("There was an error processing your request.. <br> Please contact support");
                    return View();
                }
            }
            catch (Exception e)
            {
                Danger("Oops! Something went wrong.. <br> Please contact support.");
                return View();
                throw e;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AuthLog(Roles = "Customer")]
        public ActionResult UnpaidDeliveries(string refkey)
        {
            if (!String.IsNullOrEmpty(refkey))
            {
                try
                {
                    List<Delivery> result = logic.returnUnpaidDeliveries(User.Identity.GetUserId()).Where(x => x.DeliveryRef == refkey).ToList();
                    return View(result);
                }
                catch (Exception e)
                {
                    Danger("Oops! Something went wrong.. <br> Please contact support");
                    List<Delivery> result = logic.returnUnpaidDeliveries(User.Identity.GetUserId());
                    return View(result);
                    throw e;
                }
            }
            else
            {
                try
                {
                    List<Delivery> result = logic.returnUnpaidDeliveries(User.Identity.GetUserId());
                    if (result != null)
                    {
                        return View(result);
                    }
                    else
                    {
                        Success("You Have No Outstanding Payments For Any Deliveries!");
                        return View();
                    }
                }
                catch (Exception e)
                {
                    Danger("Oops! Something went wrong.. <br> Please contact support..");
                    return View("YourDeliveries");
                    throw e;
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthLog(Roles = "Customer")]
        public ActionResult ConfirmOrder(int id)
        {
            try
            {
                Delivery delToPay = logic.searchDelivery(id);
                // Create the order in your DB and get the ID
                string amount = delToPay.TotalCost.ToString();
                string orderId = delToPay.DelID.ToString();
                string name = "SuchTrans Logistics, Order #" + orderId;
                string description = "Delivery Payment";

                string site = "";
                string merchant_id = "";
                string merchant_key = "";

                site = "https://sandbox.payfast.co.za/eng/process?";
                merchant_id = "10000100";
                merchant_key = "46f0cd694581a";

                // Build the query string for payment site



                StringBuilder str = new StringBuilder();
                str.Append("merchant_id=" + HttpUtility.UrlEncode(merchant_id));
                str.Append("&merchant_key=" + HttpUtility.UrlEncode(merchant_key));
                str.Append("&return_url=" + HttpUtility.UrlEncode(System.Configuration.ConfigurationManager.AppSettings["PF_ReturnURL"]) + "?id=" + id);
                str.Append("&cancel_url=" + HttpUtility.UrlEncode(System.Configuration.ConfigurationManager.AppSettings["PF_CancelURL"]) + "?id=" + id);
                str.Append("&m_payment_id=" + HttpUtility.UrlEncode(orderId));
                str.Append("&amount=" + HttpUtility.UrlEncode(amount));
                str.Append("&item_name=" + HttpUtility.UrlEncode(name));
                str.Append("&item_description=" + HttpUtility.UrlEncode(description));

                // Redirect to PayFast
                return Redirect(site + str.ToString());
            }
            catch (Exception e)
            {
                Danger("Oops! Something went wrong.. <br> Please contact support..");
                return View();
                throw e;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthLog(Roles = "Customer")]
        public ActionResult SuccessPF(int? id)
        {
            Delivery del = logic.searchDelivery(id);
            del.Paid = true;
            logic.updateDelivery(del);
            Success("Payment Was Successful!");
            return View(del);
        }

        [AuthLog(Roles = "Customer")]
        public ActionResult CancelPF(int? id)
        {
            Danger("Payment For Delivery Was Cancelled By User");
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthLog(Roles = "Customer")]
        public ActionResult CancelDelivery(int id)
        {
            try
            {
                Delivery delivery = db.Delivery.Find(id);
                return View(delivery);
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
        /// <param name="ReasonForArchive"></param>
        /// <returns></returns>
        [AuthLog(Roles = "Customer")]
        [HttpPost, ActionName("CancelDelivery")]
        public ActionResult CancelDeliveryConfirmed(int id, string ReasonForArchive)
        {
            Delivery delivery = db.Delivery.Find(id);
            Deliveryjob deliveryJob = db.Deliveryjob.Where(x => x.DelID == id).FirstOrDefault();
            JobQueue jobQueue = db.JobQueue.Where(x => x.JobID == deliveryJob.JobID).FirstOrDefault();
            TruckDriver td = db.TruckDriver.Where(x => x.DriverID == deliveryJob.DriverID).FirstOrDefault();
            Fleet truck = db.Fleet.Where(x => x.TruckId == deliveryJob.TruckID).FirstOrDefault();
            try
            {

                ArchivedDeliveries arcDel = new ArchivedDeliveries();

                DateTime currentDate = DateTime.Now;
                DateTime DeliveryDate = delivery.DeliveryDate;

                double days = (DeliveryDate - currentDate).TotalDays;

                if (days > 5)
                {
                    Session["DeliveryID"] = delivery.DelID;
                    Session["CurrentDate"] = delivery.CurrentDate;
                    Session["DeliveryDate"] = delivery.DeliveryDate;
                    Session["PickUpArea"] = delivery.PickUpArea;
                    Session["DropOffArea"] = delivery.DropOffArea;
                    Session["DropOffAdd"] = delivery.DropOffAdd;
                    Session["CargoSize"] = delivery.CargoSize;
                    Session["CargoWeight"] = delivery.CargoWeight;
                    Session["TotalCost"] = delivery.TotalCost;
                    Session["Paid"] = delivery.Paid;
                    Session["DeliveryRef"] = delivery.DeliveryRef;
                    Session["DeliveryStatus"] = delivery.DeliveryStatus;
                    Session["ReasonForArchive"] = "Cancelled Delivery";
                    Session["CustomerID"] = delivery.CustomerID;

                    //arclogic.ArchiveDelivery(arcDel);
                    if (jobQueue != null)
                    {
                        db.JobQueue.Attach(jobQueue);
                        db.JobQueue.Remove(jobQueue);
                    }

                    if (deliveryJob != null)
                    {
                        db.Deliveryjob.Attach(deliveryJob);
                        db.Deliveryjob.Remove(deliveryJob);
                    }

                    //db.SaveChanges();
                    if (delivery != null)
                    {
                        db.Delivery.Attach(delivery);
                        db.Delivery.Remove(delivery);
                    }

                    db.SaveChanges();

                    if (td != null && truck != null)
                    {
                        td.Avail = true;
                        truck.Availability = true;
                        db.Entry(truck).State = EntityState.Modified;
                        //db.SaveChanges();
                        db.Entry(td).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                        if (td != null && truck == null)
                    {
                        td.Avail = true;
                        db.Entry(td).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                        if (truck != null && td == null)
                    {
                        truck.Availability = true;
                        db.Entry(truck).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    //db.Delivery.Remove(delivery);
                    //db.SaveChanges();

                    arcDel.DeliveryID = Convert.ToInt32(Session["DeliveryID"]);
                    arcDel.CurrentDate = Convert.ToDateTime(Session["CurrentDate"]);
                    arcDel.DeliveryDate = Convert.ToDateTime(Session["DeliveryDate"]);
                    arcDel.PickUpArea = Convert.ToString(Session["PickUpArea"]);
                    arcDel.DropOffArea = Convert.ToString(Session["DropOffArea"]);
                    arcDel.DropOffAddress = Convert.ToString(Session["DropOffAdd"]);
                    arcDel.CargoSize = Convert.ToInt32(Session["CargoSize"]);
                    arcDel.CargoWeight = Convert.ToDecimal(Session["CargoWeight"]);
                    arcDel.TotalCost = Convert.ToDecimal(Session["TotalCost"]);
                    arcDel.Paid = Convert.ToBoolean(Session["Paid"]);
                    arcDel.DeliveryRef = Convert.ToString(Session["DeliveryRef"]);
                    arcDel.DeliveryStatus = Convert.ToString(Session["DeliveryStatus"]);
                    arcDel.ReasonForArchive = Convert.ToString(Session["ReasonForArchive"]);
                    arcDel.CustomerID = Convert.ToInt32(Session["CustomerID"]);
                    arclogic.ArchiveDelivery(arcDel);

                    Success("You have successfully cancelled your delivery!");
                    return RedirectToAction("YourDeliveries", "Delivery");
                }
                else
                if (days < 5 && delivery.DeliveryStatus != "Delivered")
                {
                    Danger("Cannot cancel delivery within 5 days of The delivery date.. <br> For more information, please contact support.");
                    return RedirectToAction("CancelDelivery", new { id = delivery.DelID });
                }
                else
                if (days < 0 && delivery.DeliveryStatus == "Delivered")
                {
                    Danger("Delivery has already been completed.. <br> Please check status of delivery or for more information, please contact support.");
                    return RedirectToAction("CancelDelivery", new { id = delivery.DelID });
                }
                else
                {
                    return View();
                }
            }
            catch (Exception e)
            {
                Danger("Oops!! Something went wrong.. Please contact support. <br> Error: " + e.Message + "<br> StackTrace: " + e.StackTrace);
                return RedirectToAction("CancelDelivery", new { id = delivery.DelID });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AuthLog(Roles = "Customer")]
        public ActionResult ChangeDestination(int id)
        {
            try
            {
                Delivery d = logic.searchDelivery(id);
                if (d != null)
                {
                    IEnumerable<SelectListItem> dropOff = db.AreaRates.Select(c => new SelectListItem
                    {
                        Value = c.Area,
                        Text = c.Area
                    });
                    ViewBag.DropOffArea = dropOff;
                    return View(d);
                }
                else
                {
                    IEnumerable<SelectListItem> dropOff = db.AreaRates.Select(c => new SelectListItem
                    {
                        Value = c.Area,
                        Text = c.Area
                    });
                    ViewBag.DropOffArea = dropOff;
                    Danger("Delivery does not exist.. Please contact support");
                    return View();
                }
            }
            catch (Exception e)
            {
                IEnumerable<SelectListItem> dropOff = db.AreaRates.Select(c => new SelectListItem
                {
                    Value = c.Area,
                    Text = c.Area
                });
                ViewBag.DropOffArea = dropOff;
                Danger("Oops!! Something went wrong.. Please contact support. <br> Error: " + e.Message + "<br> StackTrace: " + e.StackTrace);
                return View();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="delivery"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthLog(Roles = "Customer")]
        public ActionResult ChangeDestination([Bind(Include = "DelID,CustomerID,CurrentDate,DeliveryDate,PickUpArea,DropOffArea,DropOffAdd,CargoSize,CargoWeight,TotalCost,Paid,DeliveryRef,DeliveryStatus")] Delivery delivery)
        {
            DateTime currentDate = DateTime.Now;
            DateTime DeliveryDate = delivery.DeliveryDate;

            double days = (DeliveryDate - currentDate).TotalDays;

            if (ModelState.IsValid)
            {
                try
                {
                    if (days > 5)
                    {
                        db.Entry(delivery).State = EntityState.Modified;
                        db.SaveChanges();
                        Success("Successfully changed the delivery destination!");
                        return RedirectToAction("YourDeliveries");
                    }
                    else
                    {
                        Danger("Cannot change destination of a delivery thats under 5 days away!");
                        return RedirectToAction("ChangeDestination", new { id = delivery.DelID });
                    }
                }
                catch (Exception e)
                {
                    Danger("Oops!! Something went wrong.. Please contact support. <br> Error: " + e.Message + "<br> StackTrace: " + e.StackTrace);
                    return RedirectToAction("Index");
                }
            }

            IEnumerable<SelectListItem> dropOff = db.AreaRates.Select(c => new SelectListItem
            {
                Value = c.Area,
                Text = c.Area
            });
            ViewBag.DropOffArea = dropOff;
            return View();
        }


    }
}
