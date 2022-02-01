print 'SYSTEM_REST_TABLES admin';
-- delete from SYSTEM_REST_TABLES;
--GO

set nocount on;
GO

-- 06/18/2011 Paul.  SYSTEM_REST_TABLES are nearly identical to SYSTEM_SYNC_TABLES,
-- but the Module tables typically refer to the base view instead of the raw table. 
-- 06/18/2011 Paul.  We do not anticipate a need access to all the system tables via the REST API. 

-- System Tables
-- 04/16/2021 Paul.  ACL_ROLES is used by the react client. 
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'ACL_ROLES'                       , 'vwACL_ROLES'                     , 'ACLRoles'                 , null                       , 0, null, 1, 0, null, 0;
-- 10/24/2011 Paul.  The HTML5 Offline Client needs access to the config table. 
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'CONFIG'                          , 'vwCONFIG_Sync'                   , 'Config'                   , null                       , 0, null, 1, 0, null, 0;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'CURRENCIES'                      , 'vwCURRENCIES'                    , 'Currencies'               , null                       , 0, null, 1, 0, null, 0;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'LANGUAGES'                       , 'vwLANGUAGES'                     , null                       , null                       , 0, null, 1, 0, null, 0;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'TIMEZONES'                       , 'vwTIMEZONES'                     , null                       , null                       , 0, null, 1, 0, null, 0;
-- 08/08/2019 Paul.  React Client needs access to the RulesWizard. 
GO

-- System UI Tables
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'DETAILVIEWS_FIELDS'              , 'vwDETAILVIEWS_FIELDS'            , null                       , null                       , 2, 'DETAIL_NAME', 1, 0, null, 0;
-- 08/31/2011 Paul.  DETAILVIEWS_RELATIONSHIPS does have a module associated with it. 
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'DETAILVIEWS_RELATIONSHIPS'       , 'vwDETAILVIEWS_RELATIONSHIPS'     , 'DetailViewsRelationships' , null                       , 2, 'DETAIL_NAME', 1, 0, null, 0;
if exists(select * from vwMODULES where MODULE_NAME = 'DetailViewsRelationships' and (REST_ENABLED = 0 or REST_ENABLED is null)) begin -- then
	update MODULES
	   set REST_ENABLED         = 1
	     , MODIFIED_USER_ID     = null    
	     , DATE_MODIFIED        =  getdate()           
	     , DATE_MODIFIED_UTC    =  getutcdate()        
	 where MODULE_NAME          = 'DetailViewsRelationships'
	   and DELETED              = 0;
end -- if;
GO

exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'DYNAMIC_BUTTONS'                 , 'vwDYNAMIC_BUTTONS'               , 'DynamicButtons'           , null                       , 1, 'MODULE_NAME', 1, 0, null, 0;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'EDITVIEWS_FIELDS'                , 'vwEDITVIEWS_FIELDS'              , null                       , null                       , 2, 'EDIT_NAME'  , 1, 0, null, 0;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'GRIDVIEWS_COLUMNS'               , 'vwGRIDVIEWS_COLUMNS'             , null                       , null                       , 2, 'GRID_NAME'  , 1, 0, null, 0;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'MODULES'                         , 'vwMODULES'                       , 'Modules'                  , null                       , 1, 'MODULE_NAME', 1, 0, null, 0;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'SHORTCUTS'                       , 'vwSHORTCUTS'                     , 'Shortcuts'                , null                       , 1, 'MODULE_NAME', 1, 0, null, 0;
-- 04/28/2021 Paul.  React needs access to the help text. 
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'TERMINOLOGY_HELP'                , 'vwTERMINOLOGY_HELP'              , 'Terminology'              , null                       , 1, 'MODULE_NAME', 1, 0, null, 0;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'TERMINOLOGY'                     , 'vwTERMINOLOGY'                   , 'Terminology'              , null                       , 3, 'MODULE_NAME', 1, 0, null, 0;
GO

-- User Tables
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'TAB_MENUS'                       , 'vwMODULES_TabMenu_ByUser'        , 'Modules'                  , null                       , 0, null, 1, 1, 'USER_ID'         , 0;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwUSERS_TEAM_MEMBERSHIPS'        , 'vwUSERS_TEAM_MEMBERSHIPS'        , 'Users'                    , 'Teams'                    , 0, null, 0, 0, 'USER_ID', 1, 'USER_ID';
-- 10/14/2020 Paul.  vwTEAM_MEMBERSHIPS_List is needed by the React Client. 
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwTEAM_MEMBERSHIPS_List'         , 'vwTEAM_MEMBERSHIPS_List'         , 'Teams'                    , 'Users'                    , 0, null, 1, 0, null, 1;

