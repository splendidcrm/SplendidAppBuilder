set nocount on;
GO

-- 12/28/2007 Paul.  UnifiedSearch should be customizable. 
-- delete from DETAILVIEWS_RELATIONSHIPS where DETAIL_NAME = 'Home.UnifiedSearch';
-- 01/01/2008 Paul.  We should not need to fix the search on a clean install. 
-- 08/15/2012 Paul.  Add SearchBugs to Home.UnifiedSearch. 
-- 12/08/2014 Paul.  Add ChatMessages. 
if not exists(select * from DETAILVIEWS_RELATIONSHIPS where DETAIL_NAME = 'Home.UnifiedSearch' and DELETED = 0) begin -- then
	print 'DETAILVIEWS_RELATIONSHIPS Home.UnifiedSearch';
	exec dbo.spDETAILVIEWS_InsertOnly               'Home.UnifiedSearch'       , 'Home'              , 'vwHOME_Edit', '15%', '35%', null;
	exec dbo.spDETAILVIEWS_RELATIONSHIPS_InsertOnly 'Home.UnifiedSearch'       , 'Contacts'         , '~/Contacts/SearchContacts'          , 0, 'Contacts.LBL_LIST_FORM_TITLE'     , null, null, null, null;
	exec dbo.spDETAILVIEWS_RELATIONSHIPS_InsertOnly 'Home.UnifiedSearch'       , 'Accounts'         , '~/Accounts/SearchAccounts'          , 1, 'Accounts.LBL_LIST_FORM_TITLE'     , null, null, null, null;
	exec dbo.spDETAILVIEWS_RELATIONSHIPS_InsertOnly 'Home.UnifiedSearch'       , 'Leads'            , '~/Leads/SearchLeads'                , 2, 'Leads.LBL_LIST_FORM_TITLE'        , null, null, null, null;
	exec dbo.spDETAILVIEWS_RELATIONSHIPS_InsertOnly 'Home.UnifiedSearch'       , 'Opportunities'    , '~/Opportunities/SearchOpportunities', 3, 'Opportunities.LBL_LIST_FORM_TITLE', null, null, null, null;
	exec dbo.spDETAILVIEWS_RELATIONSHIPS_InsertOnly 'Home.UnifiedSearch'       , 'Prospects'        , '~/Prospects/SearchProspects'        , 4, 'Prospects.LBL_LIST_FORM_TITLE'    , null, null, null, null;
	exec dbo.spDETAILVIEWS_RELATIONSHIPS_InsertOnly 'Home.UnifiedSearch'       , 'Cases'            , '~/Cases/SearchCases'                , 5, 'Cases.LBL_LIST_FORM_TITLE'        , null, null, null, null;
	exec dbo.spDETAILVIEWS_RELATIONSHIPS_InsertOnly 'Home.UnifiedSearch'       , 'Project'          , '~/Projects/SearchProjects'          , 6, 'Project.LBL_LIST_FORM_TITLE'      , null, null, null, null;
	exec dbo.spDETAILVIEWS_RELATIONSHIPS_InsertOnly 'Home.UnifiedSearch'       , 'Bugs'             , '~/Bugs/SearchBugs'                  , 7, 'Bugs.LBL_LIST_FORM_TITLE'         , null, null, null, null;
	exec dbo.spDETAILVIEWS_RELATIONSHIPS_InsertOnly 'Home.UnifiedSearch'       , 'ChatMessages'     , '~/ChatMessages/SearchChatMessages'  , 8, 'ChatMessages.LBL_LIST_FORM_TITLE' , null, null, null, null;
end else begin
	-- 08/15/2012 Paul.  Fix Project.LBL_LIST_FORM_TITLE. 
	if exists(select * from DETAILVIEWS_RELATIONSHIPS where DETAIL_NAME = 'Home.UnifiedSearch' and TITLE = 'Projects.LBL_LIST_FORM_TITLE' and DELETED = 0) begin -- then
		update DETAILVIEWS_RELATIONSHIPS
		   set TITLE             = 'Project.LBL_LIST_FORM_TITLE'
		     , DATE_MODIFIED     = getdate()
		     , DATE_MODIFIED_UTC = getutcdate()
		     , MODIFIED_USER_ID  = null
		 where DETAIL_NAME       = 'Home.UnifiedSearch'
		   and TITLE             = 'Projects.LBL_LIST_FORM_TITLE'
		   and DELETED           = 0;
	end -- if;
	exec dbo.spDETAILVIEWS_RELATIONSHIPS_InsertOnly 'Home.UnifiedSearch'       , 'Bugs'             , '~/Bugs/SearchBugs'                  , 7, 'Bugs.LBL_LIST_FORM_TITLE'         , null, null, null, null;
	exec dbo.spDETAILVIEWS_RELATIONSHIPS_InsertOnly 'Home.UnifiedSearch'       , 'ChatMessages'     , '~/ChatMessages/SearchChatMessages'  , 8, 'ChatMessages.LBL_LIST_FORM_TITLE' , null, null, null, null;
end -- if;
GO

set nocount off;
GO

