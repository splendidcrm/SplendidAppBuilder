if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwUSERS_Edit')
	Drop View dbo.vwUSERS_Edit;
GO


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
-- 11/08/2008 Paul.  Move description to base view. 
-- 07/09/2010 Paul.  SMTP values belong in the OUTBOUND_EMAILS table. 
-- 06/02/2016 Paul.  We need SMTP server values. 
-- 01/17/2017 Paul.  Add support for Office 365 OAuth credentials. 
-- 02/01/2017 Paul.  Add support for Exchange using Username/Password. 
-- 01/30/2019 Paul.  Ease conversion to Oracle. 
-- 07/14/2020 Paul.  Create dummy ICLOUD_SECURITY_CODE field just in case it is required. 
-- 02/12/2022 Paul.  Apple now uses OAuth. 
Create View dbo.vwUSERS_Edit
as
select vwUSERS.*
     , dbo.fnFullAddressHtml(vwUSERS.ADDRESS_STREET, vwUSERS.ADDRESS_CITY, vwUSERS.ADDRESS_STATE, vwUSERS.ADDRESS_POSTALCODE, vwUSERS.ADDRESS_COUNTRY) as ADDRESS_HTML
     , OUTBOUND_EMAILS.MAIL_SMTPUSER
     , OUTBOUND_EMAILS.MAIL_SMTPPASS
     , OUTBOUND_EMAILS.MAIL_SMTPSERVER
     , OUTBOUND_EMAILS.MAIL_SMTPPORT
     , OUTBOUND_EMAILS.MAIL_SMTPAUTH_REQ
     , OUTBOUND_EMAILS.MAIL_SMTPSSL
     , (case when OUTBOUND_EMAILS.MAIL_SENDTYPE is null and OUTBOUND_EMAILS.MAIL_SMTPPORT in (25, 465, 587) then N'smtp' else OUTBOUND_EMAILS.MAIL_SENDTYPE end) as MAIL_SENDTYPE
     , (select count(*) from OAUTH_TOKENS where OAUTH_TOKENS.ASSIGNED_USER_ID = vwUSERS.ID and OAUTH_TOKENS.NAME = N'Office365'  and OAUTH_TOKENS.DELETED = 0) as OFFICE365_OAUTH_ENABLED
     , (select count(*) from OAUTH_TOKENS where OAUTH_TOKENS.ASSIGNED_USER_ID = vwUSERS.ID and OAUTH_TOKENS.NAME = N'GoogleApps' and OAUTH_TOKENS.DELETED = 0) as GOOGLEAPPS_OAUTH_ENABLED
     , (select count(*) from OAUTH_TOKENS where OAUTH_TOKENS.ASSIGNED_USER_ID = vwUSERS.ID and OAUTH_TOKENS.NAME = N'iCloud'     and OAUTH_TOKENS.DELETED = 0) as ICLOUD_OAUTH_ENABLED
     , cast(null as nvarchar(25)) as ICLOUD_SECURITY_CODE
  from            vwUSERS
  left outer join USERS
               on USERS.ID = vwUSERS.ID
  left outer join OUTBOUND_EMAILS
               on OUTBOUND_EMAILS.USER_ID         = USERS.ID
              and OUTBOUND_EMAILS.TYPE            = N'system-override'
              and (OUTBOUND_EMAILS.MAIL_SMTPUSER   is not null or OUTBOUND_EMAILS.MAIL_SENDTYPE is not null)
              and OUTBOUND_EMAILS.DELETED         = 0

GO

Grant Select on dbo.vwUSERS_Edit to public;
GO

