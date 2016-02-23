USE [ThereforePREPROD]
GO

/****** Object:  Table [dbo].[CIPRES_Type_Event_Sagilea]    Script Date: 23/02/2016 14:59:25 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CIPRES_Type_Event_Sagilea](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Id_type_evt_sagilea] [int] NOT NULL,
	[Libelle_type_evt_sagilea] [nvarchar](100) NOT NULL,
	[Workflow_therefore] [nvarchar](50) NULL,
	[Libelle_type_doc_therefore] [nvarchar](50) NULL,
	[ActDeces] [smallint] NULL,
	[ActNaiss] [smallint] NULL,
	[AdjRad] [smallint] NULL,
	[AttestVit] [smallint] NULL,
	[ChgtAdress] [smallint] NULL,
	[RIB_Cot] [smallint] NULL,
	[RIB_Pres] [smallint] NULL,
	[AttestPE] [smallint] NULL,
	[CertConjt] [smallint] NULL,
	[CertScol] [smallint] NULL,
	[Famille_therefore] [nvarchar](50) NULL,
	[Tele] [smallint] NULL CONSTRAINT [DF_CIPRES_Type_Event_Sagilea_Tele]  DEFAULT ((0))
) ON [PRIMARY]

GO


