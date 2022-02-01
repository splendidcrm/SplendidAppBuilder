if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'fnCONFIG_String' and ROUTINE_TYPE = 'FUNCTION')
	Drop Function dbo.fnCONFIG_String;
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
-- 04/23/2017 Paul.  Deleted flag was not being checked. 
-- 10/22/2017 Paul.  Increase sized of result. 
Create Function dbo.fnCONFIG_String(@NAME nvarchar(60))
returns nvarchar(4000)
as
  begin
	declare @VALUE_varchar nvarchar(4000);
	select top 1 @VALUE_varchar = convert(nvarchar(4000), VALUE)
	  from CONFIG
	 where NAME = @NAME
	   and DELETED = 0;
	return @VALUE_varchar;
  end
GO

Grant Execute on dbo.fnCONFIG_String to public
GO

