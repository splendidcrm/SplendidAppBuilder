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

using SplendidCRM;

namespace SplendidWebApi.Controllers
{
	[Authorize]
	[ApiController]
	//[Route("[controller]")]
	[Route("Processes/Rest.svc")]
	//[Route("Rest.svc")]
	public class ProcessesRestController : ControllerBase
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
		private SplendidCRM.TimeZone TimeZone           = new SplendidCRM.TimeZone();
		private Utils                Utils              ;
		private SqlProcs             SqlProcs           ;
		private SplendidError        SplendidError      ;
		private SplendidCache        SplendidCache      ;
		private RestUtil             RestUtil           ;
		private SplendidDynamic      SplendidDynamic    ;
		private SplendidInit         SplendidInit       ;
		private SplendidCRM.Crm.Config           Config           = new SplendidCRM.Crm.Config();
		private WF4ApprovalActivityService WF4ApprovalActivity;

		public ProcessesRestController(IWebHostEnvironment hostingEnvironment, IMemoryCache memoryCache, HttpSessionState Session, Security Security, Utils Utils, SplendidError SplendidError, SplendidCache SplendidCache, RestUtil RestUtil, SplendidDynamic SplendidDynamic, SplendidInit SplendidInit, WF4ApprovalActivityService WF4ApprovalActivity)
		{
			this.hostingEnvironment  = hostingEnvironment ;
			this.memoryCache         = memoryCache        ;
			this.Session             = Session            ;
			this.Security            = Security           ;
			this.L10n                = new L10N(Sql.ToString(Session["USER_LANG"]));
			this.Sql                 = new Sql(Session, Security);
			this.SqlProcs            = new SqlProcs(Security, Sql);
			this.Utils               = Utils              ;
			this.SplendidError       = SplendidError      ;
			this.SplendidCache       = SplendidCache      ;
			this.RestUtil            = RestUtil           ;
			this.SplendidDynamic     = SplendidDynamic    ;
			this.SplendidInit        = SplendidInit       ;
			this.WF4ApprovalActivity = WF4ApprovalActivity;
		}

		[HttpGet("[action]")]
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public Dictionary<string, object> GetProcessStatus(Guid ID)
		{
			L10N L10n = new L10N(Sql.ToString(Session["USER_SETTINGS/CULTURE"]));
			if ( !Security.IsAuthenticated() )
			{
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}
			// 08/20/2014 Paul.  We need to continually update the SplendidSession so that it expires along with the ASP.NET Session. 
			SplendidSession.CreateSession(Session);
			
			DataTable dt = new DataTable();
			dt.Columns.Add("PENDING_PROCESS_ID", typeof(System.Guid   ));
			dt.Columns.Add("ProcessStatus"     , typeof(System.String ));
			dt.Columns.Add("ShowApprove"       , typeof(System.Boolean));
			dt.Columns.Add("ShowReject"        , typeof(System.Boolean));
			dt.Columns.Add("ShowRoute"         , typeof(System.Boolean));
			dt.Columns.Add("ShowClaim"         , typeof(System.Boolean));
			dt.Columns.Add("USER_TASK_TYPE"    , typeof(System.String ));
			dt.Columns.Add("PROCESS_USER_ID"   , typeof(System.Guid   ));
			dt.Columns.Add("ASSIGNED_TEAM_ID"  , typeof(System.Guid   ));
			dt.Columns.Add("PROCESS_TEAM_ID"   , typeof(System.Guid   ));
			
			Guid gPENDING_PROCESS_ID = ID;
			string sProcessStatus    = String.Empty;
			bool   bShowApprove      = false;
			bool   bShowReject       = false;
			bool   bShowRoute        = false;
			bool   bShowClaim        = false;
			string sUSER_TASK_TYPE   = String.Empty;
			Guid   gPROCESS_USER_ID  = Guid.Empty;
			Guid   gASSIGNED_TEAM_ID = Guid.Empty;
			Guid   gPROCESS_TEAM_ID  = Guid.Empty;
			bool bFound = WF4ApprovalActivity.GetProcessStatus(Application, L10n, gPENDING_PROCESS_ID, ref sProcessStatus, ref bShowApprove, ref bShowReject, ref bShowRoute, ref bShowClaim, ref sUSER_TASK_TYPE, ref gPROCESS_USER_ID, ref gASSIGNED_TEAM_ID, ref gPROCESS_TEAM_ID);
			{
				DataRow row = dt.NewRow();
				dt.Rows.Add(row);
				row["PENDING_PROCESS_ID"] = gPENDING_PROCESS_ID;
				row["ProcessStatus"     ] = sProcessStatus     ;
				row["ShowApprove"       ] = bShowApprove       ;
				row["ShowReject"        ] = bShowReject        ;
				row["ShowRoute"         ] = bShowRoute         ;
				row["ShowClaim"         ] = bShowClaim         ;
				row["USER_TASK_TYPE"    ] = sUSER_TASK_TYPE    ;
				row["PROCESS_USER_ID"   ] = gPROCESS_USER_ID   ;
				row["ASSIGNED_TEAM_ID"  ] = gASSIGNED_TEAM_ID  ;
				row["PROCESS_TEAM_ID"   ] = gPROCESS_TEAM_ID   ;
			}
			
			string sBaseURI = String.Empty;
			Guid     gTIMEZONE         = Sql.ToGuid  (Session["USER_SETTINGS/TIMEZONE"]);
			SplendidCRM.TimeZone T10n  = TimeZone.CreateTimeZone(gTIMEZONE);
			// 04/01/2020 Paul.  Move json utils to RestUtil. 
			Dictionary<string, object> dictResponse = RestUtil.ToJson(sBaseURI, "Processes", dt, T10n);
			return dictResponse;
		}

