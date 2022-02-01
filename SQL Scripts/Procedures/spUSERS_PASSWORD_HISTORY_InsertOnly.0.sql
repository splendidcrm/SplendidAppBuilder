if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spUSERS_PASSWORD_HISTORY_InsertOnly' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spUSERS_PASSWORD_HISTORY_InsertOnly;
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
Create Procedure dbo.spUSERS_PASSWORD_HISTORY_InsertOnly
	( @MODIFIED_USER_ID  uniqueidentifier
	, @USER_ID           uniqueidentifier
	, @USER_HASH         nvarchar(32)
	)
as
  begin
	set nocount on
	
	declare @HistoryMax   int;
	declare @HistoryCount int;
	declare @OLDEST_ID    uniqueidentifier;

	insert into USERS_PASSWORD_HISTORY
		( ID               
		, CREATED_BY       
		, DATE_ENTERED     
		, MODIFIED_USER_ID 
		, DATE_MODIFIED    
		, USER_ID          
		, USER_HASH        
		)
	values
		(  newid()          
		, @MODIFIED_USER_ID 
		,  getdate()        
		, @MODIFIED_USER_ID 
		,  getdate()        
		, @USER_ID          
		, @USER_HASH        
		);

	set @HistoryMax = dbo.fnCONFIG_Int(N'Password.HistoryMaximum');
	if @HistoryMax is null or @HistoryMax < 0 begin -- then
		set @HistoryMax = 0;
	end -- if;

-- #if SQL_Server /*
	select @HistoryCount = count(*)
	  from USERS_PASSWORD_HISTORY
	 where USER_ID     = @USER_ID;

	while @HistoryCount > @HistoryMax begin -- do
		select top 1 @OLDEST_ID = ID
		  from USERS_PASSWORD_HISTORY
		 where USER_ID     = @USER_ID
		 order by DATE_ENTERED;
		
		delete from USERS_PASSWORD_HISTORY
		  where ID = @OLDEST_ID;
		
		select @HistoryCount = count(*)
		  from USERS_PASSWORD_HISTORY
		 where USER_ID     = @USER_ID;
	end -- while;
-- #endif SQL_Server */




  end
GO
 
Grant Execute on dbo.spUSERS_PASSWORD_HISTORY_InsertOnly to public;
GO
 
 
