if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spSqlBuildSystemTrigger' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spSqlBuildSystemTrigger;
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
-- 04/25/2011 Paul.  We've stopped supporting SQL 2000, so we can use varchar(max). 
-- This also fixes a problem for a customer with 100 custom fields. 
Create Procedure dbo.spSqlBuildSystemTrigger(@TABLE_NAME varchar(80))
as
  begin
	set nocount on
	
	-- 04/25/2011 Paul.  We've stopped supporting SQL 2000, so we can use varchar(max). 
	declare @Command           varchar(max);
	declare @CRLF         char(2);
	declare @TRIGGER_NAME varchar(90);
	declare @COLUMN_NAME  varchar(80);
	declare @COLUMN_TYPE  varchar(20);
	declare @PRIMARY_KEY  varchar(10);
	declare @TEST         bit;

	set @TEST = 0;
	set @PRIMARY_KEY = 'ID';
	if exists (select * from vwSqlTables where TABLE_NAME = @TABLE_NAME) begin -- then
		set @CRLF = char(13) + char(10);

		set @TRIGGER_NAME = 'tr' + @TABLE_NAME + '_System';
		if exists (select * from sys.objects where name = @TRIGGER_NAME and type = 'TR') begin -- then
			set @Command = 'Drop   Trigger dbo.' + @TRIGGER_NAME;
			if @TEST = 0 begin -- then
				print @Command;
				exec(@Command);
			end -- if;
		end -- if;

		if not exists (select * from sys.objects where name = @TRIGGER_NAME and type = 'TR') begin -- then
			-- 07/26/2008 Paul.  Add AUDIT_ACTION to speed workflow processing. 
			set @Command = '';
			set @Command = @Command + 'Create Trigger dbo.' + @TRIGGER_NAME + ' on dbo.' + @TABLE_NAME + @CRLF;
			set @Command = @Command + 'for insert, update' + @CRLF;
			set @Command = @Command + 'as' + @CRLF;
			set @Command = @Command + '  begin' + @CRLF;
			set @Command = @Command + '	declare @BIND_TOKEN varchar(255);' + @CRLF;
			set @Command = @Command + '	exec spSqlGetTransactionToken @BIND_TOKEN out;' + @CRLF;
			set @Command = @Command + '	insert into dbo.SYSTEM_EVENTS' + @CRLF;
			set @Command = @Command + '	     ( ID'            + @CRLF;
			set @Command = @Command + '	     , DATE_ENTERED'  + @CRLF;
			set @Command = @Command + '	     , TABLE_ID'      + @CRLF;
			set @Command = @Command + '	     , TABLE_NAME'    + @CRLF;
			set @Command = @Command + '	     , TABLE_COLUMNS' + @CRLF;
			set @Command = @Command + '	     , TABLE_TOKEN'   + @CRLF;
			set @Command = @Command + '	     , TABLE_ACTION'  + @CRLF;
			set @Command = @Command + '	     )' + @CRLF;
			set @Command = @Command + '	select newid()'       + @CRLF;
			set @Command = @Command + '	     , getdate()'     + @CRLF;
			set @Command = @Command + '	     , inserted.ID'   + @CRLF;
			set @Command = @Command + '	     , ''' + @TABLE_NAME  + '''' + @CRLF;
			set @Command = @Command + '	     , columns_updated()'   + @CRLF;
			set @Command = @Command + '	     , @BIND_TOKEN'         + @CRLF;
			set @Command = @Command + '	     , (case when deleted.ID is null then 0 else 1 end)' + @CRLF;
			set @Command = @Command + '	  from            inserted' + @CRLF;
			set @Command = @Command + '	  left outer join deleted'  + @CRLF;
			set @Command = @Command + '	               on deleted.ID = inserted.ID;' + @CRLF;
			set @Command = @Command + '  end' + @CRLF;
			if @TEST = 1 begin -- then
				print @Command + @CRLF;
			end else begin
				print substring(@Command, 1, charindex(@CRLF, @Command));
				exec(@Command);
			end -- if;
		end -- if;
	end -- if;
  end
GO


Grant Execute on dbo.spSqlBuildSystemTrigger to public;
GO


