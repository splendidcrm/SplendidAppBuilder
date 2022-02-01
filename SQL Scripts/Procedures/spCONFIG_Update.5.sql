if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spCONFIG_Update' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spCONFIG_Update;
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
-- 09/28/2008 Paul.  max_users is a protected config value that cannot be edited by an admin. 
-- 09/15/2009 Paul.  Convert data type to nvarchar(max) to support Azure. 
Create Procedure dbo.spCONFIG_Update
	( @MODIFIED_USER_ID  uniqueidentifier
	, @CATEGORY          nvarchar(32)
	, @NAME              nvarchar(60)
	, @VALUE             nvarchar(max)
	)
as
  begin
	set nocount on
	
	declare @ID uniqueidentifier;
	-- BEGIN Oracle Exception
		select @ID = ID
		  from CONFIG
		 where NAME = @NAME 
		   and DELETED = 0;
	-- END Oracle Exception
	if dbo.fnIsEmptyGuid(@ID) = 1 begin -- then
		set @ID = newid();
		insert into CONFIG
			( ID               
			, CREATED_BY       
			, DATE_ENTERED     
			, MODIFIED_USER_ID 
			, DATE_MODIFIED    
			, CATEGORY         
			, NAME             
			, VALUE            
			)
		values
			( @ID               
			, @MODIFIED_USER_ID 
			,  getdate()        
			, @MODIFIED_USER_ID 
			,  getdate()        
			, @CATEGORY         
			, @NAME             
			, @VALUE            
			);
	end else begin
		-- 09/28/2008 Paul.  max_users can be inserted, but it cannot be updated. 
		if lower(@NAME) <> N'max_users' begin -- then
			update CONFIG
			   set MODIFIED_USER_ID  = @MODIFIED_USER_ID 
			     , DATE_MODIFIED     =  getdate()        
			     , DATE_MODIFIED_UTC =  getutcdate()     
			     , CATEGORY          = @CATEGORY         
			     , NAME              = @NAME             
			     , VALUE             = @VALUE            
			 where ID                = @ID               ;
		end -- if;
	end -- if;

	-- 09/20/2007 Paul.  Create private teams when enabling team management. 
	if @NAME = N'enable_team_management' begin -- then
		if dbo.fnCONFIG_Boolean(@NAME) = 1 begin -- then
			-- 09/14/2008 Paul.  A single space after the procedure simplifies the migration to DB2. 
			exec dbo.spTEAMS_InitPrivate ;
		end -- if;
	end -- if;
  end
GO

Grant Execute on dbo.spCONFIG_Update to public;
GO

