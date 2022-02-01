if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spFIELDS_META_DATA_Update' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spFIELDS_META_DATA_Update;
GO


/**********************************************************************************************************************
 * Copyright (C) 2005-2022 SplendidCRM Software, Inc. 
 * MIT License
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
 * files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
 * modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
 * is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
 * LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR 
 * IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 *********************************************************************************************************************/
-- 04/21/2006 Paul.  MASS_UPDATE was added in SugarCRM 4.0.
-- 01/10/2007 Paul.  DROPDOWN_LIST was added as it can be modified.
-- 01/02/2008 Paul.  Make sure to add the column to the audit table and to add the triggers. 
-- 08/27/2008 Paul.  PostgreSQL does not allow modifying input parameters.  Use a local temp variable. 
-- 11/17/2008 Paul.  We need to update the audit views for the workflow engine. 
-- 04/18/2016 Paul.  Allow disable recompile so that we can do in the background. 
-- 12/18/2017 Paul.  Add Archive access right. 
-- 07/27/2018 Paul.  We need to update the Custom table not the main table. 
Create Procedure dbo.spFIELDS_META_DATA_Update
	( @ID                uniqueidentifier
	, @MODIFIED_USER_ID  uniqueidentifier
	, @MAX_SIZE          int
	, @REQUIRED          bit
	, @AUDITED           bit
	, @DEFAULT_VALUE     nvarchar(255)
	, @DROPDOWN_LIST     nvarchar(50)
	, @MASS_UPDATE       bit
	, @DISABLE_RECOMPILE bit = null
	)
