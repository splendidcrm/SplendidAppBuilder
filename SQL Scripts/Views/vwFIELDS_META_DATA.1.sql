if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwFIELDS_META_DATA')
	Drop View dbo.vwFIELDS_META_DATA;
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
-- 04/21/2006 Paul.  MASS_UPDATE was added in SugarCRM 4.0.
Create View dbo.vwFIELDS_META_DATA
as
select ID
     , NAME
     , LABEL
     , CUSTOM_MODULE
     , DATA_TYPE
     , MAX_SIZE
     , REQUIRED_OPTION
     , AUDITED
     , DEFAULT_VALUE
     , EXT1
     , EXT2
     , EXT3
     , MASS_UPDATE
     , DATE_ENTERED
     , DATE_MODIFIED
     , DATE_MODIFIED_UTC
  from FIELDS_META_DATA
 where DELETED = 0

GO

Grant Select on dbo.vwFIELDS_META_DATA to public;
GO

