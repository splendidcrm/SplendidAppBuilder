
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = '$modulename$.PopupView.Inline';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = '$modulename$.PopupView.Inline' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS $modulename$.PopupView.Inline';
	exec dbo.spEDITVIEWS_InsertOnly            '$modulename$.PopupView.Inline', '$modulename$'      , 'vw$tablename$_Edit'      , '15%', '35%', null;
$modulepopupviewdatainline$
end -- if;
--GO

