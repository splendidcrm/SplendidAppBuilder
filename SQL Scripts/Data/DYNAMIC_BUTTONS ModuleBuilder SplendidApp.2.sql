

print 'DYNAMIC_BUTTONS ModuleBuilder';
GO

set nocount on;
GO

-- delete from DYNAMIC_BUTTONS where VIEW_NAME = 'ModuleBuilder.WizardView';
if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'ModuleBuilder.WizardView' and COMMAND_NAME = 'SelectTemplate' and DELETED = 0) begin -- then
	print 'DYNAMIC_BUTTONS ModuleBuilder.WizardView SplendidApp';
	exec dbo.spDYNAMIC_BUTTONS_InsButton 'ModuleBuilder.WizardView'  , 3, 'ModuleBuilder'   , 'edit', null, null, 'SelectTemplate', null, 'ModuleBuilder.LBL_SELECT_TEMPLATE_LABEL', 'ModuleBuilder.LBL_SELECT_TEMPLATE_LABEL', null, null, null;
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

