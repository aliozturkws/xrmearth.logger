USE [DATABASE_NAME]

/****** Object:  Table [dbo].[Application]    Script Date: 25.10.2017 14:34:34 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Application](
	[ID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Namespace] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[AssemblyGuid] [uniqueidentifier] NOT NULL,
	[AssemblyVersion] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO



CREATE TABLE [dbo].[ApplicationInstance](
	[ID] [uniqueidentifier] NOT NULL,
	[ApplicationID] [uniqueidentifier] NOT NULL,
	[Path] [nvarchar](max) NULL,
	[StartAt] [datetime] NOT NULL,
	[FinishAt] [datetime] NULL,
	[Parameters] [nvarchar](max) NULL,
	[Result] [nvarchar](max) NULL,
	[Summary] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[ApplicationInstance]  WITH CHECK ADD FOREIGN KEY([ApplicationID])
REFERENCES [dbo].[Application] ([ID])
GO



CREATE TABLE [dbo].[ApplicationLog](
	[ID] [uniqueidentifier] NOT NULL,
	[ApplicationInstanceID] [uniqueidentifier] NOT NULL,
	[ParentCallerMember] [nvarchar](max) NULL,
	[CallerMember] [nvarchar](max) NULL,
	[Message] [nvarchar](max) NULL,
	[Type] [int] NOT NULL,
	[LogLevel] [int] NULL,
	[Tag1] [nvarchar](max) NULL,
	[Tag2] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[ApplicationLog]  WITH CHECK ADD FOREIGN KEY([ApplicationInstanceID])
REFERENCES [dbo].[ApplicationInstance] ([ID])
GO


