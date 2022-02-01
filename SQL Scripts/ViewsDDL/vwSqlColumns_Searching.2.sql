if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwSqlColumns_Searching')
	Drop View dbo.vwSqlColumns_Searching;
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
-- 12/08/2007 Paul.  At least one field ends in _ID but is not a unique identifier. 
-- 01/16/2008 Paul.  Simplify conversion to Oracle. 
Create View dbo.vwSqlColumns_Searching
as
select ObjectName
     , ColumnName
     , ColumnType
     , ColumnName as NAME
     , ColumnName as DISPLAY_NAME
     , SqlDbType
     , (case 
        when dbo.fnSqlColumns_IsEnum(ObjectName, ColumnName, CsType) = 1 then N'enum'
        else CsType
        end) as CsType
     , colid
  from vwSqlColumns
 where ColumnName not in (N'ID', N'ID_C')
--   and ColumnName not like '%_ID'      
   and ColumnType <> N'uniqueidentifier'

GO

Grant Select on dbo.vwSqlColumns_Searching to public;
GO


