if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spDASHLETS_USERS_Enable' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spDASHLETS_USERS_Enable;
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
-- 01/28/2010 Paul.  The ASSIGNED_USER_ID must be used when managing dashlets. 
Create Procedure dbo.spDASHLETS_USERS_Enable
	( @ID               uniqueidentifier
	, @MODIFIED_USER_ID uniqueidentifier
	)
as
  begin
	set nocount on
	
	declare @ASSIGNED_USER_ID uniqueidentifier;
	declare @SWAP_ID       uniqueidentifier;
	declare @DETAIL_NAME   nvarchar(50);
	declare @DASHLET_ORDER int;

	set @ASSIGNED_USER_ID = @MODIFIED_USER_ID;
	if exists(select * from DASHLETS_USERS where ID = @ID and DELETED = 0) begin -- then
		-- BEGIN Oracle Exception
			select @DETAIL_NAME   = DETAIL_NAME
			     , @DASHLET_ORDER = DASHLET_ORDER
			  from DASHLETS_USERS
			 where ID          = @ID
			   and DELETED     = 0;
		-- END Oracle Exception

		-- 01/28/2010 Paul.  The ASSIGNED_USER_ID must be used when managing dashlets. 
		-- BEGIN Oracle Exception
			select @SWAP_ID = ID
			  from DASHLETS_USERS
			 where ASSIGNED_USER_ID = @ASSIGNED_USER_ID 
			   and DETAIL_NAME      = @DETAIL_NAME
			   and DASHLET_ORDER    = 0
			   and DELETED          = 0;
		-- END Oracle Exception
		-- 01/04/2005 Paul.  If there is a module at 0, shift all DASHLETS_USERS so that this one can be 1. 
		if dbo.fnIsEmptyGuid(@SWAP_ID) = 0 begin -- then
			-- 04/02/2006 Paul.  Catch the Oracle NO_DATA_FOUND exception. 
			-- 01/28/2010 Paul.  The ASSIGNED_USER_ID must be used when managing dashlets. 
			-- BEGIN Oracle Exception
				update DASHLETS_USERS
				   set DASHLET_ORDER    = DASHLET_ORDER + 1
				 where ASSIGNED_USER_ID = @ASSIGNED_USER_ID 
				   and DETAIL_NAME      = @DETAIL_NAME
				   and DASHLET_ORDER   >= 0
				   and DELETED          = 0;
			-- END Oracle Exception
		end -- if;
		
		-- 01/04/2006 Paul.  DASHLETS_USERS made visible start at tab 0. 
		-- BEGIN Oracle Exception
			update DASHLETS_USERS
			   set MODIFIED_USER_ID = @MODIFIED_USER_ID 
			     , DATE_MODIFIED    =  getdate()        
			     , DATE_MODIFIED_UTC=  getutcdate()     
			     , DASHLET_ORDER    = 0
			     , DASHLET_ENABLED  = 1
			 where ID               = @ID
			   and DELETED          = 0;
		-- END Oracle Exception
	end -- if;
  end
GO

Grant Execute on dbo.spDASHLETS_USERS_Enable to public;
GO

