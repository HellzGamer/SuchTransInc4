using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Inc2SuchTrans.Models;

namespace Inc2SuchTrans.BLL
{
    public class CustomerLogic
    {
        STLogisticsEntities db = new STLogisticsEntities();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customer"></param>
        public void addCustomer(Customer customer)
        {
            db.Customer.Add(customer);
            db.SaveChanges();
        }


        public List<Customer> returnAllCustomers()
        {
            try
            {
                return db.Customer.ToList();
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int getCurrentUserId(string userId)
        {
            try
            {
                Customer currentCust = db.Customer.ToList().Find(x => x.UserID == userId);
                return currentCust.CustomerID;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Customer searchCustomer(int? id)
        {
            Customer result = db.Customer.Find(id);
            return result;
        }


        public void updateDetails(Customer cust)
        {
            try
            {
                db.Customer.Attach(cust);
                var entry = db.Entry(cust);
                entry.Property(x => x.CustomerName).IsModified = true;
                entry.Property(x => x.CustomerSurname).IsModified = true;
                entry.Property(x => x.PostalCode).IsModified = true;
                entry.Property(x => x.CustomerAddress).IsModified = true;
                entry.Property(x => x.City).IsModified = true;
                entry.Property(x => x.ContactNumber).IsModified = true;
                entry.Property(x => x.Email).IsModified = true;
                entry.Property(x => x.IDNumber).IsModified = true;
                entry.Property(x => x.LastModified).IsModified = true;

                db.SaveChanges();
            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }
}