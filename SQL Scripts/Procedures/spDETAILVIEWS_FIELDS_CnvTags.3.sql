if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spDETAILVIEWS_FIELDS_CnvTags' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spDETAILVIEWS_FIELDS_CnvTags;
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
Create Procedure dbo.spDETAILVIEWS_FIELDS_CnvTags
	( @DETAIL_NAME       nvarchar( 50)
	, @FIELD_INDEX       int
	, @COLSPAN           int
	)
as
  begin
	declare @TEMP_FIELD_INDEX int;
	declare @ID               uniqueidentifier;
	declare @DATA_LABEL       nvarchar(150);
	declare @DATA_FIELD       nvarchar(1000);
	declare @DATA_FORMAT      nvarchar(max);
	
	set @DATA_LABEL       = N'.LBL_TAG_SET_NAME';
	set @DATA_FIELD       = N'TAG_SET_NAME';
	set @DATA_FORMAT      = N'{0}';
	set @TEMP_FIELD_INDEX = @FIELD_INDEX;
	-- 11/24/2006 Paul.  First make sure that the data field does not already exist. 
	-- BEGIN Oracle Exception
		select @ID = ID
		  from DETAILVIEWS_FIELDS
		 where DETAIL_NAME  = @DETAIL_NAME
		   and DATA_FIELD   = @DATA_FIELD
		   and DELETED      = 0            
		   and DEFAULT_VIEW = 0            ;
	-- END Oracle Exception
	if dbo.fnIsEmptyGuid(@ID) = 1 begin -- then
		-- 11/24/2006 Paul.  Search for a Blank record at the specified index position. 
		-- BEGIN Oracle Exception
			select @ID = ID
			  from DETAILVIEWS_FIELDS
			 where DETAIL_NAME  = @DETAIL_NAME
			   and FIELD_INDEX  = @TEMP_FIELD_INDEX
			   and FIELD_TYPE   = N'Blank'     
			   and DELETED      = 0            
			   and DEFAULT_VIEW = 0            ;
		-- END Oracle Exception
		-- 11/24/2006 Paul.  If blank was not found at the expected position, try and locate the first blank. 
		if dbo.fnIsEmptyGuid(@ID) = 1 begin -- then
			-- BEGIN Oracle Exception
				select @ID = ID
				  from DETAILVIEWS_FIELDS
				 where DETAIL_NAME  = @DETAIL_NAME
				   and FIELD_INDEX  = (select min(FIELD_INDEX)
				                         from DETAILVIEWS_FIELDS
				                        where DETAIL_NAME  = @DETAIL_NAME
				                          and FIELD_TYPE   = N'Blank'
				                          and DELETED      = 0
				                          and DEFAULT_VIEW = 0)
				   and FIELD_TYPE   = N'Blank'     
				   and DELETED      = 0            
				   and DEFAULT_VIEW = 0            ;
			-- END Oracle Exception
		end -- if;

		if dbo.fnIsEmptyGuid(@ID) = 0 begin -- then
			update DETAILVIEWS_FIELDS
			   set MODIFIED_USER_ID = null              
			     , DATE_MODIFIED    = getdate()
			     , DATE_MODIFIED_UTC= getutcdate()
			     , FIELD_TYPE       = N'Tags'           
			     , DATA_LABEL       = @DATA_LABEL       
			     , DATA_FIELD       = @DATA_FIELD       
			     , DATA_FORMAT      = @DATA_FORMAT      
			     , COLSPAN          = @COLSPAN          
			 where ID = @ID;
		end else begin
			select @TEMP_FIELD_INDEX = max(FIELD_INDEX) + 1
			  from DETAILVIEWS_FIELDS
			 where DETAIL_NAME  = @DETAIL_NAME
			   and DELETED      = 0            
			   and DEFAULT_VIEW = 0            ;
			exec dbo.spDETAILVIEWS_FIELDS_InsTags @DETAIL_NAME, @TEMP_FIELD_INDEX, @COLSPAN;
		end -- if;
	end -- if;
  end
GO

Grant Execute on dbo.spDETAILVIEWS_FIELDS_CnvTags to public;
GO

