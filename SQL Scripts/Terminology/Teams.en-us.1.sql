

-- Terminology generated from database [SplendidCRM5_50] on 11/18/2010 1:19:42 AM.
print 'TERMINOLOGY Teams en-us';
GO

set nocount on;
GO

exec dbo.spTERMINOLOGY_InsertOnly N'ERR_INVALID_TEAM'                              , N'en-US', N'Teams', null, null, N'Invalid Team.';
exec dbo.spTERMINOLOGY_InsertOnly N'ERR_TEAM_NOT_FOUND'                            , N'en-US', N'Teams', null, null, N'Team Not Found.';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_ADD_TEAM_SET'                              , N'en-US', N'Teams', null, null, N'Add';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_DESCRIPTION'                               , N'en-US', N'Teams', null, null, N'Description:';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_DISABLE'                                   , N'en-US', N'Teams', null, null, N'Disable';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_DYNAMIC'                                   , N'en-US', N'Teams', null, null, N'Dynamic';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_ENABLE'                                    , N'en-US', N'Teams', null, null, N'Enable';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_LIST_DESCRIPTION'                          , N'en-US', N'Teams', null, null, N'Description';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_LIST_FORM_TITLE'                           , N'en-US', N'Teams', null, null, N'Team List';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_LIST_MEMBERSHIP'                           , N'en-US', N'Teams', null, null, N'Membership';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_LIST_NAME'                                 , N'en-US', N'Teams', null, null, N'Name';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_LIST_PRIMARY_TEAM'                         , N'en-US', N'Teams', null, null, N'Primary';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_LIST_PRIVATE'                              , N'en-US', N'Teams', null, null, N'Private';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_LIST_TEAM'                                 , N'en-US', N'Teams', null, null, N'Team';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_NAME'                                      , N'en-US', N'Teams', null, null, N'Name:';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_OPTIONAL'                                  , N'en-US', N'Teams', null, null, N'Optional';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_PRIVATE'                                   , N'en-US', N'Teams', null, null, N'Private:';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_PRIVATE_TEAM_DESC'                         , N'en-US', N'Teams', null, null, N'Private team for ';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_REPLACE_TEAM_SET'                          , N'en-US', N'Teams', null, null, N'Replace';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_REQUIRE'                                   , N'en-US', N'Teams', null, null, N'Require';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_SEARCH_FORM_TITLE'                         , N'en-US', N'Teams', null, null, N'Search';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_SINGULAR'                                  , N'en-US', N'Teams', null, null, N'Singular';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_TEAM'                                      , N'en-US', N'Teams', null, null, N'Team:';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_TEAMS'                                     , N'en-US', N'Teams', null, null, N'Teams';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_TEAMS_DISABLED'                            , N'en-US', N'Teams', null, null, N'Currently Disabled';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_TEAMS_DYNAMIC'                             , N'en-US', N'Teams', null, null, N' and Dynamic';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_TEAMS_ENABLED'                             , N'en-US', N'Teams', null, null, N'Currently Enabled';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_TEAMS_NOT_DYNAMIC'                         , N'en-US', N'Teams', null, null, N' and Not Dynamic';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_TEAMS_NOT_REQUIRED'                        , N'en-US', N'Teams', null, null, N' and Not Required';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_TEAMS_REQUIRED'                            , N'en-US', N'Teams', null, null, N' and Required';
exec dbo.spTERMINOLOGY_InsertOnly N'LNK_NEW_TEAM'                                  , N'en-US', N'Teams', null, null, N'Create Team';
exec dbo.spTERMINOLOGY_InsertOnly N'LNK_TEAM_LIST'                                 , N'en-US', N'Teams', null, null, N'Teams';
-- 06/04/2015 Paul.  Add module abbreviation. 
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_MODULE_ABBREVIATION'                       , N'en-US', N'Teams', null, null, N'Tm';
-- 04/12/2016 Paul.  Add parent team and custom fields. 
-- 04/28/2016 Paul.  Rename parent to PARENT_ID. 
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_PARENT_NAME'                               , N'en-US', N'Teams', null, null, N'Parent Team:';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_PARENT_ID'                                 , N'en-US', N'Teams', null, null, N'Parent Team:';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_LIST_PARENT_NAME'                          , N'en-US', N'Teams', null, null, N'Parent Team';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_TEAM_HIERARCHY'                            , N'en-US', N'Teams', null, null, N'Team Hierarchy';
-- 10/16/2020 Paul.  LBL_HIERARCHY is used on the Team DetailView page. 
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_HIERARCHY'                                 , N'en-US', N'Teams', null, null, N'Team Hierarchy';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_HIERARCHICAL'                              , N'en-US', N'Teams', null, null, N'Hierarchical';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_NON_HIERARCHICAL'                          , N'en-US', N'Teams', null, null, N'Non-hierarchical';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_TEAMS_HIERARCHICAL'                        , N'en-US', N'Teams', null, null, N' and Hierarchical';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_TEAMS_NON_HIERARCHICAL'                    , N'en-US', N'Teams', null, null, N' and Non-hierarchical';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_CURRENT'                                   , N'en-US', N'Teams', null, null, N'Current:';
-- 01/08/2018 Paul.  Change the name from Teams. 
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_TEAM_TREE_ROOT'                            , N'en-US', N'Teams', null, null, N'Teams';
GO
/* -- #if Oracle
	COMMIT WORK;
END;
/

BEGIN
-- #endif Oracle */
exec dbo.spTERMINOLOGY_InsertOnly N'Teams'                                         , N'en-US', null, N'moduleList'                        ,  44, N'Teams';
-- 03/27/2018 Paul.  This is used for Undelete Team. 
exec dbo.spTERMINOLOGY_InsertOnly N'Teams'                                         , N'en-US', null, N'moduleListSingular'                ,  44, N'Team';

exec dbo.spTERMINOLOGY_InsertOnly N'Member'                                        , N'en-US', null, N'team_membership_dom'               ,   1, N'Member';
exec dbo.spTERMINOLOGY_InsertOnly N'Member Reports-to'                             , N'en-US', null, N'team_membership_dom'               ,   2, N'Member reports-to';

-- 02/22/2017 Paul.  Convert to ctlSearchView. 
exec dbo.spTERMINOLOGY_InsertOnly N'0'                                             , N'en-US', null, N'team_private_dom'                  ,   1, N'No';
exec dbo.spTERMINOLOGY_InsertOnly N'1'                                             , N'en-US', null, N'team_private_dom'                  ,   2, N'Yes';

-- 10/14/2020 Paul.  Easier to create list for EXPLICIT_ASSIGN than to create custom view. 
exec dbo.spTERMINOLOGY_InsertOnly N'0'                                             , N'en-US', null, N'team_explicit_assign'              ,   2, N'Member reports-to';
exec dbo.spTERMINOLOGY_InsertOnly N'1'                                             , N'en-US', null, N'team_explicit_assign'              ,   1, N'Member';

GO


set nocount off;
GO

/* -- #if Oracle
	COMMIT WORK;
END;
/
-- #endif Oracle */

/* -- #if IBM_DB2
	commit;
  end
/

call dbo.spTERMINOLOGY_Teams_en_us()
/

call dbo.spSqlDropProcedure('spTERMINOLOGY_Teams_en_us')
/
-- #endif IBM_DB2 */
