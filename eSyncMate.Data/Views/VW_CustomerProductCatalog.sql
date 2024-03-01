CREATE VIEW [dbo].[VW_CustomerProductCatalog]
AS
	SELECT
	TOP 50
	C.ID,
	C.ERPCustomerID,
	C.TCIN,
	C.PartnerSKU,
	C.ProductTitle,
	C.ItemType,
	C.ItemTypeID,
	C.Relationship,
	C.PublishStatus,
	C.DataUpdatesStatus,
	C.Price,
	C.OfferPrice,
	C.MAPPrice,
	C.OfferDiscount,
	C.PriceLastUpdated,
	C.DistributionCenterName,
	C.DistributionCenterID,
	C.Inventory,
	C.InventoryLastUpdated,
	C.CreatedDate,
	C.CreatedBy,
	C.ModifiedDate,
	C.ModifiedBy
	FROM CustomerProductCatalog C WITH (NOLOCK)
	
GO