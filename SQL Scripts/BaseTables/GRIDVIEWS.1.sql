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
-- 10/28/2009 Paul.  Add UTC date to allow this table to sync. 
-- 11/22/2010 Paul.  Add support for Business Rules Framework. 
-- 09/20/2012 Paul.  We need a SCRIPT field that is form specific. 
-- 02/11/2017 Paul.  New index based on missing indexes query. 
-- 02/22/2021 Paul.  The React client needs a way to determine the default sort, besides NAME asc. 
if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'GRIDVIEWS' and TABLE_TYPE = 'BASE TABLE')
  begin
	print 'Create Table dbo.GRIDVIEWS';
	Create Table dbo.GRIDVIEWS
		( ID                                 uniqueidentifier not null default(newid()) constraint PK_GRIDVIEWS primary key
		, DELETED                            bit not null default(0)
		, CREATED_BY                         uniqueidentifier null
		, DATE_ENTERED                       datetime not null default(getdate())
		, MODIFIED_USER_ID                   uniqueidentifier null
		, DATE_MODIFIED                      datetime not null default(getdate())
		, DATE_MODIFIED_UTC                  datetime null default(getutcdate())

		, NAME                               nvarchar( 50) not null
		, MODULE_NAME                        nvarchar( 25) not null
		, VIEW_NAME                          nvarchar( 50) not null

		, PRE_LOAD_EVENT_ID                  uniqueidentifier null
		, POST_LOAD_EVENT_ID                 uniqueidentifier null
		, SCRIPT                             nvarchar(max) null

		, SORT_FIELD                         nvarchar(50) null
		, SORT_DIRECTION                     nvarchar(10) null
		)

	create index IDX_GRIDVIEWS_NAME on dbo.GRIDVIEWS (NAME, DELETED)
	-- 02/11/2017 Paul.  New index based on missing indexes query. 
	create index IDX_GRIDVIEWS_DELETED_VIEW on dbo.GRIDVIEWS (DELETED, VIEW_NAME)
  end
GO


