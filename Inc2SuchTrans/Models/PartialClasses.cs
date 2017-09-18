using System.ComponentModel.DataAnnotations;

namespace Inc2SuchTrans.Models
{
    [MetadataType(typeof(CustomerMetaData))]
    public partial class Customer
    {

    }

    [MetadataType(typeof(QuoteMetaData))]
    public partial class Quote
    {

    }

    [MetadataType(typeof(DeliveryMetaData))]
    public partial class Delivery
    {

    }

    [MetadataType(typeof(DeliveryJobMetaData))]
    public partial class DeliveryJob
    {

    }

    [MetadataType(typeof(JobQueueMetaData))]
    public partial class JobQueue
    {

    }
    [MetadataType(typeof(EmployeeMetaData))]
    public partial class Employee
    {

    }
}