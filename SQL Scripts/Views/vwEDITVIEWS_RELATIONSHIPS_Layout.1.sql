if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwEDITVIEWS_RELATIONSHIPS_Layout')
	Drop View dbo.vwEDITVIEWS_RELATIONSHIPS_Layout;
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
-- 10/29/2015 Paul.  CONTROL_NAME is need to allow copying of the layout. 
Create View dbo.vwEDITVIEWS_RELATIONSHIPS_Layout
as
select EDITVIEWS_RELATIONSHIPS.ID
     , EDITVIEWS_RELATIONSHIPS.EDIT_NAME
     , EDITVIEWS_RELATIONSHIPS.MODULE_NAME
     , EDITVIEWS_RELATIONSHIPS.TITLE
     , EDITVIEWS_RELATIONSHIPS.CONTROL_NAME
     , EDITVIEWS_RELATIONSHIPS.RELATIONSHIP_ORDER
     , EDITVIEWS_RELATIONSHIPS.RELATIONSHIP_ENABLED
     , EDITVIEWS_RELATIONSHIPS.NEW_RECORD_ENABLED
     , EDITVIEWS_RELATIONSHIPS.EXISTING_RECORD_ENABLED
     , EDITVIEWS_RELATIONSHIPS.ALTERNATE_VIEW
  from      EDITVIEWS_RELATIONSHIPS
 inner join MODULES
         on MODULES.MODULE_NAME    = EDITVIEWS_RELATIONSHIPS.MODULE_NAME
        and MODULES.DELETED        = 0
        and MODULES.MODULE_ENABLED = 1
 where EDITVIEWS_RELATIONSHIPS.DELETED = 0

GO

Grant Select on dbo.vwEDITVIEWS_RELATIONSHIPS_Layout to public;
GO

