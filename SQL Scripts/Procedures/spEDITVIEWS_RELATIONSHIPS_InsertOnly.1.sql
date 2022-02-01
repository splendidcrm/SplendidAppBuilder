if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spEDITVIEWS_RELATIONSHIPS_InsertOnly' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spEDITVIEWS_RELATIONSHIPS_InsertOnly;
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
Create Procedure dbo.spEDITVIEWS_RELATIONSHIPS_InsertOnly
	( @EDIT_NAME               nvarchar(50)
	, @MODULE_NAME             nvarchar(50)
	, @CONTROL_NAME            nvarchar(100)
	, @RELATIONSHIP_ENABLED    bit
	, @RELATIONSHIP_ORDER      int
	, @NEW_RECORD_ENABLED      bit
	, @EXISTING_RECORD_ENABLED bit
	, @TITLE                   nvarchar(100)
	, @ALTERNATE_VIEW          nvarchar(50)
	)
as
  begin
	if not exists(select * from EDITVIEWS_RELATIONSHIPS where EDIT_NAME = @EDIT_NAME and CONTROL_NAME = @CONTROL_NAME and DELETED = 0) begin -- then
		insert into EDITVIEWS_RELATIONSHIPS
			( ID                 
			, CREATED_BY         
			, DATE_ENTERED       
			, MODIFIED_USER_ID   
			, DATE_MODIFIED      
			, EDIT_NAME          
			, MODULE_NAME        
			, CONTROL_NAME       
			, RELATIONSHIP_ORDER 
			, RELATIONSHIP_ENABLED
			, NEW_RECORD_ENABLED     
			, EXISTING_RECORD_ENABLED
			, TITLE              
			, ALTERNATE_VIEW     
			)
		values 
			( newid()             
			, null                
			,  getdate()          
			, null                
			,  getdate()          
			, @EDIT_NAME          
			, @MODULE_NAME        
			, @CONTROL_NAME       
			, @RELATIONSHIP_ORDER 
			, @RELATIONSHIP_ENABLED
			, @NEW_RECORD_ENABLED     
			, @EXISTING_RECORD_ENABLED
			, @TITLE              
			, @ALTERNATE_VIEW     
			);
	end -- if;
  end
GO
 
Grant Execute on dbo.spEDITVIEWS_RELATIONSHIPS_InsertOnly to public;
GO
 
