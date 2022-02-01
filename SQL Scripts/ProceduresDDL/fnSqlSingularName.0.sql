if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'fnSqlSingularName' and ROUTINE_TYPE = 'FUNCTION')
	Drop Function dbo.fnSqlSingularName;
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
Create Function dbo.fnSqlSingularName(@NAME varchar(80))
returns varchar(80)
as
  begin
	declare @SINGULAR_NAME varchar(80);
	if right(@NAME, 3) = 'IES' begin -- then
		set @SINGULAR_NAME = substring(@NAME, 1, len(@NAME) - 3) + 'Y';
	end else if right(@NAME, 1) = 'S' begin -- then
		set @SINGULAR_NAME = substring(@NAME, 1, len(@NAME) - 1);
	end else begin
		set @SINGULAR_NAME = @NAME;
	end -- if;
	return @SINGULAR_NAME;
  end
GO

Grant Execute on dbo.fnSqlSingularName to public
GO



