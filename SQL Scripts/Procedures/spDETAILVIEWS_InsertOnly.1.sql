if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spDETAILVIEWS_InsertOnly' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spDETAILVIEWS_InsertOnly;
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
Create Procedure dbo.spDETAILVIEWS_InsertOnly
	( @NAME              nvarchar(50)
	, @MODULE_NAME       nvarchar(25)
	, @VIEW_NAME         nvarchar(50)
	, @LABEL_WIDTH       nvarchar(10)
	, @FIELD_WIDTH       nvarchar(10)
	, @DATA_COLUMNS      int = null
	)
as
  begin
	if not exists(select * from DETAILVIEWS where NAME = @NAME and DELETED = 0) begin -- then
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
			)
		values 
			( newid()           
			, null              
			,  getdate()        
			, null              
			,  getdate()        
			, @NAME             
			, @MODULE_NAME      
			, @VIEW_NAME        
			, @LABEL_WIDTH      
			, @FIELD_WIDTH      
			, @DATA_COLUMNS     
			);
	end -- if;
  end
GO
 
Grant Execute on dbo.spDETAILVIEWS_InsertOnly to public;
GO
 
