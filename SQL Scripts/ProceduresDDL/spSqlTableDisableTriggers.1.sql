if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spSqlTableDisableTriggers' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spSqlTableDisableTriggers;
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
Create Procedure dbo.spSqlTableDisableTriggers
	( @TABLE_NAME        varchar(255)
	)
as
  begin
	set nocount on

	declare @COMMAND       varchar(2000);
	declare @COLUMN_NAME   varchar(255);

	-- 04/27/2014 Paul.  Use simplified call to manage triggers. Both are valid. 
	set @COMMAND = 'alter table ' + upper(@TABLE_NAME) + ' disable trigger all';
	--set @COMMAND = 'disable trigger all on ' + upper(@TABLE_NAME);
	exec(@COMMAND);
	--declare TRIGGER_CURSOR cursor for
	--select TRIGGERS.name
	--  from      sys.objects        TRIGGERS
	-- inner join sys.objects        TABLES
	--         on TABLES.object_id = TRIGGERS.parent_object_id
	-- where TRIGGERS.type = 'TR'
	--   and TABLES.name = @TABLE_NAME;
	--open TRIGGER_CURSOR;
	--fetch next from TRIGGER_CURSOR into @COLUMN_NAME;
	--while @@FETCH_STATUS = 0 begin -- do
	--	set @COMMAND = 'alter table ' + upper(@TABLE_NAME) + ' disable trigger ' +  @COLUMN_NAME + ';';
	--	exec(@COMMAND);
	--	fetch next from TRIGGER_CURSOR into @COLUMN_NAME;
	--end -- while;
	--close TRIGGER_CURSOR;
	--deallocate TRIGGER_CURSOR;
  end
GO

Grant Execute on dbo.spSqlTableDisableTriggers to public;
GO


