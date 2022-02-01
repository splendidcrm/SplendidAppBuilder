set nocount on;
GO

-- 12/02/2009 Paul.  Correct Users.PopupView URL_FIELD.
-- delete from GRIDVIEWS_COLUMNS where GRID_NAME = 'Users.PopupView';
if not exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'Users.PopupView' and DELETED = 0) begin -- then
	print 'GRIDVIEWS_COLUMNS Users.PopupView';
	exec dbo.spGRIDVIEWS_InsertOnly           'Users.PopupView', 'Users', 'vwUSERS_ASSIGNED_TO_List';
	exec dbo.spGRIDVIEWS_COLUMNS_InsHyperLink 'Users.PopupView'             , 1, 'Users.LBL_LIST_NAME'                      , 'FULL_NAME'       , 'FULL_NAME'       , '40%', 'listViewTdLinkS1', 'ID USER_NAME', 'SelectUser(''{0}'', ''{1}'');', null, 'Users', null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsHyperLink 'Users.PopupView'             , 2, 'Users.LBL_LIST_USER_NAME'                 , 'USER_NAME'       , 'USER_NAME'       , '40%', 'listViewTdLinkS1', 'ID USER_NAME', 'SelectUser(''{0}'', ''{1}'');', null, 'Users', null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.PopupView'             , 3, 'Users.LBL_LIST_DEPARTMENT'                , 'DEPARTMENT'      , 'DEPARTMENT'      , '20%';
end else begin
	if exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'Users.PopupView' and URL_FIELD = 'ID FULL_NAME' and DELETED = 0) begin -- then
		print 'Correct Users.PopupView URL_FIELD.';
		update GRIDVIEWS_COLUMNS
		   set URL_FIELD          = 'ID USER_NAME'
		     , DATE_MODIFIED      = getdate()
		     , MODIFIED_USER_ID   = null
		 where GRID_NAME          = 'Users.PopupView'
		   and URL_FIELD          = 'ID FULL_NAME'
		   and DELETED            = 0;
	end -- if;
end -- if;
GO

set nocount off;
GO


