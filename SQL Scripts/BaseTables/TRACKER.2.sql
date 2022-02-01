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
-- 03/08/2012 Paul.  Add ACTION to the tracker table so that we can create quick user activity reports. 
if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'TRACKER' and TABLE_TYPE = 'BASE TABLE')
  begin
	print 'Create Table dbo.TRACKER';
	Create Table dbo.TRACKER
		( ID                                 uniqueidentifier not null default(newid()) constraint PK_TRACKER primary key
		, DELETED                            bit not null default(0)
		, CREATED_BY                         uniqueidentifier null
		, DATE_ENTERED                       datetime not null default(getdate())
		, MODIFIED_USER_ID                   uniqueidentifier null
		, DATE_MODIFIED                      datetime not null default(getdate())
		, DATE_MODIFIED_UTC                  datetime null default(getutcdate())

		, USER_ID                            uniqueidentifier not null
		, ACTION                             nvarchar(25) null default('detailview')
		, MODULE_NAME                        nvarchar(25) null
		, ITEM_ID                            uniqueidentifier not null
		, ITEM_SUMMARY                       nvarchar(255) null
		)

	-- 03/08/2012 Paul.  Add ACTION to the tracker table so that we can create quick user activity reports. 
	create index IDX_TRACKER_USER_ID     on dbo.TRACKER (USER_ID, ACTION, DELETED)
	create index IDX_TRACKER_ITEM_ID     on dbo.TRACKER (ITEM_ID, ACTION, DELETED)
	-- 08/26/2010 Paul.  Add IDX_TRACKER_USER_MODULE to speed spTRACKER_Update. 
	create index IDX_TRACKER_USER_MODULE on dbo.TRACKER (USER_ID, ACTION, DELETED, MODULE_NAME, ID)

	-- 11/03/2009 Paul.  This foreign key will give us trouble on the offline client. 
	-- alter table dbo.TRACKER add constraint FK_TRACKER_USER_ID foreign key ( USER_ID ) references dbo.USERS ( ID )
  end
GO


