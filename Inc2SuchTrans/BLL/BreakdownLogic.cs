using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Inc2SuchTrans.Models;

namespace Inc2SuchTrans.BLL
{
    public class BreakdownLogic
    {
        STLogisticsEntities db = new STLogisticsEntities();
        public List<Breakdowns> getAllBreakDowns()
        {
            try
            {
                return db.Breakdowns.ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void addBreakDown(Breakdowns b)
        {
            try
            {
                db.Breakdowns.Add(b);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}