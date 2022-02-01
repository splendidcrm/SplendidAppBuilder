if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spTERMINOLOGY_ALIASES_Update' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spTERMINOLOGY_ALIASES_Update;
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
-- 07/24/2006 Paul.  Increase the MODULE_NAME to 25 to match the size in the MODULES table.
Create Procedure dbo.spTERMINOLOGY_ALIASES_Update
	( @ID                 uniqueidentifier output
	, @MODIFIED_USER_ID   uniqueidentifier
	, @ALIAS_NAME         nvarchar(50)
	, @ALIAS_MODULE_NAME  nvarchar(25)
	, @ALIAS_LIST_NAME    nvarchar(50)
	, @NAME               nvarchar(50)
	, @MODULE_NAME        nvarchar(25)
	, @LIST_NAME          nvarchar(50)
	)
as
  begin
	set nocount on
	
	if not exists(select * from TERMINOLOGY_ALIASES where ID = @ID) begin -- then
		if dbo.fnIsEmptyGuid(@ID) = 1 begin -- then
			set @ID = newid();
		end -- if;
		insert into TERMINOLOGY_ALIASES
			( ID                
			, CREATED_BY        
			, DATE_ENTERED      
			, MODIFIED_USER_ID  
			, DATE_MODIFIED     
			, ALIAS_NAME        
			, ALIAS_MODULE_NAME 
			, ALIAS_LIST_NAME   
			, NAME              
			, MODULE_NAME       
			, LIST_NAME         
			)
		values 	( @ID                
			, @MODIFIED_USER_ID        
			,  getdate()         
			, @MODIFIED_USER_ID  
			,  getdate()         
			, @ALIAS_NAME        
			, @ALIAS_MODULE_NAME 
			, @ALIAS_LIST_NAME   
			, @NAME              
			, @MODULE_NAME       
			, @LIST_NAME         
			);
	end else begin
		update TERMINOLOGY_ALIASES
		   set MODIFIED_USER_ID   = @MODIFIED_USER_ID  
		     , DATE_MODIFIED      =  getdate()        
		     , DATE_MODIFIED_UTC =  getutcdate()     
		     , ALIAS_NAME         = @ALIAS_NAME        
		     , ALIAS_MODULE_NAME  = @ALIAS_MODULE_NAME 
		     , ALIAS_LIST_NAME    = @ALIAS_LIST_NAME   
		     , NAME               = @NAME              
		     , MODULE_NAME        = @MODULE_NAME       
		     , LIST_NAME          = @LIST_NAME         
		 where ID                 = @ID                ;
	end -- if;
  end
GO

Grant Execute on dbo.spTERMINOLOGY_ALIASES_Update to public;
GO

