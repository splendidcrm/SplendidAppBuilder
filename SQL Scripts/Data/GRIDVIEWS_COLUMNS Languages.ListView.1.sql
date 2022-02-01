set nocount on;
GO

-- 09/15/2019 Paul.  The React Client sees True/False and the ASP.NET client sees 1/0.  Need a list that supports both, simultaneously. 
-- delete from GRIDVIEWS_COLUMNS where GRID_NAME = 'Languages.ListView';
if not exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'Languages.ListView' and DELETED = 0) begin -- then
	print 'GRIDVIEWS_COLUMNS Languages.ListView';
	exec dbo.spGRIDVIEWS_InsertOnly           'Languages.ListView', 'Languages', 'vwLANGUAGES';
	exec dbo.spGRIDVIEWS_COLUMNS_InsHyperLink 'Languages.ListView'     ,  2, 'Terminology.LBL_LIST_LANG'           , 'NAME'            , 'NAME'            , '20%', 'listViewTdLinkS1', 'ID', '~/Administration/Languages/view.aspx?id={0}', null, 'Languages', null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Languages.ListView'     ,  3, 'Terminology.LBL_LIST_NAME_NAME'      , 'DISPLAY_NAME'    , 'DISPLAY_NAME'    , '30%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Languages.ListView'     ,  4, 'Terminology.LBL_LIST_DISPLAY_NAME'   , 'NATIVE_NAME'     , 'NATIVE_NAME'     , '30%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBoundList 'Languages.ListView'     ,  5, 'Administration.LNK_ENABLED'          , 'ACTIVE'          , 'ACTIVE'          , '5%', 'yesno_list';
end else begin
	-- 09/15/2019 Paul.  The React Client sees True/False and the ASP.NET client sees 1/0.  Need a list that supports both, simultaneously. 
	if exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'Languages.ListView' and LIST_NAME = 'yesno_dom' and DELETED = 0) begin -- then
		update GRIDVIEWS_COLUMNS
		   set LIST_NAME          = 'yesno_list'
		     , DATE_MODIFIED      = getdate()
		     , DATE_MODIFIED_UTC  = getutcdate()
		 where GRID_NAME          = 'Languages.ListView'
		   and LIST_NAME          = 'yesno_dom'
		   and DELETED            = 0;
	end -- if;
end -- if;
GO

set nocount off;
GO


