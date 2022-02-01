if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spUSER_PREFERENCES_WriteOffset' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spUSER_PREFERENCES_WriteOffset;
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
-- 09/15/2009 Paul.  Convert data type to varbinary(max) to support Azure. 
-- 09/15/2009 Paul.  updatetext, readtext and textptr() have been deprecated in SQL Server and are not supported in Azure. 
-- http://msdn.microsoft.com/en-us/library/ms143729.aspx
Create Procedure dbo.spUSER_PREFERENCES_WriteOffset
	( @ID                   uniqueidentifier
	, @FILE_POINTER         binary(16)
	, @MODIFIED_USER_ID     uniqueidentifier
	, @FILE_OFFSET          int
	, @BYTES                varbinary(max)
	)
as
  begin
	set nocount on
	
	-- 10/22/2005 Paul.  @ID is used in Oracle and MySQL. 
-- #if SQL_Server /*
	raiserror(N'updatetext, readtext and textptr() have been deprecated. ', 16, 1);
	-- updatetext USER_PREFERENCES.CONTENT
	--            @FILE_POINTER
	--            @FILE_OFFSET
	--            null -- 0 deletes no data, null deletes all data from insertion point. 
	--            @BYTES;
-- #endif SQL_Server */



  end
GO
 
Grant Execute on dbo.spUSER_PREFERENCES_WriteOffset to public;
GO



