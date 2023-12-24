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
using System.Collections.Generic;

using Microsoft.AspNetCore.Http;

namespace SplendidCRM
{
	public class SplendidControlThis : SqlObj
	{
		private DataRow         Row      ;
		private DataTable       Table    ;
		private string          Module   ;
		
		public SplendidControlThis(Security Security, SplendidControl Container, string sModule, DataRow Row)
		{
			this.Security  = Security ;
			this.Container = Container;
			this.Module    = sModule  ;
			this.Row       = Row      ;
			if ( Row != null )
				this.Table = Row.Table;
			this.L10n      = Container.GetL10n();
		}

		public SplendidControlThis(Security Security, SplendidControl Container, string sModule, DataTable Table)
		{
			this.Security  = Security ;
			this.Container = Container;
			this.Module    = sModule  ;
			this.Table     = Table    ;
			this.L10n      = Container.GetL10n();
		}

		public object this[string columnName]
		{
			get
			{
				if ( Row != null )
					return Row[columnName];
				return null;
			}
			set
			{
				if ( Row != null )
					Row[columnName] = value;
			}
		}

		// 02/15/2014 Paul.  Provide access to the Request object so that we can determine if the record is new. 
		public HttpRequest Request
		{
			get
			{
				return Container.Request;
			}
		}

		public void AddColumn(string columnName, string typeName)
		{
			if ( Table != null )
			{
				if ( !Table.Columns.Contains(columnName) )
				{
					if ( Sql.IsEmptyString(typeName) )
						Table.Columns.Add(columnName);
					else
						Table.Columns.Add(columnName, Type.GetType(typeName));
				}
			}
		}

		// http://msdn.microsoft.com/en-us/library/system.data.datacolumn.expression(v=VS.80).aspx
		public void AddColumnExpression(string columnName, string typeName, string sExpression)
		{
			if ( Table != null )
			{
				if ( !Table.Columns.Contains(columnName) )
				{
					Table.Columns.Add(columnName, Type.GetType(typeName), sExpression);
				}
			}
		}

		// 07/05/2012 Paul.  Provide access to the current user. 
		public Guid USER_ID()
		{
			return Security.USER_ID;
		}

		public string USER_NAME()
		{
			return Security.USER_NAME;
		}

		public string FULL_NAME()
		{
			return Security.FULL_NAME;
		}

		public Guid TEAM_ID()
		{
			return Security.TEAM_ID;
		}

		public string TEAM_NAME()
		{
			return Security.TEAM_NAME;
		}

		// 05/12/2013 Paul.  Provide a way to decrypt inside a business rule.  
		// The business rules do not have access to the config variables, so the Guid values will need to be hard-coded in the rule. 
		public string DecryptPassword(string sPASSWORD, Guid gKEY, Guid gIV)
		{
			return Security.DecryptPassword(sPASSWORD, gKEY, gIV);
		}
	}

	public class SplendidWizardThis : SqlObj
	{
		private DataRow         Row              ;
		private string          Module           ;
		private Guid            gASSIGNED_USER_ID;
		
		// 04/27/2018 Paul.  We need to be able to generate an error message. 
		public SplendidWizardThis(Security Security, SplendidControl Container, string sModule, DataRow Row)
		{
			this.Security          = Security  ;
			this.Container         = Container ;
			this.L10n              = Container.GetL10n();
			this.Row               = Row       ;
			this.Module            = sModule   ;
			this.gASSIGNED_USER_ID = Guid.Empty;
			if ( Row.Table != null && Row.Table.Columns.Contains("ASSIGNED_USER_ID") )
				gASSIGNED_USER_ID = Sql.ToGuid(Row["ASSIGNED_USER_ID"]);
		}
		
