if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwGRIDVIEWS_COLUMNS')
	Drop View dbo.vwGRIDVIEWS_COLUMNS;
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
-- 05/02/2006 Paul.  Add URL_ASSIGNED_FIELD to support ACL. 
-- 05/22/2009 Paul.  Add MODULE_NAME to allow export. 
-- 02/16/2010 Paul.  Add MODULE_TYPE so that we can lookup custom field IDs. 
-- 10/09/2010 Paul.  Add PARENT_FIELD so that we can establish dependent listboxes. 
-- 09/20/2012 Paul.  We need a SCRIPT field that is form specific. 
Create View dbo.vwGRIDVIEWS_COLUMNS
as
select GRIDVIEWS_COLUMNS.ID
     , GRIDVIEWS_COLUMNS.DELETED
     , GRIDVIEWS_COLUMNS.GRID_NAME
     , GRIDVIEWS_COLUMNS.COLUMN_INDEX
     , GRIDVIEWS_COLUMNS.COLUMN_TYPE
     , GRIDVIEWS_COLUMNS.DEFAULT_VIEW
     , GRIDVIEWS_COLUMNS.HEADER_TEXT
     , GRIDVIEWS_COLUMNS.SORT_EXPRESSION
     , GRIDVIEWS_COLUMNS.ITEMSTYLE_WIDTH
     , GRIDVIEWS_COLUMNS.ITEMSTYLE_CSSCLASS
     , GRIDVIEWS_COLUMNS.ITEMSTYLE_HORIZONTAL_ALIGN
     , GRIDVIEWS_COLUMNS.ITEMSTYLE_VERTICAL_ALIGN
     , GRIDVIEWS_COLUMNS.ITEMSTYLE_WRAP
     , GRIDVIEWS_COLUMNS.DATA_FIELD
     , GRIDVIEWS_COLUMNS.DATA_FORMAT
     , GRIDVIEWS_COLUMNS.URL_FIELD
     , GRIDVIEWS_COLUMNS.URL_FORMAT
     , GRIDVIEWS_COLUMNS.URL_TARGET
     , GRIDVIEWS_COLUMNS.LIST_NAME
     , GRIDVIEWS_COLUMNS.URL_MODULE
     , GRIDVIEWS_COLUMNS.URL_ASSIGNED_FIELD
     , GRIDVIEWS.VIEW_NAME
     , GRIDVIEWS.MODULE_NAME
     , GRIDVIEWS_COLUMNS.MODULE_TYPE
     , GRIDVIEWS_COLUMNS.PARENT_FIELD
     , GRIDVIEWS.SCRIPT
  from      GRIDVIEWS_COLUMNS
 inner join GRIDVIEWS
         on GRIDVIEWS.NAME    = GRIDVIEWS_COLUMNS.GRID_NAME
        and GRIDVIEWS.DELETED = 0
 where GRIDVIEWS_COLUMNS.DELETED = 0

GO

Grant Select on dbo.vwGRIDVIEWS_COLUMNS to public;
GO

