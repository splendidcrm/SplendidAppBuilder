if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spSCHEDULERS_InsertOnly' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spSCHEDULERS_InsertOnly;
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
-- 12/31/2007 Paul.  Don't need to insert LAST_RUN. 
Create Procedure dbo.spSCHEDULERS_InsertOnly
	( @MODIFIED_USER_ID  uniqueidentifier
	, @NAME              nvarchar(255)
	, @JOB               nvarchar(255)
	, @DATE_TIME_START   datetime
	, @DATE_TIME_END     datetime
	, @JOB_INTERVAL      nvarchar(100)
	, @TIME_FROM         datetime
	, @TIME_TO           datetime
	, @STATUS            nvarchar(25)
	, @CATCH_UP          bit
	)
as
  begin
	set nocount on
	
	declare @ID uniqueidentifier;
	

	-- BEGIN Oracle Exception
		select @ID = ID
		  from SCHEDULERS
		 where NAME    = @NAME
		   and DELETED = 0    ;
	-- END Oracle Exception
	if dbo.fnIsEmptyGuid(@ID) = 1 begin -- then
		set @ID = newid();
		insert into SCHEDULERS
			( ID               
			, CREATED_BY       
			, DATE_ENTERED     
			, MODIFIED_USER_ID 
			, DATE_MODIFIED    
			, NAME             
			, JOB              
			, DATE_TIME_START  
			, DATE_TIME_END    
			, JOB_INTERVAL     
			, TIME_FROM        
			, TIME_TO          
			, STATUS           
			, CATCH_UP         
			)
		values 	( @ID               
			, @MODIFIED_USER_ID       
			,  getdate()        
			, @MODIFIED_USER_ID 
			,  getdate()        
			, @NAME             
			, @JOB              
			, @DATE_TIME_START  
			, @DATE_TIME_END    
			, @JOB_INTERVAL     
			, @TIME_FROM        
			, @TIME_TO          
			, @STATUS           
			, @CATCH_UP         
			);
	end -- if;
  end
GO
 
Grant Execute on dbo.spSCHEDULERS_InsertOnly to public;
GO

