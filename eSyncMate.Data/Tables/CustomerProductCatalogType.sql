CREATE TYPE dbo.CustomerProductCatalogType AS TABLE
(
    TCIN					NVARCHAR(250) NULL,
    PartnerSKU				NVARCHAR(250) NULL,
    ProductTitle            NVARCHAR(MAX) NULL,	
    ItemType				NVARCHAR(50) NULL,
    ItemTypeID				NVARCHAR(50) NULL,
    Relationship			NVARCHAR(50) NULL,
    PublishStatus			NVARCHAR(50) NULL,
    DataUpdatesStatus		NVARCHAR(50) NULL,
    Price					DECIMAL NULL,
    OfferPrice				DECIMAL NULL,
    MAPPrice				DECIMAL NULL,
    OfferDiscount			DECIMAL NULL,
    PriceLastUpdated		DATETIME NULL,
    DistributionCenterName	NVARCHAR(100) NULL,
    DistributionCenterID	NVARCHAR(50) NULL,
    Inventory				INT NULL,
    InventoryLastUpdated    DATETIME NULL
);