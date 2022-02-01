-- 09/07/2010 Paul.  Not sure why we are dropping this view after creating it, but it does not appear to be used anywhere. 
-- 09/10/2010 Paul.  This was just an old version of the view.  Move to the top. 
if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwDETAILVIEWS_USERS')
	Drop View dbo.vwDETAILVIEWS_USERS;
GO


if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwDASHLETS_USERS')
	Drop View dbo.vwDASHLETS_USERS;
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
-- 07/10/2009 Paul.  Defer the enabled check to the code so that we can see if data exists at all. 
-- 07/12/2009 Paul.  Include RELATIONSHIP_ORDER column to be compatible with DETAILVIEWS_RELATIONSHIPS view. 
-- 03/09/2014 Paul.  User dashlets do notrequire a DetailView record.  The filter was causing problems with Module dashlets. 
Create View dbo.vwDASHLETS_USERS
as
select DASHLETS_USERS.ID
     , DASHLETS_USERS.ASSIGNED_USER_ID
     , DASHLETS_USERS.DETAIL_NAME
     , DASHLETS_USERS.MODULE_NAME
     , DASHLETS_USERS.TITLE
     , DASHLETS_USERS.CONTROL_NAME
     , DASHLETS_USERS.DASHLET_ORDER
     , DASHLETS_USERS.DASHLET_ENABLED
     , DASHLETS_USERS.DASHLET_ORDER     as RELATIONSHIP_ORDER
  from      DASHLETS_USERS
-- inner join DETAILVIEWS
--         on DETAILVIEWS.NAME       = DASHLETS_USERS.DETAIL_NAME
--        and DETAILVIEWS.DELETED    = 0
 inner join MODULES
         on MODULES.MODULE_NAME    = DASHLETS_USERS.MODULE_NAME
        and MODULES.DELETED        = 0
        and MODULES.MODULE_ENABLED = 1
 where DASHLETS_USERS.DELETED = 0

GO

Grant Select on dbo.vwDASHLETS_USERS to public;
GO