		[HttpGet("[action]")]
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public Dictionary<string, object> GetProcessHistory(Guid ID)
		{
			L10N L10n = new L10N(Sql.ToString(Session["USER_SETTINGS/CULTURE"]));
			if ( !Security.IsAuthenticated() )
			{
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}
			// 08/20/2014 Paul.  We need to continually update the SplendidSession so that it expires along with the ASP.NET Session. 
			SplendidSession.CreateSession(Session);
			
			StringBuilder sbDumpSQL = new StringBuilder();
			string sPROCESS_NUMBER = String.Empty;
			string sPARENT_NAME    = String.Empty;
			DataTable dt = SplendidCRM.Users.ProcessHistoryView.GetTable(Application, L10n, ID, ref sPROCESS_NUMBER, ref sPARENT_NAME, sbDumpSQL);
			string sBaseURI = String.Empty;
			
			Guid     gTIMEZONE         = Sql.ToGuid  (Session["USER_SETTINGS/TIMEZONE"]);
			SplendidCRM.TimeZone T10n  = TimeZone.CreateTimeZone(gTIMEZONE);
			// 04/01/2020 Paul.  Move json utils to RestUtil. 
			Dictionary<string, object> dictResponse = RestUtil.ToJson(sBaseURI, "ProcessHistory", dt, T10n);
			dictResponse.Add("__total", dt.Rows.Count);
			dictResponse.Add("__title", String.Format(L10n.Term("Processes.LBL_MY_PROCESSES_NAME_FORMAT"), sPROCESS_NUMBER, sPARENT_NAME));
			if ( Sql.ToBoolean(Application["CONFIG.show_sql"]) )
			{
				dictResponse.Add("__sql", sbDumpSQL.ToString());
			}
			return dictResponse;
		}

		[HttpGet("[action]")]
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public Dictionary<string, object> GetProcessNotes(Guid ID)
		{
			L10N L10n = new L10N(Sql.ToString(Session["USER_SETTINGS/CULTURE"]));
			if ( !Security.IsAuthenticated() )
			{
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}
			// 08/20/2014 Paul.  We need to continually update the SplendidSession so that it expires along with the ASP.NET Session. 
			SplendidSession.CreateSession(Session);
			
			StringBuilder sbDumpSQL = new StringBuilder();
			string sPROCESS_NUMBER = String.Empty;
			string sPARENT_NAME    = String.Empty;
			DataTable dt = SplendidCRM.Users.ProcessNotesView.GetTable(Application, L10n, ID, ref sPROCESS_NUMBER, ref sPARENT_NAME, sbDumpSQL);
			string sBaseURI = String.Empty;
			
			Guid     gTIMEZONE         = Sql.ToGuid  (Session["USER_SETTINGS/TIMEZONE"]);
			SplendidCRM.TimeZone T10n  = TimeZone.CreateTimeZone(gTIMEZONE);
			// 04/01/2020 Paul.  Move json utils to RestUtil. 
			Dictionary<string, object> dictResponse = RestUtil.ToJson(sBaseURI, "ProcessHistory", dt, T10n);
			dictResponse.Add("__total", dt.Rows.Count);
			dictResponse.Add("__title", String.Format(L10n.Term("Processes.LBL_MY_PROCESSES_NAME_FORMAT"), sPROCESS_NUMBER, sPARENT_NAME));
			if ( Sql.ToBoolean(Application["CONFIG.show_sql"]) )
			{
				dictResponse.Add("__sql", sbDumpSQL.ToString());
			}
			return dictResponse;
		}

