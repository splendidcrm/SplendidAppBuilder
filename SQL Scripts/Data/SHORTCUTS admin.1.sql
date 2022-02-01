print 'SHORTCUTS admin';
-- delete SHORTCUTS
GO

set nocount on;
GO

if not exists (select * from SHORTCUTS where MODULE_NAME = 'Administration' and DELETED = 0) begin -- then
	exec dbo.spSHORTCUTS_InsertOnly null, 'Administration'        , 'Users.LNK_NEW_USER'                    , '~/Users/edit.aspx'                      , 'CreateUsers.gif'         , 1,  1, 'Users', 'edit';
end -- if;
GO

if not exists (select * from SHORTCUTS where MODULE_NAME = 'Dropdown' and DELETED = 0) begin -- then
	exec dbo.spSHORTCUTS_InsertOnly null, 'Dropdown'              , 'Dropdown.LNK_NEW_DROPDOWN'             , '~/Administration/Dropdown/edit.aspx'    , 'CreateDropdown.gif'      , 1,  1, 'Dropdown', 'edit';
	exec dbo.spSHORTCUTS_InsertOnly null, 'Dropdown'              , 'Dropdown.LNK_DROPDOWNS'                , '~/Administration/Dropdown/default.aspx' , 'CreateDropdown.gif'      , 1,  2, 'Dropdown', 'list';
end -- if;
GO

if not exists (select * from SHORTCUTS where MODULE_NAME = 'EditCustomFields' and DELETED = 0) begin -- then
	exec dbo.spSHORTCUTS_InsertOnly null, 'EditCustomFields'      , 'EditCustomFields.LNK_SELECT_CUSTOM_FIELD'     , '~/Administration/EditCustomFields/default.aspx'         , 'Administration.gif'   , 1,  1, 'EditCustomFields'  , 'list';
end -- if;
GO

-- 09/08/2007 Paul.  All the relationships to be edited. 
-- 04/19/2010 Paul.  Add EditView Relationships. 
-- 02/28/2016 Paul.  Point to new layout editor. 
if not exists (select * from SHORTCUTS where MODULE_NAME = 'DynamicLayout' and DELETED = 0) begin -- then
	exec dbo.spSHORTCUTS_InsertOnly null, 'DynamicLayout'         , 'DynamicLayout.LNK_LAYOUT_EDITOR'              , '~/Administration/DynamicLayout/html5/default.aspx'            , 'Administration.gif'   , 1,  1, 'DynamicLayout'     , 'edit';
	exec dbo.spSHORTCUTS_InsertOnly null, 'DynamicLayout'         , 'DynamicLayout.LNK_LAYOUT_DETAILVIEWS'         , '~/Administration/DynamicLayout/DetailViews/default.aspx'      , 'Administration.gif'   , 1,  2, 'DynamicLayout'     , 'edit';
	exec dbo.spSHORTCUTS_InsertOnly null, 'DynamicLayout'         , 'DynamicLayout.LNK_LAYOUT_EDITVIEWS'           , '~/Administration/DynamicLayout/EditViews/default.aspx'        , 'Administration.gif'   , 1,  3, 'DynamicLayout'     , 'edit';
	exec dbo.spSHORTCUTS_InsertOnly null, 'DynamicLayout'         , 'DynamicLayout.LNK_LAYOUT_GRIDVIEWS'           , '~/Administration/DynamicLayout/GridViews/default.aspx'        , 'Administration.gif'   , 1,  4, 'DynamicLayout'     , 'edit';
	exec dbo.spSHORTCUTS_InsertOnly null, 'DynamicLayout'         , 'DynamicLayout.LNK_LAYOUT_RELATIONSHIPS'       , '~/Administration/DynamicLayout/Relationships/default.aspx'    , 'Administration.gif'   , 1,  5, 'DynamicLayout'     , 'edit';
	exec dbo.spSHORTCUTS_InsertOnly null, 'DynamicLayout'         , 'DynamicLayout.LNK_LAYOUT_EDIT_RELATIONSHIPS'  , '~/Administration/DynamicLayout/EditRelationships/default.aspx', 'Administration.gif'   , 1,  6, 'DynamicLayout'     , 'edit';
