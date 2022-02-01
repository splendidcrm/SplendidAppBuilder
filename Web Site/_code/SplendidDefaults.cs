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
using System.Data;
using System.Data.Common;
using System.Web;
using System.Threading;
using System.Globalization;
/*
using System.Web.SessionState;
using System.Text;
using System.Xml;
using System.Diagnostics;
*/

namespace SplendidCRM
{
	/// <summary>
	/// Summary description for SplendidDefaults.
	/// </summary>
	public class SplendidDefaults
	{
		private HttpApplicationState Application = new HttpApplicationState();

		public SplendidDefaults()
		{
		}

		// 12/22/2007 Paul.  Inside the timer event, there is no current context, so we need to pass the application. 
		public string Culture()
		{
			string sCulture = Sql.ToString(Application["CONFIG.default_language"]);
			// 12/22/2007 Paul.  The cache is not available when we are inside the timer event. 
			// 02/18/2008 Paul.  The Languages function is now thread safe, so it can be called from the timer. 
			//if ( HttpContext.Current != null && HttpContext.Current.Cache != null )
			{
				// 01/08/2017 Paul.  We are getting an odd exception from within the workflow thread. Just ignore and continue. 
// 12/16/2021 Paul.  We need to prevent ciruclar references, so don't try and validate the default language. 
#if false
				try
				{
					DataView vwLanguages = new DataView(SplendidCache.Languages(Application));
					// 05/20/2008 Paul.  Normalize culture before lookup. 
					vwLanguages.RowFilter = "NAME = '" + L10N.NormalizeCulture(sCulture) +"'";
					if ( vwLanguages.Count > 0 )
						sCulture = Sql.ToString(vwLanguages[0]["NAME"]);
				}
				catch
				{
				}
#endif
			}
			if ( Sql.IsEmptyString(sCulture) )
				sCulture = "en-US";
			return L10N.NormalizeCulture(sCulture);
		}

		public string Theme()
		{
			string sTheme = Sql.ToString(Application["CONFIG.default_theme"]);
			// 10/16/2015 Paul.  Change default theme to our newest theme. 
			// 10/02/2016 Paul.  Make the default theme Arctic. 
			if ( Sql.IsEmptyString(sTheme) )
				sTheme = "Arctic";
			return sTheme;
		}

		public string MobileTheme()
		{
			string sTheme = Sql.ToString(Application["CONFIG.default_mobile_theme"]);
			if ( Sql.IsEmptyString(sTheme) )
				sTheme = "Mobile";
			return sTheme;
		}

		public string DateFormat()
		{
			string sDateFormat = Sql.ToString(Application["CONFIG.default_date_format"]);
			if ( Sql.IsEmptyString(sDateFormat) )
				sDateFormat = "MM/dd/yyyy";
			// 11/28/2005 Paul.  Need to make sure that the default format is valid. 
			else if ( !IsValidDateFormat(sDateFormat) )
				sDateFormat = DateFormat(sDateFormat);
			return sDateFormat;
		}

		public static bool IsValidDateFormat(string sDateFormat)
		{
			if ( sDateFormat.IndexOf("m") >= 0 || sDateFormat.IndexOf("yyyy") < 0 )
				return false;
			return true;
		}

		public static string DateFormat(string sDateFormat)
		{
			// 11/12/2005 Paul.  "m" is not valid for .NET month formatting.  Must use MM. 
			if ( sDateFormat.IndexOf("m") >= 0 )
			{
				sDateFormat = sDateFormat.Replace("m", "M");
			}
			// 11/12/2005 Paul.  Require 4 digit year.  Otherwise default date in Pipeline of 12/31/2100 would get converted to 12/31/00. 
			if ( sDateFormat.IndexOf("yyyy") < 0 )
			{
				sDateFormat = sDateFormat.Replace("yy", "yyyy");
			}
			return sDateFormat;
		}

		public string TimeFormat()
		{
			string sTimeFormat = Sql.ToString(Application["CONFIG.default_time_format"]);
			if ( Sql.IsEmptyString(sTimeFormat) || sTimeFormat == "H:i" )
				sTimeFormat = "h:mm tt";
			return sTimeFormat;
		}

