USE [ThereforePREPROD]
GO

/****** Object:  View [dbo].[V_CIPRES_REPRISE]    Script Date: 23/02/2016 15:03:28 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[V_CIPRES_REPRISE]
AS
SELECT        1 AS wkf, 1 AS reprise, CASE WHEN TES.Workflow_therefore IS NULL THEN 'W99' ELSE TES.Workflow_therefore END AS thereflow, 
                         TES.Libelle_type_doc_therefore AS type_doc, E.id AS evt_id, TE.Libelle AS evt_lib, TES.Famille_therefore AS famille_lib, E.id_parent AS evt_id_parent, 
                         E.no_tiers AS id_tiers_sagilea, T.pk_tiers AS id_tiers, S_T.nom + ' ' + S_T.prenom AS identite, TT.code_type_tiers AS type_tiers, E.idDestinataire AS id_dest_sagilea, 
                         TD.pk_tiers AS id_dest, E.id_pj AS no_doc, E.Date_Evt AS date_evt_crea, U1.Nom + ' ' + U1.Prenom AS gest_crea, E.date_cloture AS date_evt_clot, 
                         U2.Nom + ' ' + U2.Prenom AS gest_clot, CASE WHEN E.media = 'P' OR
                         E.media = 'S' THEN 'Appli Mobile' WHEN E.media = 'T' THEN 'Note Téléphonique' ELSE RTRIM(TM.libelle) END AS media_lib, E.sens, 
                         CASE WHEN TS .Libelle = 'Traité' THEN 'Traité' ELSE 'En cours' END AS statu_lib, E.Commentaire AS evt_commentaire, 'Evènement créé le ' + CONVERT(VARCHAR, 
                         E.Date_Evt, 120) + CHAR(13) + CASE ISNULL(CONVERT(VARCHAR, E.date_cloture, 120), '') WHEN '' THEN ' ' ELSE 'Evènement cloturé le ' + CONVERT(VARCHAR, 
                         E.date_cloture, 120) + ' par ' + U2.Nom + ' ' + U2.Prenom + CHAR(13) END + ISNULL(E.Commentaire, '') AS commentaire, E.fichier_original, TES.ActDeces, 
                         TES.ActNaiss, TES.AdjRad, TES.AttestVit, TES.ChgtAdress, TES.RIB_Cot, TES.RIB_Pres, TES.AttestPE, TES.CertConjt, TES.CertScol, TES.Tele
FROM            SERVSAGILEA.sagilea_db_miroir.dbo.evenementiel AS E INNER JOIN
                         SERVSAGILEA.sagilea_db_miroir.dbo.type_evenement AS TE ON TE.id = E.Type_Evt LEFT OUTER JOIN
                         SERVSAGILEA.sagilea_db_miroir.dbo.tiers AS S_T ON S_T.id = E.no_tiers INNER JOIN
                         SERVSAGILEA.sagilea_db_miroir.dbo.type_famille_evenement AS TFE ON TFE.id = TE.Famille INNER JOIN
                         SERVSAGILEA.sagilea_db_miroir.dbo.type_media AS TM ON TM.id = E.media LEFT OUTER JOIN
                         SERVSAGILEA.sagilea_db_miroir.dbo.users AS U1 ON U1.Id = E.User_Creation LEFT OUTER JOIN
                         SERVSAGILEA.sagilea_db_miroir.dbo.users AS U2 ON U2.Id = E.user_cloture INNER JOIN
                         SERVSAGILEA.sagilea_db_miroir.dbo.type_statut AS TS ON TS.id = E.Statut INNER JOIN
                         SERVREF01.cipres_referentiel.dbo.tbl_tiers AS T ON T.id_sagilea = CONVERT(NVARCHAR(30), E.no_tiers) INNER JOIN
                         SERVREF01.cipres_referentiel.dbo.tbl_tiers AS TD ON TD.id_sagilea = CONVERT(NVARCHAR(30), E.idDestinataire) INNER JOIN
                         SERVREF01.cipres_referentiel.dbo.ref_type_tiers AS TT ON TT.pk_type_tiers = T.fk_type_tiers LEFT OUTER JOIN
                         ThereforePROD.dbo.CIPRES_Type_Event_Sagilea AS TES ON TES.Id_type_evt_sagilea = E.Type_Evt LEFT OUTER JOIN
                         ThereforePROD.dbo.CIPRES_Migration AS CM ON CM.evt_id = E.id
WHERE        (E.Date_Evt >= CAST('20130701' AS date)) AND (E.Date_Evt <= CAST('20130731' AS date)) AND (E.date_cloture IS NOT NULL)

GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[53] 4[17] 2[19] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "E"
            Begin Extent = 
               Top = 6
               Left = 463
               Bottom = 135
               Right = 675
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "TE"
            Begin Extent = 
               Top = 138
               Left = 515
               Bottom = 267
               Right = 708
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "S_T"
            Begin Extent = 
               Top = 162
               Left = 38
               Bottom = 291
               Right = 241
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "TFE"
            Begin Extent = 
               Top = 270
               Left = 515
               Bottom = 399
               Right = 685
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "TM"
            Begin Extent = 
               Top = 294
               Left = 38
               Bottom = 423
               Right = 208
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "U1"
            Begin Extent = 
               Top = 402
               Left = 471
               Bottom = 531
               Right = 641
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "U2"
            Begin Extent = 
               Top = 402
               Left = 679
               Bottom = 531
               Right = 849
            End
            DisplayFlags = 280
            TopColumn = 0
   ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_CIPRES_REPRISE'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'      End
         Begin Table = "TS"
            Begin Extent = 
               Top = 426
               Left = 38
               Bottom = 555
               Right = 272
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "T"
            Begin Extent = 
               Top = 12
               Left = 218
               Bottom = 141
               Right = 425
            End
            DisplayFlags = 280
            TopColumn = 2
         End
         Begin Table = "TD"
            Begin Extent = 
               Top = 143
               Left = 805
               Bottom = 272
               Right = 1012
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "TT"
            Begin Extent = 
               Top = 38
               Left = 14
               Bottom = 161
               Right = 184
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "TES"
            Begin Extent = 
               Top = 144
               Left = 248
               Bottom = 273
               Right = 477
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "CM"
            Begin Extent = 
               Top = 276
               Left = 248
               Bottom = 405
               Right = 433
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 22
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 3105
         Alias = 2355
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_CIPRES_REPRISE'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_CIPRES_REPRISE'
GO


