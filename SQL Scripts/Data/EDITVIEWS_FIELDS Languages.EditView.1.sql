set nocount on;
GO

-- 05/17/2010 Paul.  Allow editing of Languages. 
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'Languages.EditView';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Languages.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Languages.EditView';
	exec dbo.spEDITVIEWS_InsertOnly          'Languages.EditView'       , 'Languages', 'vwLANGUAGES', '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound     'Languages.EditView'       , 0, 'Languages.LBL_NAME'                    , 'NAME'               , 1, 1,   10, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound     'Languages.EditView'       , 1, 'Languages.LBL_LCID'                    , 'LCID'               , 1, 1,   10, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl   'Languages.EditView'       , 2, 'Languages.LBL_ACTIVE'                  , 'ACTIVE'             , 0, 1, 'CheckBox', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound     'Languages.EditView'       , 3, 'Languages.LBL_NATIVE_NAME'             , 'NATIVE_NAME'        , 1, 1,   80, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound     'Languages.EditView'       , 4, 'Languages.LBL_DISPLAY_NAME'            , 'DISPLAY_NAME'       , 1, 1,   80, 35, null;

end -- if;
GO


set nocount off;
GO


