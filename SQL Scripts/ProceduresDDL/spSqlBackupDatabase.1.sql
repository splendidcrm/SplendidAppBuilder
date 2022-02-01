if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spSqlBackupDatabase' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spSqlBackupDatabase;
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
-- 02/09/2008 Paul.  Remove the SplendidCRM folder in the backup path.  
-- It is not automatically created and we don't want to create it manually at this time. 
-- 02/25/2008 Paul.  Increase size of DBNAME. 
-- 02/21/2017 Paul.  Allow both parameters to be optional. 
Create Procedure dbo.spSqlBackupDatabase
	( @FILENAME nvarchar(255) = null out
	, @TYPE nvarchar(20) = null
	)
as
  begin
	set nocount on

	-- 12/31/2007 Paul.  The backup is place relative to the default backup directory. 
	declare @TIMESTAMP varchar(30);
	-- 02/25/2008 Paul.  The database name can be large.
	declare @DBNAME    varchar(200);
	declare @NOW       datetime;
	set @NOW    = getdate();
	set @DBNAME = db_name();
	set @TYPE   = upper(@TYPE);
	set @TIMESTAMP = convert(varchar(30), @NOW, 112) + convert(varchar(30), @NOW, 108);
	set @TIMESTAMP = substring(replace(@TIMESTAMP, ':', ''), 1, 12);
	-- 02/21/2017 Paul.  Allow both parameters to be optional. 
	if @TYPE = 'FULL' or @TYPE is null begin -- then
		if @FILENAME is null or @FILENAME = '' begin -- then
			set @FILENAME = @DBNAME + '_db_' + @TIMESTAMP + '.bak';
		end -- if;
		backup database @DBNAME to disk = @FILENAME;
	end else if @TYPE = 'LOG' begin -- then
		if @FILENAME is null or @FILENAME = '' begin -- then
			set @FILENAME = @DBNAME + '_tlog_' + @TIMESTAMP + '.trn';
		end -- if;
		backup log @DBNAME to disk = @FILENAME;
	end else begin
		raiserror(N'Unknown backup type', 16, 1);
	end -- if;
  end
GO


Grant Execute on dbo.spSqlBackupDatabase to public;
GO

-- exec spSqlBackupDatabase null, 'FULL';
-- exec spSqlBackupDatabase null, 'LOG';

