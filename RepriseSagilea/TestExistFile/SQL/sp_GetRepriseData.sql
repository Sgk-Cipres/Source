USE [ThereforePREPROD]
GO

/****** Object:  StoredProcedure [dbo].[GetRepriseData]    Script Date: 23/02/2016 15:09:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetRepriseData]
	-- Add the parameters for the stored procedure here
	@mode nvarchar(8), 
	@debut nvarchar(8),
	@fin nvarchar(8)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF @mode = 'view'
		BEGIN
			SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
			SELECT * FROM dbo.V_CIPRES_REPRISE v 
			WHERE v.date_evt_crea >= @debut AND v.date_evt_crea <= @fin
		END
	ELSE
		BEGIN
			SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
			SELECT * FROM dbo.CIPRES_Sagilea_ViewData v 
			WHERE v.date_evt_crea >= @debut AND v.date_evt_crea <= @fin
		END

END


GO


