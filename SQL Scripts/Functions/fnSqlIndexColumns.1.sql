if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'fnSqlIndexColumns' and ROUTINE_TYPE = 'FUNCTION')
	Drop Function dbo.fnSqlIndexColumns;
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
-- 09/06/2010 Paul.  Help with migration with EffiProz. 
Create Function dbo.fnSqlIndexColumns(@TABLE_NAME sysname, @object_id int, @index_id tinyint)
returns varchar(4000)
as 
  begin
	declare @colnames    varchar(4000);
	declare @thisColID   int;
	declare @thisColName sysname;
	
	set @colnames = index_col(@table_name, @index_id, 1) + (case indexkey_property(@object_id, @index_id, 1, 'IsDescending') when 1 then ' DESC' else '' end);
	set @thisColID   = 2;
	set @thisColName = index_col(@table_name, @index_id, @thisColID) + (case indexkey_property(@object_id, @index_id, @thisColID, 'IsDescending') when 1 then ' DESC' else '' end);

	while @thisColName is not null begin -- do
		set @thisColID   = @thisColID + 1;
		set @colnames    = @colnames + ', ' + @thisColName;
		set @thisColName = index_col(@table_name, @index_id, @thisColID) + (case indexkey_property(@object_id, @index_id, @thisColID, 'IsDescending') when 1 then ' DESC' else '' end);
	end -- while;
	return upper(@colNames);
  end
GO

Grant Execute on dbo.fnSqlIndexColumns to public
GO

