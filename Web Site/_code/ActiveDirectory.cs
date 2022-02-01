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
using System.Xml;
using System.Web;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography.X509Certificates;
using System.Linq;
// Install-Package System.IdentityModel.Tokens.ValidatingIssuerNameRegistry
// ValidatingIssuerNameRegistry
// http://www.cloudidentity.com/blog/2013/02/08/multitenant-sts-and-token-validation-4/

using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Logging;
using System.Threading.Tasks;
using System.Globalization;

namespace SplendidCRM
{
	[DataContract]
	public class Office365AccessToken
	{
		[DataMember] public string token_type    { get; set; }
		[DataMember] public string scope         { get; set; }
		[DataMember] public string expires_in    { get; set; }
		[DataMember] public string expires_on    { get; set; }
		[DataMember] public string access_token  { get; set; }
		[DataMember] public string refresh_token { get; set; }

		public string AccessToken
		{
			get { return access_token;  }
			set { access_token = value; }
		}
		public string RefreshToken
		{
			get { return refresh_token;  }
			set { refresh_token = value; }
		}
		public Int64 ExpiresInSeconds
		{
			get { return Sql.ToInt64(expires_in);  }
			set { expires_in = Sql.ToString(value); }
		}
		public string TokenType
		{
			get { return token_type;  }
			set { token_type = value; }
		}
	}

	// https://graph.microsoft.io/en-us/docs
	[DataContract]
	public class MicrosoftGraphProfile
	{
		[DataMember] public string id                { get; set; }
		[DataMember] public string userPrincipalName { get; set; }
		[DataMember] public string displayName       { get; set; }
		[DataMember] public string givenName         { get; set; }
		[DataMember] public string surname           { get; set; }
		[DataMember] public string jobTitle          { get; set; }
		[DataMember] public string mail              { get; set; }
		[DataMember] public string officeLocation    { get; set; }
		[DataMember] public string preferredLanguage { get; set; }
		[DataMember] public string mobilePhone       { get; set; }
		[DataMember] public string[] businessPhones  { get; set; }

		public string Name
		{
			get { return displayName; }
			set { displayName = value; }
		}
		public string FirstName
		{
			get { return givenName; }
			set { givenName = value; }
		}
		public string LastName
		{
			get { return surname; }
			set { surname = value; }
		}
		public string UserName
		{
			get { return userPrincipalName; }
			set { userPrincipalName = value; }
		}
		public string EmailAddress
		{
			get { return mail; }
			set { mail = value; }
		}
	}

	public class ActiveDirectory
	{
		private SplendidCRM.DbProviderFactories  DbProviderFactories = new SplendidCRM.DbProviderFactories();
		private HttpApplicationState Application        = new HttpApplicationState();
		private HttpContext          Context            ;
		private HttpSessionState     Session            ;
		private SplendidError        SplendidError      ;
		private SplendidInit         SplendidInit       ;
		private L10N                 L10n               ;

		public ActiveDirectory(IHttpContextAccessor httpContextAccessor, HttpSessionState Session, SplendidError SplendidError, SplendidInit SplendidInit)
		{
			this.Context             = httpContextAccessor.HttpContext;
			this.Session             = Session            ;
			this.SplendidError       = SplendidError      ;
			this.SplendidInit        = SplendidInit       ;
			this.L10n                = new L10N(Sql.ToString(Session["USER_LANG"]));
		}

		// 12/25/2018 Paul.  Logout should perform Azure or ADFS logout. 
		public string AzureLogout()
		{
			HttpRequest          Request     = Context.Request;

			string sAadTenantDomain  = Sql.ToString(Application["CONFIG.Azure.SingleSignOn.AadTenantDomain"]);
			string sAuthority        = "https://login.microsoftonline.com/" + sAadTenantDomain + "/oauth2/logout";
			string sRedirectURL = "https://" + Request.Host.Host + Request.PathBase + "/React";
			string sRequestURL = sAuthority + "?post_logout_redirect_uri=" + HttpUtility.UrlEncode(sRedirectURL);
			Debug.WriteLine(sRequestURL);
			return sRequestURL;
		}

