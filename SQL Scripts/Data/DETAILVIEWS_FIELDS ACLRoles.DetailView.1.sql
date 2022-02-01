set nocount on;
GO

if not exists(select * from DETAILVIEWS_FIELDS where DETAIL_NAME = 'ACLRoles.DetailView' and DELETED = 0) begin -- then
	print 'DETAILVIEWS_FIELDS ACLRoles.DetailView';
	exec dbo.spDETAILVIEWS_InsertOnly          'ACLRoles.DetailView', 'ACLRoles', 'vwACL_ROLES_Edit', '15%', '35%', null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'ACLRoles.DetailView' ,  0, 'ACLRoles.LBL_NAME'              , 'NAME'                             , '{0}'        , 3;
	exec dbo.spDETAILVIEWS_FIELDS_InsertOnly   'ACLRoles.DetailView' ,  1, 'TextBox', 'ACLRoles.LBL_DESCRIPTION', 'DESCRIPTION', null, null, null, null, null, 3, null;
end -- if;
GO

