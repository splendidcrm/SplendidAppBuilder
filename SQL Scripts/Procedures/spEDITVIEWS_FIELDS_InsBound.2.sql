if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spEDITVIEWS_FIELDS_InsBound' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spEDITVIEWS_FIELDS_InsBound;
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
-- 07/24/2006 Paul.  Increase the DATA_LABEL to 150 to allow a fully-qualified (NAME+MODULE_NAME+LIST_NAME) TERMINOLOGY name. 
-- 11/24/2006 Paul.  FIELD_TYPE should be of national character type. 
-- 02/24/2010 Paul.  Allow a field to be added to the end using an index of -1. 
-- 11/23/2012 Paul.  Make sure to only add the field if it does not already exist when using -1. 
Create Procedure dbo.spEDITVIEWS_FIELDS_InsBound
	( @EDIT_NAME         nvarchar( 50)
	, @FIELD_INDEX       int
	, @DATA_LABEL        nvarchar(150)
	, @DATA_FIELD        nvarchar(100)
	, @DATA_REQUIRED     bit
	, @FORMAT_TAB_INDEX  int
	, @FORMAT_MAX_LENGTH int
	, @FORMAT_SIZE       int
	, @COLSPAN           int
	)
as
  begin
	declare @ID uniqueidentifier;
	
	declare @TEMP_FIELD_INDEX int;	
	set @TEMP_FIELD_INDEX = @FIELD_INDEX;
	if @FIELD_INDEX is null or @FIELD_INDEX = -1 begin -- then
		-- BEGIN Oracle Exception
			select @TEMP_FIELD_INDEX = isnull(max(FIELD_INDEX), 0) + 1
			  from EDITVIEWS_FIELDS
			 where EDIT_NAME    = @EDIT_NAME
			   and DELETED      = 0            
			   and DEFAULT_VIEW = 0            ;
		-- END Oracle Exception
		-- BEGIN Oracle Exception
			select @ID = ID
			  from EDITVIEWS_FIELDS
			 where EDIT_NAME    = @EDIT_NAME
			   and DATA_FIELD   = @DATA_FIELD
			   and DELETED      = 0            
			   and DEFAULT_VIEW = 0            ;
		-- END Oracle Exception
	end else begin
		-- BEGIN Oracle Exception
			select @ID = ID
			  from EDITVIEWS_FIELDS
			 where EDIT_NAME    = @EDIT_NAME
			   and FIELD_INDEX  = @FIELD_INDEX
			   and DELETED      = 0            
			   and DEFAULT_VIEW = 0            ;
		-- END Oracle Exception
	end -- if;
	if dbo.fnIsEmptyGuid(@ID) = 1 begin -- then
		set @ID = newid();
		insert into EDITVIEWS_FIELDS
			( ID               
			, CREATED_BY       
			, DATE_ENTERED     
			, MODIFIED_USER_ID 
			, DATE_MODIFIED    
			, EDIT_NAME        
			, FIELD_INDEX      
			, FIELD_TYPE       
			, DATA_LABEL       
			, DATA_FIELD       
			, DATA_REQUIRED    
			, UI_REQUIRED      
			, FORMAT_TAB_INDEX 
			, FORMAT_MAX_LENGTH
			, FORMAT_SIZE      
			, COLSPAN          
			)
		values 
			( @ID               
			, null              
			,  getdate()        
			, null              
			,  getdate()        
			, @EDIT_NAME        
			, @TEMP_FIELD_INDEX 
			, N'TextBox'        
			, @DATA_LABEL       
			, @DATA_FIELD       
			, @DATA_REQUIRED    
			, @DATA_REQUIRED    
			, @FORMAT_TAB_INDEX 
			, @FORMAT_MAX_LENGTH
			, @FORMAT_SIZE      
			, @COLSPAN          
			);
	end -- if;
  end
GO
 
Grant Execute on dbo.spEDITVIEWS_FIELDS_InsBound to public;
GO
 
