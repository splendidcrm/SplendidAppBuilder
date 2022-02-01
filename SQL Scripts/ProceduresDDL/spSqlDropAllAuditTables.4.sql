if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spSqlDropAllAuditTables' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spSqlDropAllAuditTables;
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
-- 11/17/2008 Paul.  Drop audit views. 
-- 04/25/2011 Paul.  We've stopped supporting SQL 2000, so we can use varchar(max). 
-- This also fixes a problem for a customer with 100 custom fields. 
Create Procedure dbo.spSqlDropAllAuditTables
as
  begin
	set nocount on

	-- 04/25/2011 Paul.  We've stopped supporting SQL 2000, so we can use varchar(max). 
	declare @Command           varchar(max);
	declare @TABLE_NAME   varchar(80);
	declare @TRIGGER_NAME varchar(90);
	declare AUDIT_TABLES_CURSOR cursor for
	select TABLE_NAME
	  from vwSqlTables
	 where TABLE_NAME like '%_AUDIT'
	order by TABLE_NAME;

	-- 09/14/2008 Paul.  A single space after the procedure simplifies the migration to DB2. 
	exec dbo.spSqlDropAllAuditTriggers ;
	exec dbo.spSqlDropAllAuditViews ;
	
	open AUDIT_TABLES_CURSOR;
	fetch next from AUDIT_TABLES_CURSOR into @TABLE_NAME;
	while @@FETCH_STATUS = 0 begin -- do
		if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = @TABLE_NAME and TABLE_TYPE = 'BASE TABLE') begin -- then
			set @Command = 'Drop Table dbo.' + @TABLE_NAME;
			print @Command;
			exec(@Command);
		end -- if;
		fetch next from AUDIT_TABLES_CURSOR into @TABLE_NAME;
	end -- while;
	close AUDIT_TABLES_CURSOR;
	deallocate AUDIT_TABLES_CURSOR;
  end
GO


Grant Execute on dbo.spSqlDropAllAuditTables to public;
GO

-- exec dbo.spSqlDropAllAuditTables;


