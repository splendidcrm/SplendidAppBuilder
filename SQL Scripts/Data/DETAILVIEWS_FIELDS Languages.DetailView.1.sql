set nocount on;
GO

-- 02/21/2021 Paul.  Languages for React client. 
-- delete from DETAILVIEWS_FIELDS where DETAIL_NAME = 'Languages.DetailView';
if not exists(select * from DETAILVIEWS_FIELDS where DETAIL_NAME = 'Languages.DetailView' and DELETED = 0) begin -- then
	print 'DETAILVIEWS_FIELDS Languages.DetailView';
	exec dbo.spDETAILVIEWS_InsertOnly           'Languages.DetailView', 'Languages', 'vwLANGUAGES', '15%', '35%', null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound      'Languages.DetailView'     ,  0, 'Languages.LBL_NAME'                   , 'NAME'               , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound      'Languages.DetailView'     ,  1, 'Languages.LBL_LCID'                   , 'LCID'               , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsCheckbox   'Languages.DetailView'     ,  2, 'Languages.LBL_ACTIVE'                 , 'ACTIVE'             , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound      'Languages.DetailView'     ,  3, 'Languages.LBL_NATIVE_NAME'            , 'NATIVE_NAME'        , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound      'Languages.DetailView'     ,  4, 'Languages.LBL_DISPLAY_NAME'           , 'DISPLAY_NAME'       , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBlank      'Languages.DetailView'     ,  5, null;
end -- if;
GO

set nocount off;
GO


