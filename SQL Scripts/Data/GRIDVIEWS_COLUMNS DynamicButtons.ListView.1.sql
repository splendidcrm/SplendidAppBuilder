set nocount on;
GO

-- delete from GRIDVIEWS_COLUMNS where GRID_NAME = 'DynamicButtons.ListView';
if not exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'DynamicButtons.ListView' and DELETED = 0) begin -- then
	print 'GRIDVIEWS_COLUMNS DynamicButtons.ListView';
	exec dbo.spGRIDVIEWS_InsertOnly           'DynamicButtons.ListView', 'DynamicButtons', 'vwDYNAMIC_BUTTONS';
	exec dbo.spGRIDVIEWS_COLUMNS_InsHyperLink 'DynamicButtons.ListView',  2, 'DynamicButtons.LBL_LIST_CONTROL_TEXT', 'CONTROL_TEXT'    , 'CONTROL_TEXT'    , '19%', 'listViewTdLinkS1', 'ID', '~/Administration/DynamicButtons/view.aspx?id={0}', null, 'DynamicButtons', null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'DynamicButtons.ListView',  3, 'DynamicButtons.LBL_LIST_MODULE_NAME' , 'MODULE_NAME'     , 'MODULE_NAME'     , '10%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'DynamicButtons.ListView',  4, 'DynamicButtons.LBL_LIST_CONTROL_TYPE', 'CONTROL_TYPE'    , 'CONTROL_TYPE'    , '10%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'DynamicButtons.ListView',  5, 'DynamicButtons.LBL_LIST_COMMAND_NAME', 'COMMAND_NAME'    , 'COMMAND_NAME'    , '10%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'DynamicButtons.ListView',  6, 'DynamicButtons.LBL_LIST_TEXT_FIELD'  , 'TEXT_FIELD'      , 'TEXT_FIELD'      , '10%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'DynamicButtons.ListView',  7, 'DynamicButtons.LBL_LIST_URL_FORMAT'  , 'URL_FORMAT'      , 'URL_FORMAT'      , '30%';
end -- if;
GO

set nocount off;
GO


