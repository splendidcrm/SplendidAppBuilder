if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'fnLocation' and ROUTINE_TYPE = 'FUNCTION')
	Drop Function dbo.fnLocation;
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
-- 08/17/2010 Paul.  Now that we are using this function in the list views, we need to be more efficient. 
Create Function dbo.fnLocation(@CITY nvarchar(100), @STATE nvarchar(100))
returns nvarchar(200)
as
  begin
	declare @DISPLAY_NAME nvarchar(200);
	if @CITY is null begin -- then
		set @DISPLAY_NAME = @STATE;
	end else if @STATE is null begin -- then
		set @DISPLAY_NAME = @CITY;
	end else begin
		set @DISPLAY_NAME = rtrim(isnull(@CITY, N'') + N', ' + isnull(@STATE, N''));
	end -- if;
	return @DISPLAY_NAME;
  end
GO

Grant Execute on dbo.fnLocation to public
GO

