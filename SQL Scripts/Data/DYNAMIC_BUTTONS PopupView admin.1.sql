print 'DYNAMIC_BUTTONS Popup admin';
-- delete from DYNAMIC_BUTTONS where VIEW_NAME like '%.PopupView'
--GO

set nocount on;
GO

-- 08/22/2008 Paul.  Move professional modules to a separate file. 
if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = '.PopupView' and DELETED = 0) begin -- then
	print 'DYNAMIC_BUTTONS .PopupView';
	exec dbo.spDYNAMIC_BUTTONS_InsPopupClear  '.PopupView', 0, null, 'list';
	exec dbo.spDYNAMIC_BUTTONS_InsPopupCancel '.PopupView', 1, null, 'list';
end -- if;
GO

exec dbo.spDYNAMIC_BUTTONS_CopyDefault '.PopupView', 'Users.PopupView'            , 'Users'            ;
exec dbo.spDYNAMIC_BUTTONS_CopyDefault '.PopupView', 'Teams.PopupView'            , 'Teams'            ;
GO

-- 08/05/2010 Paul.  Change MultiSelect to use default buttons. 
if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = '.PopupMultiSelect' and DELETED = 0) begin -- then
	print 'DYNAMIC_BUTTONS .PopupMultiSelect';
	exec dbo.spDYNAMIC_BUTTONS_InsPopup     '.PopupMultiSelect', 0, null, 'list', null, null, 'SelectChecked();', null, '.LBL_SELECT_BUTTON_LABEL', '.LBL_SELECT_BUTTON_TITLE', null, null, null;
	exec dbo.spDYNAMIC_BUTTONS_InsPopup     '.PopupMultiSelect', 1, null, 'list', null, null, 'Cancel();'       , null, '.LBL_DONE_BUTTON_LABEL'  , '.LBL_DONE_BUTTON_TITLE'  , null, null, null;
end -- if;
GO

exec dbo.spDYNAMIC_BUTTONS_CopyDefault '.PopupMultiSelect', 'Users.PopupMultiSelect'           , 'Users'           ;
exec dbo.spDYNAMIC_BUTTONS_CopyDefault '.PopupMultiSelect', 'ACLRoles.PopupMultiSelect'        , null              ;
-- 05/04/2016 Paul.  Single role selection in layout editor. 
exec dbo.spDYNAMIC_BUTTONS_CopyDefault '.PopupView'       , 'ACLRoles.PopupView'               , null              ;
exec dbo.spDYNAMIC_BUTTONS_CopyDefault '.PopupMultiSelect', 'Teams.PopupMultiSelect'           , 'Teams'           ;
GO

set nocount off;
GO


