set nocount on;
GO

if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.SearchAdvanced' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Users.SearchAdvanced';
	exec dbo.spEDITVIEWS_InsertOnly             'Users.SearchAdvanced'    , 'Users', 'vwUSERS_List', '15%', '35%', 2;
	exec dbo.spEDITVIEWS_FIELDS_InsBound        'Users.SearchAdvanced'    ,  0, 'Users.LBL_FIRST_NAME'                   , 'FIRST_NAME'                                              , 0, null,  30, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsAutoComplete 'Users.SearchAdvanced'    ,  1, 'Users.LBL_USER_NAME'                    , 'USER_NAME'                                               , 0, null,  20, 25, 'Users', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound        'Users.SearchAdvanced'    ,  2, 'Users.LBL_LAST_NAME'                    , 'LAST_NAME'                                               , 0, null,  30, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList    'Users.SearchAdvanced'    ,  3, 'Users.LBL_STATUS'                       , 'STATUS'                                                  , 0, null, 'user_status_dom'   , null, 6;
	exec dbo.spEDITVIEWS_FIELDS_InsControl      'Users.SearchAdvanced'    ,  4, 'Users.LBL_ADMIN'                        , 'IS_ADMIN'                                                , 0, null, 'CheckBox', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound        'Users.SearchAdvanced'    ,  5, 'Users.LBL_TITLE'                        , 'TITLE'                                                   , 0, null,  50, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank        'Users.SearchAdvanced'    ,  6, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound        'Users.SearchAdvanced'    ,  7, 'Users.LBL_DEPARTMENT'                   , 'DEPARTMENT'                                              , 0, null,  75, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound        'Users.SearchAdvanced'    ,  8, 'Users.LBL_ANY_PHONE'                    , 'PHONE_HOME PHONE_MOBILE PHONE_WORK PHONE_OTHER PHONE_FAX', 0, null,  25, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound        'Users.SearchAdvanced'    ,  9, 'Users.LBL_ADDRESS'                      , 'ADDRESS_STREET'                                          , 0, null, 150, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound        'Users.SearchAdvanced'    , 10, 'Users.LBL_ANY_EMAIL'                    , 'EMAIL1 EMAIL2'                                           , 0, null, 100, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound        'Users.SearchAdvanced'    , 11, 'Users.LBL_STATE'                        , 'ADDRESS_STATE'                                           , 0, null, 100, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound        'Users.SearchAdvanced'    , 12, 'Users.LBL_CITY'                         , 'ADDRESS_CITY'                                            , 0, null, 100, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound        'Users.SearchAdvanced'    , 13, 'Users.LBL_POSTAL_CODE'                  , 'ADDRESS_POSTALCODE'                                      , 0, null,  20, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList    'Users.SearchAdvanced'    , 14, 'Users.LBL_COUNTRY'                      , 'ADDRESS_COUNTRY'                                         , 0, null, 'countries_dom', null, 6;
end else begin
	exec dbo.spEDITVIEWS_FIELDS_CnvAutoComplete 'Users.SearchAdvanced'    ,  1, 'Users.LBL_USER_NAME'                    , 'USER_NAME'                                               , 0, null,  20, 25, 'Users', null;
end -- if;
GO

if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.SearchBasic' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Users.SearchBasic';
	exec dbo.spEDITVIEWS_InsertOnly             'Users.SearchBasic'       , 'Users', 'vwUSERS_List', '11%', '22%', 3;
	exec dbo.spEDITVIEWS_FIELDS_InsBound        'Users.SearchBasic'       ,  0, 'Users.LBL_FIRST_NAME'                   , 'FIRST_NAME'                 , 0, null,  30, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound        'Users.SearchBasic'       ,  1, 'Users.LBL_LAST_NAME'                    , 'LAST_NAME'                  , 0, null,  30, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound        'Users.SearchBasic'       ,  2, 'Users.LBL_DEPARTMENT'                   , 'DEPARTMENT'                 , 0, null,  75, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList    'Users.SearchBasic'       ,  3, 'Users.LBL_STATUS'                       , 'STATUS'                     , 1, null, 'user_status_dom'   , null, null;
end else begin
	-- 01/17/2008 Paul.  ListBoxes that are not UI_REQUIRED default to querying for data that is NULL.  That is bad in his area. 
	if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.SearchBasic' and DATA_FIELD = 'STATUS' and UI_REQUIRED = 0 and DELETED = 0) begin -- then
		update EDITVIEWS_FIELDS
		   set DATA_REQUIRED    = 1
		     , UI_REQUIRED      = 1
		     , DATE_MODIFIED    = getdate()
		     , MODIFIED_USER_ID = null
		 where EDIT_NAME        = 'Users.SearchBasic'
		   and DATA_FIELD       = 'STATUS'
		   and UI_REQUIRED      = 0
		   and DELETED          = 0;
	end -- if;
end -- if;
GO

-- 03/22/2010 Paul.  Allow searching one Email1. 
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.SearchPopup' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Users.SearchPopup';
	exec dbo.spEDITVIEWS_InsertOnly             'Users.SearchPopup'       , 'Users', 'vwUSERS_List', '15%', '35%', 2;
	exec dbo.spEDITVIEWS_FIELDS_InsBound        'Users.SearchPopup'       ,  0, 'Users.LBL_FIRST_NAME'                   , 'FIRST_NAME'                 , 0, null,  30, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound        'Users.SearchPopup'       ,  1, 'Users.LBL_LAST_NAME'                    , 'LAST_NAME'                  , 0, null,  30, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsAutoComplete 'Users.SearchPopup'       ,  2, 'Users.LBL_USER_NAME'                    , 'USER_NAME'                  , 0, null,  20, 25, 'Users', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound        'Users.SearchPopup'       ,  3, 'Users.LBL_EMAIL1'                       , 'EMAIL1'                     , 0, null, 255, 25, null;
end else begin
	exec dbo.spEDITVIEWS_FIELDS_CnvAutoComplete 'Users.SearchPopup'       ,  2, 'Users.LBL_USER_NAME'                    , 'USER_NAME'                  , 0, null,  20, 25, 'Users', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound        'Users.SearchPopup'       ,  3, 'Users.LBL_EMAIL1'                       , 'EMAIL1'                     , 0, null, 255, 25, null;
end -- if;
GO

set nocount off;
GO


