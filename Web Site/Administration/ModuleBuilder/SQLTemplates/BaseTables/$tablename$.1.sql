if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = '$tablename$' and TABLE_TYPE = 'BASE TABLE')
  begin
	print 'Create Table dbo.$tablename$';
	Create Table dbo.$tablename$
		( ID                                 uniqueidentifier not null default(newid()) constraint PK_$tablename$ primary key
		, DELETED                            bit not null default(0)
		, CREATED_BY                         uniqueidentifier null
		, DATE_ENTERED                       datetime not null default(getdate())
		, MODIFIED_USER_ID                   uniqueidentifier null
		, DATE_MODIFIED                      datetime not null default(getdate())
		, DATE_MODIFIED_UTC                  datetime null default(getutcdate())

$createtablefields$
		)

$createtableindexes$
  end
GO


