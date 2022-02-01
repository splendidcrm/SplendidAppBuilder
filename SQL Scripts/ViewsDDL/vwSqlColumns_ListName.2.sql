if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwSqlColumns_ListName')
	Drop View dbo.vwSqlColumns_ListName;
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
-- 02/09/2007 Paul.  Use the EDITVIEWS_FIELDS to determine if a column is an enum. 
Create View dbo.vwSqlColumns_ListName
as
select ObjectName
     , ColumnName                  as DATA_FIELD
     , EDITVIEWS_FIELDS.CACHE_NAME as LIST_NAME
  from      vwSqlColumns
 inner join EDITVIEWS_FIELDS
         on EDITVIEWS_FIELDS.DATA_FIELD   = vwSqlColumns.ColumnName
        and EDITVIEWS_FIELDS.DELETED      = 0
        and EDITVIEWS_FIELDS.FIELD_TYPE   = N'ListBox'
        and EDITVIEWS_FIELDS.DEFAULT_VIEW = 0
        and EDITVIEWS_FIELDS.CACHE_NAME is not null
 inner join EDITVIEWS
         on EDITVIEWS.NAME                = EDITVIEWS_FIELDS.EDIT_NAME
        and EDITVIEWS.VIEW_NAME           = vwSqlColumns.ObjectName + N'_Edit'
        and EDITVIEWS.DELETED             = 0
 where CsType in(N'string', N'ansistring')

GO

Grant Select on dbo.vwSqlColumns_ListName to public;
GO


