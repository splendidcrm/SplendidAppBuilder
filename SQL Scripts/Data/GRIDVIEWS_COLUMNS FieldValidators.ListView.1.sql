set nocount on;
GO

-- 09/12/2009 Paul.  Allow editing of Field Validators. 
-- delete from GRIDVIEWS_COLUMNS where GRID_NAME = 'FieldValidators.ListView' and DELETED = 0;
if not exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'FieldValidators.ListView' and DELETED = 0) begin -- then
	print 'GRIDVIEWS_COLUMNS FieldValidators.ListView';
	exec dbo.spGRIDVIEWS_InsertOnly           'FieldValidators.ListView', 'FieldValidators', 'vwFIELD_VALIDATORS';
	exec dbo.spGRIDVIEWS_COLUMNS_InsHyperLink 'FieldValidators.ListView'   , 2, 'FieldValidators.LBL_LIST_NAME'             , 'NAME'            , 'NAME'            , '50%', 'listViewTdLinkS1', 'ID', '~/Administration/FieldValidators/view.aspx?id={0}', null, 'FieldValidators', null;
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'FieldValidators.ListView'   , 3, 'FieldValidators.LBL_LIST_VALIDATION_TYPE'  , 'VALIDATION_TYPE' , 'VALIDATION_TYPE' , '50%';
end else begin
	exec dbo.spGRIDVIEWS_COLUMNS_ReserveIndex null, 'FieldValidators.ListView', 2;
end -- if;
GO

set nocount off;
GO


