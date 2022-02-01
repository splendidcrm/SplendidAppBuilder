if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spSAVED_SEARCH_Update' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spSAVED_SEARCH_Update;
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
-- 12/14/2007 Paul.  When updating, the NAME is only updated if not null.  Module is not updated. 
-- 12/17/2007 Paul.  There can only be one entry for the default of the module. 
-- 07/29/2008 Paul.  Don't updated ASSIGNED_USER_ID.  This will prevent global searches from being over-written. 
-- 12/16/2008 Paul.  When looking for the default view, we need to include the ASSIGNED_USER_ID. 
-- 09/15/2009 Paul.  Convert data type to nvarchar(max) to support Azure. 
-- 09/01/2010 Paul.  Store a copy of the DEFAULT_SEARCH_ID in the table so that we don't need to read the XML in order to get the value. 
Create Procedure dbo.spSAVED_SEARCH_Update
	( @ID                uniqueidentifier output
	, @MODIFIED_USER_ID  uniqueidentifier
	, @ASSIGNED_USER_ID  uniqueidentifier
	, @NAME              nvarchar(150)
	, @SEARCH_MODULE     nvarchar(150)
	, @CONTENTS          nvarchar(max)
	, @DESCRIPTION       nvarchar(max)
	, @DEFAULT_SEARCH_ID uniqueidentifier = null
	)
as
  begin
	set nocount on

	if dbo.fnIsEmptyGuid(@ID) = 1 and @NAME is null begin -- then
		-- BEGIN Oracle Exception
			select @ID = ID
			  from SAVED_SEARCH
			 where SEARCH_MODULE    = @SEARCH_MODULE
			   and ASSIGNED_USER_ID = @ASSIGNED_USER_ID
			   and NAME             is null
			   and DELETED          = 0;
		-- END Oracle Exception
	end -- if;
	
	if not exists(select * from SAVED_SEARCH where ID = @ID) begin -- then
		if dbo.fnIsEmptyGuid(@ID) = 1 begin -- then
			set @ID = newid();
		end -- if;
		insert into SAVED_SEARCH
			( ID               
			, CREATED_BY       
			, DATE_ENTERED     
			, MODIFIED_USER_ID 
			, DATE_MODIFIED    
			, ASSIGNED_USER_ID 
			, DEFAULT_SEARCH_ID
			, NAME             
			, SEARCH_MODULE    
			, CONTENTS         
			, DESCRIPTION      
			)
		values 	( @ID               
			, @MODIFIED_USER_ID 
			,  getdate()        
			, @MODIFIED_USER_ID 
			,  getdate()        
			, @ASSIGNED_USER_ID 
			, @DEFAULT_SEARCH_ID
			, @NAME             
			, @SEARCH_MODULE    
			, @CONTENTS         
			, @DESCRIPTION      
			);
	end else begin
		update SAVED_SEARCH
		   set MODIFIED_USER_ID  = @MODIFIED_USER_ID 
		     , DATE_MODIFIED     =  getdate()        
		     , DATE_MODIFIED_UTC =  getutcdate()     
-- 07/29/2008 Paul.  Don't updated ASSIGNED_USER_ID.  This will prevent global searches from being over-written. 
--		     , ASSIGNED_USER_ID  = @ASSIGNED_USER_ID 
		     , DEFAULT_SEARCH_ID = @DEFAULT_SEARCH_ID
		     , NAME              = isnull(@NAME, NAME)
		     , CONTENTS          = @CONTENTS         
		     , DESCRIPTION       = @DESCRIPTION      
		 where ID                = @ID               ;
	end -- if;
  end
GO

Grant Execute on dbo.spSAVED_SEARCH_Update to public;
GO

