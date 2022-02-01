if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'fnStoreTimeOnly' and ROUTINE_TYPE = 'FUNCTION')
	Drop Function dbo.fnStoreTimeOnly;
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
-- 06/01/2007 Paul.  Using convert to get the time is causing a problem on international installations. 
-- The date is internally stored as two 4-byte integers.  Convert to decimal and subtract the date portion. 
-- http://www.sql-server-helper.com/functions/get-date-only.aspx
-- Use decimal(15,8) for better accuracy. 
-- select cast(floor(cast(cast('06/01/2007 11:59:59.998 pm' as datetime) as decimal(15,8))) as datetime)
-- 09/06/2010 Paul.  Help with migration with EffiProz. 
Create Function dbo.fnStoreTimeOnly(@VALUE datetime)
returns datetime
as
  begin
	set @VALUE = cast(cast(@VALUE as decimal(15,8)) - floor(cast(@VALUE as decimal(15,8))) as datetime);
	return @VALUE;
  end
GO

Grant Execute on dbo.fnStoreTimeOnly to public;
GO

