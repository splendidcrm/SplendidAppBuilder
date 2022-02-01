if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwSHORTCUTS_USERS_Cross')
	Drop View dbo.vwSHORTCUTS_USERS_Cross;
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
-- 11/22/2007 Paul.  Only show the shortcut if the module of the shortcut is enabled. 
-- 03/04/2008 Paul.  Admin modules are not ment to be disabled, so show the short cuts even if they are disabled. 
-- 03/11/2008 Paul.  Must always check the deleted flag. 
Create View dbo.vwSHORTCUTS_USERS_Cross
as
select SHORTCUTS.MODULE_NAME
     , SHORTCUTS.DISPLAY_NAME
     , SHORTCUTS.RELATIVE_PATH
     , SHORTCUTS.IMAGE_NAME
     , SHORTCUTS.SHORTCUT_ENABLED
     , SHORTCUTS.SHORTCUT_ORDER
     , SHORTCUTS.SHORTCUT_MODULE
     , SHORTCUTS.SHORTCUT_ACLTYPE
     , USERS.ID                   as USER_ID
  from      SHORTCUTS
 inner join MODULES
         on MODULES.MODULE_NAME    = SHORTCUTS.SHORTCUT_MODULE
        and MODULES.DELETED        = 0
        and (MODULES.MODULE_ENABLED = 1 or MODULES.IS_ADMIN = 1)
 cross join USERS
 where SHORTCUTS.DELETED = 0

GO

Grant Select on dbo.vwSHORTCUTS_USERS_Cross to public;
GO

