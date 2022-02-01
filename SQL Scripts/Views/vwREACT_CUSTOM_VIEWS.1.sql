if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwREACT_CUSTOM_VIEWS')
	Drop View dbo.vwREACT_CUSTOM_VIEWS;
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
Create View dbo.vwREACT_CUSTOM_VIEWS
as
select REACT_CUSTOM_VIEWS.ID
     , REACT_CUSTOM_VIEWS.NAME
     , REACT_CUSTOM_VIEWS.MODULE_NAME
     , REACT_CUSTOM_VIEWS.CATEGORY
     , MODULES.IS_ADMIN
     , REACT_CUSTOM_VIEWS.CONTENT
  from      REACT_CUSTOM_VIEWS
 inner join MODULES
         on MODULES.MODULE_NAME    = REACT_CUSTOM_VIEWS.MODULE_NAME
        and MODULES.DELETED        = 0
        and MODULES.MODULE_ENABLED = 1
 where REACT_CUSTOM_VIEWS.DELETED = 0

GO

Grant Select on dbo.vwREACT_CUSTOM_VIEWS to public;
GO

