-- 10/15/2006 Paul.  Add support for Teams. 
-- 04/12/2016 Paul.  Add parent team and custom fields. 
-- 04/28/2016 Paul.  Rename parent to PARENT_ID. 
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'Teams.EditView'
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Teams.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Teams.EditView';
	exec dbo.spEDITVIEWS_InsertOnly            'Teams.EditView', 'Teams', 'vwTEAMS_Edit', '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Teams.EditView'          ,  0, 'Teams.LBL_NAME'                         , 'NAME'                       , 1, 1, 128, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Teams.EditView'          ,  1, 'Teams.LBL_PARENT_NAME'                  , 'PARENT_ID'                  , 0, 1, 'PARENT_NAME', 'Teams', null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Teams.EditView'          ,  2, 'Teams.LBL_DESCRIPTION'                  , 'DESCRIPTION'                , 0, 2,   4, 60, null;
end else begin
	-- 04/12/2016 Paul.  Add parent team and custom fields. 
	-- 04/28/2016 Paul.  Rename parent to PARENT_ID. 
	if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Teams.EditView' and DATA_FIELD = 'PARENT_TEAM_ID' and DELETED = 0) begin -- then
		update EDITVIEWS_FIELDS
		   set DATA_FIELD        = 'PARENT_ID'
		     , DISPLAY_FIELD     = 'PARENT_NAME'
		     , DATA_LABEL        = 'Teams.LBL_PARENT_NAME'
		     , DATE_MODIFIED     = getdate()
		     , DATE_MODIFIED_UTC = getutcdate()
		     , MODIFIED_USER_ID  = null
		 where EDIT_NAME         = 'Teams.EditView'
		   and DATA_FIELD        = 'PARENT_TEAM_ID'
		   and DELETED           = 0;
	end -- if;
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Teams.EditView' and DATA_FIELD = 'PARENT_ID' and DELETED = 0) begin -- then
		update EDITVIEWS_FIELDS
		   set DELETED           = 1
		     , DATE_MODIFIED     = getdate()
		     , DATE_MODIFIED_UTC = getutcdate()
		     , MODIFIED_USER_ID  = null
		 where EDIT_NAME         = 'Teams.EditView'
		   and FIELD_TYPE        = 'Blank'
		   and DELETED           = 0;
		exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Teams.EditView'          ,  1, 'Teams.LBL_PARENT_NAME'                  , 'PARENT_ID'                  , 0, 1, 'PARENT_NAME', 'Teams', null;
	end -- if;
	-- 05/11/2016 Paul.  Correct field width to support parent. 
	if exists(select * from EDITVIEWS where NAME = 'Teams.EditView' and FIELD_WIDTH = '85%' and DELETED = 0) begin -- then
		update EDITVIEWS
		   set FIELD_WIDTH       = '35%'
		     , DATE_MODIFIED     = getdate()
		     , DATE_MODIFIED_UTC = getutcdate()
		     , MODIFIED_USER_ID  = null
		 where NAME              = 'Teams.EditView'
		   and FIELD_WIDTH       = '85%'
		   and DELETED           = 0;
	end -- if;
end -- if;
GO

