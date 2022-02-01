if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spGRIDVIEWS_COLUMNS_ReserveIndex' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spGRIDVIEWS_COLUMNS_ReserveIndex;
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
Create Procedure dbo.spGRIDVIEWS_COLUMNS_ReserveIndex
	( @MODIFIED_USER_ID            uniqueidentifier
	, @GRID_NAME                   nvarchar(50)
	, @RESERVE_INDEX               int
	)
as
  begin
	declare @MIN_INDEX int;
	-- BEGIN Oracle Exception
		select @MIN_INDEX = min(COLUMN_INDEX)
		  from GRIDVIEWS_COLUMNS
		 where GRID_NAME         = @GRID_NAME       
		   and DELETED           = 0                
		   and DEFAULT_VIEW      = 0                ;
	-- END Oracle Exception
	while @MIN_INDEX < @RESERVE_INDEX begin -- do
		update GRIDVIEWS_COLUMNS
		   set COLUMN_INDEX      = COLUMN_INDEX + 1 
		     , DATE_MODIFIED     =  getdate()       
		     , DATE_MODIFIED_UTC =  getutcdate()    
		     , MODIFIED_USER_ID  = @MODIFIED_USER_ID
		 where GRID_NAME         = @GRID_NAME       
		   and DELETED           = 0                
		   and DEFAULT_VIEW      = 0                ;
		set @MIN_INDEX = @MIN_INDEX + 1;
	end -- while;

	-- BEGIN Oracle Exception
		select @MIN_INDEX = min(COLUMN_INDEX)
		  from GRIDVIEWS_COLUMNS
		 where GRID_NAME         = @GRID_NAME       
		   and DELETED           = 0                
		   and DEFAULT_VIEW      = 1                ;
	-- END Oracle Exception
	while @MIN_INDEX < @RESERVE_INDEX begin -- do
		update GRIDVIEWS_COLUMNS
		   set COLUMN_INDEX      = COLUMN_INDEX + 1 
		     , DATE_MODIFIED     =  getdate()       
		     , DATE_MODIFIED_UTC =  getutcdate()    
		     , MODIFIED_USER_ID  = @MODIFIED_USER_ID
		 where GRID_NAME         = @GRID_NAME       
		   and DELETED           = 0                
		   and DEFAULT_VIEW      = 1                ;
		set @MIN_INDEX = @MIN_INDEX + 1;
	end -- while;
  end
GO
 
Grant Execute on dbo.spGRIDVIEWS_COLUMNS_ReserveIndex to public;
GO
 
