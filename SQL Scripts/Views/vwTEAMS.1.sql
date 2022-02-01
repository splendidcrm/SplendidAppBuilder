if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwTEAMS')
	Drop View dbo.vwTEAMS;
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
-- 10/28/2009 Paul.  Add UTC date to allow this table to sync. 
-- 08/01/2010 Paul.  Add CREATED_BY_NAME and MODIFIED_BY_NAME so that we can display the full name in lists like Sugar. 
-- 04/12/2016 Paul.  Add parent team and custom fields. 
Create View dbo.vwTEAMS
as
select TEAMS.ID
     , TEAMS.NAME
     , TEAMS.DESCRIPTION
     , TEAMS.PRIVATE
     , TEAMS.DATE_ENTERED
     , TEAMS.DATE_MODIFIED
     , TEAMS.DATE_MODIFIED_UTC
     , USERS_CREATED_BY.USER_NAME  as CREATED_BY
     , USERS_MODIFIED_BY.USER_NAME as MODIFIED_BY
     , dbo.fnFullName(USERS_CREATED_BY.FIRST_NAME , USERS_CREATED_BY.LAST_NAME ) as CREATED_BY_NAME
     , dbo.fnFullName(USERS_MODIFIED_BY.FIRST_NAME, USERS_MODIFIED_BY.LAST_NAME) as MODIFIED_BY_NAME
     , TEAMS.PARENT_ID
     , PARENT_TEAMS.NAME           as PARENT_NAME
     , TEAMS_CSTM.*
  from            TEAMS
  left outer join TEAMS                  PARENT_TEAMS
               on PARENT_TEAMS.ID      = TEAMS.PARENT_ID
              and PARENT_TEAMS.DELETED = 0
  left outer join TEAMS_CSTM
               on TEAMS_CSTM.ID_C      = TEAMS.ID
  left outer join USERS USERS_CREATED_BY
               on USERS_CREATED_BY.ID  = TEAMS.CREATED_BY
  left outer join USERS USERS_MODIFIED_BY
               on USERS_MODIFIED_BY.ID = TEAMS.MODIFIED_USER_ID
 where TEAMS.DELETED = 0

GO

Grant Select on dbo.vwTEAMS to public;
GO

 
