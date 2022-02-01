if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spDETAILVIEWS_RELATIONSHIPS_InsertOnly' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spDETAILVIEWS_RELATIONSHIPS_InsertOnly;
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
-- 09/08/2007 Paul.  We need a title when we migrate to WebParts. 
-- 12/16/2007 Paul.  Make the title optional to reduce problems during upgrade. 
-- 09/24/2009 Paul.  The new Silverlight charts exceeded the control name length of 50. 
-- 03/05/2011 Paul.  If @RELATIONSHIP_ORDER is null, then add control to the bottom. 
-- 10/13/2012 Paul.  Add table info for HTML5 Offline Client. 
-- 03/01/2013 Paul.  Sort Field and Sort Direction may be invalid. Correct for this. 
-- 03/20/2016 Paul.  Increase PRIMARY_FIELD size to 255 to support OfficeAddin. 
Create Procedure dbo.spDETAILVIEWS_RELATIONSHIPS_InsertOnly
	( @DETAIL_NAME         nvarchar(50)
	, @MODULE_NAME         nvarchar(50)
	, @CONTROL_NAME        nvarchar(100)
	, @RELATIONSHIP_ORDER  int
	, @TITLE               nvarchar(100) = null
	, @TABLE_NAME          nvarchar(50) = null
	, @PRIMARY_FIELD       nvarchar(255) = null
	, @SORT_FIELD          nvarchar(50) = null
	, @SORT_DIRECTION      nvarchar(10) = null
	)
as
  begin

	declare @TEMP_RELATIONSHIP_ORDER  int;
	declare @TEMP_SORT_FIELD          nvarchar(50);
	declare @TEMP_SORT_DIRECTION      nvarchar(10);
	set @TEMP_RELATIONSHIP_ORDER = @RELATIONSHIP_ORDER;
	set @TEMP_SORT_FIELD         = @SORT_FIELD;
	set @TEMP_SORT_DIRECTION     = @SORT_DIRECTION;
	if @RELATIONSHIP_ORDER is null or @RELATIONSHIP_ORDER = -1 begin -- then
		-- 09/09/2012 Paul.  Only include enabled relationships. 
		-- BEGIN Oracle Exception
			select @TEMP_RELATIONSHIP_ORDER = isnull(max(RELATIONSHIP_ORDER), 0) + 1
			  from DETAILVIEWS_RELATIONSHIPS
			 where DETAIL_NAME          = @DETAIL_NAME
			   and RELATIONSHIP_ENABLED = 1
			   and DELETED              = 0;
		-- END Oracle Exception
	end -- if;
	-- 03/01/2013 Paul.  Sort Field and Sort Direction may be invalid. Correct for this. 
	if @TABLE_NAME is not null and @TEMP_SORT_FIELD is not null begin -- then
		if not exists(select * from vwSqlColumns where ObjectName = @TABLE_NAME and ColumnName = @SORT_FIELD) begin -- then
			set @TEMP_SORT_FIELD     = null;
			set @TEMP_SORT_DIRECTION = null;
		end -- if;
	end -- if;
	if not exists(select * from DETAILVIEWS_RELATIONSHIPS where DETAIL_NAME = @DETAIL_NAME and CONTROL_NAME = @CONTROL_NAME and DELETED = 0) begin -- then
		insert into DETAILVIEWS_RELATIONSHIPS
			( ID                 
			, CREATED_BY         
			, DATE_ENTERED       
			, MODIFIED_USER_ID   
			, DATE_MODIFIED      
			, DETAIL_NAME        
			, MODULE_NAME        
			, CONTROL_NAME       
			, RELATIONSHIP_ORDER 
			, TITLE              
			, TABLE_NAME         
			, PRIMARY_FIELD      
			, SORT_FIELD         
			, SORT_DIRECTION     
			)
		values 
			( newid()                 
			, null                    
			,  getdate()              
			, null                    
			,  getdate()              
			, @DETAIL_NAME            
			, @MODULE_NAME            
			, @CONTROL_NAME           
			, @TEMP_RELATIONSHIP_ORDER
			, @TITLE                  
			, @TABLE_NAME             
			, @PRIMARY_FIELD          
			, @TEMP_SORT_FIELD        
			, @TEMP_SORT_DIRECTION    
			);
	end else begin
		-- 03/01/2013 Paul.  If this is an old entry, make sure that it has an updated sort field. 
		if @TABLE_NAME is not null and @TEMP_SORT_FIELD is not null begin -- then
			if exists(select * from DETAILVIEWS_RELATIONSHIPS where DETAIL_NAME = @DETAIL_NAME and CONTROL_NAME = @CONTROL_NAME and TABLE_NAME is null and SORT_FIELD is null and DELETED = 0) begin -- then
				update DETAILVIEWS_RELATIONSHIPS
				   set TABLE_NAME        = @TABLE_NAME
				     , SORT_FIELD        = @TEMP_SORT_FIELD
				     , SORT_DIRECTION    = @TEMP_SORT_DIRECTION
				     , DATE_MODIFIED     = getdate()
				     , DATE_MODIFIED_UTC = getutcdate()
				     , MODIFIED_USER_ID  = null
				 where DETAIL_NAME       = @DETAIL_NAME
				   and CONTROL_NAME      = @CONTROL_NAME
				   and TABLE_NAME        is null 
				   and SORT_FIELD        is null
				   and DELETED           = 0;
			end -- if;
		end -- if;
	end -- if;
  end
GO
 
Grant Execute on dbo.spDETAILVIEWS_RELATIONSHIPS_InsertOnly to public;
GO
 
