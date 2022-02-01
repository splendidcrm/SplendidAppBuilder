set nocount on;
GO

-- 08/05/2006 Paul.  Convert MailOptions to a dynamic view so that fields can be easily removed. 
-- 08/05/2006 Paul.  SplendidCRM does not support anything other than the build-in .NET mail.
-- 01/20/2008 Paul.  Add EMAIL1 so that users can be the target of a campaign. 
-- 07/09/2010 Paul.  Remove MAIL_FROMNAME and MAIL_FROMADDRESS. 
-- 12/15/2012 Paul.  Remove MAIL_FROMNAME and MAIL_FROMADDRESS from creation section. 
-- delete from DETAILVIEWS_FIELDS where DETAIL_NAME = 'Users.MailOptions';
if not exists(select * from DETAILVIEWS_FIELDS where DETAIL_NAME = 'Users.MailOptions' and DELETED = 0) begin -- then
	print 'DETAILVIEWS_FIELDS Users.MailOptions';
	exec dbo.spDETAILVIEWS_InsertOnly          'Users.MailOptions', 'Users', 'vwUSERS_Edit', '15%', '35%', null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Users.MailOptions'  ,  1, 'Users.LBL_EMAIL'                 , 'EMAIL1'                           , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Users.MailOptions'  ,  2, 'Users.LBL_OTHER_EMAIL'           , 'EMAIL2'                           , '{0}'        , null;
--	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Users.MailOptions'  ,  3, 'Users.LBL_MAIL_FROMNAME'         , 'MAIL_FROMNAME'                    , '{0}'        , null;
--	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Users.MailOptions'  ,  4, 'Users.LBL_MAIL_FROMADDRESS'      , 'MAIL_FROMADDRESS'                 , '{0}'        , null;
--	exec dbo.spDETAILVIEWS_FIELDS_InsBoundList 'Users.MailOptions'  ,  5, 'Users.LBL_MAIL_SENDTYPE'         , 'MAIL_SENDTYPE'                    , '{0}'        , 'notifymail_sendtype'  , null;
--	exec dbo.spDETAILVIEWS_FIELDS_InsBlank     'Users.MailOptions'  ,  6, null;
--	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Users.MailOptions'  ,  7, 'Users.LBL_MAIL_SMTPSERVER'       , 'MAIL_SMTPSERVER'                  , '{0}'        , null;
--	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Users.MailOptions'  ,  8, 'Users.LBL_MAIL_SMTPPORT'         , 'MAIL_SMTPPORT'                    , '{0}'        , null;
--	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Users.MailOptions'  ,  9, 'Users.LBL_MAIL_SMTPAUTH_REQ'     , 'MAIL_SMTPAUTH_REQ'                , '{0}'        , null;
--	exec dbo.spDETAILVIEWS_FIELDS_InsBlank     'Users.MailOptions'  , 10, null;
--	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Users.MailOptions'  , 11, 'Users.LBL_MAIL_SMTPUSER'         , 'MAIL_SMTPUSER'                    , '{0}'        , null;
--	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Users.MailOptions'  , 12, 'Users.LBL_MAIL_SMTPPASS'         , 'MAIL_SMTPPASS'                    , '******'     , null;
end else begin
	if not exists(select * from DETAILVIEWS_FIELDS where DETAIL_NAME = 'Users.MailOptions' and DATA_FIELD = 'EMAIL1' and DELETED = 0) begin -- then
		print 'Add EMAIL1 to Users.';
		update DETAILVIEWS_FIELDS
		   set FIELD_INDEX      = FIELD_INDEX + 2
		     , DATE_MODIFIED    = getdate()
		     , MODIFIED_USER_ID = null
		 where DETAIL_NAME      = 'Users.MailOptions'
		   and FIELD_INDEX     >= 1
		   and DELETED          = 0;
		exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Users.MailOptions'  ,  1, 'Users.LBL_EMAIL'                 , 'EMAIL1'                           , '{0}'        , null;
		exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Users.MailOptions'  ,  2, 'Users.LBL_OTHER_EMAIL'           , 'EMAIL2'                           , '{0}'        , null;
	end -- if;
	-- 07/09/2010 Paul.  Remove MAIL_FROMNAME and MAIL_FROMADDRESS.
	if exists(select * from DETAILVIEWS_FIELDS where DETAIL_NAME = 'Users.MailOptions' and DATA_FIELD in ('MAIL_FROMNAME', 'MAIL_FROMADDRESS') and DELETED = 0) begin -- then
		print 'Remove MAIL_FROMNAME and MAIL_FROMADDRESS.';
		update DETAILVIEWS_FIELDS
		   set DELETED           = 1
		     , DATE_MODIFIED     = getdate()
		     , DATE_MODIFIED_UTC = getutcdate()
		     , MODIFIED_USER_ID  = null
		 where DETAIL_NAME       = 'Users.MailOptions'
		   and DATA_FIELD        in ('MAIL_FROMNAME', 'MAIL_FROMADDRESS')
		   and DELETED           = 0;
	end -- if;
end -- if;
GO

set nocount off;
GO


