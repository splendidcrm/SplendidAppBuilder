if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spDASHLETS_USERS_Reorder' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spDASHLETS_USERS_Reorder;
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
Create Procedure dbo.spDASHLETS_USERS_Reorder
	( @MODIFIED_USER_ID uniqueidentifier
	, @ASSIGNED_USER_ID uniqueidentifier
	, @DETAIL_NAME      nvarchar(50)
	)
as
  begin
	set nocount on
	
	declare @ID                 uniqueidentifier;
	declare @DASHLET_ENABLED    bit;
	declare @DASHLET_ORDER_OLD  int;
	declare @DASHLET_ORDER_NEW  int;

	declare module_cursor cursor for
	select ID
	     , DASHLET_ENABLED
	     , DASHLET_ORDER
	  from vwDASHLETS_USERS
	 where ASSIGNED_USER_ID = @ASSIGNED_USER_ID
	   and DETAIL_NAME      = @DETAIL_NAME
	 order by DASHLET_ORDER;

/* -- #if IBM_DB2
	declare continue handler for not found
		set in_FETCH_STATUS = 1;
-- #endif IBM_DB2 */
/* -- #if MySQL
	declare continue handler for not found
		set in_FETCH_STATUS = 1;
	set in_FETCH_STATUS = 0;
-- #endif MySQL */

	set @DASHLET_ORDER_NEW = 0;
	open module_cursor;
	fetch next from module_cursor into @ID, @DASHLET_ENABLED, @DASHLET_ORDER_OLD;
	while @@FETCH_STATUS = 0 begin -- do
		if @DASHLET_ENABLED = 1 begin -- then
			if @DASHLET_ORDER_OLD != @DASHLET_ORDER_NEW begin -- then
				update DASHLETS_USERS
				   set DASHLET_ORDER    = @DASHLET_ORDER_NEW
				     , MODIFIED_USER_ID = null
				     , DATE_MODIFIED    = getdate()
				     , DATE_MODIFIED_UTC= getutcdate()
				 where ID               = @ID;
			end -- if;
			set @DASHLET_ORDER_NEW = @DASHLET_ORDER_NEW + 1;
		end else begin
			if @DASHLET_ORDER_OLD != 0 begin -- then
				update DASHLETS_USERS
				   set DASHLET_ORDER    = 0
				     , MODIFIED_USER_ID = null
				     , DATE_MODIFIED    = getdate()
				     , DATE_MODIFIED_UTC= getutcdate()
				 where ID               = @ID;
			end -- if;
		end -- if;
		fetch next from module_cursor into @ID, @DASHLET_ENABLED, @DASHLET_ORDER_OLD;
	end -- while;
	close module_cursor;

	deallocate module_cursor;
  end
GO

Grant Execute on dbo.spDASHLETS_USERS_Reorder to public;
GO

