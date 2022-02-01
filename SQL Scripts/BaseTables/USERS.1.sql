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
-- 04/21/2006 Paul.  IS_GROUP was added in SugarCRM 4.0.
-- 02/26/2008 Paul.  Increase USER_NAME so that an email can be used to login. 
-- 07/16/2008 Paul.  Remove not null requirement on PORTAL_ONLY. 
-- 05/06/2009 Paul.  Add DEFAULT_TEAM to support SugarCRM migration. 
-- 09/15/2009 Paul.  Convert data type to nvarchar(max) to support Azure. 
-- 10/28/2009 Paul.  Add UTC date to allow this table to sync. 
-- 03/16/2010 Paul.  Add IS_ADMIN_DELEGATE. 
-- 05/12/2010 Paul.  SYSTEM_GENERATED_PASSWORD and PWD_LAST_CHANGED are new to help manage forgotten password. 
-- 07/09/2010 Paul.  Move the SMTP values from USER_PREFERENCES to the main table to make it easier to access. 
-- 03/25/2011 Paul.  Add support for Google Apps. 
-- 03/25/2011 Paul.  Create a separate field for the Facebook ID. 
-- 12/13/2011 Paul.  Add support for Apple iCloud. 
-- 12/23/2011 Paul.  We need separate CTAGs for Contacts and Calendar. 
-- 12/15/2012 Paul.  Move USER_PREFERENCES to separate fields for easier access on Surface RT. 
-- 09/20/2013 Paul.  Move EXTENSION to the main table. 
-- 09/27/2013 Paul.  SMS messages need to be opt-in. 
-- 11/21/2014 Paul.  Add User Picture. 
-- 05/05/2016 Paul.  The User Primary Role is used with role-based views. 
if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'USERS' and TABLE_TYPE = 'BASE TABLE')
  begin
	print 'Create Table dbo.USERS';
	Create Table dbo.USERS
		( ID                                 uniqueidentifier not null default(newid()) constraint PK_USERS primary key
		, DELETED                            bit not null default(0)
		, CREATED_BY                         uniqueidentifier null
		, DATE_ENTERED                       datetime not null default(getdate())
		, MODIFIED_USER_ID                   uniqueidentifier null
		, DATE_MODIFIED                      datetime not null default(getdate())
		, DATE_MODIFIED_UTC                  datetime null default(getutcdate())

		, USER_NAME                          nvarchar(60) null
		, USER_PASSWORD                      nvarchar(30) null
		, USER_HASH                          nvarchar(32) null
		, FIRST_NAME                         nvarchar(30) null
		, LAST_NAME                          nvarchar(30) null
		, REPORTS_TO_ID                      uniqueidentifier null
		, IS_ADMIN                           bit null default(0)
		, IS_ADMIN_DELEGATE                  bit null default(0)
		, RECEIVE_NOTIFICATIONS              bit null default(1)
		, DESCRIPTION                        nvarchar(max) null
		, TITLE                              nvarchar(50) null
		, DEPARTMENT                         nvarchar(50) null
		, PHONE_HOME                         nvarchar(50) null
		, PHONE_MOBILE                       nvarchar(50) null
		, PHONE_WORK                         nvarchar(50) null
		, PHONE_OTHER                        nvarchar(50) null
		, PHONE_FAX                          nvarchar(50) null
		, EMAIL1                             nvarchar(100) null
		, EMAIL2                             nvarchar(100) null
		, STATUS                             nvarchar(25) null
		, ADDRESS_STREET                     nvarchar(150) null
		, ADDRESS_CITY                       nvarchar(100) null
		, ADDRESS_STATE                      nvarchar(100) null
		, ADDRESS_COUNTRY                    nvarchar(25) null
		, ADDRESS_POSTALCODE                 nvarchar(9) null
		, USER_PREFERENCES                   nvarchar(max) null
		, PORTAL_ONLY                        bit null default(0)
		, EMPLOYEE_STATUS                    nvarchar(25) null
		, MESSENGER_ID                       nvarchar(25) null
		, MESSENGER_TYPE                     nvarchar(25) null
		, IS_GROUP                           bit null default(0)
		, DEFAULT_TEAM                       uniqueidentifier null
		, SYSTEM_GENERATED_PASSWORD          bit null
		, PWD_LAST_CHANGED                   datetime null
		, MAIL_SMTPUSER                      nvarchar(60) null
		, MAIL_SMTPPASS                      nvarchar(30) null
		, GOOGLEAPPS_SYNC_CONTACTS           bit null
		, GOOGLEAPPS_SYNC_CALENDAR           bit null
		, GOOGLEAPPS_USERNAME                nvarchar(100) null
		, GOOGLEAPPS_PASSWORD                nvarchar(100) null
		, FACEBOOK_ID                        nvarchar(25) null
		, ICLOUD_SYNC_CONTACTS               bit null
		, ICLOUD_SYNC_CALENDAR               bit null
		, ICLOUD_USERNAME                    nvarchar(100) null
		, ICLOUD_PASSWORD                    nvarchar(100) null
		, ICLOUD_CTAG_CONTACTS               varchar(100) null
		, ICLOUD_CTAG_CALENDAR               varchar(100) null
		, THEME                              nvarchar(25) null
		, DATE_FORMAT                        nvarchar(25) null
		, TIME_FORMAT                        nvarchar(25) null
		, LANG                               nvarchar(10) null
		, CURRENCY_ID                        uniqueidentifier null
		, TIMEZONE_ID                        uniqueidentifier null
		, SAVE_QUERY                         bit null
		, GROUP_TABS                         bit null
		, SUBPANEL_TABS                      bit null
		, EXTENSION                          nvarchar(25) null
		, SMS_OPT_IN                         nvarchar(25) null
		, PICTURE                            nvarchar(max) null
		, PRIMARY_ROLE_ID                    uniqueidentifier null
		)

	-- 09/10/2009 Paul.  The indexes should be fully covered. 
	create index IDX_USERS_ID            on dbo.USERS (ID, DELETED, STATUS, PORTAL_ONLY, USER_NAME)
	create index IDX_USERS_USER_NAME     on dbo.USERS (USER_NAME, USER_HASH, DELETED, STATUS, PORTAL_ONLY, DEFAULT_TEAM)
	create index IDX_USERS_USER_PASSWORD on dbo.USERS (USER_PASSWORD)
  end
GO


