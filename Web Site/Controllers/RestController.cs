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
using System.Data;
using System.Data.Common;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Threading.Tasks;

using SplendidCRM;

namespace SplendidWebApi.Controllers
{
	// 01/25/2022 Paul.  We are now using the Identity to take advantage of [Authorize] attribute. 
	[Authorize]
	[SplendidSessionAuthorize]
	[ApiController]
	//[Route("[controller]")]
	[Route("[controller].svc")]
	//[Route("Rest.svc")]
	public class RestController : ControllerBase
	{
		private IWebHostEnvironment  hostingEnvironment ;
		private IMemoryCache         memoryCache        ;
		private SplendidCRM.DbProviderFactories  DbProviderFactories = new SplendidCRM.DbProviderFactories();
		private HttpApplicationState Application        = new HttpApplicationState();
		private HttpSessionState     Session            ;
		private Security             Security           ;
		private Sql                  Sql                ;
		private L10N                 L10n               ;
		private Currency             Currency           = new Currency();
		private SplendidCRM.TimeZone T10n               = new SplendidCRM.TimeZone();
		private SqlProcs             SqlProcs           ;
		private SplendidError        SplendidError      ;
		private SplendidCache        SplendidCache      ;
		private RestUtil             RestUtil           ;
		private SplendidDynamic      SplendidDynamic    ;
		private SplendidInit         SplendidInit       ;
		private SplendidCRM.Crm.Modules          Modules          ;
		private SplendidCRM.Crm.Config           Config           = new SplendidCRM.Crm.Config();
		private SplendidCRM.Crm.Password         CrmPassword      = new SplendidCRM.Crm.Password();
		private ModuleUtils.Audit                Audit            ;
		private ModuleUtils.AuditPersonalInfo    AuditPersonalInfo;
		private ActiveDirectory                  ActiveDirectory  ;
		private ModuleUtils.Login                ModuleUtilsLogin ;

		public RestController(IWebHostEnvironment hostingEnvironment, IMemoryCache memoryCache, HttpSessionState Session, Security Security, Sql Sql, SqlProcs SqlProcs, SplendidError SplendidError, SplendidCache SplendidCache, Utils Utils, RestUtil RestUtil, SplendidDynamic SplendidDynamic, SplendidInit SplendidInit, SplendidCRM.Crm.Modules Modules, ModuleUtils.Audit Audit, ModuleUtils.AuditPersonalInfo AuditPersonalInfo, ActiveDirectory ActiveDirectory, ArchiveUtils ArchiveUtils, ModuleUtils.Login Login)
		{
			this.hostingEnvironment  = hostingEnvironment ;
			this.memoryCache         = memoryCache        ;
			this.Session             = Session            ;
			this.Security            = Security           ;
			this.L10n                = new L10N(Sql.ToString(Session["USER_SETTINGS/CULTURE"]));
			this.Sql                 = Sql                ;
			this.SqlProcs            = SqlProcs           ;
			this.SplendidError       = SplendidError      ;
			this.SplendidCache       = SplendidCache      ;
			this.RestUtil            = RestUtil           ;
			this.SplendidDynamic     = SplendidDynamic    ;
			this.SplendidInit        = SplendidInit       ;
			this.Modules             = Modules            ;
			this.Audit               = Audit              ;
			this.AuditPersonalInfo   = AuditPersonalInfo  ;
			this.ActiveDirectory     = ActiveDirectory    ;
			this.ModuleUtilsLogin    = Login              ;
		}

		#region Scalar functions
		[DotNetLegacyData]
		[HttpPost("[action]")]
		public string Version()
		{
			return Sql.ToString(Application["SplendidVersion"]);
		}

		[AllowAnonymous]
		[DotNetLegacyData]
		[HttpPost("[action]")]
		public string Edition()
		{
			return Sql.ToString(Application["CONFIG.service_level"]);
		}

		[AllowAnonymous]
		[DotNetLegacyData]
		[HttpPost("[action]")]
		public DateTime UtcTime()
		{
			return DateTime.UtcNow;
		}

		[AllowAnonymous]
		[DotNetLegacyData]
		[HttpPost("[action]")]
		public bool IsAuthenticated()
		{
			return Security.IsAuthenticated();
		}

		[DotNetLegacyData]
		[HttpPost("[action]")]
		public Guid GetUserID()
		{
			Guid gUSER_ID = Guid.Empty;
			if ( Security.IsAuthenticated() )
				gUSER_ID = Security.USER_ID;
			return gUSER_ID;
		}

		// 07/15/2021 Paul.  React Client needs to access the ASP.NET_SessionId. 
		[DotNetLegacyData]
		[HttpPost("[action]")]
		public string GetUserSession()
		{
			string sUSER_SESSION = String.Empty;
			if ( Security.IsAuthenticated() && Session != null )
				sUSER_SESSION = Security.USER_SESSION;
			return sUSER_SESSION;
		}

		[DotNetLegacyData]
		[HttpPost("[action]")]
		public string GetUserName()
		{
			string sUSER_NAME = String.Empty;
			if ( Security.IsAuthenticated() )
				sUSER_NAME = Security.USER_NAME;
			return sUSER_NAME;
		}

		[DotNetLegacyData]
		[HttpPost("[action]")]
		public Guid GetTeamID()
		{
			Guid gTEAM_ID = Guid.Empty;
			if ( Security.IsAuthenticated() )
				gTEAM_ID = Security.TEAM_ID;
			return gTEAM_ID;
		}

		[DotNetLegacyData]
		[HttpPost("[action]")]
		public string GetTeamName()
		{
			string sTEAM_NAME = String.Empty;
			if ( Security.IsAuthenticated() )
				sTEAM_NAME = Security.TEAM_NAME;
			return sTEAM_NAME;
		}

		[DotNetLegacyData]
		[HttpPost("[action]")]
		public string GetUserLanguage()
		{
			string sLANG = "en-US";
			if ( Security.IsAuthenticated() )
				sLANG = Sql.ToString(Session["USER_SETTINGS/CULTURE"]);
			return sLANG;
		}

		[DotNetLegacyData]
		[HttpPost("[action]")]
		public SplendidCache.UserProfile GetUserProfile()
		{
			if ( Security.IsAuthenticated() )
			{
				// 05/27/2019 Paul.  Move GetUserProfile to cache for React client. 
				SplendidCache.UserProfile profile = SplendidCache.GetUserProfile();
				return profile;
			}
			else
			{
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}
		}

		[HttpGet("[action]")]
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public Dictionary<string, object> GetMyUserProfile()
		{
			StringBuilder sbDumpSQL = new StringBuilder();
			// 12/16/2019 Paul.  Moved GetTable to ~/_code/RestUtil.cs
			DataTable dt = new DataTable();
			SplendidCRM.DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				con.Open();
				string sSQL ;
				sSQL = "select *           " + ControlChars.CrLf
				     + "  from vwUSERS_Edit" + ControlChars.CrLf
				     + " where ID = @ID    " + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Sql.AddParameter(cmd, "@ID", Security.USER_ID);
					sbDumpSQL.Append(Sql.ExpandParameters(cmd));
					using ( DbDataAdapter da = dbf.CreateDataAdapter() )
					{
						((IDbDataAdapter)da).SelectCommand = cmd;
						da.Fill(dt);
						if ( dt.Rows.Count > 0 )
						{
							DataRow row = dt.Rows[0];
							if ( !Sql.IsEmptyString(row["MAIL_SMTPPASS"]) )
							{
								row["MAIL_SMTPPASS"] = Sql.sEMPTY_PASSWORD;
							}
						}
					}
				}
			}
			if ( dt == null || dt.Rows.Count == 0 )
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			
			string sBaseURI = Request.Scheme + "://" + Request.Host.Host + Request.Path.Value;
			// 04/01/2020 Paul.  Move json utils to RestUtil. 
			Dictionary<string, object> dict = RestUtil.ToJson(sBaseURI, "Users", dt.Rows[0], T10n);
			
