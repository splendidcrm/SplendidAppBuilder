if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwUSERS_ASSIGNED_TO')
	Drop View dbo.vwUSERS_ASSIGNED_TO;
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
-- 03/06/2006 Paul.  Oracle does not like <> ''.  Use len() > 0 instead. 
-- 12/04/2006 Paul.  Only include active users. 
-- 12/05/2006 Paul.  New users created via NTLM will have a status of NULL. 
-- 03/05/2009 Paul.  A Portal user should not be assignable. 
-- 08/02/2016 Paul.  This view will be used to get round-robin users to assign to a process. 
Create View dbo.vwUSERS_ASSIGNED_TO
as
select ID
     , USER_NAME
     , DATE_ENTERED
  from USERS
 where USER_NAME is not null
   and len(USER_NAME) > 0
   and (STATUS is null or STATUS = N'Active')
   and (PORTAL_ONLY is null or PORTAL_ONLY = 0)
   and DELETED = 0

GO

Grant Select on dbo.vwUSERS_ASSIGNED_TO to public;
GO




