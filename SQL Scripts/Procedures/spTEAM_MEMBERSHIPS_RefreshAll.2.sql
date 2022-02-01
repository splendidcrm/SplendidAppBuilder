if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spTEAM_MEMBERSHIPS_RefreshAll' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spTEAM_MEMBERSHIPS_RefreshAll;
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
Create Procedure dbo.spTEAM_MEMBERSHIPS_RefreshAll
	( @MODIFIED_USER_ID  uniqueidentifier
	)
as
  begin
	set nocount on

	declare @TEAM_ID uniqueidentifier;

	declare team_cursor cursor for
	select ID
	  from TEAMS
	 where DELETED = 0;

/* -- #if IBM_DB2
	declare continue handler for not found
		set in_FETCH_STATUS = 1;
-- #endif IBM_DB2 */
/* -- #if MySQL
	declare continue handler for not found
		set in_FETCH_STATUS = 1;
	set in_FETCH_STATUS = 0;
-- #endif MySQL */

	open team_cursor;
	fetch next from team_cursor into @TEAM_ID;
	-- 11/19/2006 Paul.  Stop if an error is found. 
	while @@FETCH_STATUS = 0 and @@ERROR = 0 begin -- do
		exec dbo.spTEAM_MEMBERSHIPS_UpdateImplicit @MODIFIED_USER_ID, @TEAM_ID;
		fetch next from team_cursor into @TEAM_ID;
	end -- while;
	close team_cursor;

	deallocate team_cursor;
  end
GO
 
Grant Execute on dbo.spTEAM_MEMBERSHIPS_RefreshAll to public;
GO
 

