if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwACL_ROLES_USERS')
	Drop View dbo.vwACL_ROLES_USERS;
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
-- 12/07/2006 Paul.  Only show active users. 
Create View dbo.vwACL_ROLES_USERS
as
select ACL_ROLES.ID   as ROLE_ID
     , ACL_ROLES.NAME as ROLE_NAME
     , USERS.ID   as USER_ID
     , dbo.fnFullName(USERS.FIRST_NAME, USERS.LAST_NAME) as FULL_NAME
     , USERS.USER_NAME
     , USERS.EMAIL1
     , USERS.PHONE_WORK
     , ACL_ROLES_USERS.DATE_ENTERED
  from           ACL_ROLES
      inner join ACL_ROLES_USERS
              on ACL_ROLES_USERS.ROLE_ID = ACL_ROLES.ID
             and ACL_ROLES_USERS.DELETED = 0
      inner join USERS
              on USERS.ID                = ACL_ROLES_USERS.USER_ID
             and USERS.DELETED           = 0
 where ACL_ROLES.DELETED = 0
  and (USERS.STATUS is null or USERS.STATUS = N'Active')

GO

Grant Select on dbo.vwACL_ROLES_USERS to public;
GO

