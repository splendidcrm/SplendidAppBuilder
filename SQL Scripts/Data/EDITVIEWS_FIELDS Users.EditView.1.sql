set nocount on;
GO

--delete from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.EditView'
-- 09/04/2006 Paul.  Remove from EMAIL and OTHER_EMAIL.  This data goes in the EmailOptions panel. 
-- 07/08/2010 Paul.  Move Users.EditAddress fields to Users.EditView
-- 08/24/2013 Paul.  Add EXTENSION_C in preparation for Asterisk click-to-call. 
-- 09/20/2013 Paul.  Move EXTENSION to the main table. 
-- 09/27/2013 Paul.  SMS messages need to be opt-in. 
-- 10/12/2020 Paul.  Employees module may be disabled, so make sure to define LBL_REPORTS_TO for use on Users.EditView. 
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Users.EditView';
	exec dbo.spEDITVIEWS_InsertOnly            'Users.EditView'         , 'Users'         , 'vwUSERS_Edit'         , '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Users.EditView'          ,  1, 'Users.LBL_EMPLOYEE_STATUS'              , 'EMPLOYEE_STATUS'            , 0, 5, 'employee_status_dom' , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Users.EditView'          ,  2, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditView'          ,  3, 'Users.LBL_TITLE'                        , 'TITLE'                      , 0, 5,  50, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditView'          ,  4, 'Users.LBL_OFFICE_PHONE'                 , 'PHONE_WORK'                 , 0, 6,  25, 20, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Users.EditView'          ,  4, 'Phone Number'                           , 'PHONE_WORK'                 , '.ERR_INVALID_PHONE_NUMBER';
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditView'          ,  5, 'Users.LBL_DEPARTMENT'                   , 'DEPARTMENT'                 , 0, 5,  50, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditView'          ,  6, 'Users.LBL_MOBILE_PHONE'                 , 'PHONE_MOBILE'               , 0, 6,  25, 20, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Users.EditView'          ,  6, 'Phone Number'                           , 'PHONE_MOBILE'               , '.ERR_INVALID_PHONE_NUMBER';
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Users.EditView'          ,  7, 'Users.LBL_REPORTS_TO'                   , 'REPORTS_TO_ID'              , 0, 5, 'REPORTS_TO_NAME'     , 'Users', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditView'          ,  8, 'Users.LBL_OTHER'                        , 'PHONE_OTHER'                , 0, 6,  25, 20, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Users.EditView'          ,  8, 'Phone Number'                           , 'PHONE_OTHER'                , '.ERR_INVALID_PHONE_NUMBER';
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditView'          ,  9, 'Users.LBL_EXTENSION'                    , 'EXTENSION'                  , 0, 5,  25, 20, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditView'          , 10, 'Users.LBL_FAX'                          , 'PHONE_FAX'                  , 0, 6,  25, 20, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Users.EditView'          , 10, 'Phone Number'                           , 'PHONE_FAX'                  , '.ERR_INVALID_PHONE_NUMBER';
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditView'          , 11, 'Users.LBL_FACEBOOK_ID'                  , 'FACEBOOK_ID'                , 0, 5,  25, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditView'          , 12, 'Users.LBL_HOME_PHONE'                   , 'PHONE_HOME'                 , 0, 6,  25, 20, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Users.EditView'          , 12, 'Phone Number'                           , 'PHONE_HOME'                 , '.ERR_INVALID_PHONE_NUMBER';
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Users.EditView'          , 13, 'Users.LBL_MESSENGER_TYPE'               , 'MESSENGER_TYPE'             , 0, 5, 'messenger_type_dom'  , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditView'          , 14, 'Users.LBL_MESSENGER_ID'                 , 'MESSENGER_ID'               , 0, 5,  25, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Users.EditView'          , 15, 'Users.LBL_PRIMARY_ADDRESS'              , 'ADDRESS_STREET'             , 0, 8,   2, 30, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditView'          , 16, 'Users.LBL_CITY'                         , 'ADDRESS_CITY'               , 0, 8, 100, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditView'          , 17, 'Users.LBL_STATE'                        , 'ADDRESS_STATE'              , 0, 8, 100, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditView'          , 18, 'Users.LBL_POSTAL_CODE'                  , 'ADDRESS_POSTALCODE'         , 0, 8,  20, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditView'          , 19, 'Users.LBL_COUNTRY'                      , 'ADDRESS_COUNTRY'            , 0, 8,  20, 10, null;
	-- 09/27/2013 Paul.  SMS messages need to be opt-in. 
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Users.EditView'          , 20, 'Users.LBL_SMS_OPT_IN'                   , 'SMS_OPT_IN'                 , 0, 1, 'dom_sms_opt_in'     , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Users.EditView'          , 21, 'Users.LBL_NOTES'                        , 'DESCRIPTION'                , 0, 7,   4, 80, 3;
end else begin
	-- 08/27/2009 Paul.  Convert the ChangeButton to a ModulePopup. 
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'Users.EditView'          ,  7, 'Contacts.LBL_REPORTS_TO'                , 'REPORTS_TO_ID'              , 0, 5, 'REPORTS_TO_NAME'     , 'Users', null;

	-- 03/25/2011 Paul.  Create a separate field for the Facebook ID. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.EditView' and DATA_FIELD = 'FACEBOOK_ID' and DELETED = 0) begin -- then
		if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.EditView' and FIELD_TYPE = 'Blank' and FIELD_INDEX = 11 and DELETED = 0) begin -- then
			update EDITVIEWS_FIELDS
			   set DELETED          = 1
			     , DATE_MODIFIED    = getdate()
			     , DATE_MODIFIED_UTC= getutcdate()
			     , MODIFIED_USER_ID = null
			 where EDIT_NAME        = 'Users.EditView'
			   and FIELD_TYPE       = 'Blank'
			   and FIELD_INDEX      = 11
			   and DELETED          = 0;
		end -- if;
		exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditView'          , 11, 'Users.LBL_FACEBOOK_ID'                  , 'FACEBOOK_ID'                , 0, 5,  25, 35, null;
	end -- if;

	-- 08/24/2013 Paul.  Add EXTENSION_C in preparation for Asterisk click-to-call. 
	-- 09/20/2013 Paul.  Move EXTENSION to the main table. 
	if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.EditView' and DATA_FIELD = 'EXTENSION_C' and DELETED = 0) begin -- then
			update EDITVIEWS_FIELDS
			   set DATA_FIELD       = 'EXTENSION'
			     , DATA_LABEL       = 'Users.LBL_EXTENSION'
			     , DATE_MODIFIED    = getdate()
			     , DATE_MODIFIED_UTC= getutcdate()
			     , MODIFIED_USER_ID = null
			 where EDIT_NAME        = 'Users.EditView'
			   and DATA_FIELD       = 'EXTENSION_C'
			   and DELETED          = 0;
	end -- if;
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.EditView' and DATA_FIELD = 'EXTENSION' and DELETED = 0) begin -- then
		if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.EditView' and FIELD_TYPE = 'Blank' and FIELD_INDEX = 9 and DELETED = 0) begin -- then
			update EDITVIEWS_FIELDS
			   set DELETED          = 1
			     , DATE_MODIFIED    = getdate()
			     , DATE_MODIFIED_UTC= getutcdate()
			     , MODIFIED_USER_ID = null
			 where EDIT_NAME        = 'Users.EditView'
			   and FIELD_TYPE       = 'Blank'
			   and FIELD_INDEX      = 9
			   and DELETED          = 0;
		end -- if;
		exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditView'          ,  9, 'Users.LBL_EXTENSION'                    , 'EXTENSION'                  , 0, 5,  25, 20, null;
	end -- if;

	-- 01/21/2008 Paul.  Some older systems still have EMAIL1 and EMAIL2 in the main. 
	if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.EditView' and DATA_FIELD in ('EMAIL1', 'EMAIL2') and DELETED = 0) begin -- then
		print 'Remove EMAIL1 and EMAIL2 from Users Main panel.';
		update EDITVIEWS_FIELDS
		   set DELETED          = 1
		     , DATE_MODIFIED    = getdate()
		     , DATE_MODIFIED_UTC= getutcdate()
		     , MODIFIED_USER_ID = null
		 where EDIT_NAME        = 'Users.EditView'
		   and DATA_FIELD       in ('EMAIL1', 'EMAIL2')
		   and DELETED          = 0;
	end -- if;
