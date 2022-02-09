-- delete from DETAILVIEWS_RELATIONSHIPS where DETAIL_NAME = '$modulename$.DetailView';

if not exists(select * from DETAILVIEWS_RELATIONSHIPS where DETAIL_NAME = '$modulename$.DetailView' and MODULE_NAME = '$relatedmodule$' and DELETED = 0) begin -- then
	print 'DETAILVIEWS_RELATIONSHIPS $modulename$.DetailView';
	exec dbo.spDETAILVIEWS_RELATIONSHIPS_InsertOnly '$modulename$.DetailView', '$relatedmodule$', '$relatedmodule$', null, '$relatedmodule$.LBL_MODULE_NAME', 'vw$tablename$_$relatedtable$', '$tablenamesingular$_ID', 'NAME', 'asc';
end -- if;
GO

