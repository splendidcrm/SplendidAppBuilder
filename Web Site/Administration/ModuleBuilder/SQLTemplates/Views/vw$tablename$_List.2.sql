if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vw$tablename$_List')
	Drop View dbo.vw$tablename$_List;
GO


Create View dbo.vw$tablename$_List
as
select *
  from vw$tablename$

GO

Grant Select on dbo.vw$tablename$_List to public;
GO


