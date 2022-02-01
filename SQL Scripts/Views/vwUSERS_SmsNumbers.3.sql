if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwUSERS_SmsNumbers')
	Drop View dbo.vwUSERS_SmsNumbers;
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
-- 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
Create View dbo.vwUSERS_SmsNumbers
as
select ID
     , NAME
     , FIRST_NAME
     , LAST_NAME
     , TITLE
     , cast(null as nvarchar(150)) as ACCOUNT_NAME
     , cast(null as uniqueidentifier) as ACCOUNT_ID
     , PHONE_HOME
     , PHONE_MOBILE
     , PHONE_WORK
     , PHONE_OTHER
     , PHONE_FAX
     , EMAIL1
     , EMAIL2
     , SMS_OPT_IN
     , cast(null as nvarchar(75))     as ASSISTANT
     , cast(null as nvarchar(25))     as ASSISTANT_PHONE
     , cast(null as nvarchar(60))     as ASSIGNED_TO
     , cast(null as uniqueidentifier) as ASSIGNED_USER_ID
     , cast(null as uniqueidentifier) as TEAM_ID
     , cast(null as nvarchar(128))    as TEAM_NAME
     , cast(null as uniqueidentifier) as TEAM_SET_ID
     , cast(null as nvarchar(200))    as TEAM_SET_NAME
     , cast(null as uniqueidentifier) as ASSIGNED_SET_ID
     , cast(null as nvarchar(200))    as ASSIGNED_SET_NAME
     , cast(null as varchar(851))     as ASSIGNED_SET_LIST
     , N'Users' as MODULE_TYPE
  from vwUSERS_List
 where PHONE_MOBILE is not null
   and len(PHONE_MOBILE) > 0

GO

Grant Select on dbo.vwUSERS_SmsNumbers to public;
GO


