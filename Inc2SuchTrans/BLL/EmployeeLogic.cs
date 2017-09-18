using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Inc2SuchTrans.Models;

namespace Inc2SuchTrans.BLL
{
    public class EmployeeLogic
    {
        STLogisticsEntities db = new STLogisticsEntities();
        public List<Employee> getAllEmployees()
        {
            try
            {
                return db.Employee.ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void addEmployee(Employee emp)
        {
            db.Employee.Add(emp);
            db.SaveChanges();
        }

        public Employee searchEmployee(int? id)
        {
            try
            {
                return db.Employee.Find(id);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int getCurrentEployeeID(string id)
        {
            try
            {
                Employee emp = db.Employee.Where(x => x.UserID == id).SingleOrDefault();
                return emp.EmployeeID;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}