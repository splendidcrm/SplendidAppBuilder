if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spDETAILVIEWS_FIELDS_UpdateUrl' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spDETAILVIEWS_FIELDS_UpdateUrl;
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
-- 08/18/2010 Paul.  Fix problem with updating URL fields. 
-- 10/30/2013 Paul.  Increase size of URL_TARGET. 
-- 02/25/2015 Paul.  Increase size of DATA_FIELD and DATA_FORMAT for OfficeAddin. 
Create Procedure dbo.spDETAILVIEWS_FIELDS_UpdateUrl
	( @MODIFIED_USER_ID            uniqueidentifier
	, @DETAIL_NAME                 nvarchar(50)
	, @DATA_FIELD                  nvarchar(1000)
	, @URL_FIELD                   nvarchar(max)
	, @URL_FORMAT                  nvarchar(max)
	, @URL_TARGET                  nvarchar( 60)
	)
as
  begin
	update DETAILVIEWS_FIELDS
	   set MODIFIED_USER_ID  = @MODIFIED_USER_ID
	     , DATE_MODIFIED     =  getdate()   
	     , DATE_MODIFIED_UTC =  getutcdate()
	     , URL_FIELD         = @URL_FIELD   
	     , URL_FORMAT        = @URL_FORMAT  
	     , URL_TARGET        = @URL_TARGET  
	 where DETAIL_NAME       = @DETAIL_NAME 
	   and DATA_FIELD        = @DATA_FIELD  
	   and DELETED           = 0            
	   and DEFAULT_VIEW      = 0            ;
  end
GO
 
Grant Execute on dbo.spDETAILVIEWS_FIELDS_UpdateUrl to public;
GO
 
