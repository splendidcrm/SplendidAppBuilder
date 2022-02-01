if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spCURRENCIES_InsertOnlyByISO' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spCURRENCIES_InsertOnlyByISO;
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
Create Procedure dbo.spCURRENCIES_InsertOnlyByISO
	( @NAME              nvarchar(36)
	, @SYMBOL            nvarchar(36)
	, @ISO4217           nvarchar(3)
	, @CONVERSION_RATE   float(53)
	, @STATUS            nvarchar(25)
	)
as
  begin
	set nocount on
	
	-- 05/01/2016 Paul.  We are going to prepopulate the list and the ISO4217 is required and unique. 
	declare @ID               uniqueidentifier;
	declare @MODIFIED_USER_ID uniqueidentifier;
	declare @TEMP_SYMBOL      nvarchar(36);
	if @ISO4217 is null or @ISO4217 = N'' begin -- then
		raiserror(N'ISO4217 is required', 16, 1);
		return;
	end -- if;
	set @TEMP_SYMBOL = @SYMBOL;
	if @TEMP_SYMBOL is null or @TEMP_SYMBOL = N'' begin -- then
		set @TEMP_SYMBOL = @ISO4217;
	end -- if;
	if not exists(select * from CURRENCIES where ISO4217 = @ISO4217) begin -- then
		if dbo.fnIsEmptyGuid(@ID) = 1 begin -- then
			set @ID = newid();
		end -- if;
		insert into CURRENCIES
			( ID               
			, CREATED_BY       
			, DATE_ENTERED     
			, MODIFIED_USER_ID 
			, DATE_MODIFIED    
			, NAME             
			, SYMBOL           
			, ISO4217          
			, CONVERSION_RATE  
			, STATUS           
			)
		values
			( @ID               
			, @MODIFIED_USER_ID 
			,  getdate()        
			, @MODIFIED_USER_ID 
			,  getdate()        
			, @NAME             
			, @TEMP_SYMBOL      
			, @ISO4217          
			, @CONVERSION_RATE  
			, @STATUS           
			);

		if not exists(select * from CURRENCIES_CSTM where ID_C = @ID) begin -- then
			insert into CURRENCIES_CSTM ( ID_C ) values ( @ID );
		end -- if;
	end -- if;
  end
GO

Grant Execute on dbo.spCURRENCIES_InsertOnlyByISO to public;
GO

