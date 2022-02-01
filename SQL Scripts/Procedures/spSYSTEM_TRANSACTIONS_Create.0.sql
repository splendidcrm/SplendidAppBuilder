if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spSYSTEM_TRANSACTIONS_Create' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spSYSTEM_TRANSACTIONS_Create;
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
-- 10/07/2009 Paul.  The goal will be to use the SQL Server 2008 MERGE statement. 
-- http://weblogs.sqlteam.com/mladenp/archive/2007/08/03/60277.aspx
-- 10/07/2009 Paul.  On SQL Server 2005 and 2008, this function should do nothing. 
-- 05/11/2013 Paul.  Dynamically create the procedure so that the same code would work on SQL Server and SQL Azure. 
declare @Command varchar(max);
if charindex('Microsoft SQL Azure', @@VERSION) > 0 begin -- then
	set @Command = 'Create Procedure dbo.spSYSTEM_TRANSACTIONS_Create
	( @ID               uniqueidentifier output
	, @MODIFIED_USER_ID uniqueidentifier
	)
as
  begin
	set nocount on

	declare @TEMP_SESSION_SPID     int;
	declare @TEMP_MODIFIED_USER_ID uniqueidentifier;

	set @TEMP_SESSION_SPID     = @@SPID;
	set @TEMP_MODIFIED_USER_ID = @MODIFIED_USER_ID;
	if @ID is null begin -- then
		set @ID = newid();
	end -- if;
	if @TEMP_MODIFIED_USER_ID is null begin -- then
		set @TEMP_MODIFIED_USER_ID = ''00000000-0000-0000-0000-000000000000'';
	end -- if;

	merge dbo.SYSTEM_TRANSACTIONS as TARGET
	using (select @ID
	            , @TEMP_MODIFIED_USER_ID
	            , getdate()
	            , @TEMP_SESSION_SPID
	            )
	   as SOURCE( ID
	            , MODIFIED_USER_ID
	            , DATE_MODIFIED
	            , SESSION_SPID
	            )
	   on (TARGET.SESSION_SPID = SOURCE.SESSION_SPID)
	 when matched then
		update set TARGET.ID               = SOURCE.ID              
		         , TARGET.MODIFIED_USER_ID = SOURCE.MODIFIED_USER_ID
		         , TARGET.DATE_MODIFIED    = SOURCE.DATE_MODIFIED   
	 when not matched then
		insert
			( ID              
			, MODIFIED_USER_ID
			, DATE_MODIFIED   
			, SESSION_SPID    
			)
		values
			( SOURCE.ID              
			, SOURCE.MODIFIED_USER_ID
			, SOURCE.DATE_MODIFIED   
			, SOURCE.SESSION_SPID    
			);
  end
';
	exec(@Command);
end else begin
	set @Command = 'Create Procedure dbo.spSYSTEM_TRANSACTIONS_Create
	( @ID               uniqueidentifier output
	, @MODIFIED_USER_ID uniqueidentifier
	)
as
  begin
	set nocount on

  end
';
	exec(@Command);
end -- if;
GO

Grant Execute on dbo.spSYSTEM_TRANSACTIONS_Create to public;
GO

