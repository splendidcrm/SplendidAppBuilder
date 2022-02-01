if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwUSERS_ASSIGNED_TO_List')
	Drop View dbo.vwUSERS_ASSIGNED_TO_List;
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
-- 12/04/2006 Paul.  Only include active users. 
-- 12/05/2006 Paul.  New users created via NTLM will have a status of NULL. 
-- 04/15/2008 Paul.  Use vwUSERS_ASSIGNED_TO as the base to be similar to vwTEAMS_ASSIGNED_TO_List. 
Create View dbo.vwUSERS_ASSIGNED_TO_List
as
select vwUSERS_List.*
  from      vwUSERS_ASSIGNED_TO
 inner join vwUSERS_List
         on vwUSERS_List.ID = vwUSERS_ASSIGNED_TO.ID

GO

Grant Select on dbo.vwUSERS_ASSIGNED_TO_List to public;
GO


