exec dbo.spTERMINOLOGY_InsertOnly N'LBL_LIST_FORM_TITLE'                                   , N'en-US', N'$modulename$', null, null, N'$displaynamesingular$ List';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_NEW_FORM_TITLE'                                    , N'en-US', N'$modulename$', null, null, N'Create $displaynamesingular$';
exec dbo.spTERMINOLOGY_InsertOnly N'LNK_$tablenamesingular$_LIST'                          , N'en-US', N'$modulename$', null, null, N'$displayname$';
exec dbo.spTERMINOLOGY_InsertOnly N'LNK_NEW_$tablenamesingular$'                           , N'en-US', N'$modulename$', null, null, N'Create $displaynamesingular$';
exec dbo.spTERMINOLOGY_InsertOnly N'LNK_REPORTS'                                           , N'en-US', N'$modulename$', null, null, N'$displaynamesingular$ Reports';
exec dbo.spTERMINOLOGY_InsertOnly N'ERR_$tablenamesingular$_NOT_FOUND'                     , N'en-US', N'$modulename$', null, null, N'$displaynamesingular$ not found.';
exec dbo.spTERMINOLOGY_InsertOnly N'NTC_REMOVE_$tablenamesingular$_CONFIRMATION'           , N'en-US', N'$modulename$', null, null, N'Are you sure?';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_MODULE_NAME'                                       , N'en-US', N'$modulename$', null, null, N'$displayname$';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_MODULE_ABBREVIATION'                               , N'en-US', N'$modulename$', null, null, N'$abbreviatedname$';

exec dbo.spTERMINOLOGY_InsertOnly N'$modulename$'                                          , N'en-US', null, N'moduleList', 100, N'$displayname$';

$moduleterminology$

$relatedterminology$