-- 12/31/2017 Paul.  We should not sync the USERS view directly as it can contain encrypted passwords. 
-- Use vwUSERS_Sync instead as it filters these fields. 
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'USERS'                           , 'vwUSERS_Sync'                    , 'Users'                    , null                       , 0, null, 1, 0, null, 0;
-- 10/04/2020 Paul.  The React Client needs access to users for assigned to selection. 
-- Do not tie to the users module as a user that cannot access the Users module can still select a user.  
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwUSERS_ASSIGNED_TO_List'        , 'vwUSERS_ASSIGNED_TO_List'        , null                       , null                       , 0, null, 0, 0, null, 0;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwTEAMS_ASSIGNED_TO_List'        , 'vwTEAMS_ASSIGNED_TO_List'        , null                       , null                       , 0, null, 0, 1, 'MEMBERSHIP_USER_ID', 0, null;

exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'IMAGES'                          , 'vwIMAGES'                        , 'Images'                   , null                       , 0, null, 0, 0, null, 0;

-- 09/12/2019 Paul.  Users.ACLRoles is used in the React Client.  USER_ID will be a required field. 
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwUSERS_ACL_ROLES'                 , 'vwUSERS_ACL_ROLES'               , 'Users'                    , null                       , 0, null, 0, 0, 'USER_ID', 1, 'USER_ID';
-- 09/13/2019 Paul.  Users.Logins is used in the React Client.  USER_ID will be a required field. 
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwUSERS_LOGINS'                    , 'vwUSERS_LOGINS'                  , 'Users'                    , null                       , 0, null, 0, 0, 'USER_ID', 1, 'USER_ID';

-- 05/12/2017 Paul.  Add HTML5 Dashboard. 
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'DASHBOARD_APPS'                    , 'vwDASHBOARD_APPS'                , null                       , null                       , 1, 'MODULE_NAME', 1, 0, null, 0;
-- update SYSTEM_REST_TABLES set ASSIGNED_FIELD_NAME = null, IS_ASSIGNED = 0, DATE_MODIFIED = getdate() where TABLE_NAME = 'DASHBOARDS' and ASSIGNED_FIELD_NAME = 'ASSIGNED_USER_ID'
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'DASHBOARDS'                        , 'vwDASHBOARDS'                    , 'Dashboard'                , null                       , 0, null, 0, 0, null, 0;
-- delete from SYSTEM_REST_TABLES where TABLE_NAME = 'DASHBOARDS_PANELS';
-- update SYSTEM_REST_TABLES set ASSIGNED_FIELD_NAME = null, IS_ASSIGNED = 0, DATE_MODIFIED = getdate() where TABLE_NAME = 'DASHBOARDS_PANELS' and ASSIGNED_FIELD_NAME = 'PARENT_ASSIGNED_USER_ID'
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'DASHBOARDS_PANELS'                 , 'vwDASHBOARDS_PANELS'             , 'DashboardPanels'          , null                       , 1, 'MODULE_NAME', 0, 0, null, 0;

-- 05/07/019 Paul.  Allow access to SAVED_SEARCH for the React client. 
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'SAVED_SEARCH'                      , 'vwSAVED_SEARCH'                  , null                       , null                       , 0, 'SEARCH_MODULE', 0, 1, 'ASSIGNED_USER_ID', 0;

-- 09/17/2019 Paul.  Allow access to SystemLog for the React client. 
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'SYSTEM_LOG'                        , 'vwSYSTEM_LOG'                    , 'SystemLog'                , null                       , 0, null, 1, 0, null, 0;
-- 03/10/2021 Paul.  Instead of allowing access to all tables to an admin, require that the table be registerd and admin acces to module. 
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'SYSTEM_SYNC_LOG'                   , 'vwSYSTEM_SYNC_LOG'               , 'SystemSyncLog'            , null                       , 0, null, 1, 0, null, 0;
GO