/*
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.EditView' and FIELD_VALIDATOR_ID is not null and DELETED = 0) begin -- then
		print 'Users.EditView: Update validators';
--		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Users.EditView'          ,  4, 'Phone Number'                           , 'PHONE_WORK'                 , '.ERR_INVALID_PHONE_NUMBER';
--		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Users.EditView'          ,  6, 'Phone Number'                           , 'PHONE_MOBILE'               , '.ERR_INVALID_PHONE_NUMBER';
--		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Users.EditView'          ,  8, 'Phone Number'                           , 'PHONE_OTHER'                , '.ERR_INVALID_PHONE_NUMBER';
--		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Users.EditView'          , 10, 'Phone Number'                           , 'PHONE_FAX'                  , '.ERR_INVALID_PHONE_NUMBER';
--		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Users.EditView'          , 12, 'Phone Number'                           , 'PHONE_HOME'                 , '.ERR_INVALID_PHONE_NUMBER';
	end -- if;
*/
	-- 07/08/2010 Paul.  Move Users.EditAddress fields to Users.EditView
	-- 09/02/2012 Paul.  Merge layout so that there is only one table to render in the HTML5 Client. 
	if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.EditAddress' and DELETED = 0) begin -- then
		exec dbo.spEDITVIEWS_FIELDS_MergeView      'Users.EditView', 'Users.EditAddress', 'Users.LBL_ADDRESS_INFORMATION', null;
	end -- if;
	-- 09/27/2013 Paul.  SMS messages need to be opt-in. 
	exec dbo.spEDITVIEWS_FIELDS_CnvBoundLst    'Users.EditView'          , 20, 'Users.LBL_SMS_OPT_IN'                   , 'SMS_OPT_IN'                 , 0, 1, 'dom_sms_opt_in'     , null, null;
