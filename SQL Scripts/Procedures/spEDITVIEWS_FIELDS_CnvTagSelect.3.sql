if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spEDITVIEWS_FIELDS_CnvTagSelect' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spEDITVIEWS_FIELDS_CnvTagSelect;
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
Create Procedure dbo.spEDITVIEWS_FIELDS_CnvTagSelect
	( @EDIT_NAME         nvarchar( 50)
	, @FIELD_INDEX       int
	, @FORMAT_TAB_INDEX  int
	, @COLSPAN           int
	)
as
  begin
	declare @ID                uniqueidentifier;
	declare @DATA_LABEL        nvarchar(150);
	declare @DATA_FIELD        nvarchar(100);
	declare @DATA_REQUIRED     bit;
	declare @DISPLAY_FIELD     nvarchar(100);
	declare @MODULE_TYPE       nvarchar(25);
	
	set @DATA_LABEL  = N'.LBL_TAG_SET_NAME';
	set @DATA_FIELD  = N'TAG_SET_NAME';
	set @MODULE_TYPE = N'Tags';

	-- 05/12/2016 Paul.  First make sure that the data field does not already exist. 
	-- BEGIN Oracle Exception
		select @ID = ID
		  from EDITVIEWS_FIELDS
		 where EDIT_NAME    = @EDIT_NAME
		   and DATA_FIELD   = @DATA_FIELD
		   and DELETED      = 0            
		   and DEFAULT_VIEW = 0            ;
	-- END Oracle Exception
	if dbo.fnIsEmptyGuid(@ID) = 1 begin -- then
		-- 05/12/2016 Paul.  Search for a Blank record at the specified index position. 
		-- BEGIN Oracle Exception
			select @ID = ID
			  from EDITVIEWS_FIELDS
			 where EDIT_NAME    = @EDIT_NAME
			   and FIELD_INDEX  = @FIELD_INDEX
			   and FIELD_TYPE   = N'Blank'     
			   and DELETED      = 0            
			   and DEFAULT_VIEW = 0            ;
		-- END Oracle Exception
		-- 05/12/2016 Paul.  If blank was not found at the expected position, try and locate the first blank. 
		if dbo.fnIsEmptyGuid(@ID) = 1 begin -- then
			-- BEGIN Oracle Exception
				select @ID = ID
				  from EDITVIEWS_FIELDS
				 where EDIT_NAME    = @EDIT_NAME
				   and FIELD_INDEX  = (select min(FIELD_INDEX)
				                         from EDITVIEWS_FIELDS
				                        where EDIT_NAME    = @EDIT_NAME
				                          and FIELD_TYPE   = N'Blank'
				                          and DELETED      = 0
				                          and DEFAULT_VIEW = 0)
				   and FIELD_TYPE   = N'Blank'     
				   and DELETED      = 0            
				   and DEFAULT_VIEW = 0            ;
			-- END Oracle Exception
		end -- if;
		if dbo.fnIsEmptyGuid(@ID) = 0 begin -- then
			update EDITVIEWS_FIELDS
			   set MODIFIED_USER_ID  =  null             
			     , DATE_MODIFIED     =  getdate()        
			     , DATE_MODIFIED_UTC =  getutcdate()     
			     , FORMAT_MAX_LENGTH = null
			     , FIELD_TYPE        = N'TagSelect'        
			     , DATA_LABEL        = @DATA_LABEL       
			     , DATA_FIELD        = @DATA_FIELD       
			     , DATA_REQUIRED     = @DATA_REQUIRED    
			     , UI_REQUIRED       = @DATA_REQUIRED    
			     , FORMAT_SIZE       = null      
			     , FORMAT_TAB_INDEX  = @FORMAT_TAB_INDEX 
			     , COLSPAN           = @COLSPAN          
			 where ID = @ID;
		end else begin
			-- 05/12/2016 Paul.  If a blank cannot be found at the expected location, just insert a new record. 
			-- 11/25/2006 Paul.  In order to force the insert, make sure to specify a unique FIELD_INDEX. 
			select @FIELD_INDEX = max(FIELD_INDEX) + 1
			  from EDITVIEWS_FIELDS
			 where EDIT_NAME    = @EDIT_NAME
			   and DELETED      = 0            
			   and DEFAULT_VIEW = 0            ;
			exec dbo.spEDITVIEWS_FIELDS_InsTagSelect @EDIT_NAME, @FIELD_INDEX, @FORMAT_TAB_INDEX, @COLSPAN;
		end -- if;
	end -- if;
  end
GO
 
Grant Execute on dbo.spEDITVIEWS_FIELDS_CnvTagSelect to public;
GO
 
