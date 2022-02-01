set nocount on;
GO

-- delete from GRIDVIEWS_COLUMNS where GRID_NAME = 'Config.ListView';
if not exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'Config.ListView' and DELETED = 0) begin -- then
	print 'GRIDVIEWS_COLUMNS Config.ListView';
	exec dbo.spGRIDVIEWS_InsertOnly           'Config.ListView', 'Config', 'vwCONFIG_List';
	exec dbo.spGRIDVIEWS_COLUMNS_InsHyperLink 'Config.ListView'        ,  2, 'Config.LBL_LIST_NAME'                , 'NAME'            , 'NAME'            , '35%', 'listViewTdLinkS1', 'ID', '~/Administration/Config/view.aspx?id={0}', null, 'Config', null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Config.ListView'        ,  3, 'Config.LBL_LIST_CATEGORY'            , 'CATEGORY'        , 'CATEGORY'        , '10%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Config.ListView'        ,  4, 'Config.LBL_LIST_VALUE'               , 'VALUE'           , 'VALUE'           , '50%';
end -- if;
GO

set nocount off;
GO


