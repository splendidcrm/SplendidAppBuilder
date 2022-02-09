
-- delete from GRIDVIEWS_COLUMNS where GRID_NAME = '$modulename$.$relatedmodule$' and DELETED = 0;
if not exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = '$modulename$.$relatedmodule$' and DELETED = 0) begin -- then
	print 'GRIDVIEWS_COLUMNS $modulename$.$relatedmodule$';
	exec dbo.spGRIDVIEWS_InsertOnly           '$modulename$.$relatedmodule$', '$modulename$', 'vw$tablename$_$relatedtable$';
	exec dbo.spGRIDVIEWS_COLUMNS_InsHyperLink '$modulename$.$relatedmodule$', 0, '$relatedmodule$.LBL_LIST_NAME', 'NAME', 'NAME', '50%', 'listViewTdLinkS1', 'ID', '~/$relatedmodule$/view.aspx?id={0}', null, '$relatedmodule$', 'ASSIGNED_USER_ID';
end -- if;
GO

