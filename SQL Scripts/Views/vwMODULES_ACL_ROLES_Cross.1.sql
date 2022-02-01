if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwMODULES_ACL_ROLES_Cross')
	Drop View dbo.vwMODULES_ACL_ROLES_Cross;
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
-- 01/16/2010 Paul.  We need the table name so that we can get the ACL Fields for a module. 
Create View dbo.vwMODULES_ACL_ROLES_Cross
as
select MODULES.MODULE_NAME
     , MODULES.TABLE_NAME
     , MODULES.DISPLAY_NAME
     , MODULES.MODULE_ENABLED
     , MODULES.TAB_ENABLED
     , MODULES.TAB_ORDER
     , MODULES.IS_ADMIN
     , ACL_ROLES.ID           as ROLE_ID
  from      MODULES
 cross join ACL_ROLES
 where MODULES.DELETED = 0
   and ACL_ROLES.DELETED = 0

GO

Grant Select on dbo.vwMODULES_ACL_ROLES_Cross to public;
GO

