set nocount on;
GO

-- 09/15/2019 Paul.  The React Client sees True/False and the ASP.NET client sees 1/0.  Need a list that supports both, simultaneously. 
-- 05/01/2016 Paul.  We are going to prepopulate the currency table so that we can be sure to get the supported ISO values correct. 
-- 09/15/2019 Paul.  The React Client sees True/False and the ASP.NET client sees 1/0.  Need a list that supports both, simultaneously. 
-- delete from GRIDVIEWS_COLUMNS where GRID_NAME = 'Currencies.ListView'
if not exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'Currencies.ListView' and DELETED = 0) begin -- then
	print 'GRIDVIEWS_COLUMNS Currencies.ListView';
	exec dbo.spGRIDVIEWS_InsertOnly             'Currencies.ListView', 'Currencies', 'vwCURRENCIES_List';
	exec dbo.spGRIDVIEWS_COLUMNS_InsHyperLink   'Currencies.ListView'  , 2, 'Currencies.LBL_LIST_NAME'                   , 'NAME'             , 'NAME'             , '30%', 'listViewTdLinkS1', 'ID', '~/Administration/Currencies/view.aspx?id={0}', null, 'Currencies', null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBoundList   'Currencies.ListView'  , 3, 'Currencies.LBL_LIST_STATUS'                 , 'STATUS'           , 'STATUS'           , '10%', 'currency_status_dom';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound       'Currencies.ListView'  , 4, 'Currencies.LBL_LIST_ISO4217'                , 'ISO4217'          , 'ISO4217'          , '10%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound       'Currencies.ListView'  , 5, 'Currencies.LBL_LIST_SYMBOL'                 , 'SYMBOL'           , 'SYMBOL'           , '10%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound       'Currencies.ListView'  , 6, 'Currencies.LBL_LIST_CONVERSION_RATE'        , 'CONVERSION_RATE'  , 'CONVERSION_RATE'  , '10%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBoundList   'Currencies.ListView'  , 7, 'Currencies.LBL_LIST_DEFAULT_CURRENCY'       , 'IS_DEFAULT'       , 'IS_DEFAULT'       , '10%', 'yesno_list';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBoundList   'Currencies.ListView'  , 8, 'Currencies.LBL_LIST_BASE_CURRENCY'          , 'IS_BASE'          , 'IS_BASE'          , '10%', 'yesno_list';
	-- 05/01/2016 Paul.  Format is not working, but keep anyway. 
	update GRIDVIEWS_COLUMNS
	   set COLUMN_TYPE        = 'BoundColumn'
	     , DATA_FORMAT        = '{0:F3}'
	 where GRID_NAME          = 'Currencies.ListView'
	   and DATA_FIELD         = 'CONVERSION_RATE'
	   and DELETED            = 0;
end else begin
	-- 09/15/2019 Paul.  The React Client sees True/False and the ASP.NET client sees 1/0.  Need a list that supports both, simultaneously. 
	if exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'Currencies.ListView' and LIST_NAME = 'yesno_dom' and DELETED = 0) begin -- then
		update GRIDVIEWS_COLUMNS
		   set LIST_NAME          = 'yesno_list'
		     , DATE_MODIFIED      = getdate()
		     , DATE_MODIFIED_UTC  = getutcdate()
		 where GRID_NAME          = 'Currencies.ListView'
		   and LIST_NAME          = 'yesno_dom'
		   and DELETED            = 0;
	end -- if;
end -- if;
GO

set nocount off;
GO


