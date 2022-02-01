if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spTERMINOLOGY_Update' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spTERMINOLOGY_Update;
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
-- 08/27/2008 Paul.  PostgreSQL does not allow modifying input parameters.  Use a local temp variable. 
-- 02/02/2009 Paul.  Need to treat empty strings as NULL to be consistent.
-- 03/06/2012 Paul.  Increase size of the NAME field so that it can include a date formula. 
Create Procedure dbo.spTERMINOLOGY_Update
	( @NAME              nvarchar(150)
	, @LANG              nvarchar(10)
	, @MODULE_NAME       nvarchar(25)
	, @LIST_NAME         nvarchar(50)
	, @LIST_ORDER        int
	, @DISPLAY_NAME      nvarchar(max)
	)
as
  begin
	set nocount on
	
	declare @ID              uniqueidentifier;
	declare @TEMP_LANG       nvarchar(10);
	declare @TEMP_LIST_ORDER int;
	declare @TEMP_NAME       nvarchar(150);
	set @TEMP_LANG       = @LANG;
	set @TEMP_LIST_ORDER = @LIST_ORDER;
	-- 11/21/2005 Paul.  LIST_ORDER is not used if LIST_NAME is null. 
	if @LIST_NAME is null begin -- then
		set @TEMP_LIST_ORDER = null;
	end -- if;
	-- 01/12/2006 Paul.  0 is not valid for list order. 
	if @TEMP_LIST_ORDER = 0 begin -- then
		set @TEMP_LIST_ORDER = null;
	end -- if;
	-- 08/22/2007 Paul.  SugarCRM still has not fixed their German language pack. Convert ge-GE to de-DE.
	if @TEMP_LANG = lower(N'ge-GE') begin -- then
		set @TEMP_LANG = N'de-DE';
	end -- if;
	-- 02/02/2009 Paul.  Need to treat empty strings as NULL to be consistent.
	set @TEMP_NAME = @NAME;
	if @TEMP_NAME = N'' begin -- then
		set @TEMP_NAME = null;
	end -- if;
	if dbo.fnTERMINOLOGY_Exists(@TEMP_NAME, @TEMP_LANG, @MODULE_NAME, @LIST_NAME, @TEMP_LIST_ORDER) = 0 begin -- then
		set @ID = newid();
		insert into TERMINOLOGY
			( ID               
			, DATE_ENTERED     
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
			,  getdate()        
			,  getdate()        
			, @TEMP_NAME        
			, @TEMP_LANG        
			, @MODULE_NAME      
			, @LIST_NAME        
			, @TEMP_LIST_ORDER       
			, @DISPLAY_NAME     
			);
	end else begin
		-- 10/09/2005 Paul.  Only update the term if it has changed. 
		-- 01/16/2006 Paul.  Function returns 1 when changed. 
		if dbo.fnTERMINOLOGY_Changed(@TEMP_NAME, @TEMP_LANG, @MODULE_NAME, @LIST_NAME, @TEMP_LIST_ORDER, @DISPLAY_NAME) = 1 begin -- then
			update TERMINOLOGY
			   set DATE_MODIFIED     =  getdate()        
			     , DATE_MODIFIED_UTC =  getutcdate()     
			     , DISPLAY_NAME      = @DISPLAY_NAME     
			 where DELETED = 0
			   and (NAME        = @TEMP_NAME   or (NAME        is null and @TEMP_NAME   is null))
			   and (LANG        = @TEMP_LANG   or (LANG        is null and @TEMP_LANG   is null))
			   and (MODULE_NAME = @MODULE_NAME or (MODULE_NAME is null and @MODULE_NAME is null))
			   and (LIST_NAME   = @LIST_NAME   or (LIST_NAME   is null and @LIST_NAME   is null))
			   and isnull(LIST_ORDER, 0) = isnull(@TEMP_LIST_ORDER, 0);  -- 01/16/2006 Paul.  @TEMP_LIST_ORDER may be zero. 
		end -- if;
	end -- if;
  end
GO

Grant Execute on dbo.spTERMINOLOGY_Update to public;
GO