end else begin
	if not exists (select * from SHORTCUTS where MODULE_NAME = 'DynamicLayout' and DISPLAY_NAME = 'DynamicLayout.LNK_LAYOUT_RELATIONSHIPS' and DELETED = 0) begin -- then
		exec dbo.spSHORTCUTS_InsertOnly null, 'DynamicLayout'         , 'DynamicLayout.LNK_LAYOUT_RELATIONSHIPS'       , '~/Administration/DynamicLayout/Relationships/default.aspx', 'Administration.gif'   , 1,  4, 'DynamicLayout'     , 'edit';
	end -- if;
	if not exists (select * from SHORTCUTS where MODULE_NAME = 'DynamicLayout' and DISPLAY_NAME = 'DynamicLayout.LNK_LAYOUT_EDIT_RELATIONSHIPS' and DELETED = 0) begin -- then
		exec dbo.spSHORTCUTS_InsertOnly null, 'DynamicLayout'         , 'DynamicLayout.LNK_LAYOUT_EDIT_RELATIONSHIPS'  , '~/Administration/DynamicLayout/EditRelationships/default.aspx', 'Administration.gif'   , 1,  5, 'DynamicLayout'     , 'edit';
	end -- if;
	if not exists (select * from SHORTCUTS where MODULE_NAME = 'DynamicLayout' and DISPLAY_NAME = 'DynamicLayout.LNK_LAYOUT_EDITOR' and DELETED = 0) begin -- then
		update SHORTCUTS
		   set SHORTCUT_ORDER    = SHORTCUT_ORDER + 1
		     , DATE_MODIFIED     = getdate()
		     , DATE_MODIFIED_UTC = getutcdate()
		     , MODIFIED_USER_ID  = null
		 where MODULE_NAME       = 'DynamicLayout'
		   and DELETED           = 0;
		exec dbo.spSHORTCUTS_InsertOnly null, 'DynamicLayout'         , 'DynamicLayout.LNK_LAYOUT_EDITOR'              , '~/Administration/DynamicLayout/html5/default.aspx', 'Administration.gif'   , 1,  1, 'DynamicLayout'     , 'edit';
	end -- if;
end -- if;
GO

if not exists (select * from SHORTCUTS where MODULE_NAME = 'ACLRoles' and DELETED = 0) begin -- then
	exec dbo.spSHORTCUTS_InsertOnly null, 'ACLRoles'              , 'ACLRoles.LIST_ROLES'                          , '~/Administration/ACLRoles/default.aspx'                 , 'Roles.gif'            , 1,  1, 'ACLRoles'          , 'list';
	exec dbo.spSHORTCUTS_InsertOnly null, 'ACLRoles'              , 'ACLRoles.LIST_ROLES_BY_USER'                  , '~/Administration/ACLRoles/ByUser.aspx'                  , 'Roles.gif'            , 1,  2, 'ACLRoles'          , 'list';
	exec dbo.spSHORTCUTS_InsertOnly null, 'ACLRoles'              , 'ACLRoles.LBL_CREATE_ROLE'                     , '~/Administration/ACLRoles/edit.aspx'                    , 'Roles.gif'            , 1,  3, 'ACLRoles'          , 'edit';
end -- if;
GO

