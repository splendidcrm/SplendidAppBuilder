print 'DYNAMIC_BUTTONS DetailView admin';
-- delete from DYNAMIC_BUTTONS where VIEW_NAME like '%.DetailView'
--GO

set nocount on;
GO

-- 08/22/2008 Paul.  Move professional modules to a separate file. 
-- 07/19/2010 Paul.  Remove all references to button keys.  They conflict with the browser keys. 
--	exec dbo.spDYNAMIC_BUTTONS_InsButton    '.DetailView', 0, null, 'edit'  , null, null, 'Edit'     , null, '.LBL_EDIT_BUTTON_LABEL'     , '.LBL_EDIT_BUTTON_TITLE'     , null, null, null;
--	exec dbo.spDYNAMIC_BUTTONS_InsButton    '.DetailView', 1, null, 'edit'  , null, null, 'Duplicate', null, '.LBL_DUPLICATE_BUTTON_LABEL', '.LBL_DUPLICATE_BUTTON_TITLE', null, null, null;
--	exec dbo.spDYNAMIC_BUTTONS_InsButton    '.DetailView', 2, null, 'delete', null, null, 'Delete'   , null, '.LBL_DELETE_BUTTON_LABEL'   , '.LBL_DELETE_BUTTON_TITLE'   , null, 'return ConfirmDelete();', null;
--	exec dbo.spDYNAMIC_BUTTONS_InsButton    '.DetailView', 3, null, null    , null, null, 'Cancel'   , null, '.LBL_CANCEL_BUTTON_LABEL'   , '.LBL_CANCEL_BUTTON_TITLE'   , null, null, null;

-- 09/06/2011 Paul.  Add View Log as a default. 
if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = '.DetailView' and DELETED = 0) begin -- then
	print 'DYNAMIC_BUTTONS .DetailView';
	exec dbo.spDYNAMIC_BUTTONS_InsEdit      '.DetailView'                     , 0, null;
	exec dbo.spDYNAMIC_BUTTONS_InsDuplicate '.DetailView'                     , 1, null;
	exec dbo.spDYNAMIC_BUTTONS_InsDelete    '.DetailView'                     , 2, null;
	exec dbo.spDYNAMIC_BUTTONS_InsCancel    '.DetailView'                     , 3, null, 1;  -- DetailView Cancel is only visible on mobile. 
	exec dbo.spDYNAMIC_BUTTONS_InsViewLog   '.DetailView'                     , 4, null;
end -- if;
GO

-- 09/12/2009 Paul.  Allow editing of Field Validators. 
exec dbo.spDYNAMIC_BUTTONS_CopyDefault '.DetailView', 'FieldValidators.DetailView' , 'FieldValidators' ;
GO

-- 09/09/2009 Paul.  Allow direct editing of the module table. 
--exec dbo.spDYNAMIC_BUTTONS_CopyDefault '.DetailView', 'Modules.DetailView'         , 'Modules'         ;
-- 05/28/2012 Paul.  Modules cannot be duplicated, deleted or audited. 
-- 09/26/2017 Paul.  Add Archive access right. 
if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'Modules.DetailView' and DELETED = 0) begin -- then
	exec dbo.spDYNAMIC_BUTTONS_InsEdit      'Modules.DetailView'              , 0, null;
	exec dbo.spDYNAMIC_BUTTONS_InsCancel    'Modules.DetailView'              , 1, null, 0;
	exec dbo.spDYNAMIC_BUTTONS_InsButton    'Modules.DetailView'              , 2, 'Modules', 'edit', null, null, 'Archive.Build', null, 'Modules.LBL_BUILD_ARCHIVE_TABLE', 'Modules.LBL_BUILD_ARCHIVE_TABLE', null, null, null;
