

-- Terminology generated from database [SplendidCRM5_50] on 11/18/2010 1:19:38 AM.
print 'TERMINOLOGY ModuleBuilder en-us';
GO

set nocount on;
GO

exec dbo.spTERMINOLOGY_InsertOnly N'LBL_CREATE_CODE_BEHIND'                        , N'en-US', N'ModuleBuilder', null, null, N'Code-Behind:';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_CREATE_CODE_BEHIND_INSTRUCTIONS'           , N'en-US', N'ModuleBuilder', null, null, N'Generate code-behind files and require a rebuild to deploy.';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_CUSTOM_ENABLED'                            , N'en-US', N'ModuleBuilder', null, null, N'Custom Enabled:';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_CUSTOM_ENABLED_INSTRUCTIONS'               , N'en-US', N'ModuleBuilder', null, null, N'Can custom fields be added to the module?';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_DISPLAY_NAME'                              , N'en-US', N'ModuleBuilder', null, null, N'Display Name:';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_DISPLAY_NAME_INSTRUCTIONS'                 , N'en-US', N'ModuleBuilder', null, null, N'The display name can contain spaces.';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_GENERATE_BUTTON_LABEL'                     , N'en-US', N'ModuleBuilder', null, null, N'Generate';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_GENERATE_BUTTON_TITLE'                     , N'en-US', N'ModuleBuilder', null, null, N'Generate';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_IMPORT_ENABLED'                            , N'en-US', N'ModuleBuilder', null, null, N'Import Enabled:';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_IMPORT_ENABLED_INSTRUCTIONS'               , N'en-US', N'ModuleBuilder', null, null, N'Can data be imported into the module?';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_INCLUDE_ASSIGNED_USER_ID'                  , N'en-US', N'ModuleBuilder', null, null, N'Include Assigned User ID:';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_INCLUDE_ASSIGNED_USER_ID_INSTRUCTIONS'     , N'en-US', N'ModuleBuilder', null, null, N'Should the ASSIGNED_USER_ID field be added to the table?';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_INCLUDE_TEAM_ID'                           , N'en-US', N'ModuleBuilder', null, null, N'Include Team ID:';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_INCLUDE_TEAM_ID_INSTRUCTIONS'              , N'en-US', N'ModuleBuilder', null, null, N'Should the TEAM_ID field be added to the table?';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_IS_ADMIN'                                  , N'en-US', N'ModuleBuilder', null, null, N'Is Admin:';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_IS_ADMIN_INSTRUCTIONS'                     , N'en-US', N'ModuleBuilder', null, null, N'Does this module belong in the Administration area?';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_LIST_DATA_TYPE'                            , N'en-US', N'ModuleBuilder', null, null, N'Data Type';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_LIST_EDIT_LABEL'                           , N'en-US', N'ModuleBuilder', null, null, N'Edit Label';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_LIST_FIELD_NAME'                           , N'en-US', N'ModuleBuilder', null, null, N'Field Name';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_LIST_FORM_TITLE'                           , N'en-US', N'ModuleBuilder', null, null, N'Module Builder';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_LIST_LIST_LABEL'                           , N'en-US', N'ModuleBuilder', null, null, N'List Label';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_LIST_MAX_SIZE'                             , N'en-US', N'ModuleBuilder', null, null, N'Max Size';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_LIST_REQUIRED'                             , N'en-US', N'ModuleBuilder', null, null, N'Required';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_MOBILE_ENABLED'                            , N'en-US', N'ModuleBuilder', null, null, N'Mobile Enabled:';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_MOBILE_ENABLED_INSTRUCTIONS'               , N'en-US', N'ModuleBuilder', null, null, N'Should the module appear on mobile devices?';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_MODULE_BUILDER_DISABLED'                   , N'en-US', N'ModuleBuilder', null, null, N'The Module Builder has been disabled in the Web.config.';
-- 03/05/2011 Paul.  LBL_MODULE_NAME is a reserved name, so just use LBL_NAME. 
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_NAME'                                      , N'en-US', N'ModuleBuilder', null, null, N'Module Name:';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_MODULE_NAME'                               , N'en-US', N'ModuleBuilder', null, null, N'Module Builder';
-- 06/04/2015 Paul.  Add module abbreviation. 
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_MODULE_ABBREVIATION'                       , N'en-US', N'ModuleBuilder', null, null, N'MB';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_MODULE_NAME_INSTRUCTIONS'                  , N'en-US', N'ModuleBuilder', null, null, N'The module name should be a valid folder name, so it should not contain spaces.';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_OVERWRITE_EXISTING'                        , N'en-US', N'ModuleBuilder', null, null, N'Overwrite Existing:';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_OVERWRITE_EXISTING_INSTRUCTIONS'           , N'en-US', N'ModuleBuilder', null, null, N'Should existing files be over-written?  Read-only files will still be preserved.';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_REPORT_ENABLED'                            , N'en-US', N'ModuleBuilder', null, null, N'Report Enabled:';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_REPORT_ENABLED_INSTRUCTIONS'               , N'en-US', N'ModuleBuilder', null, null, N'Can reports be generated for the module?';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_TAB_ENABLED'                               , N'en-US', N'ModuleBuilder', null, null, N'Tab Enabled:';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_TAB_ENABLED_INSTRUCTIONS'                  , N'en-US', N'ModuleBuilder', null, null, N'Should the module appear on the tab menu?';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_TABLE_NAME'                                , N'en-US', N'ModuleBuilder', null, null, N'Table Name:';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_TABLE_NAME_INSTRUCTIONS'                   , N'en-US', N'ModuleBuilder', null, null, N'The table name cannot contain spaces or dashes, so they are replaces with underscores.';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_WIZARD_STEP1'                              , N'en-US', N'ModuleBuilder', null, null, N'Module Definition';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_WIZARD_STEP2'                              , N'en-US', N'ModuleBuilder', null, null, N'Fields';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_WIZARD_STEP3'                              , N'en-US', N'ModuleBuilder', null, null, N'Related Modules';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_WIZARD_STEP4'                              , N'en-US', N'ModuleBuilder', null, null, N'Generation';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_MODULEBUILDER'                             , N'en-US', N'ModuleBuilder', null, null, N'Module Builder';
-- 09/12/2011 Paul.  REST_ENABLED provides a way to enable/disable a module in the REST API. 
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_REST_ENABLED'                              , N'en-US', N'ModuleBuilder', null, null, N'REST Enabled:';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_REST_ENABLED_INSTRUCTIONS'                 , N'en-US', N'ModuleBuilder', null, null, N'Is this module accessible via the REST API?';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_REACT_ONLY'                                , N'en-US', N'ModuleBuilder', null, null, N'REACT Only:';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_REACT_ONLY_INSTRUCTIONS'                   , N'en-US', N'ModuleBuilder', null, null, N'Build only for REACT Client.  No ASP.NET files will be created.';
GO

exec dbo.spTERMINOLOGY_InsertOnly N'ModuleBuilder'                                 , N'en-US', null, N'moduleList'         , 103, N'Module Builder';
GO

set nocount off;
GO

/* -- #if Oracle
	COMMIT WORK;
END;
/
-- #endif Oracle */

/* -- #if IBM_DB2
	commit;
  end
/

call dbo.spTERMINOLOGY_ModuleBuilder_en_us()
/

call dbo.spSqlDropProcedure('spTERMINOLOGY_ModuleBuilder_en_us')
/
-- #endif IBM_DB2 */
