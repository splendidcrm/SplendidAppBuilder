set nocount on;
GO

if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'ACLRoles.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS ACLRoles.EditView';
	exec dbo.spEDITVIEWS_InsertOnly            'ACLRoles.EditView', 'ACLRoles', 'vwACL_ROLES_Edit', '15%', '85%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'ACLRoles.EditView'       ,  0, 'ACLRoles.LBL_NAME'                      , 'NAME'                       , 1, 1, 150, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'ACLRoles.EditView'       ,  1, null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'ACLRoles.EditView'       ,  2, 'ACLRoles.LBL_DESCRIPTION'               , 'DESCRIPTION'                , 0, 2,   8, 60, null;
end -- if;
GO

set nocount off;
GO


