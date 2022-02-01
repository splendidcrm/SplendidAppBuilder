if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spMODULES_GROUPS_InsertOnly' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spMODULES_GROUPS_InsertOnly;
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
-- 02/24/2010 Paul.  We need to specify an order to the modules for the tab menu. 
Create Procedure dbo.spMODULES_GROUPS_InsertOnly
	( @GROUP_NAME         nvarchar(25)
	, @MODULE_NAME        nvarchar(50)
	, @MODULE_ORDER       int
	, @MODULE_MENU        bit
	)
as
  begin
	set nocount on
	
	declare @ID                 uniqueidentifier;
	declare @MODIFIED_USER_ID   uniqueidentifier;
	-- BEGIN Oracle Exception
		select @ID = ID
		  from MODULES_GROUPS
		 where GROUP_NAME        = @GROUP_NAME
		   and MODULE_NAME       = @MODULE_NAME
		   and DELETED           = 0;
	-- END Oracle Exception
	
	if dbo.fnIsEmptyGuid(@ID) = 1 begin -- then
		set @ID = newid();
		insert into MODULES_GROUPS
			( ID                
			, CREATED_BY        
			, DATE_ENTERED      
			, MODIFIED_USER_ID  
			, DATE_MODIFIED     
			, DATE_MODIFIED_UTC 
			, GROUP_NAME        
			, MODULE_NAME       
			, MODULE_ORDER      
			, MODULE_MENU       
			)
		values 	( @ID                
			, @MODIFIED_USER_ID  
			,  getdate()         
			, @MODIFIED_USER_ID  
			,  getdate()         
			,  getutcdate()      
			, @GROUP_NAME        
			, @MODULE_NAME       
			, @MODULE_ORDER      
			, @MODULE_MENU       
			);
	end -- if;
  end
GO

Grant Execute on dbo.spMODULES_GROUPS_InsertOnly to public;
GO

