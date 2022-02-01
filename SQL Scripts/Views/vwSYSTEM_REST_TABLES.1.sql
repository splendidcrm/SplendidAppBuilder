if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwSYSTEM_REST_TABLES')
	Drop View dbo.vwSYSTEM_REST_TABLES;
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
-- 05/25/2011 Paul.  Tables available to the REST API are not bound by the SYNC_ENABLED flag. 
-- 06/18/2011 Paul.  SYSTEM_REST_TABLES are nearly identical to SYSTEM_REST_TABLES,
-- but the Module tables typically refer to the base view instead of the raw table. 
-- 09/26/2016 Paul.  Use vwSqlViews so that collation can be handled in one area. 
-- 08/01/2019 Paul.  We need a ListView and EditView flags for the Rest Client. 
-- 08/02/2019 Paul.  The React Client will need access to views that require a filter, like CAMPAIGN_ID. 
Create View dbo.vwSYSTEM_REST_TABLES
as
select SYSTEM_REST_TABLES.TABLE_NAME
     , SYSTEM_REST_TABLES.IS_SYSTEM
     , SYSTEM_REST_TABLES.IS_RELATIONSHIP
     , SYSTEM_REST_TABLES.DEPENDENT_LEVEL
     , SYSTEM_REST_TABLES.VIEW_NAME
     , SYSTEM_REST_TABLES.MODULE_NAME
     , SYSTEM_REST_TABLES.MODULE_NAME_RELATED
     , SYSTEM_REST_TABLES.MODULE_SPECIFIC
     , SYSTEM_REST_TABLES.MODULE_FIELD_NAME
     , SYSTEM_REST_TABLES.IS_ASSIGNED
     , SYSTEM_REST_TABLES.ASSIGNED_FIELD_NAME
     , SYSTEM_REST_TABLES.HAS_CUSTOM
     , SYSTEM_REST_TABLES.REQUIRED_FIELDS
     , isnull(LIST_VIEWS.VIEW_NAME, SYSTEM_REST_TABLES.VIEW_NAME) as LIST_VIEW
     , isnull(EDIT_VIEWS.VIEW_NAME, SYSTEM_REST_TABLES.VIEW_NAME) as EDIT_VIEW
  from            SYSTEM_REST_TABLES
       inner join vwSqlViews                    TABLES
               on TABLES.VIEW_NAME            = SYSTEM_REST_TABLES.VIEW_NAME
  left outer join vwSqlViews                    LIST_VIEWS
               on LIST_VIEWS.VIEW_NAME        = SYSTEM_REST_TABLES.VIEW_NAME + '_List'
  left outer join vwSqlViews                    EDIT_VIEWS
               on EDIT_VIEWS.VIEW_NAME        = SYSTEM_REST_TABLES.VIEW_NAME + '_Edit'
  left outer join MODULES
               on MODULES.MODULE_NAME         = SYSTEM_REST_TABLES.MODULE_NAME
              and MODULES.DELETED             = 0
  left outer join MODULES                       RELATED_MODULES
               on RELATED_MODULES.MODULE_NAME = SYSTEM_REST_TABLES.MODULE_NAME_RELATED
              and RELATED_MODULES.DELETED     = 0
 where  SYSTEM_REST_TABLES.DELETED = 0
   and (SYSTEM_REST_TABLES.MODULE_NAME         is null or (MODULES.MODULE_ENABLED         = 1) or SYSTEM_REST_TABLES.MODULE_NAME = 'Images')
   and (SYSTEM_REST_TABLES.MODULE_NAME_RELATED is null or (RELATED_MODULES.MODULE_ENABLED = 1))

GO

/*
select *
  from vwSYSTEM_REST_TABLES
 order by IS_SYSTEM desc, IS_RELATIONSHIP, DEPENDENT_LEVEL, TABLE_NAME
*/


Grant Select on dbo.vwSYSTEM_REST_TABLES to public;
GO

