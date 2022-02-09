if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'sp$tablename$_Update' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.sp$tablename$_Update;
GO


Create Procedure dbo.sp$tablename$_Update
	( @ID                                 uniqueidentifier output
	, @MODIFIED_USER_ID                   uniqueidentifier
$createprocedureparameters$
	, @TAG_SET_NAME                       nvarchar(4000)
	)
as
  begin
	set nocount on
	
$createprocedurenormalizeteams$
	if not exists(select * from $tablename$ where ID = @ID) begin -- then
		if dbo.fnIsEmptyGuid(@ID) = 1 begin -- then
			set @ID = newid();
		end -- if;
		insert into $tablename$
			( ID                                 
			, CREATED_BY                         
			, DATE_ENTERED                       
			, MODIFIED_USER_ID                   
			, DATE_MODIFIED                      
			, DATE_MODIFIED_UTC                  
$createprocedureinsertinto$
			)
		values
			( @ID                                 
			, @MODIFIED_USER_ID                   
			,  getdate()                          
			, @MODIFIED_USER_ID                   
			,  getdate()                          
			,  getutcdate()                       
$createprocedureinsertvalues$
			);
	end else begin
		update $tablename$
		   set MODIFIED_USER_ID                     = @MODIFIED_USER_ID                   
		     , DATE_MODIFIED                        =  getdate()                          
		     , DATE_MODIFIED_UTC                    =  getutcdate()                       
$createprocedureupdate$
		 where ID                                   = @ID                                 ;
	end -- if;

	if @@ERROR = 0 begin -- then
		if not exists(select * from $tablename$_CSTM where ID_C = @ID) begin -- then
			insert into $tablename$_CSTM ( ID_C ) values ( @ID );
		end -- if;

$createprocedureupdateteams$
	end -- if;

  end
GO

Grant Execute on dbo.sp$tablename$_Update to public;
GO

