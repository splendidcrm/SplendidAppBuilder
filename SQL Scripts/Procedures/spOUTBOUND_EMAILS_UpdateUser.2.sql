if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spOUTBOUND_EMAILS_UpdateUser' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spOUTBOUND_EMAILS_UpdateUser;
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
-- 07/16/2013 Paul.  spOUTBOUND_EMAILS_Update now returns the ID. 
-- 04/20/2016 Paul.  Provide a way to allow each user to have their own SMTP server. 
-- 02/01/2017 Paul.  Add support for Exchange using Username/Password. 
Create Procedure dbo.spOUTBOUND_EMAILS_UpdateUser
	( @MODIFIED_USER_ID   uniqueidentifier
	, @USER_ID            uniqueidentifier
	, @MAIL_SMTPUSER      nvarchar(100)
	, @MAIL_SMTPPASS      nvarchar(100)
	, @MAIL_SMTPSERVER    nvarchar(100) = null
	, @MAIL_SMTPPORT      int = null
	, @MAIL_SMTPAUTH_REQ  bit = null
	, @MAIL_SMTPSSL       int = null
	, @MAIL_SENDTYPE      nvarchar(25) = null
	)
as
  begin
	set nocount on
	
	-- 07/11/2010 Paul.  Make sure to call the base Update procedure. 
	declare @ID uniqueidentifier;
	-- BEGIN Oracle Exception
		select @ID = ID
		  from OUTBOUND_EMAILS
		 where USER_ID = @USER_ID 
		   and TYPE    = N'system-override'
		   and DELETED = 0;
	-- END Oracle Exception
	exec dbo.spOUTBOUND_EMAILS_Update @ID out, @MODIFIED_USER_ID, N'system', N'system-override', @USER_ID, @MAIL_SENDTYPE, null, @MAIL_SMTPSERVER, @MAIL_SMTPPORT, @MAIL_SMTPUSER, @MAIL_SMTPPASS, @MAIL_SMTPAUTH_REQ, @MAIL_SMTPSSL, null, null, null, null;
  end
GO

Grant Execute on dbo.spOUTBOUND_EMAILS_UpdateUser to public;
GO

