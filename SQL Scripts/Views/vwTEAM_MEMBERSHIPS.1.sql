if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwTEAM_MEMBERSHIPS')
	Drop View dbo.vwTEAM_MEMBERSHIPS;
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
-- 11/24/2006 Paul.  We need to make sure that the columns do not match that of any view that will be joined to this one. 
Create View dbo.vwTEAM_MEMBERSHIPS
as
select ID      as MEMBERSHIP_ID
     , TEAM_ID as MEMBERSHIP_TEAM_ID
     , USER_ID as MEMBERSHIP_USER_ID
  from TEAM_MEMBERSHIPS
 where DELETED = 0

GO

Grant Select on dbo.vwTEAM_MEMBERSHIPS to public;
GO

