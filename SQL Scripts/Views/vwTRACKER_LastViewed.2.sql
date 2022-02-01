if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwTRACKER_LastViewed')
	Drop View dbo.vwTRACKER_LastViewed;
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
-- 04/06/2006 Paul.  The module name needs to be corrected as it will be used in the URL and the folder names are plural. 
-- 04/06/2006 Paul.  Add the IMAGE_NAME column as the filenames will not be changed. 
-- 07/26/2006 Paul.  Join to the modules table and return the relative path.  This will allow for nested modules. 
-- 07/26/2006 Paul.  Using the RELATIVE_PATH will also mean that the module name need not be corrected. 
-- 03/08/2012 Paul.  Add ACTION to the tracker table so that we can create quick user activity reports. 
-- 03/31/2012 Paul.  Increase name length to 25. 
Create View dbo.vwTRACKER_LastViewed
as
select vwTRACKER.USER_ID
     , vwTRACKER.MODULE_NAME
     , vwMODULES.RELATIVE_PATH
     , vwTRACKER.ITEM_ID
     , (case when len(vwTRACKER.ITEM_SUMMARY) > 25 then left(vwTRACKER.ITEM_SUMMARY, 25) + N'...'
        else ITEM_SUMMARY
        end) as ITEM_SUMMARY
     , vwTRACKER.DATE_ENTERED
     , vwTRACKER.MODULE_NAME as IMAGE_NAME
  from      vwTRACKER
 inner join vwMODULES
         on vwMODULES.MODULE_NAME = vwTRACKER.MODULE_NAME
 where vwTRACKER.ACTION = N'detailview'

GO

Grant Select on dbo.vwTRACKER_LastViewed to public;
GO

