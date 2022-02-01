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
-- 04/21/2006 Paul.  RELATIONSHIP_ROLE_COLUMN_VALUE was increased to nvarchar(50) in SugarCRM 4.0.
-- 10/28/2009 Paul.  Add UTC date to allow this table to sync. 
if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'RELATIONSHIPS' and TABLE_TYPE = 'BASE TABLE')
  begin
	print 'Create Table dbo.RELATIONSHIPS';
	Create Table dbo.RELATIONSHIPS
		( ID                                 uniqueidentifier not null default(newid()) constraint PK_RELATIONSHIPS primary key
		, DELETED                            bit not null default(0)
		, CREATED_BY                         uniqueidentifier null
		, DATE_ENTERED                       datetime not null default(getdate())
		, MODIFIED_USER_ID                   uniqueidentifier null
		, DATE_MODIFIED                      datetime not null default(getdate())
		, DATE_MODIFIED_UTC                  datetime null default(getutcdate())

		, RELATIONSHIP_NAME                  nvarchar(150) null
		, LHS_MODULE                         nvarchar(100) null
		, LHS_TABLE                          nvarchar( 64) null
		, LHS_KEY                            nvarchar( 64) null
		, RHS_MODULE                         nvarchar(100) null
		, RHS_TABLE                          nvarchar( 64) null
		, RHS_KEY                            nvarchar( 64) null
		, JOIN_TABLE                         nvarchar( 64) null
		, JOIN_KEY_LHS                       nvarchar( 64) null
		, JOIN_KEY_RHS                       nvarchar( 64) null
		, RELATIONSHIP_TYPE                  nvarchar( 64) null
		, RELATIONSHIP_ROLE_COLUMN           nvarchar( 64) null
		, RELATIONSHIP_ROLE_COLUMN_VALUE     nvarchar( 50) null
		, REVERSE                            bit null default(0)
		)

	create index IDX_RELATIONSHIPS_NAME on dbo.RELATIONSHIPS (RELATIONSHIP_NAME)
  end
GO


