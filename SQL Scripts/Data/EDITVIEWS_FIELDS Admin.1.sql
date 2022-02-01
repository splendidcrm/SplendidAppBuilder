print 'EDITVIEWS_FIELDS Admin';
set nocount on;
GO


-- select * from vwEDITVIEWS_FIELDS where EDIT_NAME = 'Config.EditView' order by FIELD_INDEX;
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'Config.EditView';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Config.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Config.EditView';
	exec dbo.spEDITVIEWS_InsertOnly            'Config.EditView', 'Config', 'vwCONFIG_Edit', '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Config.EditView'            ,  0, 'Config.LBL_NAME'                        , 'NAME'                             , 1, 1,  60, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Config.EditView'            ,  1, 'Config.LBL_CATEGORY'                    , 'CATEGORY'                         , 0, 1,  32, 20, null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Config.EditView'            ,  2, 'Config.LBL_VALUE'                       , 'VALUE'                            , 0, 1,   8, 80, 3;
end -- if;
GO

if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Config.SearchBasic' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Config.SearchBasic';
	exec dbo.spEDITVIEWS_InsertOnly            'Config.SearchBasic', 'Config' , 'vwCONFIG_List', '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Config.SearchBasic'         ,  0, 'Config.LBL_NAME'                        , 'NAME'                             , 0, 1,  60, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Config.SearchBasic'         ,  1, 'Config.LBL_VALUE'                       , 'VALUE'                            , 0, 1, 200, 35, null;
end -- if;
GO

-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'Currencies.EditView';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Currencies.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Currencies.EditView';
	exec dbo.spEDITVIEWS_InsertOnly            'Currencies.EditView' , 'Currencies', 'vwCURRENCIES_Edit', '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Currencies.EditView'        ,  0, 'Currencies.LBL_NAME'                    , 'NAME'                             , 1, 1,  60, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Currencies.EditView'        ,  1, 'Currencies.LBL_ISO4217'                 , 'ISO4217'                          , 1, 1,  32, 20, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Currencies.EditView'        ,  2, 'Currencies.LBL_RATE'                    , 'CONVERSION_RATE'                  , 1, 1,  32, 20, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Currencies.EditView'        ,  3, 'Currencies.LBL_SYMBOL'                  , 'SYMBOL'                           , 1, 1,  32, 20, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Currencies.EditView'        ,  4, 'Currencies.LBL_STATUS'                  , 'STATUS'                           , 1, 1, 'currency_status_dom', null, null;
end -- if;
GO

-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'Schedulers.EditView';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Schedulers.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Schedulers.EditView';
	exec dbo.spEDITVIEWS_InsertOnly            'Schedulers.EditView', 'Schedulers', 'vwSCHEDULERS', '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Schedulers.EditView'        ,  0, 'Schedulers.LBL_NAME'                    , 'NAME'                             , 1, 1,  60, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Schedulers.EditView'        ,  1, 'Schedulers.LBL_STATUS'                  , 'STATUS'                           , 1, 1, 'scheduler_status_dom', null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Schedulers.EditView'        ,  2, 'Schedulers.LBL_JOB'                     , 'JOB'                              , 1, 1, 'SchedulerJobs', null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Schedulers.EditView'        ,  3, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Schedulers.EditView'        ,  4, 'Schedulers.LBL_INTERVAL'                , 'JOB_INTERVAL'                     , 1, 1, 'CRON'           , null, 3, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Schedulers.EditView'        ,  5, 'Schedulers.LBL_DATE_TIME_START'         , 'DATE_TIME_START'                  , 0, 1, 'DateTimePicker' , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Schedulers.EditView'        ,  6, 'Schedulers.LBL_TIME_FROM'               , 'TIME_FROM'                        , 0, 1, 'TimePicker'     , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Schedulers.EditView'        ,  7, 'Schedulers.LBL_DATE_TIME_END'           , 'DATE_TIME_END'                    , 0, 1, 'DateTimePicker' , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Schedulers.EditView'        ,  8, 'Schedulers.LBL_TIME_TO'                 , 'TIME_TO'                          , 0, 1, 'TimePicker'     , null, null, null;
