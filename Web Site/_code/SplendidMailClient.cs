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
using System.Net.Mail;

using Microsoft.Extensions.Caching.Memory;

namespace SplendidCRM
{
	// 01/17/2017 Paul.  New SplendidMailClient object to encapsulate SMTP, Exchange and Google mail. 
	abstract public class SplendidMailClient
	{
		abstract public void Send(MailMessage mail);

		// 01/18/2017 Paul.  This method will return the appropriate Campaign Manager client, based on configuration. This is the global email sending account. 
		public static SplendidMailClient CreateMailClient(HttpApplicationState Application, IMemoryCache memoryCache, Security Security, SplendidError SplendidError, GoogleApps GoogleApps, Spring.Social.Office365.Office365Sync Office365Sync)
		{
			string sMAIL_SENDTYPE = Sql.ToString(Application["CONFIG.mail_sendtype"]);
			SplendidMailClient client = null;
			{
				client = new SplendidMailSmtp(Application, memoryCache, Security, SplendidError);
			}
			return client;
		}
	}
}
