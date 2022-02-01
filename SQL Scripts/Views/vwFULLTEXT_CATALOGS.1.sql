if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwFULLTEXT_CATALOGS')
	Drop View dbo.vwFULLTEXT_CATALOGS;
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
-- https://msdn.microsoft.com/en-us/library/ms190370(v=sql.90).aspx
Create View dbo.vwFULLTEXT_CATALOGS
as
select FullTextCatalogProperty(name,'ItemCount'            ) as ITEM_COUNT
     , FullTextCatalogProperty(name,'MergeStatus'          ) as MERGE_STATUS
     , FullTextCatalogProperty(name,'PopulateCompletionAge') as POPULATE_COMPLETION_AGE
     , (case FullTextCatalogProperty(name,'PopulateStatus')
        when 0 then 'Idle'
        when 1 then 'Full population in progress'
        when 2 then 'Paused'
        when 3 then 'Throttled'
        when 4 then 'Recovering'
        when 5 then 'Shutdown'
        when 6 then 'Incremental population in progress'
        when 7 then 'Building index'
        when 8 then 'Disk is full. Paused.'
        when 9 then 'Change tracking'
        else cast(FullTextCatalogProperty(name,'PopulateStatus') as varchar(4))
        end) as POPULATE_STATUS
     , FullTextCatalogProperty(name,'ImportStatus'         ) as IMPORT_STATUS
     , FullTextCatalogProperty(name,'IndexSize'            ) as INDEX_SIZE
     , FullTextCatalogProperty(name,'UniqueKeyCount'       ) as UNIQUE_KEY_COUNT
     , dateadd(ss, FullTextCatalogProperty(name, 'PopulateCompletionAge'), '1/1/1990') as LAST_POPULATION_DATE
  from sys.fulltext_catalogs
 where name = db_name() + 'Catalog'
GO

Grant Select on dbo.vwFULLTEXT_CATALOGS to public;
GO

-- select * from vwFULLTEXT_CATALOGS

