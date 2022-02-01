if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'fnCombineAddress' and ROUTINE_TYPE = 'FUNCTION')
	Drop Function dbo.fnCombineAddress;
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
Create Function dbo.fnCombineAddress
	( @ADDRESS_STREET1    nvarchar(150)
	, @ADDRESS_STREET2    nvarchar(150)
	, @ADDRESS_STREET3    nvarchar(150)
	, @ADDRESS_STREET4    nvarchar(150)
	)
returns nvarchar(600)
as
  begin
	declare @FULL_ADDRESS nvarchar(600);
	set @FULL_ADDRESS = @ADDRESS_STREET1;
	if @ADDRESS_STREET2 is not null and len(@ADDRESS_STREET2) > 0 begin -- then
		if @FULL_ADDRESS is not null begin -- then
			set @FULL_ADDRESS = isnull(@FULL_ADDRESS, N'') + nchar(13) + nchar(10);
		end -- if;
		set @FULL_ADDRESS = isnull(@FULL_ADDRESS, N'') + @ADDRESS_STREET2;
	end -- if;
	if @ADDRESS_STREET3 is not null and len(@ADDRESS_STREET3) > 0 begin -- then
		if @FULL_ADDRESS is not null begin -- then
			set @FULL_ADDRESS = isnull(@FULL_ADDRESS, N'') + nchar(13) + nchar(10);
		end -- if;
		set @FULL_ADDRESS = isnull(@FULL_ADDRESS, N'') + @ADDRESS_STREET3;
	end -- if;
	if @ADDRESS_STREET4 is not null and len(@ADDRESS_STREET4) > 0 begin -- then
		if @FULL_ADDRESS is not null begin -- then
			set @FULL_ADDRESS = isnull(@FULL_ADDRESS, N'') + nchar(13) + nchar(10);
		end -- if;
		set @FULL_ADDRESS = isnull(@FULL_ADDRESS, N'') + @ADDRESS_STREET4;
	end -- if;
	return @FULL_ADDRESS;
  end
GO

Grant Execute on dbo.fnCombineAddress to public
GO

