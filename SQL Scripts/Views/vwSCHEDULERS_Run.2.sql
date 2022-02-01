if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwSCHEDULERS_Run')
	Drop View dbo.vwSCHEDULERS_Run;
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
-- 12/31/2007 Paul.  When comparing against the CRON pattern, round the time down to the nearest 5 minute interval. 
-- 01/18/2008 Paul.  Lets make sure that the CheckVersion occurs shortly after application install. 
-- The trick is to skip the CRON filter if the CheckVersion job has never run. 
-- 01/18/2008 Paul.  Simplify code to handle LAST_RUN to match the Oracle implementation. 
Create View dbo.vwSCHEDULERS_Run
as
select vwSCHEDULERS.*
     , dbo.fnTimeRoundMinutes(getdate(), 5) as NEXT_RUN
  from vwSCHEDULERS
 where STATUS = N'Active'
   and (DATE_TIME_START is null or getdate() > DATE_TIME_START)
   and (DATE_TIME_END   is null or getdate() < DATE_TIME_END  )
   and (TIME_FROM       is null or getdate() > (dbo.fnDateAdd_Time(TIME_FROM, dbo.fnDateOnly(getdate()))))
   and (TIME_TO         is null or getdate() < (dbo.fnDateAdd_Time(TIME_TO  , dbo.fnDateOnly(getdate()))))
   and (   (JOB = N'function::CheckVersion' and LAST_RUN is null)
        or dbo.fnCronRun(JOB_INTERVAL, dbo.fnTimeRoundMinutes(getdate(), 5), 5) = 1
       )
   and (LAST_RUN is null or dbo.fnTimeRoundMinutes(getdate(), 5) > dbo.fnTimeRoundMinutes(LAST_RUN, 5))
GO

Grant Select on dbo.vwSCHEDULERS_Run to public;
GO