			// 10/26/2019 Paul.  Return the SQL to the React Client. 
			if ( Sql.ToBoolean(Application["CONFIG.show_sql"]) )
			{
				dict.Add("__sql", sbDumpSQL.ToString());
			}
			return dict;
		}

		// 06/24/2019 Paul.  Separate out so that the settings can be returned in GetReactLoginState. 
		private Dictionary<string, object> GetSingleSignOnSettings()
		{
			bool bADFS_SINGLE_SIGN_ON  = Sql.ToBoolean(Application["CONFIG.ADFS.SingleSignOn.Enabled" ]);
			bool bAZURE_SINGLE_SIGN_ON = Sql.ToBoolean(Application["CONFIG.Azure.SingleSignOn.Enabled"]);

			Dictionary<string, object> results = new Dictionary<string, object>();
			if ( bADFS_SINGLE_SIGN_ON )
			{
				//Dictionary<string, object> endpoints = new Dictionary<string, object>();
				//endpoints.Add(Sql.ToString(Application["CONFIG.ADFS.SingleSignOn.Realm"]), Sql.ToString(Application["CONFIG.ADFS.SingleSignOn.Realm"]));
				// https://technet.microsoft.com/en-us/windows-server-docs/identity/ad-fs/development/single-page-application-with-ad-fs
				results.Add("instance"         , Sql.ToString(Application["CONFIG.ADFS.SingleSignOn.Authority"        ]));
				results.Add("tenant"           , "adfs");
				// 04/30/2017 Paul.  ADFS 4.0 on Windows Server 2016 is required for ADAL.js to work. 
				results.Add("clientId"         , Sql.ToString(Application["CONFIG.ADFS.SingleSignOn.ClientId"         ]));
				// 05/01/2017 Paul.  Make sure not to validate instance AuthenticationContext(authority, false). 
				// 05/01/2017 Paul.  Native Application for Web API. 
				results.Add("mobileId"         , Sql.ToString(Application["CONFIG.ADFS.SingleSignOn.MobileClientId"   ]));
				results.Add("mobileRedirectUrl", Sql.ToString(Application["CONFIG.ADFS.SingleSignOn.MobileRedirectUrl"]));
				// 12/24/2018 Paul.  If we are building the URL by hand in React Native, then we need the Realm. 
				Dictionary<string, object> endpoints = new Dictionary<string, object>();
				endpoints.Add(Sql.ToString(Application["CONFIG.ADFS.SingleSignOn.Realm"]), Sql.ToString(Application["CONFIG.ADFS.SingleSignOn.MobileClientId"]));
				results.Add("endpoints"        , endpoints);
			}
			else if ( bAZURE_SINGLE_SIGN_ON )
			{
				Dictionary<string, object> endpoints = new Dictionary<string, object>();
				endpoints.Add(Sql.ToString(Application["CONFIG.Azure.SingleSignOn.Realm"]), Sql.ToString(Application["CONFIG.Azure.SingleSignOn.AadClientId"]));
				// https://hjnilsson.com/2016/07/20/authenticated-azure-cors-request-with-active-directory-and-adal-js/
				results.Add("instance"         , "https://login.microsoftonline.com/");
				results.Add("tenant"           , Sql.ToString(Application["CONFIG.Azure.SingleSignOn.AadTenantDomain"  ]));
				results.Add("clientId"         , Sql.ToString(Application["CONFIG.Azure.SingleSignOn.AadClientId"      ]));
				// 05/01/2017 Paul.  Will need to add permissions for Web API app above. 
				results.Add("mobileId"         , Sql.ToString(Application["CONFIG.Azure.SingleSignOn.MobileClientId"   ]));
				results.Add("mobileRedirectUrl", Sql.ToString(Application["CONFIG.Azure.SingleSignOn.MobileRedirectUrl"]));
				results.Add("endpoints"        , endpoints);
			}
			return results;
		}

		[AllowAnonymous]
		[DotNetLegacyData]
		[HttpGet("[action]")]
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public Dictionary<string, object> SingleSignOnSettings()
		{
			// 06/24/2019 Paul.  Separate out so that the settings can be returned in GetReactLoginState. 
			Dictionary<string, object> results = GetSingleSignOnSettings();
			return results;
		}

		[DotNetLegacyData]
		[HttpGet("[action]")]
		public bool ArchiveViewExists([FromQuery] string VIEW_NAME)
		{
			bool bExists = false;
			try
			{
				bExists = SplendidCache.ArchiveViewExists(VIEW_NAME);
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				throw(new Exception(ex.Message));
			}
			return bExists;
		}
		#endregion

		public class LoginParameters
		{
			public string UserName     { get; set; }
			public string Password     { get; set; }
			public string Version      { get; set; }
			public bool   MobileClient { get; set; }
		}

		#region Login
		[AllowAnonymous]
		[DotNetLegacyData]
		[HttpPost("[action]")]
		// 05/02/2017 Paul.  Need a separate flag for the mobile client. 
		public async Task<Guid> Login([FromBody] LoginParameters input)
		{
			// 11/05/2018 Paul.  Protect against null inputs. 
			string sUSER_NAME   = Sql.ToString (input.UserName    );
			string sPASSWORD    = Sql.ToString (input.Password    );
			string sVERSION     = Sql.ToString (input.Version     );
			bool bMOBILE_CLIENT = Sql.ToBoolean(input.MobileClient);
			Guid gUSER_ID       = Guid.Empty;
			
			// 02/23/2011 Paul.  SYNC service should check for lockout. 
			if ( SplendidInit.LoginFailures(sUSER_NAME) >= CrmPassword.LoginLockoutCount() )
			{
				throw(new Exception(L10n.Term("Users.ERR_USER_LOCKED_OUT")));
			}
			// 04/16/2013 Paul.  Allow system to be restricted by IP Address. 
			// 08/19/2023 Paul.  RemoteIpAddress can be null. 
			if ( !Sql.IsEmptyString(HttpContext.Connection.RemoteIpAddress) && SplendidInit.InvalidIPAddress(HttpContext.Connection.RemoteIpAddress.ToString()) )
			{
				throw(new Exception(L10n.Term("Users.ERR_INVALID_IP_ADDRESS")));
			}

			// 01/09/2017 Paul.  Add support for ADFS Single-Sign-On.  Using WS-Federation Desktop authentication (username/password). 
			string sError = String.Empty;
			if ( Sql.ToBoolean(Application["CONFIG.ADFS.SingleSignOn.Enabled"]) )
			{
				// 05/02/2017 Paul.  Need a separate flag for the mobile client. 
				gUSER_ID = ActiveDirectory.FederationServicesValidateJwt(sPASSWORD, bMOBILE_CLIENT, ref sError);
				if ( !Sql.IsEmptyGuid(gUSER_ID) )
				{
					SplendidInit.LoginUser(gUSER_ID, "ASDF");
				}
			}
			else if ( Sql.ToBoolean(Application["CONFIG.Azure.SingleSignOn.Enabled"]) )
			{
				// 05/02/2017 Paul.  Need a separate flag for the mobile client. 
				gUSER_ID = ActiveDirectory.AzureValidateJwt(sPASSWORD, bMOBILE_CLIENT, ref sError);
				if ( !Sql.IsEmptyGuid(gUSER_ID) )
				{
					SplendidInit.LoginUser(gUSER_ID, "Azure AD");
				}
			}
			// 05/16/2020 Paul.  Allow Windows Authentication using same login method. 
			else if ( Security.IsWindowsAuthentication() )
			{
				string[] arrUserName = HttpContext.User.Identity.Name.Split('\\');
				// 11/09/2007 Paul.  The domain will not be provided when debugging anonymous. 
				string sUSER_DOMAIN = String.Empty;
				string sMACHINE     = String.Empty;
				try
				{
					// 09/17/2009 Paul.  Azure does not support MachineName.  Just ignore the error. 
					sMACHINE = System.Environment.MachineName;
				}
				catch
				{
				}
				if ( arrUserName.Length > 1 )
				{
					sUSER_DOMAIN = arrUserName[0];
					sUSER_NAME   = arrUserName[1];
				}
				else
				{
					// 12/15/2007 Paul.  Use environment variable as it is always available, where as the server object is not. 
					sUSER_DOMAIN = sMACHINE;
					sUSER_NAME   = arrUserName[0];
				}
				// 08/26/2015 Paul.  A customer was having trouble with trust relationships.  The IS_ADMIN test is not critical, so we can ignore the error and continue. 
				bool bIS_ADMIN = false;
				try
				{
					// 09/17/2009 Paul.  The machine or domain name may be empty, so protect against their use. 
					bIS_ADMIN = HttpContext.User.IsInRole("BUILTIN\\Administrators") 
					              || (!Sql.IsEmptyString(sUSER_DOMAIN) && HttpContext.User.IsInRole(sUSER_DOMAIN + "\\SplendidCRM Administrators"))
					              || (!Sql.IsEmptyString(sMACHINE    ) && HttpContext.User.IsInRole(sMACHINE     + "\\SplendidCRM Administrators"))
					              || (!Sql.IsEmptyString(sUSER_DOMAIN) && HttpContext.User.IsInRole(sUSER_DOMAIN + "\\Domain Admins"));
				}
				catch(Exception ex)
				{
					SplendidError.SystemMessage("Warning", new StackTrace(true).GetFrame(0), ex);
				}
				SplendidInit.LoginUser(sUSER_NAME, String.Empty, String.Empty, String.Empty, sUSER_DOMAIN, bIS_ADMIN);
				if ( !Security.IsAuthenticated() )
				{
					SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), "Windows Authentication failed for " + sUSER_NAME);
					throw(new Exception("Windows Authentication failed for " + sUSER_NAME));
				}
			}
			else
			{
				SplendidCRM.DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					string sSQL = String.Empty;
					sSQL = "select ID                    " + ControlChars.CrLf
					     + "  from vwUSERS_Login         " + ControlChars.CrLf
					     + " where USER_NAME = @USER_NAME" + ControlChars.CrLf
					     + "   and USER_HASH = @USER_HASH" + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						string sUSER_HASH = Security.HashPassword(sPASSWORD);
						// 12/25/2009 Paul.  Use lowercase username to match the primary authentication function. 
						Sql.AddParameter(cmd, "@USER_NAME", sUSER_NAME.ToLower());
						Sql.AddParameter(cmd, "@USER_HASH", sUSER_HASH);
						gUSER_ID = Sql.ToGuid(cmd.ExecuteScalar());
						if ( Sql.IsEmptyGuid(gUSER_ID) )
						{
							Guid gUSER_LOGIN_ID = Guid.Empty;
							string sUserAgent = Request.Headers["User-Agent"];
							SqlProcs.spUSERS_LOGINS_InsertOnly(ref gUSER_LOGIN_ID, Guid.Empty, sUSER_NAME, "Anonymous", "Failed", Session.Id, HttpContext.Connection.RemoteIpAddress.ToString(), Request.Host.Value, Request.Path.Value, hostingEnvironment.WebRootPath, sUserAgent);
							// 02/20/2011 Paul.  Log the failure so that we can lockout the user. 
							SplendidInit.LoginTracking(sUSER_NAME, false);
							SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), "SECURITY: failed attempted login for " + sUSER_NAME + " using REST API");
						}
						else
						{
							SplendidInit.LoginUser(gUSER_ID, "Anonymous");
						}
					}
				}
			}
			if ( Sql.IsEmptyGuid(gUSER_ID) )
			{
				SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), "Invalid username and/or password for " + sUSER_NAME);
				throw(new Exception("Invalid username and/or password for " + sUSER_NAME));
			}
			// 01/25/2022 Paul.  We are now using the Identity to take advantage of [Authorize] attribute. 
			List<Claim> claims = new List<Claim>();
			claims.Add(new Claim(ClaimTypes.NameIdentifier, gUSER_ID.ToString()));
			ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			ClaimsPrincipal principal = new ClaimsPrincipal(identity);
			await Microsoft.AspNetCore.Authentication.AuthenticationHttpContextExtensions.SignInAsync(HttpContext, CookieAuthenticationDefaults.AuthenticationScheme, principal);
			
			return gUSER_ID;
		}

		// 02/18/2020 Paul.  Allow React Client to forget password. 
		[AllowAnonymous]
		[DotNetLegacyData]
		[HttpPost("[action]")]
		public string ForgotPassword([FromQuery] string UserName, [FromQuery] string Email)
		{
			string sUSER_NAME   = Sql.ToString(UserName);
			string sEMAIL       = Sql.ToString(Email   );
			
			// 04/16/2013 Paul.  Allow system to be restricted by IP Address. 
			// 08/19/2023 Paul.  RemoteIpAddress can be null. 
			if ( !Sql.IsEmptyString(HttpContext.Connection.RemoteIpAddress) && SplendidInit.InvalidIPAddress(HttpContext.Connection.RemoteIpAddress.ToString()) )
			{
				throw(new Exception(L10n.Term("Users.ERR_INVALID_IP_ADDRESS")));
			}

			// 10/30/2021 Paul.  Move SendForgotPasswordNotice to ModuleUtils. 
			string sError = ModuleUtilsLogin.SendForgotPasswordNotice(sUSER_NAME, sEMAIL);
			return sError;
		}
		
		[HttpPost("[action]")]
		public void Logout()
		{
			try
			{
				Guid gUSER_LOGIN_ID = Security.USER_LOGIN_ID;
				if ( !Sql.IsEmptyGuid(gUSER_LOGIN_ID) )
					SqlProcs.spUSERS_LOGINS_Logout(gUSER_LOGIN_ID);
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
			}
			HttpContext.Session.Clear();
			// 12/23/2021 Paul.  We don't have a way to create a new session Id. 
			//HttpContext.Session.Abandon();
			//// 11/15/2014 Paul.  Prevent resuse of SessionID. 
			//// http://support.microsoft.com/kb/899918
			//HttpContext.Current.Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
			
			// 01/25/2022 Paul.  We are now using the Identity to take advantage of [Authorize] attribute. 
			Microsoft.AspNetCore.Authentication.AuthenticationHttpContextExtensions.SignOutAsync(HttpContext, CookieAuthenticationDefaults.AuthenticationScheme);
		}
		#endregion

		#region React State
		// 05/27/2019 Paul.  Separate call for the React client state. 
		[AllowAnonymous]
		[DotNetLegacyData]
		[HttpGet("[action]")]
		// https://docs.microsoft.com/en-us/aspnet/core/performance/caching/response?view=aspnetcore-6.0`
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public Dictionary<string, object> GetReactLoginState()
		{
			Dictionary<string, object> results = new Dictionary<string, object>();
			
			Dictionary<string, object> CONFIG = SplendidCache.GetLoginConfig();
			results.Add("CONFIG", CONFIG);
			
			Dictionary<string, object> TERMINOLOGY = SplendidCache.GetLoginTerminology();
			results.Add("TERMINOLOGY", TERMINOLOGY);
			
			// 12/10/2022 Paul.  Allow Login Terminology Lists to be customized. 
			List<string> lstLIST_NAME = new List<string>();
			Dictionary<string, object> TERMINOLOGY_LISTS = SplendidCache.GetLoginTerminologyLists(lstLIST_NAME, TERMINOLOGY);
			results.Add("TERMINOLOGY_LISTS", TERMINOLOGY_LISTS);

			// 06/24/2019 Paul.  Separate out so that the settings can be returned in GetReactLoginState. 
			Dictionary<string, object> objSingleSignOnSettings = GetSingleSignOnSettings();
			results.Add("SingleSignOnSettings", objSingleSignOnSettings);

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
			results.Add("AUTHENTICATION", sAUTHENTICATION);

			// 12/07/2022 Paul.  Allow the LoginView to be customized. 
			List<string> lstMODULES = new List<string>();
			lstMODULES.Add("Home");
			string sModuleList = String.Join(",", lstMODULES.ToArray());
			Dictionary<string, object> objs = memoryCache.Get("ReactCustomViews." + sModuleList) as Dictionary<string, object>;
#if DEBUG
			objs = null;
#endif
			if ( objs == null )
			{
				objs = new Dictionary<string, object>();
				SplendidCache.GetAllReactCustomViews(objs, lstMODULES, "~/React/src/CustomViewsJS", false, true);
				memoryCache.Set("ReactCustomViews." + sModuleList, objs, SplendidCache.DefaultCacheExpiration());
			}
			results.Add("REACT_CUSTOM_VIEWS", objs);
			return results;
		}

		[DotNetLegacyData]
		[HttpGet("[action]")]
		// https://docs.microsoft.com/en-us/aspnet/core/performance/caching/response?view=aspnetcore-6.0`
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public Dictionary<string, object> GetReactState()
		{
			Dictionary<string, object> results = new Dictionary<string, object>();
			
			// 03/02/2019 Paul.  Functions are now static and take modules list input so that they can be used in the Admin API. 
			// 12/16/2019 Paul.  Moved GetTable to ~/_code/RestUtil.cs
			List<string> lstMODULES = RestUtil.AccessibleModules();
			
			// 02/18/2023 Paul.  Gather metrics.  Noticed odd slowless with SQL 2019. 
			DateTime dtStart = DateTime.Now;
			Dictionary<string, double> metrics = new Dictionary<string, double>();
			try
			{
				// 05/27/2019 Paul.  Move GetUserProfile to cache for React client. 
				SplendidCache.UserProfile profile = SplendidCache.GetUserProfile();
				results.Add("USER_PROFILE", profile);
				metrics.Add("USER_PROFILE", (DateTime.Now - dtStart).TotalSeconds);
				dtStart = DateTime.Now;
			
				// 12/23/2019 Paul.  Return the team tree as an object tree instead of XML. 
				results.Add("TEAM_TREE", SplendidCache.GetUserTeamTree());
				metrics.Add("TEAM_TREE", (DateTime.Now - dtStart).TotalSeconds);
				dtStart = DateTime.Now;
			
				// 07/21/2019 Paul.  We need UserAccess control for buttons. 
				Dictionary<string, object> MODULE_ACL_ACCESS = SplendidCache.GetModuleAccess(lstMODULES);
				results.Add("MODULE_ACL_ACCESS", MODULE_ACL_ACCESS);
				metrics.Add("MODULE_ACL_ACCESS", (DateTime.Now - dtStart).TotalSeconds);
				dtStart = DateTime.Now;
			
				Dictionary<string, object> ACL_ACCESS = SplendidCache.GetUserAccess(lstMODULES);
				results.Add("ACL_ACCESS", ACL_ACCESS);
				metrics.Add("ACL_ACCESS", (DateTime.Now - dtStart).TotalSeconds);
				dtStart = DateTime.Now;
			
				Dictionary<string, object> ACL_FIELD_ACCESS = SplendidCache.GetUserFieldSecurity(lstMODULES);
				results.Add("ACL_FIELD_ACCESS", ACL_FIELD_ACCESS);
				metrics.Add("ACL_FIELD_ACCESS", (DateTime.Now - dtStart).TotalSeconds);
				dtStart = DateTime.Now;
			
				// 01/22/2021 Paul.  Some customizations may be dependent on role name. 
				List<Dictionary<string, object>>  ACL_ROLES = SplendidCache.GetUserACLRoles();
				results.Add("ACL_ROLES", ACL_ROLES);
				metrics.Add("ACL_ROLES", (DateTime.Now - dtStart).TotalSeconds);
				dtStart = DateTime.Now;
			
				// 05/17/2019 Paul.  Return the modules so that we don't need a separate request for it later. 
				Dictionary<string, object> CONFIG = SplendidCache.GetAllConfig();
				// 06/10/2023 Paul.  Core app does not support classic UI. 
				CONFIG["disable_admin_classic"] = true;
				// 06/19/2023 Paul.  Separate implementation for SignalR on ASP.NET Core. 
				CONFIG["SignalR.Core"] = true;
				results.Add("CONFIG", CONFIG);
				metrics.Add("CONFIG", (DateTime.Now - dtStart).TotalSeconds);
				dtStart = DateTime.Now;
			
				Dictionary<string, object> MODULES = SplendidCache.GetAllModules(lstMODULES);
				results.Add("MODULES", MODULES);
				metrics.Add("MODULES", (DateTime.Now - dtStart).TotalSeconds);
				dtStart = DateTime.Now;
			
				Dictionary<string, object> MODULE_COLUMNS = SplendidCache.GetAllSearchColumns(lstMODULES);
				results.Add("MODULE_COLUMNS", MODULE_COLUMNS);
				metrics.Add("MODULE_COLUMNS", (DateTime.Now - dtStart).TotalSeconds);
				dtStart = DateTime.Now;
			
				// 05/26/2019 Paul.  Return Users and Teams in GetAllLayouts. 
				Dictionary<string, object> USERS = SplendidCache.GetAllUsers();
				results.Add("USERS", USERS);
				metrics.Add("USERS", (DateTime.Now - dtStart).TotalSeconds);
				dtStart = DateTime.Now;
			
				Dictionary<string, object> TEAMS = SplendidCache.GetAllTeams();
				results.Add("TEAMS", TEAMS);
				metrics.Add("TEAMS", (DateTime.Now - dtStart).TotalSeconds);
				dtStart = DateTime.Now;
			
				// 05/16/2019 Paul.  Return the tab menu so that we don't need a separate request for it later. 
				List<object> TAB_MENU = SplendidCache.GetAllTabMenus();
				results.Add("TAB_MENU", TAB_MENU);
				metrics.Add("TAB_MENU", (DateTime.Now - dtStart).TotalSeconds);
				dtStart = DateTime.Now;
			
				// 02/22/2021 Paul.  The React client needs a way to determine the default sort, besides NAME asc. 
				Dictionary<string, object> GRIDVIEWS = SplendidCache.GetAllGridViews(lstMODULES);
				results.Add("GRIDVIEWS", GRIDVIEWS);
				metrics.Add("GRIDVIEWS", (DateTime.Now - dtStart).TotalSeconds);
				dtStart = DateTime.Now;
			
				Dictionary<string, object> GRIDVIEWS_COLUMNS = SplendidCache.GetAllGridViewsColumns(lstMODULES);
				results.Add("GRIDVIEWS_COLUMNS", GRIDVIEWS_COLUMNS);
				metrics.Add("GRIDVIEWS_COLUMNS", (DateTime.Now - dtStart).TotalSeconds);
				dtStart = DateTime.Now;
			
				Dictionary<string, object> DETAILVIEWS_FIELDS = SplendidCache.GetAllDetailViewsFields(lstMODULES);
				results.Add("DETAILVIEWS_FIELDS", DETAILVIEWS_FIELDS);
				metrics.Add("DETAILVIEWS_FIELDS", (DateTime.Now - dtStart).TotalSeconds);
				dtStart = DateTime.Now;
			
				Dictionary<string, object> EDITVIEWS_FIELDS = SplendidCache.GetAllEditViewsFields(lstMODULES);
				results.Add("EDITVIEWS_FIELDS", EDITVIEWS_FIELDS);
				metrics.Add("EDITVIEWS_FIELDS", (DateTime.Now - dtStart).TotalSeconds);
				dtStart = DateTime.Now;
			
				Dictionary<string, object> DETAILVIEWS_RELATIONSHIPS = SplendidCache.GetAllDetailViewsRelationships(lstMODULES);
				results.Add("DETAILVIEWS_RELATIONSHIPS", DETAILVIEWS_RELATIONSHIPS);
				metrics.Add("DETAILVIEWS_RELATIONSHIPS", (DateTime.Now - dtStart).TotalSeconds);
				dtStart = DateTime.Now;
			
				Dictionary<string, object> EDITVIEWS_RELATIONSHIPS = SplendidCache.GetAllEditViewsRelationships(lstMODULES);
				results.Add("EDITVIEWS_RELATIONSHIPS", EDITVIEWS_RELATIONSHIPS);
				metrics.Add("EDITVIEWS_RELATIONSHIPS", (DateTime.Now - dtStart).TotalSeconds);
				dtStart = DateTime.Now;
			
				Dictionary<string, object> DYNAMIC_BUTTONS = SplendidCache.GetAllDynamicButtons(lstMODULES);
				results.Add("DYNAMIC_BUTTONS", DYNAMIC_BUTTONS);
				metrics.Add("DYNAMIC_BUTTONS", (DateTime.Now - dtStart).TotalSeconds);
				dtStart = DateTime.Now;
			
				// 08/15/2019 Paul.  Add support for menu shortcuts. 
				Dictionary<string, object> SHORTCUTS = SplendidCache.GetAllShortcuts(lstMODULES);
				results.Add("SHORTCUTS", SHORTCUTS);
				metrics.Add("SHORTCUTS", (DateTime.Now - dtStart).TotalSeconds);
				dtStart = DateTime.Now;
			
				// 03/26/2019 Paul.  Admin has more custom lists. 
				Dictionary<string, object> TERMINOLOGY_LISTS = SplendidCache.GetAllTerminologyLists(false);
				results.Add("TERMINOLOGY_LISTS", TERMINOLOGY_LISTS);
				metrics.Add("TERMINOLOGY_LISTS", (DateTime.Now - dtStart).TotalSeconds);
				dtStart = DateTime.Now;
			
				// 03/26/2019 Paul.  Admin has more custom lists. 
				Dictionary<string, object> TERMINOLOGY = SplendidCache.GetAllTerminology(lstMODULES, false);
				results.Add("TERMINOLOGY", TERMINOLOGY);
				metrics.Add("TERMINOLOGY", (DateTime.Now - dtStart).TotalSeconds);
				dtStart = DateTime.Now;
			
				// 07/01/2019 Paul.  The SubPanelsView needs to understand how to manage all relationships. 
				Dictionary<string, object> RELATIONSHIPS = SplendidCache.GetAllRelationships();
				results.Add("RELATIONSHIPS", RELATIONSHIPS);
				metrics.Add("RELATIONSHIPS", (DateTime.Now - dtStart).TotalSeconds);
				dtStart = DateTime.Now;
			
				// 09/12/2019 Paul.  User Profile needs the timezones and currencies. 
				Dictionary<string, object> TIMEZONES = SplendidCache.GetAllTimezones();
				results.Add("TIMEZONES", TIMEZONES);
				metrics.Add("TIMEZONES", (DateTime.Now - dtStart).TotalSeconds);
				dtStart = DateTime.Now;
			
				Dictionary<string, object> CURRENCIES = SplendidCache.GetAllCurrencies();
				results.Add("CURRENCIES", CURRENCIES);
				metrics.Add("CURRENCIES", (DateTime.Now - dtStart).TotalSeconds);
				dtStart = DateTime.Now;
			
				Dictionary<string, object> LANGUAGES = SplendidCache.GetAllLanguages();
				results.Add("LANGUAGES", LANGUAGES);
				metrics.Add("LANGUAGES", (DateTime.Now - dtStart).TotalSeconds);
				dtStart = DateTime.Now;
			
				Dictionary<string, object> LAST_VIEWED = SplendidCache.GetAllLastViewed();
				results.Add("LAST_VIEWED", LAST_VIEWED);
				metrics.Add("LAST_VIEWED", (DateTime.Now - dtStart).TotalSeconds);
				dtStart = DateTime.Now;
			
				Dictionary<string, object> SAVED_SEARCH = SplendidCache.GetAllSavedSearch(lstMODULES);
				results.Add("SAVED_SEARCH", SAVED_SEARCH);
				metrics.Add("SAVED_SEARCH", (DateTime.Now - dtStart).TotalSeconds);
				dtStart = DateTime.Now;
			
				// 05/24/2019 Paul.  Return Dashboard in GetAllLayouts. 
				Dictionary<string, object> DASHBOARDS = SplendidCache.GetAllDashboards();
				results.Add("DASHBOARDS", DASHBOARDS);
				metrics.Add("DASHBOARDS", (DateTime.Now - dtStart).TotalSeconds);
				dtStart = DateTime.Now;
			
				Dictionary<string, object> DASHBOARDS_PANELS = SplendidCache.GetAllDashboardPanels(lstMODULES);
				results.Add("DASHBOARDS_PANELS", DASHBOARDS_PANELS);
				metrics.Add("DASHBOARDS_PANELS", (DateTime.Now - dtStart).TotalSeconds);
				dtStart = DateTime.Now;
			
				// 08/09/2020 Paul.  Convert to comma separated string. 
				string sModuleList = String.Join(",", lstMODULES.ToArray());
				Dictionary<string, object> objs = memoryCache.Get("ReactCustomViews." + sModuleList) as Dictionary<string, object>;
#if DEBUG
				objs = null;
#endif
				if ( objs == null )
				{
					objs = new Dictionary<string, object>();
					// 12/07/2022 Paul.  Allow the LoginView to be customized. 
					SplendidCache.GetAllReactCustomViews(objs, lstMODULES, "~/React/src/CustomViewsJS", false, false);
					// 05/23/2019 Paul.  Include Dashlet views, but we do not yet have a way to separate by module. 
					SplendidCache.GetAllReactDashletViews(objs, lstMODULES, "~/React/src/DashletsJS");
					memoryCache.Set("ReactCustomViews." + sModuleList, objs, SplendidCache.DefaultCacheExpiration());
				}
				results.Add("REACT_CUSTOM_VIEWS", objs);
				metrics.Add("REACT_CUSTOM_VIEWS", (DateTime.Now - dtStart).TotalSeconds);
				results.Add("Metrics", metrics);
				// 07/12/2021 Paul.  Attempt to track timeout so that we can determine stale React state. 
				//results.Add("SessionStateTimeout", Session.Timeout);
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex.Message);
				StringBuilder sb = new StringBuilder();
				foreach ( string key in metrics.Keys )
				{
					sb.AppendLine("  " + key + Strings.Space(30 - key.Length) + ": " + metrics[key].ToString() + " sec");
				}
				Debug.Write(sb.ToString());
				throw(new Exception(ex.Message + ControlChars.CrLf + sb.ToString()));
			}
			return results;
		}
		#endregion

		#region Get
		// 10/16/2011 Paul.  HTML5 Offline Client needs access to the custom lists. 
		[HttpGet("[action]")]
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public Dictionary<string, object> GetCustomList([FromQuery] string ListName)
		{
			if ( Sql.IsEmptyString(ListName) )
				throw(new Exception("The list name must be specified."));
			// 08/22/2011 Paul.  Add admin control to REST API. 
			DataTable dt = new DataTable();
			dt.Columns.Add("NAME"        );
			dt.Columns.Add("DISPLAY_NAME");
			bool bCustomCache = false;
			// 02/24/2013 Paul.  Add custom calendar lists. 
			if ( ListName == "month_names_dom" )
			{
				for ( int i = 1; i <= 12; i++ )
				{
					string sID           = i.ToString();
					string sDISPLAY_NAME = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.MonthNames[i- 1];
					DataRow row = dt.NewRow();
					dt.Rows.Add(row);
					row["NAME"        ] = sID;
					row["DISPLAY_NAME"] = sDISPLAY_NAME;
				}
				bCustomCache = true;
			}
			else if ( ListName == "short_month_names_dom" )
			{
				for ( int i = 1; i <= 12; i++ )
				{
					string sID           = i.ToString();
					string sDISPLAY_NAME = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.AbbreviatedMonthNames[i- 1];
					DataRow row = dt.NewRow();
					dt.Rows.Add(row);
					row["NAME"        ] = sID;
					row["DISPLAY_NAME"] = sDISPLAY_NAME;
				}
				bCustomCache = true;
			}
			else if ( ListName == "day_names_dom" )
			{
				for ( int i = 0; i <= 6; i++ )
				{
					string sID           = i.ToString();
					string sDISPLAY_NAME = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.DayNames[i];
					DataRow row = dt.NewRow();
					dt.Rows.Add(row);
					row["NAME"        ] = sID;
					row["DISPLAY_NAME"] = sDISPLAY_NAME;
				}
				bCustomCache = true;
			}
			else if ( ListName == "short_day_names_dom" )
			{
				for ( int i = 0; i <= 6; i++ )
				{
					string sID           = i.ToString();
					string sDISPLAY_NAME = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[i];
					DataRow row = dt.NewRow();
					dt.Rows.Add(row);
					row["NAME"        ] = sID;
					row["DISPLAY_NAME"] = sDISPLAY_NAME;
				}
				bCustomCache = true;
			}
			else
			{
				// 10/04/2015 Paul.  Changed custom caches to a dynamic list. 
				List<SplendidCacheReference> arrCustomCaches = SplendidCache.CustomCaches;
				foreach ( SplendidCacheReference cache in arrCustomCaches )
				{
					if ( cache.Name == ListName )
					{
						string sDataValueField = cache.DataValueField;
						string sDataTextField  = cache.DataTextField ;
						SplendidCacheCallback cbkDataSource = cache.DataSource;
						foreach ( DataRow rowCustom in cbkDataSource().Rows )
						{
							DataRow row = dt.NewRow();
							dt.Rows.Add(row);
							row["NAME"        ] = Sql.ToString(rowCustom[sDataValueField]);
							row["DISPLAY_NAME"] = Sql.ToString(rowCustom[sDataTextField ]);
						}
						bCustomCache = true;
					}
				}
			}
			if ( !bCustomCache )
			{
				dt = SplendidCache.List(ListName);
			}
			string sBaseURI = Request.Scheme + "://" + Request.Host.Host + Request.Path.Value;
			// 04/01/2020 Paul.  Move json utils to RestUtil. 
			Dictionary<string, object> dictResponse = RestUtil.ToJson(sBaseURI, ListName, dt, T10n);
			return dictResponse;
		}

		// 05/21/2017 Paul.  HTML5 Dashboard requires aggregates. 
		// https://www.visualstudio.com/en-us/docs/report/analytics/aggregated-data-analytics
		private void BuildAggregateArray(string sAPPLY, ref string sGROUP_BY, ref UniqueStringCollection arrAGGREGATE)
		{
			Regex r = new Regex(@"[^A-Za-z0-9_ ]");
			if ( !Sql.IsEmptyString(sAPPLY) )
			{
				if ( sAPPLY.StartsWith("groupby((", StringComparison.InvariantCultureIgnoreCase) )
				{
					int nGroupStart = "groupby((".Length;
					int nGroupEnd   = sAPPLY.IndexOf("),", nGroupStart);
					if ( nGroupEnd > 0 )
					{
						sGROUP_BY = sAPPLY.Substring(nGroupStart, nGroupEnd - nGroupStart);
						int nAggregateStart = sAPPLY.IndexOf("aggregate(", nGroupEnd + 2, StringComparison.InvariantCultureIgnoreCase);
						if ( nAggregateStart > 0 )
						{
							nAggregateStart += "aggregate(".Length;
							int nAggregateEnd = sAPPLY.IndexOf("))", nAggregateStart);
							if ( nAggregateEnd > 0 )
							{
								string sAGGREGATE = sAPPLY.Substring(nAggregateStart, nAggregateEnd - nAggregateStart);
								foreach ( string s in sAGGREGATE.Split(',') )
								{
									string sColumnName = r.Replace(s, "").Trim();
									if ( !Sql.IsEmptyString(sColumnName) )
										arrAGGREGATE.Add(sColumnName);
								}
							}
							else
							{
								throw(new Exception("$apply is not formatted correctly. " + sAPPLY));
							}
						}
						else
						{
							throw(new Exception("$apply is not formatted correctly. " + sAPPLY));
						}
					}
					else
					{
						throw(new Exception("$apply is not formatted correctly. " + sAPPLY));
					}
				}
				else
				{
					throw(new Exception("$apply must start with groupby. " + sAPPLY));
				}
			}
		}

		// 05/25/2017 Paul.  Add support for Post operation so that we can support large Search operations in the new Dashboard. 
		[HttpPost("[action]")]
		public Dictionary<string, object> PostModuleTable([FromBody] Dictionary<string, object> dict)
		{
			string TableName         = Sql.ToString (Request.Query["TableName" ]);
			int    nSKIP             = Sql.ToInteger(Request.Query["$skip"     ]);
			int    nTOP              = Sql.ToInteger(Request.Query["$top"      ]);
			// 11/18/2019 Paul.  Move exclusively to SqlSearchClause. 
			string sFILTER           = String.Empty;  // Sql.ToString (Request.Query["$filter"   ]);
			string sORDER_BY         = Sql.ToString (Request.Query["$orderby"  ]);
			// 06/17/2013 Paul.  Add support for GROUP BY. 
			string sGROUP_BY         = Sql.ToString (Request.Query["$groupby"  ]);
			// 08/03/2011 Paul.  We need a way to filter the columns so that we can be efficient. 
			string sSELECT           = Sql.ToString (Request.Query["$select"   ]);
			// 12/03/2019 Paul.  The React Client needs access to archive data. 
			bool   bArchiveView      = Sql.ToBoolean(Request.Query["$archiveView"]);
			// 05/21/2017 Paul.  HTML5 Dashboard requires aggregates. 
			// https://www.visualstudio.com/en-us/docs/report/analytics/aggregated-data-analytics
			string sAPPLY    = String.Empty;
			Guid[] Items     = null;
			Dictionary<string, object> dictSearchValues = null;
			try
			{
				foreach ( string sName in dict.Keys )
				{
					switch ( sName )
					{
						case "TableName"       :  TableName         = Sql.ToString (dict[sName]);  break;
						case "$skip"           :  nSKIP             = Sql.ToInteger(dict[sName]);  break;
						case "$top"            :  nTOP              = Sql.ToInteger(dict[sName]);  break;
						case "$filter"         :  sFILTER           = Sql.ToString (dict[sName]);  break;
						case "$orderby"        :  sORDER_BY         = Sql.ToString (dict[sName]);  break;
						case "$groupby"        :  sGROUP_BY         = Sql.ToString (dict[sName]);  break;
						case "$select"         :  sSELECT           = Sql.ToString (dict[sName]);  break;
						case "$apply"          :  sAPPLY            = Sql.ToString (dict[sName]);  break;
						case "$archiveView"    :  bArchiveView      = Sql.ToBoolean(dict[sName]);  break;
						case "$searchvalues"   :  dictSearchValues  = dict[sName] as Dictionary<string, object>;  break;
						case "Items":
						{
							System.Collections.ArrayList lst = dict[sName] as System.Collections.ArrayList;
							if ( lst != null && lst.Count > 0 )
							{
								List<Guid> lstItems = new List<Guid>();
								foreach ( string sItemID in lst )
								{
									lstItems.Add(Sql.ToGuid(sItemID));
								}
								Items = lstItems.ToArray();
							}
							break;
						}
					}
				}
				if ( dictSearchValues != null )
				{
					string sSEARCH_VALUES = Sql.SqlSearchClause(dictSearchValues);
					// 11/18/2019 Paul.  We need to combine sFILTER with sSEARCH_VALUES. 
					if ( !Sql.IsEmptyString(sSEARCH_VALUES) )
					{
						// 11/18/2019 Paul.  The search clause will always start with an "and" if it exists. 
						if ( !Sql.IsEmptyString(sFILTER) )
						{
							sFILTER = sFILTER + sSEARCH_VALUES;
						}
						else
						{
							sFILTER = "1 = 1 " + sSEARCH_VALUES;
						}
					}
				}
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex.Message);
				throw;
			}
			// 12/03/2019 Paul.  The React Client needs access to archive data. 
			return GetModuleTableInternal(TableName, nSKIP, nTOP, sFILTER, sORDER_BY, sGROUP_BY, sSELECT, sAPPLY, Items, bArchiveView);
		}

		[HttpGet("[action]")]
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public Dictionary<string, object> GetModuleTable([FromQuery] string TableName)
		{
			int    nSKIP     = Sql.ToInteger(Request.Query["$skip"   ]);
			int    nTOP      = Sql.ToInteger(Request.Query["$top"    ]);
			string sFILTER   = Sql.ToString (Request.Query["$filter" ]);
			string sORDER_BY = Sql.ToString (Request.Query["$orderby"]);
			// 06/17/2013 Paul.  Add support for GROUP BY. 
			string sGROUP_BY = Sql.ToString (Request.Query["$groupby"]);
			// 08/03/2011 Paul.  We need a way to filter the columns so that we can be efficient. 
			string sSELECT   = Sql.ToString (Request.Query["$select" ]);
			// 05/21/2017 Paul.  HTML5 Dashboard requires aggregates. 
			// https://www.visualstudio.com/en-us/docs/report/analytics/aggregated-data-analytics
			string sAPPLY  = Sql.ToString(Request.Query["$apply"]);
			string[] arrItems = null;
			if ( Request.Query.ContainsKey("Items") )
				arrItems = Request.Query["Items"].ToArray();
			Guid[] Items = null;
			// 06/17/2011 Paul.  arrItems might be null. 
			if ( arrItems != null && arrItems.Length > 0 )
			{
				Items = new Guid[arrItems.Length];
				for ( int i = 0; i < arrItems.Length; i++ )
				{
					Items[i] = Sql.ToGuid(arrItems[i]);
				}
			}
			return GetModuleTableInternal(TableName, nSKIP, nTOP, sFILTER, sORDER_BY, sGROUP_BY, sSELECT, sAPPLY, Items, false);
		}

		// 12/03/2019 Paul.  The React Client needs access to archive data. 
		private Dictionary<string, object> GetModuleTableInternal(string TableName, int nSKIP, int nTOP, string sFILTER, string sORDER_BY, string sGROUP_BY, string sSELECT, string sAPPLY, Guid[] Items, bool bArchiveView)
		{
			Regex r = new Regex(@"[^A-Za-z0-9_]");
			string sFILTER_KEYWORDS = (" " + r.Replace(sFILTER, " ") + " ").ToLower();
			if ( sFILTER_KEYWORDS.Contains(" select ") )
			{
				throw(new Exception("Subqueries are not allowed."));
			}
			if ( sFILTER.Contains(";") )
			{
				// 06/18/2011 Paul.  This is to prevent the user from attempting to inject SQL. 
				throw(new Exception("A semicolon is not allowed anywhere in a filter. "));
			}
			if ( sORDER_BY.Contains(";") )
			{
				// 06/18/2011 Paul.  This is to prevent the user from attempting to inject SQL. 
				throw(new Exception("A semicolon is not allowed anywhere in a sort expression. "));
			}
			if ( sAPPLY.Contains(";") )
			{
				// 06/18/2011 Paul.  This is to prevent the user from attempting to inject SQL. 
				throw(new Exception("A semicolon is not allowed anywhere in a apply statement. "));
			}
			if ( !Sql.IsEmptyString(sGROUP_BY) && !Sql.IsEmptyString(sAPPLY) )
			{
				// 05/21/2017 Paul.  Need to prevent two types. 
				// https://www.visualstudio.com/en-us/docs/report/analytics/aggregated-data-analytics
				throw(new Exception("$groupby and $apply cannot both be specified. "));
			}
			// 08/22/2011 Paul.  Add admin control to REST API. 
			string sMODULE_NAME = Sql.ToString(Application["Modules." + TableName + ".ModuleName"]);
			// 01/26/2020 Paul.  Need to correct the module if archive table provided. 
			if ( bArchiveView && TableName.EndsWith("_ARCHIVE") )
			{
				sMODULE_NAME = Sql.ToString(Application["Modules." + TableName.Substring(0, TableName.Length - 8) + ".ModuleName"]);
			}
			// 08/22/2011 Paul.  Not all tables will have a module name, such as relationship tables. 
			// Tables will get another security filter later in the code. 
			if ( !Sql.IsEmptyString(sMODULE_NAME) )
			{
				int nACLACCESS = Security.GetUserAccess(sMODULE_NAME, "list");
				if ( !Sql.ToBoolean(Application["Modules." + sMODULE_NAME + ".RestEnabled"]) || nACLACCESS < 0 )
				{
					// 09/06/2017 Paul.  Include module name in error. 
					throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS") + ": " + sMODULE_NAME));
				}
			}
			// 07/05/2020 Paul.  Need to restrict admin table updates. 
			if ( TableName == "vwUSERS_TEAM_MEMBERSHIPS" || TableName == "vwUSERS_ACL_ROLES" )
			{
				bool bMyAccount = false;
				if ( Items != null && Items.Length == 1 && Items[0] == Security.USER_ID )
					bMyAccount = true;
				else if ( !Sql.IsEmptyString(sFILTER) && sFILTER.Contains(" and USER_ID = \'" + Security.USER_ID.ToString() + "\'"))
					bMyAccount = true;
				if ( Security.AdminUserAccess("Users", "view") < 0 && !bMyAccount )
				{
					throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
				}
			}
			
			UniqueStringCollection arrSELECT = new UniqueStringCollection();
			sSELECT = sSELECT.Replace(" ", "");
			if ( !Sql.IsEmptyString(sSELECT) )
			{
				foreach ( string s in sSELECT.Split(',') )
				{
					string sColumnName = r.Replace(s, "");
					if ( !Sql.IsEmptyString(sColumnName) )
						arrSELECT.Add(sColumnName);
				}
			}
			
			// 05/21/2017 Paul.  HTML5 Dashboard requires aggregates. 
			UniqueStringCollection arrAGGREGATE = null;
			if ( !Sql.IsEmptyString(sAPPLY) )
			{
				arrAGGREGATE = new UniqueStringCollection();
				BuildAggregateArray(sAPPLY, ref sGROUP_BY, ref arrAGGREGATE);
			}
			
			// 06/17/2013 Paul.  Add support for GROUP BY. 
			// 04/21/2017 Paul.  We need to return the total when using nTOP. 
			long lTotalCount = 0;
			// 05/21/2017 Paul.  HTML5 Dashboard requires aggregates. 
			// 08/01/2019 Paul.  We need a ListView and EditView flags for the Rest Client. 
			// 09/09/2019 Paul.  Send duplicate filter info. 
			// 10/26/2019 Paul.  Return the SQL to the React Client. 
			// 12/03/2019 Paul.  The React Client needs access to archive data. 
			StringBuilder sbDumpSQL = new StringBuilder();
			// 12/16/2019 Paul.  Moved GetTable to ~/_code/RestUtil.cs
			DataTable dt = RestUtil.GetTable(TableName, nSKIP, nTOP, sFILTER, sORDER_BY, sGROUP_BY, arrSELECT, Items, ref lTotalCount, arrAGGREGATE, AccessMode.related, bArchiveView, String.Empty, sbDumpSQL);
			
			string sBaseURI = Request.Scheme + "://" + Request.Host.Host + Request.Path.Value.Replace("/GetModuleTable", "/GetModuleItem");
			
			// 04/21/2017 Paul.  We need to return the total when using nTOP. 
			// 04/01/2020 Paul.  Move json utils to RestUtil. 
			Dictionary<string, object> dictResponse = RestUtil.ToJson(sBaseURI, sMODULE_NAME, dt, T10n);
			dictResponse.Add("__total", lTotalCount);
			// 10/26/2019 Paul.  Return the SQL to the React Client. 
			if ( Sql.ToBoolean(Application["CONFIG.show_sql"]) )
			{
				dictResponse.Add("__sql", sbDumpSQL.ToString());
			}
			return dictResponse;
		}

		[HttpGet("[action]")]
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public Dictionary<string, object> GetModuleList([FromQuery] string ModuleName)
		{
			if ( Sql.IsEmptyString(ModuleName) )
				throw(new Exception("The module name must be specified."));
			string sTABLE_NAME = Sql.ToString(Application["Modules." + ModuleName + ".TableName"]);
			// 02/29/2016 Paul.  Product Catalog is different than Product Templates. 
			if ( ModuleName == "ProductCatalog" )
				sTABLE_NAME = "PRODUCT_CATALOG";
			// 05/26/2020 Paul.  The Activities module collies with the Calendar list, so we have to make an exception. 
			if ( ModuleName == "Activities" )
				sTABLE_NAME = "vwACTIVITIES";
			// 05/26/2020 Paul.  The Employees module refers to the USERS table, so correct. 
			if ( ModuleName == "Employees" )
				sTABLE_NAME = "vwEMPLOYEES_Sync";
			// 05/19/2021 Paul.  ReportRules is based off of RULES table, but that is also used by RulesWizard. 
			if ( ModuleName == "ReportRules" )
				sTABLE_NAME = "vwREPORT_RULES";
			if ( Sql.IsEmptyString(sTABLE_NAME) )
				throw(new Exception("Unknown module: " + ModuleName));
			// 08/22/2011 Paul.  Add admin control to REST API. 
			int nACLACCESS = Security.GetUserAccess(ModuleName, "list");
			if ( !Security.IsAuthenticated() || !Sql.ToBoolean(Application["Modules." + ModuleName + ".RestEnabled"]) || nACLACCESS < 0 )
			{
				// 09/06/2017 Paul.  Include module name in error. 
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS") + ": " + Sql.ToString(ModuleName)));
			}
			
			int    nSKIP        = Sql.ToInteger(Request.Query["$skip"       ]);
			int    nTOP         = Sql.ToInteger(Request.Query["$top"        ]);
			string sFILTER      = Sql.ToString (Request.Query["$filter"     ]);
			string sORDER_BY    = Sql.ToString (Request.Query["$orderby"    ]);
			// 06/17/2013 Paul.  Add support for GROUP BY. 
			string sGROUP_BY    = Sql.ToString (Request.Query["$groupby"    ]);
			// 08/03/2011 Paul.  We need a way to filter the columns so that we can be efficient. 
			string sSELECT      = Sql.ToString (Request.Query["$select"     ]);
			// 12/03/2019 Paul.  The React Client needs access to archive data. 
			bool   bArchiveView = Sql.ToBoolean(Request.Query["$archiveView"]);
			
			Regex r = new Regex(@"[^A-Za-z0-9_]");
			// 10/19/2016 Paul.  We need to filter out quoted strings. 
			string sFILTER_KEYWORDS = Sql.SqlFilterLiterals(sFILTER);
			sFILTER_KEYWORDS = (" " + r.Replace(sFILTER_KEYWORDS, " ") + " ").ToLower();
			// 10/19/2016 Paul.  Add more rules to allow select keyword to be part of the contents. 
			// We do this to allow Full-Text Search, which is implemented as a sub-query. 
			int nSelectIndex     = sFILTER_KEYWORDS.IndexOf(" select ");
			int nFromIndex       = sFILTER_KEYWORDS.IndexOf(" from ");
			// 11/18/2019 Paul.  Remove all support for subqueries now that we support Post with search values. 
			//int nContainsIndex   = sFILTER_KEYWORDS.IndexOf(" contains ");
			//int nConflictedIndex = sFILTER_KEYWORDS.IndexOf(" _remote_conflicted ");
			//// 07/26/2018 Paul.  Allow a normalized phone search that used the special phone tables. 
			//int nPhoneTableIndex = sFILTER_KEYWORDS.IndexOf(" vwphone_numbers_");
			//int nNormalizeIndex  = sFILTER_KEYWORDS.IndexOf(" normalized_number ");
			if ( nSelectIndex >= 0 && nFromIndex > nSelectIndex )
			{
				//if ( !(nContainsIndex > nFromIndex || nConflictedIndex > nFromIndex || (nPhoneTableIndex > nFromIndex && nNormalizeIndex > nPhoneTableIndex )) )
					throw(new Exception("Subqueries are not allowed."));
			}

			UniqueStringCollection arrSELECT = new UniqueStringCollection();
			sSELECT = sSELECT.Replace(" ", "");
			if ( !Sql.IsEmptyString(sSELECT) )
			{
				foreach ( string s in sSELECT.Split(',') )
				{
					string sColumnName = r.Replace(s, "");
					if ( !Sql.IsEmptyString(sColumnName) )
						arrSELECT.Add(sColumnName);
				}
			}
			
			// 06/17/2013 Paul.  Add support for GROUP BY. 
			// 04/21/2017 Paul.  We need to return the total when using nTOP. 
			long lTotalCount = 0;
			// 05/21/2017 Paul.  HTML5 Dashboard requires aggregates. 
			// 08/01/2019 Paul.  We need a ListView and EditView flags for the Rest Client. 
			// 09/09/2019 Paul.  Send duplicate filter info. 
			// 10/26/2019 Paul.  Return the SQL to the React Client. 
			// 12/03/2019 Paul.  The React Client needs access to archive data. 
			StringBuilder sbDumpSQL = new StringBuilder();
			// 12/16/2019 Paul.  Moved GetTable to ~/_code/RestUtil.cs
			// 01/06/2020 Paul.  Use AccessMode.list so that we use the _List view if available. 
			DataTable dt = RestUtil.GetTable(sTABLE_NAME, nSKIP, nTOP, sFILTER, sORDER_BY, sGROUP_BY, arrSELECT, null, ref lTotalCount, null, AccessMode.list, bArchiveView, String.Empty, sbDumpSQL);
			
			string sBaseURI = Request.Scheme + "://" + Request.Host.Host + Request.Path.Value.Replace("/GetModuleList", "/GetModuleItem");
			
			// 04/21/2017 Paul.  We need to return the total when using nTOP. 
			// 04/01/2020 Paul.  Move json utils to RestUtil. 
			Dictionary<string, object> dictResponse = RestUtil.ToJson(sBaseURI, ModuleName, dt, T10n);
			dictResponse.Add("__total", lTotalCount);
			// 10/26/2019 Paul.  Return the SQL to the React Client. 
			if ( Sql.ToBoolean(Application["CONFIG.show_sql"]) )
			{
				dictResponse.Add("__sql", sbDumpSQL.ToString());
			}
			return dictResponse;
		}

		// 08/17/2019 Paul.  Post version so that we can support large filter requests.  This is common with the UnifiedSearch. 
		// 08/17/2019 Paul.  Must use stream and 'application/octet-stream'. 
		[HttpPost("[action]")]
		public Dictionary<string, object> PostModuleList([FromBody] Dictionary<string, object> dict)
		{
			string ModuleName        = Sql.ToString (Request.Query["ModuleName"]);
			int    nSKIP             = Sql.ToInteger(Request.Query["$skip"     ]);
			int    nTOP              = Sql.ToInteger(Request.Query["$top"      ]);
			// 11/18/2019 Paul.  Move exclusively to SqlSearchClause. 
			string sFILTER           = String.Empty;  // Sql.ToString (Request.Query["$filter"   ]);
			string sORDER_BY         = Sql.ToString (Request.Query["$orderby"  ]);
			// 06/17/2013 Paul.  Add support for GROUP BY. 
			string sGROUP_BY         = Sql.ToString (Request.Query["$groupby"  ]);
			// 08/03/2011 Paul.  We need a way to filter the columns so that we can be efficient. 
			string sSELECT           = Sql.ToString (Request.Query["$select"   ]);
			// 09/09/2019 Paul.  Send duplicate filter info. 
			string sDUPLICATE_FIELDS = String.Empty;
			// 12/03/2019 Paul.  The React Client needs access to archive data. 
			bool   bArchiveView      = Sql.ToBoolean(Request.Query["$archiveView"]);
			
			Regex r = new Regex(@"[^A-Za-z0-9_]");
			// 10/19/2016 Paul.  We need to filter out quoted strings. 
			string sFILTER_KEYWORDS = Sql.SqlFilterLiterals(sFILTER);
			sFILTER_KEYWORDS = (" " + r.Replace(sFILTER_KEYWORDS, " ") + " ").ToLower();
			// 10/19/2016 Paul.  Add more rules to allow select keyword to be part of the contents. 
			// We do this to allow Full-Text Search, which is implemented as a sub-query. 
			int nSelectIndex     = sFILTER_KEYWORDS.IndexOf(" select "            );
			int nFromIndex       = sFILTER_KEYWORDS.IndexOf(" from "              );
			// 11/18/2019 Paul.  Remove all support for subqueries now that we support Post with search values. 
			//int nContainsIndex   = sFILTER_KEYWORDS.IndexOf(" contains "          );
			//int nConflictedIndex = sFILTER_KEYWORDS.IndexOf(" _remote_conflicted ");
			//// 07/26/2018 Paul.  Allow a normalized phone search that used the special phone tables. 
			//int nPhoneTableIndex = sFILTER_KEYWORDS.IndexOf(" vwphone_numbers_"   );
			//int nNormalizeIndex  = sFILTER_KEYWORDS.IndexOf(" normalized_number " );
			if ( nSelectIndex >= 0 && nFromIndex > nSelectIndex )
			{
				//if ( !(nContainsIndex > nFromIndex || nConflictedIndex > nFromIndex || (nPhoneTableIndex > nFromIndex && nNormalizeIndex > nPhoneTableIndex )) )
					throw(new Exception("Subqueries are not allowed."));
			}

			Dictionary<string, object> dictSearchValues = null;
			try
			{
				foreach ( string sName in dict.Keys )
				{
					switch ( sName )
					{
						case "ModuleName"      :  ModuleName        = Sql.ToString (dict[sName]);  break;
						case "$skip"           :  nSKIP             = Sql.ToInteger(dict[sName]);  break;
						case "$top"            :  nTOP              = Sql.ToInteger(dict[sName]);  break;
						case "$filter"         :  sFILTER           = Sql.ToString (dict[sName]);  break;
						case "$orderby"        :  sORDER_BY         = Sql.ToString (dict[sName]);  break;
						case "$groupby"        :  sGROUP_BY         = Sql.ToString (dict[sName]);  break;
						case "$select"         :  sSELECT           = Sql.ToString (dict[sName]);  break;
						case "$duplicatefields":  sDUPLICATE_FIELDS = Sql.ToString (dict[sName]);  break;
						case "$archiveView"    :  bArchiveView      = Sql.ToBoolean(dict[sName]);  break;
						case "$searchvalues"   :  dictSearchValues = dict[sName] as Dictionary<string, object>;  break;
					}
				}
				if ( dictSearchValues != null )
				{
					string sSEARCH_VALUES = Sql.SqlSearchClause(dictSearchValues);
					// 11/18/2019 Paul.  We need to combine sFILTER with sSEARCH_VALUES. 
					if ( !Sql.IsEmptyString(sSEARCH_VALUES) )
					{
						// 11/18/2019 Paul.  The search clause will always start with an "and" if it exists. 
						if ( !Sql.IsEmptyString(sFILTER) )
						{
							sFILTER = sFILTER + sSEARCH_VALUES;
						}
						else
						{
							sFILTER = "1 = 1 " + sSEARCH_VALUES;
						}
					}
				}
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex.Message);
				throw;
			}
			if ( Sql.IsEmptyString(ModuleName) )
				throw(new Exception("The module name must be specified."));
			string sTABLE_NAME = Sql.ToString(Application["Modules." + ModuleName + ".TableName"]);
			// 02/29/2016 Paul.  Product Catalog is different than Product Templates. 
			if ( ModuleName == "ProductCatalog" )
				sTABLE_NAME = "PRODUCT_CATALOG";
			// 09/09/2019 Paul.  The Activities module collies with the Calendar list, so we have to make an exception. 
			if ( ModuleName == "Activities" )
				sTABLE_NAME = "vwACTIVITIES";
			// 09/09/2019 Paul.  The Employees module refers to the USERS table, so correct. 
			if ( ModuleName == "Employees" )
				sTABLE_NAME = "vwEMPLOYEES_Sync";
			// 05/19/2021 Paul.  ReportRules is based off of RULES table, but that is also used by RulesWizard. 
			if ( ModuleName == "ReportRules" )
				sTABLE_NAME = "vwREPORT_RULES";
			if ( Sql.IsEmptyString(sTABLE_NAME) )
				throw(new Exception("Unknown module: " + ModuleName));
			// 08/22/2011 Paul.  Add admin control to REST API. 
			int nACLACCESS = Security.GetUserAccess(ModuleName, "list");
			if ( !Security.IsAuthenticated() || !Sql.ToBoolean(Application["Modules." + ModuleName + ".RestEnabled"]) || nACLACCESS < 0 )
			{
				// 09/06/2017 Paul.  Include module name in error. 
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS") + ": " + Sql.ToString(ModuleName)));
			}
			UniqueStringCollection arrSELECT = new UniqueStringCollection();
			sSELECT = sSELECT.Replace(" ", "");
			if ( !Sql.IsEmptyString(sSELECT) )
			{
				foreach ( string s in sSELECT.Split(',') )
				{
					string sColumnName = r.Replace(s, "");
					if ( !Sql.IsEmptyString(sColumnName) )
						arrSELECT.Add(sColumnName);
				}
			}
			
			// 06/17/2013 Paul.  Add support for GROUP BY. 
			// 04/21/2017 Paul.  We need to return the total when using nTOP. 
			long lTotalCount = 0;
			// 05/21/2017 Paul.  HTML5 Dashboard requires aggregates. 
			// 08/01/2019 Paul.  We need a ListView and EditView flags for the Rest Client. 
			// 09/09/2019 Paul.  Send duplicate filter info. 
			// 10/26/2019 Paul.  Return the SQL to the React Client. 
			StringBuilder sbDumpSQL = new StringBuilder();
			// 12/03/2019 Paul.  The React Client needs access to archive data. 
			// 12/16/2019 Paul.  Moved GetTable to ~/_code/RestUtil.cs
			// 01/06/2020 Paul.  Use AccessMode.list so that we use the _List view if available. 
			DataTable dt = RestUtil.GetTable(sTABLE_NAME, nSKIP, nTOP, sFILTER, sORDER_BY, sGROUP_BY, arrSELECT, null, ref lTotalCount, null, AccessMode.list, bArchiveView, sDUPLICATE_FIELDS, sbDumpSQL);
			
			string sBaseURI = Request.Scheme + "://" + Request.Host.Host + Request.Path.Value.Replace("/GetModuleList", "/GetModuleItem");
			
			// 04/21/2017 Paul.  We need to return the total when using nTOP. 
			// 04/01/2020 Paul.  Move json utils to RestUtil. 
			Dictionary<string, object> dictResponse = RestUtil.ToJson(sBaseURI, ModuleName, dt, T10n);
			dictResponse.Add("__total", lTotalCount);
			// 10/26/2019 Paul.  Return the SQL to the React Client. 
			if ( Sql.ToBoolean(Application["CONFIG.show_sql"]) )
			{
				dictResponse.Add("__sql", sbDumpSQL.ToString());
			}
			return dictResponse;
		}

