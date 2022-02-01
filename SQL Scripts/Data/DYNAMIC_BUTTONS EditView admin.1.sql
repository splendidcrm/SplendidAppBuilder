print 'DYNAMIC_BUTTONS EditView admin';
-- delete from DYNAMIC_BUTTONS where VIEW_NAME like '%.EditView'
--GO

set nocount on;
GO

-- 08/22/2008 Paul.  Move professional modules to a separate file. 
--	exec dbo.spDYNAMIC_BUTTONS_InsButton  '.EditView'                      , 0, null, 'edit', null, null, 'Save'     , null, '.LBL_SAVE_BUTTON_LABEL'     , '.LBL_SAVE_BUTTON_TITLE'     , '.LBL_SAVE_BUTTON_KEY'     , null, null;
--	exec dbo.spDYNAMIC_BUTTONS_InsButton  '.EditView'                      , 1, null, null  , null, null, 'Cancel'   , null, '.LBL_CANCEL_BUTTON_LABEL'   , '.LBL_CANCEL_BUTTON_TITLE'   , '.LBL_CANCEL_BUTTON_KEY'   , null, null;

if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = '.EditView' and DELETED = 0) begin -- then
	print 'DYNAMIC_BUTTONS .EditView';
	exec dbo.spDYNAMIC_BUTTONS_InsSave   '.EditView'                       , 0, null;
	exec dbo.spDYNAMIC_BUTTONS_InsCancel '.EditView'                       , 1, null, 0;  -- EditView Cancel is always visible. 
end -- if;
GO

exec dbo.spDYNAMIC_BUTTONS_CopyDefault '.EditView', 'Users.EditView'           , 'Users'           ;
exec dbo.spDYNAMIC_BUTTONS_CopyDefault '.EditView', 'DynamicButtons.EditView'  , 'DynamicButtons'  ;
-- 09/09/2009 Paul.  Allow direct editing of the module table. 
exec dbo.spDYNAMIC_BUTTONS_CopyDefault '.EditView', 'Modules.EditView'         , 'Modules'         ;
-- 09/12/2009 Paul.  Allow editing of Field Validators. 
exec dbo.spDYNAMIC_BUTTONS_CopyDefault '.EditView', 'FieldValidators.EditView' , 'FieldValidators' ;
-- 05/17/2010 Paul.  Allow editing of Languages. 
exec dbo.spDYNAMIC_BUTTONS_CopyDefault '.EditView', 'Languages.EditView'       , 'Languages'       ;
GO

-- 05/01/2016 Paul.  Currencies now has full EditView/DetailView/ListView layouts. 
-- 03/19/2019 Paul.  Remove Clear button. 
if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'Currencies.EditView' and DELETED = 0) begin -- then
	print 'DYNAMIC_BUTTONS Currencies.EditView';
	exec dbo.spDYNAMIC_BUTTONS_InsSave   'Currencies.EditView'             , 0, null;
	exec dbo.spDYNAMIC_BUTTONS_InsCancel 'Currencies.EditView'             , 1, null, 0;  -- EditView Cancel is always visible. 
end else begin
	-- 05/01/2016 Paul.  Currencies now has full EditView/DetailView/ListView layouts. 
	update DYNAMIC_BUTTONS
	   set DELETED           = 1
	     , DATE_MODIFIED     = getdate()
	     , DATE_MODIFIED_UTC = getutcdate()
	 where VIEW_NAME         = 'Currencies.EditView'
	   and COMMAND_NAME      = 'Clear'
	   and DELETED           = 0;
	exec dbo.spDYNAMIC_BUTTONS_InsCancel 'Currencies.EditView'             , 1, null, 0;  -- EditView Cancel is always visible. 
end -- if;
GO

-- Administration
if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = '.AdminEditView' and DELETED = 0) begin -- then
	print 'DYNAMIC_BUTTONS .AdminEditView';
	exec dbo.spDYNAMIC_BUTTONS_InsSave    '.AdminEditView'                 , 0, null;
	exec dbo.spDYNAMIC_BUTTONS_InsSaveNew '.AdminEditView'                 , 1, null;
end -- if;
GO

