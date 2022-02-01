if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spDYNAMIC_BUTTONS_CopyDefault' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spDYNAMIC_BUTTONS_CopyDefault;
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
-- 04/13/2008 Paul.  Manually insert the ID to ease migration to Oracle. 
-- 03/14/2014 Paul.  Allow hidden buttons to be created. 
Create Procedure dbo.spDYNAMIC_BUTTONS_CopyDefault
	( @SOURCE_VIEW_NAME    nvarchar(50)
	, @NEW_VIEW_NAME       nvarchar(50)
	, @MODULE_NAME         nvarchar(25)
	)
as
  begin
	set nocount on
	
	if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = @NEW_VIEW_NAME and DELETED = 0) begin -- then
		insert into DYNAMIC_BUTTONS
			( ID
			, DATE_ENTERED      
			, DATE_MODIFIED     
			, VIEW_NAME         
			, CONTROL_INDEX     
			, CONTROL_TYPE      
			, DEFAULT_VIEW      
			, MODULE_NAME       
			, MODULE_ACCESS_TYPE
			, TARGET_NAME       
			, TARGET_ACCESS_TYPE
			, MOBILE_ONLY       
			, ADMIN_ONLY        
			, EXCLUDE_MOBILE    
			, HIDDEN            
			, CONTROL_TEXT      
			, CONTROL_TOOLTIP   
			, CONTROL_ACCESSKEY 
			, CONTROL_CSSCLASS  
			, TEXT_FIELD        
			, ARGUMENT_FIELD    
			, COMMAND_NAME      
			, URL_FORMAT        
			, URL_TARGET        
			, ONCLICK_SCRIPT    
			)
		select	   newid()
			,  getdate()
			,  getdate()
			, @NEW_VIEW_NAME
			,  CONTROL_INDEX     
			,  CONTROL_TYPE      
			,  DEFAULT_VIEW      
			, @MODULE_NAME       
			,  MODULE_ACCESS_TYPE
			,  TARGET_NAME       
			,  TARGET_ACCESS_TYPE
			,  MOBILE_ONLY       
			,  ADMIN_ONLY        
			,  EXCLUDE_MOBILE    
			,  HIDDEN            
			,  CONTROL_TEXT      
			,  CONTROL_TOOLTIP   
			,  CONTROL_ACCESSKEY 
			,  CONTROL_CSSCLASS  
			,  TEXT_FIELD        
			,  ARGUMENT_FIELD    
			,  COMMAND_NAME      
			,  URL_FORMAT        
			,  URL_TARGET        
			,  ONCLICK_SCRIPT    
		  from DYNAMIC_BUTTONS
		 where VIEW_NAME = @SOURCE_VIEW_NAME
		   and DELETED   = 0;
	end -- if;
  end
GO

Grant Execute on dbo.spDYNAMIC_BUTTONS_CopyDefault to public;
GO

