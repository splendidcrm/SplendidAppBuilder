if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spSqlBuildAllAuditViews' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spSqlBuildAllAuditViews;
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
-- 11/17/2008 Paul.  Audit views are used to simplify support of custom fields in workflow engine. 
-- 06/02/2009 Paul.  This procedure must be run after the data for the MODULES table has been loaded. 
-- 10/11/2015 Paul.  Don't use vwMODULES as disabled tables can cause errors with the stream generation. 
Create Procedure dbo.spSqlBuildAllAuditViews
as
  begin
	set nocount on
	print N'spSqlBuildAllAuditViews';

	declare @TABLE_NAME varchar(80);
	-- 11/17/2008 Paul.  Only use module-based tables and exclude the custom fields tables. 
	declare TABLES_CURSOR cursor for
	select vwSqlTablesAudited.TABLE_NAME
	  from      vwSqlTablesAudited
	 inner join vwSqlTables
	         on vwSqlTables.TABLE_NAME = vwSqlTablesAudited.TABLE_NAME + '_AUDIT'
	 inner join MODULES
	         on MODULES.TABLE_NAME   = vwSqlTablesAudited.TABLE_NAME
	        and MODULES.DELETED      = 0
	 where vwSqlTablesAudited.TABLE_NAME not like '%[_]CSTM'
	order by vwSqlTablesAudited.TABLE_NAME;
	
	open TABLES_CURSOR;
	fetch next from TABLES_CURSOR into @TABLE_NAME;
	while @@FETCH_STATUS = 0 begin -- do
		exec dbo.spSqlBuildAuditView @TABLE_NAME;
		fetch next from TABLES_CURSOR into @TABLE_NAME;
	end -- while;
	close TABLES_CURSOR;
	deallocate TABLES_CURSOR;
  end
GO


Grant Execute on dbo.spSqlBuildAllAuditViews to public;
GO

-- exec dbo.spSqlBuildAllAuditTables;
-- exec dbo.spSqlBuildAllAuditViews;
-- exec dbo.spSqlDropAllAuditTriggers;
-- exec dbo.spSqlDropAllAuditTables;

