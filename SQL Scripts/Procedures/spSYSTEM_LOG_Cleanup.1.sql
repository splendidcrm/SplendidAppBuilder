if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spSYSTEM_LOG_Cleanup' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spSYSTEM_LOG_Cleanup;
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
Create Procedure dbo.spSYSTEM_LOG_Cleanup
as
  begin
	set nocount on
	
	-- 02/26/2010 Paul.  We want to be selective on the entries that we delete
	-- so that we don't delete useful bug tracking or auditing information. 
	-- 10/29/2013 Paul.  Cleanup timer events. Mostly Scheduler Job. 
	delete from SYSTEM_LOG
	 where DATE_ENTERED < dbo.fnDateAdd('month', -1, getdate())
	   and ERROR_TYPE = N'Warning'
	   and METHOD in ( N'Void OnTimer(System.Object)'
	                 , N'Void InitApp(System.Web.HttpContext)'
	                 , N'Void InitSchedulerManager()'
	                 , N'Void InitWorkflowManager()'
	                 , N'System.String Term(System.Web.HttpApplicationState, System.String, System.String)'
	                 , N'Void OnTimer(System.Object)'
	                 );

	-- 02/26/2010 Paul.  User logins are slightly more interesting, so keep for 2 months. 
	delete from SYSTEM_LOG
	 where DATE_ENTERED < dbo.fnDateAdd('month', -2, getdate())
	   and ERROR_TYPE = N'Warning'
	   and METHOD in ( N'Boolean LoginUser(System.String, System.String, System.String, System.String, System.String, Boolean)'  -- (~/_code/SplendidInit.cs) User login. 
	                 , N'System.Guid LoginUser(System.String ByRef, System.String, Boolean)'  -- (~/soap.asmx.cs) SOAP User login. 
	                 , N'System.Guid Login(System.String, System.String, System.String)'  -- (~/sync.asmx.cs) SyncUser login. 
	                 );

	-- 02/26/2010 Paul.  The unowned workflow error is not interesting as it typically occurs when the web site is restarted. 
	-- 10/29/2013 Paul.  We are seen a number of Exchange Notification errors. 
	delete from SYSTEM_LOG
	 where DATE_ENTERED < dbo.fnDateAdd('month', -1, getdate())
	   and ERROR_TYPE = N'Error'
	   and METHOD in ( N'Void ExceptionNotHandled(System.Object, System.Workflow.Runtime.ServicesExceptionNotHandledEventArgs)'  -- This workflow is not owned by the WorkflowRuntime. 
	                 , N'Void SaveWorkflowInstanceState(System.Workflow.ComponentModel.Activity, Boolean)'
	                 , N'ExchangeNotificationService.SendNotificationResultType SendNotification(ExchangeNotificationService.SendNotificationResponseType)'
	                 );

	-- 08/07/2010 Paul.  The USERS_LOGINS table is getting very big. Lets keep 3 months of data. 
	-- 06/02/2020 Paul.  Increase to 6 months. 
	delete from USERS_LOGINS
	 where LOGIN_DATE < dbo.fnDateAdd('month', -6, getdate());

	-- 01/25/2015 Paul.  The SYSTEM_LOG table is getting very big. Lets keep 3 months of data. 
	delete from SYSTEM_LOG
	 where DATE_ENTERED < dbo.fnDateAdd('month', -3, getdate());
  end
GO

Grant Execute on dbo.spSYSTEM_LOG_Cleanup to public;
GO

