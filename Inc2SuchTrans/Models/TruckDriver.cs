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
    
    public partial class TruckDriver
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TruckDriver()
        {
            this.Deliveryjob = new HashSet<Deliveryjob>();
        }
    
        public int DriverID { get; set; }
        public Nullable<int> EmpID { get; set; }
        public Nullable<bool> Avail { get; set; }
        public Nullable<bool> PriorityStatus { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Deliveryjob> Deliveryjob { get; set; }
        public virtual Employee Employee { get; set; }
    }
}