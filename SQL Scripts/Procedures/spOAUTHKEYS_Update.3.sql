if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spOAUTHKEYS_Update' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spOAUTHKEYS_Update;
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
-- 04/09/2012 Paul.  Twitter has a 40 character verifier. 
-- 04/13/2012 Paul.  Facebook has a 111 character access token. 
Create Procedure dbo.spOAUTHKEYS_Update
	( @MODIFIED_USER_ID   uniqueidentifier
	, @ASSIGNED_USER_ID   uniqueidentifier
	, @NAME               nvarchar(25)
	, @TOKEN              nvarchar(200)
	, @SECRET             nvarchar(50)
	, @VERIFIER           nvarchar(50)
	)
as
  begin
	set nocount on
	
	declare @ID uniqueidentifier;
	
	exec dbo.spOAUTHKEYS_Delete @MODIFIED_USER_ID, @ASSIGNED_USER_ID, @NAME;
	
	set @ID = newid();
	insert into OAUTHKEYS
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
		, VERIFIER          
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
		, @VERIFIER          
		);
  end
GO

Grant Execute on dbo.spOAUTHKEYS_Update to public;
GO

