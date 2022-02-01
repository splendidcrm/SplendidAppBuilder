set nocount on;
GO

-- 02/20/2021 Paul.  HyperLink on DISPLAY_NAME. 
-- delete from GRIDVIEWS_COLUMNS where GRID_NAME = 'Shortcuts.ListView';
if not exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'Shortcuts.ListView' and DELETED = 0) begin -- then
	print 'GRIDVIEWS_COLUMNS Shortcuts.ListView';
	exec dbo.spGRIDVIEWS_InsertOnly           'Shortcuts.ListView', 'Shortcuts', 'vwCONFIG_List';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Shortcuts.ListView'     ,  2, 'Shortcuts.LBL_LIST_MODULE_NAME'      , 'MODULE_NAME'     , 'MODULE_NAME'     , '25%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsHyperLink 'Shortcuts.ListView'     ,  3, 'Shortcuts.LBL_LIST_DISPLAY_NAME'     , 'DISPLAY_NAME'    , 'DISPLAY_NAME'    , '30%', 'listViewTdLinkS1', 'ID', '~/Administration/Shortcuts/edit.aspx?id={0}', null, 'Shortcuts', null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Shortcuts.ListView'     ,  4, 'Shortcuts.LBL_LIST_RELATIVE_PATH'    , 'RELATIVE_PATH'   , 'RELATIVE_PATH'   , '30%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Shortcuts.ListView'     ,  5, 'Shortcuts.LBL_LIST_SHORTCUT_ORDER'   , 'SHORTCUT_ORDER'  , 'SHORTCUT_ORDER'  , '10%';
end else begin
	-- 02/20/2021 Paul.  HyperLink on DISPLAY_NAME. 
	if exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'Shortcuts.ListView' and DATA_FIELD = 'DISPLAY_NAME' and COLUMN_TYPE = 'BoundColumn' and DELETED = 0) begin -- then
		update GRIDVIEWS_COLUMNS
		   set COLUMN_TYPE        = 'TemplateColumn'
		     , ITEMSTYLE_CSSCLASS = 'listViewTdLinkS1'
		     , DATA_FORMAT        = 'HyperLink'
		     , URL_FIELD          = 'ID'
		     , URL_FORMAT         = '~/Administration/Shortcuts/edit.aspx?id={0}'
		     , URL_MODULE         = 'Shortcuts'
		     , DATE_MODIFIED      = getdate()
		     , DATE_MODIFIED_UTC  = getutcdate()
		 where GRID_NAME          = 'Shortcuts.ListView'
		    and DATA_FIELD        = 'DISPLAY_NAME'
		   and COLUMN_TYPE        = 'BoundColumn'
		   and DELETED            = 0;
		update GRIDVIEWS_COLUMNS
		   set COLUMN_TYPE        = 'BoundColumn'
		     , ITEMSTYLE_CSSCLASS = null
		     , DATA_FORMAT        = null
		     , URL_FIELD          = null
		     , URL_FORMAT         = null
		     , URL_MODULE         = null
		     , DATE_MODIFIED      = getdate()
		     , DATE_MODIFIED_UTC  = getutcdate()
		 where GRID_NAME          = 'Shortcuts.ListView'
		    and DATA_FIELD        = 'MODULE_NAME'
		   and COLUMN_TYPE        = 'TemplateColumn'
		   and DELETED            = 0;
	end -- if;
end -- if;
GO

set nocount off;
GO


