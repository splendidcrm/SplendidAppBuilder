if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'fnCamelCase' and ROUTINE_TYPE = 'FUNCTION')
	Drop Function dbo.fnCamelCase;
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
Create Function dbo.fnCamelCase(@NAME nvarchar(255))
returns nvarchar(255)
as
  begin
	declare @CAMEL_NAME  nvarchar(255);
	declare @CurrentPosR  int;
	declare @NextPosR     int;
	set @CAMEL_NAME = lower(@NAME);
	set @CAMEL_NAME = upper(left(@CAMEL_NAME, 1)) + substring(@CAMEL_NAME, 2, len(@NAME));

	set @CurrentPosR = 1;
	while charindex(' ', @CAMEL_NAME,  @CurrentPosR) > 0 begin -- do
		set @NextPosR = charindex(' ', @CAMEL_NAME,  @CurrentPosR);
		set @CAMEL_NAME = left(@CAMEL_NAME, @NextPosR-1) + ' ' + upper(substring(@CAMEL_NAME, @NextPosR+1, 1)) + substring(@CAMEL_NAME, @NextPosR+2, len(@NAME));
		set @CurrentPosR = @NextPosR+1;
	end -- while;
	return @CAMEL_NAME;
  end
GO

Grant Execute on dbo.fnCamelCase to public
GO