		public object this[string columnName]
		{
			get
			{
				bool bIsReadable  = true;
				if ( SplendidInit.bEnableACLFieldSecurity && !Sql.IsEmptyString(columnName) )
				{
					Security.ACL_FIELD_ACCESS acl = Security.GetUserFieldSecurity(Module, columnName, gASSIGNED_USER_ID);
					bIsReadable  = acl.IsReadable();
				}
				if ( bIsReadable )
					return Row[columnName];
				else
					return DBNull.Value;
			}
			set
			{
				bool bIsWriteable = true;
				if ( SplendidInit.bEnableACLFieldSecurity )
				{
					Security.ACL_FIELD_ACCESS acl = Security.GetUserFieldSecurity(Module, columnName, gASSIGNED_USER_ID);
					bIsWriteable = acl.IsWriteable();
				}
				if ( bIsWriteable )
					Row[columnName] = value;
			}
		}

		public string ListTerm(string sListName, string oField)
		{
			return Sql.ToString(L10n.Term(sListName, oField));
		}

		public string Term(string sEntryName)
		{
			return L10n.Term(sEntryName);
		}

		// 07/05/2012 Paul.  Provide access to the current user. 
		public Guid USER_ID()
		{
			return Security.USER_ID;
		}

		public string USER_NAME()
		{
			return Security.USER_NAME;
		}

		public string FULL_NAME()
		{
			return Security.FULL_NAME;
		}

		public Guid TEAM_ID()
		{
			return Security.TEAM_ID;
		}

		public string TEAM_NAME()
		{
			return Security.TEAM_NAME;
		}
	}

	// 09/17/2013 Paul.  Add Business Rules to import. 
	public class SplendidImportThis : SqlObj
	{
		private DataRow         Row              ;
		private IDbCommand      Import           ;
		private IDbCommand      ImportCSTM       ;
		private string          Module           ;
		private Guid            gASSIGNED_USER_ID;
		
		// 04/27/2018 Paul.  We need to be able to generate an error message. 
		public SplendidImportThis(Security Security, SplendidControl Container, string sModule, DataRow Row, IDbCommand cmdImport, IDbCommand cmdImportCSTM)
		{
			this.Security          = Security     ;
			this.Container         = Container    ;
			this.L10n              = Container.GetL10n();
			this.Row               = Row          ;
			this.Import            = cmdImport    ;
			this.ImportCSTM        = cmdImportCSTM;
			this.Module            = sModule      ;
			this.gASSIGNED_USER_ID = Guid.Empty   ;
			
			IDbDataParameter par = Sql.FindParameter(cmdImport, "ASSIGNED_USER_ID");
			if ( par != null )
				gASSIGNED_USER_ID = Sql.ToGuid(par.Value);
		}
		
		public object this[string columnName]
		{
			get
			{
				bool bIsReadable  = true;
				if ( SplendidInit.bEnableACLFieldSecurity && !Sql.IsEmptyString(columnName) )
				{
					Security.ACL_FIELD_ACCESS acl = Security.GetUserFieldSecurity(Module, columnName, gASSIGNED_USER_ID);
					bIsReadable  = acl.IsReadable();
				}
				if ( bIsReadable )
				{
					IDbDataParameter par = Sql.FindParameter(Import, columnName);
					if ( par != null )
					{
						return par.Value;
					}
					else if ( ImportCSTM != null )
					{
						par = Sql.FindParameter(ImportCSTM, columnName);
						if ( par != null )
							return par.Value;
					}
				}
				return DBNull.Value;
			}
			set
			{
				bool bIsWriteable = true;
				if ( SplendidInit.bEnableACLFieldSecurity )
				{
					Security.ACL_FIELD_ACCESS acl = Security.GetUserFieldSecurity(Module, columnName, gASSIGNED_USER_ID);
					bIsWriteable = acl.IsWriteable();
				}
				if ( bIsWriteable )
				{
					IDbDataParameter par = Sql.FindParameter(Import, columnName);
					if ( par != null )
					{
						Sql.SetParameter(par, value);
					}
					if ( ImportCSTM != null )
					{
						// 09/17/2013 Paul.  If setting the ID, then also set the related custom field ID. 
						if ( String.Compare(columnName, "ID", true) == 0 )
							columnName = "ID_C";
						par = Sql.FindParameter(ImportCSTM, columnName);
						if ( par != null )
							Sql.SetParameter(par, value);
					}
					// 09/17/2013 Paul.  The Row is displayed in the Results tab while the parameters are used to update the database. 
					Row[columnName] = value;
				}
			}
		}

