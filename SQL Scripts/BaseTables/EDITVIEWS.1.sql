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
-- 12/02/2007 Paul.  Add field for data columns. 
-- 10/28/2009 Paul.  Add UTC date to allow this table to sync. 
-- 10/30/2010 Paul.  Add support for Business Rules Framework. 
-- 11/11/2010 Paul.  Change to Pre Load and Post Load. 
-- 02/12/2011 Paul.  POST_VALIDATION_EVENT_ID was not used.  Instead, we use VALIDATION_EVENT_ID. 
-- 09/20/2012 Paul.  We need a SCRIPT field that is form specific. 
-- 02/11/2017 Paul.  New index based on missing indexes query. 
if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'EDITVIEWS' and TABLE_TYPE = 'BASE TABLE')
  begin
	print 'Create Table dbo.EDITVIEWS';
	Create Table dbo.EDITVIEWS
		( ID                                 uniqueidentifier not null default(newid()) constraint PK_EDITVIEWS primary key
		, DELETED                            bit not null default(0)
		, CREATED_BY                         uniqueidentifier null
		, DATE_ENTERED                       datetime not null default(getdate())
		, MODIFIED_USER_ID                   uniqueidentifier null
		, DATE_MODIFIED                      datetime not null default(getdate())
		, DATE_MODIFIED_UTC                  datetime null default(getutcdate())

		, NAME                               nvarchar( 50) not null
		, MODULE_NAME                        nvarchar( 25) not null
		, VIEW_NAME                          nvarchar( 50) not null
		, LABEL_WIDTH                        nvarchar( 10) null default('15%')
		, FIELD_WIDTH                        nvarchar( 10) null default('35%')
		, DATA_COLUMNS                       int null

		, NEW_EVENT_ID                       uniqueidentifier null
		, PRE_LOAD_EVENT_ID                  uniqueidentifier null
		, POST_LOAD_EVENT_ID                 uniqueidentifier null
		, VALIDATION_EVENT_ID                uniqueidentifier null
		, PRE_SAVE_EVENT_ID                  uniqueidentifier null
		, POST_SAVE_EVENT_ID                 uniqueidentifier null
		, SCRIPT                             nvarchar(max) null
		)

	create index IDX_EDITVIEWS_NAME on dbo.EDITVIEWS (NAME, DELETED)
	-- 02/11/2017 Paul.  New index based on missing indexes query. 
	create index IDX_EDITVIEWS_DELETED_VIEW on dbo.EDITVIEWS (DELETED, VIEW_NAME)
  end
GO


