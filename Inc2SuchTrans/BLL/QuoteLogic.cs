using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Inc2SuchTrans.Models;

namespace Inc2SuchTrans.BLL
{
    public class QuoteLogic
    {
        STLogisticsEntities db = new STLogisticsEntities();

        public double calculateQuote(double pickUpCost, double dropOffCost, double weightCost)
        {
            return pickUpCost + dropOffCost + ((pickUpCost + dropOffCost) * weightCost / 100);
        }

        public string generateReferenceKey(string pickup, string dropoff, int? size, decimal? weight, double total)
        {
            string dateTime = Convert.ToString(System.DateTime.Now).Substring(5, 2) + Convert.ToString(System.DateTime.Now).Substring(8, 2) + Convert.ToString(System.DateTime.Now).Substring(17, 2);
            string refkey = pickup.Substring(0, 2) + dateTime + Convert.ToString(total).Substring(0, 2) + dropoff.Substring(0, 3) + size + Convert.ToString(weight).Substring(0, 2);
            return refkey.Replace(" ","");
        }

        public void addQuote(Quote q)
        {
            db.Quote.Add(q);
            db.SaveChanges();
        }

        public Quote searchQuote(int? id)
        {
            return db.Quote.Find(id);
        }

        public void updateQuote(Quote q, decimal total, string refKey)
        {
            Quote toEdit = db.Quote.Find(q.QuoteID);
            toEdit.PickUpArea = q.PickUpArea;
            toEdit.DropOffArea = q.DropOffArea;
            toEdit.CargoSize = q.CargoSize;
            toEdit.CargoWeight = q.CargoWeight;
            toEdit.TotalCost = total;
            toEdit.ReferenceKey = refKey;

            db.SaveChanges();
        }
    }
}