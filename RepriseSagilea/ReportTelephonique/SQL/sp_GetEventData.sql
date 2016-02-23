USE [ThereforePREPROD]
GO

/****** Object:  StoredProcedure [dbo].[GetEventData]    Script Date: 23/02/2016 14:00:55 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetEventData] 
	-- Add the parameters for the stored procedure here
	@Id int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT
	e.id
	, e.id_parent AS idparent
	, tf.libelle AS famille 
	, media = CASE 
	WHEN e.media = 'T' THEN
	 'Appel'
	ELSE
	 'Interne'
	END
	, (SELECT libelle FROM SERVSAGILEA.sagilea_db_miroir.dbo.type_appelant WHERE id = e.appelant) AS appelant
	, CASE WHEN t.noss is null THEN 'RCS' ELSE 'N° SS' END AS ss_rcs_lib
	, ((SELECT c.libelle FROM SERVSAGILEA.sagilea_db_miroir.dbo.type_civilite c WHERE c.id = t.civilité) 
		+ ' ' + t.nom + ' ' + ISNULL(t.prenom, '') + 
		CASE WHEN t.noss is null THEN ' (' + ISNULL(t.rcs, 'non renseignée') +')' ELSE ' (' + t.noss + ')' END 
		) AS assure 
	, sens = CASE 
	WHEN e.sens = 'E' THEN
	 'Entrant'
	WHEN e.sens = 'S' THEN
	 'Sortant'
	ELSE
	 ''
	END
	, te.Libelle AS scenario
	, e.Date_Evt AS datecreation
	, e.date_cloture AS datecloture
	, (SELECT nom + ' ' + prenom FROM SERVSAGILEA.sagilea_db_miroir.dbo.users WHERE id = e.User_Creation) AS utilcreation
	, (SELECT nom + ' ' + prenom FROM SERVSAGILEA.sagilea_db_miroir.dbo.users WHERE id = e.user_cloture) AS utilcloture 
	, e.id_pj AS fichier
	, e.Commentaire
	, ISNULL(wc.LibelleEditique, wc.Libelle) AS libelle 
	, valeur = CASE 
	WHEN wc.InputType = 9 THEN
	(SELECT dbo.GetTiersIdentites(ed.value))
	WHEN wc.DataSource = 4 THEN 
	(SELECT nom + ' ' + prenom FROM SERVSAGILEA.sagilea_db_miroir.dbo.users WHERE Id = ed.value) 
	WHEN wc.DataSource = 1 or wc.DataSource = 3 THEN 
	(SELECT nom + ' ' + prenom FROM SERVSAGILEA.sagilea_db_miroir.dbo.tiers WHERE Id = ed.value) 
	WHEN wc.InputType = 3 THEN 
	wclv.libelle 
	ELSE ISNULL(ed.value, 'Non renseigné par le gestionnaire') 
	END 
	from SERVSAGILEA.sagilea_db_miroir.dbo.evenementiel e 
	inner join SERVSAGILEA.sagilea_db_miroir.dbo.type_evenement te on te.id = e.Type_Evt 
	inner join SERVSAGILEA.sagilea_db_miroir.dbo.wizard_type_evt_champs wtec on wtec.type_evt = te.id 
	inner join SERVSAGILEA.sagilea_db_miroir.dbo.type_famille_evenement tf on tf.id = te.Famille 
	inner join SERVSAGILEA.sagilea_db_miroir.dbo.wizard_champs wc on wc.id = wtec.champs
	inner join SERVSAGILEA.sagilea_db_miroir.dbo.tiers t on t.id = e.no_tiers
	left join SERVSAGILEA.sagilea_db_miroir.dbo.evenements_data ed on ed.NoEvenement = e.id and ed.champ = wc.id 
	left join SERVSAGILEA.sagilea_db_miroir.dbo.wizard_custom_list_values wclv on wclv.champ = ed.champ and CAST(wclv.id AS nvarchar) = ed.value 
	where e.id = @id
	--and wc.InputType != 9 
	group by 
	e.id 
	, e.id_parent
	, e.no_tiers
	, e.idDestinataire
	, e.appelant
	, e.media 
	, e.sens 
	, e.Date_Evt
	, e.date_cloture
	, e.User_Creation
	, e.user_cloture
	, e.id_pj 
	, e.commentaire
	, t.civilité
	, t.noss
	, t.rcs
	, t.nom
	, t.prenom
	, te.Libelle 
	, tf.libelle 
	, wc.LibelleEditique 
	, wc.Libelle 
	, ed.value 
	, wc.InputType 
	, wc.DataSource 
	, wclv.libelle 
	, wtec.ordre
	order by ordre;
END



GO


