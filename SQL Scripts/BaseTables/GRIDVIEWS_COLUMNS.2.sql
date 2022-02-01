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
-- 04/27/2006 Paul.  Add URL_MODULE to support ACL.
-- 05/02/2006 Paul.  Add URL_ASSIGNED_FIELD to support ACL. 
-- 07/24/2006 Paul.  Increase the HEADER_TEXT to 150 to allow a fully-qualified (NAME+MODULE_NAME+LIST_NAME) TERMINOLOGY name. 
-- 10/28/2009 Paul.  Add UTC date to allow this table to sync. 
-- 02/16/2010 Paul.  Add MODULE_TYPE so that we can lookup custom field IDs. 
-- 08/02/2010 Paul.  Increase the size of the URL_FIELD and URL_FORMAT so that we can add a javascript info column. 
-- 10/09/2010 Paul.  Add PARENT_FIELD so that we can establish dependent listboxes. 
-- 10/30/2013 Paul.  Increase size of URL_TARGET. 
-- 03/01/2014 Paul.  Increase size of DATA_FORMAT. 
if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'GRIDVIEWS_COLUMNS' and TABLE_TYPE = 'BASE TABLE')
  begin
	print 'Create Table dbo.GRIDVIEWS_COLUMNS';
	Create Table dbo.GRIDVIEWS_COLUMNS
		( ID                                 uniqueidentifier not null default(newid()) constraint PK_GRIDVIEWS_COLUMNS primary key
		, DELETED                            bit not null default(0)
		, CREATED_BY                         uniqueidentifier null
		, DATE_ENTERED                       datetime not null default(getdate())
		, MODIFIED_USER_ID                   uniqueidentifier null
		, DATE_MODIFIED                      datetime not null default(getdate())
		, DATE_MODIFIED_UTC                  datetime null default(getutcdate())

		, GRID_NAME                          nvarchar( 50) not null
		, COLUMN_INDEX                       int not null
		, COLUMN_TYPE                        nvarchar( 25) not null
		, DEFAULT_VIEW                       bit null default(0)

		, HEADER_TEXT                        nvarchar(150) null
		, SORT_EXPRESSION                    nvarchar( 50) null
		, ITEMSTYLE_WIDTH                    nvarchar( 10) null
		, ITEMSTYLE_CSSCLASS                 nvarchar( 50) null
		, ITEMSTYLE_HORIZONTAL_ALIGN         nvarchar( 10) null
		, ITEMSTYLE_VERTICAL_ALIGN           nvarchar( 10) null
		, ITEMSTYLE_WRAP                     bit null

		, DATA_FIELD                         nvarchar( 50) null
		, DATA_FORMAT                        nvarchar( 25) null
		, URL_FIELD                          nvarchar(max) null
		, URL_FORMAT                         nvarchar(max) null
		, URL_TARGET                         nvarchar( 60) null
		, LIST_NAME                          nvarchar( 50) null
		, URL_MODULE                         nvarchar( 25) null
		, URL_ASSIGNED_FIELD                 nvarchar( 30) null
		, MODULE_TYPE                        nvarchar( 25) null
		, PARENT_FIELD                       nvarchar( 30) null
		)

	create index IDX_GRIDVIEWS_COLUMNS_GRID_NAME on dbo.GRIDVIEWS_COLUMNS (GRID_NAME, DELETED)
  end
GO


