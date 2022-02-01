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
-- 03/06/2008 Paul.  All tables should have MODIFIED_USER_ID and DATE_MODIFIED.
-- 07/26/2008 Paul.  Add AUDIT_ACTION to speed workflow processing. 
-- drop table WORKFLOW_EVENTS;
-- 12/03/2008 Paul.  AUDIT_PARENT_ID is needed to roll-up events within a transaction. 
-- 10/28/2009 Paul.  Add UTC date to allow this table to sync. 
if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'WORKFLOW_EVENTS' and TABLE_TYPE = 'BASE TABLE')
  begin
	print 'Create Table dbo.WORKFLOW_EVENTS';
	Create Table dbo.WORKFLOW_EVENTS
		( ID                                 uniqueidentifier not null default(newid()) constraint PK_WORKFLOW_EVENTS primary key
		, DELETED                            bit not null default(0)
		, CREATED_BY                         uniqueidentifier null
		, DATE_ENTERED                       datetime not null default(getdate())
		, MODIFIED_USER_ID                   uniqueidentifier null
		, DATE_MODIFIED                      datetime not null default(getdate())
		, DATE_MODIFIED_UTC                  datetime null default(getutcdate())

		, AUDIT_VERSION                      rowversion not null
		, AUDIT_ID                           uniqueidentifier not null
		, AUDIT_TABLE                        nvarchar(50) not null
		, AUDIT_TOKEN                        varchar(255) null
		, AUDIT_ACTION                       int null
		, AUDIT_PARENT_ID                    uniqueidentifier null
		)
  end
GO


