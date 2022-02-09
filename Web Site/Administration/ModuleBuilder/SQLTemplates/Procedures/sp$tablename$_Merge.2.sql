if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'sp$tablename$_Merge' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.sp$tablename$_Merge;
GO


-- Copyright (C) 2006 SplendidCRM Software, Inc. All rights reserved.
-- NOTICE: This code has not been licensed under any public license.
Create Procedure dbo.sp$tablename$_Merge
	( @ID               uniqueidentifier
	, @MODIFIED_USER_ID uniqueidentifier
	, @MERGE_ID         uniqueidentifier
	)
as
  begin
	set nocount on

$mergeupdaterelationship$

	exec dbo.spPARENT_Merge @ID, @MODIFIED_USER_ID, @MERGE_ID;
	
	exec dbo.sp$tablename$_Delete @MERGE_ID, @MODIFIED_USER_ID;
  end
GO

Grant Execute on dbo.sp$tablename$_Merge to public;
GO

