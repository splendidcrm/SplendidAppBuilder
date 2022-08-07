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
-- 09/08/2007 Paul.  Allow relationships to be disabled. 
-- 09/08/2007 Paul.  Allow nulls in relationship order field. 
-- 09/08/2007 Paul.  We need a title when we migrate to WebParts. 
-- 09/24/2009 Paul.  The new Silverlight charts exceeded the control name length of 50. 
-- 10/28/2009 Paul.  Add UTC date to allow this table to sync. 
-- 10/13/2012 Paul.  Add table info for HTML5 Offline Client. 
-- 03/20/2016 Paul.  Increase PRIMARY_FIELD size to 255 to support OfficeAddin. 
-- 03/30/2022 Paul.  Add Insight fields. 
if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'DETAILVIEWS_RELATIONSHIPS' and TABLE_TYPE = 'BASE TABLE')
  begin
	print 'Create Table dbo.DETAILVIEWS_RELATIONSHIPS';
	Create Table dbo.DETAILVIEWS_RELATIONSHIPS
		( ID                                 uniqueidentifier not null default(newid()) constraint PK_DETAILVIEWS_RELATIONSHIPS primary key
		, DELETED                            bit not null default(0)
		, CREATED_BY                         uniqueidentifier null
		, DATE_ENTERED                       datetime not null default(getdate())
		, MODIFIED_USER_ID                   uniqueidentifier null
		, DATE_MODIFIED                      datetime not null default(getdate())
		, DATE_MODIFIED_UTC                  datetime null default(getutcdate())

		, DETAIL_NAME                        nvarchar( 50) not null
		, MODULE_NAME                        nvarchar( 50) null
		, CONTROL_NAME                       nvarchar(100) null
		, RELATIONSHIP_ORDER                 int null
		, RELATIONSHIP_ENABLED               bit null default(1)
		, TITLE                              nvarchar(100) null
		, TABLE_NAME                         nvarchar(50) null
		, PRIMARY_FIELD                      nvarchar(255) null
		, SORT_FIELD                         nvarchar(50) null
		, SORT_DIRECTION                     nvarchar(10) null
		, INSIGHT_LABEL                      nvarchar(100) null
		, INSIGHT_OPERATOR                   nvarchar(2000) null
		, INSIGHT_VIEW                       nvarchar(50) null
		)

	create index IDX_DETAILVIEWS_RELATIONSHIPS_DETAIL_NAME on dbo.DETAILVIEWS_RELATIONSHIPS (DETAIL_NAME, DELETED, RELATIONSHIP_ENABLED)
  end
GO