		public string ListTerm(string sListName, string oField)
		{
			return Sql.ToString(L10n.Term(sListName, oField));
		}

		public string Term(string sEntryName)
		{
			return L10n.Term(sEntryName);
		}

		// 07/05/2012 Paul.  Provide access to the current user. 
		public Guid USER_ID()
		{
			return Security.USER_ID;
		}

		public string USER_NAME()
		{
			return Security.USER_NAME;
		}

		public string FULL_NAME()
		{
			return Security.FULL_NAME;
		}

		public Guid TEAM_ID()
		{
			return Security.TEAM_ID;
		}

		public string TEAM_NAME()
		{
			return Security.TEAM_NAME;
		}
	}

	public class SplendidReportThis : SqlObj
	{
		private DataRow         Row              ;
		private DataTable       Table            ;
		private string          Module           ;
		private Guid            gASSIGNED_USER_ID;
		
		public SplendidReportThis(Security Security, SplendidControl Container, string sModule, DataRow Row)
		{
			this.Security            = Security           ;
			this.Container           = Container          ;
			this.L10n                = Container.GetL10n();

			this.Module            = sModule    ;
			this.Row               = Row        ;
			this.gASSIGNED_USER_ID = Guid.Empty ;
			if ( Row != null )
			{
				this.Table = Row.Table;
				if ( Table != null && Table.Columns.Contains("ASSIGNED_USER_ID") )
					gASSIGNED_USER_ID = Sql.ToGuid(Row["ASSIGNED_USER_ID"]);
			}
		}

		public SplendidReportThis(Security Security, SplendidControl Container, string sModule, DataTable Table)
		{
			this.Security            = Security           ;
			this.Container           = Container          ;
			this.L10n                = Container.GetL10n();

			this.Module            = sModule    ;
			this.Table             = Table      ;
			this.gASSIGNED_USER_ID = Guid.Empty ;
		}

		public object this[string columnName]
		{
			get
			{
				bool bIsReadable  = true;
				if ( SplendidInit.bEnableACLFieldSecurity && !Sql.IsEmptyString(columnName) )
				{
					Security.ACL_FIELD_ACCESS acl = Security.GetUserFieldSecurity(Module, columnName, gASSIGNED_USER_ID);
					bIsReadable  = acl.IsReadable();
				}
				if ( bIsReadable )
					return Row[columnName];
				else
					return DBNull.Value;
			}
			set
			{
				bool bIsWriteable = true;
				if ( SplendidInit.bEnableACLFieldSecurity )
				{
					Security.ACL_FIELD_ACCESS acl = Security.GetUserFieldSecurity(Module, columnName, gASSIGNED_USER_ID);
					bIsWriteable = acl.IsWriteable();
				}
				if ( bIsWriteable )
					Row[columnName] = value;
			}
		}

		public void AddColumn(string columnName, string typeName)
		{
			if ( Table != null )
			{
				if ( !Table.Columns.Contains(columnName) )
				{
					if ( Sql.IsEmptyString(typeName) )
						Table.Columns.Add(columnName);
					else
						Table.Columns.Add(columnName, Type.GetType(typeName));
				}
			}
		}

		// http://msdn.microsoft.com/en-us/library/system.data.datacolumn.expression(v=VS.80).aspx
		public void AddColumnExpression(string columnName, string typeName, string sExpression)
		{
			if ( Table != null )
			{
				if ( !Table.Columns.Contains(columnName) )
				{
					Table.Columns.Add(columnName, Type.GetType(typeName), sExpression);
				}
			}
		}

		public string ListTerm(string sListName, string oField)
		{
			// 12/04/2010 Paul.  We need to use the static version of Term as a report can get rendered inside a workflow, which has issues accessing the context. 
			//return Sql.ToString(L10N.Term(Application, L10n.NAME, sListName, oField));
			// 08/12/2023 Paul.  Core should allow normal access. 
			return Sql.ToString(L10n.Term(sListName, oField));
		}

		public string Term(string sEntryName)
		{
			// 12/04/2010 Paul.  We need to use the static version of Term as a report can get rendered inside a workflow, which has issues accessing the context. 
			//return L10N.Term(Application, L10n.NAME, sEntryName);
			// 08/12/2023 Paul.  Core should allow normal access. 
			return L10n.Term(sEntryName);
		}

