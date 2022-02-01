if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'fnEmailDisplayName' and ROUTINE_TYPE = 'FUNCTION')
	Drop Function dbo.fnEmailDisplayName;
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
Create Function dbo.fnEmailDisplayName(@FROM_NAME nvarchar(100), @FROM_ADDR nvarchar(100))
returns nvarchar(200)
as
  begin
	declare @DISPLAY_NAME nvarchar(200);
	if @FROM_NAME is null begin -- then
		set @DISPLAY_NAME = N' <' + @FROM_ADDR + N'>';
	end else if @FROM_ADDR is null begin -- then
		set @DISPLAY_NAME = @FROM_NAME;
	end else begin
		set @DISPLAY_NAME = @FROM_NAME + N' <' + @FROM_ADDR + N'>';
	end -- if;
	return @DISPLAY_NAME;
  end
GO

Grant Execute on dbo.fnEmailDisplayName to public
GO

