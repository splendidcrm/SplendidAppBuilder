set nocount on;
GO

if not exists(select * from DETAILVIEWS_RELATIONSHIPS where DETAIL_NAME = 'Administration.ListView' and DELETED = 0) begin -- then
	print 'DETAILVIEWS_RELATIONSHIPS Administration.ListView';
	exec dbo.spDETAILVIEWS_InsertOnly               'Administration.ListView'   , 'Administration'   , 'vwADMINISTRATION', '20%', '30%', null;
	exec dbo.spDETAILVIEWS_RELATIONSHIPS_InsertOnly 'Administration.ListView'   , 'Administration'   , 'SystemView'         ,  1, 'Administration.LBL_ADMINISTRATION_HOME_TITLE' , null, null, null, null;
	exec dbo.spDETAILVIEWS_RELATIONSHIPS_InsertOnly 'Administration.ListView'   , 'Administration'   , 'UsersView'          ,  2, 'Administration.LBL_USERS_TITLE'               , null, null, null, null;
	exec dbo.spDETAILVIEWS_RELATIONSHIPS_InsertOnly 'Administration.ListView'   , 'Administration'   , 'StudioView'         ,  3, 'Administration.LBL_STUDIO_TITLE'              , null, null, null, null;
end -- if;
GO

set nocount off;
GO


