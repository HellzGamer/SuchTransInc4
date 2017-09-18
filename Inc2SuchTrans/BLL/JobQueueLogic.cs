using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Inc2SuchTrans.Models;

namespace Inc2SuchTrans.BLL
{
    public class JobQueueLogic
    {
        STLogisticsEntities db = new STLogisticsEntities();

        public List<JobQueue> returnJobQueue()
        {
            try
            {
                return db.JobQueue.ToList().OrderByDescending(X => X.PriorityScore).ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void addJobToQueue(JobQueue q)
        {
            try
            {
                db.JobQueue.Add(q);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public JobQueue searchItem(int jobID)
        {
            try
            {
                return db.JobQueue.Where(x => x.JobID == jobID).SingleOrDefault();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void updateJobQueue(JobQueue jq)
        {
            try
            {
                db.JobQueue.Attach(jq);
                var entry = db.Entry(jq);
                entry.Property(x => x.PriorityScore).IsModified = true;

                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}