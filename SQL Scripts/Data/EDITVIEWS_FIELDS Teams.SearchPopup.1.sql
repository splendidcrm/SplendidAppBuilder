-- 12/01/2009 Paul.  Add Teams search field. 
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'Teams.SearchPopup';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Teams.SearchPopup' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Teams.SearchPopup';
	exec dbo.spEDITVIEWS_InsertOnly             'Teams.SearchPopup'         , 'Teams', 'vwTEAMS_List', '11%', '22%', 3;
	exec dbo.spEDITVIEWS_FIELDS_InsBound        'Teams.SearchPopup'         ,  0, 'Teams.LBL_NAME'                         , 'NAME'                       , 0, null, 100, 80, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList    'Teams.SearchPopup'         ,  1, 'Teams.LBL_PRIVATE'                      , 'PRIVATE'                    , 0, null, 'team_private_dom', null, 2;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup  'Teams.SearchPopup'         ,  2, 'Teams.LBL_PARENT_NAME'                  , 'PARENT_ID'                  , 0, null, 'PARENT_NAME', 'Teams', null;
end else begin
	if exists(select * from EDITVIEWS where NAME = 'Teams.SearchPopup' and DATA_COLUMNS = 1 and DELETED = 0) begin -- then
		update EDITVIEWS
		   set DATA_COLUMNS      = 3
		     , LABEL_WIDTH       = '11%'
		     , FIELD_WIDTH       = '22%'
		     , DATE_MODIFIED     = getdate()
		     , DATE_MODIFIED_UTC = getutcdate()
		 where NAME              = 'Teams.SearchPopup'
		   and DATA_COLUMNS      = 1
		   and DELETED           = 0;
		exec dbo.spEDITVIEWS_FIELDS_InsBoundList    'Teams.SearchPopup'         ,  1, 'Teams.LBL_PRIVATE'                      , 'PRIVATE'                    , 0, null, 'team_private_dom', null, 2;
		exec dbo.spEDITVIEWS_FIELDS_InsModulePopup  'Teams.SearchPopup'         ,  2, 'Teams.LBL_PARENT_NAME'                  , 'PARENT_ID'                  , 0, null, 'PARENT_NAME', 'Teams', null;
	end -- if;
end -- if;
GO

