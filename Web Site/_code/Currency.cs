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
using System.IO;
using System.Net;
using System.Web;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Diagnostics;
//using Spring.Json;

namespace SplendidCRM
{
	/// <summary>
	/// Summary description for Currency.
	/// </summary>
	// 10/09/2017 Paul.  Allow the currency to be stored in the session object. 
	[Serializable]
	public class Currency
	{
		protected Guid   m_gID             ;
		protected string m_sNAME           ;
		protected string m_sSYMBOL         ;
		// 11/10/2008 Paul.  PayPal uses the ISO value. 
		protected string m_sISO4217        ;
		protected float  m_fCONVERSION_RATE;
		protected bool   m_bUSDollars      ;
		public static bool bInsideUpdateRates = false;
		
		// 04/30/2016 Paul.  Base currency has been USD, but we should make it easy to allow a different base. 
		protected Guid m_gUSDollar  = new Guid("E340202E-6291-4071-B327-A34CB4DF239B");
		
		public Guid ID
		{
			get
			{
				return m_gID;
			}
		}

		public string NAME
		{
			get
			{
				return m_sNAME;
			}
		}

		public string SYMBOL
		{
			get
			{
				return m_sSYMBOL;
			}
		}

		public string ISO4217
		{
			get
			{
				return m_sISO4217;
			}
		}

		// 04/30/2016 Paul.  If we are connected to the currency service, then now is a good time to check for changes. 
		public float CONVERSION_RATE
		{
			get
			{
				return m_fCONVERSION_RATE;
			}
			set
			{
				m_fCONVERSION_RATE = value;
			}
		}

		//public static Currency CreateCurrency(Guid gCURRENCY_ID)
		//{
		//	return CreateCurrency(HttpContext.Current.Application, gCURRENCY_ID);
		//}

		// 11/15/2009 Paul.  We need a version of the function that accepts the application. 
		public Currency CreateCurrency(Guid gCURRENCY_ID)
		{
			Currency C10n = Application["CURRENCY." + gCURRENCY_ID.ToString()] as SplendidCRM.Currency;
			if ( C10n == null )
			{
				// 05/09/2006 Paul. First try and use the default from CONFIG. 
				gCURRENCY_ID = Sql.ToGuid(Application["CONFIG.default_currency"]);
				C10n = Application["CURRENCY." + gCURRENCY_ID.ToString()] as SplendidCRM.Currency;
				if ( C10n == null )
				{
					// Default to USD if default not specified. 
					// 04/30/2016 Paul.  Base currency has been USD, but we should make it easy to allow a different base. 
					gCURRENCY_ID = SplendidDefaults.BaseCurrencyID();
					if ( Sql.IsEmptyGuid(gCURRENCY_ID) )
						gCURRENCY_ID = new Guid("E340202E-6291-4071-B327-A34CB4DF239B");
					C10n = Application["CURRENCY." + gCURRENCY_ID.ToString()] as SplendidCRM.Currency;
				}
				// If currency is still null, then create a blank zone. 
				if ( C10n == null )
				{
					C10n = new Currency(Application, Guid.Empty, String.Empty, String.Empty, String.Empty, 1.0f);
					Application["CURRENCY." + gCURRENCY_ID.ToString()] = C10n;
				}
			}
			return C10n;
		}

		// 04/30/2016 Paul.  Require the Application so that we can get the base currency. 
		public Currency CreateCurrency(Guid gCURRENCY_ID, float fCONVERSION_RATE)
		{
			Currency C10n = CreateCurrency(gCURRENCY_ID);
			// 03/31/2007 Paul.  Create a new currency object so that we can override the rate 
			// without overriding the global value. 
			if ( fCONVERSION_RATE == 0.0 )
				fCONVERSION_RATE = 1.0F;
			return new Currency(Application, C10n.ID, C10n.NAME, C10n.SYMBOL, C10n.ISO4217, fCONVERSION_RATE);
		}

