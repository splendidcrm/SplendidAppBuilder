if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwTIMEZONES')
	Drop View dbo.vwTIMEZONES;
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
-- 01/02/2012 Paul.  Add iCal TZID. 
-- 03/26/2013 Paul.  iCloud uses linked_timezone values from http://tzinfo.rubyforge.org/doc/. 
Create View dbo.vwTIMEZONES
as
select ID                   
     , NAME                 
     , STANDARD_NAME        
     , STANDARD_ABBREVIATION
     , DAYLIGHT_NAME        
     , DAYLIGHT_ABBREVIATION
     , BIAS                 
     , STANDARD_BIAS        
     , DAYLIGHT_BIAS        
     , STANDARD_YEAR        
     , STANDARD_MONTH       
     , STANDARD_WEEK        
     , STANDARD_DAYOFWEEK   
     , STANDARD_HOUR        
     , STANDARD_MINUTE      
     , DAYLIGHT_YEAR        
     , DAYLIGHT_MONTH       
     , DAYLIGHT_WEEK        
     , DAYLIGHT_DAYOFWEEK   
     , DAYLIGHT_HOUR        
     , DAYLIGHT_MINUTE      
     , TZID
     , LINKED_TIMEZONE
  from TIMEZONES
 where DELETED = 0

GO

Grant Select on dbo.vwTIMEZONES to public;
GO