		// 11/10/2010 Paul.  Throwing an exception will be the preferred method of displaying an error. 
		public void Throw(string sMessage)
		{
			throw(new Exception(sMessage));
		}

		public bool UserIsAdmin()
		{
			return Security.IS_ADMIN;
		}

		public int UserModuleAccess(string sACCESS_TYPE)
		{
			return Security.GetUserAccess(Module, sACCESS_TYPE);
		}

		public bool UserRoleAccess(string sROLE_NAME)
		{
			return Security.GetACLRoleAccess(sROLE_NAME);
		}

		public bool UserTeamAccess(string sTEAM_NAME)
		{
			return Security.GetTeamAccess(sTEAM_NAME);
		}

		public bool UserFieldIsReadable(string sFIELD_NAME, Guid gASSIGNED_USER_ID)
		{
			Security.ACL_FIELD_ACCESS acl = Security.GetUserFieldSecurity(Module, sFIELD_NAME, gASSIGNED_USER_ID);
			return acl.IsReadable();
		}

		public bool UserFieldIsWriteable(string sFIELD_NAME, Guid gASSIGNED_USER_ID)
		{
			Security.ACL_FIELD_ACCESS acl = Security.GetUserFieldSecurity(Module, sFIELD_NAME, gASSIGNED_USER_ID);
			return acl.IsWriteable();
		}

		// 07/05/2012 Paul.  Provide access to the current user. 
		public Guid USER_ID()
		{
			return Security.USER_ID;
		}

		public string USER_NAME()
		{
			return Security.USER_NAME;
		}

		public string FULL_NAME()
		{
			return Security.FULL_NAME;
		}

		public Guid TEAM_ID()
		{
			return Security.TEAM_ID;
		}

		public string TEAM_NAME()
		{
			return Security.TEAM_NAME;
		}
	}

	// 12/12/2012 Paul.  For security reasons, we want to restrict the data types available to the rules wizard. 
	// http://www.codeproject.com/Articles/12675/How-to-reuse-the-Windows-Workflow-Foundation-WF-co
	// 09/16/2013 Paul.  ITypeProvider is obsolete in .NET 4.5, but we have not found the alternative. 
	public class SplendidRulesTypeProvider
	{
		public event EventHandler TypeLoadErrorsChanged;
		public event EventHandler TypesChanged         ;
		private Dictionary<string, Type> availableTypes;
		private Dictionary<object, Exception> typeErrors;
		private List<System.Reflection.Assembly> availableAssemblies;

		public SplendidRulesTypeProvider()
		{
			typeErrors     = new Dictionary<object, Exception>();
			availableAssemblies = new List<System.Reflection.Assembly>();
			availableAssemblies.Add(this.GetType().Assembly);
			
			availableTypes = new Dictionary<string, Type>();
			availableTypes.Add(typeof(System.Boolean ).FullName, typeof(System.Boolean ));
			availableTypes.Add(typeof(System.Byte    ).FullName, typeof(System.Byte    ));
			availableTypes.Add(typeof(System.Char    ).FullName, typeof(System.Char    ));
			availableTypes.Add(typeof(System.DateTime).FullName, typeof(System.DateTime));
			availableTypes.Add(typeof(System.Decimal ).FullName, typeof(System.Decimal ));
			availableTypes.Add(typeof(System.Double  ).FullName, typeof(System.Double  ));
			availableTypes.Add(typeof(System.Guid    ).FullName, typeof(System.Guid    ));
			availableTypes.Add(typeof(System.Int16   ).FullName, typeof(System.Int16   ));
			availableTypes.Add(typeof(System.Int32   ).FullName, typeof(System.Int32   ));
			availableTypes.Add(typeof(System.Int64   ).FullName, typeof(System.Int64   ));
			availableTypes.Add(typeof(System.SByte   ).FullName, typeof(System.SByte   ));
			availableTypes.Add(typeof(System.Single  ).FullName, typeof(System.Single  ));
			availableTypes.Add(typeof(System.String  ).FullName, typeof(System.String  ));
			availableTypes.Add(typeof(System.TimeSpan).FullName, typeof(System.TimeSpan));
			availableTypes.Add(typeof(System.UInt16  ).FullName, typeof(System.UInt16  ));
			availableTypes.Add(typeof(System.UInt32  ).FullName, typeof(System.UInt32  ));
			availableTypes.Add(typeof(System.UInt64  ).FullName, typeof(System.UInt64  ));
			availableTypes.Add(typeof(System.DBNull  ).FullName, typeof(System.DBNull  ));
// 11/03/2021 Paul.  ASP.Net components are not needed. 
#if !ReactOnlyUI
			// 03/11/2014 Paul.  Provide a way to control the dynamic buttons. 
			availableTypes.Add(typeof(SafeDynamicButtons).FullName, typeof(SafeDynamicButtons));
#endif
			// 12/12/2012 Paul.  Use TypesChanged to avoid a compiler warning; 
			if ( TypesChanged != null )
				TypesChanged(this, null);
		}

