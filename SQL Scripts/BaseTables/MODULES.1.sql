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
-- 01/04/2006 Paul.  Add CUSTOM_ENABLED if module has a _CSTM table and can be customized. 
-- 04/24/2006 Paul.  Add IS_ADMIN to simplify ACL management. 
-- 05/02/2006 Paul.  Add TABLE_NAME as direct table queries are required by SOAP and we need a mapping. 
-- 05/20/2006 Paul.  Add REPORT_ENABLED if the module can be the basis of a report. ACL rules will still apply. 
-- 10/06/2006 Paul.  Add IMPORT_ENABLED if the module can allow importing. 
-- 11/17/2007 Paul.  Add MOBILE_ENABLED flag to determine if module should be shown on mobile browser.
-- 07/20/2009 Paul.  Add SYNC_ENABLED flag to determine if module can be sync'd.
-- 09/08/2009 Paul.  Custom Paging can be enabled /disabled per module. 
-- 10/28/2009 Paul.  Add UTC date to allow this table to sync. 
-- 12/02/2009 Paul.  Add the ability to disable Mass Updates. 
-- 01/13/2010 Paul.  Allow default search to be disabled. 
-- 04/01/2010 Paul.  Add Exchange Sync flag. 
-- 04/04/2010 Paul.  Add Exchange Folders flag. 
-- 04/05/2010 Paul.  Add Exchange Create Parent flag. Need to be able to disable Account creation. 
-- 06/18/2011 Paul.  REST_ENABLED provides a way to enable/disable a module in the REST API. 
-- 03/14/2014 Paul.  DUPLICATE_CHECHING_ENABLED enables duplicate checking. 
-- 11/01/2017 Paul.  Use a module-based flag so that Record Level Security is only enabled when needed. 
-- 07/31/2019 Paul.  DEFAULT_SORT is a new field for the React Client. 
if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'MODULES' and TABLE_TYPE = 'BASE TABLE')
  begin
	print 'Create Table dbo.MODULES';
	Create Table dbo.MODULES
		( ID                                 uniqueidentifier not null default(newid()) constraint PK_MODULES primary key
		, DELETED                            bit not null default(0)
		, CREATED_BY                         uniqueidentifier null
		, DATE_ENTERED                       datetime not null default(getdate())
		, MODIFIED_USER_ID                   uniqueidentifier null
		, DATE_MODIFIED                      datetime not null default(getdate())
		, DATE_MODIFIED_UTC                  datetime null default(getutcdate())

		, MODULE_NAME                        nvarchar(25) not null
		, DISPLAY_NAME                       nvarchar(50) not null
		, RELATIVE_PATH                      nvarchar(50) not null
		, MODULE_ENABLED                     bit null default(1)
		, TAB_ENABLED                        bit null default(1)
		, MOBILE_ENABLED                     bit null default(0)
		, TAB_ORDER                          int null
		, PORTAL_ENABLED                     bit null default(0)
		, CUSTOM_ENABLED                     bit null default(0)
		, REPORT_ENABLED                     bit null default(0)
		, IMPORT_ENABLED                     bit null default(0)
		, SYNC_ENABLED                       bit null default(0)
		, REST_ENABLED                       bit null default(0)
		, IS_ADMIN                           bit null default(0)
		, CUSTOM_PAGING                      bit null default(0)
		, MASS_UPDATE_ENABLED                bit null default(0)
		, DEFAULT_SEARCH_ENABLED             bit null default(1)
		, TABLE_NAME                         nvarchar(30) null
		, EXCHANGE_SYNC                      bit null default(0)
		, EXCHANGE_FOLDERS                   bit null default(0)
		, EXCHANGE_CREATE_PARENT             bit null default(0)
		, DUPLICATE_CHECHING_ENABLED         bit null default(0)
		, RECORD_LEVEL_SECURITY_ENABLED      bit null default(0)
		, DEFAULT_SORT                       nvarchar(50) null
		)
	-- 12/30/2010 Irantha.  Add index for caching. 
	create index IX_MODULES_MODULE_NAME on dbo.MODULES(MODULE_NAME, DELETED, MODULE_ENABLED, IS_ADMIN, TAB_ORDER)
  end
GO


