if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spDETAILVIEWS_FIELDS_Update' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spDETAILVIEWS_FIELDS_Update;
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
-- 07/24/2006 Paul.  Increase the DATA_LABEL to 150 to allow a fully-qualified (NAME+MODULE_NAME+LIST_NAME) TERMINOLOGY name. 
-- 11/22/2006 Paul.  Prevent index from overlapping. This is needed to simplify inserting the TEAM field. 
-- 06/12/2009 Paul.  Add TOOL_TIP for help hover.
-- 02/16/2010 Paul.  Add MODULE_TYPE so that we can lookup custom field IDs. 
-- 06/16/2010 Paul.  Increase the size of the URL_FIELD and URL_FORMAT so that we can create an IFrame to a Google map. 
-- 08/02/2010 Paul.  Increase the size of the URL_FIELD and URL_FORMAT so that we can add a javascript info column. 
-- 10/09/2010 Paul.  Add PARENT_FIELD so that we can establish dependent listboxes. 
-- 10/30/2013 Paul.  Increase size of URL_TARGET. 
-- 02/25/2015 Paul.  Increase size of DATA_FIELD and DATA_FORMAT for OfficeAddin. 
Create Procedure dbo.spDETAILVIEWS_FIELDS_Update
	( @ID                uniqueidentifier output
	, @MODIFIED_USER_ID  uniqueidentifier
	, @DETAIL_NAME       nvarchar( 50)
	, @FIELD_INDEX       int
	, @FIELD_TYPE        nvarchar( 25)
	, @DATA_LABEL        nvarchar(150)
	, @DATA_FIELD        nvarchar(1000)
	, @DATA_FORMAT       nvarchar(max)
	, @URL_FIELD         nvarchar(max)
	, @URL_FORMAT        nvarchar(max)
	, @URL_TARGET        nvarchar( 60)
	, @LIST_NAME         nvarchar( 50)
	, @COLSPAN           int
	, @TOOL_TIP          nvarchar(150) = null
	, @MODULE_TYPE       nvarchar(25) = null
	, @PARENT_FIELD      nvarchar(30) = null
	)
