USE [ThereforePREPROD]
GO

/****** Object:  Table [dbo].[CIPRES_Migration]    Script Date: 23/02/2016 14:58:14 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CIPRES_Migration](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[evt_id] [int] NOT NULL,
	[etat] [smallint] NOT NULL,
	[etape] [nvarchar](50) NULL,
	[duree] [nvarchar](50) NULL,
	[message] [nvarchar](4000) NULL,
	[date_creation] [datetime] NULL,
	[date_modification] [datetime] NULL,
 CONSTRAINT [PK_CIPRES_Migration] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'1 = execute, 0 = non execute et -1 = erreur' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CIPRES_Migration', @level2type=N'COLUMN',@level2name=N'etat'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'table qui contien l''avancement des lignes traitées lors de la migration Sagilea' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CIPRES_Migration'
GO


