if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spACL_ROLES_USERS_Update' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spACL_ROLES_USERS_Update;
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
-- 11/13/2009 Paul.  Remove the unnecessary update as it will reduce offline client conflicts. 
-- 05/05/2016 Paul.  The User Primary Role is used with role-based views. 
-- 03/22/2017 Paul.  Update the custom field table so that the audit view will have matching custom field values. 
Create Procedure dbo.spACL_ROLES_USERS_Update
	( @MODIFIED_USER_ID  uniqueidentifier
	, @ROLE_ID           uniqueidentifier
	, @USER_ID           uniqueidentifier
	)
as
  begin
	set nocount on
	
	declare @ID uniqueidentifier;
	-- 04/26/2006 Paul.  @ACCESS_OVERRIDE is not used yet. 
	-- BEGIN Oracle Exception
		select @ID = ID
		  from ACL_ROLES_USERS
		 where ROLE_ID           = @ROLE_ID
		   and USER_ID           = @USER_ID
		   and DELETED           = 0;
	-- END Oracle Exception
	
	if dbo.fnIsEmptyGuid(@ID) = 1 begin -- then
		set @ID = newid();
		insert into ACL_ROLES_USERS
			( ID               
			, CREATED_BY       
			, DATE_ENTERED     
			, MODIFIED_USER_ID 
			, DATE_MODIFIED    
			, ROLE_ID          
			, USER_ID          
			)
		values
			( @ID               
			, @MODIFIED_USER_ID 
			,  getdate()        
			, @MODIFIED_USER_ID 
			,  getdate()        
			, @ROLE_ID          
			, @USER_ID          
			);

		-- 05/05/2016 Paul.  Add the primary role if unassigned. 
		if exists(select * from USERS where ID = @USER_ID and PRIMARY_ROLE_ID is null and DELETED = 0) begin -- then
			-- BEGIN Oracle Exception
				update USERS
				   set PRIMARY_ROLE_ID   = @ROLE_ID
				     , DATE_MODIFIED     = getdate()
				     , DATE_MODIFIED_UTC = getutcdate()
				     , MODIFIED_USER_ID  = @MODIFIED_USER_ID
				 where ID                = @USER_ID
				   and DELETED           = 0;
			-- END Oracle Exception
			-- 03/22/2017 Paul.  Update the custom field table so that the audit view will have matching custom field values. 
			-- BEGIN Oracle Exception
				update USERS_CSTM
				   set ID_C              = ID_C
				 where ID_C              = @USER_ID;
			-- END Oracle Exception
		end -- if;
	end -- if;
  end
GO
 
Grant Execute on dbo.spACL_ROLES_USERS_Update to public;
GO
 
