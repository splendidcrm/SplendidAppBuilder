set nocount on;
GO

-- delete from DETAILVIEWS_FIELDS where DETAIL_NAME = 'DynamicButtons.DetailView';
if not exists(select * from DETAILVIEWS_FIELDS where DETAIL_NAME = 'DynamicButtons.DetailView' and DELETED = 0) begin -- then 
	print 'DETAILVIEWS_FIELDS DynamicButtons.DetailView'; 
	exec dbo.spDETAILVIEWS_InsertOnly 'DynamicButtons.DetailView', 'DynamicButtons', 'vwDYNAMIC_BUTTONS_Edit', '15%', '35%', null; 
	exec dbo.spDETAILVIEWS_FIELDS_InsBound      'DynamicButtons.DetailView',  0, 'DynamicButtons.LBL_VIEW_NAME'         , 'VIEW_NAME'          , '{0}', null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound      'DynamicButtons.DetailView',  1, 'DynamicButtons.LBL_CONTROL_INDEX'     , 'CONTROL_INDEX'      , '{0}', null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBoundList  'DynamicButtons.DetailView',  2, 'DynamicButtons.LBL_CONTROL_TYPE'      , 'CONTROL_TYPE'       , '{0}', 'dynamic_button_type_dom', null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBoundList  'DynamicButtons.DetailView',  3, 'DynamicButtons.LBL_MODULE_NAME'       , 'MODULE_NAME'        , '{0}', 'Modules'                , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBoundList  'DynamicButtons.DetailView',  4, 'DynamicButtons.LBL_MODULE_ACCESS_TYPE', 'MODULE_ACCESS_TYPE' , '{0}', 'module_access_type_dom' , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBoundList  'DynamicButtons.DetailView',  5, 'DynamicButtons.LBL_TARGET_NAME'       , 'TARGET_NAME'        , '{0}', 'Modules'                , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBoundList  'DynamicButtons.DetailView',  6, 'DynamicButtons.LBL_TARGET_ACCESS_TYPE', 'TARGET_ACCESS_TYPE' , '{0}', 'module_access_type_dom' , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsCheckbox   'DynamicButtons.DetailView',  7, 'DynamicButtons.LBL_MOBILE_ONLY'       , 'MOBILE_ONLY'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsCheckbox   'DynamicButtons.DetailView',  8, 'DynamicButtons.LBL_ADMIN_ONLY'        , 'ADMIN_ONLY'         , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsCheckbox   'DynamicButtons.DetailView',  9, 'DynamicButtons.LBL_EXCLUDE_MOBILE'    , 'EXCLUDE_MOBILE'     , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound      'DynamicButtons.DetailView', 10, 'DynamicButtons.LBL_CONTROL_TEXT'      , 'CONTROL_TEXT'       , '{0}', null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound      'DynamicButtons.DetailView', 11, 'DynamicButtons.LBL_CONTROL_TOOLTIP'   , 'CONTROL_TOOLTIP'    , '{0}', null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound      'DynamicButtons.DetailView', 12, 'DynamicButtons.LBL_CONTROL_ACCESSKEY' , 'CONTROL_ACCESSKEY'  , '{0}', null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound      'DynamicButtons.DetailView', 13, 'DynamicButtons.LBL_CONTROL_CSSCLASS'  , 'CONTROL_CSSCLASS'   , '{0}', null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound      'DynamicButtons.DetailView', 14, 'DynamicButtons.LBL_TEXT_FIELD'        , 'TEXT_FIELD'         , '{0}', null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound      'DynamicButtons.DetailView', 15, 'DynamicButtons.LBL_ARGUMENT_FIELD'    , 'ARGUMENT_FIELD'     , '{0}', null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound      'DynamicButtons.DetailView', 16, 'DynamicButtons.LBL_COMMAND_NAME'      , 'COMMAND_NAME'       , '{0}', null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound      'DynamicButtons.DetailView', 17, 'DynamicButtons.LBL_URL_FORMAT'        , 'URL_FORMAT'         , '{0}', null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound      'DynamicButtons.DetailView', 18, 'DynamicButtons.LBL_URL_TARGET'        , 'URL_TARGET'         , '{0}', null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound      'DynamicButtons.DetailView', 19, 'DynamicButtons.LBL_ONCLICK_SCRIPT'    , 'ONCLICK_SCRIPT'     , '{0}', null;
	exec dbo.spDETAILVIEWS_FIELDS_InsCheckbox   'DynamicButtons.DetailView', 20, 'DynamicButtons.LBL_HIDDEN'            , 'HIDDEN'             , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBlank      'DynamicButtons.DetailView', 21, null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound      'DynamicButtons.DetailView', 22, 'DynamicButtons.LBL_BUSINESS_RULE'     , 'BUSINESS_RULE'      , '{0}', null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound      'DynamicButtons.DetailView', 23, 'DynamicButtons.LBL_BUSINESS_SCRIPT'   , 'BUSINESS_SCRIPT'    , '{0}', null;
end -- if;
GO

set nocount off;
GO


