if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spUSER_PREFERENCES_InsertByUser' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spUSER_PREFERENCES_InsertByUser;
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
-- 10/21/2008 Paul.  Increase USER_NAME to 60 to match table. 
Create Procedure dbo.spUSER_PREFERENCES_InsertByUser
	( @ID                uniqueidentifier output
	, @MODIFIED_USER_ID  uniqueidentifier
	, @USER_NAME         nvarchar(60)
	, @CATEGORY          nvarchar(255)
	)
as
  begin
	set nocount on

	declare @ASSIGNED_USER_ID uniqueidentifier;
	declare @TEMP_USER_NAME   nvarchar(60);
	declare @TEMP_CATEGORY    nvarchar(255);
	-- 01/25/2007 Paul.  Convert to lowercase to support Oracle. 	
	set @TEMP_CATEGORY  = lower(@CATEGORY );
	set @TEMP_USER_NAME = lower(@USER_NAME);

	-- BEGIN Oracle Exception
		select @ASSIGNED_USER_ID = ID
		  from USERS
		 where lower(USER_NAME) = @TEMP_USER_NAME
		   and DELETED          = 0;
	-- END Oracle Exception

	exec dbo.spUSER_PREFERENCES_Insert @ID out, @MODIFIED_USER_ID, @ASSIGNED_USER_ID, @TEMP_CATEGORY;
  end
GO

Grant Execute on dbo.spUSER_PREFERENCES_InsertByUser to public;
GO

