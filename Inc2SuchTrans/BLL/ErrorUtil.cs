using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Inc2SuchTrans.Models;

namespace Inc2SuchTrans.BLL
{
    public class ErrorUtil
    {
        public static void InsertError(string Message, string Detail)
        {
            Error error = new Error { ErrorName=Message,ErrorDetails=Detail };

            using (var db= new STLogisticsEntities())
            {
                db.Error.Add(error);

                db.SaveChanges();
            }

        }
    }
}