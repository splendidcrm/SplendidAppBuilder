if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spACL_ROLES_InsertOnly' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spACL_ROLES_InsertOnly;
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
Create Procedure dbo.spACL_ROLES_InsertOnly
	( @ID                uniqueidentifier
	, @NAME              nvarchar(150)
	, @DESCRIPTION       nvarchar(max)
	)
as
  begin
	set nocount on
	
	if not exists(select * from ACL_ROLES where ID = @ID) begin -- then
		if dbo.fnIsEmptyGuid(@ID) = 1 begin -- then
			set @ID = newid();
		end -- if;
		insert into ACL_ROLES
			( ID               
			, CREATED_BY       
			, DATE_ENTERED     
			, MODIFIED_USER_ID 
			, DATE_MODIFIED    
			, NAME             
			, DESCRIPTION      
			)
		values 	( @ID               
			, null               
			,  getdate()        
			, null               
			,  getdate()        
			, @NAME             
			, @DESCRIPTION      
			);
	end -- if;

	if not exists(select * from ACL_ROLES_CSTM where ID_C = @ID) begin -- then
		insert into ACL_ROLES_CSTM ( ID_C ) values ( @ID );
	end -- if;

  end
GO
 
Grant Execute on dbo.spACL_ROLES_InsertOnly to public;
GO
 
