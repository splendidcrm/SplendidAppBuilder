set nocount on;
GO

-- 03/05/2011 Paul.  Add search for Config. 
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'Config.SearchBasic';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Config.SearchBasic' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Config.SearchBasic';
	exec dbo.spEDITVIEWS_InsertOnly             'Config.SearchBasic'      , 'Config', 'vwCONFIG_List', '15%', '35%', 2;
	exec dbo.spEDITVIEWS_FIELDS_InsBound        'Config.SearchBasic'      ,  0, 'Config.LBL_NAME'                        , 'NAME'                       , 0, null,  50, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound        'Config.SearchBasic'      ,  1, 'Config.LBL_VALUE'                       , 'VALUE'                      , 0, null, 200, 35, null;
end -- if;
GO

set nocount off;
GO


