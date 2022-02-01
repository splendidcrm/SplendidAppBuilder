print 'MODULES admin';
GO

set nocount on;
GO

-- 05/02/2006 Paul.  Add TABLE_NAME as direct table queries are required by SOAP and we need a mapping. 
-- 05/20/2006 Paul.  Add REPORT_ENABLED if the module can be the basis of a report. ACL rules will still apply. 
-- 10/06/2006 Paul.  Add IMPORT_ENABLED if the module can allow importing. 
-- 04/11/2007 Paul.  Since we are using InsertOnly procedures, we don't need all the if exists filters. 
-- 02/09/2008 Paul.  Move maintenance code to separate file. 
-- 08/22/2008 Paul.  Move professional modules to a separate file. 
-- 09/17/2008 Paul.  Enable Mobile for the core modules (Accounts, Contacts, Leads, Opportunities, Cases, Bugs, Calls, Meetings).
-- 11/27/2008 Paul.  Re-arrange the tabs to match the order in SugarCRM 5.1b.
-- 01/13/2010 Paul.  Set default for MASS_UPDATE_ENABLED. 
-- 04/01/2010 Paul.  Add Exchange Sync flag. 
-- 05/02/2010 Paul.  Add defaults for Exchange Folders and Exchange Create Parent. 
-- 08/01/2010 Paul.  Reorder to match the latest Sugar release. 
-- 08/02/2010 Paul.  Sugar Activities is now the Calendar.  We will keep the old Calendar name for now. 
-- 09/12/2011 Paul.  REST_ENABLED provides a way to enable/disable a module in the REST API. 

-- 06/16/2017 Paul.  New home page is rendered using javascript. 
-- 05/30/2020 Paul.  Default home is now the React Client.  
exec dbo.spMODULES_InsertOnly null, 'Home'                  , '.moduleList.Home'                     , '~/React/Home/'                      , 1, 1,  1, 0, 0, 0, 0, 0, null             , 0, 0, 0, 0, 0, 0;
-- 01/11/2015 Paul.  Show the dashboard as part of new HTML5 version 9. 
-- 06/01/2017 Paul.  New dashboard is rendered using javascript. 
-- 12/08/2021 Paul.  Disable dashboard tab on SplendidApp. 
exec dbo.spMODULES_InsertOnly null, 'Dashboard'             , '.moduleList.Dashboard'                , '~/Dashboard/html5/'                 , 1, 0,  0, 0, 0, 0, 0, 0, 'DASHBOARDS'     , 0, 0, 0, 0, 0, 0;
-- 06/02/2017 Paul.  DashboardPanels is required for REST API. 
exec dbo.spMODULES_InsertOnly null, 'DashboardPanels'       , '.moduleList.DashboardPanels'          , '~/Dashboard/DashboardPanels/'       , 1, 0,  0, 0, 0, 0, 0, 0, 'DASHBOARDS_PANELS', 0, 0, 0, 0, 0, 0;
exec dbo.spMODULES_InsertOnly null, 'Administration'        , '.moduleList.Administration'           , '~/Administration/'                  , 1, 0,  0, 0, 0, 0, 0, 1, null             , 0, 0, 0, 0, 0, 0;
-- 03/09/2010 Paul.  Add Languages so that admin roles can be applied. 
exec dbo.spMODULES_InsertOnly null, 'Languages'             , 'Administration.LBL_MANAGE_LANGUAGES'  , '~/Administration/Languages/'        , 1, 0,  0, 0, 0, 0, 0, 1, 'LANGUAGES'      , 0, 0, 0, 0, 0, 0;
exec dbo.spMODULES_InsertOnly null, 'AuditEvents'           , 'Administration.LBL_AUDIT_EVENTS_TITLE', '~/Administration/AuditEvents/'      , 1, 0,  0, 0, 0, 0, 0, 1, 'AUDIT_EVENTS'   , 0, 0, 0, 0, 0, 0;
exec dbo.spMODULES_InsertOnly null, 'Dropdown'              , '.moduleList.Dropdown'                 , '~/Administration/Dropdown/'         , 1, 0,  0, 0, 0, 0, 0, 1, null             , 0, 0, 0, 0, 0, 0;
-- 03/03/2010 Paul.  Add the Config module so that shortcuts can be displayed. 
exec dbo.spMODULES_InsertOnly null, 'Config'                , '.moduleList.Config'                   , '~/Administration/Config/'           , 1, 0,  0, 0, 0, 0, 0, 1, 'CONFIG'         , 0, 0, 0, 0, 0, 0;
exec dbo.spMODULES_InsertOnly null, 'Users'                 , '.moduleList.Users'                    , '~/Users/'                           , 1, 0,  0, 0, 1, 1, 0, 1, 'USERS'          , 0, 1, 0, 0, 0, 1;
exec dbo.spMODULES_InsertOnly null, 'Import'                , '.moduleList.Import'                   , '~/Import/'                          , 1, 0,  0, 0, 0, 0, 0, 1, null             , 0, 0, 0, 0, 0, 0;
exec dbo.spMODULES_InsertOnly null, 'EditCustomFields'      , '.moduleList.EditCustomFields'         , '~/Administration/EditCustomFields/' , 1, 0,  0, 0, 0, 0, 0, 1, null             , 0, 0, 0, 0, 0, 0;

