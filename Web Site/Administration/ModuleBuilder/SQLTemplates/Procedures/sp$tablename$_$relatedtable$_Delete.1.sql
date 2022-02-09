if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'sp$tablename$_$relatedtable$_Delete' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.sp$tablename$_$relatedtable$_Delete;
GO


Create Procedure dbo.sp$tablename$_$relatedtable$_Delete
	( @MODIFIED_USER_ID          uniqueidentifier
	, @$tablenamesingular$_ID    uniqueidentifier
	, @$relatedtablesingular$_ID uniqueidentifier
	)
as
  begin
	set nocount on
	
	update $tablename$_$relatedtable$
	   set DELETED          = 1
	     , DATE_MODIFIED    = getdate()
	     , DATE_MODIFIED_UTC= getutcdate()
	     , MODIFIED_USER_ID = @MODIFIED_USER_ID
	 where $tablenamesingular$_ID    = @$tablenamesingular$_ID
	   and $relatedtablesingular$_ID = @$relatedtablesingular$_ID
	   and DELETED          = 0;
  end
GO

Grant Execute on dbo.sp$tablename$_$relatedtable$_Delete to public;
GO

