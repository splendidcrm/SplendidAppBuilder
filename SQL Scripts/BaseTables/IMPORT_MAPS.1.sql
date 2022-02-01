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
-- 10/08/2006 Paul.  Recreate IMPORT_MAPS with NAME, SOURCE and MODULE as nvarchar fields. 
-- 09/15/2009 Paul.  Convert data type to nvarchar(max) to support Azure. 
-- 10/28/2009 Paul.  Add UTC date to allow this table to sync. 
-- 09/17/2013 Paul.  Add Business Rules to import. 
if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'IMPORT_MAPS' and TABLE_TYPE = 'BASE TABLE')
  begin
	print 'Create Table dbo.IMPORT_MAPS';
	Create Table dbo.IMPORT_MAPS
		( ID                                 uniqueidentifier not null default(newid()) constraint PK_IMPORT_MAPS primary key
		, DELETED                            bit not null default(0)
		, CREATED_BY                         uniqueidentifier null
		, DATE_ENTERED                       datetime not null default(getdate())
		, MODIFIED_USER_ID                   uniqueidentifier null
		, DATE_MODIFIED                      datetime not null default(getdate())
		, DATE_MODIFIED_UTC                  datetime null default(getutcdate())

		, ASSIGNED_USER_ID                   uniqueidentifier null
		, NAME                               nvarchar(150) null
		, SOURCE                             nvarchar(25) null
		, MODULE                             nvarchar(25) not null
		, HAS_HEADER                         bit not null default(1)
		, IS_PUBLISHED                       bit not null default(0)
		, CONTENT                            nvarchar(max) null
		, RULES_XML                          nvarchar(max) null
		)

	create index IDX_IMPORT_MAPS_ASSIGNED_USER_ID_MODULE_NAME on dbo.IMPORT_MAPS (ASSIGNED_USER_ID, MODULE, NAME, DELETED)
  end
GO


