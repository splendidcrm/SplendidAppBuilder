set nocount on;
GO

-- 03/29/2012 Paul.  Add Rules Wizard support to Terminology module. 
-- 12/05/2012 Paul.  LBL_LIST_NAME is getting confused with the list version, so change to LBL_NAME. 
-- delete from GRIDVIEWS_COLUMNS where GRID_NAME = 'Terminology.ListView' and DELETED = 0;
if not exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'Terminology.ListView' and DELETED = 0) begin -- then
	print 'GRIDVIEWS_COLUMNS Terminology.ListView';
	exec dbo.spGRIDVIEWS_InsertOnly           'Terminology.ListView', 'Terminology', 'vwTERMINOLOGY';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Terminology.ListView'       ,  1, 'Terminology.LBL_LIST_MODULE_NAME'         , 'MODULE_NAME'     , 'MODULE_NAME'     , '10%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsHyperLink 'Terminology.ListView'       ,  2, 'Terminology.LBL_NAME'                     , 'NAME'            , 'NAME'            , '22%', 'listViewTdLinkS1', 'ID', '~/Administration/Terminology/view.aspx?id={0}', null, 'Terminology', null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Terminology.ListView'       ,  3, 'Terminology.LBL_LIST_LANG'                , 'LANG'            , 'LANG'            , '10%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Terminology.ListView'       ,  4, 'Terminology.LBL_LIST_LIST_NAME'           , 'LIST_NAME'       , 'LIST_NAME'       , '10%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Terminology.ListView'       ,  5, 'Terminology.LBL_LIST_LIST_ORDER'          , 'LIST_ORDER'      , 'LIST_ORDER'      , '10%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Terminology.ListView'       ,  6, 'Terminology.LBL_LIST_DISPLAY_NAME'        , 'DISPLAY_NAME'    , 'DISPLAY_NAME'    , '30%';
end -- if;
GO

set nocount off;
GO


