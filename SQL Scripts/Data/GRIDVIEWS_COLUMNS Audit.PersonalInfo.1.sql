set nocount on;
GO

-- 08/28/2019 Paul.  React Client needs an audit layout. 
-- delete from GRIDVIEWS_COLUMNS where GRID_NAME = 'Audit.PersonalInfo';
if not exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'Audit.PersonalInfo' and DELETED = 0) begin -- then
	print 'GRIDVIEWS_COLUMNS Audit.PersonalInfo';
	exec dbo.spGRIDVIEWS_InsertOnly           'Audit.PersonalInfo', 'Audit', 'vwAUDIT_VIEW';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Audit.PersonalInfo', 0, 'Audit.LBL_LIST_FIELD'        , 'FIELD_NAME'          , 'FIELD_NAME'     , '20%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Audit.PersonalInfo', 1, 'Audit.LBL_LIST_VALUE'        , 'VALUE'               , 'VALUE'          , '45%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Audit.PersonalInfo', 2, 'Audit.LBL_LIST_LEAD_SOURCE'  , 'LEAD_SOURCE'         , 'LEAD_SOURCE'    , '20%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBoundDate 'Audit.PersonalInfo', 3, 'Audit.LBL_LIST_LAST_UPDATED' , 'LAST_UPDATED'        , 'LAST_UPDATED'   , '15%', 'DateTime';
	exec dbo.spGRIDVIEWS_COLUMNS_UpdateStyle  null, 'Audit.PersonalInfo', 3, null, null, null, null, 0;
end -- if;
GO

set nocount off;
GO


