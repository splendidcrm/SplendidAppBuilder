print 'DYNAMIC_BUTTONS ListView admin';
-- delete from DYNAMIC_BUTTONS where VIEW_NAME like '%.ListView'
--GO

set nocount on;
GO


-- 06/06/2015 Paul.  New CheckAll buttons for Seven theme. 
if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'CheckAll.ListView' and DELETED = 0) begin -- then
	exec dbo.spDYNAMIC_BUTTONS_InsButtonLink 'CheckAll.ListView', 0, null, null, null, null, 'SelectPage' , '#' , null, '.LBL_SELECT_PAGE' , '.LBL_SELECT_PAGE' , null, null, null, 'SplendidGrid_CheckAll(1); return false';
	exec dbo.spDYNAMIC_BUTTONS_InsButton     'CheckAll.ListView', 1, null, null, null, null, 'SelectAll'  ,       null, '.LBL_SELECT_ALL'  , '.LBL_SELECT_ALL'  , null, null, null;
	exec dbo.spDYNAMIC_BUTTONS_InsButtonLink 'CheckAll.ListView', 2, null, null, null, null, 'DeselectAll', '#' , null, '.LBL_DESELECT_ALL', '.LBL_DESELECT_ALL', null, null, null, 'SplendidGrid_CheckAll(0); return false;';
	update DYNAMIC_BUTTONS
	   set CONTROL_CSSCLASS = 'DataGridOtherButton'
	 where VIEW_NAME = 'CheckAll.ListView';
end -- if;
GO


if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'ACLRoles.ListView' and DELETED = 0) begin -- then
	exec dbo.spDYNAMIC_BUTTONS_InsButtonLink 'ACLRoles.ListView', 0, 'ACLRoles', 'edit', null, null, 'Create', 'edit.aspx', null, '.LBL_CREATE_BUTTON_LABEL', '.LBL_CREATE_BUTTON_TITLE', null, null, null, null;
end -- if;
GO

if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'Dropdown.ListView' and DELETED = 0) begin -- then
	exec dbo.spDYNAMIC_BUTTONS_InsButtonLink 'Dropdown.ListView', 0, 'Dropdown', 'edit', null, null, 'Create', 'edit.aspx', null, '.LBL_CREATE_BUTTON_LABEL', '.LBL_CREATE_BUTTON_TITLE', null, null, null, null;
end -- if;
GO

if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'DynamicButtons.ListView' and DELETED = 0) begin -- then
	exec dbo.spDYNAMIC_BUTTONS_InsButtonLink 'DynamicButtons.ListView', 0, 'DynamicButtons', 'edit', null, null, 'Create', 'edit.aspx', null, '.LBL_CREATE_BUTTON_LABEL', '.LBL_CREATE_BUTTON_TITLE', null, null, null, null;
end -- if;
GO

if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'FieldValidators.ListView' and DELETED = 0) begin -- then
	exec dbo.spDYNAMIC_BUTTONS_InsButtonLink 'FieldValidators.ListView', 0, 'FieldValidators', 'edit', null, null, 'Create', 'edit.aspx', null, '.LBL_CREATE_BUTTON_LABEL', '.LBL_CREATE_BUTTON_TITLE', null, null, null, null;
end -- if;
GO

if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'Schedulers.ListView' and DELETED = 0) begin -- then
	exec dbo.spDYNAMIC_BUTTONS_InsButtonLink 'Schedulers.ListView', 0, 'Schedulers', 'edit', null, null, 'Create', 'edit.aspx', null, '.LBL_CREATE_BUTTON_LABEL', '.LBL_CREATE_BUTTON_TITLE', null, null, null, null;
end -- if;
GO

if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'Shortcuts.ListView' and DELETED = 0) begin -- then
	exec dbo.spDYNAMIC_BUTTONS_InsButtonLink 'Shortcuts.ListView', 0, 'Shortcuts', 'edit', null, null, 'Create', 'edit.aspx', null, '.LBL_CREATE_BUTTON_LABEL', '.LBL_CREATE_BUTTON_TITLE', null, null, null, null;
end -- if;
GO

if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'Terminology.ListView' and DELETED = 0) begin -- then
	exec dbo.spDYNAMIC_BUTTONS_InsButtonLink 'Terminology.ListView', 0, 'Terminology', 'edit', null, null, 'Create', 'edit.aspx', null, '.LBL_CREATE_BUTTON_LABEL', '.LBL_CREATE_BUTTON_TITLE', null, null, null, null;
end -- if;
GO

if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'Users.ListView' and DELETED = 0) begin -- then
	exec dbo.spDYNAMIC_BUTTONS_InsButtonLink 'Users.ListView', 0, 'Users', 'edit', null, null, 'Create', 'edit.aspx', null, '.LBL_CREATE_BUTTON_LABEL', '.LBL_CREATE_BUTTON_TITLE', null, null, null, null;
end -- if;
GO

-- 02/21/2021 Paul.  Languages buttons for React client. 
if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'Languages.ListView' and DELETED = 0) begin -- then
	print 'DYNAMIC_BUTTONS .EditView';
	exec dbo.spDYNAMIC_BUTTONS_InsButton 'Languages.ListView', 0, null, null  , null, null, 'Add'   , null, '.LBL_ADD_BUTTON_LABEL'   , '.LBL_ADD_BUTTON_TITLE'   , null, null, null;
	exec dbo.spDYNAMIC_BUTTONS_InsButton 'Languages.ListView', 1, null, null  , null, null, 'Cancel', null, '.LBL_CANCEL_BUTTON_LABEL', '.LBL_CANCEL_BUTTON_TITLE', null, null, null;
end -- if;
GO

if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'Teams.ListView' and DELETED = 0) begin -- then
	exec dbo.spDYNAMIC_BUTTONS_InsButtonLink 'Teams.ListView', 0, 'Teams', 'edit', null, null, 'Create', 'edit.aspx', null, '.LBL_CREATE_BUTTON_LABEL', '.LBL_CREATE_BUTTON_TITLE', null, null, null, null;
end -- if;
GO


set nocount off;
GO


