set nocount on;
GO

-- 05/01/2016 Paul.  We are going to prepopulate the currency table so that we can be sure to get the supported ISO values correct. 
-- delete from EDITVIEWS where NAME = 'Currencies.SearchBasic';
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'Currencies.SearchBasic';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Currencies.SearchBasic' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Currencies.SearchBasic';
	exec dbo.spEDITVIEWS_InsertOnly             'Currencies.SearchBasic'      , 'Currencies', 'vwCURRENCIES_List', '11%', '22%', 3;
	exec dbo.spEDITVIEWS_FIELDS_InsBound        'Currencies.SearchBasic'      ,  0, 'Currencies.LBL_NAME'                    , 'NAME'                       , 0, null,  36, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound        'Currencies.SearchBasic'      ,  1, 'Currencies.LBL_ISO4217'                 , 'ISO4217'                    , 0, null,  36, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList    'Currencies.SearchBasic'      ,  2, 'Currencies.LBL_STATUS'                  , 'STATUS'                     , 0, null, 'currency_status_dom' , null, 2;
end -- if;
GO

set nocount off;
GO


