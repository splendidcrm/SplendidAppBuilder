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
	// 01/25/2022 Paul.  We are now using the Identity to take advantage of [Authorize] attribute. 
	[Authorize]
	[ApiController]
	[Route("Administration/ModuleBuilder/Rest.svc")]
	public class ModuleBuilderRestController : ControllerBase
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

		public ModuleBuilderRestController(IWebHostEnvironment hostingEnvironment, IMemoryCache memoryCache, HttpSessionState Session, Security Security, Utils Utils, SplendidError SplendidError, SplendidCache SplendidCache, RestUtil RestUtil, SplendidDynamic SplendidDynamic, SplendidInit SplendidInit, SplendidCRM.Crm.Modules Modules, ModuleUtils.Audit Audit, ModuleUtils.AuditPersonalInfo AuditPersonalInfo)
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

		public class ModuleField
		{
			public string FIELD_NAME        ;
			public string EDIT_LABEL        ;
			public string LIST_LABEL        ;
			public string DATA_TYPE         ;
			public int    MAX_SIZE          ;
			public bool   REQUIRED          ;
		}

		[HttpGet("[action]")]
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public Dictionary<string, object> GetModuleFields(string ModuleName)
		{
			if ( !Security.IsAuthenticated() || !Security.IS_ADMIN )
			{
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}
			SplendidSession.CreateSession(Session);
			
			if ( Sql.IsEmptyString(ModuleName) )
				throw(new Exception("The module name must be specified."));
			string sTABLE_NAME = Sql.ToString(Application["Modules." + ModuleName + ".TableName"]);
			string sVIEW_NAME  = "vw" + sTABLE_NAME;
			bool   bValid      = Sql.ToBoolean(Application["Modules." + ModuleName + ".Valid"]);
			if ( Sql.IsEmptyString(sTABLE_NAME) && !bValid )
				throw(new Exception("Unknown module: " + ModuleName));
			
			List<ModuleField> lstFields = new List<ModuleField>();
			SplendidCRM.DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				con.Open();
				string sSQL;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					sSQL = "select ColumnName as FIELD_NAME        " + ControlChars.CrLf
					     + "     , dbo.fnL10nTerm('en-US', @MODULE_NAME, 'LBL_'      + ColumnName) as EDIT_LABEL" + ControlChars.CrLf
					     + "     , dbo.fnL10nTerm('en-US', @MODULE_NAME, 'LBL_LIST_' + ColumnName) as LIST_LABEL" + ControlChars.CrLf
					     + "     , (case when dbo.fnSqlColumns_IsEnum(@VIEW_NAME, ColumnName, CsType) = 1 then 'Dropdown' " + ControlChars.CrLf
					     + "             when ColumnType = 'nvarchar(max)'                                then 'Text Area'" + ControlChars.CrLf
					     + "             when SqlDbType = 'SqlDbType.NVarChar'                            then 'Text'     " + ControlChars.CrLf
					     + "             when SqlDbType = 'SqlDbType.VarChar'                             then 'Text'     " + ControlChars.CrLf
					     + "             when SqlDbType = 'SqlDbType.Text'                                then 'Text Area'" + ControlChars.CrLf
					     + "             when SqlDbType = 'SqlDbType.NText'                               then 'Text Area'" + ControlChars.CrLf
					     + "             when SqlDbType = 'SqlDbType.TinyInt'                             then 'Integer'  " + ControlChars.CrLf
					     + "             when SqlDbType = 'SqlDbType.Int'                                 then 'Integer'  " + ControlChars.CrLf
					     + "             when SqlDbType = 'SqlDbType.BigInt'                              then 'Integer'  " + ControlChars.CrLf
					     + "             when SqlDbType = 'SqlDbType.Real'                                then 'Decimal'  " + ControlChars.CrLf
					     + "             when SqlDbType = 'SqlDbType.Money'                               then 'Money'    " + ControlChars.CrLf
					     + "             when SqlDbType = 'SqlDbType.Bit'                                 then 'Checkbox' " + ControlChars.CrLf
					     + "             when SqlDbType = 'SqlDbType.DateTime'                            then 'Date'     " + ControlChars.CrLf
					     + "             when SqlDbType = 'SqlDbType.UniqueIdentifier'                    then 'Guid'     " + ControlChars.CrLf
					     + "             else CsType               " + ControlChars.CrLf
					     + "        end)      as DATA_TYPE         " + ControlChars.CrLf
					     + "     , length     as MAX_SIZE          " + ControlChars.CrLf
					     + "     , (case IsNullable when 1 then 0 else 1 end) as REQUIRED" + ControlChars.CrLf
					     + "  from vwSqlColumns                    " + ControlChars.CrLf
					     + " where ObjectName = @TABLE_NAME        " + ControlChars.CrLf
					     + " order by colid                        " + ControlChars.CrLf;
					
					cmd.CommandText = sSQL;
					Sql.AddParameter(cmd, "@MODULE_NAME", ModuleName);
					Sql.AddParameter(cmd, "@VIEW_NAME"  , Sql.MetadataName(cmd, sVIEW_NAME));
					Sql.AddParameter(cmd, "@TABLE_NAME" , sTABLE_NAME);
					
					using ( DbDataAdapter da = dbf.CreateDataAdapter() )
					{
						((IDbDataAdapter)da).SelectCommand = cmd;
						using ( DataTable dt = new DataTable() )
						{
							da.Fill(dt);
							foreach ( DataRow row in dt.Rows )
							{
								ModuleField lay = new ModuleField();
								lay.FIELD_NAME = Sql.ToString (row["FIELD_NAME"]);
								lay.EDIT_LABEL = Sql.ToString (row["EDIT_LABEL"]);
								lay.LIST_LABEL = Sql.ToString (row["LIST_LABEL"]);
								lay.DATA_TYPE  = Sql.ToString (row["DATA_TYPE" ]);
								lay.MAX_SIZE   = Sql.ToInteger(row["MAX_SIZE"  ]);
								lay.REQUIRED   = Sql.ToBoolean(row["REQUIRED"  ]);
								lstFields.Add(lay);
							}
						}
					}
				}
			}
			
			Dictionary<string, object> d = new Dictionary<string, object>();
			d.Add("d", lstFields);
			return d;
		}

		[HttpPost("[action]")]
		public Dictionary<string, object> GenerateModule([FromBody] Dictionary<string, object> dict)
		{
			try
			{
				if ( !Security.IsAuthenticated() || !Security.IS_ADMIN )
				{
					throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
				}
				SplendidSession.CreateSession(Session);
				
				string sDISPLAY_NAME             = String.Empty;
				string sMODULE_NAME              = String.Empty;
				string sTABLE_NAME               = String.Empty;
				bool   bTAB_ENABLED              = false;
				bool   bMOBILE_ENABLED           = false;
				bool   bCUSTOM_ENABLED           = false;
				bool   bREPORT_ENABLED           = false;
				bool   bIMPORT_ENABLED           = false;
				bool   bREST_ENABLED             = false;
				bool   bIS_ADMIN                 = false;
				bool   bINCLUDE_ASSIGNED_USER_ID = false;
				bool   bINCLUDE_TEAM_ID          = false;
				bool   bOVERWRITE_EXISTING       = false;
				bool   bCREATE_CODE_BEHIND       = false;
				bool   bREACT_ONLY               = false;
				List<string> lstRelationships = new List<string>();
				DataTable dtFields = new DataTable();
				DataColumn colFIELD_NAME = new DataColumn("FIELD_NAME", Type.GetType("System.String" ));
				DataColumn colEDIT_LABEL = new DataColumn("EDIT_LABEL", Type.GetType("System.String" ));
				DataColumn colLIST_LABEL = new DataColumn("LIST_LABEL", Type.GetType("System.String" ));
				DataColumn colDATA_TYPE  = new DataColumn("DATA_TYPE" , Type.GetType("System.String" ));
				DataColumn colMAX_SIZE   = new DataColumn("MAX_SIZE"  , Type.GetType("System.Int32"  ));
				DataColumn colREQUIRED   = new DataColumn("REQUIRED"  , Type.GetType("System.Boolean"));
				dtFields.Columns.Add(colFIELD_NAME);
				dtFields.Columns.Add(colEDIT_LABEL);
				dtFields.Columns.Add(colLIST_LABEL);
				dtFields.Columns.Add(colDATA_TYPE );
				dtFields.Columns.Add(colMAX_SIZE  );
				dtFields.Columns.Add(colREQUIRED  );
				foreach ( string sColumnName in dict.Keys )
				{
					switch ( sColumnName )
					{
						case "DISPLAY_NAME"            :  sDISPLAY_NAME             = Sql.ToString (dict[sColumnName]);  break;
						case "MODULE_NAME"             :  sMODULE_NAME              = Sql.ToString (dict[sColumnName]);  break;
						case "TABLE_NAME"              :  sTABLE_NAME               = Sql.ToString (dict[sColumnName]);  break;
						case "TAB_ENABLED"             :  bTAB_ENABLED              = Sql.ToBoolean(dict[sColumnName]);  break;
						case "MOBILE_ENABLED"          :  bMOBILE_ENABLED           = Sql.ToBoolean(dict[sColumnName]);  break;
						case "CUSTOM_ENABLED"          :  bCUSTOM_ENABLED           = Sql.ToBoolean(dict[sColumnName]);  break;
						case "REPORT_ENABLED"          :  bREPORT_ENABLED           = Sql.ToBoolean(dict[sColumnName]);  break;
						case "IMPORT_ENABLED"          :  bIMPORT_ENABLED           = Sql.ToBoolean(dict[sColumnName]);  break;
						case "REST_ENABLED"            :  bREST_ENABLED             = Sql.ToBoolean(dict[sColumnName]);  break;
						case "IS_ADMIN"                :  bIS_ADMIN                 = Sql.ToBoolean(dict[sColumnName]);  break;
						case "INCLUDE_ASSIGNED_USER_ID":  bINCLUDE_ASSIGNED_USER_ID = Sql.ToBoolean(dict[sColumnName]);  break;
						case "INCLUDE_TEAM_ID"         :  bINCLUDE_TEAM_ID          = Sql.ToBoolean(dict[sColumnName]);  break;
						case "OVERWRITE_EXISTING"      :  bOVERWRITE_EXISTING       = Sql.ToBoolean(dict[sColumnName]);  break;
						case "CREATE_CODE_BEHIND"      :  bCREATE_CODE_BEHIND       = Sql.ToBoolean(dict[sColumnName]);  break;
						case "REACT_ONLY"              :  bREACT_ONLY               = Sql.ToBoolean(dict[sColumnName]);  break;
					}
					if ( dict[sColumnName] is System.Collections.ArrayList )
					{
						System.Collections.ArrayList lst = dict[sColumnName] as System.Collections.ArrayList;
						if ( lst != null )
						{
							if ( sColumnName == "Fields" )
							{
								foreach ( Dictionary<string, object> field in lst )
								{
									DataRow row = dtFields.NewRow();
									dtFields.Rows.Add(row);
									foreach ( string sFieldName in field.Keys )
									{
										switch ( sFieldName )
										{
											case "FIELD_NAME":  row[sFieldName] = Sql.ToString (field[sFieldName]);  break;
											case "EDIT_LABEL":  row[sFieldName] = Sql.ToString (field[sFieldName]);  break;
											case "LIST_LABEL":  row[sFieldName] = Sql.ToString (field[sFieldName]);  break;
											case "DATA_TYPE" :  row[sFieldName] = Sql.ToString (field[sFieldName]);  break;
											case "MAX_SIZE"  :  row[sFieldName] = Sql.ToInteger(field[sFieldName]);  break;
											case "REQUIRED"  :  row[sFieldName] = Sql.ToBoolean(field[sFieldName]);  break;
										}
									}
								}
							}
							else if ( sColumnName == "Relationships" )
							{
								foreach ( object obj in lst )
								{
									lstRelationships.Add(Sql.ToString(obj));
								}
							}
						}
					}
				}
				bREACT_ONLY = true;
				StringBuilder sbProgress = new StringBuilder();
				GenerateModule(sDISPLAY_NAME, sMODULE_NAME, sTABLE_NAME, bTAB_ENABLED, bMOBILE_ENABLED, bCUSTOM_ENABLED, bREPORT_ENABLED, bIMPORT_ENABLED, bREST_ENABLED, bIS_ADMIN, bINCLUDE_ASSIGNED_USER_ID, bINCLUDE_TEAM_ID, bOVERWRITE_EXISTING, bCREATE_CODE_BEHIND, bREACT_ONLY, dtFields, lstRelationships, sbProgress);
				
				Dictionary<string, object> dictResponse = new Dictionary<string, object>();
				dictResponse.Add("d", sbProgress.ToString());
				return dictResponse;
			}
			catch(Exception ex)
			{
				// 03/20/2019 Paul.  Catch and log all failures, including insufficient access. 
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				throw;
			}
		}

		public string CamelCase(string sName)
		{
			string[] arrName = sName.Split('_');
			for ( int i = 0; i < arrName.Length; i++ )
			{
				if( String.Compare(arrName[i], "ID", true) == 0 )
					arrName[i] = arrName[i].ToUpper();
				else
					arrName[i] = arrName[i].Substring(0, 1).ToUpper() + arrName[i].Substring(1).ToLower();
			}
			sName = String.Join(" ", arrName);
			return sName;
		}

		private string RemoveComments(string sScript)
		{
			StringReader rdr = new StringReader(sScript);
			StringBuilder sb = new StringBuilder();
			StringWriter wtr = new StringWriter(sb);
			string sLine = null;
			while ( (sLine = rdr.ReadLine()) != null )
			{
				// 09/13/2008 Paul.  DB2 does not like any comments. 
				if ( !sLine.StartsWith("--") && !sLine.StartsWith("\t--") && !sLine.Contains("\t\t--") )
					wtr.WriteLine(sLine);
			}
			return sb.ToString();
		}


		private string TabSpace(int nNumber)
		{
			return Strings.Space(nNumber).Replace(' ', '\t');
		}

		// 03/06/2010 Paul.  Make the BuildWrapper function public and static so that it can be reused in the ModuleBuilder. 
		private void BuildWrapper(ref StringBuilder sb, string sProcedureName, ref DataRowCollection colRows, bool bCreateCommand, bool bTransaction)
		{
			// 10/07/2009 Paul.  We need to prevent the use of the system transaction function as a stand-alone call.  It makes no sense. 
			if ( String.Compare(sProcedureName, "spSYSTEM_TRANSACTIONS_Create", true) == 0 && !bCreateCommand && !bTransaction )
				return;
			
			int nColumnAlignmentSize = 5;
			int nSpace=0;
			string sPrimaryKey     = String.Empty;
			bool   bPrimaryDefault = false;
			if ( colRows.Count > 0 )
			{
				sPrimaryKey     = Sql.ToString (colRows[0]["ColumnName"]);
				bPrimaryDefault = Sql.ToBoolean(colRows[0]["cdefault"  ]);
			}
			for ( int j = 0 ; j < colRows.Count; j++ )
			{
				DataRow row = colRows[j];
				string sName = Sql.ToString(row["ColumnName"]);
				if ( sName.Length >= nColumnAlignmentSize )
					nColumnAlignmentSize = sName.Length + 1;
			}
			int k = 0;
			int nIndent = 2;
			if ( bCreateCommand )
			{
				sb.AppendLine(TabSpace(nIndent) + "#region cmd" + (sProcedureName.StartsWith("sp") ? sProcedureName.Substring(2) : sProcedureName));
				sb.AppendLine(TabSpace(nIndent) + "/// <summary>");
				sb.AppendLine(TabSpace(nIndent) + "/// " + sProcedureName);
				sb.AppendLine(TabSpace(nIndent) + "/// </summary>");
				sb.Append(TabSpace(nIndent) + "public static IDbCommand cmd" + (sProcedureName.StartsWith("sp") ? sProcedureName.Substring(2) : sProcedureName) + "(");
				sb.Append("IDbConnection con");
				k++;
			}
			else
			{
				sb.AppendLine(TabSpace(nIndent) + "#region " + sProcedureName);
				sb.AppendLine(TabSpace(nIndent) + "/// <summary>");
				sb.AppendLine(TabSpace(nIndent) + "/// " + sProcedureName);
				sb.AppendLine(TabSpace(nIndent) + "/// </summary>");
				sb.Append(TabSpace(nIndent) + "public static void " + sProcedureName + "(");
				for ( int j = 0; j < colRows.Count; j++ )
				{
					DataRow row = colRows[j];
					string sName     = Sql.ToString (row["ColumnName"]);
					string sCsType   = Sql.ToString (row["CsType"    ]);
					string sCsPrefix = Sql.ToString (row["CsPrefix"  ]);
					bool   bIsOutput = Sql.ToBoolean(row["isoutparam"]);
					string sBareName = sName.Replace("@", "");
					// 06/23/2005 Paul.  Modified User ID is automatic. 
					if ( sBareName == "MODIFIED_USER_ID" )
						continue;
					if ( k > 0 )
						sb.Append(", ");
					if ( bIsOutput )
						sb.Append("ref ");
					// 01/24/2006 Paul.  A severe error occurred on the current command. The results, if any, should be discarded. 
					// MS03-031 security patch causes this error because of stricter datatype processing.  
					// http://www.microsoft.com/technet/security/bulletin/MS03-031.mspx.
					// http://support.microsoft.com/kb/827366/
					sCsType = (sCsType == "ansistring") ? "string" : sCsType;
					sb.Append(sCsType + " " + sCsPrefix + sBareName);
					k++;
				}
				if ( bTransaction )
				{
					if ( colRows.Count > 1 )
						sb.Append(", ");
					else if ( colRows.Count == 1 )
					{
						// 11/19/2006 Paul.  Skip first parameter if MODIFIED_USER_ID. 
						if ( Sql.ToString (colRows[0]["ColumnName"]) != "@MODIFIED_USER_ID" )
							sb.Append(", ");
					}
					sb.Append("IDbTransaction trn");
				}
			}
			sb.AppendLine(")");
			sb.AppendLine(TabSpace(nIndent) + "{");
			nIndent++;
			if ( !bCreateCommand )
			{
				if ( bTransaction )
				{
					sb.AppendLine(TabSpace(nIndent) + "IDbConnection con = trn.Connection;");
				}
				else
				{
					sb.AppendLine(TabSpace(nIndent) + "DbProviderFactory dbf = DbProviderFactories.GetFactory();");
					sb.AppendLine(TabSpace(nIndent) + "using ( IDbConnection con = dbf.CreateConnection() )");
					sb.AppendLine(TabSpace(nIndent) + "{");
					nIndent++;
				}
				// 05/01/2006 Paul.  All commands now use a transaction.  This is because Oracle does not have a transaction hierarchy. 
				// So any COMMIT in a procedure, will commit the entire transaction.
				// We want the web application to be in control of the transaction.
				if ( !bTransaction )
				{
					sb.AppendLine(TabSpace(nIndent) + "con.Open();");
					// 10/07/2009 Paul.  We need to create our own global transaction ID to support auditing and workflow on SQL Azure, PostgreSQL, Oracle, DB2 and MySQL. 
					// This is because SQL Server 2005 and 2008 are the only platforms that support a global transaction ID with sp_getbindtoken. 
					sb.AppendLine(TabSpace(nIndent) + "using ( IDbTransaction trn = Sql.BeginTransaction(con) )");
					sb.AppendLine(TabSpace(nIndent) + "{");
					nIndent++;
					sb.AppendLine(TabSpace(nIndent) + "try");
					sb.AppendLine(TabSpace(nIndent) + "{");
					nIndent++;
				}
				sb.AppendLine(TabSpace(nIndent) + "using ( IDbCommand cmd = con.CreateCommand() )");
				sb.AppendLine(TabSpace(nIndent) + "{");
				nIndent++;
				// 05/01/2006 Paul.  All commands now use a transaction. 
				sb.AppendLine(TabSpace(nIndent) + "cmd.Transaction = trn;");
				sb.AppendLine(TabSpace(nIndent) + "cmd.CommandType = CommandType.StoredProcedure;");
				// 08/14/2005 Paul.  Truncate procedure names on a case-by-case basis. 
				// Oracle only supports identifiers up to 30 characters. 
				if ( sProcedureName.Length > 30 )
				{
					sb.AppendLine(TabSpace(nIndent) + "if ( Sql.IsOracle(cmd) )");
					sb.AppendLine(TabSpace(nIndent) + "	cmd.CommandText = \"" + sProcedureName.Substring(0, 30) + "\";");
					sb.AppendLine(TabSpace(nIndent) + "else");
					sb.AppendLine(TabSpace(nIndent) + "	cmd.CommandText = \"" + sProcedureName + "\";");
				}
				else
				{
					sb.AppendLine(TabSpace(nIndent) + "cmd.CommandText = \"" + sProcedureName + "\";");
				}
			}
			else
			{
				sb.AppendLine(TabSpace(nIndent) + "IDbCommand cmd = con.CreateCommand();");
				sb.AppendLine(TabSpace(nIndent) + "cmd.CommandType = CommandType.StoredProcedure;");
				// 08/14/2005 Paul.  Truncate procedure names on a case-by-case basis. 
				// Oracle only supports identifiers up to 30 characters. 
				if ( sProcedureName.Length > 30 )
				{
					sb.AppendLine(TabSpace(nIndent) + "if ( Sql.IsOracle(cmd) )");
					sb.AppendLine(TabSpace(nIndent) + "	cmd.CommandText = \"" + sProcedureName.Substring(0, 30) + "\";");
					sb.AppendLine(TabSpace(nIndent) + "else");
					sb.AppendLine(TabSpace(nIndent) + "	cmd.CommandText = \"" + sProcedureName + "\";");
				}
				else
				{
					sb.AppendLine(TabSpace(nIndent) + "cmd.CommandText = \"" + sProcedureName + "\";");
				}
			}
			for ( int j = 0 ; j < colRows.Count; j++ )
			{
				DataRow row = colRows[j];
				string sName      = Sql.ToString (row["ColumnName"]);
				string sSqlDbType = Sql.ToString (row["SqlDbType" ]);
				string sCsPrefix  = Sql.ToString (row["CsPrefix"  ]);
				string sCsType    = Sql.ToString (row["CsType"    ]);
				int    nLength    = Sql.ToInteger(row["length"    ]);
				int    nMaxLength = Sql.ToInteger(row["max_length"]);
				bool   bIsOutput  = Sql.ToBoolean(row["isoutparam"]);
				string sBareName  = sName.Replace("@", "");
				nSpace = nColumnAlignmentSize - sBareName.Length;
				nSpace = Math.Max(2, nSpace);
				int nSpaceSqlType = 26 - sSqlDbType.Length;
				nSpaceSqlType = Math.Max(0, nSpaceSqlType);
				/*
				switch ( sSqlDbType )
				{
					case "SqlDbType.VarBinary":
						sb.AppendLine(TabSpace(nIndent) + "IDbDataParameter par" + sBareName + Strings.Space(nSpace-1) + "= Sql.AddParameter(cmd, \"" + sName + "\"" + Strings.Space(nSpace-2) + ", " + sCsPrefix + sBareName + Strings.Space(nSpaceCsPrefix-2) + ");");
						break;
					default:
						sb.AppendLine(TabSpace(nIndent) + "IDbDataParameter par" + sBareName + Strings.Space(nSpace-1) + "= Sql.AddParameter(cmd, \"" + sName + "\"" + Strings.Space(nSpace-2) + ", " + sCsPrefix + sBareName + Strings.Space(nSpaceCsPrefix-2) + ");");
						break;
				}
				*/
				int nSpaceCsPrefix = 3 + nColumnAlignmentSize - sBareName.Length - sCsPrefix.Length;
				nSpaceCsPrefix = Math.Max(2, nSpaceCsPrefix);
				if ( !bCreateCommand )
				{
					// 01/24/2006 Paul.  A severe error occurred on the current command. The results, if any, should be discarded. 
					// MS03-031 security patch causes this error because of stricter datatype processing.  
					// http://www.microsoft.com/technet/security/bulletin/MS03-031.mspx.
					// http://support.microsoft.com/kb/827366/
					if ( sBareName == "MODIFIED_USER_ID" )
						sb.AppendLine(TabSpace(nIndent) + "IDbDataParameter par" + sBareName + Strings.Space(nSpace-1) + "= Sql.AddParameter(cmd, \"" + sName + "\"" + Strings.Space(nSpace-2) + ", " + " Security.USER_ID" + Strings.Space(nSpaceCsPrefix-2) + ");");
					else if ( sSqlDbType == "SqlDbType.NVarChar" )
					{
						// 09/15/2009 Paul.  For nvarchar(max), don't specify a length. 
						// 06/22/2016 Paul.  An nvarchar(max) output must specify a size when used as output. 
						if ( nMaxLength == -1 && bIsOutput )
							sb.AppendLine(TabSpace(nIndent) + "IDbDataParameter par" + sBareName + Strings.Space(nSpace-1) + "= Sql.AddParameter(cmd, \"" + sName + "\"" + Strings.Space(nSpace-2) + ", " + sCsPrefix + sBareName + Strings.Space(nSpaceCsPrefix-2) + ", 2147483647);");
						else if ( nMaxLength == -1 )
							sb.AppendLine(TabSpace(nIndent) + "IDbDataParameter par" + sBareName + Strings.Space(nSpace-1) + "= Sql.AddParameter(cmd, \"" + sName + "\"" + Strings.Space(nSpace-2) + ", " + sCsPrefix + sBareName + Strings.Space(nSpaceCsPrefix-2) + ");");
						else
							sb.AppendLine(TabSpace(nIndent) + "IDbDataParameter par" + sBareName + Strings.Space(nSpace-1) + "= Sql.AddParameter(cmd, \"" + sName + "\"" + Strings.Space(nSpace-2) + ", " + sCsPrefix + sBareName + Strings.Space(nSpaceCsPrefix-2) + "," + Strings.Space(Math.Max(1, 4-nLength.ToString().Length)) + nLength.ToString() +");");
					}
					else if ( sSqlDbType == "SqlDbType.VarChar" )
					{
						// 09/15/2009 Paul.  For varchar(max), don't specify a length. 
						// 06/22/2016 Paul.  An varchar(max) output must specify a size when used as output. 
						if ( nMaxLength == -1 && bIsOutput )
							sb.AppendLine(TabSpace(nIndent) + "IDbDataParameter par" + sBareName + Strings.Space(nSpace-1) + "= Sql.AddAnsiParam(cmd, \"" + sName + "\"" + Strings.Space(nSpace-2) + ", " + sCsPrefix + sBareName + Strings.Space(nSpaceCsPrefix-2) + ", 2147483647);");
						else if ( nMaxLength == -1 )
							sb.AppendLine(TabSpace(nIndent) + "IDbDataParameter par" + sBareName + Strings.Space(nSpace-1) + "= Sql.AddAnsiParam(cmd, \"" + sName + "\"" + Strings.Space(nSpace-2) + ", " + sCsPrefix + sBareName + Strings.Space(nSpaceCsPrefix-2) + ");");
						else
							sb.AppendLine(TabSpace(nIndent) + "IDbDataParameter par" + sBareName + Strings.Space(nSpace-1) + "= Sql.AddAnsiParam(cmd, \"" + sName + "\"" + Strings.Space(nSpace-2) + ", " + sCsPrefix + sBareName + Strings.Space(nSpaceCsPrefix-2) + "," + Strings.Space(Math.Max(1, 4-nLength.ToString().Length)) + nLength.ToString() +");");
					}
					else
						sb.AppendLine(TabSpace(nIndent) + "IDbDataParameter par" + sBareName + Strings.Space(nSpace-1) + "= Sql.AddParameter(cmd, \"" + sName + "\"" + Strings.Space(nSpace-2) + ", " + sCsPrefix + sBareName + Strings.Space(nSpaceCsPrefix-2) + ");");
				}
				else
				{
					sb.AppendLine(TabSpace(nIndent) + "IDbDataParameter par" + sBareName + Strings.Space(nSpace-1) + "= Sql.CreateParameter(cmd, \"" + sName + "\"" + Strings.Space(nSpace-2) + ", \"" + sCsType + "\"," + Strings.Space(Math.Max(1, 4-nLength.ToString().Length)) + nLength.ToString() +");");
				}
			}
			if ( !bCreateCommand )
			{
				for ( int j = 0 ; j < colRows.Count; j++ )
				{
					DataRow row = colRows[j];
					string sName      = Sql.ToString (row["ColumnName"]);
					string sBareName  = sName.Replace("@", "");
					string sCsPrefix  = Sql.ToString (row["CsPrefix"  ]);
					string sCsType    = Sql.ToString (row["CsType"    ]);
					bool   bIsOutput  = Sql.ToBoolean(row["isoutparam"]);
					nSpace   = nColumnAlignmentSize - sBareName.Length;
					nSpace   = Math.Max(2, nSpace);
					int nSpaceCsPrefix = 3 + nColumnAlignmentSize - sBareName.Length - sCsPrefix.Length;
					nSpaceCsPrefix = Math.Max(2, nSpaceCsPrefix);
					if ( bIsOutput )
						sb.AppendLine(TabSpace(nIndent) + "par" + sBareName + ".Direction = ParameterDirection.InputOutput;");
					/*
					switch ( sCsType )
					{
						case "string":
							sb.AppendLine(TabSpace(nIndent) + "par" + sBareName + Strings.Space(nSpace-2) + ".Value     = Sql.ToDBString  (" + sCsPrefix + sBareName + Strings.Space(nSpaceCsPrefix-2) + ");");
							break;
						case "DateTime":
							sb.AppendLine(TabSpace(nIndent) + "par" + sBareName + Strings.Space(nSpace-2) + ".Value     = Sql.ToDBDateTime(" + sCsPrefix + sBareName + Strings.Space(nSpaceCsPrefix-2) + ");");
							break;
						case "Guid":
							if ( sBareName == "MODIFIED_USER_ID" )
								sb.AppendLine(TabSpace(nIndent) + "par" + sBareName + Strings.Space(nSpace-2) + ".Value     = Security.USER_ID;");
							else
								sb.AppendLine(TabSpace(nIndent) + "par" + sBareName + Strings.Space(nSpace-2) + ".Value     = Sql.ToDBGuid    (" + sCsPrefix + sBareName + Strings.Space(nSpaceCsPrefix-2) + ");");
							break;
						case "Int32":
						case "short":
							sb.AppendLine(TabSpace(nIndent) + "par" + sBareName + Strings.Space(nSpace-2) + ".Value     = Sql.ToDBInteger (" + sCsPrefix + sBareName + Strings.Space(nSpaceCsPrefix-2) + ");");
							break;
						case "float":
							sb.AppendLine(TabSpace(nIndent) + "par" + sBareName + Strings.Space(nSpace-2) + ".Value     = Sql.ToDBFloat   (" + sCsPrefix + sBareName + Strings.Space(nSpaceCsPrefix-2) + ");");
							break;
						case "decimal":
							sb.AppendLine(TabSpace(nIndent) + "par" + sBareName + Strings.Space(nSpace-2) + ".Value     = Sql.ToDBDecimal (" + sCsPrefix + sBareName + Strings.Space(nSpaceCsPrefix-2) + ");");
							break;
						case "bool":
							sb.AppendLine(TabSpace(nIndent) + "par" + sBareName + Strings.Space(nSpace-2) + ".Value     = Sql.ToDBBoolean (" + sCsPrefix + sBareName + Strings.Space(nSpaceCsPrefix-2) + ");");
							break;
						case "byte[]":
							sb.AppendLine(TabSpace(nIndent) + "par" + sBareName + Strings.Space(nSpace-2) + ".Value     = Sql.ToDBBinary  (" + sCsPrefix + sBareName + Strings.Space(nSpaceCsPrefix-2) + ");");
							break;
						default:
							sb.AppendLine(TabSpace(nIndent) + "par" + sBareName + Strings.Space(nSpace-2) + ".Value     = " + sCsPrefix + sBareName + Strings.Space(nSpaceCsPrefix-2) + ";");
							break;
					}
					*/
				}
				// 03/07/2010 Paul.  Move the Trace function to the Sql class. 
				// 02/10/2012 Paul.  WORKFLOW and SYSTEM procedures are plentyful and not useful in the trace. 
				if ( bTransaction && !sProcedureName.StartsWith("spWORKFLOW_") && !sProcedureName.StartsWith("spWWF_") && !sProcedureName.StartsWith("spSCHEDULERS_") && !sProcedureName.StartsWith("spSYSTEM_") && sProcedureName != "spWORKFLOWS_UpdateLastRun" )
					sb.AppendLine(TabSpace(nIndent) + "Sql.Trace(cmd);");
				sb.AppendLine(TabSpace(nIndent) + "cmd.ExecuteNonQuery();");
				for ( int j = 0 ; j < colRows.Count; j++ )
				{
					DataRow row = colRows[j];
					string sName      = Sql.ToString (row["ColumnName"]);
					string sBareName  = sName.Replace("@", "");
					string sCsType    = Sql.ToString (row["CsType"    ]);
					string sCsPrefix  = Sql.ToString (row["CsPrefix"  ]);
					bool   bIsOutput  = Sql.ToBoolean(row["isoutparam"]);
					if ( bIsOutput )
					{
						nSpace   = nColumnAlignmentSize - sBareName.Length;
						nSpace   = Math.Max(2, nSpace);
						int nSpaceCsPrefix = 3 + nColumnAlignmentSize - sBareName.Length - sCsPrefix.Length;
						nSpaceCsPrefix = Math.Max(2, nSpaceCsPrefix);
						// 04/25/2008 Paul.  ansistring needs to be treated like a string. 
						sCsType = (sCsType == "ansistring") ? "string" : sCsType;
						switch ( sCsType )
						{
							case "string":
								sb.AppendLine(TabSpace(nIndent) + sCsPrefix + sBareName + " = Sql.ToString(par" + sBareName + ".Value);");
								break;
							case "DateTime":
								sb.AppendLine(TabSpace(nIndent) + sCsPrefix + sBareName + " = Sql.ToDateTime(par" + sBareName + ".Value);");
								break;
							case "Guid":
								sb.AppendLine(TabSpace(nIndent) + sCsPrefix + sBareName + " = Sql.ToGuid(par" + sBareName + ".Value);");
								break;
							case "Int32":
							case "short":
								sb.AppendLine(TabSpace(nIndent) + sCsPrefix + sBareName + " = Sql.ToInteger(par" + sBareName + ".Value);");
								break;
							case "float":
								sb.AppendLine(TabSpace(nIndent) + sCsPrefix + sBareName + " = Sql.ToFloat(par" + sBareName + ".Value);");
								break;
							case "decimal":
								sb.AppendLine(TabSpace(nIndent) + sCsPrefix + sBareName + " = Sql.ToDecimal(par" + sBareName + ".Value);");
								break;
							case "bool":
								sb.AppendLine(TabSpace(nIndent) + sCsPrefix + sBareName + " = Sql.ToBoolean(par" + sBareName + ".Value);");
								break;
							case "byte[]":
								sb.AppendLine(TabSpace(nIndent) + sCsPrefix + sBareName + " = Sql.ToBinary(par" + sBareName + ".Value);");
								break;
							default:
								sb.AppendLine(TabSpace(nIndent) + sCsPrefix + sBareName + " = par" + sBareName + ".Value" + ";");
								break;
						}
					}
				}
			}
			else
			{
				// 02/20/2006 Paul.  Need to set the direction. 
				for ( int j = 0 ; j < colRows.Count; j++ )
				{
					DataRow row = colRows[j];
					string sName      = Sql.ToString (row["ColumnName"]);
					string sBareName  = sName.Replace("@", "");
					bool   bIsOutput  = Sql.ToBoolean(row["isoutparam"]);
					if ( bIsOutput )
						sb.AppendLine(TabSpace(nIndent) + "par" + sBareName + ".Direction = ParameterDirection.InputOutput;");
				}
			}

			if ( !bCreateCommand )
			{
				if ( !bTransaction )
				{
					nIndent--;
					sb.AppendLine(TabSpace(nIndent) + "}");

					sb.AppendLine(TabSpace(nIndent) + "trn.Commit();");
					nIndent--;
					sb.AppendLine(TabSpace(nIndent) + "}");
					sb.AppendLine(TabSpace(nIndent) + "catch");
					sb.AppendLine(TabSpace(nIndent) + "{");
					nIndent++;
					sb.AppendLine(TabSpace(nIndent) + "trn.Rollback();");
					// 12/25/2008 Paul.  Re-throw the original exception so as to retain the call stack. 
					// The difference between these two variations is subtle but important. With the first example, the higher level
					// caller isn’t going to get all the information about the original error. The call stack in the exception is replaced
					// with a new call stack that originates at the “throw ex” statement – which is not what we want to record. The
					// second example is the only one that actually re-throws the original exception, preserving the stack trace where
					// the original error occurred.
					//sb.AppendLine(TabSpace(nIndent) + "throw(new Exception(ex.Message, ex.InnerException));");
					sb.AppendLine(TabSpace(nIndent) + "throw;");
					nIndent--;
					sb.AppendLine(TabSpace(nIndent) + "}");
					nIndent--;
					sb.AppendLine(TabSpace(nIndent) + "}");
				}
				nIndent--;
				sb.AppendLine(TabSpace(nIndent) + "}");
			}
			else
			{
				sb.AppendLine(TabSpace(nIndent) + "return cmd;");
			}
			nIndent--;
			sb.AppendLine(TabSpace(nIndent) + "}");
			sb.AppendLine(TabSpace(nIndent) + "#endregion");
			sb.AppendLine();
		}

		private void GenerateModule(string sDISPLAY_NAME, string sMODULE_NAME, string sTABLE_NAME, bool bTAB_ENABLED, bool bMOBILE_ENABLED, bool bCUSTOM_ENABLED, bool bREPORT_ENABLED, bool bIMPORT_ENABLED, bool bREST_ENABLED, bool bIS_ADMIN, bool bINCLUDE_ASSIGNED_USER_ID, bool bINCLUDE_TEAM_ID, bool bOVERWRITE_EXISTING, bool bCREATE_CODE_BEHIND, bool bREACT_ONLY, DataTable dtFields, List<string> lstRelationships, StringBuilder sbProgress)
		{
					string sDISPLAY_NAME_SINGULAR    = sDISPLAY_NAME            ;
					string sMODULE_NAME_SINGULAR     = sMODULE_NAME             ;
					string sTABLE_NAME_SINGULAR      = sTABLE_NAME              ;

					if ( sDISPLAY_NAME_SINGULAR.ToLower().EndsWith("ies") )
						sDISPLAY_NAME_SINGULAR = sDISPLAY_NAME_SINGULAR.Substring(0, sDISPLAY_NAME_SINGULAR.Length-3) + "y";
					else if ( sDISPLAY_NAME_SINGULAR.ToLower().EndsWith("s") )
						sDISPLAY_NAME_SINGULAR = sDISPLAY_NAME_SINGULAR.Substring(0, sDISPLAY_NAME_SINGULAR.Length-1);
					if ( sMODULE_NAME_SINGULAR.ToLower().EndsWith("ies") )
						sMODULE_NAME_SINGULAR = sMODULE_NAME_SINGULAR.Substring(0, sMODULE_NAME_SINGULAR.Length-3) + "y";
					else if ( sMODULE_NAME_SINGULAR.ToLower().EndsWith("s") )
						sMODULE_NAME_SINGULAR = sMODULE_NAME_SINGULAR.Substring(0, sMODULE_NAME_SINGULAR.Length-1);
					if ( sTABLE_NAME_SINGULAR.ToLower().EndsWith("ies") )
						sTABLE_NAME_SINGULAR = sTABLE_NAME_SINGULAR.Substring(0, sTABLE_NAME_SINGULAR.Length-3) + "Y";
					else if ( sTABLE_NAME_SINGULAR.ToLower().EndsWith("s") )
						sTABLE_NAME_SINGULAR = sTABLE_NAME_SINGULAR.Substring(0, sTABLE_NAME_SINGULAR.Length-1);

					// 03/08/2010 Paul.  Allow user to select Live deployment, but we still prefer the code-behind method. 
					string sWebTemplatesPath = Path.Combine(hostingEnvironment.ContentRootPath, "Administration\\ModuleBuilder\\WebTemplates");
					if ( !bCREATE_CODE_BEHIND )
						sWebTemplatesPath = Path.Combine(hostingEnvironment.ContentRootPath, "Administration\\ModuleBuilder\\WebTemplatesLive");
					string sSqlTemplatesPath = Path.Combine(hostingEnvironment.ContentRootPath, "Administration\\ModuleBuilder\\SqlTemplates");
					// 09/12/2009 Paul.  If this is an admin module, then place in the Administration namespace. 
					string sWebModulePath    = Path.Combine(hostingEnvironment.ContentRootPath, (bIS_ADMIN ? "Administration\\" : "") + sMODULE_NAME);
					// 06/04/2017 Paul.  Change to SQL Scripts Custom and Build Custom.bat. 
					string sSqlScriptsPath   = Path.Combine(hostingEnvironment.ContentRootPath, "..\\SQL Scripts Custom");

					try
					{
						if ( !Directory.Exists(sWebModulePath) )
						{
							Directory.CreateDirectory(sWebModulePath);
						}
					}
					catch(Exception ex)
					{
						sbProgress.AppendLine("<font class=error>Failed to create " + sWebModulePath + ":" + ex.Message + "</font><br>");
					}
					try
					{
						if ( !Directory.Exists(sSqlScriptsPath) )
						{
							Directory.CreateDirectory(sSqlScriptsPath);
						}
						// 03/07/2011 Paul.  If the Tables folder does not exist, then rebuild the batch file. 
						if ( System.IO.File.Exists(Path.Combine(sSqlScriptsPath, "Build Custom.bat")) && !Directory.Exists(Path.Combine(sSqlScriptsPath, "Tables")) )
						{
							System.IO.File.Delete(Path.Combine(sSqlScriptsPath, "Build Custom.bat"));
						}
						// 09/23/2009 Paul.  If we are creating the SQL Scripts folder, then also add the comment files and the build file. 
						if ( !System.IO.File.Exists(Path.Combine(sSqlScriptsPath, "Build Custom.bat")) )
						{
							try
							{
								// 06/04/2017 Paul.  Must create folders first. 
								if ( !Directory.Exists(Path.Combine(sSqlScriptsPath, "BaseTables" )) ) Directory.CreateDirectory(Path.Combine(sSqlScriptsPath, "BaseTables" ));
								if ( !Directory.Exists(Path.Combine(sSqlScriptsPath, "Tables"     )) ) Directory.CreateDirectory(Path.Combine(sSqlScriptsPath, "Tables"     ));
								if ( !Directory.Exists(Path.Combine(sSqlScriptsPath, "Data"       )) ) Directory.CreateDirectory(Path.Combine(sSqlScriptsPath, "Data"       ));
								if ( !Directory.Exists(Path.Combine(sSqlScriptsPath, "Procedures" )) ) Directory.CreateDirectory(Path.Combine(sSqlScriptsPath, "Procedures" ));
								if ( !Directory.Exists(Path.Combine(sSqlScriptsPath, "Triggers"   )) ) Directory.CreateDirectory(Path.Combine(sSqlScriptsPath, "Triggers"   ));
								if ( !Directory.Exists(Path.Combine(sSqlScriptsPath, "Terminology")) ) Directory.CreateDirectory(Path.Combine(sSqlScriptsPath, "Terminology"));
								if ( !Directory.Exists(Path.Combine(sSqlScriptsPath, "Views"      )) ) Directory.CreateDirectory(Path.Combine(sSqlScriptsPath, "Views"      ));
								
								System.IO.File.WriteAllText(Path.Combine(sSqlScriptsPath, "BaseTables\\_Comment.0.sql" ), "\r\n");
								System.IO.File.WriteAllText(Path.Combine(sSqlScriptsPath, "BaseTables\\_Comment.1.sql" ), "\r\n");
								System.IO.File.WriteAllText(Path.Combine(sSqlScriptsPath, "BaseTables\\_Comment.2.sql" ), "\r\n");
								System.IO.File.WriteAllText(Path.Combine(sSqlScriptsPath, "Tables\\_Comment.0.sql"     ), "\r\n");
								System.IO.File.WriteAllText(Path.Combine(sSqlScriptsPath, "Tables\\_Comment.1.sql"     ), "\r\n");
								System.IO.File.WriteAllText(Path.Combine(sSqlScriptsPath, "Tables\\_Comment.2.sql"     ), "\r\n");
								System.IO.File.WriteAllText(Path.Combine(sSqlScriptsPath, "Data\\_Comment.0.sql"       ), "\r\n");
								System.IO.File.WriteAllText(Path.Combine(sSqlScriptsPath, "Data\\_Comment.1.sql"       ), "\r\n");
								System.IO.File.WriteAllText(Path.Combine(sSqlScriptsPath, "Data\\_Comment.2.sql"       ), "\r\n");
								System.IO.File.WriteAllText(Path.Combine(sSqlScriptsPath, "Procedures\\_Comment.0.sql" ), "\r\n");
								System.IO.File.WriteAllText(Path.Combine(sSqlScriptsPath, "Procedures\\_Comment.1.sql" ), "\r\n");
								System.IO.File.WriteAllText(Path.Combine(sSqlScriptsPath, "Procedures\\_Comment.2.sql" ), "\r\n");
								// 09/26/2011 Paul.  Update triggers for auditing. 
								System.IO.File.WriteAllText(Path.Combine(sSqlScriptsPath, "Triggers\\_Comment.0.sql"   ), "\r\n");
								System.IO.File.WriteAllText(Path.Combine(sSqlScriptsPath, "Triggers\\_Comment.1.sql"   ), "\r\n");
								System.IO.File.WriteAllText(Path.Combine(sSqlScriptsPath, "Triggers\\_Comment.2.sql"   ), "\r\n");
								System.IO.File.WriteAllText(Path.Combine(sSqlScriptsPath, "Terminology\\_Comment.0.sql"), "\r\n");
								System.IO.File.WriteAllText(Path.Combine(sSqlScriptsPath, "Terminology\\_Comment.1.sql"), "\r\n");
								System.IO.File.WriteAllText(Path.Combine(sSqlScriptsPath, "Terminology\\_Comment.2.sql"), "\r\n");
								System.IO.File.WriteAllText(Path.Combine(sSqlScriptsPath, "Views\\_Comment.0.sql"      ), "\r\n");
								System.IO.File.WriteAllText(Path.Combine(sSqlScriptsPath, "Views\\_Comment.1.sql"      ), "\r\n");
								System.IO.File.WriteAllText(Path.Combine(sSqlScriptsPath, "Views\\_Comment.2.sql"      ), "\r\n");
								
								StringBuilder sbBuild = new StringBuilder();
								sbBuild.AppendLine("del BaseTables.sql");
								sbBuild.AppendLine("del Tables.sql");
								sbBuild.AppendLine("del Views.sql");
								sbBuild.AppendLine("del Procedures.sql");
								sbBuild.AppendLine("del Triggers.sql");
								sbBuild.AppendLine("del Data.sql");
								sbBuild.AppendLine("del Terminology.sql");
								sbBuild.AppendLine("");
								sbBuild.AppendLine("copy BaseTables\\*.0.sql         + BaseTables\\*.1.sql         + BaseTables\\*.2.sql       BaseTables.sql");
								sbBuild.AppendLine("copy Tables\\*.0.sql             + Tables\\*.1.sql             + Tables\\*.2.sql           Tables.sql");
								sbBuild.AppendLine("copy Views\\*.0.sql              + Views\\*.1.sql              + Views\\*.2.sql            Views.sql");
								sbBuild.AppendLine("copy Procedures\\*.0.sql         + Procedures\\*.1.sql         + Procedures\\*.2.sql       Procedures.sql");
								// 09/26/2011 Paul.  Update triggers for auditing. 
								sbBuild.AppendLine("copy Triggers\\*.0.sql           + Triggers\\*.1.sql           + Triggers\\*.2.sql         Triggers.sql");
								sbBuild.AppendLine("copy Data\\*.0.sql               + Data\\*.1.sql               + Data\\*.2.sql             Data.sql");
								sbBuild.AppendLine("copy Terminology\\*.0.sql        + Terminology\\*.1.sql        + Terminology\\*.2.sql      Terminology.sql");
								sbBuild.AppendLine("");
								sbBuild.AppendLine("Copy BaseTables.sql + Tables.sql + Views.sql + Procedures.sql + Triggers.sql + Data.sql + Terminology.sql \"Build Custom.sql\"");
								sbBuild.AppendLine("");
								System.IO.File.WriteAllText(Path.Combine(sSqlScriptsPath, "Build Custom.bat"), sbBuild.ToString());
							}
							catch
							{
							}
						}
					}
					catch(Exception ex)
					{
						sbProgress.AppendLine("<font class=error>Failed to create " + sSqlScriptsPath + ":" + ex.Message + "</font><br>");
					}

					StringBuilder sbCreateTableFields            = new StringBuilder();
					StringBuilder sbCreateTableIndexes           = new StringBuilder();
					StringBuilder sbCreateViewFields             = new StringBuilder();
					StringBuilder sbCreateViewJoins              = new StringBuilder();
					StringBuilder sbCreateProcedureParameters    = new StringBuilder();
					StringBuilder sbCreateProcedureInsertInto    = new StringBuilder();
					StringBuilder sbCreateProcedureInsertValues  = new StringBuilder();
					StringBuilder sbCreateProcedureUpdate        = new StringBuilder();
					StringBuilder sbCreateProcedureNormalizeTeams= new StringBuilder();
					StringBuilder sbCreateProcedureUpdateTeams   = new StringBuilder();
					// 03/07/2011 Paul.  We need the ability to alter a table to add new fields, just in case Assigned User and Team Management are enabled during re-generation. 
					StringBuilder sbAlterTableFields             = new StringBuilder();
					StringBuilder sbCallUpdateProcedure          = new StringBuilder();
					StringBuilder sbMassUpdateProcedureFields    = new StringBuilder();
					StringBuilder sbMassUpdateProcedureSets      = new StringBuilder();
					StringBuilder sbMassUpdateTeamNormalize      = new StringBuilder();
					StringBuilder sbMassUpdateTeamAdd            = new StringBuilder();
					StringBuilder sbMassUpdateTeamUpdate         = new StringBuilder();
					StringBuilder sbMergeProcedureUpdates        = new StringBuilder();
					StringBuilder sbModuleGridViewData           = new StringBuilder();
					StringBuilder sbModuleGridViewPopup          = new StringBuilder();
					StringBuilder sbModuleDetailViewData         = new StringBuilder();
					StringBuilder sbModuleEditViewData           = new StringBuilder();
					StringBuilder sbModuleEditViewSearchBasic    = new StringBuilder();
					StringBuilder sbModuleEditViewSearchAdvanced = new StringBuilder();
					StringBuilder sbModuleEditViewSearchPopup    = new StringBuilder();
					StringBuilder sbModuleTerminology            = new StringBuilder();
					// 08/08/2013 Paul.  Add delete and undelete of relationships. 
					StringBuilder sbDeleteProcedureUpdates       = new StringBuilder();
					StringBuilder sbUndeleteProcedureUpdates     = new StringBuilder();
					// 03/07/2010 Paul.  GridViewIndex will start at 1 to make room for the checkbox. 
					// 03/05/2011 Paul.  Start at 2 to make room for edit button. 
					int nGridViewIndex               = 2;
					int nGridViewPopupIndex          = 1;
					int nGridViewMAX                 = 3;
					int nDetailViewIndex             = 0;
					int nEditViewIndex               = 0;
					int nEditViewSearchBasicIndex    = 0;
					int nEditViewSearchAdvancedIndex = 0;
					int nEditViewSearchPopupIndex    = 0;
					int nEditViewSearchBasicMAX      = 1;

					// 03/06/2010 Paul.  Now that we have included NewRecord logic, we cannot assume that ASSIGNED_USER_ID exists on the form. 
					if ( bINCLUDE_ASSIGNED_USER_ID )
					{
						// 02/09/2015 Paul.  Need to prevent ambiguous reference with System.Web.DynamicData.DynamicControl when not using code-behind. 
						sbCallUpdateProcedure.AppendLine("									Guid gASSIGNED_USER_ID = new SplendidCRM.DynamicControl(this, rowCurrent, \"ASSIGNED_USER_ID\").ID;");
						sbCallUpdateProcedure.AppendLine("									if ( Sql.IsEmptyGuid(gASSIGNED_USER_ID) )");
						sbCallUpdateProcedure.AppendLine("										gASSIGNED_USER_ID = Security.USER_ID;");
					}
					// 03/06/2010 Paul.  Now that we have included NewRecord logic, we cannot assume that TEAM_ID exists on the form. 
					if ( bINCLUDE_TEAM_ID )
					{
						// 02/09/2015 Paul.  Need to prevent ambiguous reference with System.Web.DynamicData.DynamicControl when not using code-behind. 
						sbCallUpdateProcedure.AppendLine("									Guid gTEAM_ID          = new SplendidCRM.DynamicControl(this, rowCurrent, \"TEAM_ID\"         ).ID;");
						sbCallUpdateProcedure.AppendLine("									if ( Sql.IsEmptyGuid(gTEAM_ID) )");
						sbCallUpdateProcedure.AppendLine("										gTEAM_ID = Security.TEAM_ID;");
					}
					sbCallUpdateProcedure.AppendLine("									SqlProcs.sp" + sTABLE_NAME +"_Update");
					sbCallUpdateProcedure.AppendLine("										( ref gID");
					if ( bINCLUDE_ASSIGNED_USER_ID )
					{
						sbCreateTableFields          .AppendLine("		, ASSIGNED_USER_ID                   uniqueidentifier null");
						sbCreateTableIndexes         .AppendLine("	create index IDX_" + sTABLE_NAME + "_ASSIGNED_USER_ID on dbo." + sTABLE_NAME + " (ASSIGNED_USER_ID, DELETED, ID)");
						
						// 03/07/2011 Paul.  We need the ability to alter a table to add new fields, just in case Assigned User and Team Management are enabled during re-generation. 
						sbAlterTableFields           .AppendLine("if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = '" + sTABLE_NAME + "' and COLUMN_NAME = 'ASSIGNED_USER_ID') begin -- then");
						sbAlterTableFields           .AppendLine("	print 'alter table " + sTABLE_NAME + " add ASSIGNED_USER_ID uniqueidentifier null';");
						sbAlterTableFields           .AppendLine("	alter table " + sTABLE_NAME + " add ASSIGNED_USER_ID uniqueidentifier null;");
						sbAlterTableFields           .AppendLine("	create index IDX_" + sTABLE_NAME + "_ASSIGNED_USER_ID on dbo." + sTABLE_NAME + " (ASSIGNED_USER_ID, DELETED, ID)");
						sbAlterTableFields           .AppendLine("end -- if;");
						sbAlterTableFields           .AppendLine("");
						
						sbCreateProcedureParameters  .AppendLine("	, @ASSIGNED_USER_ID                   uniqueidentifier");
						sbCreateProcedureInsertInto  .AppendLine("			, ASSIGNED_USER_ID                   ");
						sbCreateProcedureInsertValues.AppendLine("			, @ASSIGNED_USER_ID                   ");
						sbCreateProcedureUpdate      .AppendLine("		     , ASSIGNED_USER_ID                     = @ASSIGNED_USER_ID                   ");
						
						sbMassUpdateProcedureFields  .AppendLine("	, @ASSIGNED_USER_ID  uniqueidentifier");
						sbMassUpdateProcedureSets    .AppendLine("			     , ASSIGNED_USER_ID  = isnull(@ASSIGNED_USER_ID, ASSIGNED_USER_ID)");
						
						sbCallUpdateProcedure.AppendLine("										, gASSIGNED_USER_ID");
					}
					if ( bINCLUDE_TEAM_ID )
					{
						sbCreateTableFields            .AppendLine("		, TEAM_ID                            uniqueidentifier null");
						// 09/23/2009 Paul.  TEAM_SET_ID was missing. 
						sbCreateTableFields            .AppendLine("		, TEAM_SET_ID                        uniqueidentifier null");
						// 03/07/2011 Paul.  Just in case ASSIGNED_USER_ID is not set with TEAM_ID. 
						if ( bINCLUDE_ASSIGNED_USER_ID )
						{
							sbCreateTableIndexes           .AppendLine("	create index IDX_" + sTABLE_NAME + "_TEAM_ID          on dbo." + sTABLE_NAME + " (TEAM_ID, ASSIGNED_USER_ID, DELETED, ID)");
							sbCreateTableIndexes           .AppendLine("	create index IDX_" + sTABLE_NAME + "_TEAM_SET_ID      on dbo." + sTABLE_NAME + " (TEAM_SET_ID, ASSIGNED_USER_ID, DELETED, ID)");
						}
						else
						{
							sbCreateTableIndexes           .AppendLine("	create index IDX_" + sTABLE_NAME + "_TEAM_ID          on dbo." + sTABLE_NAME + " (TEAM_ID, DELETED, ID)");
							sbCreateTableIndexes           .AppendLine("	create index IDX_" + sTABLE_NAME + "_TEAM_SET_ID      on dbo." + sTABLE_NAME + " (TEAM_SET_ID, DELETED, ID)");
						}
						
						// 03/07/2011 Paul.  We need the ability to alter a table to add new fields, just in case Assigned User and Team Management are enabled during re-generation. 
						sbAlterTableFields             .AppendLine("if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = '" + sTABLE_NAME + "' and COLUMN_NAME = 'TEAM_ID') begin -- then");
						sbAlterTableFields             .AppendLine("	print 'alter table " + sTABLE_NAME + " add TEAM_ID uniqueidentifier null';");
						sbAlterTableFields             .AppendLine("	alter table " + sTABLE_NAME + " add TEAM_ID uniqueidentifier null;");
						if ( bINCLUDE_ASSIGNED_USER_ID )
							sbAlterTableFields             .AppendLine("	create index IDX_" + sTABLE_NAME + "_TEAM_ID          on dbo." + sTABLE_NAME + " (TEAM_ID, ASSIGNED_USER_ID, DELETED, ID)");
						else
							sbAlterTableFields             .AppendLine("	create index IDX_" + sTABLE_NAME + "_TEAM_ID          on dbo." + sTABLE_NAME + " (TEAM_ID, DELETED, ID)");
						sbAlterTableFields             .AppendLine("end -- if;");
						sbAlterTableFields             .AppendLine("");
						sbAlterTableFields             .AppendLine("if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = '" + sTABLE_NAME + "' and COLUMN_NAME = 'TEAM_SET_ID') begin -- then");
						sbAlterTableFields             .AppendLine("	print 'alter table " + sTABLE_NAME + " add TEAM_SET_ID uniqueidentifier null';");
						sbAlterTableFields             .AppendLine("	alter table " + sTABLE_NAME + " add TEAM_SET_ID uniqueidentifier null;");
						if ( bINCLUDE_ASSIGNED_USER_ID )
							sbAlterTableFields             .AppendLine("	create index IDX_" + sTABLE_NAME + "_TEAM_SET_ID      on dbo." + sTABLE_NAME + " (TEAM_SET_ID, ASSIGNED_USER_ID, DELETED, ID)");
						else
							sbAlterTableFields             .AppendLine("	create index IDX_" + sTABLE_NAME + "_TEAM_SET_ID      on dbo." + sTABLE_NAME + " (TEAM_SET_ID, DELETED, ID)");
						sbAlterTableFields             .AppendLine("end -- if;");
						sbAlterTableFields             .AppendLine("");
						
						sbCreateProcedureParameters    .AppendLine("	, @TEAM_ID                            uniqueidentifier");
						sbCreateProcedureParameters    .AppendLine("	, @TEAM_SET_LIST                      varchar(8000)");
						sbCreateProcedureInsertInto    .AppendLine("			, TEAM_ID                            ");
						sbCreateProcedureInsertInto    .AppendLine("			, TEAM_SET_ID                        ");
						sbCreateProcedureInsertValues  .AppendLine("			, @TEAM_ID                            ");
						sbCreateProcedureInsertValues  .AppendLine("			, @TEAM_SET_ID                        ");
						sbCreateProcedureUpdate        .AppendLine("		     , TEAM_ID                              = @TEAM_ID                            ");
						sbCreateProcedureUpdate        .AppendLine("		     , TEAM_SET_ID                          = @TEAM_SET_ID                        ");
						
						sbCreateProcedureNormalizeTeams.AppendLine("	declare @TEAM_SET_ID         uniqueidentifier;");
						sbCreateProcedureNormalizeTeams.AppendLine("	exec dbo.spTEAM_SETS_NormalizeSet @TEAM_SET_ID out, @MODIFIED_USER_ID, @TEAM_ID, @TEAM_SET_LIST;");
						
						sbMassUpdateProcedureFields    .AppendLine("	, @TEAM_ID           uniqueidentifier");
						sbMassUpdateProcedureFields    .AppendLine("	, @TEAM_SET_LIST     varchar(8000)");
						sbMassUpdateProcedureFields    .AppendLine("	, @TEAM_SET_ADD      bit");
						sbMassUpdateProcedureSets      .AppendLine("			     , TEAM_ID           = isnull(@TEAM_ID         , TEAM_ID         )");
						sbMassUpdateProcedureSets      .AppendLine("			     , TEAM_SET_ID       = isnull(@TEAM_SET_ID     , TEAM_SET_ID     )");
						
						// 09/16/2009 Paul.  Needed to define @OLD_SET_ID. 
						sbMassUpdateTeamNormalize      .AppendLine("	declare @TEAM_SET_ID  uniqueidentifier;");
						sbMassUpdateTeamNormalize      .AppendLine("	declare @OLD_SET_ID   uniqueidentifier;");
						sbMassUpdateTeamNormalize      .AppendLine("");
						sbMassUpdateTeamNormalize      .AppendLine("	exec dbo.spTEAM_SETS_NormalizeSet @TEAM_SET_ID out, @MODIFIED_USER_ID, @TEAM_ID, @TEAM_SET_LIST;");
						
						sbMassUpdateTeamAdd            .AppendLine("		if @TEAM_SET_ADD = 1 and @TEAM_SET_ID is not null begin -- then");
						sbMassUpdateTeamAdd            .AppendLine("				select @OLD_SET_ID = TEAM_SET_ID");
						sbMassUpdateTeamAdd            .AppendLine("				     , @TEAM_ID    = isnull(@TEAM_ID, TEAM_ID)");
						sbMassUpdateTeamAdd            .AppendLine("				  from " + sTABLE_NAME);
						sbMassUpdateTeamAdd            .AppendLine("				 where ID                = @ID");
						sbMassUpdateTeamAdd            .AppendLine("				   and DELETED           = 0;");
						sbMassUpdateTeamAdd            .AppendLine("			if @OLD_SET_ID is not null begin -- then");
						sbMassUpdateTeamAdd            .AppendLine("				exec dbo.spTEAM_SETS_AddSet @TEAM_SET_ID out, @MODIFIED_USER_ID, @OLD_SET_ID, @TEAM_ID, @TEAM_SET_ID;");
						sbMassUpdateTeamAdd            .AppendLine("			end -- if;");
						sbMassUpdateTeamAdd            .AppendLine("		end -- if;");
						
						// 08/31/2009 Paul.  We are no longer going to use separate team relationship tables. 
						//sbMassUpdateTeamUpdate         .AppendLine("		if @TEAM_SET_ID is not null begin -- then");
						//sbMassUpdateTeamUpdate         .AppendLine("			exec dbo.sp" + sTABLE_NAME + "_TEAMS_Update @ID, @MODIFIED_USER_ID, @TEAM_SET_ID;");
						//sbMassUpdateTeamUpdate         .AppendLine("		end -- if;");
						
						sbCallUpdateProcedure          .AppendLine("										, gTEAM_ID");
						// 02/09/2015 Paul.  Need to prevent ambiguous reference with System.Web.DynamicData.DynamicControl when not using code-behind. 
						sbCallUpdateProcedure          .AppendLine("										, new SplendidCRM.DynamicControl(this, rowCurrent, \"TEAM_SET_LIST\"                      ).Text");
					}
					// 02/08/2022 Paul.  We need to combine FIRST_NAME + LAST_NAME if they exist. 
					DataView vwNAMES = new DataView(dtFields);
					vwNAMES.RowFilter = "FIELD_NAME = 'NAME'";
					if ( vwNAMES.Count == 0 )
					{
						vwNAMES.RowFilter = "FIELD_NAME = 'FIRST_NAME'";
						if ( vwNAMES.Count == 1 )
						{
							vwNAMES.RowFilter = "FIELD_NAME = 'LAST_NAME'";
							if ( vwNAMES.Count == 1 )
							{
								sbCreateViewFields            .AppendLine("     , dbo.fnFullName(" + sTABLE_NAME + ".FIRST_NAME, " + sTABLE_NAME + ".LAST_NAME) as NAME");
							}
						}
					}
					string sFIRST_TEXT_FIELD = String.Empty;
					foreach ( DataRow row in dtFields.Rows )
					{
						string sFIELD_NAME = Sql.ToString (row["FIELD_NAME"]).ToUpper();
						string sEDIT_LABEL = Sql.ToString (row["EDIT_LABEL"]);
						string sLIST_LABEL = Sql.ToString (row["LIST_LABEL"]);
						string sDATA_TYPE  = Sql.ToString (row["DATA_TYPE" ]);
						int    nMAX_SIZE   = Sql.ToInteger(row["MAX_SIZE"  ]);
						bool   bREQUIRED   = Sql.ToBoolean(row["REQUIRED"  ]);
						// 09/16/2009 Paul.  DATE_MODIFIED_UTC is a new common field used to sync. 
						if (  String.IsNullOrEmpty(sFIELD_NAME)
						   || sFIELD_NAME == "ID"              
						   || sFIELD_NAME == "DELETED"         
						   || sFIELD_NAME == "CREATED_BY"      
						   || sFIELD_NAME == "DATE_ENTERED"    
						   || sFIELD_NAME == "MODIFIED_USER_ID"
						   || sFIELD_NAME == "DATE_MODIFIED"   
						   || sFIELD_NAME == "DATE_MODIFIED_UTC"
						   || (sFIELD_NAME == "ASSIGNED_USER_ID" && bINCLUDE_ASSIGNED_USER_ID)
						   || (sFIELD_NAME == "TEAM_ID"          && bINCLUDE_TEAM_ID         )
						   || (sFIELD_NAME == "TEAM_SET_ID"      && bINCLUDE_TEAM_ID         )
						   )
						{
							continue;
						}
						string sSQL_DATA_TYPE = String.Empty;
						switch ( sDATA_TYPE )
						{
							case "Text"     :  sSQL_DATA_TYPE = "nvarchar(" + nMAX_SIZE.ToString() + ")";  break;
							// 08/17/2017 Paul.  We should be using nvarchar(max) instead of ntext. 
							case "Text Area":  sSQL_DATA_TYPE = "nvarchar(max)"   ;  break;
							case "Integer"  :  sSQL_DATA_TYPE = "int"             ;  break;
							case "bigint"   :  sSQL_DATA_TYPE = "bigint"          ;  break;
							case "Decimal"  :  sSQL_DATA_TYPE = "float"           ;  break;
							case "Money"    :  sSQL_DATA_TYPE = "money"           ;  break;
							case "Checkbox" :  sSQL_DATA_TYPE = "bit"             ;  break;
							case "Date"     :  sSQL_DATA_TYPE = "datetime"        ;  break;
							case "Dropdown" :  sSQL_DATA_TYPE = "nvarchar(50)"    ;  break;
							case "Guid"     :  sSQL_DATA_TYPE = "uniqueidentifier";  break;
						}
						sbCreateTableFields           .AppendLine("		, " + sFIELD_NAME + Strings.Space(35 - sFIELD_NAME.Length) + sSQL_DATA_TYPE + " " + (bREQUIRED ? "not null" : "null") );
						
						// 03/07/2011 Paul.  We need the ability to alter a table to add new fields, just in case Assigned User and Team Management are enabled during re-generation. 
						sbAlterTableFields           .AppendLine("if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = '" + sTABLE_NAME + "' and COLUMN_NAME = '" + sFIELD_NAME + "') begin -- then");
						sbAlterTableFields           .AppendLine("	print 'alter table " + sTABLE_NAME + " add " + sFIELD_NAME + " " + sSQL_DATA_TYPE + " null';");
						sbAlterTableFields           .AppendLine("	alter table " + sTABLE_NAME + " add " + sFIELD_NAME + " " + sSQL_DATA_TYPE + " null;");
						sbAlterTableFields           .AppendLine("end -- if;");
						sbAlterTableFields           .AppendLine("");

						if ( sFIELD_NAME == "NAME" || sFIELD_NAME == "TITLE" )
							sbCreateTableIndexes         .AppendLine("	create index IDX_" + sTABLE_NAME + "_" + sFIELD_NAME + "  on dbo." + sTABLE_NAME + " (" + sFIELD_NAME + ", DELETED, ID)");
						sbCreateViewFields            .AppendLine("     , " + sTABLE_NAME + "." + sFIELD_NAME);
						sbCreateProcedureParameters   .AppendLine("	, @" + sFIELD_NAME + Strings.Space(35 - sFIELD_NAME.Length) + sSQL_DATA_TYPE);
						sbCreateProcedureInsertInto   .AppendLine("			, "  + sFIELD_NAME + Strings.Space(35 - sFIELD_NAME.Length));
						sbCreateProcedureInsertValues .AppendLine("			, @" + sFIELD_NAME + Strings.Space(35 - sFIELD_NAME.Length));
						sbCreateProcedureUpdate       .AppendLine("		     , " + sFIELD_NAME + Strings.Space(35 - sFIELD_NAME.Length) + "  = @" + sFIELD_NAME + Strings.Space(35 - sFIELD_NAME.Length) + "");

						if ( Sql.IsEmptyString(sEDIT_LABEL) )
							sLIST_LABEL = CamelCase(sFIELD_NAME);
						if ( Sql.IsEmptyString(sEDIT_LABEL) )
							sEDIT_LABEL = sLIST_LABEL + ":";
						sbModuleTerminology.AppendLine("exec dbo.spTERMINOLOGY_InsertOnly 'LBL_"      + sFIELD_NAME + "'" + Strings.Space(50 - sFIELD_NAME.Length) + ", 'en-US', '" + sMODULE_NAME + "', null, null, '" + sEDIT_LABEL + "';");
						sbModuleTerminology.AppendLine("exec dbo.spTERMINOLOGY_InsertOnly 'LBL_LIST_" + sFIELD_NAME + "'" + Strings.Space(45 - sFIELD_NAME.Length) + ", 'en-US', '" + sMODULE_NAME + "', null, null, '" + sLIST_LABEL + "';");

						sbModuleDetailViewData.AppendLine("	exec dbo.spDETAILVIEWS_FIELDS_InsBound     '" + sMODULE_NAME + ".DetailView', " + nDetailViewIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', '{0}', null;");
						nDetailViewIndex++;
						switch ( sDATA_TYPE )
						{
							case "Text":
								sbModuleEditViewData          .AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsBound       '" + sMODULE_NAME + ".EditView', " + nEditViewIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0).ToString() + ", 1, " + nMAX_SIZE.ToString() + ", 35, null;");
								// 09/04/2009 Paul.  Add Auto-Complete to the NAME search field. 
								if ( sFIELD_NAME == "NAME" )
									sbModuleEditViewSearchAdvanced.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsAutoComplete '" + sMODULE_NAME + ".SearchAdvanced', " + nEditViewSearchAdvancedIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0).ToString() + ", 1, " + nMAX_SIZE.ToString() + ", 35, '" + sMODULE_NAME + "', null;");
								else
									sbModuleEditViewSearchAdvanced.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsBound        '" + sMODULE_NAME + ".SearchAdvanced', " + nEditViewSearchAdvancedIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0).ToString() + ", 1, " + nMAX_SIZE.ToString() + ", 35, null;");
								nEditViewIndex++;
								nEditViewSearchAdvancedIndex++;
								if ( nEditViewSearchBasicIndex < nEditViewSearchBasicMAX )
								{
									// 09/04/2009 Paul.  Add Auto-Complete to the NAME search field. 
									if ( sFIELD_NAME == "NAME" )
									{
										sbModuleEditViewSearchBasic.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsAutoComplete '" + sMODULE_NAME + ".SearchBasic', " + nEditViewSearchBasicIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0).ToString() + ", 1, " + nMAX_SIZE.ToString() + ", 35, '" + sMODULE_NAME + "', null;");
										sbModuleEditViewSearchPopup.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsAutoComplete '" + sMODULE_NAME + ".SearchPopup', " + nEditViewSearchPopupIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0).ToString() + ", 1, " + nMAX_SIZE.ToString() + ", 35, '" + sMODULE_NAME + "', null;");
									}
									else
									{
										sbModuleEditViewSearchBasic.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsBound        '" + sMODULE_NAME + ".SearchBasic', " + nEditViewSearchBasicIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0).ToString() + ", 1, " + nMAX_SIZE.ToString() + ", 35, null;");
										sbModuleEditViewSearchPopup.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsBound        '" + sMODULE_NAME + ".SearchPopup', " + nEditViewSearchPopupIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0).ToString() + ", 1, " + nMAX_SIZE.ToString() + ", 35, null;");
									}
									nEditViewSearchBasicIndex++;
									nEditViewSearchPopupIndex++;
								}
								// 03/07/2010 Paul.  GridViewIndex will start at 1 to make room for the checkbox. 
								// 03/05/2011 Paul.  Start at 2 to make room for edit button. 
								if ( nGridViewIndex == 2 )
								{
									sbModuleGridViewData .AppendLine("	exec dbo.spGRIDVIEWS_COLUMNS_InsHyperLink '" + sMODULE_NAME + ".ListView', "  + nGridViewIndex     .ToString() + ", '" + sMODULE_NAME + ".LBL_LIST_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', '" + sFIELD_NAME + "', '35%', 'listViewTdLinkS1', 'ID', '" + (bIS_ADMIN ? "~/Administration/" : "~/") + sMODULE_NAME + "/view.aspx?id={0}', null, '" + sMODULE_NAME + "', " + (bINCLUDE_ASSIGNED_USER_ID ? "'ASSIGNED_USER_ID'" : "null") + ";");
									sbModuleGridViewPopup.AppendLine("	exec dbo.spGRIDVIEWS_COLUMNS_InsHyperLink '" + sMODULE_NAME + ".PopupView', " + nGridViewPopupIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_LIST_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', '" + sFIELD_NAME + "', '45%', 'listViewTdLinkS1', 'ID " + sFIELD_NAME + "', 'Select" + sMODULE_NAME_SINGULAR + "(''{0}'', ''{1}'');', null, '" + sMODULE_NAME + "', " + (bINCLUDE_ASSIGNED_USER_ID ? "'ASSIGNED_USER_ID'" : "null") + ";");
									nGridViewIndex++;
									nGridViewPopupIndex++;
								}
								else if ( nGridViewIndex < nGridViewMAX )
								{
									sbModuleGridViewData .AppendLine("	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     '" + sMODULE_NAME + ".ListView', "  + nGridViewIndex     .ToString() + ", '" + sMODULE_NAME + ".LBL_LIST_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', '" + sFIELD_NAME + "', '20%';");
									sbModuleGridViewPopup.AppendLine("	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     '" + sMODULE_NAME + ".PopupView', " + nGridViewPopupIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_LIST_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', '" + sFIELD_NAME + "', '20%';");
									nGridViewIndex++;
									nGridViewPopupIndex++;
								}
								// 02/09/2015 Paul.  Need to prevent ambiguous reference with System.Web.DynamicData.DynamicControl when not using code-behind. 
								sbCallUpdateProcedure.AppendLine("										, new SplendidCRM.DynamicControl(this, rowCurrent, \"" + sFIELD_NAME + "\"" + Strings.Space(35 - sFIELD_NAME.Length) + ").Text");
								if ( Sql.IsEmptyString(sFIRST_TEXT_FIELD) )
									sFIRST_TEXT_FIELD = sFIELD_NAME;
								break;
							case "Text Area":
								sbModuleEditViewData          .AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   '" + sMODULE_NAME + ".EditView', " + nEditViewIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0).ToString() + ", 1,   1, 70, 3;"  );
								sbModuleEditViewSearchAdvanced.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine    '" + sMODULE_NAME + ".SearchAdvanced', " + nEditViewSearchAdvancedIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0).ToString() + ", 1,   1, 70, 3;"  );
								nEditViewIndex++;
								nEditViewSearchAdvancedIndex++;
								if ( nEditViewSearchBasicIndex < nEditViewSearchBasicMAX )
								{
									sbModuleEditViewSearchBasic.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine    '" + sMODULE_NAME + ".SearchBasic', " + nEditViewSearchBasicIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0).ToString() + ", 1,   1, 70, 3;"  );
									sbModuleEditViewSearchPopup.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine    '" + sMODULE_NAME + ".SearchPopup', " + nEditViewSearchPopupIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0).ToString() + ", 1,   1, 70, 3;"  );
									nEditViewSearchBasicIndex++;
									nEditViewSearchPopupIndex++;
								}
								if ( nGridViewIndex == 0 )
								{
									sbModuleGridViewData.AppendLine("	exec dbo.spGRIDVIEWS_COLUMNS_InsHyperLink '" + sMODULE_NAME + ".ListView', " + nGridViewIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_LIST_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', '" + sFIELD_NAME + "', '35%', 'listViewTdLinkS1', 'ID', '" + (bIS_ADMIN ? "~/Administration/" : "~/") + sMODULE_NAME + "/view.aspx?id={0}', null, '" + sMODULE_NAME + "', " + (bINCLUDE_ASSIGNED_USER_ID ? "'ASSIGNED_USER_ID'" : "null") + ";");
									nGridViewIndex++;
								}
								else if ( nGridViewIndex < nGridViewMAX )
								{
									sbModuleGridViewData.AppendLine("	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     '" + sMODULE_NAME + ".ListView', " + nGridViewIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_LIST_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', '" + sFIELD_NAME + "', '20%';");
									nGridViewIndex++;
								}
								// 02/09/2015 Paul.  Need to prevent ambiguous reference with System.Web.DynamicData.DynamicControl when not using code-behind. 
								sbCallUpdateProcedure.AppendLine("										, new SplendidCRM.DynamicControl(this, rowCurrent, \"" + sFIELD_NAME + "\"" + Strings.Space(35 - sFIELD_NAME.Length) + ").Text");
								break;
							case "Integer":
								sbModuleEditViewData          .AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsBound       '" + sMODULE_NAME + ".EditView', " + nEditViewIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0).ToString() + ", 1, 10, 10, null;");
								sbModuleEditViewSearchAdvanced.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsBound        '" + sMODULE_NAME + ".SearchAdvanced', " + nEditViewSearchAdvancedIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0).ToString() + ", 1, 10, 10, null;");
								nEditViewIndex++;
								nEditViewSearchAdvancedIndex++;
								if ( nEditViewSearchBasicIndex < nEditViewSearchBasicMAX )
								{
									sbModuleEditViewSearchBasic.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsBound        '" + sMODULE_NAME + ".SearchBasic', " + nEditViewSearchBasicIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0).ToString() + ", 1, 10, 10, null;");
									sbModuleEditViewSearchPopup.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsBound        '" + sMODULE_NAME + ".SearchPopup', " + nEditViewSearchPopupIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0).ToString() + ", 1, 10, 10, null;");
									nEditViewSearchBasicIndex++;
									nEditViewSearchPopupIndex++;
								}
								if ( nGridViewIndex == 0 )
								{
									sbModuleGridViewData.AppendLine("	exec dbo.spGRIDVIEWS_COLUMNS_InsHyperLink '" + sMODULE_NAME + ".ListView', " + nGridViewIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_LIST_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', '" + sFIELD_NAME + "', '35%', 'listViewTdLinkS1', 'ID', '" + (bIS_ADMIN ? "~/Administration/" : "~/") + sMODULE_NAME + "/view.aspx?id={0}', null, '" + sMODULE_NAME + "', " + (bINCLUDE_ASSIGNED_USER_ID ? "'ASSIGNED_USER_ID'" : "null") + ";");
									nGridViewIndex++;
								}
								else if ( nGridViewIndex < nGridViewMAX )
								{
									sbModuleGridViewData.AppendLine("	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     '" + sMODULE_NAME + ".ListView', " + nGridViewIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_LIST_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', '" + sFIELD_NAME + "', '20%';");
									nGridViewIndex++;
								}
								// 02/09/2015 Paul.  Need to prevent ambiguous reference with System.Web.DynamicData.DynamicControl when not using code-behind. 
								sbCallUpdateProcedure.AppendLine("										, new SplendidCRM.DynamicControl(this, rowCurrent, \"" + sFIELD_NAME + "\"" + Strings.Space(35 - sFIELD_NAME.Length) + ").IntegerValue");
								break;
							case "bigint" :
								sbModuleEditViewData          .AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsBound       '" + sMODULE_NAME + ".EditView', " + nEditViewIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0).ToString() + ", 1, 10, 10, null;");
								sbModuleEditViewSearchAdvanced.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsBound        '" + sMODULE_NAME + ".SearchAdvanced', " + nEditViewSearchAdvancedIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0).ToString() + ", 1, 10, 10, null;");
								nEditViewIndex++;
								nEditViewSearchAdvancedIndex++;
								if ( nEditViewSearchBasicIndex < nEditViewSearchBasicMAX )
								{
									sbModuleEditViewSearchBasic.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsBound        '" + sMODULE_NAME + ".SearchBasic', " + nEditViewSearchBasicIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0).ToString() + ", 1, 10, 10, null;");
									sbModuleEditViewSearchPopup.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsBound        '" + sMODULE_NAME + ".SearchPopup', " + nEditViewSearchPopupIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0).ToString() + ", 1, 10, 10, null;");
									nEditViewSearchBasicIndex++;
									nEditViewSearchPopupIndex++;
								}
								if ( nGridViewIndex == 0 )
								{
									sbModuleGridViewData.AppendLine("	exec dbo.spGRIDVIEWS_COLUMNS_InsHyperLink '" + sMODULE_NAME + ".ListView', " + nGridViewIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_LIST_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', '" + sFIELD_NAME + "', '35%', 'listViewTdLinkS1', 'ID', '" + (bIS_ADMIN ? "~/Administration/" : "~/") + sMODULE_NAME + "/view.aspx?id={0}', null, '" + sMODULE_NAME + "', " + (bINCLUDE_ASSIGNED_USER_ID ? "'ASSIGNED_USER_ID'" : "null") + ";");
									nGridViewIndex++;
								}
								else if ( nGridViewIndex < nGridViewMAX )
								{
									sbModuleGridViewData.AppendLine("	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     '" + sMODULE_NAME + ".ListView', " + nGridViewIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_LIST_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', '" + sFIELD_NAME + "', '20%';");
									nGridViewIndex++;
								}
								// 02/09/2015 Paul.  Need to prevent ambiguous reference with System.Web.DynamicData.DynamicControl when not using code-behind. 
								sbCallUpdateProcedure.AppendLine("										, new SplendidCRM.DynamicControl(this, rowCurrent, \"" + sFIELD_NAME + "\"" + Strings.Space(35 - sFIELD_NAME.Length) + ").IntegerValue");
								break;
							case "Decimal":
								sbModuleEditViewData          .AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsBound       '" + sMODULE_NAME + ".EditView', " + nEditViewIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0).ToString() + ", 1, 10, 10, null;");
								sbModuleEditViewSearchAdvanced.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsBound        '" + sMODULE_NAME + ".SearchAdvanced', " + nEditViewSearchAdvancedIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0).ToString() + ", 1, 10, 10, null;");
								nEditViewIndex++;
								nEditViewSearchAdvancedIndex++;
								if ( nEditViewSearchBasicIndex < nEditViewSearchBasicMAX )
								{
									sbModuleEditViewSearchBasic.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsBound        '" + sMODULE_NAME + ".SearchBasic', " + nEditViewSearchBasicIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0).ToString() + ", 1, 10, 10, null;");
									sbModuleEditViewSearchPopup.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsBound        '" + sMODULE_NAME + ".SearchPopup', " + nEditViewSearchPopupIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0).ToString() + ", 1, 10, 10, null;");
									nEditViewSearchBasicIndex++;
									nEditViewSearchPopupIndex++;
								}
								// 02/09/2015 Paul.  Need to prevent ambiguous reference with System.Web.DynamicData.DynamicControl when not using code-behind. 
								sbCallUpdateProcedure.AppendLine("										, new SplendidCRM.DynamicControl(this, rowCurrent, \"" + sFIELD_NAME + "\"" + Strings.Space(35 - sFIELD_NAME.Length) + ").FloatValue");
								break;
							case "Money":
								sbModuleEditViewData          .AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsBound       '" + sMODULE_NAME + ".EditView', " + nEditViewIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0).ToString() + ", 1, 10, 10, null;");
								sbModuleEditViewSearchAdvanced.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsBound        '" + sMODULE_NAME + ".SearchAdvanced', " + nEditViewSearchAdvancedIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0).ToString() + ", 1, 10, 10, null;");
								nEditViewIndex++;
								nEditViewSearchAdvancedIndex++;
								if ( nEditViewSearchBasicIndex < nEditViewSearchBasicMAX )
								{
									sbModuleEditViewSearchBasic.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsBound        '" + sMODULE_NAME + ".SearchBasic', " + nEditViewSearchBasicIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0).ToString() + ", 1, 10, 10, null;");
									sbModuleEditViewSearchPopup.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsBound        '" + sMODULE_NAME + ".SearchPopup', " + nEditViewSearchPopupIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0).ToString() + ", 1, 10, 10, null;");
									nEditViewSearchBasicIndex++;
									nEditViewSearchPopupIndex++;
								}
								// 02/09/2015 Paul.  Need to prevent ambiguous reference with System.Web.DynamicData.DynamicControl when not using code-behind. 
								sbCallUpdateProcedure.AppendLine("										, new SplendidCRM.DynamicControl(this, rowCurrent, \"" + sFIELD_NAME + "\"" + Strings.Space(35 - sFIELD_NAME.Length) + ").DecimalValue");
								break;
							case "Checkbox"   :
								sbModuleEditViewData          .AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsControl     '" + sMODULE_NAME + ".EditView', "       + nEditViewIndex.ToString()               + ", '" + sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0).ToString() + ", 1, 'CheckBox', null, null, null;");
								sbModuleEditViewSearchAdvanced.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsControl      '" + sMODULE_NAME + ".SearchAdvanced', " + nEditViewSearchAdvancedIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0).ToString() + ", 1, 'CheckBox', null, null, null;");
								nEditViewIndex++;
								nEditViewSearchAdvancedIndex++;
								if ( nEditViewSearchBasicIndex < nEditViewSearchBasicMAX )
								{
									sbModuleEditViewSearchBasic.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsControl      '" + sMODULE_NAME + ".SearchBasic', " + nEditViewSearchBasicIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0).ToString() + ", 1, 'CheckBox', null, null, null;");
									sbModuleEditViewSearchPopup.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsControl      '" + sMODULE_NAME + ".SearchPopup', " + nEditViewSearchPopupIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0).ToString() + ", 1, 'CheckBox', null, null, null;");
									nEditViewSearchBasicIndex++;
									nEditViewSearchPopupIndex++;
								}
								// 02/09/2015 Paul.  Need to prevent ambiguous reference with System.Web.DynamicData.DynamicControl when not using code-behind. 
								sbCallUpdateProcedure.AppendLine("										, new SplendidCRM.DynamicControl(this, rowCurrent, \"" + sFIELD_NAME + "\"" + Strings.Space(35 - sFIELD_NAME.Length) + ").Checked");
								break;
							case "Date":
								sbModuleEditViewData          .AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsControl     '" + sMODULE_NAME + ".EditView', " + nEditViewIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0).ToString() + ", 1, 'DatePicker', null, null, null;");
								sbModuleEditViewSearchAdvanced.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsControl      '" + sMODULE_NAME + ".SearchAdvanced', " + nEditViewSearchAdvancedIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0).ToString() + ", 1, 'DatePicker', null, null, null;");
								nEditViewIndex++;
								nEditViewSearchAdvancedIndex++;
								if ( nEditViewSearchBasicIndex < nEditViewSearchBasicMAX )
								{
									sbModuleEditViewSearchBasic.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsControl      '" + sMODULE_NAME + ".SearchBasic', " + nEditViewSearchBasicIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0).ToString() + ", 1, 'DatePicker', null, null, null;");
									sbModuleEditViewSearchPopup.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsControl      '" + sMODULE_NAME + ".SearchPopup', " + nEditViewSearchPopupIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0).ToString() + ", 1, 'DatePicker', null, null, null;");
									nEditViewSearchBasicIndex++;
									nEditViewSearchPopupIndex++;
								}
								// 02/09/2015 Paul.  Need to prevent ambiguous reference with System.Web.DynamicData.DynamicControl when not using code-behind. 
								sbCallUpdateProcedure.AppendLine("										, new SplendidCRM.DynamicControl(this, rowCurrent, \"" + sFIELD_NAME + "\"" + Strings.Space(35 - sFIELD_NAME.Length) + ").DateValue");
								break;
							case "Dropdown":
								sbModuleEditViewData          .AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   '" + sMODULE_NAME + ".EditView', " + nEditViewIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0).ToString() + ", 1, '" + sFIELD_NAME.ToLower() + "_dom', null, null;");
								sbModuleEditViewSearchAdvanced.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsBoundList    '" + sMODULE_NAME + ".SearchAdvanced', " + nEditViewSearchAdvancedIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0).ToString() + ", 1, '" + sFIELD_NAME.ToLower() + "_dom', null, null;");
								nEditViewIndex++;
								nEditViewSearchAdvancedIndex++;
								if ( nEditViewSearchBasicIndex < nEditViewSearchBasicMAX )
								{
									sbModuleEditViewSearchBasic.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsBoundList    '" + sMODULE_NAME + ".SearchBasic', " + nEditViewSearchBasicIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0).ToString() + ", 1, '" + sFIELD_NAME.ToLower() + "_dom', null, null;");
									sbModuleEditViewSearchPopup.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsBoundList    '" + sMODULE_NAME + ".SearchPopup', " + nEditViewSearchPopupIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0).ToString() + ", 1, '" + sFIELD_NAME.ToLower() + "_dom', null, null;");
									nEditViewSearchBasicIndex++;
									nEditViewSearchPopupIndex++;
								}
								// 02/09/2015 Paul.  Need to prevent ambiguous reference with System.Web.DynamicData.DynamicControl when not using code-behind. 
								sbCallUpdateProcedure.AppendLine("										, new SplendidCRM.DynamicControl(this, rowCurrent, \"" + sFIELD_NAME + "\"" + Strings.Space(35 - sFIELD_NAME.Length) + ").SelectedValue");
								break;
							case "Guid":
								sbModuleEditViewData          .AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsChange      '" + sMODULE_NAME + ".EditView', " + nEditViewIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0).ToString() + ", 1, '" + sFIELD_NAME.Substring(0, sFIELD_NAME.Length - 3) + "_NAME', 'return " + sMODULE_NAME_SINGULAR + "Popup();', null;");
								sbModuleEditViewSearchAdvanced.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsChange       '" + sMODULE_NAME + ".SearchAdvanced', " + nEditViewSearchAdvancedIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0).ToString() + ", 1, '" + sFIELD_NAME.Substring(0, sFIELD_NAME.Length - 3) + "_NAME', 'return " + sMODULE_NAME_SINGULAR + "Popup();', null;");
								nEditViewIndex++;
								nEditViewSearchAdvancedIndex++;
								if ( nEditViewSearchBasicIndex < nEditViewSearchBasicMAX )
								{
									sbModuleEditViewSearchBasic.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsChange       '" + sMODULE_NAME + ".SearchBasic', " + nEditViewSearchBasicIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0).ToString() + ", 1, '" + sFIELD_NAME.Substring(0, sFIELD_NAME.Length - 3) + "_NAME', 'return " + sMODULE_NAME_SINGULAR + "Popup();', null;");
									sbModuleEditViewSearchPopup.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsChange       '" + sMODULE_NAME + ".SearchPopup', " + nEditViewSearchPopupIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0).ToString() + ", 1, '" + sFIELD_NAME.Substring(0, sFIELD_NAME.Length - 3) + "_NAME', 'return " + sMODULE_NAME_SINGULAR + "Popup();', null;");
									nEditViewSearchBasicIndex++;
									nEditViewSearchPopupIndex++;
								}
								if ( nGridViewIndex == 0 )
								{
									sbModuleGridViewData.AppendLine("	exec dbo.spGRIDVIEWS_COLUMNS_InsHyperLink '" + sMODULE_NAME + ".ListView', " + nGridViewIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_LIST_" + sFIELD_NAME + "', '" + sFIELD_NAME.Substring(0, sFIELD_NAME.Length - 3) + "_NAME', '" + sFIELD_NAME.Substring(0, sFIELD_NAME.Length - 3) + "_NAME', '35%', 'listViewTdLinkS1', 'ID', '" + (bIS_ADMIN ? "~/Administration/" : "~/") + sMODULE_NAME + "/view.aspx?id={0}', null, '" + sMODULE_NAME + "', " + (bINCLUDE_ASSIGNED_USER_ID ? "'ASSIGNED_USER_ID'" : "null") + ";");
									nGridViewIndex++;
								}
								else if ( nGridViewIndex < nGridViewMAX )
								{
									sbModuleGridViewData.AppendLine("	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     '" + sMODULE_NAME + ".ListView', " + nGridViewIndex.ToString() + ", '" + sMODULE_NAME + ".LBL_LIST_" + sFIELD_NAME + "', '" + sFIELD_NAME.Substring(0, sFIELD_NAME.Length - 3) + "_NAME', '" + sFIELD_NAME.Substring(0, sFIELD_NAME.Length - 3) + "_NAME', '20%';");
									nGridViewIndex++;
								}
								// 02/09/2015 Paul.  Need to prevent ambiguous reference with System.Web.DynamicData.DynamicControl when not using code-behind. 
								sbCallUpdateProcedure.AppendLine("										, new SplendidCRM.DynamicControl(this, rowCurrent, \"" + sFIELD_NAME + "\"" + Strings.Space(35 - sFIELD_NAME.Length) + ").ID");
								break;
						}
					}
					if ( Sql.IsEmptyString(sFIRST_TEXT_FIELD) )
						sFIRST_TEXT_FIELD = "NAME";
					// 05/24/2017 Paul.  Need to add TAG_SET_NAME as it is used in the stored procedure. 
					sbCallUpdateProcedure.AppendLine("										, new SplendidCRM.DynamicControl(this, rowCurrent, \"TAG_SET_NAME\"                       ).Text");
					sbCallUpdateProcedure.AppendLine("										, trn");
					sbCallUpdateProcedure.AppendLine("										);");
					if ( bINCLUDE_ASSIGNED_USER_ID )
					{
						sbCreateViewFields.AppendLine("     , " + sTABLE_NAME +".ASSIGNED_USER_ID");
						sbCreateViewFields.AppendLine("     , USERS_ASSIGNED.USER_NAME    as ASSIGNED_TO");
						
						sbCreateViewJoins .AppendLine("  left outer join USERS                      USERS_ASSIGNED");
						sbCreateViewJoins .AppendLine("               on USERS_ASSIGNED.ID        = " + sTABLE_NAME +".ASSIGNED_USER_ID");
						
						// 08/26/2009 Paul.  Add support for dynamic teams. 
						sbModuleDetailViewData.AppendLine("	exec dbo.spDETAILVIEWS_FIELDS_InsBound     '" + sMODULE_NAME + ".DetailView', " + nDetailViewIndex.ToString() + ", '.LBL_ASSIGNED_TO'                , 'ASSIGNED_TO'                      , '{0}'        , null;");
						nDetailViewIndex++;
						// 09/23/2009 Paul.  Use new ModulePopup fort he assigned user. 
						sbModuleEditViewData  .AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup '" + sMODULE_NAME + ".EditView', " + nEditViewIndex.ToString() + ", '.LBL_ASSIGNED_TO'                       , 'ASSIGNED_USER_ID'           , 0, 1, 'ASSIGNED_TO'        , 'Users', null;");

						nEditViewIndex++;
						if ( !bINCLUDE_TEAM_ID )
						{
							sbModuleDetailViewData.AppendLine("	exec dbo.spDETAILVIEWS_FIELDS_InsBlank     '" + sMODULE_NAME + ".DetailView', " + nDetailViewIndex.ToString() + ", null;");
							nDetailViewIndex++;
							sbModuleEditViewData  .AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsBlank       '" + sMODULE_NAME + ".EditView', " + nEditViewIndex.ToString() + ", null;");
							nEditViewIndex++;
						}
						sbModuleEditViewSearchBasic   .AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsControl      '" + sMODULE_NAME + ".SearchBasic'    , " + nEditViewSearchBasicIndex.ToString() + ", '.LBL_CURRENT_USER_FILTER', 'CURRENT_USER_ONLY', 0, null, 'CheckBox', 'return ToggleUnassignedOnly();', null, null;");
						nEditViewSearchBasicIndex++;
						sbModuleEditViewSearchAdvanced.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsBoundList    '" + sMODULE_NAME + ".SearchAdvanced' , " + nEditViewSearchAdvancedIndex.ToString() + ", '.LBL_ASSIGNED_TO'     , 'ASSIGNED_USER_ID', 0, null, 'AssignedUser'    , null, 6;");
						nEditViewSearchAdvancedIndex++;

						sbModuleGridViewData .AppendLine("	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     '" + sMODULE_NAME + ".ListView', "  + nGridViewIndex     .ToString() + ", '.LBL_LIST_ASSIGNED_USER'                  , 'ASSIGNED_TO'     , 'ASSIGNED_TO'     , '10%';");
						sbModuleGridViewPopup.AppendLine("	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     '" + sMODULE_NAME + ".PopupView', " + nGridViewPopupIndex.ToString() + ", '.LBL_LIST_ASSIGNED_USER'                  , 'ASSIGNED_TO'     , 'ASSIGNED_TO'     , '10%';");
						nGridViewIndex++;
						nGridViewPopupIndex++;
					}
					if ( bINCLUDE_TEAM_ID )
					{
						sbCreateViewFields.AppendLine("     , TEAMS.ID                    as TEAM_ID");
						sbCreateViewFields.AppendLine("     , TEAMS.NAME                  as TEAM_NAME");
						// 09/23/2009 Paul.  TEAM_SET_ID was missing. 
						sbCreateViewFields.AppendLine("     , TEAM_SETS.ID                as TEAM_SET_ID");
						sbCreateViewFields.AppendLine("     , TEAM_SETS.TEAM_SET_NAME     as TEAM_SET_NAME");
						
						sbCreateViewJoins .AppendLine("  left outer join TEAMS");
						sbCreateViewJoins .AppendLine("               on TEAMS.ID                 = " + sTABLE_NAME +".TEAM_ID");
						sbCreateViewJoins .AppendLine("              and TEAMS.DELETED            = 0");
						// 09/23/2009 Paul.  TEAM_SET_ID was missing. 
						sbCreateViewJoins .AppendLine("  left outer join TEAM_SETS");
						sbCreateViewJoins .AppendLine("               on TEAM_SETS.ID             = " + sTABLE_NAME +".TEAM_SET_ID");
						sbCreateViewJoins .AppendLine("              and TEAM_SETS.DELETED        = 0");
						
						if ( !bINCLUDE_ASSIGNED_USER_ID )
						{
							sbModuleDetailViewData.AppendLine("	exec dbo.spDETAILVIEWS_FIELDS_InsBlank     '" + sMODULE_NAME + ".DetailView', " + nDetailViewIndex.ToString() + ", null;");
							nDetailViewIndex++;
							sbModuleEditViewData  .AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsBlank       '" + sMODULE_NAME + ".EditView', " + nEditViewIndex.ToString() + ", null;");
							nEditViewIndex++;
						}
						// 08/26/2009 Paul.  Add support for dynamic teams. 
						// 09/23/2009 Paul.  To allow dynamic teams to be turned off, use base team in fields. 
						sbModuleDetailViewData.AppendLine("	exec dbo.spDETAILVIEWS_FIELDS_InsBound     '" + sMODULE_NAME + ".DetailView', " + nDetailViewIndex.ToString() + ", 'Teams.LBL_TEAM'                  , 'TEAM_NAME'                        , '{0}'        , null;");
						nDetailViewIndex++;
						sbModuleEditViewData  .AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup '" + sMODULE_NAME + ".EditView', " + nEditViewIndex.ToString()     + ", 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , 'Teams', null;");
						nEditViewIndex++;

						sbModuleGridViewData .AppendLine("	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     '" + sMODULE_NAME + ".ListView', "  + nGridViewIndex     .ToString() + ", 'Teams.LBL_LIST_TEAM'                      , 'TEAM_NAME'       , 'TEAM_NAME'       , '5%';");
						sbModuleGridViewPopup.AppendLine("	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     '" + sMODULE_NAME + ".PopupView', " + nGridViewPopupIndex.ToString() + ", 'Teams.LBL_LIST_TEAM'                      , 'TEAM_NAME'       , 'TEAM_NAME'       , '10%';");
						nGridViewIndex++;
						nGridViewPopupIndex++;
					}
					sbModuleDetailViewData.AppendLine("	exec dbo.spDETAILVIEWS_FIELDS_InsBound     '" + sMODULE_NAME + ".DetailView', " + nDetailViewIndex.ToString() + ", '.LBL_DATE_MODIFIED'              , 'DATE_MODIFIED .LBL_BY MODIFIED_BY', '{0} {1} {2}', null;");
					nDetailViewIndex++;
					sbModuleDetailViewData.AppendLine("	exec dbo.spDETAILVIEWS_FIELDS_InsBound     '" + sMODULE_NAME + ".DetailView', " + nDetailViewIndex.ToString() + ", '.LBL_DATE_ENTERED'               , 'DATE_ENTERED .LBL_BY CREATED_BY'  , '{0} {1} {2}', null;");
					nDetailViewIndex++;

					sbModuleEditViewSearchBasic.AppendLine("");

					foreach ( string sRELATED_MODULE in lstRelationships )
					{
						{
							string sRELATED_TABLE           = Sql.ToString(Application["Modules." + sRELATED_MODULE + ".TableName"]);

							sbMergeProcedureUpdates.AppendLine("	update " + sTABLE_NAME + "_" + sRELATED_TABLE);
							sbMergeProcedureUpdates.AppendLine("	   set " + sTABLE_NAME_SINGULAR + "_ID       = @ID");
							sbMergeProcedureUpdates.AppendLine("	     , DATE_MODIFIED    = getdate()");
							sbMergeProcedureUpdates.AppendLine("	     , DATE_MODIFIED_UTC= getutcdate()");
							sbMergeProcedureUpdates.AppendLine("	     , MODIFIED_USER_ID = @MODIFIED_USER_ID");
							sbMergeProcedureUpdates.AppendLine("	 where " + sTABLE_NAME_SINGULAR + "_ID       = @MERGE_ID");
							sbMergeProcedureUpdates.AppendLine("	   and DELETED          = 0;");
							sbMergeProcedureUpdates.AppendLine("");
							
							// 08/08/2013 Paul.  Add delete and undelete of relationships. 
							sbDeleteProcedureUpdates.AppendLine("	update " + sTABLE_NAME + "_" + sRELATED_TABLE);
							sbDeleteProcedureUpdates.AppendLine("	   set DELETED          = 1");
							sbDeleteProcedureUpdates.AppendLine("	     , DATE_MODIFIED    = getdate()");
							sbDeleteProcedureUpdates.AppendLine("	     , DATE_MODIFIED_UTC= getutcdate()");
							sbDeleteProcedureUpdates.AppendLine("	     , MODIFIED_USER_ID = @MODIFIED_USER_ID");
							sbDeleteProcedureUpdates.AppendLine("	  where " + sTABLE_NAME_SINGULAR + "_ID       = @ID");
							sbDeleteProcedureUpdates.AppendLine("	   and DELETED          = 0;");
							sbDeleteProcedureUpdates.AppendLine("");
							
							sbUndeleteProcedureUpdates.AppendLine("	update " + sTABLE_NAME + "_" + sRELATED_TABLE);
							sbUndeleteProcedureUpdates.AppendLine("	   set DELETED          = 1");
							sbUndeleteProcedureUpdates.AppendLine("	     , DATE_MODIFIED    = getdate()");
							sbUndeleteProcedureUpdates.AppendLine("	     , DATE_MODIFIED_UTC= getutcdate()");
							sbUndeleteProcedureUpdates.AppendLine("	     , MODIFIED_USER_ID = @MODIFIED_USER_ID");
							sbUndeleteProcedureUpdates.AppendLine("	  where " + sTABLE_NAME_SINGULAR + "_ID       = @ID");
							sbUndeleteProcedureUpdates.AppendLine("	   and DELETED          = 0;");
							sbUndeleteProcedureUpdates.AppendLine("");
						}
					}
					// 03/06/2010 Paul.  EditView inline and PopupView inline will be identical to the EditView. 
					StringBuilder sbModuleEditViewDataInline  = new StringBuilder();
					StringBuilder sbModulePopupViewDataInline = new StringBuilder();
					sbModuleEditViewDataInline .Append(sbModuleEditViewData.ToString().Replace("'" + sMODULE_NAME + ".EditView'", "'" + sMODULE_NAME + ".EditView.Inline'" ));
					sbModulePopupViewDataInline.Append(sbModuleEditViewData.ToString().Replace("'" + sMODULE_NAME + ".EditView'", "'" + sMODULE_NAME + ".PopupView.Inline'"));

					System.Text.Encoding enc = System.Text.Encoding.UTF8;
					DataTable dtSQLScripts = new DataTable();
					dtSQLScripts.Columns.Add("FOLDER"        );
					dtSQLScripts.Columns.Add("NAME"          );
					dtSQLScripts.Columns.Add("PROCEDURE_NAME");
					dtSQLScripts.Columns.Add("SQL_SCRIPT"    );
					dtSQLScripts.Columns.Add("CODE_WRAPPER"  );

					DataView vwFieldsNAME = new DataView(dtFields);
					vwFieldsNAME.RowFilter = "FIELD_NAME = 'NAME'";
					string[] arrSqlFolders = Directory.GetDirectories(sSqlTemplatesPath);
					foreach ( string sSqlFolder in arrSqlFolders )
					{
						//sbProgress.AppendLine(sSqlFolder + "<br>");
						
						string[] arrSqlFolderParts = sSqlFolder.Split(Path.DirectorySeparatorChar);
						string sFolder = arrSqlFolderParts[arrSqlFolderParts.Length - 1];
						string[] arrSqlTemplates = Directory.GetFiles(sSqlFolder, "*.sql");
						foreach ( string sSqlTemplate in arrSqlTemplates )
						{
							//sbProgress.AppendLine(sSqlTemplate + "<br>");
							if ( sSqlTemplate.IndexOf("$relatedmodule$") >= 0 || sSqlTemplate.IndexOf("$relatedtable$") >= 0 )
								continue;
							// 08/24/2009 Paul.  Skip files that are team specific. 
							if ( !bINCLUDE_TEAM_ID && sSqlTemplate.IndexOf("TEAMS") >= 0 )
								continue;
							
							string sSqlScriptName = Path.GetFileName(sSqlTemplate);
							sSqlScriptName = sSqlScriptName.Replace("$modulename$"        , sMODULE_NAME         );
							sSqlScriptName = sSqlScriptName.Replace("$modulenamesingular$", sMODULE_NAME_SINGULAR);
							sSqlScriptName = sSqlScriptName.Replace("$tablename$"         , sTABLE_NAME          );
							sSqlScriptName = sSqlScriptName.Replace("$tablenamesingular$" , sTABLE_NAME_SINGULAR );
							
							DataRow rowSQL = dtSQLScripts.NewRow();
							dtSQLScripts.Rows.Add(rowSQL);
							rowSQL["FOLDER"        ] = sFolder;
							rowSQL["NAME"          ] = sSqlScriptName;
							rowSQL["PROCEDURE_NAME"] = sSqlScriptName.Split('.')[0];
							using ( StreamReader sr = new StreamReader(sSqlTemplate, enc, true) )
							{
								string sData = sr.ReadToEnd();
								// 10/03/2010 Paul.  We need to fix any GridViews that reference ASSIGNED_USER_ID if that field does not exist. 
								// 11/25/2021 Paul.  SYSTEM_REST_TABLES also has ASSIGNED_USER_ID. 
								if ( !bINCLUDE_ASSIGNED_USER_ID && (sSqlTemplate.Contains("GRIDVIEWS_COLUMNS") || sSqlTemplate.Contains("SYSTEM_REST_TABLES")))
								{
									sData = sData.Replace("\'ASSIGNED_USER_ID\'", "null");
								}
								sData = sData.Replace("$displayname$"                  , sDISPLAY_NAME         );
								// 06/04/2015 Paul.  The abbreviated name is used by the Seven theme. 
								sData = sData.Replace("$abbreviatedname$"              , sDISPLAY_NAME.Substring(0, 3));
								sData = sData.Replace("$displaynamesingular$"          , sDISPLAY_NAME_SINGULAR);
								sData = sData.Replace("$modulename$"                   , sMODULE_NAME          );
								sData = sData.Replace("$modulenamesingular$"           , sMODULE_NAME_SINGULAR );
								sData = sData.Replace("$tablename$"                    , sTABLE_NAME           );
								sData = sData.Replace("$tablenamesingular$"            , sTABLE_NAME_SINGULAR  );

								sData = sData.Replace("$tablename$"                    , sTABLE_NAME          );
								sData = sData.Replace("$tabenabled$"                   , bTAB_ENABLED    ? "1" : "0");
								sData = sData.Replace("$mobileenabled$"                , bMOBILE_ENABLED ? "1" : "0");
								sData = sData.Replace("$customenabled$"                , bCUSTOM_ENABLED ? "1" : "0");
								sData = sData.Replace("$reportenabled$"                , bREPORT_ENABLED ? "1" : "0");
								sData = sData.Replace("$importenabled$"                , bIMPORT_ENABLED ? "1" : "0");
								// 09/12/2011 Paul.  REST_ENABLED provides a way to enable/disable a module in the REST API. 
								sData = sData.Replace("$restenabled$"                  , bREST_ENABLED   ? "1" : "0");
								sData = sData.Replace("$isadmin$"                      , bIS_ADMIN       ? "1" : "0");
								// 11/25/2021 Paul.  SYSTEM_REST_TABLES also has ASSIGNED_USER_ID. 
								sData = sData.Replace("$isassigned$"                   , bINCLUDE_ASSIGNED_USER_ID ? "1" : "0");
								sData = sData.Replace("$administrationfolder$"         , bIS_ADMIN       ? "Administration/" : "");
								sData = sData.Replace("$taborder$"                     , "100");

								sData = sData.Replace("$createtablefields$"            , sbCreateTableFields            .ToString());
								sData = sData.Replace("$createtableindexes$"           , sbCreateTableIndexes           .ToString());
								sData = sData.Replace("$createviewfields$"             , sbCreateViewFields             .ToString());
								sData = sData.Replace("$createprocedureparameters$"    , sbCreateProcedureParameters    .ToString());
								sData = sData.Replace("$createprocedureinsertinto$"    , sbCreateProcedureInsertInto    .ToString());
								sData = sData.Replace("$createprocedureinsertvalues$"  , sbCreateProcedureInsertValues  .ToString());
								sData = sData.Replace("$createprocedureupdate$"        , sbCreateProcedureUpdate        .ToString());
								sData = sData.Replace("$createprocedurenormalizeteams$", sbCreateProcedureNormalizeTeams.ToString());
								sData = sData.Replace("$createprocedureupdateteams$"   , sbCreateProcedureUpdateTeams   .ToString());
								sData = sData.Replace("$altertablefields$"             , sbAlterTableFields             .ToString());

								sData = sData.Replace("$createviewjoins$"              , sbCreateViewJoins              .ToString());
								sData = sData.Replace("$massupdateviewfields$"         , sbMassUpdateProcedureFields    .ToString());
								sData = sData.Replace("$massupdatesets$"               , sbMassUpdateProcedureSets      .ToString());
								sData = sData.Replace("$massupdateteamnormalize$"      , sbMassUpdateTeamNormalize      .ToString());
								sData = sData.Replace("$massupdateteamadd$"            , sbMassUpdateTeamAdd            .ToString());
								sData = sData.Replace("$massupdateteamupdate$"         , sbMassUpdateTeamUpdate         .ToString());

								sData = sData.Replace("$mergeupdaterelationship$"      , sbMergeProcedureUpdates        .ToString());

								sData = sData.Replace("$modulegridviewdata$"           , sbModuleGridViewData           .ToString());
								sData = sData.Replace("$modulegridviewpopup$"          , sbModuleGridViewPopup          .ToString());
								sData = sData.Replace("$moduledetailviewdata$"         , sbModuleDetailViewData         .ToString());
								sData = sData.Replace("$moduleeditviewdata$"           , sbModuleEditViewData           .ToString());
								sData = sData.Replace("$moduleeditviewdatainline$"     , sbModuleEditViewDataInline     .ToString());
								sData = sData.Replace("$modulepopupviewdatainline$"    , sbModulePopupViewDataInline    .ToString());
								sData = sData.Replace("$moduleeditviewsearchbasic$"    , sbModuleEditViewSearchBasic    .ToString());
								sData = sData.Replace("$moduleeditviewsearchadvanced$" , sbModuleEditViewSearchAdvanced .ToString());
								sData = sData.Replace("$moduleeditviewsearchpopup$"    , sbModuleEditViewSearchPopup    .ToString());

								sData = sData.Replace("$moduleterminology$"            , sbModuleTerminology            .ToString());
								sData = sData.Replace("$relatedterminology$"           , "");

								// 08/08/2013 Paul.  Add delete and undelete of relationships. 
								sData = sData.Replace("$deleteprocedureupdates$"       , sbDeleteProcedureUpdates       .ToString());
								sData = sData.Replace("$undeleteprocedureupdates$"     , sbUndeleteProcedureUpdates     .ToString());

								// 04/03/2012 Paul.  If the custom module has a name field, then uncomment the favorites update procedure. 
								if ( sSqlTemplate.Contains("sp$tablename$_Update.1.sql") )
								{
									if ( vwFieldsNAME.Count > 0 )
									{
										sData = sData.Replace("--exec dbo.spSUGARFAVORITES_UpdateName", "exec dbo.spSUGARFAVORITES_UpdateName");
									}
								}
								rowSQL["SQL_SCRIPT"] = sData;

								string sSqlScriptPath = Path.Combine(sSqlScriptsPath, sFolder);
								try
								{
									if ( !Directory.Exists(sSqlScriptPath) )
									{
										Directory.CreateDirectory(sSqlScriptPath);
									}
								}
								catch(Exception ex)
								{
									sbProgress.AppendLine("<font class=error>Failed to create " + sSqlScriptPath + ":" + ex.Message + "</font><br>");
								}
								
								string sSqlScriptFile = Path.Combine(sSqlScriptPath, sSqlScriptName);
								try
								{
									sbProgress.AppendLine(sSqlScriptFile + "<br>");
									if ( bOVERWRITE_EXISTING && System.IO.File.Exists(sSqlScriptFile) )
										System.IO.File.Delete(sSqlScriptFile);
									using(StreamWriter stm = System.IO.File.CreateText(sSqlScriptFile))
									{
										stm.Write(sData);
									}
								}
								catch(Exception ex)
								{
									sbProgress.AppendLine("<font class=error>" + sSqlScriptFile + ":" + ex.Message + "</font><br>");
								}
							}
						}
					}

					foreach ( string sRELATED_MODULE in lstRelationships )
					{
						{
							string sRELATED_MODULE_SINGULAR = sRELATED_MODULE;
							string sRELATED_TABLE           = Sql.ToString(Application["Modules." + sRELATED_MODULE + ".TableName"]);
							string sRELATED_TABLE_SINGULAR  = sRELATED_TABLE;
							if ( sRELATED_MODULE_SINGULAR.ToLower().EndsWith("ies") )
								sRELATED_MODULE_SINGULAR = sRELATED_MODULE_SINGULAR.Substring(0, sRELATED_MODULE_SINGULAR.Length-3) + "Y";
							else if ( sRELATED_MODULE_SINGULAR.ToLower().EndsWith("s") )
								sRELATED_MODULE_SINGULAR = sRELATED_MODULE_SINGULAR.Substring(0, sRELATED_MODULE_SINGULAR.Length-1);
							if ( sRELATED_TABLE_SINGULAR.ToLower().EndsWith("ies") )
								sRELATED_TABLE_SINGULAR = sRELATED_TABLE_SINGULAR.Substring(0, sRELATED_TABLE_SINGULAR.Length-3) + "Y";
							else if ( sRELATED_TABLE_SINGULAR.ToLower().EndsWith("s") )
								sRELATED_TABLE_SINGULAR = sRELATED_TABLE_SINGULAR.Substring(0, sRELATED_TABLE_SINGULAR.Length-1);

							foreach ( string sSqlFolder in arrSqlFolders )
							{
								//sbProgress.AppendLine(sSqlFolder + "<br>");
								
								string[] arrSqlFolderParts = sSqlFolder.Split(Path.DirectorySeparatorChar);
								string sFolder = arrSqlFolderParts[arrSqlFolderParts.Length - 1];
								string[] arrSqlTemplates = Directory.GetFiles(sSqlFolder, "*.sql");
								foreach ( string sSqlTemplate in arrSqlTemplates )
								{
									//sbProgress.AppendLine(sSqlTemplate + "<br>");
									if ( sSqlTemplate.IndexOf("$relatedmodule$") < 0 && sSqlTemplate.IndexOf("$relatedtable$") < 0 )
										continue;
									
									string sSqlScriptName = Path.GetFileName(sSqlTemplate);
									sSqlScriptName = sSqlScriptName.Replace("$modulename$"           , sMODULE_NAME            );
									sSqlScriptName = sSqlScriptName.Replace("$modulenamesingular$"   , sMODULE_NAME_SINGULAR   );
									sSqlScriptName = sSqlScriptName.Replace("$tablename$"            , sTABLE_NAME             );
									sSqlScriptName = sSqlScriptName.Replace("$tablenamesingular$"    , sTABLE_NAME_SINGULAR    );
									sSqlScriptName = sSqlScriptName.Replace("$relatedmodule$"        , sRELATED_MODULE         );
									sSqlScriptName = sSqlScriptName.Replace("$relatedmodulesingular$", sRELATED_MODULE_SINGULAR);
									sSqlScriptName = sSqlScriptName.Replace("$relatedtable$"         , sRELATED_TABLE          );
									sSqlScriptName = sSqlScriptName.Replace("$relatedtablesingular$" , sRELATED_TABLE_SINGULAR );
									
									DataRow rowSQL = dtSQLScripts.NewRow();
									dtSQLScripts.Rows.Add(rowSQL);
									rowSQL["FOLDER"        ] = sFolder;
									rowSQL["NAME"          ] = sSqlScriptName;
									rowSQL["PROCEDURE_NAME"] = sSqlScriptName.Split('.')[0];
									using ( StreamReader sr = new StreamReader(sSqlTemplate, enc, true) )
									{
										string sData = sr.ReadToEnd();
										if ( sSqlTemplate.EndsWith("vw$tablename$_$relatedtable$.1.sql") )
										{
											// 03/03/2011 Paul.  vwDOCUMENTS table already has a DOCUMENT_NAME field. 
											if ( sRELATED_TABLE == "DOCUMENTS" )
											{
												sData = sData.Replace("     , vw$relatedtable$.NAME ", "--     , vw$relatedtable$.NAME ");
											}
											// 03/03/2011 Paul.  vwPRODUCTS table already has a PRODUCT_ID field and a PRODUCT_NAME field. 
											if ( sRELATED_TABLE == "PRODUCTS" )
											{
												sData = sData.Replace("     , vw$relatedtable$.ID   ", "--     , vw$relatedtable$.ID   ");
												sData = sData.Replace("     , vw$relatedtable$.NAME ", "--     , vw$relatedtable$.NAME ");
											}
										}
										// 03/05/2011 Paul.  Emails and Notes don't have popup pages. 
										else if ( sSqlTemplate.EndsWith("DYNAMIC_BUTTONS $modulename$.$relatedmodule$.1.sql") )
										{
											if ( sRELATED_MODULE == "Emails" || sRELATED_MODULE == "Notes" )
											{
												sData = sData.Replace("exec dbo.spDYNAMIC_BUTTONS_InsPopup  '$modulename$.$relatedmodule$', 1, '$modulename$', 'edit', '$relatedmodule$', 'list', '$relatedmodulesingular$Popup();'", "--exec dbo.spDYNAMIC_BUTTONS_InsPopup  '$modulename$.$relatedmodule$', 1, '$modulename$', 'edit', '$relatedmodule$', 'list', '$relatedmodulesingular$Popup();'");
											}
										}
										// 08/08/2013 Paul.  If this is the audit file, then also add auditing of custom field table. 
										else if ( sSqlTemplate.EndsWith("BuildAuditTable_$tablename$.1.sql") )
										{
											if ( bCUSTOM_ENABLED )
												sData += sData.Replace("$tablename$", "$tablename$_CSTM");
										}
										sData = sData.Replace("$modulename$"                 , sMODULE_NAME            );
										sData = sData.Replace("$modulenamesingular$"         , sMODULE_NAME_SINGULAR   );
										sData = sData.Replace("$tablename$"                  , sTABLE_NAME             );
										sData = sData.Replace("$tablenamesingular$"          , sTABLE_NAME_SINGULAR    );
										sData = sData.Replace("$relatedmodule$"              , sRELATED_MODULE         );
										sData = sData.Replace("$relatedmodulesingular$"      , sRELATED_MODULE_SINGULAR);
										sData = sData.Replace("$relatedtable$"               , sRELATED_TABLE          );
										sData = sData.Replace("$relatedtablesingular$"       , sRELATED_TABLE_SINGULAR );
										// 10/03/2010 Paul.  Related Assigned should only be created if related field exists. 
										// 02/08/2022 Paul.  Related now uses the base view instead of the table. 
										if ( bINCLUDE_ASSIGNED_USER_ID )
											sData = sData.Replace("$relatedviewassigned$"        , "     , vw" + sTABLE_NAME + ".ASSIGNED_USER_ID as " + sTABLE_NAME_SINGULAR + "_ASSIGNED_USER_ID");
										else
											sData = sData.Replace("$relatedviewassigned$"        , String.Empty);
										rowSQL["SQL_SCRIPT"] = sData;
										
										string sSqlScriptPath = Path.Combine(sSqlScriptsPath, sFolder);
										try
										{
											if ( !Directory.Exists(sSqlScriptPath) )
											{
												Directory.CreateDirectory(sSqlScriptPath);
											}
										}
										catch(Exception ex)
										{
											sbProgress.AppendLine("<font class=error>Failed to create " + sSqlScriptPath + ":" + ex.Message + "</font><br>");
										}
										
										string sSqlScriptFile = Path.Combine(sSqlScriptPath, sSqlScriptName);
										try
										{
											sbProgress.AppendLine(sSqlScriptFile + "<br>");
											if ( bOVERWRITE_EXISTING && System.IO.File.Exists(sSqlScriptFile) )
												System.IO.File.Delete(sSqlScriptFile);
											using(StreamWriter stm = System.IO.File.CreateText(sSqlScriptFile))
											{
												stm.Write(sData);
											}
										}
										catch(Exception ex)
										{
											sbProgress.AppendLine("<font class=error>" + sSqlScriptFile + ":" + ex.Message + "</font><br>");
										}
									}
								}
							}
						}
					}

					DataView vwSQLScripts = new DataView(dtSQLScripts);
					// 03/08/2010 Paul.  Allow user to select Live deployment, but we still prefer the code-behind method. 
					// 11/25/2021 Paul.  REACT Only will live deploy. 
					if ( !bCREATE_CODE_BEHIND || bREACT_ONLY )
					{
						// 03/06/2010 Paul.  We need to apply the SQL Scripts so that we can generate the SqlProcs code prior to generating the C# code. 
						// 09/26/2011 Paul.  Update triggers for auditing. 
						string[] arrSQLFolderTypes = new string[] { "BaseTables", "Tables", "Views", "Procedures", "Triggers", "Data", "Terminology" };
						SplendidCRM.DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							bool bSQLAzure = false;
							if ( Sql.IsSQLServer(con) )
							{
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.CommandText = "select @@VERSION";
									string sSqlVersion = Sql.ToString(cmd.ExecuteScalar());
									// 10/13/2009 Paul.  Azure Product database has a different version than the CTP environment. 
									if ( sSqlVersion.StartsWith("Microsoft SQL Azure") || (sSqlVersion.IndexOf("SQL Server") > 0 && sSqlVersion.IndexOf("CloudDB") > 0) )
									{
										bSQLAzure = true;
									}
								}
							}
							foreach ( string sSQLFolderType in arrSQLFolderTypes )
							{
								// 09/26/2011 Paul.  Don't include the triggers folder unless this module supports auditing. 
								// 08/08/2013 Paul.  This code is wrong.  We should always create the triggers, but optionally add triggers to custom field table. 
								//if ( bCUSTOM_ENABLED && sSQLFolderType == "Triggers" )
								//	continue;
								for ( int nFolderLevel = 0; nFolderLevel <= 9; nFolderLevel++ )
								{
									vwSQLScripts.RowFilter = "FOLDER = '" + sSQLFolderType + "' and NAME like '%." + nFolderLevel.ToString() + ".sql'";
									foreach ( DataRowView row in vwSQLScripts )
									{
										string sNAME           = Sql.ToString(row["NAME"          ]);
										string sPROCEDURE_NAME = Sql.ToString(row["PROCEDURE_NAME"]);
										string sSQL_SCRIPT     = Sql.ToString(row["SQL_SCRIPT"    ]);
										try
										{
											sbProgress.AppendLine(sSQLFolderType + "\\" + sNAME + "<br>");
											if ( Sql.IsSQLServer(con) )
											{
												sSQL_SCRIPT = sSQL_SCRIPT.Replace("\r\ngo\r\n", "\r\nGO\r\n");
												sSQL_SCRIPT = sSQL_SCRIPT.Replace("\r\nGo\r\n", "\r\nGO\r\n");
												if ( bSQLAzure )
												{
													if ( sSQLFolderType == "Functions" || sSQLFolderType.StartsWith("Views") || sSQLFolderType.StartsWith("Procedures") )
													{
														sSQL_SCRIPT = sSQL_SCRIPT.Replace("\r\nwith encryption\r\n", "\r\n");
													}
												}
											}
											using ( IDbCommand cmd = con.CreateCommand() )
											{
												cmd.CommandType = CommandType.Text;
												string[] aCommands = null;
												if ( Sql.IsSQLServer(con) )
												{
													aCommands = Strings.Split(sSQL_SCRIPT, "\r\nGO\r\n", -1, CompareMethod.Text);
												}
												else if ( Sql.IsOracle(con) )
												{
													aCommands = Strings.Split(sSQL_SCRIPT, "\r\n/\r\n", -1, CompareMethod.Text);
												}
												else if ( Sql.IsMySQL(con) )
												{
													sSQL_SCRIPT = RemoveComments(sSQL_SCRIPT);
													aCommands = Strings.Split(sSQL_SCRIPT, "\r\n/\r\n", -1, CompareMethod.Text);
												}
												else if ( Sql.IsDB2(con) )
												{
													sSQL_SCRIPT = RemoveComments(sSQL_SCRIPT);
													aCommands = Strings.Split(sSQL_SCRIPT, "\r\n/\r\n", -1, CompareMethod.Text);
												}
												foreach ( string sCommand in aCommands )
												{
													if ( Sql.IsOracle(con) )
													{
														cmd.CommandText = sCommand;
														// 03/20/2006 Paul.  Oracle does not like CRLF. 
														// PLS-00103: Encountered the symbol "" when expecting one of the following:     return 
														cmd.CommandText = cmd.CommandText.Replace("\r\n", "\n");
													}
													else
														cmd.CommandText = sCommand;
													cmd.CommandText = cmd.CommandText.TrimStart(" \t\r\n".ToCharArray());
													cmd.CommandText = cmd.CommandText.TrimEnd  (" \t\r\n".ToCharArray());
													if ( cmd.CommandText.Length > 0 )
													{
														cmd.ExecuteNonQuery();
													}
												}
											}
											// 11/25/2021 Paul.  REACT Only will live deploy. 
											if ( sSQLFolderType == "Procedures" && !bREACT_ONLY )
											{
												string sSQL;
												sSQL = "select *                       " + ControlChars.CrLf
												     + "  from vwSqlColumns            " + ControlChars.CrLf
												     + " where ObjectName = @OBJECTNAME" + ControlChars.CrLf
												     + "   and ObjectType = 'P'        " + ControlChars.CrLf
												     + " order by colid                " + ControlChars.CrLf;
												using ( IDbCommand cmd = con.CreateCommand() )
												{
													cmd.CommandText = sSQL;
													Sql.AddParameter(cmd, "@OBJECTNAME", Sql.MetadataName(cmd, sPROCEDURE_NAME));
													using ( DbDataAdapter da = dbf.CreateDataAdapter() )
													{
														((IDbDataAdapter)da).SelectCommand = cmd;
														using ( DataTable dt = new DataTable() )
														{
															da.Fill(dt);
															DataRowCollection colRows = dt.Rows;
															StringBuilder sb = new StringBuilder();
															sb.AppendLine("	public partial class SqlProcs");
															sb.AppendLine("	{");
															BuildWrapper(ref sb, sPROCEDURE_NAME, ref colRows, false, false);
															BuildWrapper(ref sb, sPROCEDURE_NAME, ref colRows, false, true );
															BuildWrapper(ref sb, sPROCEDURE_NAME, ref colRows, true , false);
															sb.AppendLine("	}");
															string sCODE_WRAPPER = sb.ToString();
															// 03/07/2010 Paul.  When not using a code-behind, we need to prevent namespace collissions. 
															// 08/25/2010 Paul.  We only need to replace the namespace if using the Live template. 
															if ( !bCREATE_CODE_BEHIND )
															{
																sCODE_WRAPPER = sCODE_WRAPPER.Replace("DbProviderFactory"  , "SplendidCRM.DbProviderFactory"  );
																sCODE_WRAPPER = sCODE_WRAPPER.Replace("DbProviderFactories", "SplendidCRM.DbProviderFactories");
															}
															row["CODE_WRAPPER"] = sCODE_WRAPPER;
														}
													}
												}
											}
										}
										catch(Exception ex)
										{
											sbProgress.AppendLine("<font class=error>" + sNAME + ":" + ex.Message + "</font><br>");
										}
									}
								}
							}
							// 11/26/2021 Paul.  Must also update the security rules for the new module. 
							SqlProcs.spACL_ACTIONS_Initialize();
							sbProgress.AppendLine("spACL_ACTIONS_Initialize<br>");
						}
					}
				// 11/25/2021 Paul.  REACT Only will live deploy. 
				if ( !bREACT_ONLY && Directory.Exists(sWebTemplatesPath) )
				{
					string[] arrWebTemplates = Directory.GetFiles(sWebTemplatesPath);
					foreach ( string sWebTemplate in arrWebTemplates )
					{
						if ( sWebTemplate.IndexOf("$relatedmodule$") >= 0 || sWebTemplate.IndexOf("$relatedtable$") >= 0 )
							continue;
						// 03/07/2010 Paul.  The code-behind files are not used so that the generated files will be immediately accessible. 
						// 08/25/2010 Paul.  We still want to allow code-behind files. 
						//if ( sWebTemplate.EndsWith(".cs") )
						//	continue;
						
						using ( StreamReader sr = new StreamReader(sWebTemplate, enc, true) )
						{
							string sData = sr.ReadToEnd();
							// 09/12/2009 Paul.  If this is an admin module, then place in the Administration namespace. 
							if ( bIS_ADMIN )
								sData = sData.Replace("SplendidCRM.$modulename$", "SplendidCRM.Administration.$modulename$");
							sData = sData.Replace("$modulename$"        , sMODULE_NAME         );
							sData = sData.Replace("$modulenamesingular$", sMODULE_NAME_SINGULAR);
							sData = sData.Replace("$tablename$"         , sTABLE_NAME          );
							sData = sData.Replace("$tablenamesingular$" , sTABLE_NAME_SINGULAR );
							sData = sData.Replace("$firsttextfield$"    , sFIRST_TEXT_FIELD    );

							string sWebTemplateName = Path.GetFileName(sWebTemplate);
							if ( sWebTemplateName.StartsWith("DetailView.ascx") )
							{
								if ( !bINCLUDE_ASSIGNED_USER_ID )
								{
									sData = sData.Replace("Sql.ToGuid(rdr[\"ASSIGNED_USER_ID\"])", "Guid.Empty");
								}
							}
							else if ( sWebTemplateName.StartsWith("ListView.ascx") )
							{
								if ( !bINCLUDE_ASSIGNED_USER_ID )
								{
									sData = sData.Replace("Sql.ToGuid(rdr[\"ASSIGNED_USER_ID\"])", "Guid.Empty");
									// 03/05/2011 Paul. Now that we have added the edit button, we need to remove Assigned User. 
									sData = sData.Replace(", Sql.ToGuid(Eval(\"ASSIGNED_USER_ID\"))", String.Empty);
									// 03/07/2011 Paul.  Remove ASSIGNED_USER_ID from arrSelectFields. 
									sData = sData.Replace("arrSelectFields.Add(\"ASSIGNED_USER_ID\");", String.Empty);
								}
								// 05/13/2016 Paul.  Add Tags module. 
								if ( bINCLUDE_ASSIGNED_USER_ID && bINCLUDE_TEAM_ID )
									sData = sData.Replace("$callmassupdateprocedure$", "SqlProcs.sp" + sTABLE_NAME + "_MassUpdate(sIDs, ctlMassUpdate.ASSIGNED_USER_ID, ctlMassUpdate.PRIMARY_TEAM_ID, ctlMassUpdate.TEAM_SET_LIST, ctlMassUpdate.ADD_TEAM_SET, ctlMassUpdate.TAG_SET_NAME, ctlMassUpdate.ADD_TAG_SET, trn);");
								else if ( bINCLUDE_ASSIGNED_USER_ID )
									sData = sData.Replace("$callmassupdateprocedure$", "SqlProcs.sp" + sTABLE_NAME + "_MassUpdate(sIDs, ctlMassUpdate.ASSIGNED_USER_ID, ctlMassUpdate.TAG_SET_NAME, ctlMassUpdate.ADD_TAG_SET, trn);");
								else if ( bINCLUDE_TEAM_ID )
									sData = sData.Replace("$callmassupdateprocedure$", "SqlProcs.sp" + sTABLE_NAME + "_MassUpdate(sIDs, ctlMassUpdate.PRIMARY_TEAM_ID, ctlMassUpdate.TEAM_SET_LIST, ctlMassUpdate.ADD_TEAM_SET, ctlMassUpdate.TAG_SET_NAME, ctlMassUpdate.ADD_TAG_SET, trn);");
								else  // 10/12/2009 Paul.  Remove insertion if not used. 
									sData = sData.Replace("$callmassupdateprocedure$", "");
							}
							else if ( sWebTemplateName.StartsWith("ListView.ascx.cs") )
							{
								if ( !bINCLUDE_ASSIGNED_USER_ID )
								{
									// 03/07/2011 Paul.  Remove ASSIGNED_USER_ID from arrSelectFields. 
									sData = sData.Replace("arrSelectFields.Add(\"ASSIGNED_USER_ID\");", String.Empty);
								}
							}
							else if ( sWebTemplateName.StartsWith("EditView.ascx") )
							{
								sData = sData.Replace("$callupdateprocedure$", sbCallUpdateProcedure.ToString());
							}
							else if ( sWebTemplateName.StartsWith("NewRecord.ascx") )
							{
								// 06/22/2010 Paul.  The NewRecord controls do not use rowCurrent. 
								sData = sData.Replace("$callupdateprocedure$", sbCallUpdateProcedure.ToString().Replace("rowCurrent,", "null,"));
							}

							// 03/08/2010 Paul.  Allow user to select Live deployment, but we still prefer the code-behind method. 
							if ( !bCREATE_CODE_BEHIND )
							{
								// 03/06/2010 Paul.  Insert the SqlProc wrapper into each file as needed. 
								vwSQLScripts.RowFilter = "FOLDER = 'Procedures'";
								foreach ( DataRowView row in vwSQLScripts )
								{
									string sPROCEDURE_NAME = Sql.ToString(row["PROCEDURE_NAME"]);
									string sCODE_WRAPPER   = Sql.ToString(row["CODE_WRAPPER"  ]);
									if ( !Sql.IsEmptyString(sCODE_WRAPPER) )
									{
										if ( sData.IndexOf("SqlProcs." + sPROCEDURE_NAME) >= 0 )
										{
											sData = sData.Replace("//$sqlprocs$", sCODE_WRAPPER + ControlChars.CrLf + ControlChars.CrLf + "//$sqlprocs$");
										}
									}
								}
							}
							sData = sData.Replace("//$sqlprocs$", String.Empty);
							
							string sWebModuleFile = Path.Combine(sWebModulePath, sWebTemplateName);
							try
							{
								sbProgress.AppendLine(sWebModuleFile + "<br>");
								if ( bOVERWRITE_EXISTING && System.IO.File.Exists(sWebModuleFile) )
									System.IO.File.Delete(sWebModuleFile);
								using(StreamWriter stm = System.IO.File.CreateText(sWebModuleFile))
								{
									stm.Write(sData);
								}
							}
							catch(Exception ex)
							{
								sbProgress.AppendLine("<font class=error>" + sWebTemplate + ":" + ex.Message + "</font><br>");
							}
						}
					}
					
					foreach ( string sRELATED_MODULE in lstRelationships )
					{
						{
							string sRELATED_MODULE_SINGULAR = sRELATED_MODULE;
							string sRELATED_TABLE           = Sql.ToString(Application["Modules." + sRELATED_MODULE + ".TableName"]);
							string sRELATED_TABLE_SINGULAR  = sRELATED_TABLE;
							if ( sRELATED_MODULE_SINGULAR.ToLower().EndsWith("ies") )
								sRELATED_MODULE_SINGULAR = sRELATED_MODULE_SINGULAR.Substring(0, sRELATED_MODULE_SINGULAR.Length-3) + "Y";
							else if ( sRELATED_MODULE_SINGULAR.ToLower().EndsWith("s") )
								sRELATED_MODULE_SINGULAR = sRELATED_MODULE_SINGULAR.Substring(0, sRELATED_MODULE_SINGULAR.Length-1);
							if ( sRELATED_TABLE_SINGULAR.ToLower().EndsWith("ies") )
								sRELATED_TABLE_SINGULAR = sRELATED_TABLE_SINGULAR.Substring(0, sRELATED_TABLE_SINGULAR.Length-3) + "Y";
							else if ( sRELATED_TABLE_SINGULAR.ToLower().EndsWith("s") )
								sRELATED_TABLE_SINGULAR = sRELATED_TABLE_SINGULAR.Substring(0, sRELATED_TABLE_SINGULAR.Length-1);

							foreach ( string sWebTemplate in arrWebTemplates )
							{
								if ( sWebTemplate.IndexOf("$relatedmodule$") < 0 && sWebTemplate.IndexOf("$relatedtable$") < 0 )
									continue;
								// 03/07/2010 Paul.  The code-behind files are not used so that the generated files will be immediately accessible. 
								// 08/25/2010 Paul.  We still want to allow code-behind files. 
								//if ( sWebTemplate.EndsWith(".cs") )
								//	continue;
								
								using ( StreamReader sr = new StreamReader(sWebTemplate, enc, true) )
								{
									string sData = sr.ReadToEnd();
									// 09/12/2009 Paul.  If this is an admin module, then place in the Administration namespace. 
									if ( bIS_ADMIN )
										sData = sData.Replace("SplendidCRM.$modulename$", "SplendidCRM.Administration.$modulename$");
									sData = sData.Replace("$modulename$"                 , sMODULE_NAME            );
									sData = sData.Replace("$modulenamesingular$"         , sMODULE_NAME_SINGULAR   );
									sData = sData.Replace("$tablename$"                  , sTABLE_NAME             );
									sData = sData.Replace("$tablenamesingular$"          , sTABLE_NAME_SINGULAR    );
									sData = sData.Replace("$relatedmodule$"              , sRELATED_MODULE         );
									sData = sData.Replace("$relatedmodulesingular$"      , sRELATED_MODULE_SINGULAR);
									sData = sData.Replace("$relatedtable$"               , sRELATED_TABLE          );
									sData = sData.Replace("$relatedtablesingular$"       , sRELATED_TABLE_SINGULAR );
									
									// 03/03/2011 Paul.  Project and ProjectTask need to be corrected. 
									if ( sWebTemplate.EndsWith("$relatedmodule$.ascx") )
									{
										if ( sRELATED_MODULE == "Project" )
										{
											sData = sData.Replace("~/Project/NewRecord.ascx", "~/Projects/NewRecord.ascx");
										}
										else if ( sRELATED_MODULE == "ProjectTask" )
										{
											sData = sData.Replace("~/ProjectTask/NewRecord.ascx", "~/ProjectTasks/NewRecord.ascx");
										}
									}
									if ( sWebTemplate.EndsWith("$relatedmodule$.ascx.cs") )
									{
										if ( sRELATED_MODULE == "Project" )
										{
											sData = sData.Replace("SplendidCRM.Project.NewRecord", "SplendidCRM.Projects.NewRecord");
										}
										else if ( sRELATED_MODULE == "ProjectTask" )
										{
											sData = sData.Replace("SplendidCRM.ProjectTask.NewRecord", "SplendidCRM.ProjectTasks.NewRecord");
										}
									}
									
									string sWebTemplateName = Path.GetFileName(sWebTemplate);
									sWebTemplateName = sWebTemplateName.Replace("$modulename$"           , sMODULE_NAME            );
									sWebTemplateName = sWebTemplateName.Replace("$modulenamesingular$"   , sMODULE_NAME_SINGULAR   );
									sWebTemplateName = sWebTemplateName.Replace("$tablename$"            , sTABLE_NAME             );
									sWebTemplateName = sWebTemplateName.Replace("$tablenamesingular$"    , sTABLE_NAME_SINGULAR    );
									sWebTemplateName = sWebTemplateName.Replace("$relatedmodule$"        , sRELATED_MODULE         );
									sWebTemplateName = sWebTemplateName.Replace("$relatedmodulesingular$", sRELATED_MODULE_SINGULAR);
									sWebTemplateName = sWebTemplateName.Replace("$relatedtable$"         , sRELATED_TABLE          );
									sWebTemplateName = sWebTemplateName.Replace("$relatedtablesingular$" , sRELATED_TABLE_SINGULAR );
									
									// 03/08/2010 Paul.  Allow user to select Live deployment, but we still prefer the code-behind method. 
									if ( !bCREATE_CODE_BEHIND )
									{
										// 03/06/2010 Paul.  Insert the SqlProc wrapper into each file as needed. 
										vwSQLScripts.RowFilter = "FOLDER = 'Procedures'";
										foreach ( DataRowView row in vwSQLScripts )
										{
											string sPROCEDURE_NAME = Sql.ToString(row["PROCEDURE_NAME"]);
											string sCODE_WRAPPER   = Sql.ToString(row["CODE_WRAPPER"  ]);
											if ( !Sql.IsEmptyString(sCODE_WRAPPER) )
											{
												if ( sData.IndexOf("SqlProcs." + sPROCEDURE_NAME) >= 0 )
												{
													sData = sData.Replace("//$sqlprocs$", sCODE_WRAPPER + ControlChars.CrLf + ControlChars.CrLf + "//$sqlprocs$");
												}
											}
										}
									}
									sData = sData.Replace("//$sqlprocs$", String.Empty);
									
									string sWebModuleFile = Path.Combine(sWebModulePath, sWebTemplateName);
									try
									{
										sbProgress.AppendLine(sWebModuleFile + "<br>");
										if ( bOVERWRITE_EXISTING && System.IO.File.Exists(sWebModuleFile) )
											System.IO.File.Delete(sWebModuleFile);
										using(StreamWriter stm = System.IO.File.CreateText(sWebModuleFile))
										{
											stm.Write(sData);
										}
									}
									catch(Exception ex)
									{
										sbProgress.AppendLine("<font class=error>" + sWebTemplate + ":" + ex.Message + "</font><br>");
									}
								}
							}
						}
					}
				}
					// 03/08/2010 Paul.  Allow user to select Live deployment, but we still prefer the code-behind method. 
					// 11/25/2021 Paul.  REACT Only will live deploy. 
					if ( !bCREATE_CODE_BEHIND || bREACT_ONLY )
					{
						// 03/07/2010 Paul.  After a successful build, we need to reload the cached data. 
						SplendidInit.InitApp();
						SplendidInit.LoadUserPreferences(Security.USER_ID, Sql.ToString(Session["USER_SETTINGS/THEME"]), Sql.ToString(Session["USER_SETTINGS/CULTURE"]));
					}
		}

	}
}

