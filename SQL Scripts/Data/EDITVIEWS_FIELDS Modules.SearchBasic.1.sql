set nocount on;
GO

-- 06/22/2013 Paul.  Add search for Modules. 
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'Modules.SearchBasic';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Modules.SearchBasic' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Modules.SearchBasic';
	exec dbo.spEDITVIEWS_InsertOnly             'Modules.SearchBasic'     , 'Modules', 'vwMODULES_List', '15%', '35%', 2;
	exec dbo.spEDITVIEWS_FIELDS_InsBound        'Modules.SearchBasic'     ,  0, 'Modules.LBL_MODULE_NAME'                , 'MODULE_NAME'                , 0, null,  50, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound        'Modules.SearchBasic'     ,  1, 'Modules.LBL_TABLE_NAME'                 , 'TABLE_NAME'                 , 0, null,  50, 35, null;
end -- if;
GO

set nocount off;
GO