end else begin
	if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Schedulers.EditView' and DATA_FIELD = 'JOB_INTERVAL' and FIELD_TYPE = 'TextBox' and DELETED = 0) begin -- then
		update EDITVIEWS_FIELDS
		   set FIELD_TYPE        = 'CRON'
		     , FORMAT_SIZE       = null
		     , FORMAT_MAX_LENGTH = null
		     , DATE_MODIFIED     = getdate()
		     , DATE_MODIFIED_UTC = getutcdate()
		 where EDIT_NAME         = 'Schedulers.EditView'
		   and DATA_FIELD        = 'JOB_INTERVAL'
		   and FIELD_TYPE        = 'TextBox'
		   and DELETED           = 0;
	end -- if; 
end -- if;
GO

-- 02/03/2021 Paul.  Provide a way to search. 
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'Schedulers.SearchBasic';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Schedulers.SearchBasic' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Schedulers.SearchBasic';
	exec dbo.spEDITVIEWS_InsertOnly            'Schedulers.SearchBasic', 'Schedulers', 'vwSCHEDULERS', '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Schedulers.SearchBasic'     ,  0, 'Schedulers.LBL_NAME'                    , 'NAME'                             , 1, 1,  60, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Schedulers.SearchBasic'     ,  1, null;
end -- if;
GO

-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'Terminology.EditView';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Terminology.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Terminology.EditView';
	exec dbo.spEDITVIEWS_InsertOnly            'Terminology.EditView', 'Terminology', 'vwTERMINOLOGY_Edit', '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Terminology.EditView'       ,  0, 'Terminology.LBL_NAME'                   , 'NAME'                             , 0, 1,  50, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Terminology.EditView'       ,  1, 'Terminology.LBL_LANG'                   , 'LANG'                             , 1, 1, 'Languages'           , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Terminology.EditView'       ,  2, 'Terminology.LBL_MODULE_NAME'            , 'MODULE_NAME'                      , 0, 1, 'Modules'             , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Terminology.EditView'       ,  3, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Terminology.EditView'       ,  4, 'Terminology.LBL_LIST_NAME'              , 'LIST_NAME'                        , 0, 1, 'TerminologyPickLists', null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Terminology.EditView'       ,  5, 'Terminology.LBL_LIST_ORDER'             , 'LIST_ORDER'                       , 0, 1,  10, 20, null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Terminology.EditView'       ,  6, 'Terminology.LBL_DISPLAY_NAME'           , 'DISPLAY_NAME'                     , 0, 1,   8, 80, 3;
end -- if;
GO

-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'Terminology.SearchBasic';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Terminology.SearchBasic' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Terminology.SearchBasic';
	exec dbo.spEDITVIEWS_InsertOnly            'Terminology.SearchBasic', 'Terminology', 'vwTERMINOLOGY_Edit', '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Terminology.SearchBasic'    ,  0, 'Terminology.LBL_NAME'                   , 'NAME'                             , 0, 1,  50, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Terminology.SearchBasic'    ,  1, 'Terminology.LBL_LANG'                   , 'LANG'                             , 1, 1, 'Languages'           , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Terminology.SearchBasic'    ,  2, 'Terminology.LBL_MODULE_NAME'            , 'MODULE_NAME'                      , 0, 1, 'Modules'             , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Terminology.SearchBasic'    ,  3, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Terminology.SearchBasic'    ,  4, 'Terminology.LBL_LIST_NAME'              , 'LIST_NAME'                        , 0, 1, 'TerminologyPickLists', null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Terminology.SearchBasic'    ,  5, null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Terminology.SearchBasic'    ,  6, 'Terminology.LBL_DISPLAY_NAME'           , 'DISPLAY_NAME'                     , 0, 1,   8, 80, 3;
end -- if;
GO

