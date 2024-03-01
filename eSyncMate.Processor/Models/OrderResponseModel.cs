using eSyncMate.DB.Entities;
using System.Runtime.Serialization;

namespace eSyncMate.Processor.Models
{
    [DataContract]
    public class OrderResponseModel : ResponseModel
    {
        [IgnoreDataMember]
        public Customers Customer { get; set; }
        [IgnoreDataMember]
        public Orders Order { get; set; }
        [DataMember]
        public List<OrderModel> Orders { get; set; }
    }

    public class OrderStoreResponseModel : ResponseModel
    {
        public int OrderId { get; set; }
        public string Status { get; set; }
    }

    public class OrderObjectModel 
    {
        public Customers Customer { get; set; }
        public Orders Order { get; set; }
    }
}
