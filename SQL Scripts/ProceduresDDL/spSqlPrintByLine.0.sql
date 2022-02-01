if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spSqlPrintByLine' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spSqlPrintByLine;
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
Create Procedure dbo.spSqlPrintByLine
	( @COMMAND nvarchar(max)
	)
as
  begin
	set nocount on

	declare @CurrentPosR  int;
	declare @NextPosR     int;
	declare @CRLF         nchar(2);
	declare @Line         nvarchar(4000);

	set @CRLF = char(13) + char(10);
	set @CurrentPosR = 1;
	while @CurrentPosR <= len(@COMMAND) begin -- do
		set @NextPosR = charindex(@CRLF, @COMMAND,  @CurrentPosR);
		if @NextPosR = 0 or @NextPosR is null begin -- then
			set @NextPosR = len(@COMMAND) + 1;
		end -- if;
		set @Line = substring(@COMMAND, @CurrentPosR, @NextPosR - @CurrentPosR);
		print @Line;
		set @CurrentPosR = @NextPosR + 2;
	end -- while;
  end
GO


Grant Execute on dbo.spSqlPrintByLine to public;
GO

