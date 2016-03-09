USE [Batch]
GO

/****** Object:  StoredProcedure [dbo].[ListerPdfLot]    Script Date: 09/03/2016 14:43:04 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[ListerPdfLot]

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT * FROM dbo.V_LISTE_LOT_CARTES_TP v 

END




GO


