using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Inc2SuchTrans.Models;

namespace Inc2SuchTrans.BLL
{
    public class TruckDriverLogic
    {
        STLogisticsEntities db = new STLogisticsEntities();

        public void addDriver(TruckDriver driver)
        {
            db.TruckDriver.Add(driver);
            db.SaveChanges();
        }

        public List<TruckDriver> returnAllDrivers()
        {
            try
            {
                return db.TruckDriver.ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public TruckDriver searchDriver(int? id)
        {
            try
            {
                return db.TruckDriver.Find(id);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void updateStatus(TruckDriver driver)
        {
            try
            {
                db.TruckDriver.Attach(driver);
                var entry = db.Entry(driver);
                entry.Property(x => x.Avail).IsModified = true;
                //entry.Property(x => x.TruckNumberPlate).IsModified = true;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}