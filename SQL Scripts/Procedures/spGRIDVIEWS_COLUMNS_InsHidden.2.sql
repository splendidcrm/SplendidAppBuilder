if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spGRIDVIEWS_COLUMNS_InsHidden' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spGRIDVIEWS_COLUMNS_InsHidden;
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
Create Procedure dbo.spGRIDVIEWS_COLUMNS_InsHidden
	( @GRID_NAME                   nvarchar( 50)
	, @DATA_FIELD                  nvarchar( 50)
	)
as
  begin
	declare @ID                uniqueidentifier;
	declare @COLUMN_INDEX      int;
	
	-- 08/20/2016 Paul.  We only need one record for the hidden field, so the index is not important. 
	-- BEGIN Oracle Exception
		select @ID = ID
		  from GRIDVIEWS_COLUMNS
		 where GRID_NAME    = @GRID_NAME
		   and DATA_FIELD   = @DATA_FIELD
		   and DELETED      = 0            
		   and DEFAULT_VIEW = 0            ;
	-- END Oracle Exception
	if dbo.fnIsEmptyGuid(@ID) = 1 begin -- then
		-- BEGIN Oracle Exception
			select @COLUMN_INDEX = isnull(max(COLUMN_INDEX), 0) + 1
			  from GRIDVIEWS_COLUMNS
			 where GRID_NAME    = @GRID_NAME
			   and DELETED      = 0            
			   and DEFAULT_VIEW = 0            ;
		-- END Oracle Exception

		exec dbo.spGRIDVIEWS_COLUMNS_Update
			  @ID out
			, null               -- MODIFIED_USER_ID
			, @GRID_NAME         -- GRID_NAME
			, @COLUMN_INDEX      -- COLUMN_INDEX
			, N'TemplateColumn'  -- COLUMN_TYPE
			, null               -- HEADER_TEXT
			, null               -- SORT_EXPRESSION
			, null               -- ITEMSTYLE_WIDTH
			, null               -- ITEMSTYLE_CSSCLASS
			, null               -- ITEMSTYLE_HORIZONTAL_ALIGN
			, null               -- ITEMSTYLE_VERTICAL_ALIGN
			, null               -- ITEMSTYLE_WRAP    
			, @DATA_FIELD        -- DATA_FIELD        
			, N'Hidden'          -- DATA_FORMAT       
			, null               -- URL_FIELD         
			, null               -- URL_FORMAT        
			, null               -- URL_TARGET        
			, null               -- LIST_NAME         
			, null               -- URL_MODULE        
			, null               -- URL_ASSIGNED_FIELD
			, null               -- MODULE_TYPE       
			, null               -- PARENT_FIELD      
			;
	end -- if;
  end
GO

Grant Execute on dbo.spGRIDVIEWS_COLUMNS_InsHidden to public;
GO

