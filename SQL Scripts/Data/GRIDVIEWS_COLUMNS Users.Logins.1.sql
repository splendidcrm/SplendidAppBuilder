set nocount on;
GO

-- 01/05/2021 Paul.  Include IS_ADMIN for the React Client. 
-- 01/05/2021 Paul.  Don't wrap name and dates. 
-- delete from GRIDVIEWS_COLUMNS where GRID_NAME = 'Users.Logins';
if not exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'Users.Logins' and DELETED = 0) begin -- then
	print 'GRIDVIEWS_COLUMNS Users.Logins';
	exec dbo.spGRIDVIEWS_InsertOnly           'Users.Logins', 'Users', 'vwUSERS_LOGINS';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.Logins'                    , 0, 'Users.LBL_LIST_NAME'                      , 'FULL_NAME'              , 'FULL_NAME'              , '10%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.Logins'                    , 1, 'Users.LBL_LIST_USER_NAME'                 , 'USER_NAME'              , 'USER_NAME'              , '10%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.Logins'                    , 2, 'Users.LBL_LIST_LOGIN_DATE'                , 'LOGIN_DATE'             , 'LOGIN_DATE'             , '15%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.Logins'                    , 3, 'Users.LBL_LIST_LOGOUT_DATE'               , 'LOGOUT_DATE'            , 'LOGOUT_DATE'            , '15%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBoundList 'Users.Logins'                    , 4, 'Users.LBL_LIST_LOGIN_STATUS'              , 'LOGIN_STATUS'           , 'LOGIN_STATUS'           , '10%', 'login_status_dom';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBoundList 'Users.Logins'                    , 5, 'Users.LBL_LIST_LOGIN_TYPE'                , 'LOGIN_TYPE'             , 'LOGIN_TYPE'             , '10%', 'login_type_dom';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.Logins'                    , 6, 'Users.LBL_LIST_REMOTE_HOST'               , 'REMOTE_HOST'            , 'REMOTE_HOST'            , '5%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.Logins'                    , 7, 'Users.LBL_LIST_TARGET'                    , 'TARGET'                 , 'TARGET'                 , '5%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.Logins'                    , 8, 'Users.LBL_LIST_ASPNET_SESSIONID'          , 'ASPNET_SESSIONID'       , 'ASPNET_SESSIONID'       , '10%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBoundDate 'Users.Logins'                    , 9, 'Users.LBL_LIST_ADMIN'                     , 'IS_ADMIN'               , 'IS_ADMIN'               , '5%', 'CheckBox';
	exec dbo.spGRIDVIEWS_COLUMNS_UpdateStyle  null, 'Users.Logins'              , 0, null, null, null, null, 0;
	exec dbo.spGRIDVIEWS_COLUMNS_UpdateStyle  null, 'Users.Logins'              , 2, null, null, null, null, 0;
	exec dbo.spGRIDVIEWS_COLUMNS_UpdateStyle  null, 'Users.Logins'              , 3, null, null, null, null, 0;
end else begin
	-- 01/05/2021 Paul.  Include IS_ADMIN for the React Client. 
	if not exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'Users.Logins' and DATA_FIELD = 'IS_ADMIN' and COLUMN_TYPE = 'BoundColumn' and DELETED = 0) begin -- then
		exec dbo.spGRIDVIEWS_COLUMNS_InsBoundDate 'Users.Logins'                    , 9, 'Users.LBL_LIST_ADMIN'                     , 'IS_ADMIN'               , 'IS_ADMIN'               , '5%', 'CheckBox';
		exec dbo.spGRIDVIEWS_COLUMNS_UpdateStyle  null, 'Users.Logins'              , 0, null, null, null, null, 0;
		exec dbo.spGRIDVIEWS_COLUMNS_UpdateStyle  null, 'Users.Logins'              , 2, null, null, null, null, 0;
		exec dbo.spGRIDVIEWS_COLUMNS_UpdateStyle  null, 'Users.Logins'              , 3, null, null, null, null, 0;
	end -- if;
end -- if;
GO

set nocount off;
GO