as
  begin
	set nocount on
	
	declare @CUSTOM_MODULE      nvarchar(255);
	declare @CUSTOM_TABLE       nvarchar(255);
	-- 12/18/2017 Paul.  Add Archive access right. 
	declare @ARCHIVE_TABLE      nvarchar(255);
	declare @ARCHIVE_DATABASE   nvarchar(50);
	declare @EXISTS             bit;
	declare @NAME               nvarchar(255);
	declare @DATA_TYPE          nvarchar(255);
	declare @REQUIRED_OPTION    nvarchar(255);
	declare @TEMP_MAX_SIZE      int;
	declare @TEMP_DROPDOWN_LIST nvarchar(50);
	declare @TABLE_NAME         nvarchar(255);

	set @TEMP_MAX_SIZE      = @MAX_SIZE;
	set @TEMP_DROPDOWN_LIST = @DROPDOWN_LIST;
	if @REQUIRED = 1 begin -- then
		set @REQUIRED_OPTION = 'required';
	end else begin
		set @REQUIRED_OPTION = 'optional';
	end -- if;

	-- BEGIN Oracle Exception
		select @CUSTOM_MODULE = CUSTOM_MODULE
		     , @NAME          = NAME
		     , @DATA_TYPE     = DATA_TYPE
		  from FIELDS_META_DATA
		 where ID             = @ID
		   and DELETED        = 0;
	-- END Oracle Exception

	-- 07/15/2009 Paul.  We can't clear the dropdown until after we have loaded the data type. 
	if @DATA_TYPE <> 'enum' begin -- then
		set @TEMP_DROPDOWN_LIST = null;
	end -- if;

	if @CUSTOM_MODULE is not null and @NAME is not null begin -- then
		if @DATA_TYPE = 'varchar' begin -- then
			if @TEMP_MAX_SIZE = 0 begin -- then
				set @TEMP_MAX_SIZE = 0;
			end else if @TEMP_MAX_SIZE > 4000 begin -- then
				set @TEMP_MAX_SIZE = 4000;
			end -- if;
		end else begin
			set @TEMP_MAX_SIZE = null;
		end -- if;
	
		
		set @CUSTOM_TABLE = dbo.fnCustomTableName(@CUSTOM_MODULE);
		-- 01/06/2006 Paul.  Try and add the column first as it is more likely to fail than the insert statement. 
		exec dbo.spSqlTableAlterColumn @CUSTOM_TABLE, @NAME, @DATA_TYPE, @TEMP_MAX_SIZE, @REQUIRED, @DEFAULT_VALUE;
	
		-- 04/02/2006 Paul.  Catch the Oracle NO_DATA_FOUND exception. 
		-- BEGIN Oracle Exception
			update FIELDS_META_DATA
			   set MODIFIED_USER_ID  = @MODIFIED_USER_ID 
			     , DATE_MODIFIED     =  getdate()        
			     , DATE_MODIFIED_UTC =  getutcdate()     
			     , MAX_SIZE          = @TEMP_MAX_SIZE    
			     , REQUIRED_OPTION   = @REQUIRED_OPTION  
			     , AUDITED           = @AUDITED          
			     , DEFAULT_VALUE     = @DEFAULT_VALUE    
			     , EXT1              = @TEMP_DROPDOWN_LIST    
			     , MASS_UPDATE       = @MASS_UPDATE      
			 where ID                = @ID               ;
		-- END Oracle Exception
	
		-- 01/06/2006 Paul.  If successful altering a field, then refresh all views. 
		if @@ERROR = 0 begin -- then
			-- 01/02/2008 Paul.  Make sure to add the column to the audit table and to add the triggers. 
			if exists(select * from vwSqlTablesAudited where TABLE_NAME = @CUSTOM_TABLE) begin -- then
				exec dbo.spSqlBuildAuditTable   @CUSTOM_TABLE;
				exec dbo.spSqlBuildAuditTrigger @CUSTOM_TABLE;

				-- 11/17/2008 Paul.  We need to update the audit views for the workflow engine. 
				-- BEGIN Oracle Exception
					select @TABLE_NAME = TABLE_NAME
					  from vwMODULES
					 where MODULE_NAME = @CUSTOM_MODULE;
				-- END Oracle Exception
				if @TABLE_NAME is not null begin -- then
					if exists(select * from vwSqlTablesAudited where TABLE_NAME = @TABLE_NAME) begin -- then
						exec dbo.spSqlBuildAuditView @TABLE_NAME;
					end -- if;
					
					-- 12/18/2017 Paul.  We need to refresh Stream views. 
					if exists(select * from vwSqlTablesStreamed where TABLE_NAME = @TABLE_NAME) begin -- then
						if exists(select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'fn' + @TABLE_NAME + '_AUDIT_Columns') begin -- then
							exec dbo.spSqlBuildStreamFunction @TABLE_NAME;
						end -- if;
						if exists(select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'vw' + @TABLE_NAME + '_STREAM') begin -- then
							exec dbo.spSqlBuildStreamView @TABLE_NAME;
						end -- if;
					end -- if;
					
					-- 12/18/2017 Paul.  Add Archive access right. 
					set @ARCHIVE_TABLE = @TABLE_NAME + '_ARCHIVE';
					set @ARCHIVE_DATABASE = dbo.fnCONFIG_String('Archive.Database');
					exec dbo.spSqlTableExists @EXISTS out, @ARCHIVE_TABLE, @ARCHIVE_DATABASE;
					if @EXISTS = 1 begin -- then
						-- 07/27/2018 Paul.  We need to update the Custom table not the main table. 
						exec dbo.spSqlBuildArchiveTable @CUSTOM_TABLE, @ARCHIVE_DATABASE;
						exec dbo.spSqlBuildArchiveView  @TABLE_NAME, @ARCHIVE_DATABASE;
					end -- if;
				end -- if;
			end -- if;
			-- 04/18/2016 Paul.  Allow disable recompile so that we can do in the background. 
			if isnull(@DISABLE_RECOMPILE, 0) = 0 begin -- then
				exec dbo.spSqlRefreshAllViews ;
			end -- if;
		end -- if;
	end -- if;
  end
GO

Grant Execute on dbo.spFIELDS_META_DATA_Update to public;
GO

