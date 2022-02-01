if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spEDITVIEWS_FIELDS_UpdateOnClick' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spEDITVIEWS_FIELDS_UpdateOnClick;
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
-- 09/16/2012 Paul.  Increase ONCLICK_SCRIPT to nvarchar(max). 
Create Procedure dbo.spEDITVIEWS_FIELDS_UpdateOnClick
	( @MODIFIED_USER_ID            uniqueidentifier
	, @EDIT_NAME                   nvarchar(50)
	, @DATA_FIELD                  nvarchar(100)
	, @ONCLICK_SCRIPT              nvarchar(max)
	)
as
  begin
	update EDITVIEWS_FIELDS
	   set MODIFIED_USER_ID  = @MODIFIED_USER_ID
	     , DATE_MODIFIED     =  getdate()
	     , DATE_MODIFIED_UTC =  getutcdate()
	     , ONCLICK_SCRIPT    = @ONCLICK_SCRIPT
	 where EDIT_NAME         = @EDIT_NAME
	   and DATA_FIELD        = @DATA_FIELD
	   and DELETED           = 0            ;
  end
GO
 
Grant Execute on dbo.spEDITVIEWS_FIELDS_UpdateOnClick to public;
GO
 
