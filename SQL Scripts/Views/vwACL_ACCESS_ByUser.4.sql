if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwACL_ACCESS_ByUser')
	Drop View dbo.vwACL_ACCESS_ByUser;
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
Create View dbo.vwACL_ACCESS_ByUser
as
select vwACL_ACCESS_ByModule_USERS.USER_ID
     , vwACL_ACCESS_ByModule_USERS.MODULE_NAME
     , vwACL_ACCESS_ByModule_USERS.DISPLAY_NAME
     , (case when vwACL_ACCESS_ByAccess.ACLACCESS_ADMIN  is null then vwACL_ACCESS_ByModule_USERS.ACLACCESS_ADMIN  else vwACL_ACCESS_ByAccess.ACLACCESS_ADMIN  end) as ACLACCESS_ADMIN 
     , (case when vwACL_ACCESS_ByAccess.ACLACCESS_ACCESS is null then vwACL_ACCESS_ByModule_USERS.ACLACCESS_ACCESS else vwACL_ACCESS_ByAccess.ACLACCESS_ACCESS end) as ACLACCESS_ACCESS
     , (case when vwACL_ACCESS_ByAccess.ACLACCESS_VIEW   is null then vwACL_ACCESS_ByModule_USERS.ACLACCESS_VIEW   else vwACL_ACCESS_ByAccess.ACLACCESS_VIEW   end) as ACLACCESS_VIEW  
     , (case when vwACL_ACCESS_ByAccess.ACLACCESS_LIST   is null then vwACL_ACCESS_ByModule_USERS.ACLACCESS_LIST   else vwACL_ACCESS_ByAccess.ACLACCESS_LIST   end) as ACLACCESS_LIST  
     , (case when vwACL_ACCESS_ByAccess.ACLACCESS_EDIT   is null then vwACL_ACCESS_ByModule_USERS.ACLACCESS_EDIT   else vwACL_ACCESS_ByAccess.ACLACCESS_EDIT   end) as ACLACCESS_EDIT  
     , (case when vwACL_ACCESS_ByAccess.ACLACCESS_DELETE is null then vwACL_ACCESS_ByModule_USERS.ACLACCESS_DELETE else vwACL_ACCESS_ByAccess.ACLACCESS_DELETE end) as ACLACCESS_DELETE
     , (case when vwACL_ACCESS_ByAccess.ACLACCESS_IMPORT is null then vwACL_ACCESS_ByModule_USERS.ACLACCESS_IMPORT else vwACL_ACCESS_ByAccess.ACLACCESS_IMPORT end) as ACLACCESS_IMPORT
     , (case when vwACL_ACCESS_ByAccess.ACLACCESS_EXPORT is null then vwACL_ACCESS_ByModule_USERS.ACLACCESS_EXPORT else vwACL_ACCESS_ByAccess.ACLACCESS_EXPORT end) as ACLACCESS_EXPORT
     , (case when vwACL_ACCESS_ByAccess.ACLACCESS_ARCHIVE is null then vwACL_ACCESS_ByModule_USERS.ACLACCESS_ARCHIVE else vwACL_ACCESS_ByAccess.ACLACCESS_ARCHIVE end) as ACLACCESS_ARCHIVE
     , vwACL_ACCESS_ByModule_USERS.IS_ADMIN
  from            vwACL_ACCESS_ByModule_USERS
  left outer join vwACL_ACCESS_ByAccess
               on vwACL_ACCESS_ByAccess.USER_ID     = vwACL_ACCESS_ByModule_USERS.USER_ID
              and vwACL_ACCESS_ByAccess.MODULE_NAME = vwACL_ACCESS_ByModule_USERS.MODULE_NAME

GO

Grant Select on dbo.vwACL_ACCESS_ByUser to public;
GO


