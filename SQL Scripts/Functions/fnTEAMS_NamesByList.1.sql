if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'fnTEAMS_NamesByList' and ROUTINE_TYPE = 'FUNCTION')
	Drop Function dbo.fnTEAMS_NamesByList;
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
-- 08/23/2009 Paul.  Decrease set list so that index plus ID will be less than 900 bytes. 
Create Function dbo.fnTEAMS_NamesByList(@TEAM_SET_LIST varchar(851))
returns nvarchar(200)
as
  begin
	declare @TEAM_SET_NAME nvarchar(200);
	declare @NAME          nvarchar(128);
	declare @ID            uniqueidentifier;
	declare @CurrentPosR   int;
	declare @NextPosR      int;
	set @CurrentPosR   = 1;
	set @TEAM_SET_NAME = null;
	while @CurrentPosR <= len(@TEAM_SET_LIST) begin -- do
		set @NextPosR = charindex(',', @TEAM_SET_LIST,  @CurrentPosR);
		if @NextPosR = 0 or @NextPosR is null begin -- then
			set @NextPosR = len(@TEAM_SET_LIST) + 1;
		end -- if;
		set @ID = cast(rtrim(ltrim(substring(@TEAM_SET_LIST, @CurrentPosR, @NextPosR - @CurrentPosR))) as uniqueidentifier);
		set @CurrentPosR = @NextPosR+1;

		-- BEGIN Oracle Exception
			select @NAME   = NAME
			  from TEAMS
			 where ID      = @ID
			   and DELETED = 0;
		-- END Oracle Exception
		if @NAME is not null begin -- then
			if @TEAM_SET_NAME is null begin -- then
				set @TEAM_SET_NAME = @NAME;
			end else begin
				set @TEAM_SET_NAME = substring(@TEAM_SET_NAME + N', ' + @NAME, 1, 200);
			end -- if;
		end -- if;
	end -- while;
	return @TEAM_SET_NAME;
  end
GO

Grant Execute on dbo.fnTEAMS_NamesByList to public
GO