as
  begin
	-- 06/12/2009 Paul.  PostgreSQL does not allow modifying input parameters.  Use a local temp variable. 
	declare @TEMP_DATA_LABEL        nvarchar(150);
	declare @TEMP_DATA_FIELD        nvarchar(100);
	declare @TEMP_DATA_FORMAT       nvarchar(100);
	declare @TEMP_URL_FIELD         nvarchar(500);
	declare @TEMP_URL_FORMAT        nvarchar(300);
	declare @TEMP_URL_TARGET        nvarchar( 20);
	declare @TEMP_LIST_NAME         nvarchar( 50);
	declare @TEMP_TOOL_TIP          nvarchar(150);
	declare @TEMP_MODULE_TYPE       nvarchar( 25);
	declare @TEMP_PARENT_FIELD      nvarchar( 30);

	set @TEMP_DATA_LABEL  = @DATA_LABEL ;
	set @TEMP_DATA_FIELD  = @DATA_FIELD ;
	set @TEMP_DATA_FORMAT = @DATA_FORMAT;
	set @TEMP_URL_FIELD   = @URL_FIELD  ;
	set @TEMP_URL_FORMAT  = @URL_FORMAT ;
	set @TEMP_URL_TARGET  = @URL_TARGET ;
	set @TEMP_LIST_NAME   = @LIST_NAME  ;
	set @TEMP_TOOL_TIP    = @TOOL_TIP   ;
	set @TEMP_MODULE_TYPE = @MODULE_TYPE;
	set @TEMP_PARENT_FIELD = @PARENT_FIELD;

	-- 03/19/2009 Paul.  We need to clear fields when using Blank. 
	if @FIELD_TYPE = N'Blank' begin -- then
		set @TEMP_DATA_LABEL         = null;
		set @TEMP_DATA_FIELD         = null;
		set @TEMP_DATA_FORMAT        = null;
		set @TEMP_URL_FIELD          = null;
		set @TEMP_URL_FORMAT         = null;
		set @TEMP_URL_TARGET         = null;
		set @TEMP_LIST_NAME          = null;
		set @TEMP_TOOL_TIP           = null;
		set @TEMP_MODULE_TYPE        = null;
		set @TEMP_PARENT_FIELD       = null;
	end -- if;

	-- 01/09/2006 Paul.  Can't convert EDIT_NAME and FIELD_INDEX into an ID
	-- as it would prevent the Layout Manager from working properly. 
	if not exists(select * from DETAILVIEWS_FIELDS where ID = @ID) begin -- then
		if dbo.fnIsEmptyGuid(@ID) = 1 begin -- then
			set @ID = newid();
		end -- if;
		-- 11/22/2006 Paul.  Prevent index from overlapping. 
		if exists(select * from DETAILVIEWS_FIELDS where DETAIL_NAME = @DETAIL_NAME and FIELD_INDEX = @FIELD_INDEX and DEFAULT_VIEW = 0 and DELETED = 0) begin -- then
			update DETAILVIEWS_FIELDS
			   set FIELD_INDEX   = FIELD_INDEX + 1
			 where DETAIL_NAME   = @DETAIL_NAME
			   and FIELD_INDEX  >= @FIELD_INDEX
			   and DEFAULT_VIEW  = 0
			   and DELETED       = 0;
		end -- if;
		insert into DETAILVIEWS_FIELDS
			( ID               
			, CREATED_BY       
			, DATE_ENTERED     
			, MODIFIED_USER_ID 
			, DATE_MODIFIED    
			, DETAIL_NAME      
			, FIELD_INDEX      
			, FIELD_TYPE       
			, DATA_LABEL       
			, DATA_FIELD       
			, DATA_FORMAT      
			, URL_FIELD        
			, URL_FORMAT       
			, URL_TARGET       
			, LIST_NAME        
			, COLSPAN          
			, TOOL_TIP         
			, MODULE_TYPE      
			, PARENT_FIELD     
			)
		values 
			( @ID               
			, @MODIFIED_USER_ID 
			,  getdate()        
			, @MODIFIED_USER_ID 
			,  getdate()        
			, @DETAIL_NAME      
			, @FIELD_INDEX      
			, @FIELD_TYPE       
			, @TEMP_DATA_LABEL  
			, @TEMP_DATA_FIELD  
			, @TEMP_DATA_FORMAT 
			, @TEMP_URL_FIELD   
			, @TEMP_URL_FORMAT  
			, @TEMP_URL_TARGET  
			, @TEMP_LIST_NAME   
			, @COLSPAN          
			, @TEMP_TOOL_TIP    
			, @TEMP_MODULE_TYPE 
			, @PARENT_FIELD     
			);
	end else begin
		update DETAILVIEWS_FIELDS
		   set MODIFIED_USER_ID  = @MODIFIED_USER_ID 
		     , DATE_MODIFIED     =  getdate()        
		     , DATE_MODIFIED_UTC =  getutcdate()     
		     , DETAIL_NAME       = @DETAIL_NAME      
		     , FIELD_INDEX       = @FIELD_INDEX      
		     , FIELD_TYPE        = @FIELD_TYPE       
		     , DATA_LABEL        = @TEMP_DATA_LABEL  
		     , DATA_FIELD        = @TEMP_DATA_FIELD  
		     , DATA_FORMAT       = @TEMP_DATA_FORMAT 
		     , URL_FIELD         = @TEMP_URL_FIELD   
		     , URL_FORMAT        = @TEMP_URL_FORMAT  
		     , URL_TARGET        = @TEMP_URL_TARGET  
		     , LIST_NAME         = @TEMP_LIST_NAME   
		     , COLSPAN           = @COLSPAN          
		     , TOOL_TIP          = @TEMP_TOOL_TIP    
		     , MODULE_TYPE       = @TEMP_MODULE_TYPE 
		     , PARENT_FIELD      = @TEMP_PARENT_FIELD
		 where ID                = @ID               ;
	end -- if;
  end
GO
 
Grant Execute on dbo.spDETAILVIEWS_FIELDS_Update to public;
GO
 
