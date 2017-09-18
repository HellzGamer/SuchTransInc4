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

    public partial class Delivery
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Delivery()
        {
            this.Deliveryjob = new HashSet<Deliveryjob>();
        }

        public int DelID { get; set; }
        public Nullable<int> CustomerID { get; set; }
        public Nullable<System.DateTime> CurrentDate { get; set; }
        public System.DateTime DeliveryDate { get; set; }
        public string PickUpArea { get; set; }
        public string DropOffArea { get; set; }
        public string DropOffAdd { get; set; }
        public Nullable<int> CargoSize { get; set; }
        public Nullable<decimal> CargoWeight { get; set; }
        public Nullable<decimal> TotalCost { get; set; }
        public Nullable<bool> Paid { get; set; }
        public string DeliveryRef { get; set; }
        public string DeliveryStatus { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Customer Customer1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Deliveryjob> Deliveryjob { get; set; }
    }
}
