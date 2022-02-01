if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwSHORTCUTS_Menu_ByUser')
	Drop View dbo.vwSHORTCUTS_Menu_ByUser;
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
-- 04/28/2006 Paul.  We need to look at both the access right and either the edit right or the list right. 
-- Although we could combine the union into a single query, it seems too complex. 
-- 09/09/2006 Paul.  Include import in types that can appear in shortcuts. 
-- 12/05/2006 Paul.  We need to filter on access rights for vwSHORTCUTS_USERS_Cross.SHORTCUT_MODULE, not the rights for the module being displayed. 
-- 12/05/2006 Paul.  Literals should be in unicode to reduce conversions at runtime. 
-- 09/26/2017 Paul.  Add Archive access right. 
Create View dbo.vwSHORTCUTS_Menu_ByUser
as
select vwSHORTCUTS_USERS_Cross.USER_ID
     , vwSHORTCUTS_USERS_Cross.MODULE_NAME
     , vwSHORTCUTS_USERS_Cross.DISPLAY_NAME
     , vwSHORTCUTS_USERS_Cross.RELATIVE_PATH
     , vwSHORTCUTS_USERS_Cross.IMAGE_NAME
     , vwSHORTCUTS_USERS_Cross.SHORTCUT_ORDER
     , vwSHORTCUTS_USERS_Cross.SHORTCUT_ACLTYPE
  from            vwSHORTCUTS_USERS_Cross
  left outer join vwACL_ACTIONS                       vwACL_ACTIONS_AccessOnly
               on vwACL_ACTIONS_AccessOnly.CATEGORY = vwSHORTCUTS_USERS_Cross.SHORTCUT_MODULE
              and vwACL_ACTIONS_AccessOnly.NAME     = N'access'
  left outer join vwACL_ACTIONS                       vwACL_ACTIONS_EditOnly
               on vwACL_ACTIONS_EditOnly.CATEGORY   = vwSHORTCUTS_USERS_Cross.SHORTCUT_MODULE
              and vwACL_ACTIONS_EditOnly.NAME       = N'edit'
  left outer join vwACL_ACCESS_ByAccess
               on vwACL_ACCESS_ByAccess.USER_ID     = vwSHORTCUTS_USERS_Cross.USER_ID
              and vwACL_ACCESS_ByAccess.MODULE_NAME = vwSHORTCUTS_USERS_Cross.SHORTCUT_MODULE
 where vwSHORTCUTS_USERS_Cross.SHORTCUT_ENABLED = 1
   and vwSHORTCUTS_USERS_Cross.SHORTCUT_ACLTYPE = N'edit'
   and (   (vwACL_ACCESS_ByAccess.ACLACCESS_ACCESS is not null and vwACL_ACCESS_ByAccess.ACLACCESS_ACCESS >= 0)
        or (vwACL_ACCESS_ByAccess.ACLACCESS_ACCESS is null     and vwACL_ACTIONS_AccessOnly.ACLACCESS is not null and vwACL_ACTIONS_AccessOnly.ACLACCESS >= 0)
        or (vwACL_ACCESS_ByAccess.ACLACCESS_ACCESS is null     and vwACL_ACTIONS_AccessOnly.ACLACCESS is null)
       )
   and (   (vwACL_ACCESS_ByAccess.ACLACCESS_EDIT   is not null and vwACL_ACCESS_ByAccess.ACLACCESS_EDIT >= 0)
        or (vwACL_ACCESS_ByAccess.ACLACCESS_EDIT   is null     and vwACL_ACTIONS_EditOnly.ACLACCESS is not null   and vwACL_ACTIONS_EditOnly.ACLACCESS   >= 0)
        or (vwACL_ACCESS_ByAccess.ACLACCESS_EDIT   is null     and vwACL_ACTIONS_EditOnly.ACLACCESS is null)
       )
