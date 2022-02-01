if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spDYNAMIC_BUTTONS_InsSaveConcurrency' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spDYNAMIC_BUTTONS_InsSaveConcurrency;
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
Create Procedure dbo.spDYNAMIC_BUTTONS_InsSaveConcurrency
	( @VIEW_NAME           nvarchar(50)
	, @CONTROL_INDEX       int
	, @MODULE_NAME         nvarchar(25)
	)
as
  begin
	set nocount on
	
	declare @ID uniqueidentifier;
	declare @TEMP_CONTROL_INDEX int;	
	set @TEMP_CONTROL_INDEX = @CONTROL_INDEX;
	
	if @CONTROL_INDEX = -1 begin -- then
		-- BEGIN Oracle Exception
			select @ID = ID
			  from DYNAMIC_BUTTONS
			 where VIEW_NAME     = @VIEW_NAME    
			   and COMMAND_NAME  = N'SaveConcurrency'
			   and DELETED       = 0             
			   and DEFAULT_VIEW  = 0             ;
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
			, null                         -- MODIFIED_USER_ID    
			, @VIEW_NAME                   
			, @TEMP_CONTROL_INDEX          
			, N'Button'                    -- CONTROL_TYPE
			, @MODULE_NAME                 
			, N'edit'                      -- MODULE_ACCESS_TYPE
			, null                         -- TARGET_NAME         
			, null                         -- TARGET_ACCESS_TYPE  
			, N'.LBL_SAVE_CONCURRENCY_LABEL' -- CONTROL_TEXT        
			, N'.LBL_SAVE_CONCURRENCY_TITLE' -- CONTROL_TOOLTIP     
			, null                         -- CONTROL_ACCESSKEY   
			, N'button'                    -- CONTROL_CSSCLASS
			, null                         -- TEXT_FIELD          
			, null                         -- ARGUMENT_FIELD      
			, N'SaveConcurrency'           -- COMMAND_NAME        
			, null                         -- URL_FORMAT
			, null                         -- URL_TARGET
			, null                         -- ONCLICK_SCRIPT      
			, null                         -- MOBILE_ONLY         
			, 0                            -- ADMIN_ONLY          
			, null                         -- EXCLUDE_MOBILE      
			, 1                            -- HIDDEN              
			;
	end -- if;
  end
GO

Grant Execute on dbo.spDYNAMIC_BUTTONS_InsSaveConcurrency to public;
GO