		private DbProviderFactories  DbProviderFactories = new DbProviderFactories();
		private HttpApplicationState Application = new HttpApplicationState();
		private SplendidDefaults     SplendidDefaults   = new SplendidDefaults();

		// 04/30/2016 Paul.  Require the Application so that we can get the base currency. 
		public Currency()
		{
			// 04/30/2016 Paul.  Base currency has been USD, but we should make it easy to allow a different base. 
			m_gUSDollar        = SplendidDefaults.BaseCurrencyID();
			m_gID              = m_gUSDollar;
			m_sNAME            = "U.S. Dollar";
			m_sSYMBOL          = "$";
			m_sISO4217         = "USD";
			m_fCONVERSION_RATE = 1.0f;
			m_bUSDollars       = true;
		}
		
		// 11/10/2008 Paul.  PayPal uses the ISO value. 
		// 04/30/2016 Paul.  Require the Application so that we can get the base currency. 
		public Currency
			( HttpApplicationState Application
			, Guid   gID             
			, string sNAME           
			, string sSYMBOL         
			, string sISO4217        
			, float  fCONVERSION_RATE
			)
		{
			// 04/30/2016 Paul.  Base currency has been USD, but we should make it easy to allow a different base. 
			m_gUSDollar        = SplendidDefaults.BaseCurrencyID();
			m_gID              = gID             ;
			m_sNAME            = sNAME           ;
			m_sSYMBOL          = sSYMBOL         ;
			m_sISO4217         = sISO4217        ;
			m_fCONVERSION_RATE = fCONVERSION_RATE;
			m_bUSDollars       = (m_gID == m_gUSDollar);
		}

		public float ToCurrency(float f)
		{
			// 05/10/2006 Paul.  Short-circuit the math if USD. 
			// This is more to prevent bugs than to speed calculations. 
			if ( m_bUSDollars )
				return f;
			return f * m_fCONVERSION_RATE;
		}

		public float FromCurrency(float f)
		{
			// 05/10/2006 Paul.  Short-circuit the math if USD. 
			// This is more to prevent bugs than to speed calculations. 
			if ( m_bUSDollars )
				return f;
			return f / m_fCONVERSION_RATE;
		}

		// 03/30/2007 Paul.  Decimal is the main format for currencies. 
		public Decimal ToCurrency(Decimal d)
		{
			if ( m_bUSDollars )
				return d;
			return Convert.ToDecimal(Convert.ToDouble(d) * m_fCONVERSION_RATE);
		}

		public Decimal FromCurrency(Decimal d)
		{
			// 05/10/2006 Paul.  Short-circuit the math if USD. 
			// This is more to prevent bugs than to speed calculations. 
			// 04/18/2007 Paul.  Protect against divide by zero. 
			if ( m_bUSDollars || m_fCONVERSION_RATE == 0.0 )
				return d;
			return Convert.ToDecimal(Convert.ToDouble(d) / m_fCONVERSION_RATE);
		}

// 12/16/2021 Paul.  CurrencyLayer will not be supported in SplendidApp. 
#if false
		// 05/02/2016 Paul.  Create a scheduler to ensure that the currencies are always correct. 
		public static void UpdateRates(Object sender)
		{
			HttpContext Context = sender as HttpContext;
			if ( !bInsideUpdateRates && !Sql.IsEmptyString(Context.Application["CONFIG.CurrencyLayer.AccessKey"]))
			{
				bInsideUpdateRates = true;
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						string sSQL;
						sSQL = "select *                  " + ControlChars.CrLf
						     + "  from vwCURRENCIES_List  " + ControlChars.CrLf
						     + " where STATUS  = N'Active'" + ControlChars.CrLf
						     + "   and IS_BASE = 0        " + ControlChars.CrLf
						     + " order by NAME            " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								using ( DataTable dt = new DataTable() )
								{
									da.Fill(dt);
									foreach ( DataRow row in dt.Rows )
									{
										StringBuilder sbErrors = new StringBuilder();
										string sISO4217 = Sql.ToString(row["ISO4217"]);
										// 12/04/2021 Paul.  Move GetCurrencyConversionRate to Currency object so tha SplendidApp can exclude OrderUtils. 
										float dRate = Currency.GetCurrencyConversionRate(Context.Application, sISO4217, sbErrors);
										if ( sbErrors.Length > 0 )
										{
											SplendidError.SystemMessage("Error", new StackTrace(true).GetFrame(0), sbErrors.ToString());
										}
									}
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemMessage("Error", new StackTrace(true).GetFrame(0), ex);
				}
				finally
				{
					bInsideUpdateRates = false;
				}
			}
		}

