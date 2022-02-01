set nocount on;
GO

-- 04/02/2021 Paul.  Add support for UserLogins. 
-- delete from GRIDVIEWS_COLUMNS where GRID_NAME = 'UserLogins.Export';
if not exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'UserLogins.Export' and DELETED = 0) begin -- then
	print 'GRIDVIEWS_COLUMNS UserLogins.Export';
	exec dbo.spGRIDVIEWS_InsertOnly           'UserLogins.Export', 'Users', 'vwUSERS_LOGINS', 'DATE_MODIFIED', 'desc';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'UserLogins.Export', 0, 'Users.LBL_LIST_NAME'                  , 'NAME'            , null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'UserLogins.Export', 1, 'Users.LBL_LIST_USER_NAME'             , 'USER_NAME'       , null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'UserLogins.Export', 2, 'Users.LBL_LIST_LOGIN_DATE'            , 'LOGIN_DATE'      , null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'UserLogins.Export', 3, 'Users.LBL_LIST_LOGOUT_DATE'           , 'LOGOUT_DATE'     , null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'UserLogins.Export', 4, 'Users.LBL_LIST_LOGIN_STATUS'          , 'LOGIN_STATUS'    , null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'UserLogins.Export', 5, 'Users.LBL_LIST_LOGIN_TYPE'            , 'LOGIN_TYPE'      , null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'UserLogins.Export', 6, 'Users.LBL_LIST_REMOTE_HOST'           , 'REMOTE_HOST'     , null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'UserLogins.Export', 7, 'Users.LBL_LIST_TARGET'                , 'TARGET'          , null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'UserLogins.Export', 8, 'Users.LBL_LIST_ASPNET_SESSIONID'      , 'ASPNET_SESSIONID', null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'UserLogins.Export', 9, 'Users.LBL_LIST_IS_ADMIN'              , 'IS_ADMIN'        , null, null;
end -- if;
GO

set nocount off;
GO