		public Type GetType(string name, bool throwOnError)
		{
			if ( String.IsNullOrEmpty(name) )
			{
				return null;
			}

			if ( availableTypes.ContainsKey(name) )
			{
				Type type = availableTypes[name];
				return type;
			}
			else
			{
				if ( !typeErrors.ContainsKey(name) )
				{
					typeErrors.Add(name, new Exception("SplendidRulesTypeProvider: " + name + " is not a supported data type. "));
				}
				if ( throwOnError )
				{
					throw new TypeLoadException();
				}
				else
				{
					if ( TypeLoadErrorsChanged != null )
					{
						try
						{
							EventArgs args = new EventArgs();
							TypeLoadErrorsChanged(this, args);
						}
						catch
						{
						}
					}
					return null;
				}
			}
		}

		public Type GetType(string name)
		{
			return GetType(name, false);
		}

		public Type[] GetTypes() 
		{
			Type[] result = new Type[availableTypes.Count];
			availableTypes.Values.CopyTo(result, 0);
			return result;
		}

		public System.Reflection.Assembly LocalAssembly
		{
			get { return this.GetType().Assembly; }
		}

		public IDictionary<object, Exception> TypeLoadErrors
		{
			get { return typeErrors; }
		}

		public ICollection<System.Reflection.Assembly> ReferencedAssemblies
		{
			get { return availableAssemblies; }
		}
	}

	/// <summary>
	/// Summary description for RulesUtil.
	/// </summary>
	public class RulesUtil
	{
		public static RuleSet Deserialize(string sXOML)
		{
			RuleSet rules = null;
			using ( StringReader stm = new StringReader(sXOML) )
			{
				using ( XmlTextReader xrdr = new XmlTextReader(stm) )
				{
					WorkflowMarkupSerializer serializer = new WorkflowMarkupSerializer();
					rules = (RuleSet) serializer.Deserialize(xrdr);
				}
			}
			return rules;
		}

		public static string Serialize(RuleSet rules)
		{
			StringBuilder sbXOML = new StringBuilder();
			using ( StringWriter wtr = new StringWriter(sbXOML, System.Globalization.CultureInfo.InvariantCulture) )
			{
				using ( XmlTextWriter xwtr = new XmlTextWriter(wtr) )
				{
					WorkflowMarkupSerializer serializer = new WorkflowMarkupSerializer();
					serializer.Serialize(xwtr, rules);
				}
			}
			return sbXOML.ToString();
		}

