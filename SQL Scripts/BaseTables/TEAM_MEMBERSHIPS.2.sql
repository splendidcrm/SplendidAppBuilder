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
-- 11/18/2006 Paul.  Create a covered index that will be heavily used when updating implicit team memberships. 
-- 11/18/2006 Paul.  Create a covered index that will be heavily used when modifying the organizational tree. 
-- 11/18/2006 Paul.  Add private flag.  We need to be able to isolate the user that started the private team. 
-- 10/28/2009 Paul.  Add UTC date to allow this table to sync. 
if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'TEAM_MEMBERSHIPS' and TABLE_TYPE = 'BASE TABLE')
  begin
	print 'Create Table dbo.TEAM_MEMBERSHIPS';
	Create Table dbo.TEAM_MEMBERSHIPS
		( ID                                 uniqueidentifier not null default(newid()) constraint PK_TEAM_MEMBERSHIPS primary key
		, DELETED                            bit not null default(0)
		, CREATED_BY                         uniqueidentifier null
		, DATE_ENTERED                       datetime not null default(getdate())
		, MODIFIED_USER_ID                   uniqueidentifier null
		, DATE_MODIFIED                      datetime not null default(getdate())
		, DATE_MODIFIED_UTC                  datetime null default(getutcdate())

		, TEAM_ID                            uniqueidentifier not null
		, USER_ID                            uniqueidentifier not null
		, EXPLICIT_ASSIGN                    bit null
		, IMPLICIT_ASSIGN                    bit null
		, PRIVATE                            bit null default(0)
		)

	alter table dbo.TEAM_MEMBERSHIPS add constraint FK_TEAM_MEMBERSHIPS_TEAM_ID foreign key ( TEAM_ID ) references dbo.TEAMS ( ID )
	alter table dbo.TEAM_MEMBERSHIPS add constraint FK_TEAM_MEMBERSHIPS_USER_ID foreign key ( USER_ID ) references dbo.USERS ( ID )

	create index IDX_TEAM_MEMBERSHIPS_EXPLICIT on dbo.TEAM_MEMBERSHIPS (TEAM_ID, EXPLICIT_ASSIGN, DELETED, USER_ID)
	-- 09/10/2009 Paul.  The indexes should be fully covered. 
	create index IDX_TEAM_MEMBERSHIPS_TEAM_ID  on dbo.TEAM_MEMBERSHIPS (TEAM_ID, DELETED, USER_ID, ID)
	-- 09/10/2009 Paul.  Include the PRIVATE field as it is used during login. 
	create index IDX_TEAM_MEMBERSHIPS_USER_ID  on dbo.TEAM_MEMBERSHIPS (USER_ID, DELETED, TEAM_ID, ID, PRIVATE)
  end
GO

