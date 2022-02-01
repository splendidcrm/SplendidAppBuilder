print 'RELATIONSHIPS admin';
GO

set nocount on;
GO

exec dbo.spRELATIONSHIPS_InsertOnly 'acl_roles_users'             , 'ACLRoles'     , 'acl_roles'     , 'id', 'Users'         , 'users'        , 'id', 'acl_roles_users', 'role_id', 'user_id', 'many-to-many', null, null, 0;
exec dbo.spRELATIONSHIPS_InsertOnly 'user_direct_reports'         , 'Users'        , 'users'         , 'id', 'Users'         , 'users', 'reports_to_id'   , null, null, null, 'one-to-many', null, null, 0;
exec dbo.spRELATIONSHIPS_InsertOnly 'team_memberships'            , 'Teams'     , 'teams'     , 'id', 'Users'         , 'users'        , 'id', 'team_memberships', 'team_id', 'user_id', 'many-to-many', null, null, 0;
exec dbo.spRELATIONSHIPS_InsertOnly 'team_zipcodes'               , 'Teams'     , 'teams'     , 'id', 'ZipCodes'      , 'zipcodes'     , 'id', 'teams_zipcodes'  , 'team_id', 'zipcode_id', 'many-to-many', null, null, 0;
GO

set nocount off;
GO


