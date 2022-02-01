if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spLANGUAGES_InsertOnly' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spLANGUAGES_InsertOnly;
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
Create Procedure dbo.spLANGUAGES_InsertOnly
	( @NAME              nvarchar(10)
	, @LCID              int
	, @ACTIVE            bit
	, @NATIVE_NAME       nvarchar(80)
	, @DISPLAY_NAME      nvarchar(80)
	)
as
  begin
	set nocount on
	
	declare @ID uniqueidentifier;
	declare @MODIFIED_USER_ID  uniqueidentifier;
	if not exists(select * from LANGUAGES where NAME = @NAME and DELETED = 0) begin -- then
		set @ID = newid();
		insert into LANGUAGES
			( ID               
			, CREATED_BY       
			, DATE_ENTERED     
			, MODIFIED_USER_ID 
			, DATE_MODIFIED    
			, NAME             
			, LCID             
			, ACTIVE           
			, NATIVE_NAME      
			, DISPLAY_NAME     
			)
		values
			( @ID               
			, @MODIFIED_USER_ID 
			,  getdate()        
			, @MODIFIED_USER_ID 
			,  getdate()        
			, @NAME             
			, @LCID             
			, @ACTIVE           
			, @NATIVE_NAME      
			, @DISPLAY_NAME     
			);
	end -- if;
	-- 01/13/2006 Paul.  InsertOnly is used when importing a Language Pack. Enable the language if necessary. 
	-- 05/21/2008 Paul.  Language is no longer automatically enabled. Now that we add all supported languages, only support the minimum. 
  end
GO
 
Grant Execute on dbo.spLANGUAGES_InsertOnly to public;
GO
 
