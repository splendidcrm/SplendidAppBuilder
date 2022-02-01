if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spDASHBOARDS_PANELS_InsertOnly' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spDASHBOARDS_PANELS_InsertOnly;
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
Create Procedure dbo.spDASHBOARDS_PANELS_InsertOnly
	( @DASHBOARD_ID       uniqueidentifier
	, @DASHBOARD_APP_NAME nvarchar(150)
	, @PANEL_ORDER        int
	, @ROW_INDEX          int
	, @COLUMN_WIDTH       int
	)
as
  begin
	set nocount on
	
	declare @ID                 uniqueidentifier;
	declare @MODIFIED_USER_ID   uniqueidentifier;
	declare @DASHBOARD_APP_ID   uniqueidentifier;

	select @DASHBOARD_APP_ID = ID
	  from DASHBOARD_APPS
	 where NAME    = @DASHBOARD_APP_NAME
	   and DELETED = 0;

	if not exists(select * from DASHBOARDS_PANELS where DASHBOARD_ID = @DASHBOARD_ID and DASHBOARD_APP_ID = @DASHBOARD_APP_ID) begin -- then
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
	end -- if;
  end
GO

Grant Execute on dbo.spDASHBOARDS_PANELS_InsertOnly to public;
GO

