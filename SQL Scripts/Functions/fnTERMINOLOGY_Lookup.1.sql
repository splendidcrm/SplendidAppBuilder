if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'fnTERMINOLOGY_Lookup' and ROUTINE_TYPE = 'FUNCTION')
	Drop Function dbo.fnTERMINOLOGY_Lookup;
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
-- 03/06/2012 Paul.  Increase size of the NAME field so that it can include a date formula. 
Create Function dbo.fnTERMINOLOGY_Lookup
	( @NAME              nvarchar(150)
	, @LANG              nvarchar(10)
	, @MODULE_NAME       nvarchar(25)
	, @LIST_NAME         nvarchar(50)
	)
returns nvarchar(2000)
as
  begin
	declare @DISPLAY_NAME nvarchar(max);
	if @LIST_NAME is not null begin -- then
		set @DISPLAY_NAME = (select top 1 DISPLAY_NAME
		                       from TERMINOLOGY
		                      where LANG        = @LANG
		                        and NAME        = @NAME
		                        and LIST_NAME   = @LIST_NAME
		                        and DELETED     = 0
		                    );
	end else if @MODULE_NAME is not null begin -- then
		set @DISPLAY_NAME = (select top 1 DISPLAY_NAME
		                       from TERMINOLOGY
		                      where LANG        = @LANG
		                        and NAME        = @NAME
		                        and MODULE_NAME = @MODULE_NAME
		                        and DELETED     = 0
		                    );
	end else begin
		set @DISPLAY_NAME = (select top 1 DISPLAY_NAME
		                       from TERMINOLOGY
		                      where LANG        = @LANG
		                        and NAME        = @NAME
		                        and MODULE_NAME is null
		                        and DELETED     = 0
		                    );
	end -- if;
	return @DISPLAY_NAME;
  end
GO

Grant Execute on dbo.fnTERMINOLOGY_Lookup to public
GO

