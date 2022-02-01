if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwMODULES_USERS_Cross')
	Drop View dbo.vwMODULES_USERS_Cross;
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
-- 05/20/2006 Paul.  Add REPORT_ENABLED flag as we need to restrict the list to enabled and accessible modules. 
-- 10/06/2006 Paul.  Add IMPORT_ENABLED if the module can allow importing. 
-- 11/17/2007 Paul.  Add MOBILE_ENABLED. 
-- 10/26/2009 Paul.  Add PORTAL_ENABLED. 
-- 12/06/2009 Paul.  We need the ID and TABLE_NAME when generating the SemanticModel for the ReportBuilder. 
Create View dbo.vwMODULES_USERS_Cross
as
select MODULES.MODULE_NAME
     , MODULES.DISPLAY_NAME
     , MODULES.RELATIVE_PATH
     , MODULES.MODULE_ENABLED
     , MODULES.TAB_ENABLED
     , MODULES.TAB_ORDER
     , MODULES.REPORT_ENABLED
     , MODULES.IMPORT_ENABLED
     , MODULES.IS_ADMIN
     , USERS.ID           as USER_ID
     , MODULES.MOBILE_ENABLED
     , MODULES.PORTAL_ENABLED
     , MODULES.ID
     , MODULES.TABLE_NAME
  from      MODULES
 cross join USERS
 where MODULES.DELETED = 0
   and USERS.DELETED   = 0

GO

Grant Select on dbo.vwMODULES_USERS_Cross to public;
GO

