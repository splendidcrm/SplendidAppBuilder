set nocount on;
GO

-- 08/01/2010 Paul.  We need a separate view to select the Full Name instead of the User Name. 
if not exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'Users.PopupViewName' and DELETED = 0) begin -- then
	print 'GRIDVIEWS_COLUMNS Users.PopupViewName';
	exec dbo.spGRIDVIEWS_InsertOnly           'Users.PopupViewName', 'Users', 'vwUSERS_ASSIGNED_TO_List';
	exec dbo.spGRIDVIEWS_COLUMNS_InsHyperLink 'Users.PopupViewName'         , 1, 'Users.LBL_LIST_NAME'                      , 'FULL_NAME'       , 'FULL_NAME'       , '40%', 'listViewTdLinkS1', 'ID FULL_NAME', 'SelectUser(''{0}'', ''{1}'');', null, 'Users', null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsHyperLink 'Users.PopupViewName'         , 2, 'Users.LBL_LIST_USER_NAME'                 , 'USER_NAME'       , 'USER_NAME'       , '40%', 'listViewTdLinkS1', 'ID FULL_NAME', 'SelectUser(''{0}'', ''{1}'');', null, 'Users', null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.PopupViewName'         , 3, 'Users.LBL_LIST_DEPARTMENT'                , 'DEPARTMENT'      , 'DEPARTMENT'      , '20%';
end -- if;
GO

set nocount off;
GO


