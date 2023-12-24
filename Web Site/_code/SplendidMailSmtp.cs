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
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;

using Microsoft.Extensions.Caching.Memory;

namespace SplendidCRM
{
	public class SplendidMailSmtp : SplendidMailClient
	{
		private SmtpClient     smtpClient    ;

		public SplendidMailSmtp(HttpApplicationState Application, IMemoryCache memoryCache, Security Security, SplendidError SplendidError)
		{
			string sSmtpServer      = Sql.ToString (Application["CONFIG.smtpserver"     ]);
			int    nSmtpPort        = Sql.ToInteger(Application["CONFIG.smtpport"       ]);
			bool   bSmtpAuthReq     = Sql.ToBoolean(Application["CONFIG.smtpauth_req"   ]);
			bool   bSmtpSSL         = Sql.ToBoolean(Application["CONFIG.smtpssl"        ]);
			string sSmtpUser        = Sql.ToString (Application["CONFIG.smtpuser"       ]);
			string sSmtpPassword    = Sql.ToString (Application["CONFIG.smtppass"       ]);
			string sX509Certificate = Sql.ToString (Application["CONFIG.smtpcertificate"]);
			if ( !Sql.IsEmptyString(sSmtpPassword) )
				sSmtpPassword = Security.DecryptPassword(sSmtpPassword);
			smtpClient = CreateSmtpClient(Application, memoryCache, SplendidError, sSmtpServer, nSmtpPort, bSmtpAuthReq, bSmtpSSL, sSmtpUser, sSmtpPassword, sX509Certificate);
		}

		public SplendidMailSmtp(HttpApplicationState Application, IMemoryCache memoryCache, Security Security, SplendidError SplendidError, string sSmtpServer, int nSmtpPort, bool bSmtpAuthReq, bool bSmtpSSL)
		{
			string sSmtpUser        = Sql.ToString (Application["CONFIG.smtpuser"       ]);
			string sSmtpPassword    = Sql.ToString (Application["CONFIG.smtppass"       ]);
			string sX509Certificate = Sql.ToString (Application["CONFIG.smtpcertificate"]);
			if ( !Sql.IsEmptyString(sSmtpPassword) )
				sSmtpPassword = Security.DecryptPassword(sSmtpPassword);
			if ( Sql.IsEmptyString(sSmtpServer) )
			{
				sSmtpServer   = Sql.ToString (Application["CONFIG.smtpserver"  ]);
				nSmtpPort     = Sql.ToInteger(Application["CONFIG.smtpport"    ]);
				bSmtpAuthReq  = Sql.ToBoolean(Application["CONFIG.smtpauth_req"]);
				bSmtpSSL      = Sql.ToBoolean(Application["CONFIG.smtpssl"     ]);
			}
			smtpClient = CreateSmtpClient(Application, memoryCache, SplendidError, sSmtpServer, nSmtpPort, bSmtpAuthReq, bSmtpSSL, sSmtpUser, sSmtpPassword, sX509Certificate);
		}

		public SplendidMailSmtp(HttpApplicationState Application, IMemoryCache memoryCache, SplendidError SplendidError, string sSmtpServer, int nSmtpPort, bool bSmtpAuthReq, bool bSmtpSSL, string sSmtpUser, string sSmtpPassword, string sX509Certificate)
		{
			smtpClient = CreateSmtpClient(Application, memoryCache, SplendidError, sSmtpServer, nSmtpPort, bSmtpAuthReq, bSmtpSSL, sSmtpUser, sSmtpPassword, sX509Certificate);
		}

		// 07/19/2010 Paul.  Create a new method so we can provide a way to skip the decryption of the system password. 
		// 07/18/2013 Paul.  Add support for multiple outbound emails. 
		private SmtpClient CreateSmtpClient(HttpApplicationState Application, IMemoryCache memoryCache, SplendidError SplendidError, string sSmtpServer, int nSmtpPort, bool bSmtpAuthReq, bool bSmtpSSL, string sSmtpUser, string sSmtpPassword, string sX509Certificate)
		{
			// 01/12/2008 Paul.  We must decrypt the password before using it. 
			// 02/02/2017 Paul.  Password is always in non-encrypted format. 
			//if ( !Sql.IsEmptyString(sSmtpPassword) )
			//{
			//	sSmtpPassword = Security.DecryptPassword(Application, sSmtpPassword);
			//}
			if ( Sql.IsEmptyString(sSmtpServer) )
				sSmtpServer = "127.0.0.1";
			if ( nSmtpPort == 0 )
				nSmtpPort = 25;

			// 04/17/2006 Paul.  Use config value for SMTP server. 
			// 12/21/2006 Paul.  Allow the use of SMTP servers that require authentication. 
			// 07/21/2013 Paul.  Gmail should use 587 and not 465 with EnableSsl. 
			// http://stackoverflow.com/questions/1082216/gmail-smtp-via-c-sharp-net-errors-on-all-ports
			SmtpClient client = new SmtpClient(sSmtpServer, nSmtpPort);
			client.Timeout = 60 * 1000;
			// 01/12/2008 Paul.  Use SMTP SSL flag to support Gmail. 
			if ( bSmtpSSL )
			{
				client.EnableSsl = true;
				// 11/16/2009 Paul.  One of our Live clients would like to use a client certificate for SMTP. 
				// 07/19/2010 Paul.  We are not going to support user certificates at this time. 
				if ( Sql.IsEmptyString(sSmtpPassword) && !Sql.IsEmptyString(sX509Certificate) )
				{
					try
					{
						X509Certificate cert = memoryCache.Get("SMTP.X509Certificate") as X509Certificate;
						if ( cert == null )
						{
							const string sCertHeader = "-----BEGIN CERTIFICATE-----";
							const string sCertFooter = "-----END CERTIFICATE-----";
							sX509Certificate = sX509Certificate.Trim();
							if (sX509Certificate.StartsWith(sCertHeader) && sX509Certificate.EndsWith(sCertFooter))
							{
								sX509Certificate = sX509Certificate.Substring(sCertHeader.Length, sX509Certificate.Length - sCertHeader.Length - sCertFooter.Length);
								byte[] byPKS8  = Convert.FromBase64String(sX509Certificate.Trim());
								
								cert = new X509Certificate(byPKS8);
							}
							else
							{
								throw(new Exception("Invalid X509 Certificate.  Missing BEGIN CERTIFICATE or END CERTIFICATE."));
							}
							memoryCache.Set("SMTP.X509Certificate", cert, DefaultCacheExpiration());
						}
						if ( cert != null )
							client.ClientCertificates.Add(cert);
					}
					catch(Exception ex)
					{
						SplendidError.SystemMessage("Error", new StackTrace(true).GetFrame(0), "Failed to add SMTP certificate to email: " + Utils.ExpandException(ex));
					}
				}
			}
			// 07/19/2010 Paul.  Use the user credentials if provided. 
			if ( bSmtpAuthReq && !Sql.IsEmptyString(sSmtpPassword) )
				client.Credentials = new NetworkCredential(sSmtpUser, sSmtpPassword);
			else
				client.UseDefaultCredentials = true;
			return client;
		}

		public DateTime DefaultCacheExpiration()
		{
#if DEBUG
			return DateTime.Now.AddSeconds(1);
#else
			return DateTime.Now.AddDays(1);
#endif
		}

		override public void Send(MailMessage mail)
		{
			smtpClient.Send(mail);
		}
	}
}
