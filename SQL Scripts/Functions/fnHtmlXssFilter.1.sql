if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'fnHtmlXssFilter' and ROUTINE_TYPE = 'FUNCTION')
	Drop Function dbo.fnHtmlXssFilter;
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
-- 01/06/2022 Paul.  We need a way to filter EMAILS.DESCRIPTION_HTML in the database. 
-- Ideally we would use the CONFIG.email_xss set, but that would be too slow, so manually code. 
Create Function dbo.fnHtmlXssFilter(@HTML nvarchar(max))
returns nvarchar(max)
as
  begin
	declare @VALUE nvarchar(max);
	set @VALUE = @HTML;
	if @VALUE is not null begin -- then
		-- 01/06/2022 Paul.  To be efficient, we are going to just disable the start tag and ignore the end tag. 
		set @VALUE = replace(@VALUE, '<html', '<xhtml');
		set @VALUE = replace(@VALUE, '<body', '<xbody');
		set @VALUE = replace(@VALUE, '<base', '<xbase');
		set @VALUE = replace(@VALUE, '<form', '<xform');
		set @VALUE = replace(@VALUE, '<meta', '<xmeta');
		set @VALUE = replace(@VALUE, '<style', '<xstyle');
		set @VALUE = replace(@VALUE, '<embed', '<xembed');
		set @VALUE = replace(@VALUE, '<object', '<xobject');
		set @VALUE = replace(@VALUE, '<script', '<xscript');
		set @VALUE = replace(@VALUE, '<iframe', '<xiframe');
	end -- if;
	return @VALUE;
  end
GO

Grant Execute on dbo.fnHtmlXssFilter to public
GO

