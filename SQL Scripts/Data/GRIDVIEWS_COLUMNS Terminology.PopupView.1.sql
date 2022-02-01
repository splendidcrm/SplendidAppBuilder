set nocount on;
GO

-- 10/17/2020 Paul.  The React Client needs to select from a list of terms. 
if not exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'Terminology.PopupView' and DELETED = 0) begin -- then
	print 'GRIDVIEWS_COLUMNS Terminology.PopupView';
	exec dbo.spGRIDVIEWS_InsertOnly           'Terminology.PopupView', 'Terminology', 'vwTERMINOLOGY_List';
	exec dbo.spGRIDVIEWS_COLUMNS_InsHyperLink 'Terminology.PopupView'       , 1, 'Terminology.LBL_LIST_NAME'              , 'NAME'            , 'NAME'            , '40%', 'listViewTdLinkS1', 'NAME NAME', 'SelectTerm(''{0}'', ''{1}'');', null, null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBoundList 'Terminology.PopupView'       , 2, 'Terminology.LBL_LIST_DISPLAY_NAME'      , 'DISPLAY_NAME'    , 'DISPLAY_NAME'    , '40%', 'countries_dom';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Terminology.PopupView'       , 3, 'Terminology.LBL_LIST_LIST_ORDER'        , 'LIST_ORDER'      , 'LIST_ORDER'      , '15%';
end -- if;
GO


set nocount off;
GO


