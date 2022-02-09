if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'sp$tablename$_Delete' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.sp$tablename$_Delete;
GO


Create Procedure dbo.sp$tablename$_Delete
	( @ID               uniqueidentifier
	, @MODIFIED_USER_ID uniqueidentifier
	)
as
  begin
	set nocount on
	
$deleteprocedureupdates$
	
	-- BEGIN Oracle Exception
		delete from TRACKER
		 where ITEM_ID          = @ID
		   and USER_ID          = @MODIFIED_USER_ID;
	-- END Oracle Exception
	
	exec dbo.spPARENT_Delete @ID, @MODIFIED_USER_ID;
	
	-- BEGIN Oracle Exception
		update $tablename$
		   set DELETED          = 1
		     , DATE_MODIFIED    = getdate()
		     , DATE_MODIFIED_UTC= getutcdate()
		     , MODIFIED_USER_ID = @MODIFIED_USER_ID
		 where ID               = @ID
		   and DELETED          = 0;
	-- END Oracle Exception
  end
GO

Grant Execute on dbo.sp$tablename$_Delete to public;
GO

