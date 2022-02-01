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
-- 04/21/2006 Paul.  Added in SugarCRM 4.0.
-- 10/28/2009 Paul.  Add UTC date to allow this table to sync. 
if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'ACL_ROLES_ACTIONS' and TABLE_TYPE = 'BASE TABLE')
  begin
	print 'Create Table dbo.ACL_ROLES_ACTIONS';
	Create Table dbo.ACL_ROLES_ACTIONS
		( ID                                 uniqueidentifier not null default(newid()) constraint PK_ACL_ROLES_ACTIONS primary key
		, DELETED                            bit not null default(0)
		, CREATED_BY                         uniqueidentifier null
		, DATE_ENTERED                       datetime not null default(getdate())
		, MODIFIED_USER_ID                   uniqueidentifier null
		, DATE_MODIFIED                      datetime not null default(getdate())
		, DATE_MODIFIED_UTC                  datetime null default(getutcdate())

		, ROLE_ID                            uniqueidentifier not null
		, ACTION_ID                          uniqueidentifier not null
		, ACCESS_OVERRIDE                    int null
		)

	create index IDX_ACL_ROLES_ACTIONS_ROLE_ID   on dbo.ACL_ROLES_ACTIONS (ROLE_ID  )
	create index IDX_ACL_ROLES_ACTIONS_ACTION_ID on dbo.ACL_ROLES_ACTIONS (ACTION_ID)

	alter table dbo.ACL_ROLES_ACTIONS add constraint FK_ACL_ROLES_ACTIONS_ROLE_ID   foreign key ( ROLE_ID   ) references dbo.ACL_ROLES   ( ID )
	alter table dbo.ACL_ROLES_ACTIONS add constraint FK_ACL_ROLES_ACTIONS_ACTION_ID foreign key ( ACTION_ID ) references dbo.ACL_ACTIONS ( ID )
  end
GO


