set nocount on;
GO

-- 09/04/2006 Paul.  Remove from EMAIL and OTHER_EMAIL.  This data goes in the EmailOptions panel. 
-- 08/24/2013 Paul.  Add EXTENSION_C in preparation for Asterisk click-to-call. 
-- 09/20/2013 Paul.  Move EXTENSION to the main table. 
-- 01/04/2018 Paul.  Change to Employees.LBL_REPORTS_TO. 
-- 10/29/2020 Paul.  Change to Users.LBL_REPORTS_TO as the Employees module may be disabled. 
if not exists(select * from DETAILVIEWS_FIELDS where DETAIL_NAME = 'Users.DetailView' and DELETED = 0) begin -- then
	print 'DETAILVIEWS_FIELDS Users.DetailView';
	exec dbo.spDETAILVIEWS_InsertOnly          'Users.DetailView'         , 'Users'         , 'vwUSERS_Edit'         , '15%', '35%', null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBoundList 'Users.DetailView'   ,  1, 'Users.LBL_EMPLOYEE_STATUS'       , 'EMPLOYEE_STATUS'                  , '{0}'        , 'employee_status_dom' , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBlank     'Users.DetailView'   ,  2, null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Users.DetailView'   ,  3, 'Users.LBL_TITLE'                 , 'TITLE'                            , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Users.DetailView'   ,  4, 'Users.LBL_OFFICE_PHONE'          , 'PHONE_WORK'                       , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Users.DetailView'   ,  5, 'Users.LBL_DEPARTMENT'            , 'DEPARTMENT'                       , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Users.DetailView'   ,  6, 'Users.LBL_MOBILE_PHONE'          , 'PHONE_MOBILE'                     , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsHyperLink 'Users.DetailView'   ,  7, 'Users.LBL_REPORTS_TO'            , 'REPORTS_TO_NAME'                  , '{0}'        , 'REPORTS_TO_ID'       , '~/Users/view.aspx?ID={0}', null, null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Users.DetailView'   ,  8, 'Users.LBL_OTHER'                 , 'PHONE_OTHER'                      , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Users.DetailView'   ,  9, 'Users.LBL_EXTENSION'             , 'EXTENSION'                        , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Users.DetailView'   , 10, 'Users.LBL_FAX'                   , 'PHONE_FAX'                        , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Users.DetailView'   , 11, 'Users.LBL_FACEBOOK_ID'           , 'FACEBOOK_ID'                      , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Users.DetailView'   , 12, 'Users.LBL_HOME_PHONE'            , 'PHONE_HOME'                       , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBoundList 'Users.DetailView'   , 13, 'Users.LBL_MESSENGER_TYPE'        , 'MESSENGER_TYPE'                   , '{0}'        , 'messenger_type_dom'  , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBlank     'Users.DetailView'   , 14, null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Users.DetailView'   , 15, 'Users.LBL_MESSENGER_ID'          , 'MESSENGER_ID'                     , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBlank     'Users.DetailView'   , 16, null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Users.DetailView'   , 17, 'Users.LBL_ADDRESS'               , 'ADDRESS_HTML'                     , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBlank     'Users.DetailView'   , 18, null;
	exec dbo.spDETAILVIEWS_FIELDS_InsertOnly   'Users.DetailView'   , 19, 'TextBox', 'Users.LBL_NOTES', 'DESCRIPTION', null, null, null, null, null, 3, null;
