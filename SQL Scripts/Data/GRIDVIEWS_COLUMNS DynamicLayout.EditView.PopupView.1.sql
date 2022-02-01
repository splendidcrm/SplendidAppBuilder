set nocount on;
GO

-- delete from GRIDVIEWS_COLUMNS where GRID_NAME = 'DynamicLayout.EditView.PopupView';
if not exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'DynamicLayout.EditView.PopupView' and DELETED = 0) begin -- then
	print 'GRIDVIEWS_COLUMNS DynamicLayout.EditView.PopupView';
	exec dbo.spGRIDVIEWS_InsertOnly           'DynamicLayout.EditView.PopupView'  , 'DynamicLayout', 'vwEDITVIEWS_FIELDS';
	exec dbo.spGRIDVIEWS_COLUMNS_InsHyperLink 'DynamicLayout.EditView.PopupView'  , 1, 'DynamicLayout.LBL_LIST_EDIT_NAME'   , 'EDIT_NAME'       , 'EDIT_NAME'       , '25%', 'listViewTdLinkS1', 'ID EDIT_NAME', 'SelectLayoutField(''{0}'', ''{1}'');', null, null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'DynamicLayout.EditView.PopupView'  , 2, 'DynamicLayout.LBL_LIST_FIELD_INDEX' , 'FIELD_INDEX'     , 'FIELD_INDEX'     , '10%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'DynamicLayout.EditView.PopupView'  , 3, 'DynamicLayout.LBL_LIST_FIELD_TYPE'  , 'FIELD_TYPE'      , 'FIELD_TYPE'      , '10%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'DynamicLayout.EditView.PopupView'  , 4, 'DynamicLayout.LBL_LIST_DATA_FIELD'  , 'DATA_FIELD'      , 'DATA_FIELD'      , '20%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'DynamicLayout.EditView.PopupView'  , 5, 'DynamicLayout.LBL_LIST_DATA_LABEL'  , 'DATA_LABEL'      , 'DATA_LABEL'      , '25%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'DynamicLayout.EditView.PopupView'  , 6, 'DynamicLayout.LBL_LIST_LIST_NAME'   , 'LIST_NAME'       , 'LIST_NAME'       , '10%';
end -- if;
GO

set nocount off;
GO


