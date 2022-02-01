if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spIMPORT_MAPS_Update' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spIMPORT_MAPS_Update;
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
-- 10/08/2006 Paul.  NAME, SOURCE and MODULE are now nvarchar fields. 
-- 09/15/2009 Paul.  Convert data type to nvarchar(max) to support Azure. 
-- 09/17/2013 Paul.  Add Business Rules to import. 
Create Procedure dbo.spIMPORT_MAPS_Update
	( @ID                uniqueidentifier output
	, @MODIFIED_USER_ID  uniqueidentifier
	, @ASSIGNED_USER_ID  uniqueidentifier
	, @NAME              nvarchar(150)
	, @SOURCE            nvarchar(25)
	, @MODULE            nvarchar(25)
	, @HAS_HEADER        bit
	, @IS_PUBLISHED      bit
	, @CONTENT           nvarchar(max)
	, @RULES_XML         nvarchar(max) = null
	)
as
  begin
	set nocount on
	
	if not exists(select * from IMPORT_MAPS where ID = @ID) begin -- then
		if dbo.fnIsEmptyGuid(@ID) = 1 begin -- then
			set @ID = newid();
		end -- if;
		insert into IMPORT_MAPS
			( ID               
			, CREATED_BY       
			, DATE_ENTERED     
			, MODIFIED_USER_ID 
			, DATE_MODIFIED    
			, ASSIGNED_USER_ID 
			, NAME             
			, SOURCE           
			, MODULE           
			, HAS_HEADER       
			, IS_PUBLISHED     
			, CONTENT          
			, RULES_XML        
			)
		values
			( @ID               
			, @MODIFIED_USER_ID 
			,  getdate()        
			, @MODIFIED_USER_ID 
			,  getdate()        
			, @ASSIGNED_USER_ID 
			, @NAME             
			, @SOURCE           
			, @MODULE           
			, @HAS_HEADER       
			, @IS_PUBLISHED     
			, @CONTENT          
			, @RULES_XML        
			);
	end else begin
		update IMPORT_MAPS
		   set MODIFIED_USER_ID  = @MODIFIED_USER_ID 
		     , DATE_MODIFIED     =  getdate()        
		     , DATE_MODIFIED_UTC =  getutcdate()     
		     , ASSIGNED_USER_ID  = @ASSIGNED_USER_ID 
		     , NAME              = @NAME             
		     , SOURCE            = @SOURCE           
		     , MODULE            = @MODULE           
		     , HAS_HEADER        = @HAS_HEADER       
		     , IS_PUBLISHED      = @IS_PUBLISHED     
		     , CONTENT           = @CONTENT          
		     , RULES_XML         = @RULES_XML        
		 where ID                = @ID               ;
	end -- if;
  end
GO

Grant Execute on dbo.spIMPORT_MAPS_Update to public;
GO

