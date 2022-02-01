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
-- 10/28/2009 Paul.  Add UTC date to allow this table to sync. 
-- 04/30/2016 Paul.  Add reference to log entry that modified the record. 
if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'CURRENCIES' and TABLE_TYPE = 'BASE TABLE')
  begin
	print 'Create Table dbo.CURRENCIES';
	Create Table dbo.CURRENCIES
		( ID                                 uniqueidentifier not null default(newid()) constraint PK_CURRENCIES primary key
		, DELETED                            bit not null default(0)
		, CREATED_BY                         uniqueidentifier null
		, DATE_ENTERED                       datetime not null default(getdate())
		, MODIFIED_USER_ID                   uniqueidentifier null
		, DATE_MODIFIED                      datetime not null default(getdate())
		, DATE_MODIFIED_UTC                  datetime null default(getutcdate())

		, NAME                               nvarchar(36) not null
		, SYMBOL                             nvarchar(36) not null
		, ISO4217                            nvarchar(3) not null
		, CONVERSION_RATE                    float not null default(0.0)
		, STATUS                             nvarchar(25) null
		, SYSTEM_CURRENCY_LOG_ID             uniqueidentifier null
		)

	create index IDX_CURRENCIES_NAME on dbo.CURRENCIES (NAME, DELETED)
  end
GO


