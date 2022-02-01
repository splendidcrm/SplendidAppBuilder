set nocount on;
GO

-- delete from GRIDVIEWS_COLUMNS where GRID_NAME = 'DynamicLayout.GridView.PopupView';
if not exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'DynamicLayout.GridView.PopupView' and DELETED = 0) begin -- then
	print 'GRIDVIEWS_COLUMNS DynamicLayout.GridView.PopupView';
	exec dbo.spGRIDVIEWS_InsertOnly           'DynamicLayout.GridView.PopupView'  , 'DynamicLayout', 'vwGRIDVIEWS_COLUMNS';
	exec dbo.spGRIDVIEWS_COLUMNS_InsHyperLink 'DynamicLayout.GridView.PopupView'  , 1, 'DynamicLayout.LBL_LIST_GRID_NAME'   , 'GRID_NAME'       , 'GRID_NAME'       , '25%', 'listViewTdLinkS1', 'ID GRID_NAME', 'SelectLayoutField(''{0}'', ''{1}'');', null, null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'DynamicLayout.GridView.PopupView'  , 2, 'DynamicLayout.LBL_LIST_COLUMN_INDEX', 'COLUMN_INDEX'    , 'COLUMN_INDEX'    , '10%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'DynamicLayout.GridView.PopupView'  , 3, 'DynamicLayout.LBL_LIST_COLUMN_TYPE' , 'COLUMN_TYPE'     , 'COLUMN_TYPE'     , '10%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'DynamicLayout.GridView.PopupView'  , 4, 'DynamicLayout.LBL_LIST_DATA_FORMAT' , 'DATA_FORMAT'     , 'DATA_FORMAT'     , '10%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'DynamicLayout.GridView.PopupView'  , 5, 'DynamicLayout.LBL_LIST_DATA_FIELD'  , 'DATA_FIELD'      , 'DATA_FIELD'      , '20%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'DynamicLayout.GridView.PopupView'  , 6, 'DynamicLayout.LBL_LIST_HEADER_TEXT' , 'HEADER_TEXT'     , 'HEADER_TEXT'     , '25%';
end -- if;
GO

set nocount off;
GO


