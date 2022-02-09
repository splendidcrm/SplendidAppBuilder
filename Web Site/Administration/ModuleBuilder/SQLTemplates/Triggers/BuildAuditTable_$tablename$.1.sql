-- 01/19/2010 Paul.  Don't create the audit tables on an Offline Client database. 
if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'SYSTEM_SYNC_CONFIG' and TABLE_TYPE = 'BASE TABLE') begin -- then
	exec dbo.spSqlBuildAuditTable   '$tablename$';
	exec dbo.spSqlBuildAuditTrigger '$tablename$';
	exec dbo.spSqlBuildAuditView    '$tablename$';
end -- if;
GO


