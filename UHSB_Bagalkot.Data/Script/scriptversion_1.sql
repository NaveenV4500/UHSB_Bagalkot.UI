 

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


 