end -- if;
GO

-- 02/01/2017 Paul.  Add support for Exchange using Username/Password. 
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.SmtpView' and DELETED = 0) begin -- then
	update EDITVIEWS_FIELDS
	   set DELETED           = 1
	     , DATE_MODIFIED     = getdate()
	     , DATE_MODIFIED_UTC = getutcdate()
	     , MODIFIED_USER_ID  = null
	 where EDIT_NAME = 'Users.EditMailOptions';
end -- if;

-- 08/05/2006 Paul.  Convert MailOptions to a dynamic view so that fields can be easily removed. 
-- 08/05/2006 Paul.  SplendidCRM does not support anything other than the build-in .NET mail.
-- 01/20/2008 Paul.  Add EMAIL1 so that users can be the target of a campaign. 
-- 07/08/2010 Paul.  Remove MAIL_FROMNAME and MAIL_FROMADDRESS.  Add MAIL_SMTPUSER and MAIL_SMTPPASS. 
-- 04/20/2016 Paul.  Restore MAIL_SMTPSERVER for separate email server. 
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.EditMailOptions';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.EditMailOptions' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Users.EditMailOptions';
	exec dbo.spEDITVIEWS_InsertOnly            'Users.EditMailOptions', 'Users', 'vwUSERS_Edit', '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditMailOptions'   ,  1, 'Users.LBL_EMAIL'                        , 'EMAIL1'                     , 0, 9, 100, 50, null;
	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Users.EditMailOptions'   ,  1, 'Email Address'                          , 'EMAIL1'                     , '.ERR_INVALID_EMAIL_ADDRESS';
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditMailOptions'   ,  2, 'Users.LBL_OTHER_EMAIL'                  , 'EMAIL2'                     , 0, 9, 100, 50, null;
	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Users.EditMailOptions'   ,  2, 'Email Address'                          , 'EMAIL2'                     , '.ERR_INVALID_EMAIL_ADDRESS';
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Users.EditMailOptions'   ,  3, 'Users.LBL_MAIL_SENDTYPE'                , 'MAIL_SENDTYPE'              , 0, 9, 'user_mail_send_type', null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Users.EditMailOptions'   ,  4, null;
end else begin
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.EditMailOptions' and DATA_FIELD = 'EMAIL1' and DELETED = 0) begin -- then
		print 'Add EMAIL1 to Users.';
		update EDITVIEWS_FIELDS
		   set FIELD_INDEX      = FIELD_INDEX + 2
		     , DATE_MODIFIED    = getdate()
		     , MODIFIED_USER_ID = null
		 where EDIT_NAME        = 'Users.EditMailOptions'
		   and FIELD_INDEX     >= 1
		   and DELETED          = 0;
    		-- 01/20/2008 Paul.  The reply info should not be required. 
		update EDITVIEWS_FIELDS
		   set UI_REQUIRED      = 0
		     , DATA_REQUIRED    = 0
		     , DATE_MODIFIED    = getdate()
		     , MODIFIED_USER_ID = null
		 where EDIT_NAME        = 'Users.EditMailOptions'
		   and DATA_FIELD      in ('MAIL_FROMNAME', 'MAIL_FROMADDRESS')
		   and UI_REQUIRED      = 1
		   and DELETED          = 0;
		exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditMailOptions'   ,  1, 'Users.LBL_EMAIL'                        , 'EMAIL1'                     , 0, 9, 100, 50, null;
		exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditMailOptions'   ,  2, 'Users.LBL_OTHER_EMAIL'                  , 'EMAIL2'                     , 0, 9, 100, 50, null;

	end -- if;
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.EditMailOptions' and FIELD_VALIDATOR_ID is not null and DELETED = 0) begin -- then
		print 'Users.EditMailOptions: Update validators';
		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Users.EditMailOptions'   ,  1, 'Email Address'                          , 'EMAIL1'                     , '.ERR_INVALID_EMAIL_ADDRESS';
		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Users.EditMailOptions'   ,  2, 'Email Address'                          , 'EMAIL2'                     , '.ERR_INVALID_EMAIL_ADDRESS';
	end -- if;
