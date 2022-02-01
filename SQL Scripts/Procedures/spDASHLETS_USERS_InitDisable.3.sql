if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spDASHLETS_USERS_InitDisable' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spDASHLETS_USERS_InitDisable;
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
-- 09/24/2009 Paul.  The new Silverlight charts exceeded the control name length of 50. 
Create Procedure dbo.spDASHLETS_USERS_InitDisable
	( @MODIFIED_USER_ID uniqueidentifier
	, @ASSIGNED_USER_ID uniqueidentifier
	, @DETAIL_NAME      nvarchar(50)
	, @MODULE_NAME      nvarchar(50)
	, @CONTROL_NAME     nvarchar(100)
	)
as
  begin
	set nocount on

	declare @ID uniqueidentifier;
	exec dbo.spDASHLETS_USERS_Init @MODIFIED_USER_ID, @ASSIGNED_USER_ID, @DETAIL_NAME;

	-- BEGIN Oracle Exception
		select @ID = ID
		  from DASHLETS_USERS
		 where ASSIGNED_USER_ID     = @ASSIGNED_USER_ID 
		   and DETAIL_NAME          = @DETAIL_NAME      
		   and MODULE_NAME          = @MODULE_NAME      
		   and CONTROL_NAME         = @CONTROL_NAME     
		   and DELETED              = 0                 ;
	-- END Oracle Exception

	exec dbo.spDASHLETS_USERS_Disable @ID, @MODIFIED_USER_ID;
  end
GO

Grant Execute on dbo.spDASHLETS_USERS_InitDisable to public;
GO

