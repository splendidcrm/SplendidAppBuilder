if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spSqlGetTransactionToken' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spSqlGetTransactionToken;
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
-- 04/10/2008 Paul.  sp_getbindtoken may not be accessible in a hosted environment. 
-- Wrap sp_getbindtoken in a procedure that can be bypassed.
-- The EXECUTE permission was denied on the object 'sp_getbindtoken', database 'mssqlsystemresource', 
-- 04/23/2008 Paul.  The token must be a varchar. 
-- Implicit conversion from data type nvarchar to varchar is not allowed. Use the CONVERT function to run this query.
-- 09/27/2009 Paul.  Allow the Azure commands to be enabled by the SplendidCRM Configuration Wizard.
-- 05/11/2013 Paul.  Dynamically create the procedure so that the same code would work on SQL Server and SQL Azure. 
-- 01/30/2016 Paul.  sp_getbindtoken was added back to SQL Azure, though I'm not sure if we should re-enable. 
declare @Command varchar(max);
if charindex('Microsoft SQL Azure', @@VERSION) > 0 begin -- then
	print 'Microsoft SQL Azure';
	set @Command = 'Create Procedure dbo.spSqlGetTransactionToken(@TRANSACTION_TOKEN varchar(255) out)
as
  begin
	set nocount on

	if @@TRANCOUNT > 0 begin -- then
		-- 09/15/2009 Paul.  If sp_getbindtoken is not available, use date and time until we can find a better solution. 
		-- set @TRANSACTION_TOKEN = convert(varchar(19), getutcdate(), 120);

		-- 10/07/2009 Paul.  MODIFIED_USER_ID will never be NULL, though it may be 00000000-0000-0000-0000-000000000000. 
		select @TRANSACTION_TOKEN = cast(ID as char(36)) + '','' + cast(MODIFIED_USER_ID as char(36))
		  from SYSTEM_TRANSACTIONS
		 where SESSION_SPID = @@SPID;
		-- 10/07/2009 Paul.  The SPID should always exists, but just in case, lets create it if it does not exist. 
		-- One possible reason for it not existing is if the database is modified internally. 
		if @TRANSACTION_TOKEN is null begin -- then
			declare @ID uniqueidentifier;
			exec dbo.spSYSTEM_TRANSACTIONS_Create @ID out, null;

			select @TRANSACTION_TOKEN = cast(ID as char(36)) + '','' + cast(MODIFIED_USER_ID as char(36))
			  from SYSTEM_TRANSACTIONS
			 where SESSION_SPID = @@SPID;
		end -- if;
	end -- if;
  end
';
	exec(@Command);
end else begin
	print 'Microsoft SQL Server';
	set @Command = 'Create Procedure dbo.spSqlGetTransactionToken(@TRANSACTION_TOKEN varchar(255) out)
as
  begin
	set nocount on

	if @@TRANCOUNT > 0 begin -- then
		exec sp_getbindtoken @TRANSACTION_TOKEN out;
	end -- if;
  end
';
	exec(@Command);
end -- if;
GO


Grant Execute on dbo.spSqlGetTransactionToken to public;
GO

