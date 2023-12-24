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
using System;
using System.Linq;
using System.Diagnostics;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;

namespace SplendidCRM
{
	// 07/09/2023 Paul.  Instead of using Security.IsAuthenticated(), use attribute. 
	public class SplendidSessionAuthorizeAttribute: Attribute, IAuthorizationFilter
	{
		private HttpApplicationState Application = new HttpApplicationState();

		public SplendidSessionAuthorizeAttribute()
		{
		}

		public void OnAuthorization(AuthorizationFilterContext context)
		{
			var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
			if ( allowAnonymous )
				return;
			
			if ( context != null )
			{
				bool bIsAuthenticated = false;
				if ( context.HttpContext != null )
				{
					if ( context.HttpContext.Session != null )
					{
						string sUSER_ID = context.HttpContext.Session.GetString("USER_ID");
						if ( !Sql.IsEmptyString(sUSER_ID) )
						{
							bIsAuthenticated = true;
						}
					}
				}
				if ( !bIsAuthenticated )
				{
					L10N L10n = new L10N(Sql.ToString(Application["CONFIG.default_language"]));
					context.Result = new UnauthorizedObjectResult("SplendidSessionAuthorize: " + L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS"));
				}
			}
		}
	}
}
