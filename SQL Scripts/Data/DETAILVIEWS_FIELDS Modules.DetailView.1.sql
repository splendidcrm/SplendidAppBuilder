set nocount on;
GO

-- 09/09/2009 Paul.  Allow direct editing of the module table. 
-- delete from DETAILVIEWS_FIELDS where DETAIL_NAME = 'Modules.DetailView';
-- 12/02/2009 Paul.  Add the ability to disable Mass Updates. 
-- 04/01/2010 Paul.  Add Exchange Sync flag. 
-- 12/12/2010 Paul.  Missing last parameter in spDETAILVIEWS_InsertOnly. 
-- 06/18/2011 Paul.  REST_ENABLED provides a way to enable/disable a module in the REST API. 
-- 03/14/2014 Paul.  DUPLICATE_CHECHING_ENABLED enables duplicate checking. 
-- 11/01/2017 Paul.  Use a module-based flag so that Record Level Security is only enabled when needed. 
if not exists(select * from DETAILVIEWS_FIELDS where DETAIL_NAME = 'Modules.DetailView' and DELETED = 0) begin -- then
	print 'DETAILVIEWS_FIELDS Modules.DetailView';
	exec dbo.spDETAILVIEWS_InsertOnly          'Modules.DetailView' , 'Modules', 'vwMODULES', '15%', '35%', null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Modules.DetailView' ,  0, 'Modules.LBL_MODULE_NAME'           , 'MODULE_NAME'                      , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Modules.DetailView' ,  1, 'Modules.LBL_DISPLAY_NAME'          , 'DISPLAY_NAME'                     , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Modules.DetailView' ,  2, 'Modules.LBL_RELATIVE_PATH'         , 'RELATIVE_PATH'                    , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Modules.DetailView' ,  3, 'Modules.LBL_TABLE_NAME'            , 'TABLE_NAME'                       , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Modules.DetailView' ,  4, 'Modules.LBL_TAB_ORDER'             , 'TAB_ORDER'                        , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Modules.DetailView' ,  5, 'Modules.LBL_PORTAL_ENABLED'        , 'PORTAL_ENABLED'                   , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Modules.DetailView' ,  6, 'Modules.LBL_MODULE_ENABLED'        , 'MODULE_ENABLED'                   , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Modules.DetailView' ,  7, 'Modules.LBL_TAB_ENABLED'           , 'TAB_ENABLED'                      , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Modules.DetailView' ,  8, 'Modules.LBL_IS_ADMIN'              , 'IS_ADMIN'                         , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Modules.DetailView' ,  9, 'Modules.LBL_CUSTOM_ENABLED'        , 'CUSTOM_ENABLED'                   , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Modules.DetailView' , 10, 'Modules.LBL_REPORT_ENABLED'        , 'REPORT_ENABLED'                   , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Modules.DetailView' , 11, 'Modules.LBL_IMPORT_ENABLED'        , 'IMPORT_ENABLED'                   , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Modules.DetailView' , 12, 'Modules.LBL_MOBILE_ENABLED'        , 'MOBILE_ENABLED'                   , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Modules.DetailView' , 13, 'Modules.LBL_CUSTOM_PAGING'         , 'CUSTOM_PAGING'                    , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Modules.DetailView' , 14, 'Modules.LBL_MASS_UPDATE_ENABLED'   , 'MASS_UPDATE_ENABLED'              , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Modules.DetailView' , 15, 'Modules.LBL_DEFAULT_SEARCH_ENABLED', 'DEFAULT_SEARCH_ENABLED'           , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Modules.DetailView' , 16, 'Modules.LBL_EXCHANGE_SYNC'         , 'EXCHANGE_SYNC'                    , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Modules.DetailView' , 17, 'Modules.LBL_EXCHANGE_FOLDERS'      , 'EXCHANGE_FOLDERS'                 , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Modules.DetailView' , 18, 'Modules.LBL_EXCHANGE_CREATE_PARENT', 'EXCHANGE_CREATE_PARENT'           , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Modules.DetailView' , 19, 'Modules.LBL_REST_ENABLED'          , 'REST_ENABLED'                     , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Modules.DetailView' , 20, 'Modules.LBL_DUPLICATE_CHECHING_ENABLED', 'DUPLICATE_CHECHING_ENABLED'   , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Modules.DetailView' , 21, 'Modules.LBL_RECORD_LEVEL_SECURITY_ENABLED', 'RECORD_LEVEL_SECURITY_ENABLED'        , '{0}'        , null;