-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'Shortcuts.EditView';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Shortcuts.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Shortcuts.EditView';
	exec dbo.spEDITVIEWS_InsertOnly            'Shortcuts.EditView', 'Shortcuts', 'vwSHORTCUTS_Edit', '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Shortcuts.EditView'         ,  0, 'Shortcuts.LBL_MODULE_NAME'              , 'MODULE_NAME'                      , 0, 1, 'Modules'              , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Shortcuts.EditView'         ,  1, 'Shortcuts.LBL_DISPLAY_NAME'             , 'DISPLAY_NAME'                     , 0, 1, 150, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Shortcuts.EditView'         ,  2, 'Shortcuts.LBL_RELATIVE_PATH'            , 'RELATIVE_PATH'                    , 0, 1, 255, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Shortcuts.EditView'         ,  3, 'Shortcuts.LBL_IMAGE_NAME'               , 'IMAGE_NAME'                       , 0, 1,  50, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Shortcuts.EditView'         ,  4, 'Shortcuts.LBL_SHORTCUT_ORDER'           , 'SHORTCUT_ORDER'                   , 0, 1,  10, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsCheckBox    'Shortcuts.EditView'         ,  5, 'Shortcuts.LBL_SHORTCUT_ENABLED'         , 'SHORTCUT_ENABLED'                 , 0, 1, null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Shortcuts.EditView'         ,  6, 'Shortcuts.LBL_SHORTCUT_MODULE'          , 'SHORTCUT_MODULE'                  , 0, 1, 'Modules'              , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Shortcuts.EditView'         ,  7, 'Shortcuts.LBL_SHORTCUT_ACLTYPE'         , 'SHORTCUT_ACLTYPE'                 , 0, 1, 'shortcuts_acltype_dom', null, null;
end -- if;
GO

-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'DynamicButtons.SearchBasic';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'DynamicButtons.SearchBasic' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS DynamicButtons.SearchBasic';
	exec dbo.spEDITVIEWS_InsertOnly            'DynamicButtons.SearchBasic', 'DynamicButtons', 'vwDYNAMIC_BUTTONS', '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'DynamicButtons.SearchBasic' ,  0, 'DynamicButtons.LBL_VIEW_NAME'           , 'VIEW_NAME'                        , 1, 1, 'DynamicButtonViews'   , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'DynamicButtons.SearchBasic' ,  1, null;
end -- if;
GO

-- 10/27/2019 Paul.  New layout for PasswordManager.  Needed to manually allow PasswordManager as module even as no entry exists in MODULES table. 
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'PasswordManager.EditView';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'PasswordManager.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS PasswordManager.EditView';
	exec dbo.spEDITVIEWS_InsertOnly            'PasswordManager.EditView', 'PasswordManager', 'vwCONFIG_Edit', '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'PasswordManager.EditView'   ,  0, 'PasswordManager.LBL_PREFERRED_PASSWORD_LENGTH'    , 'Password.PreferredPasswordLength'   , 0, 1,  10, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'PasswordManager.EditView'   ,  1, 'PasswordManager.LBL_MINIMUM_LOWER_CASE_CHARACTERS', 'Password.MinimumLowerCaseCharacters', 0, 1,  10, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'PasswordManager.EditView'   ,  2, 'PasswordManager.LBL_MINIMUM_UPPER_CASE_CHARACTERS', 'Password.MinimumUpperCaseCharacters', 0, 1,  10, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'PasswordManager.EditView'   ,  3, 'PasswordManager.LBL_MINIMUM_NUMERIC_CHARACTERS'   , 'Password.MinimumNumericCharacters'  , 0, 1,  10, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'PasswordManager.EditView'   ,  4, 'PasswordManager.LBL_MINIMUM_SYMBOL_CHARACTERS'    , 'Password.MinimumSymbolCharacters'   , 0, 1,  10, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'PasswordManager.EditView'   ,  5, 'PasswordManager.LBL_SYMBOL_CHARACTERS'            , 'Password.SymbolCharacters'          , 0, 1, 150, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel       'PasswordManager.EditView'   ,  6, null                                               , 'PasswordManager.LBL_SYMBOL_CHARACTERS_DEFAULT !@#$%^&*()<>?~.', -1;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'PasswordManager.EditView'   ,  7, 'PasswordManager.LBL_COMPLEXITY_NUMBER'            , 'Password.ComplexityNumber'          , 0, 1,  10, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'PasswordManager.EditView'   ,  8, 'PasswordManager.LBL_HISTORY_MAXIMUM'              , 'Password.HistoryMaximum'            , 0, 1,  10, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'PasswordManager.EditView'   ,  9, 'PasswordManager.LBL_LOGIN_LOCKOUT_COUNT'          , 'Password.LoginLockoutCount'         , 0, 1,  10, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'PasswordManager.EditView'   , 10, 'PasswordManager.LBL_EXPIRATION_DAYS'              , 'Password.ExpirationDays'            , 0, 1,  10, 10, null;