		// 05/02/2017 Paul.  Need a separate flag for the mobile client. 
		public async Task<Guid> AzureValidateJwt(string sToken, bool bMobileClient)
		{
			Guid gUSER_ID       = Guid.Empty;
			Guid gUSER_LOGIN_ID = Guid.Empty;
			string sError = String.Empty;
			try
			{
				//string sAadTenantDomain     = Sql.ToString(Application["CONFIG.Azure.SingleSignOn.AadTenantDomain"   ]);
				// https://stackoverflow.com/questions/53966951/asp-net-core-azureadjwtbearer-issuer-validation-failure/53973136
				string sValidIssuer         = Sql.ToString(Application["CONFIG.Azure.SingleSignOn.ValidIssuer"       ]);
				string sAadClientId         = Sql.ToString(Application["CONFIG.Azure.SingleSignOn.AadClientId"       ]);
				string sAadTenantId         = Sql.ToString(Application["CONFIG.Azure.SingleSignOn.AadTenantId"       ]);
				// 05/02/2017 Paul.  Need a separate flag for the mobile client. 
				// 12/05/2018 Paul.  Allow authorization by USER_NAME instead of by EMAIL1. 
				bool   bAuthByUserName     = Sql.ToBoolean(Application["CONFIG.Azure.SingleSignOn.AuthByUserName"]);
				if ( bMobileClient )
				{
					// 05/03/2017 Paul.  As we are using the MobileClientId to validate the token, we must also use it as the resourceUrl when acquiring the token. 
					sAadClientId   = Sql.ToString(Application["CONFIG.Azure.SingleSignOn.MobileClientId"  ]);
				}
				if ( Sql.IsEmptyString(sValidIssuer) )
				{
					sValidIssuer = "https://sts.windows.net/{0}/";
				}
#if DEBUG
				IdentityModelEventSource.ShowPII = true;
#endif
				// https://www.c-sharpcorner.com/article/how-to-validate-azure-ad-token-using-console-application/
				String sDiscoveryEndpoint = String.Format(CultureInfo.InvariantCulture, "https://login.microsoftonline.com/{0}/.well-known/openid-configuration", sAadTenantId);
				ConfigurationManager<OpenIdConnectConfiguration> configManager = new ConfigurationManager<OpenIdConnectConfiguration>(sDiscoveryEndpoint, new OpenIdConnectConfigurationRetriever());  
				OpenIdConnectConfiguration config = await configManager.GetConfigurationAsync();
  
				JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
  				TokenValidationParameters validationParameters = new TokenValidationParameters
				{
					ValidAudience     = sAadClientId,
					ValidIssuer       = String.Format(CultureInfo.InvariantCulture, sValidIssuer, sAadTenantId),
					IssuerSigningKeys = config.SigningKeys,
					ValidateLifetime  = false,
				};
 
				SecurityToken validatedToken = (SecurityToken) new JwtSecurityToken();
				// Throws an Exception as the token is invalid (expired, invalid-formatted, etc.)
				System.Security.Claims.ClaimsPrincipal identity = tokenHandler.ValidateToken(sToken, validationParameters, out validatedToken);
				if ( identity != null )
				{
					string sUSER_NAME  = String.Empty;
					string sLAST_NAME  = String.Empty;
					string sFIRST_NAME = String.Empty;
					string sEMAIL1     = String.Empty;
					foreach ( System.Security.Claims.Claim claim in identity.Claims )
					{
						Debug.WriteLine(claim.Type + " = " + claim.Value);
						// http://schemas.microsoft.com/claims/authnmethodsreferences = pwd
						// http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress = paul@splendidcrm.com
						// http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname = Rony
						// http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname = Paul
						// http://schemas.microsoft.com/identity/claims/identityprovider = live.com
						// http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name = live.com#paul@splendidcrm.com
						// iat = 1484136100
						// nbf = 1484136100
						// exp = 1484140000
						// name = Paul Rony
						// platf = 3
						// ver = 1.0

						// 01/15/2017 Paul.  Alternate login. 
						// http://schemas.microsoft.com/claims/authnmethodsreferences = pwd
						// http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname = Rony
						// http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname = Paul
						// http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name = paul@splendidcrm.onmicrosoft.com
						// http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn = paul@splendidcrm.onmicrosoft.com
						// iat = 1484512667
						// nbf = 1484512667
						// exp = 1484516567
						// name = Paul Rony
						// platf = 3
						// ver = 1.0
						switch ( claim.Type )
						{
							// 12/25/2018 Paul.  Remove live.com# prefix. 
							case "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"          :  sUSER_NAME  = claim.Value.Replace("live.com#", "");  break;
							case "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname"       :  sLAST_NAME  = claim.Value;  break;
							case "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname"     :  sFIRST_NAME = claim.Value;  break;
							case "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"  :  sEMAIL1     = claim.Value;  break;
						}
					}
					if ( Sql.IsEmptyString(sEMAIL1) && sUSER_NAME.Contains("@") )
						sEMAIL1 = sUSER_NAME;
					if ( !Sql.IsEmptyString(sEMAIL1) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							// 12/05/2018 Paul.  Allow authorization by USER_NAME instead of by EMAIL1. 
							if ( bAuthByUserName )
							{
								sSQL = "select ID                    " + ControlChars.CrLf
								     + "  from vwUSERS_Login         " + ControlChars.CrLf
								     + " where USER_NAME = @EMAIL1   " + ControlChars.CrLf;
							}
							else
							{
								sSQL = "select ID                    " + ControlChars.CrLf
								     + "  from vwUSERS_Login         " + ControlChars.CrLf
								     + " where EMAIL1 = @EMAIL1      " + ControlChars.CrLf;
							}
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@EMAIL1", sEMAIL1.ToLower());
								gUSER_ID = Sql.ToGuid(cmd.ExecuteScalar());
								if ( Sql.IsEmptyGuid(gUSER_ID) )
								{
									sError = "SECURITY: failed attempted login for " + sEMAIL1 + " using Azure AD/REST API";
									SplendidError.SystemMessage("Warning", new StackTrace(true).GetFrame(0), sError);
								}
							}
						}
					}
					else
					{
						sError = "SECURITY: Failed attempted login using ADFS. Missing Email ID from Claim token.";
						SplendidError.SystemMessage("Warning", new StackTrace(true).GetFrame(0), sError);
					}
				}
				else
				{
					sError = "SECURITY: failed attempted login using Azure AD. No SecurityToken identities found.";
					SplendidError.SystemMessage("Warning", new StackTrace(true).GetFrame(0), sError);
				}
			}
			catch(Exception ex)
			{
				string sUSER_NAME = "(Unknown Azure AD)";
				sError = "SECURITY: failed attempted login for " + sUSER_NAME + " using Azure AD/REST API. " + ex.Message;
				SplendidError.SystemMessage("Warning", new StackTrace(true).GetFrame(0), sError);
			}
			if ( !Sql.IsEmptyString(sError) )
			{
				throw(new Exception(sError));
			}
			return gUSER_ID;
		}

		// 12/25/2018 Paul.  Logout should perform Azure or ADFS logout. 
		public string FederationServicesLogout()
		{
			HttpRequest          Request     = Context.Request;

			string sAuthority   = Sql.ToString(Application["CONFIG.ADFS.SingleSignOn.Authority"]);
			if ( !sAuthority.EndsWith("/") )
				sAuthority += "/";
			sAuthority += "adfs/oauth2/logout";
			string sRedirectURL = "https://" + Request.Host.Host + Request.PathBase + "/React";
			string sRequestURL = sAuthority + "?post_logout_redirect_uri=" + HttpUtility.UrlEncode(sRedirectURL);
			Debug.WriteLine(sRequestURL);
			return sRequestURL;
		}

		// 05/02/2017 Paul.  Need a separate flag for the mobile client. 
		public async Task<Guid> FederationServicesValidateJwt(string sToken, bool bMobileClient)
		{
			Guid gUSER_ID       = Guid.Empty;
			Guid gUSER_LOGIN_ID = Guid.Empty;
			string sError       = String.Empty;
			try
			{
				//string sRealm      = Sql.ToString(Application["CONFIG.ADFS.SingleSignOn.Realm"     ]);
				string sAuthority  = Sql.ToString(Application["CONFIG.ADFS.SingleSignOn.Authority" ]);
				string sClientId   = Sql.ToString(Application["CONFIG.ADFS.SingleSignOn.ClientId"  ]);
				// 05/02/2017 Paul.  Need a separate flag for the mobile client. 
				//if ( bMobileClient )
				//	sClientId   = Sql.ToString(Application["CONFIG.ADFS.SingleSignOn.MobileClientId"  ]);
				// 01/08/2018 Paul.  ADFS 3.0 will require us to register both client and mobile as valid audiences. 
				string sMobileClientId = Sql.ToString(Application["CONFIG.ADFS.SingleSignOn.MobileClientId"  ]);
				if ( !sAuthority.EndsWith("/") )
					sAuthority += "/";

#if DEBUG
				IdentityModelEventSource.ShowPII = true;
#endif
				// https://docs.microsoft.com/en-us/windows-server/identity/ad-fs/development/enabling-openid-connect-with-ad-fs
				String sDiscoveryEndpoint = sAuthority + "adfs/.well-known/openid-configuration";
				ConfigurationManager<OpenIdConnectConfiguration> configManager = new ConfigurationManager<OpenIdConnectConfiguration>(sDiscoveryEndpoint, new OpenIdConnectConfigurationRetriever());
				OpenIdConnectConfiguration config = await configManager.GetConfigurationAsync();
  
				JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
  				TokenValidationParameters validationParameters = new TokenValidationParameters
				{
					ValidAudience     = sClientId,
					ValidIssuer       = sAuthority + "adfs",
					IssuerSigningKeys = config.SigningKeys,
					ValidateLifetime  = false,
				};
 
				SecurityToken validatedToken = (SecurityToken) new JwtSecurityToken();
				// Throws an Exception as the token is invalid (expired, invalid-formatted, etc.)
				System.Security.Claims.ClaimsPrincipal identity = tokenHandler.ValidateToken(sToken, validationParameters, out validatedToken);
				if ( identity != null )
				{
					string sUSER_NAME  = String.Empty;
					string sFIRST_NAME = String.Empty;
					string sLAST_NAME  = String.Empty;
					string sEMAIL1     = String.Empty;
					foreach ( System.Security.Claims.Claim claim in identity.Claims )
					{
						Debug.WriteLine(claim.Type + " = " + claim.Value);
						// http://schemas.microsoft.com/ws/2008/06/identity/claims/authenticationinstant = 1484346928
						// http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier = zwkv1IHDGSe1FDyDrc6LO2+XxDD0LWfs1SL35ZdOxF0=
						// http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn = paulrony@merchantware.local
						// http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name = MERCHANTWARE\paulrony
						// aud = FD3ABD16-F96F-4BE7-98DB-D45C55DB0048
						// iss = https://adfs4.splendidcrm.com/adfs
						// iat = 1484346928
						// exp = 1484350528
						// nonce = 8f864f39-b44e-4d02-9cb2-38d5113643af
						switch ( claim.Type )
						{
							case "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"          :  sUSER_NAME  = claim.Value;  break;
							// 01/08/2019 Paul.  Our instructions say to map SAM-Account-Name to Name ID, not name. 
							case "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier":  sUSER_NAME  = claim.Value;  break;
							case "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname"       :  sLAST_NAME  = claim.Value;  break;
							case "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname"     :  sFIRST_NAME = claim.Value;  break;
							case "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"  :  sEMAIL1     = claim.Value;  break;
						}
					}
					string[] arrUserName = sUSER_NAME.Split('\\');
					if ( arrUserName.Length > 1 )
						sUSER_NAME   = arrUserName[1];
					else
						sUSER_NAME   = arrUserName[0];
					if ( !Sql.IsEmptyString(sUSER_NAME) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							sSQL = "select ID                    " + ControlChars.CrLf
							     + "  from vwUSERS_Login         " + ControlChars.CrLf
							     + " where USER_NAME = @USER_NAME" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@USER_NAME", sUSER_NAME.ToLower());
								gUSER_ID = Sql.ToGuid(cmd.ExecuteScalar());
								if ( Sql.IsEmptyGuid(gUSER_ID) )
								{
									sError = "SECURITY: failed attempted login for " + sUSER_NAME + " using ADFS/REST API.";
									SplendidError.SystemMessage("Warning", new StackTrace(true).GetFrame(0), sError);
								}
							}
						}
					}
					else
					{
						sError = "SECURITY: Failed attempted login using ADFS/REST API. Missing Username/Name ID from Claim token.";
						SplendidError.SystemMessage("Warning", new StackTrace(true).GetFrame(0), sError);
					}
				}
				else
				{
					sError = "SECURITY: failed attempted login using ADFS/REST API. No SecurityToken identities found.";
					SplendidError.SystemMessage("Warning", new StackTrace(true).GetFrame(0), sError);
				}
			}
			catch(Exception ex)
			{
				string sUSER_NAME = "(Unknown ADFS)";
				sError = "SECURITY: failed attempted login for " + sUSER_NAME + " using ADFS/REST API. " + ex.Message;
				SplendidError.SystemMessage("Warning", new StackTrace(true).GetFrame(0), sError);
			}
			return gUSER_ID;
		}
	}
}

