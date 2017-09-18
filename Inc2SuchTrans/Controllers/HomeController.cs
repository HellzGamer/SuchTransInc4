using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inc2SuchTrans.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (User.IsInRole("Super Admin"))
            {
                return RedirectToAction("Index", "Admin");
            }
            else
                if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Admin");
            }
            else
                if (User.IsInRole("Operations Manager"))
            {
                return RedirectToAction("Index", "Admin");
            }
            else
                if (User.IsInRole("Driver"))
            {
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                return View();
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}