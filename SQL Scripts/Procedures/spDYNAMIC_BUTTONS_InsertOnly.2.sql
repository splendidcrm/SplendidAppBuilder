if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spDYNAMIC_BUTTONS_InsertOnly' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spDYNAMIC_BUTTONS_InsertOnly;
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
-- 09/12/2010 Paul.  Add default parameter EXCLUDE_MOBILE to ease migration to EffiProz. 
-- 03/14/2014 Paul.  Allow hidden buttons to be created. 
-- 09/26/2017 Paul.  Support auto numbering. 
Create Procedure dbo.spDYNAMIC_BUTTONS_InsertOnly
	( @VIEW_NAME           nvarchar(50)
	, @CONTROL_INDEX       int
	, @CONTROL_TYPE        nvarchar(25)
	, @MODULE_NAME         nvarchar(25)
	, @MODULE_ACCESS_TYPE  nvarchar(100)
	, @TARGET_NAME         nvarchar(25)
	, @TARGET_ACCESS_TYPE  nvarchar(100)
	, @CONTROL_TEXT        nvarchar(150)
	, @CONTROL_TOOLTIP     nvarchar(150)
	, @CONTROL_ACCESSKEY   nvarchar(150)
	, @CONTROL_CSSCLASS    nvarchar(50)
	, @TEXT_FIELD          nvarchar(200)
	, @ARGUMENT_FIELD      nvarchar(200)
	, @COMMAND_NAME        nvarchar(50)
	, @URL_FORMAT          nvarchar(255)
	, @URL_TARGET          nvarchar(20)
	, @ONCLICK_SCRIPT      nvarchar(255)
	, @MOBILE_ONLY         bit
	, @ADMIN_ONLY          bit
	)
as
  begin
	set nocount on
	
	declare @ID uniqueidentifier;
	
	declare @TEMP_CONTROL_INDEX int;	
	set @TEMP_CONTROL_INDEX = @CONTROL_INDEX;
	if @CONTROL_INDEX is null or @CONTROL_INDEX = -1 begin -- then
		-- BEGIN Oracle Exception
			select @TEMP_CONTROL_INDEX = isnull(max(CONTROL_INDEX), 0) + 1
			  from DYNAMIC_BUTTONS
			 where VIEW_NAME    = @VIEW_NAME   
			   and DELETED      = 0            
			   and DEFAULT_VIEW = 0            ;
		-- END Oracle Exception
		-- BEGIN Oracle Exception
			select top 1 @ID = ID
			  from DYNAMIC_BUTTONS
			 where VIEW_NAME    = @VIEW_NAME   
			   and (isnull(COMMAND_NAME, '') = isnull(@COMMAND_NAME, '') and isnull(ONCLICK_SCRIPT, '') = isnull(@ONCLICK_SCRIPT, ''))
			   and DELETED      = 0            
			   and DEFAULT_VIEW = 0            ;
		-- END Oracle Exception
	end else begin
		-- BEGIN Oracle Exception
			select @ID = ID
			  from DYNAMIC_BUTTONS
			 where VIEW_NAME     = @VIEW_NAME    
			   and CONTROL_INDEX = @CONTROL_INDEX
			   and DELETED       = 0             
			   and DEFAULT_VIEW  = 0             ;
		-- END Oracle Exception
	end -- if;
	if not exists(select * from DYNAMIC_BUTTONS where ID = @ID) begin -- then
		exec dbo.spDYNAMIC_BUTTONS_Update
			  @ID out
			, null                 -- MODIFIED_USER_ID    
			, @VIEW_NAME           
			, @TEMP_CONTROL_INDEX       
			, @CONTROL_TYPE        
			, @MODULE_NAME         
			, @MODULE_ACCESS_TYPE  
			, @TARGET_NAME         
			, @TARGET_ACCESS_TYPE  
			, @CONTROL_TEXT        
			, @CONTROL_TOOLTIP     
			, @CONTROL_ACCESSKEY   
			, @CONTROL_CSSCLASS    
			, @TEXT_FIELD          
			, @ARGUMENT_FIELD      
			, @COMMAND_NAME        
			, @URL_FORMAT          
			, @URL_TARGET          
			, @ONCLICK_SCRIPT      
			, @MOBILE_ONLY         
			, @ADMIN_ONLY          
			, null                 -- EXCLUDE_MOBILE      
			, null                 -- HIDDEN              
			;
	end -- if;
  end
GO

Grant Execute on dbo.spDYNAMIC_BUTTONS_InsertOnly to public;
GO

