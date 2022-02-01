if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spACL_ACTIONS_Initialize' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spACL_ACTIONS_Initialize;
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
-- 05/02/2005 Paul.  DB2 requires that we use nvarchar to prevent any conversions.
-- 09/26/2017 Paul.  Add Archive access right. 
Create Procedure dbo.spACL_ACTIONS_Initialize
as
  begin
	set nocount on
	
	declare @MODULE_NAME nvarchar(100);

	declare module_cursor cursor for
	select distinct MODULE_NAME
	  from vwACL_ACCESS_ByModule
	  left outer join ACL_ACTIONS
	               on ACL_ACTIONS.CATEGORY = vwACL_ACCESS_ByModule.MODULE_NAME
	 where ACL_ACTIONS.ID is null
	 order by MODULE_NAME;

	declare archive_cursor cursor for
	select MODULE_NAME
	  from vwACL_ACCESS_ByModule
	  left outer join ACL_ACTIONS
	               on ACL_ACTIONS.CATEGORY = vwACL_ACCESS_ByModule.MODULE_NAME
	              and ACL_ACTIONS.NAME     = N'archive'
	 where ACL_ACTIONS.ID is null
	 order by MODULE_NAME;

/* -- #if IBM_DB2
	declare continue handler for not found
		set in_FETCH_STATUS = 1;
-- #endif IBM_DB2 */
/* -- #if MySQL
	declare continue handler for not found
		set in_FETCH_STATUS = 1;
	set in_FETCH_STATUS = 0;
-- #endif MySQL */

	open module_cursor;
	fetch next from module_cursor into @MODULE_NAME;
	while @@FETCH_STATUS = 0 begin -- do
		exec dbo.spACL_ACTIONS_InsertOnly N'admin' , @MODULE_NAME, N'module',  1;
		exec dbo.spACL_ACTIONS_InsertOnly N'access', @MODULE_NAME, N'module', 89;
		exec dbo.spACL_ACTIONS_InsertOnly N'view'  , @MODULE_NAME, N'module', 90;
		exec dbo.spACL_ACTIONS_InsertOnly N'list'  , @MODULE_NAME, N'module', 90;
		exec dbo.spACL_ACTIONS_InsertOnly N'edit'  , @MODULE_NAME, N'module', 90;
		exec dbo.spACL_ACTIONS_InsertOnly N'delete', @MODULE_NAME, N'module', 90;
		exec dbo.spACL_ACTIONS_InsertOnly N'import', @MODULE_NAME, N'module', 90;
		exec dbo.spACL_ACTIONS_InsertOnly N'export', @MODULE_NAME, N'module', 90;
		-- 09/26/2017 Paul.  Add Archive access right. 
		exec dbo.spACL_ACTIONS_InsertOnly N'archive', @MODULE_NAME, N'module', 90;
		fetch next from module_cursor into @MODULE_NAME;
	end -- while;
	close module_cursor;
	deallocate module_cursor;

	-- 09/26/2017 Paul.  Add Archive access right. 
	open archive_cursor;
	fetch next from archive_cursor into @MODULE_NAME;
	while @@FETCH_STATUS = 0 begin -- do
		exec dbo.spACL_ACTIONS_InsertOnly N'archive', @MODULE_NAME, N'module', 90;
		fetch next from archive_cursor into @MODULE_NAME;
	end -- while;
	close archive_cursor;
	deallocate archive_cursor;
  end
GO
 
Grant Execute on dbo.spACL_ACTIONS_Initialize to public;
GO
 
