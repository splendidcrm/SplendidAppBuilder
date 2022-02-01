if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwSqlForeignKeys')
	Drop View dbo.vwSqlForeignKeys;
GO


/**********************************************************************************************************************
 * Copyright (C) 2005-2022 SplendidCRM Software, Inc. 
 * MIT License
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
 * files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
 * modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
 * is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
 * LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR 
 * IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 *********************************************************************************************************************/
Create View dbo.vwSqlForeignKeys
as
select TABLE_CONSTRAINTS.CONSTRAINT_NAME    as CONSTRAINT_NAME
     , TABLE_CONSTRAINTS.TABLE_SCHEMA       as TABLE_SCHEMA
     , TABLE_CONSTRAINTS.TABLE_NAME         as TABLE_NAME 
     , CONSTRAINT_COLUMN_USAGE.COLUMN_NAME  as COLUMN_NAME
     , PRIMARY_KEYS.TABLE_SCHEMA            as REFERENCED_TABLE_SCHEMA
     , PRIMARY_KEYS.TABLE_NAME              as REFERENCED_TABLE_NAME
     , PRIMARY_COLUMN_USAGE.COLUMN_NAME     as REFERENCED_COLUMN_NAME
  from      INFORMATION_SCHEMA.TABLE_CONSTRAINTS         TABLE_CONSTRAINTS
 inner join INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE   CONSTRAINT_COLUMN_USAGE
         on CONSTRAINT_COLUMN_USAGE.CONSTRAINT_NAME    = TABLE_CONSTRAINTS.CONSTRAINT_NAME
 inner join INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS   REFERENTIAL_CONSTRAINTS
         on REFERENTIAL_CONSTRAINTS.CONSTRAINT_NAME    = TABLE_CONSTRAINTS.CONSTRAINT_NAME
 inner join INFORMATION_SCHEMA.TABLE_CONSTRAINTS         PRIMARY_KEYS
         on PRIMARY_KEYS.CONSTRAINT_NAME               = REFERENTIAL_CONSTRAINTS.UNIQUE_CONSTRAINT_NAME
        and PRIMARY_KEYS.CONSTRAINT_TYPE               = 'PRIMARY KEY'
 inner join INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE   PRIMARY_COLUMN_USAGE
         on PRIMARY_COLUMN_USAGE.CONSTRAINT_NAME       = PRIMARY_KEYS.CONSTRAINT_NAME
 where TABLE_CONSTRAINTS.CONSTRAINT_TYPE = 'FOREIGN KEY'



GO


Grant Select on dbo.vwSqlForeignKeys to public;
GO

