if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwTEAM_MEMBERSHIPS_List')
	Drop View dbo.vwTEAM_MEMBERSHIPS_List;
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
Create View dbo.vwTEAM_MEMBERSHIPS_List
as
select TEAMS.ID   as TEAM_ID
     , TEAMS.NAME as TEAM_NAME
     , USERS.ID   as USER_ID
     , dbo.fnFullName(USERS.FIRST_NAME, USERS.LAST_NAME) as FULL_NAME
     , USERS.USER_NAME
     , USERS.EMAIL1
     , USERS.PHONE_WORK
     , TEAM_MEMBERSHIPS.DATE_ENTERED
     , TEAM_MEMBERSHIPS.EXPLICIT_ASSIGN
     , TEAM_MEMBERSHIPS.IMPLICIT_ASSIGN
  from           TEAMS
      inner join TEAM_MEMBERSHIPS
              on TEAM_MEMBERSHIPS.TEAM_ID = TEAMS.ID
             and TEAM_MEMBERSHIPS.DELETED = 0
      inner join USERS
              on USERS.ID                 = TEAM_MEMBERSHIPS.USER_ID
             and USERS.DELETED            = 0
 where TEAMS.DELETED = 0

GO

Grant Select on dbo.vwTEAM_MEMBERSHIPS_List to public;
GO

