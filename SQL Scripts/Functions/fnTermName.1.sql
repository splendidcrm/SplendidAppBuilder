if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'fnTermName' and ROUTINE_TYPE = 'FUNCTION')
	Drop Function dbo.fnTermName;
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
-- 04/23/2017 Paul.  Module name can be 25 chars. 
Create Function dbo.fnTermName(@MODULE_NAME nvarchar(25), @LIST_NAME nvarchar(50), @NAME nvarchar(50))
returns nvarchar(150)
as
  begin
	declare @TERM_NAME nvarchar(200);
	if @LIST_NAME is null or @LIST_NAME = '' begin -- then
		set @TERM_NAME = isnull(@MODULE_NAME, N'') + N'.' + isnull(@NAME, N'');
	end else begin
		set @TERM_NAME = isnull(@MODULE_NAME, N'') + N'.' + isnull(@LIST_NAME, N'') + N'.' + isnull(@NAME, N'');
	end -- if;
	return @TERM_NAME;
  end
GO

Grant Execute on dbo.fnTermName to public
GO

