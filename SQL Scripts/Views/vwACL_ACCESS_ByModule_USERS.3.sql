if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwACL_ACCESS_ByModule_USERS')
	Drop View dbo.vwACL_ACCESS_ByModule_USERS;
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
-- 04/29/2006 Paul.  DB2 has a problem with cross joins, so place in a view so that it can easily be converted. 
-- 03/09/2010 Paul.  Allow IS_ADMIN and Team Management so that they can be managed separately. 
-- 09/26/2017 Paul.  Add Archive access right. 
Create View dbo.vwACL_ACCESS_ByModule_USERS
as
select vwACL_ACCESS_ByModule.MODULE_NAME
     , vwACL_ACCESS_ByModule.DISPLAY_NAME
     , vwACL_ACCESS_ByModule.ACLACCESS_ADMIN 
     , vwACL_ACCESS_ByModule.ACLACCESS_ACCESS
     , vwACL_ACCESS_ByModule.ACLACCESS_VIEW  
     , vwACL_ACCESS_ByModule.ACLACCESS_LIST  
     , vwACL_ACCESS_ByModule.ACLACCESS_EDIT  
     , vwACL_ACCESS_ByModule.ACLACCESS_DELETE
     , vwACL_ACCESS_ByModule.ACLACCESS_IMPORT
     , vwACL_ACCESS_ByModule.ACLACCESS_EXPORT
     , vwACL_ACCESS_ByModule.ACLACCESS_ARCHIVE
     , USERS.ID           as USER_ID
     , vwACL_ACCESS_ByModule.IS_ADMIN
  from      vwACL_ACCESS_ByModule
 cross join USERS
 where USERS.DELETED   = 0

GO

Grant Select on dbo.vwACL_ACCESS_ByModule_USERS to public;
GO

