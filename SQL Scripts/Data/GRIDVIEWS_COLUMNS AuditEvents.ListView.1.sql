set nocount on;
GO

-- 01/20/2010 Paul.  Add ability to search the new Audit Events table. 
-- 03/28/2019 Paul.  Move AuditEvents.ListView to default file for Community edition. 
-- 03/28/2019 Paul.  Convert to DateTime field. 
-- 02/24/2021 Paul.  The React client needs a way to determine the default sort, besides NAME asc. 
-- delete from GRIDVIEWS_COLUMNS where GRID_NAME = 'AuditEvents.ListView';
if not exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'AuditEvents.ListView' and DELETED = 0) begin -- then
	print 'GRIDVIEWS_COLUMNS AuditEvents.ListView';
	exec dbo.spGRIDVIEWS_InsertOnly           'AuditEvents.ListView', 'AuditEvents', 'vwAUDIT_EVENTS', 'DATE_MODIFIED', 'desc';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'AuditEvents.ListView'       , 0, 'Users.LBL_LIST_NAME'                      , 'FULL_NAME'            , 'FULL_NAME'       , '15%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'AuditEvents.ListView'       , 1, 'Users.LBL_LIST_USER_NAME'                 , 'USER_NAME'            , 'USER_NAME'       , '15%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBoundDate 'AuditEvents.ListView'       , 2, '.LBL_LIST_DATE_MODIFIED'                  , 'DATE_MODIFIED'        , 'DATE_MODIFIED'   , '15%', 'DateTime';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBoundList 'AuditEvents.ListView'       , 4, 'Audit.LBL_LIST_AUDIT_ACTION'              , 'AUDIT_ACTION'         , 'AUDIT_ACTION'    , '10%', 'audit_action_dom';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBoundList 'AuditEvents.ListView'       , 5, 'Audit.LBL_LIST_MODULE_NAME'               , 'MODULE_NAME'          , 'MODULE_NAME'     , '15%', 'Modules';
	exec dbo.spGRIDVIEWS_COLUMNS_InsHyperLink 'AuditEvents.ListView'       , 6, 'Audit.LBL_LIST_AUDIT_ITEM'                , 'AUDIT_PARENT_ID'      , 'AUDIT_PARENT_ID' , '30%', 'listViewTdLinkS1', 'MODULE_FOLDER AUDIT_PARENT_ID', '~/{0}/view.aspx?id={1}', null, null, null;
end else begin
	-- 03/28/2019 Paul.  Convert to DateTime field. 
	if exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'AuditEvents.ListView' and DATA_FIELD = 'DATE_MODIFIED' and DATA_FORMAT is null and DELETED = 0) begin -- then
		update GRIDVIEWS_COLUMNS
		   set DELETED           = 1
		     , DATE_MODIFIED     = getdate()
		     , DATE_MODIFIED_UTC = getutcdate()
		 where GRID_NAME         = 'AuditEvents.ListView'
		   and DATA_FIELD        = 'DATE_MODIFIED'
		   and DATA_FORMAT       is null
		   and DELETED           = 0;
		exec dbo.spGRIDVIEWS_COLUMNS_InsBoundDate 'AuditEvents.ListView'       , 2, '.LBL_LIST_DATE_MODIFIED'                  , 'DATE_MODIFIED'        , 'DATE_MODIFIED'   , '15%', 'DateTime';
	end -- if;
	-- 02/24/2021 Paul.  The React client needs a way to determine the default sort, besides NAME asc. 
	if exists(select * from GRIDVIEWS where NAME = 'AuditEvents.ListView' and SORT_FIELD is null and DELETED = 0) begin -- then
		exec dbo.spGRIDVIEWS_UpdateSort null, 'AuditEvents.ListView', 'DATE_MODIFIED', 'desc';
	end -- if;
end -- if;
GO

set nocount off;
GO


