if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spEDITVIEWS_FIELDS_CnvAutoComplete' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spEDITVIEWS_FIELDS_CnvAutoComplete;
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
Create Procedure dbo.spEDITVIEWS_FIELDS_CnvAutoComplete
	( @EDIT_NAME         nvarchar( 50)
	, @FIELD_INDEX       int
	, @DATA_LABEL        nvarchar(150)
	, @DATA_FIELD        nvarchar(100)
	, @DATA_REQUIRED     bit
	, @FORMAT_TAB_INDEX  int
	, @FORMAT_MAX_LENGTH int
	, @FORMAT_SIZE       int
	, @MODULE_TYPE       nvarchar(25)
	, @COLSPAN           int
	)
as
  begin
	-- 08/26/2009 Paul.  We are going to ignore the following fields for now. 
	-- Keeping them in the method will make it easy to duplicate the InsModulePopup call. 
	-- @DATA_REQUIRED     bit
	-- @FORMAT_TAB_INDEX  int
	-- @FORMAT_MAX_LENGTH int
	-- @FORMAT_SIZE       int
	-- @COLSPAN           int

	if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = @EDIT_NAME and DATA_FIELD = @DATA_FIELD and FIELD_TYPE = N'TextBox' and DELETED = 0) begin -- then
		-- 08/27/2009 Paul.  The update will take care of the main record and the default view record. 
		update EDITVIEWS_FIELDS
		   set FIELD_TYPE        = N'ModuleAutoComplete'
		     , MODULE_TYPE       = @MODULE_TYPE
		     , DISPLAY_FIELD     = null
		     , ONCLICK_SCRIPT    = null
		     , DATE_MODIFIED     = getdate()
		     , DATE_MODIFIED_UTC= getutcdate()
		     , MODIFIED_USER_ID  = null
		 where EDIT_NAME         = @EDIT_NAME
		   and DATA_FIELD        = @DATA_FIELD
		   and FIELD_TYPE        = N'TextBox'
		   and DELETED           = 0;
	end -- if;
	-- 09/10/2009 Paul.  Allow conversion from ModulePopup, where data field matches display field. 
	if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = @EDIT_NAME and DISPLAY_FIELD = @DATA_FIELD and FIELD_TYPE = N'ModulePopup' and DELETED = 0) begin -- then
		-- 08/27/2009 Paul.  The update will take care of the main record and the default view record. 
		update EDITVIEWS_FIELDS
		   set FIELD_TYPE        = N'ModuleAutoComplete'
		     , MODULE_TYPE       = @MODULE_TYPE
		     , DATA_FIELD        = @DATA_FIELD
		     , DISPLAY_FIELD     = null
		     , ONCLICK_SCRIPT    = null
		     , DATE_MODIFIED     = getdate()
		     , DATE_MODIFIED_UTC= getutcdate()
		     , MODIFIED_USER_ID  = null
		 where EDIT_NAME         = @EDIT_NAME
		   and DISPLAY_FIELD     = @DATA_FIELD
		   and FIELD_TYPE        = N'ModulePopup'
		   and DELETED           = 0;
	end -- if;
  end
GO

Grant Execute on dbo.spEDITVIEWS_FIELDS_CnvAutoComplete to public;
GO