end else begin
	if exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'Modules.DetailView' and COMMAND_NAME = 'Delete' and DELETED = 0) begin -- then
		print 'DYNAMIC_BUTTONS Modules.DetailView:  Remove duplicate, delete and audit. ';
		update DYNAMIC_BUTTONS
		   set DELETED           = 1
		     , DATE_MODIFIED     = getdate()
		     , DATE_MODIFIED_UTC = getutcdate()
		where VIEW_NAME          = 'Modules.DetailView'
		  and DELETED            = 0;
		exec dbo.spDYNAMIC_BUTTONS_InsEdit      'Modules.DetailView'              , 0, null;
		exec dbo.spDYNAMIC_BUTTONS_InsCancel    'Modules.DetailView'              , 1, null, 0;
	end -- if;
	-- 09/26/2017 Paul.  Add Archive access right. 
	if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'Modules.DetailView' and COMMAND_NAME = 'Archive.Build' and DELETED = 0) begin -- then
		exec dbo.spDYNAMIC_BUTTONS_InsButton    'Modules.DetailView'              , 2, 'Modules', 'edit', null, null, 'Archive.Build', null, 'Modules.LBL_BUILD_ARCHIVE_TABLE', 'Modules.LBL_BUILD_ARCHIVE_TABLE', null, null, null;
	end -- if;
end -- if;
GO

-- 04/29/2008 Paul.  The Cancel button should always be displayed, not just on a mobile device.
if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'Import.ImportView' and DELETED = 0) begin -- then
	print 'DYNAMIC_BUTTONS Import.ImportView';
	exec dbo.spDYNAMIC_BUTTONS_InsButton    'Import.ImportView'               , 0, null              , 'import', null, null, 'Import.Run'          , null, 'Import.LBL_RUN_BUTTON_LABEL'                 , 'Import.LBL_RUN_BUTTON_TITLE'                 , null                                        , null, null;
	exec dbo.spDYNAMIC_BUTTONS_InsButton    'Import.ImportView'               , 1, null              , 'import', null, null, 'Import.Preview'      , null, 'Import.LBL_PREVIEW_BUTTON_LABEL'             , 'Import.LBL_PREVIEW_BUTTON_TITLE'             , null                                        , null, null;
	exec dbo.spDYNAMIC_BUTTONS_InsCancel    'Import.ImportView'               , 2, null              , 0;
end -- if;
GO

-- 09/02/2008 Paul.  We need to have two edit buttons, one for the My Account and one for the administrator. 
-- 10/18/2008 Paul.  Admin User Edit needs the User ID.
-- 11/29/2008 Paul.  Needed to add the ID to the text field. 
-- 08/22/2010 Paul.  Add ONCLICK_SCRIPT to spDYNAMIC_BUTTONS_InsButtonLink. 
-- 06/05/2015 Paul.  Use separate set of buttons for MyAccount to prevent 2 edit buttons from being in the same list. 
-- 08/11/2020 Paul.  Employees module may be disabled, so create a Users version of LBL_RESET_PREFERENCES. 
-- delete from DYNAMIC_BUTTONS where VIEW_NAME = 'Users.DetailView'
if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'Users.DetailView' and DELETED = 0) begin -- then
	print 'DYNAMIC_BUTTONS Users.DetailView';
--	exec dbo.spDYNAMIC_BUTTONS_InsButtonLink 'Users.DetailView'               , 0, 'Users'           , 'edit', null, null, 'EditMyAccount'         , 'EditMyAccount.aspx', null, '.LBL_EDIT_BUTTON_LABEL'     , '.LBL_EDIT_BUTTON_TITLE'     , null, null, null, null;
	exec dbo.spDYNAMIC_BUTTONS_InsButtonLink 'Users.DetailView'               , 1, 'Users'           , 'edit', null, null, 'Edit'                  , 'edit.aspx?ID={0}'  , 'ID', '.LBL_EDIT_BUTTON_LABEL'     , '.LBL_EDIT_BUTTON_TITLE'     , null, null, null, null;
	exec dbo.spDYNAMIC_BUTTONS_InsButton     'Users.DetailView'               , 2, 'Users'           , null, null, null, 'ChangePassword'          , null, 'Users.LBL_CHANGE_PASSWORD_BUTTON_LABEL'      , 'Users.LBL_CHANGE_PASSWORD_BUTTON_TITLE'      , null, 'PasswordPopup(); return false;', null;
	exec dbo.spDYNAMIC_BUTTONS_InsDuplicate  'Users.DetailView'               , 3, 'Users';
	exec dbo.spDYNAMIC_BUTTONS_InsButton     'Users.DetailView'               , 4, 'Users'           , null, null, null, 'ResetDefaults'           , null, 'Users.LBL_RESET_PREFERENCES'                 , 'Users.LBL_RESET_PREFERENCES'                 , null, null, null;
	exec dbo.spDYNAMIC_BUTTONS_InsCancel     'Users.DetailView'               , 5, null, 1;
