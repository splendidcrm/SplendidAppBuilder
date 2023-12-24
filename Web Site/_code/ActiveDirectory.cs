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
using System.Xml;
using System.Web;
using System.Data;
using System.Diagnostics;

using Microsoft.Exchange.WebServices.Data;
using System.Runtime.Serialization;

using Microsoft.AspNetCore.Http;

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
		public Guid AzureValidateJwt(string sToken, bool bMobileClient, ref string sError)
		{
			Guid gUSER_ID       = Guid.Empty;
			Guid gUSER_LOGIN_ID = Guid.Empty;
			try
			{
				//string sAadTenantDomain     = Sql.ToString(Application["CONFIG.Azure.SingleSignOn.AadTenantDomain"   ]);
				string sAadClientId         = Sql.ToString(Application["CONFIG.Azure.SingleSignOn.AadClientId"       ]);
				//string sRealm               = Sql.ToString(Application["CONFIG.Azure.SingleSignOn.Realm"             ]);
				string sFederationMetadata  = Sql.ToString(Application["CONFIG.Azure.SingleSignOn.FederationMetadata"]);
				//string stsDiscoveryEndpoint = "https://login.microsoftonline.com/" + sAadTenantDomain + "/.well-known/openid-configuration";
				// 05/03/2017 Paul.  Instead of validating against the resource, validate against the clientId as it is easier. 
				//string sResourceUrl = Request.Url.ToString();
				//sResourceUrl = sResourceUrl.Substring(0, sResourceUrl.Length - "Rest.svc/Login".Length);
				// 05/02/2017 Paul.  Need a separate flag for the mobile client. 
				// 12/05/2018 Paul.  Allow authorization by USER_NAME instead of by EMAIL1. 
				bool   bAuthByUserName     = Sql.ToBoolean(Application["CONFIG.Azure.SingleSignOn.AuthByUserName"]);
				if ( bMobileClient )
				{
					// 05/03/2017 Paul.  As we are using the MobileClientId to validate the token, we must also use it as the resourceUrl when acquiring the token. 
					sAadClientId   = Sql.ToString(Application["CONFIG.Azure.SingleSignOn.MobileClientId"  ]);
				}

				// 02/14/2022 Paul.  Use the new metadata serializer. 
				// https://www.nuget.org/packages/Microsoft.IdentityModel.Protocols.WsFederation/
				Microsoft.IdentityModel.Protocols.WsFederation.WsFederationMetadataSerializer serializer = new Microsoft.IdentityModel.Protocols.WsFederation.WsFederationMetadataSerializer();
				Microsoft.IdentityModel.Protocols.WsFederation.WsFederationConfiguration metadata = Application["Azure.FederationMetadata"] as Microsoft.IdentityModel.Protocols.WsFederation.WsFederationConfiguration;
				if ( metadata == null )
				{
					metadata = serializer.ReadMetadata(XmlReader.Create(sFederationMetadata));
					Application["Azure.FederationMetadata"] = metadata;
				}
				
				// 02/14/2022 Paul.  Update System.IdentityModel.Tokens.Jwt to support Apple Signin. 
				// https://www.nuget.org/packages/System.IdentityModel.Tokens.Jwt/
				System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
				Microsoft.IdentityModel.Tokens.TokenValidationParameters validationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
				{
					ValidIssuer         = metadata.Issuer,
					IssuerSigningKeys   = metadata.SigningKeys,
					ValidAudience       = sAadClientId
				};

				Microsoft.IdentityModel.Tokens.SecurityToken validatedToken = null;
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
						SplendidCRM.DbProviderFactory dbf = DbProviderFactories.GetFactory();
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
									// 01/13/2017 Paul.  Cannot log an unknown user. 
									//SqlProcs.spUSERS_LOGINS_InsertOnly(ref gUSER_LOGIN_ID, Guid.Empty, sEMAIL1, "Azure AD", "Failed", Session.SessionID, Request.UserHostName, Request.Host.Host, Request.Path, Request.AppRelativeCurrentExecutionFilePath, Request.UserAgent);
									// 01/13/2017 Paul.  Cannot lock-out an unknown user. 
									//SplendidInit.LoginTracking(sEMAIL1, false);
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
				// 01/13/2017 Paul.  Cannot log an unknown user. 
				//SqlProcs.spUSERS_LOGINS_InsertOnly(ref gUSER_LOGIN_ID, Guid.Empty, sUSER_NAME, "Azure AD", "Failed", Session.SessionID, Request.UserHostName, Request.Host.Host, Request.Path, Request.AppRelativeCurrentExecutionFilePath, Request.UserAgent);
				// 01/13/2017 Paul.  Cannot lock-out an unknown user. 
				//SplendidInit.LoginTracking(sUSER_NAME, false);
				sError = "SECURITY: failed attempted login for " + sUSER_NAME + " using Azure AD/REST API. " + ex.Message;
				SplendidError.SystemMessage("Warning", new StackTrace(true).GetFrame(0), sError);
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
		public Guid FederationServicesValidateJwt(string sToken, bool bMobileClient, ref string sError)
		{
			Guid gUSER_ID       = Guid.Empty;
			Guid gUSER_LOGIN_ID = Guid.Empty;
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
				string sFederationMetadata  = sAuthority + "FederationMetadata/2007-06/FederationMetadata.xml";

				// 02/14/2022 Paul.  Use the new metadata serializer. 
				// https://www.nuget.org/packages/Microsoft.IdentityModel.Protocols.WsFederation/
				Microsoft.IdentityModel.Protocols.WsFederation.WsFederationMetadataSerializer serializer = new Microsoft.IdentityModel.Protocols.WsFederation.WsFederationMetadataSerializer();
				Microsoft.IdentityModel.Protocols.WsFederation.WsFederationConfiguration metadata = Application["ADFS.FederationMetadata"] as Microsoft.IdentityModel.Protocols.WsFederation.WsFederationConfiguration;
				if ( metadata == null )
				{
					metadata = serializer.ReadMetadata(XmlReader.Create(sFederationMetadata));
					Application["ADFS.FederationMetadata"] = metadata;
				}

				// 12/25/2018 Paul.  Not sure why server is using http instead of https.  
				// IDX10205: Issuer validation failed. Issuer: 'http://adfs4.splendidcrm.com/adfs/services/trust'. Did not match: validationParameters.ValidIssuer: 'https://adfs4.splendidcrm.com/adfs/services/trust' or validationParameters.ValidIssuers: 'null'.
				// IDX10204: Unable to validate issuer. validationParameters.ValidIssuer is null or whitespace AND validationParameters.ValidIssuers is null.
				StringList arrValidIssuers = new StringList();
				arrValidIssuers.Add(sAuthority + "adfs");
				arrValidIssuers.Add(sAuthority + "adfs/services/trust");
				arrValidIssuers.Add(sAuthority.Replace("https:", "http:") + "adfs/services/trust");
				// IDX10214: Audience validation failed. Audiences: 'urn:microsoft:userinfo'. Did not match:  validationParameters.ValidAudience: 'microsoft:identityserver:86a54b29-a28e-4bcb-9477-07e25a41ee24' or validationParameters.ValidAudiences: 'null'
				StringList arrAudiences = new StringList();
				arrAudiences.Add(sClientId);
				arrAudiences.Add("microsoft:identityserver:" + sClientId);
				// 01/08/2018 Paul.  ADFS 3.0 will require us to register both client and mobile as valid audiences. 
				if ( sClientId != sMobileClientId && !Sql.IsEmptyString(sMobileClientId) )
				{
					arrAudiences.Add(sMobileClientId);
					arrAudiences.Add("microsoft:identityserver:" + sMobileClientId);
				}
				arrAudiences.Add("urn:microsoft:userinfo");
				// 02/14/2019 Paul.  Use Grant-AdfsApplicationPermission to grant the plug-in access to the resource. 
				// https://community.dynamics.com/crm/f/117/t/246239
				// Grant-AdfsApplicationPermission -ClientRoleIdentifier "0dff791c-21b4-49cd-b3be-e7c37d29d6c0" -ServerRoleIdentifier "https://SplendidPlugin"
				// 02/14/2019 Paul.  https://SplendidPlugin is hardcoded to the Outlook and Word plug-ins. 
				arrAudiences.Add("https://SplendidPlugin");
				// 02/14/2019 Paul.  Lets include survey and mobile. 
				arrAudiences.Add("https://SplendidMobile");
				arrAudiences.Add("https://auth.expo.io/@splendidcrm/splendidsurvey");
				// 02/14/2022 Paul.  Update System.IdentityModel.Tokens.Jwt to support Apple Signin. 
				// https://www.nuget.org/packages/System.IdentityModel.Tokens.Jwt/
				System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
				Microsoft.IdentityModel.Tokens.TokenValidationParameters validationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
				{
					//ValidIssuer         = sAuthority + "adfs",
					ValidIssuers        = arrValidIssuers,
					ValidAudiences      = arrAudiences,
					IssuerSigningKeys   = metadata.SigningKeys
				};

				Microsoft.IdentityModel.Tokens.SecurityToken validatedToken = null;
				//validatedToken = tokenHandler.ReadToken(sToken);
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
						//Debug.WriteLine(claim.Type + " = " + claim.Value);
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
						SplendidCRM.DbProviderFactory dbf = DbProviderFactories.GetFactory();
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
									// 01/13/2017 Paul.  Cannot log an unknown user. 
									//SqlProcs.spUSERS_LOGINS_InsertOnly(ref gUSER_LOGIN_ID, Guid.Empty, sEMAIL1, "Azure AD", "Failed", Session.SessionID, Request.UserHostName, Request.Host.Host, Request.Path, Request.AppRelativeCurrentExecutionFilePath, Request.UserAgent);
									// 01/13/2017 Paul.  Cannot lock-out an unknown user. 
									//SplendidInit.LoginTracking(sEMAIL1, false);
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
				// 01/13/2017 Paul.  Cannot log an unknown user. 
				//SqlProcs.spUSERS_LOGINS_InsertOnly(ref gUSER_LOGIN_ID, Guid.Empty, sUSER_NAME, "Azure AD", "Failed", Session.SessionID, Request.UserHostName, Request.Host.Host, Request.Path, Request.AppRelativeCurrentExecutionFilePath, Request.UserAgent);
				// 01/13/2017 Paul.  Cannot lock-out an unknown user. 
				//SplendidInit.LoginTracking(sUSER_NAME, false);
				sError = "SECURITY: failed attempted login for " + sUSER_NAME + " using ADFS/REST API. " + ex.Message;
				SplendidError.SystemMessage("Warning", new StackTrace(true).GetFrame(0), sError);
			}
			return gUSER_ID;
		}
	}
}
