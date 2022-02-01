if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spDYNAMIC_BUTTONS_ORDER_MoveItem' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spDYNAMIC_BUTTONS_ORDER_MoveItem;
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
Create Procedure dbo.spDYNAMIC_BUTTONS_ORDER_MoveItem
	( @MODIFIED_USER_ID uniqueidentifier
	, @VIEW_NAME        nvarchar(50)
	, @OLD_INDEX        int
	, @NEW_INDEX        int
	)
as
  begin
	set nocount on
	
	declare @SWAP_ID    uniqueidentifier;
	-- BEGIN Oracle Exception
		select @SWAP_ID   = ID
		  from DYNAMIC_BUTTONS
		 where VIEW_NAME     = @VIEW_NAME
		   and CONTROL_INDEX = @OLD_INDEX
		   and DELETED       = 0;
	-- END Oracle Exception

	if dbo.fnIsEmptyGuid(@SWAP_ID) = 0 and (@OLD_INDEX > @NEW_INDEX or @OLD_INDEX < @NEW_INDEX) begin -- then
		if @OLD_INDEX < @NEW_INDEX begin -- then
			update DYNAMIC_BUTTONS
			   set CONTROL_INDEX     = CONTROL_INDEX - 1
			     , DATE_MODIFIED     = getdate()
			     , DATE_MODIFIED_UTC = getutcdate()
			     , MODIFIED_USER_ID  = @MODIFIED_USER_ID
			 where VIEW_NAME         = @VIEW_NAME
			   and CONTROL_INDEX    >= @OLD_INDEX
			   and CONTROL_INDEX    <= @NEW_INDEX
			   and DELETED           = 0;
		end else if @OLD_INDEX > @NEW_INDEX begin -- then
			update DYNAMIC_BUTTONS
			   set CONTROL_INDEX     = CONTROL_INDEX + 1
			     , DATE_MODIFIED     = getdate()
			     , DATE_MODIFIED_UTC = getutcdate()
			     , MODIFIED_USER_ID  = @MODIFIED_USER_ID
			 where VIEW_NAME         = @VIEW_NAME
			   and CONTROL_INDEX    >= @NEW_INDEX
			   and CONTROL_INDEX    <= @OLD_INDEX
			   and DELETED           = 0;
		end -- if;
		update DYNAMIC_BUTTONS
		   set CONTROL_INDEX     = @NEW_INDEX
		     , DATE_MODIFIED     = getdate()
		     , DATE_MODIFIED_UTC = getutcdate()
		     , MODIFIED_USER_ID  = @MODIFIED_USER_ID
		 where ID                = @SWAP_ID
		   and DELETED           = 0;
	end -- if;
  end
GO

Grant Execute on dbo.spDYNAMIC_BUTTONS_ORDER_MoveItem to public;
GO

