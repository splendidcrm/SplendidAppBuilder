if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwUSER_PREFERENCES')
	Drop View dbo.vwUSER_PREFERENCES;
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
-- 10/28/2009 Paul.  Add UTC date to allow this table to sync. 
Create View dbo.vwUSER_PREFERENCES
as
select USER_PREFERENCES.ID
     , USER_PREFERENCES.CATEGORY
     , USER_PREFERENCES.ASSIGNED_USER_ID
     , USER_PREFERENCES.DATE_MODIFIED
     , USER_PREFERENCES.DATE_MODIFIED_UTC
     , lower(USERS.USER_NAME)             as ASSIGNED_USER_NAME
  from            USER_PREFERENCES
  left outer join USERS
               on USERS.ID = USER_PREFERENCES.ASSIGNED_USER_ID
 where USER_PREFERENCES.DELETED = 0

GO

Grant Select on dbo.vwUSER_PREFERENCES to public;
GO

