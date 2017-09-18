using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Inc2SuchTrans.Models;

namespace Inc2SuchTrans.BLL
{
    public class ArchiveLogic
    {
        STLogisticsEntities db = new STLogisticsEntities();

        public void ArchiveDelivery(ArchivedDeliveries arcDelivery)
        {
            try
            {
                db.ArchivedDeliveries.Add(arcDelivery);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}