		// 12/12/2012 Paul.  For security reasons, we want to restrict the data types available to the rules wizard. 
		public static void RulesValidate(Guid gID, string sRULE_NAME, int nPRIORITY, string sREEVALUATION, bool bACTIVE, string sCONDITION, string sTHEN_ACTIONS, string sELSE_ACTIONS, Type thisType, SplendidRulesTypeProvider typeProvider)
		{
			RuleSet        rules      = new RuleSet("RuleSet 1");
			RuleValidation validation = new RuleValidation(thisType, typeProvider);
			RulesParser    parser     = new RulesParser(validation);
			RuleExpressionCondition condition      = parser.ParseCondition    (sCONDITION   );
			List<RuleAction>        lstThenActions = parser.ParseStatementList(sTHEN_ACTIONS);
			List<RuleAction>        lstElseActions = parser.ParseStatementList(sELSE_ACTIONS);

			Rule r = new Rule(sRULE_NAME, condition, lstThenActions, lstElseActions);
			r.Priority = nPRIORITY;
			r.Active   = bACTIVE  ;
			//r.ReevaluationBehavior = (RuleReevaluationBehavior) Enum.Parse(typeof(RuleReevaluationBehavior), sREEVALUATION);
			// 12/04/2010 Paul.  Play it safe and never-reevaluate. 
			r.ReevaluationBehavior = RuleReevaluationBehavior.Never;
			rules.Rules.Add(r);
			rules.Validate(validation);
			if ( validation.Errors.Count > 0 )
			{
				throw(new Exception(GetValidationErrors(validation)));
			}
		}

		public static string GetValidationErrors(RuleValidation validation)
		{
			StringBuilder sbErrors = new StringBuilder();
			foreach ( ValidationError err in validation.Errors )
			{
				sbErrors.AppendLine(err.ErrorText);
			}
			return sbErrors.ToString();
		}

		public static RuleSet BuildRuleSet(DataTable dtRules, RuleValidation validation)
		{
			RuleSet        rules = new RuleSet("RuleSet 1");
			RulesParser    parser = new RulesParser(validation);

			DataView vwRules = new DataView(dtRules);
			vwRules.RowFilter = "ACTIVE = 1";
			vwRules.Sort      = "PRIORITY asc";
			foreach ( DataRowView row in vwRules )
			{
				string sRULE_NAME    = Sql.ToString (row["RULE_NAME"   ]);
				int    nPRIORITY     = Sql.ToInteger(row["PRIORITY"    ]);
				string sREEVALUATION = Sql.ToString (row["REEVALUATION"]);
				bool   bACTIVE       = Sql.ToBoolean(row["ACTIVE"      ]);
				string sCONDITION    = Sql.ToString (row["CONDITION"   ]);
				string sTHEN_ACTIONS = Sql.ToString (row["THEN_ACTIONS"]);
				string sELSE_ACTIONS = Sql.ToString (row["ELSE_ACTIONS"]);
				
				RuleExpressionCondition condition      = parser.ParseCondition    (sCONDITION   );
				List<RuleAction>        lstThenActions = parser.ParseStatementList(sTHEN_ACTIONS);
				List<RuleAction>        lstElseActions = parser.ParseStatementList(sELSE_ACTIONS);
				Rule r = new Rule(sRULE_NAME, condition, lstThenActions, lstElseActions);
				r.Priority = nPRIORITY;
				r.Active   = bACTIVE  ;
				//r.ReevaluationBehavior = (RuleReevaluationBehavior) Enum.Parse(typeof(RuleReevaluationBehavior), sREEVALUATION);
				// 12/04/2010 Paul.  Play it safe and never-reevaluate. 
				r.ReevaluationBehavior = RuleReevaluationBehavior.Never;
				rules.Rules.Add(r);
			}
			rules.Validate(validation);
			if ( validation.Errors.Count > 0 )
			{
				throw(new Exception(RulesUtil.GetValidationErrors(validation)));
			}
			return rules;
		}

		// 08/16/2017 Paul.  Single action business rule. 
		public static RuleSet BuildRuleSet(string sTHEN_ACTIONS, RuleValidation validation)
		{
			RuleSet        rules = new RuleSet("RuleSet 1");
			RulesParser    parser = new RulesParser(validation);
			
			string sRULE_NAME    = "Rule 1";
			string sCONDITION    = "true";
			string sELSE_ACTIONS = String.Empty;
			
			RuleExpressionCondition condition      = parser.ParseCondition    (sCONDITION   );
			List<RuleAction>        lstThenActions = parser.ParseStatementList(sTHEN_ACTIONS);
			List<RuleAction>        lstElseActions = parser.ParseStatementList(sELSE_ACTIONS);
			Rule r = new Rule(sRULE_NAME, condition, lstThenActions, lstElseActions);
			r.Priority = 1;
			r.Active   = true;
			r.ReevaluationBehavior = RuleReevaluationBehavior.Never;
			rules.Rules.Add(r);
			
			rules.Validate(validation);
			if ( validation.Errors.Count > 0 )
			{
				throw(new Exception(RulesUtil.GetValidationErrors(validation)));
			}
			return rules;
		}

