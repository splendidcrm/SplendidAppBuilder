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
using System.Text;
using System.Data;

using Microsoft.AspNetCore.Http;

namespace SplendidCRM
{
	public class ExchangeUtils
	{
		public static Guid EXCHANGE_ID = new Guid("00000000-0000-0000-0000-00000000000D");

		// 12/13/2017 Paul.  Allow version to be changed. 
		public static bool ValidateExchange(string sSERVER_URL, string sUSER_NAME, string sPASSWORD, bool bIGNORE_CERTIFICATE, string sIMPERSONATED_TYPE, string sEXCHANGE_VERSION, StringBuilder sbErrors)
		{
			throw(new Exception("Exchange Server integration is not supported."));
		}

		public static void SendTestMessage(string sSERVER_URL, string sUSER_NAME, string sPASSWORD, string sFromAddress, string sFromName, string sToAddress, string sToName)
		{
			throw(new Exception("Exchange Server integration is not supported."));
		}

		public static void SendTestMessage(Guid gOAUTH_TOKEN_ID, string sFromAddress, string sFromName, string sToAddress, string sToName)
		{
			throw(new Exception("Exchange Server integration is not supported."));
		}
		/*
		public static int ValidateImpersonation(string sEXCHANGE_ALIAS, string sEXCHANGE_EMAIL)
		{
			throw(new Exception("Exchange Server integration is not supported."));
		}
		*/
		public static bool ValidateExchange(string sOAuthClientID, string sOAuthClientSecret, Guid gUSER_ID, string sMAILBOX, StringBuilder sbErrors)
		{
			throw(new Exception("Exchange Server integration is not supported."));
		}
		/*
		public static ExchangeService CreateExchangeService(string sEXCHANGE_ALIAS, string sEXCHANGE_EMAIL, string sMAIL_SMTPUSER, string sMAIL_SMTPPASS, Guid gEXCHANGE_ID)
		{
			throw(new Exception("Exchange Server integration is not supported."));
		}

		public static ExchangeService CreateExchangeService(ExchangeSync.UserSync User)
		{
			throw(new Exception("Exchange Server integration is not supported."));
		}

		private static void UpdateFolderTreeNodeCounts(ExchangeService service, XmlNode xFolder)
		{
			throw(new Exception("Exchange Server integration is not supported."));
		}

		public static void UpdateFolderTreeNodeCounts(ExchangeSync.UserSync User, XmlNode xFolder)
		{
			throw(new Exception("Exchange Server integration is not supported."));
		}

		private static void GetFolderTreeFromResults(ExchangeService service, XmlNode xParent, FindFoldersResults fResults)
		{
			throw(new Exception("Exchange Server integration is not supported."));
		}

		public static XmlDocument GetFolderTree(ExchangeSync.UserSync User, ref string sInboxFolderId)
		{
			throw(new Exception("Exchange Server integration is not supported."));
		}

		public static void GetFolderCount(ExchangeSync.UserSync User, string sFOLDER_ID, ref int nTotalCount, ref int nUnreadCount)
		{
			throw(new Exception("Exchange Server integration is not supported."));
		}

		public static void DeleteMessage(ExchangeSync.UserSync User, string sUNIQUE_ID)
		{
			throw(new Exception("Exchange Server integration is not supported."));
		}

		public static DataTable GetMessage(ExchangeSync.UserSync User, string sUNIQUE_ID)
		{
			throw(new Exception("Exchange Server integration is not supported."));
		}

		public static DataTable GetMessage(ExchangeService service, string sUNIQUE_ID)
		{
			throw(new Exception("Exchange Server integration is not supported."));
		}
		*/
		public static void GetMessage(Guid gMAILBOX_ID, string sUNIQUE_ID, ref string sNAME, ref string sFROM_ADDR, ref bool bIS_READ, ref int nSIZE)
		{
			throw(new Exception("Exchange Server integration is not supported."));
		}
		/*
		public static void MarkAsRead(Guid gMAILBOX_ID, string sUNIQUE_ID)
		{
			throw(new Exception("Exchange Server integration is not supported."));
		}
		*/
		public static void MarkAsUnread(Guid gMAILBOX_ID, string sUNIQUE_ID)
		{
			throw(new Exception("Exchange Server integration is not supported."));
		}
		/*
		public static DataTable GetPost(ExchangeService service, string sUNIQUE_ID)
		{
			throw(new Exception("Exchange Server integration is not supported."));
		}

		// 11/06/2010 Paul.  Return the Attachments so that we can show embedded images or download the attachments. 
		public static string GetAttachments(Item email)
		{
			throw(new Exception("Exchange Server integration is not supported."));
		}

		// 11/23/2011 Paul.  Add MAIL_SMTPUSER and MAIL_SMTPPASS so that we can avoid impersonation. 
		public static byte[] GetAttachmentData(ExchangeSync.UserSync User, string sUNIQUE_ID, string sATTACHMENT_ID, ref string sFILENAME, ref string sCONTENT_TYPE, ref bool bINLINE)
		{
			throw(new Exception("Exchange Server integration is not supported."));
		}
		*/
		// 11/23/2011 Paul.  Add MAIL_SMTPUSER and MAIL_SMTPPASS so that we can avoid impersonation. 
		public static DataTable GetFolderMessages(ExchangeSync.UserSync User, string sFOLDER_ID, int nPageSize, int nPageOffset, string sSortColumn, string sSortOrder)
		{
			throw(new Exception("Exchange Server integration is not supported."));
		}

