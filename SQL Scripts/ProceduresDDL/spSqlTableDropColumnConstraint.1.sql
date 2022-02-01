if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spSqlTableDropColumnConstraint' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spSqlTableDropColumnConstraint;
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
-- 09/15/2009 Paul.  Use alias to get working on Azure. 
-- Deprecated feature 'More than two-part column name' is not supported in this version of SQL Server.
-- 09/12/2010 Paul.  Add Oracle code to lookup the constraint name. 
Create Procedure dbo.spSqlTableDropColumnConstraint
	( @TABLE_NAME        varchar(255)
	, @COLUMN_NAME       varchar(255)
	)
as
  begin
	set nocount on
	
	declare @Command varchar(2000);
-- #if SQL_Server /*
	select @Command = 'alter table ' + objects.name + ' drop constraint ' + default_constraints.name + ';'
	  from      sys.default_constraints  default_constraints
	 inner join sys.objects              objects
	         on objects.object_id      = default_constraints.parent_object_id
	 inner join sys.columns              columns
	         on columns.object_id      = objects.object_id
	        and columns.column_id      = default_constraints.parent_column_id
	 where objects.type = 'U'
	   and default_constraints.type = 'D'
	   and objects.name = @TABLE_NAME
	   and columns.name = @COLUMN_NAME;
-- #endif SQL_Server */



	if @Command is not null begin -- then
		exec (@Command);
	end -- if;
  end
GO

Grant Execute on dbo.spSqlTableDropColumnConstraint to public;
GO

