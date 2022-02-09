
-- delete from GRIDVIEWS_COLUMNS where GRID_NAME = '$modulename$.SearchDuplicates';
if not exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = '$modulename$.SearchDuplicates' and DELETED = 0) begin -- then
	print 'GRIDVIEWS_COLUMNS $modulename$.SearchDuplicates';
	exec dbo.spGRIDVIEWS_InsertOnly           '$modulename$.SearchDuplicates', '$modulename$', 'vw$tablename$_List';
	exec dbo.spGRIDVIEWS_COLUMNS_InsHyperLink '$modulename$.SearchDuplicates'          , 1, '$modulename$.LBL_LIST_NAME'                   , 'NAME'            , 'NAME'            , '50%', 'listViewTdLinkS1', 'ID'         , '~/$modulename$/view.aspx?id={0}', null, '$modulename$', 'ASSIGNED_USER_ID';
end -- if;
GO

