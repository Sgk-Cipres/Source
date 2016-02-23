USE [ThereforePREPROD]
GO

/****** Object:  UserDefinedFunction [dbo].[GetTiersIdentites]    Script Date: 23/02/2016 14:09:33 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[GetTiersIdentites]
(	
	-- Add the parameters for the function here
	@list_id nvarchar(MAX)
)
RETURNS NVARCHAR(MAX) 
AS
BEGIN
DECLARE @s nvarchar(MAX)

	IF @list_id is not null
		
		SET @s = STUFF(
						(SELECT ',' + nom + ' ' + prenom AS tiers 
						FROM SERVSAGILEA.sagilea_db_miroir.dbo.tiers 
						WHERE id IN 
							(SELECT CAST(part as int) 
							FROM SDF_SplitString(@list_id, ',')
							)
						FOR XML PATH(''), TYPE).value('.', 'nvarchar(max)'), 1, 1, '')
	ELSE
		SET @s = ''

	RETURN @s
END

GO


