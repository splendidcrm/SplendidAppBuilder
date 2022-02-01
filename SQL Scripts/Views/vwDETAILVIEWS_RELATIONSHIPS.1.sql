if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwDETAILVIEWS_RELATIONSHIPS')
	Drop View dbo.vwDETAILVIEWS_RELATIONSHIPS;
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
-- 04/27/2006 Paul.  ACL will use the MODULE_NAME.
-- 07/11/2006 Paul.  Disable the relationship if the module is disabled. 
-- 09/08/2007 Paul.  Relationships can now be disabled.
-- 09/08/2007 Paul.  We need a title when we migrate to WebParts. 
-- 01/24/2010 Paul.  We need the ID for report dashlet management. 
-- 01/27/2010 Paul.  Remove the join to DETAILVIEWS so that we can use this table for EditView Relationships. 
-- 10/13/2012 Paul.  Add table info for HTML5 Offline Client. 
Create View dbo.vwDETAILVIEWS_RELATIONSHIPS
as
select DETAILVIEWS_RELATIONSHIPS.ID
     , DETAILVIEWS_RELATIONSHIPS.DETAIL_NAME
     , DETAILVIEWS_RELATIONSHIPS.MODULE_NAME
     , DETAILVIEWS_RELATIONSHIPS.TITLE
     , DETAILVIEWS_RELATIONSHIPS.CONTROL_NAME
     , DETAILVIEWS_RELATIONSHIPS.RELATIONSHIP_ORDER
     , DETAILVIEWS_RELATIONSHIPS.TABLE_NAME
     , DETAILVIEWS_RELATIONSHIPS.PRIMARY_FIELD
     , DETAILVIEWS_RELATIONSHIPS.SORT_FIELD
     , DETAILVIEWS_RELATIONSHIPS.SORT_DIRECTION
  from      DETAILVIEWS_RELATIONSHIPS
 inner join MODULES
         on MODULES.MODULE_NAME    = DETAILVIEWS_RELATIONSHIPS.MODULE_NAME
        and MODULES.DELETED        = 0
        and MODULES.MODULE_ENABLED = 1
 where DETAILVIEWS_RELATIONSHIPS.DELETED = 0
   and DETAILVIEWS_RELATIONSHIPS.RELATIONSHIP_ENABLED = 1

GO

Grant Select on dbo.vwDETAILVIEWS_RELATIONSHIPS to public;
GO

