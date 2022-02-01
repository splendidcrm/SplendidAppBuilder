if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwMODULES_TablesSyncd')
	Drop View dbo.vwMODULES_TablesSyncd;
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
-- 10/27/2009 Paul.  We also need the module name. 
Create View dbo.vwMODULES_TablesSyncd
as
select vwMODULES.MODULE_NAME
     , vwSqlTables.TABLE_NAME
     , cast(0 as bit)         as RELATIONSHIP
  from            vwMODULES
       inner join vwSqlTables
               on vwSqlTables.TABLE_NAME = vwMODULES.TABLE_NAME
 where vwMODULES.SYNC_ENABLED = 1
union
select vwMODULES.MODULE_NAME
     , vwSqlTables.TABLE_NAME
     , cast(1 as bit)         as RELATIONSHIP
  from            vwMODULES
       inner join vwSqlTables
               on vwSqlTables.TABLE_NAME           like vwMODULES.TABLE_NAME + N'_%'
  left outer join vwMODULES                             vwMODULES_NotSyncd
               on vwSqlTables.TABLE_NAME           like N'%_' + vwMODULES_NotSyncd.TABLE_NAME
              and (vwMODULES_NotSyncd.SYNC_ENABLED is null or vwMODULES_NotSyncd.SYNC_ENABLED = 0)
 where vwMODULES.SYNC_ENABLED = 1
   and vwMODULES_NotSyncd.ID is null
   and vwSqlTables.TABLE_NAME not in ('USERS_LAST_IMPORT', 'USERS_LOGINS', 'USERS_SIGNATURES')
   and vwSqlTables.TABLE_NAME not like N'%_AUDIT'
   and vwSqlTables.TABLE_NAME not like N'%_REMOTE'
   and vwSqlTables.TABLE_NAME not like N'%_CSTM'
   and vwSqlTables.TABLE_NAME not like N'%_SYNC'

GO

-- select * from vwMODULES_TablesSyncd order by TABLE_NAME

Grant Select on dbo.vwMODULES_TablesSyncd to public;
GO

