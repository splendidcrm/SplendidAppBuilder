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
-- 03/06/2008 Paul.  The USERS_LOGINS fields should match SYSTEM_LOG fields to simplify joins. 
-- drop table USERS_LOGINS;
-- 10/28/2009 Paul.  Add UTC date to allow this table to sync. 
-- 08/07/2010 Paul.  Create an index to speed the cleanup of the logins table. 
if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'USERS_LOGINS' and TABLE_TYPE = 'BASE TABLE')
  begin
	print 'Create Table dbo.USERS_LOGINS';
	Create Table dbo.USERS_LOGINS
		( ID                                 uniqueidentifier not null default(newid()) constraint PK_USERS_LOGINS primary key
		, DELETED                            bit not null default(0)
		, CREATED_BY                         uniqueidentifier null
		, DATE_ENTERED                       datetime not null default(getdate())
		, MODIFIED_USER_ID                   uniqueidentifier null
		, DATE_MODIFIED                      datetime not null default(getdate())
		, DATE_MODIFIED_UTC                  datetime null default(getutcdate())

		, USER_ID                            uniqueidentifier null
		, USER_NAME                          nvarchar(60) null
		, LOGIN_TYPE                         nvarchar(25) null
		, LOGIN_DATE                         datetime null
		, LOGOUT_DATE                        datetime null
		, LOGIN_STATUS                       nvarchar(25) null
		, ASPNET_SESSIONID                   nvarchar(50) null
		, REMOTE_HOST                        nvarchar(100) null
		, SERVER_HOST                        nvarchar(100) null
		, TARGET                             nvarchar(255) null
		, RELATIVE_PATH                      nvarchar(255) null
		, USER_AGENT                         nvarchar(255) null
		)

	create index IDX_USERS_LOGINS_USER_ID    on dbo.USERS_LOGINS (USER_ID)
	create index IDX_USERS_LOGINS_LOGIN_DATE on dbo.USERS_LOGINS (LOGIN_DATE)
  end
GO

