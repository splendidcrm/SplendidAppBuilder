set nocount on;
GO

-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'DynamicButtons.EditView';
-- 07/28/2010 Paul.  We need a flag to exclude a button from a mobile device. 
-- 03/14/2014 Paul.  Allow hidden buttons to be created. 
-- 08/16/2017 Paul.  Add ability to apply a business rule to a button. 
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'DynamicButtons.EditView' and DELETED = 0) begin -- then 
	print 'EDITVIEWS_FIELDS DynamicButtons.EditView'; 
	exec dbo.spEDITVIEWS_InsertOnly 'DynamicButtons.EditView', 'DynamicButtons', 'vwDYNAMIC_BUTTONS_Edit', '15%', '35%', null; 
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'DynamicButtons.EditView',  0, 'DynamicButtons.LBL_VIEW_NAME'         , 'VIEW_NAME'          , 1, 1,  50, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel       'DynamicButtons.EditView',  1, 'DynamicButtons.LBL_CONTROL_INDEX'     , 'CONTROL_INDEX'      , null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'DynamicButtons.EditView',  2, 'DynamicButtons.LBL_CONTROL_TYPE'      , 'CONTROL_TYPE'       , 1, 1, 'dynamic_button_type_dom', null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'DynamicButtons.EditView',  3, 'DynamicButtons.LBL_MODULE_NAME'       , 'MODULE_NAME'        , 0, 1, 'Modules'                , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'DynamicButtons.EditView',  4, 'DynamicButtons.LBL_MODULE_ACCESS_TYPE', 'MODULE_ACCESS_TYPE' , 0, 1, 'module_access_type_dom' , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'DynamicButtons.EditView',  5, 'DynamicButtons.LBL_TARGET_NAME'       , 'TARGET_NAME'        , 0, 1, 'Modules'                , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'DynamicButtons.EditView',  6, 'DynamicButtons.LBL_TARGET_ACCESS_TYPE', 'TARGET_ACCESS_TYPE' , 0, 1, 'module_access_type_dom' , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'DynamicButtons.EditView',  7, 'DynamicButtons.LBL_MOBILE_ONLY'       , 'MOBILE_ONLY'        , 0, 1, 'CheckBox', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'DynamicButtons.EditView',  8, 'DynamicButtons.LBL_ADMIN_ONLY'        , 'ADMIN_ONLY'         , 0, 1, 'CheckBox', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'DynamicButtons.EditView',  9, 'DynamicButtons.LBL_EXCLUDE_MOBILE'    , 'EXCLUDE_MOBILE'     , 0, 1, 'CheckBox', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'DynamicButtons.EditView', 10, 'DynamicButtons.LBL_CONTROL_TEXT'      , 'CONTROL_TEXT'       , 0, 1, 150, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'DynamicButtons.EditView', 11, 'DynamicButtons.LBL_CONTROL_TOOLTIP'   , 'CONTROL_TOOLTIP'    , 0, 1, 150, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'DynamicButtons.EditView', 12, 'DynamicButtons.LBL_CONTROL_ACCESSKEY' , 'CONTROL_ACCESSKEY'  , 0, 1, 150, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'DynamicButtons.EditView', 13, 'DynamicButtons.LBL_CONTROL_CSSCLASS'  , 'CONTROL_CSSCLASS'   , 0, 1,  50, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'DynamicButtons.EditView', 14, 'DynamicButtons.LBL_TEXT_FIELD'        , 'TEXT_FIELD'         , 0, 1, 200, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'DynamicButtons.EditView', 15, 'DynamicButtons.LBL_ARGUMENT_FIELD'    , 'ARGUMENT_FIELD'     , 0, 1, 200, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'DynamicButtons.EditView', 16, 'DynamicButtons.LBL_COMMAND_NAME'      , 'COMMAND_NAME'       , 0, 1,  50, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'DynamicButtons.EditView', 17, 'DynamicButtons.LBL_URL_FORMAT'        , 'URL_FORMAT'         , 0, 1, 255, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'DynamicButtons.EditView', 18, 'DynamicButtons.LBL_URL_TARGET'        , 'URL_TARGET'         , 0, 1,  20, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'DynamicButtons.EditView', 19, 'DynamicButtons.LBL_ONCLICK_SCRIPT'    , 'ONCLICK_SCRIPT'     , 0, 1,   3, 60, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'DynamicButtons.EditView', 20, 'DynamicButtons.LBL_HIDDEN'            , 'HIDDEN'             , 0, 1, 'CheckBox', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'DynamicButtons.EditView', 21, null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'DynamicButtons.EditView', 22, 'DynamicButtons.LBL_BUSINESS_RULE'     , 'BUSINESS_RULE'      , 0, 1,   3, 60, null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'DynamicButtons.EditView', 23, 'DynamicButtons.LBL_BUSINESS_SCRIPT'   , 'BUSINESS_SCRIPT'    , 0, 1,   3, 60, null;
end else begin
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'DynamicButtons.EditView' and DATA_FIELD = 'EXCLUDE_MOBILE' and DELETED = 0) begin -- then
		print 'DynamicButtons: Add EXCLUDE_MOBILE.';
		update EDITVIEWS_FIELDS
		   set FIELD_INDEX       = FIELD_INDEX + 1
		     , DATE_MODIFIED     = getdate()
		     , DATE_MODIFIED_UTC = getutcdate()
		     , MODIFIED_USER_ID  = null
		 where EDIT_NAME         = 'DynamicButtons.EditView'
		   and FIELD_INDEX      >= 9
		   and DELETED           = 0;
		exec dbo.spEDITVIEWS_FIELDS_InsControl     'DynamicButtons.EditView',  9, 'DynamicButtons.LBL_EXCLUDE_MOBILE'    , 'EXCLUDE_MOBILE'     , 0, 1, 'CheckBox', null, null, null;
	end -- if;
	-- 03/14/2014 Paul.  Allow hidden buttons to be created. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'DynamicButtons.EditView' and DATA_FIELD = 'HIDDEN' and DELETED = 0) begin -- then
		print 'DynamicButtons: Add HIDDEN.';
		exec dbo.spEDITVIEWS_FIELDS_InsControl     'DynamicButtons.EditView', 20, 'DynamicButtons.LBL_HIDDEN'            , 'HIDDEN'             , 0, 1, 'CheckBox', null, null, null;
		exec dbo.spEDITVIEWS_FIELDS_InsBlank       'DynamicButtons.EditView', 21, null;
	end -- if;
	-- 08/16/2017 Paul.  Increase the size of the ONCLICK_SCRIPT so that we can add a javascript info column. 
	if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'DynamicButtons.EditView' and DATA_FIELD = 'ONCLICK_SCRIPT' and FORMAT_ROWS is null and DELETED = 0) begin -- then
		print 'DynamicButtons: Update ONCLICK_SCRIPT.';
		update EDITVIEWS_FIELDS
		   set FORMAT_ROWS       = 3
		     , FORMAT_COLUMNS    = 60
		     , FORMAT_MAX_LENGTH = null
		     , FORMAT_SIZE       = null
		     , DATE_MODIFIED     = getdate()
		     , DATE_MODIFIED_UTC = getutcdate()
		     , MODIFIED_USER_ID  = null
		 where EDIT_NAME         = 'DynamicButtons.EditView'
		   and DATA_FIELD        = 'ONCLICK_SCRIPT'
		   and DELETED           = 0;
	end -- if;
	-- 08/16/2017 Paul.  Add ability to apply a business rule to a button. 
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'DynamicButtons.EditView', 22, 'DynamicButtons.LBL_BUSINESS_RULE'     , 'BUSINESS_RULE'      , 0, 1,   3, 60, null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'DynamicButtons.EditView', 23, 'DynamicButtons.LBL_BUSINESS_SCRIPT'   , 'BUSINESS_SCRIPT'    , 0, 1,   3, 60, null;
end -- if;
GO

set nocount off;
GO


