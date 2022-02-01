if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spUSERS_LOGINS_InsertOnly' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spUSERS_LOGINS_InsertOnly;
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
-- 08/27/2008 Paul.  PostgreSQL does not allow modifying input parameters.  Use a local temp variable. 
Create Procedure dbo.spUSERS_LOGINS_InsertOnly
	( @ID                uniqueidentifier output
	, @MODIFIED_USER_ID  uniqueidentifier
	, @USER_ID           uniqueidentifier
	, @USER_NAME         nvarchar(60)
	, @LOGIN_TYPE        nvarchar(25)
	, @LOGIN_STATUS      nvarchar(25)
	, @ASPNET_SESSIONID  nvarchar(50)
	, @REMOTE_HOST       nvarchar(100)
	, @SERVER_HOST       nvarchar(100)
	, @TARGET            nvarchar(255)
	, @RELATIVE_PATH     nvarchar(255)
	, @USER_AGENT        nvarchar(255)
	)
as
  begin
	set nocount on
	
	declare @TEMP_USER_ID uniqueidentifier;
	set @TEMP_USER_ID = @USER_ID;
	-- 03/02/2008 Paul.  Even though the login has failed, 
	-- try and find the user that attempted the login. 
	if dbo.fnIsEmptyGuid(@TEMP_USER_ID) = 1 begin -- then
		-- BEGIN Oracle Exception
			select @TEMP_USER_ID = ID
			  from vwUSERS_Login
			 where lower(USER_NAME) = lower(@USER_NAME);
		-- END Oracle Exception
	end -- if;

	set @ID = newid();
	insert into USERS_LOGINS
		( ID               
		, CREATED_BY       
		, DATE_ENTERED     
		, MODIFIED_USER_ID 
		, DATE_MODIFIED    
		, USER_ID          
		, USER_NAME        
		, LOGIN_TYPE       
		, LOGIN_DATE       
		, LOGIN_STATUS     
		, ASPNET_SESSIONID 
		, REMOTE_HOST      
		, SERVER_HOST      
		, TARGET           
		, RELATIVE_PATH    
		, USER_AGENT       
		)
	values 	( @ID               
		, @MODIFIED_USER_ID       
		,  getdate()        
		, @MODIFIED_USER_ID 
		,  getdate()        
		, @TEMP_USER_ID     
		, @USER_NAME        
		, @LOGIN_TYPE       
		,  getdate()        
		, @LOGIN_STATUS     
		, @ASPNET_SESSIONID 
		, @REMOTE_HOST      
		, @SERVER_HOST      
		, @TARGET           
		, @RELATIVE_PATH    
		, @USER_AGENT       
		);
  end
GO

Grant Execute on dbo.spUSERS_LOGINS_InsertOnly to public;
GO

