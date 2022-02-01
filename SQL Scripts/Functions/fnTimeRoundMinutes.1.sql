if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'fnTimeRoundMinutes' and ROUTINE_TYPE = 'FUNCTION')
	Drop Function dbo.fnTimeRoundMinutes;
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
Create Function dbo.fnTimeRoundMinutes(@VALUE datetime, @MINUTE_DIVISOR int)
returns datetime
as
  begin
	declare @MINUTES      int;
	declare @SECONDS      int;
	declare @MILLISECONDS int;
	if @VALUE is null or @MINUTE_DIVISOR is null or @MINUTE_DIVISOR <= 0 begin -- then
		return null;
	end -- if;
	set @MINUTES      = datepart(minute     , @VALUE);
	set @SECONDS      = datepart(second     , @VALUE);
	set @MILLISECONDS = datepart(millisecond, @VALUE);
	return dateadd(minute, -(@MINUTES % @MINUTE_DIVISOR), dateadd(second, -@SECONDS, dateadd(millisecond, -@MILLISECONDS, @VALUE)));
  end
GO

Grant Execute on dbo.fnTimeRoundMinutes to public;
GO