		// 12/04/2021 Paul.  Move GetCurrencyConversionRate to Currency object so tha SplendidApp can exclude OrderUtils. 
		public static float GetCurrencyConversionRate(string sDestinationCurrency, StringBuilder sbErrors)
		{
			// 04/30/2016 Paul.  Require the Application so that we can get the base currency. 
			string sSourceCurrency = SplendidDefaults.BaseCurrencyISO(Application);
			object oRate = HttpRuntime.Cache.Get("CurrencyLayer." + sSourceCurrency + sDestinationCurrency);
			float dRate = 1.0F;
			if ( oRate == null )
			{
				string sAccessKey      = Sql.ToString (Application["CONFIG.CurrencyLayer.AccessKey"     ]);
				bool   bLogConversions = Sql.ToBoolean(Application["CONFIG.CurrencyLayer.LogConversions"]);
				if ( String.Compare(sSourceCurrency, sDestinationCurrency, true) != 0 )
					dRate = Currency.GetCurrencyConversionRate(Application, sAccessKey, bLogConversions, sSourceCurrency, sDestinationCurrency, sbErrors);
			}
			else
			{
				dRate = Sql.ToFloat(oRate);
			}
			return dRate;
		}

		public class CurrencyLayerETag
		{
			public string   ETag;
			public DateTime Date;
			public float    Rate;
		}