end else begin
	-- 01/21/2008 Paul.  Some older systems still have EMAIL1 and EMAIL2 in the main. 
	if exists(select * from DETAILVIEWS_FIELDS where DETAIL_NAME = 'Users.DetailView' and DATA_FIELD in ('EMAIL1', 'EMAIL2') and DELETED = 0) begin -- then
		print 'Remove EMAIL1 and EMAIL2 from Users Main panel.';
		update DETAILVIEWS_FIELDS
		   set DELETED          = 1
		     , DATE_MODIFIED    = getdate()
		     , DATE_MODIFIED_UTC= getutcdate()
		     , MODIFIED_USER_ID = null
		 where DETAIL_NAME      = 'Users.DetailView'
		   and DATA_FIELD       in ('EMAIL1', 'EMAIL2')
		   and DELETED          = 0;
	end -- if;
	-- 08/24/2013 Paul.  Add EXTENSION_C in preparation for Asterisk click-to-call. 
	-- 09/20/2013 Paul.  Move EXTENSION to the main table. 
	if exists(select * from DETAILVIEWS_FIELDS where DETAIL_NAME = 'Users.DetailView' and DATA_FIELD = 'EXTENSION_C' and DELETED = 0) begin -- then
		-- 01/21/2018 Paul.  If there already exists an EXTENSION field, then convert to blank. 
		if exists(select * from DETAILVIEWS_FIELDS where DETAIL_NAME = 'Users.DetailView' and DATA_FIELD = 'EXTENSION' and DELETED = 0) begin -- then
			update DETAILVIEWS_FIELDS
			   set FIELD_TYPE       = 'Blank'
			     , DATA_FIELD       = null
			     , DATA_LABEL       = null
			     , DATE_MODIFIED    = getdate()
			     , DATE_MODIFIED_UTC= getutcdate()
			     , MODIFIED_USER_ID = null
			 where DETAIL_NAME      = 'Users.DetailView'
			   and DATA_FIELD       = 'EXTENSION_C'
			   and DELETED          = 0;
		end else begin
			update DETAILVIEWS_FIELDS
			   set DATA_FIELD       = 'EXTENSION'
			     , DATA_LABEL       = 'Users.LBL_EXTENSION'
			     , DATE_MODIFIED    = getdate()
			     , DATE_MODIFIED_UTC= getutcdate()
			     , MODIFIED_USER_ID = null
			 where DETAIL_NAME      = 'Users.DetailView'
			   and DATA_FIELD       = 'EXTENSION_C'
			   and DELETED          = 0;
		end -- if;
	end -- if;
	-- 01/17/2018 Paul.  We noticed multiple EXTENSION records, so check and fix. 
	if exists(select count(*) from DETAILVIEWS_FIELDS where DETAIL_NAME = 'Users.DetailView' and DATA_FIELD = 'EXTENSION' and DEFAULT_VIEW = 0 and DELETED = 0 group by DETAIL_NAME, DATA_FIELD having count(*) > 1) begin -- then
		print 'Users.DetailView: Multiple EXTENSION fields encountered. ';
		update DETAILVIEWS_FIELDS
		   set FIELD_TYPE       = 'Blank'
		     , DATA_FIELD       = null
		     , DATA_LABEL       = null
		     , DATE_MODIFIED    = getdate()
		     , DATE_MODIFIED_UTC= getutcdate()
		     , MODIFIED_USER_ID = null
		 where DETAIL_NAME      = 'Users.DetailView'
		   and DATA_FIELD       = 'EXTENSION'
		   and DELETED          = 0;
	end -- if;
	if not exists(select * from DETAILVIEWS_FIELDS where DETAIL_NAME = 'Users.DetailView' and DATA_FIELD = 'EXTENSION' and DELETED = 0) begin -- then
		exec dbo.spDETAILVIEWS_FIELDS_CnvBound     'Users.DetailView'  ,  9, 'Users.LBL_EXTENSION'              , 'EXTENSION'                        , '{0}'        , null;
	end -- if;
	exec dbo.spDETAILVIEWS_FIELDS_CnvBound     'Users.DetailView'  , 11, 'Users.LBL_FACEBOOK_ID'            , 'FACEBOOK_ID'                      , '{0}'        , null;
	-- 01/04/2018 Paul.  Change to Employees.LBL_REPORTS_TO. 
	if exists(select * from DETAILVIEWS_FIELDS where DETAIL_NAME = 'Users.DetailView' and DATA_FIELD = 'REPORTS_TO_NAME' and DATA_LABEL in ('Contacts.LBL_REPORTS_TO', 'Users.LBL_REPORTS_TO') and DELETED = 0) begin -- then
		-- 02/03/2018 Paul.  This is where the EXTENSION duplicate field problem was re-created. 
		update DETAILVIEWS_FIELDS
		   set DATA_LABEL       = 'Employees.LBL_REPORTS_TO'
		     , DATE_MODIFIED    = getdate()
		     , DATE_MODIFIED_UTC= getutcdate()
		     , MODIFIED_USER_ID = null
		 where DETAIL_NAME      = 'Users.DetailView'
		   and DATA_FIELD       = 'REPORTS_TO_NAME'
		   and DATA_LABEL       in ('Contacts.LBL_REPORTS_TO', 'Users.LBL_REPORTS_TO')
		   and DELETED          = 0;
	end -- if;
end -- if;
GO

set nocount off;
GO


