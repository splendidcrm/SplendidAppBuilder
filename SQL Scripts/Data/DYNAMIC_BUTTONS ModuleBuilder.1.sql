

print 'DYNAMIC_BUTTONS ModuleBuilder';
GO

set nocount on;
GO

-- delete from DYNAMIC_BUTTONS where VIEW_NAME = 'ModuleBuilder.WizardView';
if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'ModuleBuilder.WizardView' and DELETED = 0) begin -- then
	print 'DYNAMIC_BUTTONS ModuleBuilder.WizardView';
	exec dbo.spDYNAMIC_BUTTONS_InsButton 'ModuleBuilder.WizardView'  , 0, 'ModuleBuilder'   , 'edit', null, null, 'Back'    , null, '.LBL_BACK_BUTTON_LABEL'                 , '.LBL_BACK_BUTTON_LABEL'                 , null, null, null;
	exec dbo.spDYNAMIC_BUTTONS_InsButton 'ModuleBuilder.WizardView'  , 1, 'ModuleBuilder'   , 'edit', null, null, 'Next'    , null, '.LBL_NEXT_BUTTON_LABEL'                 , '.LBL_NEXT_BUTTON_LABEL'                 , null, null, null;
	exec dbo.spDYNAMIC_BUTTONS_InsButton 'ModuleBuilder.WizardView'  , 2, 'ModuleBuilder'   , 'edit', null, null, 'Generate', null, 'ModuleBuilder.LBL_GENERATE_BUTTON_LABEL', 'ModuleBuilder.LBL_GENERATE_BUTTON_LABEL', null, null, null;
end -- if;
GO


set nocount off;
GO


/* -- #if Oracle
	EXCEPTION
		WHEN NO_DATA_FOUND THEN
			StoO_selcnt := 0;
		WHEN OTHERS THEN
			RAISE;
	END;
	COMMIT WORK;
END;
/
-- #endif Oracle */

/* -- #if IBM_DB2
	commit;
  end
/

call dbo.spDYNAMIC_BUTTONS_ModuleBuilder()
/

call dbo.spSqlDropProcedure('spDYNAMIC_BUTTONS_ModuleBuilder')
/

-- #endif IBM_DB2 */

