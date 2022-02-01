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
-- 06/18/2011 Paul.  SYSTEM_REST_TABLES are nearly identical to SYSTEM_SYNC_TABLES,
-- but the Module tables typically refer to the base view instead of the raw table. 
-- 08/02/2019 Paul.  The React Client will need access to views that require a filter, like CAMPAIGN_ID. 
-- drop table SYSTEM_REST_TABLES;
if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'SYSTEM_REST_TABLES' and TABLE_TYPE = 'BASE TABLE')
  begin
	print 'Create Table dbo.SYSTEM_REST_TABLES';
	Create Table dbo.SYSTEM_REST_TABLES
		( ID                                 uniqueidentifier not null default(newid()) constraint PK_SYSTEM_REST_TABLES primary key
		, DELETED                            bit not null default(0)
		, CREATED_BY                         uniqueidentifier null
		, DATE_ENTERED                       datetime not null default(getdate())
		, MODIFIED_USER_ID                   uniqueidentifier null
		, DATE_MODIFIED                      datetime not null default(getdate())
		, DATE_MODIFIED_UTC                  datetime null default(getutcdate())

		, TABLE_NAME                         nvarchar(50) not null
		, VIEW_NAME                          nvarchar(60) not null
		, MODULE_NAME                        nvarchar(25) null
		, MODULE_NAME_RELATED                nvarchar(25) null
		, MODULE_SPECIFIC                    int null default(0)
		, MODULE_FIELD_NAME                  nvarchar(50) null
		, IS_SYSTEM                          bit null default(0)
		, IS_ASSIGNED                        bit null default(0)
		, ASSIGNED_FIELD_NAME                nvarchar(50) null
		, IS_RELATIONSHIP                    bit null default(0)
		, HAS_CUSTOM                         bit null default(0)
		, DEPENDENT_LEVEL                    int null default(0) -- fnSqlDependentLevel()
		, REQUIRED_FIELDS                    nvarchar(150) null
		)
	
  end
GO


