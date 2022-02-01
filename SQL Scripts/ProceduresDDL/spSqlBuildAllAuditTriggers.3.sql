if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spSqlBuildAllAuditTriggers' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spSqlBuildAllAuditTriggers;
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
-- 08/20/2008 Paul.  Build the triggers for the cached tables. 
Create Procedure dbo.spSqlBuildAllAuditTriggers
as
  begin
	set nocount on
	print N'spSqlBuildAllAuditTriggers';

	declare @TABLE_NAME varchar(80);
	declare TABLES_CURSOR cursor for
	select vwSqlTablesAudited.TABLE_NAME
	  from      vwSqlTablesAudited
	 inner join vwSqlTables
	         on vwSqlTables.TABLE_NAME = vwSqlTablesAudited.TABLE_NAME + '_AUDIT'
	order by vwSqlTablesAudited.TABLE_NAME;
	
	declare CACHE_CURSOR cursor for
	select TABLE_NAME
	  from vwSqlTablesCachedSystem
	union
	select TABLE_NAME
	  from vwSqlTablesCachedData
	 order by TABLE_NAME;
	
	open TABLES_CURSOR;
	fetch next from TABLES_CURSOR into @TABLE_NAME;
	while @@FETCH_STATUS = 0 begin -- do
		exec dbo.spSqlBuildAuditTrigger @TABLE_NAME;
		fetch next from TABLES_CURSOR into @TABLE_NAME;
	end -- while;
	close TABLES_CURSOR;
	deallocate TABLES_CURSOR;

	open CACHE_CURSOR;
	fetch next from CACHE_CURSOR into @TABLE_NAME;
	while @@FETCH_STATUS = 0 begin -- do
		exec dbo.spSqlBuildSystemTrigger @TABLE_NAME;
		fetch next from CACHE_CURSOR into @TABLE_NAME;
	end -- while;
	close CACHE_CURSOR;
	deallocate CACHE_CURSOR;
  end
GO


Grant Execute on dbo.spSqlBuildAllAuditTriggers to public;
GO

-- exec dbo.spSqlBuildAllAuditTables;
-- exec dbo.spSqlBuildAllAuditTriggers;
-- exec dbo.spSqlDropAllAuditTriggers;
-- exec dbo.spSqlDropAllAuditTables;

