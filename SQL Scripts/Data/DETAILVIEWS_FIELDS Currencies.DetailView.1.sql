set nocount on;
GO

-- 05/01/2016 Paul.  We are going to prepopulate the currency table so that we can be sure to get the supported ISO values correct. 
-- delete from DETAILVIEWS_FIELDS where DETAIL_NAME = 'Currencies.DetailView';
if not exists(select * from DETAILVIEWS_FIELDS where DETAIL_NAME = 'Currencies.DetailView' and DELETED = 0) begin -- then
	print 'DETAILVIEWS_FIELDS Currencies.DetailView';
	exec dbo.spDETAILVIEWS_InsertOnly          'Currencies.DetailView', 'Currencies', 'vwCURRENCIES_Edit', '15%', '35', 2;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Currencies.DetailView',  0, 'Currencies.LBL_NAME'                  , 'NAME'                                          , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Currencies.DetailView',  1, 'Currencies.LBL_ISO4217'               , 'ISO4217'                                       , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Currencies.DetailView',  2, 'Currencies.LBL_CONVERSION_RATE'       , 'CONVERSION_RATE'                               , '{0:F3}'     , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Currencies.DetailView',  3, 'Currencies.LBL_SYMBOL'                , 'SYMBOL'                                        , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBoundList 'Currencies.DetailView',  4, 'Currencies.LBL_STATUS'                , 'STATUS'                                        , '{0}'        , 'currency_status_dom', null;
	exec dbo.spDETAILVIEWS_FIELDS_InsCheckBox  'Currencies.DetailView',  5, 'Currencies.LBL_DEFAULT_CURRENCY'      , 'IS_DEFAULT'                                    , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Currencies.DetailView',  6, '.LBL_DATE_MODIFIED'                   , 'DATE_MODIFIED .LBL_BY MODIFIED_BY_NAME', '{0} {1} {2}', null;
	exec dbo.spDETAILVIEWS_FIELDS_InsCheckBox  'Currencies.DetailView',  7, 'Currencies.LBL_BASE_CURRENCY'         , 'IS_BASE'                                       , null;
end -- if;
GO

set nocount off;
GO


