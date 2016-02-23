USE [ThereforePREPROD]
GO

/****** Object:  StoredProcedure [dbo].[GetEventIdList]    Script Date: 23/02/2016 14:03:10 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetEventIdList]
@debut nvarchar(8),
@fin nvarchar(8) 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT DISTINCT e.id, e.id_pj 
	FROM SERVSAGILEA.sagilea_db_miroir.dbo.evenementiel e
	INNER JOIN SERVSAGILEA.sagilea_db_miroir.dbo.wizard_type_evt_champs w ON w.type_evt = e.type_evt
	WHERE (e.media = 'T' OR e.media = 'I') AND e.date_cloture is not null 
	AND e.date_evt >= @debut AND e.date_evt <= @fin

END


GO


