using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;
using Inc2SuchTrans.Models;

namespace Inc2SuchTrans.Controllers
{
    public class RoleController : Controller
    {
        ApplicationDbContext context = new ApplicationDbContext();

        /// <summary>
        /// 
        /// </summary>
        public RoleController()
        {
            context = new ApplicationDbContext();
        }

        // GET: Role
        public ActionResult Index()
        {
            try
            {
                var Roles = context.Roles.ToList();
                return View(Roles);
            }
            catch(Exception e)
            {
                return View();
                throw e;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            try
            {
                var Role = new IdentityRole();
                return View(Role);
            }
            catch(Exception e)
            {
                return View();
                throw e;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Role"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(IdentityRole Role)
        {
            context.Roles.Add(Role);
            context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}