-- delete from SHORTCUTS where MODULE_NAME = 'Terminology';
if not exists (select * from SHORTCUTS where MODULE_NAME = 'Terminology' and DELETED = 0) begin -- then
	exec dbo.spSHORTCUTS_InsertOnly null, 'Terminology'           , 'Terminology.LBL_NEW_FORM_TITLE'             , '~/Administration/Terminology/edit.aspx'          , 'Terminology.gif', 1,  1, 'Terminology', 'edit';
	exec dbo.spSHORTCUTS_InsertOnly null, 'Terminology'           , 'Administration.LBL_MANAGE_TERMINOLOGY_TITLE', '~/Administration/Terminology/default.aspx'       , 'Terminology.gif', 1,  2, 'Terminology', 'list';
	exec dbo.spSHORTCUTS_InsertOnly null, 'Terminology'           , '.LBL_IMPORT'                                , '~/Administration/Terminology/import.aspx'        , 'Import.gif'     , 1,  3, 'Terminology', 'import';
	exec dbo.spSHORTCUTS_InsertOnly null, 'Terminology'           , 'Administration.LBL_IMPORT_TERMINOLOGY_TITLE', '~/Administration/Terminology/Import/default.aspx', 'Import.gif'     , 1,  4, 'Terminology', 'import';
end -- if;
GO

-- delete from SHORTCUTS where MODULE_NAME = 'Shortcuts';
if not exists (select * from SHORTCUTS where MODULE_NAME = 'Shortcuts' and DELETED = 0) begin -- then
	exec dbo.spSHORTCUTS_InsertOnly null, 'Shortcuts'             , 'Shortcuts.LNK_NEW_SHORTCUT'            , '~/Administration/Shortcuts/edit.aspx'     , 'CreateShortcuts.gif'   , 1,  1, 'Shortcuts' , 'edit';
	exec dbo.spSHORTCUTS_InsertOnly null, 'Shortcuts'             , 'Shortcuts.LNK_SHORTCUT_LIST'           , '~/Administration/Shortcuts/default.aspx'  , 'Shortcuts.gif'         , 1,  2, 'Shortcuts' , 'list';
end -- if;
GO

-- delete from SHORTCUTS where MODULE_NAME = 'Schedulers'
if not exists (select * from SHORTCUTS where MODULE_NAME = 'Schedulers' and DELETED = 0) begin -- then
	exec dbo.spSHORTCUTS_InsertOnly null, 'Schedulers'            , 'Schedulers.LNK_LIST_SCHEDULER'         , '~/Administration/Schedulers/default.aspx'  , 'Schedulers.gif'        , 1,  1, 'Schedulers', 'list';
	exec dbo.spSHORTCUTS_InsertOnly null, 'Schedulers'            , 'Schedulers.LNK_NEW_SCHEDULER'          , '~/Administration/Schedulers/edit.aspx'     , 'CreateScheduler.gif'   , 1,  2, 'Schedulers', 'edit';
end -- if;
GO

-- delete from SHORTCUTS where MODULE_NAME = 'DynamicButtons';
if not exists (select * from SHORTCUTS where MODULE_NAME = 'DynamicButtons' and DELETED = 0) begin -- then
	exec dbo.spSHORTCUTS_InsertOnly null, 'DynamicButtons'        , 'DynamicButtons.LNK_NEW_DYNAMIC_BUTTON' , '~/Administration/DynamicButtons/edit.aspx'     , 'CreateDynamicButtons.gif', 1,  1, 'DynamicButtons' , 'edit';
	exec dbo.spSHORTCUTS_InsertOnly null, 'DynamicButtons'        , 'DynamicButtons.LNK_DYNAMIC_BUTTON_LIST', '~/Administration/DynamicButtons/default.aspx'  , 'DynamicButtons.gif'      , 1,  2, 'DynamicButtons' , 'list';
end -- if;
GO

-- 09/09/2009 Paul.  Allow direct editing of the module table. 
-- delete from SHORTCUTS where MODULE_NAME = 'Modules'
if not exists (select * from SHORTCUTS where MODULE_NAME = 'Modules' and DELETED = 0) begin -- then
	exec dbo.spSHORTCUTS_InsertOnly null, 'Modules'               , 'Modules.LBL_LIST_FORM_TITLE'           , '~/Administration/Modules/default.aspx'     , 'Administration.gif'    , 1,  1, 'Modules', 'list';
end -- if;
GO

