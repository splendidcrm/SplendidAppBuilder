
-- delete from SHORTCUTS where MODULE_NAME = '$modulename$';
if not exists (select * from SHORTCUTS where MODULE_NAME = '$modulename$' and DELETED = 0) begin -- then
	exec dbo.spSHORTCUTS_InsertOnly null, '$modulename$', '$modulename$.LNK_NEW_$tablenamesingular$' , '~/$administrationfolder$$modulename$/edit.aspx'   , 'Create$modulename$.gif', 1,  1, '$modulename$', 'edit';
	exec dbo.spSHORTCUTS_InsertOnly null, '$modulename$', '$modulename$.LNK_$tablenamesingular$_LIST', '~/$administrationfolder$$modulename$/default.aspx', '$modulename$.gif'      , 1,  2, '$modulename$', 'list';
	exec dbo.spSHORTCUTS_InsertOnly null, '$modulename$', '.LBL_IMPORT'              , '~/$administrationfolder$$modulename$/import.aspx' , 'Import.gif'        , 1,  3, '$modulename$', 'import';
end -- if;
GO

