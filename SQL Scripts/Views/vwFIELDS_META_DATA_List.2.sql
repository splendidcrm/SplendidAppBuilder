if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwFIELDS_META_DATA_List')
	Drop View dbo.vwFIELDS_META_DATA_List;
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
-- 01/10/2007 Paul.  Add EXT1 for dropdown lists. 
Create View dbo.vwFIELDS_META_DATA_List
as
select ID
     , NAME
     , LABEL
     , CUSTOM_MODULE
     , DATA_TYPE
     , MAX_SIZE
     , REQUIRED_OPTION
     , DEFAULT_VALUE
     , DATE_ENTERED
     , DATE_MODIFIED
     , DATE_MODIFIED_UTC
     , EXT1
  from vwFIELDS_META_DATA

GO

Grant Select on dbo.vwFIELDS_META_DATA_List to public;
GO

 
