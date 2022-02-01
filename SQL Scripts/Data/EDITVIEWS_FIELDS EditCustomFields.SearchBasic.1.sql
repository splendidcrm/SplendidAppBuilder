set nocount on;
GO

-- 01/29/2021 Paul.  Add EditCustomFields to React client. 
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'EditCustomFields.SearchBasic';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'EditCustomFields.SearchBasic' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS EditCustomFields.SearchBasic';
	exec dbo.spEDITVIEWS_InsertOnly             'EditCustomFields.SearchBasic', 'EditCustomFields', 'vwFIELDS_META_DATA_List', '15%', '85%', 1;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList    'EditCustomFields.SearchBasic',  5, 'EditCustomFields.LBL_MODULE_SELECT'   , 'CUSTOM_MODULE'              , 1, null, 'CustomEditModules'            , null, null;
end -- if;
GO

set nocount off;
GO


