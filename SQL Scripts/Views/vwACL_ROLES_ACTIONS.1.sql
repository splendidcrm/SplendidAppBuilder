if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwACL_ROLES_ACTIONS')
	Drop View dbo.vwACL_ROLES_ACTIONS;
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
-- 01/15/2010 Paul.  Deleted ACL_ROLES was not being filtered. 
Create View dbo.vwACL_ROLES_ACTIONS
as
select ACL_ROLES.ID          as ROLE_ID
     , ACL_ACTIONS.NAME
     , ACL_ACTIONS.CATEGORY
     , (case when ACL_ROLES_ACTIONS.ACCESS_OVERRIDE is not null then ACL_ROLES_ACTIONS.ACCESS_OVERRIDE
             else ACL_ACTIONS.ACLACCESS
        end)                 as ACLACCESS
  from           ACL_ROLES
 left outer join ACL_ROLES_ACTIONS
              on ACL_ROLES_ACTIONS.ROLE_ID = ACL_ROLES.ID
             and ACL_ROLES_ACTIONS.DELETED = 0
 left outer join ACL_ACTIONS
              on ACL_ACTIONS.ID            = ACL_ROLES_ACTIONS.ACTION_ID
             and ACL_ACTIONS.DELETED       = 0
 where ACL_ROLES.DELETED = 0
GO

Grant Select on dbo.vwACL_ROLES_ACTIONS to public;
GO


