set nocount on;
GO

-- 09/12/2009 Paul.  Allow editing of Field Validators. 
-- delete from DETAILVIEWS_FIELDS where DETAIL_NAME = 'FieldValidators.DetailView';
-- 12/12/2010 Paul.  Missing last parameter in spDETAILVIEWS_InsertOnly. 
if not exists(select * from DETAILVIEWS_FIELDS where DETAIL_NAME = 'FieldValidators.DetailView' and DELETED = 0) begin -- then
	print 'DETAILVIEWS_FIELDS FieldValidators.DetailView';
	exec dbo.spDETAILVIEWS_InsertOnly          'FieldValidators.DetailView', 'FieldValidators', 'vwFIELD_VALIDATORS', '15%', '35%', null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'FieldValidators.DetailView',  0, 'FieldValidators.LBL_NAME'              , 'NAME'              , '{0}', null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'FieldValidators.DetailView',  1, 'FieldValidators.LBL_VALIDATION_TYPE'   , 'VALIDATION_TYPE'   , '{0}', null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'FieldValidators.DetailView',  2, 'FieldValidators.LBL_REGULAR_EXPRESSION', 'REGULAR_EXPRESSION', '{0}', null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'FieldValidators.DetailView',  3, 'FieldValidators.LBL_DATA_TYPE'         , 'DATA_TYPE'         , '{0}', null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'FieldValidators.DetailView',  4, 'FieldValidators.LBL_MININUM_VALUE'     , 'MININUM_VALUE'     , '{0}', null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'FieldValidators.DetailView',  5, 'FieldValidators.LBL_MAXIMUM_VALUE'     , 'MAXIMUM_VALUE'     , '{0}', null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'FieldValidators.DetailView',  6, 'FieldValidators.LBL_COMPARE_OPERATOR'  , 'COMPARE_OPERATOR'  , '{0}', null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBlank     'FieldValidators.DetailView',  7, null;
end -- if;
GO

set nocount off;
GO


