if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwCURRENCIES')
	Drop View dbo.vwCURRENCIES;
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
-- 04/30/2016 Paul.  Add reference to log entry that modified the record. 
Create View dbo.vwCURRENCIES
as
select ID
     , NAME
     , SYMBOL
     , ISO4217
     , CONVERSION_RATE
     , STATUS
     , SYSTEM_CURRENCY_LOG_ID
     , DATE_MODIFIED
     , MODIFIED_USER_ID
     , CURRENCIES_CSTM.*
  from CURRENCIES
  left outer join CURRENCIES_CSTM
               on CURRENCIES_CSTM.ID_C = CURRENCIES.ID
 where DELETED = 0

GO

Grant Select on dbo.vwCURRENCIES to public;
GO

 