end else begin
	-- 06/05/2015 Paul.  Use separate set of buttons for MyAccount to prevent 2 edit buttons from being in the same list. 
	if exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'Users.DetailView' and COMMAND_NAME = 'EditMyAccount' and DELETED = 0) begin -- then
		print 'Use separate set of buttons for MyAccount to prevent 2 edit buttons from being in the same list.';
		update DYNAMIC_BUTTONS
		   set DELETED       = 1
		     , DATE_MODIFIED = getdate()
		     , DATE_MODIFIED_UTC = getutcdate()
		 where VIEW_NAME     = 'Users.DetailView'
		   and COMMAND_NAME  = 'EditMyAccount'
		   and DELETED       = 0;
	end -- if;
	/*
	if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'Users.DetailView' and COMMAND_NAME = 'EditMyAccount' and DELETED = 0) begin -- then
		print 'User Edit needs to be different for the My Account.';
		update DYNAMIC_BUTTONS
		   set CONTROL_INDEX = CONTROL_INDEX + 1
		     , DATE_MODIFIED = getdate()
		     , DATE_MODIFIED_UTC = getutcdate()
		 where VIEW_NAME     = 'Users.DetailView'
		   and DELETED       = 0;
		exec dbo.spDYNAMIC_BUTTONS_InsButtonLink 'Users.DetailView'               , 0, 'Users'           , 'edit', null, null, 'EditMyAccount'         , 'EditMyAccount.aspx', null, '.LBL_EDIT_BUTTON_LABEL'     , '.LBL_EDIT_BUTTON_TITLE'     , null, null, null, null;
	end -- if;
	*/
	if exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'Users.DetailView' and COMMAND_NAME = 'Edit' and (URL_FORMAT = 'edit.aspx' or TEXT_FIELD is null) and DELETED = 0) begin -- then
		print 'Admin User Edit needs the User ID.';
		update DYNAMIC_BUTTONS
		   set URL_FORMAT    = 'edit.aspx?ID={0}'
		     , TEXT_FIELD    = 'ID'
		     , DATE_MODIFIED = getdate()
		     , DATE_MODIFIED_UTC = getutcdate()
		 where VIEW_NAME     = 'Users.DetailView'
		   and COMMAND_NAME  = 'Edit'
		   and (URL_FORMAT    = 'edit.aspx' or TEXT_FIELD is null)
		   and DELETED       = 0;
	end -- if;
	-- 08/11/2020 Paul.  Employees module may be disabled, so create a Users version of LBL_RESET_PREFERENCES. 
	if exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'Users.DetailView' and COMMAND_NAME = 'ResetDefaults' and CONTROL_TEXT = 'Employees.LBL_RESET_PREFERENCES' and DELETED = 0) begin -- then
		update DYNAMIC_BUTTONS
		   set CONTROL_TEXT      = 'Users.LBL_RESET_PREFERENCES'
		     , CONTROL_TOOLTIP   = 'Users.LBL_RESET_PREFERENCES'
		     , DATE_MODIFIED     = getdate()
		     , DATE_MODIFIED_UTC = getutcdate()
		 where VIEW_NAME         = 'Users.DetailView'
		   and COMMAND_NAME      = 'ResetDefaults'
		   and CONTROL_TEXT      = 'Employees.LBL_RESET_PREFERENCES'
		   and DELETED           = 0;
	end -- if;
end -- if;
GO

