if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwUSERS_LOGINS')
	Drop View dbo.vwUSERS_LOGINS;
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
-- 07/08/2018 Paul.  The ID is needed for export. 
-- 03/27/2019 Paul.  Every searchable view should have a NAME field. 
-- 10/30/2020 Paul.  DATE_ENTERED for the React Client. 
Create View dbo.vwUSERS_LOGINS
as
select USERS_LOGINS.ID
     , USERS_LOGINS.USER_ID
     , USERS_LOGINS.USER_NAME
     , USERS_LOGINS.LOGIN_TYPE
     , USERS_LOGINS.LOGIN_DATE
     , USERS_LOGINS.LOGOUT_DATE
     , USERS_LOGINS.LOGIN_STATUS
     , USERS_LOGINS.ASPNET_SESSIONID
     , USERS_LOGINS.REMOTE_HOST
     , USERS_LOGINS.TARGET
     , USERS_LOGINS.USER_AGENT
     , USERS_LOGINS.DATE_MODIFIED
     , USERS_LOGINS.DATE_ENTERED
     , vwUSERS.FULL_NAME
     , vwUSERS.IS_ADMIN
     , vwUSERS.STATUS
     , USERS_LOGINS.USER_NAME        as NAME
  from            USERS_LOGINS
  left outer join vwUSERS
               on vwUSERS.ID = USERS_LOGINS.USER_ID

GO

Grant Select on dbo.vwUSERS_LOGINS to public;
GO

