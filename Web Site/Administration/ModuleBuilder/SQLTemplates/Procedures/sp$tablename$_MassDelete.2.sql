if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'sp$tablename$_MassDelete' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.sp$tablename$_MassDelete;
GO


Create Procedure dbo.sp$tablename$_MassDelete
	( @ID_LIST          varchar(8000)
	, @MODIFIED_USER_ID uniqueidentifier
	)
as
  begin
	set nocount on
	
	declare @ID           uniqueidentifier;
	declare @CurrentPosR  int;
	declare @NextPosR     int;
	set @CurrentPosR = 1;
	while @CurrentPosR <= len(@ID_LIST) begin -- do
		set @NextPosR = charindex(',', @ID_LIST,  @CurrentPosR);
		if @NextPosR = 0 or @NextPosR is null begin -- then
			set @NextPosR = len(@ID_LIST) + 1;
		end -- if;
		set @ID = cast(rtrim(ltrim(substring(@ID_LIST, @CurrentPosR, @NextPosR - @CurrentPosR))) as uniqueidentifier);
		set @CurrentPosR = @NextPosR+1;
		exec dbo.sp$tablename$_Delete @ID, @MODIFIED_USER_ID;
	end -- while;
  end
GO
 
Grant Execute on dbo.sp$tablename$_MassDelete to public;
GO
 
 
