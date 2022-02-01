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
-- 07/24/2006 Paul.  Increase the MODULE_NAME to 25 to match the size in the MODULES table.
-- 10/28/2009 Paul.  Add UTC date to allow this table to sync. 
-- 03/06/2012 Paul.  Increase size of the NAME field so that it can include a date formula. 
-- 02/11/2017 Paul.  New index based on missing indexes query. 
if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'TERMINOLOGY' and TABLE_TYPE = 'BASE TABLE')
  begin
	print 'Create Table dbo.TERMINOLOGY';
	Create Table dbo.TERMINOLOGY
		( ID                                 uniqueidentifier not null default(newid()) constraint PK_TERMINOLOGY primary key
		, DELETED                            bit not null default(0)
		, CREATED_BY                         uniqueidentifier null
		, DATE_ENTERED                       datetime not null default(getdate())
		, MODIFIED_USER_ID                   uniqueidentifier null
		, DATE_MODIFIED                      datetime not null default(getdate())
		, DATE_MODIFIED_UTC                  datetime null default(getutcdate())

		, NAME                               nvarchar(150) null
		, LANG                               nvarchar(10) null
		, MODULE_NAME                        nvarchar(25) null
		, LIST_NAME                          nvarchar(50) null
		, LIST_ORDER                         int null
		, DISPLAY_NAME                       nvarchar(max) null
		)

	create index IX_TERMINOLOGY_DISPLAY_NAME on dbo.TERMINOLOGY(LANG, MODULE_NAME, NAME, LIST_NAME)
	-- 12/30/2010 Irantha.  Add index for list caching. 
	create index IX_TERMINOLOGY_LIST_NAME on dbo.TERMINOLOGY(DELETED, LANG, LIST_NAME)
	-- 02/11/2017 Paul.  New index based on missing indexes query. 
	create index IDX_TERMINOLOGY_DELETED_LIST on dbo.TERMINOLOGY (DELETED, LIST_NAME)
  end
GO