union all
select vwSHORTCUTS_USERS_Cross.USER_ID
     , vwSHORTCUTS_USERS_Cross.MODULE_NAME
     , vwSHORTCUTS_USERS_Cross.DISPLAY_NAME
     , vwSHORTCUTS_USERS_Cross.RELATIVE_PATH
     , vwSHORTCUTS_USERS_Cross.IMAGE_NAME
     , vwSHORTCUTS_USERS_Cross.SHORTCUT_ORDER
     , vwSHORTCUTS_USERS_Cross.SHORTCUT_ACLTYPE
  from            vwSHORTCUTS_USERS_Cross
  left outer join vwACL_ACTIONS                       vwACL_ACTIONS_AccessOnly
               on vwACL_ACTIONS_AccessOnly.CATEGORY = vwSHORTCUTS_USERS_Cross.SHORTCUT_MODULE
              and vwACL_ACTIONS_AccessOnly.NAME     = N'access'
  left outer join vwACL_ACTIONS                       vwACL_ACTIONS_ListOnly
               on vwACL_ACTIONS_ListOnly.CATEGORY   = vwSHORTCUTS_USERS_Cross.SHORTCUT_MODULE
              and vwACL_ACTIONS_ListOnly.NAME       = N'list'
  left outer join vwACL_ACCESS_ByAccess
               on vwACL_ACCESS_ByAccess.USER_ID     = vwSHORTCUTS_USERS_Cross.USER_ID
              and vwACL_ACCESS_ByAccess.MODULE_NAME = vwSHORTCUTS_USERS_Cross.SHORTCUT_MODULE
 where vwSHORTCUTS_USERS_Cross.SHORTCUT_ENABLED = 1
   and vwSHORTCUTS_USERS_Cross.SHORTCUT_ACLTYPE = N'list'
   and (   (vwACL_ACCESS_ByAccess.ACLACCESS_ACCESS is not null and vwACL_ACCESS_ByAccess.ACLACCESS_ACCESS >= 0)
        or (vwACL_ACCESS_ByAccess.ACLACCESS_ACCESS is null     and vwACL_ACTIONS_AccessOnly.ACLACCESS is not null and vwACL_ACTIONS_AccessOnly.ACLACCESS >= 0)
        or (vwACL_ACCESS_ByAccess.ACLACCESS_ACCESS is null     and vwACL_ACTIONS_AccessOnly.ACLACCESS is null)
       )
   and (   (vwACL_ACCESS_ByAccess.ACLACCESS_LIST   is not null and vwACL_ACCESS_ByAccess.ACLACCESS_LIST >= 0)
        or (vwACL_ACCESS_ByAccess.ACLACCESS_LIST   is null     and vwACL_ACTIONS_ListOnly.ACLACCESS is not null   and vwACL_ACTIONS_ListOnly.ACLACCESS   >= 0)
        or (vwACL_ACCESS_ByAccess.ACLACCESS_LIST   is null     and vwACL_ACTIONS_ListOnly.ACLACCESS is null)
       )
union all
select vwSHORTCUTS_USERS_Cross.USER_ID
     , vwSHORTCUTS_USERS_Cross.MODULE_NAME
     , vwSHORTCUTS_USERS_Cross.DISPLAY_NAME
     , vwSHORTCUTS_USERS_Cross.RELATIVE_PATH
     , vwSHORTCUTS_USERS_Cross.IMAGE_NAME
     , vwSHORTCUTS_USERS_Cross.SHORTCUT_ORDER
     , vwSHORTCUTS_USERS_Cross.SHORTCUT_ACLTYPE
  from            vwSHORTCUTS_USERS_Cross
  left outer join vwACL_ACTIONS                       vwACL_ACTIONS_AccessOnly
               on vwACL_ACTIONS_AccessOnly.CATEGORY = vwSHORTCUTS_USERS_Cross.SHORTCUT_MODULE
              and vwACL_ACTIONS_AccessOnly.NAME     = N'access'
  left outer join vwACL_ACTIONS                       vwACL_ACTIONS_ListOnly
               on vwACL_ACTIONS_ListOnly.CATEGORY   = vwSHORTCUTS_USERS_Cross.SHORTCUT_MODULE
              and vwACL_ACTIONS_ListOnly.NAME       = N'import'
  left outer join vwACL_ACCESS_ByAccess
               on vwACL_ACCESS_ByAccess.USER_ID     = vwSHORTCUTS_USERS_Cross.USER_ID
              and vwACL_ACCESS_ByAccess.MODULE_NAME = vwSHORTCUTS_USERS_Cross.SHORTCUT_MODULE
 where vwSHORTCUTS_USERS_Cross.SHORTCUT_ENABLED = 1
   and vwSHORTCUTS_USERS_Cross.SHORTCUT_ACLTYPE = N'import'
   and (   (vwACL_ACCESS_ByAccess.ACLACCESS_ACCESS is not null and vwACL_ACCESS_ByAccess.ACLACCESS_ACCESS >= 0)
        or (vwACL_ACCESS_ByAccess.ACLACCESS_ACCESS is null     and vwACL_ACTIONS_AccessOnly.ACLACCESS is not null and vwACL_ACTIONS_AccessOnly.ACLACCESS >= 0)
        or (vwACL_ACCESS_ByAccess.ACLACCESS_ACCESS is null     and vwACL_ACTIONS_AccessOnly.ACLACCESS is null)
       )
   and (   (vwACL_ACCESS_ByAccess.ACLACCESS_IMPORT   is not null and vwACL_ACCESS_ByAccess.ACLACCESS_IMPORT >= 0)
        or (vwACL_ACCESS_ByAccess.ACLACCESS_IMPORT   is null     and vwACL_ACTIONS_ListOnly.ACLACCESS is not null   and vwACL_ACTIONS_ListOnly.ACLACCESS   >= 0)
        or (vwACL_ACCESS_ByAccess.ACLACCESS_IMPORT   is null     and vwACL_ACTIONS_ListOnly.ACLACCESS is null)
       )
