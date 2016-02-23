USE [ThereforePREPROD]
GO

/****** Object:  StoredProcedure [dbo].[EditMigrationProgress]    Script Date: 23/02/2016 15:07:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[EditMigrationProgress] 
	-- Add the parameters for the stored procedure here
	@evenement int, 
	@etat smallint = 0,
	@etape nvarchar(50),
	@duree nvarchar(50),
	@message nvarchar(4000)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN tran
		UPDATE ThereforePROD.dbo.CIPRES_Migration
		SET etat = @etat
			,etape = @etape
			,duree = @duree
			,message = @message
			,date_modification = SYSDATETIME()
		WHERE evt_id = @evenement

		IF @@ROWCOUNT = 0
			BEGIN
				INSERT ThereforePROD.dbo.CIPRES_Migration (evt_id, etat, etape, duree, message, date_creation)
				VALUES (@evenement, @etat, @etape, @duree, @message, SYSDATETIME())
			END
	COMMIT tran
END


GO


