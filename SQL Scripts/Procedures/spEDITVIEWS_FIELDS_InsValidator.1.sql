if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spEDITVIEWS_FIELDS_InsValidator' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spEDITVIEWS_FIELDS_InsValidator;
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
-- 02/21/2017 Paul.  Allow a field to be added to the end using an index of -1. 
-- 03/19/2020 Paul.  The FIELD_INDEX is not needed, so remove from update statement. 
Create Procedure dbo.spEDITVIEWS_FIELDS_InsValidator
	( @EDIT_NAME                   nvarchar(50)
	, @FIELD_INDEX                 int
	, @FIELD_VALIDATOR_NAME        nvarchar(50)
	, @DATA_FIELD                  nvarchar(100)
	, @FIELD_VALIDATOR_MESSAGE     nvarchar(150)
	)
as
  begin
	set nocount on
	
	declare @FIELD_VALIDATOR_ID uniqueidentifier;
	-- BEGIN Oracle Exception
		select @FIELD_VALIDATOR_ID = ID
		  from FIELD_VALIDATORS
		 where NAME    = @FIELD_VALIDATOR_NAME
		   and DELETED = 0;
	-- END Oracle Exception
	
	if dbo.fnIsEmptyGuid(@FIELD_VALIDATOR_ID) = 1 begin -- then
		raiserror(N'spEDITVIEWS_FIELDS_InsValidator: Could not find validator %s.', 16, 1, @FIELD_VALIDATOR_NAME);
	end else begin
		update EDITVIEWS_FIELDS
		   set DATE_MODIFIED               =  getdate()        
		     , DATE_MODIFIED_UTC           =  getutcdate()     
		     , FIELD_VALIDATOR_ID          = @FIELD_VALIDATOR_ID
		     , FIELD_VALIDATOR_MESSAGE     = @FIELD_VALIDATOR_MESSAGE
		 where EDIT_NAME                   = @EDIT_NAME
		   and DATA_FIELD                  = @DATA_FIELD
		   and DELETED                     = 0
		   and DEFAULT_VIEW                = 0
		   and FIELD_VALIDATOR_ID is null;
	end -- if;
  end
GO

Grant Execute on dbo.spEDITVIEWS_FIELDS_InsValidator to public;
GO

