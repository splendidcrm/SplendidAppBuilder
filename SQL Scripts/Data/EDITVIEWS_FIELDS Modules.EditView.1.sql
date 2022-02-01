set nocount on;
GO

-- 09/09/2009 Paul.  Allow direct editing of the module table. 
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'Modules.EditView';
-- 12/02/2009 Paul.  Add the ability to disable Mass Updates. 
-- 04/01/2010 Paul.  Add Exchange Sync flag. 
-- 06/18/2011 Paul.  REST_ENABLED provides a way to enable/disable a module in the REST API. 
-- 03/14/2014 Paul.  DUPLICATE_CHECHING_ENABLED enables duplicate checking. 
-- 06/15/2017 Paul.  Allow the RELATIVE_PATH to be editable so that html5 can be enabled or disabled manually. 
-- 11/01/2017 Paul.  Use a module-based flag so that Record Level Security is only enabled when needed. 
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Modules.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Modules.EditView';
	exec dbo.spEDITVIEWS_InsertOnly            'Modules.EditView'       , 'Modules', 'vwMODULES', '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel       'Modules.EditView'       ,  0, 'Modules.LBL_MODULE_NAME'              , 'MODULE_NAME'           , null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Modules.EditView'       ,  1, 'Modules.LBL_DISPLAY_NAME'             , 'DISPLAY_NAME'          , 1, 1, 50, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Modules.EditView'       ,  2, 'Modules.LBL_RELATIVE_PATH'            , 'RELATIVE_PATH'         , 1, 1, 50, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel       'Modules.EditView'       ,  3, 'Modules.LBL_TABLE_NAME'               , 'TABLE_NAME'            , null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Modules.EditView'       ,  4, 'Modules.LBL_TAB_ORDER'                , 'TAB_ORDER'             , 1, 1, 10, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Modules.EditView'       ,  5, 'Modules.LBL_PORTAL_ENABLED'           , 'PORTAL_ENABLED'        , 0, 1, 'CheckBox', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Modules.EditView'       ,  6, 'Modules.LBL_MODULE_ENABLED'           , 'MODULE_ENABLED'        , 0, 1, 'CheckBox', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Modules.EditView'       ,  7, 'Modules.LBL_TAB_ENABLED'              , 'TAB_ENABLED'           , 0, 1, 'CheckBox', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel       'Modules.EditView'       ,  8, 'Modules.LBL_IS_ADMIN'                 , 'IS_ADMIN'              , null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel       'Modules.EditView'       ,  9, 'Modules.LBL_CUSTOM_ENABLED'           , 'CUSTOM_ENABLED'        , null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Modules.EditView'       , 10, 'Modules.LBL_REPORT_ENABLED'           , 'REPORT_ENABLED'        , 0, 1, 'CheckBox', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Modules.EditView'       , 11, 'Modules.LBL_IMPORT_ENABLED'           , 'IMPORT_ENABLED'        , 0, 1, 'CheckBox', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Modules.EditView'       , 12, 'Modules.LBL_MOBILE_ENABLED'           , 'MOBILE_ENABLED'        , 0, 1, 'CheckBox', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Modules.EditView'       , 13, 'Modules.LBL_CUSTOM_PAGING'            , 'CUSTOM_PAGING'         , 0, 1, 'CheckBox', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Modules.EditView'       , 14, 'Modules.LBL_MASS_UPDATE_ENABLED'      , 'MASS_UPDATE_ENABLED'   , 0, 1, 'CheckBox', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Modules.EditView'       , 15, 'Modules.LBL_DEFAULT_SEARCH_ENABLED'   , 'DEFAULT_SEARCH_ENABLED', 0, 1, 'CheckBox', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Modules.EditView'       , 16, 'Modules.LBL_EXCHANGE_SYNC'            , 'EXCHANGE_SYNC'         , 0, 1, 'CheckBox', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Modules.EditView'       , 17, 'Modules.LBL_EXCHANGE_FOLDERS'         , 'EXCHANGE_FOLDERS'      , 0, 1, 'CheckBox', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Modules.EditView'       , 18, 'Modules.LBL_EXCHANGE_CREATE_PARENT'   , 'EXCHANGE_CREATE_PARENT', 0, 1, 'CheckBox', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Modules.EditView'       , 19, 'Modules.LBL_REST_ENABLED'             , 'REST_ENABLED'          , 0, 1, 'CheckBox', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Modules.EditView'       , 20, 'Modules.LBL_DUPLICATE_CHECHING_ENABLED', 'DUPLICATE_CHECHING_ENABLED', 0, 1, 'CheckBox', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Modules.EditView'       , 21, 'Modules.LBL_RECORD_LEVEL_SECURITY_ENABLED', 'RECORD_LEVEL_SECURITY_ENABLED', 0, 1, 'CheckBox', null, null, null;
