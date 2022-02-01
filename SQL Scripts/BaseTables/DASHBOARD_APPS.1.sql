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
-- 06/16/2017 Paul.  Add DEFAULT_SETTINGS. 
if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'DASHBOARD_APPS' and TABLE_TYPE = 'BASE TABLE')
  begin
	print 'Create Table dbo.DASHBOARD_APPS';
	Create Table dbo.DASHBOARD_APPS
		( ID                                 uniqueidentifier not null default(newid()) constraint PK_DASHBOARD_APPS primary key
		, DELETED                            bit not null default(0)
		, CREATED_BY                         uniqueidentifier null
		, DATE_ENTERED                       datetime not null default(getdate())
		, MODIFIED_USER_ID                   uniqueidentifier null
		, DATE_MODIFIED                      datetime not null default(getdate())
		, DATE_MODIFIED_UTC                  datetime null default(getutcdate())

		, NAME                               nvarchar(150) null
		, CATEGORY                           nvarchar( 25) null
		, MODULE_NAME                        nvarchar( 50) null
		, TITLE                              nvarchar(100) null
		, SETTINGS_EDITVIEW                  nvarchar( 50) null
		, IS_ADMIN                           bit null default(0)
		, APP_ENABLED                        bit null default(1)
		, SCRIPT_URL                         nvarchar(2083) null
		, DEFAULT_SETTINGS                   nvarchar(max) null
		)

	create index IDX_DASHBOARD_APPS_MODULE on dbo.DASHBOARD_APPS (MODULE_NAME, DELETED, ID)
  end
GO

