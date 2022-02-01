set nocount on;
GO

-- 03/27/2019 Paul.  Same as UserLogins.ListView, 
-- 02/24/2021 Paul.  The React client needs a way to determine the default sort, besides NAME asc. 
-- delete from GRIDVIEWS_COLUMNS where GRID_NAME = 'UserLogins.ListView';
if not exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'UserLogins.ListView' and DELETED = 0) begin -- then
	print 'GRIDVIEWS_COLUMNS UserLogins.ListView';
	exec dbo.spGRIDVIEWS_InsertOnly           'UserLogins.ListView', 'Users', 'vwUSERS_LOGINS', 'DATE_MODIFIED', 'desc';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'UserLogins.ListView'    , 0, 'Users.LBL_LIST_NAME'                  , 'FULL_NAME'       , 'FULL_NAME'       , '10%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'UserLogins.ListView'    , 1, 'Users.LBL_LIST_USER_NAME'             , 'USER_NAME'       , 'USER_NAME'       , '10%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBoundDate 'UserLogins.ListView'    , 2, 'Users.LBL_LIST_LOGIN_DATE'            , 'LOGIN_DATE'      , 'LOGIN_DATE'      , '15%', 'DateTime';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBoundDate 'UserLogins.ListView'    , 3, 'Users.LBL_LIST_LOGOUT_DATE'           , 'LOGOUT_DATE'     , 'LOGOUT_DATE'     , '15%', 'DateTime';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBoundList 'UserLogins.ListView'    , 4, 'Users.LBL_LIST_LOGIN_STATUS'          , 'LOGIN_STATUS'    , 'LOGIN_STATUS'    , '10%', 'login_status_dom';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBoundList 'UserLogins.ListView'    , 5, 'Users.LBL_LIST_LOGIN_TYPE'            , 'LOGIN_TYPE'      , 'LOGIN_TYPE'      , '10%', 'login_type_dom';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'UserLogins.ListView'    , 6, 'Users.LBL_LIST_REMOTE_HOST'           , 'REMOTE_HOST'     , 'REMOTE_HOST'     , '5%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'UserLogins.ListView'    , 7, 'Users.LBL_LIST_TARGET'                , 'TARGET'          , 'TARGET'          , '5%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'UserLogins.ListView'    , 8, 'Users.LBL_LIST_ASPNET_SESSIONID'      , 'ASPNET_SESSIONID', 'ASPNET_SESSIONID', '10%';
end else begin
	-- 02/24/2021 Paul.  The React client needs a way to determine the default sort, besides NAME asc. 
	if exists(select * from GRIDVIEWS where NAME = 'UserLogins.ListView' and SORT_FIELD is null and DELETED = 0) begin -- then
		exec dbo.spGRIDVIEWS_UpdateSort null, 'UserLogins.ListView', 'DATE_MODIFIED', 'desc';
	end -- if;
end -- if;
GO

set nocount off;
GO


