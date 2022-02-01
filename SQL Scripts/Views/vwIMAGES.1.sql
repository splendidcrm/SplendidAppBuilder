if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwIMAGES')
	Drop View dbo.vwIMAGES;
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
-- 11/23/2010 Paul.  Every module should have a NAME field. 
-- 05/27/2016 Paul.  REST API requires DATE_MODIFIED_UTC. 
Create View dbo.vwIMAGES
as
select IMAGES.ID
     , IMAGES.PARENT_ID
     , IMAGES.FILENAME
     , IMAGES.FILENAME                  as NAME
     , IMAGES.FILE_MIME_TYPE
     , IMAGES.DATE_ENTERED 
     , IMAGES.DATE_MODIFIED_UTC
     , USERS_CREATED_BY.USER_NAME       as CREATED_BY
     , dbo.fnFullName(USERS_CREATED_BY.FIRST_NAME , USERS_CREATED_BY.LAST_NAME ) as CREATED_BY_NAME
  from            IMAGES
  left outer join USERS USERS_CREATED_BY
               on USERS_CREATED_BY.ID = IMAGES.CREATED_BY
 where IMAGES.DELETED = 0

GO

Grant Select on dbo.vwIMAGES to public;
GO

