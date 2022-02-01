set nocount on;
GO

if not exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'Users.Export' and DELETED = 0) begin -- then
	print 'GRIDVIEWS_COLUMNS Users.Export';
	exec dbo.spGRIDVIEWS_InsertOnly           'Users.Export'            , 'Users', 'vwUSERS_List';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.Export'            ,  1, 'Users.LBL_LIST_FULL_NAME'                     , 'FULL_NAME'                  , null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.Export'            ,  2, 'Users.LBL_LIST_NAME'                          , 'NAME'                       , null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.Export'            ,  3, 'Users.LBL_LIST_USER_NAME'                     , 'USER_NAME'                  , null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.Export'            ,  4, 'Users.LBL_LIST_FIRST_NAME'                    , 'FIRST_NAME'                 , null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.Export'            ,  5, 'Users.LBL_LIST_LAST_NAME'                     , 'LAST_NAME'                  , null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.Export'            ,  6, 'Users.LBL_LIST_REPORTS_TO_NAME'               , 'REPORTS_TO_NAME'            , null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.Export'            ,  7, 'Users.LBL_LIST_IS_ADMIN'                      , 'IS_ADMIN'                   , null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.Export'            ,  8, 'Users.LBL_LIST_PORTAL_ONLY'                   , 'PORTAL_ONLY'                , null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.Export'            ,  9, 'Users.LBL_LIST_RECEIVE_NOTIFICATIONS'         , 'RECEIVE_NOTIFICATIONS'      , null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.Export'            , 10, 'Users.LBL_LIST_TITLE'                         , 'TITLE'                      , null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.Export'            , 11, 'Users.LBL_LIST_DEPARTMENT'                    , 'DEPARTMENT'                 , null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.Export'            , 12, 'Users.LBL_LIST_PHONE_HOME'                    , 'PHONE_HOME'                 , null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.Export'            , 13, 'Users.LBL_LIST_PHONE_MOBILE'                  , 'PHONE_MOBILE'               , null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.Export'            , 14, 'Users.LBL_LIST_PHONE_WORK'                    , 'PHONE_WORK'                 , null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.Export'            , 15, 'Users.LBL_LIST_PHONE_OTHER'                   , 'PHONE_OTHER'                , null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.Export'            , 16, 'Users.LBL_LIST_PHONE_FAX'                     , 'PHONE_FAX'                  , null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.Export'            , 17, 'Users.LBL_LIST_EMAIL1'                        , 'EMAIL1'                     , null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.Export'            , 18, 'Users.LBL_LIST_EMAIL2'                        , 'EMAIL2'                     , null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.Export'            , 19, 'Users.LBL_LIST_STATUS'                        , 'STATUS'                     , null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.Export'            , 20, 'Users.LBL_LIST_EMPLOYEE_STATUS'               , 'EMPLOYEE_STATUS'            , null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.Export'            , 21, 'Users.LBL_LIST_MESSENGER_TYPE'                , 'MESSENGER_TYPE'             , null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.Export'            , 22, 'Users.LBL_LIST_ADDRESS_STREET'                , 'ADDRESS_STREET'             , null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.Export'            , 23, 'Users.LBL_LIST_ADDRESS_CITY'                  , 'ADDRESS_CITY'               , null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.Export'            , 24, 'Users.LBL_LIST_ADDRESS_STATE'                 , 'ADDRESS_STATE'              , null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.Export'            , 25, 'Users.LBL_LIST_ADDRESS_COUNTRY'               , 'ADDRESS_COUNTRY'            , null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.Export'            , 26, 'Users.LBL_LIST_ADDRESS_POSTALCODE'            , 'ADDRESS_POSTALCODE'         , null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.Export'            , 27, 'Users.LBL_LIST_IS_GROUP'                      , 'IS_GROUP'                   , null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.Export'            , 28, '.LBL_LIST_DATE_ENTERED'                       , 'DATE_ENTERED'               , null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.Export'            , 29, '.LBL_LIST_DATE_MODIFIED'                      , 'DATE_MODIFIED'              , null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.Export'            , 30, 'Users.LBL_LIST_DESCRIPTION'                   , 'DESCRIPTION'                , null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.Export'            , 31, 'Users.LBL_LIST_USER_PREFERENCES'              , 'USER_PREFERENCES'           , null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.Export'            , 32, 'Users.LBL_LIST_DEFAULT_TEAM_NAME'             , 'DEFAULT_TEAM_NAME'          , null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.Export'            , 33, 'Users.LBL_LIST_IS_ADMIN_DELEGATE'             , 'IS_ADMIN_DELEGATE'          , null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.Export'            , 34, '.LBL_LIST_CREATED_BY_NAME'                    , 'CREATED_BY_NAME'            , null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.Export'            , 35, '.LBL_LIST_MODIFIED_BY_NAME'                   , 'MODIFIED_BY_NAME'           , null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.Export'            , 36, 'Users.LBL_LIST_GOOGLEAPPS_SYNC_CONTACTS'      , 'GOOGLEAPPS_SYNC_CONTACTS'   , null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.Export'            , 37, 'Users.LBL_LIST_GOOGLEAPPS_SYNC_CALENDAR'      , 'GOOGLEAPPS_SYNC_CALENDAR'   , null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.Export'            , 38, 'Users.LBL_LIST_GOOGLEAPPS_USERNAME'           , 'GOOGLEAPPS_USERNAME'        , null, null;
end -- if;
GO

set nocount off;
GO


