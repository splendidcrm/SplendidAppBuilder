if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwMODULES_RenameTabs')
	Drop View dbo.vwMODULES_RenameTabs;
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
-- 12/05/2006 Paul.  Literals should be in unicode to reduce conversions at runtime. 
Create View dbo.vwMODULES_RenameTabs
as
select TERMINOLOGY.ID
     , TERMINOLOGY.NAME
     , TERMINOLOGY.LANG
     , TERMINOLOGY.LIST_NAME
     , TERMINOLOGY.LIST_ORDER
     , TERMINOLOGY.DISPLAY_NAME
     , vwMODULES.TAB_ORDER
  from      TERMINOLOGY
 inner join vwMODULES
         on vwMODULES.MODULE_NAME = TERMINOLOGY.NAME
 where TERMINOLOGY.DELETED = 0
   and TERMINOLOGY.LIST_NAME = N'moduleList'
   and vwMODULES.TAB_ENABLED = 1
GO

Grant Select on dbo.vwMODULES_RenameTabs to public;
GO


