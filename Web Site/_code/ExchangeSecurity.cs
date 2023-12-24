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
using System.Data;
//using Microsoft.Exchange.WebServices.Data;
//using System.Security.Cryptography.X509Certificates;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace SplendidCRM
{
	public class ExchangeSession : Dictionary<string, object>
	{
	}

	public class ExchangeSecurity
	{
		private IMemoryCache         Cache              ;
		private DbProviderFactories  DbProviderFactories = new DbProviderFactories();
		private HttpApplicationState Application        = new HttpApplicationState();
		private HttpSessionState     Session            ;
		private Security             Security           ;
		private SplendidCache        SplendidCache      ;
		private Crm.Config           Config             = new Crm.Config();

		public ExchangeSecurity(IMemoryCache memoryCache, HttpSessionState Session, Security Security, SplendidCache SplendidCache)
		{
			this.Cache               = memoryCache        ;
			this.Session             = Session            ;
			this.Security            = Security           ;
			this.SplendidCache       = SplendidCache      ;
		}

		public DateTime DefaultCacheExpiration()
		{
#if DEBUG
			return DateTime.Now.AddSeconds(1);
#else
			return DateTime.Now.AddHours(2);
#endif
		}

		public void SetUserAccess(ExchangeSession Session, string sMODULE_NAME, string sACCESS_TYPE, int nACLACCESS)
		{
			if ( Sql.IsEmptyString(sMODULE_NAME) )
				throw(new Exception("sMODULE_NAME should not be empty."));
			Session["ACLACCESS_" + sMODULE_NAME + "_" + sACCESS_TYPE] = nACLACCESS;
		}

		// 01/17/2010 Paul.  Field Security values are stored in the Session cache. 
		public void SetUserFieldSecurity(ExchangeSession Session, string sMODULE_NAME, string sFIELD_NAME, int nACLACCESS)
		{
			if ( Sql.IsEmptyString(sMODULE_NAME) )
				throw(new Exception("sMODULE_NAME should not be empty."));
			if ( Sql.IsEmptyString(sFIELD_NAME) )
				throw(new Exception("sFIELD_NAME should not be empty."));
			// 01/17/2010 Paul.  Zero is a special value that means NOT_SET.  
			if ( nACLACCESS != 0 )
				Session["ACLFIELD_" + sMODULE_NAME + "_" + sFIELD_NAME] = nACLACCESS;
		}
		
		protected static int GetUserFieldSecurity(ExchangeSession Session, string sMODULE_NAME, string sFIELD_NAME)
		{
			if ( Sql.IsEmptyString(sMODULE_NAME) )
				throw(new Exception("sMODULE_NAME should not be empty."));
			string sAclKey = "ACLFIELD_" + sMODULE_NAME + "_" + sFIELD_NAME;
			int nACLACCESS = 0;
			// 03/29/2010 Paul.  Our ExchangeSession dictionary will throw an exception if the key does not exist, so check for existence. 
			if ( Session.ContainsKey(sAclKey) )
				nACLACCESS = Sql.ToInteger(Session[sAclKey]);
			// 01/17/2010 Paul.  Zero is a special value that means NOT_SET, so grant full access. 
			if ( nACLACCESS == 0 )
				return Security.ACL_FIELD_ACCESS.FULL_ACCESS;
			return nACLACCESS;
		}
		
		public Security.ACL_FIELD_ACCESS GetUserFieldSecurity(ExchangeSession Session, string sMODULE_NAME, string sFIELD_NAME, Guid gASSIGNED_USER_ID)
		{
			int nACLACCESS = GetUserFieldSecurity(Session, sMODULE_NAME, sFIELD_NAME);
			Security.ACL_FIELD_ACCESS acl = new Security.ACL_FIELD_ACCESS(Security, nACLACCESS, gASSIGNED_USER_ID);
			return acl;
		}
		
		// 06/09/2009 Paul.  We need to access LoadUserACL from the SOAP calls. 
		public ExchangeSession LoadUserACL(Guid gUSER_ID)
		{
			ExchangeSession Session = Cache.Get("ExchangeSession." + gUSER_ID.ToString()) as ExchangeSession;
			if ( Session == null )
			{
				Session = new ExchangeSession();
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					string sSQL;
					// 03/31/2010 Paul.  Get the culture out of the User Preferences. 
					//12/15/2012 Paul.  Move USER_PREFERENCES to separate fields for easier access on Surface RT. 
					sSQL = "select *            " + ControlChars.CrLf
					//     + "     , (select USER_PREFERENCES from vwUSERS where vwUSERS.ID = vwUSERS_Login.ID) as USER_PREFERENCES" + ControlChars.CrLf
					     + "  from vwUSERS_Login" + ControlChars.CrLf
					     + " where ID = @ID     " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@ID", gUSER_ID);
						using ( IDataReader rdr = cmd.ExecuteReader() )
						{
							if ( rdr.Read() )
							{
								Session["USER_ID"          ] = Sql.ToGuid   (rdr["ID"               ]);
								Session["TEAM_ID"          ] = Sql.ToGuid   (rdr["TEAM_ID"          ]);
								Session["USER_NAME"        ] = Sql.ToString (rdr["USER_NAME"        ]);
								Session["FULL_NAME"        ] = Sql.ToString (rdr["FULL_NAME"        ]);
								Session["IS_ADMIN"         ] = Sql.ToBoolean(rdr["IS_ADMIN"         ]);
								Session["IS_ADMIN_DELEGATE"] = Sql.ToBoolean(rdr["IS_ADMIN_DELEGATE"]);
								// 04/07/2010 Paul.  Add Exchange Email as it will be need for Push Subscriptions. 
								try
								{
									Session["EXCHANGE_ALIAS"   ] = Sql.ToString (rdr["EXCHANGE_ALIAS"   ]);
									Session["EXCHANGE_EMAIL"   ] = Sql.ToString (rdr["EXCHANGE_EMAIL"   ]);
								}
								catch
								{
								}
								// 07/09/2010 Paul.  Move the SMTP values from USER_PREFERENCES to the main table to make it easier to access. 
								try
								{
									Session["MAIL_SMTPUSER"    ] = Sql.ToString (rdr["MAIL_SMTPUSER"    ]);
									Session["MAIL_SMTPPASS"    ] = Sql.ToString (rdr["MAIL_SMTPPASS"    ]);
								}
								catch
								{
								}
								// 05/05/2016 Paul.  The User Primary Role is used with role-based views. 
								try
								{
									Session["PRIMARY_ROLE_ID"  ] = Sql.ToGuid   (rdr["PRIMARY_ROLE_ID"  ]);
									Session["PRIMARY_ROLE_NAME"] = Sql.ToString (rdr["PRIMARY_ROLE_NAME"]);
								}
								catch
								{
								}
								// 01/17/2017 Paul.  The gEXCHANGE_ID is to lookup the OAuth credentials. 
								try
								{
									Session["OFFICE365_OAUTH_ENABLED"] = Sql.ToBoolean(rdr["OFFICE365_OAUTH_ENABLED"]);
								}
								catch
								{
								}
								
								string sCulture = "en-US";
								// 12/15/2012 Paul.  Move USER_PREFERENCES to separate fields for easier access on Surface RT. 
								//string sUSER_PREFERENCES = Sql.ToString(rdr["USER_PREFERENCES"]);
								//if ( !Sql.IsEmptyString(sUSER_PREFERENCES) )
								//{
								//	XmlDocument xml = new XmlDocument();
								// 01/20/2015 Paul.  Disable XmlResolver to prevent XML XXE. 
								// https://www.owasp.org/index.php/XML_External_Entity_(XXE)_Processing
								// http://stackoverflow.com/questions/14230988/how-to-prevent-xxe-attack-xmldocument-in-net
								//	xml.XmlResolver = null;
								//	try
								//	{
								//		xml.LoadXml(sUSER_PREFERENCES);
								//		sCulture = L10N.NormalizeCulture(XmlUtil.SelectSingleNode(xml, "culture"));
								//	}
								//	catch
								//	{
								//	}
								//}
								try
								{
									sCulture = Sql.ToString(rdr["LANG"]);
									if ( Sql.IsEmptyString(sCulture) )
										sCulture = "en-US";
									// 04/20/2018 Paul.  Alternate language mapping to convert en-CA to en_US. 
									sCulture = L10N.AlternateLanguage(Application, sCulture);
								}
								catch
								{
								}
								Session["USER_SETTINGS/CULTURE"] = sCulture;
							}
							else
							{
								// 07/06/2017 Paul.  If we don't throw an exception here, we will get a cryptic error later
								// The given key was not present in the dictionary.at System.Collections.Generic.Dictionary`2.get_Item(TKey key)
								throw(new Exception(gUSER_ID.ToString() + " is not a valid or active user for service requests."));
							}
						}
					}
					
					// 03/09/2010 Paul.  Admin roles are managed separately. 
					// 09/26/2017 Paul.  Add Archive access right. 
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
						Sql.AddParameter(cmd, "@USER_ID", gUSER_ID);
						using ( IDataReader rdr = cmd.ExecuteReader() )
						{
							while ( rdr.Read() )
							{
								string sMODULE_NAME = Sql.ToString(rdr["MODULE_NAME"]);
								this.SetUserAccess(Session, sMODULE_NAME, "admin" , Sql.ToInteger(rdr["ACLACCESS_ADMIN" ]));
								this.SetUserAccess(Session, sMODULE_NAME, "access", Sql.ToInteger(rdr["ACLACCESS_ACCESS"]));
								this.SetUserAccess(Session, sMODULE_NAME, "view"  , Sql.ToInteger(rdr["ACLACCESS_VIEW"  ]));
								this.SetUserAccess(Session, sMODULE_NAME, "list"  , Sql.ToInteger(rdr["ACLACCESS_LIST"  ]));
								this.SetUserAccess(Session, sMODULE_NAME, "edit"  , Sql.ToInteger(rdr["ACLACCESS_EDIT"  ]));
								this.SetUserAccess(Session, sMODULE_NAME, "delete", Sql.ToInteger(rdr["ACLACCESS_DELETE"]));
								this.SetUserAccess(Session, sMODULE_NAME, "import", Sql.ToInteger(rdr["ACLACCESS_IMPORT"]));
								this.SetUserAccess(Session, sMODULE_NAME, "export", Sql.ToInteger(rdr["ACLACCESS_EXPORT"]));
								// 09/26/2017 Paul.  Add Archive access right. 
								this.SetUserAccess(Session, sMODULE_NAME, "archive", Sql.ToInteger(rdr["ACLACCESS_ARCHIVE"]));
							}
						}
					}
					if ( SplendidInit.bEnableACLFieldSecurity )
					{
						sSQL = "select MODULE_NAME                   " + ControlChars.CrLf
						     + "     , FIELD_NAME                    " + ControlChars.CrLf
						     + "     , ACLACCESS                     " + ControlChars.CrLf
						     + "  from vwACL_FIELD_ACCESS_ByUserAlias" + ControlChars.CrLf
						     + " where USER_ID = @USER_ID            " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@USER_ID", gUSER_ID);
							using ( IDataReader rdr = cmd.ExecuteReader() )
							{
								while ( rdr.Read() )
								{
									string sMODULE_NAME = Sql.ToString (rdr["MODULE_NAME"]);
									string sFIELD_NAME  = Sql.ToString (rdr["FIELD_NAME" ]);
									int    nACLACCESS   = Sql.ToInteger(rdr["ACLACCESS"  ]);
									this.SetUserFieldSecurity(Session, sMODULE_NAME, sFIELD_NAME, nACLACCESS);
								}
							}
						}
					}
				}
				Cache.Set("ExchangeSession." + gUSER_ID.ToString(), Session, DefaultCacheExpiration());
			}
			return Session;
		}

		// 03/25/2010 Paul.  We need a separate GetUserAccess function as the Session will not contain the access rights of the user being sync'd. 
		public int GetUserAccess(ExchangeSession Session, string sMODULE_NAME, string sACCESS_TYPE)
		{
			// 06/04/2006 Paul.  Verify that sMODULE_NAME is not empty.  
			if ( Sql.IsEmptyString(sMODULE_NAME) )
				throw(new Exception("sMODULE_NAME should not be empty."));
			// 03/25/2010 Paul.  Don't apply admin rules when syncing with Exchange. 

			// 12/05/2006 Paul.  We need to combine Activity and Calendar related modules into a single access value. 
			int nACLACCESS = 0;
			if ( sMODULE_NAME == "Calendar" )
			{
				// 12/05/2006 Paul.  The Calendar related views only combine Calls and Meetings. 
				int nACLACCESS_Calls    = this.GetUserAccess(Session, "Calls"   , sACCESS_TYPE);
				int nACLACCESS_Meetings = this.GetUserAccess(Session, "Meetings", sACCESS_TYPE);
				// 12/05/2006 Paul. Use the max value so that the Activities will be displayed if either are accessible. 
				nACLACCESS = Math.Max(nACLACCESS_Calls, nACLACCESS_Meetings);
			}
			else if ( sMODULE_NAME == "Activities" )
			{
				// 12/05/2006 Paul.  The Activities combines Calls, Meetings, Tasks, Notes and Emails. 
				int nACLACCESS_Calls    = this.GetUserAccess(Session, "Calls"   , sACCESS_TYPE);
				int nACLACCESS_Meetings = this.GetUserAccess(Session, "Meetings", sACCESS_TYPE);
				int nACLACCESS_Tasks    = this.GetUserAccess(Session, "Tasks"   , sACCESS_TYPE);
				int nACLACCESS_Notes    = this.GetUserAccess(Session, "Notes"   , sACCESS_TYPE);
				int nACLACCESS_Emails   = this.GetUserAccess(Session, "Emails"  , sACCESS_TYPE);
				nACLACCESS = nACLACCESS_Calls;
				nACLACCESS = Math.Max(nACLACCESS, nACLACCESS_Meetings);
				nACLACCESS = Math.Max(nACLACCESS, nACLACCESS_Tasks   );
				nACLACCESS = Math.Max(nACLACCESS, nACLACCESS_Notes   );
				nACLACCESS = Math.Max(nACLACCESS, nACLACCESS_Emails  );
			}
			else
			{
				string sAclKey = "ACLACCESS_" + sMODULE_NAME + "_" + sACCESS_TYPE;
				// 04/27/2006 Paul.  If no specific level is provided, then look to the Module level. 
				if ( !Session.ContainsKey(sAclKey) )  // Session[sAclKey] == null
					nACLACCESS = Sql.ToInteger(Application[sAclKey]);
				else
					nACLACCESS = Sql.ToInteger(Session[sAclKey]);
				if ( sACCESS_TYPE != "access" && nACLACCESS >= 0 )
				{
					// 04/27/2006 Paul.  The access type can over-ride any other type. 
					// A simple trick is to take the minimum of the two values.  
					// If either value is denied, then the result will be negative. 
					sAclKey = "ACLACCESS_" + sMODULE_NAME + "_access";
					int nAccessLevel = 0;
					if ( !Session.ContainsKey(sAclKey) )  // Session[sAclKey] == null
						nAccessLevel = Sql.ToInteger(Application[sAclKey]);
					else
						nAccessLevel = Sql.ToInteger(Session[sAclKey]);
					if ( nAccessLevel < 0 )
						nACLACCESS = nAccessLevel;
				}
			}
			return nACLACCESS;
		}

		public void Filter(ExchangeSession Session, IDbCommand cmd, Guid gASSIGNED_USER_ID,string sMODULE_NAME, string sACCESS_TYPE)
		{
			this.Filter(Session, cmd, gASSIGNED_USER_ID, sMODULE_NAME, sACCESS_TYPE, "ASSIGNED_USER_ID", false);
		}
		
		// 03/25/2010 Paul.  We need a separate SecurityFilter function as the Session will not contain the access rights of the user being sync'd. 
		public void Filter(ExchangeSession Session, IDbCommand cmd, Guid gASSIGNED_USER_ID, string sMODULE_NAME, string sACCESS_TYPE, string sASSIGNED_USER_ID_Field, bool bActivitiesFilter)
		{
			bool bModuleIsTeamed        = Sql.ToBoolean(Application["Modules." + sMODULE_NAME + ".Teamed"  ]);
			bool bModuleIsAssigned      = Sql.ToBoolean(Application["Modules." + sMODULE_NAME + ".Assigned"]);
			bool bEnableTeamManagement  = Sql.ToBoolean(Application["CONFIG.enable_team_management"        ]);
			bool bRequireTeamManagement = Sql.ToBoolean(Application["CONFIG.require_team_management"       ]);
			bool bRequireUserAssignment = Sql.ToBoolean(Application["CONFIG.require_user_assignment"       ]);
			bool bEnableDynamicTeams    = Sql.ToBoolean(Application["CONFIG.enable_dynamic_teams"          ]);
			// 04/26/2018 Paul.  Exchange Sync needs to follow team hierarchy rules. 
			bool bEnableTeamHierarchy   = Sql.ToBoolean(Application["CONFIG.enable_team_hierarchy"         ]);
			// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
			bool bEnableDynamicAssignment = Sql.ToBoolean(Application["CONFIG.enable_dynamic_assignment"   ]);
			// 03/25/2010 Paul.  Don't apply admin rules when syncing with Exchange. 
			bool bIsAdmin = false;
			if ( bModuleIsTeamed )
			{
				if ( bIsAdmin )
					bRequireTeamManagement = false;

				if ( bEnableTeamManagement )
				{
					// 11/12/2009 Paul.  Use the NextPlaceholder function so that we can call the security filter multiple times. 
					// We need this to support offline sync. 
					string sFieldPlaceholder = Sql.NextPlaceholder(cmd, "MEMBERSHIP_USER_ID");
					if ( bEnableDynamicTeams )
					{
						// 08/31/2009 Paul.  Dynamic Teams are handled just like regular teams except using a different view. 
						if ( bRequireTeamManagement )
							cmd.CommandText += "       inner ";
						else
							cmd.CommandText += "  left outer ";
						// 04/26/2018 Paul.  Exchange Sync needs to follow team hierarchy rules. 
						if ( !bEnableTeamHierarchy )
						{
							// 11/27/2009 Paul.  Use Sql.MetadataName() so that the view name can exceed 30 characters, but still be truncated for Oracle. 
							// 11/27/2009 Paul.  vwTEAM_SET_MEMBERSHIPS_Security has a distinct clause to reduce duplicate rows. 
							cmd.CommandText += "join " + Sql.MetadataName(cmd, "vwTEAM_SET_MEMBERSHIPS_Security") + " vwTEAM_SET_MEMBERSHIPS" + ControlChars.CrLf;
							cmd.CommandText += "               on vwTEAM_SET_MEMBERSHIPS.MEMBERSHIP_TEAM_SET_ID = TEAM_SET_ID" + ControlChars.CrLf;
							cmd.CommandText += "              and vwTEAM_SET_MEMBERSHIPS.MEMBERSHIP_USER_ID     = @" + sFieldPlaceholder + ControlChars.CrLf;
						}
						else
						{
							if ( Sql.IsOracle(cmd) )
							{
								cmd.CommandText += "join table(" + Sql.MetadataName(cmd, "fnTEAM_SET_HIERARCHY_MEMBERSHIPS") + "(@" + sFieldPlaceholder + ")) vwTEAM_SET_MEMBERSHIPS" + ControlChars.CrLf;
								cmd.CommandText += "               on vwTEAM_SET_MEMBERSHIPS.MEMBERSHIP_TEAM_SET_ID = TEAM_SET_ID" + ControlChars.CrLf;
							}
							else
							{
								string fnPrefix = (Sql.IsSQLServer(cmd) ? "dbo." : String.Empty);
								cmd.CommandText += "join " + fnPrefix + Sql.MetadataName(cmd, "fnTEAM_SET_HIERARCHY_MEMBERSHIPS") + "(@" + sFieldPlaceholder + ") vwTEAM_SET_MEMBERSHIPS" + ControlChars.CrLf;
								cmd.CommandText += "               on vwTEAM_SET_MEMBERSHIPS.MEMBERSHIP_TEAM_SET_ID = TEAM_SET_ID" + ControlChars.CrLf;
							}
						}
					}
					else
					{
						if ( bRequireTeamManagement )
							cmd.CommandText += "       inner ";
						else
							cmd.CommandText += "  left outer ";
						// 04/26/2018 Paul.  Exchange Sync needs to follow team hierarchy rules. 
						if ( !bEnableTeamHierarchy )
						{
							cmd.CommandText += "join vwTEAM_MEMBERSHIPS" + ControlChars.CrLf;
							cmd.CommandText += "               on vwTEAM_MEMBERSHIPS.MEMBERSHIP_TEAM_ID = TEAM_ID" + ControlChars.CrLf;
							cmd.CommandText += "              and vwTEAM_MEMBERSHIPS.MEMBERSHIP_USER_ID = @" + sFieldPlaceholder + ControlChars.CrLf;
						}
						else
						{
							if ( Sql.IsOracle(cmd) )
							{
								cmd.CommandText += "join table(fnTEAM_HIERARCHY_MEMBERSHIPS(@" + sFieldPlaceholder + ")) vwTEAM_MEMBERSHIPS" + ControlChars.CrLf;
								cmd.CommandText += "               on vwTEAM_MEMBERSHIPS.MEMBERSHIP_TEAM_ID = TEAM_ID" + ControlChars.CrLf;
							}
							else
							{
								string fnPrefix = (Sql.IsSQLServer(cmd) ? "dbo." : String.Empty);
								cmd.CommandText += "join " + fnPrefix + "fnTEAM_HIERARCHY_MEMBERSHIPS(@" + sFieldPlaceholder + ") vwTEAM_MEMBERSHIPS" + ControlChars.CrLf;
								cmd.CommandText += "               on vwTEAM_MEMBERSHIPS.MEMBERSHIP_TEAM_ID = TEAM_ID" + ControlChars.CrLf;
							}
						}
					}
					Sql.AddParameter(cmd, "@" + sFieldPlaceholder, gASSIGNED_USER_ID);
				}
			}
			int nACLACCESS = 0;
			// 08/30/2009 Paul.  Since the activities view does not allow us to filter on each module type, apply the Calls ACL rules to all activities. 
			// 12/0232017 Paul.  Activities views will use new function that accepts an array of modules. 
			//if ( bActivitiesFilter )
			//	nACLACCESS = ExchangeSecurity.GetUserAccess(Application, Session, "Calls", sACCESS_TYPE);
			//else
				nACLACCESS = this.GetUserAccess(Session, sMODULE_NAME, sACCESS_TYPE);
			
			// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
			string sASSIGNED_SET_ID_Field = sASSIGNED_USER_ID_Field.Replace("ASSIGNED_USER_ID", "ASSIGNED_SET_ID");
			if ( bModuleIsAssigned && bEnableDynamicAssignment )
			{
				// 01/01/2008 Paul.  We need a quick way to require user assignments across the system. 
				// 01/02/2008 Paul.  Make sure owner rule does not apply to admins. 
				if ( nACLACCESS == ACL_ACCESS.OWNER || (bRequireUserAssignment && !bIsAdmin) )
				{
					string sFieldPlaceholder = Sql.NextPlaceholder(cmd, sASSIGNED_SET_ID_Field);
					if ( bRequireUserAssignment )
						cmd.CommandText += "       inner ";
					else
						cmd.CommandText += "  left outer ";
					cmd.CommandText += "join vwASSIGNED_SET_MEMBERSHIPS" + ControlChars.CrLf;
					cmd.CommandText += "               on vwASSIGNED_SET_MEMBERSHIPS.MEMBERSHIP_ASSIGNED_SET_ID  = " + sASSIGNED_SET_ID_Field + ControlChars.CrLf;
					cmd.CommandText += "              and vwASSIGNED_SET_MEMBERSHIPS.MEMBERSHIP_ASSIGNED_USER_ID = @" + sFieldPlaceholder + ControlChars.CrLf;
					Sql.AddParameter(cmd, "@" + sFieldPlaceholder, Security.USER_ID);
				}
			}
			
			cmd.CommandText += " where 1 = 1" + ControlChars.CrLf;
			if ( bModuleIsTeamed )
			{
				if ( bEnableTeamManagement && !bRequireTeamManagement && !bIsAdmin )
				{
					// 08/31/2009 Paul.  Dynamic Teams are handled just like regular teams except using a different view. 
					// 09/01/2009 Paul.  Don't use MEMBERSHIP_ID as it is not included in the index. 
					if ( bEnableDynamicTeams )
						cmd.CommandText += "   and (TEAM_SET_ID is null or vwTEAM_SET_MEMBERSHIPS.MEMBERSHIP_TEAM_SET_ID is not null)" + ControlChars.CrLf;
					else
						cmd.CommandText += "   and (TEAM_ID is null or vwTEAM_MEMBERSHIPS.MEMBERSHIP_TEAM_ID is not null)" + ControlChars.CrLf;
				}
			}
			if ( bModuleIsAssigned )
			{
				// 01/01/2008 Paul.  We need a quick way to require user assignments across the system. 
				// 01/02/2008 Paul.  Make sure owner rule does not apply to admins. 
				if ( nACLACCESS == ACL_ACCESS.OWNER || (bRequireUserAssignment && !bIsAdmin) )
				{
					// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
					if ( bEnableDynamicAssignment )
					{
						if ( (!bRequireUserAssignment && !bIsAdmin) )
						{
							cmd.CommandText += "   and (" + sASSIGNED_SET_ID_Field + " is null or vwASSIGNED_SET_MEMBERSHIPS.MEMBERSHIP_ASSIGNED_SET_ID is not null)" + ControlChars.CrLf;
						}
					}
					else
					{
						string sFieldPlaceholder = Sql.NextPlaceholder(cmd, sASSIGNED_USER_ID_Field);
						// 01/22/2007 Paul.  If ASSIGNED_USER_ID is null, then let everybody see it. 
						// This was added to work around a bug whereby the ASSIGNED_USER_ID was not automatically assigned to the creating user. 
						// 01/27/2011 Paul.  Need to be able to call show_unassigned from the ExchangeSync service. 
						bool bShowUnassigned = Config.show_unassigned();
						if ( bShowUnassigned )
						{
							if ( Sql.IsOracle(cmd) || Sql.IsDB2(cmd) )
								cmd.CommandText += "   and (" + sASSIGNED_USER_ID_Field + " is null or upper(" + sASSIGNED_USER_ID_Field + ") = upper(@" + sFieldPlaceholder + "))" + ControlChars.CrLf;
							else
								cmd.CommandText += "   and (" + sASSIGNED_USER_ID_Field + " is null or "       + sASSIGNED_USER_ID_Field +  " = @"       + sFieldPlaceholder + ")"  + ControlChars.CrLf;
						}
						/*
						// 02/13/2009 Paul.  We have a problem with the NOTES table as used in Activities lists. 
						// Notes are not assigned specifically to anyone so the ACTIVITY_ASSIGNED_USER_ID may return NULL. 
						// Notes should assume the ownership of the parent record, but we are also going to allow NULL for previous SplendidCRM installations. 
						// 02/13/2009 Paul.  This issue affects Notes, Quotes, Orders, Invoices and Orders, so just rely upon fixing the views. 
						else if ( sASSIGNED_USER_ID_Field == "ACTIVITY_ASSIGNED_USER_ID" )
						{
							if ( Sql.IsOracle(cmd) || Sql.IsDB2(cmd) )
								cmd.CommandText += "   and ((ACTIVITY_ASSIGNED_USER_ID is null and ACTIVITY_TYPE = N'Notes') or (upper(" + sASSIGNED_USER_ID_Field + ") = upper(@" + sFieldPlaceholder + ")))" + ControlChars.CrLf;
							else
								cmd.CommandText += "   and ((ACTIVITY_ASSIGNED_USER_ID is null and ACTIVITY_TYPE = N'Notes') or ("       + sASSIGNED_USER_ID_Field +  " = @"       + sFieldPlaceholder  + "))" + ControlChars.CrLf;
						}
						*/
						else
						{
							if ( Sql.IsOracle(cmd) || Sql.IsDB2(cmd) )
								cmd.CommandText += "   and upper(" + sASSIGNED_USER_ID_Field + ") = upper(@" + sFieldPlaceholder + ")" + ControlChars.CrLf;
							else
								cmd.CommandText += "   and "       + sASSIGNED_USER_ID_Field +  " = @"       + sFieldPlaceholder       + ControlChars.CrLf;
						}
						Sql.AddParameter(cmd, "@" + sFieldPlaceholder, gASSIGNED_USER_ID);
					}
				}
			}
		}
	}
}
