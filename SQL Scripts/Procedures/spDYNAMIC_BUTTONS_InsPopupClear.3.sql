if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spDYNAMIC_BUTTONS_InsPopupClear' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spDYNAMIC_BUTTONS_InsPopupClear;
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
Create Procedure dbo.spDYNAMIC_BUTTONS_InsPopupClear
	( @VIEW_NAME           nvarchar(50)
	, @CONTROL_INDEX       int
	, @MODULE_NAME         nvarchar(25)
	, @MODULE_ACCESS_TYPE  nvarchar(100)
	)
as
  begin
	set nocount on
	
	declare @ID uniqueidentifier;
	declare @TARGET_NAME         nvarchar(25);
	declare @TARGET_ACCESS_TYPE  nvarchar(100);
	declare @ONCLICK_SCRIPT      nvarchar(255);
	declare @TEXT_FIELD          nvarchar(200);
	declare @CONTROL_TEXT        nvarchar(150);
	declare @CONTROL_TOOLTIP     nvarchar(150);
	declare @CONTROL_ACCESSKEY   nvarchar(150);
	declare @ARGUMENT_FIELD      nvarchar(200);
	declare @COMMAND_NAME        varchar(50);
	declare @MOBILE_ONLY         bit;

	-- BEGIN Oracle Exception
		select @ID = ID
		  from DYNAMIC_BUTTONS
		 where VIEW_NAME     = @VIEW_NAME    
		   and CONTROL_INDEX = @CONTROL_INDEX
		   and DELETED       = 0             
		   and DEFAULT_VIEW  = 0             ;
	-- END Oracle Exception

	-- 04/29/2008 Paul.  Popups should return false so that the button does not actually submit the form. 
	set @ONCLICK_SCRIPT    = N'Clear(); return false;';
	set @CONTROL_TEXT      = N'.LBL_CLEAR_BUTTON_LABEL';
	set @CONTROL_TOOLTIP   = N'.LBL_CLEAR_BUTTON_TITLE';
	set @CONTROL_ACCESSKEY = N'.LBL_CLEAR_BUTTON_KEY'  ;
	set @COMMAND_NAME      = N'Clear';

	if not exists(select * from DYNAMIC_BUTTONS where ID = @ID) begin -- then
		exec dbo.spDYNAMIC_BUTTONS_Update
			  @ID out
			, null                 -- MODIFIED_USER_ID    
			, @VIEW_NAME           
			, @CONTROL_INDEX       
			, N'Button'            -- CONTROL_TYPE
			, @MODULE_NAME         
			, @MODULE_ACCESS_TYPE  
			, @TARGET_NAME         
			, @TARGET_ACCESS_TYPE  
			, @CONTROL_TEXT        
			, @CONTROL_TOOLTIP     
			, @CONTROL_ACCESSKEY   
			, N'button'            -- CONTROL_CSSCLASS
			, @TEXT_FIELD          
			, @ARGUMENT_FIELD      
			, @COMMAND_NAME        
			, null                 -- URL_FORMAT
			, null                 -- URL_TARGET
			, @ONCLICK_SCRIPT      
			, @MOBILE_ONLY         
			, 0                    -- ADMIN_ONLY          
			, null                 -- EXCLUDE_MOBILE      
			, null                 -- HIDDEN              
			;
	end -- if;
  end
GO

Grant Execute on dbo.spDYNAMIC_BUTTONS_InsPopupClear to public;
GO

