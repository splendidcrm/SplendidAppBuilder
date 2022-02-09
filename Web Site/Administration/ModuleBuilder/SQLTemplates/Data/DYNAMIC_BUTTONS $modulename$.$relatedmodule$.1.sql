
if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = '$modulename$.$relatedmodule$' and DELETED = 0) begin -- then
	print 'DYNAMIC_BUTTONS $modulename$.$relatedmodule$';
	exec dbo.spDYNAMIC_BUTTONS_InsButton '$modulename$.$relatedmodule$', 0, '$modulename$', 'edit', '$relatedmodule$', 'edit', '$relatedmodule$.Create'         , null, '.LBL_NEW_BUTTON_LABEL'   , '.LBL_NEW_BUTTON_TITLE'   , '.LBL_NEW_BUTTON_KEY'   , null, null;
	exec dbo.spDYNAMIC_BUTTONS_InsPopup  '$modulename$.$relatedmodule$', 1, '$modulename$', 'edit', '$relatedmodule$', 'list', '$relatedmodulesingular$Popup();', null, '.LBL_SELECT_BUTTON_LABEL', '.LBL_SELECT_BUTTON_TITLE', '.LBL_SELECT_BUTTON_KEY', null, null;
	exec dbo.spDYNAMIC_BUTTONS_InsButton '$modulename$.$relatedmodule$', 2, '$modulename$', 'view', '$relatedmodule$', 'list', '$relatedmodule$.Search'         , null, '.LBL_SEARCH_BUTTON_LABEL', '.LBL_SEARCH_BUTTON_TITLE', null, null, null;
end -- if;
GO

