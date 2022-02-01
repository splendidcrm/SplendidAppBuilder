if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spUSERS_Undelete' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spUSERS_Undelete;
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
Create Procedure dbo.spUSERS_Undelete
	( @ID               uniqueidentifier
	, @MODIFIED_USER_ID uniqueidentifier
	, @AUDIT_TOKEN      varchar(255)
	)
as
  begin
	set nocount on
	
	-- BEGIN Oracle Exception
		update PROSPECT_LISTS_PROSPECTS
		   set DELETED          = 0
		     , DATE_MODIFIED    = getdate()
		     , DATE_MODIFIED_UTC= getutcdate()
		     , MODIFIED_USER_ID = @MODIFIED_USER_ID
		 where RELATED_ID       = @ID
		   and DELETED          = 1
		   and ID in (select ID from PROSPECT_LISTS_PROSPECTS_AUDIT where AUDIT_TOKEN = @AUDIT_TOKEN and RELATED_ID = @ID);
	-- END Oracle Exception
	
	-- BEGIN Oracle Exception
		update CALLS_USERS
		   set DELETED          = 0
		     , DATE_MODIFIED    = getdate()
		     , DATE_MODIFIED_UTC= getutcdate()
		     , MODIFIED_USER_ID = @MODIFIED_USER_ID
		 where USER_ID          = @ID
		   and DELETED          = 1
		   and ID in (select ID from CALLS_USERS_AUDIT where AUDIT_TOKEN = @AUDIT_TOKEN and USER_ID = @ID);
	-- END Oracle Exception
	
	-- BEGIN Oracle Exception
		update CONTACTS_USERS
		   set DELETED          = 0
		     , DATE_MODIFIED    = getdate()
		     , DATE_MODIFIED_UTC= getutcdate()
		     , MODIFIED_USER_ID = @MODIFIED_USER_ID
		 where USER_ID          = @ID
		   and DELETED          = 1
		   and ID in (select ID from CONTACTS_USERS_AUDIT where AUDIT_TOKEN = @AUDIT_TOKEN and USER_ID = @ID);
	-- END Oracle Exception
	
	/*
	-- BEGIN Oracle Exception
		-- 11/13/2005 Paul.  Not sure if it makes sense to delete email relationships as they amount to a log.
		update EMAILMAN
		   set DELETED          = 0
		     , DATE_MODIFIED    = getdate()
		     , DATE_MODIFIED_UTC= getutcdate()
		     , MODIFIED_USER_ID = @MODIFIED_USER_ID
		 where USER_ID          = @ID
		   and DELETED          = 1
		   and ID in (select ID from EMAILMAN_AUDIT where AUDIT_TOKEN = @AUDIT_TOKEN and USER_ID = @ID);
	-- END Oracle Exception
	
	-- BEGIN Oracle Exception
		update EMAILMAN_SENT
		   set DELETED          = 0
		     , DATE_MODIFIED    = getdate()
		     , DATE_MODIFIED_UTC= getutcdate()
		     , MODIFIED_USER_ID = @MODIFIED_USER_ID
		 where USER_ID          = @ID
		   and DELETED          = 1
		   and ID in (select ID from EMAILMAN_SENT_AUDIT where AUDIT_TOKEN = @AUDIT_TOKEN and USER_ID = @ID);
	-- END Oracle Exception
	
	-- BEGIN Oracle Exception
		update EMAILS_USERS
		   set DELETED          = 0
		     , DATE_MODIFIED    = getdate()
		     , DATE_MODIFIED_UTC= getutcdate()
		     , MODIFIED_USER_ID = @MODIFIED_USER_ID
		 where USER_ID          = @ID
		   and DELETED          = 1
		   and ID in (select ID from EMAILS_USERS_AUDIT where AUDIT_TOKEN = @AUDIT_TOKEN and USER_ID = @ID);
	-- END Oracle Exception
	*/
	-- BEGIN Oracle Exception
		update MEETINGS_USERS
		   set DELETED          = 0
		     , DATE_MODIFIED    = getdate()
		     , DATE_MODIFIED_UTC= getutcdate()
		     , MODIFIED_USER_ID = @MODIFIED_USER_ID
		 where USER_ID          = @ID
		   and DELETED          = 1
		   and ID in (select ID from MEETINGS_USERS_AUDIT where AUDIT_TOKEN = @AUDIT_TOKEN and USER_ID = @ID);
	-- END Oracle Exception
	
	-- 08/08/2013 Paul.  USERS_FEEDS is not audited. 
	/*
	-- BEGIN Oracle Exception
		update USERS_FEEDS
		   set DELETED          = 0
		     , DATE_MODIFIED    = getdate()
		     , DATE_MODIFIED_UTC= getutcdate()
		     , MODIFIED_USER_ID = @MODIFIED_USER_ID
		 where USER_ID          = @ID
		   and DELETED          = 1
		   and ID in (select ID from USERS_FEEDS_AUDIT where AUDIT_TOKEN = @AUDIT_TOKEN and USER_ID = @ID);
	-- END Oracle Exception
	*/
	
	exec dbo.spPARENT_Undelete @ID, @MODIFIED_USER_ID, @AUDIT_TOKEN, N'Users';

	-- 08/07/2013 Paul.  Team Memberships are not audited, so we cannot undelete them. 
	-- This should not be an issue as we do not allow users to be deleted from the Admin panel 
	-- so there will be little need to undelete a user. 
	/*
	if dbo.fnCONFIG_Boolean(N'enable_team_management') = 1 begin -- then
		-- BEGIN Oracle Exception
			update TEAM_MEMBERSHIPS
			   set DELETED          = 0
			     , DATE_MODIFIED    = getdate()
			     , DATE_MODIFIED_UTC= getutcdate()
			     , MODIFIED_USER_ID = @MODIFIED_USER_ID
			 where USER_ID          = @ID
			   and DELETED          = 1
			   and ID in (select ID from TEAM_MEMBERSHIPS_AUDIT where AUDIT_TOKEN = @AUDIT_TOKEN and USER_ID = @ID);
		-- END Oracle Exception
	end -- if;
	*/
	
	-- BEGIN Oracle Exception
		-- 01/30/2019 Paul.  We should be creating the matching custom audit record. 
		update USERS_CSTM
		   set ID_C             = ID_C
		 where ID_C in 
			(select ID
			   from USERS
			 where ID               = @ID
			   and DELETED          = 1
			   and ID in (select ID from USERS_AUDIT where AUDIT_TOKEN = @AUDIT_TOKEN and ID = @ID)
			);
		update USERS
		   set DELETED          = 0
		     , DATE_MODIFIED    = getdate()
		     , DATE_MODIFIED_UTC= getutcdate()
		     , MODIFIED_USER_ID = @MODIFIED_USER_ID
		 where ID               = @ID
		   and DELETED          = 1
		   and ID in (select ID from USERS_AUDIT where AUDIT_TOKEN = @AUDIT_TOKEN and ID = @ID);
	-- END Oracle Exception
  end
GO
 
Grant Execute on dbo.spUSERS_Undelete to public;
GO
 
 
