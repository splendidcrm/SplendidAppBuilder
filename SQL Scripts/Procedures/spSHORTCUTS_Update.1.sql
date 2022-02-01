if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spSHORTCUTS_Update' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spSHORTCUTS_Update;
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
Create Procedure dbo.spSHORTCUTS_Update
	( @ID                uniqueidentifier output
	, @MODIFIED_USER_ID  uniqueidentifier
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
	if not exists(select * from SHORTCUTS where ID = @ID) begin -- then
		if dbo.fnIsEmptyGuid(@ID) = 1 begin -- then
			set @ID = newid();
		end -- if;
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
			, @SHORTCUT_ORDER   
			, @SHORTCUT_MODULE  
			, @SHORTCUT_ACLTYPE 
			);
	end else begin
		update SHORTCUTS
		   set MODIFIED_USER_ID  = @MODIFIED_USER_ID 
		     , DATE_MODIFIED     =  getdate()        
		     , DATE_MODIFIED_UTC =  getutcdate()     
		     , MODULE_NAME       = @MODULE_NAME      
		     , DISPLAY_NAME      = @DISPLAY_NAME     
		     , RELATIVE_PATH     = @RELATIVE_PATH    
		     , IMAGE_NAME        = @IMAGE_NAME       
		     , SHORTCUT_ENABLED  = @SHORTCUT_ENABLED 
		     , SHORTCUT_ORDER    = @SHORTCUT_ORDER   
		     , SHORTCUT_MODULE   = @SHORTCUT_MODULE  
		     , SHORTCUT_ACLTYPE  = @SHORTCUT_ACLTYPE 
		 where ID                = @ID               ;
	end -- if;
  end
GO
 
Grant Execute on dbo.spSHORTCUTS_Update to public;
GO
 