		// 06/02/2021 Paul.  React client needs to share code. 
		public static DataTable BuildRuleDataTable(Dictionary<string, object> dictRulesXml)
		{
			DataTable dtRules = new DataTable();
			DataColumn colID           = new DataColumn("ID"          , typeof(System.Guid   ));
			DataColumn colRULE_NAME    = new DataColumn("RULE_NAME"   , typeof(System.String ));
			DataColumn colPRIORITY     = new DataColumn("PRIORITY"    , typeof(System.Int32  ));
			DataColumn colREEVALUATION = new DataColumn("REEVALUATION", typeof(System.String ));
			DataColumn colACTIVE       = new DataColumn("ACTIVE"      , typeof(System.Boolean));
			DataColumn colCONDITION    = new DataColumn("CONDITION"   , typeof(System.String ));
			DataColumn colTHEN_ACTIONS = new DataColumn("THEN_ACTIONS", typeof(System.String ));
			DataColumn colELSE_ACTIONS = new DataColumn("ELSE_ACTIONS", typeof(System.String ));
			dtRules.Columns.Add(colID          );
			dtRules.Columns.Add(colRULE_NAME   );
			dtRules.Columns.Add(colPRIORITY    );
			dtRules.Columns.Add(colREEVALUATION);
			dtRules.Columns.Add(colACTIVE      );
			dtRules.Columns.Add(colCONDITION   );
			dtRules.Columns.Add(colTHEN_ACTIONS);
			dtRules.Columns.Add(colELSE_ACTIONS);
			if ( dictRulesXml != null )
			{
				if ( dictRulesXml.ContainsKey("NewDataSet") )
				{
					Dictionary<string, object> dictNewDataSet = dictRulesXml["NewDataSet"] as Dictionary<string, object>;
					if ( dictNewDataSet != null )
					{
						if ( dictNewDataSet.ContainsKey("Table1") )
						{
							System.Collections.ArrayList lstTable1 = dictNewDataSet["Table1"] as System.Collections.ArrayList;
							if ( lstTable1 != null )
							{
								foreach ( Dictionary<string, object> dictRule in lstTable1 )
								{
									DataRow row = dtRules.NewRow();
									dtRules.Rows.Add(row);
									row["ID"          ] = (dictRule.ContainsKey("ID"          ) ? Sql.ToString(dictRule["ID"          ]) : String.Empty);
									row["RULE_NAME"   ] = (dictRule.ContainsKey("RULE_NAME"   ) ? Sql.ToString(dictRule["RULE_NAME"   ]) : String.Empty);
									row["PRIORITY"    ] = (dictRule.ContainsKey("PRIORITY"    ) ? Sql.ToString(dictRule["PRIORITY"    ]) : String.Empty);
									row["REEVALUATION"] = (dictRule.ContainsKey("REEVALUATION") ? Sql.ToString(dictRule["REEVALUATION"]) : String.Empty);
									row["ACTIVE"      ] = (dictRule.ContainsKey("ACTIVE"      ) ? Sql.ToString(dictRule["ACTIVE"      ]) : String.Empty);
									row["CONDITION"   ] = (dictRule.ContainsKey("CONDITION"   ) ? Sql.ToString(dictRule["CONDITION"   ]) : String.Empty);
									row["THEN_ACTIONS"] = (dictRule.ContainsKey("THEN_ACTIONS") ? Sql.ToString(dictRule["THEN_ACTIONS"]) : String.Empty);
									row["ELSE_ACTIONS"] = (dictRule.ContainsKey("ELSE_ACTIONS") ? Sql.ToString(dictRule["ELSE_ACTIONS"]) : String.Empty);
								}
							}
						}
					}
				}
			}
			return dtRules;
		}
	}
}

