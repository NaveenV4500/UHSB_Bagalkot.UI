 

  Alter table UserMaster
  add [Address] [nvarchar](100) NULL,
	  [LandSize] [decimal](10, 2) NULL,
      [EmployeeID] int NULL,
      [EmailID] nvarchar(150) NULL

  Go
  ALTER TABLE [dbo].[UHSB_ItemImages]
ADD CreatedDate DATETIME DEFAULT GETDATE(),
    ModifiedDate DATETIME NULL,
	CreatedBy int,
	ModifiedBy int;

Go
 

CREATE TABLE [dbo].[UHSB_SeedPlantingCenterMaster](
	[CenterId] [int] NOT NULL,
	[DistrictId] [int] NULL,
	[Centername_eng] [varchar](100) NULL,
	[Centername_knd] [nvarchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[CenterId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[UHSB_SeedPlantingCenterMaster]  WITH CHECK ADD  CONSTRAINT [FK_UHSB_SeedPlantingCenterMaster_Districts] FOREIGN KEY([DistrictId])
REFERENCES [dbo].[UHSB_Districts] ([DistrictId])
GO

ALTER TABLE [dbo].[UHSB_SeedPlantingCenterMaster] CHECK CONSTRAINT [FK_UHSB_SeedPlantingCenterMaster_Districts]
GO


 

CREATE TABLE [dbo].[UHSB_RecordHeadMaster](
	[HeadId] [int] NOT NULL,
	[RecordHead_eng] [varchar](100) NULL,
	[RecordHead_knd] [nvarchar](200) NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[HeadId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


 

CREATE TABLE [dbo].[UHSB_AvailabilityToolsDetails](
	[identifier] [int] IDENTITY(1,1) NOT NULL,
	[CenterId] [int] NOT NULL,
	[HeadId] [int] NOT NULL,
	[AvailToolname_eng] [varchar](100) NULL,
	[AvailToolname_knd] [nvarchar](200) NULL,
	[Quantity] [int] NULL,
	[Unit] [varchar](20) NULL,
	[Price] [decimal](18, 2) NULL,
	[AvailabilityDate] [date] NULL,
	[Remarks] [varchar](250) NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_UHSB_AvailabilityToolsDetails] PRIMARY KEY CLUSTERED 
(
	[identifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[UHSB_AvailabilityToolsDetails] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO

ALTER TABLE [dbo].[UHSB_AvailabilityToolsDetails]  WITH CHECK ADD  CONSTRAINT [FK_UHSB_AvailabilityToolsDetails_UHSB_RecordHeadMaster] FOREIGN KEY([HeadId])
REFERENCES [dbo].[UHSB_RecordHeadMaster] ([HeadId])
GO

ALTER TABLE [dbo].[UHSB_AvailabilityToolsDetails] CHECK CONSTRAINT [FK_UHSB_AvailabilityToolsDetails_UHSB_RecordHeadMaster]
GO

ALTER TABLE [dbo].[UHSB_AvailabilityToolsDetails]  WITH CHECK ADD  CONSTRAINT [FK_UHSB_AvailabilityToolsDetails_UHSB_SeedPlantingCenterMaster] FOREIGN KEY([CenterId])
REFERENCES [dbo].[UHSB_SeedPlantingCenterMaster] ([CenterId])
GO

ALTER TABLE [dbo].[UHSB_AvailabilityToolsDetails] CHECK CONSTRAINT [FK_UHSB_AvailabilityToolsDetails_UHSB_SeedPlantingCenterMaster]
GO
 
--exec USP_GetAvailabilityToolsDetails 1,1,1

CREATE PROCEDURE [dbo].[USP_GetAvailabilityToolsDetails]
    @DistrictId INT = 0,
    @CenterId   INT = 0,
	@pagetype int=0
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH CTE_Tools AS
    (
        SELECT 
            ATD.identifier,
            ATD.CenterId,
            SCM.Centername_eng,
            SCM.Centername_knd,
            SCM.DistrictId,
            ATD.HeadId,
            RHM.RecordHead_eng,
            RHM.RecordHead_knd,
            ATD.AvailToolname_eng,
            ATD.AvailToolname_knd,
            ATD.Quantity,
            ATD.Unit,
            ATD.Price,
            ATD.AvailabilityDate,
            ATD.Remarks,
            ATD.CreatedBy,
            ATD.CreatedDate,
            ATD.ModifiedBy,
            ATD.ModifiedDate
        FROM 
            UHSB_AvailabilityToolsDetails ATD
            INNER JOIN UHSB_SeedPlantingCenterMaster SCM 
                ON ATD.CenterId = SCM.CenterId
            INNER JOIN UHSB_RecordHeadMaster RHM 
                ON ATD.HeadId = RHM.HeadId
			where   (@pagetype = 0 OR ATD.HeadID = @pagetype)
    )
    SELECT *
    FROM CTE_Tools
    WHERE 
        (@DistrictId = 0 OR DistrictId = @DistrictId)
        AND (@CenterId = 0 OR CenterId = @CenterId)
    ORDER BY 
        CreatedDate DESC;
END
GO
 

 --=============================== 23-11-2025 ==============================
  alter table UHSB_RecordHeadMaster
  add DataType int 
  Go
  update UHSB_RecordHeadMaster set DataType=1
  --============================== 23-11-2025 ==============================
  CREATE TABLE [dbo].[UserOtp] (
    [OtpId] INT IDENTITY(1,1) PRIMARY KEY,
    [UserId] INT NOT NULL,
    [OTP] INT NOT NULL,
    [ExpiryTime] DATETIME NOT NULL,
    [IsUsed] BIT NOT NULL DEFAULT(0),
    [CreatedOn] DATETIME NOT NULL DEFAULT(GETDATE())
);
ALTER TABLE [dbo].[UserOtp]
ADD CONSTRAINT FK_UserOtp_usermaster
FOREIGN KEY (UserId) REFERENCES usermaster(Id);


--25-12-2025  Product tables
 CREATE TABLE dbo.UHSB_UnitMaster (
    UnitId INT IDENTITY(1,1) PRIMARY KEY,
    UnitName_eng VARCHAR(50) NOT NULL,
    UnitName_knd NVARCHAR(100) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1
);
GO



CREATE TABLE [dbo].[UHSB_Products](
	[identifier] [int] IDENTITY(1,1) NOT NULL,
	[CenterId] [int] NOT NULL,
	[HeadId] [int] NOT NULL,
	[ProductName_eng] [varchar](100) NULL,
	[ProductName_knd] [nvarchar](200) NULL,
	[Quantity] [int] NULL,
	[UnitId] [int] NULL,
	[Price] [decimal](18, 2) NULL,
	[AvailabilityDate] [date] NULL,
	[Remarks] [varchar](250) NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_UHSB_Products] PRIMARY KEY CLUSTERED 
(
	[identifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[UHSB_Products] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO

ALTER TABLE [dbo].[UHSB_Products]  WITH CHECK ADD  CONSTRAINT [FK_UHSB_Products_UHSB_RecordHeadMaster] FOREIGN KEY([HeadId])
REFERENCES [dbo].[UHSB_RecordHeadMaster] ([HeadId])
GO

ALTER TABLE [dbo].[UHSB_Products] CHECK CONSTRAINT [FK_UHSB_Products_UHSB_RecordHeadMaster]
GO

ALTER TABLE [dbo].[UHSB_Products]  WITH CHECK ADD  CONSTRAINT [FK_UHSB_Products_UHSB_SeedPlantingCenterMaster] FOREIGN KEY([CenterId])
REFERENCES [dbo].[UHSB_SeedPlantingCenterMaster] ([CenterId])
GO

ALTER TABLE [dbo].[UHSB_Products] CHECK CONSTRAINT [FK_UHSB_Products_UHSB_SeedPlantingCenterMaster]
GO

ALTER TABLE [dbo].[UHSB_Products]  WITH CHECK ADD  CONSTRAINT [FK_UHSB_Products_UnitMaster] FOREIGN KEY([UnitId])
REFERENCES [dbo].[UHSB_UnitMaster] ([UnitId])
GO

ALTER TABLE [dbo].[UHSB_Products] CHECK CONSTRAINT [FK_UHSB_Products_UnitMaster]
GO

CREATE TABLE dbo.UHSB_ProductPriceHistory (
    PriceHistoryId INT IDENTITY(1,1) PRIMARY KEY,
    ProductId INT NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    EffectiveFrom DATE NOT NULL,
    EffectiveTo DATE NULL,
    CreatedBy INT NULL,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    Remarks NVARCHAR(250),
    CONSTRAINT FK_PriceHistory_Product
    FOREIGN KEY (ProductId)
    REFERENCES dbo.UHSB_Products(identifier)
);


-- ===============================2026======================== 
CREATE TABLE [dbo].[UHSB_OrderMaster](
    [OrderId]        INT IDENTITY(1,1) NOT NULL,
    [UserId]         INT NOT NULL,
    [OrderNumber]    NVARCHAR(50) NOT NULL,
    [OrderDate]      smalldatetime NOT NULL,
    [TotalAmount]    DECIMAL(18,2) NOT NULL,
	OrderDataStatusType tinyint not null, 
    [OrderStatus]    NVARCHAR(30) NOT NULL, -- Pending, Paid, Cancelled, Delivered
    [PaymentStatus]  NVARCHAR(30) NOT NULL, -- Pending, Success, Failed
    [CreatedDate]    smalldatetime NOT NULL,
	CreateBy int  not null,
	ModifiedDate smalldatetime not null,
	ModifiedBy int  not null,

    CONSTRAINT [PK_UHSB_OrderMaster] PRIMARY KEY CLUSTERED ([OrderId])
);
GO

ALTER TABLE [dbo].[UHSB_OrderMaster]
ADD DEFAULT (GETDATE()) FOR [OrderDate];

ALTER TABLE [dbo].[UHSB_OrderMaster]
ADD DEFAULT (GETDATE()) FOR [CreatedDate];

ALTER TABLE [dbo].[UHSB_OrderMaster]
ADD CONSTRAINT [FK_UHSB_OrderMaster_UserMaster]
FOREIGN KEY ([UserId]) REFERENCES [dbo].[UserMaster]([Id]);
GO

 

CREATE TABLE [dbo].[UHSB_OrderItems](
    [OrderItemId] INT IDENTITY(1,1) NOT NULL,
    [OrderId]     INT NOT NULL,
    [ProductId]   INT NOT NULL,
    [VarietyId]   INT NOT NULL,
    [Quantity]    INT NOT NULL,
    [Price]       DECIMAL(18,2) NOT NULL,
    [TotalPrice]  decimal (18,2) Not Null,
	[CreatedDate]    smalldatetime NOT NULL,
	CreateBy int  not null,
	ModifiedDate smalldatetime not null,
	ModifiedBy int  not null,
    CONSTRAINT [PK_UHSB_OrderItems] PRIMARY KEY CLUSTERED ([OrderItemId])
);
GO

ALTER TABLE [dbo].[UHSB_OrderItems]
ADD CONSTRAINT [FK_UHSB_OrderItems_OrderMaster]
FOREIGN KEY ([OrderId]) REFERENCES [dbo].[UHSB_OrderMaster]([OrderId]);
GO
ALTER TABLE [dbo].[UHSB_OrderItems]
ADD DEFAULT (GETDATE()) FOR [CreatedDate];