using System;
using System.ComponentModel.DataAnnotations;

namespace Inc2SuchTrans.Models
{
    public class CustomerMetaData
    {
        public int CustomerID { get; set; }
        public string UserID { get; set; }

        [Display(Name = "First Name")]
        public string CustomerName { get; set; }

        [Display(Name = "Last Name")]
        public string CustomerSurname { get; set; }

        [Display(Name = "ID Number")]
        public string IDNumber { get; set; }

        [Display(Name = "City")]
        public string City { get; set; }

        [Display(Name = "Your Address")]
        public string CustomerAddress { get; set; }

        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Contact Number")]
        public string ContactNumber { get; set; }

        [Display(Name = "Date Created")]
        public Nullable<System.DateTime> DateCreated { get; set; }

        [Display(Name = "Last Modified")]
        public Nullable<System.DateTime> LastModified { get; set; }
    }

    public class QuoteMetaData
    {
        [Display(Name = "Quotation ID")]
        public int QuoteID { get; set; }

        [Display(Name = "Pick Up Area")]
        public string PickUpArea { get; set; }

        [Display(Name = "Drop Off Area")]
        public string DropOffArea { get; set; }

        [Display(Name = "Cargo Size (metres)")]
        public Nullable<int> CargoSize { get; set; }

        [Display(Name = "Cargo Weight (tons)")]
        public Nullable<decimal> CargoWeight { get; set; }

        [Display(Name = "Total Cost (Rands)")]
        public Nullable<decimal> TotalCost { get; set; }
    }

    public class DeliveryMetaData
    {
        [Display(Name = "Delivery ID")]
        public int DelID { get; set; }

        [Display(Name = "Customer Number")]
        public Nullable<int> CustomerID { get; set; }

        [Display(Name = "Date of Booking")]
        public Nullable<System.DateTime> CurrentDate { get; set; }

        [Display(Name = "Delivery Date")]
        public Nullable<System.DateTime> DeliveryDate { get; set; }

        [Display(Name = "Pick Up Area")]
        public string PickUpArea { get; set; }

        [Display(Name = "Drop Off Area")]
        public string DropOffArea { get; set; }

        [Display(Name = "Drop Off Address")]
        public string DropOffAdd { get; set; }

        [Display(Name = "Cargo Size (metres)")]
        public Nullable<int> CargoSize { get; set; }

        [Display(Name = "Cargo Weight (tons)")]
        public Nullable<decimal> CargoWeight { get; set; }

        [Display(Name = "Total Cost (rands)")]
        public Nullable<decimal> TotalCost { get; set; }

        [Display(Name = "Paid")]
        public Nullable<bool> Paid { get; set; }

        [Display(Name = "Reference Key")]
        public string DeliveryRef { get; set; }

        [Display(Name = "Delivery Status")]
        public string DeliveryStatus { get; set; }
    }

    public class DeliveryJobMetaData
    {
        [Display(Name = "Job ID")]
        public int JobID { get; set; }

        [Display(Name = "Delivery ID")]
        public Nullable<int> DelID { get; set; }

        [Display(Name = "Truck ID")]
        public Nullable<int> TruckID { get; set; }

        [Display(Name = "Driver ID")]
        public Nullable<int> DriverID { get; set; }

        [Display(Name = "Job Status")]
        public string JobStatus { get; set; }

        [Display(Name = "Port Delay")]
        public Nullable<bool> PortDelay { get; set; }
    }

    public class JobQueueMetaData
    {
        [Display(Name = "Queue ID")]
        public int queueId { get; set; }

        [Display(Name = "Job ID")]
        public Nullable<int> JobID { get; set; }

        [Display(Name = "Priority Rating")]
        public Nullable<int> PriorityScore { get; set; }

        [Display(Name = "Status")]
        public string QueueStatus { get; set; }
    }

    public class EmployeeMetaData
    {
        [Display(Name = "Employee ID")]
        public int EmployeeID { get; set; }

        [Display(Name = "User ID")]
        public string UserID { get; set; }

        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }

        [Display(Name = "Employee Surname")]
        public string EmployeeSurname { get; set; }

        [Display(Name = "SA ID Number")]
        public string IDNumber { get; set; }

        [Display(Name = "City")]
        public string City { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }

        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [Display(Name = "Position")]
        public string Position { get; set; }

        [Display(Name = "Contact Number")]
        public string ContactNumber { get; set; }
    }
}