if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spDASHLETS_InsertOnly' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spDASHLETS_InsertOnly;
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
-- 09/24/2009 Paul.  The new Silverlight charts exceeded the control name length of 50. 
-- 01/24/2010 Paul.  Allow multiple. 
Create Procedure dbo.spDASHLETS_InsertOnly
	( @CATEGORY            nvarchar(25)
	, @MODULE_NAME         nvarchar(50)
	, @CONTROL_NAME        nvarchar(100)
	, @TITLE               nvarchar(100)
	, @ALLOW_MULTIPLE      bit = null
	)
as
  begin
	if not exists(select * from DASHLETS where MODULE_NAME = @MODULE_NAME and CONTROL_NAME = @CONTROL_NAME and DELETED = 0) begin -- then
		insert into DASHLETS
			( ID                 
			, CREATED_BY         
			, DATE_ENTERED       
			, MODIFIED_USER_ID   
			, DATE_MODIFIED      
			, CATEGORY           
			, MODULE_NAME        
			, CONTROL_NAME       
			, TITLE              
			, DASHLET_ENABLED    
			, ALLOW_MULTIPLE     
			)
		values 
			( newid()             
			, null                
			,  getdate()          
			, null                
			,  getdate()          
			, @CATEGORY           
			, @MODULE_NAME        
			, @CONTROL_NAME       
			, @TITLE              
			, 1                   
			, @ALLOW_MULTIPLE     
			);
	end -- if;
  end
GO
 
Grant Execute on dbo.spDASHLETS_InsertOnly to public;
GO
 
