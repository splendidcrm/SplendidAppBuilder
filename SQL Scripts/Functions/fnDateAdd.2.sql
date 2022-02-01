if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'fnDateAdd' and ROUTINE_TYPE = 'FUNCTION')
	Drop Function dbo.fnDateAdd;
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
Create Function dbo.fnDateAdd(@DATE_PART varchar(20), @INTERVAL int, @VALUE datetime)
returns datetime
as
  begin
	if @DATE_PART = 'year' begin -- then
		return dateadd(year, @INTERVAL, @VALUE);
	end else if @DATE_PART = 'quarter' begin -- then
		return dateadd(quarter, @INTERVAL, @VALUE);
	end else if @DATE_PART = 'month' begin -- then
		return dateadd(month, @INTERVAL, @VALUE);
	end else if @DATE_PART = 'week' begin -- then
		return dateadd(week, @INTERVAL, @VALUE);
	end else if @DATE_PART = 'day' begin -- then
		return dateadd(day, @INTERVAL, @VALUE);
	end else if @DATE_PART = 'hour' begin -- then
		return dateadd(hour, @INTERVAL, @VALUE);
	end else if @DATE_PART = 'minute' begin -- then
		return dateadd(minute, @INTERVAL, @VALUE);
	end else if @DATE_PART = 'second' begin -- then
		return dateadd(second, @INTERVAL, @VALUE);
	end -- if;
	return null;
  end
GO

Grant Execute on dbo.fnDateAdd to public;
GO

