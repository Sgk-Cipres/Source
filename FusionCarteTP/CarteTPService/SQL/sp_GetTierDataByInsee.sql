USE [Batch]
GO

/****** Object:  StoredProcedure [dbo].[GetTierDataByInsee]    Script Date: 04/03/2016 16:26:43 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetTierDataByInsee]
	-- Add the parameters for the stored procedure here
	@insee nvarchar(13)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT 
		1 as wkf
		,1 as reprise
		,'W99' as thereflow
		,'Envoi carte Tiers Payant' as type_doc
		,'ASSURE' as type_tiers
		,TiersId as id_tiers
		,Nom + ' ' + Prenom AS identite
		,'Courrier' as media
		,'S' as sens
		,'Traité' as statu_lib
		, 0 as ActDeces
		, 0 as ActNaiss
		, 0 as AdjRad
		, 0 as AttestVit
		, 0 as ChgtAdress
		, 0 as RIB_Cot
		, 0 as RIB_Pres
		, 0 as AttestPE
		, 0 as CertConjt
		, 0 as CertScol
		, 0 as Tele
	FROM ListeAssureBeneficiaires
	WHERE NumeroSecu = @insee;
END


GO