end else begin
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Modules.EditView' and DATA_FIELD = 'MASS_UPDATE_ENABLED' and DELETED = 0) begin -- then
		exec dbo.spEDITVIEWS_FIELDS_InsControl     'Modules.EditView'       , 14, 'Modules.LBL_MASS_UPDATE_ENABLED'      , 'MASS_UPDATE_ENABLED', 0, 1, 'CheckBox', null, null, null;
	end -- if;
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Modules.EditView' and DATA_FIELD = 'DEFAULT_SEARCH_ENABLED' and DELETED = 0) begin -- then
		update EDITVIEWS_FIELDS
		   set DELETED           = 1
		     , DATE_MODIFIED     = getdate()
		     , MODIFIED_USER_ID  = null
		 where EDIT_NAME         = 'Modules.EditView'
		   and FIELD_TYPE        = 'Blank'
		   and FIELD_INDEX       = 15
		   and DELETED           = 0;
		exec dbo.spEDITVIEWS_FIELDS_InsControl     'Modules.EditView'       , 15, 'Modules.LBL_DEFAULT_SEARCH_ENABLED'   , 'DEFAULT_SEARCH_ENABLED', 0, 1, 'CheckBox', null, null, null;
	end -- if;
	-- 04/01/2010 Paul.  Add Exchange Sync flag. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Modules.EditView' and DATA_FIELD = 'EXCHANGE_SYNC' and DELETED = 0) begin -- then
		exec dbo.spEDITVIEWS_FIELDS_InsControl     'Modules.EditView'       , 16, 'Modules.LBL_EXCHANGE_SYNC'            , 'EXCHANGE_SYNC'         , 0, 1, 'CheckBox', null, null, null;
	end -- if;
	-- 04/04/2010 Paul.  Add Exchange Folders flag. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Modules.EditView' and DATA_FIELD = 'EXCHANGE_FOLDERS' and DELETED = 0) begin -- then
		exec dbo.spEDITVIEWS_FIELDS_InsControl     'Modules.EditView'       , 17, 'Modules.LBL_EXCHANGE_FOLDERS'         , 'EXCHANGE_FOLDERS'      , 0, 1, 'CheckBox', null, null, null;
	end -- if;
	-- 04/05/2010 Paul.  Add Exchange Create Parent flag. Need to be able to disable Account creation. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Modules.EditView' and DATA_FIELD = 'EXCHANGE_CREATE_PARENT' and DELETED = 0) begin -- then
		exec dbo.spEDITVIEWS_FIELDS_InsControl     'Modules.EditView'       , 17, 'Modules.LBL_EXCHANGE_CREATE_PARENT'   , 'EXCHANGE_CREATE_PARENT', 0, 1, 'CheckBox', null, null, null;
	end -- if;
	-- 06/23/2010 Paul.  Allow editing of the Portal flag. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Modules.EditView' and DATA_FIELD = 'PORTAL_ENABLED' and DELETED = 0) begin -- then
		update EDITVIEWS_FIELDS
		   set DELETED           = 1
		     , DATE_MODIFIED     = getdate()
		     , MODIFIED_USER_ID  = null
		 where EDIT_NAME         = 'Modules.EditView'
		   and FIELD_TYPE        = 'Blank'
		   and FIELD_INDEX       = 5
		   and DELETED           = 0;
		exec dbo.spEDITVIEWS_FIELDS_InsControl     'Modules.EditView'       ,  5, 'Modules.LBL_PORTAL_ENABLED'   , 'PORTAL_ENABLED', 0, 1, 'CheckBox', null, null, null;
	end -- if;
	-- 06/18/2011 Paul.  REST_ENABLED provides a way to enable/disable a module in the REST API. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Modules.EditView' and DATA_FIELD = 'REST_ENABLED' and DELETED = 0) begin -- then
		exec dbo.spEDITVIEWS_FIELDS_InsControl     'Modules.EditView'       , 19, 'Modules.LBL_REST_ENABLED'             , 'REST_ENABLED'          , 0, 1, 'CheckBox', null, null, null;
	end -- if;
	-- 03/14/2014 Paul.  DUPLICATE_CHECHING_ENABLED enables duplicate checking. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Modules.EditView' and DATA_FIELD = 'DUPLICATE_CHECHING_ENABLED' and DELETED = 0) begin -- then
		exec dbo.spEDITVIEWS_FIELDS_InsControl     'Modules.EditView'       , 20, 'Modules.LBL_DUPLICATE_CHECHING_ENABLED', 'DUPLICATE_CHECHING_ENABLED', 0, 1, 'CheckBox', null, null, null;
		exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Modules.EditView'       , 21, null;
	end -- if;
	-- 06/15/2017 Paul.  Allow the RELATIVE_PATH to be editable so that html5 can be enabled or disabled manually. 
	if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Modules.EditView' and DATA_FIELD = 'RELATIVE_PATH' and FIELD_TYPE = 'Label' and DELETED = 0) begin -- then
		update EDITVIEWS_FIELDS
		   set FIELD_TYPE        = 'TextBox'
		     , DATA_REQUIRED     = 1
		     , UI_REQUIRED       = 1
		     , FORMAT_TAB_INDEX  = 1
		     , FORMAT_SIZE       = 35
		     , FORMAT_MAX_LENGTH = 50
		     , DATE_MODIFIED     = getdate()
		     , MODIFIED_USER_ID  = null
		 where EDIT_NAME         = 'Modules.EditView'
		   and DATA_FIELD        = 'RELATIVE_PATH'
		   and FIELD_TYPE        = 'Label'
		   and DELETED           = 0;
	end -- if;
	-- 11/01/2017 Paul.  Use a module-based flag so that Record Level Security is only enabled when needed. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Modules.EditView' and DATA_FIELD = 'RECORD_LEVEL_SECURITY_ENABLED' and DELETED = 0) begin -- then
		update EDITVIEWS_FIELDS
		   set DELETED           = 1
		     , DATE_MODIFIED     = getdate()
		     , MODIFIED_USER_ID  = null
		 where EDIT_NAME         = 'Modules.EditView'
		   and FIELD_TYPE        = 'Blank'
		   and FIELD_INDEX       = 21
		   and DELETED           = 0;
		exec dbo.spEDITVIEWS_FIELDS_InsControl     'Modules.EditView'       , 21, 'Modules.LBL_RECORD_LEVEL_SECURITY_ENABLED'   , 'RECORD_LEVEL_SECURITY_ENABLED', 0, 1, 'CheckBox', null, null, null;
	end -- if;
end -- if;
GO


set nocount off;
GO


