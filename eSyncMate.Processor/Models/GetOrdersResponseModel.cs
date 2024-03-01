namespace eSyncMate.Processor.Models
{
    public class GetOrdersResponseModel : ResponseModel
    {
        public List<OrderDataModel> Orders { get; set; }
    }
    public class GetOrderFilesResponseModel : ResponseModel
    {
        public List<OrderFileModel> Files { get; set; }
    }

    public class GetOrderStoresResponseModel : ResponseModel
    {
        public List<OrderStoreDataModel> OrderStores { get; set; }
    }
}
