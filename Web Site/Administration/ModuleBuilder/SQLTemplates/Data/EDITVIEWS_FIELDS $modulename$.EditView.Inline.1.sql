
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = '$modulename$.EditView.Inline';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = '$modulename$.EditView.Inline' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS $modulename$.EditView.Inline';
	exec dbo.spEDITVIEWS_InsertOnly            '$modulename$.EditView.Inline', '$modulename$'      , 'vw$tablename$_Edit'      , '15%', '35%', null;
$moduleeditviewdatainline$
end -- if;
--GO

