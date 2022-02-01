-- 02/22/2017 Paul.  Convert to ctlSearchView. 
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'Teams.SearchBasic';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Teams.SearchBasic' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Teams.SearchBasic';
	exec dbo.spEDITVIEWS_InsertOnly             'Teams.SearchBasic'         , 'Teams', 'vwTEAMS_List', '11%', '22%', 3;
	exec dbo.spEDITVIEWS_FIELDS_InsBound        'Teams.SearchBasic'         ,  0, 'Teams.LBL_NAME'                         , 'NAME'                       , 0, null, 100, 80, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList    'Teams.SearchBasic'         ,  1, 'Teams.LBL_PRIVATE'                      , 'PRIVATE'                    , 0, null, 'team_private_dom', null, 2;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup  'Teams.SearchBasic'         ,  2, 'Teams.LBL_PARENT_NAME'                  , 'PARENT_ID'                  , 0, null, 'PARENT_NAME', 'Teams', null;
		update EDITVIEWS
		   set DATA_COLUMNS      = 3
		     , LABEL_WIDTH       = '11%'
		     , FIELD_WIDTH       = '22%'
		     , DATE_MODIFIED     = getdate()
		     , DATE_MODIFIED_UTC = getutcdate()
		 where NAME              = 'Teams.SearchBasic'
		   and DATA_COLUMNS      = 1
		   and DELETED           = 0;
end -- if;
GO

