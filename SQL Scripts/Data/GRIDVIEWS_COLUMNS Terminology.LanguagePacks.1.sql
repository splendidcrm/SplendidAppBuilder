set nocount on;
GO

-- 09/11/2021 Paul.  React client. 
-- delete from GRIDVIEWS_COLUMNS where GRID_NAME = 'Terminology.LanguagePacks';
if not exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'Terminology.LanguagePacks' and DELETED = 0) begin -- then
	print 'GRIDVIEWS_COLUMNS Terminology.LanguagePacks';
	exec dbo.spGRIDVIEWS_InsertOnly           'Terminology.LanguagePacks', 'Terminology', 'vwTERMINOLOGY', 'Name', 'asc';
	exec dbo.spGRIDVIEWS_COLUMNS_InsHyperLink 'Terminology.LanguagePacks'        ,  0, 'Terminology.LBL_LIST_IMPORT_NAME'          , 'Name'                     , 'Name'                , '25%', 'listViewTdLinkS1', 'Name', '~/Administration/Terminology/import.aspx?Name={0}', null, null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Terminology.LanguagePacks'        ,  1, 'Terminology.LBL_LIST_IMPORT_DATE'          , 'Date'                     , 'Date'                , '15%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Terminology.LanguagePacks'        ,  2, 'Terminology.LBL_LIST_IMPORT_DESCRIPTION'   , 'Description'              , 'Description'         , '60%';
end -- if;
GO

-- 09/11/2021 Paul.  React client. 
-- delete from GRIDVIEWS_COLUMNS where GRID_NAME = 'Terminology.SplendidLanguagePacks';
if not exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'Terminology.SplendidLanguagePacks' and DELETED = 0) begin -- then
	print 'GRIDVIEWS_COLUMNS Terminology.SplendidLanguagePacks';
	exec dbo.spGRIDVIEWS_InsertOnly           'Terminology.SplendidLanguagePacks', 'Terminology', 'vwTERMINOLOGY', 'Name', 'asc';
	exec dbo.spGRIDVIEWS_COLUMNS_InsHyperLink 'Terminology.SplendidLanguagePacks',  0, 'Terminology.LBL_LIST_IMPORT_NAME'         , 'Name'                     , 'Name'                , '25%', 'listViewTdLinkS1', 'Name', '~/Administration/Terminology/import.aspx?Name={0}', null, null, null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Terminology.SplendidLanguagePacks',  1, 'Terminology.LBL_LIST_IMPORT_DATE'         , 'Date'                     , 'Date'                , '15%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'Terminology.SplendidLanguagePacks',  2, 'Terminology.LBL_LIST_IMPORT_DESCRIPTION'  , 'Description'              , 'Description'         , '60%';
end -- if;
GO

set nocount off;
GO


