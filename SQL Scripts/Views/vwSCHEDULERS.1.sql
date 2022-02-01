if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwSCHEDULERS')
	Drop View dbo.vwSCHEDULERS;
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
-- 08/01/2010 Paul.  Add CREATED_BY_NAME and MODIFIED_BY_NAME so that we can display the full name in lists like Sugar. 
Create View dbo.vwSCHEDULERS
as
select SCHEDULERS.ID
     , SCHEDULERS.NAME
     , SCHEDULERS.JOB
     , SCHEDULERS.DATE_TIME_START
     , SCHEDULERS.DATE_TIME_END
     , SCHEDULERS.JOB_INTERVAL
     , SCHEDULERS.TIME_FROM
     , SCHEDULERS.TIME_TO
     , SCHEDULERS.LAST_RUN
     , SCHEDULERS.STATUS
     , SCHEDULERS.CATCH_UP
     , SCHEDULERS.DATE_ENTERED
     , SCHEDULERS.DATE_MODIFIED
     , USERS_CREATED_BY.USER_NAME  as CREATED_BY
     , USERS_MODIFIED_BY.USER_NAME as MODIFIED_BY
     , SCHEDULERS.CREATED_BY       as CREATED_BY_ID
     , dbo.fnFullName(USERS_CREATED_BY.FIRST_NAME , USERS_CREATED_BY.LAST_NAME ) as CREATED_BY_NAME
     , dbo.fnFullName(USERS_MODIFIED_BY.FIRST_NAME, USERS_MODIFIED_BY.LAST_NAME) as MODIFIED_BY_NAME
  from            SCHEDULERS
  left outer join USERS                      USERS_CREATED_BY
               on USERS_CREATED_BY.ID      = SCHEDULERS.CREATED_BY
  left outer join USERS                      USERS_MODIFIED_BY
               on USERS_MODIFIED_BY.ID     = SCHEDULERS.MODIFIED_USER_ID
 where SCHEDULERS.DELETED = 0

GO

Grant Select on dbo.vwSCHEDULERS to public;
GO

