set nocount on;
GO

-- delete from DETAILVIEWS_FIELDS where DETAIL_NAME = 'Schedulers.DetailView';
if not exists(select * from DETAILVIEWS_FIELDS where DETAIL_NAME = 'Schedulers.DetailView' and DELETED = 0) begin -- then
	print 'DETAILVIEWS_FIELDS Schedulers.DetailView';
	exec dbo.spDETAILVIEWS_InsertOnly           'Schedulers.DetailView', 'Schedulers', 'vwSCHEDULERS', '15%', '35%', null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound      'Schedulers.DetailView'    ,  0, 'Schedulers.LBL_NAME'                  , 'NAME'               , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBoundList  'Schedulers.DetailView'    ,  1, 'Schedulers.LBL_STATUS'                , 'STATUS'             , '{0}'        , 'scheduler_status_dom', null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound      'Schedulers.DetailView'    ,  2, 'Schedulers.LBL_JOB'                   , 'JOB'                , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBlank      'Schedulers.DetailView'    ,  3, null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound      'Schedulers.DetailView'    ,  4, 'Schedulers.LBL_DATE_TIME_START'       , 'DATE_TIME_START'    , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound      'Schedulers.DetailView'    ,  5, 'Schedulers.LBL_TIME_FROM'             , 'TIME_FROM'          , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound      'Schedulers.DetailView'    ,  6, 'Schedulers.LBL_DATE_TIME_END'         , 'DATE_TIME_END'      , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound      'Schedulers.DetailView'    ,  7, 'Schedulers.LBL_TIME_TO'               , 'TIME_TO'            , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound      'Schedulers.DetailView'    ,  8, 'Schedulers.LBL_LAST_RUN'              , 'LAST_RUN'           , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound      'Schedulers.DetailView'    ,  9, 'Schedulers.LBL_INTERVAL'              , 'JOB_INTERVAL'       , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound      'Schedulers.DetailView'    , 10, 'Schedulers.LBL_CATCH_UP'              , 'CATCH_UP'           , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBlank      'Schedulers.DetailView'    , 11, null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound      'Schedulers.DetailView'    , 12, '.LBL_DATE_ENTERED'                    , 'DATE_ENTERED'       , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound      'Schedulers.DetailView'    , 13, '.LBL_DATE_MODIFIED'                   , 'DATE_MODIFIED'      , '{0}'        , null;
end else begin
	-- 03/29/2021 Paul.  INTERVAL is not a valid field. 
	if exists(select * from DETAILVIEWS_FIELDS where DETAIL_NAME = 'Schedulers.DetailView' and DATA_FIELD = 'INTERVAL' and DELETED = 0) begin -- then
		update DETAILVIEWS_FIELDS
		   set DATA_FIELD        = 'JOB_INTERVAL'
		     , DATE_MODIFIED     = getdate()
		     , DATE_MODIFIED_UTC = getutcdate()     
		     , MODIFIED_USER_ID  = null
		 where DETAIL_NAME       = 'Schedulers.DetailView'
		   and DATA_FIELD        = 'INTERVAL'
		   and DELETED           = 0;
	end -- if;
end -- if;
GO


set nocount off;
GO


