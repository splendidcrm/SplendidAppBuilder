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
-- 07/24/2006 Paul.  Increase the DATA_LABEL to 150 to allow a fully-qualified (NAME+MODULE_NAME+LIST_NAME) TERMINOLOGY name. 
-- 06/12/2009 Paul.  Add TOOL_TIP for help hover.
-- 10/28/2009 Paul.  Add UTC date to allow this table to sync. 
-- 02/16/2010 Paul.  Add MODULE_TYPE so that we can lookup custom field IDs. 
-- 06/16/2010 Paul.  Increase the size of the URL_FIELD and URL_FORMAT so that we can create an IFrame to a Google map. 
-- 08/02/2010 Paul.  Increase the size of the URL_FIELD and URL_FORMAT so that we can add a javascript info column. 
-- 10/09/2010 Paul.  Add PARENT_FIELD so that we can establish dependent listboxes. 
-- 10/30/2013 Paul.  Increase size of URL_TARGET. 
-- 02/25/2015 Paul.  Increase size of DATA_FIELD and DATA_FORMAT for OfficeAddin. 
if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'DETAILVIEWS_FIELDS' and TABLE_TYPE = 'BASE TABLE')
  begin
	print 'Create Table dbo.DETAILVIEWS_FIELDS';
	Create Table dbo.DETAILVIEWS_FIELDS
		( ID                                 uniqueidentifier not null default(newid()) constraint PK_DETAILVIEWS_FIELDS primary key
		, DELETED                            bit not null default(0)
		, CREATED_BY                         uniqueidentifier null
		, DATE_ENTERED                       datetime not null default(getdate())
		, MODIFIED_USER_ID                   uniqueidentifier null
		, DATE_MODIFIED                      datetime not null default(getdate())
		, DATE_MODIFIED_UTC                  datetime null default(getutcdate())

		, DETAIL_NAME                        nvarchar( 50) not null
		, FIELD_INDEX                        int not null
		, FIELD_TYPE                         nvarchar( 25) not null
		, DEFAULT_VIEW                       bit null default(0)

		, DATA_LABEL                         nvarchar(150) null
		, DATA_FIELD                         nvarchar(1000) null
		, DATA_FORMAT                        nvarchar(max) null
		, URL_FIELD                          nvarchar(max) null
		, URL_FORMAT                         nvarchar(max) null
		, URL_TARGET                         nvarchar( 60) null
		, LIST_NAME                          nvarchar( 50) null
		, COLSPAN                            int null
		, MODULE_TYPE                        nvarchar(25) null
		, TOOL_TIP                           nvarchar(150) null
		, PARENT_FIELD                       nvarchar(30) null
		)

	create index IDX_DETAILVIEWS_FIELDS_DETAIL_NAME on dbo.DETAILVIEWS_FIELDS (DETAIL_NAME, DELETED)
  end
GO


