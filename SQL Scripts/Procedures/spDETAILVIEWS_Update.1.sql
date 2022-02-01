if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spDETAILVIEWS_Update' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spDETAILVIEWS_Update;
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
-- 12/02/2007 Paul.  Add field for data columns. 
-- 10/30/2010 Paul.  Add support for Business Rules Framework. 
-- 11/11/2010 Paul.  Change to Pre Load and Post Load. 
-- 09/20/2012 Paul.  We need a SCRIPT field that is form specific. 
Create Procedure dbo.spDETAILVIEWS_Update
	( @ID                  uniqueidentifier output
	, @MODIFIED_USER_ID    uniqueidentifier
	, @NAME                nvarchar(50)
	, @MODULE_NAME         nvarchar(25)
	, @VIEW_NAME           nvarchar(50)
	, @LABEL_WIDTH         nvarchar(10)
	, @FIELD_WIDTH         nvarchar(10)
	, @DATA_COLUMNS        int
	, @PRE_LOAD_EVENT_ID   uniqueidentifier = null
	, @POST_LOAD_EVENT_ID  uniqueidentifier = null
	, @SCRIPT              nvarchar(max) = null
	)
as
  begin
	if not exists(select * from DETAILVIEWS where NAME = @NAME and DELETED = 0) begin -- then
		if dbo.fnIsEmptyGuid(@ID) = 1 begin -- then
			set @ID = newid();
		end -- if;
		insert into DETAILVIEWS
			( ID                 
			, CREATED_BY         
			, DATE_ENTERED       
			, MODIFIED_USER_ID   
			, DATE_MODIFIED      
			, NAME               
			, MODULE_NAME        
			, VIEW_NAME          
			, LABEL_WIDTH        
			, FIELD_WIDTH        
			, DATA_COLUMNS       
			, PRE_LOAD_EVENT_ID  
			, POST_LOAD_EVENT_ID 
			, SCRIPT             
			)
		values 
			( @ID                 
			, @MODIFIED_USER_ID   
			,  getdate()          
			, @MODIFIED_USER_ID   
			,  getdate()          
			, @NAME               
			, @MODULE_NAME        
			, @VIEW_NAME          
			, @LABEL_WIDTH        
			, @FIELD_WIDTH        
			, @DATA_COLUMNS       
			, @PRE_LOAD_EVENT_ID  
			, @POST_LOAD_EVENT_ID 
			, @SCRIPT             
			);
	end else begin
		update DETAILVIEWS
		   set MODIFIED_USER_ID    = @MODIFIED_USER_ID   
		     , DATE_MODIFIED       =  getdate()          
		     , DATE_MODIFIED_UTC   =  getutcdate()       
		     , NAME                = @NAME               
		     , MODULE_NAME         = @MODULE_NAME        
		     , VIEW_NAME           = @VIEW_NAME          
		     , LABEL_WIDTH         = @LABEL_WIDTH        
		     , FIELD_WIDTH         = @FIELD_WIDTH        
		     , DATA_COLUMNS        = @DATA_COLUMNS       
		     , PRE_LOAD_EVENT_ID   = @PRE_LOAD_EVENT_ID  
		     , POST_LOAD_EVENT_ID  = @POST_LOAD_EVENT_ID 
		     , SCRIPT              = @SCRIPT             
		 where NAME                = @NAME               
		   and DELETED             = 0                   ;
	end -- if;
  end
GO
 
Grant Execute on dbo.spDETAILVIEWS_Update to public;
GO
 
