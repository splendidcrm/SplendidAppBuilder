if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwDETAILVIEWS_FIELDS')
	Drop View dbo.vwDETAILVIEWS_FIELDS;
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
-- 12/02/2007 Paul.  Add data columns. 
-- 05/22/2009 Paul.  Add MODULE_NAME to allow export. 
-- 06/12/2009 Paul.  Add TOOL_TIP for help hover.
-- 02/16/2010 Paul.  Add MODULE_TYPE so that we can lookup custom field IDs. 
-- 10/09/2010 Paul.  Add PARENT_FIELD so that we can establish dependent listboxes. 
-- 09/20/2012 Paul.  We need a SCRIPT field that is form specific. 
Create View dbo.vwDETAILVIEWS_FIELDS
as
select DETAILVIEWS_FIELDS.ID
     , DETAILVIEWS_FIELDS.DELETED
     , DETAILVIEWS_FIELDS.DETAIL_NAME
     , DETAILVIEWS_FIELDS.FIELD_INDEX
     , DETAILVIEWS_FIELDS.FIELD_TYPE
     , DETAILVIEWS_FIELDS.DEFAULT_VIEW
     , DETAILVIEWS_FIELDS.DATA_LABEL
     , DETAILVIEWS_FIELDS.DATA_FIELD
     , DETAILVIEWS_FIELDS.DATA_FORMAT
     , DETAILVIEWS_FIELDS.URL_FIELD
     , DETAILVIEWS_FIELDS.URL_FORMAT
     , DETAILVIEWS_FIELDS.URL_TARGET
     , DETAILVIEWS_FIELDS.LIST_NAME
     , DETAILVIEWS_FIELDS.COLSPAN
     , DETAILVIEWS.LABEL_WIDTH
     , DETAILVIEWS.FIELD_WIDTH
     , DETAILVIEWS.DATA_COLUMNS
     , DETAILVIEWS.VIEW_NAME
     , DETAILVIEWS.MODULE_NAME
     , DETAILVIEWS_FIELDS.TOOL_TIP
     , DETAILVIEWS_FIELDS.MODULE_TYPE
     , DETAILVIEWS_FIELDS.PARENT_FIELD
     , DETAILVIEWS.SCRIPT
  from      DETAILVIEWS_FIELDS
 inner join DETAILVIEWS
         on DETAILVIEWS.NAME    = DETAILVIEWS_FIELDS.DETAIL_NAME
        and DETAILVIEWS.DELETED = 0
 where DETAILVIEWS_FIELDS.DELETED = 0

GO

Grant Select on dbo.vwDETAILVIEWS_FIELDS to public;
GO

