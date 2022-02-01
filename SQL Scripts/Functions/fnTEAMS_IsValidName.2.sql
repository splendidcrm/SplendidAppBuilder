if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'fnTEAMS_IsValidName' and ROUTINE_TYPE = 'FUNCTION')
	Drop Function dbo.fnTEAMS_IsValidName;
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
Create Function dbo.fnTEAMS_IsValidName(@ID uniqueidentifier, @NAME nvarchar(128))
returns bit
as
  begin
	declare @IsValid bit;
	set @IsValid = 1;
	if exists(select NAME
	            from dbo.TEAMS
	           where NAME = @NAME 
	             and DELETED   = 0
	             and (ID <> @ID or @ID is null)
	         ) begin -- then
		set @IsValid = 0;
	end -- if;
	return @IsValid;
  end
GO

Grant Execute on dbo.fnTEAMS_IsValidName to public
GO

