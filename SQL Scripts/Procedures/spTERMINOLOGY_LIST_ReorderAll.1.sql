if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spTERMINOLOGY_LIST_ReorderAll' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spTERMINOLOGY_LIST_ReorderAll;
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
Create Procedure dbo.spTERMINOLOGY_LIST_ReorderAll
	( @MODIFIED_USER_ID  uniqueidentifier
	)
as
  begin
	set nocount on
	
	declare @ID             uniqueidentifier;
	declare @LANG           nvarchar(10);
	declare @LIST_NAME      nvarchar(50);

-- #if SQL_Server /*
	declare list_cursor cursor for
	select vwTERMINOLOGY.LANG
	     , vwTERMINOLOGY.LIST_NAME
	  from      vwTERMINOLOGY
	 inner join vwLANGUAGES
	         on vwLANGUAGES.NAME   = vwTERMINOLOGY.LANG
	        and vwLANGUAGES.ACTIVE = 1
	 where vwTERMINOLOGY.LIST_NAME is not null
	 group by vwTERMINOLOGY.LANG, vwTERMINOLOGY.LIST_NAME
	 order by vwTERMINOLOGY.LANG, vwTERMINOLOGY.LIST_NAME;
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

	open list_cursor;
	fetch next from list_cursor into @LANG, @LIST_NAME;
	while @@FETCH_STATUS = 0 begin -- do
		--print @LANG + N'.' + @LIST_NAME;
		exec dbo.spTERMINOLOGY_LIST_Reorder @MODIFIED_USER_ID, @LANG, @LIST_NAME;
		fetch next from list_cursor into @LANG, @LIST_NAME;
/* -- #if Oracle
		IF list_cursor%NOTFOUND THEN
			StoO_sqlstatus := 2;
			StoO_fetchstatus := -1;
		ELSE
			StoO_sqlstatus := 0;
			StoO_fetchstatus := 0;
		END IF;
-- #endif Oracle */
	end -- while;
	close list_cursor;

	deallocate list_cursor;
  end
GO
 
-- exec dbo.spTERMINOLOGY_LIST_ReorderAll null;

Grant Execute on dbo.spTERMINOLOGY_LIST_ReorderAll to public;
GO
 
