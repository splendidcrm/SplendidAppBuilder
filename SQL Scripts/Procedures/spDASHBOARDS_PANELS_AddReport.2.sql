if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spDASHBOARDS_PANELS_AddReport' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spDASHBOARDS_PANELS_AddReport;
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
Create Procedure dbo.spDASHBOARDS_PANELS_AddReport
	( @MODIFIED_USER_ID   uniqueidentifier
	, @ASSIGNED_USER_ID   uniqueidentifier
	, @TEAM_ID            uniqueidentifier
	, @DASHBOARD_ID       uniqueidentifier
	, @DASHBOARD_CATEGORY nvarchar(50)
	, @REPORT_ID          uniqueidentifier
	)
as
  begin
	set nocount on
	
	declare @ID                  uniqueidentifier;
	declare @TEAM_SET_ID         uniqueidentifier;
	declare @TEAM_SET_LIST       varchar(8000);
	declare @DASHBOARD_APP_ID    uniqueidentifier;
	declare @NAME                nvarchar(150);
	declare @CATEGORY            nvarchar( 25);
	declare @MODULE_NAME         nvarchar( 50);
	declare @TITLE               nvarchar(100);
	declare @SETTINGS_EDITVIEW   nvarchar( 50);
	declare @SCRIPT_URL          nvarchar(2083);
	declare @DEFAULT_SETTINGS    nvarchar(max);
	declare @PANEL_ORDER         int;
	declare @ROW_INDEX           int;
	declare @COLUMN_WIDTH        int;

	set @NAME              = N'Report: ' + cast(@REPORT_ID as char(36));
	set @CATEGORY          = N'Report';
	set @MODULE_NAME       = N'Reports';
	set @TITLE             = null;
	set @SETTINGS_EDITVIEW = null;
	set @SCRIPT_URL        = N'~/html5/Dashlets/ReportViewerFrame.js';
	set @DEFAULT_SETTINGS  = N'REPORT_ID=' + cast(@REPORT_ID as char(36));

	select @TITLE = substring(NAME, 1, 100)
	  from REPORTS
	 where ID     = @REPORT_ID;

	set @DASHBOARD_APP_ID = newid();
	insert into DASHBOARD_APPS
		( ID                 
		, CREATED_BY         
		, DATE_ENTERED       
		, MODIFIED_USER_ID   
		, DATE_MODIFIED      
		, NAME               
		, CATEGORY           
		, MODULE_NAME        
		, TITLE              
		, SETTINGS_EDITVIEW  
		, SCRIPT_URL         
		, IS_ADMIN           
		, APP_ENABLED        
		, DEFAULT_SETTINGS   
		)
	values 
		( @DASHBOARD_APP_ID   
		,  null               
		,  getdate()          
		,  null               
		,  getdate()          
		, @NAME               
		, @CATEGORY           
		, @MODULE_NAME        
		, @TITLE              
		, @SETTINGS_EDITVIEW  
		, @SCRIPT_URL         
		, 0                   
		, 1                   
		, @DEFAULT_SETTINGS   
		);

	if not exists(select * from DASHBOARDS where ID = @DASHBOARD_ID and DELETED = 0) begin -- then
		exec dbo.spTEAM_SETS_NormalizeSet @TEAM_SET_ID out, @MODIFIED_USER_ID, @TEAM_ID, @TEAM_SET_LIST;
		set @DASHBOARD_ID = newid();
		insert into DASHBOARDS
			( ID               
			, CREATED_BY       
			, DATE_ENTERED     
			, MODIFIED_USER_ID 
			, DATE_MODIFIED    
			, ASSIGNED_USER_ID 
			, TEAM_ID          
			, TEAM_SET_ID      
			, NAME             
			, CATEGORY         
			, DESCRIPTION      
			, CONTENT          
			)
		values 	( @DASHBOARD_ID     
			, @MODIFIED_USER_ID 
			,  getdate()        
			, @MODIFIED_USER_ID 
			,  getdate()        
			, @ASSIGNED_USER_ID 
			, @TEAM_ID          
			, @TEAM_SET_ID      
			, @TITLE            
			, @DASHBOARD_CATEGORY
			, null              
			, null              
			);
	end -- if;

	set @COLUMN_WIDTH = 12;
	select @PANEL_ORDER = isnull(max(PANEL_ORDER), 0) + 1
	     , @ROW_INDEX   = isnull(max(ROW_INDEX  ), 0) + 1
	  from DASHBOARDS_PANELS
	 where DASHBOARD_ID = @DASHBOARD_ID
	   and DELETED      = 0;

	set @ID = newid();
	insert into DASHBOARDS_PANELS
		( ID                
		, CREATED_BY        
		, DATE_ENTERED      
		, MODIFIED_USER_ID  
		, DATE_MODIFIED     
		, DATE_MODIFIED_UTC 
		, DASHBOARD_ID      
		, DASHBOARD_APP_ID  
		, PANEL_ORDER       
		, ROW_INDEX         
		, COLUMN_WIDTH      
		)
	values 	( @ID                
		, @MODIFIED_USER_ID  
		,  getdate()         
		, @MODIFIED_USER_ID  
		,  getdate()         
		,  getutcdate()      
		, @DASHBOARD_ID      
		, @DASHBOARD_APP_ID  
		, @PANEL_ORDER       
		, @ROW_INDEX         
		, @COLUMN_WIDTH      
		);
  end
GO

Grant Execute on dbo.spDASHBOARDS_PANELS_AddReport to public;
GO