-- 02/18/2016 Paul.  Point to new layout editor. 
exec dbo.spMODULES_InsertOnly null, 'DynamicLayout'         , '.moduleList.DynamicLayout'            , '~/Administration/DynamicLayout/html5/', 1, 0,  0, 0, 0, 0, 0, 1, null             , 0, 0, 0, 0, 0, 0;
if exists (select * from MODULES where MODULE_NAME = 'DynamicLayout' and RELATIVE_PATH = '~/Administration/DynamicLayout/' and DELETED = 0) begin -- then
	print 'MODULES: Enable new DynamicLayout editor. ';
	update MODULES
	   set RELATIVE_PATH       = '~/Administration/DynamicLayout/html5/'
	     , DATE_MODIFIED       = getdate()
	     , DATE_MODIFIED_UTC   = getutcdate()
	     , MODIFIED_USER_ID    = null
	 where MODULE_NAME         = 'DynamicLayout'
	   and RELATIVE_PATH       = '~/Administration/DynamicLayout/'
	   and DELETED             = 0;
end -- if;
GO

exec dbo.spMODULES_InsertOnly null, 'Terminology'           , '.moduleList.Terminology'              , '~/Administration/Terminology/'      , 1, 0,  0, 0, 0, 0, 0, 1, 'TERMINOLOGY'    , 0, 0, 0, 0, 0, 0;
-- 02/20/2021 Paul.  React client relies upon MASS_UPDATE_ENABLED
if exists(select * from MODULES where MODULE_NAME = 'Terminology' and MASS_UPDATE_ENABLED is null) begin -- then
	update MODULES
	   set MASS_UPDATE_ENABLED = 1
	     , DATE_MODIFIED       = getdate()
	     , DATE_MODIFIED_UTC   = getutcdate()
	     , MODIFIED_USER_ID    = null
	 where MODULE_NAME         = 'Terminology'
	   and MASS_UPDATE_ENABLED is null;
end -- if;

-- 04/22/2006 Paul.  Add ACLRoles as a module.  Set the CUSTOM_ENABLED flag. 
-- 05/26/2007 Paul.  There is no compelling reason to allow ACLRoles to be customized. 
exec dbo.spMODULES_InsertOnly null, 'ACLRoles'              , '.moduleList.ACLRoles'                 , '~/Administration/ACLRoles/'         , 1, 0,  0, 0, 0, 0, 0, 1, 'ACL_ROLES'      , 0, 0, 0, 0, 0, 0;
-- 10/25/2006 Paul.  Create the Help module so that access rights can be defined. 
exec dbo.spMODULES_InsertOnly null, 'Help'                  , '.moduleList.Help'                     , '~/Help/'                            , 1, 0,  0, 0, 0, 0, 0, 1, null             , 0, 0, 0, 0, 0, 0;
-- 12/14/2007 Paul.  Need to a a module record for Shortcuts so that its own shortcuts will appear. 
-- 07/24/2008 Paul.  Admin modules are not typically reported on.
exec dbo.spMODULES_InsertOnly null, 'Shortcuts'             , '.moduleList.Shortcuts'                , '~/Administration/Shortcuts/'        , 1, 0,  0, 0, 1, 0, 0, 1, 'SHORTCUTS'      , 0, 0, 0, 0, 0, 0;
exec dbo.spMODULES_InsertOnly null, 'Schedulers'            , '.moduleList.Schedulers'               , '~/Administration/Schedulers/'       , 1, 0,  0, 0, 1, 0, 0, 1, 'SCHEDULERS'     , 0, 0, 0, 0, 0, 0;
-- 05/13/2008 Paul.  DynamicButtons should be treated as a module. 
exec dbo.spMODULES_InsertOnly null, 'DynamicButtons'        , '.moduleList.DynamicButtons'           , '~/Administration/DynamicButtons/'   , 1, 0,  0, 0, 0, 0, 0, 1, 'DYNAMIC_BUTTONS', 0, 0, 0, 0, 0, 0;
-- 05/13/2008 Paul.  Currencies module.
exec dbo.spMODULES_InsertOnly null, 'Currencies'            , '.moduleList.Currencies'               , '~/Administration/Currencies/'       , 1, 0,  0, 0, 0, 0, 0, 1, 'CURRENCIES'     , 0, 0, 0, 0, 0, 0;
-- 05/13/2008 Paul.  System Log.
exec dbo.spMODULES_InsertOnly null, 'SystemLog'             , '.moduleList.SystemLog'                , '~/Administration/SystemLog/'        , 1, 0,  0, 0, 0, 0, 0, 1, 'SYSTEM_LOG'     , 0, 0, 0, 0, 0, 0;
-- 05/13/2008 Paul.  User Log.
-- 07/11/2018 Paul.  Default to enable Mass update so that we can export logins. 
exec dbo.spMODULES_InsertOnly null, 'UserLogins'            , '.moduleList.UserLogins'               , '~/Administration/UserLogins/'       , 1, 0,  0, 0, 0, 0, 0, 1, 'USERS_LOGINS'   , 0, 1, 0, 0, 0, 0;
-- 07/11/2018 Paul.  Add export checkboxes to UserLogins. 
if exists (select * from MODULES where MODULE_NAME = 'UserLogins' and isnull(MASS_UPDATE_ENABLED, 0) = 0 and DELETED = 0) begin -- then
	print 'MODULES: Add export checkboxes to UserLogins.  ';
	update MODULES
	   set MASS_UPDATE_ENABLED = 1
	     , DATE_MODIFIED       = getdate()
	     , DATE_MODIFIED_UTC   = getutcdate()
	     , MODIFIED_USER_ID    = null
	 where MODULE_NAME         = 'UserLogins'
	   and isnull(MASS_UPDATE_ENABLED, 0) = 0
	   and DELETED             = 0;
