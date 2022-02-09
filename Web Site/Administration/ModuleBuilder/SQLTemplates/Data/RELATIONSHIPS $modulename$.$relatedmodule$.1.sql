
exec dbo.spRELATIONSHIPS_InsertOnly '$modulename$_$relatedmodule$', '$modulename$', '$tablename$', 'ID', '$relatedmodule$', '$relatedtable$', 'ID', '$tablename$_$relatedtable$', '$tablenamesingular$_ID', '$relatedtablesingular$_ID', 'many-to-many', null, null, 0;
GO

