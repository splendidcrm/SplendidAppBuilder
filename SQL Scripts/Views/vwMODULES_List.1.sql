if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwMODULES_List')
	Drop View dbo.vwMODULES_List;
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
-- 09/08/2010 Paul.  We need a separate view for the list as the default view filters by MODULE_ENABLED 
-- and we don't want to filter by that flag in the ListView, DetailView or EditView. 
-- 06/18/2011 Paul.  REST_ENABLED provides a way to enable/disable a module in the REST API. 
-- 01/19/2013 Paul.  This view is not using on Surface RT. 
-- 03/14/2014 Paul.  DUPLICATE_CHECHING_ENABLED enables duplicate checking. 
-- 07/31/2019 Paul.  DEFAULT_SORT is a new field for the React Client. 
Create View dbo.vwMODULES_List
as
select ID
     , MODULE_NAME as NAME
     , MODULE_NAME
     , DISPLAY_NAME
     , RELATIVE_PATH
     , MODULE_ENABLED
     , TAB_ENABLED
     , TAB_ORDER
     , PORTAL_ENABLED
     , CUSTOM_ENABLED
     , IS_ADMIN
     , TABLE_NAME
     , REPORT_ENABLED
     , IMPORT_ENABLED
     , SYNC_ENABLED
     , MOBILE_ENABLED
     , CUSTOM_PAGING
     , DATE_MODIFIED
     , DATE_MODIFIED_UTC
     , MASS_UPDATE_ENABLED
     , DEFAULT_SEARCH_ENABLED
     , EXCHANGE_SYNC
     , EXCHANGE_FOLDERS
     , EXCHANGE_CREATE_PARENT
     , REST_ENABLED
     , DUPLICATE_CHECHING_ENABLED
     , RECORD_LEVEL_SECURITY_ENABLED
     , DEFAULT_SORT
  from MODULES
 where DELETED        = 0

GO

Grant Select on dbo.vwMODULES_List to public;
GO


