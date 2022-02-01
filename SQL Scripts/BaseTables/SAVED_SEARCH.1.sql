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
-- 09/15/2009 Paul.  Convert data type to nvarchar(max) to support Azure. 
-- 10/28/2009 Paul.  Add UTC date to allow this table to sync. 
-- 09/01/2010 Paul.  Store a copy of the DEFAULT_SEARCH_ID in the table so that we don't need to read the XML in order to get the value. 
-- 09/01/2010 Paul.  We also need a separate module-only field so that the query will get all records for the module. 
-- 09/02/1010 Paul.  Adding the default search caused lots of problems, so we are going to ignore the fields for now. 
if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'SAVED_SEARCH' and TABLE_TYPE = 'BASE TABLE')
  begin
	print 'Create Table dbo.SAVED_SEARCH';
	Create Table dbo.SAVED_SEARCH
		( ID                                 uniqueidentifier not null default(newid()) constraint PK_SAVED_SEARCH primary key
		, DELETED                            bit not null default(0)
		, CREATED_BY                         uniqueidentifier null
		, DATE_ENTERED                       datetime not null default(getdate())
		, MODIFIED_USER_ID                   uniqueidentifier null
		, DATE_MODIFIED                      datetime not null default(getdate())
		, DATE_MODIFIED_UTC                  datetime null default(getutcdate())

		, ASSIGNED_USER_ID                   uniqueidentifier null
		, DEFAULT_SEARCH_ID                  uniqueidentifier null
		, NAME                               nvarchar(150) null
		, SEARCH_MODULE                      nvarchar(150) null
		, CONTENTS                           nvarchar(max) null
		, DESCRIPTION                        nvarchar(max) null
		)

	create index IDX_SAVED_SEARCH on dbo.SAVED_SEARCH (ASSIGNED_USER_ID, SEARCH_MODULE, NAME, DELETED, ID)
  end
GO


