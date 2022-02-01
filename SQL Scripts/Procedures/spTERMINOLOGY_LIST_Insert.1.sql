if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spTERMINOLOGY_LIST_Insert' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spTERMINOLOGY_LIST_Insert;
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
-- 07/24/2006 Paul.  Increase the MODULE_NAME to 25 to match the size in the MODULES table.
-- 05/26/2007 Paul.  Truncate NAME to 25 characters as all lists have this maximum.
-- 08/27/2008 Paul.  PostgreSQL does not allow modifying input parameters.  Use a local temp variable. 
-- 03/06/2012 Paul.  Increase size of the NAME field so that it can include a date formula. 
Create Procedure dbo.spTERMINOLOGY_LIST_Insert
	( @ID                uniqueidentifier output
	, @MODIFIED_USER_ID  uniqueidentifier
	, @NAME              nvarchar(150)
	, @LANG              nvarchar(10)
	, @MODULE_NAME       nvarchar(25)
	, @LIST_NAME         nvarchar(50)
	, @LIST_ORDER        int
	, @DISPLAY_NAME      nvarchar(max)
	)
as
  begin
	set nocount on
	
	declare @TEMP_LIST_ORDER int;
	set @TEMP_LIST_ORDER = @LIST_ORDER;
	-- First look for an existing key, if found, then overwrite. Duplicates are not allowed. 
	-- BEGIN Oracle Exception
		select @ID = ID
		  from TERMINOLOGY
		 where NAME        = @NAME
		   and LANG        = @LANG
		   and (MODULE_NAME = @MODULE_NAME or (MODULE_NAME is null and @MODULE_NAME is null))
		   and LIST_NAME   = @LIST_NAME
		   and DELETED     = 0;
	-- END Oracle Exception
	if dbo.fnIsEmptyGuid(@ID) = 0 begin -- then
		update TERMINOLOGY
		   set DISPLAY_NAME     = @DISPLAY_NAME
		     , DATE_MODIFIED    = getdate()
		     , DATE_MODIFIED_UTC= getutcdate()
		     , MODIFIED_USER_ID = @MODIFIED_USER_ID
		 where ID               = @ID;
	end else begin
		-- LIST_ORDER -1 means add to end. 
		if @TEMP_LIST_ORDER = -1 begin -- then
			-- BEGIN Oracle Exception
				select @TEMP_LIST_ORDER = max(LIST_ORDER) + 1
				  from TERMINOLOGY
				 where LANG        = @LANG
				   and (MODULE_NAME = @MODULE_NAME or (MODULE_NAME is null and @MODULE_NAME is null))
				   and LIST_NAME   = @LIST_NAME
				   and DELETED     = 0;
			-- END Oracle Exception
		end -- if;
			-- 04/02/2006 Paul.  Catch the Oracle NO_DATA_FOUND exception. 
		-- BEGIN Oracle Exception
			update TERMINOLOGY
			   set LIST_ORDER       = LIST_ORDER + 1
			     , DATE_MODIFIED    = getdate()
			     , DATE_MODIFIED_UTC= getutcdate()
			     , MODIFIED_USER_ID = @MODIFIED_USER_ID
			 where LANG             = @LANG
			   and (MODULE_NAME = @MODULE_NAME or (MODULE_NAME is null and @MODULE_NAME is null))
			   and LIST_NAME        = @LIST_NAME
			   and LIST_ORDER      >= @TEMP_LIST_ORDER
			   and DELETED          = 0;
		-- END Oracle Exception

		set @ID = newid();
		insert into TERMINOLOGY
			( ID               
			, CREATED_BY       
			, DATE_ENTERED     
			, MODIFIED_USER_ID 
			, DATE_MODIFIED    
			, NAME             
			, LANG             
			, MODULE_NAME      
			, LIST_NAME        
			, LIST_ORDER       
			, DISPLAY_NAME     
			)
		values
			( @ID               
			, @MODIFIED_USER_ID 
			,  getdate()        
			, @MODIFIED_USER_ID 
			,  getdate()        
			, @NAME             
			, @LANG             
			, @MODULE_NAME      
			, @LIST_NAME        
			, @TEMP_LIST_ORDER       
			, @DISPLAY_NAME     
			);
	end -- if;
  end
GO

Grant Execute on dbo.spTERMINOLOGY_LIST_Insert to public;
GO