		public static string GetFolderId(string sUSERNAME, string sPASSWORD, Guid gMAILBOX_ID, string sMAILBOX)
		{
			throw(new Exception("Exchange Server integration is not supported."));
		}

		public static DataTable GetFolderMessages(string sUSERNAME, string sPASSWORD, Guid gMAILBOX_ID, string sMAILBOX, bool bONLY_SINCE, string sEXCHANGE_WATERMARK)
		{
			throw(new Exception("Exchange Server integration is not supported."));
		}
		/*
		public static Guid ImportMessage(HttpSessionState Session, ExchangeService service, IDbConnection con, string sPARENT_TYPE, Guid gPARENT_ID, string sEXCHANGE_ALIAS, Guid gUSER_ID, Guid gASSIGNED_USER_ID, Guid gTEAM_ID, string sTEAM_SET_LIST, string sREMOTE_KEY)
		{
			throw(new Exception("Exchange Server integration is not supported."));
		}

		// 07/31/2010 Paul.  Add support for IPM.Post import. 
		public static Guid ImportPost(HttpSessionState Session, ExchangeService service, IDbConnection con, string sPARENT_TYPE, Guid gPARENT_ID, string sEXCHANGE_ALIAS, Guid gUSER_ID, Guid gASSIGNED_USER_ID, Guid gTEAM_ID, string sTEAM_SET_LIST, string sREMOTE_KEY)
		{
			throw(new Exception("Exchange Server integration is not supported."));
		}

		public static Guid ImportMessage(ExchangeSync.UserSync User, HttpSessionState Session, string sPARENT_TYPE, Guid gPARENT_ID, Guid gASSIGNED_USER_ID, Guid gTEAM_ID, string sTEAM_SET_LIST, string sREMOTE_KEY)
		{
			throw(new Exception("Exchange Server integration is not supported."));
		}

		public static string NormalizeInternetAddressName(EmailAddress addr)
		{
			throw(new Exception("Exchange Server integration is not supported."));
		}

		public static void BuildAddressList(EmailAddress addr, StringBuilder sbTO_ADDRS, StringBuilder sbTO_ADDRS_NAMES, StringBuilder sbTO_ADDRS_EMAILS)
		{
			throw(new Exception("Exchange Server integration is not supported."));
		}

		public static Guid FindTargetTrackerKey(EmailMessage email, string sHtmlBody, string sTextBody)
		{
			throw(new Exception("Exchange Server integration is not supported."));
		}

		public static string EmbedInlineImages(EmailMessage email, string sDESCRIPTION_HTML)
		{
			throw(new Exception("Exchange Server integration is not supported."));
		}
		*/
		public static Guid ImportInboundEmail(IDbConnection con, Guid gMAILBOX_ID, string sINTENT, Guid gGROUP_ID, Guid gGROUP_TEAM_ID, string sUNIQUE_ID, string sUNIQUE_MESSAGE_ID)
		{
			throw(new Exception("Exchange Server integration is not supported."));
		}
	}
}
