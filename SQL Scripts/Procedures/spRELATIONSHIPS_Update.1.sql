if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spRELATIONSHIPS_Update' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spRELATIONSHIPS_Update;
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
-- 04/29/2006 Paul.  @RELATIONSHIP_ROLE_COLUMN_VALUE is too long for Oracle, so reduce globally. 
Create Procedure dbo.spRELATIONSHIPS_Update
	( @ID                              uniqueidentifier output
	, @MODIFIED_USER_ID                uniqueidentifier
	, @RELATIONSHIP_NAME               nvarchar(150)
	, @LHS_MODULE                      nvarchar(100)
	, @LHS_TABLE                       nvarchar(64)
	, @LHS_KEY                         nvarchar(64)
	, @RHS_MODULE                      nvarchar(100)
	, @RHS_TABLE                       nvarchar(64)
	, @RHS_KEY                         nvarchar(64)
	, @JOIN_TABLE                      nvarchar(64)
	, @JOIN_KEY_LHS                    nvarchar(64)
	, @JOIN_KEY_RHS                    nvarchar(64)
	, @RELATIONSHIP_TYPE               nvarchar(64)
	, @RELATIONSHIP_ROLE_COLUMN        nvarchar(64)
	, @RELATIONSHIP_ROLE_COL_VALUE     nvarchar(50)
	, @REVERSE                         bit
	)
as
  begin
	set nocount on
	
	if not exists(select * from RELATIONSHIPS where ID = @ID) begin -- then
		if dbo.fnIsEmptyGuid(@ID) = 1 begin -- then
			set @ID = newid();
		end -- if;
		insert into RELATIONSHIPS
			( ID                             
			, CREATED_BY                     
			, DATE_ENTERED                   
			, MODIFIED_USER_ID               
			, DATE_MODIFIED                  
			, RELATIONSHIP_NAME              
			, LHS_MODULE                     
			, LHS_TABLE                      
			, LHS_KEY                        
			, RHS_MODULE                     
			, RHS_TABLE                      
			, RHS_KEY                        
			, JOIN_TABLE                     
			, JOIN_KEY_LHS                   
			, JOIN_KEY_RHS                   
			, RELATIONSHIP_TYPE              
			, RELATIONSHIP_ROLE_COLUMN       
			, RELATIONSHIP_ROLE_COLUMN_VALUE 
			, REVERSE                        
			)
		values 	( @ID                             
			, @MODIFIED_USER_ID                     
			,  getdate()                      
			, @MODIFIED_USER_ID               
			,  getdate()                      
			, @RELATIONSHIP_NAME              
			, @LHS_MODULE                     
			, @LHS_TABLE                      
			, @LHS_KEY                        
			, @RHS_MODULE                     
			, @RHS_TABLE                      
			, @RHS_KEY                        
			, @JOIN_TABLE                     
			, @JOIN_KEY_LHS                   
			, @JOIN_KEY_RHS                   
			, @RELATIONSHIP_TYPE              
			, @RELATIONSHIP_ROLE_COLUMN       
			, @RELATIONSHIP_ROLE_COL_VALUE 
			, @REVERSE                        
			);
	end else begin
		update RELATIONSHIPS
		   set MODIFIED_USER_ID                = @MODIFIED_USER_ID               
		     , DATE_MODIFIED                   =  getdate()                      
		     , DATE_MODIFIED_UTC               =  getutcdate()                   
		     , RELATIONSHIP_NAME               = @RELATIONSHIP_NAME              
		     , LHS_MODULE                      = @LHS_MODULE                     
		     , LHS_TABLE                       = @LHS_TABLE                      
		     , LHS_KEY                         = @LHS_KEY                        
		     , RHS_MODULE                      = @RHS_MODULE                     
		     , RHS_TABLE                       = @RHS_TABLE                      
		     , RHS_KEY                         = @RHS_KEY                        
		     , JOIN_TABLE                      = @JOIN_TABLE                     
		     , JOIN_KEY_LHS                    = @JOIN_KEY_LHS                   
		     , JOIN_KEY_RHS                    = @JOIN_KEY_RHS                   
		     , RELATIONSHIP_TYPE               = @RELATIONSHIP_TYPE              
		     , RELATIONSHIP_ROLE_COLUMN        = @RELATIONSHIP_ROLE_COLUMN       
		     , RELATIONSHIP_ROLE_COLUMN_VALUE  = @RELATIONSHIP_ROLE_COL_VALUE 
		     , REVERSE                         = @REVERSE                        
		 where ID                              = @ID                             ;
	end -- if;
  end
GO
 
Grant Execute on dbo.spRELATIONSHIPS_Update to public;
GO
 
