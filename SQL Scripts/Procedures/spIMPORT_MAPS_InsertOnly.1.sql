if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spIMPORT_MAPS_InsertOnly' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spIMPORT_MAPS_InsertOnly;
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
-- 09/15/2009 Paul.  Convert data type to nvarchar(max) to support Azure. 
Create Procedure dbo.spIMPORT_MAPS_InsertOnly
	( @ID                uniqueidentifier output
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
	
	declare @MODIFIED_USER_ID  uniqueidentifier;
	declare @ASSIGNED_USER_ID  uniqueidentifier;
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
	end -- if;
  end
GO

Grant Execute on dbo.spIMPORT_MAPS_InsertOnly to public;
GO