end -- if;
GO

-- 09/09/2009 Paul.  Allow direct editing of the module table. 
exec dbo.spMODULES_InsertOnly null, 'Modules'               , '.moduleList.Modules'                  , '~/Administration/Modules/'          , 1, 0,  0, 0, 0, 0, 0, 1, 'MODULES'        , 0, 0, 0, 0, 0, 0;
-- 09/12/2009 Paul.  Allow editing of the field validators. 
exec dbo.spMODULES_InsertOnly null, 'FieldValidators'       , '.moduleList.FieldValidators'          , '~/Administration/FieldValidators/'  , 1, 0,  0, 0, 0, 0, 0, 1, 'FIELD_VALIDATORS', 0, 0, 0, 0, 0, 0;
-- 11/22/2009 Paul.  System Sync Log.
exec dbo.spMODULES_InsertOnly null, 'SystemSyncLog'         , '.moduleList.SystemSyncLog'            , '~/Administration/SystemSyncLog/'    , 1, 0,  0, 0, 0, 0, 0, 1, 'SYSTEM_SYNC_LOG' , 0, 0, 0, 0, 0, 1;

-- 11/01/2020 Paul.  Enable REST for SystemSyncLog to allow export from React Client.
if exists (select * from MODULES where MODULE_NAME = 'SystemSyncLog' and isnull(REST_ENABLED, 0) = 0 and DELETED = 0) begin -- then
	print 'MODULES: Enable REST for SystemSyncLog to allow export from React Client. ';
	update MODULES
	   set REST_ENABLED        = 1
	     , DATE_MODIFIED       = getdate()
	     , DATE_MODIFIED_UTC   = getutcdate()
	     , MODIFIED_USER_ID    = null
	 where MODULE_NAME         = 'SystemSyncLog'
	   and isnull(REST_ENABLED, 0) = 0
	   and DELETED             = 0;
end -- if;

-- 04/23/2011 Paul.  DetailViewsRelationships should be treated as a module so that the merge modules can be retrieved by the Word Plug-in. 
exec dbo.spMODULES_InsertOnly null, 'DetailViewsRelationships', '.moduleList.DetailViewsRelationships' , '~/Administration/DetailViewsRelationships/'   , 1, 0,  0, 0, 0, 0, 0, 1, 'DETAILVIEWS_RELATIONSHIPS', 0, 0, 0, 0, 0, 0;

-- 06/08/2012 Paul.  Add an images module to make it easier to get the Image name in the DetailView. 
exec dbo.spMODULES_InsertOnly null, 'Images'                , '.moduleList.Images'                   , '~/Images/'                          , 1, 0,  0, 0, 0, 0, 0, 0, 'IMAGES'          , 0, 0, 0, 0, 0, 1;
-- 07/11/2018 Paul.  Correct query to use REST_ENABLED instead of MASS_UPDATE_ENABLED. 
if exists (select * from MODULES where MODULE_NAME = 'Images' and isnull(REST_ENABLED, 0) = 0 and DELETED = 0) begin -- then
	print 'MODULES: Enable REST for Images to allow display in DetailView. ';
	update MODULES
	   set REST_ENABLED        = 1
	     , DATE_MODIFIED       = getdate()
	     , DATE_MODIFIED_UTC   = getutcdate()
	     , MODIFIED_USER_ID    = null
	 where MODULE_NAME         = 'Images'
	   and isnull(REST_ENABLED, 0) = 0
	   and DELETED             = 0;
end -- if;

-- 08/07/2013 Paul.  Add Undelete module. 
exec dbo.spMODULES_InsertOnly null, 'Undelete'               , '.moduleList.Undelete'                , '~/Administration/Undelete/'         , 1, 0,  0, 0, 0, 0, 0, 1, null              , 0, 0, 0, 0, 0, 0;


-- 08/24/2008 Paul.  Reorder the modules. 
exec dbo.spMODULES_Reorder null;
GO

set nocount off;
GO