// 12/21/2021 TODO.  ExportModuleList
#if false
		// 12/15/2019 Paul.  Export needs to follow the same parsing rules as PostModuleList. 
		[HttpPost("[action]")]
		public Dictionary<string, object> ExportModuleList([FromBody] Dictionary<string, object> dict)
		{
			string ModuleName        = Sql.ToString (Request.Query["ModuleName"]);
			int    nSKIP             = Sql.ToInteger(Request.Query["$skip"     ]);
			int    nTOP              = Sql.ToInteger(Request.Query["$top"      ]);
			// 11/18/2019 Paul.  Move exclusively to SqlSearchClause. 
			string sFILTER           = String.Empty;  // Sql.ToString (Request.Query["$filter"   ]);
			string sORDER_BY         = Sql.ToString (Request.Query["$orderby"  ]);
			// 06/17/2013 Paul.  Add support for GROUP BY. 
			string sGROUP_BY         = Sql.ToString (Request.Query["$groupby"  ]);
			// 08/03/2011 Paul.  We need a way to filter the columns so that we can be efficient. 
			string sSELECT           = Sql.ToString (Request.Query["$select"   ]);
			// 09/09/2019 Paul.  Send duplicate filter info. 
			string sDUPLICATE_FIELDS = String.Empty;
			// 12/03/2019 Paul.  The React Client needs access to archive data. 
			bool   bArchiveView      = Sql.ToBoolean(Request.Query["$archiveView"]);
			
			Regex r = new Regex(@"[^A-Za-z0-9_]");
			// 10/19/2016 Paul.  We need to filter out quoted strings. 
			string sFILTER_KEYWORDS = Sql.SqlFilterLiterals(sFILTER);
			sFILTER_KEYWORDS = (" " + r.Replace(sFILTER_KEYWORDS, " ") + " ").ToLower();
			// 10/19/2016 Paul.  Add more rules to allow select keyword to be part of the contents. 
			// We do this to allow Full-Text Search, which is implemented as a sub-query. 
			int nSelectIndex     = sFILTER_KEYWORDS.IndexOf(" select "            );
			int nFromIndex       = sFILTER_KEYWORDS.IndexOf(" from "              );
			// 11/18/2019 Paul.  Remove all support for subqueries now that we support Post with search values. 
			//int nContainsIndex   = sFILTER_KEYWORDS.IndexOf(" contains "          );
			//int nConflictedIndex = sFILTER_KEYWORDS.IndexOf(" _remote_conflicted ");
			//// 07/26/2018 Paul.  Allow a normalized phone search that used the special phone tables. 
			//int nPhoneTableIndex = sFILTER_KEYWORDS.IndexOf(" vwphone_numbers_"   );
			//int nNormalizeIndex  = sFILTER_KEYWORDS.IndexOf(" normalized_number " );
			if ( nSelectIndex >= 0 && nFromIndex > nSelectIndex )
			{
				//if ( !(nContainsIndex > nFromIndex || nConflictedIndex > nFromIndex || (nPhoneTableIndex > nFromIndex && nNormalizeIndex > nPhoneTableIndex )) )
					throw(new Exception("Subqueries are not allowed."));
			}

			string     sExportFormat    = String.Empty;
			string     sExportRange     = String.Empty;
			List<Guid> arrSelectedItems = new List<Guid>();
			Dictionary<string, object> dictSearchValues = null;
			try
			{
				foreach ( string sName in dict.Keys )
				{
					switch ( sName )
					{
						case "ModuleName"      :  ModuleName        = Sql.ToString (dict[sName]);  break;
						case "$skip"           :  nSKIP             = Sql.ToInteger(dict[sName]);  break;
						case "$top"            :  nTOP              = Sql.ToInteger(dict[sName]);  break;
						case "$filter"         :  sFILTER           = Sql.ToString (dict[sName]);  break;
						case "$orderby"        :  sORDER_BY         = Sql.ToString (dict[sName]);  break;
						case "$groupby"        :  sGROUP_BY         = Sql.ToString (dict[sName]);  break;
						case "$select"         :  sSELECT           = Sql.ToString (dict[sName]);  break;
						case "$duplicatefields":  sDUPLICATE_FIELDS = Sql.ToString (dict[sName]);  break;
						case "$archiveView"    :  bArchiveView      = Sql.ToBoolean(dict[sName]);  break;
						case "$searchvalues"   :  dictSearchValues  = dict[sName] as Dictionary<string, object>;  break;
						case "$exportformat"   :  sExportFormat     = Sql.ToString (dict[sName]);  break;
						case "$exportrange"    :  sExportRange      = Sql.ToString (dict[sName]);  break;
						case "$selecteditems"  :
						{
							System.Collections.ArrayList arr = dict[sName] as System.Collections.ArrayList;
							if ( arr != null )
							{
								for ( int i = 0; i < arr.Count; i++ )
								{
									Guid g =Sql.ToGuid(arr[i]);
									if ( !Sql.IsEmptyGuid(g) )
									{
										arrSelectedItems.Add(g);
									}
								}
								}
							break;
						}
					}
				}
				if ( dictSearchValues != null )
				{
					string sSEARCH_VALUES = Sql.SqlSearchClause(dictSearchValues);
					// 11/18/2019 Paul.  We need to combine sFILTER with sSEARCH_VALUES. 
					if ( !Sql.IsEmptyString(sSEARCH_VALUES) )
					{
						// 11/18/2019 Paul.  The search clause will always start with an "and" if it exists. 
						if ( !Sql.IsEmptyString(sFILTER) )
						{
							sFILTER = sFILTER + sSEARCH_VALUES;
						}
						else
						{
							sFILTER = "1 = 1 " + sSEARCH_VALUES;
						}
					}
				}
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex.Message);
				throw;
			}
			if ( Sql.IsEmptyString(ModuleName) )
				throw(new Exception("The module name must be specified."));
			if ( Sql.IsEmptyString(sExportFormat) )
				throw(new Exception("The export format must be specified."));
			if ( Sql.IsEmptyString(sExportRange) )
				throw(new Exception("The export range must be specified."));
			if ( sExportRange == "All" )
			{
				nSKIP = 0;
				nTOP = -1;
			}
			else if ( sExportRange == "Page" )
			{
				arrSelectedItems.Clear();
			}
			else if ( sExportRange == "Selected" )
			{
				// 10/17/2006 Paul.  There must be one selected record to continue. 
				if ( arrSelectedItems.Count == 0 )
					throw(new Exception(L10n.Term(".LBL_LISTVIEW_NO_SELECTED")));
			}
			else
			{
				throw(new Exception("The valid export range must be specified."));
			}
			// 12/15/2019 Paul.  Export has a layout list. 
			UniqueStringCollection arrSelectFields = new UniqueStringCollection();
			sSELECT = SplendidDynamic.ExportGridColumns(ModuleName + ".Export", arrSelectFields);

			string sTABLE_NAME = Sql.ToString(Application["Modules." + ModuleName + ".TableName"]);
			// 02/29/2016 Paul.  Product Catalog is different than Product Templates. 
			if ( ModuleName == "ProductCatalog" )
				sTABLE_NAME = "PRODUCT_CATALOG";
			// 09/09/2019 Paul.  The Activities module collies with the Calendar list, so we have to make an exception. 
			if ( ModuleName == "Activities" )
				sTABLE_NAME = "vwACTIVITIES";
			// 09/09/2019 Paul.  The Employees module refers to the USERS table, so correct. 
			if ( ModuleName == "Employees" )
				sTABLE_NAME = "vwEMPLOYEES_Sync";
			// 05/19/2021 Paul.  ReportRules is based off of RULES table, but that is also used by RulesWizard. 
			if ( ModuleName == "ReportRules" )
				sTABLE_NAME = "vwREPORT_RULES";
			if ( Sql.IsEmptyString(sTABLE_NAME) )
				throw(new Exception("Unknown module: " + ModuleName));
			// 08/22/2011 Paul.  Add admin control to REST API. 
			int nACLACCESS = Security.GetUserAccess(ModuleName, "export");
			if ( !Security.IsAuthenticated() || !Sql.ToBoolean(Application["Modules." + ModuleName + ".RestEnabled"]) || nACLACCESS < 0 )
			{
				// 09/06/2017 Paul.  Include module name in error. 
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS") + ": " + Sql.ToString(ModuleName)));
			}
			UniqueStringCollection arrSELECT = new UniqueStringCollection();
			sSELECT = sSELECT.Replace(" ", "");
			if ( !Sql.IsEmptyString(sSELECT) )
			{
				foreach ( string s in sSELECT.Split(',') )
				{
					string sColumnName = r.Replace(s, "");
					if ( !Sql.IsEmptyString(sColumnName) )
						arrSELECT.Add(sColumnName);
				}
			}
			
			// 06/17/2013 Paul.  Add support for GROUP BY. 
			// 04/21/2017 Paul.  We need to return the total when using nTOP. 
			long lTotalCount = 0;
			// 05/21/2017 Paul.  HTML5 Dashboard requires aggregates. 
			// 08/01/2019 Paul.  We need a ListView and EditView flags for the Rest Client. 
			// 09/09/2019 Paul.  Send duplicate filter info. 
			// 10/26/2019 Paul.  Return the SQL to the React Client. 
			StringBuilder sbDumpSQL = new StringBuilder();
			// 12/03/2019 Paul.  The React Client needs access to archive data. 
			// 12/16/2019 Paul.  Moved GetTable to ~/_code/RestUtil.cs
			DataTable dt = RestUtil.GetTable(sTABLE_NAME, nSKIP, nTOP, sFILTER, sORDER_BY, sGROUP_BY, arrSELECT, arrSelectedItems.ToArray(), ref lTotalCount, null, AccessMode.list, bArchiveView, sDUPLICATE_FIELDS, sbDumpSQL);
			DataView vwMain = new DataView(dt);
			
			// 12/14/2019 Paul.  I'm not sure why this was necessary in the ListView code, but we are going to rely upon the Security.Filter() to manage. 
			//if ( nACLACCESS == ACL_ACCESS.OWNER )
			//	vwMain.RowFilter = "ASSIGNED_USER_ID = '" + Security.USER_ID.ToString() + "'";
			int    nStartRecord        = 0;
			int    nEndRecord          = vwMain.Count;
			string sExportTempFileName = String.Empty;
			string sExportTempPathName = String.Empty;
			string sContentType        = String.Empty;
			string sTempPath           = Path.GetTempPath();
			sTempPath = Path.Combine(sTempPath, "Splendid");
			// 12/15/2019 Paul.  The Splendid folder may not exist. 
			if ( !Directory.Exists(sTempPath) )
			{
				Directory.CreateDirectory(sTempPath);
			}
			switch ( sExportFormat )
			{
				case "csv"  :
				{
					sContentType = "text/csv";
					sExportTempFileName = Guid.NewGuid().ToString() + "_" + ModuleName + ".csv";
					sExportTempPathName = Path.Combine(sTempPath, sExportTempFileName);
					Session["TempFile." + sExportTempFileName] = sExportTempPathName;
					// 08/06/2008 yxy21969.  Make sure to encode all URLs. 
					// 12/20/2009 Paul.  Use our own encoding so that a space does not get converted to a +. 
					using ( FileStream stm = System.IO.File.Create(sExportTempPathName) )
					{
						SplendidExport.ExportDelimited(stm, vwMain, ModuleName, nStartRecord, nEndRecord, ',' );
					}
					break;
				}
				case "tab"  :
				{
					sContentType = "text/txt";
					sExportTempFileName = Guid.NewGuid().ToString() + "_" + ModuleName + ".txt";
					sExportTempPathName = Path.Combine(sTempPath, sExportTempFileName);
					Session["TempFile." + sExportTempFileName] = sExportTempPathName;
					// 08/06/2008 yxy21969.  Make sure to encode all URLs. 
					// 12/20/2009 Paul.  Use our own encoding so that a space does not get converted to a +. 
					using ( FileStream stm = System.IO.File.Create(sExportTempPathName) )
					{
						SplendidExport.ExportDelimited(stm, vwMain, ModuleName, nStartRecord, nEndRecord, '\t');
					}
					break;
				}
				case "xml"  :
				{
					sContentType = "text/xml";
					sExportTempFileName = Guid.NewGuid().ToString() + "_" + ModuleName + ".xml";
					sExportTempPathName = Path.Combine(sTempPath, sExportTempFileName);
					Session["TempFile." + sExportTempFileName] = sExportTempPathName;
					// 08/06/2008 yxy21969.  Make sure to encode all URLs. 
					// 12/20/2009 Paul.  Use our own encoding so that a space does not get converted to a +. 
					using ( FileStream stm = System.IO.File.Create(sExportTempPathName) )
					{
						SplendidExport.ExportXml(stm, vwMain, ModuleName, nStartRecord, nEndRecord);
					}
					break;
				}
				//case "Excel":
				default     :
				{
					// 08/25/2012 Paul.  Change Excel export type to use Open XML as the previous format is not supported on Office 2010. 
					sContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";  //"application/vnd.ms-excel";
					sExportTempFileName = Guid.NewGuid().ToString() + "_" + ModuleName + ".xlsx";
					sExportTempPathName = Path.Combine(sTempPath, sExportTempFileName);
					Session["TempFile." + sExportTempFileName] = sExportTempPathName;
					// 08/25/2012 Paul.  Change Excel export type to use Open XML as the previous format is not supported on Office 2010. 
					// 12/20/2009 Paul.  Use our own encoding so that a space does not get converted to a +. 
					using ( FileStream stm = System.IO.File.Create(sExportTempPathName) )
					{
						SplendidExport.ExportExcelOpenXML(stm, vwMain, ModuleName, nStartRecord, nEndRecord);
					}
					break;
				}
			}
			Dictionary<string, object> dictResponse = new Dictionary<string, object>();
			Dictionary<string, object> d = new Dictionary<string, object>();
			dictResponse.Add("d", d);
#if DEBUG
			d.Add("ExportPathName", sExportTempPathName);
#endif
			d.Add("ExportFileName", sExportTempFileName);
			d.Add("ContentType"   , sContentType);

			dictResponse.Add("__total", lTotalCount);
			// 10/26/2019 Paul.  Return the SQL to the React Client. 
			if ( Sql.ToBoolean(Application["CONFIG.show_sql"]) )
			{
				dictResponse.Add("__sql", sbDumpSQL.ToString());
			}
			return dictResponse;
		}
