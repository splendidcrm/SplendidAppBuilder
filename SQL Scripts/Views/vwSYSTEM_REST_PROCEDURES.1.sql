if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwSYSTEM_REST_PROCEDURES')
	Drop View dbo.vwSYSTEM_REST_PROCEDURES;
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
-- 11/25/2020 Paul.  We need a way to call a generic procedure.  Security is still managed through SYSTEM_REST_TABLES. 
Create View dbo.vwSYSTEM_REST_PROCEDURES
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
     , null as LIST_VIEW
     , null as EDIT_VIEW
  from            SYSTEM_REST_TABLES
       inner join vwSqlProcedures
               on vwSqlProcedures.NAME        = SYSTEM_REST_TABLES.VIEW_NAME
  left outer join MODULES
               on MODULES.MODULE_NAME         = SYSTEM_REST_TABLES.MODULE_NAME
              and MODULES.DELETED             = 0
 where  SYSTEM_REST_TABLES.DELETED = 0
   and (SYSTEM_REST_TABLES.MODULE_NAME         is null or (MODULES.MODULE_ENABLED         = 1))

GO

Grant Select on dbo.vwSYSTEM_REST_PROCEDURES to public;
GO

