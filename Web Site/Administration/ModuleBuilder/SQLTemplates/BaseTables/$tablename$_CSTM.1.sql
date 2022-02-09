if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = '$tablename$_CSTM' and TABLE_TYPE = 'BASE TABLE')
  begin
	print 'Create Table dbo.$tablename$_CSTM';
	Create Table dbo.$tablename$_CSTM
		( ID_C                               uniqueidentifier not null constraint PK_$tablename$_CSTM primary key
		)
  end
GO


