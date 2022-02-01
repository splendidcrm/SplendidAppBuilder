if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spSHORTCUTS_InsertOnly' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spSHORTCUTS_InsertOnly;
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
-- 04/28/2006 Paul.  Added SHORTCUT_MODULE to help with ACL. 
-- 04/28/2006 Paul.  Added SHORTCUT_ACLTYPE to help with ACL. 
-- 07/24/2006 Paul.  Increase the DISPLAY_NAME to 150 to allow a fully-qualified (NAME+MODULE_NAME+LIST_NAME) TERMINOLOGY name. 
-- 06/02/2012 Paul.  Auto-number the shortcut order. 
-- 09/28/2015 Paul.  Also allow -1 to indicate auto-number. 
Create Procedure dbo.spSHORTCUTS_InsertOnly
	( @MODIFIED_USER_ID  uniqueidentifier
	, @MODULE_NAME       nvarchar( 25)
	, @DISPLAY_NAME      nvarchar(150)
	, @RELATIVE_PATH     nvarchar(255)
	, @IMAGE_NAME        nvarchar( 50)
	, @SHORTCUT_ENABLED  bit
	, @SHORTCUT_ORDER    int
	, @SHORTCUT_MODULE   nvarchar( 25)
	, @SHORTCUT_ACLTYPE  nvarchar(100)
	)
as
  begin
	set nocount on
	
	declare @ID uniqueidentifier;
	-- 09/28/2015 Paul.  Oracle typically has an issue with modifying input parameters. 
	declare @TEMP_SHORTCUT_ORDER    int;
	set @TEMP_SHORTCUT_ORDER = @SHORTCUT_ORDER;
	if @TEMP_SHORTCUT_ORDER is null or @TEMP_SHORTCUT_ORDER = -1 begin -- then
		-- BEGIN Oracle Exception
			select @TEMP_SHORTCUT_ORDER = isnull(max(SHORTCUT_ORDER) + 1, 0)
			  from vwSHORTCUTS
			 where MODULE_NAME   = @MODULE_NAME  ;
		-- END Oracle Exception
	end -- if;
	
	-- BEGIN Oracle Exception
		select @ID = ID
		  from SHORTCUTS
		 where MODULE_NAME   = @MODULE_NAME  
		   and DISPLAY_NAME  = @DISPLAY_NAME 
		   and RELATIVE_PATH = @RELATIVE_PATH
		   and DELETED       = 0             ;
	-- END Oracle Exception
	if dbo.fnIsEmptyGuid(@ID) = 1 begin -- then
		set @ID = newid();
		insert into SHORTCUTS
			( ID               
			, CREATED_BY       
			, DATE_ENTERED     
			, MODIFIED_USER_ID 
			, DATE_MODIFIED    
			, MODULE_NAME      
			, DISPLAY_NAME     
			, RELATIVE_PATH    
			, IMAGE_NAME       
			, SHORTCUT_ENABLED 
			, SHORTCUT_ORDER   
			, SHORTCUT_MODULE  
			, SHORTCUT_ACLTYPE 
			)
		values 
			( @ID               
			, @MODIFIED_USER_ID 
			,  getdate()        
			, @MODIFIED_USER_ID 
			,  getdate()        
			, @MODULE_NAME      
			, @DISPLAY_NAME     
			, @RELATIVE_PATH    
			, @IMAGE_NAME       
			, @SHORTCUT_ENABLED 
			, @TEMP_SHORTCUT_ORDER
			, @SHORTCUT_MODULE  
			, @SHORTCUT_ACLTYPE 
			);
	end -- if;
  end
GO
 
Grant Execute on dbo.spSHORTCUTS_InsertOnly to public;
GO
 
