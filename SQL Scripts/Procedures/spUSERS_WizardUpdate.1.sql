if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spUSERS_WizardUpdate' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spUSERS_WizardUpdate;
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
-- 12/15/2012 Paul.  Move USER_PREFERENCES to separate fields for easier access on Surface RT. 
Create Procedure dbo.spUSERS_WizardUpdate
	( @ID               uniqueidentifier
	, @MODIFIED_USER_ID uniqueidentifier
	, @DATE_FORMAT      nvarchar(25)
	, @TIME_FORMAT      nvarchar(25)
	, @LANG             nvarchar(10)
	, @CURRENCY_ID      uniqueidentifier
	, @TIMEZONE_ID      uniqueidentifier
	)
as
  begin
	set nocount on

	update USERS
	   set DATE_FORMAT      = @DATE_FORMAT              
	     , TIME_FORMAT      = @TIME_FORMAT              
	     , LANG             = @LANG                     
	     , CURRENCY_ID      = @CURRENCY_ID              
	     , TIMEZONE_ID      = @TIMEZONE_ID              
	     , DATE_MODIFIED    = getdate()
	     , DATE_MODIFIED_UTC= getutcdate()
	     , MODIFIED_USER_ID = @MODIFIED_USER_ID
	 where ID               = @ID
	   and DELETED          = 0;
  end
GO

Grant Execute on dbo.spUSERS_WizardUpdate to public;
GO