		public string TimeZone()
		{
			// 08/08/2006 Paul.  Pull the default timezone and fall-back to Eastern US only if empty. 
			string sDEFAULT_TIMEZONE = Sql.ToString(Application["CONFIG.default_timezone"]);
			if ( Sql.IsEmptyGuid(sDEFAULT_TIMEZONE) )
				sDEFAULT_TIMEZONE = "BFA61AF7-26ED-4020-A0C1-39A15E4E9E0A";
			return sDEFAULT_TIMEZONE;
		}

		public string TimeZone(SplendidCache SplendidCache, int nTimez)
		{
			string sTimeZone = String.Empty;
			DataView vwTimezones = new DataView(SplendidCache.Timezones());
			vwTimezones.RowFilter = "BIAS = " + nTimez.ToString();
			if ( vwTimezones.Count > 0 )
				sTimeZone = Sql.ToString(vwTimezones[0]["ID"]);
			else
				sTimeZone = TimeZone();
			return sTimeZone;
		}

		public string CurrencyID()
		{
			// 08/08/2006 Paul.  Pull the default currency and fall-back to Dollars only if empty. 
			string sDEFAULT_CURRENCY = Sql.ToString(Application["CONFIG.default_currency"]);
			if ( Sql.IsEmptyGuid(sDEFAULT_CURRENCY) )
			{
				sDEFAULT_CURRENCY = "E340202E-6291-4071-B327-A34CB4DF239B";
			}
			return sDEFAULT_CURRENCY;
		}

		// 04/30/2016 Paul.  Base currency has been USD, but we should make it easy to allow a different base. 
		public Guid BaseCurrencyID()
		{
			Guid gBASE_CURRENCY = Sql.ToGuid(Application["CONFIG.base_currency"]);
			if ( Sql.IsEmptyGuid(gBASE_CURRENCY) )
				gBASE_CURRENCY = new Guid("E340202E-6291-4071-B327-A34CB4DF239B");
			return gBASE_CURRENCY;
		}

		public string BaseCurrencyISO()
		{
			string sBASE_ISO4217 = "USD";
			Guid gBASE_CURRENCY = BaseCurrencyID();
			Currency C10n = Application["CURRENCY." + gBASE_CURRENCY.ToString()] as SplendidCRM.Currency;
			if ( C10n != null )
			{
				sBASE_ISO4217 = C10n.ISO4217;
				if ( Sql.IsEmptyString(sBASE_ISO4217) )
					sBASE_ISO4217 = "USD";
			}
			return sBASE_ISO4217;
		}

		public string GroupSeparator()
		{
			// 02/29/2008 Paul.  The config value should only be used as an override.  We should default to the .NET culture value. 
			string sGROUP_SEPARATOR = Sql.ToString(Application["CONFIG.default_number_grouping_seperator"]);
			if ( Sql.IsEmptyString(sGROUP_SEPARATOR) )
				sGROUP_SEPARATOR  = Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyGroupSeparator;
			return sGROUP_SEPARATOR;
		}

		public string DecimalSeparator()
		{
			// 02/29/2008 Paul.  The config value should only be used as an override.  We should default to the .NET culture value. 
			string sDECIMAL_SEPARATOR = Sql.ToString(Application["CONFIG.default_decimal_seperator"]);
			if ( Sql.IsEmptyString(sDECIMAL_SEPARATOR) )
				sDECIMAL_SEPARATOR = Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;
			return sDECIMAL_SEPARATOR;
		}

		public string generate_graphcolor(string sInput, int nInstance)
		{
			string sColor = String.Empty;
			if ( nInstance < 20 )
			{
				string[] arrGraphColor =
				{
					"0xFF0000"
					, "0x00FF00"
					, "0x0000FF"
					, "0xFF6600"
					, "0x42FF8E"
					, "0x6600FF"
					, "0xFFFF00"
					, "0x00FFFF"
					, "0xFF00FF"
					, "0x66FF00"
					, "0x0066FF"
					, "0xFF0066"
					, "0xCC0000"
					, "0x00CC00"
					, "0x0000CC"
					, "0xCC6600"
					, "0x00CC66"
					, "0x6600CC"
					, "0xCCCC00"
					, "0x00CCCC"
				};
				sColor = arrGraphColor[nInstance];
			}
			else
			{
				sColor = "0x00CCCC";
				//sColor = "0x" + substr(md5(sInput), 0, 6);
			}
			return sColor;
		}

	}
}


