if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spSYSTEM_LOG_InsertOnly' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spSYSTEM_LOG_InsertOnly;
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
-- 09/15/2009 Paul.  Convert data type to nvarchar(max) to support Azure. 
Create Procedure dbo.spSYSTEM_LOG_InsertOnly
	( @MODIFIED_USER_ID  uniqueidentifier
	, @USER_ID           uniqueidentifier
	, @USER_NAME         nvarchar(255)
	, @MACHINE           nvarchar(60)
	, @ASPNET_SESSIONID  nvarchar(50)
	, @REMOTE_HOST       nvarchar(100)
	, @SERVER_HOST       nvarchar(100)
	, @TARGET            nvarchar(255)
	, @RELATIVE_PATH     nvarchar(255)
	, @PARAMETERS        nvarchar(2000)
	, @ERROR_TYPE        nvarchar(25)
	, @FILE_NAME         nvarchar(255)
	, @METHOD            nvarchar(450)
	, @LINE_NUMBER       int
	, @MESSAGE           nvarchar(max)
	)
as
  begin
	set nocount on
	
	declare @ID uniqueidentifier;
	set @ID = newid();
	insert into SYSTEM_LOG
		( ID               
		, CREATED_BY       
		, DATE_ENTERED     
		, MODIFIED_USER_ID 
		, DATE_MODIFIED    
		, USER_ID          
		, USER_NAME        
		, MACHINE          
		, ASPNET_SESSIONID 
		, REMOTE_HOST      
		, SERVER_HOST      
		, TARGET           
		, RELATIVE_PATH    
		, PARAMETERS       
		, ERROR_TYPE       
		, FILE_NAME        
		, METHOD           
		, LINE_NUMBER      
		, MESSAGE          
		)
	values 	( @ID               
		, @MODIFIED_USER_ID       
		,  getdate()        
		, @MODIFIED_USER_ID 
		,  getdate()        
		, @USER_ID          
		, @USER_NAME        
		, @MACHINE          
		, @ASPNET_SESSIONID 
		, @REMOTE_HOST      
		, @SERVER_HOST      
		, @TARGET           
		, @RELATIVE_PATH    
		, @PARAMETERS       
		, @ERROR_TYPE       
		, @FILE_NAME        
		, @METHOD           
		, @LINE_NUMBER      
		, @MESSAGE          
		);
  end
GO

Grant Execute on dbo.spSYSTEM_LOG_InsertOnly to public;
GO

