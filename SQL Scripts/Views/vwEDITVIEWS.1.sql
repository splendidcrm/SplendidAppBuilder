if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwEDITVIEWS')
	Drop View dbo.vwEDITVIEWS;
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
-- 10/30/2010 Paul.  Add support for Business Rules Framework. 
-- 11/11/2010 Paul.  Change to Pre Load and Post Load. 
-- 09/20/2012 Paul.  We need a SCRIPT field that is form specific. 
-- 02/14/2013 Paul.  Add DATA_COLUMNS. 
Create View dbo.vwEDITVIEWS
as
select EDITVIEWS.ID
     , EDITVIEWS.NAME
     , EDITVIEWS.MODULE_NAME
     , EDITVIEWS.VIEW_NAME
     , EDITVIEWS.LABEL_WIDTH
     , EDITVIEWS.FIELD_WIDTH
     , EDITVIEWS.SCRIPT
     , EDITVIEWS.DATA_COLUMNS
     , NEW_EVENT_RULES.ID           as NEW_EVENT_ID
     , NEW_EVENT_RULES.NAME         as NEW_EVENT_NAME
     , PRE_LOAD_EVENT_RULES.ID      as PRE_LOAD_EVENT_ID
     , PRE_LOAD_EVENT_RULES.NAME    as PRE_LOAD_EVENT_NAME
     , POST_LOAD_EVENT_RULES.ID     as POST_LOAD_EVENT_ID
     , POST_LOAD_EVENT_RULES.NAME   as POST_LOAD_EVENT_NAME
     , VALIDATION_EVENT_RULES.ID    as VALIDATION_EVENT_ID
     , VALIDATION_EVENT_RULES.NAME  as VALIDATION_EVENT_NAME
     , PRE_SAVE_EVENT_RULES.ID      as PRE_SAVE_EVENT_ID
     , PRE_SAVE_EVENT_RULES.NAME    as PRE_SAVE_EVENT_NAME
     , POST_SAVE_EVENT_RULES.ID     as POST_SAVE_EVENT_ID
     , POST_SAVE_EVENT_RULES.NAME   as POST_SAVE_EVENT_NAME
  from            EDITVIEWS
  left outer join RULES                            NEW_EVENT_RULES
               on NEW_EVENT_RULES.ID             = EDITVIEWS.NEW_EVENT_ID
              and NEW_EVENT_RULES.DELETED        = 0
  left outer join RULES                            PRE_LOAD_EVENT_RULES
               on PRE_LOAD_EVENT_RULES.ID        = EDITVIEWS.PRE_LOAD_EVENT_ID
              and PRE_LOAD_EVENT_RULES.DELETED   = 0
  left outer join RULES                            POST_LOAD_EVENT_RULES
               on POST_LOAD_EVENT_RULES.ID       = EDITVIEWS.POST_LOAD_EVENT_ID
              and POST_LOAD_EVENT_RULES.DELETED  = 0
  left outer join RULES                            VALIDATION_EVENT_RULES
               on VALIDATION_EVENT_RULES.ID      = EDITVIEWS.VALIDATION_EVENT_ID
              and VALIDATION_EVENT_RULES.DELETED = 0
  left outer join RULES                            PRE_SAVE_EVENT_RULES
               on PRE_SAVE_EVENT_RULES.ID        = EDITVIEWS.PRE_SAVE_EVENT_ID
              and PRE_SAVE_EVENT_RULES.DELETED   = 0
  left outer join RULES                            POST_SAVE_EVENT_RULES
               on POST_SAVE_EVENT_RULES.ID       = EDITVIEWS.POST_SAVE_EVENT_ID
              and POST_SAVE_EVENT_RULES.DELETED  = 0
 where EDITVIEWS.DELETED = 0

GO

Grant Select on dbo.vwEDITVIEWS to public;
GO

