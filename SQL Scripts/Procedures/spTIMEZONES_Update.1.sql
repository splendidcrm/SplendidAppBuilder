if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spTIMEZONES_Update' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spTIMEZONES_Update;
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
-- 01/02/2012 Paul.  Add iCal TZID. 
-- 03/26/2013 Paul.  iCloud uses linked_timezone values from http://tzinfo.rubyforge.org/doc/. 
Create Procedure dbo.spTIMEZONES_Update
	( @ID                     uniqueidentifier output
	, @MODIFIED_USER_ID       uniqueidentifier
	, @NAME                   nvarchar(100)
	, @STANDARD_NAME          nvarchar(100)
	, @STANDARD_ABBREVIATION  nvarchar(10)
	, @DAYLIGHT_NAME          nvarchar(100)
	, @DAYLIGHT_ABBREVIATION  nvarchar(10)
	, @BIAS                   int
	, @STANDARD_BIAS          int
	, @DAYLIGHT_BIAS          int
	, @STANDARD_YEAR          int
	, @STANDARD_MONTH         int
	, @STANDARD_WEEK          int
	, @STANDARD_DAYOFWEEK     int
	, @STANDARD_HOUR          int
	, @STANDARD_MINUTE        int
	, @DAYLIGHT_YEAR          int
	, @DAYLIGHT_MONTH         int
	, @DAYLIGHT_WEEK          int
	, @DAYLIGHT_DAYOFWEEK     int
	, @DAYLIGHT_HOUR          int
	, @DAYLIGHT_MINUTE        int
	, @TZID                   nvarchar(50) = null
	, @LINKED_TIMEZONE        nvarchar(50) = null
	)
as
  begin
	set nocount on
	
	if not exists(select * from TIMEZONES where ID = @ID) begin -- then
		if dbo.fnIsEmptyGuid(@ID) = 1 begin -- then
			set @ID = newid();
		end -- if;
		insert into TIMEZONES
			( ID                    
			, CREATED_BY            
			, DATE_ENTERED          
			, MODIFIED_USER_ID      
			, DATE_MODIFIED         
			, NAME                  
			, STANDARD_NAME         
			, STANDARD_ABBREVIATION 
			, DAYLIGHT_NAME         
			, DAYLIGHT_ABBREVIATION 
			, BIAS                  
			, STANDARD_BIAS         
			, DAYLIGHT_BIAS         
			, STANDARD_YEAR         
			, STANDARD_MONTH        
			, STANDARD_WEEK         
			, STANDARD_DAYOFWEEK    
			, STANDARD_HOUR         
			, STANDARD_MINUTE       
			, DAYLIGHT_YEAR         
			, DAYLIGHT_MONTH        
			, DAYLIGHT_WEEK         
			, DAYLIGHT_DAYOFWEEK    
			, DAYLIGHT_HOUR         
			, DAYLIGHT_MINUTE       
			, TZID                  
			, LINKED_TIMEZONE       
			)
		values
			( @ID                    
			, @MODIFIED_USER_ID      
			,  getdate()             
			, @MODIFIED_USER_ID      
			,  getdate()             
			, @NAME                  
			, @STANDARD_NAME         
			, @STANDARD_ABBREVIATION 
			, @DAYLIGHT_NAME         
			, @DAYLIGHT_ABBREVIATION 
			, @BIAS                  
			, @STANDARD_BIAS         
			, @DAYLIGHT_BIAS         
			, @STANDARD_YEAR         
			, @STANDARD_MONTH        
			, @STANDARD_WEEK         
			, @STANDARD_DAYOFWEEK    
			, @STANDARD_HOUR         
			, @STANDARD_MINUTE       
			, @DAYLIGHT_YEAR         
			, @DAYLIGHT_MONTH        
			, @DAYLIGHT_WEEK         
			, @DAYLIGHT_DAYOFWEEK    
			, @DAYLIGHT_HOUR         
			, @DAYLIGHT_MINUTE       
			, @TZID                  
			, @LINKED_TIMEZONE       
			);
	end else begin
		update TIMEZONES
		   set MODIFIED_USER_ID       = @MODIFIED_USER_ID      
		     , DATE_MODIFIED          =  getdate()             
		     , DATE_MODIFIED_UTC      =  getutcdate()          
		     , NAME                   = @NAME                  
		     , STANDARD_NAME          = @STANDARD_NAME         
		     , STANDARD_ABBREVIATION  = @STANDARD_ABBREVIATION 
		     , DAYLIGHT_NAME          = @DAYLIGHT_NAME         
		     , DAYLIGHT_ABBREVIATION  = @DAYLIGHT_ABBREVIATION 
		     , BIAS                   = @BIAS                  
		     , STANDARD_BIAS          = @STANDARD_BIAS         
		     , DAYLIGHT_BIAS          = @DAYLIGHT_BIAS         
		     , STANDARD_YEAR          = @STANDARD_YEAR         
		     , STANDARD_MONTH         = @STANDARD_MONTH        
		     , STANDARD_WEEK          = @STANDARD_WEEK         
		     , STANDARD_DAYOFWEEK     = @STANDARD_DAYOFWEEK    
		     , STANDARD_HOUR          = @STANDARD_HOUR         
		     , STANDARD_MINUTE        = @STANDARD_MINUTE       
		     , DAYLIGHT_YEAR          = @DAYLIGHT_YEAR         
		     , DAYLIGHT_MONTH         = @DAYLIGHT_MONTH        
		     , DAYLIGHT_WEEK          = @DAYLIGHT_WEEK         
		     , DAYLIGHT_DAYOFWEEK     = @DAYLIGHT_DAYOFWEEK    
		     , DAYLIGHT_HOUR          = @DAYLIGHT_HOUR         
		     , DAYLIGHT_MINUTE        = @DAYLIGHT_MINUTE       
		     , TZID                   = @TZID                  
		     , LINKED_TIMEZONE        = @LINKED_TIMEZONE       
		 where ID                     = @ID                    ;
	end -- if;
  end
GO
 
Grant Execute on dbo.spTIMEZONES_Update to public;
GO
 
