if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spSqlTableIndexExists' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spSqlTableIndexExists;
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
-- 06/28/2018 Paul.  Output should be int. 
Create Procedure dbo.spSqlTableIndexExists
	( @EXISTS           bit output
	, @TABLE_NAME       nvarchar(80)
	, @INDEX_NAME       nvarchar(120)
	, @ARCHIVE_DATABASE nvarchar(50)
	)
as
  begin
	set nocount on

	declare @COMMAND              nvarchar(max);
	declare @PARAM_DEFINTION      nvarchar(100);
	declare @ARCHIVE_DATABASE_DOT nvarchar(50);
	set @PARAM_DEFINTION = N'@COUNT_VALUE int output';
	set @EXISTS   = 0;
	
	if len(@ARCHIVE_DATABASE) > 0 begin -- then
		set @ARCHIVE_DATABASE_DOT = '[' + @ARCHIVE_DATABASE + '].';
	end else begin
		set @ARCHIVE_DATABASE_DOT = '';
	end -- if;

	set @COMMAND = N'select @COUNT_VALUE = count(*) from ' + @ARCHIVE_DATABASE_DOT + 'sys.indexes where name = ''' + @INDEX_NAME + '''';
	exec sp_executesql @COMMAND, @PARAM_DEFINTION, @COUNT_VALUE = @EXISTS output;
  end
GO


Grant Execute on dbo.spSqlTableIndexExists to public;
GO

/*
declare @EXISTS bit;
exec spSqlTableIndexExists @EXISTS out, 'OPPORTUNITIES', 'IDXR_OPPORTUNITIES_ASSIGNED_SET_ID', 'SplendidCRM_Archive';
print @EXISTS;
*/

