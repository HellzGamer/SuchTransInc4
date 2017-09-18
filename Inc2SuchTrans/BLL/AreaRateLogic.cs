using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Inc2SuchTrans.Models;

namespace Inc2SuchTrans.BLL
{
    public class AreaRateLogic
    {
        STLogisticsEntities db = new STLogisticsEntities();

        public List<AreaRates> returnAllAreas()
        {
            try
            {
                return db.AreaRates.ToList();
            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }
}