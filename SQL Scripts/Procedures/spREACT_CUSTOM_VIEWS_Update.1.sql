if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spREACT_CUSTOM_VIEWS_Update' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spREACT_CUSTOM_VIEWS_Update;
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
-- 12/06/2021 Paul.  spSUGARFAVORITES_UpdateName does not apply to REACT_CUSTOM_VIEWS. 
Create Procedure dbo.spREACT_CUSTOM_VIEWS_Update
	( @ID                 uniqueidentifier output
	, @MODIFIED_USER_ID   uniqueidentifier
	, @NAME               nvarchar(100)
	, @MODULE_NAME        nvarchar(50)
	, @CATEGORY           nvarchar(25)
	, @CONTENT            nvarchar(max)
	)
as
  begin
	set nocount on
	
	if exists(select * from REACT_CUSTOM_VIEWS where NAME = @NAME and MODULE_NAME = @MODULE_NAME and CATEGORY = @CATEGORY and (ID <> @ID or @ID is null)) begin -- then
		raiserror(N'spREACT_CUSTOM_VIEWS_Update: A custom view for module %s and category %s', 16, 1, @MODULE_NAME, @CATEGORY);
		return;
	end -- if;
	if not exists(select * from REACT_CUSTOM_VIEWS where ID = @ID) begin -- then
		if dbo.fnIsEmptyGuid(@ID) = 1 begin -- then
			set @ID = newid();
		end -- if;
		insert into REACT_CUSTOM_VIEWS
			( ID                
			, CREATED_BY        
			, DATE_ENTERED      
			, MODIFIED_USER_ID  
			, DATE_MODIFIED     
			, DATE_MODIFIED_UTC 
			, NAME              
			, MODULE_NAME       
			, CATEGORY          
			, CONTENT           
			)
		values 	( @ID                
			, @MODIFIED_USER_ID        
			,  getdate()         
			, @MODIFIED_USER_ID  
			,  getdate()         
			,  getutcdate()      
			, @NAME              
			, @MODULE_NAME       
			, @CATEGORY          
			, @CONTENT           
			);
	end else begin
		update REACT_CUSTOM_VIEWS
		   set MODIFIED_USER_ID   = @MODIFIED_USER_ID  
		     , DATE_MODIFIED      =  getdate()         
		     , DATE_MODIFIED_UTC  =  getutcdate()      
		     , NAME               = @NAME              
		     , MODULE_NAME        = @MODULE_NAME       
		     , CATEGORY           = @CATEGORY          
		     , CONTENT            = @CONTENT           
		 where ID                 = @ID                ;
		
		-- 12/06/2021 Paul.  spSUGARFAVORITES_UpdateName does not apply to REACT_CUSTOM_VIEWS. 
	end -- if;
  end
GO

Grant Execute on dbo.spREACT_CUSTOM_VIEWS_Update to public;
GO

