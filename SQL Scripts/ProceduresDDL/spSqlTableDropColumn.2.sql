if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spSqlTableDropColumn' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spSqlTableDropColumn;
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
Create Procedure dbo.spSqlTableDropColumn
	( @TABLE_NAME        varchar(255)
	, @COLUMN_NAME       varchar(255)
	)
as
  begin
	set nocount on

	declare @Command   varchar(2000);
	declare @OldColumn varchar(100);
	declare @NewColumn varchar(100);

	exec dbo.spSqlTableDropColumnConstraint @TABLE_NAME, @COLUMN_NAME;

	set @Command = 'alter table ' + @TABLE_NAME + ' drop column ' + @COLUMN_NAME;
	exec (@Command);

	-- 07/15/2009 Jamie.  When dropping a column, we also need to drop it from the audit table. 
	-- However, since we want to retain the audit, just rename the filed and include the drop date. 
	if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = @TABLE_NAME + '_AUDIT' and COLUMN_NAME = @COLUMN_NAME) begin -- then
-- #if SQL_Server /*
		set @OldColumn = @TABLE_NAME + '_AUDIT' + '.' + @COLUMN_NAME;
		set @NewColumn = upper(@COLUMN_NAME) + '_' + convert(varchar(8), getdate(), 112) + '_' + replace(convert(varchar(8), getdate(), 108), ':', '');
		exec sp_rename @OldColumn, @NewColumn, 'COLUMN';
-- #endif SQL_Server */
	end -- if;
  end
GO

Grant Execute on dbo.spSqlTableDropColumn to public;
GO


