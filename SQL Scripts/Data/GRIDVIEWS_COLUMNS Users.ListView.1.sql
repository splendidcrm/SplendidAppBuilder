set nocount on;
GO

-- 10/21/2020 Paul.  Convert USER_NAME into a hyperlink. 
-- 10/21/2020 Paul.  Correct URL to navigate to Administration pages. 
-- 11/10/2020 Paul.  Can't use ~/Administration/Users as this layout is used on React and ASP.NET, but React is easier to re-route. 
if not exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'Users.ListView' and DELETED = 0) begin -- then
	print 'GRIDVIEWS_COLUMNS Users.ListView';
	exec dbo.spGRIDVIEWS_InsertOnly           'Users.ListView'         , 'Users'         , 'vwUSERS_List'         ;
	exec dbo.spGRIDVIEWS_COLUMNS_InsHyperLink 'Users.ListView'             , 1, 'Users.LBL_LIST_NAME'                      , 'FULL_NAME'       , 'FULL_NAME'       , '20%', 'listViewTdLinkS1', 'ID'         , '~/Users/view.aspx?id={0}', null, 'Users', null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsHyperLink 'Users.ListView'             , 2, 'Users.LBL_LIST_USER_NAME'                 , 'USER_NAME'       , 'USER_NAME'       , '20%', 'listViewTdLinkS1', 'ID'         , '~/Users/view.aspx?id={0}', null, 'Users', null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.ListView'             , 3, 'Users.LBL_LIST_DEPARTMENT'                , 'DEPARTMENT'      , 'DEPARTMENT'      , '20%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsHyperLink 'Users.ListView'             , 4, 'Users.LBL_LIST_EMAIL'                     , 'EMAIL1'          , 'EMAIL1'          , '20%', 'listViewTdLinkS1', 'ID'         , '~/Emails/edit.aspx?PARENT_ID={0}', null, null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.ListView'             , 5, 'Users.LBL_LIST_PRIMARY_PHONE'             , 'PHONE_WORK'      , 'PHONE_WORK'      , '15%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.ListView'             , 6, 'Users.LBL_LIST_STATUS'                    , 'STATUS'          , 'STATUS'          , '10%';
end else begin
	exec dbo.spGRIDVIEWS_COLUMNS_ReserveIndex null, 'Users.ListView', 1;
	-- 11/29/2010 Paul.  Create Email record instead of using mailto. 
	if exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'Users.ListView' and DATA_FIELD = 'EMAIL1' and URL_FORMAT = 'mailto:{0}' and DELETED = 0) begin -- then
		print 'GRIDVIEWS_COLUMNS Users.ListView: Create Email record instead of using mailto. ';
		update GRIDVIEWS_COLUMNS
		   set URL_FIELD        = 'ID'
		     , URL_FORMAT        = '~/Emails/edit.aspx?PARENT_ID={0}'
		     , DATE_MODIFIED     = getdate()
		     , DATE_MODIFIED_UTC = getutcdate()
		 where GRID_NAME         = 'Users.ListView'
		   and DATA_FIELD        = 'EMAIL1'
		   and URL_FORMAT        = 'mailto:{0}'
		   and DELETED           = 0;
	end -- if;
	-- 10/21/2020 Paul.  Convert USER_NAME into a hyperlink. 
	if exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'Users.ListView' and DATA_FIELD = 'USER_NAME' and COLUMN_TYPE = 'BoundColumn' and DELETED = 0) begin -- then
		print 'GRIDVIEWS_COLUMNS Users.ListView: Convert USER_NAME into a hyperlink.  ';
		update GRIDVIEWS_COLUMNS
		   set DELETED           = 1
		     , DATE_MODIFIED     = getdate()
		     , DATE_MODIFIED_UTC = getutcdate()
		 where GRID_NAME         = 'Users.ListView'
		   and DATA_FIELD        = 'USER_NAME'
		   and COLUMN_TYPE       = 'BoundColumn'
		   and DELETED           = 0;
		exec dbo.spGRIDVIEWS_COLUMNS_InsHyperLink 'Users.ListView'             , 2, 'Users.LBL_LIST_USER_NAME'                 , 'USER_NAME'       , 'USER_NAME'       , '20%', 'listViewTdLinkS1', 'ID'         , '~/Users/view.aspx?id={0}', null, 'Users', null;
	end -- if;
	-- 10/21/2020 Paul.  Correct URL to navigate to Administration pages. 
	-- 11/10/2020 Paul.  Can't use ~/Administration/Users as this layout is used on React and ASP.NET, but React is easier to re-route. 
	if exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'Users.ListView' and URL_FORMAT = '~/Administration/Users/view.aspx?id={0}' and DELETED = 0) begin -- then
		print 'GRIDVIEWS_COLUMNS Users.ListView: Correct URL to navigate to Administration pages. ';
		update GRIDVIEWS_COLUMNS
		   set URL_FORMAT        = '~/Users/view.aspx?id={0}'
		     , DATE_MODIFIED     = getdate()
		     , DATE_MODIFIED_UTC = getutcdate()
		 where GRID_NAME         = 'Users.ListView'
		   and URL_FORMAT        = '~/Administration/Users/view.aspx?id={0}'
		   and DELETED           = 0;
	end -- if;
end -- if;
GO

set nocount off;
GO