-- 06/05/2015 Paul.  Use separate set of buttons for MyAccount to prevent 2 edit buttons from being in the same list. 
-- 08/11/2020 Paul.  Employees module may be disabled, so create a Users version of LBL_RESET_PREFERENCES. 
if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'Users.DetailView.MyAccount' and DELETED = 0) begin -- then
	print 'DYNAMIC_BUTTONS Users.DetailView.MyAccount';
	exec dbo.spDYNAMIC_BUTTONS_InsButtonLink 'Users.DetailView.MyAccount'     , 0, 'Users'           , 'edit', null, null, 'EditMyAccount'         , 'EditMyAccount.aspx', null, '.LBL_EDIT_BUTTON_LABEL'     , '.LBL_EDIT_BUTTON_TITLE'     , null, null, null, null;
	exec dbo.spDYNAMIC_BUTTONS_InsButton     'Users.DetailView.MyAccount'     , 1, 'Users'           , null, null, null, 'ChangePassword'          , null, 'Users.LBL_CHANGE_PASSWORD_BUTTON_LABEL'      , 'Users.LBL_CHANGE_PASSWORD_BUTTON_TITLE'      , null, 'PasswordPopup(); return false;', null;
	exec dbo.spDYNAMIC_BUTTONS_InsDuplicate  'Users.DetailView.MyAccount'     , 2, 'Users';
	exec dbo.spDYNAMIC_BUTTONS_InsButton     'Users.DetailView.MyAccount'     , 3, 'Users'           , null, null, null, 'ResetDefaults'           , null, 'Users.LBL_RESET_PREFERENCES'                 , 'Users.LBL_RESET_PREFERENCES'                 , null, null, null;
	exec dbo.spDYNAMIC_BUTTONS_InsCancel     'Users.DetailView.MyAccount'     , 4, null, 1;
end else begin
	-- 08/11/2020 Paul.  Employees module may be disabled, so create a Users version of LBL_RESET_PREFERENCES. 
	if exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'Users.DetailView.MyAccount' and COMMAND_NAME = 'ResetDefaults' and CONTROL_TEXT = 'Employees.LBL_RESET_PREFERENCES' and DELETED = 0) begin -- then
		update DYNAMIC_BUTTONS
		   set CONTROL_TEXT      = 'Users.LBL_RESET_PREFERENCES'
		     , CONTROL_TOOLTIP   = 'Users.LBL_RESET_PREFERENCES'
		     , DATE_MODIFIED     = getdate()
		     , DATE_MODIFIED_UTC = getutcdate()
		 where VIEW_NAME         = 'Users.DetailView.MyAccount'
		   and COMMAND_NAME      = 'ResetDefaults'
		   and CONTROL_TEXT      = 'Employees.LBL_RESET_PREFERENCES'
		   and DELETED           = 0;
	end -- if;
end -- if;
GO

-- Administration
if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = '.AdminDetailView' and DELETED = 0) begin -- then
	print 'DYNAMIC_BUTTONS .AdminDetailView';
	exec dbo.spDYNAMIC_BUTTONS_InsEdit      '.AdminDetailView'                , 0, null;
	exec dbo.spDYNAMIC_BUTTONS_InsDelete    '.AdminDetailView'                , 1, null;
	exec dbo.spDYNAMIC_BUTTONS_InsCancel    '.AdminDetailView'                , 2, null, 1;
end -- if;
GO

-- 01/31/2012 Paul.  ACLRoles should not have a View Change Log. 
-- 08/15/2017 Paul.  Add Export button. 
--exec dbo.spDYNAMIC_BUTTONS_CopyDefault '.DetailView'     , 'ACLRoles.DetailView'        , 'ACLRoles'        ;
if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'ACLRoles.DetailView' and DELETED = 0) begin -- then
	print 'DYNAMIC_BUTTONS ACLRoles.DetailView';
	exec dbo.spDYNAMIC_BUTTONS_InsEdit      'ACLRoles.DetailView'             , 0, 'ACLRoles'        ;
	exec dbo.spDYNAMIC_BUTTONS_InsDuplicate 'ACLRoles.DetailView'             , 1, 'ACLRoles'        ;
	exec dbo.spDYNAMIC_BUTTONS_InsDelete    'ACLRoles.DetailView'             , 2, 'ACLRoles'        ;
	exec dbo.spDYNAMIC_BUTTONS_InsButton    'ACLRoles.DetailView'             , 4, 'ACLRoles'       , 'edit'  , null, null, 'Export'       , null, '.LBL_EXPORT_BUTTON_LABEL'                , '.LBL_EXPORT_BUTTON_TITLE'                , null, null, null;
