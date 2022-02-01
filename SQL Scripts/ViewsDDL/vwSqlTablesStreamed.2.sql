if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwSqlTablesStreamed')
	Drop View dbo.vwSqlTablesStreamed;
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
-- 08/22/2017 Paul.  Manually specify default collation to ease migration to Azure
-- 06/25/2018 Paul.  Data Privacy is not streamed. 
-- 05/24/2020 Paul.  Exclude Azure tables. 
Create View dbo.vwSqlTablesStreamed
as
select TABLE_NAME
  from vwSqlTables
 where exists(select * from vwSqlTables AUDIT_TABLES where AUDIT_TABLES.TABLE_NAME = vwSqlTables.TABLE_NAME + N'_AUDIT')
   and exists(select * from vwSqlTables CSTM_TABLES  where CSTM_TABLES.TABLE_NAME  = vwSqlTables.TABLE_NAME + N'_CSTM')
   and exists(select * from INFORMATION_SCHEMA.COLUMNS vwSqlColumns  where vwSqlColumns.TABLE_NAME = vwSqlTables.TABLE_NAME collate database_default and vwSqlColumns.COLUMN_NAME = 'ASSIGNED_USER_ID')
   and TABLE_NAME not like 'AZURE[_]%'
   and TABLE_NAME not in
( N'CALLS'
, N'DATA_PRIVACY'
, N'MEETINGS'
, N'EMAILS'
, N'TASKS'
, N'NOTES'
, N'SMS_MESSAGES'
, N'TWITTER_MESSAGES'
, N'CALL_MARKETING'
, N'PROJECT'
, N'PROJECT_TASK'
, N'PRODUCT_TEMPLATES'
, N'INVOICES_LINE_ITEMS'
, N'ORDERS_LINE_ITEMS'
, N'QUOTES_LINE_ITEMS'
, N'REVENUE_LINE_ITEMS'
, N'PAYMENTS'
, N'CREDIT_CARDS'
, N'REGIONS'
, N'SURVEY_QUESTIONS'
, N'TEST_CASES'
, N'TEST_PLANS'
, N'THREADS'
, N'USERS'
)
GO


Grant Select on dbo.vwSqlTablesStreamed to public;
GO

-- select * from vwSqlTablesStreamed order by 1;