end else begin
	if not exists(select * from DETAILVIEWS_FIELDS where DETAIL_NAME = 'Modules.DetailView' and DATA_FIELD = 'MASS_UPDATE_ENABLED' and DELETED = 0) begin -- then
		exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Modules.DetailView' , 14, 'Modules.LBL_MASS_UPDATE_ENABLED' , 'MASS_UPDATE_ENABLED'              , '{0}'        , null;
		exec dbo.spDETAILVIEWS_FIELDS_InsBlank     'Modules.DetailView' , 15, null;
	end -- if;
	if not exists(select * from DETAILVIEWS_FIELDS where DETAIL_NAME = 'Modules.DetailView' and DATA_FIELD = 'DEFAULT_SEARCH_ENABLED' and DELETED = 0) begin -- then
		exec dbo.spDETAILVIEWS_FIELDS_CnvBound     'Modules.DetailView' , 15, 'Modules.LBL_DEFAULT_SEARCH_ENABLED' , 'DEFAULT_SEARCH_ENABLED'        , '{0}'        , null;
	end -- if;
	-- 04/01/2010 Paul.  Add Exchange Sync flag. 
	if not exists(select * from DETAILVIEWS_FIELDS where DETAIL_NAME = 'Modules.DetailView' and DATA_FIELD = 'EXCHANGE_SYNC' and DELETED = 0) begin -- then
		exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Modules.DetailView' , 16, 'Modules.LBL_EXCHANGE_SYNC'       , 'EXCHANGE_SYNC'                    , '{0}'        , null;
	end -- if;
	-- 04/04/2010 Paul.  Add Exchange Folders flag. 
	if not exists(select * from DETAILVIEWS_FIELDS where DETAIL_NAME = 'Modules.DetailView' and DATA_FIELD = 'EXCHANGE_FOLDERS' and DELETED = 0) begin -- then
		exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Modules.DetailView' , 17, 'Modules.LBL_EXCHANGE_FOLDERS'    , 'EXCHANGE_FOLDERS'                 , '{0}'        , null;
	end -- if;
	-- 04/05/2010 Paul.  Add Exchange Create Parent flag. Need to be able to disable Account creation. 
	if not exists(select * from DETAILVIEWS_FIELDS where DETAIL_NAME = 'Modules.DetailView' and DATA_FIELD = 'EXCHANGE_CREATE_PARENT' and DELETED = 0) begin -- then
		exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Modules.DetailView' , 18, 'Modules.LBL_EXCHANGE_CREATE_PARENT', 'EXCHANGE_CREATE_PARENT'         , '{0}'        , null;
	end -- if;
	-- 06/23/2010 Paul.  Allow display of the Portal flag. 
	if not exists(select * from DETAILVIEWS_FIELDS where DETAIL_NAME = 'Modules.DetailView' and DATA_FIELD = 'PORTAL_ENABLED' and DELETED = 0) begin -- then
		update DETAILVIEWS_FIELDS
		   set DELETED           = 1
		     , DATE_MODIFIED     = getdate()
		     , MODIFIED_USER_ID  = null
		 where DETAIL_NAME       = 'Modules.DetailView'
		   and FIELD_TYPE        = 'Blank'
		   and FIELD_INDEX       = 5
		   and DELETED           = 0;
		exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Modules.DetailView' ,  5, 'Modules.LBL_PORTAL_ENABLED'        , 'PORTAL_ENABLED'                   , '{0}'        , null;
	end -- if;
	-- 06/18/2011 Paul.  REST_ENABLED provides a way to enable/disable a module in the REST API. 
	if not exists(select * from DETAILVIEWS_FIELDS where DETAIL_NAME = 'Modules.DetailView' and DATA_FIELD = 'REST_ENABLED' and DELETED = 0) begin -- then
		exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Modules.DetailView' , 19, 'Modules.LBL_REST_ENABLED'          , 'REST_ENABLED'                     , '{0}'        , null;
	end -- if;
	-- 03/14/2014 Paul.  DUPLICATE_CHECHING_ENABLED enables duplicate checking. 
	if not exists(select * from DETAILVIEWS_FIELDS where DETAIL_NAME = 'Modules.DetailView' and DATA_FIELD = 'DUPLICATE_CHECHING_ENABLED' and DELETED = 0) begin -- then
		exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Modules.DetailView' , 20, 'Modules.LBL_DUPLICATE_CHECHING_ENABLED', 'DUPLICATE_CHECHING_ENABLED', '{0}'        , null;
	end -- if;
	-- 11/01/2017 Paul.  Use a module-based flag so that Record Level Security is only enabled when needed. 
	if not exists(select * from DETAILVIEWS_FIELDS where DETAIL_NAME = 'Modules.DetailView' and DATA_FIELD = 'RECORD_LEVEL_SECURITY_ENABLED' and DELETED = 0) begin -- then
		exec dbo.spDETAILVIEWS_FIELDS_CnvBound     'Modules.DetailView' , 21, 'Modules.LBL_RECORD_LEVEL_SECURITY_ENABLED', 'RECORD_LEVEL_SECURITY_ENABLED'     , '{0}'        , null;
	end -- if;
end -- if;
GO

set nocount off;
GO


