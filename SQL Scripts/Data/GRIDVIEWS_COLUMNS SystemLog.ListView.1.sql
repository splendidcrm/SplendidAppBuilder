set nocount on;
GO

-- 02/24/2021 Paul.  The React client needs a way to determine the default sort, besides NAME asc. 
-- 08/18/2021 Paul.  Line number should not have any decimal places. 
-- delete from GRIDVIEWS_COLUMNS where GRID_NAME = 'SystemLog.ListView';
if not exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'SystemLog.ListView' and DELETED = 0) begin -- then
	print 'GRIDVIEWS_COLUMNS SystemLog.ListView';
	exec dbo.spGRIDVIEWS_InsertOnly           'SystemLog.ListView', 'SystemLog', 'vwSYSTEM_LOG', 'DATE_ENTERED', 'desc';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBoundDate 'SystemLog.ListView'     ,  0, '.LBL_LIST_DATE_ENTERED'              , 'DATE_ENTERED'    , 'DATE_ENTERED'    , '10%', 'DateTime';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'SystemLog.ListView'     ,  1, 'SystemLog.LBL_LIST_USER_NAME'        , 'USER_NAME'       , 'USER_NAME'       , '10%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'SystemLog.ListView'     ,  2, 'SystemLog.LBL_LIST_ERROR_TYPE'       , 'ERROR_TYPE'      , 'ERROR_TYPE'      , '10%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'SystemLog.ListView'     ,  3, 'SystemLog.LBL_LIST_MESSAGE'          , 'MESSAGE'         , 'MESSAGE'         , '20%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'SystemLog.ListView'     ,  4, 'SystemLog.LBL_LIST_FILE_NAME'        , 'FILE_NAME'       , 'FILE_NAME'       , '15%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'SystemLog.ListView'     ,  5, 'SystemLog.LBL_LIST_METHOD'           , 'METHOD'          , 'METHOD'          , '10%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'SystemLog.ListView'     ,  6, 'SystemLog.LBL_LIST_LINE_NUMBER'      , 'LINE_NUMBER'     , 'LINE_NUMBER'     , '5%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'SystemLog.ListView'     ,  7, 'SystemLog.LBL_LIST_RELATIVE_PATH'    , 'RELATIVE_PATH'   , 'RELATIVE_PATH'   , '5%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'SystemLog.ListView'     ,  8, 'SystemLog.LBL_LIST_PARAMETERS'       , 'PARAMETERS'      , 'PARAMETERS'      , '15%';
	exec dbo.spGRIDVIEWS_COLUMNS_UpdateStyle  null, 'SystemLog.ListView'    , 0, null, null, null, null, 0;
	exec dbo.spGRIDVIEWS_COLUMNS_UpdateFormat null, 'SystemLog.ListView', 'LINE_NUMBER', '{0:N}';
end else begin
	-- 02/24/2021 Paul.  The React client needs a way to determine the default sort, besides NAME asc. 
	if exists(select * from GRIDVIEWS where NAME = 'SystemLog.ListView' and SORT_FIELD is null and DELETED = 0) begin -- then
		exec dbo.spGRIDVIEWS_UpdateSort null, 'SystemLog.ListView', 'DATE_ENTERED', 'desc';
	end -- if;
	-- 08/18/2021 Paul.  Line number should not have any decimal places. 
	if exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'SystemLog.ListView' and DATA_FIELD = 'LINE_NUMBER' and DATA_FORMAT is null and DELETED = 0) begin -- then
		exec dbo.spGRIDVIEWS_COLUMNS_UpdateFormat null, 'AzureSystemLog.ListView', 'LINE_NUMBER', '{0:N}';
	end -- if;
end -- if;
GO

set nocount off;
GO


