if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwDETAILVIEWS_RELATIONSHIPS_La')
	Drop View dbo.vwDETAILVIEWS_RELATIONSHIPS_La;
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
-- 09/08/2007 Paul.  vwDETAILVIEWS_RELATIONSHIPS_Layout is too long for Oracle, so reduce to 30 characters. 
-- 09/08/2007 Paul.  We need a title when we migrate to WebParts. 
-- 01/27/2010 Paul.  Remove the join to DETAILVIEWS so that we can use this table for EditView Relationships. 
-- 10/13/2012 Paul.  Add table info for HTML5 Offline Client. 
-- 02/14/2013 Paul.  Add CONTROL_NAME to make it easy to copy. 
-- 03/30/2022 Paul.  Add Insight fields. 
Create View dbo.vwDETAILVIEWS_RELATIONSHIPS_La
as
select DETAILVIEWS_RELATIONSHIPS.ID
     , DETAILVIEWS_RELATIONSHIPS.DETAIL_NAME
     , DETAILVIEWS_RELATIONSHIPS.MODULE_NAME
     , DETAILVIEWS_RELATIONSHIPS.TITLE
     , DETAILVIEWS_RELATIONSHIPS.CONTROL_NAME
     , DETAILVIEWS_RELATIONSHIPS.RELATIONSHIP_ORDER
     , DETAILVIEWS_RELATIONSHIPS.RELATIONSHIP_ENABLED
     , DETAILVIEWS_RELATIONSHIPS.TABLE_NAME
     , DETAILVIEWS_RELATIONSHIPS.PRIMARY_FIELD
     , DETAILVIEWS_RELATIONSHIPS.SORT_FIELD
     , DETAILVIEWS_RELATIONSHIPS.SORT_DIRECTION
     , DETAILVIEWS_RELATIONSHIPS.INSIGHT_LABEL
     , DETAILVIEWS_RELATIONSHIPS.INSIGHT_VIEW
     , DETAILVIEWS_RELATIONSHIPS.INSIGHT_OPERATOR
  from      DETAILVIEWS_RELATIONSHIPS
 inner join MODULES
         on MODULES.MODULE_NAME    = DETAILVIEWS_RELATIONSHIPS.MODULE_NAME
        and MODULES.DELETED        = 0
        and MODULES.MODULE_ENABLED = 1
 where DETAILVIEWS_RELATIONSHIPS.DELETED = 0

GO

Grant Select on dbo.vwDETAILVIEWS_RELATIONSHIPS_La to public;
GO

