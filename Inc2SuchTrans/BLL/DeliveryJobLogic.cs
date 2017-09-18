using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Inc2SuchTrans.Models;

namespace Inc2SuchTrans.BLL
{
    public class DeliveryJobLogic
    {
        STLogisticsEntities db = new STLogisticsEntities();

        public void addDeliveryJob(Deliveryjob dj)
        {
            try
            {
                db.Deliveryjob.Add(dj);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<Deliveryjob> getAllDeliveryJobs()
        {
            try
            {
                return db.Deliveryjob.ToList().OrderBy(x => x.JobStatus).ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void updateDetails(Deliveryjob dj)
        {
            try
            {
                db.Deliveryjob.Attach(dj);
                var entry = db.Entry(dj);
                entry.Property(x => x.DriverID).IsModified = true;
                entry.Property(x => x.TruckID).IsModified = true;
                entry.Property(x => x.JobStatus).IsModified = true;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public Deliveryjob findJobByTruck(int id)
        {
            Deliveryjob dj = new Deliveryjob();

            try
            {
                dj = db.Deliveryjob.Where(x => x.TruckID == id).FirstOrDefault();
                if (dj != null)
                {
                    return dj;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}