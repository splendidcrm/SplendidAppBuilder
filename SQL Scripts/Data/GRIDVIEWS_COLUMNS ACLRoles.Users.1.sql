set nocount on;
GO

-- 02/11/2021 Paul.  ACLRoles.Users for the React Client. 
-- delete from GRIDVIEWS_COLUMNS where GRID_NAME = 'ACLRoles.Users';
if not exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'ACLRoles.Users' and DELETED = 0) begin -- then
	print 'GRIDVIEWS_COLUMNS ACLRoles.Users';
	exec dbo.spGRIDVIEWS_InsertOnly           'ACLRoles.Users', 'ACLRoles', 'vwACL_ROLES_USERS';
	exec dbo.spGRIDVIEWS_COLUMNS_InsHyperLink 'ACLRoles.Users'           , 0, 'Users.LBL_LIST_NAME'                        , 'FULL_NAME'              , 'FULL_NAME'              , '22%', 'listViewTdLinkS1', 'USER_ID', '~/Users/view.aspx?id={0}', null, 'Users', null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'ACLRoles.Users'           , 1, 'Users.LBL_LIST_USER_NAME'                   , 'USER_NAME'              , 'USER_NAME'              , '22%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'ACLRoles.Users'           , 2, 'Users.LBL_LIST_EMAIL'                       , 'EMAIL1'                 , 'EMAIL1'                 , '22%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'ACLRoles.Users'           , 3, '.LBL_LIST_PHONE'                            , 'PHONE_WORK'             , 'PHONE_WORK'             , '22%';
end -- if;
GO

set nocount off;
GO