end -- if;
GO

-- 02/01/2017 Paul.  Add support for Exchange using Username/Password. 
-- 02/02/2017 Paul.  Server and Port are optional as server values will be used if blank. 
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.SmtpView';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.SmtpView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Users.SmtpView';
	exec dbo.spEDITVIEWS_InsertOnly            'Users.SmtpView'  , 'Users', 'vwUSERS_Edit', '20%', '30%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.SmtpView'  ,  1, 'Users.LBL_MAIL_SMTPSERVER'           , 'MAIL_SMTPSERVER'             , 0, 10,  50, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.SmtpView'  ,  2, 'Users.LBL_MAIL_SMTPPORT'             , 'MAIL_SMTPPORT'               , 0, 10,  10, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Users.SmtpView'  ,  3, 'Users.LBL_MAIL_SMTPAUTH_REQ'         , 'MAIL_SMTPAUTH_REQ'           , 0, 10, 'CheckBox'               , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Users.SmtpView'  ,  4, 'Users.LBL_MAIL_SMTPSSL'              , 'MAIL_SMTPSSL'                , 0, 10, 'CheckBox'               , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.SmtpView'  ,  5, 'Users.LBL_MAIL_SMTPUSER'             , 'MAIL_SMTPUSER'               , 1, 10, 100, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsPassword    'Users.SmtpView'  ,  6, 'Users.LBL_MAIL_SMTPPASS'             , 'MAIL_SMTPPASS'               , 1, 10, 100, 25, null;
end -- if;
GO

-- 03/08/2017 Paul.  Need to protect against left-over fields. 
if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.EditMailOptions' and DATA_FIELD in ('MAIL_SMTPSERVER', 'MAIL_SMTPPORT', 'MAIL_SMTPAUTH_REQ', 'MAIL_SMTPSSL') and DELETED = 0) begin -- then
	update EDITVIEWS_FIELDS
	   set DELETED           = 1
	     , DATE_MODIFIED     = getdate()
	     , DATE_MODIFIED_UTC = getutcdate()
	     , MODIFIED_USER_ID  = null
	 where EDIT_NAME         = 'Users.EditMailOptions'
	   and DATA_FIELD        in ('MAIL_SMTPSERVER', 'MAIL_SMTPPORT', 'MAIL_SMTPAUTH_REQ', 'MAIL_SMTPSSL')
	   and DELETED           = 0;
