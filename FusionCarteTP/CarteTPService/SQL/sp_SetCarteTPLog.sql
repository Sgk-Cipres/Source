USE [Batch]
GO

/****** Object:  StoredProcedure [dbo].[SetCarteTPLog]    Script Date: 04/03/2016 16:27:48 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SetCarteTPLog] 
	-- Add the parameters for the stored procedure here
	@cartes nvarchar(50)
	, @pli nvarchar(20)
	, @tiers int
	, @source nvarchar(max)
	, @page smallint
	, @edition date
	, @etat smallint
	, @message nvarchar(max)
	, @enveloppe nvarchar(max)
	, @intermediaire nvarchar(max)
	, @xml nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	BEGIN tran
		UPDATE dbo.CartesLog
		SET IdTiersAssure = @tiers
			,FichierSource = @source
			,PageSource = @page
			,DateEdition = @edition
			,EtatTraitement = @etat
			,MsgTraitement = @message
			,DateTraitement = SYSDATETIME()
			,FichierEnveloppe = @enveloppe
			,FichierIntermediaire = @intermediaire
			,FichierXml = @xml
		WHERE NumCartes = @cartes
		AND RefPli = @pli

		IF @@ROWCOUNT = 0
			BEGIN
				INSERT dbo.CartesLog (
							NumCartes
							, RefPli
							, IdTiersAssure
							, FichierSource
							, PageSource
							, DateEdition
							, EtatTraitement
							, MsgTraitement
							, FichierEnveloppe
							, FichierIntermediaire
							, FichierXml
							, DateTraitement)
				VALUES (
							@cartes
							, @pli
							, @tiers
							, @source
							, @page
							, @edition
							, @etat
							, @message
							, @enveloppe
							, @intermediaire
							, @xml
							, SYSDATETIME())
			END
	COMMIT tran
END

GO


