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
if not exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spPARENT_Delete' and ROUTINE_TYPE = 'PROCEDURE') begin -- then
	exec('Create Procedure dbo.spPARENT_Delete
		( @ID               uniqueidentifier
		, @MODIFIED_USER_ID uniqueidentifier
		)
	as
	  begin
		set nocount on
		
	  end');
end -- if;
GO



if not exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spPARENT_Undelete' and ROUTINE_TYPE = 'PROCEDURE') begin -- then
	exec('Create Procedure dbo.spPARENT_Undelete
		( @ID               uniqueidentifier
		, @MODIFIED_USER_ID uniqueidentifier
		, @AUDIT_TOKEN      varchar(255)
		, @PARENT_TYPE      nvarchar(25)
		)
	as
	  begin
		set nocount on
		
	  end');
end -- if;
GO


if not exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spSqlBuildStreamFunction' and ROUTINE_TYPE = 'PROCEDURE') begin -- then
	exec('Create Procedure dbo.spSqlBuildStreamFunction(@TABLE_NAME varchar(80))
	as
	  begin
		set nocount on
		
	  end');
end -- if;
GO


if not exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spSqlBuildStreamView' and ROUTINE_TYPE = 'PROCEDURE') begin -- then
	exec('Create Procedure dbo.spSqlBuildStreamView(@TABLE_NAME varchar(80))
	as
	  begin
		set nocount on
		
	  end');
end -- if;
GO


if not exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spSqlBuildArchiveTable' and ROUTINE_TYPE = 'PROCEDURE') begin -- then
	exec('Create Procedure dbo.spSqlBuildArchiveTable
		( @TABLE_NAME       nvarchar(80)
		, @ARCHIVE_DATABASE nvarchar(50)
		)
	as
	  begin
		set nocount on
	
	  end');
end -- if;
GO


if not exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spSqlBuildArchiveView' and ROUTINE_TYPE = 'PROCEDURE') begin -- then
	exec('Create Procedure dbo.spSqlBuildArchiveView
		( @TABLE_NAME       nvarchar(80)
		, @ARCHIVE_DATABASE nvarchar(50)
		)
	as
	  begin
		set nocount on
	
	  end');
end -- if;
GO