end -- if;
GO

-- 07/01/2020 Paul.  Users.EditView.Settings for the React Client. 
-- 12/16/2020 Paul.  DEFAULT_TEAM instead of DEFAULT_TEAM_ID. 
-- 11/11/2021 Paul.  PASSWORD field should be of type Password so that it is not displayed in plain text when entering. 
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.EditView.Settings';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.EditView.Settings' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Users.EditView.Settings';
	exec dbo.spEDITVIEWS_InsertOnly            'Users.EditView.Settings', 'Users'         , 'vwUSERS_Edit'         , '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditView.Settings'      ,  0, 'Users.LBL_FIRST_NAME'               , 'FIRST_NAME'                              , 0, 0, 100, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditView.Settings'      ,  1, 'Users.LBL_USER_NAME'                , 'USER_NAME'                               , 1, 0, 100, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditView.Settings'      ,  2, 'Users.LBL_LAST_NAME'                , 'LAST_NAME'                               , 1, 0, 100, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsPassword    'Users.EditView.Settings'      ,  3, 'Users.LBL_PASSWORD'                 , 'PASSWORD'                                , 0, 0, 100, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Users.EditView.Settings'      ,  4, 'Users.LBL_STATUS'                   , 'STATUS'                                  , 1, 0, 'user_status_dom'  , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Users.EditView.Settings'      ,  5, 'Users.LBL_DEFAULT_TEAM'             , 'DEFAULT_TEAM'                            , 0, 0, 'DEFAULT_TEAM_NAME', 'Teams', null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Users.EditView.Settings'      ,  6, 'Users.LBL_PICTURE'                  , 'PICTURE'                                 , 0, 0, 'Picture'          , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Users.EditView.Settings'      ,  7, null;
	exec dbo.spEDITVIEWS_FIELDS_InsHeader      'Users.EditView.Settings'      ,  8, 'Users.LBL_USER_SETTINGS', 3;
	exec dbo.spEDITVIEWS_FIELDS_InsCheckBox    'Users.EditView.Settings'      ,  9, 'Users.LBL_ADMIN'                    , 'IS_ADMIN'                                , 0, 0, null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel       'Users.EditView.Settings'      , 10, null                                 , 'Users.LBL_ADMIN_TEXT'                    , null;
	exec dbo.spEDITVIEWS_FIELDS_InsCheckBox    'Users.EditView.Settings'      , 11, 'Users.LBL_ADMIN_DELEGATE'           , 'IS_ADMIN_DELEGATE'                       , 0, 0, null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel       'Users.EditView.Settings'      , 12, null                                 , 'Users.LBL_ADMIN_DELEGATE_TEXT'           , null;
	exec dbo.spEDITVIEWS_FIELDS_InsCheckBox    'Users.EditView.Settings'      , 13, 'Users.LBL_RECEIVE_NOTIFICATIONS'    , 'RECEIVE_NOTIFICATIONS'                   , 0, 0, null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel       'Users.EditView.Settings'      , 14, null                                 , 'Users.LBL_RECEIVE_NOTIFICATIONS_TEXT'    , null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Users.EditView.Settings'      , 15, 'Users.LBL_THEME'                    , 'THEME'                                   , 1, 0, 'Themes'     , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel       'Users.EditView.Settings'      , 16, null                                 , 'Users.LBL_THEME_TEXT'                    , null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Users.EditView.Settings'      , 17, 'Users.LBL_LANGUAGE'                 , 'LANG'                                    , 1, 0, 'Languages'  , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel       'Users.EditView.Settings'      , 18, null                                 , 'Users.LBL_LANGUAGE_TEXT'                 , null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Users.EditView.Settings'      , 19, 'Users.LBL_DATE_FORMAT'              , 'DATE_FORMAT'                             , 1, 0, 'DateFormats', null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel       'Users.EditView.Settings'      , 20, null                                 , 'Users.LBL_DATE_FORMAT_TEXT'              , null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Users.EditView.Settings'      , 21, 'Users.LBL_TIME_FORMAT'              , 'TIME_FORMAT'                             , 1, 0, 'TimeFormats', null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel       'Users.EditView.Settings'      , 22, null                                 , 'Users.LBL_TIME_FORMAT_TEXT'              , null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Users.EditView.Settings'      , 23, 'Users.LBL_TIMEZONE'                 , 'TIMEZONE_ID'                             , 1, 0, 'TimeZones'  , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel       'Users.EditView.Settings'      , 24, null                                 , 'Users.LBL_TIMEZONE_TEXT'                 , null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Users.EditView.Settings'      , 25, 'Users.LBL_CURRENCY'                 , 'CURRENCY_ID'                             , 1, 0, 'Currencies' , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel       'Users.EditView.Settings'      , 26, null                                 , 'Users.LBL_CURRENCY_TEXT'                 , null;
	exec dbo.spEDITVIEWS_FIELDS_InsCheckBox    'Users.EditView.Settings'      , 27, 'Users.LBL_SAVE_QUERY'               , 'SAVE_QUERY'                              , 0, 0, null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel       'Users.EditView.Settings'      , 28, null                                 , 'Users.LBL_SAVE_QUERY_TEXT'               , null;
	exec dbo.spEDITVIEWS_FIELDS_InsCheckBox    'Users.EditView.Settings'      , 29, 'Users.LBL_GROUP_TABS'               , 'GROUP_TABS'                              , 0, 0, null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel       'Users.EditView.Settings'      , 30, null                                 , 'Users.LBL_GROUP_TABS_TEXT'               , null;
	exec dbo.spEDITVIEWS_FIELDS_InsCheckBox    'Users.EditView.Settings'      , 31, 'Users.LBL_SUBPANEL_TABS'            , 'SUBPANEL_TABS'                           , 0, 0, null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel       'Users.EditView.Settings'      , 32, null                                 , 'Users.LBL_SUBPANEL_TABS_TEXT'            , null;
	exec dbo.spEDITVIEWS_FIELDS_InsCheckBox    'Users.EditView.Settings'      , 33, 'Users.LBL_SYSTEM_GENERATED_PASSWORD', 'LBL_SYSTEM_GENERATED_PASSWORD_TEXT'      , 0, 0, null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel       'Users.EditView.Settings'      , 34, null                                 , 'Users.LBL_SYSTEM_GENERATED_PASSWORD_TEXT', null;
