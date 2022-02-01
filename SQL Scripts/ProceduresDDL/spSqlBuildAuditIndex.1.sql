if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spSqlBuildAuditIndex' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spSqlBuildAuditIndex;
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
Create Procedure dbo.spSqlBuildAuditIndex(@TABLE_NAME varchar(80))
as
  begin
	set nocount on

	-- 04/25/2011 Paul.  We've stopped supporting SQL 2000, so we can use varchar(max). 
	declare @Command           varchar(max);
	declare @AUDIT_TABLE       varchar(90);
	declare @CRLF              char(2);
	declare @TEST              bit;
	
	set @TEST = 0;
	set @CRLF = char(13) + char(10);
	set @AUDIT_TABLE = @TABLE_NAME + '_AUDIT';
	if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = @AUDIT_TABLE and TABLE_TYPE = 'BASE TABLE') begin -- then
		if exists (select * from sys.indexes where name = 'IDX_' + @AUDIT_TABLE) begin -- then
			set @Command = 'drop   index ' + @AUDIT_TABLE + '.IDX_' + @AUDIT_TABLE;
			if @TEST = 1 begin -- then
				print @Command;
			end else begin
				print @Command;
				exec(@Command);
			end -- if;
		end -- if;

		if right(@TABLE_NAME, 5) = '_CSTM' begin -- then
			-- 11/18/2008 Paul.  Include the audit action in the CSTM table as the workflow engine needs to get just the update action and not the insert. 
			set @Command = 'create index IDX_' + @AUDIT_TABLE + ' on dbo.' + @AUDIT_TABLE + '(ID_C, AUDIT_TOKEN, AUDIT_ACTION)';
			if @TEST = 1 begin -- then
				print @Command;
			end else begin
				print @Command;
				exec(@Command);
			end -- if;
		end -- if;
		-- 12/17/2009 Paul.  end if is the same as having a line break, so lets make it explicit. 
		if exists (select * from vwSqlColumns where ObjectName = @AUDIT_TABLE and ColumnName = 'ID') begin -- then
			set @Command = 'create index IDX_' + @AUDIT_TABLE + ' on dbo.' + @AUDIT_TABLE + '(ID, AUDIT_VERSION, AUDIT_TOKEN)';
			if @TEST = 1 begin -- then
				print @Command;
			end else begin
				print @Command;
				exec(@Command);
			end -- if;
		end -- if;
	end -- if;
  end
GO


Grant Execute on dbo.spSqlBuildAuditIndex to public;
GO

-- exec dbo.spSqlBuildAuditIndex 'CONTACTS';

