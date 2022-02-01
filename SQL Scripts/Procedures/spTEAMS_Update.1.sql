if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spTEAMS_Update' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spTEAMS_Update;
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
-- 10/14/2006 Paul.  The PRIVATE flag is not editable. 
-- 09/15/2009 Paul.  Convert data type to nvarchar(max) to support Azure. 
-- 01/06/2012 Paul.  If the team name changes, we need to update team set names. 
-- 04/12/2016 Paul.  Add parent team and custom fields. 
-- 04/28/2016 Paul.  Rename parent to PARENT_ID. 
-- 07/24/2019 Paul.  Prevent duplicates. 
Create Procedure dbo.spTEAMS_Update
	( @ID                uniqueidentifier output
	, @MODIFIED_USER_ID  uniqueidentifier
	, @NAME              nvarchar(128)
	, @DESCRIPTION       nvarchar(max)
	, @PARENT_ID         uniqueidentifier = null
	)
as
  begin
	set nocount on
	
	declare @OLD_NAME nvarchar(128);

	if dbo.fnTEAMS_IsValidName(@ID, @NAME) = 0 begin -- then
		raiserror(N'spTEAMS_Update: The name %s already exists.  Duplicate names are not allowed. ', 16, 1, @NAME);
	end else begin
		if not exists(select * from TEAMS where ID = @ID) begin -- then
			if dbo.fnIsEmptyGuid(@ID) = 1 begin -- then
				set @ID = newid();
			end -- if;
			insert into TEAMS
				( ID               
				, CREATED_BY       
				, DATE_ENTERED     
				, MODIFIED_USER_ID 
				, DATE_MODIFIED    
				, NAME             
				, DESCRIPTION      
				, PARENT_ID   
				)
			values 	( @ID               
				, @MODIFIED_USER_ID 
				,  getdate()        
				, @MODIFIED_USER_ID 
				,  getdate()        
				, @NAME             
				, @DESCRIPTION      
				, @PARENT_ID        
				);
		end else begin
			select @OLD_NAME = NAME
			  from TEAMS
			 where ID = @ID;
			
			update TEAMS
			   set MODIFIED_USER_ID  = @MODIFIED_USER_ID 
			     , DATE_MODIFIED     =  getdate()        
			     , DATE_MODIFIED_UTC =  getutcdate()     
			     , NAME              = @NAME             
			     , DESCRIPTION       = @DESCRIPTION      
			     , PARENT_ID         = @PARENT_ID        
			 where ID                = @ID               ;
			
			-- 01/06/2012 Paul.  If the team name changes, we need to update team set names. 
			if @OLD_NAME <> @NAME begin -- then
				exec dbo.spTEAM_SETS_UpdateNames @MODIFIED_USER_ID, @ID;
			end -- if;
		end -- if;
		-- 04/18/2016 Paul.  Make sure that the custom field table exists. 
		if @@ERROR = 0 begin -- then
			if not exists(select * from TEAMS_CSTM where ID_C = @ID) begin -- then
				insert into TEAMS_CSTM ( ID_C ) values ( @ID );
			end -- if;
		end -- if;
	end -- if;
  end
GO
 
Grant Execute on dbo.spTEAMS_Update to public;
GO
 
