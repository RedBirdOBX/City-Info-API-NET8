/****** Object:  Table [dbo].[PointsOfInterest]    Script Date: 2/19/2025 7:33:40 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PointsOfInterest](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PointGuid] [uniqueidentifier] NOT NULL,
	[CityId] [int] NOT NULL,
	[CityGuid] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](200) NULL,
	[CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_PointsOfInterest] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PointsOfInterest] ADD  CONSTRAINT [DF_PointsOfInterest_CityId]  DEFAULT ((0)) FOR [CityId]
GO


