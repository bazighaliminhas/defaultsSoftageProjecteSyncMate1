CREATE TABLE [dbo].[CustomerProductCatalog](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ERPCustomerID] [nvarchar](250) NULL,
	[TCIN] [nvarchar](250) NULL,
	[PartnerSKU] [nvarchar](250) NULL,
	[ProductTitle] [nvarchar](max) NULL,
	[ItemType] [nvarchar](50) NULL,
	[ItemTypeID] [nvarchar](50) NULL,
	[Relationship] [nvarchar](50) NULL,
	[PublishStatus] [nvarchar](50) NULL,
	[DataUpdatesStatus] [nvarchar](50) NULL,
	[Price] [decimal](18, 0) NULL,
	[OfferPrice] [decimal](18, 0) NULL,
	[MAPPrice] [decimal](18, 0) NULL,
	[OfferDiscount] [decimal](18, 0) NULL,
	[PriceLastUpdated] [datetime] NULL,
	[DistributionCenterName] [nvarchar](100) NULL,
	[DistributionCenterID] [nvarchar](50) NULL,
	[Inventory] [int] NULL,
	[InventoryLastUpdated] [datetime] NULL,
	CreatedDate			 DATETIME NOT NULL,
	CreatedBy			 INT NOT NULL,
	ModifiedDate		 DATETIME NULL,
	ModifiedBy			 INT NULL
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UC_ERPCustomerID_PartnerSKU] UNIQUE NONCLUSTERED 
(
	[ERPCustomerID] ASC,
	[PartnerSKU] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