end else begin
	-- 01/31/2012 Paul.  The ViewLog command was preventing the FieldSecurity command from begin created. 
	if exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'ACLRoles.DetailView' and COMMAND_NAME = 'ViewLog' and DELETED = 0) begin -- then
		print 'ACLRoles.DetailView: Remove View Change Log';
		update DYNAMIC_BUTTONS
		   set DELETED           = 1
		     , MODIFIED_USER_ID  = null
		     , DATE_MODIFIED     = getdate()
		     , DATE_MODIFIED_UTC = getutcdate()
		 where VIEW_NAME         = 'ACLRoles.DetailView'
		   and COMMAND_NAME      = 'ViewLog'
		   and DELETED           = 0;
	end -- if;
	-- 08/15/2017 Paul.  Add Export button. 
	if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'ACLRoles.DetailView' and COMMAND_NAME = 'Export' and DELETED = 0) begin -- then
		exec dbo.spDYNAMIC_BUTTONS_InsButton    'ACLRoles.DetailView'             , -1, 'ACLRoles'       , 'edit'  , null, null, 'Export'       , null, '.LBL_EXPORT_BUTTON_LABEL'                , '.LBL_EXPORT_BUTTON_TITLE'                , null, null, null;
	end -- if;
end -- if;
GO


exec dbo.spDYNAMIC_BUTTONS_CopyDefault '.AdminDetailView', 'Config.DetailView'          , 'Config'          ;
exec dbo.spDYNAMIC_BUTTONS_CopyDefault '.DetailView'     , 'Schedulers.DetailView'      , 'Schedulers'      ;
exec dbo.spDYNAMIC_BUTTONS_CopyDefault '.DetailView'     , 'Terminology.DetailView'     , 'Terminology'     ;
exec dbo.spDYNAMIC_BUTTONS_CopyDefault '.DetailView'     , 'Teams.DetailView'           , 'Teams'           ;
GO


-- 05/01/2016 Paul.  We are going to prepopulate the currency table so that we can be sure to get the supported ISO values correct. 
-- delete from DYNAMIC_BUTTONS where VIEW_NAME = 'Currencies.DetailView';
if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'Currencies.DetailView' and DELETED = 0) begin -- then
	print 'DYNAMIC_BUTTONS Currencies.DetailView';
	exec dbo.spDYNAMIC_BUTTONS_InsEdit      'Currencies.DetailView'           , 0, null;
	exec dbo.spDYNAMIC_BUTTONS_InsCancel    'Currencies.DetailView'           , 1, null, 1;
	exec dbo.spDYNAMIC_BUTTONS_InsButton    'Currencies.DetailView'           , 2, 'Currencies'      , 'edit'  , null, null, 'Currencies.MakeDefault', null, 'Currencies.LBL_MAKE_DEFAULT', 'Currencies.LBL_MAKE_DEFAULT', null, 'return ConfirmChange();', null;
	exec dbo.spDYNAMIC_BUTTONS_InsButton    'Currencies.DetailView'           , 3, 'Currencies'      , 'edit'  , null, null, 'Currencies.MakeBase'   , null, 'Currencies.LBL_MAKE_BASE'   , 'Currencies.LBL_MAKE_BASE'   , null, 'return ConfirmChange();', null;
	exec dbo.spDYNAMIC_BUTTONS_InsButton    'Currencies.DetailView'           , 4, 'Currencies'      , 'edit'  , null, null, 'Currencies.UpdateRate' , null, 'Currencies.LBL_UPDATE_RATE' , 'Currencies.LBL_UPDATE_RATE' , null, null, null;
end -- if;
GO

-- 04/01/2019 Paul.  Add Shortcuts module for Admin API. 
exec dbo.spDYNAMIC_BUTTONS_CopyDefault '.DetailView', 'Shortcuts.DetailView'      , 'Shortcuts'      ;
exec dbo.spDYNAMIC_BUTTONS_CopyDefault '.DetailView', 'DynamicButtons.DetailView' , 'DynamicButtons' ;
-- 02/21/2021 Paul.  Languages for React client. 
exec dbo.spDYNAMIC_BUTTONS_CopyDefault '.DetailView', 'Languages.DetailView'      , 'Languages'      ;
GO


-- 03/06/2018 Paul.  Add ViewLog to Teams if missing. 
if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'Teams.DetailView' and COMMAND_NAME = 'ViewLog' and DELETED = 0) begin -- then
	exec dbo.spDYNAMIC_BUTTONS_InsViewLog   'Teams.DetailView'                     , -1, null;
end -- if;
GO

set nocount off;
GO


