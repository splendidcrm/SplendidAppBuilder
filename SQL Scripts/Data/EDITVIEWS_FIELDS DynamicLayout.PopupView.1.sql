set nocount on;
GO

-- 04/11/2011 Paul.  Add support for Dynamic Layout popups. 
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'DynamicLayout.DetailView.PopupView';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'DynamicLayout.DetailView.PopupView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS DynamicLayout.DetailView.PopupView';
	exec dbo.spEDITVIEWS_InsertOnly             'DynamicLayout.DetailView.PopupView', 'DynamicLayout', 'vwDETAILVIEWS_FIELDS', '15%', '35%', 2;
	exec dbo.spEDITVIEWS_FIELDS_InsBound        'DynamicLayout.DetailView.PopupView',  0, 'DynamicLayout.LBL_DETAIL_NAME'     , 'DETAIL_NAME'             , 0, null,  50, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound        'DynamicLayout.DetailView.PopupView',  1, 'DynamicLayout.LBL_LIST_DATA_FIELD' , 'DATA_FIELD'              , 0, null,  50, 35, null;
end -- if;
GO

-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'DynamicLayout.EditView.PopupView';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'DynamicLayout.EditView.PopupView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS DynamicLayout.EditView.PopupView';
	exec dbo.spEDITVIEWS_InsertOnly             'DynamicLayout.EditView.PopupView', 'DynamicLayout', 'vwEDITVIEWS_FIELDS', '15%', '35%', 2;
	exec dbo.spEDITVIEWS_FIELDS_InsBound        'DynamicLayout.EditView.PopupView',  0, 'DynamicLayout.LBL_EDIT_NAME'       , 'EDIT_NAME'               , 0, null,  50, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound        'DynamicLayout.EditView.PopupView',  1, 'DynamicLayout.LBL_LIST_DATA_FIELD' , 'DATA_FIELD'              , 0, null,  50, 35, null;
end -- if;
GO

-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'DynamicLayout.GridView.PopupView';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'DynamicLayout.GridView.PopupView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS DynamicLayout.GridView.PopupView';
	exec dbo.spEDITVIEWS_InsertOnly             'DynamicLayout.GridView.PopupView', 'DynamicLayout', 'vwGRIDVIEWS_COLUMNS', '15%', '35%', 2;
	exec dbo.spEDITVIEWS_FIELDS_InsBound        'DynamicLayout.GridView.PopupView',  0, 'DynamicLayout.LBL_GRID_NAME'       , 'GRID_NAME'               , 0, null,  50, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound        'DynamicLayout.GridView.PopupView',  1, 'DynamicLayout.LBL_LIST_DATA_FIELD' , 'DATA_FIELD'              , 0, null,  50, 35, null;
end -- if;
GO

set nocount off;
GO


