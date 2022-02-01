-- 10/14/2020 Paul.  Teams.Users for the React Client. 
-- delete from GRIDVIEWS_COLUMNS where GRID_NAME = 'Teams.Users';
if not exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'Teams.Users' and DELETED = 0) begin -- then
	print 'GRIDVIEWS_COLUMNS Teams.Users';
	exec dbo.spGRIDVIEWS_InsertOnly           'Teams.Users', 'Users', 'vwTEAM_MEMBERSHIPS_List';
	exec dbo.spGRIDVIEWS_COLUMNS_InsHyperLink 'Teams.Users'              , 0, 'Users.LBL_LIST_NAME'                        , 'FULL_NAME'              , 'FULL_NAME'              , '18%', 'listViewTdLinkS1', 'USER_ID', '~/Users/view.aspx?id={0}', null, 'Users', null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Teams.Users'              , 1, 'Users.LBL_LIST_USER_NAME'                   , 'USER_NAME'              , 'USER_NAME'              , '18%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBoundList 'Teams.Users'              , 2, 'Teams.LBL_LIST_MEMBERSHIP'                  , 'EXPLICIT_ASSIGN'        , 'EXPLICIT_ASSIGN'        , '18%', 'team_explicit_assign';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Teams.Users'              , 3, 'Users.LBL_LIST_EMAIL'                       , 'EMAIL1'                 , 'EMAIL1'                 , '18%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Teams.Users'              , 4, '.LBL_LIST_PHONE'                            , 'PHONE_WORK'             , 'PHONE_WORK'             , '15%';
end -- if;
GO

