set nocount on;
GO

-- 07/11/2018 Paul.  Increase index to add space for checkboxes. 
-- select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'Users.LoginView' order by COLUMN_INDEX
-- delete from GRIDVIEWS_COLUMNS where GRID_NAME = 'Users.LoginView';
if not exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'Users.LoginView' and DELETED = 0) begin -- then
	print 'GRIDVIEWS_COLUMNS Users.LoginView';
	exec dbo.spGRIDVIEWS_InsertOnly           'Users.LoginView', 'Users', 'vwUSERS_LOGINS';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.LoginView'            , 1, 'Users.LBL_LIST_NAME'                      , 'FULL_NAME'       , 'FULL_NAME'       , '10%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.LoginView'            , 2, 'Users.LBL_LIST_USER_NAME'                 , 'USER_NAME'       , 'USER_NAME'       , '10%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.LoginView'            , 3, 'Users.LBL_LIST_LOGIN_DATE'                , 'LOGIN_DATE'      , 'LOGIN_DATE'      , '15%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.LoginView'            , 4, 'Users.LBL_LIST_LOGOUT_DATE'               , 'LOGOUT_DATE'     , 'LOGOUT_DATE'     , '15%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBoundList 'Users.LoginView'            , 5, 'Users.LBL_LIST_LOGIN_STATUS'              , 'LOGIN_STATUS'    , 'LOGIN_STATUS'    , '10%', 'login_status_dom';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBoundList 'Users.LoginView'            , 6, 'Users.LBL_LIST_LOGIN_TYPE'                , 'LOGIN_TYPE'      , 'LOGIN_TYPE'      , '10%', 'login_type_dom';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.LoginView'            , 7, 'Users.LBL_LIST_REMOTE_HOST'               , 'REMOTE_HOST'     , 'REMOTE_HOST'     , '5%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.LoginView'            , 8, 'Users.LBL_LIST_TARGET'                    , 'TARGET'          , 'TARGET'          , '5%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.LoginView'            , 9, 'Users.LBL_LIST_ASPNET_SESSIONID'          , 'ASPNET_SESSIONID', 'ASPNET_SESSIONID', '10%';
end else begin
	-- 07/11/2018 Paul.  Increase index to add space for checkboxes. 
	if exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'Users.LoginView' and COLUMN_INDEX = 0 and DELETED = 0) begin -- then
		print 'GRIDVIEWS_COLUMNS Users.LoginView: Shift indexes to make space for checkboxes.';
		update GRIDVIEWS_COLUMNS
		   set COLUMN_INDEX      = COLUMN_INDEX + 1
		     , DATE_MODIFIED     = getdate()
		     , DATE_MODIFIED_UTC = getutcdate()
		 where GRID_NAME         = 'Users.LoginView'
		   and DELETED           = 0;
	end -- if;
end -- if;
GO

set nocount off;
GO


