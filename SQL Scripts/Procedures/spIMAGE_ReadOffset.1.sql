if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spIMAGE_ReadOffset' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spIMAGE_ReadOffset;
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
Create Procedure dbo.spIMAGE_ReadOffset
	( @ID                   uniqueidentifier
	, @FILE_OFFSET          int
	, @READ_SIZE            int
	, @BYTES                varbinary(max) output
	)
as
  begin
	set nocount on
	
	-- 08/12/2005 Paul.  Oracle returns its data in the @BYTES field. 
	-- 10/22/2005 Paul.  MySQL can also return data in @BYTES, but using a recordset has fewer limitations. 
	-- 01/25/2007 Paul.  Protect against a read error by ensuring that the file size is zero if no content. 
-- #if SQL_Server /*
	raiserror(N'updatetext, readtext and textptr() have been deprecated. ', 16, 1);
	-- declare @FILE_SIZE    bigint;
	-- declare @FILE_POINTER binary(16);
	-- select @FILE_SIZE    = isnull(datalength(CONTENT), 0)
	--      , @FILE_POINTER = textptr(CONTENT)
	--   from IMAGES
	--  where ID            = @ID;
	-- if @FILE_OFFSET + @READ_SIZE > @FILE_SIZE begin -- then
	-- 	set @READ_SIZE = @FILE_SIZE - @FILE_OFFSET;
	-- end -- if;
	-- if @READ_SIZE > 0 begin -- then
	-- 	readtext IMAGES.CONTENT @FILE_POINTER @FILE_OFFSET @READ_SIZE;
	-- end -- if;
-- #endif SQL_Server */



  end
GO
 
Grant Execute on dbo.spIMAGE_ReadOffset to public;
GO


