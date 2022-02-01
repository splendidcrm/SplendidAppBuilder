if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spTEAM_MEMBERSHIPS_Delete' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spTEAM_MEMBERSHIPS_Delete;
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
-- 08/07/2013 Paul.  Add Oracle Exception. 
-- 02/22/2015 Paul.  When removing a user from a team, also remove default team for that user if it is the same team. 
Create Procedure dbo.spTEAM_MEMBERSHIPS_Delete
	( @MODIFIED_USER_ID uniqueidentifier
	, @TEAM_ID          uniqueidentifier
	, @USER_ID          uniqueidentifier
	)
as
  begin
	set nocount on
	
	-- BEGIN Oracle Exception
		update TEAM_MEMBERSHIPS
		   set DELETED          = 1
		     , DATE_MODIFIED    = getdate()
		     , DATE_MODIFIED_UTC= getutcdate()
		     , MODIFIED_USER_ID = @MODIFIED_USER_ID
		 where TEAM_ID          = @TEAM_ID
		   and USER_ID          = @USER_ID
		   and DELETED          = 0;
	-- END Oracle Exception

	-- 11/18/2006 Paul.  Refresh all the implicit assignments any time a member is removed. 
	-- Just make sure not to use spTEAM_MEMBERSHIPS_Delete inside spTEAM_MEMBERSHIPS_UpdateImplicit. 
	exec dbo.spTEAM_MEMBERSHIPS_UpdateImplicit @MODIFIED_USER_ID, @TEAM_ID;

	-- 02/22/2015 Paul.  When removing a user from a team, also remove default team for that user if it is the same team. 
	-- This will prevent records created by this user from being accessed by this same user. 
	if exists(select * from USERS where ID = @USER_ID and DELETED = 0 and DEFAULT_TEAM = @TEAM_ID) begin -- then
		update USERS
		   set DEFAULT_TEAM      = null
		     , DATE_MODIFIED     = getdate()
		     , DATE_MODIFIED_UTC = getutcdate()
		     , MODIFIED_USER_ID  = @MODIFIED_USER_ID
		 where ID                = @USER_ID
		   and DEFAULT_TEAM      = @TEAM_ID
		   and DELETED           = 0;
	end -- if;
  end
GO

Grant Execute on dbo.spTEAM_MEMBERSHIPS_Delete to public;
GO

