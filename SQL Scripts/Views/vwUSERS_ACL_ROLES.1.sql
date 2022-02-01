if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwUSERS_ACL_ROLES')
	Drop View dbo.vwUSERS_ACL_ROLES;
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
-- 05/05/2016 Paul.  The User Primary Role is used with role-based views. 
Create View dbo.vwUSERS_ACL_ROLES
as
select USERS.ID              as USER_ID
     , ACL_ROLES.ID          as ROLE_ID
     , ACL_ROLES.NAME        as ROLE_NAME
     , ACL_ROLES.DESCRIPTION
     , ACL_ROLES_USERS.DATE_ENTERED
     , (case when USERS.PRIMARY_ROLE_ID = ACL_ROLES.ID then 1 else 0 end) as IS_PRIMARY_ROLE
  from           USERS
      inner join ACL_ROLES_USERS
              on ACL_ROLES_USERS.USER_ID = USERS.ID
             and ACL_ROLES_USERS.DELETED = 0
      inner join ACL_ROLES
              on ACL_ROLES.ID            = ACL_ROLES_USERS.ROLE_ID
             and ACL_ROLES.DELETED       = 0
 where USERS.DELETED = 0

GO

Grant Select on dbo.vwUSERS_ACL_ROLES to public;
GO

