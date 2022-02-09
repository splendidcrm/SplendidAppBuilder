
-- delete from GRIDVIEWS_COLUMNS where GRID_NAME = '$modulename$.Export';
if not exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = '$modulename$.Export' and DELETED = 0) begin -- then
	print 'GRIDVIEWS_COLUMNS $modulename$.Export';
	exec dbo.spGRIDVIEWS_InsertOnly           '$modulename$.Export', '$modulename$', 'vw$tablename$_List';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     '$modulename$.Export'         ,  1, '$modulename$.LBL_LIST_NAME'                       , 'NAME'                       , null, null;
end -- if;
GO

