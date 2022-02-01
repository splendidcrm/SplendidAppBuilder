set nocount on;
GO

-- delete from GRIDVIEWS_COLUMNS where GRID_NAME = 'ACLRoles.ListView'
if not exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'ACLRoles.ListView' and DELETED = 0) begin -- then
	print 'GRIDVIEWS_COLUMNS ACLRoles.ListView';
	exec dbo.spGRIDVIEWS_InsertOnly           'ACLRoles.ListView', 'ACLRoles', 'vwACL_ROLES_List';
	exec dbo.spGRIDVIEWS_COLUMNS_InsHyperLink 'ACLRoles.ListView'          , 1, 'ACLRoles.LBL_NAME'                        , 'NAME'            , 'NAME'            , '20%', 'listViewTdLinkS1', 'ID', '~/Administration/ACLRoles/view.aspx?id={0}', null, 'ACLRoles', null;
end -- if;
GO

set nocount off;
GO