end -- if;
GO

-- 11/22/2020 Paul.  Add Dropdown editor support to React Client. 
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'Dropdown.EditView';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Dropdown.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Dropdown.EditView';
	exec dbo.spEDITVIEWS_InsertOnly            'Dropdown.EditView', 'Dropdown', 'vwTERMINOLOGY', '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Dropdown.EditView'          ,  1, 'Dropdown.LBL_DROPDOWN'                     , 'LIST_NAME'                       , 1, 1, 150, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Dropdown.EditView'          ,  2, 'Dropdown.LBL_LANGUAGE'                     , 'LANG'                            , 1, 1, 'Languages', null, null, null;
end -- if;
GO

-- 11/22/2020 Paul.  Add Dropdown editor support to React Client. 
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'Dropdown.SearchBasic';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Dropdown.SearchBasic' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Dropdown.SearchBasic';
	exec dbo.spEDITVIEWS_InsertOnly            'Dropdown.SearchBasic', 'Dropdown', 'vwTERMINOLOGY', '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Dropdown.SearchBasic'       ,  1, 'Dropdown.LBL_DROPDOWN'                     , 'LIST_NAME'                       , 1, 1, 'TerminologyPickLists', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Dropdown.SearchBasic'       ,  2, 'Dropdown.LBL_LANGUAGE'                     , 'LANG'                            , 1, 1, 'Languages', null, null, null;
end -- if;
GO

-- 01/30/2021 Paul.  Add EditCustomFields support to React Client. 
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'EditCustomFields.NewRecord';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'EditCustomFields.NewRecord' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS EditCustomFields.NewRecord';
	exec dbo.spEDITVIEWS_InsertOnly            'EditCustomFields.NewRecord', 'EditCustomFields', 'vwFIELDS_META_DATA', '30%', '70%', 1;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'EditCustomFields.NewRecord' ,  0, 'EditCustomFields.COLUMN_TITLE_NAME'           , 'NAME'                           , 1, 1, 255, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'EditCustomFields.NewRecord' ,  1, 'EditCustomFields.COLUMN_TITLE_LABEL'          , 'LABEL'                          , 1, 1, 255, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'EditCustomFields.NewRecord' ,  2, 'EditCustomFields.COLUMN_TITLE_DATA_TYPE'      , 'DATA_TYPE'                      , 1, 1, 'custom_field_type_dom', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'EditCustomFields.NewRecord' ,  3, 'EditCustomFields.COLUMN_TITLE_MAX_SIZE'       , 'MAX_SIZE'                       , 0, 1, 255, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsCheckBox    'EditCustomFields.NewRecord' ,  4, 'EditCustomFields.COLUMN_TITLE_REQUIRED_OPTION', 'REQUIRED'                       , 0, 1, null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'EditCustomFields.NewRecord' ,  5, 'EditCustomFields.COLUMN_TITLE_DEFAULT_VALUE'  , 'DEFAULT_VALUE'                  , 0, 1, 255, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'EditCustomFields.NewRecord' ,  6, 'EditCustomFields.LBL_DROPDOWN_LIST'           , 'DROPDOWN_LIST'                  , 1, 1, 'TerminologyPickLists', null, null, null
end -- if;
GO