		[HttpPost("[action]")]
		public void DeleteProcessNote()
		{
			L10N L10n = new L10N(Sql.ToString(Session["USER_SETTINGS/CULTURE"]));
			if ( !Security.IsAuthenticated() )
			{
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}
			// 08/20/2014 Paul.  We need to continually update the SplendidSession so that it expires along with the ASP.NET Session. 
			SplendidSession.CreateSession(Session);
			
			Guid gID = Sql.ToGuid(Request.Query["ID"]);
			if ( Sql.IsEmptyGuid(gID) )
			{
				throw(new Exception("ID is empty"));
			}
			SqlProcs.spPROCESSES_NOTES_Delete(gID);
		}

		[HttpPost("[action]")]
		public void AddProcessNote([FromBody] string sNOTES)
		{
			L10N L10n = new L10N(Sql.ToString(Session["USER_SETTINGS/CULTURE"]));
			if ( !Security.IsAuthenticated() )
			{
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}
			// 08/20/2014 Paul.  We need to continually update the SplendidSession so that it expires along with the ASP.NET Session. 
			SplendidSession.CreateSession(Session);
			
			Guid gID = Sql.ToGuid(Request.Query["ID"]);
			if ( Sql.IsEmptyGuid(gID) )
			{
				throw(new Exception("ID is empty"));
			}
			SqlProcs.spPROCESSES_NOTES_InsertOnly(gID, sNOTES);
		}

		[HttpPost("[action]")]
		// 03/13/2011 Paul.  Must use octet-stream instead of json, outherwise we get the following error. 
		// Incoming message for operation 'CreateRecord' (contract 'AddressService' with namespace 'http://tempuri.org/') contains an unrecognized http body format value 'Json'. 
		// The expected body format value is 'Raw'. This can be because a WebContentTypeMapper has not been configured on the binding. See the documentation of WebContentTypeMapper for more details.
		//xhr.setRequestHeader('content-type', 'application/octet-stream');
		public void ProcessAction([FromBody] IDictionary<string, object> dict)
		{
			L10N L10n = new L10N(Sql.ToString(Session["USER_SETTINGS/CULTURE"]));
			if ( !Security.IsAuthenticated() )
			{
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}
			// 08/20/2014 Paul.  We need to continually update the SplendidSession so that it expires along with the ASP.NET Session. 
			SplendidSession.CreateSession(Session);
			
			string sACTION             = (dict.ContainsKey("ACTION"            ) ? Sql.ToString(dict["ACTION"            ]) : String.Empty);
			Guid   gPENDING_PROCESS_ID = (dict.ContainsKey("PENDING_PROCESS_ID") ? Sql.ToGuid  (dict["PENDING_PROCESS_ID"]) : Guid.Empty  );
			Guid   gPROCESS_USER_ID    = (dict.ContainsKey("PROCESS_USER_ID"   ) ? Sql.ToGuid  (dict["PROCESS_USER_ID"   ]) : Guid.Empty  );
			string sPROCESS_NOTES      = (dict.ContainsKey("PROCESS_NOTES"     ) ? Sql.ToString(dict["PROCESS_NOTES"     ]) : String.Empty);
			if ( Sql.IsEmptyGuid(gPENDING_PROCESS_ID) )
			{
				throw(new Exception("PENDING_PROCESS_ID is empty"));
			}
			
			string sProcessStatus    = String.Empty;
			bool   bShowApprove      = false;
			bool   bShowReject       = false;
			bool   bShowRoute        = false;
			bool   bShowClaim        = false;
			string sUSER_TASK_TYPE   = String.Empty;
			Guid   gEXISTING_USER_ID = Guid.Empty;
			Guid   gASSIGNED_TEAM_ID = Guid.Empty;
			Guid   gPROCESS_TEAM_ID  = Guid.Empty;
			bool bFound = WF4ApprovalActivity.GetProcessStatus(Application, L10n, gPENDING_PROCESS_ID, ref sProcessStatus, ref bShowApprove, ref bShowReject, ref bShowRoute, ref bShowClaim, ref sUSER_TASK_TYPE, ref gEXISTING_USER_ID, ref gASSIGNED_TEAM_ID, ref gPROCESS_TEAM_ID);
			if ( bFound )
			{
				switch ( sACTION )
				{
					case "Approve":
						if ( bShowApprove )
							WF4ApprovalActivity.Approve(Application, L10n, gPENDING_PROCESS_ID, Security.USER_ID);
						else
							throw(new Exception(sACTION + " is an unsupported action."));
						break;
					case "Reject":
						if ( bShowReject )
							WF4ApprovalActivity.Reject(Application, gPENDING_PROCESS_ID, Security.USER_ID);
						else
							throw(new Exception(sACTION + " is an unsupported action."));
						break;
					case "Route":
						if ( bShowRoute )
							WF4ApprovalActivity.Route(Application, L10n, gPENDING_PROCESS_ID, Security.USER_ID);
						else
							throw(new Exception(sACTION + " is an unsupported action."));
						break;
					case "Claim":
						if ( bShowClaim )
							WF4ApprovalActivity.Claim(Application, gPENDING_PROCESS_ID, Security.USER_ID);
						else
							throw(new Exception(sACTION + " is an unsupported action."));
						break;
					case "Cancel":
						WF4ApprovalActivity.Cancel(Application, gPENDING_PROCESS_ID, Security.USER_ID);
						break;
					case "ChangeProcessUser":
						if ( !Sql.IsEmptyGuid(gPROCESS_TEAM_ID) && !Sql.IsEmptyGuid(gPROCESS_USER_ID) )
							WF4ApprovalActivity.ChangeProcessUser(Application, gPENDING_PROCESS_ID, gPROCESS_USER_ID, sPROCESS_NOTES);
						else if ( Sql.IsEmptyGuid(gPROCESS_USER_ID) )
							throw(new Exception("PROCESS_USER_ID was not specified."));
						else if ( Sql.IsEmptyGuid(gPROCESS_TEAM_ID) )
							throw(new Exception(sACTION + " is an unsupported action."));
						break;
					case "ChangeAssignedUser":
						if ( !Sql.IsEmptyGuid(gASSIGNED_TEAM_ID) && !Sql.IsEmptyGuid(gPROCESS_USER_ID) )
							WF4ApprovalActivity.ChangeAssignedUser(Application, gPENDING_PROCESS_ID, gPROCESS_USER_ID, sPROCESS_NOTES);
						else if ( Sql.IsEmptyGuid(gPROCESS_USER_ID) )
							throw(new Exception("PROCESS_USER_ID was not specified."));
						else if ( Sql.IsEmptyGuid(gPROCESS_TEAM_ID) )
							throw(new Exception(sACTION + " is an unsupported action."));
						break;
					default:
						throw(new Exception(sACTION + " is an unsupported action."));
				}
			}
			else
			{
				throw(new Exception(gPENDING_PROCESS_ID.ToString() + " is not available."));
			}
		}

