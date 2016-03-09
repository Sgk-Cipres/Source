USE [Batch]
GO

/****** Object:  Table [dbo].[CartesLog]    Script Date: 04/03/2016 16:36:57 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CartesLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdTiersAssure] [int] NULL,
	[NumCartes] [nvarchar](50) NOT NULL,
	[DateEdition] [date] NOT NULL,
	[RefPli] [nvarchar](20) NOT NULL,
	[FichierSource] [nvarchar](max) NOT NULL,
	[PageSource] [smallint] NULL,
	[DateTraitement] [datetime] NULL,
	[EtatTraitement] [smallint] NOT NULL,
	[MsgTraitement] [nvarchar](max) NULL,
	[FichierEnveloppe] [nvarchar](max) NULL,
	[FichierIntermediaire] [nvarchar](max) NULL,
	[FichierXml] [nvarchar](max) NULL,
 CONSTRAINT [PK_CartesLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[CartesLog] ADD  CONSTRAINT [DF_Table_1_TraitementEtat]  DEFAULT ((0)) FOR [EtatTraitement]
GO


