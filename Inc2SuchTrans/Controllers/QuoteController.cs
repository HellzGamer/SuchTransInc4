using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inc2SuchTrans.BLL;
using Inc2SuchTrans.Models;
using System.Globalization;
using System.Net.Mail;
using System.IO;
using HiQPdf;

namespace Inc2SuchTrans.Controllers
{
    public class QuoteController : BaseController
    {
        STLogisticsEntities db = new STLogisticsEntities();
        QuoteLogic logic = new QuoteLogic();
        // GET: Quote
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Creating a quotation GET
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
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
        /// Creating a quote POST
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(string PickUpArea, string DropOffArea, int CargoSize, string CargoWeight)
        {
            double pickUpCost = 0.00, dropOffCost = 0.00, weightCost = 0.00, quoteTotal;
            string refKey = "";
            decimal weight;

            if (string.IsNullOrEmpty(PickUpArea))
            {
                Danger("Please Select A Pick Up Area!");
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
            if (string.IsNullOrEmpty(DropOffArea))
            {
                Danger("Please Select A Drop Off Area!");
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
            if (string.IsNullOrEmpty(CargoWeight))
            {
                Danger("Please Enter A Valid Weight In The Input Provided! <br/> Weight Must Be In Tons");
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

            try
            {
                if (ModelState.IsValid)
                {
                    double temp = Double.Parse(CargoWeight, CultureInfo.InvariantCulture);
                    temp = Math.Round(temp, 2);

                    decimal.TryParse(temp.ToString("0.00"), out weight);

                    if (CargoSize == 6)
                    {
                        Cargo c = db.Cargo.SingleOrDefault(x => x.PickUpArea == PickUpArea);
                        AreaRates dropOff = db.AreaRates.SingleOrDefault(x => x.Area == DropOffArea);
                        pickUpCost = Convert.ToDouble(c.C6m);
                        dropOffCost = Convert.ToDouble(dropOff.Rate_6m_);

                        if (weight >= 22 && weight < 25)
                        {
                            weightCost = 20;
                        }
                        else
                            if (weight >= 25 && weight < 27)
                        {
                            weightCost = 30;
                        }
                        else
                            if (weight >= 27 && weight < 29)
                        {
                            weightCost = 40;
                        }
                        else
                            if (weight >= 29)
                        {
                            weightCost = 50;
                        }
                        else
                        {
                            weightCost = 0;
                        }

                    }
                    else
                   if (CargoSize == 12)
                    {
                        Cargo c = db.Cargo.SingleOrDefault(x => x.PickUpArea == PickUpArea);
                        AreaRates dropOff = db.AreaRates.SingleOrDefault(x => x.Area == DropOffArea);
                        pickUpCost = Convert.ToDouble(c.C12m);
                        dropOffCost = Convert.ToDouble(dropOff.Rate_12m_);

                        if (weight >= 24 && weight < 27)
                        {
                            weightCost = 20;
                        }

                        else
                            if (weight >= 27)
                        {
                            weightCost = 30;
                        }
                        else
                        {
                            weightCost = 0;
                        }
                    }

                    quoteTotal = logic.calculateQuote(pickUpCost, dropOffCost, weightCost);


                    Quote quote = new Quote();
                    refKey = logic.generateReferenceKey(PickUpArea, DropOffArea, CargoSize, weight, quoteTotal);
                    quote.PickUpArea = PickUpArea;
                    quote.DropOffArea = DropOffArea;
                    quote.CargoSize = CargoSize;
                    quote.CargoWeight = weight;

                    quote.TotalCost = Math.Round(Convert.ToDecimal(quoteTotal), 2);
                    quote.ReferenceKey = refKey;
                    logic.addQuote(quote);

                    Session["QuoteID"] = quote.QuoteID;
                    Session["PUArea"] = PickUpArea;
                    Session["DOArea"] = DropOffArea;
                    Session["CargoSize"] = CargoSize;
                    Session["CargoWeight"] = CargoWeight;
                    Session["TotalCost"] = quoteTotal;
                    Session["ReferenceKey"] = refKey;
                    return RedirectToAction("Details", new { id = quote.QuoteID });
                }
                else
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

            }
            catch (Exception e)
            {
                Danger("Oops! Something went wrong..<br> We were unable to process your request Please contact support..");
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
                throw e;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int? id)
        {
            try
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
                Quote quote = logic.searchQuote(id);
                return View(quote);
            }
            catch (Exception e)
            {
                Danger("Oops! Something went wrong..<br> We were unable to process your request Please contact support..");
                return View();
                throw e;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(int? QuoteID, string PickUpArea, string DropOffArea, int CargoSize, string CargoWeight)
        {
            if (string.IsNullOrEmpty(PickUpArea))
            {
                Danger("Please Select A Pick Up Area!");
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
            if (string.IsNullOrEmpty(DropOffArea))
            {
                Danger("Please Select A Drop Off Area!");
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

            //string cWeight = Convert.ToString(CargoWeight);

            if (string.IsNullOrEmpty(CargoWeight))
            {
                Danger("Please Enter A Valid Weight In The Input Provided! <br/> Weight Must Be In Tons");
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

            double pickUpCost = 0.00, dropOffCost = 0.00, weightCost = 0.00, quoteTotal = 0.00;

            try
            {
                if (ModelState.IsValid)
                {
                    Quote newQuote = new Quote();
                    decimal weight;
                    double temp = Double.Parse(CargoWeight.ToString(), CultureInfo.InvariantCulture);
                    temp = Math.Round(temp, 2);

                    decimal.TryParse(temp.ToString("0.00"), out weight);

                    if (CargoSize == 6)
                    {
                        Cargo c = db.Cargo.SingleOrDefault(x => x.PickUpArea == PickUpArea);
                        AreaRates dropOff = db.AreaRates.SingleOrDefault(x => x.Area == DropOffArea);
                        pickUpCost = Convert.ToDouble(c.C6m);
                        dropOffCost = Convert.ToDouble(dropOff.Rate_6m_);

                        if (weight >= 22 && weight < 25)
                        {
                            weightCost = 20;
                        }
                        else
                            if (weight >= 25 && weight < 27)
                        {
                            weightCost = 30;
                        }
                        else
                            if (weight >= 27 && weight < 29)
                        {
                            weightCost = 40;
                        }
                        else
                            if (weight >= 29)
                        {
                            weightCost = 50;
                        }
                        else
                        {
                            weightCost = 0;
                        }

                    }
                    else
                   if (CargoSize == 12)
                    {
                        Cargo c = db.Cargo.SingleOrDefault(x => x.PickUpArea == PickUpArea);
                        AreaRates dropOff = db.AreaRates.SingleOrDefault(x => x.Area == DropOffArea);
                        pickUpCost = Convert.ToDouble(c.C12m);
                        dropOffCost = Convert.ToDouble(dropOff.Rate_12m_);

                        if (weight >= 24 && weight < 27)
                        {
                            weightCost = 20;
                        }

                        else
                            if (weight >= 27)
                        {
                            weightCost = 30;
                        }
                        else
                        {
                            weightCost = 0;
                        }
                    }

                    quoteTotal = logic.calculateQuote(pickUpCost, dropOffCost, weightCost);
                    string refKey = logic.generateReferenceKey(PickUpArea, DropOffArea, CargoSize, weight, quoteTotal);
                    string datetime = Convert.ToString(System.DateTime.Now);
                    //logic.updateQuote(toUdpate, Convert.ToDecimal(quoteTotal), refKey);
                    newQuote.PickUpArea = PickUpArea;
                    newQuote.DropOffArea = DropOffArea;
                    newQuote.CargoSize = CargoSize;
                    newQuote.CargoWeight = weight;
                    newQuote.TotalCost = Convert.ToDecimal(quoteTotal);
                    newQuote.ReferenceKey = refKey;
                    logic.addQuote(newQuote);

                    Session["PUArea"] = PickUpArea;
                    Session["DOArea"] = DropOffArea;
                    Session["CargoSize"] = CargoSize;
                    Session["CargoWeight"] = CargoWeight;
                    Session["TotalCost"] = quoteTotal;

                    return RedirectToAction("Details", new { id = newQuote.QuoteID });
                }
                else
                {
                    return View();
                }
            }
            catch (Exception e)
            {
                Danger("Oops! Something went wrong..<br> We were unable to process your request Please contact support..");
                return View();
                throw e;
            }
        }

        public ActionResult Details(int? id)
        {
            try
            {
                Quote q = logic.searchQuote(id);
                this.Session["PArea"] = q.PickUpArea;
                this.Session["DArea"] = q.DropOffArea;
                this.Session["size"] = q.CargoSize;
                this.Session["weight"] = q.CargoWeight;
                this.Session["Total"] = q.TotalCost;
                this.Session["ref"] = q.ReferenceKey;
                return View(q);
            }
            catch (Exception e)
            {
                Danger("Oops! Something went wrong..<br> We were unable to process your request Please contact support..");
                return View(e.Message);
                throw e;
            }
        }

        /// <summary>
        /// get
        /// </summary>
        /// <returns></returns>
        public ActionResult EmailQuote(int? id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult EmailQuote(int? id, MailModel _objModelMail)
        {
            string url = Url.Action("CreateFromEmail", "Delivery",
                       new System.Web.Routing.RouteValueDictionary(), "http", Request.Url.Host);


            try
            {
                Quote quote = logic.searchQuote(id);

                string pdfurl = "Quote/Details/" + id.ToString();
                // get the HTML code of this view
                string htmlToConvert = RenderViewAsString("Details", quote);

                // the base URL to resolve relative images and css
                String thisPageUrl = this.ControllerContext.HttpContext.Request.Url.AbsoluteUri;
                String baseUrl = thisPageUrl.Substring(0, thisPageUrl.Length - pdfurl.Length);

                // instantiate the HiQPdf HTML to PDF converter
                HtmlToPdf htmlToPdfConverter = new HtmlToPdf();
                htmlToPdfConverter.HtmlLoadedTimeout = 600;

                // hide the button in the created PDF
                htmlToPdfConverter.HiddenHtmlElements = new string[] { "#convertThisPageButtonDiv" };

                // render the HTML code as PDF in memory
                byte[] pdfBuffer = htmlToPdfConverter.ConvertHtmlToMemory(htmlToConvert, baseUrl);

                Attachment att = new Attachment(new MemoryStream(pdfBuffer), "Quotation Details.pdf");

                if (ModelState.IsValid)
                {
                    MailMessage mail = new MailMessage();
                    mail.To.Add(_objModelMail.To);
                    mail.From = new MailAddress("suchtranslogistics@gmail.com");
                    mail.Subject = "Quotation Details";
                    string Body = "Welcome!" + "<br/>" + "Your requested quotation Details are as follows:" + "<br/>"
                                + "Pick Up Area: " + Session["PUArea"].ToString() + "<br/>"
                                + "Drop Off Area: " + Session["DOArea"].ToString() + "<br/>"
                                + "Cargo Size: " + Session["CargoSize"].ToString() + "m" + "<br/>"
                                + "Cargo Weight(tons): " + Session["CargoWeight"].ToString() + "<br/>"
                                + "Total Cost: R" + Session["TotalCost"].ToString() + "<br/>"
                                + "You can choose to accept this quote and book a delivery with us with the following link: " + url + "?id=" + Session["QuoteID"]
                                ;
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

                    Success("Email has been sent.<br> Please check your email to make sure you have received it!");
                    return View("EmailQuote");
                }
                else
                {
                    return View();
                }
            }
            catch (Exception e)
            {
                Danger("Something went wrong! <br/> Please check your internet connection and try again or contact support" + "Error: " + e.Message + " " + e.StackTrace);
                return View();
                throw e;
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
    }
}