union all
select vwSHORTCUTS_USERS_Cross.USER_ID
     , vwSHORTCUTS_USERS_Cross.MODULE_NAME
     , vwSHORTCUTS_USERS_Cross.DISPLAY_NAME
     , vwSHORTCUTS_USERS_Cross.RELATIVE_PATH
     , vwSHORTCUTS_USERS_Cross.IMAGE_NAME
     , vwSHORTCUTS_USERS_Cross.SHORTCUT_ORDER
     , vwSHORTCUTS_USERS_Cross.SHORTCUT_ACLTYPE
  from            vwSHORTCUTS_USERS_Cross
  left outer join vwACL_ACTIONS                       vwACL_ACTIONS_AccessOnly
               on vwACL_ACTIONS_AccessOnly.CATEGORY = vwSHORTCUTS_USERS_Cross.SHORTCUT_MODULE
              and vwACL_ACTIONS_AccessOnly.NAME     = N'access'
  left outer join vwACL_ACTIONS                       vwACL_ACTIONS_ListOnly
               on vwACL_ACTIONS_ListOnly.CATEGORY   = vwSHORTCUTS_USERS_Cross.SHORTCUT_MODULE
              and vwACL_ACTIONS_ListOnly.NAME       = N'archive'
  left outer join vwACL_ACCESS_ByAccess
               on vwACL_ACCESS_ByAccess.USER_ID     = vwSHORTCUTS_USERS_Cross.USER_ID
              and vwACL_ACCESS_ByAccess.MODULE_NAME = vwSHORTCUTS_USERS_Cross.SHORTCUT_MODULE
 where vwSHORTCUTS_USERS_Cross.SHORTCUT_ENABLED = 1
   and vwSHORTCUTS_USERS_Cross.SHORTCUT_ACLTYPE = N'archive'
   and (   (vwACL_ACCESS_ByAccess.ACLACCESS_ACCESS is not null and vwACL_ACCESS_ByAccess.ACLACCESS_ACCESS >= 0)
        or (vwACL_ACCESS_ByAccess.ACLACCESS_ACCESS is null     and vwACL_ACTIONS_AccessOnly.ACLACCESS is not null and vwACL_ACTIONS_AccessOnly.ACLACCESS >= 0)
        or (vwACL_ACCESS_ByAccess.ACLACCESS_ACCESS is null     and vwACL_ACTIONS_AccessOnly.ACLACCESS is null)
       )
   and (   (vwACL_ACCESS_ByAccess.ACLACCESS_IMPORT   is not null and vwACL_ACCESS_ByAccess.ACLACCESS_IMPORT >= 0)
        or (vwACL_ACCESS_ByAccess.ACLACCESS_IMPORT   is null     and vwACL_ACTIONS_ListOnly.ACLACCESS is not null   and vwACL_ACTIONS_ListOnly.ACLACCESS   >= 0)
        or (vwACL_ACCESS_ByAccess.ACLACCESS_IMPORT   is null     and vwACL_ACTIONS_ListOnly.ACLACCESS is null)
       )

GO

Grant Select on dbo.vwSHORTCUTS_Menu_ByUser to public;
GO

