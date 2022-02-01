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
-- 07/16/2013 Paul.  USER_ID should be nullable so that table can contain system email accounts. 
-- 04/20/2016 Paul.  Add team management to Outbound Emails. 
-- 01/17/2017 Paul.  Increase size of @MAIL_SENDTYPE to fit office365. 
-- drop table dbo.OUTBOUND_EMAILS;
if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'OUTBOUND_EMAILS' and TABLE_TYPE = 'BASE TABLE')
  begin
	print 'Create Table dbo.OUTBOUND_EMAILS';
	Create Table dbo.OUTBOUND_EMAILS
		( ID                                 uniqueidentifier not null default(newid()) constraint PK_OUTBOUND_EMAILS primary key
		, DELETED                            bit not null default(0)
		, CREATED_BY                         uniqueidentifier null
		, DATE_ENTERED                       datetime not null default(getdate())
		, MODIFIED_USER_ID                   uniqueidentifier null
		, DATE_MODIFIED                      datetime not null default(getdate())
		, DATE_MODIFIED_UTC                  datetime null default(getutcdate())

		, NAME                               nvarchar(50) null default('system')
		, TYPE                               nvarchar(15) null default('user')
		, USER_ID                            uniqueidentifier null
		, MAIL_SENDTYPE                      nvarchar(25) null default('smtp')
		, MAIL_SMTPTYPE                      nvarchar(20) null default('other')
		, MAIL_SMTPSERVER                    nvarchar(100) null
		, MAIL_SMTPPORT                      int null default(0)
		, MAIL_SMTPUSER                      nvarchar(100) null
		, MAIL_SMTPPASS                      nvarchar(100) null
		, MAIL_SMTPAUTH_REQ                  bit null default(0)
		, MAIL_SMTPSSL                       int null default(0)
		, FROM_NAME                          nvarchar(100) null
		, FROM_ADDR                          nvarchar(100) null
		, TEAM_ID                            uniqueidentifier null
		, TEAM_SET_ID                        uniqueidentifier null
		)

	create index IDX_OUTBOUND_EMAILS_USER_ID on dbo.OUTBOUND_EMAILS (USER_ID, TYPE, DELETED, ID)
  end
GO

