if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spACL_ROLES_ACTIONS_Update' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spACL_ROLES_ACTIONS_Update;
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
-- 12/17/2017 Paul.  Add helpful message. 
Create Procedure dbo.spACL_ROLES_ACTIONS_Update
	( @ID                uniqueidentifier output
	, @MODIFIED_USER_ID  uniqueidentifier
	, @ROLE_ID           uniqueidentifier
	, @ACTION_NAME       nvarchar(25)
	, @MODULE_NAME       nvarchar(25)
	, @ACCESS_OVERRIDE   int
	)
as
  begin
	set nocount on

	declare @ACTION_ID uniqueidentifier;

	-- BEGIN Oracle Exception
		select @ACTION_ID = ID
		  from ACL_ACTIONS
		 where NAME     = @ACTION_NAME
		   and CATEGORY = @MODULE_NAME
		   and DELETED  = 0           ;
	-- END Oracle Exception
	-- 12/17/2017 Paul.  Add helpful message. 
	if @ACTION_ID is null begin -- then
		raiserror(N'spACL_ROLES_ACTIONS_Update: Could not find action "%s" for module "%s".', 16, 1, @ACTION_NAME, @MODULE_NAME);
		return;
	end -- if;
	
	-- BEGIN Oracle Exception
		select @ID = ID
		  from ACL_ROLES_ACTIONS
		 where ROLE_ID   = @ROLE_ID  
		   and ACTION_ID = @ACTION_ID
		   and DELETED   = 0         ;
	-- END Oracle Exception
	if dbo.fnIsEmptyGuid(@ID) = 1 begin -- then
		set @ID = newid();
		insert into ACL_ROLES_ACTIONS
			( ID               
			, CREATED_BY       
			, DATE_ENTERED     
			, MODIFIED_USER_ID 
			, DATE_MODIFIED    
			, ROLE_ID          
			, ACTION_ID        
			, ACCESS_OVERRIDE  
			)
		values 	( @ID               
			, @MODIFIED_USER_ID       
			,  getdate()        
			, @MODIFIED_USER_ID 
			,  getdate()        
			, @ROLE_ID          
			, @ACTION_ID        
			, @ACCESS_OVERRIDE  
			);
	end else begin
		update ACL_ROLES_ACTIONS
		   set MODIFIED_USER_ID  = @MODIFIED_USER_ID 
		     , DATE_MODIFIED     =  getdate()        
		     , DATE_MODIFIED_UTC =  getutcdate()     
		     , ACCESS_OVERRIDE   = @ACCESS_OVERRIDE  
		 where ID                = @ID               ;
	end -- if;
  end
GO
 
Grant Execute on dbo.spACL_ROLES_ACTIONS_Update to public;
GO

