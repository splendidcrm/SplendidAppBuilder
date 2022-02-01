if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwDYNAMIC_BUTTONS')
	Drop View dbo.vwDYNAMIC_BUTTONS;
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
-- 07/28/2010 Paul.  We need a flag to exclude a button from a mobile device. 
-- 03/14/2014 Paul.  Allow hidden buttons to be created. 
-- 08/16/2017 Paul.  Add ability to apply a business rule to a button. 
-- 04/02/2019 Paul.  All modules must have a name field. 
-- 04/02/2019 Paul.  DATE_MODIFIED and DATE_ENTERED for detail view. 
Create View dbo.vwDYNAMIC_BUTTONS
as
select ID
     , VIEW_NAME + isnull(' ' + CONTROL_TEXT, '')     as NAME
     , VIEW_NAME
     , CONTROL_INDEX
     , CONTROL_TYPE
     , DEFAULT_VIEW
     , MODULE_NAME
     , MODULE_ACCESS_TYPE
     , TARGET_NAME
     , TARGET_ACCESS_TYPE
     , MOBILE_ONLY
     , ADMIN_ONLY
     , EXCLUDE_MOBILE
     , CONTROL_TEXT
     , CONTROL_TOOLTIP
     , CONTROL_ACCESSKEY
     , CONTROL_CSSCLASS
     , TEXT_FIELD
     , ARGUMENT_FIELD
     , COMMAND_NAME
     , URL_FORMAT
     , URL_TARGET
     , ONCLICK_SCRIPT
     , HIDDEN
     , BUSINESS_RULE
     , BUSINESS_SCRIPT
     , DATE_ENTERED
     , DATE_MODIFIED
     , DATE_MODIFIED_UTC
  from DYNAMIC_BUTTONS
 where DELETED = 0

GO

Grant Select on dbo.vwDYNAMIC_BUTTONS to public;
GO

