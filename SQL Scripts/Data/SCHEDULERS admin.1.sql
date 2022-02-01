print 'SCHEDULERS admin';
GO

set nocount on;
GO

exec dbo.spSCHEDULERS_InsertOnly null, N'Backup Database Sunday at 11pm'             , N'function::BackupDatabase'                              , null, null, N'0::23::*::*::0'  , null, null, N'Active'  , 0;
exec dbo.spSCHEDULERS_InsertOnly null, N'Backup Transaction Log Mon-Sat at 11pm'     , N'function::BackupTransactionLog'                        , null, null, N'0::23::*::*::1-6', null, null, N'Inactive', 0;
-- 02/26/2010 Paul.  Clean the SYSTEM_LOG table of warnings once a week. 
exec dbo.spSCHEDULERS_InsertOnly null, N'Clean System Log Sunday at 10pm'            , N'function::CleanSystemLog'                              , null, null, N'0::22::*::*::0'  , null, null, N'Active'  , 0;

set nocount off;
GO


