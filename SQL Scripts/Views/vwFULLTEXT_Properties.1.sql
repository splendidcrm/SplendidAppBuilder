if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwFULLTEXT_Properties')
	Drop View dbo.vwFULLTEXT_Properties;
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
Create View dbo.vwFULLTEXT_Properties
as
select substring(@@version, 1, charindex(char(10), @@version)) as SQL_SERVER_VERSION
     , ServerProperty('Edition')             as SQL_SERVER_EDITION
     , db_name()                             as DATABASE_NAME
     , ServerProperty('IsFullTextInstalled') as FULLTEXT_INSTALLED
     , (select fulltext_catalog_id from sys.fulltext_catalogs where name = db_name() + 'Catalog') as FULLTEXT_CATALOG_ID
     , (select 1 from sys.fulltext_document_types where document_type = '.pptx') as OFFICE_DOCUMENT_TYPE
     , (select 1 from sys.fulltext_document_types where document_type = '.pdf' ) as PDF_DOCUMENT_TYPE
GO

Grant Select on dbo.vwFULLTEXT_Properties to public;
GO

-- select * from vwFULLTEXT_Properties

