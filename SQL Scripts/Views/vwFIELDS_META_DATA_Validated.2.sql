if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwFIELDS_META_DATA_Validated')
	Drop View dbo.vwFIELDS_META_DATA_Validated;
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
-- 07/07/2007 Paul.  Join to the Modules table to get the table name. 
-- Custom Fields was failing fror PRODUCT_TEMPLATES and PROJECT_TASK.
-- 07/07/2007 Paul.  The code searches for the custom module by table name, not by module name. 
-- 07/07/2207 Paul.  Use vwMODULES so that deleted flag and module flag are applied. 
-- 02/18/2009 Paul.  Include the module name to simplify code to generate valid workflow update columns. 
-- 02/18/2009 Paul.  We need to know if the column is an identity so the workflow engine can avoid updating it.
Create View dbo.vwFIELDS_META_DATA_Validated
as
select vwFIELDS_META_DATA.ID
     , vwFIELDS_META_DATA.NAME
     , vwFIELDS_META_DATA.LABEL
     , vwMODULES.MODULE_NAME
     , vwMODULES.TABLE_NAME
     , vwMODULES.TABLE_NAME               as CUSTOM_MODULE
     , vwFIELDS_META_DATA.DATA_TYPE
     , vwFIELDS_META_DATA.MAX_SIZE
     , vwFIELDS_META_DATA.REQUIRED_OPTION
     , vwFIELDS_META_DATA.DEFAULT_VALUE
     , vwSqlColumns.CsType
     , vwSqlColumns.colid
     , vwSqlColumns.IsIdentity
  from      vwFIELDS_META_DATA
 inner join vwMODULES
         on vwMODULES.MODULE_NAME   = vwFIELDS_META_DATA.CUSTOM_MODULE
 inner join vwSqlColumns
         on vwSqlColumns.ObjectName = vwMODULES.TABLE_NAME + '_CSTM'
        and vwSqlColumns.ColumnName = vwFIELDS_META_DATA.NAME

GO

Grant Select on dbo.vwFIELDS_META_DATA_Validated to public;
GO

 
