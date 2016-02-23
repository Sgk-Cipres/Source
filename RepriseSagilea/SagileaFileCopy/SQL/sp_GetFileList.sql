USE [ThereforePROD]
GO

/****** Object:  StoredProcedure [dbo].[GetFileList]    Script Date: 23/02/2016 14:56:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetFileList]
	@debut nvarchar(8),
	@fin nvarchar(8) 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT DISTINCT e.id, e.no_tiers, e.date_evt, e.id_pj 
	FROM SERVSAGILEA.sagilea_db.dbo.evenementiel e
	WHERE e.Type_Evt = 2 AND e.User_Creation = 100 
	AND e.date_evt >= @debut AND e.date_evt <= @fin
END


GO


