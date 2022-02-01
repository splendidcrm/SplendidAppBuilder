set nocount on;
GO

-- 09/12/2019 Paul.  Users.ACLRoles for the React Client. 
-- 11/10/2020 Paul.  Use new CheckBox type. 
-- delete from GRIDVIEWS_COLUMNS where GRID_NAME = 'Users.ACLRoles'
if not exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'Users.ACLRoles' and DELETED = 0) begin -- then
	print 'GRIDVIEWS_COLUMNS Users.ACLRoles';
	exec dbo.spGRIDVIEWS_InsertOnly           'Users.ACLRoles', 'ACLRoles', 'vwUSERS_ACL_ROLES';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.ACLRoles'           , 1, 'ACLRoles.LBL_LIST_NAME'                     , 'ROLE_NAME'              , 'ROLE_NAME'                     , '30%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Users.ACLRoles'           , 2, 'ACLRoles.LBL_LIST_DESCRIPTION'              , 'DESCRIPTION'            , 'DESCRIPTION'                   , '50%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBoundDate 'Users.ACLRoles'           , 3, 'ACLRoles.LBL_IS_PRIMARY_ROLE'               , 'IS_PRIMARY_ROLE'        , 'IS_PRIMARY_ROLE'               , '10%', 'CheckBox';
end else begin
	if exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'Users.ACLRoles' and DATA_FIELD = 'IS_PRIMARY_ROLE' and COLUMN_TYPE = 'BoundColumn' and DELETED = 0) begin -- then
		update GRIDVIEWS_COLUMNS
		  set COLUMN_TYPE       = 'TemplateColumn'
		    , DATA_FORMAT       = 'CheckBox'
		    , DATE_MODIFIED     = getdate()
		    , DATE_MODIFIED_UTC = getutcdate()
		 where GRID_NAME        = 'Users.ACLRoles'
		   and DATA_FIELD       = 'IS_PRIMARY_ROLE'
		   and COLUMN_TYPE      = 'BoundColumn'
		   and DELETED          = 0;
	end -- if;
end -- if;
GO

set nocount off;
GO


