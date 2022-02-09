
-- delete from GRIDVIEWS_COLUMNS where GRID_NAME = '$modulename$.PopupView' and DELETED = 0;
if not exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = '$modulename$.PopupView' and DELETED = 0) begin -- then
	print 'GRIDVIEWS_COLUMNS $modulename$.PopupView';
	exec dbo.spGRIDVIEWS_InsertOnly           '$modulename$.PopupView', '$modulename$'      , 'vw$tablename$_List'      ;
$modulegridviewpopup$
end -- if;
GO

