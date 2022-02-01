if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwACL_ROLES_ACTIONS_Category')
	Drop View dbo.vwACL_ROLES_ACTIONS_Category;
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
Create View dbo.vwACL_ROLES_ACTIONS_Category
as
select ACL_ROLES_ACTIONS.ID
     , ACL_ROLES_ACTIONS.DELETED
     , ACL_ROLES_ACTIONS.CREATED_BY
     , ACL_ROLES_ACTIONS.DATE_ENTERED
     , ACL_ROLES_ACTIONS.MODIFIED_USER_ID
     , ACL_ROLES_ACTIONS.DATE_MODIFIED
     , ACL_ROLES_ACTIONS.DATE_MODIFIED_UTC
     , ACL_ROLES_ACTIONS.ROLE_ID
     , ACL_ROLES_ACTIONS.ACTION_ID
     , ACL_ROLES_ACTIONS.ACCESS_OVERRIDE
     , ACL_ACTIONS.CATEGORY
  from      ACL_ROLES_ACTIONS
 inner join ACL_ACTIONS
         on ACL_ACTIONS.ID = ACL_ROLES_ACTIONS.ACTION_ID

GO

Grant Select on dbo.vwACL_ROLES_ACTIONS_Category to public;
GO


