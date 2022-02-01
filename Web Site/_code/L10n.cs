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
using Microsoft.AspNetCore.Http;

namespace SplendidCRM
{
	/// <summary>
	/// Summary description for L10n.
	/// </summary>
	public class L10N
	{
		protected string m_sNAME;
		
		public string NAME
		{
			get
			{
				return m_sNAME;
			}
		}

		private HttpApplicationState Application      = new HttpApplicationState();
		private SplendidDefaults     SplendidDefaults = new SplendidDefaults();

		public L10N()
		{
			this.m_sNAME = SplendidDefaults.Culture();
		}

		public L10N(string sNAME)
		{
			// 11/19/2005 Paul.  We may be connecting to MySQL, so the language may have an underscore. 
			if ( Sql.IsEmptyString(sNAME) )
				sNAME = SplendidDefaults.Culture();
			this.m_sNAME = NormalizeCulture(sNAME);
		}

		public L10N(HttpContext Context)
		{
			string sNAME = SplendidDefaults.Culture();
			HttpSessionState Session = Context.Features.Get<HttpSessionState>();
			if ( Session != null )
			{
				sNAME = Sql.ToString(Session["USER_LANG"]);
			}
			this.m_sNAME = NormalizeCulture(sNAME);
		}

		public bool IsLanguageRTL()
		{
			bool bRTL = false;
			switch ( m_sNAME.Substring(0, 2) )
			{
				case "ar":  bRTL = true;  break;
				case "he":  bRTL = true;  break;
				case "ur":  bRTL = true;  break;
				case "fa":  bRTL = true;  break;  // 12/17/2008 Paul.  Farsi is also RTL. 
			}
			return bRTL;
		}

		// 04/20/2018 Paul.  Alternate language mapping to convert en-CA to en_US. 
		public string AlternateLanguage(string sCulture)
		{
			string sAlternateName = Sql.ToString(Application["CONFIG.alternate_language." + sCulture]);
			if ( !Sql.IsEmptyString(sAlternateName) )
				sCulture = sAlternateName;
			return sCulture;
		}

		public static string NormalizeCulture(string sCulture)
		{
			// 08/28/2005 Paul.  Default to English if nothing specified. 
			// 09/02/2008 Paul.  Default to English if nothing specified.  This can happen if a user is created programatically. 
			if ( Sql.IsEmptyString(sCulture) )
				sCulture = "en-US";
			sCulture = sCulture.Replace("_", "-");
			// 05/20/2008 Paul.  We are now storing the language in the proper case, so make sure to normalize with proper case. 
			sCulture = sCulture.Substring(0, 2).ToLower() + sCulture.Substring(2).ToUpper();
			return sCulture;
		}

		public object Term(string sListName, object oField)
		{
			return Term(m_sNAME, sListName, oField);
		}

		// 08/17/2005 Paul.  Special Term function that helps with a list. 
		public object Term(string sCultureName, string sListName, object oField)
		{
			// 01/11/2008 Paul.  Protect against uninitialized variables. 
			if ( String.IsNullOrEmpty(sListName) )
				return String.Empty;

			if ( oField == null || oField == DBNull.Value )
				return oField;
			// 11/28/2005 Paul.  Convert field to string instead of cast.  Cast will not work for integer fields. 
			return Term(sCultureName, sListName + oField.ToString());
		}

		public string Term(string sEntryName)
		{
			return Term(m_sNAME, sEntryName);
		}

		public string Term(string sCultureName, string sEntryName)
		{
			// 01/11/2008 Paul.  Protect against uninitialized variables. 
			if ( String.IsNullOrEmpty(sEntryName) || Application == null )
				return String.Empty;

			//string sNAME = "en-us";
			object oDisplayName = Application[sCultureName + "." + sEntryName];
			if ( oDisplayName == null )
			{
				// 01/11/2007 Paul.  Default to English if term not found. 
				// There are just too many untranslated terms when importing a SugarCRM Language Pack. 
				oDisplayName = Application["en-US." + sEntryName];
				if ( oDisplayName == null )
				{
					// Prevent parameter out of range errors with <asp:Button AccessKey="" />
					if ( sEntryName.EndsWith("_BUTTON_KEY") )
						return String.Empty;
					// 07/07/2008 Paul.  If the entry name is not found, post a warning message
					// then define the entry so that we will only get one warning per run. 
					if ( sEntryName.Contains(".") )
					{
						Application["en-US." + sEntryName] = sEntryName;
// 12/19/2021 Paul.  Cannot access SplendidError. 
#if false
						// 09/18/2009 Paul.  The end-user should not see these any more. 
						// There are simply too many false-positives that are caused by a page or control being bound twice. 
						SplendidError.SystemMessage("Warning", new StackTrace(true).GetFrame(0), "L10N.Term: \"" + sEntryName + "\" not found.");
#endif
					}
					return sEntryName;
				}
			}
			return oDisplayName.ToString();
		}

