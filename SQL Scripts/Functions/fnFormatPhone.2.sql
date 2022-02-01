if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'fnFormatPhone' and ROUTINE_TYPE = 'FUNCTION')
	Drop Function dbo.fnFormatPhone;
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
Create Function dbo.fnFormatPhone(@PHONE nvarchar(25))
returns nvarchar(25)
as
  begin
	declare @FORMATTED nvarchar(25);
	set @FORMATTED = dbo.fnNormalizePhone(@PHONE);
	if @FORMATTED is not null begin -- then
		if substring(@FORMATTED, 1, 1) = '1' and len(@FORMATTED) = 11 begin -- then
			if @FORMATTED like '1[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]' begin -- then
				set @FORMATTED = substring(@FORMATTED, 2, 10);
			end -- if;
		end -- if;
	
		-- 11/24/2017 Paul.  Any phone numbers without 10 characters are returned unmodified, except for trim. 
		if len(@FORMATTED) <> 10 begin -- then
			return ltrim(rtrim(@PHONE));
		end -- if;
	
		-- 11/24/2017 Paul.  Build US standard phone number. 
		set @FORMATTED = '(' + substring(@FORMATTED,1,3) + ') ' + substring(@FORMATTED, 4, 3) + '-' + substring(@FORMATTED, 7 ,4);
	end -- if;
	return @FORMATTED;
  end
GO

Grant Execute on dbo.fnFormatPhone to public
GO

