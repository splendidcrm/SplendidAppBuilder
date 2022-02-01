if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spEDITVIEWS_FIELDS_LstChange' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spEDITVIEWS_FIELDS_LstChange;
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
-- 11/25/2006 Paul.  Create a procedure to convert a BoundList to a ChangeButton. 
-- This is because SugarCRM changed the Assigned To listbox to a Change field. 
-- 09/16/2012 Paul.  Increase ONCLICK_SCRIPT to nvarchar(max). 
Create Procedure dbo.spEDITVIEWS_FIELDS_LstChange
	( @EDIT_NAME         nvarchar( 50)
	, @FIELD_INDEX       int
	, @DATA_LABEL        nvarchar(150)
	, @DATA_FIELD        nvarchar(100)
	, @DATA_REQUIRED     bit
	, @FORMAT_TAB_INDEX  int
	, @DISPLAY_FIELD     nvarchar(100)
	, @ONCLICK_SCRIPT    nvarchar(max)
	, @COLSPAN           int
	)
as
  begin
	declare @ID uniqueidentifier;
	
	-- 11/25/2006 Paul.  First make sure that the data field exists. 
	-- BEGIN Oracle Exception
		select @ID = ID
		  from EDITVIEWS_FIELDS
		 where EDIT_NAME    = @EDIT_NAME
		   and DATA_FIELD   = @DATA_FIELD
		   and FIELD_TYPE   = N'ListBox'
		   and DELETED      = 0            
		   and DEFAULT_VIEW = 0            ;
	-- END Oracle Exception
	if dbo.fnIsEmptyGuid(@ID) = 0 begin -- then
		update EDITVIEWS_FIELDS
		   set MODIFIED_USER_ID =  null             
		     , DATE_MODIFIED    =  getdate()        
		     , DATE_MODIFIED_UTC=  getutcdate()     
		     , FIELD_TYPE       = N'ChangeButton'   
		     , DATA_LABEL       = @DATA_LABEL       
		     , CACHE_NAME       = null
		     , DISPLAY_FIELD    = @DISPLAY_FIELD    
		     , DATA_REQUIRED    = @DATA_REQUIRED    
		     , UI_REQUIRED      = @DATA_REQUIRED    
		     , ONCLICK_SCRIPT   = @ONCLICK_SCRIPT   
		     , FORMAT_TAB_INDEX = @FORMAT_TAB_INDEX 
		     , COLSPAN          = @COLSPAN          
		 where ID = @ID;
	end -- if;
	
	-- 11/25/2006 Paul.  Also change the default view. 
	-- BEGIN Oracle Exception
		select @ID = ID
		  from EDITVIEWS_FIELDS
		 where EDIT_NAME    = @EDIT_NAME
		   and DATA_FIELD   = @DATA_FIELD
		   and FIELD_TYPE   = N'ListBox'
		   and DELETED      = 0            
		   and DEFAULT_VIEW = 1            ;
	-- END Oracle Exception
	if dbo.fnIsEmptyGuid(@ID) = 0 begin -- then
		update EDITVIEWS_FIELDS
		   set MODIFIED_USER_ID =  null             
		     , DATE_MODIFIED    =  getdate()        
		     , DATE_MODIFIED_UTC=  getutcdate()     
		     , FIELD_TYPE       = N'ChangeButton'   
		     , DATA_LABEL       = @DATA_LABEL       
		     , CACHE_NAME       = null
		     , DISPLAY_FIELD    = @DISPLAY_FIELD    
		     , DATA_REQUIRED    = @DATA_REQUIRED    
		     , UI_REQUIRED      = @DATA_REQUIRED    
		     , ONCLICK_SCRIPT   = @ONCLICK_SCRIPT   
		     , FORMAT_TAB_INDEX = @FORMAT_TAB_INDEX 
		     , COLSPAN          = @COLSPAN          
		 where ID = @ID;
	end -- if;
  end
GO
 
Grant Execute on dbo.spEDITVIEWS_FIELDS_LstChange to public;
GO
 
