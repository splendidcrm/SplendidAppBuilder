-- 09/12/2019 Paul.  Users.Teams for the React Client. 
-- delete from GRIDVIEWS_COLUMNS where GRID_NAME = 'Users.Teams'
if not exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'Users.Teams' and DELETED = 0) begin -- then
	print 'GRIDVIEWS_COLUMNS Users.Teams';
	exec dbo.spGRIDVIEWS_InsertOnly           'Users.Teams', 'Teams', 'vwUSERS_TEAM_MEMBERSHIPS';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.Teams'              , 1, 'Teams.LBL_LIST_NAME'                        , 'TEAM_NAME'              , 'TEAM_NAME'                     , '30%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.Teams'              , 2, 'Teams.LBL_LIST_DESCRIPTION'                 , 'DESCRIPTION'            , 'DESCRIPTION'                   , '60%';
end -- if;
GO

