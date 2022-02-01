if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwCURRENCIES_Edit')
	Drop View dbo.vwCURRENCIES_Edit;
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
Create View dbo.vwCURRENCIES_Edit
as
select vwCURRENCIES.*
     , (case when cast(vwCURRENCIES.ID as char(36)) = dbo.fnCONFIG_String('base_currency'   ) or (vwCURRENCIES.ID = 'e340202e-6291-4071-b327-a34cb4df239b' and dbo.fnCONFIG_String('base_currency'   ) is null) then 1 else 0 end) as IS_BASE
     , (case when cast(vwCURRENCIES.ID as char(36)) = dbo.fnCONFIG_String('default_currency') or (vwCURRENCIES.ID = 'e340202e-6291-4071-b327-a34cb4df239b' and dbo.fnCONFIG_String('default_currency') is null) then 1 else 0 end) as IS_DEFAULT
     , USERS_MODIFIED_BY.USER_NAME as MODIFIED_BY
     , dbo.fnFullName(USERS_MODIFIED_BY.FIRST_NAME, USERS_MODIFIED_BY.LAST_NAME) as MODIFIED_BY_NAME
  from            vwCURRENCIES
  left outer join USERS                      USERS_MODIFIED_BY
               on USERS_MODIFIED_BY.ID     = vwCURRENCIES.MODIFIED_USER_ID

GO

Grant Select on dbo.vwCURRENCIES_Edit to public;
GO

 
