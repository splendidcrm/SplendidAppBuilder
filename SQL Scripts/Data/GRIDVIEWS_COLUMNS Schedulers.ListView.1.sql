set nocount on;
GO

-- delete from GRIDVIEWS_COLUMNS where GRID_NAME = 'Schedulers.ListView';
if not exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'Schedulers.ListView' and DELETED = 0) begin -- then
	print 'GRIDVIEWS_COLUMNS Schedulers.ListView';
	exec dbo.spGRIDVIEWS_InsertOnly           'Schedulers.ListView', 'Schedulers', 'vwSCHEDULERS';
	exec dbo.spGRIDVIEWS_COLUMNS_InsHyperLink 'Schedulers.ListView'    ,  2, 'Schedulers.LBL_SCHEDULER'            , 'NAME'            , 'NAME'            , '35%', 'listViewTdLinkS1', 'ID', '~/Administration/Schedulers/view.aspx?id={0}', null, 'Schedulers', null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Schedulers.ListView'    ,  3, 'Schedulers.LBL_INTERVAL'             , 'JOB_INTERVAL'    , 'JOB_INTERVAL'    , '15%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBoundDate 'Schedulers.ListView'    ,  4, 'Schedulers.LBL_DATE_TIME_START'      , 'DATE_TIME_START' , 'DATE_TIME_START' , '15%', 'Date';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBoundDate 'Schedulers.ListView'    ,  5, 'Schedulers.LBL_DATE_TIME_END'        , 'DATE_TIME_END'   , 'DATE_TIME_END'   , '15%', 'Date';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Schedulers.ListView'    ,  6, 'Schedulers.LBL_LIST_STATUS'          , 'STATUS'          , 'STATUS'          , '5%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBoundDate 'Schedulers.ListView'    ,  7, 'Schedulers.LBL_LAST_RUN'             , 'LAST_RUN'        , 'LAST_RUN'        , '15%', 'DateTime';
end -- if;
GO

set nocount off;
GO


