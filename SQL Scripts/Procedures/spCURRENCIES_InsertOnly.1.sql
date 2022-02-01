if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spCURRENCIES_InsertOnly' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spCURRENCIES_InsertOnly;
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
-- 05/01/2016 Paul.  We are going to prepopulate the list and the ISO4217 is required and unique. 
-- 04/15/2020 Paul.  Allow the USD default to be renamed. 
Create Procedure dbo.spCURRENCIES_InsertOnly
	( @ID                uniqueidentifier
	, @MODIFIED_USER_ID  uniqueidentifier
	, @NAME              nvarchar(36)
	, @SYMBOL            nvarchar(36)
	, @ISO4217           nvarchar(3)
	, @CONVERSION_RATE   float(53)
	, @STATUS            nvarchar(25)
	)
as
  begin
	set nocount on
	
	-- 05/01/2016 Paul.  We are going to prepopulate the list and the ISO4217 is required and unique. 
	declare @TEMP_ID     uniqueidentifier;
	declare @TEMP_SYMBOL nvarchar(36);
	set @TEMP_ID = @ID;
	-- BEGIN Oracle Exception
		select @TEMP_ID = ID
		  from CURRENCIES
		 where ISO4217  = @ISO4217
		   and DELETED  = 0;
	-- END Oracle Exception

	if @ISO4217 is null or @ISO4217 = N'' begin -- then
		raiserror(N'ISO4217 is required', 16, 1);
		return;
	end -- if;
	if exists(select * from CURRENCIES where DELETED = 0 and ISO4217 = @ISO4217 and (ID <> @ID or @ID is null)) begin -- then
		if @ID <> 'E340202E-6291-4071-B327-A34CB4DF239B' begin -- then
			raiserror(N'ISO4217 must be unique', 16, 1);
		end -- if;
		return;
	end -- if;
	set @TEMP_SYMBOL = @SYMBOL;
	if @TEMP_SYMBOL is null or @TEMP_SYMBOL = N'' begin -- then
		set @TEMP_SYMBOL = @ISO4217;
	end -- if;
	if not exists(select * from CURRENCIES where ID = @TEMP_ID) begin -- then
		if dbo.fnIsEmptyGuid(@TEMP_ID) = 1 begin -- then
			set @TEMP_ID = newid();
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
			( @TEMP_ID          
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

Grant Execute on dbo.spCURRENCIES_InsertOnly to public;
GO

