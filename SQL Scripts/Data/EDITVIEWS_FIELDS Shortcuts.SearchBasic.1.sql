set nocount on;
GO

-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'Shortcuts.SearchBasic';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Shortcuts.SearchBasic' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Shortcuts.SearchBasic';
	exec dbo.spEDITVIEWS_InsertOnly             'Shortcuts.SearchBasic'   , 'Shortcuts', 'vwSHORTCUTS_Edit', '15%', '35%', 2;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList    'Shortcuts.SearchBasic'   ,  0, 'Shortcuts.LBL_MODULE_NAME'              , 'MODULE_NAME'                , 1, null, 'Modules'           , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank        'Shortcuts.SearchBasic'   ,  1, null;
end -- if;
GO

set nocount off;
GO


