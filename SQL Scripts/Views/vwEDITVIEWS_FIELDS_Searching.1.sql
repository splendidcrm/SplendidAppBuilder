if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwEDITVIEWS_FIELDS_Searching')
	Drop View dbo.vwEDITVIEWS_FIELDS_Searching;
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
-- 04/17/2009 Paul.  Key off of the view name so that we don't have to change other areas of the code. 
Create View dbo.vwEDITVIEWS_FIELDS_Searching
as
select EDITVIEWS_FIELDS_Search.EDIT_NAME
     , EDITVIEWS_Search.VIEW_NAME
     , EDITVIEWS_Search.MODULE_NAME
     , EDITVIEWS_FIELDS_Module.DATA_FIELD
  from      EDITVIEWS_FIELDS                    EDITVIEWS_FIELDS_Search
 inner join EDITVIEWS                           EDITVIEWS_Search
         on EDITVIEWS_Search.NAME             = EDITVIEWS_FIELDS_Search.EDIT_NAME
        and EDITVIEWS_Search.DELETED          = 0
 inner join EDITVIEWS                           EDITVIEWS_Module
         on EDITVIEWS_Module.MODULE_NAME      = EDITVIEWS_Search.MODULE_NAME
        and EDITVIEWS_Module.DELETED          = 0
 inner join EDITVIEWS_FIELDS                    EDITVIEWS_FIELDS_Module
         on EDITVIEWS_FIELDS_Module.EDIT_NAME = EDITVIEWS_Module.NAME
        and EDITVIEWS_FIELDS_Module.DELETED   = 0
 where EDITVIEWS_FIELDS_Search.DELETED   = 0
   and (EDITVIEWS_FIELDS_Module.DEFAULT_VIEW = 0 or EDITVIEWS_FIELDS_Module.DEFAULT_VIEW is null)
   and EDITVIEWS_FIELDS_Module.DATA_FIELD is not null
union
select EDITVIEWS_FIELDS_Search.EDIT_NAME
     , EDITVIEWS_Search.VIEW_NAME
     , EDITVIEWS_Search.MODULE_NAME
     , EDITVIEWS_FIELDS_Module.DISPLAY_FIELD
  from      EDITVIEWS_FIELDS                    EDITVIEWS_FIELDS_Search
 inner join EDITVIEWS                           EDITVIEWS_Search
         on EDITVIEWS_Search.NAME             = EDITVIEWS_FIELDS_Search.EDIT_NAME
        and EDITVIEWS_Search.DELETED          = 0
 inner join EDITVIEWS                           EDITVIEWS_Module
         on EDITVIEWS_Module.MODULE_NAME      = EDITVIEWS_Search.MODULE_NAME
        and EDITVIEWS_Module.DELETED          = 0
 inner join EDITVIEWS_FIELDS                    EDITVIEWS_FIELDS_Module
         on EDITVIEWS_FIELDS_Module.EDIT_NAME = EDITVIEWS_Module.NAME
        and EDITVIEWS_FIELDS_Module.DELETED   = 0
 where EDITVIEWS_FIELDS_Search.DELETED   = 0
   and (EDITVIEWS_FIELDS_Module.DEFAULT_VIEW = 0 or EDITVIEWS_FIELDS_Module.DEFAULT_VIEW is null)
   and EDITVIEWS_FIELDS_Module.DISPLAY_FIELD is not null

GO

Grant Select on dbo.vwEDITVIEWS_FIELDS_Searching to public;
GO

