using Microsoft.VisualBasic;

namespace eSyncMate.Processor.Models
{
    public class CustomerProductCatalogModel
    {
        public int Id { get; set; }
    }

    public class CustomerProductCatalogDataModel
    {
        public int Id { get; set; }
        public string ERPCustomerID { get; set; }
        public string TCIN { get; set; }
        public string PartnerSKU { get; set; }
        public string ProductTitle { get; set; }
        public string ItemType { get; set; }
        public string ItemTypeID { get; set; }
        public string Relationship { get; set; }
        public string PublishStatus { get; set; }
        public string DataUpdatesStatus { get; set; }
        public decimal Price { get; set; }
        public decimal OfferPrice { get; set; }
        public decimal MAPPrice { get; set; }
        public decimal OfferDiscount { get; set; }
        public DateTime PriceLastUpdated { get; set; }
        public string DistributionCenterName { get; set; }
        public string DistributionCenterID { get; set; }

        public int Inventory { get; set; }
        public DateTime InventoryLastUpdated { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
       

    }

    public class SaveCustomerProductCatalogDataModel
    {
        public int Id { get; set; }
        public string ERPCustomerID { get; set; }
        public string TCIN { get; set; }
        public string PartnerSKU { get; set; }
        public string ProductTitle { get; set; }
        public string ItemType { get; set; }
        public string ItemTypeID { get; set; }
        public string Relationship { get; set; }
        public string PublishStatus { get; set; }
        public string DataUpdatesStatus { get; set; }
        public decimal Price { get; set; }
        public decimal OfferPrice { get; set; }
        public decimal MAPPrice { get; set; }
        public decimal OfferDiscount { get; set; }
        public DateTime PriceLastUpdated { get; set; }
        public string DistributionCenterName { get; set; }
        public string DistributionCenterID { get; set; }

        public int Inventory { get; set; }
        public DateTime InventoryLastUpdated { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
    }

    public class UpdateCustomerProductCatalogDataModel
    {
        public int Id { get; set; }
        public string ERPCustomerID { get; set; }
        public string TCIN { get; set; }
        public string PartnerSKU { get; set; }
        public string ProductTitle { get; set; }
        public string ItemType { get; set; }
        public string ItemTypeID { get; set; }
        public string Relationship { get; set; }
        public string PublishStatus { get; set; }
        public string DataUpdatesStatus { get; set; }
        public decimal Price { get; set; }
        public decimal OfferPrice { get; set; }
        public decimal MAPPrice { get; set; }
        public decimal OfferDiscount { get; set; }
        public DateTime PriceLastUpdated { get; set; }
        public string DistributionCenterName { get; set; }

        public string DistributionCenterID { get; set; }

        public int Inventory { get; set; }
        public DateTime InventoryLastUpdated { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int ModifiedBy { get; set; }
    }

    public class CustomerProductCatalogSearchModel
    {
        public string SearchOption { get; set; }
        public string SearchValue { get; set; }
    }
}
