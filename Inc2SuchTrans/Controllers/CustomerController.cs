using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inc2SuchTrans.Models;
using Inc2SuchTrans.BLL;
using Microsoft.AspNet.Identity;
using Inc2SuchTrans.CustomFilters;

namespace Inc2SuchTrans.Controllers
{
    public class CustomerController : BaseController
    {
        CustomerLogic cLogic = new CustomerLogic();
        STLogisticsEntities db = new STLogisticsEntities();
        // GET: Customer
        public ActionResult Index()
        {
            try
            {
                return View(cLogic.returnAllCustomers());
            }
            catch(Exception e)
            {
                Danger("Oops! Something went wrong..<br> Please contact support..");
                return View();
                throw e;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthLog(Roles ="Customer")]
        public ActionResult Edit(int? id)
        {
            try
            {
                Customer cust = cLogic.searchCustomer(id);
                return View(cust);
            } 
            catch(Exception e)
            {
                Danger("Oops! SOmething went wrong.. Please contact support");
                return View();
                throw e;
            }
        }

        [AuthLog(Roles = "Customer")]
        [HttpPost]
        public ActionResult Edit(int id,string CustomerName, string CustomerSurname, string IDNumber, string City, string CustomerAddress, string PostalCode, string Email, string ContactNumber)
        {
            if(String.IsNullOrEmpty(CustomerName))
            {
                Danger("Please Enter Your Name!");
                Customer cust = cLogic.searchCustomer(id);
                return View(cust);
            }

            try
            {
                Customer cust = cLogic.searchCustomer(id);
                if(cust!=null)
                {
                    cust.UserID = User.Identity.GetUserId();
                    cust.CustomerName = CustomerName;
                    cust.CustomerSurname = CustomerSurname;
                    cust.IDNumber = IDNumber;
                    cust.City = City;
                    cust.CustomerAddress = CustomerAddress;
                    cust.PostalCode = PostalCode;
                    cust.Email = Email;
                    cust.ContactNumber = ContactNumber;
                    cust.LastModified = System.DateTime.Now;

                    cLogic.updateDetails(cust);
                }
                Success("Successfully Updated Details!");
                return RedirectToAction("Details", new { id = cust.CustomerID });
            }
            catch(Exception e)
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
        public ActionResult Details(int? id)
        {
            try
            {
                id = cLogic.getCurrentUserId(User.Identity.GetUserId());
                Customer cust = cLogic.searchCustomer(id);
                return View(cust);
            }
            catch (Exception e)
            {
                Danger("Oops! Something went wrong.. <br> Cannot retrieve your details. Please contact support..");
                return View();
                throw e;
            }
        }
    }
}