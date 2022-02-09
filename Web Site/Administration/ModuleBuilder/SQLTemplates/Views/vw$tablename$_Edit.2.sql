if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vw$tablename$_Edit')
	Drop View dbo.vw$tablename$_Edit;
GO


Create View dbo.vw$tablename$_Edit
as
select *
  from vw$tablename$

GO

Grant Select on dbo.vw$tablename$_Edit to public;
GO


