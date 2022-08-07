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
	[Route("Processes/Rest.svc")]
	public class ProcessRestController : ControllerBase
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
		private Utils                Utils              ;
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

		public ProcessRestController(IWebHostEnvironment hostingEnvironment, IMemoryCache memoryCache, HttpSessionState Session, Security Security, Utils Utils, SplendidError SplendidError, SplendidCache SplendidCache, RestUtil RestUtil, SplendidDynamic SplendidDynamic, SplendidInit SplendidInit, SplendidCRM.Crm.Modules Modules, ModuleUtils.Audit Audit, ModuleUtils.AuditPersonalInfo AuditPersonalInfo)
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
			this.Modules             = Modules            ;
			this.Audit               = Audit              ;
			this.AuditPersonalInfo   = AuditPersonalInfo  ;
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
			//bool bFound = WF4ApprovalActivity.GetProcessStatus(Application, L10n, gPENDING_PROCESS_ID, ref sProcessStatus, ref bShowApprove, ref bShowReject, ref bShowRoute, ref bShowClaim, ref sUSER_TASK_TYPE, ref gPROCESS_USER_ID, ref gASSIGNED_TEAM_ID, ref gPROCESS_TEAM_ID);
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
			// 04/01/2020 Paul.  Move json utils to RestUtil. 
			Dictionary<string, object> dict = RestUtil.ToJson(sBaseURI, "Processes", dt, T10n);
			return dict;
		}

	}
}