end else begin
	-- 12/16/2020 Paul.  DEFAULT_TEAM instead of DEFAULT_TEAM_ID. 
	if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.EditView.Settings' and DATA_FIELD = 'DEFAULT_TEAM_ID' and DELETED = 0) begin -- then
		update EDITVIEWS_FIELDS
		   set DATA_FIELD        = 'DEFAULT_TEAM'
		     , DATE_MODIFIED     = getdate()
		     , DATE_MODIFIED_UTC = getutcdate()
		     , MODIFIED_USER_ID  = null
		 where EDIT_NAME         = 'Users.EditView.Settings'
		   and DATA_FIELD        = 'DEFAULT_TEAM_ID'
		   and DELETED           = 0;
	end -- if;
	-- 11/11/2021 Paul.  PASSWORD field should be of type Password so that it is not displayed in plain text when entering. 
	if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.EditView.Settings' and DATA_FIELD = 'PASSWORD' and FIELD_TYPE = 'TextBox' and DELETED = 0) begin -- then
		update EDITVIEWS_FIELDS
		   set FIELD_TYPE        = 'Password'
		     , DATE_MODIFIED     = getdate()
		     , DATE_MODIFIED_UTC = getutcdate()
		     , MODIFIED_USER_ID  = null
		 where EDIT_NAME         = 'Users.EditView.Settings'
		   and DATA_FIELD        = 'PASSWORD'
		   and FIELD_TYPE        = 'TextBox' 
		   and DELETED           = 0;
	end -- if;
end -- if;
GO

set nocount off;
GO


