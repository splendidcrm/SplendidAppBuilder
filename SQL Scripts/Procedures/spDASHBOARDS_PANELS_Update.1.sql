if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spDASHBOARDS_PANELS_Update' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spDASHBOARDS_PANELS_Update;
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
Create Procedure dbo.spDASHBOARDS_PANELS_Update
	( @ID                 uniqueidentifier output
	, @MODIFIED_USER_ID   uniqueidentifier
	, @DASHBOARD_ID       uniqueidentifier
	, @DASHBOARD_APP_ID   uniqueidentifier
	, @PANEL_ORDER        int
	, @ROW_INDEX          int
	, @COLUMN_WIDTH       int
	)
as
  begin
	set nocount on
	
	if not exists(select * from DASHBOARDS_PANELS where ID = @ID) begin -- then
		if dbo.fnIsEmptyGuid(@ID) = 1 begin -- then
			set @ID = newid();
		end -- if;
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
	end else begin
		update DASHBOARDS_PANELS
		   set MODIFIED_USER_ID   = @MODIFIED_USER_ID  
		     , DATE_MODIFIED      =  getdate()         
		     , DATE_MODIFIED_UTC  =  getutcdate()      
		     , DASHBOARD_ID       = @DASHBOARD_ID      
		     , DASHBOARD_APP_ID   = @DASHBOARD_APP_ID  
		     , PANEL_ORDER        = @PANEL_ORDER       
		     , ROW_INDEX          = @ROW_INDEX         
		     , COLUMN_WIDTH       = @COLUMN_WIDTH      
		 where ID                 = @ID                ;
	end -- if;
  end
GO

Grant Execute on dbo.spDASHBOARDS_PANELS_Update to public;
GO

