set nocount on;
GO

-- 08/01/2016 Paul.  Roles needed for BPMN. 
-- 10/30/2020 Paul.  Add the DESCRIPTION for the React Client. 
if not exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'ACLRoles.PopupView' and DELETED = 0) begin -- then
	print 'GRIDVIEWS_COLUMNS ACLRoles.PopupView';
	exec dbo.spGRIDVIEWS_InsertOnly           'ACLRoles.PopupView', 'ACLRoles', 'vwACL_ROLES';
	exec dbo.spGRIDVIEWS_COLUMNS_InsHyperLink 'ACLRoles.PopupView'         ,  1, 'ACLRoles.LBL_LIST_NAME'                   , 'NAME'            , 'NAME'            , '49%', 'listViewTdLinkS1', 'ID NAME', 'SelectACLRole(''{0}'', ''{1}'');', null, 'ACLRoles', null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'ACLRoles.PopupView'         ,  2, 'Tags.LBL_LIST_DESCRIPTION'                , 'DESCRIPTION'     , 'DESCRIPTION'     , '49%';
end else begin
	if not exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'ACLRoles.PopupView' and DATA_FIELD = 'DESCRIPTION' and DELETED = 0) begin -- then
		update GRIDVIEWS_COLUMNS
		   set ITEMSTYLE_WIDTH    = '49%'
		     , DATE_MODIFIED      = getdate()
		     , MODIFIED_USER_ID   = null
		 where GRID_NAME          = 'ACLRoles.PopupView'
		   and DATA_FIELD         = 'NAME'
		   and ITEMSTYLE_WIDTH    = '100%'
		   and DELETED            = 0;
		exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'ACLRoles.PopupView'         ,  2, 'Tags.LBL_LIST_DESCRIPTION'                , 'DESCRIPTION'     , 'DESCRIPTION'     , '49%';
	end -- if;
end -- if;
GO

set nocount off;
GO


