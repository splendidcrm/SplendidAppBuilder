set nocount on;
GO

-- 08/20/2019 Paul.  React Client needs an audit layout. 
-- delete from GRIDVIEWS_COLUMNS where GRID_NAME = 'Audit.ListView';
if not exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'Audit.ListView' and DELETED = 0) begin -- then
	print 'GRIDVIEWS_COLUMNS Audit.ListView';
	exec dbo.spGRIDVIEWS_InsertOnly           'Audit.ListView', 'Audit', 'vwAUDIT_VIEW';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Audit.ListView', 0, 'Audit.LBL_FIELD_NAME'            , 'FIELD_NAME'          , 'FIELD_NAME'     , '5%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Audit.ListView', 1, 'Audit.LBL_OLD_NAME'              , 'BEFORE_VALUE'        , 'BEFORE_VALUE'   , '40%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Audit.ListView', 2, 'Audit.LBL_NEW_VALUE'             , 'AFTER_VALUE'         , 'AFTER_VALUE'    , '40%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Audit.ListView', 3, 'Audit.LBL_CHANGED_BY'            , 'CREATED_BY'          , 'CREATED_BY'     , '5%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBoundDate 'Audit.ListView', 4, 'Audit.LBL_LIST_DATE'             , 'DATE_CREATED'        , 'DATE_CREATED'   , '10%', 'DateTime';
	exec dbo.spGRIDVIEWS_COLUMNS_UpdateStyle  null, 'Audit.ListView', 0, null, null, null, null, 0;
	exec dbo.spGRIDVIEWS_COLUMNS_UpdateStyle  null, 'Audit.ListView', 4, null, null, null, null, 0;
end -- if;
GO

set nocount off;
GO


