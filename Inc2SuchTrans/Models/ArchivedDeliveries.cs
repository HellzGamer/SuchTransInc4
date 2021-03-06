//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Inc2SuchTrans.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ArchivedDeliveries
    {
        public int ArchDelID { get; set; }
        public Nullable<int> DeliveryID { get; set; }
        public Nullable<System.DateTime> CurrentDate { get; set; }
        public Nullable<System.DateTime> DeliveryDate { get; set; }
        public string PickUpArea { get; set; }
        public string DropOffArea { get; set; }
        public string DropOffAddress { get; set; }
        public Nullable<int> CargoSize { get; set; }
        public Nullable<decimal> CargoWeight { get; set; }
        public Nullable<decimal> TotalCost { get; set; }
        public Nullable<bool> Paid { get; set; }
        public string DeliveryRef { get; set; }
        public string DeliveryStatus { get; set; }
        public string ReasonForArchive { get; set; }
        public Nullable<int> CustomerID { get; set; }
    }
}
