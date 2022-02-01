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
-- 01/20/2010 Paul.  We don't need any default values as this table will be populated by values from WORKFLOW_EVENTS. 
if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'AUDIT_EVENTS' and TABLE_TYPE = 'BASE TABLE')
  begin
	print 'Create Table dbo.AUDIT_EVENTS';
	Create Table dbo.AUDIT_EVENTS
		( ID                                 uniqueidentifier not null constraint PK_AUDIT_EVENTS primary key
		, DELETED                            bit not null default(0)
		, CREATED_BY                         uniqueidentifier null
		, DATE_ENTERED                       datetime not null
		, MODIFIED_USER_ID                   uniqueidentifier null
		, DATE_MODIFIED                      datetime not null
		, DATE_MODIFIED_UTC                  datetime null default(getutcdate())

		, AUDIT_ID                           uniqueidentifier not null
		, AUDIT_TABLE                        nvarchar(50) not null
		, AUDIT_TOKEN                        varchar(255) null
		, AUDIT_ACTION                       int null
		, AUDIT_PARENT_ID                    uniqueidentifier null
		)
  end
GO