		[HttpGet("[action]")]
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public Dictionary<string, object> ProcessUsers()
		{
			Guid   gTEAM_ID  = Sql.ToGuid   (Request.Query["TEAM_ID" ]);
			int    nSKIP     = Sql.ToInteger(Request.Query["$skip"   ]);
			int    nTOP      = Sql.ToInteger(Request.Query["$top"    ]);
			string sFILTER   = Sql.ToString (Request.Query["$filter" ]);
			string sORDER_BY = Sql.ToString (Request.Query["$orderby"]);
			string sGROUP_BY = Sql.ToString (Request.Query["$groupby"]);
			string sSELECT   = Sql.ToString (Request.Query["$select" ]);
			Regex r = new Regex(@"[^A-Za-z0-9_]");
			string sFILTER_KEYWORDS = (" " + r.Replace(sFILTER, " ") + " ").ToLower();
			if ( sFILTER_KEYWORDS.Contains(" select ") )
			{
				throw(new Exception("Subqueries are not allowed."));
			}
			if ( sFILTER.Contains(";") )
			{
				throw(new Exception("A semicolon is not allowed anywhere in a filter. "));
			}
			if ( sORDER_BY.Contains(";") )
			{
				throw(new Exception("A semicolon is not allowed anywhere in a sort expression. "));
			}
			if ( !Security.IsAuthenticated() )
			{
				L10N L10n = new L10N(Sql.ToString(Session["USER_SETTINGS/CULTURE"]));
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}
			SplendidSession.CreateSession(Session);
			
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
			
			DataTable dt = GetTable("vwUSERS_ASSIGNED_TO_List", gTEAM_ID, nSKIP, nTOP, sFILTER, sORDER_BY, sGROUP_BY, arrSELECT);
			
			string sBaseURI = Request.Scheme + "://" + Request.Host.Host + Request.Path.Value.Replace("/GetModuleTable", "/GetModuleItem");
			
			Guid     gTIMEZONE         = Sql.ToGuid  (Session["USER_SETTINGS/TIMEZONE"]);
			SplendidCRM.TimeZone T10n  = TimeZone.CreateTimeZone(gTIMEZONE);
			// 04/01/2020 Paul.  Move json utils to RestUtil. 
			Dictionary<string, object> dictResponse = RestUtil.ToJson(sBaseURI, "Users", dt, T10n);
			return dictResponse;
		}

