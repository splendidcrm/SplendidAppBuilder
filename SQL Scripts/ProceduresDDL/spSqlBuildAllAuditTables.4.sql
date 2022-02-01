if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spSqlBuildAllAuditTables' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spSqlBuildAllAuditTables;
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
-- 11/17/2008 Paul.  Build audit views. 
Create Procedure dbo.spSqlBuildAllAuditTables
as
  begin
	set nocount on
	print N'spSqlBuildAllAuditTables';

	declare @TABLE_NAME varchar(80);
	declare TABLES_CURSOR cursor for
	select TABLE_NAME
	  from vwSqlTablesAudited
	order by TABLE_NAME;
	
	-- 07/25/2009 Paul.  We need to add a rowversion field to any sync'd table. 
	exec dbo.spSqlUpdateSyncdTables ;

	open TABLES_CURSOR;
	fetch next from TABLES_CURSOR into @TABLE_NAME;
	while @@FETCH_STATUS = 0 begin -- do
		exec dbo.spSqlBuildAuditTable @TABLE_NAME;
		fetch next from TABLES_CURSOR into @TABLE_NAME;
	end -- while;
	close TABLES_CURSOR;
	deallocate TABLES_CURSOR;

	-- 09/14/2008 Paul.  A single space after the procedure simplifies the migration to DB2. 
	exec dbo.spSqlBuildAllAuditTriggers ;
	exec dbo.spSqlBuildAllAuditViews ;
  end
GO


Grant Execute on dbo.spSqlBuildAllAuditTables to public;
GO

-- exec dbo.spSqlBuildAllAuditTables;
-- exec dbo.spSqlDropAllAuditTables;

-- exec dbo.spSqlBuildAllStreamTables;
-- exec dbo.spSqlDropAllStreamTables;


