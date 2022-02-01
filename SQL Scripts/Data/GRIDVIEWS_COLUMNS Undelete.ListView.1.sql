set nocount on;
GO

-- 09/14/2021 Paul.  Add Undelete support to React Client. 
-- delete from GRIDVIEWS_COLUMNS where GRID_NAME = 'Undelete.ListView';
if not exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'Undelete.ListView' and DELETED = 0) begin -- then
	print 'GRIDVIEWS_COLUMNS Undelete.ListView';
	exec dbo.spGRIDVIEWS_InsertOnly           'Undelete.ListView', 'Undelete', 'vwCONFIG', 'AUDIT_DATE', 'desc';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Undelete.ListView'   ,  1, 'Undelete.LBL_LIST_NAME'                , 'NAME'            , 'NAME'       , '70%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Undelete.ListView'   ,  2, 'Undelete.LBL_LIST_AUDIT_TOKEN'         , 'AUDIT_TOKEN'     , 'AUDIT_TOKEN', '10%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Undelete.ListView'   ,  3, 'Undelete.LBL_LIST_MODIFIED_BY'         , 'MODIFIED_BY'     , 'MODIFIED_BY', '10%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Undelete.ListView'   ,  4, 'Undelete.LBL_LIST_AUDIT_DATE'          , 'AUDIT_DATE'      , 'AUDIT_DATE' , '10%';
	exec dbo.spGRIDVIEWS_COLUMNS_UpdateStyle  null, 'Undelete.ListView', 2, null, null, null, null, 0;
	exec dbo.spGRIDVIEWS_COLUMNS_UpdateStyle  null, 'Undelete.ListView', 3, null, null, null, null, 0;
	exec dbo.spGRIDVIEWS_COLUMNS_UpdateStyle  null, 'Undelete.ListView', 4, null, null, null, null, 0;
end -- if;
GO


set nocount off;
GO


