print 'MODULES admin';
GO

set nocount on;
GO

-- 03/09/2010 Paul.  Add ModuleBuilder so that admin roles can be applied. 
exec dbo.spMODULES_InsertOnly null, 'ModuleBuilder'         , 'ModuleBuilder.LBL_MODULEBUILDER'      , '~/Administration/ModuleBuilder/'    , 1, 0,  0, 0, 0, 0, 0, 1, null                 , 0, 0, 0, 0, 0, 0;


set nocount off;
GO


