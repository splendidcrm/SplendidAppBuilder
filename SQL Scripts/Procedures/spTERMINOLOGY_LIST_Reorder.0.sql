if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spTERMINOLOGY_LIST_Reorder' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spTERMINOLOGY_LIST_Reorder;
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
-- 08/24/2008 Paul.  The extension of this procedure is zero so that we do not have to rename any other procedures. 
-- The intent is to call this procedure any time the list order changes to ensure that there are not gaps or overlaps. 
Create Procedure dbo.spTERMINOLOGY_LIST_Reorder
	( @MODIFIED_USER_ID  uniqueidentifier
	, @LANG              nvarchar(10)
	, @LIST_NAME         nvarchar(50)
	)
as
  begin
	set nocount on
	
	declare @ID             uniqueidentifier;
	-- 08/24/2010 Paul.  Name can be 50 chars. 
	-- 03/06/2012 Paul.  Increase size of the NAME field so that it can include a date formula. 
	declare @NAME           nvarchar(150);
	declare @LIST_ORDER_OLD int;
	declare @LIST_ORDER_NEW int;

-- #if SQL_Server /*
	declare terminology_cursor cursor for
	select ID
	     , NAME
	     , LIST_ORDER
	  from vwTERMINOLOGY
	 where LANG      = @LANG
	   and LIST_NAME = @LIST_NAME
	 order by MODULE_NAME, LIST_ORDER, NAME;
-- #endif SQL_Server */

/* -- #if IBM_DB2
	declare continue handler for not found
		set in_FETCH_STATUS = 1;
-- #endif IBM_DB2 */
/* -- #if MySQL
	declare continue handler for not found
		set in_FETCH_STATUS = 1;
	set in_FETCH_STATUS = 0;
-- #endif MySQL */

	set @LIST_ORDER_NEW = 1;
	open terminology_cursor;
	fetch next from terminology_cursor into @ID, @NAME, @LIST_ORDER_OLD;
	while @@FETCH_STATUS = 0 begin -- do
		if @LIST_ORDER_OLD != @LIST_ORDER_NEW begin -- then
			print N'Correcting list order of ' + @LANG + N'.' + @LIST_NAME + N'.' + @NAME + N' (' + cast(@LIST_ORDER_NEW as varchar(10)) + N')';
			update TERMINOLOGY
			   set LIST_ORDER       = @LIST_ORDER_NEW
			     , MODIFIED_USER_ID = null
			     , DATE_MODIFIED    = getdate()
			     , DATE_MODIFIED_UTC= getutcdate()
			 where ID               = @ID;
		end -- if;
		set @LIST_ORDER_NEW = @LIST_ORDER_NEW + 1;
		fetch next from terminology_cursor into @ID, @NAME, @LIST_ORDER_OLD;
/* -- #if Oracle
		IF terminology_cursor%NOTFOUND THEN
			StoO_sqlstatus := 2;
			StoO_fetchstatus := -1;
		ELSE
			StoO_sqlstatus := 0;
			StoO_fetchstatus := 0;
		END IF;
-- #endif Oracle */
	end -- while;
	close terminology_cursor;

	deallocate terminology_cursor;
  end
GO
 
-- exec dbo.spTERMINOLOGY_LIST_Reorder null, 'en-US', 'moduleList';
/*
select ID
     , NAME
     , LIST_ORDER
  from vwTERMINOLOGY
 where LANG      = 'en-US'
   and LIST_NAME = 'moduleList'
 order by MODULE_NAME, LIST_ORDER, NAME;
*/

Grant Execute on dbo.spTERMINOLOGY_LIST_Reorder to public;
GO
 
