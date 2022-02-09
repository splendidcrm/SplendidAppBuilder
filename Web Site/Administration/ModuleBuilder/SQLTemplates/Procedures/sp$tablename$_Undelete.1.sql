if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'sp$tablename$_Undelete' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.sp$tablename$_Undelete;
GO


Create Procedure dbo.sp$tablename$_Undelete
	( @ID               uniqueidentifier
	, @MODIFIED_USER_ID uniqueidentifier
	, @AUDIT_TOKEN      varchar(255)
	)
as
  begin
	set nocount on
	
$undeleteprocedureupdates$
	
	exec dbo.spPARENT_Undelete @ID, @MODIFIED_USER_ID, @AUDIT_TOKEN, N'$modulename$';
	
	-- BEGIN Oracle Exception
		update $tablename$
		   set DELETED          = 0
		     , DATE_MODIFIED    = getdate()
		     , DATE_MODIFIED_UTC= getutcdate()
		     , MODIFIED_USER_ID = @MODIFIED_USER_ID
		 where ID               = @ID
		   and DELETED          = 1
		   and ID in (select ID from $tablename$_AUDIT where AUDIT_TOKEN = @AUDIT_TOKEN and ID = @ID);
	-- END Oracle Exception
	
  end
GO

Grant Execute on dbo.sp$tablename$_Undelete to public;
GO

