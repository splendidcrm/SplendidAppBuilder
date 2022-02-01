if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'fnTERMINOLOGY_Changed' and ROUTINE_TYPE = 'FUNCTION')
	Drop Function dbo.fnTERMINOLOGY_Changed;
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
-- 07/24/2006 Paul.  Increase the MODULE_NAME to 25 to match the size in the MODULES table.
-- 01/14/2010 Paul.  In order to detect a case-significant change in the DISPLAY_NAME, first convert to binary. 
-- http://vyaskn.tripod.com/case_sensitive_search_in_sql_server.htm
-- 03/06/2012 Paul.  Increase size of the NAME field so that it can include a date formula. 
Create Function dbo.fnTERMINOLOGY_Changed
	( @NAME              nvarchar(150)
	, @LANG              nvarchar(10)
	, @MODULE_NAME       nvarchar(25)
	, @LIST_NAME         nvarchar(50)
	, @LIST_ORDER        int
	, @DISPLAY_NAME      nvarchar(max)
	)
returns bit
as
  begin
	declare @Changed bit;
	set @Changed = 0;
	if not exists(select *
	                from TERMINOLOGY
	               where DELETED = 0
	                 and (NAME         = @NAME         or (NAME         is null and @NAME         is null))
	                 and (LANG         = @LANG         or (LANG         is null and @LANG         is null))
	                 and (MODULE_NAME  = @MODULE_NAME  or (MODULE_NAME  is null and @MODULE_NAME  is null))
	                 and (LIST_NAME    = @LIST_NAME    or (LIST_NAME    is null and @LIST_NAME    is null))
	                 and (cast(DISPLAY_NAME as varbinary(4000)) = cast(@DISPLAY_NAME as varbinary(4000)) or (DISPLAY_NAME is null and @DISPLAY_NAME is null))
	                 and isnull(LIST_ORDER, 0) = isnull(@LIST_ORDER, 0)) begin -- then
		set @Changed = 1;
	end -- if;
	return @Changed;
  end
GO

Grant Execute on dbo.fnTERMINOLOGY_Changed to public
GO

