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
using System.Collections.Generic;
using System.Diagnostics;

namespace SplendidCRM
{
	public class SplendidSession
	{
		private static int nSessionTimeout = 20;
		// 11/16/2014 Paul.  Using a local session variable means that this system will not work on a web farm unless sticky sessions are used. 
		// The alternative is to use the Claims approach of OWIN, but that system seems to be CPU intensive with all the encrypting and decrypting of the claim data. 
		// The claim data is just an encrypted package of non-sensitive user information, such as User ID, User Name and Email. 
		// The claim data is effectively session data that is encrypted and stored in a cookie. 
		private static Dictionary<string, SplendidSession> dictSessions;

		public DateTime Expiration;
		public Guid     USER_ID   ;
		public string   USER_NAME ;

		public static void CreateSession(HttpSessionState Session)
		{
			if ( Session != null )
			{
				if ( dictSessions == null )
				{
					dictSessions = new Dictionary<string, SplendidSession>();
					nSessionTimeout = HttpSessionState.Timeout;
				}
				Guid gUSER_ID = Sql.ToGuid(Session["USER_ID"]);
				if ( !Sql.IsEmptyGuid(gUSER_ID) )
				{
					SplendidSession ss = new SplendidSession();
					ss.Expiration = DateTime.Now.AddMinutes(nSessionTimeout);
					ss.USER_ID   = gUSER_ID;
					ss.USER_NAME = Sql.ToString(Session["USER_NAME"]);
					dictSessions[Session.Id] = ss;
				}
				else
				{
					if ( dictSessions.ContainsKey(Session.Id) )
						dictSessions.Remove(Session.Id);
				}
			}
		}

		public static SplendidSession GetSession(string sSessionID)
		{
			SplendidSession ss = null;
			if ( dictSessions.ContainsKey(sSessionID) )
			{
				ss = dictSessions[sSessionID];
				if ( ss.Expiration < DateTime.Now )
				{
					dictSessions.Remove(sSessionID);
					ss = null;
				}
			}
			return ss;
		}

		public static void PurgeOldSessions(SplendidError SplendidError)
		{
			try
			{
				if ( dictSessions != null )
				{
					DateTime dtNow = DateTime.Now;
					// 11/16/2014 Paul.  We cannot use foreach to remove items from a dictionary, so use a separate list. 
					List<string> arrSessions = new List<string>();
					foreach ( string sSessionID in dictSessions.Keys )
						arrSessions.Add(sSessionID);
					foreach ( string sSessionID in arrSessions )
					{
						SplendidSession ss = dictSessions[sSessionID];
						if ( ss.Expiration < dtNow )
							dictSessions.Remove(sSessionID);
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemMessage("Error", new StackTrace(true).GetFrame(0), ex);
			}
		}
	}
}


