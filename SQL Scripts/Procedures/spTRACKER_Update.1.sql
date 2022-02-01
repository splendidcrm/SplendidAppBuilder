if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spTRACKER_Update' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spTRACKER_Update;
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
-- 07/14/2007 Paul.  Add support for DB2.
-- 05/07/2010 Paul.  Instead of trying to group the entries and limit by module,
-- 08/26/2010 Paul.  Restore max on a per-module basis. 
-- 03/08/2012 Paul.  Add ACTION to the tracker table so that we can create quick user activity reports. 
-- 05/05/2013 Paul.  Must include the ACTION in the oldest query. 
Create Procedure dbo.spTRACKER_Update
	( @MODIFIED_USER_ID  uniqueidentifier
	, @USER_ID           uniqueidentifier
	, @MODULE_NAME       nvarchar(25)
	, @ITEM_ID           uniqueidentifier
	, @ITEM_SUMMARY      nvarchar(255)
	, @ACTION            nvarchar(25) = null
	)
as
  begin
	set nocount on
	
	declare @HistoryMax   int;
	declare @HistoryCount int;
	declare @OLDEST_ID    uniqueidentifier;

	-- 07/16/2005 Paul. SugarCRM only keeps one entry per USER_ID/ITEM_ID
	if @ACTION = N'detailview' begin -- then
		-- BEGIN Oracle Exception
			delete from TRACKER
			 where USER_ID = @USER_ID
			   and ITEM_ID = @ITEM_ID
			   and ACTION  = @ACTION
			   and DELETED = 0;
		-- END Oracle Exception
	end -- if;
	
	insert into TRACKER
		( ID               
		, CREATED_BY       
		, DATE_ENTERED     
		, MODIFIED_USER_ID 
		, DATE_MODIFIED    
		, USER_ID          
		, ACTION           
		, MODULE_NAME      
		, ITEM_ID          
		, ITEM_SUMMARY     
		)
	values
		(  newid()          
		, @MODIFIED_USER_ID 
		,  getdate()        
		, @MODIFIED_USER_ID 
		,  getdate()        
		, @USER_ID          
		, @ACTION           
		, @MODULE_NAME      
		, @ITEM_ID          
		, @ITEM_SUMMARY     
		);

	-- Prune any excess tracker items. 
	set @HistoryMax = dbo.fnCONFIG_Int(N'history_max_viewed');
	if @HistoryMax is null or @HistoryMax = 0 begin -- then
		set @HistoryMax = 10;
	end -- if;
	-- 05/07/2010 Paul.  Instead of trying to group the entries and limit by module,
	-- we are simply going to limit the total to 100 and expect it to cover the most popular modules. 
	-- 08/26/2010 Paul.  Restore max on a per-module basis. 
	-- set @HistoryMax = 100;
	
-- #if SQL_Server /*
	if @ACTION = N'detailview' begin -- then
		select @HistoryCount = count(*)
		  from TRACKER
		 where USER_ID     = @USER_ID
		   and MODULE_NAME = @MODULE_NAME
		   and ACTION      = @ACTION
		   and DELETED     = 0;
	
		while @HistoryCount > @HistoryMax begin -- do
			-- 05/05/2013 Paul.  Must include the ACTION in the oldest query. 
			select top 1 @OLDEST_ID = ID
			  from TRACKER
			 where USER_ID     = @USER_ID
			   and MODULE_NAME = @MODULE_NAME
			   and ACTION      = @ACTION
			 order by DATE_ENTERED;
			
			delete from TRACKER
			  where ID = @OLDEST_ID;
			
			select @HistoryCount = count(*)
			  from TRACKER
			 where USER_ID     = @USER_ID
			   and MODULE_NAME = @MODULE_NAME
			   and ACTION      = @ACTION
			   and DELETED     = 0;
		end -- while;
	end -- if;
-- #endif SQL_Server */




  end
GO

Grant Execute on dbo.spTRACKER_Update to public;
GO

