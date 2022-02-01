set nocount on;
GO

if not exists(select * from DETAILVIEWS_FIELDS where DETAIL_NAME = 'Config.DetailView' and DELETED = 0) begin -- then
	print 'DETAILVIEWS_FIELDS Config.DetailView';
	exec dbo.spDETAILVIEWS_InsertOnly           'Config.DetailView', 'Config', 'vwCONFIG_Edit', '15%', '35%', null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound      'Config.DetailView'        ,  0, 'Config.LBL_NAME'                      , 'NAME'               , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound      'Config.DetailView'        ,  1, 'Config.LBL_CATEGORY'                  , 'CATEGORY'           , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound      'Config.DetailView'        ,  2, 'Config.LBL_VALUE'                     , 'VALUE'              , '{0}'        , 3;
end -- if;
GO

set nocount off;
GO


