set nocount on;
GO

-- 01/29/2021 Paul.  Add EditCustomFields to React client. 
-- 02/22/2021 Paul.  Make use of new procedure spGRIDVIEWS_COLUMNS_UpdateFormat.
-- delete from GRIDVIEWS_COLUMNS where GRID_NAME = 'EditCustomFields.ListView';
if not exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'EditCustomFields.ListView' and DELETED = 0) begin -- then
	print 'GRIDVIEWS_COLUMNS EditCustomFields.ListView';
	exec dbo.spGRIDVIEWS_InsertOnly           'EditCustomFields.ListView', 'EditCustomFields', 'vwFIELDS_META_DATA_List';
	exec dbo.spGRIDVIEWS_COLUMNS_InsHyperLink 'EditCustomFields.ListView',  2, 'EditCustomFields.COLUMN_TITLE_NAME'           , 'NAME'            , 'NAME'           , '22%', 'listViewTdLinkS1', 'ID', '~/Administration/EditCustomFields/edit.aspx?id={0}', null, 'EditCustomFields', null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'EditCustomFields.ListView',  3, 'EditCustomFields.COLUMN_TITLE_LABEL'          , 'LABEL'           , 'LABEL'          , '22%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'EditCustomFields.ListView',  4, 'EditCustomFields.COLUMN_TITLE_DATA_TYPE'      , 'DATA_TYPE'       , 'DATA_TYPE'      , '10%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'EditCustomFields.ListView',  5, 'EditCustomFields.COLUMN_TITLE_MAX_SIZE'       , 'MAX_SIZE'        , 'MAX_SIZE'       , '10%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'EditCustomFields.ListView',  6, 'EditCustomFields.COLUMN_TITLE_REQUIRED_OPTION', 'REQUIRED_OPTION' , 'REQUIRED_OPTION', '10%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'EditCustomFields.ListView',  7, 'EditCustomFields.COLUMN_TITLE_DEFAULT_VALUE'  , 'DEFAULT_VALUE'   , 'DEFAULT_VALUE'  , '10%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'EditCustomFields.ListView',  8, 'EditCustomFields.COLUMN_TITLE_DROPDOWN'       , 'EXT1'            , 'EXT1'           , '10%';
	exec dbo.spGRIDVIEWS_COLUMNS_UpdateFormat null, 'EditCustomFields.ListView',  'MAX_SIZE', '{0:N0}';
end -- if;
GO

set nocount off;
GO


