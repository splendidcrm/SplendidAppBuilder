set nocount on;
GO

-- 04/11/2011 Paul.  Add support for Dynamic Layout popups. 
-- delete from GRIDVIEWS_COLUMNS where GRID_NAME = 'DynamicLayout.DetailView.PopupView';
if not exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'DynamicLayout.DetailView.PopupView' and DELETED = 0) begin -- then
	print 'GRIDVIEWS_COLUMNS DynamicLayout.DetailView.PopupView';
	exec dbo.spGRIDVIEWS_InsertOnly           'DynamicLayout.DetailView.PopupView', 'DynamicLayout', 'vwDETAILVIEWS_FIELDS';
	exec dbo.spGRIDVIEWS_COLUMNS_InsHyperLink 'DynamicLayout.DetailView.PopupView', 1, 'DynamicLayout.LBL_LIST_DETAIL_NAME' , 'DETAIL_NAME'     , 'DETAIL_NAME'     , '20%', 'listViewTdLinkS1', 'ID DETAIL_NAME', 'SelectLayoutField(''{0}'', ''{1}'');', null, null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'DynamicLayout.DetailView.PopupView', 2, 'DynamicLayout.LBL_LIST_FIELD_INDEX' , 'FIELD_INDEX'     , 'FIELD_INDEX'     , '10%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'DynamicLayout.DetailView.PopupView', 3, 'DynamicLayout.LBL_LIST_FIELD_TYPE'  , 'FIELD_TYPE'      , 'FIELD_TYPE'      , '10%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'DynamicLayout.DetailView.PopupView', 4, 'DynamicLayout.LBL_LIST_DATA_FIELD'  , 'DATA_FIELD'      , 'DATA_FIELD'      , '20%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'DynamicLayout.DetailView.PopupView', 5, 'DynamicLayout.LBL_LIST_DATA_LABEL'  , 'DATA_LABEL'      , 'DATA_LABEL'      , '25%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'DynamicLayout.DetailView.PopupView', 6, 'DynamicLayout.LBL_LIST_LIST_NAME'   , 'LIST_NAME'       , 'LIST_NAME'       , '10%';
end -- if;
GO

set nocount off;
GO