		// 06/30/2007 Paul.  Prevent parameter out of range errors with <asp:Button AccessKey="" />.  Not all access keys end in _BUTTON_KEY. 
		public string AccessKey(string sEntryName)
		{
			// 01/11/2008 Paul.  Protect against uninitialized variables. 
			if ( String.IsNullOrEmpty(sEntryName) )
				return String.Empty;

			//string sNAME = "en-us";
			object oDisplayName = Application[NAME + "." + sEntryName];
			if ( oDisplayName == null )
			{
				// 01/11/2007 Paul.  Default to English if term not found. 
				// There are just too many untranslated terms when importing a SugarCRM Language Pack. 
				oDisplayName = Application["en-US." + sEntryName];
				if ( oDisplayName == null )
				{
					return String.Empty;
				}
			}
			// 06/30/2007 Paul.  AccessKey too long, cannot be more than one character.
			// 07/03/2007 Paul.  Protect against an empty AccessKey string. 
			string sAccessKey = oDisplayName.ToString();
			if ( sAccessKey.Length == 0 )
				return String.Empty;
			return sAccessKey.Substring(0, 1);
		}

		public string AliasedTerm(string sEntryName)
		{
			// 01/11/2008 Paul.  Protect against uninitialized variables. 
			if ( String.IsNullOrEmpty(sEntryName) )
				return String.Empty;

			object oAliasedName = Application["ALIAS_" + sEntryName];
			if ( oAliasedName == null )
				return Term(sEntryName);
			return Term(oAliasedName.ToString());
		}

		public void SetTerm(string sLANG, string sMODULE_NAME, string sNAME, string sDISPLAY_NAME)
		{
			Application[sLANG + "." + sMODULE_NAME + "." + sNAME] = sDISPLAY_NAME;
		}

		public void SetTerm(string sLANG, string sMODULE_NAME, string sLIST_NAME, string sNAME, string sDISPLAY_NAME)
		{
			// 01/13/2006 Paul.  Don't include MODULE_NAME when used with a list. DropDownLists are populated without the module name in the list name. 
			// 01/13/2006 Paul.  We can remove the module, but not the dot.  Otherwise it breaks all other code that references a list term. 
			sMODULE_NAME = String.Empty;
			Application[sLANG + "." + sMODULE_NAME + "." + sLIST_NAME + "." + sNAME] = sDISPLAY_NAME;
		}

		public void SetAlias(string sALIAS_MODULE_NAME, string sALIAS_LIST_NAME, string sALIAS_NAME, string sMODULE_NAME, string sLIST_NAME, string sNAME)
		{
			if ( Sql.IsEmptyString(sALIAS_LIST_NAME) )
				Application["ALIAS_" + sALIAS_MODULE_NAME + "." + sALIAS_NAME] = sMODULE_NAME + "." + sNAME;
			else
				Application["ALIAS_" + sALIAS_MODULE_NAME + "." + sALIAS_LIST_NAME + "." + sALIAS_NAME] = sMODULE_NAME + "." + sLIST_NAME + "." + sNAME;
		}
		
		public string TermJavaScript(string sEntryName)
		{
			string sDisplayName = Term(sEntryName);
			sDisplayName = sDisplayName.Replace("\'", "\\\'");
			sDisplayName = sDisplayName.Replace("\"", "\\\"");
			sDisplayName = sDisplayName.Replace(ControlChars.CrLf, @"\r\n");
			return sDisplayName;
		}
	}
}


