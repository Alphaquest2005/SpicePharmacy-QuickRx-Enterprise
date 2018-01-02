
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 06/26/2014 12:32:35
-- Generated from EDMX file: C:\Insight Software\PrismApplication1\RMS\RMS.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [GMPRMSPOS];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Batch]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Batch];
GO
IF OBJECT_ID(N'[dbo].[Item]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Item];
GO
IF OBJECT_ID(N'[dbo].[TransactionHold]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TransactionHold];
GO
IF OBJECT_ID(N'[dbo].[TransactionHoldEntry]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TransactionHoldEntry];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Batch'
CREATE TABLE [dbo].[Batch] (
    [CustomerDepositMade] decimal(19,4)  NOT NULL,
    [CustomerDepositRedeemed] decimal(19,4)  NOT NULL,
    [LastUpdated] datetime  NOT NULL,
    [StoreID] int  NOT NULL,
    [BatchNumber] int IDENTITY(1,1) NOT NULL,
    [Status] smallint  NOT NULL,
    [RegisterID] int  NOT NULL,
    [OpeningTime] datetime  NULL,
    [ClosingTime] datetime  NULL,
    [OpeningTotal] decimal(19,4)  NOT NULL,
    [ClosingTotal] decimal(19,4)  NOT NULL,
    [Sales] decimal(19,4)  NOT NULL,
    [Returns] decimal(19,4)  NOT NULL,
    [Tax] decimal(19,4)  NOT NULL,
    [SalesPlusTax] decimal(19,4)  NOT NULL,
    [Commission] decimal(19,4)  NOT NULL,
    [PaidOut] decimal(19,4)  NOT NULL,
    [Dropped] decimal(19,4)  NOT NULL,
    [PaidOnAccount] decimal(19,4)  NOT NULL,
    [PaidToAccount] decimal(19,4)  NOT NULL,
    [CustomerCount] int  NOT NULL,
    [NoSalesCount] int  NOT NULL,
    [AbortedTransCount] int  NOT NULL,
    [TotalTendered] decimal(19,4)  NOT NULL,
    [TotalChange] decimal(19,4)  NOT NULL,
    [Discounts] decimal(19,4)  NOT NULL,
    [CostOfGoods] decimal(19,4)  NOT NULL,
    [LayawayPaid] decimal(19,4)  NOT NULL,
    [LayawayClosed] decimal(19,4)  NOT NULL,
    [Shipping] decimal(19,4)  NOT NULL,
    [DBTimeStamp] binary(8)  NULL,
    [TenderRoundingError] decimal(19,4)  NOT NULL,
    [DebitSurcharge] decimal(19,4)  NOT NULL,
    [CashBackSurcharge] decimal(19,4)  NOT NULL,
    [Vouchers] decimal(19,4)  NOT NULL
);
GO

-- Creating table 'Item'
CREATE TABLE [dbo].[Item] (
    [BinLocation] nvarchar(20)  NOT NULL,
    [BuydownPrice] decimal(19,4)  NOT NULL,
    [BuydownQuantity] float  NOT NULL,
    [CommissionAmount] decimal(19,4)  NOT NULL,
    [CommissionMaximum] decimal(19,4)  NOT NULL,
    [CommissionMode] int  NOT NULL,
    [CommissionPercentProfit] real  NOT NULL,
    [CommissionPercentSale] real  NOT NULL,
    [Description] nvarchar(30)  NOT NULL,
    [FoodStampable] bit  NOT NULL,
    [HQID] int  NOT NULL,
    [ItemNotDiscountable] bit  NOT NULL,
    [LastReceived] datetime  NULL,
    [LastUpdated] datetime  NOT NULL,
    [Notes] nvarchar(max)  NULL,
    [QuantityCommitted] float  NOT NULL,
    [SerialNumberCount] int  NOT NULL,
    [TareWeightPercent] float  NOT NULL,
    [ID] int IDENTITY(1,1) NOT NULL,
    [ItemLookupCode] nvarchar(25)  NOT NULL,
    [DepartmentID] int  NOT NULL,
    [CategoryID] int  NOT NULL,
    [MessageID] int  NOT NULL,
    [Price] decimal(19,4)  NOT NULL,
    [PriceA] decimal(19,4)  NOT NULL,
    [PriceB] decimal(19,4)  NOT NULL,
    [PriceC] decimal(19,4)  NOT NULL,
    [SalePrice] decimal(19,4)  NOT NULL,
    [SaleStartDate] datetime  NULL,
    [SaleEndDate] datetime  NULL,
    [QuantityDiscountID] int  NOT NULL,
    [TaxID] int  NOT NULL,
    [ItemType] smallint  NOT NULL,
    [Cost] decimal(19,4)  NOT NULL,
    [Quantity] float  NOT NULL,
    [ReorderPoint] float  NOT NULL,
    [RestockLevel] float  NOT NULL,
    [TareWeight] float  NOT NULL,
    [SupplierID] int  NOT NULL,
    [TagAlongItem] int  NOT NULL,
    [TagAlongQuantity] float  NOT NULL,
    [ParentItem] int  NOT NULL,
    [ParentQuantity] float  NOT NULL,
    [BarcodeFormat] smallint  NOT NULL,
    [PriceLowerBound] decimal(19,4)  NOT NULL,
    [PriceUpperBound] decimal(19,4)  NOT NULL,
    [PictureName] nvarchar(50)  NOT NULL,
    [LastSold] datetime  NULL,
    [ExtendedDescription] nvarchar(max)  NOT NULL,
    [SubDescription1] nvarchar(30)  NOT NULL,
    [SubDescription2] nvarchar(30)  NOT NULL,
    [SubDescription3] nvarchar(30)  NOT NULL,
    [UnitOfMeasure] nvarchar(4)  NOT NULL,
    [SubCategoryID] int  NOT NULL,
    [QuantityEntryNotAllowed] bit  NOT NULL,
    [PriceMustBeEntered] bit  NOT NULL,
    [BlockSalesReason] nvarchar(30)  NOT NULL,
    [BlockSalesAfterDate] datetime  NULL,
    [Weight] float  NOT NULL,
    [Taxable] bit  NOT NULL,
    [DBTimeStamp] binary(8)  NULL,
    [BlockSalesBeforeDate] datetime  NULL,
    [LastCost] decimal(19,4)  NOT NULL,
    [ReplacementCost] decimal(19,4)  NOT NULL,
    [WebItem] bit  NOT NULL,
    [BlockSalesType] int  NOT NULL,
    [BlockSalesScheduleID] int  NOT NULL,
    [SaleType] int  NOT NULL,
    [SaleScheduleID] int  NOT NULL,
    [Consignment] bit  NOT NULL,
    [Inactive] bit  NOT NULL,
    [LastCounted] datetime  NULL,
    [DoNotOrder] bit  NOT NULL,
    [MSRP] decimal(19,4)  NOT NULL,
    [DateCreated] datetime  NOT NULL,
    [Content] nvarchar(max)  NOT NULL,
    [UsuallyShip] nvarchar(255)  NOT NULL,
    [NumberFormat] nvarchar(20)  NULL,
    [ItemCannotBeRet] bit  NULL,
    [ItemCannotBeSold] bit  NULL,
    [IsAutogenerated] bit  NULL,
    [IsGlobalvoucher] bit  NOT NULL,
    [DeleteZeroBalanceEntry] bit  NULL,
    [TenderID] int  NOT NULL
);
GO

-- Creating table 'TransactionHold'
CREATE TABLE [dbo].[TransactionHold] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [StoreID] int  NOT NULL,
    [TransactionType] smallint  NOT NULL,
    [HoldComment] nvarchar(255)  NOT NULL,
    [RecallID] int  NOT NULL,
    [Comment] nvarchar(50)  NOT NULL,
    [PriceLevel] smallint  NOT NULL,
    [DiscountMethod] smallint  NOT NULL,
    [DiscountPercent] float  NOT NULL,
    [Taxable] bit  NOT NULL,
    [CustomerID] int  NOT NULL,
    [DeltaDeposit] decimal(19,4)  NOT NULL,
    [DepositOverride] bit  NOT NULL,
    [DepositPrevious] decimal(19,4)  NOT NULL,
    [PaymentsPrevious] decimal(19,4)  NOT NULL,
    [TaxPrevious] decimal(19,4)  NOT NULL,
    [SalesRepID] int  NOT NULL,
    [ShipToID] int  NOT NULL,
    [TransactionTime] datetime  NOT NULL,
    [ExpirationOrDueDate] datetime  NOT NULL,
    [ReturnMode] bit  NOT NULL,
    [ReferenceNumber] nvarchar(50)  NOT NULL,
    [ShippingChargePurchased] decimal(19,4)  NOT NULL,
    [ShippingChargeOverride] bit  NOT NULL,
    [ShippingServiceID] int  NOT NULL,
    [ShippingTrackingNumber] nvarchar(255)  NOT NULL,
    [ShippingNotes] nvarchar(max)  NOT NULL,
    [DBTimeStamp] binary(8)  NULL,
    [ReasonCodeID] int  NOT NULL,
    [ExchangeID] int  NOT NULL,
    [ChannelType] int  NOT NULL,
    [DefaultDiscountReasonCodeID] int  NOT NULL,
    [DefaultReturnReasonCodeID] int  NOT NULL,
    [DefaultTaxChangeReasonCodeID] int  NOT NULL,
    [BatchNumber] int  NOT NULL
);
GO

-- Creating table 'TransactionHoldEntry'
CREATE TABLE [dbo].[TransactionHoldEntry] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [EntryKey] nvarchar(10)  NOT NULL,
    [StoreID] int  NOT NULL,
    [TransactionHoldID] int  NOT NULL,
    [RecallID] int  NOT NULL,
    [Description] nvarchar(30)  NOT NULL,
    [QuantityPurchased] float  NOT NULL,
    [QuantityOnOrder] float  NOT NULL,
    [QuantityRTD] float  NOT NULL,
    [QuantityReserved] float  NOT NULL,
    [Price] decimal(19,4)  NOT NULL,
    [FullPrice] decimal(19,4)  NOT NULL,
    [PriceSource] smallint  NOT NULL,
    [Comment] nvarchar(255)  NOT NULL,
    [DetailID] int  NOT NULL,
    [Taxable] bit  NOT NULL,
    [ItemID] int  NOT NULL,
    [SalesRepID] int  NOT NULL,
    [SerialNumber1] nvarchar(20)  NOT NULL,
    [SerialNumber2] nvarchar(20)  NOT NULL,
    [SerialNumber3] nvarchar(20)  NOT NULL,
    [VoucherNumber] nvarchar(20)  NOT NULL,
    [VoucherExpirationDate] datetime  NULL,
    [DBTimeStamp] binary(8)  NULL,
    [DiscountReasonCodeID] int  NOT NULL,
    [ReturnReasonCodeID] int  NOT NULL,
    [TaxChangeReasonCodeID] int  NOT NULL,
    [ItemTaxID] int  NOT NULL,
    [ComponentQuantityReserved] float  NOT NULL,
    [TransactionTime] datetime  NULL,
    [IsAddMoney] bit  NOT NULL,
    [VoucherID] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [BatchNumber] in table 'Batch'
ALTER TABLE [dbo].[Batch]
ADD CONSTRAINT [PK_Batch]
    PRIMARY KEY CLUSTERED ([BatchNumber] ASC);
GO

-- Creating primary key on [ID] in table 'Item'
ALTER TABLE [dbo].[Item]
ADD CONSTRAINT [PK_Item]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'TransactionHold'
ALTER TABLE [dbo].[TransactionHold]
ADD CONSTRAINT [PK_TransactionHold]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'TransactionHoldEntry'
ALTER TABLE [dbo].[TransactionHoldEntry]
ADD CONSTRAINT [PK_TransactionHoldEntry]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------