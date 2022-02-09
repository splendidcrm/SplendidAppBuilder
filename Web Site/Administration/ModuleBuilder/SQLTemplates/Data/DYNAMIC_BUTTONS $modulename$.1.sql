
exec dbo.spDYNAMIC_BUTTONS_CopyDefault '.DetailView', '$modulename$.DetailView', '$modulename$';
exec dbo.spDYNAMIC_BUTTONS_CopyDefault '.EditView'  , '$modulename$.EditView'  , '$modulename$';
exec dbo.spDYNAMIC_BUTTONS_CopyDefault '.PopupView' , '$modulename$.PopupView' , '$modulename$';
GO

if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = '$modulename$.EditView' and COMMAND_NAME = 'SaveDuplicate' and DELETED = 0) begin -- then
	exec dbo.spDYNAMIC_BUTTONS_InsSaveDuplicate '$modulename$.EditView', -1, null;
end -- if;
GO

if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = '$modulename$.EditView' and COMMAND_NAME = 'SaveConcurrency' and DELETED = 0) begin -- then
	exec dbo.spDYNAMIC_BUTTONS_InsSaveConcurrency '$modulename$.EditView', -1, null;
end -- if;
GO

