if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spGRIDVIEWS_COLUMNS_InsTagSelect' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spGRIDVIEWS_COLUMNS_InsTagSelect;
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
Create Procedure dbo.spGRIDVIEWS_COLUMNS_InsTagSelect
	( @GRID_NAME                   nvarchar( 50)
	, @COLUMN_INDEX                int
	, @ITEMSTYLE_WIDTH             nvarchar( 10)
	)
as
  begin
	declare @ID                uniqueidentifier;
	declare @HEADER_TEXT       nvarchar(150);
	declare @DATA_FIELD        nvarchar( 50);
	declare @SORT_EXPRESSION   nvarchar( 50);
	
	set @HEADER_TEXT     = N'.LBL_LIST_TAG_SET_NAME';
	set @DATA_FIELD      = N'TAG_SET_NAME';
	set @SORT_EXPRESSION = N'TAG_SET_NAME';

	-- 08/20/2016 Paul.  Insert only means that the grid and index is unique. 
	-- BEGIN Oracle Exception
		select @ID = ID
		  from GRIDVIEWS_COLUMNS
		 where GRID_NAME    = @GRID_NAME
		   and COLUMN_INDEX = @COLUMN_INDEX
		   and DELETED      = 0            
		   and DEFAULT_VIEW = 0            ;
	-- END Oracle Exception
	if dbo.fnIsEmptyGuid(@ID) = 1 begin -- then
		exec dbo.spGRIDVIEWS_COLUMNS_Update
			  @ID out
			, null               -- MODIFIED_USER_ID
			, @GRID_NAME         -- GRID_NAME
			, @COLUMN_INDEX      -- COLUMN_INDEX
			, N'TemplateColumn'  -- COLUMN_TYPE
			, @HEADER_TEXT       -- HEADER_TEXT
			, @SORT_EXPRESSION   -- SORT_EXPRESSION
			, @ITEMSTYLE_WIDTH   -- ITEMSTYLE_WIDTH
			, null               -- ITEMSTYLE_CSSCLASS
			, null               -- ITEMSTYLE_HORIZONTAL_ALIGN
			, null               -- ITEMSTYLE_VERTICAL_ALIGN
			, null               -- ITEMSTYLE_WRAP    
			, @DATA_FIELD        -- DATA_FIELD        
			, N'Tags'            -- DATA_FORMAT       
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

Grant Execute on dbo.spGRIDVIEWS_COLUMNS_InsTagSelect to public;
GO

