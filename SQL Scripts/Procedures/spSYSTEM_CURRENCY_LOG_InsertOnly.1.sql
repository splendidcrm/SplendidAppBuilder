if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spSYSTEM_CURRENCY_LOG_InsertOnly' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spSYSTEM_CURRENCY_LOG_InsertOnly;
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
Create Procedure dbo.spSYSTEM_CURRENCY_LOG_InsertOnly
	( @ID                   uniqueidentifier output
	, @MODIFIED_USER_ID     uniqueidentifier
	, @SERVICE_NAME         nvarchar(50)
	, @SOURCE_ISO4217       nvarchar(3)
	, @DESTINATION_ISO4217  nvarchar(3)
	, @CONVERSION_RATE      float(53)
	, @RAW_CONTENT          nvarchar(max)
	)
as
  begin
	set nocount on
	
	set @ID = newid();
	insert into SYSTEM_CURRENCY_LOG
		( ID                  
		, CREATED_BY          
		, DATE_ENTERED        
		, MODIFIED_USER_ID    
		, DATE_MODIFIED       
		, DATE_MODIFIED_UTC   
		, SERVICE_NAME        
		, SOURCE_ISO4217      
		, DESTINATION_ISO4217 
		, CONVERSION_RATE     
		, RAW_CONTENT         
		)
	values 	( @ID                  
		, @MODIFIED_USER_ID    
		,  getdate()           
		, @MODIFIED_USER_ID    
		,  getdate()           
		,  getutcdate()        
		, @SERVICE_NAME        
		, @SOURCE_ISO4217      
		, @DESTINATION_ISO4217 
		, @CONVERSION_RATE     
		, @RAW_CONTENT         
		);
  end
GO

Grant Execute on dbo.spSYSTEM_CURRENCY_LOG_InsertOnly to public;
GO

