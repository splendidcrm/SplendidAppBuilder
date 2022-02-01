if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spSqlDropDefaultConstraint' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spSqlDropDefaultConstraint;
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
Create Procedure dbo.spSqlDropDefaultConstraint(@TABLE_NAME nvarchar(50) out, @COLUMN_NAME nvarchar(50))
as
  begin
	set nocount on

	declare @COMMAND varchar(1000);
	select @COMMAND = 'alter table ' + sys.tables.name + ' drop constraint ' + sys.default_constraints.name
	  from      sys.all_columns
	 inner join sys.tables
	         on sys.tables.object_id              = sys.all_columns.object_id
	 inner join sys.default_constraints
	         on sys.default_constraints.object_id = sys.all_columns.default_object_id
	 where sys.tables.name      = @TABLE_NAME
	   and sys.all_columns.name = @COLUMN_NAME;

	if @COMMAND is not null begin -- then
		print @COMMAND;
		exec(@COMMAND);
	end -- if;
  end
GO


Grant Execute on dbo.spSqlDropDefaultConstraint to public;
GO

