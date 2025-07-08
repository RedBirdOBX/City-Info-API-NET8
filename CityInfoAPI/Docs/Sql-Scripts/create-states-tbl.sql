/****** Object:  Table [dbo].[States]    Script Date: 6/25/2025 7:41:39 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[States](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StateGuid] [uniqueidentifier] NOT NULL,
	[StateCode] [char](2) NOT NULL,
	[Name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_States] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[States] ADD  CONSTRAINT [DF_States_StateGuid]  DEFAULT (newid()) FOR [StateGuid]
GO


