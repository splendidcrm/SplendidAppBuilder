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
-- 04/21/2006 Paul.  Added in SugarCRM 4.0.
-- 12/30/2007 Paul.  Allow DATE_TIME_START to be null. 
-- 10/28/2009 Paul.  Add UTC date to allow this table to sync. 
if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'SCHEDULERS' and TABLE_TYPE = 'BASE TABLE')
  begin
	print 'Create Table dbo.SCHEDULERS';
	Create Table dbo.SCHEDULERS
		( ID                                 uniqueidentifier not null default(newid()) constraint PK_SCHEDULERS primary key
		, DELETED                            bit not null default(0)
		, CREATED_BY                         uniqueidentifier null
		, DATE_ENTERED                       datetime not null default(getdate())
		, MODIFIED_USER_ID                   uniqueidentifier null
		, DATE_MODIFIED                      datetime not null default(getdate())
		, DATE_MODIFIED_UTC                  datetime null default(getutcdate())

		, NAME                               nvarchar(255) not null
		, JOB                                nvarchar(255) not null
		, DATE_TIME_START                    datetime null
		, DATE_TIME_END                      datetime null
		, JOB_INTERVAL                       nvarchar(100) not null
		, TIME_FROM                          datetime null
		, TIME_TO                            datetime null
		, LAST_RUN                           datetime null
		, STATUS                             nvarchar(25) null
		, CATCH_UP                           bit null default(1)
		)

	create index IDX_SCHEDULERS_DATE_TIME_START on dbo.SCHEDULERS (DATE_TIME_START, DELETED)
  end
GO


