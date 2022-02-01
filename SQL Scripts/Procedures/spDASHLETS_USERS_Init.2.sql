if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spDASHLETS_USERS_Init' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spDASHLETS_USERS_Init;
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
Create Procedure dbo.spDASHLETS_USERS_Init
	( @MODIFIED_USER_ID uniqueidentifier
	, @ASSIGNED_USER_ID uniqueidentifier
	, @DETAIL_NAME      nvarchar(50)
	)
as
  begin
	set nocount on

	-- 07/10/2009 Paul.  If there are no relationships, then copy the default relationships. 	
	-- 08/01/2009 Paul.  Make sure to ignore deleted records. 
	if not exists(select * from DASHLETS_USERS where ASSIGNED_USER_ID = @ASSIGNED_USER_ID and DETAIL_NAME = @DETAIL_NAME and DELETED = 0) begin -- then
		insert into DASHLETS_USERS
			( CREATED_BY          
			, DATE_ENTERED        
			, MODIFIED_USER_ID    
			, DATE_MODIFIED       
			, ASSIGNED_USER_ID    
			, DETAIL_NAME         
			, MODULE_NAME         
			, CONTROL_NAME        
			, DASHLET_ORDER       
			, DASHLET_ENABLED     
			, TITLE               
			)
		select	  MODIFIED_USER_ID    
			, DATE_MODIFIED       
			, @MODIFIED_USER_ID   
			, getdate()           
			, @ASSIGNED_USER_ID   
			, DETAIL_NAME         
			, MODULE_NAME         
			, CONTROL_NAME        
			, RELATIONSHIP_ORDER  
			, RELATIONSHIP_ENABLED
			, TITLE               
		  from DETAILVIEWS_RELATIONSHIPS
		 where DETAIL_NAME = @DETAIL_NAME
		   and DELETED     = 0;
	end -- if;

	exec dbo.spDASHLETS_USERS_Reorder @MODIFIED_USER_ID, @ASSIGNED_USER_ID, @DETAIL_NAME;
  end
GO

Grant Execute on dbo.spDASHLETS_USERS_Init to public;
GO

