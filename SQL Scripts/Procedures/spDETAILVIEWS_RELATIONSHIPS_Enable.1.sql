if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spDETAILVIEWS_RELATIONSHIPS_Enable' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spDETAILVIEWS_RELATIONSHIPS_Enable;
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
Create Procedure dbo.spDETAILVIEWS_RELATIONSHIPS_Enable
	( @ID               uniqueidentifier
	, @MODIFIED_USER_ID uniqueidentifier
	)
as
  begin
	set nocount on
	
	declare @SWAP_ID            uniqueidentifier;
	declare @DETAIL_NAME        nvarchar(50);
	declare @RELATIONSHIP_ORDER int;
	if exists(select * from DETAILVIEWS_RELATIONSHIPS where ID = @ID and DELETED = 0) begin -- then
		-- BEGIN Oracle Exception
			select @DETAIL_NAME        = DETAIL_NAME
			     , @RELATIONSHIP_ORDER = RELATIONSHIP_ORDER
			  from DETAILVIEWS_RELATIONSHIPS
			 where ID          = @ID
			   and DELETED     = 0;
		-- END Oracle Exception

		-- BEGIN Oracle Exception
			select @SWAP_ID           = ID
			  from DETAILVIEWS_RELATIONSHIPS
			 where DETAIL_NAME        = @DETAIL_NAME
			   and RELATIONSHIP_ORDER = 0
			   and DELETED            = 0;
		-- END Oracle Exception
		-- 01/04/2005 Paul.  If there is a module at 0, shift all DETAILVIEWS_RELATIONSHIPS so that this one can be 1. 
		if dbo.fnIsEmptyGuid(@SWAP_ID) = 0 begin -- then
			-- 04/02/2006 Paul.  Catch the Oracle NO_DATA_FOUND exception. 
			-- BEGIN Oracle Exception
				update DETAILVIEWS_RELATIONSHIPS
				   set RELATIONSHIP_ORDER = RELATIONSHIP_ORDER + 1
				 where DETAIL_NAME        = @DETAIL_NAME
				   and RELATIONSHIP_ORDER >= 0
				   and DELETED = 0;
			-- END Oracle Exception
		end -- if;
		
		-- 01/04/2006 Paul.  DETAILVIEWS_RELATIONSHIPS made visible start at tab 0. 
		-- BEGIN Oracle Exception
			update DETAILVIEWS_RELATIONSHIPS
			   set MODIFIED_USER_ID     = @MODIFIED_USER_ID 
			     , DATE_MODIFIED        =  getdate()        
			     , DATE_MODIFIED_UTC    =  getutcdate()     
			     , RELATIONSHIP_ORDER   = 0
			     , RELATIONSHIP_ENABLED = 1
			 where ID                   = @ID
			   and DELETED              = 0;
		-- END Oracle Exception
	end -- if;
  end
GO

Grant Execute on dbo.spDETAILVIEWS_RELATIONSHIPS_Enable to public;
GO