-- 09/12/2009 Paul.  Allow editing of Field Validators. 
-- delete from SHORTCUTS where MODULE_NAME = 'FieldValidators';
if not exists (select * from SHORTCUTS where MODULE_NAME = 'FieldValidators' and DELETED = 0) begin -- then
	exec dbo.spSHORTCUTS_InsertOnly null, 'FieldValidators'       , 'FieldValidators.LNK_NEW_FIELD_VALIDATOR' , '~/Administration/FieldValidators/edit.aspx'             , 'CreateFieldValidators.gif', 1,  1, 'FieldValidators', 'edit';
	exec dbo.spSHORTCUTS_InsertOnly null, 'FieldValidators'       , 'FieldValidators.LNK_FIELD_VALIDATOR_LIST', '~/Administration/FieldValidators/default.aspx'          , 'FieldValidators.gif'      , 1,  2, 'FieldValidators', 'list';
end -- if;
GO

-- 03/03/2010 Paul.  We need a quick access to the config edit link. 
-- delete from SHORTCUTS where MODULE_NAME = 'Config';
if not exists (select * from SHORTCUTS where MODULE_NAME = 'Config' and DELETED = 0) begin -- then
	exec dbo.spSHORTCUTS_InsertOnly null, 'Config'                , 'Config.LNK_NEW_CONFIG'                   , '~/Administration/Config/edit.aspx'                      , 'Config.gif'               , 1,  1, 'Config', 'edit';
	exec dbo.spSHORTCUTS_InsertOnly null, 'Config'                , 'Config.LNK_CONFIG_LIST'                  , '~/Administration/Config/default.aspx'                   , 'Config.gif'               , 1,  2, 'Config', 'list';
end -- if;
GO

-- 05/03/2016 Paul.  Full editing of currencies requires shortcuts.  But, we don't need to allow creation as the list is prepopulated. 
-- delete from SHORTCUTS where MODULE_NAME = 'Currencies';
if not exists (select * from SHORTCUTS where MODULE_NAME = 'Currencies' and DELETED = 0) begin -- then
--	exec dbo.spSHORTCUTS_InsertOnly null, 'Currencies'            , 'Currencies.LNK_NEW_CURRENCY'             , '~/Administration/Currencies/edit.aspx'                    , 'CreateCurrencies.gif'      , 1,  1, 'Currencies'   , 'edit';
	exec dbo.spSHORTCUTS_InsertOnly null, 'Currencies'            , 'Currencies.LNK_CURRENCY_LIST'            , '~/Administration/Currencies/default.aspx'                 , 'Currencies.gif'            , 1,  2, 'Currencies'   , 'list';
	exec dbo.spSHORTCUTS_InsertOnly null, 'Currencies'            , 'CurrencyLayer.LBL_CURRENCYLAYER_SETTINGS', '~/Administration/CurrencyLayer/default.aspx'              , 'CurrencyLayer.gif'         , 1,  3, 'CurrencyLayer', 'edit';
end -- if;
GO

-- delete from SHORTCUTS where MODULE_NAME = 'Teams';
if not exists (select * from SHORTCUTS where MODULE_NAME = 'Teams' and DELETED = 0) begin -- then
	exec dbo.spSHORTCUTS_InsertOnly null, 'Teams'                 , 'Teams.LNK_NEW_TEAM'                           , '~/Administration/Teams/edit.aspx'         , 'CreateTeams.gif'      , 1,  1, 'Teams'             , 'edit';
	exec dbo.spSHORTCUTS_InsertOnly null, 'Teams'                 , 'Teams.LNK_TEAM_LIST'                          , '~/Administration/Teams/default.aspx'      , 'Teams.gif'            , 1,  2, 'Teams'             , 'list';
	exec dbo.spSHORTCUTS_InsertOnly null, 'Teams'                 , 'TeamNotices.LNK_TEAM_NOTICE_LIST'             , '~/Administration/TeamNotices/default.aspx', 'Teams.gif'            , 1,  3, 'TeamNotices'       , 'list';
end -- if;
GO

set nocount off;
GO


