if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spDYNAMIC_BUTTONS_InsEdit' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spDYNAMIC_BUTTONS_InsEdit;
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
-- 09/12/2010 Paul.  Add default parameter ONCLICK_SCRIPT to ease migration to EffiProz. 
Create Procedure dbo.spDYNAMIC_BUTTONS_InsEdit
	( @VIEW_NAME           nvarchar(50)
	, @CONTROL_INDEX       int
	, @MODULE_NAME         nvarchar(25)
	)
as
  begin
	set nocount on
	
	exec dbo.spDYNAMIC_BUTTONS_InsButtonLink
		  @VIEW_NAME
		, @CONTROL_INDEX
		, @MODULE_NAME
		, N'edit'
		, null
		, null
		, N'Edit'
		, N'edit.aspx?ID={0}'
		, N'ID'
		, N'.LBL_EDIT_BUTTON_LABEL'
		, N'.LBL_EDIT_BUTTON_TITLE'
		, N'.LBL_EDIT_BUTTON_KEY'
		, null
		, null
		, null          -- ONCLICK_SCRIPT
		;

  end
GO

Grant Execute on dbo.spDYNAMIC_BUTTONS_InsEdit to public;
GO

