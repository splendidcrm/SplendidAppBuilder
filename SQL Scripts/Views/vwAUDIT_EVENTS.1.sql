if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwAUDIT_EVENTS')
	Drop View dbo.vwAUDIT_EVENTS;
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
-- 01/20/2010 Paul.  Correct for the singular module names. 
-- 03/27/2019 Paul.  Every searchable view should have a NAME field. 
-- 11/13/2020 Paul.  Add DATE_ENTERED to support default view of React Client. 
Create View dbo.vwAUDIT_EVENTS
as
select dbo.fnFullName(USERS.FIRST_NAME, USERS.LAST_NAME) as FULL_NAME
     , USERS.USER_NAME
     , USERS.USER_NAME            as NAME
     , USERS.ID                   as USER_ID
     , AUDIT_EVENTS.DATE_ENTERED
     , AUDIT_EVENTS.DATE_MODIFIED
     , AUDIT_EVENTS.AUDIT_ID
     , AUDIT_EVENTS.AUDIT_TABLE
     , AUDIT_EVENTS.AUDIT_ACTION
     , AUDIT_EVENTS.AUDIT_PARENT_ID
     , MODULES.MODULE_NAME
     , (case MODULES.MODULE_NAME
        when N'Project'     then N'Projects'
        when N'ProjectTask' then N'ProjectTasks'
        else MODULES.MODULE_NAME
        end) as MODULE_FOLDER
  from      AUDIT_EVENTS
 inner join USERS
         on USERS.ID           = AUDIT_EVENTS.MODIFIED_USER_ID
 inner join MODULES
         on MODULES.TABLE_NAME + N'_AUDIT' = AUDIT_EVENTS.AUDIT_TABLE

GO

Grant Select on dbo.vwAUDIT_EVENTS to public;
GO


