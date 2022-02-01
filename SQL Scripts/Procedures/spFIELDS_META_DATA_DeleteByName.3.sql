if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spFIELDS_META_DATA_DeleteByName' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spFIELDS_META_DATA_DeleteByName;
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
-- 03/29/2011 Paul.  Ease migration to Oracle. 
-- 04/18/2016 Paul.  Allow disable recompile so that we can do in the background. 
Create Procedure dbo.spFIELDS_META_DATA_DeleteByName
	( @MODIFIED_USER_ID  uniqueidentifier
	, @CUSTOM_MODULE     nvarchar(255)
	, @NAME              nvarchar(255)
	, @DISABLE_RECOMPILE bit = null
	)
as
  begin
	set nocount on
	
	declare @ID uniqueidentifier;
	-- BEGIN Oracle Exception
		select @ID = ID
		  from FIELDS_META_DATA
		 where @CUSTOM_MODULE = CUSTOM_MODULE
		   and @NAME          = NAME
		   and DELETED        = 0;
	-- END Oracle Exception

	if dbo.fnIsEmptyGuid(@ID) = 0 begin -- then
		exec dbo.spFIELDS_META_DATA_Delete @ID, @MODIFIED_USER_ID, @DISABLE_RECOMPILE;
	end -- if;
  end
GO

Grant Execute on dbo.spFIELDS_META_DATA_DeleteByName to public;
GO