-- 01/30/2021 Paul.  Add EditCustomFields support to React Client. 
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'EditCustomFields.EditView';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'EditCustomFields.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS EditCustomFields.EditView';
	exec dbo.spEDITVIEWS_InsertOnly            'EditCustomFields.EditView', 'EditCustomFields', 'vwFIELDS_META_DATA', '30%', '70%', 1;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel       'EditCustomFields.EditView'  ,  0, 'EditCustomFields.COLUMN_TITLE_NAME'           , 'NAME'                           , null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel       'EditCustomFields.EditView'  ,  1, 'EditCustomFields.COLUMN_TITLE_LABEL'          , 'LABEL'                          , null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel       'EditCustomFields.EditView'  ,  2, 'EditCustomFields.COLUMN_TITLE_DATA_TYPE'      , 'DATA_TYPE'                      , null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'EditCustomFields.EditView'  ,  3, 'EditCustomFields.COLUMN_TITLE_MAX_SIZE'       , 'MAX_SIZE'                       , 0, 1, 255, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsCheckBox    'EditCustomFields.EditView'  ,  4, 'EditCustomFields.COLUMN_TITLE_REQUIRED_OPTION', 'REQUIRED'                       , 0, 1, null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'EditCustomFields.EditView'  ,  5, 'EditCustomFields.COLUMN_TITLE_DEFAULT_VALUE'  , 'DEFAULT_VALUE'                  , 0, 1, 255, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'EditCustomFields.EditView'  ,  6, 'EditCustomFields.LBL_DROPDOWN_LIST'           , 'DROPDOWN_LIST'                  , 1, 1, 'TerminologyPickLists', null, null, null
end -- if;
GO

-- 11/22/2020 Paul.  Add Dropdown editor support to React Client. 
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'Terminology.RenameTabs';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Terminology.RenameTabs' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Terminology.RenameTabs';
	exec dbo.spEDITVIEWS_InsertOnly            'Terminology.RenameTabs', 'Terminology', 'vwMODULES_RenameTabs', '15%', '85%', 1;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Terminology.RenameTabs'     ,  1, 'Dropdown.LBL_LANGUAGE'                       , 'LANG'                            , 1, 1, 'Languages', null, null, null;
end -- if;
GO

-- 09/09/2021 Paul.  Add Undelete support to React Client. 
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'Undelete.SearchBasic';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Undelete.SearchBasic' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Undelete.SearchBasic';
	exec dbo.spEDITVIEWS_InsertOnly            'Undelete.SearchBasic', 'Undelete', 'vwCONFIG_Edit', '11%', '22%', 3;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Undelete.SearchBasic'       ,  0, 'Undelete.LBL_NAME'                          , 'NAME'                   , null, null, 255, 30, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Undelete.SearchBasic'       ,  1, 'Undelete.LBL_MODULE_NAME'                   , 'MODULE_NAME'            , null, null, 'AuditedModules', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Undelete.SearchBasic'       ,  2, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Undelete.SearchBasic'       ,  3, 'Undelete.LBL_AUDIT_TOKEN'                   , 'AUDIT_TOKEN'            , null, null, 255, 30, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Undelete.SearchBasic'       ,  4, 'Undelete.LBL_MODIFIED_BY'                   , 'MODIFIED_USER_ID'       , null, null, 'ActiveUsers'   , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Undelete.SearchBasic'       ,  5, 'Undelete.LBL_AUDIT_DATE'                    , 'AUDIT_DATE'             , NULL, null, 'DateRange'     , null, null, null;
end -- if;
GO

