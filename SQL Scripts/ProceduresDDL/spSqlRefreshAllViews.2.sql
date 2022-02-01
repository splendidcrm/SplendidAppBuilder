if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spSqlRefreshAllViews' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spSqlRefreshAllViews;
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
-- 12/16/2006 Paul.  Provide a way to disable the refresh. 
-- 08/02/2007 Paul.  We need to catch view errors as they can abort the entire procedure mid-way.
-- This error is hard to trap, so use the FMTONLY trick. 
-- http://www.developmentnow.com/g/95_2006_3_0_0_728267/Im-sure-this-is-an-easy-one--Error-trap-to-skip-over-a-bad-object-.htm
-- exec ('set fmtonly on select * from ' + @ViewName + ' where 1 = 0');
-- 08/02/2007 Paul.  Skipping invalid views is not a solution.  The solution appears to be to order the views by table name length. 
-- By compiling short views first, dependent views should work because their length is typically longer. 
-- 01/07/2008 Paul.  Filter schema by TABLE_SCHEMA to prevent compiler errors. 
-- 07/14/2010 Paul.  Some customers are encountering issues with our refresh.  So only refresh views that start with vw. 
-- 11/02/2017 Paul.  Use try/catch to allow recompile to continue after error. 
Create Procedure dbo.spSqlRefreshAllViews
as
  begin
	set nocount on

	declare @Pass     int;
	declare @PassMax  int;
	declare @ViewName varchar(90);
	declare views_cursor cursor for
	select TABLE_NAME
	  from INFORMATION_SCHEMA.VIEWS
	 where TABLE_NAME not in ('sysconstraints', 'syssegments')
	   and TABLE_SCHEMA = 'dbo'
	   and TABLE_NAME like 'vw%'
	 order by len(TABLE_NAME);
/* -- #if IBM_DB2
	declare continue handler for not found
		set @FETCH_STATUS = 1;
-- #endif IBM_DB2 */
	set @PassMax = dbo.fnCONFIG_Int('refreshallviews_maxpass');
	if @PassMax is null begin -- then
		set @PassMax = 9;
	end -- if;
-- #if SQL_Server /*
	if @PassMax = 0 begin -- then
		print 'spSqlRefreshAllViews has been disabled';
	end -- if;
-- #endif SQL_Server */

	-- 01/02/2005 Paul.  There is no way to get a list of view dependencies, 
	-- so just repeat the refresh as many times as the views are deep.
	set @Pass = 1;
	while @Pass <= @PassMax begin -- do
		print 'Pass ' + cast(@Pass as varchar(10));
		open views_cursor;
		fetch next from views_cursor into @ViewName;
		while @@FETCH_STATUS = 0 begin -- do
			--print 'sp_refreshview ' + @ViewName;
			-- 11/02/2017 Paul.  Use try/catch to allow recompile to continue after error. 
			begin try
				exec sp_refreshview @ViewName;
			end try
			begin catch
				print 'Failed to recompile view ' + @ViewName;
				print ERROR_MESSAGE();
			end catch
			fetch next from views_cursor into @ViewName;
		end -- while;
		close views_cursor;
		set @Pass = @Pass + 1;
	end -- while;
	deallocate views_cursor;
  end
GO


Grant Execute on dbo.spSqlRefreshAllViews to public;
GO


