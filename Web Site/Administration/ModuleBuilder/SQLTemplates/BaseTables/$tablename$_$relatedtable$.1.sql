if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = '$tablename$_$relatedtable$' and TABLE_TYPE = 'BASE TABLE')
  begin
	print 'Create Table dbo.$tablename$_$relatedtable$';
	Create Table dbo.$tablename$_$relatedtable$
		( ID                                 uniqueidentifier not null default(newid()) constraint PK_$tablename$_$relatedtable$ primary key
		, DELETED                            bit not null default(0)
		, CREATED_BY                         uniqueidentifier null
		, DATE_ENTERED                       datetime not null default(getdate())
		, MODIFIED_USER_ID                   uniqueidentifier null
		, DATE_MODIFIED                      datetime not null default(getdate())
		, DATE_MODIFIED_UTC                  datetime null default(getutcdate())

		, $tablenamesingular$_ID             uniqueidentifier not null
		, $relatedtablesingular$_ID          uniqueidentifier not null
		)

	create index IDX_$tablename$_$relatedtable$_$tablenamesingular$_ID    on dbo.$tablename$_$relatedtable$ ($tablenamesingular$_ID, $relatedtablesingular$_ID, DELETED)
	create index IDX_$tablename$_$relatedtable$_$relatedtablesingular$_ID on dbo.$tablename$_$relatedtable$ ($relatedtablesingular$_ID, $tablenamesingular$_ID, DELETED)
  end
GO


