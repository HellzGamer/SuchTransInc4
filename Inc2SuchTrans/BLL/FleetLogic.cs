using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Inc2SuchTrans.Models;

namespace Inc2SuchTrans.BLL
{
    public class FleetLogic
    {
        STLogisticsEntities db = new STLogisticsEntities();

        public void addTruck(Fleet truck)
        {
            db.Fleet.Add(truck);
            db.SaveChanges();
        }

        public List<Fleet> returnAllTrucks()
        {
            try
            {
                return db.Fleet.ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Fleet searchTruck(int id)
        {
            try
            {
                return db.Fleet.Find(id);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void updateStatus(Fleet truck)
        {
            try
            {
                db.Fleet.Attach(truck);
                var entry = db.Entry(truck);
                entry.Property(x => x.Availability).IsModified = true;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int truckId(string tnp)
        {
            try
            {
                Fleet f = db.Fleet.Where(x => x.TruckNumberPlate == tnp).First();
                return f.TruckId;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}