if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwCURRENCIES_LISTBOX')
	Drop View dbo.vwCURRENCIES_LISTBOX;
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
-- 05/29/2008 Paul.  ISO4217 is needed to process PayPal transactions. 
-- 05/01/2016 Paul.  We are going to prepopulate the list and the ISO4217 is required and unique. 
Create View dbo.vwCURRENCIES_LISTBOX
as
select ID
     , NAME
     , SYMBOL
     , NAME + N': ' + SYMBOL as NAME_SYMBOL
     , CONVERSION_RATE
     , ISO4217
  from CURRENCIES
 where DELETED = 0
   and (STATUS is null or STATUS = N'Active')

GO

Grant Select on dbo.vwCURRENCIES_LISTBOX to public;
GO

 
