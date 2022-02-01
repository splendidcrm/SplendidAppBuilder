if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spSqlUpdateIndex' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spSqlUpdateIndex;
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
-- 09/15/2009 Paul.  Azure does not support sysindexes view. 
-- 08/17/2010 Paul.  If the index is not found, then the value will be NULL. 
-- 04/25/2011 Paul.  We've stopped supporting SQL 2000, so we can use varchar(max). 
-- This also fixes a problem for a customer with 100 custom fields. 
Create Procedure dbo.spSqlUpdateIndex
	( @INDEX_NAME  varchar(80)
	, @TABLE_NAME  varchar(80)
	, @FIELD1      varchar(40)
	, @FIELD2      varchar(40) = null
	, @FIELD3      varchar(40) = null
	, @FIELD4      varchar(40) = null
	, @FIELD5      varchar(40) = null
	, @FIELD6      varchar(40) = null
	)
as
  begin
	set nocount on

	-- 04/25/2011 Paul.  We've stopped supporting SQL 2000, so we can use varchar(max). 
	declare @Command           varchar(max);
	declare @CRLF              char(2);
	declare @TEST              bit;
	declare @REBUILD_INDEX     bit;
	declare @INDEX_ID          int;
	declare @EXISTING_FIELD1   varchar(40);
	declare @EXISTING_FIELD2   varchar(40);
	declare @EXISTING_FIELD3   varchar(40);
	declare @EXISTING_FIELD4   varchar(40);
	declare @EXISTING_FIELD5   varchar(40);
	declare @EXISTING_FIELD6   varchar(40);
	
	set @TEST = 0
	set @CRLF = char(13) + char(10);
	select @INDEX_ID = index_id
	  from sys.indexes
	 where name = @INDEX_NAME;

	if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = @TABLE_NAME) begin -- then
		if @INDEX_ID is not null begin -- then
			set @EXISTING_FIELD1 = index_col(@TABLE_NAME, @INDEX_ID, 1);
			set @EXISTING_FIELD2 = index_col(@TABLE_NAME, @INDEX_ID, 2);
			set @EXISTING_FIELD3 = index_col(@TABLE_NAME, @INDEX_ID, 3);
			set @EXISTING_FIELD4 = index_col(@TABLE_NAME, @INDEX_ID, 4);
			set @EXISTING_FIELD5 = index_col(@TABLE_NAME, @INDEX_ID, 5);
			set @EXISTING_FIELD6 = index_col(@TABLE_NAME, @INDEX_ID, 6);
	
			set @REBUILD_INDEX = 0;
			if          isnull(@FIELD1, '') <> isnull(@EXISTING_FIELD1, '') begin -- then
				set @REBUILD_INDEX = 1;
			end else if isnull(@FIELD2, '') <> isnull(@EXISTING_FIELD2, '') begin -- then
				set @REBUILD_INDEX = 1;
			end else if isnull(@FIELD3, '') <> isnull(@EXISTING_FIELD3, '') begin -- then
				set @REBUILD_INDEX = 1;
			end else if isnull(@FIELD4, '') <> isnull(@EXISTING_FIELD4, '') begin -- then
				set @REBUILD_INDEX = 1;
			end else if isnull(@FIELD5, '') <> isnull(@EXISTING_FIELD5, '') begin -- then
				set @REBUILD_INDEX = 1;
			end else if isnull(@FIELD6, '') <> isnull(@EXISTING_FIELD6, '') begin -- then
				set @REBUILD_INDEX = 1;
			end -- if;
	
			if @REBUILD_INDEX = 1 begin -- then
				-- 09/15/2009 Paul.  Use new syntax to drop an index. 
				-- Deprecated feature 'DROP INDEX with two-part name' is not supported in this version of SQL Server.
				set @Command = 'drop   index ' + @INDEX_NAME + ' on ' + @TABLE_NAME;
				if @TEST = 1 begin -- then
					print @Command + ';';
				end else begin
					print @Command + ';';
					exec(@Command);
				end -- if;
				-- 08/17/2010 Paul.  Use NULL instead of zero as the indicator. 
				set @INDEX_ID = null;
			end -- if;
		end -- if;
	
		-- 08/17/2010 Paul.  If the index is not found, then the value will be NULL. 
		if @INDEX_ID is null begin -- then
			set @Command = 'create index ' + @INDEX_NAME + ' on dbo.' + @TABLE_NAME + ' (' + @FIELD1;
			if @FIELD2 is not null begin -- then
				set @Command = @Command + ', ' + @FIELD2;
			end -- if;
			if @FIELD3 is not null begin -- then
				set @Command = @Command + ', ' + @FIELD3;
			end -- if;
			if @FIELD4 is not null begin -- then
				set @Command = @Command + ', ' + @FIELD4;
			end -- if;
			if @FIELD5 is not null begin -- then
				set @Command = @Command + ', ' + @FIELD5;
			end -- if;
			if @FIELD6 is not null begin -- then
				set @Command = @Command + ', ' + @FIELD6;
			end -- if;
	
			set @Command = @Command + ')';
			if @TEST = 1 begin -- then
				print @Command + ';';
			end else begin
				print @Command + ';';
				exec(@Command);
			end -- if;
		end -- if;
	end -- if;
  end
GO


Grant Execute on dbo.spSqlUpdateIndex to public;
GO

