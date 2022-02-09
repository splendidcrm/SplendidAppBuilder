

print 'CONFIG defaults';
GO

set nocount on;
GO

exec dbo.spCONFIG_InsertOnly null, 'system', 'disable_favorites'              , 'true';
exec dbo.spCONFIG_InsertOnly null, 'system', 'disable_following'              , 'true';
exec dbo.spCONFIG_InsertOnly null, 'system', 'enable_activity_streams'        , 'false';
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

call dbo.spCONFIG_SplendidApp()
/

call dbo.spSqlDropProcedure('spCONFIG_SplendidApp')
/

-- #endif IBM_DB2 */

