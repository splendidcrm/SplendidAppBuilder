if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwACL_ACCESS_ByRole')
	Drop View dbo.vwACL_ACCESS_ByRole;
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
-- 12/05/2006 Paul.  Literals should be in unicode to reduce conversions at runtime. 
-- 12/05/2006 Paul.  iFrames should not be excluded because the My Portal tab can be disabled and edited. 
-- 02/03/2009 Paul.  Exclude Teams from role management. 
-- 03/09/2010 Paul.  Allow IS_ADMIN and Team Management so that they can be managed separately. 
-- 04/17/2016 Paul.  Allow Calendar to be disabled. 
-- 09/26/2017 Paul.  Add Archive access right. 
Create View dbo.vwACL_ACCESS_ByRole
as
select vwMODULES_ACL_ROLES_Cross.MODULE_NAME
     , vwMODULES_ACL_ROLES_Cross.DISPLAY_NAME
     , vwMODULES_ACL_ROLES_Cross.ROLE_ID
     , isnull((select min(ACLACCESS) from vwACL_ROLES_ACTIONS where ROLE_ID = vwMODULES_ACL_ROLES_Cross.ROLE_ID and CATEGORY = vwMODULES_ACL_ROLES_Cross.MODULE_NAME and NAME = N'admin' ),  1) as ACLACCESS_ADMIN 
     , isnull((select min(ACLACCESS) from vwACL_ROLES_ACTIONS where ROLE_ID = vwMODULES_ACL_ROLES_Cross.ROLE_ID and CATEGORY = vwMODULES_ACL_ROLES_Cross.MODULE_NAME and NAME = N'access'), 89) as ACLACCESS_ACCESS
     , isnull((select min(ACLACCESS) from vwACL_ROLES_ACTIONS where ROLE_ID = vwMODULES_ACL_ROLES_Cross.ROLE_ID and CATEGORY = vwMODULES_ACL_ROLES_Cross.MODULE_NAME and NAME = N'view'  ), 90) as ACLACCESS_VIEW  
     , isnull((select min(ACLACCESS) from vwACL_ROLES_ACTIONS where ROLE_ID = vwMODULES_ACL_ROLES_Cross.ROLE_ID and CATEGORY = vwMODULES_ACL_ROLES_Cross.MODULE_NAME and NAME = N'list'  ), 90) as ACLACCESS_LIST  
     , isnull((select min(ACLACCESS) from vwACL_ROLES_ACTIONS where ROLE_ID = vwMODULES_ACL_ROLES_Cross.ROLE_ID and CATEGORY = vwMODULES_ACL_ROLES_Cross.MODULE_NAME and NAME = N'edit'  ), 90) as ACLACCESS_EDIT  
     , isnull((select min(ACLACCESS) from vwACL_ROLES_ACTIONS where ROLE_ID = vwMODULES_ACL_ROLES_Cross.ROLE_ID and CATEGORY = vwMODULES_ACL_ROLES_Cross.MODULE_NAME and NAME = N'delete'), 90) as ACLACCESS_DELETE
     , isnull((select min(ACLACCESS) from vwACL_ROLES_ACTIONS where ROLE_ID = vwMODULES_ACL_ROLES_Cross.ROLE_ID and CATEGORY = vwMODULES_ACL_ROLES_Cross.MODULE_NAME and NAME = N'import'), 90) as ACLACCESS_IMPORT
     , isnull((select min(ACLACCESS) from vwACL_ROLES_ACTIONS where ROLE_ID = vwMODULES_ACL_ROLES_Cross.ROLE_ID and CATEGORY = vwMODULES_ACL_ROLES_Cross.MODULE_NAME and NAME = N'export'), 90) as ACLACCESS_EXPORT
     , isnull((select min(ACLACCESS) from vwACL_ROLES_ACTIONS where ROLE_ID = vwMODULES_ACL_ROLES_Cross.ROLE_ID and CATEGORY = vwMODULES_ACL_ROLES_Cross.MODULE_NAME and NAME = N'archive'), 90) as ACLACCESS_ARCHIVE
     , vwMODULES_ACL_ROLES_Cross.IS_ADMIN
  from vwMODULES_ACL_ROLES_Cross
 where vwMODULES_ACL_ROLES_Cross.MODULE_ENABLED = 1
   and vwMODULES_ACL_ROLES_Cross.MODULE_NAME not in (N'Activities', N'Home')
GO

Grant Select on dbo.vwACL_ACCESS_ByRole to public;
GO


