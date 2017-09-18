using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Inc2SuchTrans.Models;

namespace Inc2SuchTrans.BLL
{
    public class CargoLogic
    {
        STLogisticsEntities db = new STLogisticsEntities();

        public List<Cargo> returnAllCargo()
        {
            try
            {
                return db.Cargo.ToList();
            }
            catch(Exception e)
            {
                throw e;
            }
        }

    }
}