-- sp_helptext spDETAILVIEWS_RELATIONSHIPS_InsertOnly
if not exists(select * from DETAILVIEWS_RELATIONSHIPS where DETAIL_NAME = 'Teams.DetailView' and DELETED = 0) begin -- then
	print 'DETAILVIEWS_RELATIONSHIPS Teams.DetailView Professional';
	exec dbo.spDETAILVIEWS_RELATIONSHIPS_InsertOnly 'Teams.DetailView'         , 'Users'            , 'Users'              ,  1, 'Users.LBL_MODULE_NAME'            , 'vwTEAM_MEMBERSHIPS_List', 'TEAM_ID', 'FULL_NAME', 'asc';
end -- if;
GO

