if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwIMPORT_MAPS')
	Drop View dbo.vwIMPORT_MAPS;
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
-- 08/01/2010 Paul.  Add ASSIGNED_TO_NAME so that we can display the full name in lists like Sugar. 
-- 08/01/2010 Paul.  Add CREATED_BY_NAME and MODIFIED_BY_NAME so that we can display the full name in lists like Sugar. 
-- 09/17/2013 Paul.  Add Business Rules to import. 
Create View dbo.vwIMPORT_MAPS
as
select IMPORT_MAPS.ID
     , IMPORT_MAPS.NAME
     , IMPORT_MAPS.SOURCE
     , IMPORT_MAPS.MODULE
     , IMPORT_MAPS.HAS_HEADER
     , IMPORT_MAPS.IS_PUBLISHED
     , IMPORT_MAPS.ASSIGNED_USER_ID
     , IMPORT_MAPS.DATE_ENTERED
     , IMPORT_MAPS.DATE_MODIFIED
     , USERS_ASSIGNED.USER_NAME    as ASSIGNED_TO
     , USERS_CREATED_BY.USER_NAME  as CREATED_BY
     , USERS_MODIFIED_BY.USER_NAME as MODIFIED_BY
     , dbo.fnFullName(USERS_ASSIGNED.FIRST_NAME   , USERS_ASSIGNED.LAST_NAME   ) as ASSIGNED_TO_NAME
     , dbo.fnFullName(USERS_CREATED_BY.FIRST_NAME , USERS_CREATED_BY.LAST_NAME ) as CREATED_BY_NAME
     , dbo.fnFullName(USERS_MODIFIED_BY.FIRST_NAME, USERS_MODIFIED_BY.LAST_NAME) as MODIFIED_BY_NAME
     , IMPORT_MAPS.RULES_XML
  from            IMPORT_MAPS
  left outer join USERS USERS_ASSIGNED
               on USERS_ASSIGNED.ID    = IMPORT_MAPS.ASSIGNED_USER_ID
  left outer join USERS USERS_CREATED_BY
               on USERS_CREATED_BY.ID  = IMPORT_MAPS.CREATED_BY
  left outer join USERS USERS_MODIFIED_BY
               on USERS_MODIFIED_BY.ID = IMPORT_MAPS.MODIFIED_USER_ID
 where IMPORT_MAPS.DELETED = 0

GO

Grant Select on dbo.vwIMPORT_MAPS to public;
GO


