if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spOAUTH_TOKENS_Update' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spOAUTH_TOKENS_Update;
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
-- 04/13/2012 Paul.  Facebook has a 111 character access token. 
-- 09/05/2015 Paul.  Google now uses OAuth 2.0. 
-- 01/19/2017 Paul.  The Microsoft OAuth token can be large, but less than 2000 bytes. 
-- 12/02/2020 Paul.  The Microsoft OAuth token is now about 2400, so increase to 4000 characters.
Create Procedure dbo.spOAUTH_TOKENS_Update
	( @MODIFIED_USER_ID   uniqueidentifier
	, @ASSIGNED_USER_ID   uniqueidentifier
	, @NAME               nvarchar(50)
	, @TOKEN              nvarchar(4000)
	, @SECRET             nvarchar(50)
	, @TOKEN_EXPIRES_AT   datetime = null
	, @REFRESH_TOKEN      nvarchar(4000) = null
	)
as
  begin
	set nocount on

	declare @ID uniqueidentifier;
	
	exec dbo.spOAUTH_TOKENS_Delete @MODIFIED_USER_ID, @ASSIGNED_USER_ID, @NAME;
	
	set @ID = newid();
	insert into OAUTH_TOKENS
		( ID                
		, CREATED_BY        
		, DATE_ENTERED      
		, MODIFIED_USER_ID  
		, DATE_MODIFIED     
		, DATE_MODIFIED_UTC 
		, ASSIGNED_USER_ID  
		, NAME              
		, TOKEN             
		, SECRET            
		, TOKEN_EXPIRES_AT  
		, REFRESH_TOKEN     
		)
	values 	( @ID                
		, @MODIFIED_USER_ID  
		,  getdate()         
		, @MODIFIED_USER_ID  
		,  getdate()         
		,  getutcdate()      
		, @ASSIGNED_USER_ID  
		, @NAME              
		, @TOKEN             
		, @SECRET            
		, @TOKEN_EXPIRES_AT  
		, @REFRESH_TOKEN     
		);
  end
GO

Grant Execute on dbo.spOAUTH_TOKENS_Update to public;
GO

