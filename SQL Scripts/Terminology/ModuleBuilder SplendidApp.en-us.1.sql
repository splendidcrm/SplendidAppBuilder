

print 'TERMINOLOGY ModuleBuilder en-us';
GO

set nocount on;
GO

exec dbo.spTERMINOLOGY_InsertOnly N'LBL_SELECT_TEMPLATE_LABEL' , N'en-US', N'ModuleBuilder', null, null, N'Select from Template';
GO

set nocount off;
GO

/* -- #if Oracle
	COMMIT WORK;
END;
/
-- #endif Oracle */

/* -- #if IBM_DB2
	commit;
  end
/

call dbo.spTERMINOLOGY_ModuleBuilder_en_us()
/

call dbo.spSqlDropProcedure('spTERMINOLOGY_ModuleBuilder_en_us')
/
-- #endif IBM_DB2 */
