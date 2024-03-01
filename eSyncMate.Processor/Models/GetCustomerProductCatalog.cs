using eSyncMate.DB.Entities;

namespace eSyncMate.Processor.Models
{
    public class GetCustomerProductCatalog : ResponseModel
    {
        public List<CustomerProductCatalogDataModel> CustomerProductCatalog { get; set; }
    }
}
