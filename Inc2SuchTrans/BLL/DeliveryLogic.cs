using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Inc2SuchTrans.Models;

namespace Inc2SuchTrans.BLL
{
    public class DeliveryLogic
    {
        STLogisticsEntities db = new STLogisticsEntities();

        public List<Delivery> returnAllDeliveries()
        {
            return db.Delivery.ToList();
        }


        public void addDelivery(Delivery del)
        {
            db.Delivery.Add(del);
            db.SaveChanges();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="del"></param>
        public void updateDelivery(Delivery del)
        {
            try
            {
                db.Delivery.Attach(del);
                var entry = db.Entry(del);
                entry.Property(x => x.Paid).IsModified = true;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public int getDeliveryId(string refkey)
        {
            try
            {
                Delivery del = db.Delivery.ToList().Find(x => x.DeliveryRef == refkey);
                return del.DelID;
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        
        //public List<Delivery> returnSearchDelivery(int id)

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Delivery searchDelivery(int? id)
        {
            try
            {
                return db.Delivery.Find(id);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pickUp"></param>
        /// <param name="dropoff"></param>
        /// <param name="cargoSize"></param>
        /// <param name="cargoWeight"></param>
        /// <returns></returns>
        public decimal determineTotalCost(string pickUp, string dropoff, int cargoSize, decimal cargoWeight)
        {
            double pickUpCost = 0.00, dropOffCost = 0.00, weightCost = 0.00, Total;
            decimal TotalCost;

            if (cargoSize == 6)
            {
                Cargo c = db.Cargo.SingleOrDefault(x => x.PickUpArea == pickUp);
                AreaRates dropOff = db.AreaRates.SingleOrDefault(x => x.Area == dropoff);
                pickUpCost = Convert.ToDouble(c.C6m);
                dropOffCost = Convert.ToDouble(dropOff.Rate_6m_);

                if (cargoWeight >= 22 && cargoWeight < 25)
                {
                    weightCost = 20;
                }
                else
                    if (cargoWeight >= 25 && cargoWeight < 27)
                {
                    weightCost = 30;
                }
                else
                    if (cargoWeight >= 27 && cargoWeight < 29)
                {
                    weightCost = 40;
                }
                else
                    if (cargoWeight >= 29)
                {
                    weightCost = 50;
                }
                else
                {
                    weightCost = 0;
                }

            }
            else
                   if (cargoSize == 12)
            {
                Cargo c = db.Cargo.SingleOrDefault(x => x.PickUpArea == pickUp);
                AreaRates dropOff = db.AreaRates.SingleOrDefault(x => x.Area == dropoff);
                pickUpCost = Convert.ToDouble(c.C12m);
                dropOffCost = Convert.ToDouble(dropOff.Rate_12m_);

                if (cargoWeight >= 24 && cargoWeight < 27)
                {
                    weightCost = 20;
                }

                else
                    if (cargoWeight >= 27)
                {
                    weightCost = 30;
                }
                else
                {
                    weightCost = 0;
                }
            }
            Total = pickUpCost + dropOffCost + ((pickUpCost + dropOffCost) * weightCost / 100);
            decimal.TryParse(Total.ToString("0.00"), out TotalCost);
            return TotalCost;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public List<Delivery> searchCustomerDeliveries(string UserId)
        {
            try
            {
                Customer cust = db.Customer.ToList().Find(x => x.UserID == UserId);
                List<Delivery> del = db.Delivery.ToList().FindAll(x => x.CustomerID == cust.CustomerID).ToList();
                return del;
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public List<Delivery> returnUnpaidDeliveries(string UserId)
        {
            try
            {
                Customer cust = db.Customer.ToList().Find(x => x.UserID == UserId);
                if (cust != null)
                {
                    List<Delivery> del = db.Delivery.ToList().FindAll(x => x.CustomerID == cust.CustomerID).ToList().Where(x => x.Paid == false).ToList(); ;
                    return del;
                }
                else
                    return null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}