#endif
		[HttpGet("[action]")]
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public Dictionary<string, object> GetModuleItem([FromQuery] string ModuleName, [FromQuery] Guid ID)
		{
			if ( Sql.IsEmptyString(ModuleName) )
				throw(new Exception("The module name must be specified."));
			string sTABLE_NAME = Sql.ToString(Application["Modules." + ModuleName + ".TableName"]);
			// 05/26/2020 Paul.  Product Catalog is different than Product Templates. 
			if ( ModuleName == "ProductCatalog" )
				sTABLE_NAME = "PRODUCT_CATALOG";
			// 05/26/2020 Paul.  The Activities module collies with the Calendar list, so we have to make an exception. 
			if ( ModuleName == "Activities" )
				sTABLE_NAME = "vwACTIVITIES";
			// 05/26/2020 Paul.  The Employees module refers to the USERS table, so correct. 
			if ( ModuleName == "Employees" )
				sTABLE_NAME = "vwEMPLOYEES_Sync";
			// 05/19/2021 Paul.  ReportRules is based off of RULES table, but that is also used by RulesWizard. 
			if ( ModuleName == "ReportRules" )
				sTABLE_NAME = "vwREPORT_RULES";
			if ( Sql.IsEmptyString(sTABLE_NAME) )
				throw(new Exception("Unknown module: " + ModuleName));
			// 08/22/2011 Paul.  Add admin control to REST API. 
			int nACLACCESS = Security.GetUserAccess(ModuleName, "view");
			if ( !Security.IsAuthenticated() || !Sql.ToBoolean(Application["Modules." + ModuleName + ".RestEnabled"]) || nACLACCESS < 0 )
			{
				// 09/06/2017 Paul.  Include module name in error. 
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS") + ": " + Sql.ToString(ModuleName)));
			}
			// 12/03/2019 Paul.  The React Client needs access to archive data. 
			bool   bArchiveView = Sql.ToBoolean(Request.Query["$archiveView"]);
			// 12/03/2019 Paul.  Provide a way to elevate the security check. 
			AccessMode enumAccessMode = AccessMode.view;
			string sAccessMode  = Sql.ToString (Request.Query["$accessMode" ]);
			if ( sAccessMode == "edit" )
				enumAccessMode = AccessMode.edit;
			
			Guid[] arrITEMS = new Guid[1] { ID };
			// 06/17/2013 Paul.  Add support for GROUP BY. 
			// 04/21/2017 Paul.  We need to return the total when using nTOP. 
			long lTotalCount = 0;
			// 05/21/2017 Paul.  HTML5 Dashboard requires aggregates. 
			// 08/01/2019 Paul.  We need a ListView and EditView flags for the Rest Client. 
			// 09/09/2019 Paul.  Send duplicate filter info. 
			// 10/26/2019 Paul.  Return the SQL to the React Client. 
			StringBuilder sbDumpSQL = new StringBuilder();
			// 12/16/2019 Paul.  Moved GetTable to ~/_code/RestUtil.cs
			DataTable dt = RestUtil.GetTable(sTABLE_NAME, 0, 1, String.Empty, String.Empty, String.Empty, null, arrITEMS, ref lTotalCount, null, enumAccessMode, bArchiveView, String.Empty, sbDumpSQL);
			if ( dt == null || dt.Rows.Count == 0 )
				throw(new Exception("Item not found: " + ModuleName + " " + ID.ToString()));
			
			string sBaseURI = Request.Scheme + "://" + Request.Host.Host + Request.Path.Value;
			
			// 04/01/2020 Paul.  Move json utils to RestUtil. 
			Dictionary<string, object> dict = RestUtil.ToJson(sBaseURI, ModuleName, dt.Rows[0], T10n);
			// 04/28/2019 Paul.  Add tracker for React client. 
			if ( dt.Columns.Contains("NAME") )
			{
				string sName = Sql.ToString(dt.Rows[0]["NAME"]);
				try
				{
					// 11/25/2020 Paul.  Correct the action. 
					if ( sAccessMode == "edit" )
						SqlProcs.spTRACKER_Update(Security.USER_ID, ModuleName, ID, sName, "save");
					else
						SqlProcs.spTRACKER_Update(Security.USER_ID, ModuleName, ID, sName, "detailview");
				}
				catch(Exception ex)
				{
					// 04/28/2019 Paul.  There is no compelling reason to send this error to the user. 
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			
			string sEXPAND = Sql.ToString (Request.Query["$expand"]);
			if ( sEXPAND == "*" )
			{
				SplendidCRM.DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					Dictionary<string, object> d       = dict["d"] as Dictionary<string, object>;
					Dictionary<string, object> results = d["results"] as Dictionary<string, object>;
					DataTable dtRelationships = SplendidCache.DetailViewRelationships(ModuleName + ".DetailView");
					foreach ( DataRow row in dtRelationships.Rows )
					{
						try
						{
							string sRELATED_MODULE     = Sql.ToString(row["MODULE_NAME"]);
							string sRELATED_TABLE      = Sql.ToString(Application["Modules." + sRELATED_MODULE + ".TableName"]);
							string sRELATED_FIELD_NAME = SplendidCRM.Crm.Modules.SingularTableName(sRELATED_TABLE) + "_ID";
							if ( !d.ContainsKey(sRELATED_MODULE) && Security.GetUserAccess(sRELATED_MODULE, "list") >= 0 )
							{
								using ( DataTable dtSYNC_TABLES = SplendidCache.RestTables(sRELATED_TABLE, true) )
								{
									string sSQL = String.Empty;
									if ( dtSYNC_TABLES != null && dtSYNC_TABLES.Rows.Count > 0 )
									{
										UniqueStringCollection arrSearchFields = new UniqueStringCollection();
										SplendidDynamic.SearchGridColumns(ModuleName + "." + sRELATED_MODULE, arrSearchFields);
										
										// 07/01/2018 Paul.  Add data privacy flag for the module. 
										bool bIS_DATA_PRIVACY_MODULE = false;
										if ( Config.enable_data_privacy() && dtSYNC_TABLES.Columns.Contains("IS_DATA_PRIVACY_MODULE") )
											bIS_DATA_PRIVACY_MODULE = Sql.ToBoolean(dtSYNC_TABLES.Rows[0]["IS_DATA_PRIVACY_MODULE"]);
										string sVIEW_NAME = "vw" + sTABLE_NAME + "_" + sRELATED_TABLE;
										sSQL = "select " + Sql.FormatSelectFields(arrSearchFields)
											+ (bIS_DATA_PRIVACY_MODULE ? Sql.AppendDataPrivacyField(sVIEW_NAME) : String.Empty)
										     + "  from " + sVIEW_NAME + ControlChars.CrLf;
										using ( IDbCommand cmd = con.CreateCommand() )
										{
											cmd.CommandText = sSQL;
											Security.Filter(cmd, sRELATED_MODULE, "list");
											Sql.AppendParameter(cmd, ID, sRELATED_FIELD_NAME);
											sbDumpSQL.Append(";" + ControlChars.CrLf + Sql.ExpandParameters(cmd));
											
											using ( DbDataAdapter da = dbf.CreateDataAdapter() )
											{
												((IDbDataAdapter)da).SelectCommand = cmd;
												using ( DataTable dtSubPanel = new DataTable() )
												{
													da.Fill(dtSubPanel);
													// 04/01/2020 Paul.  Move json utils to RestUtil. 
													results.Add(sRELATED_MODULE, RestUtil.RowsToDictionary(sBaseURI, sRELATED_MODULE, dtSubPanel, T10n));
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
						}
					}
				}
			}
			
			// 10/26/2019 Paul.  Return the SQL to the React Client. 
			if ( Sql.ToBoolean(Application["CONFIG.show_sql"]) )
			{
				dict.Add("__sql", sbDumpSQL.ToString());
			}
			return dict;
		}

		// 08/19/2019 Paul.  The React Client needs access to audit data. 
		[HttpGet("[action]")]
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public Dictionary<string, object> GetModuleAudit([FromQuery] string ModuleName, [FromQuery] Guid ID)
		{
			if ( Sql.IsEmptyString(ModuleName) )
				throw(new Exception("The module name must be specified."));
			string sTABLE_NAME = Sql.ToString(Application["Modules." + ModuleName + ".TableName"]);
			if ( Sql.IsEmptyString(sTABLE_NAME) )
				throw(new Exception("Unknown module: " + ModuleName));
			// 08/22/2011 Paul.  Add admin control to REST API. 
			int nACLACCESS = Security.GetUserAccess(ModuleName, "view");
			if ( !Security.IsAuthenticated() || !Sql.ToBoolean(Application["Modules." + ModuleName + ".RestEnabled"]) || nACLACCESS < 0 )
			{
				// 09/06/2017 Paul.  Include module name in error. 
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS") + ": " + Sql.ToString(ModuleName)));
			}
			// 05/21/2017 Paul.  HTML5 Dashboard requires aggregates. 
			// 08/01/2019 Paul.  We need a ListView and EditView flags for the Rest Client. 
			string        sNAME     = String.Empty;
			StringBuilder sbDumpSQL = new StringBuilder();
			// 10/31/2021 Paul.  Moved GetAuditData to ModuleUtils. 
			DataTable     dt        = Audit.GetAuditData(ModuleName, ID, ref sNAME, sbDumpSQL);
			// 08/27/2019 Paul.  New records will not have any items.  This is not an error. 
			//if ( dt == null || dt.Rows.Count == 0 )
			//	throw(new Exception("Item not found: " + ModuleName + " " + ID.ToString()));
			
			string sBaseURI = Request.Scheme + "://" + Request.Host.Host + Request.Path.Value;
			
			DataView vwMain = new DataView(dt);
			vwMain.Sort = "DATE_CREATED desc, FIELD_NAME asc";
			// 04/01/2020 Paul.  Move json utils to RestUtil. 
			Dictionary<string, object> dict = RestUtil.ToJson(sBaseURI, ModuleName, vwMain, T10n);
			// 03/11/2021 Paul.  Return the SQL to the React Client. 
			if ( Sql.ToBoolean(Application["CONFIG.show_sql"]) )
			{
				dict.Add("__sql", sbDumpSQL.ToString());
			}
			return dict;
		}

		[HttpGet("[action]")]
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public Dictionary<string, object> GetModuleItemByAudit([FromQuery] string ModuleName, [FromQuery] Guid AUDIT_ID)
		{
			if ( Sql.IsEmptyString(ModuleName) )
				throw(new Exception("The module name must be specified."));
			string sTABLE_NAME = Sql.ToString(Application["Modules." + ModuleName + ".TableName"]);
			if ( Sql.IsEmptyString(sTABLE_NAME) )
				throw(new Exception("Unknown module: " + ModuleName));
			// 08/22/2011 Paul.  Add admin control to REST API. 
			int nACLACCESS = Security.GetUserAccess(ModuleName, "view");
			if ( !Security.IsAuthenticated() || !Sql.ToBoolean(Application["Modules." + ModuleName + ".RestEnabled"]) || nACLACCESS < 0 )
			{
				// 09/06/2017 Paul.  Include module name in error. 
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS") + ": " + Sql.ToString(ModuleName)));
			}
			Guid          gID          = Guid.Empty;
			bool          bArchiveView = false;
			StringBuilder sbDumpSQL    = new StringBuilder();
			// 10/31/2021 Paul.  Moved GetAuditData to ModuleUtils. 
			Audit.GetAuditData(ModuleName, AUDIT_ID, ref gID, ref bArchiveView, sbDumpSQL);
			
			string sBaseURI = Request.Scheme + "://" + Request.Host.Host + Request.Path.Value;
			
			Dictionary<string, object> dict = new Dictionary<string, object>();
			Dictionary<string, object> results = new Dictionary<string, object>();
			Dictionary<string, object> data = new Dictionary<string, object>();
			data.Add("ID"         , gID         );
			data.Add("ArchiveView", bArchiveView);
			results.Add("results" , data        );
			dict.Add("d", results);
			if ( Sql.ToBoolean(Application["CONFIG.show_sql"]) )
			{
				dict.Add("__sql", sbDumpSQL.ToString());
			}
			return dict;
		}

		// 08/19/2019 Paul.  The React Client needs access to audit data. 
		[HttpGet("[action]")]
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public Dictionary<string, object> GetModulePersonal([FromQuery] string ModuleName, [FromQuery] Guid ID)
		{
			if ( Sql.IsEmptyString(ModuleName) )
				throw(new Exception("The module name must be specified."));
			string sTABLE_NAME = Sql.ToString(Application["Modules." + ModuleName + ".TableName"]);
			if ( Sql.IsEmptyString(sTABLE_NAME) )
				throw(new Exception("Unknown module: " + ModuleName));
			// 08/22/2011 Paul.  Add admin control to REST API. 
			int nACLACCESS = Security.GetUserAccess(ModuleName, "view");
			if ( !Security.IsAuthenticated() || !Sql.ToBoolean(Application["Modules." + ModuleName + ".RestEnabled"]) || nACLACCESS < 0 )
			{
				// 09/06/2017 Paul.  Include module name in error. 
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS") + ": " + Sql.ToString(ModuleName)));
			}
			// 05/21/2017 Paul.  HTML5 Dashboard requires aggregates. 
			// 08/01/2019 Paul.  We need a ListView and EditView flags for the Rest Client. 
			string        sNAME     = String.Empty;
			StringBuilder sbDumpSQL = new StringBuilder();
			// 10/31/2021 Paul.  Moved GetAuditData to ModuleUtils from Audit/PopupPersonalInfo. 
			DataTable     dt        = AuditPersonalInfo.GetAuditData(ModuleName, ID, ref sNAME, sbDumpSQL);
			// 08/27/2019 Paul.  New records will not have any items.  This is not an error. 
			//if ( dt == null || dt.Rows.Count == 0 )
			//	throw(new Exception("Item not found: " + ModuleName + " " + ID.ToString()));
			
			string sBaseURI = Request.Scheme + "://" + Request.Host.Host + Request.Path.Value;
			// 04/01/2020 Paul.  Move json utils to RestUtil. 
			Dictionary<string, object> dict = RestUtil.ToJson(sBaseURI, ModuleName, dt, T10n);
			// 03/11/2021 Paul.  Return the SQL to the React Client. 
			if ( Sql.ToBoolean(Application["CONFIG.show_sql"]) )
			{
				dict.Add("__sql", sbDumpSQL.ToString());
			}
			return dict;
		}

		// 05/15/2020 Paul.  The React Client needs to get the Edit fields for the EmailTemplates editor. 
		[HttpGet("[action]")]
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public Dictionary<string, object> GetSqlColumns([FromQuery] string ModuleName, [FromQuery] string Mode)
		{
			if ( Sql.IsEmptyString(ModuleName) )
				throw(new Exception("The module name must be specified."));
			// 08/22/2011 Paul.  Add admin control to REST API. 
			if ( !Security.IsAuthenticated() )
			{
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}
			
			DataTable dt = null;
			if ( Mode == "import" )
			{
				dt = SplendidCache.ImportColumns(ModuleName);
			}
			else
			{
				dt = SplendidCache.SqlColumns(ModuleName, Mode);
			}
			
			string sBaseURI = Request.Scheme + "://" + Request.Host.Host + Request.Path.Value;
			
			// 04/01/2020 Paul.  Move json utils to RestUtil. 
			Dictionary<string, object> dictResponse = RestUtil.ToJson(sBaseURI, ModuleName, dt, T10n);
			return dictResponse;
		}

		// 07/31/2022 Paul.  GetRelationshipInsights is used by new theme. 
		[HttpGet("[action]")]
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public Dictionary<string, object> GetRelationshipInsights([FromQuery] string ModuleName, [FromQuery] Guid ID)
		{
			if ( Sql.IsEmptyString(ModuleName) )
				throw(new Exception("The module name must be specified."));
			int nACLACCESS = Security.GetUserAccess(ModuleName, "edit");
			if ( !Security.IsAuthenticated() || !Sql.ToBoolean(Application["Modules." + ModuleName + ".RestEnabled"]) || nACLACCESS < 0 )
			{
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS") + ": " + ModuleName));
			}
			Guid gCURRENCY_ID = Sql.ToGuid(Session["USER_SETTINGS/CURRENCY"]);
			Currency C10n = Currency.CreateCurrency(gCURRENCY_ID);
			
			Dictionary<string, object> d = new Dictionary<string, object>();
			StringBuilder sbDumpSQL = new StringBuilder();
			SplendidCRM.DbProviderFactory dbf = DbProviderFactories.GetFactory();
			DataTable dtRelationships = SplendidCache.DetailViewRelationships(ModuleName + ".DetailView");
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				con.Open();
				foreach ( DataRow row in dtRelationships.Rows )
				{
					string sMODULE_NAME      = Sql.ToString(row["MODULE_NAME"     ]);
					string sCONTROL_NAME     = Sql.ToString(row["CONTROL_NAME"    ]);
					string sTABLE_NAME       = Sql.ToString(row["TABLE_NAME"      ]);
					string sPRIMARY_FIELD    = Sql.ToString(row["PRIMARY_FIELD"   ]);
					string sINSIGHT_VIEW     = Sql.ToString(row["INSIGHT_VIEW"    ]);
					string sINSIGHT_LABEL    = Sql.ToString(row["INSIGHT_LABEL"   ]);
					string sINSIGHT_OPERATOR = Sql.ToString(row["INSIGHT_OPERATOR"]);
					string sSQL              = String.Empty;
					if ( Sql.IsEmptyString(sINSIGHT_VIEW) )
					{
						sINSIGHT_VIEW = sTABLE_NAME;
					}
					if ( Sql.IsEmptyString(sINSIGHT_OPERATOR) )
					{
						if ( sCONTROL_NAME == "ActivityStream" )
							sINSIGHT_OPERATOR = "max(STREAM_DATE)";
						else
							sINSIGHT_OPERATOR = "count(*)";
					}
					if ( Sql.IsEmptyString(sINSIGHT_LABEL) )
					{
						sINSIGHT_LABEL = ".LBL_INSIGHT_TOTAL";
					}
					sSQL = "select " + sINSIGHT_OPERATOR + ControlChars.CrLf
					     + "  from " + sINSIGHT_VIEW     + ControlChars.CrLf;
					Dictionary<string, object> dictControl = new Dictionary<string, object>();
					dictControl.Add("INSIGHT_LABEL", sINSIGHT_LABEL);
					d.Add(sCONTROL_NAME, dictControl);
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						// 03/31/2022 Paul.   We need to apply the same relationship filters as used by each relationship panel. 
						using ( DataTable dtSYNC_TABLES = SplendidCache.RestTables(sTABLE_NAME, false) )
						{
							if ( dtSYNC_TABLES != null && dtSYNC_TABLES.Rows.Count > 0 )
							{
								DataRow rowSYNC_TABLE = dtSYNC_TABLES.Rows[0];
								sMODULE_NAME = Sql.ToString (rowSYNC_TABLE["MODULE_NAME"]);
								string sVIEW_NAME           = Sql.ToString (rowSYNC_TABLE["VIEW_NAME"          ]);
								bool   bHAS_CUSTOM          = Sql.ToBoolean(rowSYNC_TABLE["HAS_CUSTOM"         ]);
								int    nMODULE_SPECIFIC     = Sql.ToInteger(rowSYNC_TABLE["MODULE_SPECIFIC"    ]);
								string sMODULE_FIELD_NAME   = Sql.ToString (rowSYNC_TABLE["MODULE_FIELD_NAME"  ]);
								bool   bIS_RELATIONSHIP     = Sql.ToBoolean(rowSYNC_TABLE["IS_RELATIONSHIP"    ]);
								string sMODULE_NAME_RELATED = Sql.ToString (rowSYNC_TABLE["MODULE_NAME_RELATED"]);
								bool   bIS_ASSIGNED         = Sql.ToBoolean(rowSYNC_TABLE["IS_ASSIGNED"        ]);
								string sASSIGNED_FIELD_NAME = Sql.ToString (rowSYNC_TABLE["ASSIGNED_FIELD_NAME"]);
								bool   bIS_SYSTEM           = Sql.ToBoolean(rowSYNC_TABLE["IS_SYSTEM"          ]);
								//sTABLE_NAME        = r.Replace(sTABLE_NAME       , "");
								//sVIEW_NAME         = r.Replace(sVIEW_NAME        , "");
								//sMODULE_FIELD_NAME = r.Replace(sMODULE_FIELD_NAME, "");
								// 03/31/2022 Paul.  All tables will be relationship tables, including vwACCOUNTS_BALANCE. 
								if ( bIS_RELATIONSHIP )
								{
									// 03/31/2022 Paul.  We don't want to over-ride the Module Name from DetailViewRelationships, unless we are using the RestTable information. 
									DataView vwRelationships = new DataView(SplendidCache.ReportingRelationships());
									vwRelationships.RowFilter = "(JOIN_TABLE = '" + sTABLE_NAME + "' and RELATIONSHIP_TYPE = 'many-to-many') or (RHS_TABLE = '" + sTABLE_NAME + "' and RELATIONSHIP_TYPE = 'one-to-many')";
									if ( vwRelationships.Count > 0 )
									{
										foreach ( DataRowView rowRelationship in vwRelationships )
										{
											string sJOIN_KEY_LHS             = Sql.ToString(rowRelationship["JOIN_KEY_LHS"            ]).ToUpper();
											string sJOIN_KEY_RHS             = Sql.ToString(rowRelationship["JOIN_KEY_RHS"            ]).ToUpper();
											string sLHS_MODULE               = Sql.ToString(rowRelationship["LHS_MODULE"              ]);
											string sRHS_MODULE               = Sql.ToString(rowRelationship["RHS_MODULE"              ]);
											string sLHS_TABLE                = Sql.ToString(rowRelationship["LHS_TABLE"               ]).ToUpper();
											string sRHS_TABLE                = Sql.ToString(rowRelationship["RHS_TABLE"               ]).ToUpper();
											string sLHS_KEY                  = Sql.ToString(rowRelationship["LHS_KEY"                 ]).ToUpper();
											string sRHS_KEY                  = Sql.ToString(rowRelationship["RHS_KEY"                 ]).ToUpper();
											string sRELATIONSHIP_TYPE        = Sql.ToString(rowRelationship["RELATIONSHIP_TYPE"       ]);
											string sRELATIONSHIP_ROLE_COLUMN = Sql.ToString(rowRelationship["RELATIONSHIP_ROLE_COLUMN"]).ToUpper();
											//sJOIN_KEY_LHS = r.Replace(sJOIN_KEY_LHS, String.Empty);
											//sJOIN_KEY_RHS = r.Replace(sJOIN_KEY_RHS, String.Empty);
											//sLHS_MODULE   = r.Replace(sLHS_MODULE  , String.Empty);
											//sRHS_MODULE   = r.Replace(sRHS_MODULE  , String.Empty);
											//sLHS_TABLE    = r.Replace(sLHS_TABLE   , String.Empty);
											//sRHS_TABLE    = r.Replace(sRHS_TABLE   , String.Empty);
											//sLHS_KEY      = r.Replace(sLHS_KEY     , String.Empty);
											//sRHS_KEY      = r.Replace(sRHS_KEY     , String.Empty);
											if ( sRELATIONSHIP_TYPE == "many-to-many" )
											{
												cmd.CommandText += "   and " + sJOIN_KEY_LHS + " in " + ControlChars.CrLf;
												cmd.CommandText += "(select " + sLHS_KEY + " from " + sLHS_TABLE + ControlChars.CrLf;
												Security.Filter(cmd, sLHS_MODULE, "list");
												cmd.CommandText += ")" + ControlChars.CrLf;
													
												// 11/12/2009 Paul.  We don't want to deal with relationships to multiple tables, so just ignore for now. 
												if ( sRELATIONSHIP_ROLE_COLUMN != "RELATED_TYPE" )
												{
													cmd.CommandText += "   and " + sJOIN_KEY_RHS + " in " + ControlChars.CrLf;
													cmd.CommandText += "(select " + sRHS_KEY + " from " + sRHS_TABLE + ControlChars.CrLf;
													Security.Filter(cmd, sRHS_MODULE, "list");
													cmd.CommandText += ")" + ControlChars.CrLf;
												}
											}
											else if ( sRELATIONSHIP_TYPE == "one-to-many" )
											{
												cmd.CommandText += "   and " + sRHS_KEY + " in " + ControlChars.CrLf;
												cmd.CommandText += "(select " + sLHS_KEY + " from " + sLHS_TABLE + ControlChars.CrLf;
												Security.Filter(cmd, sLHS_MODULE, "list");
												cmd.CommandText += ")" + ControlChars.CrLf;
											}
										}
										cmd.CommandText += "  where 1 = 1" + ControlChars.CrLf;
									}
									else
									{
										cmd.CommandText += "  where 1 = 1" + ControlChars.CrLf;
										if ( !Sql.IsEmptyString(sMODULE_NAME))
										{
											// 11/05/2009 Paul.  We could query the foreign key tables to perpare the filters, but that is slow. 
											string sMODULE_TABLE_NAME   = Sql.ToString(Application["Modules." + sMODULE_NAME + ".TableName"]).ToUpper();
											if ( !Sql.IsEmptyString(sMODULE_TABLE_NAME) )
											{
												// 06/04/2011 Paul.  New function to get the singular name. 
												string sMODULE_FIELD_ID = SplendidCRM.Crm.Modules.SingularTableName(sMODULE_TABLE_NAME) + "_ID";
												
												cmd.CommandText += "   and " + sMODULE_FIELD_ID + " in " + ControlChars.CrLf;
												// 03/30/2016 Paul.  Corporate database does not provide direct access to tables.  Must use view. 
												cmd.CommandText += "(select ID from " + (sMODULE_TABLE_NAME.Substring(0, 2).ToUpper() == "VW" ? sMODULE_TABLE_NAME : "vw" + sMODULE_TABLE_NAME) + ControlChars.CrLf;
												Security.Filter(cmd, sMODULE_NAME, "list");
												cmd.CommandText += ")" + ControlChars.CrLf;
											}
										}
										// 11/05/2009 Paul.  We cannot use the standard filter on the Teams table. 
										if ( !Sql.IsEmptyString(sMODULE_NAME_RELATED) && !sMODULE_NAME_RELATED.StartsWith("Team") )
										{
											string sMODULE_TABLE_RELATED = Sql.ToString(Application["Modules." + sMODULE_NAME_RELATED + ".TableName"]).ToUpper();
											if ( !Sql.IsEmptyString(sMODULE_TABLE_RELATED) )
											{
												// 06/04/2011 Paul.  New function to get the singular name. 
												string sMODULE_RELATED_ID = SplendidCRM.Crm.Modules.SingularTableName(sMODULE_TABLE_RELATED) + "_ID";
												
												// 11/05/2009 Paul.  Some tables use ASSIGNED_USER_ID as the relationship ID instead of the USER_ID. 
												if ( sMODULE_RELATED_ID == "USER_ID" && !Sql.IsEmptyString(sASSIGNED_FIELD_NAME) )
													sMODULE_RELATED_ID = sASSIGNED_FIELD_NAME;
												
												cmd.CommandText += "   and " + sMODULE_RELATED_ID + " in " + ControlChars.CrLf;
												// 03/30/2016 Paul.  Corporate database does not provide direct access to tables.  Must use view. 
												cmd.CommandText += "(select ID from " + (sMODULE_TABLE_RELATED.Substring(0, 2).ToUpper() == "VW" ? sMODULE_TABLE_RELATED : "vw" + sMODULE_TABLE_RELATED)  + ControlChars.CrLf;
												Security.Filter(cmd, sMODULE_NAME_RELATED, "list");
												cmd.CommandText += ")" + ControlChars.CrLf;
											}
										}
									}
								}
								else
								{
									Security.Filter(cmd, sMODULE_NAME, "list");
								}
							}
							else
							{
								// 03/31/2022 Paul.  STREAM table will not be returned by SplendidCache.RestTables. 
								Security.Filter(cmd, ModuleName, "list");
							}
							Sql.AppendParameter(cmd, ID, sPRIMARY_FIELD);
							// 04/01/2022 Paul.  Grouping by count returns empty list. 
							if ( sINSIGHT_OPERATOR != "count(*)" )
							{
								cmd.CommandText += " group by " + sPRIMARY_FIELD;
							}
							sbDumpSQL.AppendLine(Sql.ExpandParameters(cmd) + ";");
							try
							{
								string sINSIGHT_VALUE = String.Empty;
								object oINSIGHT_VALUE = cmd.ExecuteScalar();
								if ( oINSIGHT_VALUE == null )
								{
									sINSIGHT_VALUE = "-";
								}
								else if ( oINSIGHT_VALUE.GetType() == typeof(System.DateTime) )
								{
									if ( oINSIGHT_VALUE == DBNull.Value )
										sINSIGHT_VALUE = "-";
									else
										sINSIGHT_VALUE = Sql.ToDateString(oINSIGHT_VALUE);
								}
								else if ( oINSIGHT_VALUE.GetType() == typeof(System.Decimal) )
								{
									// 03/31/2022 Paul.  Assume it is money. 
									Decimal dValue = C10n.ToCurrency(Convert.ToDecimal(oINSIGHT_VALUE));
									sINSIGHT_VALUE = dValue.ToString("c0");
								}
								else if ( oINSIGHT_VALUE != DBNull.Value )
								{
									sINSIGHT_VALUE = Sql.ToString(oINSIGHT_VALUE);
								}
								dictControl.Add("INSIGHT_VALUE", sINSIGHT_VALUE);
							}
							catch(Exception ex)
							{
								dictControl.Add("INSIGHT_VALUE", ex.Message);
							}
						}
					}
				}
			}
			
			Dictionary<string, object> dictResponse = new Dictionary<string, object>();
			dictResponse.Add("d", d);
			if ( Sql.ToBoolean(Application["CONFIG.show_sql"]) )
			{
				dictResponse.Add("__sql", sbDumpSQL.ToString());
			}
			return dictResponse;
		}
		#endregion

		#region Update
		[DotNetLegacyData]
		[HttpPost("[action]")]
		// 03/13/2011 Paul.  Must use octet-stream instead of json, outherwise we get the following error. 
		// Incoming message for operation 'CreateRecord' (contract 'AddressService' with namespace 'http://tempuri.org/') contains an unrecognized http body format value 'Json'. 
		// The expected body format value is 'Raw'. This can be because a WebContentTypeMapper has not been configured on the binding. See the documentation of WebContentTypeMapper for more details.
		//xhr.setRequestHeader('content-type', 'application/octet-stream');
		// 02/07/2022 Paul.  Must follow old convention of returning value under d.  { d: value }
		public Guid UpdateModuleTable([FromBody] Dictionary<string, object> dict)
		{
			string sTableName = Sql.ToString(Request.Query["TableName"]);
			if ( Sql.IsEmptyString(sTableName) )
				throw(new Exception("The table name must be specified."));
			// 08/22/2011 Paul.  Add admin control to REST API. 
			string sMODULE_NAME = Sql.ToString(Application["Modules." + sTableName + ".ModuleName"]);
			// 08/22/2011 Paul.  Not all tables will have a module name, such as relationship tables. 
			// Tables will get another security filter later in the code. 
			if ( !Sql.IsEmptyString(sMODULE_NAME) )
			{
				int nACLACCESS = Security.GetUserAccess(sMODULE_NAME, "edit");
				if ( !Sql.ToBoolean(Application["Modules." + sMODULE_NAME + ".RestEnabled"]) || nACLACCESS < 0 )
				{
					// 09/06/2017 Paul.  Include module name in error. 
					throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS") + ": " + sMODULE_NAME));
				}
			}
			
			// 04/01/2020 Paul.  Move UpdateTable to RestUtil. 
			Guid gID = RestUtil.UpdateTable(sTableName, dict);
			return gID;
		}

		[DotNetLegacyData]
		[HttpPost("[action]")]
		// 02/07/2022 Paul.  Must follow old convention of returning value under d.  { d: value }
		public Guid UpdateModule([FromBody] Dictionary<string, object> dict)
		{
			string sModuleName = Sql.ToString(Request.Query["ModuleName"]);
			if ( Sql.IsEmptyString(sModuleName) )
				throw(new Exception("The module name must be specified."));
			// 08/22/2011 Paul.  Add admin control to REST API. 
			int nACLACCESS = Security.GetUserAccess(sModuleName, "edit");
			if ( !Security.IsAuthenticated() || !Sql.ToBoolean(Application["Modules." + sModuleName + ".RestEnabled"]) || nACLACCESS < 0 )
			{
				// 09/06/2017 Paul.  Include module name in error. 
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS") + ": " + sModuleName));
			}
			
			string sTableName = Sql.ToString(Application["Modules." + sModuleName + ".TableName"]);
			if ( Sql.IsEmptyString(sTableName) )
				throw(new Exception("Unknown module: " + sModuleName));
			
			// 04/01/2020 Paul.  Move UpdateTable to RestUtil. 
			Guid gID = RestUtil.UpdateTable(sTableName, dict);
			// 04/28/2019 Paul.  Add tracker for React client. 
			if ( dict.ContainsKey("NAME") || dict.ContainsKey("DOCUMENT_NAME") || dict.ContainsKey("FIRST_NAME") )
			{
				string sName = String.Empty;
				if ( dict.ContainsKey("NAME") )
					sName = Sql.ToString(dict["NAME"]);
				else if ( dict.ContainsKey("DOCUMENT_NAME") )
					sName = Sql.ToString(dict["DOCUMENT_NAME"]);
				else
				{
					if ( dict.ContainsKey("FIRST_NAME") )
						sName = Sql.ToString(dict["FIRST_NAME"]);
					if ( dict.ContainsKey("LAST_NAME") )
						sName = (sName + " " + Sql.ToString(dict["LAST_NAME"])).Trim();
				}
				try
				{
					if ( !Sql.IsEmptyString(sName) )
						SqlProcs.spTRACKER_Update(Security.USER_ID, sModuleName, gID, sName, "save");
				}
				catch(Exception ex)
				{
					// 04/28/2019 Paul.  There is no compelling reason to send this error to the user. 
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return gID;
		}

		// 04/01/2020 Paul.  Move UpdateTable to RestUtil. 

		// 07/17/2019 Paul.  MassUpdateModule is a simplified version of UpdateModule. 
		[HttpPost("[action]")]
		public void MassUpdateModule([FromBody] Dictionary<string, object> dict)
		{
			string sModuleName = Sql.ToString(Request.Query["ModuleName"]);
			if ( Sql.IsEmptyString(sModuleName) )
				throw(new Exception("The module name must be specified."));
			// 08/22/2011 Paul.  Add admin control to REST API. 
			int nACLACCESS = Security.GetUserAccess(sModuleName, "edit");
			if ( !Security.IsAuthenticated() || !Sql.ToBoolean(Application["Modules." + sModuleName + ".RestEnabled"]) || nACLACCESS < 0 )
			{
				// 09/06/2017 Paul.  Include module name in error. 
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS") + ": " + sModuleName));
			}
			
			string sTABLE_NAME = Sql.ToString(Application["Modules." + sModuleName + ".TableName"]);
			if ( Sql.IsEmptyString(sTABLE_NAME) )
				throw(new Exception("Unknown module: " + sModuleName));
			
			try
			{
				DataTable dtUPDATE = new DataTable(sTABLE_NAME);
				foreach ( string sColumnName in dict.Keys )
				{
					if ( sColumnName != "ID" && sColumnName != "ID_LIST")
					{
						// 01/20/2021 Paul.  We need to filter out empty or null values before SplendidDynamic.UpdateCustomFields() as there is no filter on empty fields in that method. 
						if ( dict[sColumnName] is System.Collections.ArrayList )
						{
							System.Collections.ArrayList lst = dict[sColumnName] as System.Collections.ArrayList;
							if ( lst.Count > 0 )
							{
								dtUPDATE.Columns.Add(sColumnName.ToUpper());
							}
						}
						else
						{
							if ( !Sql.IsEmptyString(dict[sColumnName]) )
							{
								dtUPDATE.Columns.Add(sColumnName.ToUpper());
							}
						}
					}
				}
				List<Guid> arrID_LIST = new List<Guid>();
				DataRow row = dtUPDATE.NewRow();
				dtUPDATE.Rows.Add(row);
				foreach ( string sColumnName in dict.Keys )
				{
					// 09/09/2011 Paul.  Multi-selection list boxes will come in as an ArrayList. 
					if ( dict[sColumnName] is System.Collections.ArrayList )
					{
						System.Collections.ArrayList lst = dict[sColumnName] as System.Collections.ArrayList;
						if ( sColumnName == "ID_LIST" )
						{
							if ( lst.Count > 0 )
							{
								foreach ( string item in lst )
								{
									arrID_LIST.Add(new Guid(item));
								}
							}
						}
						else
						{
							// 03/04/2016 Paul.  Line items will be included with Quotes, Orders and Invoices. 
							{
								XmlDocument xml = new XmlDocument();
								xml.AppendChild(xml.CreateXmlDeclaration("1.0", "UTF-8", null));
								xml.AppendChild(xml.CreateElement("Values"));
								if ( lst.Count > 0 )
								{
									foreach ( string item in lst )
									{
										XmlNode xValue = xml.CreateElement("Value");
										xml.DocumentElement.AppendChild(xValue);
										xValue.InnerText = item;
									}
								}
								// 01/20/2021 Paul.  Column may not exist now that we filter empty. 
								if ( dtUPDATE.Columns.Contains(sColumnName) )
								{
									row[sColumnName] = xml.OuterXml;
								}
							}
						}
					}
					else if ( sColumnName != "ID" && sColumnName != "ID_LIST")
					{
						// 01/20/2021 Paul.  Column may not exist now that we filter empty. 
						if ( dtUPDATE.Columns.Contains(sColumnName) )
						{
							row[sColumnName] = dict[sColumnName];
						}
					}
				}
				
				if ( arrID_LIST.Count == 0 )
				{
					throw(new Exception(L10n.Term(".LBL_LISTVIEW_NO_SELECTED")));
				}
				
				SplendidCRM.DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					// 06/03/2011 Paul.  Cache the Rest Table data. 
					// 11/26/2009 Paul.  System tables cannot be updated. 
					using ( DataTable dtSYNC_TABLES = SplendidCache.RestTables(sTABLE_NAME, true) )
					{
						string sSQL = String.Empty;
						if ( dtSYNC_TABLES != null && dtSYNC_TABLES.Rows.Count > 0 )
						{
							DataRow rowSYNC_TABLE = dtSYNC_TABLES.Rows[0];
							string sMODULE_NAME = Sql.ToString (rowSYNC_TABLE["MODULE_NAME"]);
							string sVIEW_NAME   = Sql.ToString (rowSYNC_TABLE["VIEW_NAME"  ]);
							bool   bHAS_CUSTOM  = Sql.ToBoolean(rowSYNC_TABLE["HAS_CUSTOM" ]);
							// 02/14/2010 Paul.  GetUserAccess requires a non-null sMODULE_NAME. 
							// Lets catch the exception here so that we can throw a meaningful error. 
							if ( Sql.IsEmptyString(sMODULE_NAME) )
							{
								throw(new Exception("sMODULE_NAME should not be empty for table " + sTABLE_NAME));
							}
							
							bool bEnableTeamManagement  = Config.enable_team_management();
							bool bRequireTeamManagement = Config.require_team_management();
							bool bRequireUserAssignment = Config.require_user_assignment();
							DataTable dtMetadata     = SplendidCache.SqlColumns(sTABLE_NAME);
							DataTable dtCustomFields = SplendidCache.FieldsMetaData_Validated(sTABLE_NAME);
							using ( IDbTransaction trn = Sql.BeginTransaction(con) )
							{
								try
								{
									IDbCommand cmdUpdate = null;
									cmdUpdate = SqlProcs.Factory(con, "sp" + sTABLE_NAME + "_Update");
									cmdUpdate.Transaction = trn;
									// 02/13/2018 Paul.  Azure can timeout, so lets wait for an hour. 
									cmdUpdate.CommandTimeout = 60 * 60;
									foreach ( Guid gID in arrID_LIST )
									{
										bool      bRecordExists              = false;
										bool      bAccessAllowed             = false;
										Guid      gLOCAL_ASSIGNED_USER_ID    = Guid.Empty;
										DataRow   rowCurrent                 = null;
										DataTable dtCurrent                  = new DataTable();
										// 10/05/2020 Paul.  Must use view not table. 
										sSQL = "select *"              + ControlChars.CrLf
										     + "  from " + sVIEW_NAME  + ControlChars.CrLf
										     + " where 1 = 1"          + ControlChars.CrLf;
										using ( IDbCommand cmd = con.CreateCommand() )
										{
											cmd.CommandText = sSQL;
											cmd.Transaction = trn;
											Sql.AppendParameter(cmd, gID, "ID");
											using ( DbDataAdapter da = dbf.CreateDataAdapter() )
											{
												((IDbDataAdapter)da).SelectCommand = cmd;
												// 11/27/2009 Paul.  It may be useful to log the SQL during errors at this location. 
												try
												{
													da.Fill(dtCurrent);
												}
												catch
												{
													SplendidError.SystemError(new StackTrace(true).GetFrame(0), Sql.ExpandParameters(cmd));
													throw;
												}
												if ( dtCurrent.Rows.Count > 0 )
												{
													rowCurrent = dtCurrent.Rows[0];
													bRecordExists = true;
													// 01/18/2010 Paul.  Apply ACL Field Security. 
													if ( dtCurrent.Columns.Contains("ASSIGNED_USER_ID") )
													{
														gLOCAL_ASSIGNED_USER_ID = Sql.ToGuid(rowCurrent["ASSIGNED_USER_ID"]);
													}
												}
											}
										}
										if ( bRecordExists )
										{
											// 07/17/2019 Paul.  Now perform a request to determine with access rights applied. 
											// 10/05/2020 Paul.  Must use view not table. 
											sSQL = "select count(*)"       + ControlChars.CrLf
											     + "  from " + sVIEW_NAME  + ControlChars.CrLf;
											using ( IDbCommand cmd = con.CreateCommand() )
											{
												cmd.CommandText = sSQL;
												cmd.Transaction = trn;
												Security.Filter(cmd, sMODULE_NAME, "edit");
												Sql.AppendParameter(cmd, gID, "ID");
												try
												{
													if ( Sql.ToInteger(cmd.ExecuteScalar()) > 0 )
													{
														if ( (nACLACCESS > ACL_ACCESS.OWNER) || (nACLACCESS == ACL_ACCESS.OWNER && Security.USER_ID == gLOCAL_ASSIGNED_USER_ID) || !dtCurrent.Columns.Contains("ASSIGNED_USER_ID") )
															bAccessAllowed = true;
													}
												}
												catch
												{
													SplendidError.SystemError(new StackTrace(true).GetFrame(0), Sql.ExpandParameters(cmd));
													throw;
												}
											}
										}
										if ( bRecordExists && bAccessAllowed )
										{
											// 10/05/2020 Paul.  We need to manually hande Replace/Add of users, teams and tags. 
											string sASSIGNED_SET_LIST = rowCurrent.Table.Columns.Contains("ASSIGNED_SET_LIST") ? Sql.ToString(rowCurrent["ASSIGNED_SET_LIST"]) : String.Empty;
											string sTEAM_SET_LIST     = rowCurrent.Table.Columns.Contains("TEAM_SET_LIST"    ) ? Sql.ToString(rowCurrent["TEAM_SET_LIST"    ]) : String.Empty;
											string sTAG_SET_NAME      = rowCurrent.Table.Columns.Contains("TAG_SET_NAME"     ) ? Sql.ToString(rowCurrent["TAG_SET_NAME"     ]) : String.Empty;
											string sNAICS_SET_NAME    = rowCurrent.Table.Columns.Contains("NAICS_SET_NAME"   ) ? Sql.ToString(rowCurrent["NAICS_SET_NAME"   ]) : String.Empty;
											if ( row.Table.Columns.Contains("ASSIGNED_SET_LIST") && !Sql.IsEmptyString(row["ASSIGNED_SET_LIST"]) )
											{
												if ( row.Table.Columns.Contains("ASSIGNED_SET_ADD") && Sql.ToBoolean(row["ASSIGNED_SET_ADD"]) )
												{
													if ( !Sql.IsEmptyString(sASSIGNED_SET_LIST) )
														sASSIGNED_SET_LIST += ",";
													sASSIGNED_SET_LIST += Sql.ToString(row["ASSIGNED_SET_LIST"]);
												}
												else
												{
													sASSIGNED_SET_LIST = Sql.ToString(row["ASSIGNED_SET_LIST"]);
												}
											}
											if ( row.Table.Columns.Contains("TEAM_SET_LIST") && !Sql.IsEmptyString(row["TEAM_SET_LIST"]) )
											{
												if ( row.Table.Columns.Contains("TEAM_SET_ADD") && Sql.ToBoolean(row["TEAM_SET_ADD"]) )
												{
													if ( !Sql.IsEmptyString(sTEAM_SET_LIST) )
														sTEAM_SET_LIST += ",";
													sTEAM_SET_LIST += Sql.ToString(row["TEAM_SET_LIST"]);
												}
												else
												{
													sTEAM_SET_LIST = Sql.ToString(row["TEAM_SET_LIST"]);
												}
											}
											if ( row.Table.Columns.Contains("TAG_SET_NAME") && !Sql.IsEmptyString(row["TAG_SET_NAME"]) )
											{
												if ( row.Table.Columns.Contains("TAG_SET_ADD") && Sql.ToBoolean(row["TAG_SET_ADD"]) )
												{
													if ( !Sql.IsEmptyString(sTAG_SET_NAME) )
														sTAG_SET_NAME += ",";
													sTAG_SET_NAME += Sql.ToString(row["TAG_SET_NAME"]);
												}
												else
												{
													sTAG_SET_NAME = Sql.ToString(row["TAG_SET_NAME"]);
												}
											}
											if ( row.Table.Columns.Contains("NAICS_SET_NAME") && !Sql.IsEmptyString(row["NAICS_SET_NAME"]) )
											{
												if ( row.Table.Columns.Contains("ADD_NAICS_CODE_SET") && Sql.ToBoolean(row["ADD_NAICS_CODE_SET"]) )
												{
													if ( !Sql.IsEmptyString(sNAICS_SET_NAME) )
														sNAICS_SET_NAME += ",";
													sNAICS_SET_NAME += Sql.ToString(row["NAICS_SET_NAME"]);
												}
												else
												{
													sNAICS_SET_NAME = Sql.ToString(row["NAICS_SET_NAME"]);
												}
											}

											// 07/17/2019 Paul.  We need to null all procedure parameters. 
											foreach(IDbDataParameter par in cmdUpdate.Parameters)
											{
												string sParameterName = Sql.ExtractDbName(cmdUpdate, par.ParameterName).ToUpper();
												if ( sParameterName == "ID" )
													par.Value = gID;
												// 10/09/2020 Paul.  Not sure why were were not setting modified user. 
												else if ( sParameterName == "MODIFIED_USER_ID" )
													par.Value = Sql.ToDBGuid(Security.USER_ID);
												else
													par.Value = DBNull.Value;
											}
											// 07/17/2019 Paul.  Now initialize values with current record values. 
											foreach ( DataColumn col in rowCurrent.Table.Columns )
											{
												IDbDataParameter par = Sql.FindParameter(cmdUpdate, col.ColumnName);
												// 11/26/2009 Paul.  The UTC modified date should be set to Now. 
												// 10/09/2020 Paul.  Not sure why were were not setting modified user. 
												if ( par != null && String.Compare(col.ColumnName, "MODIFIED_USER_ID", true) != 0 && String.Compare(col.ColumnName, "DATE_MODIFIED_UTC", true) != 0 )
													par.Value = rowCurrent[col.ColumnName];
											}
											foreach ( DataColumn col in row.Table.Columns )
											{
												// 01/18/2010 Paul.  Apply ACL Field Security. 
												// 02/01/2010 Paul.  System tables may not have a valid Module name, so Field Security will not apply. 
												bool bIsWriteable = true;
												if ( SplendidInit.bEnableACLFieldSecurity && !Sql.IsEmptyString(sMODULE_NAME) )
												{
													Security.ACL_FIELD_ACCESS acl = Security.GetUserFieldSecurity(sMODULE_NAME, col.ColumnName, Guid.Empty);
													bIsWriteable = acl.IsWriteable();
												}
												if ( bIsWriteable )
												{
													IDbDataParameter par = Sql.FindParameter(cmdUpdate, col.ColumnName);
													// 11/26/2009 Paul.  The UTC modified date should be set to Now. 
													// 10/05/2020 Paul.  Do not update values that are null. 
													if ( par != null && row[col.ColumnName] != null && row[col.ColumnName] != DBNull.Value && !Sql.IsEmptyString(row[col.ColumnName]) )
													{
														// 10/05/2020 Paul.  We need to manually hande Replace/Add of users, teams and tags. 
														if ( col.ColumnName == "ASSIGNED_SET_LIST" && !Sql.IsEmptyString(sASSIGNED_SET_LIST) )
														{
															par.Value = sASSIGNED_SET_LIST;
														}
														else if ( col.ColumnName == "TEAM_SET_LIST" && !Sql.IsEmptyString(sTEAM_SET_LIST) )
														{
															par.Value = sTEAM_SET_LIST;
														}
														else if ( col.ColumnName == "TAG_SET_NAME" && !Sql.IsEmptyString(sTAG_SET_NAME) )
														{
															par.Value = sTAG_SET_NAME;
														}
														else if ( col.ColumnName == "NAICS_SET_NAME" && !Sql.IsEmptyString(sNAICS_SET_NAME) )
														{
															par.Value = sNAICS_SET_NAME;
														}
														else
														{
															// 05/22/2017 Paul.  Shared function to convert from Json to DB. 
															// 04/01/2020 Paul.  Move json utils to RestUtil. 
															par.Value = RestUtil.DBValueFromJsonValue(par.DbType, row[col.ColumnName], T10n);
														}
													}
												}
											}
											// 02/13/2018 Paul.  We should be using ExecuteNonQuery instead of ExecuateScalar. 
											cmdUpdate.ExecuteNonQuery();
											if ( bHAS_CUSTOM )
											{
												SplendidDynamic.UpdateCustomFields(row, trn, gID, sTABLE_NAME, dtCustomFields);
											}
										}
										else
										{
											throw(new Exception(L10n.Term("ACL.LBL_NO_ACCESS")));
										}
									}
									trn.Commit();
								}
								catch(Exception ex)
								{
									// 02/13/2017 Paul.  Capture this error as the following can generate an "This SqlTransaction has completed" error on Azure. 
									try
									{
										trn.Rollback();
									}
									catch
									{
									}
									SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
									throw;
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

		public class UpdateRelatedItemParameters
		{
			public string   ModuleName    { get; set; }
			public Guid     ID            { get; set; }
			public string   RelatedModule { get; set; }
			public Guid     RelatedID     { get; set; }
		}

		[HttpPost("[action]")]
		public void UpdateRelatedItem([FromBody] UpdateRelatedItemParameters input)
		{
			string ModuleName    = Sql.ToString(input.ModuleName   );
			Guid   ID            = Sql.ToGuid  (input.ID           );
			string RelatedModule = Sql.ToString(input.RelatedModule);
			Guid   RelatedID     = Sql.ToGuid  (input.RelatedID    );
			if ( Sql.IsEmptyString(ModuleName) )
				throw(new Exception("The module name must be specified."));
			string sTABLE_NAME = Sql.ToString(Application["Modules." + ModuleName + ".TableName"]);
			if ( Sql.IsEmptyString(sTABLE_NAME) )
				throw(new Exception("Unknown module: " + ModuleName));
			// 08/22/2011 Paul.  Add admin control to REST API. 
			int nACLACCESS = Security.GetUserAccess(ModuleName, "edit");
			if ( !Security.IsAuthenticated() || !Sql.ToBoolean(Application["Modules." + ModuleName + ".RestEnabled"]) || nACLACCESS < 0 )
			{
				// 09/06/2017 Paul.  Include module name in error. 
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS") + ": " + Sql.ToString(ModuleName)));
			}
			
			if ( Sql.IsEmptyString(RelatedModule) )
				throw(new Exception("The related module name must be specified."));
			string sRELATED_TABLE = Sql.ToString(Application["Modules." + RelatedModule + ".TableName"]);
			if ( Sql.IsEmptyString(sRELATED_TABLE) )
				throw(new Exception("Unknown module: " + RelatedModule));
			// 08/22/2011 Paul.  Add admin control to REST API. 
			nACLACCESS = Security.GetUserAccess(RelatedModule, "edit");
			if ( !Security.IsAuthenticated() || !Sql.ToBoolean(Application["Modules." + RelatedModule + ".RestEnabled"]) || nACLACCESS < 0 )
			{
				// 09/06/2017 Paul.  Include module name in error. 
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS") + ": " + Sql.ToString(RelatedModule)));
			}
			
			string sRELATIONSHIP_TABLE = sTABLE_NAME + "_" + sRELATED_TABLE;
			string sMODULE_FIELD_NAME  = SplendidCRM.Crm.Modules.SingularTableName(sTABLE_NAME   ) + "_ID";
			string sRELATED_FIELD_NAME = SplendidCRM.Crm.Modules.SingularTableName(sRELATED_TABLE) + "_ID";
			// 11/24/2012 Paul.  In the special cases of Accounts Related and Contacts Reports To, we need to correct the field name. 
			if ( sMODULE_FIELD_NAME == "ACCOUNT_ID" && sRELATED_FIELD_NAME == "ACCOUNT_ID" )
			{
				sRELATIONSHIP_TABLE = "ACCOUNTS_MEMBERS";
				sRELATED_FIELD_NAME = "PARENT_ID";
			}
			else if ( sMODULE_FIELD_NAME == "CONTACT_ID" && sRELATED_FIELD_NAME == "CONTACT_ID" )
			{
				sRELATIONSHIP_TABLE = "CONTACTS_DIRECT_REPORTS";
				sRELATED_FIELD_NAME = "REPORTS_TO_ID";
			}
			// 05/27/2020 Paul. Correct some relationships. 
			// 10/15/2020 Paul.  Admin can remove users from teams and teams from users. 
			else if ( sRELATIONSHIP_TABLE == "USERS_TEAMS" || sRELATIONSHIP_TABLE == "TEAMS_USERS" )
			{
				sRELATIONSHIP_TABLE = "USERS_TEAM_MEMBERSHIPS";
			}
			// 03/09/2021 Paul.  Correct ROLE_ID field name. 
			else if ( sRELATIONSHIP_TABLE == "ACL_ROLES_USERS" && sMODULE_FIELD_NAME == "ACL_ROLE_ID" )
			{
				sMODULE_FIELD_NAME = "ROLE_ID";
			}
			// 08/23/2021 Paul.  Correct azure relationships. 
			else if ( sRELATIONSHIP_TABLE == "AZURE_ORDERS_AZURE_APP_UPDATES" )
			{
				sRELATIONSHIP_TABLE = "AZURE_APP_UPDATES_ORDERS";
				sRELATED_FIELD_NAME = "APP_UPDATE_ID";
			}
			else if ( sRELATIONSHIP_TABLE == "AZURE_APP_UPDATES_AZURE_ORDERS" )
			{
				sRELATIONSHIP_TABLE = "AZURE_APP_UPDATES_ORDERS";
				sMODULE_FIELD_NAME  = "APP_UPDATE_ID";
			}
			else if ( sRELATIONSHIP_TABLE == "AZURE_APP_PRICES_AZURE_SERVICE_LEVELS" )
			{
				sRELATIONSHIP_TABLE = "AZURE_APP_SERVICE_LEVELS";
				sMODULE_FIELD_NAME  = "APP_PRICE_ID";
				sRELATED_FIELD_NAME = "SERVICE_LEVEL_ID";
			}
			else if ( sRELATIONSHIP_TABLE == "AZURE_SERVICE_LEVELS_AZURE_APP_PRICES" )
			{
				sRELATIONSHIP_TABLE = "AZURE_APP_SERVICE_LEVELS";
				sMODULE_FIELD_NAME  = "SERVICE_LEVEL_ID";
				sRELATED_FIELD_NAME = "APP_PRICE_ID";
			}
			
			// 06/04/2011 Paul.  For relationships, we first need to check the access rights of the parent record. 
			bool bExcludeSystemTables = true;
			if ( Security.AdminUserAccess(ModuleName, "edit") >= 0 )
			{
				bExcludeSystemTables = false;
			}
			// 02/27/2021 Paul.  Both vwACCOUNTS_PROJECTS and vwPROJECTS_ACCOUNTS are both in RestTables, so we need a secondary check for the update procedure, which might not exist. 
			SplendidCRM.DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				IDbCommand cmdUpdate  = null;
				try
				{
					cmdUpdate = SqlProcs.Factory(con, "sp" + sRELATIONSHIP_TABLE + "_Update");
				}
				catch
				{
				}
				DataTable dtSYNC_TABLES = SplendidCache.RestTables("vw" + sRELATIONSHIP_TABLE, bExcludeSystemTables);
				if ( cmdUpdate == null || (dtSYNC_TABLES != null && dtSYNC_TABLES.Rows.Count == 0) )
				{
					sRELATIONSHIP_TABLE = sRELATED_TABLE + "_" + sTABLE_NAME;
					try
					{
						cmdUpdate = SqlProcs.Factory(con, "sp" + sRELATIONSHIP_TABLE + "_Update");
					}
					catch
					{
					}
					dtSYNC_TABLES = SplendidCache.RestTables("vw" + sRELATIONSHIP_TABLE, bExcludeSystemTables);
					if ( cmdUpdate == null || (dtSYNC_TABLES != null && dtSYNC_TABLES.Rows.Count == 0) )
					{
						throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS") + " to relationship between modules " + ModuleName + " and " + RelatedModule));
					}
				}
				UpdateRelatedItem(sTABLE_NAME, sRELATIONSHIP_TABLE, sMODULE_FIELD_NAME, ID, sRELATED_FIELD_NAME, RelatedID, bExcludeSystemTables);
			}
		}

		private void UpdateRelatedItem(string sTABLE_NAME, string sRELATIONSHIP_TABLE, string sMODULE_FIELD_NAME, Guid gID, string sRELATED_FIELD_NAME, Guid gRELATED_ID, bool bExcludeSystemTables)
		{
			try
			{
				if ( Security.IsAuthenticated() )
				{
					Regex  r        = new Regex(@"[^A-Za-z0-9_]");
					sTABLE_NAME = r.Replace(sTABLE_NAME, "").ToUpper();
					
					SplendidCRM.DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						// 06/03/2011 Paul.  Cache the Rest Table data. 
						// 11/26/2009 Paul.  System tables cannot be updated. 
						// 06/04/2011 Paul.  For relationships, we first need to check the access rights of the parent record. 
						// 05/28/2020 Paul.  USERS table is a system table, and we need to allow spUSERS_ACL_ROLES_Update and spUSERS_TEAM_MEMBERSHIPS_Update for the React Client. 
						// 06/04/2011 Paul.  For relationships, we first need to check the access rights of the parent record. 
						using ( DataTable dtSYNC_TABLES = SplendidCache.RestTables(sTABLE_NAME, bExcludeSystemTables) )
						{
							string sSQL = String.Empty;
							if ( dtSYNC_TABLES != null && dtSYNC_TABLES.Rows.Count > 0 )
							{
								DataRow rowSYNC_TABLE = dtSYNC_TABLES.Rows[0];
								string sMODULE_NAME = Sql.ToString (rowSYNC_TABLE["MODULE_NAME"]);
								string sVIEW_NAME   = Sql.ToString (rowSYNC_TABLE["VIEW_NAME"  ]);
								if ( Sql.IsEmptyString(sMODULE_NAME) )
								{
									throw(new Exception("sMODULE_NAME should not be empty for table " + sTABLE_NAME));
								}
								
								int nACLACCESS = Security.GetUserAccess(sMODULE_NAME, "edit");
								// 11/11/2009 Paul.  First check if the user has access to this module. 
								if ( nACLACCESS >= 0 )
								{
									bool      bRecordExists              = false;
									bool      bAccessAllowed             = false;
									Guid      gLOCAL_ASSIGNED_USER_ID    = Guid.Empty;
									DataRow   rowCurrent                 = null;
									DataTable dtCurrent                  = new DataTable();
									sSQL = "select *"              + ControlChars.CrLf
									     + "  from " + sTABLE_NAME + ControlChars.CrLf
									     + " where DELETED = 0"    + ControlChars.CrLf;
									using ( IDbCommand cmd = con.CreateCommand() )
									{
										cmd.CommandText = sSQL;
										// 02/14/2018 Paul.  Azure can timeout, so lets wait for an hour. 
										cmd.CommandTimeout = 60 * 60;
										Sql.AppendParameter(cmd, gID, "ID");
										using ( DbDataAdapter da = dbf.CreateDataAdapter() )
										{
											((IDbDataAdapter)da).SelectCommand = cmd;
											// 11/27/2009 Paul.  It may be useful to log the SQL during errors at this location. 
											try
											{
												da.Fill(dtCurrent);
											}
											catch
											{
												SplendidError.SystemError(new StackTrace(true).GetFrame(0), Sql.ExpandParameters(cmd));
												throw;
											}
											if ( dtCurrent.Rows.Count > 0 )
											{
												rowCurrent = dtCurrent.Rows[0];
												bRecordExists = true;
												// 01/18/2010 Paul.  Apply ACL Field Security. 
												if ( dtCurrent.Columns.Contains("ASSIGNED_USER_ID") )
												{
													gLOCAL_ASSIGNED_USER_ID = Sql.ToGuid(rowCurrent["ASSIGNED_USER_ID"]);
												}
											}
										}
									}
									// 06/04/2011 Paul.  We are not ready to handle conflicts. 
									//if ( !bConflicted )
									{
										if ( bRecordExists )
										{
											sSQL = "select count(*)"       + ControlChars.CrLf
											     + "  from " + sTABLE_NAME + ControlChars.CrLf;
											using ( IDbCommand cmd = con.CreateCommand() )
											{
												cmd.CommandText = sSQL;
												Security.Filter(cmd, sMODULE_NAME, "edit");
												Sql.AppendParameter(cmd, gID, "ID");
												try
												{
													if ( Sql.ToInteger(cmd.ExecuteScalar()) > 0 )
													{
														if ( (nACLACCESS > ACL_ACCESS.OWNER) || (nACLACCESS == ACL_ACCESS.OWNER && Security.USER_ID == gLOCAL_ASSIGNED_USER_ID) || !dtCurrent.Columns.Contains("ASSIGNED_USER_ID") )
															bAccessAllowed = true;
													}
												}
												catch
												{
													SplendidError.SystemError(new StackTrace(true).GetFrame(0), Sql.ExpandParameters(cmd));
													throw;
												}
											}
										}
										if ( bAccessAllowed )
										{
											// 11/24/2012 Paul.  We do not need to check for RestTable access as that step was already done. 
											IDbCommand cmdUpdate = SqlProcs.Factory(con, "sp" + sRELATIONSHIP_TABLE + "_Update");
											using ( IDbTransaction trn = Sql.BeginTransaction(con) )
											{
												try
												{
													cmdUpdate.Transaction = trn;
													foreach(IDbDataParameter par in cmdUpdate.Parameters)
													{
														string sParameterName = Sql.ExtractDbName(cmdUpdate, par.ParameterName).ToUpper();
														if ( sParameterName == sMODULE_FIELD_NAME )
															par.Value = gID;
														else if ( sParameterName == sRELATED_FIELD_NAME )
															par.Value = gRELATED_ID;
														else if ( sParameterName == "MODIFIED_USER_ID" )
															par.Value = Sql.ToDBGuid(Security.USER_ID);
														else
															par.Value = DBNull.Value;
													}
													cmdUpdate.ExecuteScalar();
													trn.Commit();
												}
												catch(Exception ex)
												{
													// 02/13/2017 Paul.  Capture this error as the following can generate an "This SqlTransaction has completed" error on Azure. 
													try
													{
														trn.Rollback();
													}
													catch
													{
													}
													SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
													throw;
												}
											}
										}
										else
										{
											throw(new Exception(L10n.Term("ACL.LBL_NO_ACCESS")));
										}
									}
								}
								else
								{
									throw(new Exception(L10n.Term("ACL.LBL_NO_ACCESS")));
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

		// 07/09/2019 Paul.  UpdateRelatedList is identical to UpdateRelatedItem but accepts an array. 
		public class UpdateRelatedListParameters
		{
			public string   ModuleName    { get; set; }
			public Guid     ID            { get; set; }
			public string   RelatedModule { get; set; }
			public Guid[]   RelatedList   { get; set; }
		}

		[HttpPost("[action]")]
		public void UpdateRelatedList([FromBody] UpdateRelatedListParameters input)
		{
			string ModuleName    = Sql.ToString(input.ModuleName   );
			Guid   ID            = Sql.ToGuid  (input.ID           );
			string RelatedModule = Sql.ToString(input.RelatedModule);
			Guid[] RelatedList   = input.RelatedList;
			if ( Sql.IsEmptyString(ModuleName) )
				throw(new Exception("The module name must be specified."));
			string sTABLE_NAME = Sql.ToString(Application["Modules." + ModuleName + ".TableName"]);
			if ( Sql.IsEmptyString(sTABLE_NAME) )
				throw(new Exception("Unknown module: " + ModuleName));
			// 08/22/2011 Paul.  Add admin control to REST API. 
			int nACLACCESS = Security.GetUserAccess(ModuleName, "edit");
			if ( !Security.IsAuthenticated() || !Sql.ToBoolean(Application["Modules." + ModuleName + ".RestEnabled"]) || nACLACCESS < 0 )
			{
				// 09/06/2017 Paul.  Include module name in error. 
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS") + ": " + Sql.ToString(ModuleName)));
			}
			
			if ( Sql.IsEmptyString(RelatedModule) )
				throw(new Exception("The related module name must be specified."));
			string sRELATED_TABLE = Sql.ToString(Application["Modules." + RelatedModule + ".TableName"]);
			if ( Sql.IsEmptyString(sRELATED_TABLE) )
				throw(new Exception("Unknown module: " + RelatedModule));
			// 08/22/2011 Paul.  Add admin control to REST API. 
			nACLACCESS = Security.GetUserAccess(RelatedModule, "edit");
			if ( !Security.IsAuthenticated() || !Sql.ToBoolean(Application["Modules." + RelatedModule + ".RestEnabled"]) || nACLACCESS < 0 )
			{
				// 09/06/2017 Paul.  Include module name in error. 
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS") + ": " + Sql.ToString(RelatedModule)));
			}
			// 07/05/2020 Paul.  Need to restrict admin table updates.  Use table names because we know they are all uppercase. 
			if ( (sTABLE_NAME == "USERS" && sRELATED_TABLE == "ACL_ROLES") || (sTABLE_NAME == "ACL_ROLES" && sRELATED_TABLE == "USERS") || (sTABLE_NAME == "USERS" && sRELATED_TABLE == "TEAMS") || (sTABLE_NAME == "TEAMS" && sRELATED_TABLE == "USERS") )
			{
				if ( Security.AdminUserAccess("Users", "view") < 0 )
				{
					throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
				}
			}

			string sRELATIONSHIP_TABLE = sTABLE_NAME + "_" + sRELATED_TABLE;
			string sMODULE_FIELD_NAME  = SplendidCRM.Crm.Modules.SingularTableName(sTABLE_NAME   ) + "_ID";
			string sRELATED_FIELD_NAME = SplendidCRM.Crm.Modules.SingularTableName(sRELATED_TABLE) + "_ID";
			// 11/24/2012 Paul.  In the special cases of Accounts Related and Contacts Reports To, we need to correct the field name. 
			if ( sMODULE_FIELD_NAME == "ACCOUNT_ID" && sRELATED_FIELD_NAME == "ACCOUNT_ID" )
			{
				sRELATIONSHIP_TABLE = "ACCOUNTS_MEMBERS";
				sRELATED_FIELD_NAME = "PARENT_ID";
			}
			else if ( sMODULE_FIELD_NAME == "CONTACT_ID" && sRELATED_FIELD_NAME == "CONTACT_ID" )
			{
				sRELATIONSHIP_TABLE = "CONTACTS_DIRECT_REPORTS";
				sRELATED_FIELD_NAME = "REPORTS_TO_ID";
			}
			// 05/27/2020 Paul. Correct some relationships. 
			// 10/15/2020 Paul.  Admin can add users from teams and teams from users. 
			else if ( sRELATIONSHIP_TABLE == "USERS_TEAMS" || sRELATIONSHIP_TABLE == "TEAMS_USERS" )
			{
				sRELATIONSHIP_TABLE = "USERS_TEAM_MEMBERSHIPS";
			}
			// 03/09/2021 Paul.  Correct ROLE_ID field name. 
			else if ( sRELATIONSHIP_TABLE == "ACL_ROLES_USERS" && sMODULE_FIELD_NAME == "ACL_ROLE_ID" )
			{
				sMODULE_FIELD_NAME = "ROLE_ID";
			}
			
			// 06/04/2011 Paul.  For relationships, we first need to check the access rights of the parent record. 
			bool bExcludeSystemTables = true;
			if ( Security.AdminUserAccess(ModuleName, "edit") >= 0 )
			{
				bExcludeSystemTables = false;
			}
			DataTable dtSYNC_TABLES = SplendidCache.RestTables("vw" + sRELATIONSHIP_TABLE, bExcludeSystemTables);
			if ( dtSYNC_TABLES != null && dtSYNC_TABLES.Rows.Count == 0 )
			{
				sRELATIONSHIP_TABLE = sRELATED_TABLE + "_" + sTABLE_NAME;
				dtSYNC_TABLES = SplendidCache.RestTables("vw" + sRELATIONSHIP_TABLE, bExcludeSystemTables);
				if ( dtSYNC_TABLES != null && dtSYNC_TABLES.Rows.Count == 0 )
				{
					throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS") + " to relationship between modules " + ModuleName + " and " + RelatedModule));
				}
			}
			//01/18/2022 Paul.  spCAMPAIGNS_PROSPECT_LISTS_Update needs to be converted to spPROSPECT_LISTS_CAMPAIGNS_Update, so perform a second relationship lookup. 
			SplendidCRM.DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				IDbCommand cmdUpdate  = null;
				try
				{
					cmdUpdate = SqlProcs.Factory(con, "sp" + sRELATIONSHIP_TABLE + "_Update");
				}
				catch
				{
				}
				if ( cmdUpdate == null )
				{
					sRELATIONSHIP_TABLE = sRELATED_TABLE + "_" + sTABLE_NAME;
					try
					{
						cmdUpdate = SqlProcs.Factory(con, "sp" + sRELATIONSHIP_TABLE + "_Update");
					}
					catch
					{
						throw(new Exception("Could not find relationship stored procedures " + "sp" + sRELATED_TABLE + "_" + sTABLE_NAME + "_Update" + " or " + "sp" + sTABLE_NAME + "_" + sRELATED_TABLE + "_Update"));
					}
				}
			}
			UpdateRelatedList(sTABLE_NAME, sRELATIONSHIP_TABLE, sMODULE_FIELD_NAME, ID, sRELATED_FIELD_NAME, RelatedList, bExcludeSystemTables);
		}

		// 07/09/2019 Paul.  UpdateRelatedList is identical to UpdateRelatedItem but accepts an array. 
		private void UpdateRelatedList(string sTABLE_NAME, string sRELATIONSHIP_TABLE, string sMODULE_FIELD_NAME, Guid gID, string sRELATED_FIELD_NAME, Guid[] arrRELATED_ID, bool bExcludeSystemTables)
		{
			try
			{
				if ( Security.IsAuthenticated() )
				{
					Regex  r        = new Regex(@"[^A-Za-z0-9_]");
					sTABLE_NAME = r.Replace(sTABLE_NAME, "").ToUpper();
					
					SplendidCRM.DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						// 06/03/2011 Paul.  Cache the Rest Table data. 
						// 11/26/2009 Paul.  System tables cannot be updated. 
						// 06/04/2011 Paul.  For relationships, we first need to check the access rights of the parent record. 
						// 05/28/2020 Paul.  USERS table is a system table, and we need to allow spUSERS_ACL_ROLES_Update and spUSERS_TEAM_MEMBERSHIPS_Update for the React Client. 
						// 06/04/2011 Paul.  For relationships, we first need to check the access rights of the parent record. 
						using ( DataTable dtSYNC_TABLES = SplendidCache.RestTables(sTABLE_NAME, bExcludeSystemTables) )
						{
							string sSQL = String.Empty;
							if ( dtSYNC_TABLES != null && dtSYNC_TABLES.Rows.Count > 0 )
							{
								DataRow rowSYNC_TABLE = dtSYNC_TABLES.Rows[0];
								string sMODULE_NAME = Sql.ToString (rowSYNC_TABLE["MODULE_NAME"]);
								string sVIEW_NAME   = Sql.ToString (rowSYNC_TABLE["VIEW_NAME"  ]);
								if ( Sql.IsEmptyString(sMODULE_NAME) )
								{
									throw(new Exception("sMODULE_NAME should not be empty for table " + sTABLE_NAME));
								}
								
								int nACLACCESS = Security.GetUserAccess(sMODULE_NAME, "edit");
								// 11/11/2009 Paul.  First check if the user has access to this module. 
								if ( nACLACCESS >= 0 )
								{
									bool      bRecordExists              = false;
									bool      bAccessAllowed             = false;
									Guid      gLOCAL_ASSIGNED_USER_ID    = Guid.Empty;
									DataRow   rowCurrent                 = null;
									DataTable dtCurrent                  = new DataTable();
									sSQL = "select *"              + ControlChars.CrLf
									     + "  from " + sTABLE_NAME + ControlChars.CrLf
									     + " where DELETED = 0"    + ControlChars.CrLf;
									using ( IDbCommand cmd = con.CreateCommand() )
									{
										cmd.CommandText = sSQL;
										// 02/14/2018 Paul.  Azure can timeout, so lets wait for an hour. 
										cmd.CommandTimeout = 60 * 60;
										Sql.AppendParameter(cmd, gID, "ID");
										using ( DbDataAdapter da = dbf.CreateDataAdapter() )
										{
											((IDbDataAdapter)da).SelectCommand = cmd;
											// 11/27/2009 Paul.  It may be useful to log the SQL during errors at this location. 
											try
											{
												da.Fill(dtCurrent);
											}
											catch
											{
												SplendidError.SystemError(new StackTrace(true).GetFrame(0), Sql.ExpandParameters(cmd));
												throw;
											}
											if ( dtCurrent.Rows.Count > 0 )
											{
												rowCurrent = dtCurrent.Rows[0];
												bRecordExists = true;
												// 01/18/2010 Paul.  Apply ACL Field Security. 
												if ( dtCurrent.Columns.Contains("ASSIGNED_USER_ID") )
												{
													gLOCAL_ASSIGNED_USER_ID = Sql.ToGuid(rowCurrent["ASSIGNED_USER_ID"]);
												}
											}
										}
									}
									// 06/04/2011 Paul.  We are not ready to handle conflicts. 
									//if ( !bConflicted )
									{
										if ( bRecordExists )
										{
											sSQL = "select count(*)"       + ControlChars.CrLf
											     + "  from " + sTABLE_NAME + ControlChars.CrLf;
											using ( IDbCommand cmd = con.CreateCommand() )
											{
												cmd.CommandText = sSQL;
												Security.Filter(cmd, sMODULE_NAME, "edit");
												Sql.AppendParameter(cmd, gID, "ID");
												try
												{
													if ( Sql.ToInteger(cmd.ExecuteScalar()) > 0 )
													{
														if ( (nACLACCESS > ACL_ACCESS.OWNER) || (nACLACCESS == ACL_ACCESS.OWNER && Security.USER_ID == gLOCAL_ASSIGNED_USER_ID) || !dtCurrent.Columns.Contains("ASSIGNED_USER_ID") )
															bAccessAllowed = true;
													}
												}
												catch
												{
													SplendidError.SystemError(new StackTrace(true).GetFrame(0), Sql.ExpandParameters(cmd));
													throw;
												}
											}
										}
										if ( bAccessAllowed )
										{
											// 11/24/2012 Paul.  We do not need to check for RestTable access as that step was already done. 
											IDbCommand cmdUpdate = SqlProcs.Factory(con, "sp" + sRELATIONSHIP_TABLE + "_Update");
											using ( IDbTransaction trn = Sql.BeginTransaction(con) )
											{
												try
												{
													cmdUpdate.Transaction = trn;
													IDbDataParameter parRELATED_ID = null;
													foreach(IDbDataParameter par in cmdUpdate.Parameters)
													{
														string sParameterName = Sql.ExtractDbName(cmdUpdate, par.ParameterName).ToUpper();
														if ( sParameterName == sMODULE_FIELD_NAME )
															par.Value = gID;
														else if ( sParameterName == sRELATED_FIELD_NAME )
															parRELATED_ID = par;
														else if ( sParameterName == "MODIFIED_USER_ID" )
															par.Value = Sql.ToDBGuid(Security.USER_ID);
														else
															par.Value = DBNull.Value;
													}
													foreach ( Guid gRELATED_ID in arrRELATED_ID )
													{
														parRELATED_ID.Value = gRELATED_ID;
														cmdUpdate.ExecuteScalar();
													}
													trn.Commit();
												}
												catch(Exception ex)
												{
													// 02/13/2017 Paul.  Capture this error as the following can generate an "This SqlTransaction has completed" error on Azure. 
													try
													{
														trn.Rollback();
													}
													catch
													{
													}
													SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
													throw;
												}
											}
										}
										else
										{
											throw(new Exception(L10n.Term("ACL.LBL_NO_ACCESS")));
										}
									}
								}
								else
								{
									throw(new Exception(L10n.Term("ACL.LBL_NO_ACCESS")));
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

		// 10/17/2020 Paul.  Regions.Countries needs a way to add by value. 
		public class UpdateRelatedValuesParameters
		{
			public string   ModuleName    { get; set; }
			public Guid     ID            { get; set; }
			public string   RelatedTable  { get; set; }
			public string[] RelatedValues { get; set; }
		}

		[HttpPost("[action]")]
		public void UpdateRelatedValues([FromBody] UpdateRelatedValuesParameters input)
		{
			string   ModuleName    = Sql.ToString(input.ModuleName);
			Guid     ID            = Sql.ToGuid  (input.ID        );
			string   RelatedTable  = Sql.ToString(input.RelatedTable);
			string[] RelatedValues = input.RelatedValues;
			if ( Sql.IsEmptyString(ModuleName) )
				throw(new Exception("The module name must be specified."));
			string sTABLE_NAME = Sql.ToString(Application["Modules." + ModuleName + ".TableName"]);
			if ( Sql.IsEmptyString(sTABLE_NAME) )
				throw(new Exception("Unknown module: " + ModuleName));
			// 08/22/2011 Paul.  Add admin control to REST API. 
			int nACLACCESS = Security.GetUserAccess(ModuleName, "edit");
			if ( !Security.IsAuthenticated() || !Sql.ToBoolean(Application["Modules." + ModuleName + ".RestEnabled"]) || nACLACCESS < 0 )
			{
				// 09/06/2017 Paul.  Include module name in error. 
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS") + ": " + Sql.ToString(ModuleName)));
			}
			
			string sRELATIONSHIP_TABLE = sTABLE_NAME + "_" + RelatedTable;
			string sMODULE_FIELD_NAME  = SplendidCRM.Crm.Modules.SingularTableName(sTABLE_NAME ) + "_ID";
			string sRELATED_FIELD_NAME = SplendidCRM.Crm.Modules.SingularTableName(RelatedTable);
			// 06/04/2011 Paul.  For relationships, we first need to check the access rights of the parent record. 
			bool bExcludeSystemTables = true;
			if ( Security.AdminUserAccess(ModuleName, "edit") >= 0 )
			{
				bExcludeSystemTables = false;
			}
			DataTable dtSYNC_TABLES = SplendidCache.RestTables("vw" + sRELATIONSHIP_TABLE, bExcludeSystemTables);
			if ( dtSYNC_TABLES != null && dtSYNC_TABLES.Rows.Count == 0 )
			{
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS") + " to relationship between modules " + ModuleName + " and " + RelatedTable));
			}
			UpdateRelatedValues(sTABLE_NAME, sRELATIONSHIP_TABLE, sMODULE_FIELD_NAME, ID, sRELATED_FIELD_NAME, RelatedValues, bExcludeSystemTables);
		}

		private void UpdateRelatedValues(string sTABLE_NAME, string sRELATIONSHIP_TABLE, string sMODULE_FIELD_NAME, Guid gID, string sRELATED_FIELD_NAME, string[] arrRELATED_VALUES, bool bExcludeSystemTables)
		{
			try
			{
				if ( Security.IsAuthenticated() )
				{
					Regex  r        = new Regex(@"[^A-Za-z0-9_]");
					sTABLE_NAME = r.Replace(sTABLE_NAME, "").ToUpper();
					
					SplendidCRM.DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						// 06/03/2011 Paul.  Cache the Rest Table data. 
						// 11/26/2009 Paul.  System tables cannot be updated. 
						// 06/04/2011 Paul.  For relationships, we first need to check the access rights of the parent record. 
						// 05/28/2020 Paul.  USERS table is a system table, and we need to allow spUSERS_ACL_ROLES_Update and spUSERS_TEAM_MEMBERSHIPS_Update for the React Client. 
						// 06/04/2011 Paul.  For relationships, we first need to check the access rights of the parent record. 
						using ( DataTable dtSYNC_TABLES = SplendidCache.RestTables(sTABLE_NAME, bExcludeSystemTables) )
						{
							string sSQL = String.Empty;
							if ( dtSYNC_TABLES != null && dtSYNC_TABLES.Rows.Count > 0 )
							{
								DataRow rowSYNC_TABLE = dtSYNC_TABLES.Rows[0];
								string sMODULE_NAME = Sql.ToString (rowSYNC_TABLE["MODULE_NAME"]);
								string sVIEW_NAME   = Sql.ToString (rowSYNC_TABLE["VIEW_NAME"  ]);
								if ( Sql.IsEmptyString(sMODULE_NAME) )
								{
									throw(new Exception("sMODULE_NAME should not be empty for table " + sTABLE_NAME));
								}
								
								int nACLACCESS = Security.GetUserAccess(sMODULE_NAME, "edit");
								// 11/11/2009 Paul.  First check if the user has access to this module. 
								if ( nACLACCESS >= 0 )
								{
									bool      bRecordExists              = false;
									bool      bAccessAllowed             = false;
									Guid      gLOCAL_ASSIGNED_USER_ID    = Guid.Empty;
									DataRow   rowCurrent                 = null;
									DataTable dtCurrent                  = new DataTable();
									sSQL = "select *"              + ControlChars.CrLf
									     + "  from " + sTABLE_NAME + ControlChars.CrLf
									     + " where DELETED = 0"    + ControlChars.CrLf;
									using ( IDbCommand cmd = con.CreateCommand() )
									{
										cmd.CommandText = sSQL;
										// 02/14/2018 Paul.  Azure can timeout, so lets wait for an hour. 
										cmd.CommandTimeout = 60 * 60;
										Sql.AppendParameter(cmd, gID, "ID");
										using ( DbDataAdapter da = dbf.CreateDataAdapter() )
										{
											((IDbDataAdapter)da).SelectCommand = cmd;
											// 11/27/2009 Paul.  It may be useful to log the SQL during errors at this location. 
											try
											{
												da.Fill(dtCurrent);
											}
											catch
											{
												SplendidError.SystemError(new StackTrace(true).GetFrame(0), Sql.ExpandParameters(cmd));
												throw;
											}
											if ( dtCurrent.Rows.Count > 0 )
											{
												rowCurrent = dtCurrent.Rows[0];
												bRecordExists = true;
												// 01/18/2010 Paul.  Apply ACL Field Security. 
												if ( dtCurrent.Columns.Contains("ASSIGNED_USER_ID") )
												{
													gLOCAL_ASSIGNED_USER_ID = Sql.ToGuid(rowCurrent["ASSIGNED_USER_ID"]);
												}
											}
										}
									}
									if ( bRecordExists )
									{
										sSQL = "select count(*)"       + ControlChars.CrLf
											    + "  from " + sTABLE_NAME + ControlChars.CrLf;
										using ( IDbCommand cmd = con.CreateCommand() )
										{
											cmd.CommandText = sSQL;
											Security.Filter(cmd, sMODULE_NAME, "edit");
											Sql.AppendParameter(cmd, gID, "ID");
											try
											{
												if ( Sql.ToInteger(cmd.ExecuteScalar()) > 0 )
												{
													if ( (nACLACCESS > ACL_ACCESS.OWNER) || (nACLACCESS == ACL_ACCESS.OWNER && Security.USER_ID == gLOCAL_ASSIGNED_USER_ID) || !dtCurrent.Columns.Contains("ASSIGNED_USER_ID") )
														bAccessAllowed = true;
												}
											}
											catch
											{
												SplendidError.SystemError(new StackTrace(true).GetFrame(0), Sql.ExpandParameters(cmd));
												throw;
											}
										}
									}
									if ( bAccessAllowed )
									{
										IDbCommand cmdUpdate = SqlProcs.Factory(con, "sp" + sRELATIONSHIP_TABLE + "_Update");
										using ( IDbTransaction trn = Sql.BeginTransaction(con) )
										{
											try
											{
												cmdUpdate.Transaction = trn;
												IDbDataParameter parRELATED_VALUE = null;
												foreach(IDbDataParameter par in cmdUpdate.Parameters)
												{
													string sParameterName = Sql.ExtractDbName(cmdUpdate, par.ParameterName).ToUpper();
													if ( sParameterName == sMODULE_FIELD_NAME )
														par.Value = gID;
													else if ( sParameterName == sRELATED_FIELD_NAME )
														parRELATED_VALUE = par;
													else if ( sParameterName == "MODIFIED_USER_ID" )
														par.Value = Sql.ToDBGuid(Security.USER_ID);
													else
														par.Value = DBNull.Value;
												}
												foreach ( string sRELATED_VALUE in arrRELATED_VALUES )
												{
													parRELATED_VALUE.Value = sRELATED_VALUE;
													cmdUpdate.ExecuteScalar();
												}
												trn.Commit();
											}
											catch(Exception ex)
											{
												// 02/13/2017 Paul.  Capture this error as the following can generate an "This SqlTransaction has completed" error on Azure. 
												try
												{
													trn.Rollback();
												}
												catch
												{
												}
												SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
												throw;
											}
										}
									}
									else
									{
										throw(new Exception(L10n.Term("ACL.LBL_NO_ACCESS")));
									}
								}
								else
								{
									throw(new Exception(L10n.Term("ACL.LBL_NO_ACCESS")));
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

// 12/24/2021 TODO.  Not sure if we want to support SendEmail. 
#if false
		public class UpdateEmailReadStatusParameters
		{
			public Guid     ID            { get; set; }
		}

		// 11/05/2020 Paul.  Although we could be more flexible and allow any email status, we really only need to set to read if unread. 
		[HttpPost("[action]")]
		public void UpdateEmailReadStatus([FromBody] UpdateEmailReadStatusParameters input)
		{
			Guid ID = Sql.ToGuid(input.ID);
			string ModuleName = "Emails";
			if ( Sql.IsEmptyString(ModuleName) )
				throw(new Exception("The module name must be specified."));
			int nACLACCESS = Security.GetUserAccess(ModuleName, "edit");
			if ( !Security.IsAuthenticated() || !Sql.ToBoolean(Application["Modules." + ModuleName + ".RestEnabled"]) || nACLACCESS < 0 )
			{
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS") + ": " + Sql.ToString(ModuleName)));
			}
			
			SplendidCRM.DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				string sSQL ;
				// 06/18/2023 Paul.  Was not including TYPE or STATUS. 
				sSQL = "select ID           " + ControlChars.CrLf
				     + "     , NAME         " + ControlChars.CrLf
				     + "     , TYPE         " + ControlChars.CrLf
				     + "     , STATUS       " + ControlChars.CrLf
				     + "  from vwEMAILS_Edit" + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Security.Filter(cmd, ModuleName, "view");
					Sql.AppendParameter(cmd, ID, "ID", false);
					con.Open();

					using ( DbDataAdapter da = dbf.CreateDataAdapter() )
					{
						((IDbDataAdapter)da).SelectCommand = cmd;
						using ( DataTable dt = new DataTable() )
						{
							da.Fill(dt);
							if ( dt.Rows.Count > 0 )
							{
								DataRow rdr = dt.Rows[0];
								string sEMAIL_TYPE   = Sql.ToString(rdr["TYPE"  ]).ToLower();
								string sEMAIL_STATUS = Sql.ToString(rdr["STATUS"]).ToLower();
								if ( sEMAIL_TYPE == "inbound" && sEMAIL_STATUS == "unread" )
								{
									SqlProcs.spEMAILS_UpdateStatus(ID, "read");
								}
							}
						}
					}
				}
			}
		}

		// 01/24/2021 Paul.  Allow SendEmail from React Client. 
		[DotNetLegacyData]
		[HttpPost("[action]")]
		public string SendEmail([FromQuery] Guid ID)
		{
			string ModuleName = "Emails";
			if ( Sql.IsEmptyString(ModuleName) )
				throw(new Exception("The module name must be specified."));
			int nACLACCESS = Security.GetUserAccess(ModuleName, "edit");
			if ( !Security.IsAuthenticated() || !Sql.ToBoolean(Application["Modules." + ModuleName + ".RestEnabled"]) || nACLACCESS < 0 )
			{
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS") + ": " + Sql.ToString(ModuleName)));
			}
			string sTABLE_NAME = Sql.ToString(Application["Modules." + ModuleName + ".TableName"]);
			if ( Sql.IsEmptyString(sTABLE_NAME) )
				throw(new Exception("Unknown module: " + ModuleName));
			if ( Sql.IsEmptyGuid(ID) )
				throw(new Exception("Unspecified ID: " + ID));
			
			// 01/24/2021 Paul.  Return status so that we can take action on error. 
			string sSTATUS = "draft";
			// 01/23/2021 Paul.  Get the time first to ensure proper access. 
			SplendidCRM.DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				string sSQL ;
				sSQL = "select STATUS" + ControlChars.CrLf
				    + "  from vw" + sTABLE_NAME + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Security.Filter(cmd, ModuleName, "edit");
					Sql.AppendParameter(cmd, ID, "ID", false);
					con.Open();

					using ( DbDataAdapter da = dbf.CreateDataAdapter() )
					{
						((IDbDataAdapter)da).SelectCommand = cmd;

						DataTable dt = new DataTable();
						da.Fill(dt);
						if ( dt.Rows.Count > 0 )
						{
							// 01/24/2021 Paul.  Clear the send error so thatwe can send again. 
							sSTATUS = Sql.ToString(dt.Rows[0]["STATUS"]);
							if ( sSTATUS == "send_error" )
								SqlProcs.spEMAILS_UpdateStatus(ID, "draft");

							dt = new DataTable();
							cmd.Parameters.Clear();
							sSQL = "select TYPE                " + ControlChars.CrLf
							     + "     , STATUS              " + ControlChars.CrLf
							     + "  from vwEMAILS_ReadyToSend" + ControlChars.CrLf
							     + " where ID = @ID            " + ControlChars.CrLf;
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@ID", ID);
							da.Fill(dt);
							if ( dt.Rows.Count > 0 )
							{
								int nEmailsSent = 0;
								try
								{
									sSTATUS = "draft";
									// 06/18/2023 Paul.  Don't update unless necessary. 
									if ( Sql.ToString(dt.Rows[0]["STATUS"]) != sSTATUS )
										SqlProcs.spEMAILS_UpdateStatus(ID, sSTATUS);
									// 07/10/2010 Paul.  The Offline Client cannot send emails.  Just mark as draft & out. 
									// It should get sent when it is copied to the server. 
									// 12/20/2007 Paul.  SendEmail was moved to EmailUtils.
									// 05/19/2008 Paul.  Application is a required parameter so that SendEmail can be called within the scheduler. 
									// 11/05/2010 Paul.  Each user can have their own email account, but they all will share the same server. 
									// Remove all references to USER_SETTINGS/MAIL_FROMADDRESS and USER_SETTINGS/MAIL_FROMNAME. 
									EmailUtils.SendEmail(ID, Security.FULL_NAME, Security.EMAIL1, ref nEmailsSent);
									sSTATUS = "sent";
									SqlProcs.spEMAILS_UpdateStatus(ID, sSTATUS);
								}
								catch(Exception ex)
								{
									// 05/15/2008 Paul.  Mark the status as error so that scheduler will not try to resend. 
									if ( nEmailsSent > 0 )
									{
										sSTATUS = "partial";
										SqlProcs.spEMAILS_UpdateStatus(ID, sSTATUS);
									}
									else
									{
										sSTATUS = "send_error";
										SqlProcs.spEMAILS_UpdateStatus(ID, sSTATUS);
									}
									SplendidError.SystemError(new StackTrace(true).GetFrame(0), Utils.ExpandException(ex));
									sSTATUS = ex.Message;
								}
							}
							else
							{
								sSTATUS = "Not ready to send";
							}
						}
						else
						{
							sSTATUS = L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS") + ": " + ID.ToString();
						}
					}
				}
			}
			return sSTATUS;
		}
#endif

		// 07/05/2021 Paul.  We need a way to call a generic procedure.  Security is still managed through SYSTEM_REST_TABLES. 
		[HttpPost("[action]")]
		public Dictionary<string, object> ExecProcedure([FromBody] Dictionary<string, object> dict)
		{
			try
			{
				if ( !Security.IsAuthenticated() )
				{
					throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
				}
				
				string sProcedureName = Sql.ToString(Request.Query["ProcedureName"]);
				DataTable dtUPDATE = new DataTable(sProcedureName);
				foreach ( string sColumnName in dict.Keys )
				{
					dtUPDATE.Columns.Add(sColumnName);
				}
				DataRow row = dtUPDATE.NewRow();
				dtUPDATE.Rows.Add(row);
				foreach ( string sColumnName in dict.Keys )
				{
					// 09/09/2011 Paul.  Multi-selection list boxes will come in as an ArrayList. 
					if ( dict[sColumnName] is System.Collections.ArrayList )
					{
						System.Collections.ArrayList lst = dict[sColumnName] as System.Collections.ArrayList;
						XmlDocument xml = new XmlDocument();
						xml.AppendChild(xml.CreateXmlDeclaration("1.0", "UTF-8", null));
						xml.AppendChild(xml.CreateElement("Values"));
						if ( lst.Count > 0 )
						{
							foreach ( string item in lst )
							{
								XmlNode xValue = xml.CreateElement("Value");
								xml.DocumentElement.AppendChild(xValue);
								xValue.InnerText = item;
							}
						}
						row[sColumnName] = xml.OuterXml;
					}
					else
					{
						row[sColumnName] = dict[sColumnName];
					}
				}
				Dictionary<string, object> d = new Dictionary<string, object>();
				StringBuilder sbDumpSQL = new StringBuilder();
				SplendidCRM.DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( DataTable dtSYNC_TABLES = SplendidCache.RestProcedures(sProcedureName, true) )
				{
					string sSQL = String.Empty;
					if ( dtSYNC_TABLES != null && dtSYNC_TABLES.Rows.Count > 0 )
					{
						DataRow rowSYNC_TABLE = dtSYNC_TABLES.Rows[0];
						string sPROCEDURE_NAME       = Sql.ToString (rowSYNC_TABLE["TABLE_NAME"         ]);
						string sMODULE_NAME          = Sql.ToString (rowSYNC_TABLE["MODULE_NAME"        ]);
						string sREQUIRED_FIELDS      = Sql.ToString (rowSYNC_TABLE["REQUIRED_FIELDS"    ]);
						bool   bEnableTeamManagement = Config.enable_team_management();
						
						int nACLACCESS = Security.GetUserAccess(sMODULE_NAME, "edit");
						if ( nACLACCESS >= 0 )
						{
							if ( !Sql.IsEmptyString(sREQUIRED_FIELDS) )
							{
								// 07/05/2021 Paul.  Allow comma as separator. 
								string[] arrREQUIRED_FIELDS = sREQUIRED_FIELDS.ToUpper().Replace(",", " ").Split(' ');
								string sMISSING_FIELDS = String.Empty;
								foreach ( string sREQUIRED_FIELD in arrREQUIRED_FIELDS )
								{
									if ( !dtUPDATE.Columns.Contains(sREQUIRED_FIELD) )
									{
										if ( !Sql.IsEmptyString(sMISSING_FIELDS) )
											sMISSING_FIELDS += " ";
										sMISSING_FIELDS += sREQUIRED_FIELD;
									}
								}
								if ( !Sql.IsEmptyString(sMISSING_FIELDS) )
								{
									throw(new Exception("Missing required fields: " + sMISSING_FIELDS));
								}
							}

							using ( IDbConnection con = dbf.CreateConnection() )
							{
								con.Open();
								using ( IDbTransaction trn = Sql.BeginTransaction(con) )
								{
									try
									{
										IDbCommand cmd = SqlProcs.Factory(con, sPROCEDURE_NAME);
										cmd.Transaction = trn;
										foreach(IDbDataParameter par in cmd.Parameters)
										{
											// 03/27/2010 Paul.  The ParameterName will start with @, so we need to remove it. 
											string sParameterName = Sql.ExtractDbName(cmd, par.ParameterName).ToUpper();
											if ( sParameterName == "TEAM_ID" && bEnableTeamManagement )
												par.Value = Sql.ToDBGuid(Security.TEAM_ID);
											else if ( sParameterName == "ASSIGNED_USER_ID" )
												par.Value = Sql.ToDBGuid(Security.USER_ID);
											else if ( sParameterName == "MODIFIED_USER_ID" )
												par.Value = Sql.ToDBGuid(Security.USER_ID);
											else
												par.Value = DBNull.Value;
										}
										foreach ( DataColumn col in row.Table.Columns )
										{
											IDbDataParameter par = Sql.FindParameter(cmd, col.ColumnName);
											if ( par != null )
											{
												// 05/22/2017 Paul.  Shared function to convert from Json to DB. 
												par.Value = RestUtil.DBValueFromJsonValue(par.DbType, row[col.ColumnName], T10n);
											}
										}
										sbDumpSQL.Append(Sql.ExpandParameters(cmd));
										cmd.ExecuteNonQuery();
										trn.Commit();
										foreach(IDbDataParameter par in cmd.Parameters)
										{
											if ( par.Direction == ParameterDirection.InputOutput || par.Direction == ParameterDirection.Output )
											{
												string sParameterName = Sql.ExtractDbName(cmd, par.ParameterName).ToUpper();
												d.Add(sParameterName, par.Value);
											}
										}
									}
									catch
									{
										// 02/13/2017 Paul.  Capture this error as the following can generate an "This SqlTransaction has completed" error on Azure. 
										try
										{
											trn.Rollback();
										}
										catch
										{
										}
										throw;
									}
								}
							}
						}
						else
						{
							throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
						}
					}
					else
					{
						throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS") + ": " + sProcedureName));
					}
				}
				
				Dictionary<string, object> dictResponse = new Dictionary<string, object>();
				dictResponse.Add("d", d);
				if ( Sql.ToBoolean(Application["CONFIG.show_sql"]) )
				{
					dictResponse.Add("__sql", sbDumpSQL.ToString());
				}
				return dictResponse;
			}
			catch(Exception ex)
			{
				// 03/20/2019 Paul.  Catch and log all failures, including insufficient access. 
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				throw;
			}
		}

		// 10/17/2020 Paul.  Regions.Countries needs a way to add by value. 
		public class ChangePasswordParameters
		{
			public Guid     USER_ID      { get; set; }
			public string   OLD_PASSWORD { get; set; }
			public string   NEW_PASSWORD { get; set; }
		}

		// 01/09/2022 Paul.  Add support for ChangePassword.
		[HttpPost("[action]")]
		public void ChangePassword([FromBody] ChangePasswordParameters input)
		{
			Guid     USER_ID      = Sql.ToGuid  (input.USER_ID     );
			string   OLD_PASSWORD = Sql.ToString(input.OLD_PASSWORD);
			string   NEW_PASSWORD = Sql.ToString(input.NEW_PASSWORD);
			try
			{
				// 01/10/2022 Paul.  Only an admin can change the password for another user. 
				if ( Sql.IsEmptyGuid(USER_ID) )
				{
					throw(new Exception(L10n.Term(".ERR_MISSING_REQUIRED_FIELDS")));
				}
				else if ( Sql.IsEmptyString(NEW_PASSWORD) )
				{
					throw(new Exception(L10n.Term(".ERR_MISSING_REQUIRED_FIELDS")));
				}
				else if ( !(Security.AdminUserAccess("Users", "edit") >= 0) )
				{
					if ( USER_ID != Security.USER_ID )
					{
						throw(new Exception(L10n.Term(".LBL_INSUFFICIENT_ACCESS")));
					}
				}
				
				bool bValidOldPassword = false;
				string sUSER_NAME = String.Empty;
				SplendidCRM.DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					string sSQL ;
					// 07/17/2006 Paul.  The USER_HASH has been removed from the main vwUSERS view to prevent its use in reports. 
					sSQL = "select *                     " + ControlChars.CrLf
					     + "  from vwUSERS_Login         " + ControlChars.CrLf
					     + " where ID        = @ID       " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@ID", USER_ID);
						// 01/10/2022 Paul.  Validate the USER_ID on the fist pass. 
						using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
						{
							if ( rdr.Read() )
							{
								sUSER_NAME = Sql.ToString(rdr["USER_NAME"]);
							}
							else
							{
								throw(new Exception(L10n.Term("Users.ERR_USER_NOT_FOUND")));
							}
						}
						if ( !(Security.AdminUserAccess("Users", "view") >= 0) )
						{
							// 02/13/2009 Paul.  We need to allow a user with a blank password to change his password. 
							if ( !Sql.IsEmptyString(OLD_PASSWORD) )
							{
								string sUSER_HASH = Security.HashPassword(OLD_PASSWORD);
								cmd.CommandText += "   and USER_HASH = @USER_HASH" + ControlChars.CrLf;
								Sql.AddParameter(cmd, "@USER_HASH", sUSER_HASH);
							}
							else
							{
								// 11/19/2005 Paul.  Handle the special case of the password stored as NULL or empty string. 
								cmd.CommandText += "   and (USER_HASH = '' or USER_HASH is null)" + ControlChars.CrLf;
							}
							using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
							{
								if ( rdr.Read() )
								{
									bValidOldPassword = true;
								}
							}
							if ( !bValidOldPassword )
							{
								throw(new Exception(L10n.Term("Users.ERR_PASSWORD_INCORRECT_OLD")));
							}
						}
					}
				}
				if ( bValidOldPassword || (Security.AdminUserAccess("Users", "edit") >= 0) )
				{
					SplendidCRM.SplendidPassword ctlNEW_PASSWORD_STRENGTH = new SplendidCRM.SplendidPassword();
					ctlNEW_PASSWORD_STRENGTH.PreferredPasswordLength             = CrmPassword.PreferredPasswordLength            ;
					ctlNEW_PASSWORD_STRENGTH.MinimumLowerCaseCharacters          = CrmPassword.MinimumLowerCaseCharacters         ;
					ctlNEW_PASSWORD_STRENGTH.MinimumUpperCaseCharacters          = CrmPassword.MinimumUpperCaseCharacters         ;
					ctlNEW_PASSWORD_STRENGTH.MinimumNumericCharacters            = CrmPassword.MinimumNumericCharacters           ;
					ctlNEW_PASSWORD_STRENGTH.MinimumSymbolCharacters             = CrmPassword.MinimumSymbolCharacters            ;
					ctlNEW_PASSWORD_STRENGTH.PrefixText                          = CrmPassword.PrefixText                         ;
					ctlNEW_PASSWORD_STRENGTH.TextStrengthDescriptions            = CrmPassword.TextStrengthDescriptions           ;
					ctlNEW_PASSWORD_STRENGTH.SymbolCharacters                    = CrmPassword.SymbolCharacters                   ;
					ctlNEW_PASSWORD_STRENGTH.ComplexityNumber                    = CrmPassword.ComplexityNumber                   ;

					ctlNEW_PASSWORD_STRENGTH.MessageRemainingCharacters          = L10n.Term("Users.LBL_PASSWORD_REMAINING_CHARACTERS");
					ctlNEW_PASSWORD_STRENGTH.MessageRemainingNumbers             = L10n.Term("Users.LBL_PASSWORD_REMAINING_NUMBERS"   );
					ctlNEW_PASSWORD_STRENGTH.MessageRemainingLowerCase           = L10n.Term("Users.LBL_PASSWORD_REMAINING_LOWERCASE" );
					ctlNEW_PASSWORD_STRENGTH.MessageRemainingUpperCase           = L10n.Term("Users.LBL_PASSWORD_REMAINING_UPPERCASE" );
					ctlNEW_PASSWORD_STRENGTH.MessageRemainingMixedCase           = L10n.Term("Users.LBL_PASSWORD_REMAINING_MIXEDCASE" );
					ctlNEW_PASSWORD_STRENGTH.MessageRemainingSymbols             = L10n.Term("Users.LBL_PASSWORD_REMAINING_SYMBOLS"   );
					ctlNEW_PASSWORD_STRENGTH.MessageSatisfied                    = L10n.Term("Users.LBL_PASSWORD_SATISFIED"           );

					string sPASSWORD_REQUIREMENTS = String.Empty;
					if ( ctlNEW_PASSWORD_STRENGTH.IsValid(NEW_PASSWORD, ref sPASSWORD_REQUIREMENTS) )
					{
						string sUSER_HASH = Security.HashPassword(NEW_PASSWORD);
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							// 02/20/2011 Paul.  Prevent use of previous passwords. 
							sSQL = "select count(*)                " + ControlChars.CrLf
							     + "  from vwUSERS_PASSWORD_HISTORY" + ControlChars.CrLf
							     + " where USER_ID   = @USER_ID    " + ControlChars.CrLf
							     + "   and USER_HASH = @USER_HASH  " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@USER_ID"  , USER_ID);
								Sql.AddParameter(cmd, "@USER_HASH", sUSER_HASH);
								int nLastPassword = Sql.ToInteger(cmd.ExecuteScalar());
								if ( nLastPassword == 0 )
								{
									SqlProcs.spUSERS_PasswordUpdate(USER_ID, sUSER_HASH);
									// 02/23/2011 Paul.  Clear any existing failures so that the user can login. 
									// This is how an administrator will reset the failure count. 
									SplendidInit.LoginTracking(sUSER_NAME, true);
								}
								else
								{
									throw(new Exception(L10n.Term("Users.ERR_CANNOT_REUSE_PASSWORD")));
								}
							}
						}
					}
					else
					{
						throw(new Exception(sPASSWORD_REQUIREMENTS));
					}
				}
			}
			catch(Exception ex)
			{
				// 01/10/2022 Paul.  Log all change password failures. 
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), "Change Password for " + USER_ID.ToString() + " failed.  " + ex.Message);
				throw;
			}
		}
		#endregion

		// 05/08/2019 Paul.  The React client will need to update the saved search of each module. 
		[DotNetLegacyData]
		[HttpPost("[action]")]
		// 02/07/2022 Paul.  Must follow old convention of returning value under d.  { d: value }
		public Guid UpdateSavedSearch([FromBody] Dictionary<string, object> dict)
		{
			Guid gID = Guid.Empty;
			//try
			{
				// 05/13/2019 Paul.  The React client will need to save the search view by ID. 
				// 01/19/2020 Paul.  The ID may not be provided, so we need to pevent a missing exception. 
				       gID                = (dict.ContainsKey("ID"               ) ? Sql.ToGuid  (dict["ID"               ]) : Guid.Empty  );
				string sNAME              = (dict.ContainsKey("NAME"             ) ? Sql.ToString(dict["NAME"             ]) : String.Empty);
				string sSEARCH_MODULE     = (dict.ContainsKey("SEARCH_MODULE"    ) ? Sql.ToString(dict["SEARCH_MODULE"    ]) : String.Empty);
				Guid   gDEFAULT_SEARCH_ID = (dict.ContainsKey("DEFAULT_SEARCH_ID") ? Sql.ToGuid  (dict["DEFAULT_SEARCH_ID"]) : Guid.Empty  );
				string sCONTENTS          = (dict.ContainsKey("CONTENTS"         ) ? Sql.ToString(dict["CONTENTS"         ]) : String.Empty);
				string sMODULE            = Sql.ToString(sSEARCH_MODULE).Split('.')[0];
				// 05/08/2019 Paul.  Use the standard filter to verify that the user can view the record. 
				if ( !Sql.IsEmptyString(sMODULE) && (Security.GetUserAccess(sMODULE, "view") >= 0) )
				{
					SqlProcs.spSAVED_SEARCH_Update(ref gID, Security.USER_ID, sNAME, sSEARCH_MODULE, sCONTENTS, String.Empty, gDEFAULT_SEARCH_ID);
					SplendidCache.ClearSavedSearch(sSEARCH_MODULE);
				}
			}
			//catch
			{
				// 02/04/2007 Paul.  Don't catch the exception.  
				// It is a web service, so the exception will be handled properly by the AJAX framework. 
			}
			return gID;
		}

		public class DeleteSavedSearchParameters
		{
			public Guid   ID            { get; set; }
			public string SEARCH_MODULE { get; set; }
		}

		[HttpPost("[action]")]
		public void DeleteSavedSearch([FromBody] DeleteSavedSearchParameters input)
		{
			Guid   ID            = Sql.ToGuid  (input.ID           );
			string SEARCH_MODULE = Sql.ToString(input.SEARCH_MODULE);
			int nACLACCESS = Security.GetUserAccess(SEARCH_MODULE, "list");
			if ( !Security.IsAuthenticated() || !Sql.ToBoolean(Application["Modules." + SEARCH_MODULE + ".RestEnabled"]) || nACLACCESS < 0 )
			{
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS") + ": " + Sql.ToString(SEARCH_MODULE)));
			}
			SqlProcs.spSAVED_SEARCH_Delete(ID);
		}

		#region Delete
		public class DeleteModuleItemParameters
		{
			public string ModuleName    { get; set; }
			public Guid   ID            { get; set; }
		}

		// 3.2 Method Tunneling through POST. 
		[HttpPost("[action]")]
		public void DeleteModuleItem([FromBody] DeleteModuleItemParameters input)
		{
			string ModuleName    = Sql.ToString(input.ModuleName   );
			Guid   ID            = Sql.ToGuid  (input.ID           );
			if ( Sql.IsEmptyString(ModuleName) )
				throw(new Exception("The module name must be specified."));
			string sTABLE_NAME = Sql.ToString(Application["Modules." + ModuleName + ".TableName"]);
			if ( Sql.IsEmptyString(sTABLE_NAME) )
				throw(new Exception("Unknown module: " + ModuleName));
			// 08/22/2011 Paul.  Add admin control to REST API. 
			int nACLACCESS = Security.GetUserAccess(ModuleName, "delete");
			if ( !Security.IsAuthenticated() || !Sql.ToBoolean(Application["Modules." + ModuleName + ".RestEnabled"]) || nACLACCESS < 0 )
			{
				// 09/06/2017 Paul.  Include module name in error. 
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS") + ": " + Sql.ToString(ModuleName)));
			}
			
			// 07/16/2019 Paul.  Add support for MassDelete. 
			Guid[] arrID_LIST = new Guid[1] { ID };
			DeleteTableItems(sTABLE_NAME, arrID_LIST, false);
		}

		// 07/16/2019 Paul.  Add support for MassDelete. 
		public class MassDeleteModuleParameters
		{
			public string ModuleName    { get; set; }
			public Guid[] ID_LIST       { get; set; }
		}

		[HttpPost("[action]")]
		public void MassDeleteModule([FromBody] MassDeleteModuleParameters input)
		{
			string ModuleName = Sql.ToString(input.ModuleName);
			Guid[] ID_LIST    = input.ID_LIST                 ;
			if ( Sql.IsEmptyString(ModuleName) )
				throw(new Exception("The module name must be specified."));
			string sTABLE_NAME = Sql.ToString(Application["Modules." + ModuleName + ".TableName"]);
			if ( Sql.IsEmptyString(sTABLE_NAME) )
				throw(new Exception("Unknown module: " + ModuleName));
			int nACLACCESS = Security.GetUserAccess(ModuleName, "delete");
			if ( !Security.IsAuthenticated() || !Sql.ToBoolean(Application["Modules." + ModuleName + ".RestEnabled"]) || nACLACCESS < 0 )
			{
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS") + ": " + Sql.ToString(ModuleName)));
			}
			
			DeleteTableItems(sTABLE_NAME, ID_LIST, false);
		}

		// 07/16/2019 Paul.  Add support for MassDelete. 
		// 08/10/2020 Paul.  Separate method to delete recurrences. 
		private void DeleteTableItems(string sTABLE_NAME, Guid[] arrID_LIST, bool bDeleteRecurrences)
		{
			try
			{
				if ( Security.IsAuthenticated() )
				{
					Regex  r        = new Regex(@"[^A-Za-z0-9_]");
					sTABLE_NAME = r.Replace(sTABLE_NAME, "").ToUpper();
					
					SplendidCRM.DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						// 06/03/2011 Paul.  Cache the Rest Table data. 
						// 11/26/2009 Paul.  System tables cannot be updated. 
						using ( DataTable dtSYNC_TABLES = SplendidCache.RestTables(sTABLE_NAME, true) )
						{
							string sSQL = String.Empty;
							if ( dtSYNC_TABLES != null && dtSYNC_TABLES.Rows.Count > 0 )
							{
								DataRow rowSYNC_TABLE = dtSYNC_TABLES.Rows[0];
								string sMODULE_NAME = Sql.ToString (rowSYNC_TABLE["MODULE_NAME"]);
								string sVIEW_NAME   = Sql.ToString (rowSYNC_TABLE["VIEW_NAME"  ]);
								if ( Sql.IsEmptyString(sMODULE_NAME) )
								{
									throw(new Exception("sMODULE_NAME should not be empty for table " + sTABLE_NAME));
								}
								
								int nACLACCESS = Security.GetUserAccess(sMODULE_NAME, "edit");
								// 11/11/2009 Paul.  First check if the user has access to this module. 
								if ( nACLACCESS >= 0 )
								{
									// 07/16/2019 Paul.  Add support for MassDelete. 
									using ( IDbTransaction trn = Sql.BeginTransaction(con) )
									{
										try
										{
											IDbCommand cmdDelete = SqlProcs.Factory(con, "sp" + sTABLE_NAME + "_Delete");
											cmdDelete.Transaction = trn;
											foreach ( Guid gID in arrID_LIST )
											{
												bool      bRecordExists              = false;
												bool      bAccessAllowed             = false;
												Guid      gLOCAL_ASSIGNED_USER_ID    = Guid.Empty;
												DataRow   rowCurrent                 = null;
												DataTable dtCurrent                  = new DataTable();
												sSQL = "select *"              + ControlChars.CrLf
												     + "  from " + sTABLE_NAME + ControlChars.CrLf
												     + " where 1 = 1"          + ControlChars.CrLf;
												using ( IDbCommand cmd = con.CreateCommand() )
												{
													cmd.CommandText = sSQL;
													cmd.Transaction = trn;
													Sql.AppendParameter(cmd, gID, "ID");
													using ( DbDataAdapter da = dbf.CreateDataAdapter() )
													{
														((IDbDataAdapter)da).SelectCommand = cmd;
														// 11/27/2009 Paul.  It may be useful to log the SQL during errors at this location. 
														try
														{
															da.Fill(dtCurrent);
														}
														catch
														{
															SplendidError.SystemError(new StackTrace(true).GetFrame(0), Sql.ExpandParameters(cmd));
															throw;
														}
														if ( dtCurrent.Rows.Count > 0 )
														{
															rowCurrent = dtCurrent.Rows[0];
															bRecordExists = true;
															// 01/18/2010 Paul.  Apply ACL Field Security. 
															if ( dtCurrent.Columns.Contains("ASSIGNED_USER_ID") )
															{
																gLOCAL_ASSIGNED_USER_ID = Sql.ToGuid(rowCurrent["ASSIGNED_USER_ID"]);
															}
														}
													}
												}
												// 06/04/2011 Paul.  We are not ready to handle conflicts. 
												//if ( !bConflicted )
												{
													if ( bRecordExists )
													{
														sSQL = "select count(*)"       + ControlChars.CrLf
														     + "  from " + sTABLE_NAME + ControlChars.CrLf;
														using ( IDbCommand cmd = con.CreateCommand() )
														{
															cmd.CommandText = sSQL;
															cmd.Transaction = trn;
															Security.Filter(cmd, sMODULE_NAME, "delete");
															Sql.AppendParameter(cmd, gID, "ID");
															try
															{
																if ( Sql.ToInteger(cmd.ExecuteScalar()) > 0 )
																{
																	// 06/08/2021 Paul.  A global dashboard should allow anyone to delete, so simply treat as owner required. 
																	if ( sMODULE_NAME == "Dashboard" )
																		nACLACCESS = ACL_ACCESS.OWNER;
																	if ( (nACLACCESS > ACL_ACCESS.OWNER) || (nACLACCESS == ACL_ACCESS.OWNER && Security.USER_ID == gLOCAL_ASSIGNED_USER_ID) || !dtCurrent.Columns.Contains("ASSIGNED_USER_ID") )
																		bAccessAllowed = true;
																}
															}
															catch
															{
																SplendidError.SystemError(new StackTrace(true).GetFrame(0), Sql.ExpandParameters(cmd));
																throw;
															}
														}
													}
													if ( bAccessAllowed )
													{
														// 07/16/2019 Paul.  Add support for MassDelete. 
														foreach(IDbDataParameter par in cmdDelete.Parameters)
														{
															string sParameterName = Sql.ExtractDbName(cmdDelete, par.ParameterName).ToUpper();
															if ( sParameterName == "ID" )
																par.Value = gID;
															else if ( sParameterName == "MODIFIED_USER_ID" )
																par.Value = Sql.ToDBGuid(Security.USER_ID);
															else
																par.Value = DBNull.Value;
														}
														cmdDelete.ExecuteScalar();
													}
													else
													{
														throw(new Exception(L10n.Term("ACL.LBL_NO_ACCESS")));
													}
												}
											}
											trn.Commit();
										}
										catch(Exception ex)
										{
											// 02/13/2017 Paul.  Capture this error as the following can generate an "This SqlTransaction has completed" error on Azure. 
											try
											{
												trn.Rollback();
											}
											catch
											{
											}
											SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
											throw;
										}
									}
								}
								else
								{
									throw(new Exception(L10n.Term("ACL.LBL_NO_ACCESS")));
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

		public class DeleteRelatedItemParameters
		{
			public string ModuleName    { get; set; }
			public Guid   ID            { get; set; }
			public string RelatedModule { get; set; }
			public Guid   RelatedID     { get; set; }
		}

		[HttpPost("[action]")]
		public void DeleteRelatedItem([FromBody] DeleteRelatedItemParameters input)
		{
			string ModuleName    = Sql.ToString(input.ModuleName   );
			Guid   ID            = Sql.ToGuid  (input.ID           );
			string RelatedModule = Sql.ToString(input.RelatedModule);
			Guid   RelatedID     = Sql.ToGuid  (input.RelatedID    );
			if ( Sql.IsEmptyString(ModuleName) )
				throw(new Exception("The module name must be specified."));
			string sTABLE_NAME = Sql.ToString(Application["Modules." + ModuleName + ".TableName"]);
			if ( Sql.IsEmptyString(sTABLE_NAME) )
				throw(new Exception("Unknown module: " + ModuleName));
			// 08/22/2011 Paul.  Add admin control to REST API. 
			int nACLACCESS = Security.GetUserAccess(ModuleName, "edit");
			if ( !Security.IsAuthenticated() || !Sql.ToBoolean(Application["Modules." + ModuleName + ".RestEnabled"]) || nACLACCESS < 0 )
			{
				// 09/06/2017 Paul.  Include module name in error. 
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS") + ": " + Sql.ToString(ModuleName)));
			}
			
			if ( Sql.IsEmptyString(RelatedModule) )
				throw(new Exception("The related module name must be specified."));
			string sRELATED_TABLE = Sql.ToString(Application["Modules." + RelatedModule + ".TableName"]);
			if ( Sql.IsEmptyString(sRELATED_TABLE) )
				throw(new Exception("Unknown module: " + RelatedModule));
			// 08/22/2011 Paul.  Add admin control to REST API. 
			nACLACCESS = Security.GetUserAccess(RelatedModule, "edit");
			if ( !Security.IsAuthenticated() || !Sql.ToBoolean(Application["Modules." + RelatedModule + ".RestEnabled"]) || nACLACCESS < 0 )
			{
				// 09/06/2017 Paul.  Include module name in error. 
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS") + ": " + Sql.ToString(RelatedModule)));
			}
			// 07/05/2020 Paul.  Need to restrict admin table updates.  Use table names because we know they are all uppercase. 
			if ( (sTABLE_NAME == "USERS" && sRELATED_TABLE == "ACL_ROLES") || (sTABLE_NAME == "ACL_ROLES" && sRELATED_TABLE == "USERS") || (sTABLE_NAME == "USERS" && sRELATED_TABLE == "TEAMS") || (sTABLE_NAME == "TEAMS" && sRELATED_TABLE == "USERS") )
			{
				if ( Security.AdminUserAccess("Users", "view") < 0 )
				{
					throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
				}
			}
		
			string sRELATIONSHIP_TABLE = sTABLE_NAME + "_" + sRELATED_TABLE;
			string sMODULE_FIELD_NAME  = SplendidCRM.Crm.Modules.SingularTableName(sTABLE_NAME   ) + "_ID";
			string sRELATED_FIELD_NAME = SplendidCRM.Crm.Modules.SingularTableName(sRELATED_TABLE) + "_ID";
			// 11/24/2012 Paul.  In the special cases of Accounts Related and Contacts Reports To, we need to correct the field name. 
			if ( sMODULE_FIELD_NAME == "ACCOUNT_ID" && sRELATED_FIELD_NAME == "ACCOUNT_ID" )
			{
				sRELATIONSHIP_TABLE = "ACCOUNTS_MEMBERS";
				sRELATED_FIELD_NAME = "PARENT_ID";
			}
			else if ( sMODULE_FIELD_NAME == "CONTACT_ID" && sRELATED_FIELD_NAME == "CONTACT_ID" )
			{
				sRELATIONSHIP_TABLE = "CONTACTS_DIRECT_REPORTS";
				sRELATED_FIELD_NAME = "REPORTS_TO_ID";
			}
			// 05/27/2020 Paul.  Correct some relationships. 
			// 10/15/2020 Paul.  Admin can remove users from teams and teams from users. 
			else if ( sRELATIONSHIP_TABLE == "USERS_TEAMS" || sRELATIONSHIP_TABLE == "TEAMS_USERS" )
			{
				sRELATIONSHIP_TABLE = "USERS_TEAM_MEMBERSHIPS";
			}
			// 03/09/2021 Paul.  Correct ROLE_ID field name. 
			else if ( sRELATIONSHIP_TABLE == "ACL_ROLES_USERS" && sMODULE_FIELD_NAME == "ACL_ROLE_ID" )
			{
				sMODULE_FIELD_NAME = "ROLE_ID";
			}
			// 08/23/2021 Paul.  Correct azure relationships. 
			else if ( sRELATIONSHIP_TABLE == "AZURE_ORDERS_AZURE_APP_UPDATES" )
			{
				sRELATIONSHIP_TABLE = "AZURE_APP_UPDATES_ORDERS";
				sRELATED_FIELD_NAME = "APP_UPDATE_ID";
			}
			else if ( sRELATIONSHIP_TABLE == "AZURE_APP_UPDATES_AZURE_ORDERS" )
			{
				sRELATIONSHIP_TABLE = "AZURE_APP_UPDATES_ORDERS";
				sMODULE_FIELD_NAME  = "APP_UPDATE_ID";
			}
			else if ( sRELATIONSHIP_TABLE == "AZURE_APP_PRICES_AZURE_SERVICE_LEVELS" )
			{
				sRELATIONSHIP_TABLE = "AZURE_APP_SERVICE_LEVELS";
				sMODULE_FIELD_NAME  = "APP_PRICE_ID";
				sRELATED_FIELD_NAME = "SERVICE_LEVEL_ID";
			}
			else if ( sRELATIONSHIP_TABLE == "AZURE_SERVICE_LEVELS_AZURE_APP_PRICES" )
			{
				sRELATIONSHIP_TABLE = "AZURE_APP_SERVICE_LEVELS";
				sMODULE_FIELD_NAME  = "SERVICE_LEVEL_ID";
				sRELATED_FIELD_NAME = "APP_PRICE_ID";
			}
			// 11/21/2021 Paul.  Document Revisions is not a valid module name, so we need to manully convert. 
			else if ( sRELATIONSHIP_TABLE == "DOCUMENTS_DOCUMENTS" )
			{
				sRELATIONSHIP_TABLE = "DOCUMENT_REVISIONS";
				sMODULE_FIELD_NAME  = "DOCUMENT_ID";
				sRELATED_FIELD_NAME = "ID";
			}
			
			// 06/04/2011 Paul.  For relationships, we first need to check the access rights of the parent record. 
			bool bExcludeSystemTables = true;
			if ( Security.AdminUserAccess(ModuleName, "edit") >= 0 )
			{
				bExcludeSystemTables = false;
			}
			// 02/27/2021 Paul.  Both vwACCOUNTS_PROJECTS and vwPROJECTS_ACCOUNTS are both in RestTables, so we need a secondary check for the update procedure, which might not exist. 
			SplendidCRM.DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				IDbCommand cmdUpdate  = null;
				try
				{
					// 03/09/2021 Paul.  Correct procedure name. 
					cmdUpdate = SqlProcs.Factory(con, "sp" + sRELATIONSHIP_TABLE + "_Delete");
				}
				catch
				{
				}
				DataTable dtSYNC_TABLES = SplendidCache.RestTables("vw" + sRELATIONSHIP_TABLE, bExcludeSystemTables);
				if ( cmdUpdate == null || (dtSYNC_TABLES != null && dtSYNC_TABLES.Rows.Count == 0) )
				{
					sRELATIONSHIP_TABLE = sRELATED_TABLE + "_" + sTABLE_NAME;
					try
					{
						cmdUpdate = SqlProcs.Factory(con, "sp" + sRELATIONSHIP_TABLE + "_Delete");
					}
					catch
					{
					}
					dtSYNC_TABLES = SplendidCache.RestTables("vw" + sRELATIONSHIP_TABLE, bExcludeSystemTables);
					if ( cmdUpdate == null || (dtSYNC_TABLES != null && dtSYNC_TABLES.Rows.Count == 0) )
					{
						throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS") + " to relationship between modules " + ModuleName + " and " + RelatedModule));
					}
				}
				DeleteRelatedItem(sTABLE_NAME, sRELATIONSHIP_TABLE, sMODULE_FIELD_NAME, ID, sRELATED_FIELD_NAME, RelatedID, bExcludeSystemTables);
			}
		}

		private void DeleteRelatedItem(string sTABLE_NAME, string sRELATIONSHIP_TABLE, string sMODULE_FIELD_NAME, Guid gID, string sRELATED_FIELD_NAME, Guid gRELATED_ID, bool bExcludeSystemTables)
		{
			try
			{
				if ( Security.IsAuthenticated() )
				{
					Regex  r        = new Regex(@"[^A-Za-z0-9_]");
					sTABLE_NAME = r.Replace(sTABLE_NAME, "").ToUpper();
					
					SplendidCRM.DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						// 06/03/2011 Paul.  Cache the Rest Table data. 
						// 11/26/2009 Paul.  System tables cannot be updated. 
						// 06/04/2011 Paul.  For relationships, we first need to check the access rights of the parent record. 
						using ( DataTable dtSYNC_TABLES = SplendidCache.RestTables(sTABLE_NAME, bExcludeSystemTables) )
						{
							string sSQL = String.Empty;
							if ( dtSYNC_TABLES != null && dtSYNC_TABLES.Rows.Count > 0 )
							{
								DataRow rowSYNC_TABLE = dtSYNC_TABLES.Rows[0];
								string sMODULE_NAME = Sql.ToString (rowSYNC_TABLE["MODULE_NAME"]);
								string sVIEW_NAME   = Sql.ToString (rowSYNC_TABLE["VIEW_NAME"  ]);
								if ( Sql.IsEmptyString(sMODULE_NAME) )
								{
									throw(new Exception("sMODULE_NAME should not be empty for table " + sTABLE_NAME));
								}
								
								int nACLACCESS = Security.GetUserAccess(sMODULE_NAME, "edit");
								// 11/11/2009 Paul.  First check if the user has access to this module. 
								if ( nACLACCESS >= 0 )
								{
									bool      bRecordExists              = false;
									bool      bAccessAllowed             = false;
									Guid      gLOCAL_ASSIGNED_USER_ID    = Guid.Empty;
									DataRow   rowCurrent                 = null;
									DataTable dtCurrent                  = new DataTable();
									sSQL = "select *"              + ControlChars.CrLf
									     + "  from " + sTABLE_NAME + ControlChars.CrLf
									     + " where DELETED = 0"    + ControlChars.CrLf;
									using ( IDbCommand cmd = con.CreateCommand() )
									{
										cmd.CommandText = sSQL;
										// 02/14/2018 Paul.  Azure can timeout, so lets wait for an hour. 
										cmd.CommandTimeout = 60 * 60;
										Sql.AppendParameter(cmd, gID, "ID");
										using ( DbDataAdapter da = dbf.CreateDataAdapter() )
										{
											((IDbDataAdapter)da).SelectCommand = cmd;
											// 11/27/2009 Paul.  It may be useful to log the SQL during errors at this location. 
											try
											{
												da.Fill(dtCurrent);
											}
											catch
											{
												SplendidError.SystemError(new StackTrace(true).GetFrame(0), Sql.ExpandParameters(cmd));
												throw;
											}
											if ( dtCurrent.Rows.Count > 0 )
											{
												rowCurrent = dtCurrent.Rows[0];
												bRecordExists = true;
												// 01/18/2010 Paul.  Apply ACL Field Security. 
												if ( dtCurrent.Columns.Contains("ASSIGNED_USER_ID") )
												{
													gLOCAL_ASSIGNED_USER_ID = Sql.ToGuid(rowCurrent["ASSIGNED_USER_ID"]);
												}
											}
										}
									}
									// 06/04/2011 Paul.  We are not ready to handle conflicts. 
									//if ( !bConflicted )
									{
										if ( bRecordExists )
										{
											sSQL = "select count(*)"       + ControlChars.CrLf
											     + "  from " + sTABLE_NAME + ControlChars.CrLf;
											using ( IDbCommand cmd = con.CreateCommand() )
											{
												cmd.CommandText = sSQL;
												Security.Filter(cmd, sMODULE_NAME, "delete");
												Sql.AppendParameter(cmd, gID, "ID");
												try
												{
													if ( Sql.ToInteger(cmd.ExecuteScalar()) > 0 )
													{
														if ( (nACLACCESS > ACL_ACCESS.OWNER) || (nACLACCESS == ACL_ACCESS.OWNER && Security.USER_ID == gLOCAL_ASSIGNED_USER_ID) || !dtCurrent.Columns.Contains("ASSIGNED_USER_ID") )
															bAccessAllowed = true;
													}
												}
												catch
												{
													SplendidError.SystemError(new StackTrace(true).GetFrame(0), Sql.ExpandParameters(cmd));
													throw;
												}
											}
										}
										if ( bAccessAllowed )
										{
											// 11/24/2012 Paul.  We do not need to check for RestTable access as that step was already done. 
											IDbCommand cmdDelete = SqlProcs.Factory(con, "sp" + sRELATIONSHIP_TABLE + "_Delete");
											using ( IDbTransaction trn = Sql.BeginTransaction(con) )
											{
												try
												{
													cmdDelete.Transaction = trn;
													foreach(IDbDataParameter par in cmdDelete.Parameters)
													{
														string sParameterName = Sql.ExtractDbName(cmdDelete, par.ParameterName).ToUpper();
														if ( sParameterName == sMODULE_FIELD_NAME )
															par.Value = gID;
														else if ( sParameterName == sRELATED_FIELD_NAME )
															par.Value = gRELATED_ID;
														else if ( sParameterName == "MODIFIED_USER_ID" )
															par.Value = Sql.ToDBGuid(Security.USER_ID);
														else
															par.Value = DBNull.Value;
													}
													cmdDelete.ExecuteScalar();
													trn.Commit();
												}
												catch(Exception ex)
												{
													// 02/13/2017 Paul.  Capture this error as the following can generate an "This SqlTransaction has completed" error on Azure. 
													try
													{
														trn.Rollback();
													}
													catch
													{
													}
													SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
													throw;
												}
											}
										}
										else
										{
											throw(new Exception(L10n.Term("ACL.LBL_NO_ACCESS")));
										}
									}
								}
								else
								{
									throw(new Exception(L10n.Term("ACL.LBL_NO_ACCESS")));
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

		// 10/16/2020 Paul.  Regions.Countries needs a way to delete by value. 
		public class DeleteRelatedValueParameters
		{
			public string ModuleName   { get; set; }
			public Guid   ID           { get; set; }
			public string RelatedTable { get; set; }
			public string RelatedValue { get; set; }
		}

		[HttpPost("[action]")]
		public void DeleteRelatedValue([FromBody] DeleteRelatedValueParameters input)
		{
			string ModuleName   = Sql.ToString(input.ModuleName  );
			Guid   ID           = Sql.ToGuid  (input.ID          );
			string RelatedTable = Sql.ToString(input.RelatedTable);
			string RelatedValue = Sql.ToString(input.RelatedValue);
			if ( Sql.IsEmptyString(ModuleName) )
				throw(new Exception("The module name must be specified."));
			string sTABLE_NAME = Sql.ToString(Application["Modules." + ModuleName + ".TableName"]);
			if ( Sql.IsEmptyString(sTABLE_NAME) )
				throw(new Exception("Unknown module: " + ModuleName));
			// 08/22/2011 Paul.  Add admin control to REST API. 
			int nACLACCESS = Security.GetUserAccess(ModuleName, "edit");
			if ( !Security.IsAuthenticated() || !Sql.ToBoolean(Application["Modules." + ModuleName + ".RestEnabled"]) || nACLACCESS < 0 )
			{
				// 09/06/2017 Paul.  Include module name in error. 
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS") + ": " + Sql.ToString(ModuleName)));
			}
			
			string sRELATIONSHIP_TABLE = sTABLE_NAME + "_" + RelatedTable;
			string sMODULE_FIELD_NAME  = SplendidCRM.Crm.Modules.SingularTableName(sTABLE_NAME ) + "_ID";
			string sRELATED_FIELD_NAME = SplendidCRM.Crm.Modules.SingularTableName(RelatedTable);
			bool bExcludeSystemTables = true;
			// 06/04/2011 Paul.  For relationships, we first need to check the access rights of the parent record. 
			if ( Security.AdminUserAccess(ModuleName, "edit") >= 0 )
			{
				bExcludeSystemTables = false;
			}
			DataTable dtSYNC_TABLES = SplendidCache.RestTables("vw" + sRELATIONSHIP_TABLE, bExcludeSystemTables);
			if ( dtSYNC_TABLES != null && dtSYNC_TABLES.Rows.Count == 0 )
			{
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS") + " to relationship between modules " + ModuleName + " and " + RelatedTable));
			}
			DeleteRelatedValue(sTABLE_NAME, sRELATIONSHIP_TABLE, sMODULE_FIELD_NAME, ID, sRELATED_FIELD_NAME, RelatedValue, bExcludeSystemTables);
		}

		private void DeleteRelatedValue(string sTABLE_NAME, string sRELATIONSHIP_TABLE, string sMODULE_FIELD_NAME, Guid gID, string sRELATED_FIELD_NAME, string sRELATED_VALUE, bool bExcludeSystemTables)
		{
			try
			{
				if ( Security.IsAuthenticated() )
				{
					Regex  r        = new Regex(@"[^A-Za-z0-9_]");
					sRELATIONSHIP_TABLE = r.Replace(sRELATIONSHIP_TABLE, "").ToUpper();
					
					SplendidCRM.DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						// 06/03/2011 Paul.  Cache the Rest Table data. 
						// 11/26/2009 Paul.  System tables cannot be updated. 
						// 06/04/2011 Paul.  For relationships, we first need to check the access rights of the parent record. 
						using ( DataTable dtSYNC_TABLES = SplendidCache.RestTables("vw" + sRELATIONSHIP_TABLE, bExcludeSystemTables) )
						{
							string sSQL = String.Empty;
							if ( dtSYNC_TABLES != null && dtSYNC_TABLES.Rows.Count > 0 )
							{
								DataRow rowSYNC_TABLE = dtSYNC_TABLES.Rows[0];
								string sMODULE_NAME = Sql.ToString (rowSYNC_TABLE["MODULE_NAME"]);
								string sVIEW_NAME   = Sql.ToString (rowSYNC_TABLE["VIEW_NAME"  ]);
								if ( Sql.IsEmptyString(sMODULE_NAME) )
								{
									throw(new Exception("sMODULE_NAME should not be empty for table " + sRELATIONSHIP_TABLE));
								}
								
								int nACLACCESS = Security.GetUserAccess(sMODULE_NAME, "edit");
								// 11/11/2009 Paul.  First check if the user has access to this module. 
								if ( nACLACCESS >= 0 )
								{
									bool      bRecordExists              = false;
									bool      bAccessAllowed             = false;
									Guid      gLOCAL_ASSIGNED_USER_ID    = Guid.Empty;
									DataRow   rowCurrent                 = null;
									DataTable dtCurrent                  = new DataTable();
									sSQL = "select *"              + ControlChars.CrLf
									     + "  from " + sTABLE_NAME + ControlChars.CrLf
									     + " where DELETED = 0"    + ControlChars.CrLf;
									using ( IDbCommand cmd = con.CreateCommand() )
									{
										cmd.CommandText = sSQL;
										// 02/14/2018 Paul.  Azure can timeout, so lets wait for an hour. 
										cmd.CommandTimeout = 60 * 60;
										Sql.AppendParameter(cmd, gID, "ID");
										using ( DbDataAdapter da = dbf.CreateDataAdapter() )
										{
											((IDbDataAdapter)da).SelectCommand = cmd;
											// 11/27/2009 Paul.  It may be useful to log the SQL during errors at this location. 
											try
											{
												da.Fill(dtCurrent);
											}
											catch
											{
												SplendidError.SystemError(new StackTrace(true).GetFrame(0), Sql.ExpandParameters(cmd));
												throw;
											}
											if ( dtCurrent.Rows.Count > 0 )
											{
												rowCurrent = dtCurrent.Rows[0];
												bRecordExists = true;
												// 01/18/2010 Paul.  Apply ACL Field Security. 
												if ( dtCurrent.Columns.Contains("ASSIGNED_USER_ID") )
												{
													gLOCAL_ASSIGNED_USER_ID = Sql.ToGuid(rowCurrent["ASSIGNED_USER_ID"]);
												}
											}
										}
									}
									if ( bRecordExists )
									{
										sSQL = "select count(*)"       + ControlChars.CrLf
											    + "  from " + sTABLE_NAME + ControlChars.CrLf;
										using ( IDbCommand cmd = con.CreateCommand() )
										{
											cmd.CommandText = sSQL;
											Security.Filter(cmd, sMODULE_NAME, "delete");
											Sql.AppendParameter(cmd, gID, "ID");
											try
											{
												if ( Sql.ToInteger(cmd.ExecuteScalar()) > 0 )
												{
													if ( (nACLACCESS > ACL_ACCESS.OWNER) || (nACLACCESS == ACL_ACCESS.OWNER && Security.USER_ID == gLOCAL_ASSIGNED_USER_ID) || !dtCurrent.Columns.Contains("ASSIGNED_USER_ID") )
														bAccessAllowed = true;
												}
											}
											catch
											{
												SplendidError.SystemError(new StackTrace(true).GetFrame(0), Sql.ExpandParameters(cmd));
												throw;
											}
										}
									}
									if ( bAccessAllowed )
									{
										IDbCommand cmdDelete = SqlProcs.Factory(con, "sp" + sRELATIONSHIP_TABLE + "_Delete");
										using ( IDbTransaction trn = Sql.BeginTransaction(con) )
										{
											try
											{
												cmdDelete.Transaction = trn;
												foreach(IDbDataParameter par in cmdDelete.Parameters)
												{
													string sParameterName = Sql.ExtractDbName(cmdDelete, par.ParameterName).ToUpper();
													if ( sParameterName == sMODULE_FIELD_NAME )
														par.Value = gID;
													else if ( sParameterName == sRELATED_FIELD_NAME )
														par.Value = sRELATED_VALUE;
													else if ( sParameterName == "MODIFIED_USER_ID" )
														par.Value = Sql.ToDBGuid(Security.USER_ID);
													else
														par.Value = DBNull.Value;
												}
												cmdDelete.ExecuteScalar();
												trn.Commit();
											}
											catch(Exception ex)
											{
												// 02/13/2017 Paul.  Capture this error as the following can generate an "This SqlTransaction has completed" error on Azure. 
												try
												{
													trn.Rollback();
												}
												catch
												{
												}
												SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
												throw;
											}
										}
									}
									else
									{
										throw(new Exception(L10n.Term("ACL.LBL_NO_ACCESS")));
									}
								}
								else
								{
									throw(new Exception(L10n.Term("ACL.LBL_NO_ACCESS")));
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
		#endregion
	}
}
