if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'fnSqlColumns_IsEnum' and ROUTINE_TYPE = 'FUNCTION')
	Drop Function dbo.fnSqlColumns_IsEnum;
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
-- 02/09/2007 Paul.  Use the EDITVIEWS_FIELDS to determine if a column is an enum. 
-- 09/16/2010 Paul.  CsType can be SqlDbType.DateTime. 
-- 12/12/2010 Paul.  EffiProz needs the ColumnName field to be greater than 35 due to an internal variable. 
-- 09/13/2011 Paul.  The Workflow EditView will append _AUDIT to the table name, so we need to remove that. 
-- Workflow EditView appends _AUDIT to prevent the inclusion of addtional fields in the base view, such as CITY in the vwACCOUNTS view. 
Create Function dbo.fnSqlColumns_IsEnum(@ModuleView nvarchar(50), @ColumnName nvarchar(50), @CsType nvarchar(20))
returns bit
as
  begin
	declare @IS_ENUM bit;
	declare @TableView nvarchar(50)
	set @IS_ENUM = 0;
	set @TableView = @ModuleView;
	if right(@TableView, 6) = '_AUDIT' begin -- then
		set @TableView = substring(@TableView, 1, len(@TableView) - 6);
	end -- if;
	if @CsType = N'string' or @CsType = N'ansistring' begin -- then
		if exists(select *
		            from      EDITVIEWS_FIELDS
		           inner join EDITVIEWS
		                   on EDITVIEWS.NAME      = EDITVIEWS_FIELDS.EDIT_NAME
		                  and EDITVIEWS.VIEW_NAME = @TableView + N'_Edit'
		                  and EDITVIEWS.DELETED   = 0
		           where EDITVIEWS_FIELDS.DELETED = 0
	                     and EDITVIEWS_FIELDS.FIELD_TYPE   = N'ListBox'
		             and EDITVIEWS_FIELDS.DEFAULT_VIEW = 0
		             and EDITVIEWS_FIELDS.DATA_FIELD   = @ColumnName
		             and EDITVIEWS_FIELDS.CACHE_NAME is not null) begin -- then
			set @IS_ENUM = 1;
		end -- if;
	end -- if;
	return @IS_ENUM;
  end
GO

Grant Execute on dbo.fnSqlColumns_IsEnum to public
GO

