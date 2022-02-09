-- delete from DETAILVIEWS_FIELDS where DETAIL_NAME = '$modulename$.DetailView';

if not exists(select * from DETAILVIEWS_FIELDS where DETAIL_NAME = '$modulename$.DetailView' and DELETED = 0) begin -- then
	print 'DETAILVIEWS_FIELDS $modulename$.DetailView';
	exec dbo.spDETAILVIEWS_InsertOnly          '$modulename$.DetailView'   , '$modulename$', 'vw$tablename$_Edit', '15%', '35%';
$moduledetailviewdata$
end -- if;
GO

