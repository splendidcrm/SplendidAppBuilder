if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwUSERS_Sync')
	Drop View dbo.vwUSERS_Sync;
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
-- 12/31/2017 Paul.  We should not sync the USERS view directly as it can contain encrypted passwords. 
-- 04/13/2018 Paul.  Add STATUS even though it is always Active as some old code is including it in the query. 
-- 05/18/2020 Paul.  The Reac Client needs access to the portal users. 
-- 05/27/2020 Paul.  Fields needed by React Client: IS_ADMIN, IS_ADMIN_DELEGATE, PORTAL_ONLY, RECEIVE_NOTIFICATIONS
-- 11/10/2020 Paul.  Typical fields used by React Client export: EMPLOYEE_STATUS, MESSENGER_TYPE, IS_GROUP, CREATED_BY_NAME, MODIFIED_BY_NAME, GOOGLEAPPS_SYNC_CONTACTS, GOOGLEAPPS_SYNC_CALENDAR, GOOGLEAPPS_USERNAME
Create View dbo.vwUSERS_Sync
as
select USERS.ID
     , dbo.fnFullName(USERS.FIRST_NAME, USERS.LAST_NAME) as FULL_NAME
     , dbo.fnFullName(USERS.FIRST_NAME, USERS.LAST_NAME) as NAME
     , USERS.USER_NAME
     , USERS.FIRST_NAME
     , USERS.LAST_NAME
     , USERS.REPORTS_TO_ID
     , dbo.fnFullName(REPORTS_TO_USERS.FIRST_NAME, REPORTS_TO_USERS.LAST_NAME) as REPORTS_TO_NAME
     , USERS.TITLE
     , USERS.DEPARTMENT
     , USERS.PHONE_HOME
     , USERS.PHONE_MOBILE
     , USERS.PHONE_WORK
     , USERS.PHONE_OTHER
     , USERS.PHONE_FAX
     , USERS.EMAIL1
     , USERS.EMAIL2
     , USERS.STATUS
     , USERS.ADDRESS_STREET
     , USERS.ADDRESS_CITY
     , USERS.ADDRESS_STATE
     , USERS.ADDRESS_COUNTRY
     , USERS.ADDRESS_POSTALCODE
     , USERS.DATE_ENTERED
     , USERS.DATE_MODIFIED
     , USERS.DATE_MODIFIED_UTC
     , USERS.DESCRIPTION
     , USERS.USER_PREFERENCES
     , USERS.CREATED_BY            as CREATED_BY_ID
     , USERS.MODIFIED_USER_ID
     , USERS.DEFAULT_TEAM
     , TEAMS.NAME                  as DEFAULT_TEAM_NAME
     , USERS.THEME
     , USERS.DATE_FORMAT
     , USERS.TIME_FORMAT
     , USERS.LANG
     , USERS.CURRENCY_ID
     , USERS.TIMEZONE_ID
     , USERS.SAVE_QUERY
     , USERS.GROUP_TABS
     , USERS.SUBPANEL_TABS
     , USERS.EXTENSION
     , USERS.SMS_OPT_IN
     , USERS.PICTURE
     , USERS.PRIMARY_ROLE_ID    as PRIMARY_ROLE_ID
     , replace(replace(ACL_ROLES.NAME, ' ', ''), '''', '') as PRIMARY_ROLE_NAME
     , USERS.IS_ADMIN
     , USERS.IS_ADMIN_DELEGATE
     , USERS.PORTAL_ONLY
     , USERS.RECEIVE_NOTIFICATIONS
     , USERS.EMPLOYEE_STATUS
     , USERS.MESSENGER_TYPE
     , USERS.IS_GROUP
     , dbo.fnFullName(USERS_CREATED_BY.FIRST_NAME , USERS_CREATED_BY.LAST_NAME ) as CREATED_BY_NAME
     , dbo.fnFullName(USERS_MODIFIED_BY.FIRST_NAME, USERS_MODIFIED_BY.LAST_NAME) as MODIFIED_BY_NAME
     , USERS.GOOGLEAPPS_SYNC_CONTACTS
     , USERS.GOOGLEAPPS_SYNC_CALENDAR
     , USERS.GOOGLEAPPS_USERNAME
     , USERS_CSTM.*
  from            USERS
  left outer join USERS REPORTS_TO_USERS
               on REPORTS_TO_USERS.ID       = USERS.REPORTS_TO_ID
              and REPORTS_TO_USERS.DELETED  = 0
  left outer join USERS USERS_CREATED_BY
               on USERS_CREATED_BY.ID       = USERS.CREATED_BY
  left outer join USERS USERS_MODIFIED_BY
               on USERS_MODIFIED_BY.ID      = USERS.MODIFIED_USER_ID
  left outer join USERS_CSTM
               on USERS_CSTM.ID_C           = USERS.ID
  left outer join TEAMS
               on TEAMS.ID                  = USERS.DEFAULT_TEAM
  left outer join ACL_ROLES_USERS
               on ACL_ROLES_USERS.USER_ID   = USERS.ID
              and ACL_ROLES_USERS.ROLE_ID   = USERS.PRIMARY_ROLE_ID
              and ACL_ROLES_USERS.DELETED   = 0
  left outer join ACL_ROLES
               on ACL_ROLES.ID              = ACL_ROLES_USERS.ROLE_ID
              and ACL_ROLES.DELETED         = 0
 where USERS.DELETED = 0
   and USERS.USER_NAME is not null
   and USERS.STATUS = N'Active'
--   and isnull(USERS.PORTAL_ONLY, 0) = 0

GO

Grant Select on dbo.vwUSERS_Sync to public;
GO


