if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwSqlTablesBase')
	Drop View dbo.vwSqlTablesBase;
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
-- 05/01/2009 Paul.  We need to isolate tables that are non CRM tables. 
-- 09/08/2009 Paul.  Azura requires the use of aliases when dealing with INFORMATION_SCHEMA. 
-- 09/22/2016 Paul.  Manually specify default collation to ease migration to Azure
Create View dbo.vwSqlTablesBase
as
select TABLES.TABLE_NAME  collate database_default  as TABLE_NAME
  from      INFORMATION_SCHEMA.TABLES   TABLES
 inner join INFORMATION_SCHEMA.COLUMNS  COLUMNS
         on COLUMNS.TABLE_NAME        = TABLES.TABLE_NAME
 where TABLES.TABLE_TYPE = N'BASE TABLE'
   and TABLES.TABLE_NAME   not in (N'dtproperties', N'sysdiagrams')
   and COLUMNS.COLUMN_NAME in (N'ID', N'ID_C')
   and COLUMNS.DATA_TYPE   = N'uniqueidentifier'
GO


Grant Select on dbo.vwSqlTablesBase to public;
GO

