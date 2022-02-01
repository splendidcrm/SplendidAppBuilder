if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwFULLTEXT_INDEXES')
	Drop View dbo.vwFULLTEXT_INDEXES;
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
Create View dbo.vwFULLTEXT_INDEXES
as
select object_name(fulltext_indexes.object_id) as TABLE_NAME
  from      sys.fulltext_indexes                    fulltext_indexes
 inner join sys.fulltext_catalogs                   fulltext_catalogs
         on fulltext_catalogs.fulltext_catalog_id = fulltext_indexes.fulltext_catalog_id
 where fulltext_catalogs.name = db_name() + 'Catalog'
GO

Grant Select on dbo.vwFULLTEXT_INDEXES to public;
GO

-- select * from vwFULLTEXT_INDEXES

