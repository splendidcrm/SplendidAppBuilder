if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwTERMINOLOGY_Active')
	Drop View dbo.vwTERMINOLOGY_Active;
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
Create View dbo.vwTERMINOLOGY_Active
as
select vwTERMINOLOGY.ID
     , vwTERMINOLOGY.NAME
     , vwTERMINOLOGY.LANG
     , vwTERMINOLOGY.MODULE_NAME
     , vwTERMINOLOGY.LIST_NAME
     , vwTERMINOLOGY.LIST_ORDER
     , vwTERMINOLOGY.DISPLAY_NAME
  from      vwLANGUAGES_Active
 inner join vwTERMINOLOGY
         on vwTERMINOLOGY.LANG = vwLANGUAGES_Active.NAME

GO

Grant Select on dbo.vwTERMINOLOGY_Active to public;
GO

