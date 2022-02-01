-- 04/12/2016 Paul.  Add parent team and custom fields. 
-- 04/28/2016 Paul.  Rename parent to PARENT_ID. 
-- delete from GRIDVIEWS_COLUMNS where GRID_NAME = 'Teams.ListView'
if not exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'Teams.ListView' and DELETED = 0) begin -- then
	print 'GRIDVIEWS_COLUMNS Teams.ListView';
	exec dbo.spGRIDVIEWS_InsertOnly           'Teams.ListView', 'Teams', 'vwTEAMS_List';
	exec dbo.spGRIDVIEWS_COLUMNS_InsHyperLink 'Teams.ListView'             , 1, 'Teams.LBL_LIST_NAME'                      , 'NAME'            , 'NAME'            , '30%', 'listViewTdLinkS1', 'ID', '~/Administration/Teams/view.aspx?id={0}', null, 'Teams', null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Teams.ListView'             , 2, 'Teams.LBL_LIST_DESCRIPTION'               , 'DESCRIPTION'     , 'DESCRIPTION'     , '45%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Teams.ListView'             , 3, 'Teams.LBL_LIST_PARENT_NAME'               , 'PARENT_NAME'     , 'PARENT_NAME'     , '20%';
end else begin
	exec dbo.spGRIDVIEWS_COLUMNS_ReserveIndex null, 'Teams.ListView', 1;
	-- 04/12/2016 Paul.  Add parent team and custom fields. 
	-- 04/28/2016 Paul.  Rename parent to PARENT_ID. 
	if exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'Teams.ListView' and DATA_FIELD = 'PARENT_TEAM_NAME' and DELETED = 0) begin -- then
		update GRIDVIEWS_COLUMNS
		   set DATA_FIELD       = 'PARENT_NAME'
		     , DATE_MODIFIED    = getdate()
		     , DATE_MODIFIED_UTC= getutcdate()
		     , MODIFIED_USER_ID = null
		 where GRID_NAME        = 'Teams.ListView'
		   and DATA_FIELD       = 'PARENT_TEAM_NAME'
		   and DELETED          = 0;
	end -- if;
	-- 02/22/2017 Paul.  Fix filter. 
	if not exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'Teams.ListView' and DATA_FIELD = 'PARENT_NAME' and DELETED = 0) begin -- then
		exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Teams.ListView'             , 3, 'Teams.LBL_LIST_PARENT_NAME'               , 'PARENT_NAME'     , 'PARENT_NAME'     , '20%';
	end -- if;
end -- if;
GO

