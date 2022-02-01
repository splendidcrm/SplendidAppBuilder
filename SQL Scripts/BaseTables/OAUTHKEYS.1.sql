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
-- 04/09/2012 Paul.  Twitter has a 40 character verifier. 
-- 04/13/2012 Paul.  Facebook has a 111 character access token. 
-- drop table dbo.OAUTHKEYS;
if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'OAUTHKEYS' and TABLE_TYPE = 'BASE TABLE')
  begin
	print 'Create Table dbo.OAUTHKEYS';
	Create Table dbo.OAUTHKEYS
		( ID                                 uniqueidentifier not null default(newid()) constraint PK_OAUTHKEYS primary key
		, DELETED                            bit not null default(0)
		, CREATED_BY                         uniqueidentifier null
		, DATE_ENTERED                       datetime not null default(getdate())
		, MODIFIED_USER_ID                   uniqueidentifier null
		, DATE_MODIFIED                      datetime not null default(getdate())
		, DATE_MODIFIED_UTC                  datetime null default(getutcdate())

		, ASSIGNED_USER_ID                   uniqueidentifier null
		, NAME                               nvarchar(25) null
		, TOKEN                              nvarchar(200) null
		, SECRET                             nvarchar(50) null
		, VERIFIER                           nvarchar(50) null
		)

	create index IDX_OAUTHKEYS on dbo.OAUTHKEYS (ASSIGNED_USER_ID, NAME, DELETED)
  end
GO


