set nocount on;
GO

if not exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'ModuleBuilder.PopupView' and DELETED = 0) begin -- then
	print 'GRIDVIEWS_COLUMNS ModuleBuilder.PopupView';
	exec dbo.spGRIDVIEWS_InsertOnly           'ModuleBuilder.PopupView', 'ModuleBuilder', 'vwMODULES';
	exec dbo.spGRIDVIEWS_COLUMNS_InsHyperLink 'ModuleBuilder.PopupView',  1, 'Modules.LBL_LIST_MODULE_NAME', 'MODULE_NAME', 'MODULE_NAME', '99%', 'listViewTdLinkS1', 'MODULE_NAME MODULE_NAME', 'SelectModule(''{0}'', ''{1}'');', null, null, null;
end -- if;
GO

set nocount off;
GO


