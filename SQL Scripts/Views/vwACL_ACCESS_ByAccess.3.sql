if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwACL_ACCESS_ByAccess')
	Drop View dbo.vwACL_ACCESS_ByAccess;
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
-- 03/09/2010 Paul.  Allow IS_ADMIN and Team Management so that they can be managed separately. 
-- 09/26/2017 Paul.  Add Archive access right. 
Create View dbo.vwACL_ACCESS_ByAccess
as
select USERS.ID as USER_ID
     , vwACL_ACCESS_ByRole.MODULE_NAME
     , vwACL_ACCESS_ByRole.DISPLAY_NAME
     , min(ACLACCESS_ADMIN ) as ACLACCESS_ADMIN 
     , min(ACLACCESS_ACCESS) as ACLACCESS_ACCESS
     , min(ACLACCESS_VIEW  ) as ACLACCESS_VIEW  
     , min(ACLACCESS_LIST  ) as ACLACCESS_LIST  
     , min(ACLACCESS_EDIT  ) as ACLACCESS_EDIT  
     , min(ACLACCESS_DELETE) as ACLACCESS_DELETE
     , min(ACLACCESS_IMPORT) as ACLACCESS_IMPORT
     , min(ACLACCESS_EXPORT) as ACLACCESS_EXPORT
     , min(ACLACCESS_ARCHIVE) as ACLACCESS_ARCHIVE
     , vwACL_ACCESS_ByRole.IS_ADMIN
  from       vwACL_ACCESS_ByRole
  inner join ACL_ROLES_USERS
          on ACL_ROLES_USERS.ROLE_ID = vwACL_ACCESS_ByRole.ROLE_ID
         and ACL_ROLES_USERS.DELETED = 0
  inner join USERS
          on USERS.ID                = ACL_ROLES_USERS.USER_ID
         and USERS.DELETED           = 0
 group by USERS.ID, vwACL_ACCESS_ByRole.MODULE_NAME, vwACL_ACCESS_ByRole.DISPLAY_NAME, vwACL_ACCESS_ByRole.IS_ADMIN
GO

Grant Select on dbo.vwACL_ACCESS_ByAccess to public;
GO


