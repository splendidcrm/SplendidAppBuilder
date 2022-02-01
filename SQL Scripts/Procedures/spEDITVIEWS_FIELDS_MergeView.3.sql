if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spEDITVIEWS_FIELDS_MergeView' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spEDITVIEWS_FIELDS_MergeView;
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
Create Procedure dbo.spEDITVIEWS_FIELDS_MergeView
	( @EDIT_NAME         nvarchar( 50)
	, @MERGE_NAME        nvarchar( 50)
	, @DATA_LABEL1       nvarchar(150)
	, @DATA_LABEL2       nvarchar(150)
	)
as
  begin
	declare @ID          uniqueidentifier;
	declare @FIELD_WIDTH nvarchar(10);
	declare @FIELD_INDEX int;

-- #if SQL_Server /*
	declare merge_cursor cursor for
	select ID
	     , FIELD_WIDTH
	  from vwEDITVIEWS_FIELDS
	 where EDIT_NAME = @MERGE_NAME
	 order by FIELD_INDEX;
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

	select @FIELD_INDEX = isnull(max(FIELD_INDEX), 0)
	  from vwEDITVIEWS_FIELDS
	 where EDIT_NAME = @EDIT_NAME;

	if exists(select * from vwEDITVIEWS_FIELDS where EDIT_NAME = @MERGE_NAME) begin -- then
		set @FIELD_INDEX = @FIELD_INDEX + 1;
		exec dbo.spEDITVIEWS_FIELDS_InsSeparator @EDIT_NAME, @FIELD_INDEX;
		if @DATA_LABEL1 is not null and @DATA_LABEL2 is not null begin -- then
			set @FIELD_INDEX = @FIELD_INDEX + 1;
			exec dbo.spEDITVIEWS_FIELDS_InsHeader    @EDIT_NAME, @FIELD_INDEX, @DATA_LABEL1, null;
			set @FIELD_INDEX = @FIELD_INDEX + 1;
			exec dbo.spEDITVIEWS_FIELDS_InsHeader    @EDIT_NAME, @FIELD_INDEX, @DATA_LABEL2, null;
		end else if @DATA_LABEL1 is not null begin -- then
			set @FIELD_INDEX = @FIELD_INDEX + 1;
			exec dbo.spEDITVIEWS_FIELDS_InsHeader    @EDIT_NAME, @FIELD_INDEX, @DATA_LABEL1, 3;
		end -- if;
	end -- if;

	open merge_cursor;
	fetch next from merge_cursor into @ID, @FIELD_WIDTH;
	while @@FETCH_STATUS = 0 begin -- do
		set @FIELD_INDEX = @FIELD_INDEX + 1;
		-- 09/03/2012 Paul.  We need to make sure to convert 1 column 85% to colspan. 
		update EDITVIEWS_FIELDS
		   set EDIT_NAME         = @EDIT_NAME
		     , FIELD_INDEX       = @FIELD_INDEX
		     , COLSPAN           = (case when @FIELD_WIDTH = '85%' then 3 else COLSPAN end)
		     , DATE_MODIFIED     = getdate()
		     , DATE_MODIFIED_UTC = getutcdate()
		     , MODIFIED_USER_ID  = null
		 where ID                = @ID;
		fetch next from merge_cursor into @ID, @FIELD_WIDTH;
/* -- #if Oracle
		IF merge_cursor%NOTFOUND THEN
			StoO_sqlstatus := 2;
			StoO_fetchstatus := -1;
		ELSE
			StoO_sqlstatus := 0;
			StoO_fetchstatus := 0;
		END IF;
-- #endif Oracle */
	end -- while;
	close merge_cursor;
	deallocate merge_cursor;

	update EDITVIEWS
	   set DELETED           = 1
	     , DATE_MODIFIED     = getdate()
	     , DATE_MODIFIED_UTC = getutcdate()
	     , MODIFIED_USER_ID  = null
	 where NAME              = @MERGE_NAME;
  end
GO

Grant Execute on dbo.spEDITVIEWS_FIELDS_MergeView to public;
GO