-- 11/25/2020 Paul.  We need a way to call a generic procedure.  Security is still managed through SYSTEM_REST_TABLES. 
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'spTERMINOLOGY_LIST_Insert'         , 'spTERMINOLOGY_LIST_Insert'       , 'Terminology'              , null                       , 0, null, 1, 0, null, 0, 'LANG LIST_NAME';
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'spTERMINOLOGY_LIST_Delete'         , 'spTERMINOLOGY_LIST_Delete'       , 'Terminology'              , null                       , 0, null, 1, 0, null, 0, 'ID';
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'spTERMINOLOGY_LIST_MoveItem'       , 'spTERMINOLOGY_LIST_MoveItem'     , 'Terminology'              , null                       , 0, null, 1, 0, null, 0, 'LANG LIST_NAME';
-- 02/20/2021 Paul.  Configure Tabs. 
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'spMODULES_TAB_ORDER_MoveItem'      , 'spMODULES_TAB_ORDER_MoveItem'    , 'Modules'                  , null                       , 0, null, 1, 0, null, 0, 'OLD_INDEX NEW_INDEX';
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'spMODULES_TAB_Show'                , 'spMODULES_TAB_Show'              , 'Modules'                  , null                       , 0, null, 1, 0, null, 0, 'ID';
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'spMODULES_TAB_Hide'                , 'spMODULES_TAB_Hide'              , 'Modules'                  , null                       , 0, null, 1, 0, null, 0, 'ID';
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'spMODULES_TAB_ShowMobile'          , 'spMODULES_TAB_ShowMobile'        , 'Modules'                  , null                       , 0, null, 1, 0, null, 0, 'ID';
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'spMODULES_TAB_HideMobile'          , 'spMODULES_TAB_HideMobile'        , 'Modules'                  , null                       , 0, null, 1, 0, null, 0, 'ID';
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'spMODULES_Enable'                  , 'spMODULES_Enable'                , 'Modules'                  , null                       , 0, null, 1, 0, null, 0, 'ID';
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'spMODULES_Disable'                 , 'spMODULES_Disable'               , 'Modules'                  , null                       , 0, null, 1, 0, null, 0, 'ID';
-- 02/21/2021 Paul.  Languages. 
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'spLANGUAGES_Enable'                 , 'spLANGUAGES_Enable'             , 'Languages'                , null                       , 0, null, 1, 0, null, 0, 'NAME';
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'spLANGUAGES_Disable'                , 'spLANGUAGES_Disable'            , 'Languages'                , null                       , 0, null, 1, 0, null, 0, 'NAME';
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'spLANGUAGES_Delete'                 , 'spLANGUAGES_Delete'             , 'Languages'                , null                       , 0, null, 1, 0, null, 0, 'NAME';

-- 01/19/2021 Paul.  System tables need by the React client. 
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwACL_ROLES_USERS'                 , 'vwACL_ROLES_USERS'               , 'ACLRoles'                 , 'Users'                    , 0, null, 1, 0, null, 1, null;
-- 01/29/2021 Paul.  Add EditCustomFields to React client. 
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwFIELDS_META_DATA_List'           , 'vwFIELDS_META_DATA_List'         , 'EditCustomFields'         , null                       , 0, null, 1, 0, null, 0, null;
-- 02/18/2021 Paul.  System tables need by the React client. 
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwMODULES_RenameTabs'              , 'vwMODULES_RenameTabs'            , null                       , null                       , 0, null, 1, 0, null, 0, 'LANG';
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwMODULES_CONFIGURE_TABS'          , 'vwMODULES_CONFIGURE_TABS'        , null                       , null                       , 0, null, 1, 0, null, 0, null;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwSYSTEM_CURRENCY_LOG'             , 'vwSYSTEM_CURRENCY_LOG'           , null                       , null                       , 0, null, 1, 0, null, 0, null;
GO

-- 03/11/2021 Paul.  All system tables will require registration. 
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'SCHEDULERS'                        , 'vwSCHEDULERS'                    , 'Schedulers'               , null                       , 0, null, 1, 0, null, 0, null;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'USERS_LOGINS'                      , 'vwUSERS_LOGINS'                  , 'UserLogins'               , null                       , 0, null, 1, 0, null, 0, null;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'AUDIT_EVENTS'                      , 'vwAUDIT_EVENTS'                  , 'AuditEvents'              , null                       , 0, null, 1, 0, null, 0, null;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'FIELD_VALIDATORS'                  , 'vwFIELD_VALIDATORS'              , 'FieldValidators'          , null                       , 0, null, 1, 0, null, 0, null;
-- 07/06/2021 Paul.  Provide an quick and easy way to enable/disable React client. 
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'spMODULES_UpdateRelativePath'    , 'spMODULES_UpdateRelativePath'    , 'Modules'                  , null                       , 0, null, 1, 0, null, 0, 'MODULE_NAME RELATIVE_PATH';

-- 09/09/2021 Paul.  System tables need by the React client. 
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'spSqlBackupDatabase'               , 'spSqlBackupDatabase'             , 'Administration'           , null                       , 0, null, 1, 0, null, 0, null;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'spSCHEDULERS_UpdateStatus'         , 'spSCHEDULERS_UpdateStatus'       , 'Schedulers'               , null                       , 0, null, 1, 0, null, 0, null;
GO


set nocount off;
GO


