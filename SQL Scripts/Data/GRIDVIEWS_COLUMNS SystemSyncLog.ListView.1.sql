set nocount on;
GO

-- 10/30/2020 Paul.  The React Client needs a layout for SystemSyncLog. 
-- 02/24/2021 Paul.  The React client needs a way to determine the default sort, besides NAME asc. 
-- delete from GRIDVIEWS_COLUMNS where GRID_NAME = 'SystemSyncLog.ListView';
if not exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'SystemSyncLog.ListView' and DELETED = 0) begin -- then
	print 'GRIDVIEWS_COLUMNS SystemSyncLog.ListView';
	exec dbo.spGRIDVIEWS_InsertOnly           'SystemSyncLog.ListView', 'SystemSyncLog', 'vwSYSTEM_SYNC_LOG', 'DATE_ENTERED', 'desc';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBoundDate 'SystemSyncLog.ListView'     ,  0, '.LBL_LIST_DATE_ENTERED'              , 'DATE_ENTERED'    , 'DATE_ENTERED'    , '10%', 'DateTime';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'SystemSyncLog.ListView'     ,  2, 'SystemLog.LBL_LIST_ERROR_TYPE'       , 'ERROR_TYPE'      , 'ERROR_TYPE'      , '10%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'SystemSyncLog.ListView'     ,  3, 'SystemLog.LBL_LIST_MESSAGE'          , 'MESSAGE'         , 'MESSAGE'         , '40%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'SystemSyncLog.ListView'     ,  4, 'SystemLog.LBL_LIST_FILE_NAME'        , 'FILE_NAME'       , 'FILE_NAME'       , '25%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'SystemSyncLog.ListView'     ,  5, 'SystemLog.LBL_LIST_METHOD'           , 'METHOD'          , 'METHOD'          , '10%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'SystemSyncLog.ListView'     ,  6, 'SystemLog.LBL_LIST_LINE_NUMBER'      , 'LINE_NUMBER'     , 'LINE_NUMBER'     , '5%';
	exec dbo.spGRIDVIEWS_COLUMNS_UpdateStyle  null, 'SystemSyncLog.ListView'    , 0, null, null, null, null, 0;
end else begin
	-- 02/24/2021 Paul.  The React client needs a way to determine the default sort, besides NAME asc. 
	if exists(select * from GRIDVIEWS where NAME = 'SystemSyncLog.ListView' and SORT_FIELD is null and DELETED = 0) begin -- then
		exec dbo.spGRIDVIEWS_UpdateSort null, 'SystemSyncLog.ListView', 'DATE_ENTERED', 'desc';
	end -- if;
end -- if;
GO

set nocount off;
GO


