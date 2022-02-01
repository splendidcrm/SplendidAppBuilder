set nocount on;
GO

-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'SystemLog.SearchBasic';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'SystemLog.SearchBasic' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS SystemLog.SearchBasic';
	exec dbo.spEDITVIEWS_InsertOnly             'SystemLog.SearchBasic'   , 'SystemLog', 'vwSYSTEM_LOG', '11%', '22%', 3;
	exec dbo.spEDITVIEWS_FIELDS_InsControl      'SystemLog.SearchBasic'   ,  0, '.LBL_DATE_ENTERED'                      , 'DATE_ENTERED'               , 0, null, 'DateRange'          , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList    'SystemLog.SearchBasic'   ,  1, 'SystemLog.LBL_ERROR_TYPE'               , 'ERROR_TYPE'                 , 0, null, 'system_log_type_dom', null, 3;
	exec dbo.spEDITVIEWS_FIELDS_InsBound        'SystemLog.SearchBasic'   ,  2, 'SystemLog.LBL_SERVER_HOST'              , 'SERVER_HOST'                , 0, null,  60, 25, null;

	exec dbo.spEDITVIEWS_FIELDS_InsBound        'SystemLog.SearchBasic'   ,  3, 'Users.LBL_USER_NAME'                    , 'USER_NAME'                  , 0, null,  60, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound        'SystemLog.SearchBasic'   ,  4, 'SystemLog.LBL_RELATIVE_PATH'            , 'LBL_RELATIVE_PATH'          , 0, null, 100, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound        'SystemLog.SearchBasic'   ,  5, 'SystemLog.LBL_REMOTE_HOST'              , 'REMOTE_HOST'                , 0, null,  60, 25, null;

	exec dbo.spEDITVIEWS_FIELDS_InsBound        'SystemLog.SearchBasic'   ,  6, 'SystemLog.LBL_MESSAGE'                  , 'MESSAGE'                    , 0, null, 100, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound        'SystemLog.SearchBasic'   ,  7, 'SystemLog.LBL_FILE_NAME'                , 'FILE_NAME'                  , 0, null, 100, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound        'SystemLog.SearchBasic'   ,  8, 'SystemLog.LBL_METHOD'                   , 'METHOD'                     , 0, null, 100, 25, null;
end -- if;
GO

set nocount off;
GO


