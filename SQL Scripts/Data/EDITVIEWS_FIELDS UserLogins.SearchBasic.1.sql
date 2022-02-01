set nocount on;
GO

-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'UserLogins.SearchBasic';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'UserLogins.SearchBasic' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS UserLogins.SearchBasic';
	exec dbo.spEDITVIEWS_InsertOnly             'UserLogins.SearchBasic'  , 'Users', 'vwUSERS_LOGINS', '11%', '22%', 3;
	exec dbo.spEDITVIEWS_FIELDS_InsBound        'UserLogins.SearchBasic'  ,  0, 'Users.LBL_NAME'                         , 'FULL_NAME'                  , 0, null,  30, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsAutoComplete 'UserLogins.SearchBasic'  ,  1, 'Users.LBL_USER_NAME'                    , 'USER_NAME'                  , 0, null,  30, 25, 'Users', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank        'UserLogins.SearchBasic'  ,  2, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound        'UserLogins.SearchBasic'  ,  3, 'Users.LBL_REMOTE_HOST'                  , 'REMOTE_HOST'                , 0, null,  75, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound        'UserLogins.SearchBasic'  ,  4, 'Users.LBL_TARGET'                       , 'TARGET'                     , 0, null,  75, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound        'UserLogins.SearchBasic'  ,  5, 'Users.LBL_ASPNET_SESSIONID'             , 'ASPNET_SESSIONID'           , 0, null,  75, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl      'UserLogins.SearchBasic'  ,  6, 'Users.LBL_LOGIN_DATE'                   , 'LOGIN_DATE'                 , 0, null, 'DateRange'          , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl      'UserLogins.SearchBasic'  ,  7, 'Users.LBL_LOGOUT_DATE'                  , 'LOGOUT_DATE'                , 0, null, 'DateRange'          , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList    'UserLogins.SearchBasic'  ,  8, 'Users.LBL_LOGIN_STATUS'                 , 'LOGIN_STATUS'               , 0, null, 'login_status_dom'   , null, 3;
end else begin
	exec dbo.spEDITVIEWS_FIELDS_CnvAutoComplete 'UserLogins.SearchBasic'  ,  1, 'Users.LBL_USER_NAME'                    , 'USER_NAME'                  , 0, null,  30, 25, 'Users', null;
end -- if;
GO

set nocount off;
GO


