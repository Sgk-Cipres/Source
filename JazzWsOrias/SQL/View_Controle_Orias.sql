USE [app_db]
GO

/****** Object:  View [dbo].[View_Controle_Orias]    Script Date: 25/06/2015 18:05:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*ORDER BY Courtiers.[Code Courtier]*/
CREATE VIEW [dbo].[Jazz_View_Controle_Orias]
AS
SELECT        dbo.Courtiers.[Code Courtier], dbo.Courtiers.[Nom Commun], dbo.Courtiers.[Nom Courtier], dbo.ORIAS.[num orias] AS Orias, dbo.Courtiers.SIRET, 
                         dbo.Courtiers.Siret2 AS [Affilié Siret], dbo.Courtiers.Orias2 AS [Affilié Orias], [Cat. principale].libelle_categorie AS Principale, 
                         [Cat. Accessoire].libelle_categorie AS Accessoire
FROM            dbo.[Types Categories Courtier] AS [Cat. Accessoire] RIGHT OUTER JOIN
                         dbo.ORIAS ON [Cat. Accessoire].id_categorie = dbo.ORIAS.Statut_Accessoire LEFT OUTER JOIN
                         dbo.[Types Categories Courtier] AS [Cat. principale] ON dbo.ORIAS.Statut_Principal = [Cat. principale].id_categorie FULL OUTER JOIN
                         dbo.Courtiers ON dbo.ORIAS.[code courtier] = dbo.Courtiers.[Code Courtier]
WHERE        (dbo.ORIAS.[date effet] > '20141231') OR
                         (dbo.ORIAS.[date fin] IS NULL)

GO
