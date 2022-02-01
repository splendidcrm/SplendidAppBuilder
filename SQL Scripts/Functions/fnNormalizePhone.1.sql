if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'fnNormalizePhone' and ROUTINE_TYPE = 'FUNCTION')
	Drop Function dbo.fnNormalizePhone;
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
-- 11/24/2017 Paul.  Convert empty string to null. 
-- 08/15/2018 Paul.  Use like clause for more flexible phone number lookup. 
Create Function dbo.fnNormalizePhone(@PHONE nvarchar(25))
returns nvarchar(25)
as
  begin
	declare @NORMALIZED nvarchar(25);
	set @NORMALIZED = @PHONE;
	if @NORMALIZED is not null begin -- then
		set @NORMALIZED = replace(@NORMALIZED, N' ', N'');
		set @NORMALIZED = replace(@NORMALIZED, N'+', N'');
		set @NORMALIZED = replace(@NORMALIZED, N'(', N'');
		set @NORMALIZED = replace(@NORMALIZED, N')', N'');
		set @NORMALIZED = replace(@NORMALIZED, N'-', N'');
		set @NORMALIZED = replace(@NORMALIZED, N'.', N'');
		-- 08/15/2018 Paul.  Use like clause for more flexible phone number lookup. 
		set @NORMALIZED = replace(@NORMALIZED, N'[', N'');
		set @NORMALIZED = replace(@NORMALIZED, N']', N'');
		set @NORMALIZED = replace(@NORMALIZED, N'#', N'');
		set @NORMALIZED = replace(@NORMALIZED, N'*', N'');
		set @NORMALIZED = replace(@NORMALIZED, N'%', N'');
		if len(@NORMALIZED) = 0 begin -- then
			set @NORMALIZED = null;
		end -- if;
	end -- if;
	return @NORMALIZED;
  end
GO

Grant Execute on dbo.fnNormalizePhone to public
GO

