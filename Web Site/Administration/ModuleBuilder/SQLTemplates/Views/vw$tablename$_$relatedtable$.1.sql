if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vw$tablename$_$relatedtable$')
	Drop View dbo.vw$tablename$_$relatedtable$;
GO


Create View dbo.vw$tablename$_$relatedtable$
as
select vw$tablename$.ID               as $tablenamesingular$_ID
     , vw$tablename$.NAME             as $tablenamesingular$_NAME
$relatedviewassigned$
     , vw$relatedtable$.ID                 as $relatedtablesingular$_ID
     , vw$relatedtable$.NAME               as $relatedtablesingular$_NAME
     , vw$relatedtable$.*
  from           vw$tablename$
      inner join $tablename$_$relatedtable$
              on $tablename$_$relatedtable$.$tablenamesingular$_ID = vw$tablename$.ID
             and $tablename$_$relatedtable$.DELETED    = 0
      inner join vw$relatedtable$
              on vw$relatedtable$.ID                = $tablename$_$relatedtable$.$relatedtablesingular$_ID

GO

Grant Select on dbo.vw$tablename$_$relatedtable$ to public;
GO


