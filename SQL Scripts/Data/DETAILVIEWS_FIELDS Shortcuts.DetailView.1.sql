set nocount on;
GO

-- delete from DETAILVIEWS_FIELDS where DETAIL_NAME = 'Shortcuts.DetailView';
if not exists(select * from DETAILVIEWS_FIELDS where DETAIL_NAME = 'Shortcuts.DetailView' and DELETED = 0) begin -- then
	print 'DETAILVIEWS_FIELDS Shortcuts.DetailView';
	exec dbo.spDETAILVIEWS_InsertOnly           'Shortcuts.DetailView', 'Shortcuts', 'vwSHORTCUTS_Edit', '15%', '35%', null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBoundList  'Shortcuts.DetailView'     ,  0, 'Shortcuts.LBL_MODULE_NAME'            , 'MODULE_NAME'        , '{0}'        , 'Modules'  , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound      'Shortcuts.DetailView'     ,  1, 'Shortcuts.LBL_DISPLAY_NAME'           , 'DISPLAY_NAME'       , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound      'Shortcuts.DetailView'     ,  2, 'Shortcuts.LBL_RELATIVE_PATH'          , 'RELATIVE_PATH'      , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound      'Shortcuts.DetailView'     ,  3, 'Shortcuts.LBL_IMAGE_NAME'             , 'IMAGE_NAME'         , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound      'Shortcuts.DetailView'     ,  4, 'Shortcuts.LBL_SHORTCUT_ORDER'         , 'SHORTCUT_ORDER'     , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsCheckBox   'Shortcuts.DetailView'     ,  5, 'Shortcuts.LBL_SHORTCUT_ENABLED'       , 'SHORTCUT_ENABLED'   , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBoundList  'Shortcuts.DetailView'     ,  6, 'Shortcuts.LBL_SHORTCUT_MODULE'        , 'SHORTCUT_MODULE'    , '{0}'        , 'Modules'              , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBoundList  'Shortcuts.DetailView'     ,  7, 'Shortcuts.LBL_SHORTCUT_ACLTYPE'       , 'SHORTCUT_ACLTYPE'   , '{0}'        , 'shortcuts_acltype_dom', null;
end -- if;
GO

set nocount off;
GO