		private DataTable GetTable(string sTABLE_NAME, Guid gTEAM_ID, int nSKIP, int nTOP, string sFILTER, string sORDER_BY, string sGROUP_BY, UniqueStringCollection arrSELECT)
		{
			DataTable dt = null;
			try
			{
				if ( Security.IsAuthenticated() )
				{
					string sMATCH_NAME = String.Empty;
					SplendidCRM.DbProviderFactory dbf = DbProviderFactories.GetFactory();
					Regex r = new Regex(@"[^A-Za-z0-9_]");
					sTABLE_NAME = r.Replace(sTABLE_NAME, "");
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						using ( DataTable dtSYNC_TABLES = SplendidCache.RestTables(sTABLE_NAME, false) )
						{
							string sSQL = String.Empty;
							bool bEnableTeamHierarchy = Config.enable_team_hierarchy();
							if ( dtSYNC_TABLES != null && dtSYNC_TABLES.Rows.Count > 0 )
							{
								DataRow rowSYNC_TABLE = dtSYNC_TABLES.Rows[0];
								string sMODULE_NAME         = Sql.ToString (rowSYNC_TABLE["MODULE_NAME"        ]);
								string sVIEW_NAME           = Sql.ToString (rowSYNC_TABLE["VIEW_NAME"          ]);
								bool   bHAS_CUSTOM          = Sql.ToBoolean(rowSYNC_TABLE["HAS_CUSTOM"         ]);
								int    nMODULE_SPECIFIC     = Sql.ToInteger(rowSYNC_TABLE["MODULE_SPECIFIC"    ]);
								string sMODULE_FIELD_NAME   = Sql.ToString (rowSYNC_TABLE["MODULE_FIELD_NAME"  ]);
								bool   bIS_RELATIONSHIP     = Sql.ToBoolean(rowSYNC_TABLE["IS_RELATIONSHIP"    ]);
								string sMODULE_NAME_RELATED = Sql.ToString (rowSYNC_TABLE["MODULE_NAME_RELATED"]);
								string sASSIGNED_FIELD_NAME = Sql.ToString (rowSYNC_TABLE["ASSIGNED_FIELD_NAME"]);
								bool   bIS_SYSTEM           = Sql.ToBoolean(rowSYNC_TABLE["IS_SYSTEM"          ]);
								sTABLE_NAME                 = Sql.ToString (rowSYNC_TABLE["TABLE_NAME"         ]);
								sTABLE_NAME        = r.Replace(sTABLE_NAME       , "");
								sVIEW_NAME         = r.Replace(sVIEW_NAME        , "");
								sMODULE_FIELD_NAME = r.Replace(sMODULE_FIELD_NAME, "");
								// 08/02/2019 Paul.  The React Client will need access to views that require a filter, like CAMPAIGN_ID. 
								if ( dtSYNC_TABLES.Columns.Contains("REQUIRED_FIELDS") )
								{
									string sREQUIRED_FIELDS = Sql.ToString (rowSYNC_TABLE["REQUIRED_FIELDS"]);
									if ( !Sql.IsEmptyString(sREQUIRED_FIELDS) )
									{
										throw(new Exception("Missing required fields: " + sREQUIRED_FIELDS));
									}
								}
								
								if ( arrSELECT != null && arrSELECT.Count > 0 )
								{
									foreach ( string sColumnName in arrSELECT )
									{
										if ( Sql.IsEmptyString(sSQL) )
											sSQL += "select " + sVIEW_NAME + "." + sColumnName + ControlChars.CrLf;
										else
											sSQL += "     , " + sVIEW_NAME + "." + sColumnName + ControlChars.CrLf;
									}
								}
								else
								{
									sSQL = "select " + sVIEW_NAME + ".*" + ControlChars.CrLf;
								}
								sSQL += "  from " + sVIEW_NAME        + ControlChars.CrLf;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.CommandText = sSQL;
									cmd.CommandTimeout = 0;
									if ( !bEnableTeamHierarchy )
									{
										cmd.CommandText += " inner join vwTEAM_MEMBERSHIPS" + ControlChars.CrLf;
										cmd.CommandText += "         on vwTEAM_MEMBERSHIPS.MEMBERSHIP_TEAM_ID = @TEAM_ID                   " + ControlChars.CrLf;
										// 01/07/2018 Paul.  Specify the view name, even though it is hard-coded to be vwUSERS_ASSIGNED_TO_List. 
										cmd.CommandText += "        and vwTEAM_MEMBERSHIPS.MEMBERSHIP_USER_ID = " + sVIEW_NAME + ".ID" + ControlChars.CrLf;
									}
									else
									{
										if ( Sql.IsOracle(cmd) )
										{
											cmd.CommandText += " inner join table(fnTEAM_HIERARCHY_USERS(@TEAM_ID)) vwTEAM_MEMBERSHIPS         " + ControlChars.CrLf;
											// 01/07/2018 Paul.  Specify the view name, even though it is hard-coded to be vwUSERS_ASSIGNED_TO_List. 
											cmd.CommandText += "         on vwTEAM_MEMBERSHIPS.MEMBERSHIP_USER_ID = " + sVIEW_NAME + ".ID" + ControlChars.CrLf;
										}
										else
										{
											string fnPrefix = (Sql.IsSQLServer(cmd) ? "dbo." : String.Empty);
											cmd.CommandText += " inner join " + fnPrefix + "fnTEAM_HIERARCHY_USERS(@TEAM_ID) vwTEAM_MEMBERSHIPS" + ControlChars.CrLf;
											// 01/07/2018 Paul.  Specify the view name, even though it is hard-coded to be vwUSERS_ASSIGNED_TO_List. 
											cmd.CommandText += "         on vwTEAM_MEMBERSHIPS.MEMBERSHIP_USER_ID = " + sVIEW_NAME + ".ID" + ControlChars.CrLf;
										}
									}
									Sql.AddParameter(cmd, "@TEAM_ID", gTEAM_ID);
									Security.Filter(cmd, sMODULE_NAME, "view");
									if ( !Sql.IsEmptyString(sFILTER) )
									{
										// 04/01/2020 Paul.  Move json utils to RestUtil. 
										string sSQL_FILTER = RestUtil.ConvertODataFilter(sFILTER, cmd);
										cmd.CommandText += "   and (" + sSQL_FILTER + ")" + ControlChars.CrLf;;
									}
									if ( Sql.IsEmptyString(sORDER_BY.Trim()) )
									{
										sORDER_BY = " order by " + sVIEW_NAME + ".DATE_MODIFIED_UTC" + ControlChars.CrLf;
									}
									else
									{
										r = new Regex(@"[^A-Za-z0-9_, ]");
										sORDER_BY = " order by " + r.Replace(sORDER_BY, "");
									}
									if ( !Sql.IsEmptyString(sGROUP_BY) )
									{
										r = new Regex(@"[^A-Za-z0-9_, ]");
										sGROUP_BY = " group by " + r.Replace(sGROUP_BY, "");
									}
									using ( DbDataAdapter da = dbf.CreateDataAdapter() )
									{
										((IDbDataAdapter)da).SelectCommand = cmd;
										dt = new DataTable(sTABLE_NAME);
										if ( nTOP > 0 )
										{
											if ( nSKIP > 0 )
											{
												int nCurrentPageIndex = nSKIP / nTOP;
												Sql.PageResults(cmd, sTABLE_NAME, sORDER_BY, nCurrentPageIndex, nTOP);
												da.Fill(dt);
											}
											else
											{
												cmd.CommandText += sGROUP_BY + sORDER_BY;
												using ( DataSet ds = new DataSet() )
												{
													ds.Tables.Add(dt);
													da.Fill(ds, 0, nTOP, sTABLE_NAME);
												}
											}
										}
										else
										{
											cmd.CommandText += sGROUP_BY + sORDER_BY;
											da.Fill(dt);
										}
										if ( SplendidInit.bEnableACLFieldSecurity && !Sql.IsEmptyString(sMODULE_NAME) )
										{
											bool bApplyACL = false;
											foreach ( DataRow row in dt.Rows )
											{
												Guid gASSIGNED_USER_ID = Guid.Empty;
												foreach ( DataColumn col in dt.Columns )
												{
													Security.ACL_FIELD_ACCESS acl = Security.GetUserFieldSecurity(sMODULE_NAME, col.ColumnName, gASSIGNED_USER_ID);
													if ( !acl.IsReadable() )
													{
														row[col.ColumnName] = DBNull.Value;
														bApplyACL = true;
													}
												}
											}
											if ( bApplyACL )
												dt.AcceptChanges();
										}
									}
								}
							}
							else
							{
								SplendidError.SystemError(new StackTrace(true).GetFrame(0), sTABLE_NAME + " cannot be accessed.");
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				string sMessage = "GetTable(" + sTABLE_NAME + ", " + sFILTER + ", " + sORDER_BY + ") " + ex.Message;
				SplendidError.SystemMessage("Error", new StackTrace(true).GetFrame(0), sMessage);
				throw(new Exception(sMessage));
			}
			return dt;
		}

		[HttpPost("[action]")]
		public Dictionary<string, object> PostModuleList([FromBody] IDictionary<string, object> dict)
		{
			string ModuleName        = "Processes";
			int    nSKIP             = Sql.ToInteger(Request.Query["$skip"     ]);
			int    nTOP              = Sql.ToInteger(Request.Query["$top"      ]);
			string sFILTER           = String.Empty;  // Sql.ToString (Request.Query["$filter"   ]);
			string sORDER_BY         = Sql.ToString (Request.Query["$orderby"  ]);
			string sSELECT           = Sql.ToString (Request.Query["$select"   ]);
			bool   bMyList           = Sql.ToBoolean(Request.Query["MyList"    ]);
			
			Regex r = new Regex(@"[^A-Za-z0-9_]");
			string sFILTER_KEYWORDS = Sql.SqlFilterLiterals(sFILTER);
			sFILTER_KEYWORDS = (" " + r.Replace(sFILTER_KEYWORDS, " ") + " ").ToLower();
			int nSelectIndex     = sFILTER_KEYWORDS.IndexOf(" select ");
			int nFromIndex       = sFILTER_KEYWORDS.IndexOf(" from "  );
			if ( nSelectIndex >= 0 && nFromIndex > nSelectIndex )
			{
				throw(new Exception("Subqueries are not allowed."));
			}

			Guid     gTIMEZONE        = Sql.ToGuid  (Session["USER_SETTINGS/TIMEZONE"]);
			SplendidCRM.TimeZone T10n = TimeZone.CreateTimeZone(gTIMEZONE);
			Dictionary<string, object> dictSearchValues = null;
			try
			{
				foreach ( string sName in dict.Keys )
				{
					switch ( sName )
					{
						case "$skip"           :  nSKIP             = Sql.ToInteger(dict[sName]);  break;
						case "$top"            :  nTOP              = Sql.ToInteger(dict[sName]);  break;
						case "$filter"         :  sFILTER           = Sql.ToString (dict[sName]);  break;
						case "$orderby"        :  sORDER_BY         = Sql.ToString (dict[sName]);  break;
						case "$select"         :  sSELECT           = Sql.ToString (dict[sName]);  break;
						case "$searchvalues"   :  dictSearchValues = dict[sName] as Dictionary<string, object>;  break;
						case "MyList"          :  bMyList           = Sql.ToBoolean(dict[sName]);  break;
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
			string sTABLE_NAME = Sql.ToString(Application["Modules." + ModuleName + ".TableName"]);
			if ( Sql.IsEmptyString(sTABLE_NAME) )
				throw(new Exception("Unknown module: " + ModuleName));
			int nACLACCESS = Security.GetUserAccess(ModuleName, "list");
			if ( !Security.IsAuthenticated() || !Sql.ToBoolean(Application["Modules." + ModuleName + ".RestEnabled"]) || nACLACCESS < 0 )
			{
				L10N L10n = new L10N(Sql.ToString(Session["USER_SETTINGS/CULTURE"]));
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
			
			long lTotalCount = 0;
			StringBuilder sbDumpSQL = new StringBuilder();
			DataTable dt = new DataTable();
			SplendidCRM.DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				string sSQL = String.Empty;
				if ( arrSELECT != null && arrSELECT.Count > 0 )
				{
					foreach ( string sColumnName in arrSELECT )
					{
						if ( sColumnName == "FAVORITE_RECORD_ID" || sColumnName == "SUBSCRIPTION_PARENT_ID" )
							continue;
						if ( Sql.IsEmptyString(sSQL) )
							sSQL += "select " + sColumnName + ControlChars.CrLf;
						else
							sSQL += "     , " + sColumnName + ControlChars.CrLf;
					}
				}
				else
				{
					sSQL = "select *" + ControlChars.CrLf;
				}
				if ( bMyList )
					sSQL += "  from vwPROCESSES_MyList" + ControlChars.CrLf;
				else
					sSQL += "  from vwPROCESSES_List" + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					WF4ApprovalActivity.Filter(Application, cmd, Security.USER_ID);
					if ( !Sql.IsEmptyString(sORDER_BY) )
						cmd.CommandText += " order by " + sORDER_BY + ControlChars.CrLf;
					else
						cmd.CommandText += " order by DATE_MODIFIED desc" + sORDER_BY + ControlChars.CrLf;
					
					sbDumpSQL.Append(Sql.ExpandParameters(cmd));

					using ( DbDataAdapter da = dbf.CreateDataAdapter() )
					{
						((IDbDataAdapter)da).SelectCommand = cmd;
						da.Fill(dt);
						lTotalCount = dt.Rows.Count;
					}
				}
			}
			
			string sBaseURI = Request.Scheme + "://" + Request.Host.Host + Request.Path.Value.Replace("/PostModuleList", "/GetModuleItem");
			// 04/01/2020 Paul.  Move json utils to RestUtil. 
			Dictionary<string, object> dictResponse = RestUtil.ToJson(sBaseURI, ModuleName, dt, T10n);
			dictResponse.Add("__total", lTotalCount);
			if ( Sql.ToBoolean(Application["CONFIG.show_sql"]) )
			{
				dictResponse.Add("__sql", sbDumpSQL.ToString());
			}
			return dictResponse;
		}

		[HttpGet("[action]")]
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public Dictionary<string, object> GetModuleItem(Guid ID)
		{
			string ModuleName = "Processes";
			string sTABLE_NAME = Sql.ToString(Application["Modules." + ModuleName + ".TableName"]);
			if ( Sql.IsEmptyString(sTABLE_NAME) )
				throw(new Exception("Unknown module: " + ModuleName));
			int nACLACCESS = Security.GetUserAccess(ModuleName, "view");
			if ( !Security.IsAuthenticated() || !Sql.ToBoolean(Application["Modules." + ModuleName + ".RestEnabled"]) || nACLACCESS < 0 )
			{
				L10N L10n = new L10N(Sql.ToString(Session["USER_SETTINGS/CULTURE"]));
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS") + ": " + Sql.ToString(ModuleName)));
			}
			SplendidSession.CreateSession(Session);
			
			//AccessMode enumAccessMode = AccessMode.view;
			//string sAccessMode  = Sql.ToString (Request.QueryString["$accessMode" ]);
			//if ( sAccessMode == "edit" )
			//	enumAccessMode = AccessMode.edit;
			
			Guid[] arrITEMS = new Guid[1] { ID };
			StringBuilder sbDumpSQL = new StringBuilder();
			DataTable dt = new DataTable();
			SplendidCRM.DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				string sSQL = String.Empty;
				sSQL = "select *          " + ControlChars.CrLf
				     + "  from vwPROCESSES" + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Security.Filter(cmd, ModuleName, "view");
					Sql.AppendParameter(cmd, ID, "ID", false);
					con.Open();
					
					sbDumpSQL.Append(Sql.ExpandParameters(cmd));
					
					using ( DbDataAdapter da = dbf.CreateDataAdapter() )
					{
						((IDbDataAdapter)da).SelectCommand = cmd;
						da.Fill(dt);
					}
				}
			}
			if ( dt == null || dt.Rows.Count == 0 )
				throw(new Exception("Item not found: " + ModuleName + " " + ID.ToString()));
			

			string sBaseURI = Request.Scheme + "://" + Request.Host.Host + Request.Path.Value;
			
			Guid     gTIMEZONE         = Sql.ToGuid  (Session["USER_SETTINGS/TIMEZONE"]);
			SplendidCRM.TimeZone T10n  = TimeZone.CreateTimeZone(gTIMEZONE);
			// 04/01/2020 Paul.  Move json utils to RestUtil. 
			Dictionary<string, object> dict = RestUtil.ToJson(sBaseURI, ModuleName, dt.Rows[0], T10n);
			if ( dt.Columns.Contains("PROCESS_NUMBER") )
			{
				string sName = Sql.ToString(dt.Rows[0]["PROCESS_NUMBER"]);
				try
				{
					SqlProcs.spTRACKER_Update(Security.USER_ID, ModuleName, ID, sName, "detailview");
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			
			if ( Sql.ToBoolean(Application["CONFIG.show_sql"]) )
			{
				dict.Add("__sql", sbDumpSQL.ToString());
			}
			return dict;
		}

	}
}
