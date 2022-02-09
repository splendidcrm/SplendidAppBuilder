if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'sp$tablename$_MassUpdate' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.sp$tablename$_MassUpdate;
GO


Create Procedure dbo.sp$tablename$_MassUpdate
	( @ID_LIST          varchar(8000)
	, @MODIFIED_USER_ID uniqueidentifier
$massupdateviewfields$
	, @TAG_SET_NAME     nvarchar(4000)
	, @TAG_SET_ADD      bit
	)
as
  begin
	set nocount on
	
	declare @ID              uniqueidentifier;
	declare @CurrentPosR     int;
	declare @NextPosR        int;

$massupdateteamnormalize$

	set @CurrentPosR = 1;
	while @CurrentPosR <= len(@ID_LIST) begin -- do
		set @NextPosR = charindex(',', @ID_LIST,  @CurrentPosR);
		if @NextPosR = 0 or @NextPosR is null begin -- then
			set @NextPosR = len(@ID_LIST) + 1;
		end -- if;
		set @ID = cast(rtrim(ltrim(substring(@ID_LIST, @CurrentPosR, @NextPosR - @CurrentPosR))) as uniqueidentifier);
		set @CurrentPosR = @NextPosR+1;

$massupdateteamadd$

		-- BEGIN Oracle Exception
			update $tablename$
			   set MODIFIED_USER_ID  = @MODIFIED_USER_ID
			     , DATE_MODIFIED     =  getdate()
			     , DATE_MODIFIED_UTC =  getutcdate()
$massupdatesets$
			 where ID                = @ID
			   and DELETED           = 0;
		-- END Oracle Exception

$massupdateteamupdate$
	end -- while;
  end
GO

Grant Execute on dbo.sp$tablename$_MassUpdate to public;
GO


