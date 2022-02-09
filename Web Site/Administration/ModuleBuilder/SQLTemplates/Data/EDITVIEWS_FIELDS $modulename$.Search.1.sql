
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = '$modulename$.SearchBasic';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = '$modulename$.SearchBasic' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS $modulename$.SearchBasic';
	exec dbo.spEDITVIEWS_InsertOnly             '$modulename$.SearchBasic'    , '$modulename$', 'vw$tablename$_List', '11%', '22%', 3;
$moduleeditviewsearchbasic$
end -- if;
GO

-- delete from EDITVIEWS_FIELDS where EDIT_NAME = '$modulename$.SearchAdvanced';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = '$modulename$.SearchAdvanced' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS $modulename$.SearchAdvanced';
	exec dbo.spEDITVIEWS_InsertOnly             '$modulename$.SearchAdvanced' , '$modulename$', 'vw$tablename$_List', '11%', '22%', 3;
$moduleeditviewsearchadvanced$
end -- if;
GO

-- delete from EDITVIEWS_FIELDS where EDIT_NAME = '$modulename$.SearchPopup';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = '$modulename$.SearchPopup' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS $modulename$.SearchPopup';
	exec dbo.spEDITVIEWS_InsertOnly             '$modulename$.SearchPopup'    , '$modulename$', 'vw$tablename$_List', '11%', '22%', 3;
$moduleeditviewsearchpopup$
end -- if;
GO

