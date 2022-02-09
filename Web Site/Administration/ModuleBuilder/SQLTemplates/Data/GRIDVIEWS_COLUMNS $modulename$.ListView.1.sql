
-- delete from GRIDVIEWS_COLUMNS where GRID_NAME = '$modulename$.ListView' and DELETED = 0;
if not exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = '$modulename$.ListView' and DELETED = 0) begin -- then
	print 'GRIDVIEWS_COLUMNS $modulename$.ListView';
	exec dbo.spGRIDVIEWS_InsertOnly           '$modulename$.ListView', '$modulename$'      , 'vw$tablename$_List'      ;
$modulegridviewdata$
end -- if;
GO

