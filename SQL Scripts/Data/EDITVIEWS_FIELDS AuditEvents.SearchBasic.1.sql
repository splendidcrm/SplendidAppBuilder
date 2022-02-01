set nocount on;
GO

-- 01/20/2010 Paul.  Add ability to search the new Audit Events table. 
-- 03/28/2019 Paul.  Move AuditEvents.SearchBasic to default file for Community edition. 
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'AuditEvents.SearchBasic';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'AuditEvents.SearchBasic' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS AuditEvents.SearchBasic';
	exec dbo.spEDITVIEWS_InsertOnly             'AuditEvents.SearchBasic'   , 'AuditEvents', 'vwAUDIT_EVENTS', '11%', '22%', 3;
	exec dbo.spEDITVIEWS_FIELDS_InsBound        'AuditEvents.SearchBasic'   ,  0, 'Users.LBL_NAME'                         , 'FULL_NAME'                  , 0, null,  30, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsAutoComplete 'AuditEvents.SearchBasic'   ,  1, 'Users.LBL_USER_NAME'                    , 'USER_NAME'                  , 0, null,  30, 25, 'Users', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank        'AuditEvents.SearchBasic'   ,  2, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl      'AuditEvents.SearchBasic'   ,  3, '.LBL_DATE_MODIFIED'                     , 'DATE_MODIFIED'              , 0, null, 'DateRange'          , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList    'AuditEvents.SearchBasic'   ,  4, 'Audit.LBL_AUDIT_ACTION'                 , 'AUDIT_ACTION'               , 1, null, 'audit_action_dom'   , null, 4;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList    'AuditEvents.SearchBasic'   ,  5, 'Audit.LBL_MODULE_NAME'                  , 'MODULE_NAME'                , 1, null, 'Modules'            , null, 4;
end -- if;
GO

set nocount off;
GO


