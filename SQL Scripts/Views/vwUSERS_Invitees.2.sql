if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwUSERS_Invitees')
	Drop View dbo.vwUSERS_Invitees;
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
-- 11/07/2005 Paul.  SQL Server needs the cast in order to compile vwACTIVITIES_Invitees.
-- 02/01/2006 Paul.  DB2 does not like comments in the middle of the Create View statement. 
-- 12/11/2009 Paul.  Only show active users and also exclude portal users. 
-- 12/12/2009 Paul.  We do not need to return the STATUS. 
-- If we include the status, then we would need a dummy STATUS field in the vwCONTACTS_Invitees view. 
Create View dbo.vwUSERS_Invitees
as
select ID          as ID
     , N'Users'    as INVITEE_TYPE
     , FULL_NAME   as NAME
     , FIRST_NAME  as FIRST_NAME
     , LAST_NAME   as LAST_NAME
     , EMAIL1      as EMAIL
     , PHONE_WORK  as PHONE
     , cast(null as uniqueidentifier) as ASSIGNED_USER_ID
  from vwUSERS
 where (STATUS      is null or STATUS      = N'Active')
   and (PORTAL_ONLY is null or PORTAL_ONLY = 0        )

GO

Grant Select on dbo.vwUSERS_Invitees to public;
GO


