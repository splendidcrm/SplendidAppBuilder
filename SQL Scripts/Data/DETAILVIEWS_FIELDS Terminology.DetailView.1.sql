set nocount on;
GO

-- delete from DETAILVIEWS_FIELDS where DETAIL_NAME = 'Terminology.DetailView';
if not exists(select * from DETAILVIEWS_FIELDS where DETAIL_NAME = 'Terminology.DetailView' and DELETED = 0) begin -- then
	print 'DETAILVIEWS_FIELDS Terminology.DetailView';
	exec dbo.spDETAILVIEWS_InsertOnly           'Terminology.DetailView', 'Terminology', 'vwTERMINOLOGY_Edit', '15%', '35%', null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound      'Terminology.DetailView'   ,  0, 'Terminology.LBL_NAME'                 , 'NAME'               , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBoundList  'Terminology.DetailView'   ,  1, 'Terminology.LBL_LANG'                 , 'LANG'               , '{0}'        , 'Languages', null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBoundList  'Terminology.DetailView'   ,  2, 'Terminology.LBL_MODULE_NAME'          , 'MODULE_NAME'        , '{0}'        , 'Modules'  , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBlank      'Terminology.DetailView'   ,  3, null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound      'Terminology.DetailView'   ,  4, 'Terminology.LBL_LIST_NAME'            , 'LIST_NAME'          , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound      'Terminology.DetailView'   ,  5, 'Terminology.LBL_LIST_ORDER'           , 'LIST_ORDER'         , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound      'Terminology.DetailView'   ,  6, 'Terminology.LBL_DISPLAY_NAME'         , 'DISPLAY_NAME'       , '{0}'        , 3;
end -- if;
GO


set nocount off;
GO


