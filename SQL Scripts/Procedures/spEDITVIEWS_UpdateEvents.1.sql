if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spEDITVIEWS_UpdateEvents' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spEDITVIEWS_UpdateEvents;
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
-- 11/11/2010 Paul.  Change to Pre Load and Post Load. 
-- 09/20/2012 Paul.  We need a SCRIPT field that is form specific. 
Create Procedure dbo.spEDITVIEWS_UpdateEvents
	( @MODIFIED_USER_ID    uniqueidentifier
	, @NAME                nvarchar(50)
	, @NEW_EVENT_ID        uniqueidentifier
	, @PRE_LOAD_EVENT_ID   uniqueidentifier
	, @POST_LOAD_EVENT_ID  uniqueidentifier
	, @VALIDATION_EVENT_ID uniqueidentifier
	, @PRE_SAVE_EVENT_ID   uniqueidentifier
	, @POST_SAVE_EVENT_ID  uniqueidentifier
	, @SCRIPT              nvarchar(max) = null
	)
as
  begin
	-- BEGIN Oracle Exception
		update EDITVIEWS
		   set MODIFIED_USER_ID    = @MODIFIED_USER_ID   
		     , DATE_MODIFIED       =  getdate()          
		     , DATE_MODIFIED_UTC   =  getutcdate()       
		     , NEW_EVENT_ID        = @NEW_EVENT_ID       
		     , PRE_LOAD_EVENT_ID   = @PRE_LOAD_EVENT_ID  
		     , POST_LOAD_EVENT_ID  = @POST_LOAD_EVENT_ID 
		     , VALIDATION_EVENT_ID = @VALIDATION_EVENT_ID
		     , PRE_SAVE_EVENT_ID   = @PRE_SAVE_EVENT_ID  
		     , POST_SAVE_EVENT_ID  = @POST_SAVE_EVENT_ID 
		     , SCRIPT              = @SCRIPT             
		 where NAME                = @NAME               
		   and DELETED             = 0                   ;
	-- END Oracle Exception
  end
GO
 
Grant Execute on dbo.spEDITVIEWS_UpdateEvents to public;
GO
 
