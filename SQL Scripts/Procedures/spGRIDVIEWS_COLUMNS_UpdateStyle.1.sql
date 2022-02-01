if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spGRIDVIEWS_COLUMNS_UpdateStyle' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spGRIDVIEWS_COLUMNS_UpdateStyle;
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
Create Procedure dbo.spGRIDVIEWS_COLUMNS_UpdateStyle
	( @MODIFIED_USER_ID            uniqueidentifier
	, @GRID_NAME                   nvarchar(50)
	, @COLUMN_INDEX                int
	, @ITEMSTYLE_WIDTH             nvarchar(10)
	, @ITEMSTYLE_CSSCLASS          nvarchar(50)
	, @ITEMSTYLE_HORIZONTAL_ALIGN  nvarchar(10)
	, @ITEMSTYLE_VERTICAL_ALIGN    nvarchar(10)
	, @ITEMSTYLE_WRAP              bit
	)
as
  begin
	update GRIDVIEWS_COLUMNS
	   set MODIFIED_USER_ID            = @MODIFIED_USER_ID 
	     , DATE_MODIFIED               =  getdate()        
	     , DATE_MODIFIED_UTC           =  getutcdate()     
	     , ITEMSTYLE_WIDTH             = isnull(@ITEMSTYLE_WIDTH           , ITEMSTYLE_WIDTH           )
	     , ITEMSTYLE_CSSCLASS          = isnull(@ITEMSTYLE_CSSCLASS        , ITEMSTYLE_CSSCLASS        )
	     , ITEMSTYLE_HORIZONTAL_ALIGN  = isnull(@ITEMSTYLE_HORIZONTAL_ALIGN, ITEMSTYLE_HORIZONTAL_ALIGN)
	     , ITEMSTYLE_VERTICAL_ALIGN    = isnull(@ITEMSTYLE_VERTICAL_ALIGN  , ITEMSTYLE_VERTICAL_ALIGN  )
	     , ITEMSTYLE_WRAP              = isnull(@ITEMSTYLE_WRAP            , ITEMSTYLE_WRAP            )
	 where GRID_NAME                   = @GRID_NAME
	   and COLUMN_INDEX                = @COLUMN_INDEX
	   and DELETED                     = 0            
	   and DEFAULT_VIEW                = 0            ;
  end
GO
 
Grant Execute on dbo.spGRIDVIEWS_COLUMNS_UpdateStyle to public;
GO
 
