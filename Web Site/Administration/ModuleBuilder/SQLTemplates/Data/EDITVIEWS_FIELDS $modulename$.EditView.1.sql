
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = '$modulename$.EditView';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = '$modulename$.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS $modulename$.EditView';
	exec dbo.spEDITVIEWS_InsertOnly            '$modulename$.EditView', '$modulename$'      , 'vw$tablename$_Edit'      , '15%', '35%', null;
$moduleeditviewdata$
end -- if;
--GO

