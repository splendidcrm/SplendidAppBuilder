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
using System.Xml;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Diagnostics;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;

namespace SplendidCRM
{
	// 12/24/2007 Paul.  Use an array to define the custom caches so that list is in the Cache module. 
	// This should reduce the number of times that we have to edit the SplendidDynamic module. 
	public delegate DataTable SplendidCacheCallback();

	public class SplendidCacheReference
	{
		private string                m_sName          ;
		private string                m_sDataValueField;
		private string                m_sDataTextField ;
		private SplendidCacheCallback m_fnDataSource   ;

		public string Name
		{
			get { return m_sName; }
		}

		public string DataValueField
		{
			get { return m_sDataValueField; }
		}

		public string DataTextField
		{
			get { return m_sDataTextField; }
		}

		public SplendidCacheCallback DataSource
		{
			get { return m_fnDataSource; }
			set { m_fnDataSource = value; }
		}

		public SplendidCacheReference(string sName, string sDataValueField, string sDataTextField, SplendidCacheCallback fnDataSource)
		{
			m_sName           = sName          ;
			m_sDataValueField = sDataValueField;
			m_sDataTextField  = sDataTextField ;
			m_fnDataSource    = fnDataSource   ;
		}
	}

	/// <summary>
	/// Summary description for SplendidCache.
	/// </summary>
	// 08/23/2015 Paul  Partial class so that we can add to it in separate files. 
	public partial class SplendidCache
	{
		private IWebHostEnvironment  hostingEnvironment ;
		private IHttpContextAccessor httpContextAccessor;
		private IMemoryCache         Cache              ;
		private DbProviderFactories  DbProviderFactories = new DbProviderFactories();
		private HttpApplicationState Application = new HttpApplicationState();
		private HttpSessionState     Session            ;
		private Security             Security           ;
		private Sql                  Sql                ;
		private L10N                 L10n               ;
		private Currency             Currency           = new Currency();
		private TimeZone             TimeZone           = new TimeZone();
		private SqlProcs             SqlProcs           ;
		private SplendidError        SplendidError      ;
		private Crm.Modules          CrmModules         ;
		private Crm.Config           Config             = new Crm.Config();
		private XmlUtil              XmlUtil            ;

		// 10/04/2015 Paul.  Changed custom caches to a dynamic list. 
		public List<SplendidCacheReference> CustomCaches = new List<SplendidCacheReference>();

		public SplendidCache(IWebHostEnvironment hostingEnvironment, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache, HttpSessionState Session, Security Security, Sql Sql, SqlProcs SqlProcs, SplendidError SplendidError, Crm.Modules Modules, XmlUtil XmlUtil)
		{
			this.hostingEnvironment  = hostingEnvironment ;
			this.httpContextAccessor = httpContextAccessor;
			this.Cache               = memoryCache        ;
			this.Session             = Session            ;
			this.Security            = Security           ;
			this.L10n                = new L10N(Sql.ToString(Session["USER_SETTINGS/CULTURE"]));
			this.Sql                 = Sql                ;
			this.SqlProcs            = SqlProcs           ;
			this.SplendidError       = SplendidError      ;
			this.CrmModules          = Modules            ;
			this.XmlUtil             = XmlUtil            ;

			CustomCaches.Add(new SplendidCacheReference("AssignedUser"      , "ID"         , "USER_NAME"   , new SplendidCacheCallback(this.AssignedUser      )));
			// 03/06/2012 Paul.  A report parameter can include an Assigned To list. 
			CustomCaches.Add(new SplendidCacheReference("AssignedTo"        , "USER_NAME"  , "USER_NAME"   , new SplendidCacheCallback(this.AssignedUser      )));
			CustomCaches.Add(new SplendidCacheReference("Currencies"        , "ID"         , "NAME_SYMBOL" , new SplendidCacheCallback(this.Currencies        )));
			CustomCaches.Add(new SplendidCacheReference("Release"           , "ID"         , "NAME"        , new SplendidCacheCallback(this.Release           )));
			CustomCaches.Add(new SplendidCacheReference("Manufacturers"     , "ID"         , "NAME"        , new SplendidCacheCallback(this.Manufacturers     )));
			// 08/13/2010 Paul.  Add discounts to line items. 
			CustomCaches.Add(new SplendidCacheReference("Discounts"         , "ID"         , "NAME"        , new SplendidCacheCallback(this.Discounts         )));
			CustomCaches.Add(new SplendidCacheReference("Shippers"          , "ID"         , "NAME"        , new SplendidCacheCallback(this.Shippers          )));
			// 02/15/2015 Paul.  Change from terminology payment_types_dom to PaymentTypes list for QuickBooks Online. 
			CustomCaches.Add(new SplendidCacheReference("PaymentTypes"      , "ID"         , "NAME"        , new SplendidCacheCallback(this.PaymentTypes      )));
			// 02/27/2015 Paul.  Change from terminology payment_terms_dom to PaymentTerms list for QuickBooks Online. 
			CustomCaches.Add(new SplendidCacheReference("PaymentTerms"      , "ID"         , "NAME"        , new SplendidCacheCallback(this.PaymentTerms      )));
			// 12/21/2010 Paul.  Allow regions to be used in a list. 
			CustomCaches.Add(new SplendidCacheReference("Regions"           , "ID"         , "NAME"        , new SplendidCacheCallback(this.Regions           )));
			CustomCaches.Add(new SplendidCacheReference("ProductTypes"      , "ID"         , "NAME"        , new SplendidCacheCallback(this.ProductTypes      )));
			CustomCaches.Add(new SplendidCacheReference("ProductCategories" , "ID"         , "NAME"        , new SplendidCacheCallback(this.ProductCategories )));
			CustomCaches.Add(new SplendidCacheReference("ContractTypes"     , "ID"         , "NAME"        , new SplendidCacheCallback(this.ContractTypes     )));
			CustomCaches.Add(new SplendidCacheReference("ForumTopics"       , "NAME"       , "NAME"        , new SplendidCacheCallback(this.ForumTopics       )));  // 07/15/2007 Paul.  Add Forum Topics to the list of possible dropdowns. 
			// 09/03/2008 Paul.  Not sure why the text was set to MODULE_NAME, but it should be DISPLAY_NAME. 
			CustomCaches.Add(new SplendidCacheReference("Modules"           , "MODULE_NAME", "DISPLAY_NAME", new SplendidCacheCallback(this.Modules           )));  // 12/13/2007 Paul.  Managing shortcuts needs a dropdown of modules. 
			// 10/18/2011 Paul.  The HTML5 Offline Client needs a list of module tables. 
			CustomCaches.Add(new SplendidCacheReference("ModuleTables"      , "MODULE_NAME", "DISPLAY_NAME", new SplendidCacheCallback(this.Modules           )));  // 12/13/2007 Paul.  Managing shortcuts needs a dropdown of modules. 
			// 11/10/2010 Paul.  Provide access to Rules Modules in SearchViews. 
			CustomCaches.Add(new SplendidCacheReference("RulesModules"      , "MODULE_NAME", "DISPLAY_NAME", new SplendidCacheCallback(this.RulesModules      )));
			CustomCaches.Add(new SplendidCacheReference("ReportingModules"  , "MODULE_NAME", "DISPLAY_NAME", new SplendidCacheCallback(this.ReportingModules  )));
			CustomCaches.Add(new SplendidCacheReference("WorkflowModules"   , "MODULE_NAME", "DISPLAY_NAME", new SplendidCacheCallback(this.WorkflowModules   )));
			CustomCaches.Add(new SplendidCacheReference("EmailGroups"       , "ID"         , "NAME"        , new SplendidCacheCallback(this.EmailGroups       )));
			CustomCaches.Add(new SplendidCacheReference("InboundEmailBounce", "ID"         , "NAME"        , new SplendidCacheCallback(this.InboundEmailBounce)));
			// 11/18/2008 Paul.  Teams can be used in the search panels. 
			CustomCaches.Add(new SplendidCacheReference("Teams"             , "ID"         , "NAME"        , new SplendidCacheCallback(this.Teams             )));
			// 01/24/2010 Paul.  Place the report list in the cache so that it would be available in SearchView. 
			CustomCaches.Add(new SplendidCacheReference("Reports"           , "ID"         , "NAME"        , new SplendidCacheCallback(this.Reports           )));
			// 09/10/2012 Paul.  Add User Signatures. 
			CustomCaches.Add(new SplendidCacheReference("UserSignatures"    , "ID"         , "NAME"        , new SplendidCacheCallback(this.UserSignatures    )));
			// 01/21/2013 Paul.  Allow Time Zones to be used in EditView. 
			CustomCaches.Add(new SplendidCacheReference("TimeZones"         , "ID"         , "NAME"        , new SplendidCacheCallback(this.TimezonesListbox  )));
			// 07/18/2013 Paul.  Add support for multiple outbound emails. 
			// 09/23/2013 Paul.  OutboundMail should use the display name field. 
			CustomCaches.Add(new SplendidCacheReference("OutboundMail"      , "ID"         , "DISPLAY_NAME", new SplendidCacheCallback(this.OutboundMail      )));
			// 09/23/2013 Paul.  Add support for multiple outbound sms. 
			CustomCaches.Add(new SplendidCacheReference("OutboundSms"       , "ID"         , "DISPLAY_NAME", new SplendidCacheCallback(this.OutboundSms       )));
			// 12/13/2013 Paul.  Allow each product to have a default tax rate. 
			// 09/23/2013 Paul.  Add support for multiple outbound sms. 
			CustomCaches.Add(new SplendidCacheReference("TaxRates"          , "ID"         , "NAME"        , new SplendidCacheCallback(this.TaxRates          )));
			// 12/12/2015 Paul.  /n Software and .netCharge use different lists. 
			CustomCaches.Add(new SplendidCacheReference("LibraryPaymentGateways", "NAME"   , "DISPLAY_NAME", new SplendidCacheCallback(this.LibraryPaymentGateways)));
			// 12/16/2015 Paul.  credit_card_year should be a custom list that adds 10 years to current year. 
			CustomCaches.Add(new SplendidCacheReference("credit_card_year"      , "NAME"   , "DISPLAY_NAME", new SplendidCacheCallback(this.CreditCardYears       )));
			// 05/12/2016 Paul.  Add Tags module. 
			CustomCaches.Add(new SplendidCacheReference("Tags"              , "ID"         , "NAME"        , new SplendidCacheCallback(this.Tags              )));
			// 02/21/2017 Paul.  Allow langauges to be used in a list. 
			CustomCaches.Add(new SplendidCacheReference("Languages"         , "NAME"       , "DISPLAY_NAME", new SplendidCacheCallback(this.Languages         )));
			// 06/27/2018 Paul.  Add ERASED_FIELDS when data privacy enabled. 
			CustomCaches.Add(new SplendidCacheReference("DataPrivacyFields" , "NAME"       , "DISPLAY_NAME", new SplendidCacheCallback(this.DataPrivacyFields )));
			// 03/26/2019 Paul.  Scheduler list so that it can be returned by REST API. 
			CustomCaches.Add(new SplendidCacheReference("SchedulerJobs"       , "NAME"     , "DISPLAY_NAME", new SplendidCacheCallback(this.SchedulerJobs       )));
			// 03/28/2019 Paul.  TerminologyPickLists so that it can be returned by REST API. 
			CustomCaches.Add(new SplendidCacheReference("TerminologyPickLists", "LIST_NAME", "LIST_NAME"   , new SplendidCacheCallback(this.TerminologyPickLists)));
			// 04/03/2019 Paul.  DynamicButtonViews so that it can be returned by REST API. 
			CustomCaches.Add(new SplendidCacheReference("DynamicButtonViews"  , "VIEW_NAME", "VIEW_NAME"   , new SplendidCacheCallback(this.DynamicButtonViews  )));
			// 05/01/2020 Paul.  Cache EmailTemplates for use in React Client. 
			CustomCaches.Add(new SplendidCacheReference("EmailTemplates"      , "ID"       , "NAME"        , new SplendidCacheCallback(this.EmailTemplates      )));
		}

		// 06/02/2016 Paul.  Activities views will use new function that accepts an array of modules. 
		public string[] arrActivityModules = new string[] {"Calls", "Meetings", "Tasks", "Emails", "Notes", "SmsMessages", "TwitterMessages", "ChatMessages"};

		// 03/26/2019 Paul.  Scheduler list so that it can be returned by REST API. 
		public DataTable SchedulerJobs()
		{
			// 05/14/2023 Paul.  L10n is not used.  Should use default or Session, not Application. 
			//L10N L10n = new L10N(HttpContext.Current.Application["USER_SETTINGS/CULTURE"] as string);
			DataTable dt = Cache.Get("SchedulerJobs") as DataTable;
			if ( dt == null )
			{
				dt = new DataTable();
				dt.Columns.Add("NAME"        , Type.GetType("System.String"));
				dt.Columns.Add("DISPLAY_NAME", Type.GetType("System.String"));
				foreach ( string sJob in SchedulerUtils.Jobs )
				{
					DataRow row = dt.NewRow();
					dt.Rows.Add(row);
					row["NAME"        ] = "function::" + sJob;
					row["DISPLAY_NAME"] = "function::" + sJob;
				}
				Cache.Set("SchedulerJobs", dt, DefaultCacheExpiration());
			}
			return dt;
		}

		// 04/03/2019 Paul.  DynamicButtonViews so that it can be returned by REST API. 
		public DataTable DynamicButtonViews()
		{
			DataTable dt = Cache.Get("DynamicButtonViews") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select distinct VIEW_NAME" + ControlChars.CrLf
						     + "  from vwDYNAMIC_BUTTONS " + ControlChars.CrLf
						     + " order by VIEW_NAME      " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("DynamicButtonViews", dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		// 02/16/2012 Paul.  We need a separate list for report parameter lists. 
		public void AddReportSource(string sName, string sDataValueField, string sDataTextField, DataTable dt)
		{
			SplendidCacheReference cacheReportList = new SplendidCacheReference(sName, sDataValueField, sDataTextField, delegate { return dt; });
			Cache.Remove("Reports.Source." + sName);
			// 02/17/2021 Paul.  We encountered an issue where a static/SpecificValues list was not populated.  if list is empty, the shorten the cache period to 2 minutes. 
			DateTime dtExpiration = DateTime.Now.AddHours(1);
			if ( dt == null || dt.Rows.Count == 0 )
			{
				dtExpiration = DateTime.Now.AddMinutes(2);
				Debug.WriteLine("SplendidCache.AddReportSource " + sName + " is empty");
			}
			Cache.Set("Reports.Source." + sName, cacheReportList, dtExpiration);
		}

		// https://www.py4u.net/discuss/1941713
		public void ClearCache(IMemoryCache cache)
		{
			if (cache == null)
			{
				throw new ArgumentNullException("Memory cache must not be null");
			}
			else if (cache is MemoryCache memCache)
			{
				memCache.Compact(1.0);
				return;
			}
			else
			{
				MethodInfo clearMethod = cache.GetType().GetMethod("Clear", BindingFlags.Instance | BindingFlags.Public);
				if (clearMethod != null)
				{
					clearMethod.Invoke(cache, null);
					return;
				}
				else
				{
					PropertyInfo prop = cache.GetType().GetProperty("EntriesCollection", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public);
					if (prop != null)
					{
						object innerCache = prop.GetValue(cache);
						if (innerCache != null)
						{
							clearMethod = innerCache.GetType().GetMethod("Clear", BindingFlags.Instance | BindingFlags.Public);
							if (clearMethod != null)
							{
								clearMethod.Invoke(innerCache, null);
								return;
							}
						}
					}
				}
			}
			throw new InvalidOperationException("Unable to clear memory cache instance of type " + cache.GetType().FullName);
		}

		// 04/30/2023 Paul.  Use reflection to get the keys from Cache. 
		// https://stackoverflow.com/questions/45597057/how-to-retrieve-a-list-of-memory-cache-keys-in-asp-net-core
		private List<string> GetCacheKeys()
		{
			var field = typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);
			var collection = field.GetValue(Cache) as ICollection;
			var items = new List<string>();
			if (collection != null)
			{
				foreach (var item in collection)
				{
					var methodInfo = item.GetType().GetProperty("Key");
					var val = methodInfo.GetValue(item);
					items.Add(val.ToString());
				}
			}
			return items;
		}

		// 08/20/2008 Paul.  Provide a central location to clear cache values based on a table change. 
		public void ClearTable(string sTABLE_NAME)
		{
			switch ( sTABLE_NAME )
			{
				// Cached Data. 
				case "CONTRACT_TYPES"           :  Cache.Remove("vwCONTRACT_TYPES_LISTBOX"               );  break;
				case "CURRENCIES"               :  Cache.Remove("vwCURRENCIES_LISTBOX"                   );  break;
				case "FORUM_TOPICS"             :  Cache.Remove("vwFORUM_TOPICS_LISTBOX"                 );  break;
				case "FORUMS"                   :  break;
				case "INBOUND_EMAILS"           :  this.ClearInboundEmails()                              ;  break;
				case "MANUFACTURERS"            :  Cache.Remove("vwMANUFACTURERS_LISTBOX"                );  break;
				case "PRODUCT_CATEGORIES"       :  Cache.Remove("vwPRODUCT_CATEGORIES_LISTBOX"           );  break;
				case "PRODUCT_TYPES"            :  Cache.Remove("vwPRODUCT_TYPES_LISTBOX"                );  break;
				case "RELEASES"                 :  Cache.Remove("vwRELEASES_LISTBOX"                     );  break;
				// 08/13/2010 Paul.  Add discounts to line items. 
				case "DISCOUNTS"                :  this.ClearDiscounts()                                  ;  break;
				case "SHIPPERS"                 :  Cache.Remove("vwSHIPPERS_LISTBOX"                     );  break;
				// 12/21/2010 Paul.  Allow regions to be used in a list. 
				case "REGIONS"                  :  Cache.Remove("vwREGIONS"                              );  break;
				case "TAX_RATES"                :  this.ClearTaxRates()                                   ;  break;
				// 11/18/2008 Paul.  Teams can be used in the search panels. 
				case "TEAMS"                    :  this.ClearTeams()                                      ;  break;
				case "USERS"                    :  this.ClearUsers()                                      ;  break;
				// Cached System Tables. 
				case "ACL_ACTIONS"              :  break;
				case "ACL_ROLES"                :  break;
				case "ACL_ROLES_ACTIONS"        :  break;
				case "ACL_ROLES_USERS"          :  break;
				case "CONFIG"                   :  break;
				case "CUSTOM_FIELDS"            :  break;
				case "DETAILVIEWS"              :  break;
				case "DETAILVIEWS_FIELDS"       :  this.ClearSet("vwDETAILVIEWS_FIELDS."        );  break;
				case "DETAILVIEWS_RELATIONSHIPS":  this.ClearSet("vwDETAILVIEWS_RELATIONSHIPS." );  break;
				// 04/19/20910 Paul.  Add separate table for EditView Relationships. 
				case "EDITVIEWS_RELATIONSHIPS"  :  this.ClearSet("vwEDITVIEWS_RELATIONSHIPS."   );  break;
				case "DYNAMIC_BUTTONS"          :  this.ClearSet("vwDYNAMIC_BUTTONS."           );  break;
				case "EDITVIEWS"                :  break;
				case "EDITVIEWS_FIELDS"         :  this.ClearSet("vwEDITVIEWS_FIELDS."          );  break;
				case "FIELDS_META_DATA"         :
					this.ClearSet("vwFIELDS_META_DATA_Validated."  );
					this.ClearSet("vwFIELDS_META_DATA_Unvalidated.");
					this.ClearSet("vwSqlColumns_Reporting."        );
					this.ClearSet("vwSqlColumns_Workflow."         );
					this.ClearSet("vwSqlColumns_Searching."        );
					break;
				case "GRIDVIEWS"                :  break;
				case "GRIDVIEWS_COLUMNS"        :  this.ClearSet("vwGRIDVIEWS_COLUMNS."         );  break;
				case "LANGUAGES"                :  this.ClearLanguages()                         ;  break;
				case "MODULES"                  :
				{
					Cache.Remove("vwMODULES");
					Cache.Remove("vwCUSTOM_EDIT_MODULES");
					foreach(string sKey in GetCacheKeys())
					{
						// 11/02/2009 Paul.  We will need a list of modules to manage offline clients. 
						// 06/02/2016 Paul.  Stream security. 
						// 06/02/2016 Paul.  Remove . in front of vwMODULES_Access_ByUser_.  It is not language dependent. 
						// 05/26/2019 Paul.  Clear React client data.
						if ( sKey.StartsWith("vwMODULES.") || sKey.Contains(".vwMODULES_Reporting_") || sKey.Contains(".vwMODULES_Import_") || sKey.EndsWith(".vwMODULES_Workflow") || sKey.Contains("vwMODULES_Access_ByUser_") || sKey.Contains("vwMODULES_Stream_ByUser_") )
							Cache.Remove(sKey);
					}
					// 10/24/2009 Paul.  Clear the newly cached item module item. 
					Cache.Remove("vwMODULES_Popup");
					// 10/24/2009 Paul.  We still can't use the standard page caching otherwise we risk getting an unauthenticated page cached, which would prevent all popups. 
					//HttpResponse.RemoveOutputCacheItem("~/Include/javascript/ModulePopupScripts.aspx");
					break;
				}
				case "RELATIONSHIPS"            :
				{
					Cache.Remove("vwRELATIONSHIPS_Reporting");
					foreach(string sKey in GetCacheKeys())
					{
						if ( sKey.EndsWith(".vwRELATIONSHIPS_Workflow") )
							Cache.Remove(sKey);
					}
						break;
				}
				case "SHORTCUTS"                :  break;
				case "TERMINOLOGY"              :  this.ClearTerminologyPickLists(); break;
				case "TERMINOLOGY_ALIASES"      :  break;
				case "TIMEZONES"                :  Cache.Remove("vwTIMEZONES");  Cache.Remove("vwTIMEZONES_LISTBOX");  break;
				// 05/01/2020 Paul.  Cache EmailTemplates for use in React Client. 
				case "EMAIL_TEMPLATES"          :  this.ClearEmailTemplates();  break;
			}
		}

		public void ClearSet(string sName)
		{
			foreach(string sKey in GetCacheKeys())
			{
				if ( sKey.StartsWith(sName) )
					Cache.Remove(sKey);
			}
		}

		public string CustomList(string sCacheName, string sValue, ref bool bCustomCache)
		{
			string sDisplayName = String.Empty;
			bCustomCache = false;
			// 10/04/2015 Paul.  Changed custom caches to a dynamic list. 
			List<SplendidCacheReference> arrCustomCaches = this.CustomCaches;
			foreach ( SplendidCacheReference cache in arrCustomCaches )
			{
				if ( cache.Name == sCacheName )
				{
					SplendidCacheCallback cbkDataSource = cache.DataSource;
					DataView vwList = new DataView(cbkDataSource());
					vwList.RowFilter = cache.DataValueField + " = '" + Sql.EscapeSQL(sValue) + "'";
					if ( vwList.Count > 0 )
						sDisplayName = Sql.ToString(vwList[0][cache.DataTextField]);
					bCustomCache = true;
					break;
				}
			}
			return sDisplayName;
		}

		// 06/27/2018 Paul.  csv and custom list requires exception. 
		public string CustomListValues(string sCacheName, string[] arrValues)
		{
			StringBuilder sb = new StringBuilder();
			List<SplendidCacheReference> arrCustomCaches = this.CustomCaches;
			foreach ( SplendidCacheReference cache in arrCustomCaches )
			{
				if ( cache.Name == sCacheName )
				{
					SplendidCacheCallback cbkDataSource = cache.DataSource;
					DataView vwList = new DataView(cbkDataSource());
					foreach ( string sValue in arrValues )
					{
						if ( sb.Length > 0 )
							sb.Append(", ");
						vwList.RowFilter = cache.DataValueField + " = '" + Sql.EscapeSQL(sValue) + "'";
						if ( vwList.Count > 0 )
							sb.Append(Sql.ToString(vwList[0][cache.DataTextField]));
						else
							sb.Append(sValue);
					}
				}
			}
			return sb.ToString();
		}

		public DateTime DefaultCacheExpiration()
		{
#if DEBUG
			return DateTime.Now.AddSeconds(1);
#else
			return DateTime.Now.AddDays(1);
#endif
		}

		public DateTime CacheExpiration5Minutes()
		{
#if DEBUG
			return DateTime.Now.AddSeconds(1);
#else
			return DateTime.Now.AddMinutes(5);
#endif
		}

		public void ClearList(string sLanguage, string sListName)
		{
			Cache.Remove(sLanguage + "." + sListName);
		}

		public DataTable List(string sListName)
		{
			DataTable dt = Cache.Get(L10n.NAME + "." + sListName) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 10/13/2005 Paul.  Use distinct because the same list appears to be duplicated in various modules. 
						// appointment_filter_dom is in an Activities and a History module.
						// ORDER BY items must appear in the select list if SELECT DISTINCT is specified. 
						// 05/20/2016 Paul.  Oracle does not allow distinct on NCLOB. 
						sSQL = "select " +  (!Sql.IsOracle(con) ? "distinct" : String.Empty) + ControlChars.CrLf
						     + "       NAME                  " + ControlChars.CrLf
						     + "     , DISPLAY_NAME          " + ControlChars.CrLf
						     + "     , LIST_ORDER            " + ControlChars.CrLf
						     + "  from vwTERMINOLOGY         " + ControlChars.CrLf
						     + " where lower(LIST_NAME) = @LIST_NAME" + ControlChars.CrLf  // 03/06/2006 Paul.  Oracle is case sensitive, and we modify the case of L10n.NAME to be lower. 
						     + "   and lower(LANG     ) = @LANG     " + ControlChars.CrLf  // 03/06/2006 Paul.  Oracle is case sensitive, and we modify the case of L10n.NAME to be lower. 
						     + " order by LIST_ORDER         " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							// 03/06/2006 Paul.  Oracle is case sensitive, and we modify the case of L10n.NAME to be lower. 
							Sql.AddParameter(cmd, "@LIST_NAME", sListName.ToLower());
							Sql.AddParameter(cmd, "@LANG"     , L10n.NAME.ToLower());
							
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set(L10n.NAME + "." + sListName, dt, DefaultCacheExpiration());
							}
						}
						// 12/03/2005 Paul.  Most lists require data, so if the language-specific list does not exist, just use English. 
						if ( dt.Rows.Count == 0 )
						{
							if ( String.Compare(L10n.NAME, "en-US", true) != 0 )
							{
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.CommandText = sSQL;
									// 03/06/2006 Paul.  Oracle is case sensitive, and we modify the case of L10n.NAME to be lower. 
									Sql.AddParameter(cmd, "@LIST_NAME", sListName.ToLower());
									Sql.AddParameter(cmd, "@LANG"     , "en-US"  .ToLower());
							
									using ( DbDataAdapter da = dbf.CreateDataAdapter() )
									{
										((IDbDataAdapter)da).SelectCommand = cmd;
										dt = new DataTable();
										da.Fill(dt);
										Cache.Set(L10n.NAME + "." + sListName, dt, DefaultCacheExpiration());
									}
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul.  Ignore list errors. 
					// 03/30/2006 Paul.  IBM DB2 is returning an error, which is causing a data-binding error. 
					// SQL1585N A system temporary table space with sufficient page size does not exist. 
					// 03/30/2006 Paul.  In case of error, we should return NULL. 
					return null;
				}
			}
			return dt;
		}

		public void ClearUsers()
		{
			Cache.Remove("vwUSERS_ASSIGNED_TO");
			Cache.Remove("vwUSERS_List");
			Cache.Remove("vwUSERS_Groups");
			// 05/26/2019 Paul.  Clear the React client data. 
			Cache.Remove("vwUSERS.ReactClient");
		}

		// 05/26/2019 Paul.  Clear the React client data. 
		public void ClearTeams()
		{
			Cache.Remove("vwTEAMS");
			Cache.Remove("vwTEAMS.ReactClient");
		}

		// 01/18/2007 Paul.  If AssignedUser list, then use the cached value to find the value. 
		public string AssignedUser(Guid gID)
		{
			string sUSER_NAME = String.Empty;
			if ( !Sql.IsEmptyGuid(gID) )
			{
				DataView vwAssignedUser = new DataView(this.AssignedUser());
				vwAssignedUser.RowFilter = "ID = '" + gID.ToString() + "'";
				if ( vwAssignedUser.Count > 0 )
				{
					sUSER_NAME = Sql.ToString(vwAssignedUser[0]["USER_NAME"]);
				}
			}
			return sUSER_NAME;
		}

		public DataTable AssignedUser()
		{
			// 04/15/2008 Paul.  When Team Management is enabled, only show users that are in this users teams. 
			bool bTeamFilter = !Security.IS_ADMIN && Config.enable_team_management();
			string sCACHE_NAME = String.Empty;
			if ( bTeamFilter )
				sCACHE_NAME = "vwTEAMS_ASSIGNED_TO." + Security.USER_ID.ToString();
			else
				sCACHE_NAME = "vwUSERS_ASSIGNED_TO";
			DataTable dt = Cache.Get(sCACHE_NAME) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						if ( bTeamFilter )
						{
							sSQL = "select ID                 " + ControlChars.CrLf
							     + "     , USER_NAME          " + ControlChars.CrLf
							     + "  from vwTEAMS_ASSIGNED_TO" + ControlChars.CrLf
							     + " where MEMBERSHIP_USER_ID = @MEMBERSHIP_USER_ID" + ControlChars.CrLf
							     + " order by USER_NAME       " + ControlChars.CrLf;
						}
						else
						{
							sSQL = "select ID                 " + ControlChars.CrLf
							     + "     , USER_NAME          " + ControlChars.CrLf
							     + "  from vwUSERS_ASSIGNED_TO" + ControlChars.CrLf
							     + " order by USER_NAME       " + ControlChars.CrLf;
						}
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							if ( bTeamFilter )
								Sql.AddParameter(cmd, "@MEMBERSHIP_USER_ID", Security.USER_ID);
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set(sCACHE_NAME, dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		public DataTable CustomEditModules()
		{
			DataTable dt = Cache.Get("vwCUSTOM_EDIT_MODULES") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select NAME                 " + ControlChars.CrLf
						     + "     , NAME as DISPLAY_NAME " + ControlChars.CrLf
						     + "  from vwCUSTOM_EDIT_MODULES" + ControlChars.CrLf
						     + " order by NAME              " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwCUSTOM_EDIT_MODULES", dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		// 11/02/2009 Paul.  We will need a list of modules to manage offline clients. 
		public List<String> AccessibleModules(Guid gUSER_ID)
		{
			List<String> arr = Cache.Get("vwMODULES_Access_ByUser_" + gUSER_ID.ToString()) as List<String>;
			if ( arr == null )
			{
				arr = new List<String>();
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select MODULE_NAME             " + ControlChars.CrLf
						     + "  from vwMODULES_Access_ByUser " + ControlChars.CrLf
						     + " where USER_ID = @USER_ID      " + ControlChars.CrLf
						     + " order by MODULE_NAME          " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@USER_ID", gUSER_ID);
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								using ( DataTable dt = new DataTable() )
								{
									da.Fill(dt);
									if ( dt.Rows.Count > 0 )
									{
										// 11/08/2009 Paul.  We need a list that can grow dynamically as some rows will be ignored. 
										for ( int i = 0; i < dt.Rows.Count; i++ )
										{
											DataRow row = dt.Rows[i];
											string sMODULE_NAME = Sql.ToString(row["MODULE_NAME"]);
											// 11/06/2009 Paul.  We need a fast way to disable modules that do not exist on the Offline Client. 
											if ( Sql.ToBoolean(Application["Modules." + sMODULE_NAME + ".Exists"]) )
											{
												arr.Add(sMODULE_NAME);
											}
											else
											{
												Debug.WriteLine(sMODULE_NAME + " does not exist");
											}
										}
										Cache.Set("vwMODULES_Access_ByUser_" + gUSER_ID.ToString(), arr, DefaultCacheExpiration());
									}
									else
									{
										// 11/08/2009 Paul.  If this is the first time an offline client user logs-in, there will be 
										// no user record and no accessible modules.  We need a default set, so requery. 
										sSQL = "select MODULE_NAME             " + ControlChars.CrLf
										     + "  from vwMODULES               " + ControlChars.CrLf
										     + " order by MODULE_NAME          " + ControlChars.CrLf;
										cmd.CommandText = sSQL;
										cmd.Parameters.Clear();
										da.Fill(dt);
										
										for ( int i = 0; i < dt.Rows.Count; i++ )
										{
											DataRow row = dt.Rows[i];
											string sMODULE_NAME = Sql.ToString(row["MODULE_NAME"]);
											// 11/06/2009 Paul.  We need a fast way to disable modules that do not exist on the Offline Client. 
											if ( Sql.ToBoolean(Application["Modules." + sMODULE_NAME + ".Exists"]) )
											{
												arr.Add(sMODULE_NAME);
											}
										}
										// 11/08/2009 Paul.  If we are using the default set of modules, then don't cache the results. 
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
			}
			return arr;
		}

		// 01/24/2018 Paul.  The Calendar needs to determine if Calls module is enabled. 
		public DataTable AccessibleModulesTable(Guid gUSER_ID)
		{
			DataTable dt = Cache.Get("vwMODULES_AccessTable_ByUser_" + gUSER_ID.ToString()) as DataTable;
			if ( dt == null )
			{
				dt = new DataTable();
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select MODULE_NAME             " + ControlChars.CrLf
						     + "     , TABLE_NAME              " + ControlChars.CrLf
						     + "     , DISPLAY_NAME            " + ControlChars.CrLf
						     + "     , RELATIVE_PATH           " + ControlChars.CrLf
						     + "  from vwMODULES_Access_ByUser " + ControlChars.CrLf
						     + " where USER_ID = @USER_ID      " + ControlChars.CrLf
						     + " order by MODULE_NAME          " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@USER_ID", gUSER_ID);
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								da.Fill(dt);
								if ( dt.Rows.Count > 0 )
								{
									// 11/08/2009 Paul.  We need a list that can grow dynamically as some rows will be ignored. 
									for ( int i = 0; i < dt.Rows.Count; i++ )
									{
										DataRow row = dt.Rows[i];
										string sMODULE_NAME = Sql.ToString(row["MODULE_NAME"]);
										// 11/06/2009 Paul.  We need a fast way to disable modules that do not exist on the Offline Client. 
										if ( !Sql.ToBoolean(Application["Modules." + sMODULE_NAME + ".Exists"]) )
										{
											row.Delete();
										}
									}
									dt.AcceptChanges();
									Cache.Set("vwMODULES_AccessTable_ByUser_" + gUSER_ID.ToString(), dt, DefaultCacheExpiration());
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemMessage("Error", new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		public DataTable ReportingModules()
		{
			// 08/06/2008 Paul.  Module names are returned translated, so make sure to cache based on the language. 
			DataTable dt = Cache.Get(L10n.NAME + ".vwMODULES_Reporting_" + Security.USER_ID.ToString()) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 12/06/2009 Paul.  We need the ID and TABLE_NAME when generating the SemanticModel for the ReportBuilder. 
						// 07/23/2010 Paul.  Make sure to sort the Modules table. 
						sSQL = "select ID                      " + ControlChars.CrLf
						     + "     , TABLE_NAME              " + ControlChars.CrLf
						     + "     , MODULE_NAME             " + ControlChars.CrLf
						     + "     , DISPLAY_NAME            " + ControlChars.CrLf
						     + "  from vwMODULES_Reporting     " + ControlChars.CrLf
						     + " where USER_ID = @USER_ID      " + ControlChars.CrLf
						     + "    or USER_ID is null         " + ControlChars.CrLf
						     + " order by DISPLAY_NAME         " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@USER_ID", Security.USER_ID);
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								foreach(DataRow row in dt.Rows)
								{
									row["DISPLAY_NAME"] = L10n.Term(Sql.ToString(row["DISPLAY_NAME"]));
								}
								Cache.Set(L10n.NAME + ".vwMODULES_Reporting_" + Security.USER_ID.ToString(), dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		// 11/10/2010 Paul.  vwMODULES_Rules is nearly identical to vwMODULES_Reporting. 
		public DataTable RulesModules()
		{
			// 08/06/2008 Paul.  Module names are returned translated, so make sure to cache based on the language. 
			DataTable dt = Cache.Get(L10n.NAME + ".vwMODULES_Rules_" + Security.USER_ID.ToString()) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 12/06/2009 Paul.  We need the ID and TABLE_NAME when generating the SemanticModel for the ReportBuilder. 
						// 07/23/2010 Paul.  Make sure to sort the Modules table. 
						sSQL = "select ID                      " + ControlChars.CrLf
						     + "     , TABLE_NAME              " + ControlChars.CrLf
						     + "     , MODULE_NAME             " + ControlChars.CrLf
						     + "     , DISPLAY_NAME            " + ControlChars.CrLf
						     + "  from vwMODULES_Rules         " + ControlChars.CrLf
						     + " where USER_ID = @USER_ID      " + ControlChars.CrLf
						     + "    or USER_ID is null         " + ControlChars.CrLf
						     + " order by DISPLAY_NAME         " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@USER_ID", Security.USER_ID);
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								foreach(DataRow row in dt.Rows)
								{
									row["DISPLAY_NAME"] = L10n.Term(Sql.ToString(row["DISPLAY_NAME"]));
								}
								Cache.Set(L10n.NAME + ".vwMODULES_Rules_" + Security.USER_ID.ToString(), dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		public DataTable WorkflowModules()
		{
			// 08/06/2008 Paul.  Module names are returned translated, so make sure to cache based on the language. 
			DataTable dt = Cache.Get(L10n.NAME + ".vwMODULES_Workflow") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select MODULE_NAME             " + ControlChars.CrLf
						     + "     , DISPLAY_NAME            " + ControlChars.CrLf
						     + "  from vwMODULES_Workflow      " + ControlChars.CrLf
						     + " order by MODULE_NAME          " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								foreach(DataRow row in dt.Rows)
								{
									row["DISPLAY_NAME"] = L10n.Term(Sql.ToString(row["DISPLAY_NAME"]));
								}
								Cache.Set(L10n.NAME + ".vwMODULES_Workflow", dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		public DataTable WorkflowRelationships()
		{
			// 08/06/2008 Paul.  Module names are returned translated, so make sure to cache based on the language. 
			DataTable dt = Cache.Get(L10n.NAME + ".vwRELATIONSHIPS_Workflow") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select *                        " + ControlChars.CrLf
						     + "  from vwRELATIONSHIPS_Workflow" + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								foreach(DataRow row in dt.Rows)
								{
									row["DISPLAY_NAME"] = L10n.Term(Sql.ToString(row["DISPLAY_NAME"]));
								}
								Cache.Set(L10n.NAME + ".vwRELATIONSHIPS_Workflow", dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		// 07/01/2019 Paul.  The SubPanelsView needs to understand how to manage all relationships. 
		public Dictionary<string, object> GetAllRelationships()
		{
			Dictionary<string, object> objs = Cache.Get("vwRELATIONSHIPS.ReactClient") as Dictionary<string, object>;
			if ( objs == null )
			{
				objs = new Dictionary<string, object>();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL = String.Empty;
							sSQL = "select *                                           " + ControlChars.CrLf
							     + "  from vwRELATIONSHIPS                             " + ControlChars.CrLf
							     + " order by LHS_MODULE, RHS_MODULE, RELATIONSHIP_TYPE" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
							
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dt = new DataTable() )
									{
										da.Fill(dt);
										for ( int i = 0; i < dt.Rows.Count; i++ )
										{
											DataRow row = dt.Rows[i];
											string sRELATIONSHIP_NAME              = Sql.ToString (row["RELATIONSHIP_NAME"             ]);
											string sLHS_MODULE                     = Sql.ToString (row["LHS_MODULE"                    ]);
											string sLHS_TABLE                      = Sql.ToString (row["LHS_TABLE"                     ]).ToUpper();
											string sLHS_KEY                        = Sql.ToString (row["LHS_KEY"                       ]).ToUpper();
											string sRHS_MODULE                     = Sql.ToString (row["RHS_MODULE"                    ]);
											string sRHS_TABLE                      = Sql.ToString (row["RHS_TABLE"                     ]).ToUpper();
											string sRHS_KEY                        = Sql.ToString (row["RHS_KEY"                       ]).ToUpper();
											string sJOIN_TABLE                     = Sql.ToString (row["JOIN_TABLE"                    ]).ToUpper();
											string sJOIN_KEY_LHS                   = Sql.ToString (row["JOIN_KEY_LHS"                  ]).ToUpper();
											string sJOIN_KEY_RHS                   = Sql.ToString (row["JOIN_KEY_RHS"                  ]).ToUpper();
											string sRELATIONSHIP_TYPE              = Sql.ToString (row["RELATIONSHIP_TYPE"             ]);
											string sRELATIONSHIP_ROLE_COLUMN       = Sql.ToString (row["RELATIONSHIP_ROLE_COLUMN"      ]).ToUpper();
											string sRELATIONSHIP_ROLE_COLUMN_VALUE = Sql.ToString (row["RELATIONSHIP_ROLE_COLUMN_VALUE"]).ToUpper();
											bool   bREVERSE                        = Sql.ToBoolean(row["REVERSE"                       ]);
											
											Dictionary<string, object> drow = new Dictionary<string, object>();
											drow.Add("RELATIONSHIP_NAME"             , sRELATIONSHIP_NAME             );
											drow.Add("LHS_MODULE"                    , sLHS_MODULE                    );
											drow.Add("LHS_TABLE"                     , sLHS_TABLE                     );
											drow.Add("LHS_KEY"                       , sLHS_KEY                       );
											drow.Add("RHS_MODULE"                    , sRHS_MODULE                    );
											drow.Add("RHS_TABLE"                     , sRHS_TABLE                     );
											drow.Add("RHS_KEY"                       , sRHS_KEY                       );
											drow.Add("JOIN_TABLE"                    , sJOIN_TABLE                    );
											drow.Add("JOIN_KEY_LHS"                  , sJOIN_KEY_LHS                  );
											drow.Add("JOIN_KEY_RHS"                  , sJOIN_KEY_RHS                  );
											drow.Add("RELATIONSHIP_TYPE"             , sRELATIONSHIP_TYPE             );
											drow.Add("RELATIONSHIP_ROLE_COLUMN"      , sRELATIONSHIP_ROLE_COLUMN      );
											drow.Add("RELATIONSHIP_ROLE_COLUMN_VALUE", sRELATIONSHIP_ROLE_COLUMN_VALUE);
											drow.Add("REVERSE"                       , bREVERSE                       );
											
											List<object> lhs = null;
											if ( objs.ContainsKey(sLHS_MODULE) )
											{
												lhs = objs[sLHS_MODULE] as List<object>;
											}
											else
											{
												lhs = new List<object>();
												objs.Add(sLHS_MODULE, lhs);
											}
											lhs.Add(drow);
											List<object> rhs = null;
											if ( objs.ContainsKey(sRHS_MODULE) )
											{
												rhs = objs[sRHS_MODULE] as List<object>;
											}
											else
											{
												rhs = new List<object>();
												objs.Add(sRHS_MODULE, rhs);
											}
											rhs.Add(drow);
										}
										Cache.Set("vwRELATIONSHIPS.ReactClient", objs, DefaultCacheExpiration());
									}
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					throw;
				}
			}
			return objs;
		}

		public DataTable ImportModules()
		{
			// 08/06/2008 Paul.  Module names are returned translated, so make sure to cache based on the language. 
			DataTable dt = Cache.Get(L10n.NAME + ".vwMODULES_Import_" + Security.USER_ID.ToString()) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select MODULE_NAME             " + ControlChars.CrLf
						     + "     , DISPLAY_NAME            " + ControlChars.CrLf
						     + "  from vwMODULES_Import        " + ControlChars.CrLf
						     + " where USER_ID = @USER_ID      " + ControlChars.CrLf
						     + "    or USER_ID is null         " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@USER_ID", Security.USER_ID);
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								foreach(DataRow row in dt.Rows)
								{
									row["DISPLAY_NAME"] = L10n.Term(Sql.ToString(row["DISPLAY_NAME"]));
								}
								Cache.Set(L10n.NAME + ".vwMODULES_Import_" + Security.USER_ID.ToString(), dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		public string[] ReportingModulesList()
		{
			// 07/23/2010 Paul.  Make sure to sort the Modules table. 
			DataView vw = new DataView(this.ReportingModules());
			vw.Sort = "DISPLAY_NAME";
			string[] arr = new string[vw.Count];
			for ( int i = 0; i < vw.Count; i++ )
			{
				arr[i] = Sql.ToString(vw[i]["MODULE_NAME"]);
			}
			return arr;
		}

		public DataTable ReportingRelationships()
		{
			DataTable dt = Cache.Get("vwRELATIONSHIPS_Reporting") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select *                        " + ControlChars.CrLf
						     + "  from vwRELATIONSHIPS_Reporting" + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwRELATIONSHIPS_Reporting", dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					// 10/20/2009 Paul.  Make sure to pass the Application as this function can be called in a background task. 
					SplendidError.SystemMessage("Error", new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		public void ClearFilterColumns(string sMODULE_NAME)
		{
			Cache.Remove("vwSqlColumns_Reporting." + sMODULE_NAME);
			Cache.Remove("vwSqlColumns_Workflow."  + sMODULE_NAME);
		}

		// 10/20/2009 Paul.  We need to allow the ReportingFilterColumns to be called from a background task. 
		public DataTable ReportingFilterColumns(string sMODULE_NAME)
		{
			DataTable dt = Cache.Get("vwSqlColumns_Reporting." + sMODULE_NAME) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 02/29/2008 Niall.  Some SQL Server 2005 installations require matching case for the parameters. 
						// Since we force the parameter to be uppercase, we must also make it uppercase in the command text. 
						sSQL = "select *                       " + ControlChars.CrLf
						     + "  from vwSqlColumns_Reporting  " + ControlChars.CrLf
						     + " where ObjectName = @OBJECTNAME" + ControlChars.CrLf
						     + " order by colid                " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							// 09/02/2008 Paul.  Standardize the case of metadata tables to uppercase.  PostgreSQL defaults to lowercase. 
							Sql.AddParameter(cmd, "@OBJECTNAME", Sql.MetadataName(cmd, "vw" + sMODULE_NAME));
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								// 06/10/2006 Paul.  The default sliding scale is not appropriate as columns can be added. 
								Cache.Set("vwSqlColumns_Reporting." + sMODULE_NAME, dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					// 10/20/2009 Paul.  Make sure to pass the Application as this function can be called in a background task. 
					SplendidError.SystemMessage("Error", new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		// 09/30/2018 Paul.  Add survey record creation to survey. 
		public DataTable SurveyTargetColumns(string sMODULE_NAME)
		{
			DataTable dt = Cache.Get("vwSqlColumns_SurveyTarget." + sMODULE_NAME) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select *                        " + ControlChars.CrLf
						     + "  from vwSqlColumns_SurveyTarget" + ControlChars.CrLf
						     + " where ObjectName = @OBJECTNAME " + ControlChars.CrLf
						     + " order by colid                 " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@OBJECTNAME", Sql.MetadataName(cmd, "sp" + CrmModules.TableName(sMODULE_NAME) + "_Update"));
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwSqlColumns_SurveyTarget." + sMODULE_NAME, dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					// 10/20/2009 Paul.  Make sure to pass the Application as this function can be called in a background task. 
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		// 06/03/2009 Paul.  This function can be call from the workflow engine, so we need to pass in the application. 
		// 07/23/2008 Paul.  Use a separate view for workflow so that we can filter the audting fields. 
		public DataTable WorkflowFilterColumns(string sMODULE_NAME)
		{
			DataTable dt = Cache.Get("vwSqlColumns_Workflow." + sMODULE_NAME) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 02/29/2008 Niall.  Some SQL Server 2005 installations require matching case for the parameters. 
						// Since we force the parameter to be uppercase, we must also make it uppercase in the command text. 
						sSQL = "select *                       " + ControlChars.CrLf
						     + "  from vwSqlColumns_Workflow   " + ControlChars.CrLf
						     + " where ObjectName = @OBJECTNAME" + ControlChars.CrLf
						     + " order by colid                " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							// 07/22/2008 Paul.  There are no views for the audit tables. 
							// 09/02/2008 Paul.  Standardize the case of metadata tables to uppercase.  PostgreSQL defaults to lowercase. 
							// 11/26/2008 Paul.  We now have audit views. 
							Sql.AddParameter(cmd, "@OBJECTNAME", Sql.MetadataName(cmd, "vw" + sMODULE_NAME));
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								// 06/10/2006 Paul.  The default sliding scale is not appropriate as columns can be added. 
								Cache.Set("vwSqlColumns_Workflow." + sMODULE_NAME, dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					// 10/20/2009 Paul.  Make sure to pass the Application as this function can be called in a background task. 
					SplendidError.SystemMessage("Error", new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		public DataTable WorkflowFilterUpdateColumns(string sMODULE_NAME)
		{
			DataTable dt = Cache.Get("vwSqlColumns_WorkflowUpdate." + sMODULE_NAME) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 02/29/2008 Niall.  Some SQL Server 2005 installations require matching case for the parameters. 
						// Since we force the parameter to be uppercase, we must also make it uppercase in the command text. 
						// 02/18/2009 Paul.  Include the custom fields in the list of workflow columns that can be updated. 
						// 02/18/2009 Paul.  We need to know if the column is an identity so the workflow engine can avoid updating it.
						sSQL = "select ObjectName                                      " + ControlChars.CrLf
						     + "     , ColumnName                                      " + ControlChars.CrLf
						     + "     , ColumnType                                      " + ControlChars.CrLf
						     + "     , NAME                                            " + ControlChars.CrLf
						     + "     , DISPLAY_NAME                                    " + ControlChars.CrLf
						     + "     , SqlDbType                                       " + ControlChars.CrLf
						     + "     , CsType                                          " + ControlChars.CrLf
						     + "     , colid                                           " + ControlChars.CrLf
						     + "     , IsIdentity                                      " + ControlChars.CrLf
						     + "  from vwSqlColumns_WorkflowUpdate                     " + ControlChars.CrLf
						     + " where ObjectName = @OBJECTNAME                        " + ControlChars.CrLf
						     + "union all                                              " + ControlChars.CrLf
						     + "select ObjectName                                      " + ControlChars.CrLf
						     + "     , ColumnName                                      " + ControlChars.CrLf
						     + "     , ColumnType                                      " + ControlChars.CrLf
						     + "     , vwSqlColumns_WorkflowUpdate.NAME                " + ControlChars.CrLf
						     + "     , DISPLAY_NAME                                    " + ControlChars.CrLf
						     + "     , SqlDbType                                       " + ControlChars.CrLf
						     + "     , vwSqlColumns_WorkflowUpdate.CsType              " + ControlChars.CrLf
						     + "     , vwSqlColumns_WorkflowUpdate.colid + 100 as colid" + ControlChars.CrLf
						     + "     , vwSqlColumns_WorkflowUpdate.IsIdentity          " + ControlChars.CrLf
						     + "  from      vwFIELDS_META_DATA_Validated               " + ControlChars.CrLf
						     + " inner join vwSqlColumns_WorkflowUpdate                " + ControlChars.CrLf
						     + "         on vwSqlColumns_WorkflowUpdate.ObjectName = vwFIELDS_META_DATA_Validated.TABLE_NAME + '_CSTM'" + ControlChars.CrLf
						     + "        and vwSqlColumns_WorkflowUpdate.NAME       = vwFIELDS_META_DATA_Validated.NAME" + ControlChars.CrLf
						     + " where vwFIELDS_META_DATA_Validated.MODULE_NAME = @MODULE_NAME" + ControlChars.CrLf
						     + " order by colid                                        " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							// 08/13/2008 Paul.  When updating inside the workflow engine, we can only update the core fields at this time. 
							// So make sure to match the fields with the update procedure. 
							// 09/02/2008 Paul.  Standardize the case of metadata tables to uppercase.  PostgreSQL defaults to lowercase. 
							Sql.AddParameter(cmd, "@OBJECTNAME", Sql.MetadataName(cmd, "sp" + sMODULE_NAME + "_Update"));
							Sql.AddParameter(cmd, "@MODULE_NAME", sMODULE_NAME);
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								// 06/10/2006 Paul.  The default sliding scale is not appropriate as columns can be added. 
								Cache.Set("vwSqlColumns_WorkflowUpdate." + sMODULE_NAME, dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		// 06/03/2009 Paul.  This function can be call from the workflow engine, so we need to pass in the application. 
		public string ReportingFilterColumnsListName(string sMODULE_NAME, string sDATA_FIELD)
		{
			string sLIST_NAME = Cache.Get("vwSqlColumns_ListName." + sMODULE_NAME + "." + sDATA_FIELD) as string;
			if ( sLIST_NAME == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 02/29/2008 Niall.  Some SQL Server 2005 installations require matching case for the parameters. 
						// Since we force the parameter to be uppercase, we must also make it uppercase in the command text. 
						sSQL = "select LIST_NAME               " + ControlChars.CrLf
						     + "  from vwSqlColumns_ListName   " + ControlChars.CrLf
						     + " where ObjectName = @OBJECTNAME" + ControlChars.CrLf
						     + "   and DATA_FIELD = @DATA_FIELD" + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							// 09/02/2008 Paul.  Standardize the case of metadata tables to uppercase.  PostgreSQL defaults to lowercase. 
							Sql.AddParameter(cmd, "@OBJECTNAME", Sql.MetadataName(cmd, "vw" + sMODULE_NAME));
							Sql.AddParameter(cmd, "@DATA_FIELD", sDATA_FIELD);
							// 07/16/2008 Paul.  Don't need the data adapter. 
							sLIST_NAME = Sql.ToString(cmd.ExecuteScalar());
							Cache.Set("vwSqlColumns_ListName." + sMODULE_NAME + "." + sDATA_FIELD, sLIST_NAME, DefaultCacheExpiration());
						}
					}
				}
				catch(Exception ex)
				{
					// 10/20/2009 Paul.  Make sure to pass the Application as this function can be called in a background task. 
					SplendidError.SystemMessage("Error", new StackTrace(true).GetFrame(0), ex);
				}
			}
			return sLIST_NAME;
		}

		public DataTable GetAllReportingFilterColumnsListName()
		{
			DataTable dt = Cache.Get("vwSqlColumns_ListName.AllModules") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select distinct                 " + ControlChars.CrLf
						     + "       ObjectName               " + ControlChars.CrLf
						     + "     , DATA_FIELD               " + ControlChars.CrLf
						     + "     , LIST_NAME                " + ControlChars.CrLf
						     + "  from vwSqlColumns_ListName    " + ControlChars.CrLf
						     + " order by ObjectName, DATA_FIELD" + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwSqlColumns_ListName.AllModules", dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					// 10/20/2009 Paul.  Make sure to pass the Application as this function can be called in a background task. 
					SplendidError.SystemMessage("Error", new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		public DataTable ImportColumns(string sMODULE_NAME)
		{
			DataTable dt = Cache.Get("vwSqlColumns_Import." + sMODULE_NAME) as DataTable;
			if ( dt == null )
			{
				string sTABLE_NAME = Sql.ToString(Application["Modules." + sMODULE_NAME + ".TableName"]);
				if ( Sql.IsEmptyString(sTABLE_NAME) )
					sTABLE_NAME = sMODULE_NAME.ToUpper();
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						/*
						con.Open();
						string sSQL;
						// 02/29/2008 Niall.  Some SQL Server 2005 installations require matching case for the parameters. 
						// Since we force the parameter to be uppercase, we must also make it uppercase in the command text. 
						sSQL = "select *                       " + ControlChars.CrLf
						     + "  from vwSqlColumns_Import     " + ControlChars.CrLf
						     + " where ObjectName = @OBJECTNAME" + ControlChars.CrLf
						     + " order by colid                " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							// 09/02/2008 Paul.  Standardize the case of metadata tables to uppercase.  PostgreSQL defaults to lowercase. 
							Sql.AddParameter(cmd, "@OBJECTNAME", Sql.MetadataName(cmd, "sp" + sMODULE_NAME + "_Update"));
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								// 06/10/2006 Paul.  The default sliding scale is not appropriate as columns can be added. 
								Cache.Set("vwSqlColumns_Import." + sMODULE_NAME, dt, DefaultCacheExpiration());
							}
						}
						*/
						// 10/08/2006 Paul.  MySQL does not seem to have a way to provide the paramters. Use the SqlProcs.Factory to build the table. 
						dt = new DataTable();
						dt.Columns.Add("ColumnName"  , Type.GetType("System.String"));
						dt.Columns.Add("NAME"        , Type.GetType("System.String"));
						dt.Columns.Add("DISPLAY_NAME", Type.GetType("System.String"));
						dt.Columns.Add("ColumnType"  , Type.GetType("System.String"));
						dt.Columns.Add("Size"        , Type.GetType("System.Int32" ));
						dt.Columns.Add("Scale"       , Type.GetType("System.Int32" ));
						dt.Columns.Add("Precision"   , Type.GetType("System.Int32" ));
						dt.Columns.Add("colid"       , Type.GetType("System.Int32" ));
						dt.Columns.Add("CustomField" , Type.GetType("System.Boolean"));

						// 05/24/2020 Paul.  Prevent duplicate TEAM_NAME entry. 
						bool bTEAM_NAME_Found = false;
						{
							IDbCommand cmdImport = null;
							try
							{
								// 03/13/2008 Paul.  The factory will throw an exception if the procedure is not found. 
								// Catching an exception is expensive, but trivial considering all the other processing that will occur. 
								// We need this same logic in ImportView.GenerateImport. 
								cmdImport = SqlProcs.Factory(con, "sp" + sTABLE_NAME + "_Import");
							}
							catch
							{
								cmdImport = SqlProcs.Factory(con, "sp" + sTABLE_NAME + "_Update");
							}
							for ( int i =0; i < cmdImport.Parameters.Count; i++ )
							{
								IDbDataParameter par = cmdImport.Parameters[i] as IDbDataParameter;
								DataRow row = dt.NewRow();
								dt.Rows.Add(row);
								row["ColumnName"  ] = par.ParameterName;
								row["NAME"        ] = Sql.ExtractDbName(cmdImport, par.ParameterName);
								row["DISPLAY_NAME"] = row["NAME"];
								row["ColumnType"  ] = par.DbType.ToString();
								row["Size"        ] = par.Size         ;
								row["Scale"       ] = par.Scale        ;
								row["Precision"   ] = par.Precision    ;
								row["colid"       ] = i                ;
								row["CustomField" ] = false            ;
								if ( Sql.ToString(row["NAME"]) == "TEAM_NAME" )
									bTEAM_NAME_Found = true;
							}
						}

						// 09/19/2007 Paul.  Add the fields from the custom table. 
						// Exclude ID_C as it is expect and required. We don't want it to appear in the mapping table. 
						con.Open();
						string sSQL;
						// 09/19/2007 Paul.  We need to allow import into TEAM fields. 
						// 12/13/2007 Paul.  Only add the TEAM fields if the base table has a TEAM_ID field. 
						// 05/24/2020 Paul.  Prevent duplicate TEAM_NAME entry. 
						if ( Config.enable_team_management() && !bTEAM_NAME_Found )
						{
							// 02/20/2008 Paul.  We have a new way to determine if a module has a TEAM_ID. 
							// Also, the update procedure already includes TEAM_ID, so we only have to add the TEAM_NAME. 
							bool bModuleIsTeamed = Sql.ToBoolean(Application["Modules." + sMODULE_NAME + ".Teamed"]);
							DataRow row = dt.NewRow();
							row = dt.NewRow();
							row["ColumnName"  ] = "TEAM_NAME";
							row["NAME"        ] = "TEAM_NAME";
							row["DISPLAY_NAME"] = "TEAM_NAME";
							row["ColumnType"  ] = "string";
							row["Size"        ] = 128;
							row["colid"       ] = dt.Rows.Count;
							row["CustomField" ] = false;
							dt.Rows.Add(row);
						}

						// 02/29/2008 Niall.  Some SQL Server 2005 installations require matching case for the parameters. 
						// Since we force the parameter to be uppercase, we must also make it uppercase in the command text. 
						sSQL = "select *                       " + ControlChars.CrLf
						     + "  from vwSqlColumns            " + ControlChars.CrLf
						     + " where ObjectName = @OBJECTNAME" + ControlChars.CrLf
						     + "   and ColumnName <> 'ID_C'    " + ControlChars.CrLf
						     + " order by colid                " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							// 09/02/2008 Paul.  Standardize the case of metadata tables to uppercase.  PostgreSQL defaults to lowercase. 
							Sql.AddParameter(cmd, "@OBJECTNAME", Sql.MetadataName(cmd, sTABLE_NAME + "_CSTM"));
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								DataTable dtCSTM = new DataTable();
								da.Fill(dtCSTM);
								foreach ( DataRow rowCSTM in dtCSTM.Rows )
								{
									DataRow row = dt.NewRow();
									row["ColumnName"  ] = Sql.ToString (rowCSTM["ColumnName"]);
									row["NAME"        ] = Sql.ToString (rowCSTM["ColumnName"]);
									row["DISPLAY_NAME"] = Sql.ToString (rowCSTM["ColumnName"]);
									row["ColumnType"  ] = Sql.ToString (rowCSTM["CsType"    ]);
									row["Size"        ] = Sql.ToInteger(rowCSTM["length"    ]);
									// 09/19/2007 Paul.  Scale and Precision are not used. 
									//row["Scale"       ] = Sql.ToInteger(rowCSTM["Scale"     ]);
									//row["Precision"   ] = Sql.ToInteger(rowCSTM["Precision" ]);
									row["colid"       ] = dt.Rows.Count;
									row["CustomField" ] = true;
									dt.Rows.Add(row);
								}
							}
						}
						Cache.Set("vwSqlColumns_Import." + sMODULE_NAME, dt, DefaultCacheExpiration());
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		public DataTable Release()
		{
			DataTable dt = Cache.Get("vwRELEASES_LISTBOX") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select ID                " + ControlChars.CrLf
						     + "     , NAME              " + ControlChars.CrLf
						     + "  from vwRELEASES_LISTBOX" + ControlChars.CrLf
						     + " order by LIST_ORDER     " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwRELEASES_LISTBOX", dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		public DataTable ProductCategories()
		{
			DataTable dt = Cache.Get("vwPRODUCT_CATEGORIES_LISTBOX") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select ID                          " + ControlChars.CrLf
						     + "     , NAME                        " + ControlChars.CrLf
						     + "  from vwPRODUCT_CATEGORIES_LISTBOX" + ControlChars.CrLf
						     + " order by LIST_ORDER               " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwPRODUCT_CATEGORIES_LISTBOX", dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		// 05/19/2012 Paul.  ProductTypes needs to be called from the scheduler, so the application must be provided. 
		public DataTable ProductTypes()
		{
			DataTable dt = Cache.Get("vwPRODUCT_TYPES_LISTBOX") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select ID                     " + ControlChars.CrLf
						     + "     , NAME                   " + ControlChars.CrLf
						     + "  from vwPRODUCT_TYPES_LISTBOX" + ControlChars.CrLf
						     + " order by LIST_ORDER          " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwPRODUCT_TYPES_LISTBOX", dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		public DataTable Manufacturers()
		{
			DataTable dt = Cache.Get("vwMANUFACTURERS_LISTBOX") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select ID                     " + ControlChars.CrLf
						     + "     , NAME                   " + ControlChars.CrLf
						     + "  from vwMANUFACTURERS_LISTBOX" + ControlChars.CrLf
						     + " order by LIST_ORDER          " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwMANUFACTURERS_LISTBOX", dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		// 08/13/2010 Paul.  Add discounts to line items. 
		public DataTable Discounts()
		{
			DataTable dt = Cache.Get("vwDISCOUNTS_LISTBOX") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 08/15/2010 Paul.  We need all the discount fields. 
						sSQL = "select *                  " + ControlChars.CrLf
						     + "  from vwDISCOUNTS_LISTBOX" + ControlChars.CrLf
						     + " order by NAME            " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwDISCOUNTS_LISTBOX", dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		public void ClearDiscounts()
		{
			Cache.Remove("vwDISCOUNTS_LISTBOX");
			Cache.Remove("vwDISCOUNTS_LISTBOX.ReactClient");
		}

		public DataTable Shippers()
		{
			DataTable dt = Cache.Get("vwSHIPPERS_LISTBOX") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select ID                " + ControlChars.CrLf
						     + "     , NAME              " + ControlChars.CrLf
						     + "  from vwSHIPPERS_LISTBOX" + ControlChars.CrLf
						     + " order by LIST_ORDER     " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwSHIPPERS_LISTBOX", dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		// 02/15/2015 Paul.  Change from terminology payment_types_dom to PaymentTypes list for QuickBooks Online. 
		public DataTable PaymentTypes()
		{
			DataTable dt = Cache.Get("vwPAYMENT_TYPES_LISTBOX") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 02/16/2015 Paul.  In order to remain compatible with existing systems, use NAME instead of ID as the primary key. 
						sSQL = "select NAME as ID             " + ControlChars.CrLf
						     + "     , NAME                   " + ControlChars.CrLf
						     + "  from vwPAYMENT_TYPES_LISTBOX" + ControlChars.CrLf
						     + " order by LIST_ORDER          " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwPAYMENT_TYPES_LISTBOX", dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		// 02/15/2015 Paul.  Change from terminology payment_terms_dom to PaymentTerms list for QuickBooks Online. 
		public DataTable PaymentTerms()
		{
			DataTable dt = Cache.Get("vwPAYMENT_TERMS_LISTBOX") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 02/16/2015 Paul.  In order to remain compatible with existing systems, use NAME instead of ID as the primary key. 
						sSQL = "select NAME as ID             " + ControlChars.CrLf
						     + "     , NAME                   " + ControlChars.CrLf
						     + "  from vwPAYMENT_TERMS_LISTBOX" + ControlChars.CrLf
						     + " order by LIST_ORDER          " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwPAYMENT_TERMS_LISTBOX", dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		// 12/21/2010 Paul.  Allow regions to be used in a list. 
		public DataTable Regions()
		{
			DataTable dt = Cache.Get("vwREGIONS") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select ID                " + ControlChars.CrLf
						     + "     , NAME              " + ControlChars.CrLf
						     + "  from vwREGIONS         " + ControlChars.CrLf
						     + " order by NAME           " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwREGIONS", dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		// 06/29/2014 Paul.  Add support for http://www.zip-tax.com/. 
		public void ClearTaxRates()
		{
			Cache.Remove("vwTAX_RATES_LISTBOX");
			Cache.Remove("vwTAX_RATES_LISTBOX.ReactClient");
		}

		public DataTable TaxRates()
		{
			DataTable dt = Cache.Get("vwTAX_RATES_LISTBOX") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 03/31/2007 Paul.  We need to cache the tax rate value for quick access in Quotes and Orders. 
						// 04/07/2016 Paul.  Tax rates per team. 
						sSQL = "select *                  " + ControlChars.CrLf
						     + "  from vwTAX_RATES_LISTBOX" + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							// 04/07/2016 Paul.  Tax rates per team. 
							if ( Sql.ToBoolean(Application["CONFIG.Orders.EnableTaxRateTeams"]) )
								Security.Filter(cmd, "TaxRates", "list");
							cmd.CommandText += " order by LIST_ORDER      " + ControlChars.CrLf;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								// 06/29/2014 Paul.  Automatically add the tax rate to the display name. 
								foreach ( DataRow row in dt.Rows )
								{
									string  sNAME  = Sql.ToString (row["NAME" ]);
									Decimal dVALUE = Sql.ToDecimal(row["VALUE"]);
									if ( !sNAME.EndsWith("%") )
									{
										string  sVALUE = dVALUE.ToString("0.000");
										// 06/29/2014 Paul.  Only use the third decimal place when necessary. 
										if ( sVALUE.EndsWith("0") )
											sVALUE = sVALUE.Substring(0, sVALUE.Length - 1);
										sNAME += "  " + sVALUE + "%";
										row["NAME"] = sNAME;
									}
								}
								Cache.Set("vwTAX_RATES_LISTBOX", dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		public DataTable ContractTypes()
		{
			DataTable dt = Cache.Get("vwCONTRACT_TYPES_LISTBOX") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select ID                      " + ControlChars.CrLf
						     + "     , NAME                    " + ControlChars.CrLf
						     + "  from vwCONTRACT_TYPES_LISTBOX" + ControlChars.CrLf
						     + " order by LIST_ORDER           " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwCONTRACT_TYPES_LISTBOX", dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		// 07/12/2014 Paul.  Currencies needs to be called from the scheduler, so the application must be provided. 
		public DataTable Currencies()
		{
			DataTable dt = Cache.Get("vwCURRENCIES_LISTBOX") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 05/29/2008 Paul.  ISO4217 is needed to process PayPal transactions. 
						sSQL = "select ID                  " + ControlChars.CrLf
						     + "     , NAME                " + ControlChars.CrLf
						     + "     , SYMBOL              " + ControlChars.CrLf
						     + "     , NAME_SYMBOL         " + ControlChars.CrLf
						     + "     , CONVERSION_RATE     " + ControlChars.CrLf
						     + "     , ISO4217             " + ControlChars.CrLf
						     + "  from vwCURRENCIES_LISTBOX" + ControlChars.CrLf
						     + " order by NAME             " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwCURRENCIES_LISTBOX", dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		// 09/12/2019 Paul.  Timezones needs to be called from the React Client, so the application must be provided. 
		public DataTable Timezones()
		{
			DataTable dt = Cache.Get("vwTIMEZONES") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 07/06/2020 Paul.   Include the sort. 
						sSQL = "select *           " + ControlChars.CrLf
						     + "  from vwTIMEZONES " + ControlChars.CrLf
						     + " order by BIAS desc" + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwTIMEZONES", dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		public DataTable TimezonesListbox()
		{
			DataTable dt = Cache.Get("vwTIMEZONES_LISTBOX") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select ID                 " + ControlChars.CrLf
						     + "     , NAME               " + ControlChars.CrLf
						     + "  from vwTIMEZONES_LISTBOX" + ControlChars.CrLf
						     + " order by BIAS desc       " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwTIMEZONES_LISTBOX", dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		public void ClearLanguages()
		{
			// 02/18/2008 Paul.  HttpRuntime.Cache is a better and faster way to get to the cache. 
			Cache.Remove("vwLANGUAGES");
		}

		//.02/18/2008 Paul.  Languages will also be used in the UI, so provide a version without parameters. 
		// 02/18/2008 Paul.  Languages needs to be called from the scheduler, so the application must be provided. 
		public DataTable Languages()
		{
			DataTable dt = Cache.Get("vwLANGUAGES") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 05/20/2008 Paul.  Only display active languages. 
						// 05/20/2008 Paul.  Sort by display name so that Chinese is not at the bottom. 
						sSQL = "select NAME              " + ControlChars.CrLf
						     + "     , NATIVE_NAME       " + ControlChars.CrLf
						     + "     , DISPLAY_NAME      " + ControlChars.CrLf
						     + "  from vwLANGUAGES_Active" + ControlChars.CrLf
						     + " order by DISPLAY_NAME   " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwLANGUAGES", dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					// 10/20/2009 Paul.  Make sure to pass the Application as this function can be called in a background task. 
					SplendidError.SystemMessage("Error", new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		// 10/24/2009 Paul.  Pass the context instead of the Application so that more information will be available to the error handling. 
		public DataTable ModulesPopups()
		{
			DataTable dt = Cache.Get("vwMODULES_Popup") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select MODULE_NAME   " + ControlChars.CrLf
						     + "     , RELATIVE_PATH " + ControlChars.CrLf
						     + "  from vwMODULES     " + ControlChars.CrLf
						     + " order by MODULE_NAME" + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								dt.Columns.Add("HAS_POPUP"    , typeof(System.Boolean));
								dt.Columns.Add("SINGULAR_NAME", typeof(System.String ));

								foreach(DataRow row in dt.Rows)
								{
									string sRELATIVE_PATH = Sql.ToString(row["RELATIVE_PATH"]);
									string sMODULE_NAME   = Sql.ToString(row["MODULE_NAME"]);
									string sSINGULAR_NAME = sMODULE_NAME;
									if ( sSINGULAR_NAME.EndsWith("ies") )
										sSINGULAR_NAME = sSINGULAR_NAME.Substring(0, sSINGULAR_NAME.Length - 3) + "y";
									else if ( sSINGULAR_NAME.EndsWith("s") )
										sSINGULAR_NAME = sSINGULAR_NAME.Substring(0, sSINGULAR_NAME.Length - 1);
									row["SINGULAR_NAME"] = sSINGULAR_NAME;
									
									// 09/03/2009 Paul.  File IO is expensive, so cache the results of the Exists test. 
									// 11/19/2009 Paul.  Simplify the exists test. 
									// 08/25/2013 Paul.  File IO is slow, so cache existance test. 
									row["HAS_POPUP"] = Utils.CachedFileExists(hostingEnvironment, sRELATIVE_PATH + "Popup.aspx");
									sRELATIVE_PATH = sRELATIVE_PATH.Replace("~/", Sql.ToString(Application["rootURL"]));
									row["RELATIVE_PATH"] = sRELATIVE_PATH;
								}
								Cache.Set("vwMODULES_Popup", dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemMessage("Error", new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		// 04/03/2010 Paul.  Exchange Sync is a per-module feature, even though only Accounts, Bugs, Cases, Leads, Opportunities and Projects are available. 
		public DataTable ExchangeModulesSync()
		{
			DataTable dt = Cache.Get("vwMODULES_ExchangeSync") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select MODULE_NAME      " + ControlChars.CrLf
						     + "     , EXCHANGE_FOLDERS " + ControlChars.CrLf
						     + "  from vwMODULES        " + ControlChars.CrLf
						     + " where EXCHANGE_SYNC = 1" + ControlChars.CrLf
						     + " order by MODULE_NAME   " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwMODULES_ExchangeSync", dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemMessage("Error", new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		public DateTime ExchangeFolderCacheExpiration()
		{
#if DEBUG
			return DateTime.Now.AddSeconds(1);
#else
			return DateTime.Now.AddHours(1);
#endif
		}

		public void ClearExchangeFolders(Guid gUSER_ID)
		{
			Cache.Remove("vwEXCHANGE_FOLDERS." + gUSER_ID.ToString());
		}

		public DataTable ExchangeFolders(Guid gUSER_ID)
		{
			DataTable dt = Cache.Get("vwEXCHANGE_FOLDERS." + gUSER_ID.ToString()) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 04/25/2010 Paul.  We want the well known folders to be first. 
						sSQL = "select *                                    " + ControlChars.CrLf
						     + "  from vwEXCHANGE_FOLDERS                   " + ControlChars.CrLf
						     + " where ASSIGNED_USER_ID = @ASSIGNED_USER_ID " + ControlChars.CrLf
						     + " order by WELL_KNOWN_FOLDER desc, MODULE_NAME, PARENT_NAME, REMOTE_KEY" + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@ASSIGNED_USER_ID", gUSER_ID);
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwEXCHANGE_FOLDERS." + gUSER_ID.ToString(), dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemMessage("Error", new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		// 10/19/2010 Paul.  Clear the PaymentGateways cache. 
		public void ClearPaymentGateways()
		{
			Cache.Remove("vwPAYMENT_GATEWAYS");
		}

		public DataTable PaymentGateways()
		{
			DataTable dt = Cache.Get("vwPAYMENT_GATEWAYS") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select *                 " + ControlChars.CrLf
						     + "  from vwPAYMENT_GATEWAYS" + ControlChars.CrLf
						     + " order by NAME           " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwPAYMENT_GATEWAYS", dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemMessage("Error", new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		// 04/22/2010 Paul.  We need to clear the module folders table any time the subscription changes. 
		public void ClearExchangeModulesFolders(string sMODULE_NAME, Guid gUSER_ID)
		{
			string sTABLE_NAME = CrmModules.TableName(sMODULE_NAME);
			Cache.Remove("vw" + sTABLE_NAME + "_ExchangeFolders." + gUSER_ID.ToString());
		}

		// 04/04/2010 Paul.  Exchange Folders is a per-module feature, even though only Accounts, Bugs, Cases, Leads, Opportunities and Projects are available. 
		public DataTable ExchangeModulesFolders(string sMODULE_NAME, Guid gUSER_ID)
		{
			string sTABLE_NAME = CrmModules.TableName(sMODULE_NAME);
			DataTable dt = Cache.Get("vw" + sTABLE_NAME + "_ExchangeFolders." + gUSER_ID.ToString()) as DataTable;
			//04/04/2010 Paul.  ExchangeModulesFolders can return NULL if Exchange Folders is not supported. 
			// At this time, only Accounts, Bugs, Cases, Contacts, Leads, Opportunities and Projects are supported. 
			if ( dt == null && (   sMODULE_NAME == "Accounts" 
			                    || sMODULE_NAME == "Bugs" 
			                    || sMODULE_NAME == "Cases" 
			                    || sMODULE_NAME == "Contacts" 
			                    || sMODULE_NAME == "Leads" 
			                    || sMODULE_NAME == "Opportunities" 
			                    || sMODULE_NAME == "Project"
			                   )
			   )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL = String.Empty;
						// 05/14/2010 Paul.  We need the NEW_FOLDER flag to determine if we should perform a first SyncAll. 
						sSQL = "select ID                                  " + ControlChars.CrLf
						     + "     , NAME                                " + ControlChars.CrLf
						     + "     , NEW_FOLDER                          " + ControlChars.CrLf
						     + "  from vw" + sTABLE_NAME + "_ExchangeFolder" + ControlChars.CrLf
						     + " where USER_ID = @USER_ID                  " + ControlChars.CrLf
						     + " order by NAME                             " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@USER_ID", gUSER_ID);
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vw" + sTABLE_NAME + "_ExchangeFolders." + gUSER_ID.ToString(), dt, ExchangeFolderCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemMessage("Error", new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		public DataTable Modules()
		{
			// 05/27/2009 Paul.  Module names are returned translated, so make sure to cache based on the language. 
			DataTable dt = Cache.Get(L10n.NAME + ".vwMODULES") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select MODULE_NAME   " + ControlChars.CrLf
						     + "     , DISPLAY_NAME  " + ControlChars.CrLf
						     + "  from vwMODULES     " + ControlChars.CrLf
						     + " order by MODULE_NAME" + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								foreach(DataRow row in dt.Rows)
								{
									row["DISPLAY_NAME"] = L10n.Term(Sql.ToString(row["DISPLAY_NAME"]));
								}
								Cache.Set(L10n.NAME + ".vwMODULES", dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		// 05/17/2019 Paul.  The React client needs to include modules with layout data. 
		public Dictionary<string, object> GetAllModules(List<string> lstMODULES)
		{
			// 08/09/2020 Paul.  Convert to comma separated string. 
			string sModuleList = String.Join(",", lstMODULES.ToArray());
			Dictionary<string, object> objs = Cache.Get("vwMODULES.ReactClient." + sModuleList) as Dictionary<string, object>;
			if ( objs == null )
			{
				objs = new Dictionary<string, object>();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL = String.Empty;
							// 08/12/2019 Paul.  ARCHIVED_ENBLED is needed for the dynamic buttons. 
							// 08/15/2019 Paul.  All remaining MODULES fields were added to vwMODULES_AppVars. 
							string sARCHIVE_DATABASE = Sql.ToString(Application["CONFIG.Archive.Database"]);
							if ( !Sql.IsEmptyString(sARCHIVE_DATABASE) )
								sARCHIVE_DATABASE = "[" + sARCHIVE_DATABASE + "].";
							// 05/30/2019 Paul.  Home needs to be manually included as it does not have a table, so it is not in vwSYSTEM_REST_TABLES. 
							// 08/24/2019 Paul.  ActivityStream needs to be manually included. 
							// 08/26/2019 Paul.  Activities needs to be amanually included. 
							// 12/03/2019 Paul.  Separate Archive View exists flag so that we can display information on DetailView. 
							// 05/17/2020 Paul.  Include Import module. 
							sSQL = "select vwMODULES_AppVars.*" + ControlChars.CrLf
							     + "     , (case when exists(select * from " + sARCHIVE_DATABASE + "INFORMATION_SCHEMA.TABLES where TABLES.TABLE_NAME =        vwMODULES_AppVars.TABLE_NAME + '_ARCHIVE') then 1 else 0 end) as ARCHIVED_ENBLED     " + ControlChars.CrLf
							     + "     , (case when exists(select * from " + sARCHIVE_DATABASE + "INFORMATION_SCHEMA.TABLES where TABLES.TABLE_NAME = 'vw' + vwMODULES_AppVars.TABLE_NAME + '_ARCHIVE') then 1 else 0 end) as ARCHIVED_VIEW_EXISTS" + ControlChars.CrLf
							     + "  from vwMODULES_AppVars  " + ControlChars.CrLf
							     + " where (   MODULE_NAME in (select MODULE_NAME from vwSYSTEM_REST_TABLES)" + ControlChars.CrLf
							     + "        or MODULE_NAME in ('Home', 'Activities', 'UserSignatures', 'Import')" + ControlChars.CrLf
							     + (Sql.ToBoolean(Application["CONFIG.enable_activity_streams"]) ? "        or MODULE_NAME in ('ActivityStream')" + ControlChars.CrLf : String.Empty )
							     + "       )" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AppendParameter(cmd, lstMODULES.ToArray(), "MODULE_NAME");
								cmd.CommandText += " order by MODULE_NAME" + ControlChars.CrLf;
							
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dt = new DataTable() )
									{
										da.Fill(dt);
										foreach ( DataRow row in dt.Rows )
										{
											string sMODULE_NAME = Sql.ToString(row["MODULE_NAME"]);
											if ( Sql.ToBoolean(Application["Modules." + sMODULE_NAME + ".Exists"]) )
											{
												Dictionary<string, object> drow = new Dictionary<string, object>();
												for ( int j = 0; j < dt.Columns.Count; j++ )
												{
													drow.Add(dt.Columns[j].ColumnName, row[j]);
												}
												if ( !objs.ContainsKey(sMODULE_NAME) )
												{
													objs.Add(sMODULE_NAME, drow);
												}
												// 03/26/2020 Paul.  ReportDesigner is also an alias for Reports in the React Client 
												if ( sMODULE_NAME == "ReportDesigner" )
												{
													if ( !objs.ContainsKey("Reports") )
													{
														objs.Add("Reports", drow);
													}
												}
											}
										}
										Cache.Set("vwMODULES.ReactClient." + sModuleList, objs, DefaultCacheExpiration());
									}
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					// 04/28/2019 Paul.  This error is not critical, so we can just log and ignore. 
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 08/14/2019 Paul.  This error is now critical, so throw. 
					throw;
				}
			}
			return objs;
		}

		public Dictionary<string, object> GetAdminModules(Dictionary<string, int> dictModuleTabOrder)
		{
			Dictionary<string, object> objs = Cache.Get("vwMODULES.ReactClient.Admin") as Dictionary<string, object>;
			if ( objs == null )
			{
				objs = new Dictionary<string, object>();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL = String.Empty;
							// 08/12/2019 Paul.  ARCHIVED_ENBLED is needed for the dynamic buttons. 
							// 08/15/2019 Paul.  All remaining MODULES fields were added to vwMODULES_AppVars. 
							// 12/03/2019 Paul.  Separate Archive View exists flag so that we can display information on DetailView. 
							string sARCHIVE_DATABASE = Sql.ToString(Application["CONFIG.Archive.Database"]);
							if ( !Sql.IsEmptyString(sARCHIVE_DATABASE) )
								sARCHIVE_DATABASE = "[" + sARCHIVE_DATABASE + "].";
							sSQL = "select vwMODULES_AppVars.*   " + ControlChars.CrLf
							     + "     , (case when exists(select * from " + sARCHIVE_DATABASE + "INFORMATION_SCHEMA.TABLES where TABLES.TABLE_NAME =        vwMODULES_AppVars.TABLE_NAME + '_ARCHIVE') then 1 else 0 end) as ARCHIVED_ENBLED     " + ControlChars.CrLf
							     + "     , (case when exists(select * from " + sARCHIVE_DATABASE + "INFORMATION_SCHEMA.TABLES where TABLES.TABLE_NAME = 'vw' + vwMODULES_AppVars.TABLE_NAME + '_ARCHIVE') then 1 else 0 end) as ARCHIVED_VIEW_EXISTS" + ControlChars.CrLf
							     + "  from vwMODULES_AppVars" + ControlChars.CrLf
							     + " order by MODULE_NAME" + ControlChars.CrLf;
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
											string sMODULE_NAME = Sql.ToString (row["MODULE_NAME"]);
											int    nTAB_ORDER   = Sql.ToInteger(row["TAB_ORDER"  ]);
											dictModuleTabOrder.Add(sMODULE_NAME, nTAB_ORDER);
											
											Dictionary<string, object> drow = new Dictionary<string, object>();
											for ( int j = 0; j < dt.Columns.Count; j++ )
											{
												drow.Add(dt.Columns[j].ColumnName, row[j]);
											}
											if ( !objs.ContainsKey(sMODULE_NAME) )
											{
												objs.Add(sMODULE_NAME, drow);
											}
										}
										Cache.Set("vwMODULES.ReactClient.Admin", objs, DefaultCacheExpiration());
									}
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					// 04/28/2019 Paul.  This error is not critical, so we can just log and ignore. 
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 08/14/2019 Paul.  This error is now critical, so throw. 
					throw;
				}
			}
			return objs;
		}

		// 05/17/2019 Paul.  The React client needs to include config with layout data. 
		public Dictionary<string, object> GetAllConfig()
		{
			Dictionary<string, object> objs = Cache.Get("vwCONFIG.ReactClient") as Dictionary<string, object>;
			if ( objs == null )
			{
				objs = new Dictionary<string, object>();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL = String.Empty;
							// 03/18/2021 Paul.  vwCONFIG_Sync returns deleted records. 
							sSQL = "select *            " + ControlChars.CrLf
							     + "  from vwCONFIG_Sync" + ControlChars.CrLf
							     + "  where DELETED = 0 " + ControlChars.CrLf
							     + " order by NAME      " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
							
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dt = new DataTable() )
									{
										da.Fill(dt);
										// 07/01/2018 Paul.  The Data Privacy module is not returned via the REST API, so we need to simulate the flag. 
										DataRow rowDataPrivacy = null;
										rowDataPrivacy = dt.NewRow();
										rowDataPrivacy["NAME" ] = "enable_data_privacy";
										rowDataPrivacy["VALUE"] = Config.enable_data_privacy();
										dt.Rows.Add(rowDataPrivacy);
										// 07/22/2019 Paul.  Field level security may not be enabled. 
										DataRow rowFieldSecurity = null;
										rowFieldSecurity= dt.NewRow();
										rowFieldSecurity["NAME" ] = "bEnableACLFieldSecurity";
										rowFieldSecurity["VALUE"] = SplendidInit.bEnableACLFieldSecurity;
										dt.Rows.Add(rowFieldSecurity);
										foreach ( DataRow row in dt.Rows )
										{
											string sNAME = Sql.ToString(row["NAME"]).ToLower();
											// 05/17/2019 Paul.  Exclude any possible password or key. 
											// 07/13/2020 Paul.  Exchange.ClientID and GoogleApps.ClientID are needed by the user profile editor. 
											// 06/19/2023 Paul.  Twilio.LogInboundMessages looks like "login". 
											// 06/19/2023 Paul.  Asterisk.LogIncomingMissedCalls like "login". 
											if ( !(sNAME.Contains("password"    )
												|| sNAME.Contains("smtppass"    )
												|| sNAME.Contains("smtpuser"    )
												|| sNAME.Contains("username"    )
												|| sNAME.Contains("login"       )
												|| sNAME.Contains("token"       )
												|| sNAME.Contains("key"         )
												|| sNAME.Contains("appid"       )
												|| sNAME.Contains("api"         )
												|| sNAME.Contains("secret"      )
												|| sNAME.Contains("certificate" )
												|| sNAME.Contains("x509"        )
												|| sNAME.Contains("oauth"       )
												|| sNAME.Contains("creditcard"  )
												|| sNAME.Contains("inboundemail")
												|| sNAME.Contains("accountsid"  )
												) || sNAME.Contains("loginboundmessages") || sNAME.Contains("logincomingmissedcalls") )
											{
												sNAME  = Sql.ToString(row["NAME" ]);
												string sVALUE = Sql.ToString(row["VALUE"]);
												if ( !objs.ContainsKey(sNAME) )
												{
													objs.Add(sNAME, sVALUE);
												}
											}
										}
										Cache.Set("vwCONFIG.ReactClient", objs, DefaultCacheExpiration());
									}
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					// 04/28/2019 Paul.  This error is not critical, so we can just log and ignore. 
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return objs;
		}

		public Dictionary<string, object> GetLoginConfig()
		{
			Dictionary<string, object> objs = Cache.Get("vwCONFIG.ReactClient.Login") as Dictionary<string, object>;
#if DEBUG
			objs = null;
#endif
			if ( objs == null )
			{
				objs = new Dictionary<string, object>();
				try
				{
					List<string> arr = new List<string>();
					arr.Add("application_name"        );
					arr.Add("service_level"           );
					arr.Add("default_language"        );
					arr.Add("default_module"          );
					arr.Add("default_theme"           );
					arr.Add("default_user_name"       );
					arr.Add("facebook.EnableLogin"    );
					// 02/17/2020 Paul. Include logo info. 
					arr.Add("company_name"            );
					arr.Add("header_home_image"       );
					arr.Add("header_logo_image"       );
					arr.Add("header_logo_width"       );
					arr.Add("header_logo_height"      );
					arr.Add("header_logo_style"       );
					arr.Add("header_background"       );
					arr.Add("arctic_header_logo_style");
					// 04/20/2021 Paul.  Add support for forced https. 
					arr.Add("Site.Https"              );
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL = String.Empty;
						// 03/18/2021 Paul.  vwCONFIG_Sync returns deleted records. 
						sSQL = "select *            " + ControlChars.CrLf
						     + "  from vwCONFIG_Sync" + ControlChars.CrLf
						     + " where DELETED = 0  " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AppendParameter(cmd, arr.ToArray(), "NAME");
							cmd.CommandText +=" order by NAME" + ControlChars.CrLf;
						
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								using ( DataTable dt = new DataTable() )
								{
									da.Fill(dt);
									foreach ( DataRow row in dt.Rows )
									{
										string sNAME = Sql.ToString(row["NAME"]).ToLower();
										sNAME  = Sql.ToString(row["NAME" ]);
										string sVALUE = Sql.ToString(row["VALUE"]);
										if ( !objs.ContainsKey(sNAME) )
										{
											objs.Add(sNAME, sVALUE);
										}
									}
									Cache.Set("vwCONFIG.ReactClient.Login", objs, DefaultCacheExpiration());
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					// 04/28/2019 Paul.  This error is not critical, so we can just log and ignore. 
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return objs;
		}

		// 08/07/2013 Paul.  Add Undelete module. 
		public DataTable AuditedModules()
		{
			DataTable dt = Cache.Get(L10n.NAME + ".vwMODULES_Audited") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select MODULE_NAME      " + ControlChars.CrLf
						     + "     , DISPLAY_NAME     " + ControlChars.CrLf
						     + "  from vwMODULES_Audited" + ControlChars.CrLf
						     + " order by MODULE_NAME   " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								foreach(DataRow row in dt.Rows)
								{
									row["DISPLAY_NAME"] = L10n.Term(Sql.ToString(row["DISPLAY_NAME"]));
								}
								Cache.Set(L10n.NAME + ".vwMODULES_Audited", dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		public void ClearTerminologyPickLists()
		{
			Cache.Remove("vwTERMINOLOGY_PickList");
			Cache.Remove("vwTERMINOLOGY_PickList.ReactClient");
			Cache.Remove("vwTERMINOLOGY_PickList.ReactClient.Admion");
		}

		public DataTable TerminologyPickLists()
		{
			DataTable dt = Cache.Get("vwTERMINOLOGY_PickList") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select *                     " + ControlChars.CrLf
						     + "  from vwTERMINOLOGY_PickList" + ControlChars.CrLf
						     + " order by LIST_NAME          " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwTERMINOLOGY_PickList", dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		public DataTable ActiveUsers()
		{
			DataTable dt = Cache.Get("vwUSERS_List") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select ID          " + ControlChars.CrLf
						     + "     , USER_NAME   " + ControlChars.CrLf
						     + "  from vwUSERS_List" + ControlChars.CrLf
						     + " order by USER_NAME" + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								// 09/16/2005 Paul.  Users change a lot, so have a very short timeout. 
								Cache.Set("vwUSERS_List", dt, DateTime.Now.AddSeconds(15));
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		public void ClearTabMenu()
		{
			try
			{
				// 02/25/2010 Paul.  Clear the GroupMenu. 
				Session.Remove("SplendidGroupMenuHtml");
				Session.Remove("vwMODULES_GROUPS_ByUser." + Security.USER_ID.ToString());
				//Cache.Remove("vwMODULES_TabMenu");
				// 04/28/2006 Paul.  The menu is now cached in the Session, so it will only get cleared when the user logs out. 
				Session.Remove("vwMODULES_TabMenu_ByUser." + Security.USER_ID.ToString());
				// 11/17/2007 Paul.  Also clear the mobile menu. 
				Session.Remove("vwMODULES_MobileMenu_ByUser." + Security.USER_ID.ToString());
				Cache.Remove("vwMODULES_Popup");
				// 10/24/2009 Paul.  ModulePopupScripts is a very popular file and we need to cache it as often as possible, yet still allow an invalidation for module changes. 
				// 10/24/2009 Paul.  We still can't use the standard page caching otherwise we risk getting an unauthenticated page cached, which would prevent all popups. 
				// HttpResponse.RemoveOutputCacheItem("~/Include/javascript/ModulePopupScripts.aspx");
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
			}
		}

		public DataTable TabMenu()
		{
			// 04/28/2006 Paul.  The menu is now cached in the Session, so it will only get cleared when the user logs out. 
			// 04/28/2006 Paul.  Include the GUID in the USER_ID to that the user does not have to log-out in order to get the correct menu. 
			DataTable dt = Session.GetTable("vwMODULES_TabMenu_ByUser." + Security.USER_ID.ToString());
			if ( dt == null )
			{
				dt = new DataTable();
				try
				{
					// 11/17/2007 Paul.  New function to determine if user is authenticated. 
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							sSQL = "select MODULE_NAME             " + ControlChars.CrLf
							     + "     , DISPLAY_NAME            " + ControlChars.CrLf
							     + "     , RELATIVE_PATH           " + ControlChars.CrLf
							     + "  from vwMODULES_TabMenu_ByUser" + ControlChars.CrLf
							     + " where USER_ID = @USER_ID      " + ControlChars.CrLf
							     + "    or USER_ID is null         " + ControlChars.CrLf
							     + " order by TAB_ORDER            " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@USER_ID", Security.USER_ID);
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dt);
									// 11/06/2009 Paul.  We need a fast way to disable modules that do not exist on the Offline Client. 
									foreach ( DataRow row in dt.Rows )
									{
										string sMODULE_NAME = Sql.ToString(row["MODULE_NAME"]);
										if ( !Sql.ToBoolean(Application["Modules." + sMODULE_NAME + ".Exists"]) )
										{
											row.Delete();
										}
									}
									dt.AcceptChanges();
									Session.SetTable("vwMODULES_TabMenu_ByUser." + Security.USER_ID.ToString(), dt);
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 11/21/2005 Paul. Ignore error, but then we need to find a way to display the connection error. 
					// The most likely failure here is a connection failure. 
				}
			}
			return dt;
		}

		// 05/16/2019 Paul.  Return the tab menu so that we don't need a separate request for it later. 
		public List<object> GetAllTabMenus()
		{
			// 07/21/2019 Paul.  This per user data, so it should be session based. 
			List<object> objs = Session["vwMODULES.TabMenu.ReactClient"] as List<object>;
			if ( objs == null )
			{
				objs = new List<object>();
				try
				{
					// 11/17/2007 Paul.  New function to determine if user is authenticated. 
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							sSQL = "select MODULE_NAME             " + ControlChars.CrLf
							     + "     , DISPLAY_NAME            " + ControlChars.CrLf
							     + "     , RELATIVE_PATH           " + ControlChars.CrLf
							     + "  from vwMODULES_TabMenu_ByUser" + ControlChars.CrLf
							     + " where USER_ID = @USER_ID      " + ControlChars.CrLf
							     + "    or USER_ID is null         " + ControlChars.CrLf
							     + " order by TAB_ORDER            " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@USER_ID", Security.USER_ID);
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dt = new DataTable() )
									{
										da.Fill(dt);
										dt.Columns.Add("EDIT_ACLACCESS", typeof(System.String));
										// 11/06/2009 Paul.  We need a fast way to disable modules that do not exist on the Offline Client. 
										foreach ( DataRow row in dt.Rows )
										{
											string sMODULE_NAME = Sql.ToString(row["MODULE_NAME"]);
											if ( Sql.ToBoolean(Application["Modules." + sMODULE_NAME + ".Exists"]) )
											{
												int nEDIT_ACLACCESS = Security.GetUserAccess(sMODULE_NAME, "edit");
												row["EDIT_ACLACCESS"] = nEDIT_ACLACCESS.ToString();
												Dictionary<string, object> drow = new Dictionary<string, object>();
												for ( int j = 0; j < dt.Columns.Count; j++ )
												{
													drow.Add(dt.Columns[j].ColumnName, row[j]);
												}
												objs.Add(drow);
											}
										}
										Session["vwMODULES.TabMenu.ReactClient"] = objs;
									}
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 11/21/2005 Paul. Ignore error, but then we need to find a way to display the connection error. 
					// The most likely failure here is a connection failure. 
				}
			}
			return objs;
		}

		public DataTable TabFeeds()
		{
			DataTable dt = Session.GetTable("vwFEEDS_MyTabFeeds_ByUser." + Security.USER_ID.ToString());
			if ( dt == null )
			{
				dt = new DataTable();
				try
				{
					// 11/17/2007 Paul.  New function to determine if user is authenticated. 
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							sSQL = "select TITLE             " + ControlChars.CrLf
							     + "     , URL               " + ControlChars.CrLf
							     + "  from vwFEEDS_MyTabFeeds" + ControlChars.CrLf
							     + " where USER_ID = @USER_ID" + ControlChars.CrLf
							     + "    or USER_ID is null   " + ControlChars.CrLf
							     + " order by RANK           " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@USER_ID", Security.USER_ID);
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dt);
									Session.SetTable("vwFEEDS_MyTabFeeds_ByUser." + Security.USER_ID.ToString(), dt);
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		public DataTable MobileMenu()
		{
			// 04/28/2006 Paul.  The menu is now cached in the Session, so it will only get cleared when the user logs out. 
			// 04/28/2006 Paul.  Include the GUID in the USER_ID to that the user does not have to log-out in order to get the correct menu. 
			DataTable dt = Session.GetTable("vwMODULES_MobileMenu_ByUser." + Security.USER_ID.ToString());
			if ( dt == null )
			{
				dt = new DataTable();
				try
				{
					// 11/17/2007 Paul.  New function to determine if user is authenticated. 
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							sSQL = "select MODULE_NAME                " + ControlChars.CrLf
							     + "     , DISPLAY_NAME               " + ControlChars.CrLf
							     + "     , RELATIVE_PATH              " + ControlChars.CrLf
							     + "  from vwMODULES_MobileMenu_ByUser" + ControlChars.CrLf
							     + " where USER_ID = @USER_ID         " + ControlChars.CrLf
							     + "    or USER_ID is null            " + ControlChars.CrLf
							     + " order by TAB_ORDER               " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@USER_ID", Security.USER_ID);
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dt);
									Session.SetTable("vwMODULES_MobileMenu_ByUser." + Security.USER_ID.ToString(), dt);
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 11/21/2005 Paul. Ignore error, but then we need to find a way to display the connection error. 
					// The most likely failure here is a connection failure. 
				}
			}
			return dt;
		}

		public void ClearShortcuts(string sMODULE_NAME)
		{
			Session.Remove("vwSHORTCUTS_Menu_ByUser." + sMODULE_NAME + "." + Security.USER_ID.ToString());
		}

		public DataTable Shortcuts(string sMODULE_NAME)
		{
			// 04/28/2006 Paul.  The shortcuts is now cached in the Session, so it will only get cleared when the user logs out. 
			// 04/28/2006 Paul.  Include the GUID in the USER_ID to that the user does not have to log-out in order to get the correct menu. 
			DataTable dt = Session.GetTable("vwSHORTCUTS_Menu_ByUser." + sMODULE_NAME + "." + Security.USER_ID.ToString());
			if ( dt == null )
			{
				dt = new DataTable();
				try
				{
					// 11/17/2007 Paul.  New function to determine if user is authenticated. 
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							// 09/26/2017 Paul.  Add Archive access right. 
							sSQL = "select MODULE_NAME                            " + ControlChars.CrLf
							     + "     , DISPLAY_NAME                           " + ControlChars.CrLf
							     + "     , RELATIVE_PATH                          " + ControlChars.CrLf
							     + "     , IMAGE_NAME                             " + ControlChars.CrLf
							     + "     , SHORTCUT_ACLTYPE                       " + ControlChars.CrLf
							     + "  from vwSHORTCUTS_Menu_ByUser                " + ControlChars.CrLf
							     + " where MODULE_NAME = @MODULE_NAME             " + ControlChars.CrLf
							     + "   and (USER_ID = @USER_ID or USER_ID is null)" + ControlChars.CrLf
							     + " order by SHORTCUT_ORDER                      " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@MODULE_NAME", sMODULE_NAME);
								Sql.AddParameter(cmd, "@USER_ID"    , Security.USER_ID);
								
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dt);
									Session.SetTable("vwSHORTCUTS_Menu_ByUser." + sMODULE_NAME + "." + Security.USER_ID.ToString(), dt);
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 11/21/2005 Paul. Ignore error, but then we need to find a way to display the connection error. 
					// The most likely failure here is a connection failure. 
				}
			}
			return dt;
		}

		public DataTable LastViewed(string sMODULE_NAME)
		{
			DataTable dt = new DataTable("vwTRACKER");
			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					string sSQL;
					sSQL = "select *                         " + ControlChars.CrLf
					     + "  from vwTRACKER_LastViewed      " + ControlChars.CrLf
					     + " where USER_ID     = @USER_ID    " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@USER_ID"    , Security.USER_ID);
						if( !Sql.IsEmptyString(sMODULE_NAME) && sMODULE_NAME != "Home" )
						{
							cmd.CommandText += "   and MODULE_NAME = @MODULE_NAME" + ControlChars.CrLf;
							Sql.AddParameter(cmd, "@MODULE_NAME", sMODULE_NAME    );
						}
						cmd.CommandText += " order by DATE_ENTERED desc      " + ControlChars.CrLf;
						try
						{
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								using ( DataSet ds = new DataSet() )
								{
									ds.Tables.Add(dt);
									int nHistoryMaxViewed = Sql.ToInteger(Application["CONFIG.history_max_viewed"]);
									if ( nHistoryMaxViewed == 0 )
										nHistoryMaxViewed = 10;
									da.Fill(ds, 0, nHistoryMaxViewed, "vwTRACKER");
								}
							}
						}
						catch(Exception ex)
						{
							SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
			}
			return dt;
		}

		// 04/28/2019 Paul.  Flag to include Favorites and LastViewed for the React client. 
		public Dictionary<string, object> GetAllLastViewed()
		{
			// 05/26/2019 Paul.  Last viewed changes too much to try to cache. 
			Dictionary<string, object> objs = null;
			if ( objs == null )
			{
				objs = new Dictionary<string, object>();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL = String.Empty;
							sSQL = "select MODULE_NAME                 " + ControlChars.CrLf
							     + "     , ITEM_ID                     " + ControlChars.CrLf
							     + "     , ITEM_SUMMARY                " + ControlChars.CrLf
							     + "  from vwTRACKER_LastViewed        " + ControlChars.CrLf
							     + " where USER_ID = @USER_ID          " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@USER_ID", Security.USER_ID);
								// 11/25/2020 Paul.  Must sort by DATE_ENTERED in order for last item to be at the top. 
								cmd.CommandText += " order by MODULE_NAME, DATE_ENTERED desc" + ControlChars.CrLf;
							
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dt = new DataTable() )
									{
										da.Fill(dt);
										string sLAST_MODULE = String.Empty;
										List<object> arrLastModule = null;
										foreach ( DataRow row in dt.Rows )
										{
											string sMODULE_NAME  = Sql.ToString(row["MODULE_NAME" ]);
											Guid   gITEM_ID      = Sql.ToGuid  (row["ITEM_ID"     ]);
											string sITEM_SUMMARY = Sql.ToString(row["ITEM_SUMMARY"]);
											if ( arrLastModule == null || sLAST_MODULE != sMODULE_NAME )
											{
												arrLastModule = new List<object>();
												objs.Add(sMODULE_NAME, arrLastModule);
												sLAST_MODULE = sMODULE_NAME;
											}
											Dictionary<string, object> obj = new Dictionary<string, object>();
											obj["ID"  ] = gITEM_ID;
											obj["NAME"] = sITEM_SUMMARY;
											arrLastModule.Add(obj);
										}
									}
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					// 04/28/2019 Paul.  This error is not critical, so we can just log and ignore. 
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return objs;
		}

		public DataTable Themes()
		{
			DataTable dt = Cache.Get("Themes") as DataTable;
			if ( dt == null )
			{
				try
				{
					dt = new DataTable();
					dt.Columns.Add("NAME", Type.GetType("System.String"));
					
					FileInfo objInfo = null;
					string[] arrDirectories;
					// 03/07/2007 Paul.  Theme files have moved to App_Themes in version 1.4. 
					if ( Directory.Exists(Path.Combine(hostingEnvironment.ContentRootPath, "App_Themes")) )
						arrDirectories = Directory.GetDirectories(Path.Combine(hostingEnvironment.ContentRootPath, "App_Themes"));
					else
						arrDirectories = Directory.GetDirectories(Path.Combine(hostingEnvironment.ContentRootPath, "Themes"));
					for ( int i = 0 ; i < arrDirectories.Length ; i++ )
					{
						// 12/04/2005 Paul.  Only include theme if an images folder exists.  This is a quick test. 
						// 08/14/2006 Paul.  Mono uses a different slash than Windows, so use Path.Combine(). 
						if ( Directory.Exists(Path.Combine(arrDirectories[i], "images")) )
						{
							// 11/17/2007 Paul.  Don't allow the user to select the Mobile theme.
							objInfo = new FileInfo(arrDirectories[i]);
#if !DEBUG
							if ( objInfo.Name == "Mobile" )
								continue;
#endif
							DataRow row = dt.NewRow();
							row["NAME"] = objInfo.Name;
							dt.Rows.Add(row);
						}
					}
					// 11/19/2005 Paul.  The themes cache need never expire as themes almost never change. 
					Cache.Set("Themes", dt, DefaultCacheExpiration());
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		public string XmlFile(string sPATH_NAME)
		{
			string sDATA = Cache.Get("XmlFile." + sPATH_NAME) as string;
			if ( sDATA == null )
			{
				try
				{
					using ( StreamReader rd = new StreamReader(sPATH_NAME, System.Text.Encoding.UTF8) )
					{
						sDATA = rd.ReadToEnd();
					}
					// 11/19/2005 Paul.  The file cache need never expire as themes almost never change. 
					Cache.Set("XmlFile." + sPATH_NAME, sDATA, DefaultCacheExpiration());
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					throw(new Exception("Could not load file: " + sPATH_NAME, ex));
				}
			}
			return sDATA;
		}

		public void ClearGridView(string sGRID_NAME)
		{
			Cache.Remove("vwGRIDVIEWS_COLUMNS." + sGRID_NAME);
			Cache.Remove("vwGRIDVIEWS_RULES."   + sGRID_NAME);
		}

		// 05/05/2016 Paul.  The User Primary Role is used with role-based views. 
		public DataTable GridViewColumns(string sGRID_NAME, string sPRIMARY_ROLE_NAME)
		{
			DataTable dt = null;
			// 10/1/2017 Paul.  Role-based view will not be applied with the ArchiveView. 
			// 02/23/2018 Paul.  Allow roll-based view while in archive mode. 
			if ( Sql.IsEmptyString(sPRIMARY_ROLE_NAME) )
			{
				dt = this.GridViewColumns(sGRID_NAME);
			}
			else
			{
				dt = this.GridViewColumns(sGRID_NAME + "." + sPRIMARY_ROLE_NAME);
				if ( dt.Rows.Count == 0 )
					dt = this.GridViewColumns(sGRID_NAME);
			}
			return dt;
		}

		public DataTable GridViewColumns(string sGRID_NAME)
		{
			DataTable dt = Cache.Get("vwGRIDVIEWS_COLUMNS." + sGRID_NAME) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 01/09/2006 Paul.  Exclude DEFAULT_VIEW. 
						sSQL = "select *                     " + ControlChars.CrLf
						     + "  from vwGRIDVIEWS_COLUMNS   " + ControlChars.CrLf
						     + " where GRID_NAME = @GRID_NAME" + ControlChars.CrLf
						     + "   and (DEFAULT_VIEW = 0 or DEFAULT_VIEW is null)" + ControlChars.CrLf
						     + " order by COLUMN_INDEX       " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@GRID_NAME", sGRID_NAME);
							
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwGRIDVIEWS_COLUMNS." + sGRID_NAME, dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 11/21/2005 Paul. Ignore error, but then we need to find a way to display the connection error. 
					// The most likely failure here is a connection failure. 
					dt = new DataTable();
				}
			}
			return dt;
		}

		// 03/11/2014 Paul.  This rule could be for EditView, DetailView or GridView, so we have to clear them all. 
		public void ClearBusinessRules()
		{
			foreach(string sKey in GetCacheKeys())
			{
				if ( sKey.StartsWith("vwEDITVIEWS_RULES.") || sKey.StartsWith("vwDETAILVIEWS_RULES.") || sKey.StartsWith("vwGRIDVIEWS_RULES.") )
					Cache.Remove(sKey);
			}
		}

		// 05/10/2016 Paul.  The User Primary Role is used with role-based views. 
		public DataTable GridViewRules(string sNAME, string sPRIMARY_ROLE_NAME)
		{
			DataTable dt = null;
			// 10/1/2017 Paul.  Role-based view will not be applied with the ArchiveView. 
			// 02/23/2018 Paul.  Allow roll-based view while in archive mode. 
			if ( Sql.IsEmptyString(sPRIMARY_ROLE_NAME) )
			{
				dt = this.GridViewRules(sNAME);
			}
			else
			{
				dt = this.GridViewRules(sNAME + "." + sPRIMARY_ROLE_NAME);
				if ( dt.Rows.Count == 0 )
					dt = this.GridViewRules(sNAME);
			}
			return dt;
		}

		// 11/22/2010 Paul.  Apply Business Rules. 
		public DataTable GridViewRules(string sNAME)
		{
			DataTable dt = Cache.Get("vwGRIDVIEWS_RULES." + sNAME) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select *                  " + ControlChars.CrLf
						     + "  from vwGRIDVIEWS_RULES  " + ControlChars.CrLf
						     + " where NAME = @NAME       " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@NAME", sNAME);
							
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwGRIDVIEWS_RULES." + sNAME, dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 11/21/2005 Paul. Ignore error, but then we need to find a way to display the connection error. 
					// The most likely failure here is a connection failure. 
					dt = new DataTable();
				}
			}
			return dt;
		}

		// 12/04/2010 Paul.  We need to cache the business rules separately so that they can be accessed by Reports. 
		public void ClearReportRules(Guid gID)
		{
			Cache.Remove("vwBUSINESS_RULES." + gID.ToString());
		}

		public DataTable ReportRules(Guid gID)
		{
			DataTable dt = Cache.Get("vwREPORT_RULES." + gID.ToString()) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select MODULE_NAME " + ControlChars.CrLf
						     + "     , XOML        " + ControlChars.CrLf
						     + "  from vwRULES_Edit" + ControlChars.CrLf
						     + " where ID = @ID    " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@ID", gID);
							
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwREPORT_RULES." + gID.ToString(), dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 11/21/2005 Paul. Ignore error, but then we need to find a way to display the connection error. 
					// The most likely failure here is a connection failure. 
					dt = new DataTable();
				}
			}
			return dt;
		}

		public void ClearDetailView(string sDETAIL_NAME)
		{
			Cache.Remove("vwDETAILVIEWS_FIELDS." + sDETAIL_NAME);
			Cache.Remove("vwDETAILVIEWS_RULES."  + sDETAIL_NAME);
		}

		// 05/05/2016 Paul.  The User Primary Role is used with role-based views. 
		public DataTable DetailViewFields(string sDETAIL_NAME, string sPRIMARY_ROLE_NAME)
		{
			DataTable dtFields = null;
			// 10/1/2017 Paul.  Role-based view will not be applied with the ArchiveView. 
			// 02/23/2018 Paul.  Allow roll-based view while in archive mode. 
			if ( Sql.IsEmptyString(sPRIMARY_ROLE_NAME) )
			{
				dtFields = this.DetailViewFields(sDETAIL_NAME);
			}
			else
			{
				dtFields = this.DetailViewFields(sDETAIL_NAME + "." + sPRIMARY_ROLE_NAME);
				if ( dtFields.Rows.Count == 0 )
					dtFields = this.DetailViewFields(sDETAIL_NAME);
			}
			return dtFields;
		}

		public DataTable DetailViewFields(string sDETAIL_NAME)
		{
			DataTable dt = Cache.Get("vwDETAILVIEWS_FIELDS." + sDETAIL_NAME) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 01/09/2006 Paul.  Exclude DEFAULT_VIEW. 
						sSQL = "select *                         " + ControlChars.CrLf
						     + "  from vwDETAILVIEWS_FIELDS      " + ControlChars.CrLf
						     + " where DETAIL_NAME = @DETAIL_NAME" + ControlChars.CrLf
						     + "   and (DEFAULT_VIEW = 0 or DEFAULT_VIEW is null)" + ControlChars.CrLf
						     + " order by FIELD_INDEX            " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@DETAIL_NAME", sDETAIL_NAME);
							
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwDETAILVIEWS_FIELDS." + sDETAIL_NAME, dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 11/21/2005 Paul. Ignore error, but then we need to find a way to display the connection error. 
					// The most likely failure here is a connection failure. 
					dt = new DataTable();
				}
			}
			return dt;
		}

		// 05/10/2016 Paul.  The User Primary Role is used with role-based views. 
		public DataTable DetailViewRules(string sNAME, string sPRIMARY_ROLE_NAME)
		{
			DataTable dt = null;
			// 10/1/2017 Paul.  Role-based view will not be applied with the ArchiveView. 
			// 02/23/2018 Paul.  Allow roll-based view while in archive mode. 
			if ( Sql.IsEmptyString(sPRIMARY_ROLE_NAME) )
			{
				dt = this.DetailViewRules(sNAME);
			}
			else
			{
				dt = this.DetailViewRules(sNAME + "." + sPRIMARY_ROLE_NAME);
				if ( dt.Rows.Count == 0 )
					dt = this.DetailViewRules(sNAME);
			}
			return dt;
		}

		// 11/10/2010 Paul.  Apply Business Rules. 
		public DataTable DetailViewRules(string sNAME)
		{
			DataTable dt = Cache.Get("vwDETAILVIEWS_RULES." + sNAME) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select *                  " + ControlChars.CrLf
						     + "  from vwDETAILVIEWS_RULES" + ControlChars.CrLf
						     + " where NAME = @NAME       " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@NAME", sNAME);
							
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwDETAILVIEWS_RULES." + sNAME, dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 11/21/2005 Paul. Ignore error, but then we need to find a way to display the connection error. 
					// The most likely failure here is a connection failure. 
					dt = new DataTable();
				}
			}
			return dt;
		}

		public DataTable TabGroups()
		{
			DataTable dt = Cache.Get("vwTAB_GROUPS") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 02/25/2010 Paul.  The Group may not be visible on the menu bar. 
						sSQL = "select NAME          " + ControlChars.CrLf
						     + "     , TITLE         " + ControlChars.CrLf
						     + "     , GROUP_MENU    " + ControlChars.CrLf
						     + "  from vwTAB_GROUPS  " + ControlChars.CrLf
						     + " where ENABLED = 1   " + ControlChars.CrLf
						     + " order by GROUP_ORDER" + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwTAB_GROUPS", dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 11/21/2005 Paul. Ignore error, but then we need to find a way to display the connection error. 
					// The most likely failure here is a connection failure. 
					dt = new DataTable();
				}
			}
			return dt;
		}

		public DataTable ModuleGroups()
		{
			DataTable dt = Cache.Get("vwMODULES_GROUPS") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 02/24/2010 Paul.  We need to specify an order to the modules for the tab menu. 
						sSQL = "select GROUP_NAME                  " + ControlChars.CrLf
						     + "     , MODULE_NAME                 " + ControlChars.CrLf
						     + "     , MODULE_MENU                 " + ControlChars.CrLf
						     + "  from vwMODULES_GROUPS            " + ControlChars.CrLf
						     + " where ENABLED = 1                 " + ControlChars.CrLf
						     + " order by GROUP_ORDER, MODULE_ORDER, MODULE_NAME" + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwMODULES_GROUPS", dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 11/21/2005 Paul. Ignore error, but then we need to find a way to display the connection error. 
					// The most likely failure here is a connection failure. 
					dt = new DataTable();
				}
			}
			return dt;
		}

		public DataTable ModuleGroupsByUser()
		{
			DataTable dt = Cache.Get("vwMODULES_GROUPS_ByUser." + Security.USER_ID.ToString()) as DataTable;
			if ( dt == null )
			{
				dt = new DataTable();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							// 02/24/2010 Paul.  We need to specify an order to the modules for the tab menu. 
							sSQL = "select GROUP_NAME                  " + ControlChars.CrLf
							     + "     , MODULE_NAME                 " + ControlChars.CrLf
							     + "     , DISPLAY_NAME                " + ControlChars.CrLf
							     + "     , RELATIVE_PATH               " + ControlChars.CrLf
							     + "  from vwMODULES_GROUPS_ByUser     " + ControlChars.CrLf
							     + " where USER_ID = @USER_ID          " + ControlChars.CrLf
							     + " order by GROUP_ORDER, MODULE_ORDER, MODULE_NAME" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@USER_ID", Security.USER_ID);
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									dt = new DataTable();
									da.Fill(dt);
									// 11/06/2009 Paul.  We need a fast way to disable modules that do not exist on the Offline Client. 
									foreach ( DataRow row in dt.Rows )
									{
										string sMODULE_NAME = Sql.ToString(row["MODULE_NAME"]);
										if ( !Sql.ToBoolean(Application["Modules." + sMODULE_NAME + ".Exists"]) )
										{
											row.Delete();
										}
									}
									dt.AcceptChanges();
									Cache.Set("vwMODULES_GROUPS_ByUser." + Security.USER_ID.ToString(), dt, DefaultCacheExpiration());
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		public void ClearDetailViewRelationships(string sDETAIL_NAME)
		{
			Cache.Remove("vwDETAILVIEWS_RELATIONSHIPS." + sDETAIL_NAME);
		}

		public DataTable DetailViewRelationships(string sDETAIL_NAME)
		{
			DataTable dt = Cache.Get("vwDETAILVIEWS_RELATIONSHIPS." + sDETAIL_NAME) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select *                          " + ControlChars.CrLf
						     + "  from vwDETAILVIEWS_RELATIONSHIPS" + ControlChars.CrLf
						     + " where DETAIL_NAME = @DETAIL_NAME " + ControlChars.CrLf
						     + " order by RELATIONSHIP_ORDER      " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@DETAIL_NAME", sDETAIL_NAME);
							
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwDETAILVIEWS_RELATIONSHIPS." + sDETAIL_NAME, dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 11/21/2005 Paul. Ignore error, but then we need to find a way to display the connection error. 
					// The most likely failure here is a connection failure. 
					dt = new DataTable();
				}
			}
			return dt;
		}

		// 04/19/20910 Paul.  Add separate table for EditView Relationships. 
		public void ClearEditViewRelationships(string sEDIT_NAME)
		{
			// 04/27/2010 Paul.  There are two cached items that need to be cleared. 
			Cache.Remove("vwEDITVIEWS_RELATIONSHIPS." + sEDIT_NAME + ".NewRecord"     );
			Cache.Remove("vwEDITVIEWS_RELATIONSHIPS." + sEDIT_NAME + ".ExistingRecord");
		}

		// 04/19/20910 Paul.  Add separate table for EditView Relationships. 
		public DataTable EditViewRelationships(string sEDIT_NAME, bool bNewRecord)
		{
			DataTable dt = Cache.Get("vwEDITVIEWS_RELATIONSHIPS." + sEDIT_NAME + (bNewRecord ? ".NewRecord" : ".ExistingRecord")) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select *                          " + ControlChars.CrLf
						     + "  from vwEDITVIEWS_RELATIONSHIPS  " + ControlChars.CrLf
						     + " where EDIT_NAME = @EDIT_NAME     " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							if ( bNewRecord )
								cmd.CommandText += "   and NEW_RECORD_ENABLED = 1     " + ControlChars.CrLf;
							else
								cmd.CommandText += "   and EXISTING_RECORD_ENABLED = 1" + ControlChars.CrLf;
							cmd.CommandText += " order by RELATIONSHIP_ORDER      " + ControlChars.CrLf;
							Sql.AddParameter(cmd, "@EDIT_NAME", sEDIT_NAME);
							
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwEDITVIEWS_RELATIONSHIPS." + sEDIT_NAME + (bNewRecord ? ".NewRecord" : ".ExistingRecord"), dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 11/21/2005 Paul. Ignore error, but then we need to find a way to display the connection error. 
					// The most likely failure here is a connection failure. 
					dt = new DataTable();
				}
			}
			return dt;
		}

		public void ClearUserDashlets(string sDETAIL_NAME)
		{
			Session.Remove("vwDASHLETS_USERS." + sDETAIL_NAME);
		}

		// 07/10/2009 Paul.  We are now allowing relationships to be user-specific. 
		public DataTable UserDashlets(string sDETAIL_NAME, Guid gUSER_ID)
		{
			DataTable dt = Session.GetTable("vwDASHLETS_USERS." + sDETAIL_NAME);
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 09/20/2009 Paul.  Use the special view vwDASHLETS_USERS_Assigned that will included deleted dashlets. 
						// This will allow a user to delete all dashlets without having the defaults re-assigned. 
						// 12/03/2009 Paul.  The Title is used for the tabbed subpanels. 
						// 01/24/2010 Paul.  We need the ID for report dashlet management. 
						sSQL = "select ID                              " + ControlChars.CrLf
						     + "     , DETAIL_NAME                     " + ControlChars.CrLf
						     + "     , MODULE_NAME                     " + ControlChars.CrLf
						     + "     , CONTROL_NAME                    " + ControlChars.CrLf
						     + "     , TITLE                           " + ControlChars.CrLf
						     + "     , RELATIONSHIP_ORDER              " + ControlChars.CrLf
						     + "  from vwDASHLETS_USERS                " + ControlChars.CrLf
						     + " where DASHLET_ENABLED  = 1            " + ControlChars.CrLf
						     + "   and ASSIGNED_USER_ID = @USER_ID     " + ControlChars.CrLf
						     + "   and DETAIL_NAME      = @DETAIL_NAME " + ControlChars.CrLf
						     + "union                                  " + ControlChars.CrLf
						     + "select ID                              " + ControlChars.CrLf
						     + "     , DETAIL_NAME                     " + ControlChars.CrLf
						     + "     , MODULE_NAME                     " + ControlChars.CrLf
						     + "     , CONTROL_NAME                    " + ControlChars.CrLf
						     + "     , TITLE                           " + ControlChars.CrLf
						     + "     , RELATIONSHIP_ORDER              " + ControlChars.CrLf
						     + "  from vwDETAILVIEWS_RELATIONSHIPS     " + ControlChars.CrLf
						     + " where DETAIL_NAME = @DETAIL_NAME      " + ControlChars.CrLf
						     + "   and not exists(select *                              " + ControlChars.CrLf
						     + "                    from vwDASHLETS_USERS_Assigned      " + ControlChars.CrLf
						     + "                   where ASSIGNED_USER_ID = @USER_ID    " + ControlChars.CrLf
						     + "                     and DETAIL_NAME      = @DETAIL_NAME" + ControlChars.CrLf
						     + "                 )                                      " + ControlChars.CrLf
						     + " order by RELATIONSHIP_ORDER           " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@USER_ID"    , gUSER_ID    );
							Sql.AddParameter(cmd, "@DETAIL_NAME", sDETAIL_NAME);
							
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Session.SetTable("vwDASHLETS_USERS." + sDETAIL_NAME, dt);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 11/21/2005 Paul. Ignore error, but then we need to find a way to display the connection error. 
					// The most likely failure here is a connection failure. 
					dt = new DataTable();
				}
			}
			return dt;
		}

		public void ClearEditView(string sEDIT_NAME)
		{
			// 03/16/2012 Paul.  With the addition of the SearchView flag, we need to update the key to include the flag. 
			Cache.Remove("vwEDITVIEWS_FIELDS." + sEDIT_NAME + ".True" );
			Cache.Remove("vwEDITVIEWS_FIELDS." + sEDIT_NAME + ".False");
			Cache.Remove("vwEDITVIEWS_RULES."  + sEDIT_NAME);
		}

		// 10/13/2011 Paul.  Special list of EditViews for the search area with IS_MULTI_SELECT. 
		public DataTable EditViewFields(string sEDIT_NAME)
		{
			return EditViewFields(sEDIT_NAME, false);
		}

		// 05/05/2016 Paul.  The User Primary Role is used with role-based views. 
		public DataTable EditViewFields(string sEDIT_NAME, string sPRIMARY_ROLE_NAME)
		{
			DataTable dtFields = null;
			// 10/1/2017 Paul.  Role-based view will not be applied with the ArchiveView. 
			// 02/23/2018 Paul.  Allow roll-based view while in archive mode. 
			if ( Sql.IsEmptyString(sPRIMARY_ROLE_NAME) )
			{
				dtFields = this.EditViewFields(sEDIT_NAME);
			}
			else
			{
				dtFields = this.EditViewFields(sEDIT_NAME + "." + sPRIMARY_ROLE_NAME);
				if ( dtFields.Rows.Count == 0 )
					dtFields = this.EditViewFields(sEDIT_NAME);
			}
			return dtFields;
		}

		// 05/05/2016 Paul.  The User Primary Role is used with role-based views. 
		public DataTable EditViewFields(string sEDIT_NAME, string sPRIMARY_ROLE_NAME, bool bSearchView)
		{
			DataTable dtFields = null;
			// 10/1/2017 Paul.  Role-based view will not be applied with the ArchiveView. 
			// 02/23/2018 Paul.  Allow roll-based view while in archive mode. 
			if ( Sql.IsEmptyString(sPRIMARY_ROLE_NAME) )
			{
				dtFields = this.EditViewFields(sEDIT_NAME, bSearchView);
			}
			else
			{
				dtFields = this.EditViewFields(sEDIT_NAME + "." + sPRIMARY_ROLE_NAME, bSearchView);
				if ( dtFields.Rows.Count == 0 )
					dtFields = this.EditViewFields(sEDIT_NAME, bSearchView);
			}
			return dtFields;
		}

		public DataTable EditViewFields(string sEDIT_NAME, bool bSearchView)
		{
			DataTable dt = Cache.Get("vwEDITVIEWS_FIELDS." + sEDIT_NAME + "." + bSearchView.ToString()) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 01/09/2006 Paul.  Exclude DEFAULT_VIEW. 
						// 10/13/2011 Paul.  Special list of EditViews for the search area with IS_MULTI_SELECT. 
						sSQL = "select *                     " + ControlChars.CrLf
						     + "  from " + (bSearchView ? "vwEDITVIEWS_FIELDS_SearchView" : "vwEDITVIEWS_FIELDS") + ControlChars.CrLf
						     + " where EDIT_NAME = @EDIT_NAME" + ControlChars.CrLf
						     + "   and (DEFAULT_VIEW = 0 or DEFAULT_VIEW is null)" + ControlChars.CrLf
						     + " order by FIELD_INDEX        " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@EDIT_NAME", sEDIT_NAME);
							
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwEDITVIEWS_FIELDS." + sEDIT_NAME + "." + bSearchView.ToString(), dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 11/21/2005 Paul. Ignore error, but then we need to find a way to display the connection error. 
					// The most likely failure here is a connection failure. 
					dt = new DataTable();
				}
			}
			return dt;
		}

		// 06/29/2012 Paul.  Business Rules need to be cleared after saving. 
		public void ClearEditViewRules(string sNAME)
		{
			Cache.Remove("vwEDITVIEWS_RULES." + sNAME);
		}

		// 05/10/2016 Paul.  The User Primary Role is used with role-based views. 
		public DataTable EditViewRules(string sNAME, string sPRIMARY_ROLE_NAME)
		{
			DataTable dt = null;
			// 10/1/2017 Paul.  Role-based view will not be applied with the ArchiveView. 
			// 02/23/2018 Paul.  Allow roll-based view while in archive mode. 
			if ( Sql.IsEmptyString(sPRIMARY_ROLE_NAME) )
			{
				dt = this.EditViewRules(sNAME);
			}
			else
			{
				dt = this.EditViewRules(sNAME + "." + sPRIMARY_ROLE_NAME);
				if ( dt.Rows.Count == 0 )
					dt = this.EditViewRules(sNAME);
			}
			return dt;
		}

		// 11/10/2010 Paul.  Apply Business Rules. 
		public DataTable EditViewRules(string sNAME)
		{
			DataTable dt = Cache.Get("vwEDITVIEWS_RULES." + sNAME) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select *                " + ControlChars.CrLf
						     + "  from vwEDITVIEWS_RULES" + ControlChars.CrLf
						     + " where NAME = @NAME     " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@NAME", sNAME);
							
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwEDITVIEWS_RULES." + sNAME, dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 11/21/2005 Paul. Ignore error, but then we need to find a way to display the connection error. 
					// The most likely failure here is a connection failure. 
					dt = new DataTable();
				}
			}
			return dt;
		}

		public void ClearDynamicButtons(string sVIEW_NAME)
		{
			Cache.Remove("vwDYNAMIC_BUTTONS." + sVIEW_NAME);
		}

		public DataTable DynamicButtons(string sVIEW_NAME)
		{
			DataTable dt = Cache.Get("vwDYNAMIC_BUTTONS." + sVIEW_NAME) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 01/09/2006 Paul.  Exclude DEFAULT_VIEW. 
						sSQL = "select *                     " + ControlChars.CrLf
						     + "  from vwDYNAMIC_BUTTONS     " + ControlChars.CrLf
						     + " where VIEW_NAME = @VIEW_NAME" + ControlChars.CrLf
						     + "   and (DEFAULT_VIEW = 0 or DEFAULT_VIEW is null)" + ControlChars.CrLf
						     + " order by CONTROL_INDEX      " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@VIEW_NAME", sVIEW_NAME);
							
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwDYNAMIC_BUTTONS." + sVIEW_NAME, dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 11/21/2005 Paul. Ignore error, but then we need to find a way to display the connection error. 
					// The most likely failure here is a connection failure. 
					dt = new DataTable();
				}
			}
			return dt;
		}

		public void ClearFieldsMetaData(string sMODULE_NAME)
		{
			Cache.Remove("vwFIELDS_META_DATA_Validated." + sMODULE_NAME);
			ClearSearchColumns(sMODULE_NAME);
		}

		// 09/09/2009 Paul.  Change the field name to be more obvious. 
		// 08/30/2016 Paul.  The Business Process Engine needs to be able to get the list of custom fields in a background thread. 
		public DataTable FieldsMetaData_Validated(string sTABLE_NAME)
		{
			DataTable dt = Cache.Get("vwFIELDS_META_DATA_Validated." + sTABLE_NAME) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select *                             " + ControlChars.CrLf
						     + "  from vwFIELDS_META_DATA_Validated  " + ControlChars.CrLf
						     + " where TABLE_NAME = @TABLE_NAME      " + ControlChars.CrLf
						     + " order by colid                      " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@TABLE_NAME", sTABLE_NAME);
							
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwFIELDS_META_DATA_Validated." + sTABLE_NAME, dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 11/21/2005 Paul. Ignore error, but then we need to find a way to display the connection error. 
					// The most likely failure here is a connection failure. 
					dt = new DataTable();
				}
			}
			return dt;
		}

		// 11/26/2009 Paul.  We need the actual table fields when sync'ing with the offline client. 
		public DataTable SqlColumns(string sTABLE_NAME)
		{
			DataTable dt = Cache.Get("vwSqlColumns." + sTABLE_NAME) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select ColumnName              " + ControlChars.CrLf
						     + "     , CsType                  " + ControlChars.CrLf
						     + "     , length                  " + ControlChars.CrLf
						     + "  from vwSqlColumns            " + ControlChars.CrLf
						     + " where ObjectName = @TABLE_NAME" + ControlChars.CrLf
						     + " order by colid                " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							// 09/02/2008 Paul.  Standardize the case of metadata tables to uppercase.  PostgreSQL defaults to lowercase. 
							Sql.AddParameter(cmd, "@TABLE_NAME", Sql.MetadataName(cmd, sTABLE_NAME));
							
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwSqlColumns." + sTABLE_NAME, dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 11/21/2005 Paul. Ignore error, but then we need to find a way to display the connection error. 
					// The most likely failure here is a connection failure. 
					dt = new DataTable();
				}
			}
			return dt;
		}

		// 05/15/2020 Paul.  The React Client needs to get the Edit fields for the EmailTemplates editor. 
		public DataTable SqlColumns(string sMODULE_NAME, string sMODE)
		{
			DataTable dt = Cache.Get("vwSqlColumns." + sMODULE_NAME + "." + sMODE) as DataTable;
			if ( dt == null )
			{
				string sTABLE_NAME = CrmModules.TableName(sMODULE_NAME);
				if ( !Sql.IsEmptyString(sTABLE_NAME) )
				{
					string sVIEW_NAME = "vw" + sTABLE_NAME;
					switch ( sMODE )
					{
						case "edit":  sVIEW_NAME += "_Edit";  break;
						case "list":  sVIEW_NAME += "_List";  break;
						case "view":  sVIEW_NAME += "_Edit";  break;
					}
					try
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							// 05/15/2020 Paul.  The field names should match those from ImportColumns(). 
							sSQL = "select ColumnName                " + ControlChars.CrLf
							     + "     , ColumnName as NAME        " + ControlChars.CrLf
							     + "     , ColumnName as DISPLAY_NAME" + ControlChars.CrLf
							     + "     , CsType     as ColumnType  " + ControlChars.CrLf
							     + "     , length     as Size        " + ControlChars.CrLf
							     + "     , colid                     " + ControlChars.CrLf
							     + "  from vwSqlColumns              " + ControlChars.CrLf
							     + " where ObjectName = @OBJECTNAME  " + ControlChars.CrLf
							     + " order by colid                  " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								// 09/02/2008 Paul.  Standardize the case of metadata tables to uppercase.  PostgreSQL defaults to lowercase. 
								Sql.AddParameter(cmd, "@OBJECTNAME", sVIEW_NAME);
								
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									dt = new DataTable();
									da.Fill(dt);
									Cache.Set("vwSqlColumns." + sMODULE_NAME + "." + sMODE, dt, DefaultCacheExpiration());
								}
							}
						}
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						// 11/21/2005 Paul. Ignore error, but then we need to find a way to display the connection error. 
						// The most likely failure here is a connection failure. 
						dt = new DataTable();
					}
				}
			}
			return dt;
		}

		// 05/25/2008 Paul.  We needed an easy way to get to the custom fields for tables not related to a module. 
		// 05/25/2008 Paul.  There is no automated clearing of this cache entry. The admin must manually perform a Reload. 
		public DataTable FieldsMetaData_UnvalidatedCustomFields(string sTABLE_NAME)
		{
			DataTable dt = Cache.Get("vwFIELDS_META_DATA_Unvalidated." + sTABLE_NAME) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select ColumnName   as NAME    " + ControlChars.CrLf
						     + "     , CsType       as CsType  " + ControlChars.CrLf
						     + "     , length       as MAX_SIZE" + ControlChars.CrLf
						     + "  from vwSqlColumns            " + ControlChars.CrLf
						     + " where ObjectName = @TABLE_NAME" + ControlChars.CrLf
						     + "   and ColumnName <> 'ID_C'    " + ControlChars.CrLf
						     + " order by colid                " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							// 09/02/2008 Paul.  Standardize the case of metadata tables to uppercase.  PostgreSQL defaults to lowercase. 
							Sql.AddParameter(cmd, "@TABLE_NAME", Sql.MetadataName(cmd, sTABLE_NAME + "_CSTM"));
							
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwFIELDS_META_DATA_Unvalidated." + sTABLE_NAME, dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 11/21/2005 Paul. Ignore error, but then we need to find a way to display the connection error. 
					// The most likely failure here is a connection failure. 
					dt = new DataTable();
				}
			}
			return dt;
		}

		public DataTable ForumTopics()
		{
			DataTable dt = Cache.Get("vwFORUM_TOPICS_LISTBOX") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select NAME                    " + ControlChars.CrLf
						     + "  from vwFORUM_TOPICS_LISTBOX  " + ControlChars.CrLf
						     + " order by LIST_ORDER           " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwFORUM_TOPICS_LISTBOX", dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		public void ClearSavedSearch(string sMODULE)
		{
			Session.Remove("vwSAVED_SEARCH." + sMODULE);
			Session.Remove("vwSAVED_SEARCH.ReactClient");
		}

		public DataTable SavedSearch(string sMODULE)
		{
			DataTable dt = Session.GetTable("vwSAVED_SEARCH." + sMODULE);
			if ( dt == null )
			{
				dt = new DataTable();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							// 07/29/2008 Paul.  A global saved search is one where ASSIGNED_USER_ID is null. 
							// 09/01/2010 Paul.  Store a copy of the DEFAULT_SEARCH_ID in the table so that we don't need to read the XML in order to get the value. 
							sSQL = "select ID                         " + ControlChars.CrLf
							     + "     , NAME                       " + ControlChars.CrLf
							     + "     , CONTENTS                   " + ControlChars.CrLf
							     + "     , DEFAULT_SEARCH_ID          " + ControlChars.CrLf
							     + "  from vwSAVED_SEARCH             " + ControlChars.CrLf
							     + " where (ASSIGNED_USER_ID = @USER_ID or ASSIGNED_USER_ID is null)" + ControlChars.CrLf
							     + "   and SEARCH_MODULE    = @MODULE " + ControlChars.CrLf
							     + " order by NAME                    " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@USER_ID", Security.USER_ID);
								Sql.AddParameter(cmd, "@MODULE" , sMODULE         );
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dt);
									Session.SetTable("vwSAVED_SEARCH." + sMODULE, dt);
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 11/21/2005 Paul. Ignore error, but then we need to find a way to display the connection error. 
					// The most likely failure here is a connection failure. 
				}
			}
			return dt;
		}

		// 04/28/2019 Paul.  Flag to include Favorites and LastViewed for the React client. 
		public Dictionary<string, object> GetAllSavedSearch(List<string> lstMODULES)
		{
			Dictionary<string, object> objs = Session["vwSAVED_SEARCH.ReactClient"] as Dictionary<string, object>;
			if ( objs == null )
			{
				objs = new Dictionary<string, object>();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL = String.Empty;
							sSQL = "select ID                         " + ControlChars.CrLf
							     + "     , NAME                       " + ControlChars.CrLf
							     + "     , CONTENTS                   " + ControlChars.CrLf
							     + "     , DEFAULT_SEARCH_ID          " + ControlChars.CrLf
							     + "     , SEARCH_MODULE              " + ControlChars.CrLf
							     + "  from vwSAVED_SEARCH             " + ControlChars.CrLf
							     + " where (ASSIGNED_USER_ID = @USER_ID or ASSIGNED_USER_ID is null)" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@USER_ID", Security.USER_ID);
								// 05/07/2019 Paul.  We cannot filter by modules as some are Accounts.SearchHome. 
								//Sql.AppendParameter(cmd, lstMODULES.ToArray(), "SEARCH_MODULE");
								cmd.CommandText += " order by SEARCH_MODULE, NAME" + ControlChars.CrLf;
							
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dt = new DataTable() )
									{
										da.Fill(dt);
										// 05/08/2019 Paul.  The saved searches need to be returned as an array per module with the default (NAME is null) on top. 
										string sLAST_MODULE = String.Empty;
										List<object> arrModuleSearch = new List<object>();
										foreach ( DataRow row in dt.Rows )
										{
											string sSEARCH_MODULE     = Sql.ToString(row["SEARCH_MODULE"    ]);
											Guid   gID                = Sql.ToGuid  (row["ID"               ]);
											string sNAME              = Sql.ToString(row["NAME"             ]);
											string sCONTENTS          = Sql.ToString(row["CONTENTS"         ]);
											Guid   gDEFAULT_SEARCH_ID = Sql.ToGuid  (row["DEFAULT_SEARCH_ID"]);
											
											string sMODULE_NAME = sSEARCH_MODULE.Split('.')[0];
											// 01/08/2021 Paul.  A home page dashlet saves the layout name with the module name. 
											if ( sSEARCH_MODULE.EndsWith(".SearchHome") )
												sMODULE_NAME = sSEARCH_MODULE;
											if ( sLAST_MODULE != sMODULE_NAME )
											{
												sLAST_MODULE = sMODULE_NAME;
												arrModuleSearch = new List<object>();
												objs[sMODULE_NAME] = arrModuleSearch;
											}
											Dictionary<string, object> obj = new Dictionary<string, object>();
											obj["ID"               ] = gID               ;
											obj["NAME"             ] = sNAME             ;
											obj["CONTENTS"         ] = sCONTENTS         ;
											obj["DEFAULT_SEARCH_ID"] = gDEFAULT_SEARCH_ID;
											arrModuleSearch.Add(obj);
										}
										// 01/08/2021 Paul.  Should be saving the objects not the data table. 
										Session["vwSAVED_SEARCH.ReactClient"] = objs;
									}
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					// 04/28/2019 Paul.  This error is not critical, so we can just log and ignore. 
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return objs;
		}

		public void ClearSearchColumns(string sVIEW_NAME)
		{
			Cache.Remove("vwSqlColumns_Searching." + sVIEW_NAME);
		}

		public DataTable SearchColumns(string sVIEW_NAME)
		{
			DataTable dt = Cache.Get("vwSqlColumns_Searching." + sVIEW_NAME) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 02/29/2008 Niall.  Some SQL Server 2005 installations require matching case for the parameters. 
						// Since we force the parameter to be uppercase, we must also make it uppercase in the command text. 
						sSQL = "select *                       " + ControlChars.CrLf
						     + "  from vwSqlColumns_Searching  " + ControlChars.CrLf
						     + " where ObjectName = @OBJECTNAME" + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							// 09/02/2008 Paul.  Standardize the case of metadata tables to uppercase.  PostgreSQL defaults to lowercase. 
							Sql.AddParameter(cmd, "@OBJECTNAME", Sql.MetadataName(cmd, sVIEW_NAME));
							// 04/16/2009 Paul.  A customer did not want to see all fields, just the ones that were available in the EditView. 
							if ( Sql.ToBoolean(Application["CONFIG.search_editable_columns_only"]) )
							{
								// 04/17/2009 Paul.  Key off of the view name so that we don't have to change other areas of the code. 
								cmd.CommandText += "   and ColumnName in (select DATA_FIELD from vwEDITVIEWS_FIELDS_Searching where VIEW_NAME = @VIEW_NAME)" + ControlChars.CrLf;
								Sql.AddParameter(cmd, "@VIEW_NAME", sVIEW_NAME);
							}
							cmd.CommandText += " order by colid                " + ControlChars.CrLf;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								// 06/10/2006 Paul.  The default sliding scale is not appropriate as columns can be added. 
								Cache.Set("vwSqlColumns_Searching." + sVIEW_NAME, dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		public Dictionary<string, object> GetAllSearchColumns(List<string> lstMODULES)
		{
			// 08/09/2020 Paul.  Convert to comma separated string. 
			string sModuleList = String.Join(",", lstMODULES.ToArray());
			Dictionary<string, object> objs = Cache.Get("vwMODULES.Columns.ReactClient." + sModuleList) as Dictionary<string, object>;
			if ( objs == null )
			{
				objs = new Dictionary<string, object>();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						foreach ( string sMODULE_NAME in lstMODULES )
						{
							string sTABLE_NAME = CrmModules.TableName(sMODULE_NAME);
							if ( !Sql.IsEmptyString(sTABLE_NAME) )
							{
								List<object> arrModuleColumns = new List<object>();
								// 10/14/2020 Paul.  We need to use a copy as we will be modifying the database. 
								using ( DataTable dt = this.SearchColumns("vw" + sTABLE_NAME).Copy() )
								{
									foreach ( DataRow row in dt.Rows )
									{
										string sNAME         = Sql.ToString(row["NAME"        ]);
										string sDISPLAY_NAME = Sql.ToString(row["DISPLAY_NAME"]);
										row["DISPLAY_NAME"] = Utils.TableColumnName(L10n, sMODULE_NAME, sDISPLAY_NAME);
									}
									// 08/31/2019 Paul.  Presort so that we don't have to on the client. 
									DataView vw = new DataView(dt);
									vw.Sort = "DISPLAY_NAME";
									foreach ( DataRowView row in vw )
									{
										Dictionary<string, object> obj = new Dictionary<string, object>();
										obj["NAME"        ] = Sql.ToString(row["NAME"        ]);
										obj["DISPLAY_NAME"] = Sql.ToString(row["DISPLAY_NAME"]);
										arrModuleColumns.Add(obj);
									}
								}
								objs[sMODULE_NAME] = arrModuleColumns;
							}
						}
						Cache.Set("vwMODULES.Columns.ReactClient." + sModuleList, objs, DefaultCacheExpiration());
					}
				}
				catch(Exception ex)
				{
					// 04/28/2019 Paul.  This error is not critical, so we can just log and ignore. 
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return objs;
		}

		public void ClearEmailGroups()
		{
			Cache.Remove("vwUSERS_Groups");
		}

		public DataTable EmailGroups()
		{
			DataTable dt = Cache.Get("vwUSERS_Groups") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select *             " + ControlChars.CrLf
						     + "  from vwUSERS_Groups" + ControlChars.CrLf
						     + " order by NAME       " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwUSERS_Groups", dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		public void ClearInboundEmails()
		{
			Cache.Remove("vwINBOUND_EMAILS_Bounce");
			Cache.Remove("vwINBOUND_EMAILS_Monitored");
		}


		// 02/16/2008 Paul.  InboundEmailBounce needs to be called from the scheduler, so the application must be provided. 
		// 10/27/2008 Paul.  Pass the context instead of the Application so that more information will be available to the error handling. 
		public DataTable InboundEmailBounce()
		{
			// 02/16/2008 Paul.  When called from the scheduler, the context is not available, so get the cache from HttpRuntime. 
			DataTable dt = Cache.Get("vwINBOUND_EMAILS_Bounce") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select *                      " + ControlChars.CrLf
						     + "  from vwINBOUND_EMAILS_Bounce" + ControlChars.CrLf
						     + " order by NAME                " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwINBOUND_EMAILS_Bounce", dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemMessage("Error", new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		// 02/16/2008 Paul.  InboundEmailMonitored needs to be called from the scheduler, so the application must be provided. 
		// 10/27/2008 Paul.  Pass the context instead of the Application so that more information will be available to the error handling. 
		public DataTable InboundEmailMonitored()
		{
			// 02/16/2008 Paul.  When called from the scheduler, the context is not available, so get the cache from HttpRuntime. 
			DataTable dt = Cache.Get("vwINBOUND_EMAILS_Monitored") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select *                       " + ControlChars.CrLf
						     + "  from vwINBOUND_EMAILS_Monitored" + ControlChars.CrLf
						     + " order by NAME                 " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwINBOUND_EMAILS_Monitored", dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemMessage("Error", new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		// 11/18/2008 Paul.  Teams can be used in the search panels. 
		public DataTable Teams()
		{
			DataTable dt = null;
			// 11/25/2006 Paul.  An admin can see all teams, but most users will only see the teams which they are assigned to. 
			if ( Security.IS_ADMIN )
			{
				dt = Cache.Get("vwTEAMS") as DataTable;
				if ( dt == null )
				{
					try
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							sSQL = "select ID     " + ControlChars.CrLf
							     + "     , NAME   " + ControlChars.CrLf
							     + "  from vwTEAMS" + ControlChars.CrLf
							     + " order by NAME" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									dt = new DataTable();
									da.Fill(dt);
									Cache.Set("vwTEAMS", dt, DefaultCacheExpiration());
								}
							}
						}
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					}
				}
			}
			else
			{
				dt = Session.GetTable("vwTEAMS_MyList");
				if ( dt == null )
				{
					try
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							sSQL = "select ID                                      " + ControlChars.CrLf
							     + "     , NAME                                    " + ControlChars.CrLf
							     + "  from vwTEAMS_MyList                          " + ControlChars.CrLf
							     + " where MEMBERSHIP_USER_ID = @MEMBERSHIP_USER_ID" + ControlChars.CrLf
							     + " order by NAME                                 " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@MEMBERSHIP_USER_ID", Security.USER_ID);
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									dt = new DataTable();
									da.Fill(dt);
									Session.SetTable("vwTEAMS_MyList", dt);
								}
							}
						}
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					}
				}
			}
			return dt;
		}

		// 12/23/2019 Paul.  Return the team tree as an object tree instead of XML. 
		[Serializable()]
		public class ReactTeam
		{
			public string     ID   ;
			public string     NAME ;
			public List<ReactTeam> TEAMS;

			public void ProcessNodes(XmlNode xTeam)
			{
				XmlNode xID   = xTeam.Attributes.GetNamedItem("ID"  );
				XmlNode xNAME = xTeam.Attributes.GetNamedItem("NAME");
				if ( xID != null )
					this.ID   = Sql.ToString(xID  .Value);
				if ( xNAME != null )
					this.NAME = Sql.ToString(xNAME.Value);
				if ( xTeam.ChildNodes != null && xTeam.ChildNodes.Count > 0 )
				{
					this.TEAMS = new List<ReactTeam>();
					foreach ( XmlNode xTEAM in xTeam.ChildNodes )
					{
						ReactTeam team = new ReactTeam();
						team.ProcessNodes(xTEAM);
						this.TEAMS.Add(team);
					}
				}
			}
		}

		// 12/23/2019 Paul.  Return the team tree as an object tree instead of XML. 
		public List<object> GetUserTeamTree()
		{
			List<object> lstTeams = Session["REACT_TEAM_TREE." + Security.USER_ID.ToString()] as List<object>;
			bool bEnableTeamManagement = Config.enable_team_management();
			bool bEnableTeamHierarchy  = Config.enable_team_hierarchy();
			if ( lstTeams == null && bEnableTeamManagement && bEnableTeamHierarchy )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using (IDbConnection con = dbf.CreateConnection())
					{
						string sSQL;
						string sRootName = L10n.Term("Teams.LBL_TEAM_TREE_ROOT").Replace("'", "");
						sSQL = "select TEAMS.ID                          as '@ID'       " + ControlChars.CrLf
						     + "     , replace(TEAMS.NAME, '&', '&amp;') as '@NAME'     " + ControlChars.CrLf
						     + "     , TEAMS.PARENT_ID                   as '@PARENT_ID'" + ControlChars.CrLf
						     + "     , dbo.fnTEAM_HIERARCHY_ChildrenXml(TEAMS.ID)       " + ControlChars.CrLf
						     + "  from      TEAMS                                       " + ControlChars.CrLf
						     + " inner join TEAM_MEMBERSHIPS                            " + ControlChars.CrLf
						     + "         on TEAM_MEMBERSHIPS.TEAM_ID = TEAMS.ID         " + ControlChars.CrLf
						     + " where TEAM_MEMBERSHIPS.USER_ID         = @USER_ID      " + ControlChars.CrLf
						     + "   and TEAM_MEMBERSHIPS.EXPLICIT_ASSIGN = 1             " + ControlChars.CrLf
						     + "   and TEAM_MEMBERSHIPS.DELETED         = 0             " + ControlChars.CrLf
						     + "   and TEAMS.PRIVATE = 0                                " + ControlChars.CrLf
						     + "   and TEAMS.DELETED = 0                                " + ControlChars.CrLf
						     + " order by '@NAME'                                       " + ControlChars.CrLf
						     + "   for xml path('TEAM'), root('" + sRootName + "'), type" + ControlChars.CrLf;
						using (IDbCommand cmd = con.CreateCommand())
						{
							con.Open();
							cmd.CommandText = sSQL;
							Guid gUSER_ID = Security.USER_ID;
							Sql.AddParameter(cmd, "@USER_ID", gUSER_ID);
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								StringBuilder sbXML = new StringBuilder();
								using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
								{
									while ( rdr.Read() )
									{
										sbXML.Append(Sql.ToString(rdr[0]));
									}
								}
								string sXML = sbXML.ToString();
								if ( !Sql.IsEmptyString(sXML) )
								{
									lstTeams = new List<object>();

									XmlDocument xml = new XmlDocument();
									xml.LoadXml(sXML);
									XmlNodeList nlTeams = xml.DocumentElement.ChildNodes;
									foreach ( XmlNode xTEAM in nlTeams )
									{
										ReactTeam team = new ReactTeam();
										team.ProcessNodes(xTEAM);
										lstTeams.Add(team);
									}
								}
								Session["REACT_TEAM_TREE." + Security.USER_ID.ToString()] = lstTeams;
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return lstTeams;
		}

		// 05/12/2016 Paul.  Tags can be used in the search panels. 
		public DataTable Tags()
		{
			DataTable dt = null;
			dt = Cache.Get("vwTAGS") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select ID     " + ControlChars.CrLf
						     + "     , NAME   " + ControlChars.CrLf
						     + "  from vwTAGS " + ControlChars.CrLf
						     + " order by NAME" + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwTAGS", dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		public DataTable ACLFieldAliases()
		{
			DataTable dt = Cache.Get("vwACL_FIELDS_ALIASES") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select *                   " + ControlChars.CrLf
						     + "  from vwACL_FIELDS_ALIASES" + ControlChars.CrLf
						     + " order by MODULE_NAME, NAME" + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwACL_FIELDS_ALIASES", dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemMessage("Error", new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		public void ClearReport(Guid gID)
		{
			string sID = gID.ToString();
			//Cache.Remove("vwREPORTS." + sID);
			//Cache.Remove("vwREPORTS.Parameters." + sID);
			//Cache.Remove("vwREPORTS.Parameters.EditView." + sID);
			foreach(string sKey in GetCacheKeys())
			{
				// 05/03/2011 Paul.  We need to include the USER_ID because we cache the Assigned User ID and the Team ID. 
				if ( sKey.StartsWith("vwREPORTS.") && sKey.Contains(sID) )
					Cache.Remove(sKey);
			}
		}

		// 05/03/2011 Paul.  We need to include the USER_ID because we cache the Assigned User ID and the Team ID. 
		public DataTable ReportParametersEditView(Guid gID, Guid gUSER_ID)
		{
			DataTable dt = Cache.Get("vwREPORTS.Parameters.EditView." + gID.ToString() + "." + gUSER_ID.ToString()) as DataTable;
			if ( dt == null )
			{
				dt = new DataTable();
				dt.Columns.Add("EDIT_NAME"                   , typeof(String ) );
				dt.Columns.Add("FIELD_INDEX"                 , typeof(Int32  ) );
				dt.Columns.Add("FIELD_TYPE"                  , typeof(String ) );
				dt.Columns.Add("DATA_LABEL"                  , typeof(String ) );
				dt.Columns.Add("DATA_FIELD"                  , typeof(String ) );
				dt.Columns.Add("DATA_FORMAT"                 , typeof(String ) );
				dt.Columns.Add("DISPLAY_FIELD"               , typeof(String ) );
				dt.Columns.Add("CACHE_NAME"                  , typeof(String ) );
				dt.Columns.Add("DATA_REQUIRED"               , typeof(Boolean) );
				dt.Columns.Add("UI_REQUIRED"                 , typeof(Boolean) );
				dt.Columns.Add("ONCLICK_SCRIPT"              , typeof(String ) );
				dt.Columns.Add("FORMAT_SCRIPT"               , typeof(String ) );
				dt.Columns.Add("FORMAT_TAB_INDEX"            , typeof(Int16  ) );
				dt.Columns.Add("FORMAT_MAX_LENGTH"           , typeof(Int32  ) );
				dt.Columns.Add("FORMAT_SIZE"                 , typeof(Int32  ) );
				dt.Columns.Add("FORMAT_ROWS"                 , typeof(Int32  ) );
				dt.Columns.Add("FORMAT_COLUMNS"              , typeof(Int32  ) );
				dt.Columns.Add("COLSPAN"                     , typeof(Int32  ) );
				dt.Columns.Add("ROWSPAN"                     , typeof(Int32  ) );
				dt.Columns.Add("LABEL_WIDTH"                 , typeof(String ) );
				dt.Columns.Add("FIELD_WIDTH"                 , typeof(String ) );
				dt.Columns.Add("DATA_COLUMNS"                , typeof(Int32  ) );
				dt.Columns.Add("MODULE_TYPE"                 , typeof(String ) );
				dt.Columns.Add("RELATED_SOURCE_MODULE_NAME"  , typeof(String ) );
				dt.Columns.Add("RELATED_SOURCE_VIEW_NAME"    , typeof(String ) );
				dt.Columns.Add("RELATED_SOURCE_ID_FIELD"     , typeof(String ) );
				dt.Columns.Add("RELATED_SOURCE_NAME_FIELD"   , typeof(String ) );
				dt.Columns.Add("RELATED_VIEW_NAME"           , typeof(String ) );
				dt.Columns.Add("RELATED_ID_FIELD"            , typeof(String ) );
				dt.Columns.Add("RELATED_NAME_FIELD"          , typeof(String ) );
				dt.Columns.Add("RELATED_JOIN_FIELD"          , typeof(String ) );
				dt.Columns.Add("PARENT_FIELD"                , typeof(String ) );
				dt.Columns.Add("FIELD_VALIDATOR_MESSAGE"     , typeof(String ) );
				dt.Columns.Add("VALIDATION_TYPE"             , typeof(String ) );
				dt.Columns.Add("REGULAR_EXPRESSION"          , typeof(String ) );
				dt.Columns.Add("DATA_TYPE"                   , typeof(String ) );
				dt.Columns.Add("MININUM_VALUE"               , typeof(String ) );
				dt.Columns.Add("MAXIMUM_VALUE"               , typeof(String ) );
				dt.Columns.Add("COMPARE_OPERATOR"            , typeof(String ) );
				dt.Columns.Add("TOOL_TIP"                    , typeof(String ) );
				
				try
				{
					// 01/15/2013 Paul.  A customer wants to be able to change the assigned user as this was previously allowed in report prompts. 
					bool bShowAssignedUser = false;
						bShowAssignedUser = Sql.ToBoolean(Application["CONFIG.Reports.ShowAssignedUser"]);
					// 05/03/2011 Paul.  We need to include the USER_ID because we cache the Assigned User ID and the Team ID. 
					DataTable dtReportParameters = this.ReportParameters(gID, gUSER_ID);
					if ( dtReportParameters != null && dtReportParameters.Rows.Count > 0 )
					{
						string sMODULE_NAME = Sql.ToString(dtReportParameters.Rows[0]["MODULE_NAME"]);
						DataTable dtEditView = this.EditViewFields(sMODULE_NAME + ".EditView");
						foreach ( DataRow rowParameter in dtReportParameters.Rows )
						{
							// 03/09/2012 Paul.  Making the data field upper case will simplify tests later. 
							string sDATA_FIELD    = Sql.ToString(rowParameter["NAME"         ]).ToUpper();
							string sDATA_LABEL    = Sql.ToString(rowParameter["PROMPT"       ]);
							string sDATA_TYPE     = Sql.ToString(rowParameter["DATA_TYPE"    ]);
							string sDEFAULT_VALUE = Sql.ToString(rowParameter["DEFAULT_VALUE"]);
							bool   bHIDDEN        = Sql.ToBoolean(rowParameter["HIDDEN"      ]);
							// 02/16/2012 Paul.  We need a separate list for report parameter lists. 
							string sDATA_SET_NAME = Sql.ToString (rowParameter["DATA_SET_NAME"]);
							bool   bMULTI_VALUE   = Sql.ToBoolean(rowParameter["MULTI_VALUE"  ]);
							bool bFieldFound = false;
							// 04/09/2011 Paul.  ID is not allowed as a parameter because it is used by the Report ID in the Dashlet. 
							// 02/03/2012 Paul.  Add support for the Hidden flag. 
							if ( sDATA_FIELD == "ID" || bHIDDEN )
								continue;
							foreach ( DataRow rowEditView in dtEditView.Rows )
							{
								// 03/09/2012 Paul.  ASSIGNED_USER_ID is a special parameter that is not a prompt. 
								if ( sDATA_FIELD == Sql.ToString(rowEditView["DATA_FIELD"]) && sDATA_FIELD != "ASSIGNED_USER_ID" && sDATA_FIELD != "TEAM_ID" )
								{
									bFieldFound = true;
									DataRow row = dt.NewRow();
									dt.Rows.Add(row);
									// 07/29/2012 Paul.  Since we are searching, we should follow SearchView rules as we don't want field level security to prevent editing. 
									row["EDIT_NAME"                 ] = sMODULE_NAME + ".SearchView";
									row["FIELD_INDEX"               ] = dt.Rows.Count;
									row["FIELD_TYPE"                ] = rowEditView["FIELD_TYPE"                ];
									// 03/04/2012 Paul.  Custom label was being applied to field, not label. 
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : Sql.ToString(rowEditView["DATA_LABEL"]);
									row["DATA_FIELD"                ] = rowEditView["DATA_FIELD"                ];
									row["DATA_FORMAT"               ] = rowEditView["DATA_FORMAT"               ];
									row["DISPLAY_FIELD"             ] = rowEditView["DISPLAY_FIELD"             ];
									row["CACHE_NAME"                ] = rowEditView["CACHE_NAME"                ];
									row["DATA_REQUIRED"             ] = rowEditView["DATA_REQUIRED"             ];
									row["UI_REQUIRED"               ] = rowEditView["UI_REQUIRED"               ];
									row["ONCLICK_SCRIPT"            ] = rowEditView["ONCLICK_SCRIPT"            ];
									row["FORMAT_SCRIPT"             ] = rowEditView["FORMAT_SCRIPT"             ];
									row["FORMAT_TAB_INDEX"          ] = rowEditView["FORMAT_TAB_INDEX"          ];
									row["FORMAT_MAX_LENGTH"         ] = rowEditView["FORMAT_MAX_LENGTH"         ];
									row["FORMAT_SIZE"               ] = rowEditView["FORMAT_SIZE"               ];
									row["FORMAT_ROWS"               ] = rowEditView["FORMAT_ROWS"               ];
									row["FORMAT_COLUMNS"            ] = rowEditView["FORMAT_COLUMNS"            ];
									//row["COLSPAN"                   ] = rowEditView["COLSPAN"                   ];
									//row["ROWSPAN"                   ] = rowEditView["ROWSPAN"                   ];
									row["LABEL_WIDTH"               ] = rowEditView["LABEL_WIDTH"               ];
									row["FIELD_WIDTH"               ] = rowEditView["FIELD_WIDTH"               ];
									row["DATA_COLUMNS"              ] = rowEditView["DATA_COLUMNS"              ];
									row["MODULE_TYPE"               ] = rowEditView["MODULE_TYPE"               ];
									//row["RELATED_SOURCE_MODULE_NAME"] = rowEditView["RELATED_SOURCE_MODULE_NAME"];
									//row["RELATED_SOURCE_VIEW_NAME"  ] = rowEditView["RELATED_SOURCE_VIEW_NAME"  ];
									//row["RELATED_SOURCE_ID_FIELD"   ] = rowEditView["RELATED_SOURCE_ID_FIELD"   ];
									//row["RELATED_SOURCE_NAME_FIELD" ] = rowEditView["RELATED_SOURCE_NAME_FIELD" ];
									//row["RELATED_VIEW_NAME"         ] = rowEditView["RELATED_VIEW_NAME"         ];
									//row["RELATED_ID_FIELD"          ] = rowEditView["RELATED_ID_FIELD"          ];
									//row["RELATED_NAME_FIELD"        ] = rowEditView["RELATED_NAME_FIELD"        ];
									//row["RELATED_JOIN_FIELD"        ] = rowEditView["RELATED_JOIN_FIELD"        ];
									//row["PARENT_FIELD"              ] = rowEditView["PARENT_FIELD"              ];
									row["FIELD_VALIDATOR_MESSAGE"   ] = rowEditView["FIELD_VALIDATOR_MESSAGE"   ];
									row["VALIDATION_TYPE"           ] = rowEditView["VALIDATION_TYPE"           ];
									row["REGULAR_EXPRESSION"        ] = rowEditView["REGULAR_EXPRESSION"        ];
									row["DATA_TYPE"                 ] = rowEditView["DATA_TYPE"                 ];
									row["MININUM_VALUE"             ] = rowEditView["MININUM_VALUE"             ];
									row["MAXIMUM_VALUE"             ] = rowEditView["MAXIMUM_VALUE"             ];
									row["COMPARE_OPERATOR"          ] = rowEditView["COMPARE_OPERATOR"          ];
									row["TOOL_TIP"                  ] = rowEditView["TOOL_TIP"                  ];
									// 03/04/2012 Paul.  Apply MultiValue flag if set in the report. 
									if ( Sql.ToString(row["FIELD_TYPE"]) == "ListBox" && bMULTI_VALUE )
										row["FORMAT_ROWS"] = 4;
									// 03/24/2018 Paul.  It does not make sense to use the mandatory field flag on an report serach. 
									row["UI_REQUIRED"] = false;
									// 04/28/2018 Paul.  We need to make sure to treat data sets as a list. 
									if ( !Sql.IsEmptyString(sDATA_SET_NAME) )
									{
										row["FIELD_TYPE"                ] = "ListBox";
										row["CACHE_NAME"                ] = sDATA_SET_NAME;
										row["MODULE_TYPE"               ] = DBNull.Value;
										if ( bMULTI_VALUE )
											row["FORMAT_ROWS"] = 4;
									}
									break;
								}
							}
							// 03/09/2012 Paul.  ASSIGNED_USER_ID is a special parameter that is not a prompt. 
							// 01/15/2013 Paul.  A customer wants to be able to change the assigned user as this was previously allowed in report prompts. 
							if ( !bFieldFound && (bShowAssignedUser || (sDATA_FIELD != "ASSIGNED_USER_ID" && sDATA_FIELD != "TEAM_ID")) )
							{
								DataRow row = dt.NewRow();
								dt.Rows.Add(row);
								// 07/29/2012 Paul.  Since we are searching, we should follow SearchView rules as we don't want field level security to prevent editing. 
								row["EDIT_NAME"                 ] = sMODULE_NAME + ".SearchView";
								row["FIELD_INDEX"               ] = dt.Rows.Count;
								// 03/06/2012 Paul.  A report parameter can have a special Date Rule field. 
								if ( sDATA_FIELD == "DATE_RULE" )
								{
									row["FIELD_TYPE"                ] = "ListBox";
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : sMODULE_NAME + ".LBL_" + sDATA_FIELD;
									row["DATA_FIELD"                ] = sDATA_FIELD;
									row["CACHE_NAME"                ] = "date_rule_dom";
								}
								// 11/15/2011 Paul.  If the value starts with a equals, then this is a formula and should not be treated as a date control. 
								// 02/16/2018 Paul.  After calculating the value, also update the UI as it looks ugly to have formula in the field. 
								else if ( sDEFAULT_VALUE.StartsWith("=") && !(sDATA_TYPE == "DateTime" || sDATA_FIELD.StartsWith("DATE_") || sDATA_FIELD.EndsWith("_DATE") || sDATA_FIELD.Contains("_DATE_") || sDATA_FIELD.StartsWith("DATETIME_") || sDATA_FIELD.EndsWith("_DATETIME") || sDATA_FIELD.Contains("_DATETIME_")) )
								{
									row["FIELD_TYPE"                ] = "TextBox";
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : sMODULE_NAME + ".LBL_" + sDATA_FIELD;
									row["DATA_FIELD"                ] = sDATA_FIELD;
								}
								// 02/16/2012 Paul.  We need a separate list for report parameter lists. 
								// 04/28/2018 Paul.  Data Set has higher priority than _ID type. 
								else if ( !Sql.IsEmptyString(sDATA_SET_NAME) )
								{
									row["FIELD_TYPE"                ] = "ListBox";
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : sMODULE_NAME + ".LBL_" + sDATA_FIELD;
									row["DATA_FIELD"                ] = sDATA_FIELD;
									row["CACHE_NAME"                ] = sDATA_SET_NAME;
									if ( bMULTI_VALUE )
										row["FORMAT_ROWS"] = 4;
								}
								else if ( sDATA_FIELD.EndsWith("_ID") )
								{
									// 01/15/2013 Paul.  If the field ends in ID, then the module name must be determined from the field. 
									string sPOPUP_TABLE_NAME = sDATA_FIELD.Substring(0, sDATA_FIELD.Length - 3);
									if ( sDATA_FIELD == "ASSIGNED_USER_ID" )
										sPOPUP_TABLE_NAME = "USERS";
									else if ( sPOPUP_TABLE_NAME.EndsWith("Y") )
										sPOPUP_TABLE_NAME = sDATA_FIELD.Substring(0, sDATA_FIELD.Length - 4) + "IES";
									else if ( sPOPUP_TABLE_NAME != "PROJECT" && sPOPUP_TABLE_NAME != "PROJECT_TASK" )
										sPOPUP_TABLE_NAME += "S";
									string sPOPUP_MODULE_NAME = CrmModules.ModuleName(sPOPUP_TABLE_NAME);
									row["FIELD_TYPE"                ] = "ModulePopup";
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : sPOPUP_MODULE_NAME + ".LBL_" + sDATA_FIELD;
									row["DATA_FIELD"                ] = sDATA_FIELD;
									row["MODULE_TYPE"               ] = sPOPUP_MODULE_NAME;
									row["DISPLAY_FIELD"             ] = sDATA_FIELD.Substring(0, sDATA_FIELD.Length - 2) + "NAME";
									// 04/09/2011 Paul.  Auto-submit the selection. 
									// Auto-submit is not working because we need to hit the actual submit button in order to get parameter processing. 
									//row["DATA_FORMAT"               ] = "1";
								}
								else if ( sDATA_FIELD.StartsWith("DATETIME_") || sDATA_FIELD.EndsWith("_DATETIME") || sDATA_FIELD.Contains("_DATETIME_") )
								{
									row["FIELD_TYPE"                ] = "DateTimePicker";
									// 04/11/2016 Paul.  Use BuildTermName. 
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : Utils.BuildTermName(sMODULE_NAME, sDATA_FIELD);
									row["DATA_FIELD"                ] = sDATA_FIELD;
									// 02/16/2018 Paul.  Special support for between clause as a parameter. Needed to be separated into 2 report parameters. 
									if ( bMULTI_VALUE )
									{
										DataRow row2 = dt.NewRow();
										dt.Rows.Add(row2);
										for ( int i = 0; i < dt.Columns.Count; i++ )
										{
											row2[i] = row[i];
										}
										row ["DATA_FIELD"] = sDATA_FIELD + "_AFTER" ;
										row2["DATA_FIELD"] = sDATA_FIELD + "_BEFORE";
										if ( Session != null )
										{
											row ["DATA_LABEL"] =  L10n.Term(Utils.BuildTermName(sMODULE_NAME, sDATA_FIELD)) + " " + L10n.Term("SavedSearch.LBL_SEARCH_AFTER" );
											row2["DATA_LABEL"] =  L10n.Term(Utils.BuildTermName(sMODULE_NAME, sDATA_FIELD)) + " " + L10n.Term("SavedSearch.LBL_SEARCH_BEFORE");
										}
									}
								}
								else if ( sDATA_TYPE == "DateTime" || sDATA_FIELD.StartsWith("DATE_") || sDATA_FIELD.EndsWith("_DATE") || sDATA_FIELD.Contains("_DATE_") )
								{
									row["FIELD_TYPE"                ] = "DatePicker";
									// 04/11/2016 Paul.  Use BuildTermName. 
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : Utils.BuildTermName(sMODULE_NAME, sDATA_FIELD);
									row["DATA_FIELD"                ] = sDATA_FIELD;
									// 04/11/2016 Paul.  Special support for between clause as a parameter. Needed to be separated into 2 report parameters. 
									if ( bMULTI_VALUE )
									{
										DataRow row2 = dt.NewRow();
										dt.Rows.Add(row2);
										for ( int i = 0; i < dt.Columns.Count; i++ )
										{
											row2[i] = row[i];
										}
										row ["DATA_FIELD"] = sDATA_FIELD + "_AFTER" ;
										row2["DATA_FIELD"] = sDATA_FIELD + "_BEFORE";
										if ( Session != null )
										{
											row ["DATA_LABEL"] =  L10n.Term(Utils.BuildTermName(sMODULE_NAME, sDATA_FIELD)) + " " + L10n.Term("SavedSearch.LBL_SEARCH_AFTER" );
											row2["DATA_LABEL"] =  L10n.Term(Utils.BuildTermName(sMODULE_NAME, sDATA_FIELD)) + " " + L10n.Term("SavedSearch.LBL_SEARCH_BEFORE");
										}
									}
								}
								// 03/06/2012 Paul.  A report parameter can include an Assigned To list. 
								else if ( sDATA_FIELD == "ASSIGNED_TO" )
								{
									row["FIELD_TYPE"                ] = "ListBox";
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : sMODULE_NAME + ".LBL_" + sDATA_FIELD;
									row["DATA_FIELD"                ] = sDATA_FIELD;
									row["CACHE_NAME"                ] = "AssignedTo";
									if ( bMULTI_VALUE )
										row["FORMAT_ROWS"] = 4;
								}
								else
								{
									row["FIELD_TYPE"                ] = "TextBox";
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : sMODULE_NAME + ".LBL_" + sDATA_FIELD;
									row["DATA_FIELD"                ] = sDATA_FIELD;
								}
								//row["DATA_FORMAT"               ] = rowEditView["DATA_FORMAT"               ];
								//row["DISPLAY_FIELD"             ] = rowEditView["DISPLAY_FIELD"             ];
								//row["CACHE_NAME"                ] = rowEditView["CACHE_NAME"                ];
								//row["DATA_REQUIRED"             ] = rowEditView["DATA_REQUIRED"             ];
								//row["UI_REQUIRED"               ] = rowEditView["UI_REQUIRED"               ];
								//row["ONCLICK_SCRIPT"            ] = rowEditView["ONCLICK_SCRIPT"            ];
								//row["FORMAT_SCRIPT"             ] = rowEditView["FORMAT_SCRIPT"             ];
								//row["FORMAT_TAB_INDEX"          ] = rowEditView["FORMAT_TAB_INDEX"          ];
								//row["FORMAT_MAX_LENGTH"         ] = rowEditView["FORMAT_MAX_LENGTH"         ];
								//row["FORMAT_SIZE"               ] = rowEditView["FORMAT_SIZE"               ];
								//row["FORMAT_ROWS"               ] = rowEditView["FORMAT_ROWS"               ];
								//row["FORMAT_COLUMNS"            ] = rowEditView["FORMAT_COLUMNS"            ];
								//row["COLSPAN"                   ] = rowEditView["COLSPAN"                   ];
								//row["ROWSPAN"                   ] = rowEditView["ROWSPAN"                   ];
								//row["LABEL_WIDTH"               ] = rowEditView["LABEL_WIDTH"               ];
								//row["FIELD_WIDTH"               ] = rowEditView["FIELD_WIDTH"               ];
								//row["DATA_COLUMNS"              ] = rowEditView["DATA_COLUMNS"              ];
								//row["MODULE_TYPE"               ] = rowEditView["MODULE_TYPE"               ];
							}
						}
					}
					Cache.Set("vwREPORTS.Parameters.EditView." + gID.ToString() + "." + gUSER_ID.ToString(), dt, DefaultCacheExpiration());
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		// 05/03/2011 Paul.  We need to include the USER_ID because we cache the Assigned User ID and the Team ID. 
		public DataTable ReportParameters(Guid gID, Guid gUSER_ID)
		{
			DataTable dt = Cache.Get("vwREPORTS.Parameters." + gID.ToString() + "." + gUSER_ID.ToString()) as DataTable;
			if ( dt == null )
			{
				dt = new DataTable();
				dt.Columns.Add("NAME"         , typeof(String));
				dt.Columns.Add("MODULE_NAME"  , typeof(String));
				dt.Columns.Add("DATA_TYPE"    , typeof(String));  // String, Boolean, DateTime, Integer, Float
				dt.Columns.Add("NULLABLE"     , typeof(bool  ));
				dt.Columns.Add("ALLOW_BLANK"  , typeof(bool  ));
				dt.Columns.Add("MULTI_VALUE"  , typeof(bool  ));
				// 02/03/2012 Paul.  Add support for the Hidden flag. 
				dt.Columns.Add("HIDDEN"       , typeof(bool  ));
				dt.Columns.Add("PROMPT"       , typeof(String));
				dt.Columns.Add("DEFAULT_VALUE", typeof(String));
				// 02/16/2012 Paul.  We need a separate list for report parameter lists. 
				dt.Columns.Add("DATA_SET_NAME"   , typeof(String));
				
				try
				{
					DataTable dtReport = this.Report(gID);
					if ( dtReport.Rows.Count > 0 )
					{
						DataRow rdr = dtReport.Rows[0];
						string sRDL         = Sql.ToString(rdr["RDL"        ]);
						string sMODULE_NAME = Sql.ToString(rdr["MODULE_NAME"]);
						
						RdlDocument rdl = new RdlDocument(hostingEnvironment, Session, Security, this, XmlUtil);
						rdl.LoadRdl(sRDL);
						rdl.NamespaceManager.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
						
						// 02/16/2012 Paul.  We need a separate list for report parameter lists. 
						string sReportID = rdl.SelectNodeValue("rd:ReportID");
						XmlNodeList nlReportParameters = rdl.SelectNodesNS("ReportParameters/ReportParameter");
						foreach ( XmlNode xReportParameter in nlReportParameters )
						{
							DataRow row = dt.NewRow();
							dt.Rows.Add(row);
							// 11/15/2011 Paul.  Must use rdl.SelectNodeValue to get the properties. 
							string sName         = XmlUtil.GetNamedItem    (xReportParameter, "Name"    );
							string sDataType     = rdl.SelectNodeValue(xReportParameter, "DataType");
							bool   bNullable     = Sql.ToBoolean(rdl.SelectNodeValue(xReportParameter, "Nullable"  ));
							bool   bAllowBlank   = Sql.ToBoolean(rdl.SelectNodeValue(xReportParameter, "AllowBlank"));
							bool   bMultiValue   = Sql.ToBoolean(rdl.SelectNodeValue(xReportParameter, "MultiValue"));
							// 02/03/2012 Paul.  Add support for the Hidden flag. 
							bool   bHidden       = Sql.ToBoolean(rdl.SelectNodeValue(xReportParameter, "Hidden"    ));
							string sPrompt       = rdl.SelectNodeValue(xReportParameter, "Prompt"  );
							// 02/16/2012 Paul.  We need a separate list for report parameter lists. 
							string sDataSetName    = rdl.SelectNodeValue(xReportParameter, "ValidValues/DataSetReference/DataSetName");
							string sDefaultValue   = rdl.SelectNodeValue(xReportParameter, "DefaultValue/Values/Value");
							// 02/16/2012 Paul.  Add support for specific parameter values. 
							XmlNodeList nlValidValues = rdl.SelectNodesNS(xReportParameter, "ValidValues/ParameterValues/ParameterValue");
							if ( nlValidValues.Count > 0 )
							{
								DataTable dtValidValues = new DataTable();
								dtValidValues.Columns.Add("VALUE", typeof(String));
								dtValidValues.Columns.Add("NAME" , typeof(String));
								foreach ( XmlNode xValidValue in nlValidValues )
								{
									DataRow rowValid = dtValidValues.NewRow();
									rowValid["VALUE"] = rdl.SelectNodeValue(xValidValue, "Value");
									rowValid["NAME" ] = rdl.SelectNodeValue(xValidValue, "Label");
									dtValidValues.Rows.Add(rowValid);
								}
								this.AddReportSource(sReportID + "." + sName + ".SpecificValues", "VALUE", "NAME", dtValidValues);
								row["DATA_SET_NAME"] = sReportID + "." + sName + ".SpecificValues";
							}
							// 03/04/2012 Paul.  Collection of values. 
							XmlNodeList nlDefaultValues = rdl.SelectNodesNS(xReportParameter, "DefaultValue/Values/Value");
							if ( nlDefaultValues.Count > 0 )
							{
								if ( bMultiValue )
								{
									XmlDocument xml = new XmlDocument();
									xml.AppendChild(xml.CreateXmlDeclaration("1.0", "UTF-8", null));
									xml.AppendChild(xml.CreateElement("Values"));
									foreach ( XmlNode xDefaultValue in nlDefaultValues )
									{
										XmlNode xValue = xml.CreateElement("Value");
										xml.DocumentElement.AppendChild(xValue);
										// 10/05/2012 Paul.  Check default value for null, not new value. 
										bool bNull = Sql.ToBoolean(XmlUtil.GetNamedItem(xDefaultValue, "xsi:nil"));
										if ( !bNull )
											xValue.InnerText = xDefaultValue.InnerText;
									}
									row["DEFAULT_VALUE"] = xml.OuterXml;
								}
								else
								{
									// <Value xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:nil="true" />
									XmlNode xDefaultValue = nlDefaultValues[0];
									bool bNull = Sql.ToBoolean(XmlUtil.GetNamedItem(xDefaultValue, "xsi:nil"));
									if ( !bNull )
										row["DEFAULT_VALUE"] = Sql.ToString(xDefaultValue.InnerText);
								}
							}
							
							row["NAME"       ] = sName       ;
							row["MODULE_NAME"] = sMODULE_NAME;
							row["DATA_TYPE"  ] = sDataType   ;
							row["NULLABLE"   ] = bNullable   ;
							row["ALLOW_BLANK"] = bAllowBlank ;
							row["MULTI_VALUE"] = bMultiValue ;
							// 02/03/2012 Paul.  Add support for the Hidden flag. 
							row["HIDDEN"     ] = bHidden     ;
							row["PROMPT"     ] = sPrompt     ;
							// 02/16/2012 Paul.  We need a separate list for report parameter lists. 
							if ( !Sql.IsEmptyString(sDataSetName) )
								row["DATA_SET_NAME"] = sReportID + "." + sDataSetName;
							if ( String.Compare(sName, "ASSIGNED_USER_ID", true) == 0 )
							{
								row["DEFAULT_VALUE"] = Security.USER_ID.ToString();
							}
							else if ( String.Compare(sName, "TEAM_ID", true) == 0 )
							{
								row["DEFAULT_VALUE"] = Security.TEAM_ID.ToString();
							}
						}
					}
					Cache.Set("vwREPORTS.Parameters." + gID.ToString() + "." + gUSER_ID.ToString(), dt, DefaultCacheExpiration());
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		// 08/11/2014 Paul.  We need a similar method to get report parameters from an unsaved report. 
		public DataTable ReportParametersEditView(string sMODULE_NAME, string sRDL)
		{
			DataTable dt = null;
			if ( dt == null )
			{
				dt = new DataTable();
				dt.Columns.Add("EDIT_NAME"                   , typeof(String ) );
				dt.Columns.Add("FIELD_INDEX"                 , typeof(Int32  ) );
				dt.Columns.Add("FIELD_TYPE"                  , typeof(String ) );
				dt.Columns.Add("DATA_LABEL"                  , typeof(String ) );
				dt.Columns.Add("DATA_FIELD"                  , typeof(String ) );
				dt.Columns.Add("DATA_FORMAT"                 , typeof(String ) );
				dt.Columns.Add("DISPLAY_FIELD"               , typeof(String ) );
				dt.Columns.Add("CACHE_NAME"                  , typeof(String ) );
				dt.Columns.Add("DATA_REQUIRED"               , typeof(Boolean) );
				dt.Columns.Add("UI_REQUIRED"                 , typeof(Boolean) );
				dt.Columns.Add("ONCLICK_SCRIPT"              , typeof(String ) );
				dt.Columns.Add("FORMAT_SCRIPT"               , typeof(String ) );
				dt.Columns.Add("FORMAT_TAB_INDEX"            , typeof(Int16  ) );
				dt.Columns.Add("FORMAT_MAX_LENGTH"           , typeof(Int32  ) );
				dt.Columns.Add("FORMAT_SIZE"                 , typeof(Int32  ) );
				dt.Columns.Add("FORMAT_ROWS"                 , typeof(Int32  ) );
				dt.Columns.Add("FORMAT_COLUMNS"              , typeof(Int32  ) );
				dt.Columns.Add("COLSPAN"                     , typeof(Int32  ) );
				dt.Columns.Add("ROWSPAN"                     , typeof(Int32  ) );
				dt.Columns.Add("LABEL_WIDTH"                 , typeof(String ) );
				dt.Columns.Add("FIELD_WIDTH"                 , typeof(String ) );
				dt.Columns.Add("DATA_COLUMNS"                , typeof(Int32  ) );
				dt.Columns.Add("MODULE_TYPE"                 , typeof(String ) );
				dt.Columns.Add("RELATED_SOURCE_MODULE_NAME"  , typeof(String ) );
				dt.Columns.Add("RELATED_SOURCE_VIEW_NAME"    , typeof(String ) );
				dt.Columns.Add("RELATED_SOURCE_ID_FIELD"     , typeof(String ) );
				dt.Columns.Add("RELATED_SOURCE_NAME_FIELD"   , typeof(String ) );
				dt.Columns.Add("RELATED_VIEW_NAME"           , typeof(String ) );
				dt.Columns.Add("RELATED_ID_FIELD"            , typeof(String ) );
				dt.Columns.Add("RELATED_NAME_FIELD"          , typeof(String ) );
				dt.Columns.Add("RELATED_JOIN_FIELD"          , typeof(String ) );
				dt.Columns.Add("PARENT_FIELD"                , typeof(String ) );
				dt.Columns.Add("FIELD_VALIDATOR_MESSAGE"     , typeof(String ) );
				dt.Columns.Add("VALIDATION_TYPE"             , typeof(String ) );
				dt.Columns.Add("REGULAR_EXPRESSION"          , typeof(String ) );
				dt.Columns.Add("DATA_TYPE"                   , typeof(String ) );
				dt.Columns.Add("MININUM_VALUE"               , typeof(String ) );
				dt.Columns.Add("MAXIMUM_VALUE"               , typeof(String ) );
				dt.Columns.Add("COMPARE_OPERATOR"            , typeof(String ) );
				dt.Columns.Add("TOOL_TIP"                    , typeof(String ) );
				
				try
				{
					// 01/15/2013 Paul.  A customer wants to be able to change the assigned user as this was previously allowed in report prompts. 
					bool bShowAssignedUser = false;
					bShowAssignedUser = Sql.ToBoolean(Application["CONFIG.Reports.ShowAssignedUser"]);
					// 05/03/2011 Paul.  We need to include the USER_ID because we cache the Assigned User ID and the Team ID. 
					DataTable dtReportParameters = this.ReportParameters(sMODULE_NAME, sRDL);
					if ( dtReportParameters != null && dtReportParameters.Rows.Count > 0 )
					{
						DataTable dtEditView = this.EditViewFields(sMODULE_NAME + ".EditView");
						foreach ( DataRow rowParameter in dtReportParameters.Rows )
						{
							// 03/09/2012 Paul.  Making the data field upper case will simplify tests later. 
							string sDATA_FIELD    = Sql.ToString(rowParameter["NAME"         ]).ToUpper();
							string sDATA_LABEL    = Sql.ToString(rowParameter["PROMPT"       ]);
							string sDATA_TYPE     = Sql.ToString(rowParameter["DATA_TYPE"    ]);
							string sDEFAULT_VALUE = Sql.ToString(rowParameter["DEFAULT_VALUE"]);
							bool   bHIDDEN        = Sql.ToBoolean(rowParameter["HIDDEN"      ]);
							// 02/16/2012 Paul.  We need a separate list for report parameter lists. 
							string sDATA_SET_NAME = Sql.ToString (rowParameter["DATA_SET_NAME"]);
							bool   bMULTI_VALUE   = Sql.ToBoolean(rowParameter["MULTI_VALUE"  ]);
							bool bFieldFound = false;
							// 04/09/2011 Paul.  ID is not allowed as a parameter because it is used by the Report ID in the Dashlet. 
							// 02/03/2012 Paul.  Add support for the Hidden flag. 
							if ( sDATA_FIELD == "ID" || bHIDDEN )
								continue;
							foreach ( DataRow rowEditView in dtEditView.Rows )
							{
								// 03/09/2012 Paul.  ASSIGNED_USER_ID is a special parameter that is not a prompt. 
								if ( sDATA_FIELD == Sql.ToString(rowEditView["DATA_FIELD"]) && sDATA_FIELD != "ASSIGNED_USER_ID" && sDATA_FIELD != "TEAM_ID" )
								{
									bFieldFound = true;
									DataRow row = dt.NewRow();
									dt.Rows.Add(row);
									// 07/29/2012 Paul.  Since we are searching, we should follow SearchView rules as we don't want field level security to prevent editing. 
									row["EDIT_NAME"                 ] = sMODULE_NAME + ".SearchView";
									row["FIELD_INDEX"               ] = dt.Rows.Count;
									row["FIELD_TYPE"                ] = rowEditView["FIELD_TYPE"                ];
									// 03/04/2012 Paul.  Custom label was being applied to field, not label. 
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : Sql.ToString(rowEditView["DATA_LABEL"]);
									row["DATA_FIELD"                ] = rowEditView["DATA_FIELD"                ];
									row["DATA_FORMAT"               ] = rowEditView["DATA_FORMAT"               ];
									row["DISPLAY_FIELD"             ] = rowEditView["DISPLAY_FIELD"             ];
									row["CACHE_NAME"                ] = rowEditView["CACHE_NAME"                ];
									row["DATA_REQUIRED"             ] = rowEditView["DATA_REQUIRED"             ];
									row["UI_REQUIRED"               ] = rowEditView["UI_REQUIRED"               ];
									row["ONCLICK_SCRIPT"            ] = rowEditView["ONCLICK_SCRIPT"            ];
									row["FORMAT_SCRIPT"             ] = rowEditView["FORMAT_SCRIPT"             ];
									row["FORMAT_TAB_INDEX"          ] = rowEditView["FORMAT_TAB_INDEX"          ];
									row["FORMAT_MAX_LENGTH"         ] = rowEditView["FORMAT_MAX_LENGTH"         ];
									row["FORMAT_SIZE"               ] = rowEditView["FORMAT_SIZE"               ];
									row["FORMAT_ROWS"               ] = rowEditView["FORMAT_ROWS"               ];
									row["FORMAT_COLUMNS"            ] = rowEditView["FORMAT_COLUMNS"            ];
									//row["COLSPAN"                   ] = rowEditView["COLSPAN"                   ];
									//row["ROWSPAN"                   ] = rowEditView["ROWSPAN"                   ];
									row["LABEL_WIDTH"               ] = rowEditView["LABEL_WIDTH"               ];
									row["FIELD_WIDTH"               ] = rowEditView["FIELD_WIDTH"               ];
									row["DATA_COLUMNS"              ] = rowEditView["DATA_COLUMNS"              ];
									row["MODULE_TYPE"               ] = rowEditView["MODULE_TYPE"               ];
									//row["RELATED_SOURCE_MODULE_NAME"] = rowEditView["RELATED_SOURCE_MODULE_NAME"];
									//row["RELATED_SOURCE_VIEW_NAME"  ] = rowEditView["RELATED_SOURCE_VIEW_NAME"  ];
									//row["RELATED_SOURCE_ID_FIELD"   ] = rowEditView["RELATED_SOURCE_ID_FIELD"   ];
									//row["RELATED_SOURCE_NAME_FIELD" ] = rowEditView["RELATED_SOURCE_NAME_FIELD" ];
									//row["RELATED_VIEW_NAME"         ] = rowEditView["RELATED_VIEW_NAME"         ];
									//row["RELATED_ID_FIELD"          ] = rowEditView["RELATED_ID_FIELD"          ];
									//row["RELATED_NAME_FIELD"        ] = rowEditView["RELATED_NAME_FIELD"        ];
									//row["RELATED_JOIN_FIELD"        ] = rowEditView["RELATED_JOIN_FIELD"        ];
									//row["PARENT_FIELD"              ] = rowEditView["PARENT_FIELD"              ];
									row["FIELD_VALIDATOR_MESSAGE"   ] = rowEditView["FIELD_VALIDATOR_MESSAGE"   ];
									row["VALIDATION_TYPE"           ] = rowEditView["VALIDATION_TYPE"           ];
									row["REGULAR_EXPRESSION"        ] = rowEditView["REGULAR_EXPRESSION"        ];
									row["DATA_TYPE"                 ] = rowEditView["DATA_TYPE"                 ];
									row["MININUM_VALUE"             ] = rowEditView["MININUM_VALUE"             ];
									row["MAXIMUM_VALUE"             ] = rowEditView["MAXIMUM_VALUE"             ];
									row["COMPARE_OPERATOR"          ] = rowEditView["COMPARE_OPERATOR"          ];
									row["TOOL_TIP"                  ] = rowEditView["TOOL_TIP"                  ];
									// 03/04/2012 Paul.  Apply MultiValue flag if set in the report. 
									if ( Sql.ToString(row["FIELD_TYPE"]) == "ListBox" && bMULTI_VALUE )
										row["FORMAT_ROWS"] = 4;
									// 03/24/2018 Paul.  It does not make sense to use the mandatory field flag on an report serach. 
									row["UI_REQUIRED"] = false;
									// 04/28/2018 Paul.  We need to make sure to treat data sets as a list. 
									if ( !Sql.IsEmptyString(sDATA_SET_NAME) )
									{
										row["FIELD_TYPE"                ] = "ListBox";
										row["CACHE_NAME"                ] = sDATA_SET_NAME;
										row["MODULE_TYPE"               ] = DBNull.Value;
										if ( bMULTI_VALUE )
											row["FORMAT_ROWS"] = 4;
									}
									break;
								}
							}
							// 03/09/2012 Paul.  ASSIGNED_USER_ID is a special parameter that is not a prompt. 
							// 01/15/2013 Paul.  A customer wants to be able to change the assigned user as this was previously allowed in report prompts. 
							if ( !bFieldFound && (bShowAssignedUser || (sDATA_FIELD != "ASSIGNED_USER_ID" && sDATA_FIELD != "TEAM_ID")) )
							{
								DataRow row = dt.NewRow();
								dt.Rows.Add(row);
								// 07/29/2012 Paul.  Since we are searching, we should follow SearchView rules as we don't want field level security to prevent editing. 
								row["EDIT_NAME"                 ] = sMODULE_NAME + ".SearchView";
								row["FIELD_INDEX"               ] = dt.Rows.Count;
								// 03/06/2012 Paul.  A report parameter can have a special Date Rule field. 
								if ( sDATA_FIELD == "DATE_RULE" )
								{
									row["FIELD_TYPE"                ] = "ListBox";
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : sMODULE_NAME + ".LBL_" + sDATA_FIELD;
									row["DATA_FIELD"                ] = sDATA_FIELD;
									row["CACHE_NAME"                ] = "date_rule_dom";
								}
								// 11/15/2011 Paul.  If the value starts with a equals, then this is a formula and should not be treated as a date control. 
								else if ( sDEFAULT_VALUE.StartsWith("=") )
								{
									row["FIELD_TYPE"                ] = "TextBox";
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : sMODULE_NAME + ".LBL_" + sDATA_FIELD;
									row["DATA_FIELD"                ] = sDATA_FIELD;
								}
								// 04/28/2018 Paul.  Data Set has higher priority than _ID type. 
								else if ( !Sql.IsEmptyString(sDATA_SET_NAME) )
								{
									row["FIELD_TYPE"                ] = "ListBox";
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : sMODULE_NAME + ".LBL_" + sDATA_FIELD;
									row["DATA_FIELD"                ] = sDATA_FIELD;
									row["CACHE_NAME"                ] = sDATA_SET_NAME;
									if ( bMULTI_VALUE )
										row["FORMAT_ROWS"] = 4;
								}
								else if ( sDATA_FIELD.EndsWith("_ID") )
								{
									// 01/15/2013 Paul.  If the field ends in ID, then the module name must be determined from the field. 
									string sPOPUP_TABLE_NAME = sDATA_FIELD.Substring(0, sDATA_FIELD.Length - 3);
									if ( sDATA_FIELD == "ASSIGNED_USER_ID" )
										sPOPUP_TABLE_NAME = "USERS";
									else if ( sPOPUP_TABLE_NAME.EndsWith("Y") )
										sPOPUP_TABLE_NAME = sDATA_FIELD.Substring(0, sDATA_FIELD.Length - 4) + "IES";
									else if ( sPOPUP_TABLE_NAME != "PROJECT" && sPOPUP_TABLE_NAME != "PROJECT_TASK" )
										sPOPUP_TABLE_NAME += "S";
									string sPOPUP_MODULE_NAME = CrmModules.ModuleName(sPOPUP_TABLE_NAME);
									row["FIELD_TYPE"                ] = "ModulePopup";
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : sPOPUP_MODULE_NAME + ".LBL_" + sDATA_FIELD;
									row["DATA_FIELD"                ] = sDATA_FIELD;
									row["MODULE_TYPE"               ] = sPOPUP_MODULE_NAME;
									row["DISPLAY_FIELD"             ] = sDATA_FIELD.Substring(0, sDATA_FIELD.Length - 2) + "NAME";
									// 04/09/2011 Paul.  Auto-submit the selection. 
									// Auto-submit is not working because we need to hit the actual submit button in order to get parameter processing. 
									//row["DATA_FORMAT"               ] = "1";
								}
								else if ( sDATA_FIELD.StartsWith("DATETIME_") || sDATA_FIELD.EndsWith("_DATETIME") || sDATA_FIELD.Contains("_DATETIME_") )
								{
									row["FIELD_TYPE"                ] = "DateTimePicker";
									// 04/11/2016 Paul.  Use BuildTermName. 
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : Utils.BuildTermName(sMODULE_NAME, sDATA_FIELD);
									row["DATA_FIELD"                ] = sDATA_FIELD;
								}
								else if ( sDATA_TYPE == "DateTime" || sDATA_FIELD.StartsWith("DATE_") || sDATA_FIELD.EndsWith("_DATE") || sDATA_FIELD.Contains("_DATE_") )
								{
									row["FIELD_TYPE"                ] = "DatePicker";
									// 04/11/2016 Paul.  Use BuildTermName. 
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : Utils.BuildTermName(sMODULE_NAME, sDATA_FIELD);
									row["DATA_FIELD"                ] = sDATA_FIELD;
									// 04/11/2016 Paul.  Special support for between clause as a parameter. Needed to be separated into 2 report parameters. 
									if ( bMULTI_VALUE )
									{
										DataRow row2 = dt.NewRow();
										dt.Rows.Add(row2);
										for ( int i = 0; i < dt.Columns.Count; i++ )
										{
											row2[i] = row[i];
										}
										row ["DATA_FIELD"] = sDATA_FIELD + "_AFTER" ;
										row2["DATA_FIELD"] = sDATA_FIELD + "_BEFORE";
										if ( Session != null )
										{
											row ["DATA_LABEL"] =  L10n.Term(Utils.BuildTermName(sMODULE_NAME, sDATA_FIELD)) + " " + L10n.Term("SavedSearch.LBL_SEARCH_AFTER" );
											row2["DATA_LABEL"] =  L10n.Term(Utils.BuildTermName(sMODULE_NAME, sDATA_FIELD)) + " " + L10n.Term("SavedSearch.LBL_SEARCH_BEFORE");
										}
									}
								}
								// 03/06/2012 Paul.  A report parameter can include an Assigned To list. 
								else if ( sDATA_FIELD == "ASSIGNED_TO" )
								{
									row["FIELD_TYPE"                ] = "ListBox";
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : sMODULE_NAME + ".LBL_" + sDATA_FIELD;
									row["DATA_FIELD"                ] = sDATA_FIELD;
									row["CACHE_NAME"                ] = "AssignedTo";
									if ( bMULTI_VALUE )
										row["FORMAT_ROWS"] = 4;
								}
								else
								{
									row["FIELD_TYPE"                ] = "TextBox";
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : sMODULE_NAME + ".LBL_" + sDATA_FIELD;
									row["DATA_FIELD"                ] = sDATA_FIELD;
								}
								//row["DATA_FORMAT"               ] = rowEditView["DATA_FORMAT"               ];
								//row["DISPLAY_FIELD"             ] = rowEditView["DISPLAY_FIELD"             ];
								//row["CACHE_NAME"                ] = rowEditView["CACHE_NAME"                ];
								//row["DATA_REQUIRED"             ] = rowEditView["DATA_REQUIRED"             ];
								//row["UI_REQUIRED"               ] = rowEditView["UI_REQUIRED"               ];
								//row["ONCLICK_SCRIPT"            ] = rowEditView["ONCLICK_SCRIPT"            ];
								//row["FORMAT_SCRIPT"             ] = rowEditView["FORMAT_SCRIPT"             ];
								//row["FORMAT_TAB_INDEX"          ] = rowEditView["FORMAT_TAB_INDEX"          ];
								//row["FORMAT_MAX_LENGTH"         ] = rowEditView["FORMAT_MAX_LENGTH"         ];
								//row["FORMAT_SIZE"               ] = rowEditView["FORMAT_SIZE"               ];
								//row["FORMAT_ROWS"               ] = rowEditView["FORMAT_ROWS"               ];
								//row["FORMAT_COLUMNS"            ] = rowEditView["FORMAT_COLUMNS"            ];
								//row["COLSPAN"                   ] = rowEditView["COLSPAN"                   ];
								//row["ROWSPAN"                   ] = rowEditView["ROWSPAN"                   ];
								//row["LABEL_WIDTH"               ] = rowEditView["LABEL_WIDTH"               ];
								//row["FIELD_WIDTH"               ] = rowEditView["FIELD_WIDTH"               ];
								//row["DATA_COLUMNS"              ] = rowEditView["DATA_COLUMNS"              ];
								//row["MODULE_TYPE"               ] = rowEditView["MODULE_TYPE"               ];
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		// 08/11/2014 Paul.  We need a similar method to get report parameters from an unsaved report. 
		public DataTable ReportParameters(string sMODULE_NAME, string sRDL)
		{
			DataTable dt = null;
			if ( dt == null )
			{
				dt = new DataTable();
				dt.Columns.Add("NAME"         , typeof(String));
				dt.Columns.Add("MODULE_NAME"  , typeof(String));
				dt.Columns.Add("DATA_TYPE"    , typeof(String));  // String, Boolean, DateTime, Integer, Float
				dt.Columns.Add("NULLABLE"     , typeof(bool  ));
				dt.Columns.Add("ALLOW_BLANK"  , typeof(bool  ));
				dt.Columns.Add("MULTI_VALUE"  , typeof(bool  ));
				// 02/03/2012 Paul.  Add support for the Hidden flag. 
				dt.Columns.Add("HIDDEN"       , typeof(bool  ));
				dt.Columns.Add("PROMPT"       , typeof(String));
				dt.Columns.Add("DEFAULT_VALUE", typeof(String));
				// 02/16/2012 Paul.  We need a separate list for report parameter lists. 
				dt.Columns.Add("DATA_SET_NAME"   , typeof(String));
				
				try
				{
					if ( !Sql.IsEmptyString(sMODULE_NAME) )
					{
						
						RdlDocument rdl = new RdlDocument(hostingEnvironment, Session, Security, this, XmlUtil);
						rdl.LoadRdl(sRDL);
						rdl.NamespaceManager.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
						
						// 02/16/2012 Paul.  We need a separate list for report parameter lists. 
						string sReportID = rdl.SelectNodeValue("rd:ReportID");
						XmlNodeList nlReportParameters = rdl.SelectNodesNS("ReportParameters/ReportParameter");
						foreach ( XmlNode xReportParameter in nlReportParameters )
						{
							DataRow row = dt.NewRow();
							dt.Rows.Add(row);
							// 11/15/2011 Paul.  Must use rdl.SelectNodeValue to get the properties. 
							string sName         = XmlUtil.GetNamedItem    (xReportParameter, "Name"    );
							string sDataType     = rdl.SelectNodeValue(xReportParameter, "DataType");
							bool   bNullable     = Sql.ToBoolean(rdl.SelectNodeValue(xReportParameter, "Nullable"  ));
							bool   bAllowBlank   = Sql.ToBoolean(rdl.SelectNodeValue(xReportParameter, "AllowBlank"));
							bool   bMultiValue   = Sql.ToBoolean(rdl.SelectNodeValue(xReportParameter, "MultiValue"));
							// 02/03/2012 Paul.  Add support for the Hidden flag. 
							bool   bHidden       = Sql.ToBoolean(rdl.SelectNodeValue(xReportParameter, "Hidden"    ));
							string sPrompt       = rdl.SelectNodeValue(xReportParameter, "Prompt"  );
							// 02/16/2012 Paul.  We need a separate list for report parameter lists. 
							string sDataSetName    = rdl.SelectNodeValue(xReportParameter, "ValidValues/DataSetReference/DataSetName");
							string sDefaultValue   = rdl.SelectNodeValue(xReportParameter, "DefaultValue/Values/Value");
							// 02/16/2012 Paul.  Add support for specific parameter values. 
							XmlNodeList nlValidValues = rdl.SelectNodesNS(xReportParameter, "ValidValues/ParameterValues/ParameterValue");
							if ( nlValidValues.Count > 0 )
							{
								DataTable dtValidValues = new DataTable();
								dtValidValues.Columns.Add("VALUE", typeof(String));
								dtValidValues.Columns.Add("NAME" , typeof(String));
								foreach ( XmlNode xValidValue in nlValidValues )
								{
									DataRow rowValid = dtValidValues.NewRow();
									rowValid["VALUE"] = rdl.SelectNodeValue(xValidValue, "Value");
									rowValid["NAME" ] = rdl.SelectNodeValue(xValidValue, "Label");
									dtValidValues.Rows.Add(rowValid);
								}
								this.AddReportSource(sReportID + "." + sName + ".SpecificValues", "VALUE", "NAME", dtValidValues);
								row["DATA_SET_NAME"] = sReportID + "." + sName + ".SpecificValues";
							}
							// 03/04/2012 Paul.  Collection of values. 
							XmlNodeList nlDefaultValues = rdl.SelectNodesNS(xReportParameter, "DefaultValue/Values/Value");
							if ( nlDefaultValues.Count > 0 )
							{
								if ( bMultiValue )
								{
									XmlDocument xml = new XmlDocument();
									xml.AppendChild(xml.CreateXmlDeclaration("1.0", "UTF-8", null));
									xml.AppendChild(xml.CreateElement("Values"));
									foreach ( XmlNode xDefaultValue in nlDefaultValues )
									{
										XmlNode xValue = xml.CreateElement("Value");
										xml.DocumentElement.AppendChild(xValue);
										// 10/05/2012 Paul.  Check default value for null, not new value. 
										bool bNull = Sql.ToBoolean(XmlUtil.GetNamedItem(xDefaultValue, "xsi:nil"));
										if ( !bNull )
											xValue.InnerText = xDefaultValue.InnerText;
									}
									row["DEFAULT_VALUE"] = xml.OuterXml;
								}
								else
								{
									// <Value xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:nil="true" />
									XmlNode xDefaultValue = nlDefaultValues[0];
									bool bNull = Sql.ToBoolean(XmlUtil.GetNamedItem(xDefaultValue, "xsi:nil"));
									if ( !bNull )
										row["DEFAULT_VALUE"] = Sql.ToString(xDefaultValue.InnerText);
								}
							}
							
							row["NAME"       ] = sName       ;
							row["MODULE_NAME"] = sMODULE_NAME;
							row["DATA_TYPE"  ] = sDataType   ;
							row["NULLABLE"   ] = bNullable   ;
							row["ALLOW_BLANK"] = bAllowBlank ;
							row["MULTI_VALUE"] = bMultiValue ;
							// 02/03/2012 Paul.  Add support for the Hidden flag. 
							row["HIDDEN"     ] = bHidden     ;
							row["PROMPT"     ] = sPrompt     ;
							// 02/16/2012 Paul.  We need a separate list for report parameter lists. 
							if ( !Sql.IsEmptyString(sDataSetName) )
								row["DATA_SET_NAME"] = sReportID + "." + sDataSetName;
							if ( String.Compare(sName, "ASSIGNED_USER_ID", true) == 0 )
							{
								row["DEFAULT_VALUE"] = Security.USER_ID.ToString();
							}
							else if ( String.Compare(sName, "TEAM_ID", true) == 0 )
							{
								row["DEFAULT_VALUE"] = Security.TEAM_ID.ToString();
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		// 10/01/2012 Paul.  Caching by name might not be a good idea as we may have issues clearing the cache value. 
		public Guid ReportByName(string sReportName)
		{
			Guid gID = Guid.Empty;
			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					string sSQL;
					sSQL = "select ID            " + ControlChars.CrLf
					     + "  from vwREPORTS_Edit" + ControlChars.CrLf
					     + " where NAME = @NAME  " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@NAME", sReportName);
						gID = Sql.ToGuid(cmd.ExecuteScalar());
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
			}
			return gID;
		}

		// 04/06/2011 Paul.  Cache reports. 
		public DataTable Report(Guid gID)
		{
			DataTable dt = Cache.Get("vwREPORTS." + gID.ToString()) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select *             " + ControlChars.CrLf
						     + "  from vwREPORTS_Edit" + ControlChars.CrLf
						     + " where ID = @ID      " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@ID", gID);
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwREPORTS." + gID.ToString(), dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		// 01/24/2010 Paul.  Clear the Report List on save. 
		public void ClearReports()
		{
			try
			{
				Session.Remove("vwREPORTS_List." + Security.USER_ID.ToString());
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
			}
		}

		// 01/24/2010 Paul.  Place the report list in the cache so that it would be available in SearchView. 
		public DataTable Reports()
		{
			DataTable dt = Session.GetTable("vwREPORTS_List." + Security.USER_ID.ToString());
			if ( dt == null )
			{
				dt = new DataTable();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							sSQL = "select ID            " + ControlChars.CrLf
							     + "     , NAME          " + ControlChars.CrLf
							     + "  from vwREPORTS_List" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								//Sql.AddParameter(cmd, "@ASSIGNED_USER_ID", Security.USER_ID);
								// 01/20/2011 Paul.  Use new Security.Filter() function to apply Team and ACL security rules.
								// This new approach to report security should have been applied many months ago. 
								Security.Filter(cmd, "Reports", "list");
								cmd.CommandText += " order by NAME" + ControlChars.CrLf;
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dt);
									Session.SetTable("vwREPORTS_List." + Security.USER_ID.ToString(), dt);
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		public DataTable ChartParametersEditView(Guid gID, Guid gUSER_ID)
		{
			DataTable dt = Cache.Get("vwCHARTS.Parameters.EditView." + gID.ToString() + "." + gUSER_ID.ToString()) as DataTable;
			if ( dt == null )
			{
				dt = new DataTable();
				dt.Columns.Add("EDIT_NAME"                   , typeof(String ) );
				dt.Columns.Add("FIELD_INDEX"                 , typeof(Int32  ) );
				dt.Columns.Add("FIELD_TYPE"                  , typeof(String ) );
				dt.Columns.Add("DATA_LABEL"                  , typeof(String ) );
				dt.Columns.Add("DATA_FIELD"                  , typeof(String ) );
				dt.Columns.Add("DATA_FORMAT"                 , typeof(String ) );
				dt.Columns.Add("DISPLAY_FIELD"               , typeof(String ) );
				dt.Columns.Add("CACHE_NAME"                  , typeof(String ) );
				dt.Columns.Add("DATA_REQUIRED"               , typeof(Boolean) );
				dt.Columns.Add("UI_REQUIRED"                 , typeof(Boolean) );
				dt.Columns.Add("ONCLICK_SCRIPT"              , typeof(String ) );
				dt.Columns.Add("FORMAT_SCRIPT"               , typeof(String ) );
				dt.Columns.Add("FORMAT_TAB_INDEX"            , typeof(Int16  ) );
				dt.Columns.Add("FORMAT_MAX_LENGTH"           , typeof(Int32  ) );
				dt.Columns.Add("FORMAT_SIZE"                 , typeof(Int32  ) );
				dt.Columns.Add("FORMAT_ROWS"                 , typeof(Int32  ) );
				dt.Columns.Add("FORMAT_COLUMNS"              , typeof(Int32  ) );
				dt.Columns.Add("COLSPAN"                     , typeof(Int32  ) );
				dt.Columns.Add("ROWSPAN"                     , typeof(Int32  ) );
				dt.Columns.Add("LABEL_WIDTH"                 , typeof(String ) );
				dt.Columns.Add("FIELD_WIDTH"                 , typeof(String ) );
				dt.Columns.Add("DATA_COLUMNS"                , typeof(Int32  ) );
				dt.Columns.Add("MODULE_TYPE"                 , typeof(String ) );
				dt.Columns.Add("RELATED_SOURCE_MODULE_NAME"  , typeof(String ) );
				dt.Columns.Add("RELATED_SOURCE_VIEW_NAME"    , typeof(String ) );
				dt.Columns.Add("RELATED_SOURCE_ID_FIELD"     , typeof(String ) );
				dt.Columns.Add("RELATED_SOURCE_NAME_FIELD"   , typeof(String ) );
				dt.Columns.Add("RELATED_VIEW_NAME"           , typeof(String ) );
				dt.Columns.Add("RELATED_ID_FIELD"            , typeof(String ) );
				dt.Columns.Add("RELATED_NAME_FIELD"          , typeof(String ) );
				dt.Columns.Add("RELATED_JOIN_FIELD"          , typeof(String ) );
				dt.Columns.Add("PARENT_FIELD"                , typeof(String ) );
				dt.Columns.Add("FIELD_VALIDATOR_MESSAGE"     , typeof(String ) );
				dt.Columns.Add("VALIDATION_TYPE"             , typeof(String ) );
				dt.Columns.Add("REGULAR_EXPRESSION"          , typeof(String ) );
				dt.Columns.Add("DATA_TYPE"                   , typeof(String ) );
				dt.Columns.Add("MININUM_VALUE"               , typeof(String ) );
				dt.Columns.Add("MAXIMUM_VALUE"               , typeof(String ) );
				dt.Columns.Add("COMPARE_OPERATOR"            , typeof(String ) );
				dt.Columns.Add("TOOL_TIP"                    , typeof(String ) );
				
				try
				{
					// 01/15/2013 Paul.  A customer wants to be able to change the assigned user as this was previously allowed in report prompts. 
					bool bShowAssignedUser = false;
					bShowAssignedUser = Sql.ToBoolean(Application["CONFIG.Reports.ShowAssignedUser"]);
					// 05/03/2011 Paul.  We need to include the USER_ID because we cache the Assigned User ID and the Team ID. 
					DataTable dtChartParameters = this.ChartParameters(gID, gUSER_ID);
					if ( dtChartParameters != null && dtChartParameters.Rows.Count > 0 )
					{
						string sMODULE_NAME = Sql.ToString(dtChartParameters.Rows[0]["MODULE_NAME"]);
						DataTable dtEditView = this.EditViewFields(sMODULE_NAME + ".EditView");
						foreach ( DataRow rowParameter in dtChartParameters.Rows )
						{
							string sDATA_FIELD    = Sql.ToString(rowParameter["NAME"         ]);
							string sDATA_LABEL    = Sql.ToString(rowParameter["PROMPT"       ]);
							string sDATA_TYPE     = Sql.ToString(rowParameter["DATA_TYPE"    ]);
							string sDEFAULT_VALUE = Sql.ToString(rowParameter["DEFAULT_VALUE"]);
							bool   bHIDDEN        = Sql.ToBoolean(rowParameter["HIDDEN"      ]);
							// 02/16/2012 Paul.  We need a separate list for report parameter lists. 
							string sDATA_SET_NAME = Sql.ToString (rowParameter["DATA_SET_NAME"]);
							bool   bMULTI_VALUE   = Sql.ToBoolean(rowParameter["MULTI_VALUE"  ]);
							bool bFieldFound = false;
							// 04/09/2011 Paul.  ID is not allowed as a parameter because it is used by the Report ID in the Dashlet. 
							// 02/03/2012 Paul.  Add support for the Hidden flag. 
							if ( sDATA_FIELD == "ID" || bHIDDEN )
								continue;
							foreach ( DataRow rowEditView in dtEditView.Rows )
							{
								if ( sDATA_FIELD == Sql.ToString(rowEditView["DATA_FIELD"]) )
								{
									bFieldFound = true;
									DataRow row = dt.NewRow();
									dt.Rows.Add(row);
									// 07/29/2012 Paul.  Since we are searching, we should follow SearchView rules as we don't want field level security to prevent editing. 
									row["EDIT_NAME"                 ] = sMODULE_NAME + ".SearchView";
									row["FIELD_INDEX"               ] = dt.Rows.Count;
									row["FIELD_TYPE"                ] = rowEditView["FIELD_TYPE"                ];
									// 03/04/2012 Paul.  Custom label was being applied to field, not label. 
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : Sql.ToString(rowEditView["DATA_LABEL"]);
									row["DATA_FIELD"                ] = rowEditView["DATA_FIELD"                ];
									row["DATA_FORMAT"               ] = rowEditView["DATA_FORMAT"               ];
									row["DISPLAY_FIELD"             ] = rowEditView["DISPLAY_FIELD"             ];
									row["CACHE_NAME"                ] = rowEditView["CACHE_NAME"                ];
									row["DATA_REQUIRED"             ] = rowEditView["DATA_REQUIRED"             ];
									row["UI_REQUIRED"               ] = rowEditView["UI_REQUIRED"               ];
									row["ONCLICK_SCRIPT"            ] = rowEditView["ONCLICK_SCRIPT"            ];
									row["FORMAT_SCRIPT"             ] = rowEditView["FORMAT_SCRIPT"             ];
									row["FORMAT_TAB_INDEX"          ] = rowEditView["FORMAT_TAB_INDEX"          ];
									row["FORMAT_MAX_LENGTH"         ] = rowEditView["FORMAT_MAX_LENGTH"         ];
									row["FORMAT_SIZE"               ] = rowEditView["FORMAT_SIZE"               ];
									row["FORMAT_ROWS"               ] = rowEditView["FORMAT_ROWS"               ];
									row["FORMAT_COLUMNS"            ] = rowEditView["FORMAT_COLUMNS"            ];
									//row["COLSPAN"                   ] = rowEditView["COLSPAN"                   ];
									//row["ROWSPAN"                   ] = rowEditView["ROWSPAN"                   ];
									row["LABEL_WIDTH"               ] = rowEditView["LABEL_WIDTH"               ];
									row["FIELD_WIDTH"               ] = rowEditView["FIELD_WIDTH"               ];
									row["DATA_COLUMNS"              ] = rowEditView["DATA_COLUMNS"              ];
									row["MODULE_TYPE"               ] = rowEditView["MODULE_TYPE"               ];
									//row["RELATED_SOURCE_MODULE_NAME"] = rowEditView["RELATED_SOURCE_MODULE_NAME"];
									//row["RELATED_SOURCE_VIEW_NAME"  ] = rowEditView["RELATED_SOURCE_VIEW_NAME"  ];
									//row["RELATED_SOURCE_ID_FIELD"   ] = rowEditView["RELATED_SOURCE_ID_FIELD"   ];
									//row["RELATED_SOURCE_NAME_FIELD" ] = rowEditView["RELATED_SOURCE_NAME_FIELD" ];
									//row["RELATED_VIEW_NAME"         ] = rowEditView["RELATED_VIEW_NAME"         ];
									//row["RELATED_ID_FIELD"          ] = rowEditView["RELATED_ID_FIELD"          ];
									//row["RELATED_NAME_FIELD"        ] = rowEditView["RELATED_NAME_FIELD"        ];
									//row["RELATED_JOIN_FIELD"        ] = rowEditView["RELATED_JOIN_FIELD"        ];
									//row["PARENT_FIELD"              ] = rowEditView["PARENT_FIELD"              ];
									row["FIELD_VALIDATOR_MESSAGE"   ] = rowEditView["FIELD_VALIDATOR_MESSAGE"   ];
									row["VALIDATION_TYPE"           ] = rowEditView["VALIDATION_TYPE"           ];
									row["REGULAR_EXPRESSION"        ] = rowEditView["REGULAR_EXPRESSION"        ];
									row["DATA_TYPE"                 ] = rowEditView["DATA_TYPE"                 ];
									row["MININUM_VALUE"             ] = rowEditView["MININUM_VALUE"             ];
									row["MAXIMUM_VALUE"             ] = rowEditView["MAXIMUM_VALUE"             ];
									row["COMPARE_OPERATOR"          ] = rowEditView["COMPARE_OPERATOR"          ];
									row["TOOL_TIP"                  ] = rowEditView["TOOL_TIP"                  ];
									// 03/04/2012 Paul.  Apply MultiValue flag if set in the report. 
									if ( Sql.ToString(row["FIELD_TYPE"]) == "ListBox" && bMULTI_VALUE )
										row["FORMAT_ROWS"] = 4;
									// 03/24/2018 Paul.  It does not make sense to use the mandatory field flag on an report serach. 
									row["UI_REQUIRED"] = false;
									// 04/28/2018 Paul.  We need to make sure to treat data sets as a list. 
									if ( !Sql.IsEmptyString(sDATA_SET_NAME) )
									{
										row["FIELD_TYPE"                ] = "ListBox";
										row["CACHE_NAME"                ] = sDATA_SET_NAME;
										row["MODULE_TYPE"               ] = DBNull.Value;
										if ( bMULTI_VALUE )
											row["FORMAT_ROWS"] = 4;
									}
									break;
								}
							}
							// 01/15/2013 Paul.  ASSIGNED_USER_ID is a special parameter that is not a prompt. 
							// 01/15/2013 Paul.  A customer wants to be able to change the assigned user as this was previously allowed in report prompts. 
							if ( !bFieldFound && (bShowAssignedUser || (sDATA_FIELD != "ASSIGNED_USER_ID" && sDATA_FIELD != "TEAM_ID")) )
							{
								DataRow row = dt.NewRow();
								dt.Rows.Add(row);
								// 07/29/2012 Paul.  Since we are searching, we should follow SearchView rules as we don't want field level security to prevent editing. 
								row["EDIT_NAME"                 ] = sMODULE_NAME + ".SearchView";
								row["FIELD_INDEX"               ] = dt.Rows.Count;
								// 11/15/2011 Paul.  If the value starts with a equals, then this is a formula and should not be treated as a date control. 
								if ( sDEFAULT_VALUE.StartsWith("=") )
								{
									row["FIELD_TYPE"                ] = "TextBox";
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : sMODULE_NAME + ".LBL_" + sDATA_FIELD;
									row["DATA_FIELD"                ] = sDATA_FIELD;
								}
								// 02/16/2012 Paul.  We need a separate list for report parameter lists. 
								// 04/28/2018 Paul.  Data Set has higher priority than _ID type. 
								else if ( !Sql.IsEmptyString(sDATA_SET_NAME) )
								{
									row["FIELD_TYPE"                ] = "ListBox";
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : sMODULE_NAME + ".LBL_" + sDATA_FIELD;
									row["DATA_FIELD"                ] = sDATA_FIELD;
									row["CACHE_NAME"                ] = sDATA_SET_NAME;
									if ( bMULTI_VALUE )
										row["FORMAT_ROWS"] = 4;
								}
								else if ( sDATA_FIELD.EndsWith("_ID") )
								{
									// 01/15/2013 Paul.  If the field ends in ID, then the module name must be determined from the field. 
									string sPOPUP_TABLE_NAME = sDATA_FIELD.Substring(0, sDATA_FIELD.Length - 3);
									if ( sDATA_FIELD == "ASSIGNED_USER_ID" )
										sPOPUP_TABLE_NAME = "USERS";
									else if ( sPOPUP_TABLE_NAME.EndsWith("Y") )
										sPOPUP_TABLE_NAME = sDATA_FIELD.Substring(0, sDATA_FIELD.Length - 4) + "IES";
									else if ( sPOPUP_TABLE_NAME != "PROJECT" && sPOPUP_TABLE_NAME != "PROJECT_TASK" )
										sPOPUP_TABLE_NAME += "S";
									string sPOPUP_MODULE_NAME = CrmModules.ModuleName(sPOPUP_TABLE_NAME);
									row["FIELD_TYPE"                ] = "ModulePopup";
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : sPOPUP_MODULE_NAME + ".LBL_" + sDATA_FIELD;
									row["DATA_FIELD"                ] = sDATA_FIELD;
									row["MODULE_TYPE"               ] = sPOPUP_MODULE_NAME;
									row["DISPLAY_FIELD"             ] = sDATA_FIELD.Substring(0, sDATA_FIELD.Length - 2) + "NAME";
									// 04/09/2011 Paul.  Auto-submit the selection. 
									// Auto-submit is not working because we need to hit the actual submit button in order to get parameter processing. 
									//row["DATA_FORMAT"               ] = "1";
								}
								else if ( sDATA_FIELD.StartsWith("DATETIME_") || sDATA_FIELD.EndsWith("_DATETIME") || sDATA_FIELD.Contains("_DATETIME_") )
								{
									row["FIELD_TYPE"                ] = "DateTimePicker";
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : sMODULE_NAME + ".LBL_" + sDATA_FIELD;
									row["DATA_FIELD"                ] = sDATA_FIELD;
								}
								else if ( sDATA_TYPE == "DateTime" || sDATA_FIELD.StartsWith("DATE_") || sDATA_FIELD.EndsWith("_DATE") || sDATA_FIELD.Contains("_DATE_") )
								{
									row["FIELD_TYPE"                ] = "DatePicker";
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : sMODULE_NAME + ".LBL_" + sDATA_FIELD;
									row["DATA_FIELD"                ] = sDATA_FIELD;
								}
								else
								{
									row["FIELD_TYPE"                ] = "TextBox";
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : sMODULE_NAME + ".LBL_" + sDATA_FIELD;
									row["DATA_FIELD"                ] = sDATA_FIELD;
								}
								//row["DATA_FORMAT"               ] = rowEditView["DATA_FORMAT"               ];
								//row["DISPLAY_FIELD"             ] = rowEditView["DISPLAY_FIELD"             ];
								//row["CACHE_NAME"                ] = rowEditView["CACHE_NAME"                ];
								//row["DATA_REQUIRED"             ] = rowEditView["DATA_REQUIRED"             ];
								//row["UI_REQUIRED"               ] = rowEditView["UI_REQUIRED"               ];
								//row["ONCLICK_SCRIPT"            ] = rowEditView["ONCLICK_SCRIPT"            ];
								//row["FORMAT_SCRIPT"             ] = rowEditView["FORMAT_SCRIPT"             ];
								//row["FORMAT_TAB_INDEX"          ] = rowEditView["FORMAT_TAB_INDEX"          ];
								//row["FORMAT_MAX_LENGTH"         ] = rowEditView["FORMAT_MAX_LENGTH"         ];
								//row["FORMAT_SIZE"               ] = rowEditView["FORMAT_SIZE"               ];
								//row["FORMAT_ROWS"               ] = rowEditView["FORMAT_ROWS"               ];
								//row["FORMAT_COLUMNS"            ] = rowEditView["FORMAT_COLUMNS"            ];
								//row["COLSPAN"                   ] = rowEditView["COLSPAN"                   ];
								//row["ROWSPAN"                   ] = rowEditView["ROWSPAN"                   ];
								//row["LABEL_WIDTH"               ] = rowEditView["LABEL_WIDTH"               ];
								//row["FIELD_WIDTH"               ] = rowEditView["FIELD_WIDTH"               ];
								//row["DATA_COLUMNS"              ] = rowEditView["DATA_COLUMNS"              ];
								//row["MODULE_TYPE"               ] = rowEditView["MODULE_TYPE"               ];
							}
						}
					}
					Cache.Set("vwCHARTS.Parameters.EditView." + gID.ToString() + "." + gUSER_ID.ToString(), dt, DefaultCacheExpiration());
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		// 05/03/2011 Paul.  We need to include the USER_ID because we cache the Assigned User ID and the Team ID. 
		public DataTable ChartParameters(Guid gID, Guid gUSER_ID)
		{
			DataTable dt = Cache.Get("vwCHARTS.Parameters." + gID.ToString() + "." + gUSER_ID.ToString()) as DataTable;
			if ( dt == null )
			{
				dt = new DataTable();
				dt.Columns.Add("NAME"         , typeof(String));
				dt.Columns.Add("MODULE_NAME"  , typeof(String));
				dt.Columns.Add("DATA_TYPE"    , typeof(String));  // String, Boolean, DateTime, Integer, Float
				dt.Columns.Add("NULLABLE"     , typeof(bool  ));
				dt.Columns.Add("ALLOW_BLANK"  , typeof(bool  ));
				dt.Columns.Add("MULTI_VALUE"  , typeof(bool  ));
				// 02/03/2012 Paul.  Add support for the Hidden flag. 
				dt.Columns.Add("HIDDEN"       , typeof(bool  ));
				dt.Columns.Add("PROMPT"       , typeof(String));
				dt.Columns.Add("DEFAULT_VALUE", typeof(String));
				// 02/16/2012 Paul.  We need a separate list for report parameter lists. 
				dt.Columns.Add("DATA_SET_NAME"   , typeof(String));
				
				try
				{
					DataTable dtChart = this.Chart(gID);
					if ( dtChart.Rows.Count > 0 )
					{
						DataRow rdr = dtChart.Rows[0];
						string sRDL         = Sql.ToString(rdr["RDL"        ]);
						string sMODULE_NAME = Sql.ToString(rdr["MODULE_NAME"]);
						
						RdlDocument rdl = new RdlDocument(hostingEnvironment, Session, Security, this, XmlUtil);
						rdl.LoadRdl(sRDL);
						rdl.NamespaceManager.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
						
						// 02/16/2012 Paul.  We need a separate list for report parameter lists. 
						string sReportID = rdl.SelectNodeValue("rd:ReportID");
						XmlNodeList nlReportParameters = rdl.SelectNodesNS("ReportParameters/ReportParameter");
						foreach ( XmlNode xReportParameter in nlReportParameters )
						{
							DataRow row = dt.NewRow();
							dt.Rows.Add(row);
							// 11/15/2011 Paul.  Must use rdl.SelectNodeValue to get the properties. 
							string sName         = XmlUtil.GetNamedItem    (xReportParameter, "Name"    );
							string sDataType     = rdl.SelectNodeValue(xReportParameter, "DataType");
							bool   bNullable     = Sql.ToBoolean(rdl.SelectNodeValue(xReportParameter, "Nullable"  ));
							bool   bAllowBlank   = Sql.ToBoolean(rdl.SelectNodeValue(xReportParameter, "AllowBlank"));
							bool   bMultiValue   = Sql.ToBoolean(rdl.SelectNodeValue(xReportParameter, "MultiValue"));
							// 02/03/2012 Paul.  Add support for the Hidden flag. 
							bool   bHidden       = Sql.ToBoolean(rdl.SelectNodeValue(xReportParameter, "Hidden"    ));
							string sPrompt       = rdl.SelectNodeValue(xReportParameter, "Prompt"  );
							// 02/16/2012 Paul.  We need a separate list for report parameter lists. 
							string sDataSetName    = rdl.SelectNodeValue(xReportParameter, "ValidValues/DataSetReference/DataSetName");
							string sDefaultValue   = rdl.SelectNodeValue(xReportParameter, "DefaultValue/Values/Value");
							// 02/16/2012 Paul.  Add support for specific parameter values. 
							XmlNodeList nlValidValues = rdl.SelectNodesNS(xReportParameter, "ValidValues/ParameterValues/ParameterValue");
							if ( nlValidValues.Count > 0 )
							{
								DataTable dtValidValues = new DataTable();
								dtValidValues.Columns.Add("VALUE", typeof(String));
								dtValidValues.Columns.Add("NAME" , typeof(String));
								foreach ( XmlNode xValidValue in nlValidValues )
								{
									DataRow rowValid = dtValidValues.NewRow();
									rowValid["VALUE"] = rdl.SelectNodeValue(xValidValue, "Value");
									rowValid["NAME" ] = rdl.SelectNodeValue(xValidValue, "Label");
									dtValidValues.Rows.Add(rowValid);
								}
								this.AddReportSource(sReportID + "." + sName + ".SpecificValues", "VALUE", "NAME", dtValidValues);
								row["DATA_SET_NAME"] = sReportID + "." + sName + ".SpecificValues";
							}
							XmlNodeList nlDefaultValues = rdl.SelectNodesNS(xReportParameter, "DefaultValue/Values");
							if ( nlDefaultValues.Count > 0 )
							{
								if ( bMultiValue )
								{
									XmlDocument xml = new XmlDocument();
									xml.AppendChild(xml.CreateXmlDeclaration("1.0", "UTF-8", null));
									xml.AppendChild(xml.CreateElement("Values"));
									foreach ( XmlNode xDefaultValue in nlDefaultValues )
									{
										XmlNode xValue = xml.CreateElement("Value");
										xml.DocumentElement.AppendChild(xValue);
										// 10/05/2012 Paul.  Check default value for null, not new value. 
										bool bNull = Sql.ToBoolean(XmlUtil.GetNamedItem(xDefaultValue, "xsi:nil"));
										if ( !bNull )
											xValue.InnerText = xDefaultValue.InnerText;
									}
									row["DEFAULT_VALUE"] = xml.OuterXml;
								}
								else
								{
									// <Value xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:nil="true" />
									XmlNode xDefaultValue = nlDefaultValues[0];
									bool bNull = Sql.ToBoolean(XmlUtil.GetNamedItem(xDefaultValue, "xsi:nil"));
									if ( !bNull )
										row["DEFAULT_VALUE"] = Sql.ToString(xDefaultValue.InnerText);
								}
							}
							
							row["NAME"       ] = sName       ;
							row["MODULE_NAME"] = sMODULE_NAME;
							row["DATA_TYPE"  ] = sDataType   ;
							row["NULLABLE"   ] = bNullable   ;
							row["ALLOW_BLANK"] = bAllowBlank ;
							row["MULTI_VALUE"] = bMultiValue ;
							// 02/03/2012 Paul.  Add support for the Hidden flag. 
							row["HIDDEN"     ] = bHidden     ;
							row["PROMPT"     ] = sPrompt     ;
							// 02/16/2012 Paul.  We need a separate list for report parameter lists. 
							if ( !Sql.IsEmptyString(sDataSetName) )
								row["DATA_SET_NAME"] = sReportID + "." + sDataSetName;
							if ( String.Compare(sName, "ASSIGNED_USER_ID", true) == 0 )
							{
								row["DEFAULT_VALUE"] = Security.USER_ID.ToString();
							}
							else if ( String.Compare(sName, "TEAM_ID", true) == 0 )
							{
								row["DEFAULT_VALUE"] = Security.TEAM_ID.ToString();
							}
						}
					}
					Cache.Set("vwCHARTS.Parameters." + gID.ToString() + "." + gUSER_ID.ToString(), dt, DefaultCacheExpiration());
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		// 10/29/2011 Paul.  Cache Charts. 
		public DataTable Chart(Guid gID)
		{
			DataTable dt = Cache.Get("vwCHARTS." + gID.ToString()) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select *             " + ControlChars.CrLf
						     + "  from vwCHARTS_Edit" + ControlChars.CrLf
						     + " where ID = @ID      " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@ID", gID);
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwCHARTS." + gID.ToString(), dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		// 10/29/2011 Paul.  Clear the Chart List on save. 
		public void ClearCharts()
		{
			try
			{
				Session.Remove("vwCHARTS_List." + Security.USER_ID.ToString());
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
			}
		}

		public void ClearChart(Guid gID)
		{
			string sID = gID.ToString();
			foreach(string sKey in GetCacheKeys())
			{
				if ( sKey.StartsWith("vwCHART.") && sKey.Contains(sID) )
					Cache.Remove(sKey);
			}
		}

		// 10/29/2011 Paul.  Place the chart list in the cache so that it would be available in SearchView. 
		public DataTable Charts()
		{
			DataTable dt = Session.GetTable("vwCHARTS_List." + Security.USER_ID.ToString());
			if ( dt == null )
			{
				dt = new DataTable();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							sSQL = "select ID            " + ControlChars.CrLf
							     + "     , NAME          " + ControlChars.CrLf
							     + "  from vwCHARTS_List" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Security.Filter(cmd, "Charts", "list");
								cmd.CommandText += " order by NAME" + ControlChars.CrLf;
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dt);
									Session.SetTable("vwCHARTS_List." + Security.USER_ID.ToString(), dt);
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		// 06/03/2011 Paul.  Cache the Sync Table data. 
		public DataTable SyncTables(string sTABLE_NAME, bool bExcludeSystemTables)
		{
			DataTable dt = Session.GetTable("vwSYSTEM_SYNC_TABLES." + sTABLE_NAME);
			if ( dt == null )
			{
				dt = new DataTable();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							sSQL = "select MODULE_NAME                " + ControlChars.CrLf
							     + "     , TABLE_NAME                 " + ControlChars.CrLf
							     + "     , VIEW_NAME                  " + ControlChars.CrLf
							     + "     , MODULE_SPECIFIC            " + ControlChars.CrLf
							     + "     , MODULE_FIELD_NAME          " + ControlChars.CrLf
							     + "     , IS_ASSIGNED                " + ControlChars.CrLf
							     + "     , ASSIGNED_FIELD_NAME        " + ControlChars.CrLf
							     + "     , HAS_CUSTOM                 " + ControlChars.CrLf
							     + "     , IS_RELATIONSHIP            " + ControlChars.CrLf
							     + "     , MODULE_NAME_RELATED        " + ControlChars.CrLf
							     + "  from vwSYSTEM_SYNC_TABLES       " + ControlChars.CrLf
							     + " where TABLE_NAME = @TABLE_NAME   " + ControlChars.CrLf;
							if ( bExcludeSystemTables )
								sSQL += "   and IS_SYSTEM = 0              " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								cmd.CommandTimeout = 0;
								Sql.AddParameter(cmd, "@TABLE_NAME", sTABLE_NAME);
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dt);
									Session.SetTable("vwSYSTEM_SYNC_TABLES." + sTABLE_NAME, dt);
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		// 06/03/2011 Paul.  Cache the Rest Table data. 
		public DataTable RestTables(string sTABLE_NAME, bool bExcludeSystemTables)
		{
			// 07/09/2020 Paul.  The users table may be accessed by non-admin. 
			DataTable dt = Session.GetTable("vwSYSTEM_REST_TABLES." + sTABLE_NAME + (bExcludeSystemTables ? String.Empty : ".Admin"));
			if ( dt == null )
			{
				dt = new DataTable();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							// 05/25/2011 Paul.  Tables available to the REST API are not bound by the SYNC_ENABLED flag. 
							// 09/28/2011 Paul.  Include the system flag so that we can cache only system tables. 
							// 07/01/2018 Paul.  Add data privacy flag for the module. 
							// 08/01/2019 Paul.  We need a ListView and EditView flags for the Rest Client. 
							sSQL = "select *                          " + ControlChars.CrLf;
							if ( Config.enable_data_privacy() )
								sSQL += "     , (case when exists(select * from vwDATA_PRIVACY_FIELDS where vwDATA_PRIVACY_FIELDS.MODULE_NAME = vwSYSTEM_REST_TABLES.MODULE_NAME) then 1 else 0 end) as IS_DATA_PRIVACY_MODULE" + ControlChars.CrLf;
							sSQL += ""
							     + "  from vwSYSTEM_REST_TABLES       " + ControlChars.CrLf
							     + " where TABLE_NAME = @TABLE_NAME   " + ControlChars.CrLf;
							if ( bExcludeSystemTables )
								sSQL += "   and IS_SYSTEM = 0              " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								cmd.CommandTimeout = 0;
								Sql.AddParameter(cmd, "@TABLE_NAME", sTABLE_NAME);
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dt);
#if DEBUG
									// 03/11/2021 Paul.  If not found, then don't cache when in debug mode. 
									if ( dt.Rows.Count > 0 )
#endif
										Session.SetTable("vwSYSTEM_REST_TABLES." + sTABLE_NAME + (bExcludeSystemTables ? String.Empty : ".Admin"), dt);
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		// 11/25/2020 Paul.  We need a way to call a generic procedure.  Security is still managed through SYSTEM_REST_TABLES. 
		public DataTable RestProcedures(string sPROCEDURE_NAME, bool bExcludeSystemTables)
		{
			DataTable dt = Session.GetTable("vwSYSTEM_REST_PROCEDURES." + sPROCEDURE_NAME + (bExcludeSystemTables ? String.Empty : ".Admin"));
			if ( dt == null )
			{
				dt = new DataTable();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							sSQL = "select *                          " + ControlChars.CrLf
							     + "  from vwSYSTEM_REST_PROCEDURES   " + ControlChars.CrLf
							     + " where TABLE_NAME = @TABLE_NAME   " + ControlChars.CrLf;
							if ( bExcludeSystemTables )
								sSQL += "   and IS_SYSTEM = 0              " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								cmd.CommandTimeout = 0;
								Sql.AddParameter(cmd, "@TABLE_NAME", sPROCEDURE_NAME);
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dt);
									Session.SetTable("vwSYSTEM_REST_PROCEDURES." + sPROCEDURE_NAME + (bExcludeSystemTables ? String.Empty : ".Admin"), dt);
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		public void ClearFavorites()
		{
			Session.Remove("vwSUGARFAVORITES_MyFavorites");
			Session.Remove("vwSUGARFAVORITES_MyFavorites.React");
		}

		public DataTable Favorites()
		{
			DataTable dt = Session.GetTable("vwSUGARFAVORITES_MyFavorites");
			if ( dt == null )
			{
				dt = new DataTable();
				try
				{
					// 11/17/2007 Paul.  New function to determine if user is authenticated. 
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							sSQL = "select *                           " + ControlChars.CrLf
							     + "  from vwSUGARFAVORITES_MyFavorites" + ControlChars.CrLf
							     + " where USER_ID = @USER_ID          " + ControlChars.CrLf
							     + " order by ITEM_SUMMARY             " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@USER_ID", Security.USER_ID);
								
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dt);
									Session.SetTable("vwSUGARFAVORITES_MyFavorites", dt);
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 11/21/2005 Paul. Ignore error, but then we need to find a way to display the connection error. 
					// The most likely failure here is a connection failure. 
				}
			}
			return dt;
		}

		// 10/02/2016 Paul.  Favorites for use on Arctic theme. 
		public DataView Favorites(string sMODULE_NAME)
		{
			DataTable dtFavorites = Favorites();
			DataView vwFavorites = new DataView(dtFavorites);
			// 02/08/2018 Paul.  We can only filter if there is data and there is only data if authenticated. 
			if ( dtFavorites.Columns.Contains("MODULE_NAME") && !Sql.IsEmptyString(sMODULE_NAME) && sMODULE_NAME != "Home" )
				vwFavorites.RowFilter = "MODULE_NAME = '" + sMODULE_NAME + "'";
			return vwFavorites;
		}

		// 04/28/2019 Paul.  Flag to include Favorites and LastViewed for the React client. 
		public Dictionary<string, object> GetAllFavorites()
		{
			Dictionary<string, object> objs = Cache.Get("vwSUGARFAVORITES_MyFavorites.React") as Dictionary<string, object>;
			if ( objs == null )
			{
				objs = new Dictionary<string, object>();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						using ( DataTable dt = this.Favorites() )
						{
							DataView vw = new DataView(dt);
							vw.Sort = "MODULE_NAME, ITEM_SUMMARY";
							
							string sLAST_MODULE = String.Empty;
							List<object> arrLastModule = null;
							foreach ( DataRowView row in vw )
							{
								string sMODULE_NAME  = Sql.ToString(row["MODULE_NAME" ]);
								Guid   gITEM_ID      = Sql.ToGuid  (row["ITEM_ID"     ]);
								string sITEM_SUMMARY = Sql.ToString(row["ITEM_SUMMARY"]);
								if ( arrLastModule == null || sLAST_MODULE != sMODULE_NAME )
								{
									arrLastModule = new List<object>();
									objs.Add(sMODULE_NAME, arrLastModule);
									sLAST_MODULE = sMODULE_NAME;
								}
								Dictionary<string, object> obj = new Dictionary<string, object>();
								obj["ID"  ] = gITEM_ID;
								obj["NAME"] = sITEM_SUMMARY;
								arrLastModule.Add(obj);
							}
							Cache.Set("vwSUGARFAVORITES_MyFavorites.ReactClient", objs, DefaultCacheExpiration());
						}
					}
				}
				catch(Exception ex)
				{
					// 04/28/2019 Paul.  This error is not critical, so we can just log and ignore. 
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return objs;
		}

		// 10/09/2015 Paul.  Add support for subscriptions. 
		public void ClearSubscriptions()
		{
			Session.Remove("vwSUBSCRIPTIONS_MySubscriptions");
		}

		public DataTable Subscriptions()
		{
			DataTable dt = Session.GetTable("vwSUBSCRIPTIONS_MySubscriptions");
			if ( dt == null )
			{
				dt = new DataTable();
				try
				{
					// 11/17/2007 Paul.  New function to determine if user is authenticated. 
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							sSQL = "select *                              " + ControlChars.CrLf
							     + "  from vwSUBSCRIPTIONS                " + ControlChars.CrLf
							     + " where SUBSCRIPTION_USER_ID = @USER_ID" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@USER_ID", Security.USER_ID);
								
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dt);
									Session.SetTable("vwSUBSCRIPTIONS_MySubscriptions", dt);
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 11/21/2005 Paul. Ignore error, but then we need to find a way to display the connection error. 
					// The most likely failure here is a connection failure. 
				}
			}
			return dt;
		}


		// 09/10/2012 Paul.  Add User Signatures. 
		public void ClearUserSignatures()
		{
			Session.Remove("vwUSERS_SIGNATURES");
		}

		// 09/10/2012 Paul.  Add User Signatures. 
		public DataTable UserSignatures()
		{
			DataTable dt = Session.GetTable("vwUSERS_SIGNATURES");
			if ( dt == null )
			{
				dt = new DataTable();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							// 05/03/2020 Paul.  The React Client needs the PRIMARY_SIGNATURE. 
							sSQL = "select ID                             " + ControlChars.CrLf
							     + "     , NAME                           " + ControlChars.CrLf
							     + "     , SIGNATURE_HTML                 " + ControlChars.CrLf
							     + "     , PRIMARY_SIGNATURE              " + ControlChars.CrLf
							     + "  from vwUSERS_SIGNATURES             " + ControlChars.CrLf
							     + " where USER_ID = @USER_ID             " + ControlChars.CrLf
							     + " order by PRIMARY_SIGNATURE desc, NAME" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@USER_ID", Security.USER_ID);
								
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dt);
									Session.SetTable("vwUSERS_SIGNATURES", dt);
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		// 05/03/2020 Paul.  Emails.EditView should use cached list of signatures. 
		public Dictionary<string, object> GetUserSignatures()
		{
			Dictionary<string, object> objs = new Dictionary<string, object>();
			try
			{
				if ( Security.IsAuthenticated() )
				{
					DataTable dt = UserSignatures();
					foreach ( DataRow row in dt.Rows )
					{
						Guid   gID                = Sql.ToGuid   (row["ID"               ]);
						string sNAME              = Sql.ToString (row["NAME"             ]);
						string sSIGNATURE_HTML    = Sql.ToString (row["SIGNATURE_HTML"   ]);
						bool   bPRIMARY_SIGNATURE = Sql.ToBoolean(row["PRIMARY_SIGNATURE"]);
						Dictionary<string, object> obj = new Dictionary<string, object>();
						obj["ID"               ] = gID               ;
						obj["NAME"             ] = sNAME             ;
						obj["SIGNATURE_HTML"   ] = sSIGNATURE_HTML   ;
						obj["PRIMARY_SIGNATURE"] = bPRIMARY_SIGNATURE;
						objs.Add(gID.ToString(), obj);
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				throw;
			}
			return objs;
		}

		// 07/18/2013 Paul.  Add support for multiple outbound emails. 
		// 04/20/2016 Paul.  Add team management to Outbound Emails. 
		public void ClearOutboundMail()
		{
			foreach(string sKey in GetCacheKeys())
			{
				if ( sKey.StartsWith("vwOUTBOUND_EMAILS") )
					Cache.Remove(sKey);
			}
		}

		// 07/18/2013 Paul.  Add support for multiple outbound emails. 
		// 04/20/2016 Paul.  Add team management to Outbound Emails. 
		public DataTable OutboundMail()
		{
			Guid gUSER_ID = Security.USER_ID;
			// 02/09/2017 Paul.  Change from Session to global cache. 
			//HttpSessionState Session = HttpContext.Current.Session;
			//DataTable dt = Session["vwOUTBOUND_EMAILS." + gUSER_ID.ToString()] as DataTable;
			DataTable dt = Cache.Get("vwOUTBOUND_EMAILS." + gUSER_ID.ToString()) as DataTable;
			if ( dt == null )
			{
				dt = new DataTable();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							// 01/17/2017 Paul.  Get all fields. 
							sSQL = "select *                " + ControlChars.CrLf
							     + "  from vwOUTBOUND_EMAILS" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								// 04/20/2016 Paul.  Add team management to Outbound Emails. 
								// 04/20/2016 Paul.  We are not going to use the default filter as the Require Team flag can dramatically change the behavior. 
								// We want old records with null TEAM_ID to still be accessible, so we must use the left outer join. 
								//Security.Filter(cmd, "OutboundEmail", "list");
								bool bEnableTeamManagement  = Config.enable_team_management();
								bool bEnableDynamicTeams    = Config.enable_dynamic_teams();
								if ( bEnableTeamManagement )
								{
									if ( bEnableDynamicTeams )
									{
										cmd.CommandText += "  left outer join " + Sql.MetadataName(cmd, "vwTEAM_SET_MEMBERSHIPS_Security") + " vwTEAM_SET_MEMBERSHIPS" + ControlChars.CrLf;
										cmd.CommandText += "               on vwTEAM_SET_MEMBERSHIPS.MEMBERSHIP_TEAM_SET_ID = TEAM_SET_ID        " + ControlChars.CrLf;
										cmd.CommandText += "              and vwTEAM_SET_MEMBERSHIPS.MEMBERSHIP_USER_ID     = @MEMBERSHIP_USER_ID" + ControlChars.CrLf;
									}
									else
									{
										cmd.CommandText += "  left outer join vwTEAM_MEMBERSHIPS" + ControlChars.CrLf;
										cmd.CommandText += "               on vwTEAM_MEMBERSHIPS.MEMBERSHIP_TEAM_ID = TEAM_ID            " + ControlChars.CrLf;
										cmd.CommandText += "              and vwTEAM_MEMBERSHIPS.MEMBERSHIP_USER_ID = @MEMBERSHIP_USER_ID" + ControlChars.CrLf;
									}
									Sql.AddParameter(cmd, "@MEMBERSHIP_USER_ID", Security.USER_ID);
								}
								cmd.CommandText += " where 1 = 1" + ControlChars.CrLf;
								if ( bEnableTeamManagement )
								{
									if ( bEnableDynamicTeams )
										cmd.CommandText += "   and (TEAM_SET_ID is null or vwTEAM_SET_MEMBERSHIPS.MEMBERSHIP_TEAM_SET_ID is not null)" + ControlChars.CrLf;
									else
										cmd.CommandText += "   and (TEAM_ID is null or vwTEAM_MEMBERSHIPS.MEMBERSHIP_TEAM_ID is not null)" + ControlChars.CrLf;
								}
								cmd.CommandText += "   and (USER_ID = @USER_ID or USER_ID is null)" + ControlChars.CrLf;
								Sql.AddParameter(cmd, "@USER_ID", gUSER_ID);
								cmd.CommandText += " order by USER_ID desc, DISPLAY_NAME  " + ControlChars.CrLf;
								
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dt);
									// 02/09/2017 Paul.  Change from Session to global cache. 
									//Session["vwOUTBOUND_EMAILS." + gUSER_ID.ToString()] = dt;
									Cache.Set("vwOUTBOUND_EMAILS." + gUSER_ID.ToString(), dt, DefaultCacheExpiration());
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		// 05/05/2020 Paul.  Emails.EditView should use cached list of OutboundMail. 
		public Dictionary<string, object> GetOutboundMail()
		{
			Dictionary<string, object> objs = new Dictionary<string, object>();
			try
			{
				if ( Security.IsAuthenticated() )
				{
					DataTable dt = OutboundMail();
					foreach ( DataRow row in dt.Rows )
					{
						Guid   gID                       = Sql.ToGuid   (row["ID"                      ]);
						string sNAME                     = Sql.ToString (row["NAME"                    ]);
						string sDISPLAY_NAME             = Sql.ToString (row["DISPLAY_NAME"            ]);
						string sTYPE                     = Sql.ToString (row["TYPE"                    ]);
						string sFROM_NAME                = Sql.ToString (row["FROM_NAME"               ]);
						string sFROM_ADDR                = Sql.ToString (row["FROM_ADDR"               ]);
						bool   bOFFICE365_OAUTH_ENABLED  = Sql.ToBoolean(row["OFFICE365_OAUTH_ENABLED" ]);
						bool   bGOOGLEAPPS_OAUTH_ENABLED = Sql.ToBoolean(row["GOOGLEAPPS_OAUTH_ENABLED"]);
						Dictionary<string, object> obj = new Dictionary<string, object>();
						obj["ID"                      ] = gID                      ;
						obj["NAME"                    ] = sNAME                    ;
						obj["DISPLAY_NAME"            ] = sDISPLAY_NAME            ;
						obj["TYPE"                    ] = sTYPE                    ;
						obj["FROM_NAME"               ] = sFROM_NAME               ;
						obj["FROM_ADDR"               ] = sFROM_ADDR               ;
						obj["OFFICE365_OAUTH_ENABLED" ] = bOFFICE365_OAUTH_ENABLED ;
						obj["GOOGLEAPPS_OAUTH_ENABLED"] = bGOOGLEAPPS_OAUTH_ENABLED;
						objs.Add(gID.ToString(), obj);
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				throw;
			}
			return objs;
		}

		// 09/23/2013 Paul.  Add support for multiple outbound sms. 
		public void ClearOutboundSms()
		{
			Session.Remove("vwOUTBOUND_SMS");
		}

		// 09/23/2013 Paul.  Add support for multiple outbound sms. 
		public DataTable OutboundSms()
		{
			DataTable dt = Session.GetTable("vwOUTBOUND_SMS") as DataTable;
			if ( dt == null )
			{
				dt = new DataTable();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							sSQL = "select ID                           " + ControlChars.CrLf
							     + "     , NAME                         " + ControlChars.CrLf
							     + "     , FROM_NUMBER                  " + ControlChars.CrLf
							     + "     , DISPLAY_NAME                 " + ControlChars.CrLf
							     + "  from vwOUTBOUND_SMS               " + ControlChars.CrLf
							     + " where USER_ID = @USER_ID           " + ControlChars.CrLf
							     + "    or USER_ID is null              " + ControlChars.CrLf
							     + " order by USER_ID desc, DISPLAY_NAME" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@USER_ID", Security.USER_ID);
								
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dt);
									Session.SetTable("vwOUTBOUND_SMS", dt);
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		// 05/05/2020 Paul.  Emails.EditView should use cached list of OutboundMail. 
		public Dictionary<string, object> GetOutboundSms()
		{
			Dictionary<string, object> objs = new Dictionary<string, object>();
			try
			{
				if ( Security.IsAuthenticated() )
				{
					DataTable dt = OutboundSms();
					foreach ( DataRow row in dt.Rows )
					{
						Guid   gID           = Sql.ToGuid   (row["ID"          ]);
						string sNAME         = Sql.ToString (row["NAME"        ]);
						string sDISPLAY_NAME = Sql.ToString (row["DISPLAY_NAME"]);
						string sFROM_NUMBER  = Sql.ToString (row["FROM_NUMBER" ]);
						Dictionary<string, object> obj = new Dictionary<string, object>();
						obj["ID"          ] = gID          ;
						obj["NAME"        ] = sNAME        ;
						obj["DISPLAY_NAME"] = sDISPLAY_NAME;
						obj["FROM_NUMBER" ] = sFROM_NUMBER ;
						objs.Add(gID.ToString(), obj);
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				throw;
			}
			return objs;
		}

		// 12/24/2012 Paul.  Cache the activity reminders for 5 minutes. 
		public void ClearUserReminders()
		{
			Cache.Remove("vwACTIVITIES_Reminders." + Security.USER_ID.ToString());
		}

		// 12/24/2012 Paul.  Cache the activity reminders for 5 minutes. 
		public DataTable UserReminders()
		{
			DataTable dt = Cache.Get("vwACTIVITIES_Reminders." + Security.USER_ID.ToString()) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 12/25/2012 Paul.  Date math is handled by the view. 
						sSQL = "select *                     " + ControlChars.CrLf
						     + "  from vwACTIVITIES_Reminders" + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							// 06/02/2016 Paul.  Activities views will use new function that accepts an array of modules. 
							// 06/09/2017 Paul.  We need to include tasks. 
							// 12/03/2017 Paul.  Module name field needs to be a parameter because it can change between MODULE_NAME and ACTIVITY_TYPE. 
							Security.Filter(cmd, new string[] {"Calls", "Meetings", "Tasks"}, "list", "ASSIGNED_USER_ID", "ACTIVITY_TYPE");
							Sql.AppendParameter(cmd, Security.USER_ID, "USER_ID");
							cmd.CommandText += " order by DATE_START" + ControlChars.CrLf;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwACTIVITIES_Reminders." + Security.USER_ID.ToString(), dt, CacheExpiration5Minutes());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		public void ClearTwitterTracks()
		{
			Session.Remove("vwTWITTER_TRACKS");
			Cache.Remove("vwTWITTER_TRACKS");
		}

		// 10/26/2013 Paul.  Add TwitterTrackers to the cache. 
		public string MyTwitterTracks()
		{
			if ( Session["vwTWITTER_TRACKS"] == null )
			{
				try
				{
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							sSQL = "select distinct NAME              " + ControlChars.CrLf
							     + "  from vwTWITTER_TRACKS_Active    " + ControlChars.CrLf
							     + " where ASSIGNED_USER_ID = @USER_ID" + ControlChars.CrLf
							     + " order by NAME                    " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@USER_ID", Security.USER_ID);
								
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dt = new DataTable() )
									{
										da.Fill(dt);
										// 10/27/2013 Paul.  Prebuild the tracks string so that a page request is fast. 
										StringBuilder sbTracks = new StringBuilder();
										if ( dt != null && dt.Rows.Count > 0 )
										{
											foreach ( DataRow row in dt.Rows )
											{
												string sNAME = Sql.ToString(row["NAME"]);
												if ( !Sql.IsEmptyString(sNAME) )
												{
													if ( sbTracks.Length > 0 )
														sbTracks.Append(',');
													// 10/27/2013 Paul.  The Twitter events are mapped in lowercase, make lowercase in advance. 
													sbTracks.Append(sNAME.ToLower());
												}
											}
										}
										Session["vwTWITTER_TRACKS"] = sbTracks.ToString();
									}
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return Sql.ToString(Session["vwTWITTER_TRACKS"]);
		}

		public DataTable TwitterTracks(HttpApplicationState Application)
		{
			DataTable dt = Cache.Get("vwTWITTER_TRACKS") as DataTable;
			if ( dt == null )
			{
				dt = new DataTable();
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select NAME                   " + ControlChars.CrLf
						     + "     , TYPE                   " + ControlChars.CrLf
						     + "  from vwTWITTER_TRACKS_Active" + ControlChars.CrLf
						     + " order by NAME                " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								da.Fill(dt);
								Cache.Set("vwTWITTER_TRACKS", dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		public void ClearChatChannels()
		{
			Session.Remove("vwCHAT_CHANNELS");
			Cache.Remove("vwCHAT_CHANNELS");
		}

		// 11/10/2014 Paul.  Add ChatChannels to the cache. 
		public string MyChatChannels()
		{
			if ( Session["vwCHAT_CHANNELS"] == null )
			{
				try
				{
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							sSQL = "select ID             " + ControlChars.CrLf
							     + "  from vwCHAT_CHANNELS" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Security.Filter(cmd, "ChatChannels", "list");
								
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dt = new DataTable() )
									{
										da.Fill(dt);
										StringBuilder sbChannels = new StringBuilder();
										if ( dt != null && dt.Rows.Count > 0 )
										{
											foreach ( DataRow row in dt.Rows )
											{
												Guid gID = Sql.ToGuid(row["ID"]);
												if ( sbChannels.Length > 0 )
													sbChannels.Append(',');
												sbChannels.Append(gID.ToString());
											}
										}
										Session["vwCHAT_CHANNELS"] = sbChannels.ToString();
									}
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return Sql.ToString(Session["vwCHAT_CHANNELS"]);
		}

		// 12/12/2015 Paul.  /n Software and .netCharge use different lists. 
		public DataTable LibraryPaymentGateways()
		{
			string sLibraryListName = "payment_gateway_dom";
			// dotnetCharge.dll or nsoftware.InPayWeb.dll. 
			if ( Sql.ToString(Application["CONFIG.PaymentGateway.Library"]) == "nsoftware.InPayWeb" )
				sLibraryListName = "payment_gateway_nsoftware";
			DataTable dt = Cache.Get("vwAPI_PAYMENT_GATEWAYS." + sLibraryListName) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select NAME                         " + ControlChars.CrLf
						     + "     , DISPLAY_NAME                 " + ControlChars.CrLf
						     + "  from vwTERMINOLOGY                " + ControlChars.CrLf
						     + " where lower(LIST_NAME) = @LIST_NAME" + ControlChars.CrLf
						     + "   and lower(LANG     ) = @LANG     " + ControlChars.CrLf
						     + " order by LIST_ORDER                " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@LIST_NAME", sLibraryListName);
							// 12/12/2015 Paul.  Use the english list as the primary location. 
							Sql.AddParameter(cmd, "@LANG"     , "en-us");
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwAPI_PAYMENT_GATEWAYS." + sLibraryListName, dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		// 12/16/2015 Paul.  credit_card_year should be a custom list that adds 10 years to current year. 
		public DataTable CreditCardYears()
		{
			DataTable dt = Cache.Get(".credit_card_year") as DataTable;
			if ( dt == null )
			{
				try
				{
					dt = new DataTable();
					dt.Columns.Add("NAME"        );
					dt.Columns.Add("DISPLAY_NAME");
					for ( int i = DateTime.Today.Year - 1; i <  DateTime.Today.Year + 10; i++ )
					{
						DataRow row = dt.NewRow();
						dt.Rows.Add(row);
						row["NAME"        ] = i.ToString();
						row["DISPLAY_NAME"] = i.ToString();
					}
					Cache.Set(".credit_card_year", dt, DefaultCacheExpiration());
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		public DataTable StreamModules(Guid gUSER_ID)
		{
			DataTable dt = Cache.Get(L10n.NAME + ".vwMODULES_Stream_ByUser_" + gUSER_ID.ToString()) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select MODULE_NAME             " + ControlChars.CrLf
						     + "     , DISPLAY_NAME            " + ControlChars.CrLf
						     + "  from vwMODULES_Stream_ByUser " + ControlChars.CrLf
						     + " where USER_ID = @USER_ID      " + ControlChars.CrLf
						     + " order by MODULE_NAME          " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@USER_ID", gUSER_ID);
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								foreach ( DataRow row in dt.Rows )
								{
									row["DISPLAY_NAME"] = L10n.Term(Sql.ToString(row["DISPLAY_NAME"]));
								}
								Cache.Set(L10n.NAME + ".vwMODULES_Stream_ByUser_" + gUSER_ID.ToString(), dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemMessage("Error", new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		public List<string> StreamModulesArray(Guid gUSER_ID)
		{
			List<string> arr = new List<string>();
			DataTable dt = StreamModules(gUSER_ID);
			foreach ( DataRow row in dt.Rows )
			{
				string sMODULE_NAME = Sql.ToString(row["MODULE_NAME"]);
				arr.Add(sMODULE_NAME);
			}
			return arr;
		}

		// 10/05/2017 Paul.  Add Archive relationship view. 
		public void ClearArchiveViewExists()
		{
			foreach(string sKey in GetCacheKeys())
			{
				if ( sKey.StartsWith("ArchiveViewExists.") )
					Cache.Remove(sKey);
			}
		}

		// 10/05/2017 Paul.  Add Archive relationship view. 
		public bool ArchiveViewExists(string sVIEW_NAME)
		{
			string sARCHIVE_VIEW = sVIEW_NAME + "_ARCHIVE";
			object oExists = Cache.Get("ArchiveViewExists." + sARCHIVE_VIEW);
			if ( oExists == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select count(*)              " + ControlChars.CrLf
						     + "  from vwSqlViews            " + ControlChars.CrLf
						     + " where VIEW_NAME = @VIEW_NAME" + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@VIEW_NAME", Sql.MetadataName(con, sARCHIVE_VIEW));
							oExists = Sql.ToBoolean(cmd.ExecuteScalar());
							Cache.Set("ArchiveViewExists." + sARCHIVE_VIEW, oExists, DefaultCacheExpiration());
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemMessage("Error", new StackTrace(true).GetFrame(0), ex);
				}
			}
			return Sql.ToBoolean(oExists);
		}

		// 06/27/2018 Paul.  Add ERASED_FIELDS when data privacy enabled. 
		public DataTable DataPrivacyFields()
		{
			L10N L10n = new L10N(Session["USER_SETTINGS/CULTURE"] as string);
			DataTable dt = Cache.Get(L10n.NAME + "." + "vwDATA_PRIVACY_FIELDS") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select MODULE_NAME               " + ControlChars.CrLf
						     + "     , FIELD_NAME                " + ControlChars.CrLf
						     + "  from vwDATA_PRIVACY_FIELDS     " + ControlChars.CrLf
						     + " order by MODULE_NAME, FIELD_NAME" + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								dt.Columns.Add("NAME"        , Type.GetType("System.String"));
								dt.Columns.Add("DISPLAY_NAME", Type.GetType("System.String"));
								foreach ( DataRow row in dt.Rows )
								{
									string sMODULE_NAME = Sql.ToString(row["MODULE_NAME"]);
									string sDISPLAY_NAME = L10n.Term(sMODULE_NAME + ".LBL_" + Sql.ToString(row["FIELD_NAME"]));
									sDISPLAY_NAME = sDISPLAY_NAME.Replace(":", String.Empty);
									row["NAME"        ] = sMODULE_NAME + "." + Sql.ToString(row["FIELD_NAME"]);
									row["DISPLAY_NAME"] = L10n.Term(".moduleList." + sMODULE_NAME) + sDISPLAY_NAME;
								}
								Cache.Set(L10n.NAME + "." + "vwDATA_PRIVACY_FIELDS", dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemMessage("Error", new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		#region REST API helpers
		// 02/22/2021 Paul.  The React client needs a way to determine the default sort, besides NAME asc. 
		public Dictionary<string, object> GetAllGridViews(List<string> lstMODULES)
		{
			// 08/09/2020 Paul.  Convert to comma separated string. 
			string sModuleList = String.Join(",", lstMODULES.ToArray());
			Dictionary<string, object> objs = Cache.Get("vwGRIDVIEWS.ReactClient." + sModuleList) as Dictionary<string, object>;
			if ( objs == null )
			{
				objs = new Dictionary<string, object>();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						List<string> lstMODULES_WithAdmin = new List<string>(lstMODULES);
						// 10/05/2020 Paul.  Users is always available for the Users.PopupView. 
						lstMODULES_WithAdmin.Add("Users"          );
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL = String.Empty;
							sSQL = "select *          " + ControlChars.CrLf
							     + "  from vwGRIDVIEWS" + ControlChars.CrLf
							     + " order by NAME    " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
							
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dt = new DataTable() )
									{
										da.Fill(dt);
										for ( int i = 0; i < dt.Rows.Count; i++ )
										{
											DataRow row = dt.Rows[i];
											string sGRID_NAME   = Sql.ToString(row["NAME" ]);
											string sMODULE_NAME = String.Empty;
											string[] arrGRID_NAME = sGRID_NAME.Split('.');
											if ( arrGRID_NAME.Length > 0 )
											{
												if ( arrGRID_NAME[0] == "ListView" || arrGRID_NAME[0] == "PopupView" || arrGRID_NAME[0] == "Activities" )
													sMODULE_NAME = arrGRID_NAME[0];
												else if ( Sql.ToBoolean(Application["Modules." + arrGRID_NAME[1] + ".Valid"]) )
													sMODULE_NAME = arrGRID_NAME[1];
												else
													sMODULE_NAME = arrGRID_NAME[0];
											}
											if ( !lstMODULES_WithAdmin.Contains(sMODULE_NAME) )
												continue;
											Dictionary<string, object> drow = new Dictionary<string, object>();
											for ( int j = 0; j < dt.Columns.Count; j++ )
											{
												if ( dt.Columns[j].ColumnName == "ID" )
													continue;
												drow.Add(dt.Columns[j].ColumnName, row[j]);
											}
											objs.Add(sGRID_NAME, drow);
										}
										Cache.Set("vwGRIDVIEWS.ReactClient." + sModuleList, objs, DefaultCacheExpiration());
									}
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					throw;
				}
			}
			return objs;
		}

		// 03/02/2019 Paul.  Functions are now static and take modules list input so that they can be used in the Admin API. 
		public Dictionary<string, object> GetAllGridViewsColumns(List<string> lstMODULES)
		{
			// 08/09/2020 Paul.  Convert to comma separated string. 
			string sModuleList = String.Join(",", lstMODULES.ToArray());
			Dictionary<string, object> objs = Cache.Get("vwGRIDVIEWS_COLUMNS.ReactClient." + sModuleList) as Dictionary<string, object>;
			if ( objs == null )
			{
				objs = new Dictionary<string, object>();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						List<string> lstMODULES_WithAdmin = new List<string>(lstMODULES);
						// 10/05/2020 Paul.  Users is always available for the Users.PopupView. 
						lstMODULES_WithAdmin.Add("Home"           );
						lstMODULES_WithAdmin.Add("Users"          );
						// 08/30/2021 Paul.  Add Azure modules if they exist. 
						lstMODULES_WithAdmin.Add("DnsNames"       );
						lstMODULES_WithAdmin.Add("ResourceGroups" );
						lstMODULES_WithAdmin.Add("SqlDatabases"   );
						lstMODULES_WithAdmin.Add("SqlServers"     );
						lstMODULES_WithAdmin.Add("StorageAccounts");
						lstMODULES_WithAdmin.Add("VirtualMachines");
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL = String.Empty;
							sSQL = "select *                                         " + ControlChars.CrLf
							     + "  from vwGRIDVIEWS_COLUMNS                       " + ControlChars.CrLf
							     + " where (DEFAULT_VIEW = 0 or DEFAULT_VIEW is null)" + ControlChars.CrLf
							     + " order by GRID_NAME, COLUMN_INDEX                " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
							
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dt = new DataTable() )
									{
										da.Fill(dt);
										bool bClearScript = false;
										string sLAST_VIEW_NAME = String.Empty;
										List<Dictionary<string, object>> layout = null;
										for ( int i = 0; i < dt.Rows.Count; i++ )
										{
											DataRow row = dt.Rows[i];
											string sGRID_NAME   = Sql.ToString(row["GRID_NAME" ]);
											string sDATA_FIELD  = Sql.ToString(row["DATA_FIELD"]);
											string sMODULE_NAME = String.Empty;
											string[] arrGRID_NAME = sGRID_NAME.Split('.');
											if ( arrGRID_NAME.Length > 0 )
											{
												if ( arrGRID_NAME[0] == "ListView" || arrGRID_NAME[0] == "PopupView" || arrGRID_NAME[0] == "Activities" )
													sMODULE_NAME = arrGRID_NAME[0];
												else if ( Sql.ToBoolean(Application["Modules." + arrGRID_NAME[1] + ".Valid"]) )
													sMODULE_NAME = arrGRID_NAME[1];
												else
													sMODULE_NAME = arrGRID_NAME[0];
											}
											if ( !lstMODULES_WithAdmin.Contains(sMODULE_NAME) )
												continue;
											if ( sLAST_VIEW_NAME != sGRID_NAME )
											{
												bClearScript = false;
												sLAST_VIEW_NAME = sGRID_NAME;
												layout = new List<Dictionary<string, object>>();
												objs.Add(sLAST_VIEW_NAME, layout);
											}
											bool bIsReadable = true;
											if ( SplendidInit.bEnableACLFieldSecurity && !Sql.IsEmptyString(sDATA_FIELD) )
											{
												Security.ACL_FIELD_ACCESS acl = Security.GetUserFieldSecurity(sMODULE_NAME, sDATA_FIELD, Guid.Empty);
												bIsReadable  = acl.IsReadable();
											}
											// 09/20/2012 Paul.  We need a SCRIPT field that is form specific, but only on the first record of the layout. 
											if ( bClearScript )
												row["SCRIPT"] = DBNull.Value;
											bClearScript = true;
											if ( bIsReadable )
											{
												Dictionary<string, object> drow = new Dictionary<string, object>();
												for ( int j = 0; j < dt.Columns.Count; j++ )
												{
													if ( dt.Columns[j].ColumnName == "ID" )
														continue;
													// 10/13/2012 Paul.  Must not return value as a string as the client is expecting numerics and booleans in their native format. 
													drow.Add(dt.Columns[j].ColumnName, row[j]);
												}
												layout.Add(drow);
											}
										}
										Cache.Set("vwGRIDVIEWS_COLUMNS.ReactClient." + sModuleList, objs, DefaultCacheExpiration());
									}
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					throw;
				}
			}
			return objs;
		}

		// 03/02/2019 Paul.  Functions are now static and take modules list input so that they can be used in the Admin API. 
		public Dictionary<string, object> GetAllDetailViewsFields(List<string> lstMODULES)
		{
			// 08/09/2020 Paul.  Convert to comma separated string. 
			string sModuleList = String.Join(",", lstMODULES.ToArray());
			Dictionary<string, object> objs = Cache.Get("vwDETAILVIEWS_FIELDS.ReactClient." + sModuleList) as Dictionary<string, object>;
			if ( objs == null )
			{
				objs = new Dictionary<string, object>();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						List<string> lstMODULES_WithAdmin = new List<string>(lstMODULES);
						// 10/05/2020 Paul.  Users is always available for the Users.PopupView. 
						lstMODULES_WithAdmin.Add("Home"           );
						lstMODULES_WithAdmin.Add("Users"          );
						lstMODULES_WithAdmin.Add("Google"         );
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL = String.Empty;
							sSQL = "select *                                         " + ControlChars.CrLf
							     + "  from vwDETAILVIEWS_FIELDS                      " + ControlChars.CrLf
							     + " where (DEFAULT_VIEW = 0 or DEFAULT_VIEW is null)" + ControlChars.CrLf
							     + " order by DETAIL_NAME, FIELD_INDEX               " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
							
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dt = new DataTable() )
									{
										da.Fill(dt);
										bool bClearScript = false;
										string sLAST_VIEW_NAME = String.Empty;
										List<Dictionary<string, object>> layout = null;
										for ( int i = 0; i < dt.Rows.Count; i++ )
										{
											DataRow row = dt.Rows[i];
											string sDETAIL_NAME = Sql.ToString (row["DETAIL_NAME"]);
											string sDATA_FIELD  = Sql.ToString (row["DATA_FIELD" ]);
											string sMODULE_NAME = String.Empty;
											string[] arrDETAIL_NAME = sDETAIL_NAME.Split('.');
											if ( arrDETAIL_NAME.Length > 0 )
												sMODULE_NAME = arrDETAIL_NAME[0];
											if ( !lstMODULES_WithAdmin.Contains(sMODULE_NAME) )
												continue;
											if ( sLAST_VIEW_NAME != sDETAIL_NAME )
											{
												bClearScript = false;
												sLAST_VIEW_NAME = sDETAIL_NAME;
												layout = new List<Dictionary<string, object>>();
												objs.Add(sLAST_VIEW_NAME, layout);
											}
											bool bIsReadable  = true;
											if ( SplendidInit.bEnableACLFieldSecurity && !Sql.IsEmptyString(sDATA_FIELD) )
											{
												// 09/03/2011 Paul.  Can't apply Owner rights without the item record. 
												Security.ACL_FIELD_ACCESS acl = Security.GetUserFieldSecurity(sMODULE_NAME, sDATA_FIELD, Guid.Empty);
												bIsReadable  = acl.IsReadable();
											}
											// 09/20/2012 Paul.  We need a SCRIPT field that is form specific, but only on the first record of the layout. 
											if ( bClearScript )
												row["SCRIPT"] = DBNull.Value;
											bClearScript = true;
											if ( bIsReadable )
											{
												Dictionary<string, object> drow = new Dictionary<string, object>();
												for ( int j = 0; j < dt.Columns.Count; j++ )
												{
													if ( dt.Columns[j].ColumnName == "ID" )
														continue;
													// 10/13/2012 Paul.  Must not return value as a string as the client is expecting numerics and booleans in their native format. 
													drow.Add(dt.Columns[j].ColumnName, row[j]);
												}
												layout.Add(drow);
											}
										}
										Cache.Set("vwDETAILVIEWS_FIELDS.ReactClient." + sModuleList, objs, DefaultCacheExpiration());
									}
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					throw;
				}
			}
			return objs;
		}

		// 03/02/2019 Paul.  Functions are now static and take modules list input so that they can be used in the Admin API. 
		public Dictionary<string, object> GetAllEditViewsFields(List<string> lstMODULES)
		{
			// 08/09/2020 Paul.  Convert to comma separated string. 
			string sModuleList = String.Join(",", lstMODULES.ToArray());
			Dictionary<string, object> objs = Cache.Get("vwEDITVIEWS_FIELDS.ReactClient." + sModuleList) as Dictionary<string, object>;
			if ( objs == null )
			{
				objs = new Dictionary<string, object>();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						List<string> lstMODULES_WithAdmin = new List<string>(lstMODULES);
						// 10/05/2020 Paul.  Users is always available for the Users.PopupView. 
						lstMODULES_WithAdmin.Add("Home"           );
						lstMODULES_WithAdmin.Add("Users"          );
						lstMODULES_WithAdmin.Add("Google"         );
						// 10/27/2021 Paul.  Add Configurator for AdminWizard. 
						lstMODULES_WithAdmin.Add("Configurator"   );
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL = String.Empty;
							// 05/16/2019 Paul.  The React client will use the IS_MULTI_SELECT field at some point. 
							sSQL = "select *                                         " + ControlChars.CrLf
							     + "  from vwEDITVIEWS_FIELDS_SearchView             " + ControlChars.CrLf
							     + " where (DEFAULT_VIEW = 0 or DEFAULT_VIEW is null)" + ControlChars.CrLf
							     + " order by EDIT_NAME, FIELD_INDEX                 " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
							
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dt = new DataTable() )
									{
										da.Fill(dt);
										bool bClearScript = false;
										string sLAST_VIEW_NAME = String.Empty;
										List<Dictionary<string, object>> layout = null;
										for ( int i = 0; i < dt.Rows.Count; i++ )
										{
											DataRow row = dt.Rows[i];
											string sEDIT_NAME     = Sql.ToString(row["EDIT_NAME"    ]);
											string sFIELD_TYPE    = Sql.ToString(row["FIELD_TYPE"   ]);
											string sDATA_FIELD    = Sql.ToString(row["DATA_FIELD"   ]);
											string sDATA_FORMAT   = Sql.ToString(row["DATA_FORMAT"  ]);
											string sDISPLAY_FIELD = Sql.ToString(row["DISPLAY_FIELD"]);
											string sMODULE_NAME   = String.Empty;
											string[] arrEDIT_NAME = sEDIT_NAME.Split('.');
											if ( arrEDIT_NAME.Length > 0 )
												sMODULE_NAME = arrEDIT_NAME[0];
											if ( !lstMODULES_WithAdmin.Contains(sMODULE_NAME) )
												continue;
											if ( sLAST_VIEW_NAME != sEDIT_NAME )
											{
												bClearScript = false;
												sLAST_VIEW_NAME = sEDIT_NAME;
												layout = new List<Dictionary<string, object>>();
												objs.Add(sLAST_VIEW_NAME, layout);
											}
											bool bIsReadable  = true;
											bool bIsWriteable = true;
											if ( SplendidInit.bEnableACLFieldSecurity )
											{
												// 09/03/2011 Paul.  Can't apply Owner rights without the item record. 
												Security.ACL_FIELD_ACCESS acl = Security.GetUserFieldSecurity(sMODULE_NAME, sDATA_FIELD, Guid.Empty);
												bIsReadable  = acl.IsReadable();
												// 02/16/2011 Paul.  We should allow a Read-Only field to be searchable, so always allow writing if the name contains Search. 
												bIsWriteable = acl.IsWriteable() || sEDIT_NAME.Contains(".Search");
											}
											if ( !bIsReadable )
											{
												row["FIELD_TYPE"] = "Blank";
											}
											else if ( !bIsWriteable )
											{
												row["FIELD_TYPE"] = "Label";
											}
											// 09/20/2012 Paul.  We need a SCRIPT field that is form specific, but only on the first record of the layout. 
											if ( bClearScript )
												row["SCRIPT"] = DBNull.Value;
											bClearScript = true;
											Dictionary<string, object> drow = new Dictionary<string, object>();
											for ( int j = 0; j < dt.Columns.Count; j++ )
											{
												if ( dt.Columns[j].ColumnName == "ID" )
													continue;
												// 10/13/2012 Paul.  Must not return value as a string as the client is expecting numerics and booleans in their native format. 
												drow.Add(dt.Columns[j].ColumnName, row[j]);
											}
											layout.Add(drow);
										}
										Cache.Set("vwEDITVIEWS_FIELDS.ReactClient." + sModuleList, objs, DefaultCacheExpiration());
									}
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					throw;
				}
			}
			return objs;
		}

		// 03/02/2019 Paul.  Functions are now static and take modules list input so that they can be used in the Admin API. 
		public Dictionary<string, object> GetAllDetailViewsRelationships(List<string> lstMODULES)
		{
			// 08/09/2020 Paul.  Convert to comma separated string. 
			string sModuleList = String.Join(",", lstMODULES.ToArray());
			Dictionary<string, object> objs = Cache.Get("vwDETAILVIEWS_RELATIONSHIPS.ReactClient." + sModuleList) as Dictionary<string, object>;
			if ( objs == null )
			{
				objs = new Dictionary<string, object>();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL = String.Empty;
							// 10/09/2012 Paul.  Make sure to filter by modules with REST enabled. 
							// 11/30/2012 Paul.  Activities is not a real table, but it should be included. 
							// 05/09/2017 Paul.  Adding new Dashboard to HTML5 client. 
							// 05/09/2017 Paul.  Adding new Home to HTML5 client. 
							// 08/20/2019 Paul.  Include ActivityStream for the React Client. 
							// 03/09/2021 Paul.  Include CurrencyLayer for the React Client. 
							// 08/28/2021 Paul.  Include Azure non-table relationships for the React Client. 
							sSQL = "select *                                                                   " + ControlChars.CrLf
							     + "  from vwDETAILVIEWS_RELATIONSHIPS                                         " + ControlChars.CrLf
							     + " where MODULE_NAME in (select MODULE_NAME from vwSYSTEM_REST_TABLES)       " + ControlChars.CrLf
							     + "    or MODULE_NAME in ('Activities', 'Dashboard', 'Home', 'ActivityStream', 'CurrencyLayer')" + ControlChars.CrLf
							     + "    or MODULE_NAME in ('Administration', 'DnsNames', 'ResourceGroups', 'SqlDatabases', 'SqlServers', 'StorageAccounts', 'VirtualMachines')" + ControlChars.CrLf
							     + " order by DETAIL_NAME, RELATIONSHIP_ORDER                                  " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
							
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dt = new DataTable() )
									{
										da.Fill(dt);
										string sLAST_VIEW_NAME = String.Empty;
										List<Dictionary<string, object>> layout = null;
										for ( int i = 0; i < dt.Rows.Count; i++ )
										{
											DataRow row = dt.Rows[i];
											string sDETAIL_NAME = Sql.ToString(row["DETAIL_NAME"]);
											string sMODULE_NAME = Sql.ToString(row["MODULE_NAME"]);
											if ( !lstMODULES.Contains(sMODULE_NAME) )
												continue;
											if ( sLAST_VIEW_NAME != sDETAIL_NAME )
											{
												sLAST_VIEW_NAME = sDETAIL_NAME;
												layout = new List<Dictionary<string, object>>();
												objs.Add(sLAST_VIEW_NAME, layout);
											}
											bool bVisible = (Security.GetUserAccess(sMODULE_NAME, "list") >= 0);
											if ( bVisible )
											{
												Dictionary<string, object> drow = new Dictionary<string, object>();
												for ( int j = 0; j < dt.Columns.Count; j++ )
												{
													if ( dt.Columns[j].ColumnName == "ID" )
														continue;
													// 10/13/2012 Paul.  Must not return value as a string as the client is expecting numerics and booleans in their native format. 
													drow.Add(dt.Columns[j].ColumnName, row[j]);
												}
												layout.Add(drow);
											}
										}
										Cache.Set("vwDETAILVIEWS_RELATIONSHIPS.ReactClient." + sModuleList, objs, DefaultCacheExpiration());
									}
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					throw;
				}
			}
			return objs;
		}

		// 03/02/2019 Paul.  Functions are now static and take modules list input so that they can be used in the Admin API. 
		public Dictionary<string, object> GetAllEditViewsRelationships(List<string> lstMODULES)
		{
			// 08/09/2020 Paul.  Convert to comma separated string. 
			string sModuleList = String.Join(",", lstMODULES.ToArray());
			Dictionary<string, object> objs = Cache.Get("vwEDITVIEWS_RELATIONSHIPS.ReactClient." + sModuleList) as Dictionary<string, object>;
			if ( objs == null )
			{
				objs = new Dictionary<string, object>();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL = String.Empty;
							// 10/09/2012 Paul.  Make sure to filter by modules with REST enabled. 
							// 11/30/2012 Paul.  Activities is not a real table, but it should be included. 
							sSQL = "select *                                                            " + ControlChars.CrLf
							     + "  from vwEDITVIEWS_RELATIONSHIPS                                    " + ControlChars.CrLf
							     + " where MODULE_NAME in (select MODULE_NAME from vwSYSTEM_REST_TABLES)" + ControlChars.CrLf
							     + "    or MODULE_NAME = 'Activities'                                   " + ControlChars.CrLf
							     + " order by EDIT_NAME, RELATIONSHIP_ORDER                             " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
							
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dt = new DataTable() )
									{
										da.Fill(dt);
										string sLAST_VIEW_NAME = String.Empty;
										List<Dictionary<string, object>> layout = null;
										for ( int i = 0; i < dt.Rows.Count; i++ )
										{
											DataRow row = dt.Rows[i];
											string sEDIT_NAME   = Sql.ToString(row["EDIT_NAME"  ]);
											string sMODULE_NAME = Sql.ToString(row["MODULE_NAME"]);
											if ( !lstMODULES.Contains(sMODULE_NAME) )
												continue;
											if ( sLAST_VIEW_NAME != sEDIT_NAME )
											{
												sLAST_VIEW_NAME = sEDIT_NAME;
												layout = new List<Dictionary<string, object>>();
												objs.Add(sLAST_VIEW_NAME, layout);
											}
											bool bVisible = (Security.GetUserAccess(sMODULE_NAME, "list") >= 0);
											if ( bVisible )
											{
												Dictionary<string, object> drow = new Dictionary<string, object>();
												for ( int j = 0; j < dt.Columns.Count; j++ )
												{
													if ( dt.Columns[j].ColumnName == "ID" )
														continue;
													// 10/13/2012 Paul.  Must not return value as a string as the client is expecting numerics and booleans in their native format. 
													drow.Add(dt.Columns[j].ColumnName, row[j]);
												}
												layout.Add(drow);
											}
										}
										Cache.Set("vwEDITVIEWS_RELATIONSHIPS.ReactClient." + sModuleList, objs, DefaultCacheExpiration());
									}
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					throw;
				}
			}
			return objs;
		}

		// 03/02/2019 Paul.  Functions are now static and take modules list input so that they can be used in the Admin API. 
		public Dictionary<string, object> GetAllDynamicButtons(List<string> lstMODULES)
		{
			// 08/09/2020 Paul.  Convert to comma separated string. 
			string sModuleList = String.Join(",", lstMODULES.ToArray());
			Dictionary<string, object> objs = Cache.Get("vwDYNAMIC_BUTTONS.ReactClient." + sModuleList) as Dictionary<string, object>;
			if ( objs == null )
			{
				objs = new Dictionary<string, object>();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						List<string> lstMODULES_WithAdmin = new List<string>(lstMODULES);
						lstMODULES_WithAdmin.Add("Google"         );
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL = String.Empty;
							sSQL = "select *                                         " + ControlChars.CrLf
							     + "  from vwDYNAMIC_BUTTONS                         " + ControlChars.CrLf
							     + " where (DEFAULT_VIEW = 0 or DEFAULT_VIEW is null)" + ControlChars.CrLf
							     + " order by VIEW_NAME, CONTROL_INDEX               " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
							
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dt = new DataTable() )
									{
										da.Fill(dt);
										// 09/28/2021 Paul.  Compute the access rights 
										dt.Columns.Add("MODULE_ACLACCESS", typeof(System.String));
										dt.Columns.Add("TARGET_ACLACCESS", typeof(System.String));
										string sLAST_VIEW_NAME = String.Empty;
										List<Dictionary<string, object>> layout = null;
										for ( int i = 0; i < dt.Rows.Count; i++ )
										{
											DataRow row = dt.Rows[i];
											string sVIEW_NAME          = Sql.ToString (row["VIEW_NAME"         ]);
											string sCONTROL_TYPE       = Sql.ToString (row["CONTROL_TYPE"      ]);
											string sMODULE_NAME        = Sql.ToString (row["MODULE_NAME"       ]);
											string sMODULE_ACCESS_TYPE = Sql.ToString (row["MODULE_ACCESS_TYPE"]);
											string sTARGET_NAME        = Sql.ToString (row["TARGET_NAME"       ]);
											string sTARGET_ACCESS_TYPE = Sql.ToString (row["TARGET_ACCESS_TYPE"]);
											bool   bADMIN_ONLY         = Sql.ToBoolean(row["ADMIN_ONLY"        ]);
											// 10/13/2012 Paul.  Layouts that start with a dot are templates and can be ignored. 
											// 12/19/2019 Paul.  .ArchiveExists is now a global list of buttons, so we cannot ignore non-module lists. 
											//if ( sVIEW_NAME.StartsWith(".") )
											//	continue;
											if ( !Sql.IsEmptyString(sMODULE_NAME) && !lstMODULES_WithAdmin.Contains(sMODULE_NAME) )
												continue;
											if ( !Sql.IsEmptyString(sTARGET_NAME) && !lstMODULES_WithAdmin.Contains(sTARGET_NAME) )
												continue;
											if ( sLAST_VIEW_NAME != sVIEW_NAME )
											{
												sLAST_VIEW_NAME = sVIEW_NAME;
												layout = new List<Dictionary<string, object>>();
												objs.Add(sLAST_VIEW_NAME, layout);
											}
											// 09/28/2021 Paul.  Compute the access rights 
											row["MODULE_ACLACCESS"] = "0";
											row["TARGET_ACLACCESS"] = "0";
											bool bVisible = (bADMIN_ONLY && Security.IS_ADMIN || !bADMIN_ONLY);
											if ( String.Compare(sCONTROL_TYPE, "Button", true) == 0 || String.Compare(sCONTROL_TYPE, "HyperLink", true) == 0 || String.Compare(sCONTROL_TYPE, "ButtonLink", true) == 0 )
											{
												if ( bVisible && !Sql.IsEmptyString(sMODULE_NAME) && !Sql.IsEmptyString(sMODULE_ACCESS_TYPE) )
												{
													int nACLACCESS = Security.GetUserAccess(sMODULE_NAME, sMODULE_ACCESS_TYPE);
													// 09/28/2021 Paul.  Compute the access rights 
													row["MODULE_ACLACCESS"] = nACLACCESS.ToString();
													// 09/03/2011 Paul.  Can't apply Owner rights without the item record. 
													//bVisible = (nACLACCESS > ACL_ACCESS.OWNER) || (nACLACCESS == ACL_ACCESS.OWNER && ((Security.USER_ID == gASSIGNED_USER_ID) || (!bIsPostBack && rdr == null) || (rdr != null && bShowUnassigned && Sql.IsEmptyGuid(gASSIGNED_USER_ID))));
													if ( bVisible && !Sql.IsEmptyString(sTARGET_NAME) && !Sql.IsEmptyString(sTARGET_ACCESS_TYPE) )
													{
														nACLACCESS = Security.GetUserAccess(sTARGET_NAME, sTARGET_ACCESS_TYPE);
														// 09/28/2021 Paul.  Compute the access rights 
														row["TARGET_ACLACCESS"] = nACLACCESS.ToString();
														// 09/03/2011 Paul.  Can't apply Owner rights without the item record. 
														//bVisible = (nACLACCESS > ACL_ACCESS.OWNER) || (nACLACCESS == ACL_ACCESS.OWNER && ((Security.USER_ID == gASSIGNED_USER_ID) || (!bIsPostBack && rdr == null) || (rdr != null && bShowUnassigned && Sql.IsEmptyGuid(gASSIGNED_USER_ID))));
													}
												}
											}
											if ( bVisible )
											{
												Dictionary<string, object> drow = new Dictionary<string, object>();
												for ( int j = 0; j < dt.Columns.Count; j++ )
												{
													if ( dt.Columns[j].ColumnName == "ID" )
														continue;
													// 10/13/2012 Paul.  Must not return value as a string as the client is expecting numerics and booleans in their native format. 
													drow.Add(dt.Columns[j].ColumnName, row[j]);
												}
												layout.Add(drow);
											}
										}
										Cache.Set("vwDYNAMIC_BUTTONS.ReactClient." + sModuleList, objs, DefaultCacheExpiration());
									}
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					throw;
				}
			}
			return objs;
		}

		// 08/15/2019 Paul.  Add support for menu shortcuts. 
		public Dictionary<string, object> GetAllShortcuts(List<string> lstMODULES)
		{
			List<string> lstMODULES_WithAdmin = new List<string>(lstMODULES);
			lstMODULES_WithAdmin.Add("Users"      );
			lstMODULES_WithAdmin.Add("Audit"      );
			lstMODULES_WithAdmin.Add("Google"     );
			lstMODULES_WithAdmin.Add("LinkedIn"   );
			lstMODULES_WithAdmin.Add("Twitter"    );
			lstMODULES_WithAdmin.Add("Salesforce" );
			lstMODULES_WithAdmin.Add("SavedSearch");
			
			string sModuleList = lstMODULES_WithAdmin.ToString();
			Dictionary<string, object> objs = Cache.Get("vwSHORTCUTS_Menu_ByUser.ReactClient." + sModuleList) as Dictionary<string, object>;
			if ( objs == null )
			{
				objs = new Dictionary<string, object>();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL = String.Empty;
							sSQL = "select *                                         " + ControlChars.CrLf
							     + "  from vwSHORTCUTS_Menu_ByUser                   " + ControlChars.CrLf
							     + " where (USER_ID = @USER_ID or USER_ID is null)   " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@USER_ID", Security.USER_ID);
								Sql.AppendParameter(cmd, lstMODULES_WithAdmin.ToArray(), "MODULE_NAME", false);
								cmd.CommandText += " order by MODULE_NAME, SHORTCUT_ORDER" + ControlChars.CrLf;
							
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dt = new DataTable() )
									{
										da.Fill(dt);
										string sLAST_MODULE_NAME = String.Empty;
										List<Dictionary<string, object>> layout = null;
										for ( int i = 0; i < dt.Rows.Count; i++ )
										{
											DataRow row = dt.Rows[i];
											string sMODULE_NAME      = Sql.ToString (row["MODULE_NAME"     ]);
											string sSHORTCUT_ACLTYPE = Sql.ToString (row["SHORTCUT_ACLTYPE"]);
											if ( sLAST_MODULE_NAME != sMODULE_NAME )
											{
												sLAST_MODULE_NAME = sMODULE_NAME;
												layout = new List<Dictionary<string, object>>();
												objs.Add(sLAST_MODULE_NAME, layout);
											}
											int nACLACCESS = Security.GetUserAccess(sMODULE_NAME, sSHORTCUT_ACLTYPE);
											if ( Security.IS_ADMIN || nACLACCESS >= 0 )
											{
												Dictionary<string, object> drow = new Dictionary<string, object>();
												for ( int j = 0; j < dt.Columns.Count; j++ )
												{
													drow.Add(dt.Columns[j].ColumnName, row[j]);
												}
												layout.Add(drow);
											}
										}
										Cache.Set("vwSHORTCUTS_Menu_ByUser.ReactClient." + sModuleList, objs, DefaultCacheExpiration());
									}
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					throw;
				}
			}
			return objs;
		}


		// 03/02/2019 Paul.  Functions are now static and take modules list input so that they can be used in the Admin API. 
		// 03/26/2019 Paul.  Admin has more custom lists. 
		public Dictionary<string, object> GetAllTerminology(List<string> lstMODULES, bool bAdmin)
		{
			// 09/27/2020 Paul.  Terminology is language specific. 
			Dictionary<string, object> objs = Cache.Get("vwTERMINOLOGY.ReactClient." + L10n.NAME + (bAdmin ? ".Admin": "")) as Dictionary<string, object>;
			if ( objs == null )
			{
				objs = new Dictionary<string, object>();
				// 05/19/2018 Paul.  Capture the last command for error tracking. 
				string sLastCommand = String.Empty;
				try
				{
					if ( Security.IsAuthenticated() )
					{
						List<string> lstMODULES_WithAdmin = new List<string>(lstMODULES);
						// 06/07/2020 Paul.  Home is always available. 
						lstMODULES_WithAdmin.Add("Home"           );
						// 10/05/2022 Paul.  Merge is always available. 
						lstMODULES_WithAdmin.Add("Merge"          );
						lstMODULES_WithAdmin.Add("Users"          );
						lstMODULES_WithAdmin.Add("Audit"          );
						lstMODULES_WithAdmin.Add("OAuth"          );
						// 12/10/2019 Paul.  SavedSearch is always available. 
						lstMODULES_WithAdmin.Add("SavedSearch"    );
						// 05/20/2020 Paul.  Rules Wizard is always available. 
						lstMODULES_WithAdmin.Add("Rules"          );
						// 05/29/2020 Paul.  ACLActions is used on the User Profile page. 
						lstMODULES_WithAdmin.Add("ACLActions"     );
						// 10/27/2021 Paul.  Add Configurator for AdminWizard. 
						lstMODULES_WithAdmin.Add("Configurator"   );
						if ( bAdmin )
						{
							lstMODULES_WithAdmin.Add("Google"         );
							lstMODULES_WithAdmin.Add("LinkedIn"       );
							lstMODULES_WithAdmin.Add("Twitter"        );
							lstMODULES_WithAdmin.Add("Salesforce"     );
							lstMODULES_WithAdmin.Add("SavedSearch"    );
							lstMODULES_WithAdmin.Add("PasswordManager");
							lstMODULES_WithAdmin.Add("Updater"        );
						}
					
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL = String.Empty;
							sSQL = "select NAME               " + ControlChars.CrLf
							     + "     , MODULE_NAME        " + ControlChars.CrLf
							     + "     , LIST_NAME          " + ControlChars.CrLf
							     + "     , DISPLAY_NAME       " + ControlChars.CrLf
							     + "  from vwTERMINOLOGY      " + ControlChars.CrLf
							     + " where lower(LANG) = @LANG" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@LANG", L10n.NAME.ToLower());
								cmd.CommandText += "   and ( 1 = 0" + ControlChars.CrLf;
								cmd.CommandText += "         or MODULE_NAME is null" + ControlChars.CrLf;
								cmd.CommandText += "     ";
								Sql.AppendParameter(cmd, lstMODULES_WithAdmin.ToArray(), "MODULE_NAME", true);
								cmd.CommandText += "       )" + ControlChars.CrLf;
								cmd.CommandText += " order by MODULE_NAME, LIST_NAME, NAME" + ControlChars.CrLf;
							
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dt = new DataTable() )
									{
										// 05/19/2018 Paul.  Capture the last command for error tracking. 
										sLastCommand = Sql.ExpandParameters(cmd);
										da.Fill(dt);
										string sLAST_MODULE_NAME = String.Empty;
										objs.Add(L10n.NAME + ".Loaded", true);
										for ( int i = 0; i < dt.Rows.Count; i++ )
										{
											DataRow row = dt.Rows[i];
											string sNAME         = Sql.ToString(row["NAME"        ]);
											string sMODULE_NAME  = Sql.ToString(row["MODULE_NAME" ]);
											string sLIST_NAME    = Sql.ToString(row["LIST_NAME"   ]);
											string sDISPLAY_NAME = Sql.ToString(row["DISPLAY_NAME"]);
											// 02/02/2013 Paul.  The HTML5 Offline Client and Browser Extensions crash because of an exception here. 
											// {"ExceptionDetail":{"HelpLink":null,"InnerException":null,"Message":"An item with the same key has already been added."}}
											// 02/02/2013 Paul.  Custom fields can have a table name instead of a module name. 
											if ( !Sql.IsEmptyString(sMODULE_NAME) && sMODULE_NAME == sMODULE_NAME.ToUpper() )
											{
												string sTABLE_NAME = sMODULE_NAME;
												if ( !Sql.IsEmptyString(Application["Modules." + sTABLE_NAME + ".ModuleName"]) )
													sMODULE_NAME = Sql.ToString(Application["Modules." + sTABLE_NAME + ".ModuleName"]);
											}
											if ( sLAST_MODULE_NAME != sMODULE_NAME )
											{
												sLAST_MODULE_NAME = sMODULE_NAME;
												// 03/19/2016 Paul.  Bad data with incorrect case on module name can lead to an exception. 
												if ( objs.ContainsKey(L10n.NAME + "." + sMODULE_NAME + ".Loaded") )
													objs.Add(L10n.NAME + "." + sMODULE_NAME + ".Loaded", true);
											}
											if ( !Sql.IsEmptyString(sLIST_NAME) )
												objs[L10n.NAME + "." + "." + sLIST_NAME + "." + sNAME] = sDISPLAY_NAME;
											else
												objs[L10n.NAME + "." + sMODULE_NAME + "." + sNAME] = sDISPLAY_NAME;
										}
									}
									// 10/12/2012 Paul.  Since we are replacing the entire Terminology List object, we need to include custom lists. 
									using ( DataTable dt = this.Currencies() )
									{
										string sLIST_NAME = "Currencies";
										for ( int i = 0; i < dt.Rows.Count; i++ )
										{
											DataRow row = dt.Rows[i];
											string sID           = Sql.ToString(row["ID"         ]);
											string sDISPLAY_NAME = Sql.ToString(row["NAME_SYMBOL"]);
											objs[L10n.NAME + "." + "." + sLIST_NAME + "." + sID] = sDISPLAY_NAME;
										}
									}
									// 07/02/2020 Paul.  User Profile needs, TimeZones, Themes, Languages, Date/Time formats. 
									using ( DataTable dt = this.Timezones() )
									{
										string sLIST_NAME = "TimeZones";
										for ( int i = 0; i < dt.Rows.Count; i++ )
										{
											DataRow row = dt.Rows[i];
											string sID           = Sql.ToString(row["ID"  ]);
											string sDISPLAY_NAME = Sql.ToString(row["NAME"]);
											objs[L10n.NAME + "." + "." + sLIST_NAME + "." + sID] = sDISPLAY_NAME;
										}
									}
									using ( DataTable dt = this.Themes() )
									{
										string sLIST_NAME = "Themes";
										for ( int i = 0; i < dt.Rows.Count; i++ )
										{
											DataRow row = dt.Rows[i];
											string sID           = Sql.ToString(row["NAME"]);
											string sDISPLAY_NAME = Sql.ToString(row["NAME"]);
											objs[L10n.NAME + "." + "." + sLIST_NAME + "." + sID] = sDISPLAY_NAME;
										}
									}
									using ( DataTable dt = this.Release() )
									{
										string sLIST_NAME = "Release";
										for ( int i = 0; i < dt.Rows.Count; i++ )
										{
											DataRow row = dt.Rows[i];
											string sID           = Sql.ToString(row["ID"  ]);
											string sDISPLAY_NAME = Sql.ToString(row["NAME"]);
											objs[L10n.NAME + "." + "." + sLIST_NAME + "." + sID] = sDISPLAY_NAME;
										}
									}
									// 12/18/2017 Paul.  Order modules will not exist on Community Edition. 
									if ( Sql.ToBoolean(Application["Modules.ContractTypes.RestEnabled"]) )
									{
										using ( DataTable dt = this.ContractTypes() )
										{
											string sLIST_NAME = "ContractTypes";
											for ( int i = 0; i < dt.Rows.Count; i++ )
											{
												DataRow row = dt.Rows[i];
												string sID           = Sql.ToString(row["ID"  ]);
												string sDISPLAY_NAME = Sql.ToString(row["NAME"]);
												objs[L10n.NAME + "." + "." + sLIST_NAME + "." + sID] = sDISPLAY_NAME;
											}
										}
									}
									using ( DataTable dt = this.AssignedUser() )
									{
										string sLIST_NAME = "AssignedUser";
										for ( int i = 0; i < dt.Rows.Count; i++ )
										{
											DataRow row = dt.Rows[i];
											string sID           = Sql.ToString(row["ID"       ]);
											string sDISPLAY_NAME = Sql.ToString(row["USER_NAME"]);
											objs[L10n.NAME + "." + "." + sLIST_NAME + "." + sID] = sDISPLAY_NAME;
										}
									}
									// 06/13/2017 Paul.  HTML5 Dashboard needs the teams list. 
									if ( Config.enable_team_management() )
									{
										using ( DataTable dt = this.Teams() )
										{
											string sLIST_NAME = "Teams";
											for ( int i = 0; i < dt.Rows.Count; i++ )
											{
												DataRow row = dt.Rows[i];
												string sID           = Sql.ToString(row["ID"  ]);
												string sDISPLAY_NAME = Sql.ToString(row["NAME"]);
												objs[L10n.NAME + "." + "." + sLIST_NAME + "." + sID] = sDISPLAY_NAME;
											}
										}
									}
									// 09/02/2020 Paul.  We were getting arabic on the react client when using CurrentCulture.  
									CultureInfo culture = CultureInfo.CreateSpecificCulture(L10n.NAME);
									// 02/24/2013 Paul.  Build Calendar names from culture instead of from terminology. 
									objs[L10n.NAME + ".Calendar.YearMonthPattern"] = culture.DateTimeFormat.YearMonthPattern;
									objs[L10n.NAME + ".Calendar.MonthDayPattern" ] = culture.DateTimeFormat.MonthDayPattern;
									objs[L10n.NAME + ".Calendar.LongDatePattern" ] = culture.DateTimeFormat.LongDatePattern;
									objs[L10n.NAME + ".Calendar.ShortTimePattern"] = Sql.ToString(Session["USER_SETTINGS/TIMEFORMAT"]);
									objs[L10n.NAME + ".Calendar.ShortDatePattern"] = Sql.ToString(Session["USER_SETTINGS/DATEFORMAT"]);
									objs[L10n.NAME + ".Calendar.FirstDayOfWeek"  ] = ((int) culture.DateTimeFormat.FirstDayOfWeek).ToString();
									for ( int i = 1; i <= 12; i++ )
									{
										string sID           = i.ToString();
										string sDISPLAY_NAME = culture.DateTimeFormat.MonthNames[i- 1];
										objs[L10n.NAME + "." + ".month_names_dom." + sID] = sDISPLAY_NAME;
									}
									for ( int i = 1; i <= 12; i++ )
									{
										string sID           = i.ToString();
										string sDISPLAY_NAME = culture.DateTimeFormat.AbbreviatedMonthNames[i- 1];
										objs[L10n.NAME + "." + ".short_month_names_dom." + sID] = sDISPLAY_NAME;
									}
									for ( int i = 0; i <= 6; i++ )
									{
										string sID           = i.ToString();
										string sDISPLAY_NAME = culture.DateTimeFormat.DayNames[i];
										objs[L10n.NAME + "." + ".day_names_dom." + sID] = sDISPLAY_NAME;
									}
									for ( int i = 0; i <= 6; i++ )
									{
										string sID           = i.ToString();
										string sDISPLAY_NAME = culture.DateTimeFormat.AbbreviatedDayNames[i];
										objs[L10n.NAME + "." + ".short_day_names_dom." + sID] = sDISPLAY_NAME;
									}
									// 07/03/2020 Paul.  Languages is global, not just admin. 
									using ( DataTable dt = this.Languages() )
									{
										Guid   gTIMEZONE = Sql.ToGuid  (Session["USER_SETTINGS/TIMEZONE"]);
										TimeZone T10n = TimeZone.CreateTimeZone(gTIMEZONE);
											
										string sLIST_NAME = "Languages";
										for ( int i = 0; i < dt.Rows.Count; i++ )
										{
											DataRow row = dt.Rows[i];
											string sNAME         = Sql.ToString(row["NAME"       ]);
											string sDISPLAY_NAME = Sql.ToString(row["NATIVE_NAME"]);
											objs[L10n.NAME + "." + "." + sLIST_NAME + "." + sNAME] = sDISPLAY_NAME;
											// 07/03/2020 Paul.  Include all date patterns for all supported langauges. 
											try
											{
												CultureInfo oldCulture   = Thread.CurrentThread.CurrentCulture   ;
												CultureInfo oldUICulture = Thread.CurrentThread.CurrentUICulture ;
												Thread.CurrentThread.CurrentCulture   = CultureInfo.CreateSpecificCulture(sNAME);
												Thread.CurrentThread.CurrentUICulture = new CultureInfo(sNAME);

												DateTime dtNow = T10n.FromServerTime(DateTime.Now);
												DateTimeFormatInfo oDateInfo   = Thread.CurrentThread.CurrentCulture.DateTimeFormat;
												NumberFormatInfo   oNumberInfo = Thread.CurrentThread.CurrentCulture.NumberFormat  ;

												String[] aDateTimePatterns = oDateInfo.GetAllDateTimePatterns();
												foreach ( string sPattern in aDateTimePatterns )
												{
													// 11/12/2005 Paul.  Only allow patterns that have a full year. 
													// 10/15/2013 Paul.  Allow 2-digit year. 
													if ( sPattern.IndexOf("yy") >= 0 && sPattern.IndexOf("dd") >= 0 && sPattern.IndexOf("mm") <  0 )
													{
														sDISPLAY_NAME  = sPattern + "   " + dtNow.ToString(sPattern);
														objs[L10n.NAME + "." + "." + "DateFormat." + sNAME + "." + sPattern] = sDISPLAY_NAME;
													}
													if ( sPattern.IndexOf("yy") <  0 && sPattern.IndexOf("mm") >= 0 )
													{
														sDISPLAY_NAME = sPattern + "   " + dtNow.ToString(sPattern);
														objs[L10n.NAME + "." + "." + "TimeFormat." + sNAME + "." + sPattern] = sDISPLAY_NAME;
													}
												}
												Thread.CurrentThread.CurrentCulture = oldCulture  ;
												Thread.CurrentThread.CurrentCulture = oldUICulture;
											}
											catch(Exception ex)
											{
												SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
											}
										}
									}
									// 02/28/2016 Paul.  Order management in html5. 
									// 12/18/2017 Paul.  Order modules will not exist on Community Edition. 
									if ( Sql.ToBoolean(Application["Modules.TaxRates.RestEnabled"]) )
									{
										using ( DataTable dt = this.TaxRates() )
										{
											string sLIST_NAME = "TaxRates";
											for ( int i = 0; i < dt.Rows.Count; i++ )
											{
												DataRow row = dt.Rows[i];
												string sID           = Sql.ToString(row["ID"  ]);
												string sDISPLAY_NAME = Sql.ToString(row["NAME"]);
												objs[L10n.NAME + "." + "." + sLIST_NAME + "." + sID] = sDISPLAY_NAME;
											}
										}
									}
									// 12/18/2017 Paul.  Order modules will not exist on Community Edition. 
									if ( Sql.ToBoolean(Application["Modules.Shippers.RestEnabled"]) )
									{
										using ( DataTable dt = this.Shippers() )
										{
											string sLIST_NAME = "Shippers";
											for ( int i = 0; i < dt.Rows.Count; i++ )
											{
												DataRow row = dt.Rows[i];
												string sID           = Sql.ToString(row["ID"  ]);
												string sDISPLAY_NAME = Sql.ToString(row["NAME"]);
												objs[L10n.NAME + "." + "." + sLIST_NAME + "." + sID] = sDISPLAY_NAME;
											}
										}
									}
									// 12/18/2017 Paul.  Order modules will not exist on Community Edition. 
									if ( Sql.ToBoolean(Application["Modules.Discounts.RestEnabled"]) )
									{
										using ( DataTable dt = this.Discounts() )
										{
											DataView vwDiscounts = new DataView(dt);
											// 02/29/2016 Paul.  The Line Items Editor uses a reduced set of discounts. 
											vwDiscounts.RowFilter = "PRICING_FORMULA in ('PercentageDiscount', 'FixedDiscount')";
											string sLIST_NAME = "Discounts";
											for ( int i = 0; i < vwDiscounts.Count; i++ )
											{
												DataRowView row = vwDiscounts[i];
												string sID           = Sql.ToString(row["ID"  ]);
												string sDISPLAY_NAME = Sql.ToString(row["NAME"]);
												objs[L10n.NAME + "." + "." + sLIST_NAME + "." + sID] = sDISPLAY_NAME;
											}
										}
									}
									// 12/18/2017 Paul.  Order modules will not exist on Community Edition. 
									if ( Sql.ToBoolean(Application["Modules.Quotes.RestEnabled"]) || Sql.ToBoolean(Application["Modules.Orders.RestEnabled"]) || Sql.ToBoolean(Application["Modules.Invoices.RestEnabled"]) )
									{
										using ( DataTable dt = this.List("pricing_formula_dom") )
										{
											DataView vwPricingFormulas = new DataView(dt);
											// 02/29/2016 Paul.  The Line Items Editor uses a reduced set of pricing formulas. 
											vwPricingFormulas.RowFilter = "NAME in ('PercentageDiscount', 'FixedDiscount')";
											string sLIST_NAME = "pricing_formula_line_items";
											for ( int i = 0; i < vwPricingFormulas.Count; i++ )
											{
												DataRowView row = vwPricingFormulas[i];
												string sNAME         = Sql.ToString(row["NAME"        ]);
												string sDISPLAY_NAME = Sql.ToString(row["DISPLAY_NAME"]);
												objs[L10n.NAME + "." + "." + sLIST_NAME + "." + sNAME] = sDISPLAY_NAME;
											}
										}
										using ( DataTable dt = this.PaymentTerms() )
										{
											string sLIST_NAME = "PaymentTerms";
											for ( int i = 0; i < dt.Rows.Count; i++ )
											{
												DataRow row = dt.Rows[i];
												string sID           = Sql.ToString(row["ID"  ]);
												string sDISPLAY_NAME = Sql.ToString(row["NAME"]);
												objs[L10n.NAME + "." + "." + sLIST_NAME + "." + sID] = sDISPLAY_NAME;
											}
										}
									}
									// 08/19/2019 Paul.  Modules are used by the ReportDesigner, so it is a non-admin list. 
									using ( DataTable dt = this.Modules() )
									{
										string sLIST_NAME = "Modules";
										for ( int i = 0; i < dt.Rows.Count; i++ )
										{
											DataRow row = dt.Rows[i];
											string sNAME         = Sql.ToString(row["MODULE_NAME" ]);
											string sDISPLAY_NAME = Sql.ToString(row["DISPLAY_NAME"]);
											objs[L10n.NAME + "." + "." + sLIST_NAME + "." + sNAME] = sDISPLAY_NAME;
										}
									}
									// 04/15/2021 Paul.  RulesModules is used by Business Rules module. 
									using ( DataTable dt = this.RulesModules() )
									{
										string sLIST_NAME = "RulesModules";
										for ( int i = 0; i < dt.Rows.Count; i++ )
										{
											DataRow row = dt.Rows[i];
											string sMODULE_NAME  = Sql.ToString(row["MODULE_NAME" ]);
											string sDISPLAY_NAME = Sql.ToString(row["DISPLAY_NAME"]);
											// 05/07/2021 Paul.  Prefilter the list to only those modules that can be edited. 
											if ( Security.GetUserAccess(sMODULE_NAME, "edit") >= 0 )
												objs[L10n.NAME + "." + "." + sLIST_NAME + "." + sMODULE_NAME] = sDISPLAY_NAME;
										}
										// 03/29/2012 Paul.  Add Rules Wizard support to Terminology module. 
										if ( Security.IS_ADMIN )
										{
											objs[L10n.NAME + "." + "." + sLIST_NAME + "." + "Terminology"] = L10n.Term(".moduleList.Terminology");
										}
									}
									// 05/01/2020 Paul.  Provide Email Templates to the React Client. 
									using ( DataTable dt = this.EmailTemplates() )
									{
										string sLIST_NAME = "EmailTemplates";
										for ( int i = 0; i < dt.Rows.Count; i++ )
										{
											DataRow row = dt.Rows[i];
											string sID           = Sql.ToString(row["ID"  ]);
											string sDISPLAY_NAME = Sql.ToString(row["NAME"]);
											objs[L10n.NAME + "." + "." + sLIST_NAME + "." + sID] = sDISPLAY_NAME;
										}
									}
									// 05/01/2020 Paul.  Provide User Signatures to the React Client. 
									using ( DataTable dt = this.UserSignatures() )
									{
										string sLIST_NAME = "UserSignatures";
										for ( int i = 0; i < dt.Rows.Count; i++ )
										{
											DataRow row = dt.Rows[i];
											string sID           = Sql.ToString(row["ID"  ]);
											string sDISPLAY_NAME = Sql.ToString(row["NAME"]);
											objs[L10n.NAME + "." + "." + sLIST_NAME + "." + sID] = sDISPLAY_NAME;
										}
									}
									// 05/01/2020 Paul.  Provide User Mailboxes to the React Client. 
									// 01/24/2021 Paul.  Use OutboundMail DISPLAY_NAME. 
									using ( DataTable dt = this.OutboundMail() )
									{
										string sLIST_NAME = "OutboundMail";
										for ( int i = 0; i < dt.Rows.Count; i++ )
										{
											DataRow row = dt.Rows[i];
											string sID           = Sql.ToString(row["ID"  ]);
											string sDISPLAY_NAME = Sql.ToString(row["DISPLAY_NAME"]);
											objs[L10n.NAME + "." + "." + sLIST_NAME + "." + sID] = sDISPLAY_NAME;
										}
									}
									// 04/18/2022 Paul.  EmailMarketing requires InboundEmailBounce list. 
									using ( DataTable dt = this.InboundEmailBounce() )
									{
										string sLIST_NAME = "InboundEmailBounce";
										for ( int i = 0; i < dt.Rows.Count; i++ )
										{
											DataRow row = dt.Rows[i];
											string sID           = Sql.ToString(row["ID"  ]);
											string sDISPLAY_NAME = Sql.ToString(row["NAME"]);
											objs[L10n.NAME + "." + "." + sLIST_NAME + "." + sID] = sDISPLAY_NAME;
										}
									}
									// 01/24/2021 Paul.  Add OutboundSms. 
									using ( DataTable dt = this.OutboundSms() )
									{
										string sLIST_NAME = "OutboundSms";
										for ( int i = 0; i < dt.Rows.Count; i++ )
										{
											DataRow row = dt.Rows[i];
											string sID           = Sql.ToString(row["ID"  ]);
											string sDISPLAY_NAME = Sql.ToString(row["DISPLAY_NAME"]);
											objs[L10n.NAME + "." + "." + sLIST_NAME + "." + sID] = sDISPLAY_NAME;
										}
									}
									// 12/17/2020 Paul.  Provide ReportingModules to the React Client. 
									using ( DataTable dt = this.ReportingModules() )
									{
										string sLIST_NAME = "ReportingModules";
										for ( int i = 0; i < dt.Rows.Count; i++ )
										{
											DataRow row = dt.Rows[i];
											string sID           = Sql.ToString(row["MODULE_NAME" ]);
											string sDISPLAY_NAME = Sql.ToString(row["DISPLAY_NAME"]);
											objs[L10n.NAME + "." + "." + sLIST_NAME + "." + sID] = sDISPLAY_NAME;
										}
									}
									// 03/26/2019 Paul.  Admin has more custom lists. 
									if ( bAdmin )
									{
										// 06/29/2020 Paul.  Order modules will not exist on Community Edition. 
										if ( Sql.ToBoolean(Application["Modules.Manufacturers.RestEnabled"]) || Sql.ToBoolean(Application["Modules.ProductTemplates.RestEnabled"]) )
										{
											using ( DataTable dt = this.Manufacturers() )
											{
												string sLIST_NAME = "Manufacturers";
												for ( int i = 0; i < dt.Rows.Count; i++ )
												{
													DataRow row = dt.Rows[i];
													string sNAME         = Sql.ToString(row["ID"  ]);
													string sDISPLAY_NAME = Sql.ToString(row["NAME"]);
													objs[L10n.NAME + "." + "." + sLIST_NAME + "." + sNAME] = sDISPLAY_NAME;
												}
											}
										}
										// 06/29/2020 Paul.  Order modules will not exist on Community Edition. 
										if ( Sql.ToBoolean(Application["Modules.ProductTypes.RestEnabled"]) || Sql.ToBoolean(Application["Modules.ProductTemplates.RestEnabled"]) )
										{
											using ( DataTable dt = this.ProductTypes() )
											{
												string sLIST_NAME = "ProductTypes";
												for ( int i = 0; i < dt.Rows.Count; i++ )
												{
													DataRow row = dt.Rows[i];
													string sNAME         = Sql.ToString(row["ID"  ]);
													string sDISPLAY_NAME = Sql.ToString(row["NAME"]);
													objs[L10n.NAME + "." + "." + sLIST_NAME + "." + sNAME] = sDISPLAY_NAME;
												}
											}
										}
										// 06/29/2020 Paul.  Order modules will not exist on Community Edition. 
										if ( Sql.ToBoolean(Application["Modules.ProductCategories.RestEnabled"]) || Sql.ToBoolean(Application["Modules.ProductTemplates.RestEnabled"]) )
										{
											using ( DataTable dt = this.ProductCategories() )
											{
												string sLIST_NAME = "ProductCategories";
												for ( int i = 0; i < dt.Rows.Count; i++ )
												{
													DataRow row = dt.Rows[i];
													string sNAME         = Sql.ToString(row["ID"  ]);
													string sDISPLAY_NAME = Sql.ToString(row["NAME"]);
													objs[L10n.NAME + "." + "." + sLIST_NAME + "." + sNAME] = sDISPLAY_NAME;
												}
											}
										}
										using ( DataTable dt = this.SchedulerJobs() )
										{
											string sLIST_NAME = "SchedulerJobs";
											for ( int i = 0; i < dt.Rows.Count; i++ )
											{
												DataRow row = dt.Rows[i];
												string sNAME         = Sql.ToString(row["NAME"        ]);
												string sDISPLAY_NAME = Sql.ToString(row["DISPLAY_NAME"]);
												objs[L10n.NAME + "." + "." + sLIST_NAME + "." + sNAME] = sDISPLAY_NAME;
											}
										}
										using ( DataTable dt = this.TerminologyPickLists() )
										{
											string sLIST_NAME = "TerminologyPickLists";
											for ( int i = 0; i < dt.Rows.Count; i++ )
											{
												DataRow row = dt.Rows[i];
												string sNAME         = Sql.ToString(row["LIST_NAME"]);
												string sDISPLAY_NAME = Sql.ToString(row["LIST_NAME"]);
												objs[L10n.NAME + "." + "." + sLIST_NAME + "." + sNAME] = sDISPLAY_NAME;
											}
										}
										using ( DataTable dt = this.DynamicButtonViews() )
										{
											string sLIST_NAME = "DynamicButtonViews";
											for ( int i = 0; i < dt.Rows.Count; i++ )
											{
												DataRow row = dt.Rows[i];
												string sNAME         = Sql.ToString(row["VIEW_NAME"]);
												string sDISPLAY_NAME = Sql.ToString(row["VIEW_NAME"]);
												objs[L10n.NAME + "." + "." + sLIST_NAME + "." + sNAME] = sDISPLAY_NAME;
											}
										}
										using ( DataTable dt = this.LibraryPaymentGateways() )
										{
											string sLIST_NAME = "LibraryPaymentGateways";
											for ( int i = 0; i < dt.Rows.Count; i++ )
											{
												DataRow row = dt.Rows[i];
												string sNAME         = Sql.ToString(row["NAME"        ]);
												string sDISPLAY_NAME = Sql.ToString(row["DISPLAY_NAME"]);
												objs[L10n.NAME + "." + "." + sLIST_NAME + "." + sNAME] = sDISPLAY_NAME;
											}
										}
										// 03/03/2020 Paul.  Include lists used by Layout Manager. 
										using ( DataTable dtModules = this.ModulesPopups() )
										{
											DataView vwModules = new DataView(dtModules);
											vwModules.RowFilter = "HAS_POPUP = 1";
											string sLIST_NAME = "ModuleTypes";
											foreach ( DataRowView row in vwModules )
											{
												string sNAME         = Sql.ToString(row["MODULE_NAME"]);
												string sDISPLAY_NAME = Sql.ToString(row["MODULE_NAME"]);
												objs[L10n.NAME + "." + "." + sLIST_NAME + "." + sNAME] = sDISPLAY_NAME;
											}
										}
										List<string> FIELD_VALIDATORS = new List<string>();
										sSQL = "select ID                " + ControlChars.CrLf
										     + "     , NAME              " + ControlChars.CrLf
										     + "  from vwFIELD_VALIDATORS" + ControlChars.CrLf
										     + " order by NAME           " + ControlChars.CrLf;
										using ( IDbCommand cmdFIELD_VALIDATORS = con.CreateCommand() )
										{
											cmdFIELD_VALIDATORS.CommandText = sSQL;
											using ( DbDataAdapter daFIELD_VALIDATORS = dbf.CreateDataAdapter() )
											{
												((IDbDataAdapter)daFIELD_VALIDATORS).SelectCommand = cmdFIELD_VALIDATORS;
												using ( DataTable dt = new DataTable() )
												{
													daFIELD_VALIDATORS.Fill(dt);
													string sLIST_NAME = "FieldValidators";
													foreach ( DataRow row in dt.Rows )
													{
														string sNAME         = Sql.ToString(row["ID"  ]);
														string sDISPLAY_NAME = Sql.ToString(row["NAME"]);
														objs[L10n.NAME + "." + "." + sLIST_NAME + "." + sNAME] = sDISPLAY_NAME;
													}
												}
											}
										}
										// 01/29/2021 Paul.  Add EditCustomFields to React client. 
										using ( DataTable dtModules = this.CustomEditModules() )
										{
											List<string> MODULE_TYPES = new List<string>();
											string sLIST_NAME = "CustomEditModules";
											foreach ( DataRow row in dtModules.Rows )
											{
												string sNAME         = Sql.ToString(row["NAME"]);
												string sDISPLAY_NAME = L10n.Term(".moduleList." + sNAME);
												objs[L10n.NAME + "." + "." + sLIST_NAME + "." + sNAME] = sDISPLAY_NAME;
											}
										}
										// 06/02/2021 Paul.  Provide WorkflowModules to the React Client. 
										using ( DataTable dt = this.WorkflowModules() )
										{
											string sLIST_NAME = "WorkflowModules";
											for ( int i = 0; i < dt.Rows.Count; i++ )
											{
												DataRow row = dt.Rows[i];
												string sID           = Sql.ToString(row["MODULE_NAME" ]);
												string sDISPLAY_NAME = Sql.ToString(row["DISPLAY_NAME"]);
												objs[L10n.NAME + "." + "." + sLIST_NAME + "." + sID] = sDISPLAY_NAME;
											}
										}
										// 09/13/2021 Paul.  Provide AuditedModules to the React Client. 
										using ( DataTable dt = this.AuditedModules() )
										{
											string sLIST_NAME = "AuditedModules";
											for ( int i = 0; i < dt.Rows.Count; i++ )
											{
												DataRow row = dt.Rows[i];
												string sID           = Sql.ToString(row["MODULE_NAME" ]);
												string sDISPLAY_NAME = Sql.ToString(row["DISPLAY_NAME"]);
												objs[L10n.NAME + "." + "." + sLIST_NAME + "." + sID] = sDISPLAY_NAME;
											}
										}
										// 09/13/2021 Paul.  Provide ActiveUsers to the React Client. 
										using ( DataTable dt = this.ActiveUsers() )
										{
											string sLIST_NAME = "ActiveUsers";
											for ( int i = 0; i < dt.Rows.Count; i++ )
											{
												DataRow row = dt.Rows[i];
												string sID           = Sql.ToString(row["ID"       ]);
												string sDISPLAY_NAME = Sql.ToString(row["USER_NAME"]);
												objs[L10n.NAME + "." + "." + sLIST_NAME + "." + sID] = sDISPLAY_NAME;
											}
										}
									}
								}
								Cache.Set("vwTERMINOLOGY.ReactClient" + (bAdmin ? ".Admin": ""), objs, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 05/19/2018 Paul.  Capture the last command for error tracking. 
					if ( ex.Message.Contains("The server supports a maximum of 2100 parameters") )
						SplendidError.SystemMessage("Error", new StackTrace(true).GetFrame(0), sLastCommand);
					throw;
				}
			}
			return objs;
		}

		public Dictionary<string, object> GetLoginTerminology()
		{
			Dictionary<string, object> objs = Cache.Get("vwTERMINOLOGY.ReactClient.Login") as Dictionary<string, object>;
			if ( objs == null )
			{
				objs = new Dictionary<string, object>();
				try
				{
					List<string> arr = new List<string>();
					arr.Add("LBL_USER_NAME"          );
					arr.Add("LBL_PASSWORD"           );
					arr.Add("LBL_LOGIN_BUTTON_LABEL" );
					arr.Add("LBL_SUBMIT_BUTTON_LABEL");
					arr.Add("NTC_LOGIN_MESSAGE"      );
					arr.Add("LBL_FORGOT_PASSWORD"    );
					arr.Add("LBL_EMAIL"              );
					// 08/05/2020 Paul.  Mobile Client terms. 
					arr.Add("LNK_MOBILE_CLIENT"      );
					arr.Add("LBL_REMOTE_SERVER"      );
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL = String.Empty;
						sSQL = "select NAME               " + ControlChars.CrLf
						     + "     , MODULE_NAME        " + ControlChars.CrLf
						     + "     , LIST_NAME          " + ControlChars.CrLf
						     + "     , DISPLAY_NAME       " + ControlChars.CrLf
						     + "  from vwTERMINOLOGY      " + ControlChars.CrLf
						     + " where LIST_NAME is null  " + ControlChars.CrLf
						     + "   and (MODULE_NAME is null or MODULE_NAME in ('Users', 'Offline'))" + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							string sLANG = Sql.ToString(Application["CONFIG.default_language"]);
							if ( Sql.IsEmptyString(sLANG) )
								sLANG = "en-US";
							Sql.AppendParameter(cmd, sLANG, "LANG");
							Sql.AppendParameter(cmd, arr.ToArray(), "NAME");
							cmd.CommandText += " order by MODULE_NAME, LIST_NAME, NAME" + ControlChars.CrLf;
							
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								using ( DataTable dt = new DataTable() )
								{
									da.Fill(dt);
									for ( int i = 0; i < dt.Rows.Count; i++ )
									{
										DataRow row = dt.Rows[i];
										string sNAME         = Sql.ToString(row["NAME"        ]);
										string sMODULE_NAME  = Sql.ToString(row["MODULE_NAME" ]);
										string sLIST_NAME    = Sql.ToString(row["LIST_NAME"   ]);
										string sDISPLAY_NAME = Sql.ToString(row["DISPLAY_NAME"]);
										objs[sLANG + "." + sMODULE_NAME + "." + sNAME] = sDISPLAY_NAME;
									}
								}
							}
							Cache.Set("vwTERMINOLOGY.ReactClient.Login", objs, DefaultCacheExpiration());
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					throw;
				}
			}
			return objs;
		}

		// 03/02/2019 Paul.  Functions are now static and take modules list input so that they can be used in the Admin API. 
		// 03/26/2019 Paul.  Admin has more custom lists. 
		public Dictionary<string, object> GetAllTerminologyLists(bool bAdmin)
		{
			// 09/27/2020 Paul.  Terminology is language specific. 
			Dictionary<string, object> objs = Cache.Get("vwTERMINOLOGY_PickList.ReactClient." + L10n.NAME + (bAdmin ? ".Admin" : "")) as Dictionary<string, object>;
			if ( objs == null )
			{
				objs = new Dictionary<string, object>();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL = String.Empty;
							sSQL = "select distinct                " + ControlChars.CrLf
							     + "       NAME                    " + ControlChars.CrLf
							     + "     , DISPLAY_NAME            " + ControlChars.CrLf
							     + "     , LIST_NAME               " + ControlChars.CrLf
							     + "     , LIST_ORDER              " + ControlChars.CrLf
							     + "  from vwTERMINOLOGY           " + ControlChars.CrLf
							     + " where lower(LANG) = @LANG     " + ControlChars.CrLf
							     + "   and LIST_NAME is not null   " + ControlChars.CrLf
							     + " order by LIST_NAME, LIST_ORDER" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@LANG", L10n.NAME.ToLower());
							
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dt = new DataTable() )
									{
										da.Fill(dt);
										string sLAST_LIST_NAME = String.Empty;
										List<string> layout = null;
										for ( int i = 0; i < dt.Rows.Count; i++ )
										{
											DataRow row = dt.Rows[i];
											string sNAME      = Sql.ToString(row["NAME"     ]);
											string sLIST_NAME = Sql.ToString(row["LIST_NAME"]);
											if ( sLAST_LIST_NAME != sLIST_NAME )
											{
												sLAST_LIST_NAME = sLIST_NAME;
												layout = new List<string>();
												objs.Add(L10n.NAME + "." + sLAST_LIST_NAME, layout);
											}
											layout.Add(sNAME);
										}
									}
									// 10/12/2012 Paul.  Since we are replacing the entire Terminology List object, we need to include custom lists. 
									using ( DataTable dt = this.Currencies() )
									{
										List<string> layout = new List<string>();
										objs.Add(L10n.NAME + ".Currencies", layout);
										for ( int i = 0; i < dt.Rows.Count; i++ )
										{
											DataRow row = dt.Rows[i];
											string sID = Sql.ToString(row["ID"]);
											layout.Add(sID);
										}
									}
									// 07/02/2020 Paul.  User Profile needs, TimeZones, Themes, Languages, Date/Time formats. 
									using ( DataTable dt = this.Timezones() )
									{
										List<string> layout = new List<string>();
										objs.Add(L10n.NAME + ".TimeZones", layout);
										for ( int i = 0; i < dt.Rows.Count; i++ )
										{
											DataRow row = dt.Rows[i];
											string sID = Sql.ToString(row["ID"]);
											layout.Add(sID);
										}
									}
									using ( DataTable dt = this.Themes() )
									{
										List<string> layout = new List<string>();
										objs.Add(L10n.NAME + ".Themes", layout);
										for ( int i = 0; i < dt.Rows.Count; i++ )
										{
											DataRow row = dt.Rows[i];
											string sID = Sql.ToString(row["NAME"]);
											layout.Add(sID);
										}
									}
									using ( DataTable dt = this.Release() )
									{
										List<string> layout = new List<string>();
										objs.Add(L10n.NAME + ".Release", layout);
										for ( int i = 0; i < dt.Rows.Count; i++ )
										{
											DataRow row = dt.Rows[i];
											string sID = Sql.ToString(row["ID"]);
											layout.Add(sID);
										}
									}
									// 12/18/2017 Paul.  Order modules will not exist on Community Edition. 
									if ( Sql.ToBoolean(Application["Modules.ContractTypes.RestEnabled"]) )
									{
										using ( DataTable dt = this.ContractTypes() )
										{
											List<string> layout = new List<string>();
											objs.Add(L10n.NAME + ".ContractTypes", layout);
											for ( int i = 0; i < dt.Rows.Count; i++ )
											{
												DataRow row = dt.Rows[i];
												string sID = Sql.ToString(row["ID"]);
												layout.Add(sID);
											}
										}
									}
									using ( DataTable dt = this.AssignedUser() )
									{
										List<string> layout = new List<string>();
										objs.Add(L10n.NAME + ".AssignedUser", layout);
										for ( int i = 0; i < dt.Rows.Count; i++ )
										{
											DataRow row = dt.Rows[i];
											string sID = Sql.ToString(row["ID"]);
											layout.Add(sID);
										}
									}
									// 06/13/2017 Paul.  HTML5 Dashboard needs the teams list. 
									if ( Config.enable_team_management() )
									{
										using ( DataTable dt = this.Teams() )
										{
											List<string> layout = new List<string>();
											objs.Add(L10n.NAME + ".Teams", layout);
											for ( int i = 0; i < dt.Rows.Count; i++ )
											{
												DataRow row = dt.Rows[i];
												string sID = Sql.ToString(row["ID"]);
												layout.Add(sID);
											}
										}
									}
									// 02/24/2013 Paul.  Build Calendar names from culture instead of from terminology. 
									List<string> lstMonthNames = new List<string>();
									objs.Add(L10n.NAME + ".month_names_dom", lstMonthNames);
									for ( int i = 1; i <= 12; i++ )
									{
										string sID = i.ToString();
										lstMonthNames.Add(sID);
									}
									List<string> lstShortMonthNames = new List<string>();
									objs.Add(L10n.NAME + ".short_month_names_dom", lstShortMonthNames);
									for ( int i = 1; i <= 12; i++ )
									{
										string sID = i.ToString();
										lstShortMonthNames.Add(sID);
									}
									List<string> lstDayNames = new List<string>();
									objs.Add(L10n.NAME + ".day_names_dom", lstDayNames);
									for ( int i = 0; i <= 6; i++ )
									{
										string sID = i.ToString();
										lstDayNames.Add(sID);
									}
									List<string> lstShortDayNames = new List<string>();
									objs.Add(L10n.NAME + ".short_day_names_dom", lstShortDayNames);
									for ( int i = 0; i <= 6; i++ )
									{
										string sID = i.ToString();
										lstShortDayNames.Add(sID);
									}
									// 07/03/2020 Paul.  Languages is global, not just admin. 
									using ( DataTable dt = this.Languages() )
									{
										Guid   gTIMEZONE = Sql.ToGuid  (Session["USER_SETTINGS/TIMEZONE"]);
										TimeZone T10n = TimeZone.CreateTimeZone(gTIMEZONE);

										DataView vw = new DataView(dt);
										List<string> layout = new List<string>();
										objs.Add(L10n.NAME + ".Languages", layout);
										for ( int i = 0; i < vw.Count; i++ )
										{
											DataRowView row = vw[i];
											string sNAME = Sql.ToString(row["NAME"]);
											layout.Add(sNAME);
											// 07/03/2020 Paul.  Include all date patterns for all supported langauges. 
											try
											{
												CultureInfo oldCulture   = Thread.CurrentThread.CurrentCulture   ;
												CultureInfo oldUICulture = Thread.CurrentThread.CurrentUICulture ;
												Thread.CurrentThread.CurrentCulture   = CultureInfo.CreateSpecificCulture(sNAME);
												Thread.CurrentThread.CurrentUICulture = new CultureInfo(sNAME);

												DateTime dtNow = T10n.FromServerTime(DateTime.Now);
												DateTimeFormatInfo oDateInfo   = Thread.CurrentThread.CurrentCulture.DateTimeFormat;
												NumberFormatInfo   oNumberInfo = Thread.CurrentThread.CurrentCulture.NumberFormat  ;

												List<string> layoutDATE_FORMAT = new List<string>();
												objs.Add(L10n.NAME + ".DateFormat." + sNAME, layoutDATE_FORMAT);
												List<string> layoutTIME_FORMAT = new List<string>();
												objs.Add(L10n.NAME + ".TimeFormat." + sNAME, layoutTIME_FORMAT);

												String[] aDateTimePatterns = oDateInfo.GetAllDateTimePatterns();
												foreach ( string sPattern in aDateTimePatterns )
												{
													// 11/12/2005 Paul.  Only allow patterns that have a full year. 
													// 10/15/2013 Paul.  Allow 2-digit year. 
													if ( sPattern.IndexOf("yy") >= 0 && sPattern.IndexOf("dd") >= 0 && sPattern.IndexOf("mm") <  0 )
													{
														layoutDATE_FORMAT.Add(sPattern);
													}
													if ( sPattern.IndexOf("yy") <  0 && sPattern.IndexOf("mm") >= 0 )
													{
														layoutTIME_FORMAT.Add(sPattern);
													}
												}
												Thread.CurrentThread.CurrentCulture = oldCulture  ;
												Thread.CurrentThread.CurrentCulture = oldUICulture;
											}
											catch(Exception ex)
											{
												SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
											}
										}
									}
									// 02/28/2016 Paul.  Order management in html5. 
									// 12/18/2017 Paul.  Order modules will not exist on Community Edition. 
									if ( Sql.ToBoolean(Application["Modules.TaxRates.RestEnabled"]) )
									{
										using ( DataTable dt = this.TaxRates() )
										{
											List<string> layout = new List<string>();
											objs.Add(L10n.NAME + ".TaxRates", layout);
											for ( int i = 0; i < dt.Rows.Count; i++ )
											{
												DataRow row = dt.Rows[i];
												string sID = Sql.ToString(row["ID"]);
												layout.Add(sID);
											}
										}
									}
									// 12/18/2017 Paul.  Order modules will not exist on Community Edition. 
									if ( Sql.ToBoolean(Application["Modules.Shippers.RestEnabled"]) )
									{
										using ( DataTable dt = this.Shippers() )
										{
											List<string> layout = new List<string>();
											objs.Add(L10n.NAME + ".Shippers", layout);
											for ( int i = 0; i < dt.Rows.Count; i++ )
											{
												DataRow row = dt.Rows[i];
												string sID = Sql.ToString(row["ID"]);
												layout.Add(sID);
											}
										}
									}
									// 12/18/2017 Paul.  Order modules will not exist on Community Edition. 
									if ( Sql.ToBoolean(Application["Modules.Discounts.RestEnabled"]) )
									{
										using ( DataTable dt = this.Discounts() )
										{
											DataView vwDiscounts = new DataView(dt);
											// 02/29/2016 Paul.  The Line Items Editor uses a reduced set of discounts. 
											vwDiscounts.RowFilter = "PRICING_FORMULA in ('PercentageDiscount', 'FixedDiscount')";
											List<string> layout = new List<string>();
											objs.Add(L10n.NAME + ".Discounts", layout);
											for ( int i = 0; i < vwDiscounts.Count; i++ )
											{
												DataRowView row = vwDiscounts[i];
												string sID = Sql.ToString(row["ID"]);
												layout.Add(sID);
											}
										}
									}
									// 12/18/2017 Paul.  Order modules will not exist on Community Edition. 
									if ( Sql.ToBoolean(Application["Modules.Quotes.RestEnabled"]) || Sql.ToBoolean(Application["Modules.Orders.RestEnabled"]) || Sql.ToBoolean(Application["Modules.Invoices.RestEnabled"]) )
									{
										using ( DataTable dt = this.List("pricing_formula_dom") )
										{
											DataView vwPricingFormulas = new DataView(dt);
											// 02/29/2016 Paul.  The Line Items Editor uses a reduced set of pricing formulas. 
											vwPricingFormulas.RowFilter = "NAME in ('PercentageDiscount', 'FixedDiscount')";
											List<string> layout = new List<string>();
											objs.Add(L10n.NAME + ".pricing_formula_line_items", layout);
											for ( int i = 0; i < vwPricingFormulas.Count; i++ )
											{
												DataRowView row = vwPricingFormulas[i];
												string sID = Sql.ToString(row["NAME"]);
												layout.Add(sID);
											}
										}
										using ( DataTable dt = this.PaymentTerms() )
										{
											List<string> layout = new List<string>();
											objs.Add(L10n.NAME + ".PaymentTerms", layout);
											for ( int i = 0; i < dt.Rows.Count; i++ )
											{
												DataRow row = dt.Rows[i];
												string sID = Sql.ToString(row["ID"]);
												layout.Add(sID);
											}
										}
									}
									// 08/19/2019 Paul.  Modules are used by the ReportDesigner, so it is a non-admin list. 
									using ( DataTable dt = this.Modules() )
									{
										DataView vw = new DataView(dt);
										List<string> layout = new List<string>();
										objs.Add(L10n.NAME + ".Modules", layout);
										for ( int i = 0; i < vw.Count; i++ )
										{
											DataRowView row = vw[i];
											string sID = Sql.ToString(row["MODULE_NAME"]);
											layout.Add(sID);
										}
									}
									// 04/15/2021 Paul.  RulesModules is used by Business Rules module. 
									using ( DataTable dt = this.RulesModules() )
									{
										DataView vw = new DataView(dt);
										vw.Sort = "DISPLAY_NAME";
										List<string> layout = new List<string>();
										objs.Add(L10n.NAME + ".RulesModules", layout);
										for ( int i = 0; i < vw.Count; i++ )
										{
											DataRowView row = vw[i];
											string sMODULE_NAME = Sql.ToString(row["MODULE_NAME"]);
											// 05/07/2021 Paul.  Prefilter the list to only those modules that can be edited. 
											if ( Security.GetUserAccess(sMODULE_NAME, "edit") >= 0 )
												layout.Add(sMODULE_NAME);
										}
										// 03/29/2012 Paul.  Add Rules Wizard support to Terminology module. 
										if ( Security.IS_ADMIN )
										{
											layout.Add("Terminology");
										}
									}
									// 05/01/2020 Paul.  Provide Email Templates to the React Client. 
									using ( DataTable dt = this.EmailTemplates() )
									{
										List<string> layout = new List<string>();
										objs.Add(L10n.NAME + ".EmailTemplates", layout);
										for ( int i = 0; i < dt.Rows.Count; i++ )
										{
											DataRow row = dt.Rows[i];
											string sID = Sql.ToString(row["ID"]);
											layout.Add(sID);
										}
									}
									// 05/01/2020 Paul.  Provide User Signatures to the React Client. 
									using ( DataTable dt = this.UserSignatures() )
									{
										List<string> layout = new List<string>();
										objs.Add(L10n.NAME + ".UserSignatures", layout);
										for ( int i = 0; i < dt.Rows.Count; i++ )
										{
											DataRow row = dt.Rows[i];
											string sID = Sql.ToString(row["ID"]);
											layout.Add(sID);
										}
									}
									// 05/01/2020 Paul.  Provide User Mailboxes to the React Client. 
									using ( DataTable dt = this.OutboundMail() )
									{
										List<string> layout = new List<string>();
										objs.Add(L10n.NAME + ".OutboundMail", layout);
										for ( int i = 0; i < dt.Rows.Count; i++ )
										{
											DataRow row = dt.Rows[i];
											string sID = Sql.ToString(row["ID"]);
											layout.Add(sID);
										}
									}
									// 04/18/2022 Paul.  EmailMarketing requires InboundEmailBounce list. 
									using ( DataTable dt = this.InboundEmailBounce() )
									{
										List<string> layout = new List<string>();
										objs.Add(L10n.NAME + ".InboundEmailBounce", layout);
										for ( int i = 0; i < dt.Rows.Count; i++ )
										{
											DataRow row = dt.Rows[i];
											string sID = Sql.ToString(row["ID"]);
											layout.Add(sID);
										}
									}
									// 01/24/2021 Paul.  Add OutboundSms. 
									using ( DataTable dt = this.OutboundSms() )
									{
										List<string> layout = new List<string>();
										objs.Add(L10n.NAME + ".OutboundSms", layout);
										for ( int i = 0; i < dt.Rows.Count; i++ )
										{
											DataRow row = dt.Rows[i];
											string sID = Sql.ToString(row["ID"]);
											layout.Add(sID);
										}
									}
									// 12/17/2020 Paul.  Provide ReportingModules to the React Client. 
									using ( DataTable dt = this.ReportingModules() )
									{
										List<string> layout = new List<string>();
										objs.Add(L10n.NAME + ".ReportingModules", layout);
										for ( int i = 0; i < dt.Rows.Count; i++ )
										{
											DataRow row = dt.Rows[i];
											string sID = Sql.ToString(row["MODULE_NAME"]);
											layout.Add(sID);
										}
									}
									// 03/26/2019 Paul.  Admin has more custom lists. 
									if ( bAdmin )
									{
										// 06/29/2020 Paul.  Order modules will not exist on Community Edition. 
										if ( Sql.ToBoolean(Application["Modules.Manufacturers.RestEnabled"]) || Sql.ToBoolean(Application["Modules.ProductTemplates.RestEnabled"]) )
										{
											using ( DataTable dt = this.Manufacturers() )
											{
												List<string> layout = new List<string>();
												objs.Add(L10n.NAME + ".Manufacturers", layout);
												for ( int i = 0; i < dt.Rows.Count; i++ )
												{
													DataRow row = dt.Rows[i];
													string sID = Sql.ToString(row["ID"]);
													layout.Add(sID);
												}
											}
										}
										// 06/29/2020 Paul.  Order modules will not exist on Community Edition. 
										if ( Sql.ToBoolean(Application["Modules.ProductTypes.RestEnabled"]) || Sql.ToBoolean(Application["Modules.ProductTemplates.RestEnabled"]) )
										{
											using ( DataTable dt = this.ProductTypes() )
											{
												List<string> layout = new List<string>();
												objs.Add(L10n.NAME + ".ProductTypes", layout);
												for ( int i = 0; i < dt.Rows.Count; i++ )
												{
													DataRow row = dt.Rows[i];
													string sID = Sql.ToString(row["ID"]);
													layout.Add(sID);
												}
											}
										}
										// 06/29/2020 Paul.  Order modules will not exist on Community Edition. 
										if ( Sql.ToBoolean(Application["Modules.ProductCategories.RestEnabled"]) || Sql.ToBoolean(Application["Modules.ProductTemplates.RestEnabled"]) )
										{
											using ( DataTable dt = this.ProductCategories() )
											{
												List<string> layout = new List<string>();
												objs.Add(L10n.NAME + ".ProductCategories", layout);
												for ( int i = 0; i < dt.Rows.Count; i++ )
												{
													DataRow row = dt.Rows[i];
													string sID = Sql.ToString(row["ID"]);
													layout.Add(sID);
												}
											}
										}
										using ( DataTable dt = this.SchedulerJobs() )
										{
											DataView vw = new DataView(dt);
											List<string> layout = new List<string>();
											objs.Add(L10n.NAME + ".SchedulerJobs", layout);
											for ( int i = 0; i < vw.Count; i++ )
											{
												DataRowView row = vw[i];
												string sID = Sql.ToString(row["NAME"]);
												layout.Add(sID);
											}
										}
										using ( DataTable dt = this.TerminologyPickLists() )
										{
											DataView vw = new DataView(dt);
											List<string> layout = new List<string>();
											objs.Add(L10n.NAME + ".TerminologyPickLists", layout);
											for ( int i = 0; i < vw.Count; i++ )
											{
												DataRowView row = vw[i];
												string sID = Sql.ToString(row["LIST_NAME"]);
												layout.Add(sID);
											}
										}
										using ( DataTable dt = this.DynamicButtonViews() )
										{
											DataView vw = new DataView(dt);
											List<string> layout = new List<string>();
											objs.Add(L10n.NAME + ".DynamicButtonViews", layout);
											for ( int i = 0; i < vw.Count; i++ )
											{
												DataRowView row = vw[i];
												string sID = Sql.ToString(row["VIEW_NAME"]);
												layout.Add(sID);
											}
										}
										using ( DataTable dt = this.LibraryPaymentGateways() )
										{
											DataView vw = new DataView(dt);
											List<string> layout = new List<string>();
											objs.Add(L10n.NAME + ".LibraryPaymentGateways", layout);
											for ( int i = 0; i < vw.Count; i++ )
											{
												DataRowView row = vw[i];
												string sID = Sql.ToString(row["NAME"]);
												layout.Add(sID);
											}
										}
										// 03/03/2020 Paul.  Include lists used by Layout Manager. 
										using ( DataTable dtModules = this.ModulesPopups() )
										{
											DataView vwModules = new DataView(dtModules);
											vwModules.RowFilter = "HAS_POPUP = 1";
											List<string> MODULE_TYPES = new List<string>();
											foreach ( DataRowView row in vwModules )
											{
												string sNAME = Sql.ToString(row["MODULE_NAME"]);
												MODULE_TYPES.Add(sNAME);
											}
											objs.Add(L10n.NAME + ".ModuleTypes", MODULE_TYPES);
										}
										List<string> FIELD_VALIDATORS = new List<string>();
										sSQL = "select ID                " + ControlChars.CrLf
										     + "     , NAME              " + ControlChars.CrLf
										     + "  from vwFIELD_VALIDATORS" + ControlChars.CrLf
										     + " order by NAME           " + ControlChars.CrLf;
										using ( IDbCommand cmdFIELD_VALIDATORS = con.CreateCommand() )
										{
											cmdFIELD_VALIDATORS.CommandText = sSQL;
											using ( DbDataAdapter daFIELD_VALIDATORS = dbf.CreateDataAdapter() )
											{
												((IDbDataAdapter)daFIELD_VALIDATORS).SelectCommand = cmdFIELD_VALIDATORS;
												using ( DataTable dt = new DataTable() )
												{
													daFIELD_VALIDATORS.Fill(dt);
													foreach ( DataRow row in dt.Rows )
													{
														string sNAME = Sql.ToString(row["ID"]);
														FIELD_VALIDATORS.Add(sNAME);
													}
												}
											}
										}
										objs.Add(L10n.NAME + ".FieldValidators", FIELD_VALIDATORS);
										// 01/29/2021 Paul.  Add EditCustomFields to React client. 
										using ( DataTable dtModules = this.CustomEditModules() )
										{
											List<string> MODULE_TYPES = new List<string>();
											foreach ( DataRow row in dtModules.Rows )
											{
												string sNAME = Sql.ToString(row["NAME"]);
												MODULE_TYPES.Add(sNAME);
											}
											objs.Add(L10n.NAME + ".CustomEditModules", MODULE_TYPES);
										}
										// 06/02/2021 Paul.  Provide WorkflowModules to the React Client. 
										using ( DataTable dt = this.WorkflowModules() )
										{
											List<string> layout = new List<string>();
											objs.Add(L10n.NAME + ".WorkflowModules", layout);
											for ( int i = 0; i < dt.Rows.Count; i++ )
											{
												DataRow row = dt.Rows[i];
												string sID = Sql.ToString(row["MODULE_NAME"]);
												layout.Add(sID);
											}
										}
										// 09/13/2021 Paul.  Provide AuditedModules to the React Client. 
										using ( DataTable dt = this.AuditedModules() )
										{
											List<string> layout = new List<string>();
											objs.Add(L10n.NAME + ".AuditedModules", layout);
											for ( int i = 0; i < dt.Rows.Count; i++ )
											{
												DataRow row = dt.Rows[i];
												string sID = Sql.ToString(row["MODULE_NAME"]);
												layout.Add(sID);
											}
										}
										// 09/13/2021 Paul.  Provide ActiveUsers to the React Client. 
										using ( DataTable dt = this.ActiveUsers() )
										{
											List<string> layout = new List<string>();
											objs.Add(L10n.NAME + ".ActiveUsers", layout);
											for ( int i = 0; i < dt.Rows.Count; i++ )
											{
												DataRow row = dt.Rows[i];
												string sID = Sql.ToString(row["ID"]);
												layout.Add(sID);
											}
										}
									}
								}
								Cache.Set("vwTERMINOLOGY_PickList.ReactClient" + (bAdmin ? ".Admin" : ""), objs, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					throw;
				}
			}
			return objs;
		}

		// 12/10/2022 Paul.  Allow Login Terminology Lists to be customized. 
		public Dictionary<string, object> GetLoginTerminologyLists(List<string> lstLIST_NAME, Dictionary<string, object> TERMINOLOGY)
		{
			// 09/27/2020 Paul.  Terminology is language specific. 
			L10N L10n = new L10N(Session["USER_SETTINGS/CULTURE"] as string);
			Dictionary<string, object> objs = Cache.Get("vwTERMINOLOGY_PickList.ReactClient.Login") as Dictionary<string, object>;
			if ( objs == null )
			{
				objs = new Dictionary<string, object>();
				try
				{
					// 12/14/2022 Paul.  Most companies will not return list data. 
					if ( lstLIST_NAME.Count > 0 )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL = String.Empty;
							sSQL = "select distinct                " + ControlChars.CrLf
							     + "       NAME                    " + ControlChars.CrLf
							     + "     , DISPLAY_NAME            " + ControlChars.CrLf
							     + "     , LIST_NAME               " + ControlChars.CrLf
							     + "     , LIST_ORDER              " + ControlChars.CrLf
							     + "  from vwTERMINOLOGY           " + ControlChars.CrLf
							     + " where lower(LANG) = @LANG     " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@LANG", L10n.NAME.ToLower());
								Sql.AppendParameter(cmd, lstLIST_NAME.ToArray(), "LIST_NAME");
								cmd.CommandText += " order by LIST_NAME, LIST_ORDER" + ControlChars.CrLf;
							
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dt = new DataTable() )
									{
										da.Fill(dt);
										string sLAST_LIST_NAME = String.Empty;
										List<string> layout = null;
										for ( int i = 0; i < dt.Rows.Count; i++ )
										{
											DataRow row = dt.Rows[i];
											string sNAME         = Sql.ToString(row["NAME"        ]);
											string sLIST_NAME    = Sql.ToString(row["LIST_NAME"   ]);
											string sDISPLAY_NAME = Sql.ToString(row["DISPLAY_NAME"]);
											if ( sLAST_LIST_NAME != sLIST_NAME )
											{
												sLAST_LIST_NAME = sLIST_NAME;
												layout = new List<string>();
												objs.Add(L10n.NAME + "." + sLAST_LIST_NAME, layout);
											}
											layout.Add(sNAME);
											// 12/10/2022 Paul.  Make sure to include display name in TERMINOLOGY, as TERMINOLOGY_LIST only includes lists. 
											TERMINOLOGY[L10n.NAME + "." + "." + sLIST_NAME + "." + sNAME] = sDISPLAY_NAME;
										}
									}
								}
								// 12/10/2022 Paul.  In case the list is custom, search the custom caches. 
								foreach ( string sCACHE_NAME in lstLIST_NAME )
								{
									List<SplendidCacheReference> arrCustomCaches = this.CustomCaches;
									foreach ( SplendidCacheReference cache in arrCustomCaches )
									{
										if ( cache.Name == sCACHE_NAME )
										{
											string sDataValueField = cache.DataValueField;
											string sDataTextField  = cache.DataTextField ;
											SplendidCacheCallback cbkDataSource = cache.DataSource;
											using ( DataTable dt = cbkDataSource() )
											{
												List<string> layout = new List<string>();
												objs.Add(L10n.NAME + "." + sCACHE_NAME, layout);
												for ( int i = 0; i < dt.Rows.Count; i++ )
												{
													DataRow row = dt.Rows[i];
													string sID   = Sql.ToString(row[sDataValueField]);
													string sNAME = Sql.ToString(row[sDataValueField]);
													layout.Add(sID);
													// 12/10/2022 Paul.  Make sure to include display name in TERMINOLOGY, as TERMINOLOGY_LIST only includes lists. 
													TERMINOLOGY[L10n.NAME + "." + "." + sCACHE_NAME + "." + sID] = sNAME;
												}
											}
										}
									}
								}
							}
						}
					}
					Cache.Set("vwTERMINOLOGY_PickList.ReactClient.Login", objs, DefaultCacheExpiration());
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					throw;
				}
			}
			return objs;
		}

		// 03/02/2019 Paul.  Functions are now static and take modules list input so that they can be used in the Admin API. 
		public Dictionary<string, object> GetAllTaxRates()
		{
			Dictionary<string, object> objs = Cache.Get("vwTAX_RATES_LISTBOX.ReactClient") as Dictionary<string, object>;
			if ( objs == null )
			{
				objs = new Dictionary<string, object>();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL = String.Empty;
							// 04/07/2016 Paul.  Tax rates per team. 
							sSQL = "select *                  " + ControlChars.CrLf
								 + "  from vwTAX_RATES_LISTBOX" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								// 04/07/2016 Paul.  Tax rates per team. 
								if ( Sql.ToBoolean(Application["CONFIG.Orders.EnableTaxRateTeams"]) )
									Security.Filter(cmd, "TaxRates", "list");
								cmd.CommandText += " order by LIST_ORDER      " + ControlChars.CrLf;
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dt = new DataTable() )
									{
										da.Fill(dt);
										for ( int i = 0; i < dt.Rows.Count; i++ )
										{
											DataRow row = dt.Rows[i];
											Dictionary<string, object> drow = new Dictionary<string, object>();
											for ( int j = 0; j < dt.Columns.Count; j++ )
											{
												drow.Add(dt.Columns[j].ColumnName, row[j]);
											}
											objs.Add(Sql.ToString(row["ID"]), drow);
										}
										Cache.Set("vwTAX_RATES_LISTBOX.ReactClient", objs, DefaultCacheExpiration());
									}
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					throw;
				}
			}
			return objs;
		}

		// 03/02/2019 Paul.  Functions are now static and take modules list input so that they can be used in the Admin API. 
		public Dictionary<string, object> GetAllDiscounts()
		{
			Dictionary<string, object> objs = Cache.Get("vwDISCOUNTS_LISTBOX.ReactClient") as Dictionary<string, object>;
			if ( objs == null )
			{
				objs = new Dictionary<string, object>();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL = String.Empty;
							sSQL = "select *                  " + ControlChars.CrLf
							     + "  from vwDISCOUNTS_LISTBOX" + ControlChars.CrLf
							     + " where PRICING_FORMULA in ('PercentageDiscount', 'FixedDiscount')" + ControlChars.CrLf
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
										for ( int i = 0; i < dt.Rows.Count; i++ )
										{
											DataRow row = dt.Rows[i];
											Dictionary<string, object> drow = new Dictionary<string, object>();
											for ( int j = 0; j < dt.Columns.Count; j++ )
											{
												drow.Add(dt.Columns[j].ColumnName, row[j]);
											}
											objs.Add(Sql.ToString(row["ID"]), drow);
										}
										Cache.Set("vwDISCOUNTS_LISTBOX.ReactClient", objs, DefaultCacheExpiration());
									}
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					throw;
				}
			}
			return objs;
		}

		// 09/12/2019 Paul.  User Profile needs the timezones and currencies. 
		public Dictionary<string, object> GetAllTimezones()
		{
			Dictionary<string, object> objs = Cache.Get("vwTIMEZONES.ReactClient") as Dictionary<string, object>;
			if ( objs == null )
			{
				objs = new Dictionary<string, object>();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						using ( DataTable dt = this.Timezones() )
						{
							for ( int i = 0; i < dt.Rows.Count; i++ )
							{
								DataRow row = dt.Rows[i];
								Dictionary<string, object> drow = new Dictionary<string, object>();
								for ( int j = 0; j < dt.Columns.Count; j++ )
								{
									drow.Add(dt.Columns[j].ColumnName, row[j]);
								}
								objs.Add(Sql.ToString(row["ID"]), drow);
							}
							Cache.Set("vwTIMEZONES.ReactClient", objs, DefaultCacheExpiration());
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					throw;
				}
			}
			return objs;
		}

		// 09/12/2019 Paul.  User Profile needs the timezones and currencies. 
		public Dictionary<string, object> GetAllCurrencies()
		{
			Dictionary<string, object> objs = Cache.Get("vwCURRENCIES.ReactClient") as Dictionary<string, object>;
			if ( objs == null )
			{
				objs = new Dictionary<string, object>();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						using ( DataTable dt = this.Currencies() )
						{
							for ( int i = 0; i < dt.Rows.Count; i++ )
							{
								DataRow row = dt.Rows[i];
								Dictionary<string, object> drow = new Dictionary<string, object>();
								for ( int j = 0; j < dt.Columns.Count; j++ )
								{
									drow.Add(dt.Columns[j].ColumnName, row[j]);
								}
								objs.Add(Sql.ToString(row["ID"]), drow);
							}
							Cache.Set("vwCURRENCIES.ReactClient", objs, DefaultCacheExpiration());
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					throw;
				}
			}
			return objs;
		}

		// 09/12/2019 Paul.  User Profile needs the timezones and currencies. 
		public Dictionary<string, object> GetAllLanguages()
		{
			Dictionary<string, object> objs = Cache.Get("vwLANGUAGES.ReactClient") as Dictionary<string, object>;
			if ( objs == null )
			{
				objs = new Dictionary<string, object>();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						using ( DataTable dt = this.Languages() )
						{
							for ( int i = 0; i < dt.Rows.Count; i++ )
							{
								DataRow row = dt.Rows[i];
								Dictionary<string, object> drow = new Dictionary<string, object>();
								for ( int j = 0; j < dt.Columns.Count; j++ )
								{
									drow.Add(dt.Columns[j].ColumnName, row[j]);
								}
								objs.Add(Sql.ToString(row["NAME"]), drow);
							}
							Cache.Set("vwLANGUAGES.ReactClient", objs, DefaultCacheExpiration());
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					throw;
				}
			}
			return objs;
		}

		// 03/02/2019 Paul.  Functions are now static and take modules list input so that they can be used in the Admin API. 
		public void GetAllReactCustomViews(Dictionary<string, object> objs, List<string> lstMODULES, string sFolder, bool bIS_ADMIN, bool bLogin)
		{
			try
			{
				if ( Security.IsAuthenticated() || bLogin )
				{
					string sCustomViewsJS = sFolder.Replace("~", hostingEnvironment.ContentRootPath).Replace("/", "\\");  // Context.Server.MapPath(sFolder);
					if ( Directory.Exists(sCustomViewsJS) )
					{
						FileInfo objInfo = null;
						// 03/01/2019 Paul.  List of modules. 
						string[] arrModuleDirectories = Directory.GetDirectories(sCustomViewsJS);
						for ( int nModuleDirectory = 0 ; nModuleDirectory < arrModuleDirectories.Length ; nModuleDirectory++ )
						{
							objInfo = new FileInfo(arrModuleDirectories[nModuleDirectory]);
							if ( lstMODULES.Contains(objInfo.Name) )
							{
								string sMODULE_NAME = objInfo.Name;
								Dictionary<string, object> module = new Dictionary<string, object>();
								objs.Add(sMODULE_NAME, module);
								// 03/01/2019 Paul.  List of layout types. 
								string[] arrTypeDirectories = Directory.GetDirectories(objInfo.FullName);
								for ( int nTypeDirectory = 0 ; nTypeDirectory < arrTypeDirectories.Length ; nTypeDirectory++ )
								{
									objInfo = new FileInfo(arrTypeDirectories[nTypeDirectory]);
									// 06/26/2019 Paul.  Continue with ListView naming convention.  Add SubPanels. 
									if ( objInfo.Name == "DetailViews" || objInfo.Name == "EditViews" || objInfo.Name == "ListViews" || objInfo.Name == "SubPanels" )
									{
										string sLAYOUT_TYPE = objInfo.Name;
										Dictionary<string, object> layout = new Dictionary<string, object>();
										module.Add(sLAYOUT_TYPE, layout);
										// 03/01/2019 Paul.  list of layouts. 
										string[] arrFiles = Directory.GetFiles(objInfo.FullName);
										for ( int nFile = 0 ; nFile < arrFiles.Length ; nFile++ )
										{
											objInfo = new FileInfo(arrFiles[nFile]);
											// 03/01/2019 Paul.  Allow js or tsx.  The benefit to tsx is that IIS does not return this file type by default. 
											if ( objInfo.Name.EndsWith(".js") || objInfo.Name.EndsWith(".tsx") )
											{
												string sLAYOUT_NAME = objInfo.Name.Substring(0, objInfo.Name.Length - objInfo.Extension.Length);
												string sLAYOUT_BODY = File.ReadAllText(objInfo.FullName);
												layout.Add(sLAYOUT_NAME, sLAYOUT_BODY);
											}
										}
									}
								}
							}
						}
					}
					// 07/25/2019 Paul.  The database react custom views will replace the defaults on the file system. 
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL = String.Empty;
						sSQL = "select NAME                       " + ControlChars.CrLf
						     + "     , MODULE_NAME                " + ControlChars.CrLf
						     + "     , CATEGORY                   " + ControlChars.CrLf
						     + "     , CONTENT                    " + ControlChars.CrLf
						     + "  from vwREACT_CUSTOM_VIEWS       " + ControlChars.CrLf
						     + " where IS_ADMIN = @IS_ADMIN       " + ControlChars.CrLf
						     + "   and CATEGORY in (N'DetailViews', N'EditViews', N'ListViews', N'SubPanels')" + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@IS_ADMIN", bIS_ADMIN);
							// 07/25/2019 Paul.  Don't filter by module when getting the admin views. 
							if ( !bIS_ADMIN )
								Sql.AppendParameter(cmd, lstMODULES.ToArray(), "MODULE_NAME", true);
							cmd.CommandText += " order by MODULE_NAME, NAME" + ControlChars.CrLf;
						
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								using ( DataTable dt = new DataTable() )
								{
									da.Fill(dt);
									foreach ( DataRow row in dt.Rows )
									{
										string sMODULE_NAME = Sql.ToString(row["MODULE_NAME"]);
										string sLAYOUT_TYPE = Sql.ToString(row["CATEGORY"   ]);
										string sLAYOUT_NAME = Sql.ToString(row["NAME"       ]);
										string sLAYOUT_BODY = Sql.ToString(row["CONTENT"    ]);
										Dictionary<string, object> module = null;
										if ( objs.ContainsKey(sMODULE_NAME) )
										{
											module = objs[sMODULE_NAME] as Dictionary<string, object>;
										}
										else
										{
											module = new Dictionary<string, object>();
											objs.Add(sMODULE_NAME, module);
										}
										Dictionary<string, object> layout = null;
										if ( module.ContainsKey(sLAYOUT_TYPE) )
										{
											layout = module[sLAYOUT_TYPE] as Dictionary<string, object>;
										}
										else
										{
											layout = new Dictionary<string, object>();
											module.Add(sLAYOUT_TYPE, layout);
										}
										if ( layout.ContainsKey(sLAYOUT_NAME) )
										{
											layout[sLAYOUT_NAME] = sLAYOUT_BODY;
										}
										else
										{
											layout.Add(sLAYOUT_NAME, sLAYOUT_BODY);
										}
									}
								}
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				throw;
			}
		}

		// 05/23/2019 Paul.  Include Dashlet views, but we do not yet have a way to separate by module. 
		public void GetAllReactDashletViews(Dictionary<string, object> objs, List<string> lstMODULES, string sFolder)
		{
			try
			{
				if ( Security.IsAuthenticated() )
				{
					Dictionary<string, object> layout = new Dictionary<string, object>();
					objs.Add("Dashlets", layout);
					
					string sCustomViewsJS = sFolder.Replace("~", hostingEnvironment.ContentRootPath).Replace("/", "\\");  // Context.Server.MapPath(sFolder);
					if ( Directory.Exists(sCustomViewsJS) )
					{
						string[] arrFiles = Directory.GetFiles(sCustomViewsJS);
						for ( int nFile = 0 ; nFile < arrFiles.Length ; nFile++ )
						{
							FileInfo objInfo = new FileInfo(arrFiles[nFile]);
							// 03/01/2019 Paul.  Allow js or tsx.  The benefit to tsx is that IIS does not return this file type by default. 
							if ( objInfo.Name.EndsWith(".js") || objInfo.Name.EndsWith(".tsx") )
							{
								string sLAYOUT_NAME = objInfo.Name.Substring(0, objInfo.Name.Length - objInfo.Extension.Length);
								string sLAYOUT_BODY = File.ReadAllText(objInfo.FullName);
								// 11/18/2019 Paul.  Just in case there is a JS and a TSX of the same filename. 
								if ( layout.ContainsKey(sLAYOUT_NAME) )
								{
									layout[sLAYOUT_NAME] = sLAYOUT_BODY;
								}
								else
								{
									layout.Add(sLAYOUT_NAME, sLAYOUT_BODY);
								}
							}
						}
					}
					// 07/25/2019 Paul.  The database react custom views will replace the defaults on the file system. 
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL = String.Empty;
						sSQL = "select NAME                       " + ControlChars.CrLf
						     + "     , MODULE_NAME                " + ControlChars.CrLf
						     + "     , CATEGORY                   " + ControlChars.CrLf
						     + "     , CONTENT                    " + ControlChars.CrLf
						     + "  from vwREACT_CUSTOM_VIEWS       " + ControlChars.CrLf
						     + " where CATEGORY = N'Dashlets'     " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AppendParameter(cmd, lstMODULES.ToArray(), "MODULE_NAME", true);
							cmd.CommandText += " order by MODULE_NAME, NAME" + ControlChars.CrLf;
						
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								using ( DataTable dt = new DataTable() )
								{
									da.Fill(dt);
									foreach ( DataRow row in dt.Rows )
									{
										string sLAYOUT_NAME = Sql.ToString(row["NAME"   ]);
										string sLAYOUT_BODY = Sql.ToString(row["CONTENT"]);
										if ( layout.ContainsKey(sLAYOUT_NAME) )
										{
											layout[sLAYOUT_NAME] = sLAYOUT_BODY;
										}
										else
										{
											layout.Add(sLAYOUT_NAME, sLAYOUT_BODY);
										}
									}
								}
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				throw;
			}
		}

		// 05/24/2019 Paul.  Return Dashboard in GetAllLayouts. 
		public Dictionary<string, object> GetAllDashboards()
		{
			Dictionary<string, object> objs = Session["vwDASHBOARDS.ReactClient"] as Dictionary<string, object>;
			if ( objs == null )
			{
				objs = new Dictionary<string, object>();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL = String.Empty;
							// 02/23/2020 Paul.  ASSIGNED_USER_ID is used to determine if the dashboard is global. 
							sSQL = "select ID                         " + ControlChars.CrLf
							     + "     , NAME                       " + ControlChars.CrLf
							     + "     , CATEGORY                   " + ControlChars.CrLf
							     + "     , ASSIGNED_USER_ID           " + ControlChars.CrLf
							     + "  from vwDASHBOARDS               " + ControlChars.CrLf
							     + " where ASSIGNED_USER_ID = @USER_ID" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@USER_ID", Security.USER_ID);
								cmd.CommandText += " order by CATEGORY, NAME" + ControlChars.CrLf;
							
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dt = new DataTable() )
									{
										da.Fill(dt);
										string sLAST_CATEGORY = String.Empty;
										List<object> arrCategories = null;
										foreach ( DataRow row in dt.Rows )
										{
											Guid   gID       = Sql.ToGuid  (row["ID"      ]);
											string sNAME     = Sql.ToString(row["NAME"    ]);
											string sCATEGORY = Sql.ToString(row["CATEGORY"]);
											if ( arrCategories == null || sLAST_CATEGORY != sCATEGORY )
											{
												arrCategories = new List<object>();
												objs.Add(sCATEGORY, arrCategories);
												sLAST_CATEGORY = sCATEGORY;
											}
											Dictionary<string, object> obj = new Dictionary<string, object>();
											obj["ID"              ] = gID      ;
											obj["NAME"            ] = sNAME    ;
											obj["CATEGORY"        ] = sCATEGORY;
											// 06/15/2019 Paul.  Include ASSIGNED_USER_ID so that we can determine if it is a global dashboard. 
											obj["ASSIGNED_USER_ID"] = Security.USER_ID;
											arrCategories.Add(obj);
										}
									}
								}
							}
							// 02/23/2020 Paul.  ASSIGNED_USER_ID is used to determine if the dashboard is global. 
							sSQL = "select ID                         " + ControlChars.CrLf
							     + "     , NAME                       " + ControlChars.CrLf
							     + "     , CATEGORY                   " + ControlChars.CrLf
							     + "     , ASSIGNED_USER_ID           " + ControlChars.CrLf
							     + "  from vwDASHBOARDS               " + ControlChars.CrLf
							     + " where ASSIGNED_USER_ID is null   " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								cmd.CommandText += " order by CATEGORY, NAME" + ControlChars.CrLf;
							
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dt = new DataTable() )
									{
										da.Fill(dt);
										string sLAST_CATEGORY = String.Empty;
										List<object> arrCategories = null;
										foreach ( DataRow row in dt.Rows )
										{
											Guid   gID       = Sql.ToGuid  (row["ID"      ]);
											string sNAME     = Sql.ToString(row["NAME"    ]);
											string sCATEGORY = Sql.ToString(row["CATEGORY"]);
											if ( arrCategories == null || sLAST_CATEGORY != sCATEGORY )
											{
												arrCategories = new List<object>();
												objs.Add(sCATEGORY + ".Default", arrCategories);
												sLAST_CATEGORY = sCATEGORY;
											}
											Dictionary<string, object> obj = new Dictionary<string, object>();
											obj["ID"              ] = gID      ;
											obj["NAME"            ] = sNAME    ;
											obj["CATEGORY"        ] = sCATEGORY;
											// 06/15/2019 Paul.  Include ASSIGNED_USER_ID so that we can determine if it is a global dashboard. 
											obj["ASSIGNED_USER_ID"] = null     ;
											arrCategories.Add(obj);
										}
									}
								}
							}
							Session["vwDASHBOARDS.ReactClient"] = objs;
						}
					}
				}
				catch(Exception ex)
				{
					// 04/28/2019 Paul.  This error is not critical, so we can just log and ignore. 
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return objs;
		}

		public Dictionary<string, object> GetAllDashboardPanels(List<string> lstMODULES)
		{
			Dictionary<string, object> objs = Session["vwDASHBOARDS_PANELS.ReactClient"] as Dictionary<string, object>;
			if ( objs == null )
			{
				objs = new Dictionary<string, object>();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL = String.Empty;
							sSQL = "select DASHBOARD_ID                          " + ControlChars.CrLf
							     + "     , DASHBOARD_NAME                        " + ControlChars.CrLf
							     + "     , ID                                    " + ControlChars.CrLf
							     + "     , DASHBOARD_APP_ID                      " + ControlChars.CrLf
							     + "     , NAME                                  " + ControlChars.CrLf
							     + "     , CATEGORY                              " + ControlChars.CrLf
							     + "     , MODULE_NAME                           " + ControlChars.CrLf
							     + "     , TITLE                                 " + ControlChars.CrLf
							     + "     , SETTINGS_EDITVIEW                     " + ControlChars.CrLf
							     + "     , IS_ADMIN                              " + ControlChars.CrLf
							     + "     , APP_ENABLED                           " + ControlChars.CrLf
							     + "     , SCRIPT_URL                            " + ControlChars.CrLf
							     + "     , DEFAULT_SETTINGS                      " + ControlChars.CrLf
							     + "     , PANEL_ORDER                           " + ControlChars.CrLf
							     + "     , ROW_INDEX                             " + ControlChars.CrLf
							     + "     , COLUMN_WIDTH                          " + ControlChars.CrLf
							     + "  from vwDASHBOARDS_PANELS                   " + ControlChars.CrLf
							     + " where (   PARENT_ASSIGNED_USER_ID = @USER_ID" + ControlChars.CrLf
							     + "        or PARENT_ASSIGNED_USER_ID is null)  " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@USER_ID", Security.USER_ID);
								Sql.AppendParameter(cmd, lstMODULES.ToArray(), "MODULE_NAME");
								cmd.CommandText += " order by DASHBOARD_ID, PANEL_ORDER" + ControlChars.CrLf;
							
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dt = new DataTable() )
									{
										da.Fill(dt);
										Guid gLAST_DASHBOARD = Guid.Empty;
										List<object> arrDashboards = null;
										foreach ( DataRow row in dt.Rows )
										{
											Guid   gDASHBOARD_ID      = Sql.ToGuid   (row["DASHBOARD_ID"        ]);
											string sDASHBOARD_NAME    = Sql.ToString (row["DASHBOARD_NAME"      ]);
											Guid   gID                = Sql.ToGuid   (row["ID"                  ]);
											string sDASHBOARD_APP_ID  = Sql.ToString (row["DASHBOARD_APP_ID"    ]);
											string sNAME              = Sql.ToString (row["NAME"                ]);
											string sCATEGORY          = Sql.ToString (row["CATEGORY"            ]);
											string sMODULE_NAME       = Sql.ToString (row["MODULE_NAME"         ]);
											string sTITLE             = Sql.ToString (row["TITLE"               ]);
											string sSETTINGS_EDITVIEW = Sql.ToString (row["SETTINGS_EDITVIEW"   ]);
											bool   bIS_ADMIN          = Sql.ToBoolean(row["IS_ADMIN"            ]);
											bool   bAPP_ENABLED       = Sql.ToBoolean(row["APP_ENABLED"         ]);
											string sSCRIPT_URL        = Sql.ToString (row["SCRIPT_URL"          ]);
											string sDEFAULT_SETTINGS  = Sql.ToString (row["DEFAULT_SETTINGS"    ]);
											int    nPANEL_ORDER       = Sql.ToInteger(row["PANEL_ORDER"         ]);
											int    nROW_INDEX         = Sql.ToInteger(row["ROW_INDEX"           ]);
											int    nCOLUMN_WIDTH      = Sql.ToInteger(row["COLUMN_WIDTH"        ]);
											if ( arrDashboards == null || gLAST_DASHBOARD != gDASHBOARD_ID )
											{
												arrDashboards = new List<object>();
												objs.Add(gDASHBOARD_ID.ToString(), arrDashboards);
												gLAST_DASHBOARD = gDASHBOARD_ID;
											}
											Dictionary<string, object> obj = new Dictionary<string, object>();
											obj["DASHBOARD_ID"        ] = gDASHBOARD_ID      ;
											obj["DASHBOARD_NAME"      ] = sDASHBOARD_NAME    ;
											obj["ID"                  ] = gID                ;
											obj["DASHBOARD_APP_ID"    ] = sDASHBOARD_APP_ID  ;
											obj["NAME"                ] = sNAME              ;
											obj["CATEGORY"            ] = sCATEGORY          ;
											obj["MODULE_NAME"         ] = sMODULE_NAME       ;
											obj["TITLE"               ] = sTITLE             ;
											obj["SETTINGS_EDITVIEW"   ] = sSETTINGS_EDITVIEW ;
											obj["IS_ADMIN"            ] = bIS_ADMIN          ;
											obj["APP_ENABLED"         ] = bAPP_ENABLED       ;
											obj["SCRIPT_URL"          ] = sSCRIPT_URL        ;
											obj["DEFAULT_SETTINGS"    ] = sDEFAULT_SETTINGS  ;
											obj["PANEL_ORDER"         ] = nPANEL_ORDER       ;
											obj["ROW_INDEX"           ] = nROW_INDEX         ;
											obj["COLUMN_WIDTH"        ] = nCOLUMN_WIDTH      ;
											arrDashboards.Add(obj);
										}
										Session["vwDASHBOARDS_PANELS.ReactClient"] = objs;
									}
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					// 04/28/2019 Paul.  This error is not critical, so we can just log and ignore. 
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return objs;
		}

		// 05/26/2019 Paul.  Return Users and Teams in GetAllLayouts. 
		public Dictionary<string, object> GetAllUsers()
		{
			Dictionary<string, object> objs = Cache.Get("vwUSERS.ReactClient") as Dictionary<string, object>;
			if ( objs == null )
			{
				objs = new Dictionary<string, object>();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL = String.Empty;
							sSQL = "select ID                      " + ControlChars.CrLf
							     + "     , NAME                    " + ControlChars.CrLf
							     + "     , USER_NAME               " + ControlChars.CrLf
							     + "  from vwUSERS_ASSIGNED_TO_List" + ControlChars.CrLf
							     + " order by USER_NAME asc        " + ControlChars.CrLf;
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
											Guid   gID        = Sql.ToGuid  (row["ID"       ]);
											string sNAME      = Sql.ToString(row["NAME"     ]);
											string sUSER_NAME = Sql.ToString(row["USER_NAME"]);
											Dictionary<string, object> obj = new Dictionary<string, object>();
											obj["ID"       ] = gID       ;
											obj["NAME"     ] = sNAME     ;
											obj["USER_NAME"] = sUSER_NAME;
											objs.Add(gID.ToString(), obj);
										}
										Cache.Set("vwUSERS.ReactClient", objs, DefaultCacheExpiration());
									}
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					// 04/28/2019 Paul.  This error is not critical, so we can just log and ignore. 
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return objs;
		}

		// 05/26/2019 Paul.  Return Users and Teams in GetAllLayouts. 
		public Dictionary<string, object> GetAllTeams()
		{
			Dictionary<string, object> objs = Cache.Get("vwTEAMS.ReactClient") as Dictionary<string, object>;
			if ( objs == null )
			{
				objs = new Dictionary<string, object>();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL = String.Empty;
							// 01/17/2020 Paul.  There is a need for all the custom fields in the react client. 
							sSQL = "select *               " + ControlChars.CrLf
							     + "  from vwTEAMS         " + ControlChars.CrLf
							     + " order by NAME asc     " + ControlChars.CrLf;
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
											Guid   gID        = Sql.ToGuid  (row["ID"       ]);
											string sNAME      = Sql.ToString(row["NAME"     ]);
											Dictionary<string, object> obj = new Dictionary<string, object>();
											obj["ID"       ] = gID       ;
											obj["NAME"     ] = sNAME     ;
											objs.Add(gID.ToString(), obj);
											// 01/17/2020 Paul.  There is a need for all the custom fields in the react client. 
											foreach ( DataColumn col in dt.Columns )
											{
												// 01/17/2020 Paul.  We don't need a bunch of standard fields, just the custom. 
												switch ( col.ColumnName )
												{
													case "DATE_ENTERED"     :
													case "DATE_MODIFIED"    :
													case "DATE_MODIFIED_UTC":
													case "DESCRIPTION"      :
													case "CREATED_BY"       :
													case "MODIFIED_BY"      :
													case "CREATED_BY_NAME"  :
													case "MODIFIED_BY_NAME" :
													case "ID_C"             :
													case "ID"               :
													case "NAME"             :
														break;
													default:
													{
														obj[col.ColumnName] = Sql.ToString(row[col.ColumnName]);
														break;
													}
												}
											}
										}
										Cache.Set("vwTEAMS.ReactClient", objs, DefaultCacheExpiration());
									}
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					// 04/28/2019 Paul.  This error is not critical, so we can just log and ignore. 
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return objs;
		}

		// 12/22/2021 Paul.  In order to return data, all public properties must have get/set. 
		// http://www.binaryintellect.net/articles/a1e0e49e-d4d0-4b7c-b758-84234f14047b.aspx
		// 12/23/2021 Paul.  We need to prevent UserProfile from getting converted to camel case. 
		public class UserProfile
		{
			public Guid   USER_ID                      { get; set; }
			// 07/15/2021 Paul.  React Client needs to access the ASP.NET_SessionId. 
			public string USER_SESSION                 { get; set; }
			public string USER_NAME                    { get; set; }
			public string FULL_NAME                    { get; set; }
			public Guid   TEAM_ID                      { get; set; }
			public string TEAM_NAME                    { get; set; }
			public string USER_LANG                    { get; set; }
			public string USER_DATE_FORMAT             { get; set; }
			public string USER_TIME_FORMAT             { get; set; }
			// 04/23/2013 Paul.  The HTML5 Offline Client now supports Atlantic theme. 
			public string USER_THEME                   { get; set; }
			public string USER_CURRENCY_ID             { get; set; }
			public string USER_TIMEZONE_ID             { get; set; }
			// 10/28/2021 Paul.  This is our indicator to redirect to User Wizard. 
			public string ORIGINAL_TIMEZONE_ID         { get; set; }
			// 11/21/2014 Paul.  Add User Picture. 
			public string PICTURE                      { get; set; }
			public string EXCHANGE_ALIAS               { get; set; }
			// 01/22/2021 Paul.  Access may require the Exchange Email. 
			public string EXCHANGE_EMAIL               { get; set; }
			// 12/01/2014 Paul.  Add SignalR fields. 
			public string USER_EXTENSION               { get; set; }
			public string USER_FULL_NAME               { get; set; }
			public string USER_PHONE_WORK              { get; set; }
			public string USER_SMS_OPT_IN              { get; set; }
			public string USER_PHONE_MOBILE            { get; set; }
			public string USER_TWITTER_TRACKS          { get; set; }
			public string USER_CHAT_CHANNELS           { get; set; }
			// 09/17/2020 Paul.  Add PhoneBurner SignalR support. 
			public string PHONEBURNER_TOKEN_EXPIRES_AT { get; set; }
			// 02/26/2016 Paul.  Use values from C# NumberFormatInfo. 
			public string USER_CurrencyDecimalDigits   { get; set; }
			public string USER_CurrencyDecimalSeparator{ get; set; }
			public string USER_CurrencyGroupSeparator  { get; set; }
			public string USER_CurrencyGroupSizes      { get; set; }
			public string USER_CurrencyNegativePattern { get; set; }
			public string USER_CurrencyPositivePattern { get; set; }
			public string USER_CurrencySymbol          { get; set; }
			// 01/22/2021 Paul.  Customizations may be based on the PRIMARY_ROLE_ID and not the name. 
			public string PRIMARY_ROLE_ID              { get; set; }
			// 05/05/2016 Paul.  The User Primary Role is used with role-based views. 
			public string PRIMARY_ROLE_NAME            { get; set; }
			// 03/02/2019 Paul.  We need to know if they are an admin or admin delegate. 
			public bool   IS_ADMIN                     { get; set; }
			public bool   IS_ADMIN_DELEGATE            { get; set; }
			// 05/07/2019 Paul.  Maintain SearchView state. 
			public bool   SAVE_QUERY                   { get; set; }
			// 12/16/2019 Paul.  Menu needs to know if logout should be displayed. 
			public string AUTHENTICATION               { get; set; }
			// 03/29/2021 Paul.  Allow display of impersonation state. 
			public bool   USER_IMPERSONATION            { get; set; }
		}

		public UserProfile GetUserProfile()
		{
			UserProfile profile = null;
			if ( Security.IsAuthenticated() )
			{
				profile = new UserProfile();
				profile.USER_ID          = Security.USER_ID  ;
				// 07/15/2021 Paul.  React Client needs to access the ASP.NET_SessionId. 
				profile.USER_SESSION     = Security.USER_SESSION;
				profile.USER_NAME        = Security.USER_NAME;
				profile.FULL_NAME        = Security.FULL_NAME;
				profile.TEAM_ID          = Security.TEAM_ID  ;
				profile.TEAM_NAME        = Security.TEAM_NAME;
				profile.USER_LANG        = Sql.ToString(Session["USER_SETTINGS/CULTURE"   ]);
				profile.USER_DATE_FORMAT = Sql.ToString(Session["USER_SETTINGS/DATEFORMAT"]);
				profile.USER_TIME_FORMAT = Sql.ToString(Session["USER_SETTINGS/TIMEFORMAT"]);
				// 04/23/2013 Paul.  The HTML5 Offline Client now supports Atlantic theme. 
				profile.USER_THEME       = Sql.ToString(Session["USER_SETTINGS/THEME"     ]);
				profile.USER_CURRENCY_ID = Sql.ToString(Session["USER_SETTINGS/CURRENCY"  ]);
				profile.USER_TIMEZONE_ID = Sql.ToString(Session["USER_SETTINGS/TIMEZONE"  ]);
				// 10/28/2021 Paul.  This is our indicator to redirect to User Wizard. 
				profile.ORIGINAL_TIMEZONE_ID = Sql.ToString(Session["USER_SETTINGS/TIMEZONE/ORIGINAL"  ]);
				// 11/21/2014 Paul.  Add User Picture. 
				profile.PICTURE          = Security.PICTURE  ;
				profile.EXCHANGE_ALIAS   = Sql.ToString(Session["EXCHANGE_ALIAS"          ]);
				// 01/22/2021 Paul.  Access may require the Exchange Email. 
				profile.EXCHANGE_EMAIL   = Sql.ToString(Session["EXCHANGE_EMAIL"          ]);
				// 12/01/2014 Paul.  Add SignalR fields. 
				profile.USER_EXTENSION      = Sql.ToString(Session["EXTENSION"   ]);
				profile.USER_FULL_NAME      = Sql.ToString(Session["FULL_NAME"   ]);
				profile.USER_PHONE_WORK     = Sql.ToString(Session["PHONE_WORK"  ]);
				profile.USER_SMS_OPT_IN     = Sql.ToString(Session["SMS_OPT_IN"  ]);
				profile.USER_PHONE_MOBILE   = Sql.ToString(Session["PHONE_MOBILE"]);
				profile.USER_TWITTER_TRACKS = this.MyTwitterTracks();
				profile.USER_CHAT_CHANNELS  = this.MyChatChannels();
				// 09/17/2020 Paul.  Add PhoneBurner SignalR support. 
				DateTime dtOAUTH_EXPIRES_AT = this.GetOAuthTokenExpiresAt("PhoneBurner", Security.USER_ID);
				profile.PHONEBURNER_TOKEN_EXPIRES_AT = (dtOAUTH_EXPIRES_AT != DateTime.MinValue ? profile.PHONEBURNER_TOKEN_EXPIRES_AT = RestUtil.ToJsonDate(dtOAUTH_EXPIRES_AT) : String.Empty);
				
				// 02/26/2016 Paul.  Use values from C# NumberFormatInfo. 
				// 09/02/2020 Paul.  We were getting arabic on the react client when using CurrentCulture.  
				CultureInfo culture = CultureInfo.CreateSpecificCulture(profile.USER_LANG);
				// 11/02/2020 Paul.  Use the currency specify in the profile, not the default language currency. 
				Guid gCURRENCY_ID = Sql.ToGuid(Session["USER_SETTINGS/CURRENCY"]);
				Currency C10n = Currency.CreateCurrency(gCURRENCY_ID);
				profile.USER_CurrencyDecimalDigits    = culture.NumberFormat.CurrencyDecimalDigits   .ToString();
				profile.USER_CurrencyDecimalSeparator = culture.NumberFormat.CurrencyDecimalSeparator;
				profile.USER_CurrencyGroupSeparator   = culture.NumberFormat.CurrencyGroupSeparator  ;
				profile.USER_CurrencyGroupSizes       = culture.NumberFormat.CurrencyGroupSizes[0]   .ToString();
				profile.USER_CurrencyNegativePattern  = culture.NumberFormat.CurrencyNegativePattern .ToString();
				profile.USER_CurrencyPositivePattern  = culture.NumberFormat.CurrencyPositivePattern .ToString();
				profile.USER_CurrencySymbol           = C10n.SYMBOL;
				
				// 01/22/2021 Paul.  Customizations may be based on the PRIMARY_ROLE_ID and not the name. 
				profile.PRIMARY_ROLE_ID     = Sql.ToString(Session["PRIMARY_ROLE_ID"  ]);
				// 05/05/2016 Paul.  The User Primary Role is used with role-based views. 
				profile.PRIMARY_ROLE_NAME   = Sql.ToString(Session["PRIMARY_ROLE_NAME"]);
				// 03/02/2019 Paul.  We need to know if they are an admin or admin delegate. 
				profile.IS_ADMIN          = Security.IS_ADMIN         ;
				profile.IS_ADMIN_DELEGATE = Security.IS_ADMIN_DELEGATE;
				// 05/07/2019 Paul.  Maintain SearchView state. 
				profile.SAVE_QUERY        = Sql.ToBoolean(Session["USER_SETTINGS/SAVE_QUERY"]);
				// 03/29/2021 Paul.  Allow display of impersonation state. 
				profile.USER_IMPERSONATION = Security.IsImpersonating();
				
				// 12/16/2019 Paul.  Menu needs to know if logout should be displayed. 
				string sAUTHENTICATION = "CRM";
				bool bADFS_SINGLE_SIGN_ON  = Sql.ToBoolean(Application["CONFIG.ADFS.SingleSignOn.Enabled" ]);
				bool bAZURE_SINGLE_SIGN_ON = Sql.ToBoolean(Application["CONFIG.Azure.SingleSignOn.Enabled"]);
				// 11/18/2019 Paul.  Include Authentication method. 
				if ( bADFS_SINGLE_SIGN_ON || bAZURE_SINGLE_SIGN_ON )
				{
					sAUTHENTICATION = "SingleSignOn";
				}
				else if ( Security.IsWindowsAuthentication() )
				{
					sAUTHENTICATION = "Windows";
				}
				profile.AUTHENTICATION = sAUTHENTICATION;
				return profile;
			}
			return null;
		}

		public DateTime GetOAuthTokenExpiresAt(string sSERVICE_NAME, Guid gUSER_ID)
		{
			DateTime dtOAUTH_EXPIRES_AT = DateTime.MinValue;
			if ( Application["CONFIG." + sSERVICE_NAME + "." + gUSER_ID.ToString() + ".OAuthAccessToken"] == null || Application["CONFIG." + sSERVICE_NAME + "." + gUSER_ID.ToString() + ".OAuthExpiresAt"] == null )
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					string sSQL = String.Empty;
					sSQL = "select TOKEN                               " + ControlChars.CrLf
					     + "     , TOKEN_EXPIRES_AT                    " + ControlChars.CrLf
					     + "     , REFRESH_TOKEN                       " + ControlChars.CrLf
					     + "  from vwOAUTH_TOKENS                      " + ControlChars.CrLf
					     + " where NAME             = @NAME            " + ControlChars.CrLf
					     + "   and ASSIGNED_USER_ID = @ASSIGNED_USER_ID" + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@NAME"            , sSERVICE_NAME);
						Sql.AddParameter(cmd, "@ASSIGNED_USER_ID", gUSER_ID);
						using ( IDataReader rdr = cmd.ExecuteReader() )
						{
							if ( rdr.Read() )
							{
								string sOAUTH_ACCESS_TOKEN  = Sql.ToString  (rdr["TOKEN"           ]);
								string sOAUTH_REFRESH_TOKEN = Sql.ToString  (rdr["REFRESH_TOKEN"   ]);
								dtOAUTH_EXPIRES_AT = Sql.ToDateTime(rdr["TOKEN_EXPIRES_AT"]);
								if ( dtOAUTH_EXPIRES_AT > DateTime.Now )
								{
									Application["CONFIG." + sSERVICE_NAME + "." + Security.USER_ID.ToString() + ".OAuthAccessToken" ] = sOAUTH_ACCESS_TOKEN ;
									Application["CONFIG." + sSERVICE_NAME + "." + Security.USER_ID.ToString() + ".OAuthRefreshToken"] = sOAUTH_REFRESH_TOKEN;
									Application["CONFIG." + sSERVICE_NAME + "." + Security.USER_ID.ToString() + ".OAuthExpiresAt"   ] = dtOAUTH_EXPIRES_AT.ToShortDateString() + " " + dtOAUTH_EXPIRES_AT.ToShortTimeString();
								}
							}
						}
					}
				}
			}
			else
			{
				dtOAUTH_EXPIRES_AT = Sql.ToDateTime(Application["CONFIG." + sSERVICE_NAME + "." + Security.USER_ID.ToString() + ".OAuthExpiresAt"]);
			}
			return dtOAUTH_EXPIRES_AT;
		}

		// 07/21/2019 Paul.  We need UserAccess control for buttons. 
		public Dictionary<string, object> GetUserAccess(List<string> lstMODULES)
		{
			Dictionary<string, object> objs = Session["vwACL_ACCESS_ByUser.ReactClient"] as Dictionary<string, object>;
			if ( objs == null )
			{
				objs = new Dictionary<string, object>();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL = String.Empty;
							sSQL = "select MODULE_NAME          " + ControlChars.CrLf
							     + "     , ACLACCESS_ADMIN      " + ControlChars.CrLf
							     + "     , ACLACCESS_ACCESS     " + ControlChars.CrLf
							     + "     , ACLACCESS_VIEW       " + ControlChars.CrLf
							     + "     , ACLACCESS_LIST       " + ControlChars.CrLf
							     + "     , ACLACCESS_EDIT       " + ControlChars.CrLf
							     + "     , ACLACCESS_DELETE     " + ControlChars.CrLf
							     + "     , ACLACCESS_IMPORT     " + ControlChars.CrLf
							     + "     , ACLACCESS_EXPORT     " + ControlChars.CrLf
							     + "     , ACLACCESS_ARCHIVE    " + ControlChars.CrLf
							     + "     , IS_ADMIN             " + ControlChars.CrLf
							     + "  from vwACL_ACCESS_ByUser  " + ControlChars.CrLf
							     + " where USER_ID = @USER_ID   " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@USER_ID", Security.USER_ID);
								Sql.AppendParameter(cmd, lstMODULES.ToArray(), "MODULE_NAME");
								
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dt = new DataTable() )
									{
										da.Fill(dt);
										foreach ( DataRow row in dt.Rows )
										{
											string sMODULE_NAME = Sql.ToString (row["MODULE_NAME"      ]);
											int    admin        = Sql.ToInteger(row["ACLACCESS_ADMIN"  ]);
											int    access       = Sql.ToInteger(row["ACLACCESS_ACCESS" ]);
											int    view         = Sql.ToInteger(row["ACLACCESS_VIEW"   ]);
											int    list         = Sql.ToInteger(row["ACLACCESS_LIST"   ]);
											int    edit         = Sql.ToInteger(row["ACLACCESS_EDIT"   ]);
											int    delete       = Sql.ToInteger(row["ACLACCESS_DELETE" ]);
											int    import       = Sql.ToInteger(row["ACLACCESS_IMPORT" ]);
											int    export       = Sql.ToInteger(row["ACLACCESS_EXPORT" ]);
											int    archive      = Sql.ToInteger(row["ACLACCESS_ARCHIVE"]);

											Dictionary<string, int> module = new Dictionary<string, int>();
											module["admin"  ] = admin  ;
											module["access" ] = access ;
											module["view"   ] = view   ;
											module["list"   ] = list   ;
											module["edit"   ] = edit   ;
											module["delete" ] = delete ;
											module["import" ] = import ;
											module["export" ] = export ;
											module["archive"] = archive;
											objs.Add(sMODULE_NAME, module);
										}
										Session["vwACL_ACCESS_ByUser.ReactClient"] = objs;
									}
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					// 04/28/2019 Paul.  This error is not critical, so we can just log and ignore. 
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return objs;
		}

		public Dictionary<string, object> GetModuleAccess(List<string> lstMODULES)
		{
			Dictionary<string, object> objs = Cache.Get("vwACL_ACCESS_ByModule.ReactClient") as Dictionary<string, object>;
			if ( objs == null )
			{
				objs = new Dictionary<string, object>();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL = String.Empty;
							sSQL = "select MODULE_NAME          " + ControlChars.CrLf
							     + "     , ACLACCESS_ADMIN      " + ControlChars.CrLf
							     + "     , ACLACCESS_ACCESS     " + ControlChars.CrLf
							     + "     , ACLACCESS_VIEW       " + ControlChars.CrLf
							     + "     , ACLACCESS_LIST       " + ControlChars.CrLf
							     + "     , ACLACCESS_EDIT       " + ControlChars.CrLf
							     + "     , ACLACCESS_DELETE     " + ControlChars.CrLf
							     + "     , ACLACCESS_IMPORT     " + ControlChars.CrLf
							     + "     , ACLACCESS_EXPORT     " + ControlChars.CrLf
							     + "     , ACLACCESS_ARCHIVE    " + ControlChars.CrLf
							     + "     , IS_ADMIN             " + ControlChars.CrLf
							     + "  from vwACL_ACCESS_ByModule" + ControlChars.CrLf
							     + " where 1 = 1                " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AppendParameter(cmd, lstMODULES.ToArray(), "MODULE_NAME");
								cmd.CommandText += " order by MODULE_NAME" + ControlChars.CrLf;
								
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dt = new DataTable() )
									{
										da.Fill(dt);
										foreach ( DataRow row in dt.Rows )
										{
											string sMODULE_NAME = Sql.ToString (row["MODULE_NAME"      ]);
											int    admin        = Sql.ToInteger(row["ACLACCESS_ADMIN"  ]);
											int    access       = Sql.ToInteger(row["ACLACCESS_ACCESS" ]);
											int    view         = Sql.ToInteger(row["ACLACCESS_VIEW"   ]);
											int    list         = Sql.ToInteger(row["ACLACCESS_LIST"   ]);
											int    edit         = Sql.ToInteger(row["ACLACCESS_EDIT"   ]);
											int    delete       = Sql.ToInteger(row["ACLACCESS_DELETE" ]);
											int    import       = Sql.ToInteger(row["ACLACCESS_IMPORT" ]);
											int    export       = Sql.ToInteger(row["ACLACCESS_EXPORT" ]);
											int    archive      = Sql.ToInteger(row["ACLACCESS_ARCHIVE"]);

											Dictionary<string, int> module = new Dictionary<string, int>();
											module["admin"  ] = admin  ;
											module["access" ] = access ;
											module["view"   ] = view   ;
											module["list"   ] = list   ;
											module["edit"   ] = edit   ;
											module["delete" ] = delete ;
											module["import" ] = import ;
											module["export" ] = export ;
											module["archive"] = archive;
											objs.Add(sMODULE_NAME, module);
										}
										Cache.Set("vwACL_ACCESS_ByModule.ReactClient", objs, DefaultCacheExpiration());
									}
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					// 04/28/2019 Paul.  This error is not critical, so we can just log and ignore. 
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return objs;
		}

		public Dictionary<string, object> GetUserFieldSecurity(List<string> lstMODULES)
		{
			Dictionary<string, object> objs = Session["vwACL_FIELD_ACCESS_ByUserAlias.ReactClient"] as Dictionary<string, object>;
			// 07/22/2019 Paul.  Field level security may not be enabled. 
			if ( objs == null )
			{
				objs = new Dictionary<string, object>();
				try
				{
					if ( SplendidInit.bEnableACLFieldSecurity )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL = String.Empty;
							sSQL = "select MODULE_NAME                   " + ControlChars.CrLf
							     + "     , FIELD_NAME                    " + ControlChars.CrLf
							     + "     , ACLACCESS                     " + ControlChars.CrLf
							     + "  from vwACL_FIELD_ACCESS_ByUserAlias" + ControlChars.CrLf
							     + " where USER_ID = @USER_ID            " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@USER_ID", Security.USER_ID);
								Sql.AppendParameter(cmd, lstMODULES.ToArray(), "MODULE_NAME");
								cmd.CommandText +=" order by MODULE_NAME, FIELD_NAME    " + ControlChars.CrLf;
								
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dt = new DataTable() )
									{
										da.Fill(dt);
										string sLAST_MODULE = String.Empty;
										Dictionary<string, int> module = null;
										foreach ( DataRow row in dt.Rows )
										{
											string sMODULE_NAME = Sql.ToString (row["MODULE_NAME"]);
											string sFIELD_NAME  = Sql.ToString (row["FIELD_NAME" ]);
											int    nACLACCESS   = Sql.ToInteger(row["ACLACCESS"  ]);
											if ( module == null || sLAST_MODULE != sMODULE_NAME )
											{
												module = new Dictionary<string, int>();
												objs.Add(sMODULE_NAME, module);
												sLAST_MODULE = sMODULE_NAME;
											}
											module[sFIELD_NAME] = nACLACCESS;
										}
										Session["vwACL_FIELD_ACCESS_ByUserAlias.ReactClient"] = objs;
									}
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					// 04/28/2019 Paul.  This error is not critical, so we can just log and ignore. 
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return objs;
		}

		// 01/22/2021 Paul.  Some customizations may be dependent on role name. 
		public List<Dictionary<string, object>> GetUserACLRoles()
		{
			List<Dictionary<string, object>> objs = Session["vwACL_ROLES_USERS.ReactClient"] as List<Dictionary<string, object>>;
			if ( objs == null )
			{
				objs = new List<Dictionary<string, object>>();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL = String.Empty;
							sSQL = "select ROLE_ID           " + ControlChars.CrLf
							     + "     , ROLE_NAME         " + ControlChars.CrLf
							     + "  from vwACL_ROLES_USERS " + ControlChars.CrLf
							     + " where USER_ID = @USER_ID" + ControlChars.CrLf
							     + " order by ROLE_NAME      " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@USER_ID", Security.USER_ID);
								
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dt = new DataTable() )
									{
										da.Fill(dt);
										foreach ( DataRow row in dt.Rows )
										{
											Dictionary<string, object> drow = new Dictionary<string, object>();
											for ( int j = 0; j < dt.Columns.Count; j++ )
											{
												drow.Add(dt.Columns[j].ColumnName, row[j]);
											}
											objs.Add(drow);
										}
										Session["vwACL_ROLES_USERS.ReactClient"] = objs;
									}
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					// 04/28/2019 Paul.  This error is not critical, so we can just log and ignore. 
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return objs;
		}

		// 05/01/2020 Paul.  Cache EmailTemplates for use in React Client. 
		public void ClearEmailTemplates()
		{
			Cache.Remove("vwEMAIL_TEMPLATES_List");
		}

		// 05/01/2020 Paul.  Cache EmailTemplates for use in React Client. 
		public DataTable EmailTemplates()
		{
			DataTable dt = Cache.Get("vwEMAIL_TEMPLATES_List") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select ID                    " + ControlChars.CrLf
						     + "     , NAME                  " + ControlChars.CrLf
						     + "  from vwEMAIL_TEMPLATES_List" + ControlChars.CrLf
						     + " order by NAME desc          " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Set("vwEMAIL_TEMPLATES_List", dt, DefaultCacheExpiration());
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		#endregion
	}

}

