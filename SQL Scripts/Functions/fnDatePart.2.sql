if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'fnDatePart' and ROUTINE_TYPE = 'FUNCTION')
	Drop Function dbo.fnDatePart;
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
Create Function dbo.fnDatePart(@DATE_PART varchar(20), @VALUE datetime)
returns int
as
  begin
	if          @DATE_PART = 'year'        or @DATE_PART = 'yy' or @DATE_PART = 'yyyy' begin -- then
		return datepart(  year       ,    @VALUE);
	end else if @DATE_PART = 'quarter'     or @DATE_PART = 'qq' or @DATE_PART = 'q' begin -- then
		return datepart(  quarter    ,    @VALUE);
	end else if @DATE_PART = 'month'       or @DATE_PART = 'mm' or @DATE_PART = 'm' begin -- then
		return datepart(  month      ,    @VALUE);
	end else if @DATE_PART = 'dayofyear'   or @DATE_PART = 'dy' or @DATE_PART = 'y' begin -- then
		return datepart(  dayofyear  ,    @VALUE);
	end else if @DATE_PART = 'day'         or @DATE_PART = 'dd' or @DATE_PART = 'd' begin -- then
		return datepart(  day        ,    @VALUE);
	end else if @DATE_PART = 'week'        or @DATE_PART = 'ww' or @DATE_PART = 'wk' begin -- then
		return datepart(  week       ,    @VALUE);
	end else if @DATE_PART = 'weekday'     or @DATE_PART = 'dw' begin -- then
		return datepart(  weekday    ,    @VALUE);
	end else if @DATE_PART = 'hour'        or @DATE_PART = 'hh' begin -- then
		return datepart(  hour       ,    @VALUE);
	end else if @DATE_PART = 'minute'      or @DATE_PART = 'mi' or @DATE_PART = 'n' begin -- then
		return datepart(  minute     ,    @VALUE);
	end else if @DATE_PART = 'second'      or @DATE_PART = 'ss' or @DATE_PART = 's' begin -- then
		return datepart(  second     ,    @VALUE);
	end else if @DATE_PART = 'millisecond' or @DATE_PART = 'ms' begin -- then
		return datepart(  millisecond,    @VALUE);
	end -- if;
	return null;
  end
GO

Grant Execute on dbo.fnDatePart to public;
GO


