if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'fnCustomTableName' and ROUTINE_TYPE = 'FUNCTION')
	Drop Function dbo.fnCustomTableName;
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
-- 12/16/2006 Paul.  Not all module names can be easily converted to a custom module name.  Use the MODULES table to convert. 
Create Function dbo.fnCustomTableName(@MODULE_NAME nvarchar(255))
returns nvarchar(255)
as
  begin
	declare @CUSTOM_NAME nvarchar(255);
	select top 1 @CUSTOM_NAME = TABLE_NAME + N'_CSTM'
	  from MODULES
	 where MODULE_NAME = @MODULE_NAME;

	if @CUSTOM_NAME is null begin -- then
		set @CUSTOM_NAME = @MODULE_NAME + N'_CSTM';
	end -- if;
	return @CUSTOM_NAME;
  end
GO

Grant Execute on dbo.fnCustomTableName to public
GO

