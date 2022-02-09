if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'sp$tablename$_$relatedtable$_Update' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.sp$tablename$_$relatedtable$_Update;
GO


Create Procedure dbo.sp$tablename$_$relatedtable$_Update
	( @MODIFIED_USER_ID          uniqueidentifier
	, @$tablenamesingular$_ID    uniqueidentifier
	, @$relatedtablesingular$_ID uniqueidentifier
	)
as
  begin
	set nocount on
	
	declare @ID uniqueidentifier;
	-- BEGIN Oracle Exception
		select @ID = ID
		  from $tablename$_$relatedtable$
		 where $tablenamesingular$_ID = @$tablenamesingular$_ID
		   and $relatedtablesingular$_ID = @$relatedtablesingular$_ID
		   and DELETED           = 0;
	-- END Oracle Exception
	
	if dbo.fnIsEmptyGuid(@ID) = 1 begin -- then
		set @ID = newid();
		insert into $tablename$_$relatedtable$
			( ID               
			, CREATED_BY       
			, DATE_ENTERED     
			, MODIFIED_USER_ID 
			, DATE_MODIFIED    
			, DATE_MODIFIED_UTC
			, $tablenamesingular$_ID
			, $relatedtablesingular$_ID
			)
		values
			( @ID               
			, @MODIFIED_USER_ID 
			,  getdate()        
			, @MODIFIED_USER_ID 
			,  getdate()        
			,  getutcdate()     
			, @$tablenamesingular$_ID
			, @$relatedtablesingular$_ID
			);
	end -- if;
  end
GO

Grant Execute on dbo.sp$tablename$_$relatedtable$_Update to public;
GO

