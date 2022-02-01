if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'fnDateSpecial' and ROUTINE_TYPE = 'FUNCTION')
	Drop Function dbo.fnDateSpecial;
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
-- 02/21/2011 Paul.  Add Tomorrow, Next Week, Next Month, Next Quarter, Next Year. 
Create Function dbo.fnDateSpecial(@NAME varchar(20))
returns datetime
as
  begin
	if @NAME = 'today' begin -- then
		return dbo.fnDateOnly(getdate());
	end else if @NAME = 'yesterday' begin -- then
		return dbo.fnDateAdd('day', -1, dbo.fnDateOnly(getdate()));
	end else if @NAME = 'tomorrow' begin -- then
		return dbo.fnDateAdd('day',  1, dbo.fnDateOnly(getdate()));
	end else if @NAME = 'this week' begin -- then
		return dbo.fnDateAdd('day', -(dbo.fnDatePart('weekday', getdate())-1), dbo.fnDateOnly(getdate()));
	end else if @NAME = 'last week' begin -- then
		return dbo.fnDateAdd('day', -(dbo.fnDatePart('weekday', getdate())-1)-7, dbo.fnDateOnly(getdate()));
	end else if @NAME = 'next week' begin -- then
		return dbo.fnDateAdd('day', -(dbo.fnDatePart('weekday', getdate())-1)+7, dbo.fnDateOnly(getdate()));
	end else if @NAME = 'this month' begin -- then
		return dbo.fnDateAdd('day', -(dbo.fnDatePart('day', getdate())-1), dbo.fnDateOnly(getdate()));
	end else if @NAME = 'last month' begin -- then
		return dbo.fnDateAdd('month', -1, dbo.fnDateAdd('day', -(dbo.fnDatePart('day', getdate())-1), dbo.fnDateOnly(getdate())));
	end else if @NAME = 'next month' begin -- then
		return dbo.fnDateAdd('month',  1, dbo.fnDateAdd('day', -(dbo.fnDatePart('day', getdate())-1), dbo.fnDateOnly(getdate())));
	end else if @NAME = 'this quarter' begin -- then
		return dateadd(qq, datediff(qq, 0, getdate()),  0);
	end else if @NAME = 'last quarter' begin -- then
		return dbo.fnDateAdd('month', -3, dateadd(qq, datediff(qq, 0, getdate()),  0));
	end else if @NAME = 'next quarter' begin -- then
		return dbo.fnDateAdd('month',  3, dateadd(qq, datediff(qq, 0, getdate()),  0));
	end else if @NAME = 'this year' begin -- then
		return dbo.fnDateAdd('month', -(dbo.fnDatePart('month', getdate())-1), dbo.fnDateAdd('day', -(dbo.fnDatePart('day', getdate())-1), dbo.fnDateOnly(getdate())));
	end else if @NAME = 'last year' begin -- then
		return dbo.fnDateAdd('year', -1, dbo.fnDateAdd('month', -(dbo.fnDatePart('month', getdate())-1), dbo.fnDateAdd('day', -(dbo.fnDatePart('day', getdate())-1), dbo.fnDateOnly(getdate()))));
	end else if @NAME = 'next year' begin -- then
		return dbo.fnDateAdd('year',  1, dbo.fnDateAdd('month', -(dbo.fnDatePart('month', getdate())-1), dbo.fnDateAdd('day', -(dbo.fnDatePart('day', getdate())-1), dbo.fnDateOnly(getdate()))));
	end -- if;
	return null;
  end
GO

/*
select dbo.fnDateSpecial('today'       ) as Today
     , dbo.fnDateSpecial('yesterday'   ) as Yesterday
     , dbo.fnDateSpecial('this week'   ) as ThisWeek
     , dbo.fnDateSpecial('last week'   ) as LastWeek
     , dbo.fnDateSpecial('this month'  ) as ThisMonth
     , dbo.fnDateSpecial('last month'  ) as LastMonth
     , dbo.fnDateSpecial('this quarter') as ThisQuarter
     , dbo.fnDateSpecial('last quarter') as LastQuarter
     , dbo.fnDateSpecial('this year'   ) as ThisYear
     , dbo.fnDateSpecial('last year'   ) as LastYear
*/

Grant Execute on dbo.fnDateSpecial to public;
GO

