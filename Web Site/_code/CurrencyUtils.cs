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
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;

namespace SplendidCRM
{
	/// <summary>
	/// Summary description for Currency.
	/// </summary>
	public class CurrencyUtils
	{
		public static bool bInsideUpdateRates = false;

		private IWebHostEnvironment  hostingEnvironment ;
		private IMemoryCache         memoryCache        ;
		private DbProviderFactories  DbProviderFactories = new DbProviderFactories();
		private HttpApplicationState Application         = new HttpApplicationState();
		private SplendidDefaults     SplendidDefaults    = new SplendidDefaults();
		private HttpContext          Context            ;
		private HttpSessionState     Session            ;
		private Security             Security           ;
		private Sql                  Sql                ;
		private SqlProcs             SqlProcs           ;
		private SplendidError        SplendidError      ;
		private SplendidCache        SplendidCache      ;
		private SplendidDynamic      SplendidDynamic    ;

		public CurrencyUtils(IWebHostEnvironment hostingEnvironment, IMemoryCache memoryCache, IHttpContextAccessor httpContextAccessor, HttpSessionState Session, Security Security, Sql Sql, SqlProcs SqlProcs, SplendidError SplendidError, SplendidCache SplendidCache, SplendidDynamic SplendidDynamic)
		{
			this.hostingEnvironment  = hostingEnvironment ;
			this.memoryCache         = memoryCache        ;
			this.Context             = httpContextAccessor.HttpContext;
			this.Session             = Session            ;
			this.Security            = Security           ;;
			this.Sql                 = Sql                ;
			this.SqlProcs            = SqlProcs           ;
			this.SplendidError       = SplendidError      ;
			this.SplendidCache       = SplendidCache      ;
			this.SplendidDynamic     = SplendidDynamic    ;
		}

#pragma warning disable CS1998
		public async ValueTask UpdateRates(CancellationToken token)
		{
			UpdateRates();
		}
#pragma warning restore CS1998

		// 05/02/2016 Paul.  Create a scheduler to ensure that the currencies are always correct. 
		public void UpdateRates()
		{
			if ( !bInsideUpdateRates && !Sql.IsEmptyString(Application["CONFIG.CurrencyLayer.AccessKey"]))
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
										float dRate = GetCurrencyConversionRate(sISO4217, sbErrors);
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
		public float GetCurrencyConversionRate(string sDestinationCurrency, StringBuilder sbErrors)
		{
			// 04/30/2016 Paul.  Require the Application so that we can get the base currency. 
			string sSourceCurrency = SplendidDefaults.BaseCurrencyISO();
			object oRate = memoryCache.Get("CurrencyLayer." + sSourceCurrency + sDestinationCurrency);
			float dRate = 1.0F;
			if ( oRate == null )
			{
				string sAccessKey      = Sql.ToString (Application["CONFIG.CurrencyLayer.AccessKey"     ]);
				bool   bLogConversions = Sql.ToBoolean(Application["CONFIG.CurrencyLayer.LogConversions"]);
				if ( String.Compare(sSourceCurrency, sDestinationCurrency, true) != 0 )
					dRate = GetCurrencyConversionRate(sAccessKey, bLogConversions, sSourceCurrency, sDestinationCurrency, sbErrors);
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

		public float GetCurrencyConversionRate(String sAccessKey, bool bLogConversions, string sSourceCurrency, string sDestinationCurrency, StringBuilder sbErrors)
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
									JsonDocument json = JsonDocument.Parse(sJsonResponse);
									bool   bSuccess   = json.RootElement.GetProperty("success"  ).GetBoolean();
									string sTimestamp = json.RootElement.GetProperty("timestamp").GetString();
									string sSource    = json.RootElement.GetProperty("source"   ).GetString();
									// {"success":false,"error":{"code":105,"info":"Access Restricted - Your current Subscription Plan does not support HTTPS Encryption."}}
									JsonElement jsonQuotes;
									JsonElement jsonError;
									if ( bSuccess && json.RootElement.TryGetProperty("quotes", out jsonQuotes) )
									{
										dRate = (float) jsonQuotes.GetProperty(sSourceCurrency.ToUpper() + sDestinationCurrency.ToUpper()).GetDecimal();
										int nRateLifetime = Sql.ToInteger(Application["CONFIG.CurrencyLayer.RateLifetime"]);
										if ( nRateLifetime <= 0 )
											nRateLifetime = 90;
										memoryCache.Set("CurrencyLayer." + sSourceCurrency + sDestinationCurrency, dRate, DateTime.Now.AddMinutes(nRateLifetime));
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
													if ( sSourceCurrency == SplendidDefaults.BaseCurrencyISO() )
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
									else if ( json.RootElement.TryGetProperty("error", out jsonError) )
									{
										string sInfo = jsonError.GetProperty("info").GetString();
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
					SplendidError.SystemMessage("Error", new StackTrace(true).GetFrame(0), "CurrencyLayer " + sSourceCurrency + sDestinationCurrency + ": " + sbErrors.ToString());
				}
			}
			catch(Exception ex)
			{
				sbErrors.AppendLine(ex.Message);
				SplendidError.SystemMessage("Error", new StackTrace(true).GetFrame(0), "CurrencyLayer " + sSourceCurrency + sDestinationCurrency + ": " + Utils.ExpandException(ex));
			}
			return dRate;
		}
	}
}

