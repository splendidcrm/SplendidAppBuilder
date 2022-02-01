if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwSqlTablesCachedSystem')
	Drop View dbo.vwSqlTablesCachedSystem;
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
-- 09/24/2009 Paul.  Add the DASHLETS tables so that they are not audited. 
-- 01/17/2010 Paul.  Add ACL Fields. 
-- 12/04/2010 Paul.  Add PAYMENT_GATEWAYS, DISCOUNTS and RULES. 
-- 05/30/2014 Paul.  Add EDITVIEWS_RELATIONSHIPS. 
-- 12/17/2017 Paul.  Add MODULES_ARCHIVE_RELATED. 
-- 07/25/2019 Paul.  Add REACT_CUSTOM_VIEWS. 
Create View dbo.vwSqlTablesCachedSystem
as
select TABLE_NAME
  from vwSqlTables
 where TABLE_NAME in
( N'ACL_ACTIONS'
, N'ACL_FIELDS'
, N'ACL_FIELDS_ALIASES'
, N'ACL_ROLES'
, N'ACL_ROLES_ACTIONS'
, N'ACL_ROLES_USERS'
, N'CONFIG'
, N'CUSTOM_FIELDS'
, N'DASHLETS'
, N'DASHLETS_USERS'
, N'DETAILVIEWS'
, N'DETAILVIEWS_FIELDS'
, N'DETAILVIEWS_RELATIONSHIPS'
, N'DISCOUNTS'
, N'DYNAMIC_BUTTONS'
, N'EDITVIEWS'
, N'EDITVIEWS_FIELDS'
, N'EDITVIEWS_RELATIONSHIPS'
, N'FIELDS_META_DATA'
, N'GRIDVIEWS'
, N'GRIDVIEWS_COLUMNS'
, N'LANGUAGES'
, N'MODULES'
, N'MODULES_ARCHIVE_RELATED'
, N'PAYMENT_GATEWAYS'
, N'REACT_CUSTOM_VIEWS'
, N'RELATIONSHIPS'
, N'RULES'
, N'SHORTCUTS'
, N'TERMINOLOGY'
, N'TERMINOLOGY_ALIASES'
, N'TIMEZONES'
)
GO


Grant Select on dbo.vwSqlTablesCachedSystem to public;
GO