-- 10/27/2021 Paul.  Administration.AdminWizard layout is used as a collection of values and not for layout purposes. 
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'Configurator.AdminWizard.Company';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Configurator.AdminWizard.Company' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Configurator.AdminWizard.Company';
	exec dbo.spEDITVIEWS_InsertOnly            'Configurator.AdminWizard.Company', 'Configurator', 'vwCONFIG_Edit', '15%', '35%', 2;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Configurator.AdminWizard.Company',  0, 'Configurator.LBL_COMPANY_NAME'          , 'company_name'           , 0, 1, 255, 40, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Configurator.AdminWizard.Company',  1, 'Configurator.LBL_HEADER_LOGO_IMAGE'     , 'header_logo_image'      , 0, 1, 255, 40, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Configurator.AdminWizard.Company',  2, 'Configurator.LBL_HEADER_LOGO_WIDTH'     , 'header_logo_width'      , 0, 1, 255, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Configurator.AdminWizard.Company',  3, 'Configurator.LBL_HEADER_LOGO_HEIGHT'    , 'header_logo_height'     , 0, 1, 255, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Configurator.AdminWizard.Company',  4, 'Configurator.LBL_HEADER_LOGO_STYLE'     , 'header_logo_style'      , 0, 1, 255, 20, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Configurator.AdminWizard.Company',  5, 'Configurator.LBL_ATLANTIC_HOME_IMAGE'   , 'header_home_image'      , 0, 1, 255, 40, null;
end -- if;
GO

-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'Configurator.AdminWizard.Locale';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Configurator.AdminWizard.Locale' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Configurator.AdminWizard.Locale';
	exec dbo.spEDITVIEWS_InsertOnly            'Configurator.AdminWizard.Locale' , 'Configurator', 'vwCONFIG_Edit', '15%', '35%', 2;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Configurator.AdminWizard.Locale' ,  0, 'Users.LBL_LANGUAGE'                     , 'default_language'       , 0, 2, 'Languages'       , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Configurator.AdminWizard.Locale' ,  1, 'Users.LBL_CURRENCY'                     , 'default_currency'       , 0, 2, 'Currencies'      , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Configurator.AdminWizard.Locale' ,  2, 'Users.LBL_DATE_FORMAT'                  , 'default_date_format'    , 0, 2, 'DateFormat.en-US', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Configurator.AdminWizard.Locale' ,  3, 'Users.LBL_TIME_FORMAT'                  , 'default_language'       , 0, 2, 'TimeForamt.en-US', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Configurator.AdminWizard.Locale' ,  4, 'Users.LBL_TIMEZONE'                     , 'default_timezone'       , 0, 2, 'TimeZones'       , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Configurator.AdminWizard.Locale' ,  5, null;
end -- if;
GO

-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'Configurator.AdminWizard.Mail';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Configurator.AdminWizard.Mail' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Configurator.AdminWizard.Mail';
	exec dbo.spEDITVIEWS_InsertOnly            'Configurator.AdminWizard.Mail', 'Configurator', 'vwCONFIG_Edit', '15%', '35%', 2;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Configurator.AdminWizard.Mail'  ,  0, 'EmailMan.LBL_NOTIFY_FROMNAME'           , 'fromname'               , 0, 3, 128, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Configurator.AdminWizard.Mail'  ,  1, 'EmailMan.LBL_NOTIFY_FROMADDRESS'        , 'fromaddress'            , 0, 3, 128, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Configurator.AdminWizard.Mail'  ,  2, 'EmailMan.LBL_MAIL_SMTPSERVER'           , 'smtpserver'             , 0, 3,  64, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Configurator.AdminWizard.Mail'  ,  3, 'EmailMan.LBL_MAIL_SMTPPORT'             , 'smtpport'               , 0, 3,  10, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsCheckBox    'Configurator.AdminWizard.Mail'  ,  4, 'EmailMan.LBL_MAIL_SMTPAUTH_REQ'         , 'smtpauth_req'           , 0, 3, null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsCheckBox    'Configurator.AdminWizard.Mail'  ,  5, 'EmailMan.LBL_MAIL_SMTPSSL'              , 'smtpssl'                , 0, 3, null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Configurator.AdminWizard.Mail'  ,  6, 'EmailMan.LBL_MAIL_SMTPUSER'             , 'smtpuser'               , 0, 3,  64, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsPassword    'Configurator.AdminWizard.Mail'  ,  7, 'EmailMan.LBL_MAIL_SMTPPASS'             , 'password'               , 0, 3,  64, 25, null;
end -- if;
GO

set nocount off;
GO


