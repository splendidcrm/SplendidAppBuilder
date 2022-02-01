set nocount on;
GO

-- 09/12/2009 Paul.  Allow editing of Field Validators. 
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'FieldValidators.EditView';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'FieldValidators.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS FieldValidators.EditView';
	exec dbo.spEDITVIEWS_InsertOnly          'FieldValidators.EditView' , 'FieldValidators', 'vwFIELD_VALIDATORS', '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound     'FieldValidators.EditView' , 0, 'FieldValidators.LBL_NAME'              , 'NAME'               , 1, 1,   50, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound     'FieldValidators.EditView' , 1, 'FieldValidators.LBL_VALIDATION_TYPE'   , 'VALIDATION_TYPE'    , 1, 1,   50, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound     'FieldValidators.EditView' , 2, 'FieldValidators.LBL_REGULAR_EXPRESSION', 'REGULAR_EXPRESSION' , 0, 1, 2000, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound     'FieldValidators.EditView' , 3, 'FieldValidators.LBL_DATA_TYPE'         , 'DATA_TYPE'          , 0, 1,   25, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound     'FieldValidators.EditView' , 4, 'FieldValidators.LBL_MININUM_VALUE'     , 'MININUM_VALUE'      , 0, 1,  255, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound     'FieldValidators.EditView' , 5, 'FieldValidators.LBL_MAXIMUM_VALUE'     , 'MAXIMUM_VALUE'      , 0, 1,  255, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound     'FieldValidators.EditView' , 6, 'FieldValidators.LBL_COMPARE_OPERATOR'  , 'COMPARE_OPERATOR'   , 0, 1,   25, 35, null;

end -- if;
GO

set nocount off;
GO


