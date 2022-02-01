if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spDYNAMIC_BUTTONS_InsViewLog' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spDYNAMIC_BUTTONS_InsViewLog;
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
-- 03/14/2014 Paul.  Allow hidden buttons to be created. 
Create Procedure dbo.spDYNAMIC_BUTTONS_InsViewLog
	( @VIEW_NAME           nvarchar(50)
	, @CONTROL_INDEX       int
	, @MODULE_NAME         nvarchar(25)
	)
as
  begin
	set nocount on
	
	declare @ID uniqueidentifier;
	
	-- BEGIN Oracle Exception
		select @ID = ID
		  from DYNAMIC_BUTTONS
		 where VIEW_NAME     = @VIEW_NAME    
		   and COMMAND_NAME  = N'ViewLog'    
		   and DELETED       = 0             
		   and DEFAULT_VIEW  = 0             ;
	-- END Oracle Exception
	if not exists(select * from DYNAMIC_BUTTONS where ID = @ID) begin -- then
		exec dbo.spDYNAMIC_BUTTONS_Update
			  @ID out
			, null                    -- MODIFIED_USER_ID    
			, @VIEW_NAME           
			, @CONTROL_INDEX       
			, N'Button'               -- CONTROL_TYPE
			, @MODULE_NAME         
			, N'view'                 -- MODULE_ACCESS_TYPE  
			, null                    -- TARGET_NAME         
			, null                    -- TARGET_ACCESS_TYPE  
			, N'.LNK_VIEW_CHANGE_LOG' -- CONTROL_TEXT        
			, N'.LNK_VIEW_CHANGE_LOG' -- CONTROL_TOOLTIP     
			, null                    -- CONTROL_ACCESSKEY   
			, N'button'               -- CONTROL_CSSCLASS
			, null                    -- TEXT_FIELD          
			, null                    -- ARGUMENT_FIELD      
			, N'ViewLog'              -- COMMAND_NAME        
			, null                    -- URL_FORMAT
			, null                    -- URL_TARGET
			, N'return PopupAudit();' -- ONCLICK_SCRIPT      
			, 0                       -- MOBILE_ONLY         
			, 0                       -- ADMIN_ONLY          
			, 1                       -- EXCLUDE_MOBILE
			, null                    -- HIDDEN              
			;
	end -- if;
  end
GO

Grant Execute on dbo.spDYNAMIC_BUTTONS_InsViewLog to public;
GO