exec dbo.spDYNAMIC_BUTTONS_CopyDefault '.EditView', 'ACLRoles.EditView'         , 'ACLRoles'         ;
exec dbo.spDYNAMIC_BUTTONS_CopyDefault '.EditView', 'Config.EditView'           , 'Config'           ;
exec dbo.spDYNAMIC_BUTTONS_CopyDefault '.EditView', 'ConfigureSettings.EditView', 'ConfigureSettings';
exec dbo.spDYNAMIC_BUTTONS_CopyDefault '.EditView', 'Dropdown.EditView'         , 'Dropdown'         ;
exec dbo.spDYNAMIC_BUTTONS_CopyDefault '.EditView', 'Schedulers.EditView'       , 'Schedulers'       ;
exec dbo.spDYNAMIC_BUTTONS_CopyDefault '.EditView', 'Shortcuts.EditView'        , 'Shortcuts'        ;
exec dbo.spDYNAMIC_BUTTONS_CopyDefault '.EditView', 'Terminology.EditView'      , 'Terminology'      ;
exec dbo.spDYNAMIC_BUTTONS_CopyDefault '.EditView', 'Teams.EditView'            , 'Teams'            ;
GO

if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'DynamicLayout.EditView' and DELETED = 0) begin -- then
	print 'DYNAMIC_BUTTONS DynamicLayout.EditView';
	exec dbo.spDYNAMIC_BUTTONS_InsSave   'DynamicLayout.EditView'          , 1, null;
	exec dbo.spDYNAMIC_BUTTONS_InsCancel 'DynamicLayout.EditView'          , 2, null              , 0;
	exec dbo.spDYNAMIC_BUTTONS_InsButton 'DynamicLayout.EditView'          , 3, null              , null  , null              , null, 'New'                     , null, '.LBL_NEW_BUTTON_LABEL'                       , '.LBL_NEW_BUTTON_TITLE'                       , '.LBL_NEW_BUTTON_KEY'                       , null, null;
	exec dbo.spDYNAMIC_BUTTONS_InsButton 'DynamicLayout.EditView'          , 4, null              , null  , null              , null, 'Defaults'                , null, '.LBL_DEFAULTS_BUTTON_LABEL'                  , '.LBL_DEFAULTS_BUTTON_TITLE'                  , '.LBL_DEFAULTS_BUTTON_KEY'                  , null, null;
end -- if;
GO

if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'EditCustomFields.EditView' and DELETED = 0) begin -- then
	print 'DYNAMIC_BUTTONS EditCustomFields.EditView';
	exec dbo.spDYNAMIC_BUTTONS_InsSave   'EditCustomFields.EditView'       , 1, null;
	exec dbo.spDYNAMIC_BUTTONS_InsCancel 'EditCustomFields.EditView'       , 2, null              , 0;
end -- if;
GO

if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'PasswordManager.EditView' and DELETED = 0) begin -- then
	print 'DYNAMIC_BUTTONS PasswordManager.EditView';
	exec dbo.spDYNAMIC_BUTTONS_InsButton 'PasswordManager.EditView'        , 0, null              , null  , null              , null, 'Save'                    , null, '.LBL_SAVE_BUTTON_LABEL'                      , '.LBL_SAVE_BUTTON_TITLE'                      , '.LBL_SAVE_BUTTON_KEY'                      , null, null;
	exec dbo.spDYNAMIC_BUTTONS_InsButton 'PasswordManager.EditView'        , 1, null              , null  , null              , null, 'Cancel'                  , null, '.LBL_CANCEL_BUTTON_LABEL'                    , '.LBL_CANCEL_BUTTON_TITLE'                    , '.LBL_CANCEL_BUTTON_KEY'                    , null, null;
end -- if;
GO

-- 05/07/2017 Paul.  Add HTML5 Dashboard. 
if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'Dashboard.EditView' and DELETED = 0) begin -- then
	print 'DYNAMIC_BUTTONS .EditView';
	exec dbo.spDYNAMIC_BUTTONS_InsSave   'Dashboard.EditView', 0, null;
	exec dbo.spDYNAMIC_BUTTONS_InsCancel 'Dashboard.EditView', 1, null, 0;
	exec dbo.spDYNAMIC_BUTTONS_InsButton 'Dashboard.EditView', 2, null, null  , null, null, 'Delete', null, '.LBL_DELETE_BUTTON_LABEL', '.LBL_DELETE_BUTTON_TITLE', null, null, null;
end -- if;
GO


set nocount off;
GO


