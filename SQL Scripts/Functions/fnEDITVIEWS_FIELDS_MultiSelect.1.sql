if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'fnEDITVIEWS_FIELDS_MultiSelect' and ROUTINE_TYPE = 'FUNCTION')
	Drop Function dbo.fnEDITVIEWS_FIELDS_MultiSelect;
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
-- 10/13/2011 Paul.  Special list of EditViews for the search area with IS_MULTI_SELECT. 
-- 09/11/2013 Paul.  A CheckBoxList is also a multi-select. 
Create Function dbo.fnEDITVIEWS_FIELDS_MultiSelect(@MODULE_NAME nvarchar(25), @DATA_FIELD nvarchar(100), @FIELD_TYPE nvarchar(25))
returns bit
as
  begin
	declare @IS_MULTI_SELECT bit;
	set @IS_MULTI_SELECT = 0;
	if @FIELD_TYPE = N'ListBox' or @FIELD_TYPE = N'CheckBoxList' begin -- then
		set @IS_MULTI_SELECT = 0;
		if exists(select *
		            from EDITVIEWS_FIELDS
		           where DELETED      = 0
		             and DEFAULT_VIEW = 0
		             and EDIT_NAME    = @MODULE_NAME + N'.EditView'
		             and DATA_FIELD   = @DATA_FIELD
		             and FIELD_TYPE   in (N'ListBox', N'CheckBoxList')
		             and FORMAT_ROWS  > 0
		         ) begin -- then
			set @IS_MULTI_SELECT = 1;
		end -- if;
	end -- if;
	return @IS_MULTI_SELECT;
  end
GO

Grant Execute on dbo.fnEDITVIEWS_FIELDS_MultiSelect to public
GO

