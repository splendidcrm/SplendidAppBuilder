if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'fnFullAddressHtml' and ROUTINE_TYPE = 'FUNCTION')
	Drop Function dbo.fnFullAddressHtml;
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
-- 02/14/2014 Kevin.  Convert CRLF to <br /> so that street will display as multiple lines. 
-- 04/25/2016 Paul.  Convert 2-letter country code using contries_dom. 
Create Function dbo.fnFullAddressHtml
	( @ADDRESS_STREET     nvarchar(150)
	, @ADDRESS_CITY       nvarchar(100)
	, @ADDRESS_STATE      nvarchar(100)
	, @ADDRESS_POSTALCODE nvarchar(20)
	, @ADDRESS_COUNTRY    nvarchar(100)
	)
returns nvarchar(500)
as
  begin
	declare @FULL_ADDRESS nvarchar(500);
	if len(@ADDRESS_COUNTRY) = 2 begin -- then
		set @ADDRESS_COUNTRY = dbo.fnTERMINOLOGY_Lookup(@ADDRESS_COUNTRY, N'en-US', null, N'countries_dom');
	end -- if;
	set @FULL_ADDRESS = isnull(replace(@ADDRESS_STREET, char(13) + char(10), N'<br />'), N'') + N'<br>' 
	                  + isnull(@ADDRESS_CITY      , N'') + N' ' 
	                  + isnull(@ADDRESS_STATE     , N'') + N' &nbsp;&nbsp;' 
	                  + isnull(@ADDRESS_POSTALCODE, N'') + N'<br>' 
	                  + isnull(@ADDRESS_COUNTRY   , N'') + N' ';
	return @FULL_ADDRESS;
  end
GO

Grant Execute on dbo.fnFullAddressHtml to public
GO

