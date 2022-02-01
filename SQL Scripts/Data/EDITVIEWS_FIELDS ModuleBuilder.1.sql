

print 'EDITVIEWS_FIELDS ModuleBuilder';
set nocount on;
GO


-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'ModuleBuilder.WizardView';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'ModuleBuilder.WizardView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS ModuleBuilder.WizardView';
	exec dbo.spEDITVIEWS_InsertOnly             'ModuleBuilder.WizardView', 'ModuleBuilder', 'vwMODULES', '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsAutoComplete 'ModuleBuilder.WizardView',  0, 'ModuleBuilder.LBL_DISPLAY_NAME'             , 'DISPLAY_NAME'                                           , 1, 1, 150, 25, 'Modules', null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel        'ModuleBuilder.WizardView',  1, null                                         , 'ModuleBuilder.LBL_DISPLAY_NAME_INSTRUCTIONS'            , null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound        'ModuleBuilder.WizardView',  2, 'ModuleBuilder.LBL_NAME'                     , 'MODULE_NAME'                                            , 1, 1,  50, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel        'ModuleBuilder.WizardView',  3, null                                         , 'ModuleBuilder.LBL_MODULE_NAME_INSTRUCTIONS'             , null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound        'ModuleBuilder.WizardView',  4, 'ModuleBuilder.LBL_TABLE_NAME'               , 'TABLE_NAME'                                             , 1, 1,  50, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel        'ModuleBuilder.WizardView',  5, null                                         , 'ModuleBuilder.LBL_TABLE_NAME_INSTRUCTIONS'              , null;
	exec dbo.spEDITVIEWS_FIELDS_InsCheckBox     'ModuleBuilder.WizardView',  6, 'ModuleBuilder.LBL_TAB_ENABLED'              , 'TAB_ENABLED'                                            , 0, 1, null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel        'ModuleBuilder.WizardView',  7, null                                         , 'ModuleBuilder.LBL_TAB_ENABLED_INSTRUCTIONS'             , null;
	exec dbo.spEDITVIEWS_FIELDS_InsCheckBox     'ModuleBuilder.WizardView',  8, 'ModuleBuilder.LBL_MOBILE_ENABLED'           , 'MOBILE_ENABLED'                                         , 0, 1, null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel        'ModuleBuilder.WizardView',  9, null                                         , 'ModuleBuilder.LBL_MOBILE_ENABLED_INSTRUCTIONS'          , null;
	exec dbo.spEDITVIEWS_FIELDS_InsCheckBox     'ModuleBuilder.WizardView', 10, 'ModuleBuilder.LBL_CUSTOM_ENABLED'           , 'CUSTOM_ENABLED'                                         , 0, 1, null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel        'ModuleBuilder.WizardView', 11, null                                         , 'ModuleBuilder.LBL_CUSTOM_ENABLED_INSTRUCTIONS'          , null;
	exec dbo.spEDITVIEWS_FIELDS_InsCheckBox     'ModuleBuilder.WizardView', 12, 'ModuleBuilder.LBL_REPORT_ENABLED'           , 'REPORT_ENABLED'                                         , 0, 1, null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel        'ModuleBuilder.WizardView', 13, null                                         , 'ModuleBuilder.LBL_REPORT_ENABLED_INSTRUCTIONS'          , null;
	exec dbo.spEDITVIEWS_FIELDS_InsCheckBox     'ModuleBuilder.WizardView', 14, 'ModuleBuilder.LBL_IMPORT_ENABLED'           , 'IMPORT_ENABLED'                                         , 0, 1, null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel        'ModuleBuilder.WizardView', 15, null                                         , 'ModuleBuilder.LBL_IMPORT_ENABLED_INSTRUCTIONS'          , null;
	exec dbo.spEDITVIEWS_FIELDS_InsCheckBox     'ModuleBuilder.WizardView', 16, 'ModuleBuilder.LBL_REST_ENABLED'             , 'REST_ENABLED'                                           , 0, 1, null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel        'ModuleBuilder.WizardView', 17, null                                         , 'ModuleBuilder.LBL_REST_ENABLED_INSTRUCTIONS'            , null;
	exec dbo.spEDITVIEWS_FIELDS_InsCheckBox     'ModuleBuilder.WizardView', 18, 'ModuleBuilder.LBL_IS_ADMIN'                 , 'IS_ADMIN'                                               , 0, 1, null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel        'ModuleBuilder.WizardView', 19, null                                         , 'ModuleBuilder.LBL_IS_ADMIN_INSTRUCTIONS'                , null;
	exec dbo.spEDITVIEWS_FIELDS_InsCheckBox     'ModuleBuilder.WizardView', 20, 'ModuleBuilder.LBL_INCLUDE_ASSIGNED_USER_ID' , 'INCLUDE_ASSIGNED_USER_ID'                               , 0, 1, null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel        'ModuleBuilder.WizardView', 21, null                                         , 'ModuleBuilder.LBL_INCLUDE_ASSIGNED_USER_ID_INSTRUCTIONS', null;
	exec dbo.spEDITVIEWS_FIELDS_InsCheckBox     'ModuleBuilder.WizardView', 22, 'ModuleBuilder.LBL_INCLUDE_TEAM_ID'          , 'INCLUDE_TEAM_ID'                                        , 0, 1, null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel        'ModuleBuilder.WizardView', 23, null                                         , 'ModuleBuilder.LBL_INCLUDE_TEAM_ID_INSTRUCTIONS'         , null;
	exec dbo.spEDITVIEWS_FIELDS_InsCheckBox     'ModuleBuilder.WizardView', 24, 'ModuleBuilder.LBL_OVERWRITE_EXISTING'       , 'OVERWRITE_EXISTING'                                     , 0, 1, null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel        'ModuleBuilder.WizardView', 25, null                                         , 'ModuleBuilder.LBL_OVERWRITE_EXISTING_INSTRUCTIONS'      , null;
	exec dbo.spEDITVIEWS_FIELDS_InsCheckBox     'ModuleBuilder.WizardView', 26, 'ModuleBuilder.LBL_CREATE_CODE_BEHIND'       , 'CREATE_CODE_BEHIND'                                     , 0, 1, null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel        'ModuleBuilder.WizardView', 27, null                                         , 'ModuleBuilder.LBL_CREATE_CODE_BEHIND_INSTRUCTIONS'      , null;
	exec dbo.spEDITVIEWS_FIELDS_InsCheckBox     'ModuleBuilder.WizardView', 28, 'ModuleBuilder.LBL_REACT_ONLY'               , 'REACT_ONLY'                                             , 0, 1, null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel        'ModuleBuilder.WizardView', 29, null                                         , 'ModuleBuilder.LBL_REACT_ONLY_INSTRUCTIONS'              , null;
end -- if;
GO


set nocount off;
GO

/* -- #if Oracle
	EXCEPTION
		WHEN NO_DATA_FOUND THEN
			StoO_selcnt := 0;
		WHEN OTHERS THEN
			RAISE;
	END;
	COMMIT WORK;
END;
/
-- #endif Oracle */

/* -- #if IBM_DB2
	commit;
  end
/

call dbo.spEDITVIEWS_FIELDS_ModuleBuilder()
/

call dbo.spSqlDropProcedure('spEDITVIEWS_FIELDS_ModuleBuilder')
/

-- #endif IBM_DB2 */