		public static float GetCurrencyConversionRate(tring sAccessKey, bool bLogConversions, string sSourceCurrency, string sDestinationCurrency, StringBuilder sbErrors)
		{
			float dRate = 1.0F;
			try
			{
				if ( String.Compare(sSourceCurrency, sDestinationCurrency, true) == 0 )
				{
					dRate = 1.0F;
				}
				else if ( !Sql.IsEmptyString(sAccessKey) )
				{
					bool bUseEncryptedUrl = Sql.ToBoolean(Application["CONFIG.CurrencyLayer.UseEncryptedUrl"]);
					string sBaseURL = (bUseEncryptedUrl ? "https" : "http") + "://apilayer.net/api/live?access_key=";
					HttpWebRequest objRequest = (HttpWebRequest) WebRequest.Create(sBaseURL + sAccessKey + "&source=" + sSourceCurrency.ToUpper() + "&currencies=" + sDestinationCurrency.ToUpper());
					objRequest.KeepAlive         = false;
					objRequest.AllowAutoRedirect = false;
					objRequest.Timeout           = 15000;  //15 seconds
					objRequest.Method            = "GET";
					// 04/30/2016 Paul.  Support ETags for efficient lookups. 
					CurrencyLayerETag oETag = Application["CurrencyLayer." + sSourceCurrency + sDestinationCurrency] as CurrencyLayerETag;
					if ( oETag != null )
					{
						objRequest.Headers.Add("If-None-Match", oETag.ETag);
						objRequest.IfModifiedSince = oETag.Date;
					}
					using ( HttpWebResponse objResponse = (HttpWebResponse) objRequest.GetResponse() )
					{
						if ( objResponse != null )
						{
							if ( objResponse.StatusCode == HttpStatusCode.OK || objResponse.StatusCode == HttpStatusCode.Found )
							{
								using ( StreamReader readStream = new StreamReader(objResponse.GetResponseStream(), System.Text.Encoding.UTF8) )
								{
									string sJsonResponse = readStream.ReadToEnd();
									JsonValue json = JsonValue.Parse(sJsonResponse);
									bool   bSuccess   = json.GetValueOrDefault<bool  >("success"  );
									string sTimestamp = json.GetValueOrDefault<string>("timestamp");
									string sSource    = json.GetValueOrDefault<string>("source"   );
									// {"success":false,"error":{"code":105,"info":"Access Restricted - Your current Subscription Plan does not support HTTPS Encryption."}}
									if ( bSuccess && json.ContainsName("quotes") )
									{
										JsonValue jsonQuotes = json.GetValue("quotes");
										dRate = jsonQuotes.GetValueOrDefault<float>(sSourceCurrency.ToUpper() + sDestinationCurrency.ToUpper());
										int nRateLifetime = Sql.ToInteger(Application["CONFIG.CurrencyLayer.RateLifetime"]);
										if ( nRateLifetime <= 0 )
											nRateLifetime = 90;
										HttpRuntime.Cache.Insert("CurrencyLayer." + sSourceCurrency + sDestinationCurrency, dRate, null, DateTime.Now.AddMinutes(nRateLifetime), System.Web.Caching.Cache.NoSlidingExpiration);
										oETag = new CurrencyLayerETag();
										oETag.ETag = objResponse.Headers.Get("ETag");
										oETag.Rate = dRate;
										DateTime.TryParse(objResponse.Headers.Get("Date"), out oETag.Date);
										Application["CurrencyLayer." + sSourceCurrency + sDestinationCurrency] = oETag;
										
										DbProviderFactory dbf = DbProviderFactories.GetFactory();
										using ( IDbConnection con = dbf.CreateConnection() )
										{
											con.Open();
											using ( IDbTransaction trn = Sql.BeginTransaction(con) )
											{
												try
												{
													Guid gSYSTEM_CURRENCY_LOG = Guid.Empty;
													if ( bLogConversions )
													{
														SqlProcs.spSYSTEM_CURRENCY_LOG_InsertOnly
															( ref gSYSTEM_CURRENCY_LOG
															, "CurrencyLayer"       // SERVICE_NAME
															, sSourceCurrency       // SOURCE_ISO4217
															, sDestinationCurrency  // DESTINATION_ISO4217
															, dRate                 // CONVERSION_RATE
															, sJsonResponse         // RAW_CONTENT
															, trn
															);
													}
													// 04/30/2016 Paul.  We have to update the currency record as it is used inside stored procedures. 
													if ( sSourceCurrency == SplendidDefaults.BaseCurrencyISO(Application) )
													{
														SqlProcs.spCURRENCIES_UpdateRateByISO
															( sDestinationCurrency
															, dRate
															, gSYSTEM_CURRENCY_LOG
															, trn
															);
													}
													trn.Commit();
												}
												catch
												{
													trn.Rollback();
													throw;
												}
											}
										}
									}
									else if ( json.ContainsName("error") )
									{
										JsonValue jsonError = json.GetValue("error");
										string sInfo = jsonError.GetValue<string>("info");
										sbErrors.Append(sInfo);
									}
									else
									{
										sbErrors.Append("Conversion not found for " + sSourceCurrency + " to " + sDestinationCurrency + ".");
									}
								}
							}
							else if ( objResponse.StatusCode == HttpStatusCode.NotModified )
							{
								dRate = oETag.Rate;
							}
							else
							{
								sbErrors.Append(objResponse.StatusDescription);
							}
						}
					}
				}
				else
				{
					sbErrors.Append("CurrencyLayer access key is empty.");
				}
				if ( sbErrors.Length > 0 )
				{
					SplendidError.SystemMessage(Application, "Error", new StackTrace(true).GetFrame(0), "CurrencyLayer " + sSourceCurrency + sDestinationCurrency + ": " + sbErrors.ToString());
				}
			}
			catch(Exception ex)
			{
				sbErrors.AppendLine(ex.Message);
				SplendidError.SystemMessage(Application, "Error", new StackTrace(true).GetFrame(0), "CurrencyLayer " + sSourceCurrency + sDestinationCurrency + ": " + Utils.ExpandException(ex));
			}
			return dRate;
		}
#endif
	}
}


