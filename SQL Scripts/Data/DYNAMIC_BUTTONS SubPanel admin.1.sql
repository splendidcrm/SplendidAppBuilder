print 'DYNAMIC_BUTTONS SubPanel admin';

set nocount on;
GO

if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'ACLRoles.Users' and DELETED = 0) begin -- then
	print 'DYNAMIC_BUTTONS ACLRoles SubPanel';
	exec dbo.spDYNAMIC_BUTTONS_InsPopup     'ACLRoles.Users'                  , 0, 'ACLRoles'        , 'edit', 'Users'           , 'edit', 'UserMultiSelect();'      , null, '.LBL_SELECT_BUTTON_LABEL'                    , '.LBL_SELECT_BUTTON_TITLE'                    , null, null, null;
end -- if;
GO

if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'Users.ACLRoles' and DELETED = 0) begin -- then
	print 'DYNAMIC_BUTTONS Users SubPanel';
	exec dbo.spDYNAMIC_BUTTONS_InsPopup     'Users.ACLRoles'                  , 0, 'Users'           , 'edit', 'ACLRoles'        , 'edit', 'RoleMultiSelect();'      , null, '.LBL_SELECT_BUTTON_LABEL'                    , '.LBL_SELECT_BUTTON_TITLE'                    , null, null, null;
end -- if;
GO

if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'Teams.Users' and DELETED = 0) begin -- then
	print 'DYNAMIC_BUTTONS Teams.Users SubPanel';
	exec dbo.spDYNAMIC_BUTTONS_InsPopup     'Teams.Users'                     , 1, 'Teams'           , 'edit', 'Users'           , 'edit', 'UserMultiSelect();'      , null, '.LBL_SELECT_BUTTON_LABEL'                    , '.LBL_SELECT_BUTTON_TITLE'                    , null, null, null;
end -- if;
GO

if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'Users.Teams' and DELETED = 0) begin -- then
	print 'DYNAMIC_BUTTONS Users.Teams SubPanel';
	exec dbo.spDYNAMIC_BUTTONS_InsPopup     'Users.Teams'                     , 0, 'Users'           , 'edit', 'Teams'           , 'edit', 'TeamMultiSelect();'      , null, '.LBL_SELECT_BUTTON_LABEL'                    , '.LBL_SELECT_BUTTON_TITLE'                    , null, null, null;
end -- if;
GO

set nocount off;
GO


