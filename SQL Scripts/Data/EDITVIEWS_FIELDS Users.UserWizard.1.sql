set nocount on;
GO

-- 10/27/2021 Paul.  Administration.AdminWizard layout is used as a collection of values and not for layout purposes. 
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.UserWizard.Profile';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.UserWizard.Profile' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Users.UserWizard.Profile';
	exec dbo.spEDITVIEWS_InsertOnly            'Users.UserWizard.Profile', 'Configurator', 'vwCONFIG_Edit', '15%', '35%', 2;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.UserWizard.Profile',  0, 'Users.LBL_FIRST_NAME'                   , 'FIRST_NAME'             , 0, 1, 100, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.UserWizard.Profile',  1, 'Users.LBL_LAST_NAME'                    , 'LAST_NAME'              , 1, 1, 100, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.UserWizard.Profile',  2, 'Users.LBL_EMAIL'                        , 'EAMIL1'                 , 0, 1, 100, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Users.UserWizard.Profile',  3, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.UserWizard.Profile',  4, 'Users.LBL_OFFICE_PHONE'                 , 'PHONE_WORK'             , 0, 1,  25, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.UserWizard.Profile',  5, 'Users.LBL_MOBILE_PHONE'                 , 'PHONE_MOBILE'           , 0, 1,  25, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Users.UserWizard.Profile',  6, 'Users.LBL_PRIMARY_ADDRESS'              , 'ADDRESS_STREET'         , 0, 1,   2, 30, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Users.UserWizard.Profile',  7, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.UserWizard.Profile',  8, 'Users.LBL_CITY'                         , 'ADDRESS_CITY'           , 0, 1, 100, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.UserWizard.Profile', 10, 'Users.LBL_STATE'                        , 'ADDRESS_STATE'          , 0, 1, 100, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.UserWizard.Profile', 11, 'Users.LBL_POSTAL_CODE'                  , 'ADDRESS_POSTALCODE'     , 0, 1,  20, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.UserWizard.Profile', 12, 'Users.LBL_COUNTRY'                      , 'ADDRESS_COUNTRY'        , 0, 1,  20, 25, null;
end -- if;
GO

-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.UserWizard.Locale';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.UserWizard.Locale' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Users.UserWizard.Locale';
	exec dbo.spEDITVIEWS_InsertOnly            'Users.UserWizard.Locale' , 'Configurator', 'vwCONFIG_Edit', '15%', '35%', 2;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Users.UserWizard.Locale' ,  0, 'Users.LBL_LANGUAGE'                     , 'LANG'                   , 0, 2, 'Languages' , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Users.UserWizard.Locale' ,  1, 'Users.LBL_CURRENCY'                     , 'CURRENCY_ID'            , 0, 2, 'Currencies', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Users.UserWizard.Locale' ,  2, 'Users.LBL_DATE_FORMAT'                  , 'DATE_FORMAT'            , 0, 2, 'DateFormat', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Users.UserWizard.Locale' ,  3, 'Users.LBL_TIME_FORMAT'                  , 'TIME_FORMAT'            , 0, 2, 'TimeForamt', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Users.UserWizard.Locale' ,  4, 'Users.LBL_TIMEZONE'                     , 'TIMEZONE_ID'            , 0, 2, 'TimeZones' , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Users.UserWizard.Locale' ,  5, null;
end -- if;
GO

-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.UserWizard.Mail';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.UserWizard.Mail' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Users.UserWizard.Mail';
	exec dbo.spEDITVIEWS_InsertOnly            'Users.UserWizard.Mail', 'Configurator', 'vwCONFIG_Edit', '15%', '85%', 1;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel       'Users.UserWizard.Mail'  ,  0, 'EmailMan.LBL_MAIL_SMTPSERVER'            , 'smtpserver'             , null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.UserWizard.Mail'  ,  1, 'Users.LBL_MAIL_SMTPUSER'                 , 'MAIL_SMTPUSER'          , 0, 10, 100, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsPassword    'Users.UserWizard.Mail'  ,  2, 'Users.LBL_MAIL_SMTPPASS'                 , 'MAIL_SMTPPASS'          , 0, 10, 100, 25, null;
end -- if;
GO


set nocount off;
GO


