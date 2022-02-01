if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spEDITVIEWS_FIELDS_CnvModulePopup' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spEDITVIEWS_FIELDS_CnvModulePopup;
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
-- 08/27/2009 Paul.  We need to make it easier to convert a ChangeButton to a ModulePopup. 
-- 08/27/2009 Paul.  Also allow conversion from ListBox to a ModulePopup. 
-- 04/20/2016 Paul.  Add team management to Outbound Emails. 
Create Procedure dbo.spEDITVIEWS_FIELDS_CnvModulePopup
	( @EDIT_NAME         nvarchar( 50)
	, @FIELD_INDEX       int
	, @DATA_LABEL        nvarchar(150)
	, @DATA_FIELD        nvarchar(100)
	, @DATA_REQUIRED     bit
	, @FORMAT_TAB_INDEX  int
	, @DISPLAY_FIELD     nvarchar(100)
	, @MODULE_TYPE       nvarchar(25)
	, @COLSPAN           int
	)
as
  begin
	-- 08/26/2009 Paul.  We are going to ignore the following fields for now. 
	-- Keeping them in the method will make it easy to duplicate the InsModulePopup call. 
	-- @DATA_REQUIRED     bit
	-- @FORMAT_TAB_INDEX  int
	-- @DISPLAY_FIELD     nvarchar(100)
	-- @MODULE_TYPE       nvarchar(25)
	-- @COLSPAN           int

	if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = @EDIT_NAME and DATA_FIELD = @DATA_FIELD and FIELD_TYPE = N'ChangeButton' and DELETED = 0) begin -- then
		-- 08/27/2009 Paul.  The update will take care of the main record and the default view record. 
		update EDITVIEWS_FIELDS
		   set FIELD_TYPE        = N'ModulePopup'
		     , MODULE_TYPE       = @MODULE_TYPE
		     , ONCLICK_SCRIPT    = null
		     , DATE_MODIFIED     = getdate()
		     , DATE_MODIFIED_UTC = getutcdate()
		     , MODIFIED_USER_ID  = null
		 where EDIT_NAME         = @EDIT_NAME
		   and DATA_FIELD        = @DATA_FIELD
		   and FIELD_TYPE        = N'ChangeButton'
		   and DELETED           = 0;
	end -- if;

	if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = @EDIT_NAME and DATA_FIELD = @DATA_FIELD and FIELD_TYPE = N'ListBox' and DELETED = 0) begin -- then
		-- 08/27/2009 Paul.  The update will take care of the main record and the default view record. 
		update EDITVIEWS_FIELDS
		   set FIELD_TYPE        = N'ModulePopup'
		     , MODULE_TYPE       = @MODULE_TYPE
		     , ONCLICK_SCRIPT    = null
		     , DATE_MODIFIED     = getdate()
		     , DATE_MODIFIED_UTC = getutcdate()
		     , MODIFIED_USER_ID  = null
		 where EDIT_NAME         = @EDIT_NAME
		   and DATA_FIELD        = @DATA_FIELD
		   and FIELD_TYPE        = N'ListBox'
		   and DELETED           = 0;
	end -- if;

	-- 04/20/2016 Paul.  Add team management to Outbound Emails. 
	if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = @EDIT_NAME and FIELD_INDEX = @FIELD_INDEX and FIELD_TYPE = N'Blank' and DELETED = 0) begin -- then
		update EDITVIEWS_FIELDS
		   set FIELD_TYPE        = N'ModulePopup'
		     , DATA_LABEL        = @DATA_LABEL
		     , DATA_FIELD        = @DATA_FIELD
		     , DATA_REQUIRED     = @DATA_REQUIRED
		     , UI_REQUIRED       = @DATA_REQUIRED
		     , FORMAT_TAB_INDEX  = @FORMAT_TAB_INDEX
		     , DISPLAY_FIELD     = @DISPLAY_FIELD
		     , MODULE_TYPE       = @MODULE_TYPE
		     , COLSPAN           = @COLSPAN
		     , DATE_MODIFIED     = getdate()
		     , DATE_MODIFIED_UTC = getutcdate()
		     , MODIFIED_USER_ID  = null
		 where EDIT_NAME         = @EDIT_NAME
		   and FIELD_INDEX       = @FIELD_INDEX
		   and FIELD_TYPE        = N'Blank'
		   and DELETED           = 0;
	end -- if;
  end
GO

Grant Execute on dbo.spEDITVIEWS_FIELDS_CnvModulePopup to public;
GO

