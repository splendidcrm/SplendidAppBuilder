set nocount on;
GO

-- 09/09/2009 Paul.  Allow direct editing of the module table. 
-- 06/22/2013 Paul.  Add edit link and move to front. 
-- delete from GRIDVIEWS_COLUMNS where GRID_NAME = 'Modules.ListView' and DELETED = 0;
if not exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'Modules.ListView' and DELETED = 0) begin -- then
	print 'GRIDVIEWS_COLUMNS Modules.ListView';
	exec dbo.spGRIDVIEWS_InsertOnly           'Modules.ListView', 'Modules', 'vwMODULES';
	exec dbo.spGRIDVIEWS_COLUMNS_InsHyperLink 'Modules.ListView'           ,  0, 'Modules.LBL_LIST_MODULE_NAME'             , 'MODULE_NAME'     , 'MODULE_NAME'     , '30%', 'listViewTdLinkS1', 'ID', '~/Administration/Modules/view.aspx?id={0}', null, 'Modules', null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Modules.ListView'           ,  1, 'Modules.LBL_LIST_TABLE_NAME'              , 'TABLE_NAME'      , 'TABLE_NAME'      , '15%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Modules.ListView'           ,  2, 'Modules.LBL_LIST_RELATIVE_PATH'           , 'RELATIVE_PATH'   , 'RELATIVE_PATH'   , '15%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Modules.ListView'           ,  3, 'Modules.LBL_LIST_MODULE_ENABLED'          , 'MODULE_ENABLED'  , 'MODULE_ENABLED'  , '10%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Modules.ListView'           ,  4, 'Modules.LBL_LIST_CUSTOM_PAGING'           , 'CUSTOM_PAGING'   , 'CUSTOM_PAGING'   , '10%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Modules.ListView'           ,  5, 'Modules.LBL_LIST_TAB_ORDER'               , 'TAB_ORDER'       , 'TAB_ORDER'       , '10%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Modules.ListView'           ,  6, 'Modules.LBL_LIST_IS_ADMIN'                , 'IS_ADMIN'        , 'IS_ADMIN'        , '10%';
end else begin
	if exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'Modules.ListView' and DATA_FIELD = 'MODULE_NAME' and COLUMN_INDEX = 0 and DELETED = 0) begin -- then
		print 'GRIDVIEWS_COLUMNS Modules.ListView: Move edit link to front. ';
		update GRIDVIEWS_COLUMNS
		   set COLUMN_INDEX      = COLUMN_INDEX + 1
		     , DATE_MODIFIED     = getdate()
		     , DATE_MODIFIED_UTC = getutcdate()
		 where GRID_NAME         = 'Modules.ListView'
		   and DELETED           = 0;
	end -- if;
end -- if;
GO

set nocount off;
GO


