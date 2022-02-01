

-- Terminology generated from database [SplendidCRM5_50] on 11/18/2010 1:19:35 AM.
print 'TERMINOLOGY Administration Professional en-us';
GO

set nocount on;
GO

exec dbo.spTERMINOLOGY_InsertOnly N'LBL_MODULE_BUILDER'                            , N'en-US', N'Administration', null, null, N'Manage Module Builder.';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_MODULE_BUILDER_TITLE'                      , N'en-US', N'Administration', null, null, N'Module Builder';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_TEAMS_DESC'                                , N'en-US', N'Administration', null, null, N'Manage Teams.';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_TEAMS_TITLE'                               , N'en-US', N'Administration', null, null, N'Teams';
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

call dbo.spTERMINOLOGY_Administration_Professional_en_us()
/

call dbo.spSqlDropProcedure('spTERMINOLOGY_Administration_Professional_en_us')
/
-- #endif IBM_DB2 */
