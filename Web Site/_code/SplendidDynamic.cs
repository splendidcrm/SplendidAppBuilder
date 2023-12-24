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
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
// 11/03/2021 Paul.  ASP.Net components are not needed. 
#if !ReactOnlyUI
using SplendidCRM._controls;
// 09/18/2011 Paul.  Upgrade to CKEditor 3.6.2. 
using CKEditor.NET;
using AjaxControlToolkit;
using System.Workflow.Activities.Rules;
#endif

namespace SplendidCRM
{
	/// <summary>
	/// Summary description for SplendidDynamic.
	/// </summary>
	public class SplendidDynamic
	{
		private HttpApplicationState Application        = new HttpApplicationState();
		private HttpSessionState     Session            ;
		private Security             Security           ;
		private Currency             C10n               = new Currency();
		private SplendidCache        SplendidCache      ;
		private SplendidControl      Container          ;
		private SplendidCRM.Crm.Modules          Modules         ;

		public SplendidDynamic(HttpSessionState Session, Security Security, SplendidCache SplendidCache, SplendidControl Container, SplendidCRM.Crm.Modules Modules)
		{
			this.Session             = Session            ;
			this.Security            = Security           ;
			this.SplendidCache       = SplendidCache      ;
			this.Container           = Container          ;
			this.Modules             = Modules            ;
		}

		// 05/28/2015 Paul.  The stacked layout started with the Seven theme. 
		public static bool StackedLayout(string sTheme, string sViewName)
		{
			return (sTheme == "Seven" && !sViewName.EndsWith(".Preview"));
		}

		public static bool StackedLayout(string sTheme)
		{
			return (sTheme == "Seven");
		}

// 11/03/2021 Paul.  ASP.Net components are not needed. 
#if !ReactOnlyUI
		public static void AppendGridColumns(string sGRID_NAME, DataGrid grd)
		{
			AppendGridColumns(sGRID_NAME, grd, null);
		}
#endif

		// 09/23/2015 Paul.  Need to include the data grid fields as it will be bound using the same data set. 
		public string ExportGridColumns(string sGRID_NAME, UniqueStringCollection arrDataGridSelectedFields)
		{
			StringBuilder sbSQL = new StringBuilder();
			UniqueStringCollection arrSelectFields = new UniqueStringCollection();
			if ( arrDataGridSelectedFields != null )
			{
				foreach ( string sField in arrDataGridSelectedFields )
				{
					arrSelectFields.Add(sField);
				}
			}
			// 05/03/2011 Paul.  Always include the ID as it might be used by the Export code to filter by selected items. 
			arrSelectFields.Add("ID");
			GridColumns(sGRID_NAME, arrSelectFields, null);
			// 04/20/2011 Paul.  If there are no fields in the GridView.Export, then return all fields (*). 
			if ( arrSelectFields.Count > 0 )
			{
				foreach ( string sField in arrSelectFields )
				{
					if ( sbSQL.Length > 0 )
						sbSQL.Append("     , ");
					sbSQL.AppendLine(sField);
				}
			}
			else
			{
				sbSQL.AppendLine("*");
			}
			return sbSQL.ToString();
		}

		// 01/02/2020 Paul.  We need to be able to skip fields. 
		public string ExportGridColumns(string sGRID_NAME, UniqueStringCollection arrDataGridSelectFields, StringCollection arrSkippedFields)
		{
			StringBuilder sbSQL = new StringBuilder();
			UniqueStringCollection arrSelectFields = new UniqueStringCollection();
			foreach ( string sField in arrDataGridSelectFields )
			{
				arrSelectFields.Add(sField);
			}
			// 05/03/2011 Paul.  Always include the ID as it might be used by the Export code to filter by selected items. 
			arrSelectFields.Add("ID");
			GridColumns(sGRID_NAME, arrSelectFields, arrSkippedFields);
			// 04/20/2011 Paul.  If there are no fields in the GridView.Export, then return all fields (*). 
			if ( arrSelectFields.Count > 0 )
			{
				foreach ( string sField in arrSelectFields )
				{
					if ( sbSQL.Length > 0 )
						sbSQL.Append("     , ");
					sbSQL.AppendLine(sField);
				}
			}
			else
			{
				sbSQL.AppendLine("*");
			}
			return sbSQL.ToString();
		}

		// 01/02/2020 Paul.  We need to be able to specify a prefix. 
		public string ExportGridColumns(string sGRID_NAME, UniqueStringCollection arrDataGridSelectFields, string sTABLE_PREFIX, StringCollection arrSkippedFields)
		{
			StringBuilder sbSQL = new StringBuilder();
			UniqueStringCollection arrSelectFields = new UniqueStringCollection();
			foreach ( string sField in arrDataGridSelectFields )
			{
				arrSelectFields.Add(sField);
			}
			// 05/03/2011 Paul.  Always include the ID as it might be used by the Export code to filter by selected items. 
			arrSelectFields.Add("ID");
			GridColumns(sGRID_NAME, arrSelectFields, arrSkippedFields);
			// 04/20/2011 Paul.  If there are no fields in the GridView.Export, then return all fields (*). 
			if ( arrSelectFields.Count > 0 )
			{
				foreach ( string sField in arrSelectFields )
				{
					if ( sbSQL.Length > 0 )
						sbSQL.Append("     , ");
					// 09/23/2015 Paul.  Special exception. 
					if ( sField == "FAVORITE_RECORD_ID" )
						sbSQL.AppendLine(sField);
					else
						sbSQL.AppendLine(sTABLE_PREFIX + "." + sField);
				}
			}
			else
			{
				sbSQL.AppendLine("*");
			}
			return sbSQL.ToString();
		}

		public void SearchGridColumns(string sGRID_NAME, UniqueStringCollection arrSelectFields)
		{
			StringCollection arrSkippedFields = new StringCollection();
			arrSkippedFields.Add("USER_NAME"    );
			arrSkippedFields.Add("ASSIGNED_TO"  );
			arrSkippedFields.Add("CREATED_BY"   );
			arrSkippedFields.Add("MODIFIED_BY"  );
			arrSkippedFields.Add("DATE_ENTERED" );
			arrSkippedFields.Add("DATE_MODIFIED");
			arrSkippedFields.Add("TEAM_NAME"    );
			arrSkippedFields.Add("TEAM_SET_NAME");
			// 05/15/2016 Paul.  Don't need to search ASSIGNED_TO_NAME. 
			arrSkippedFields.Add("ASSIGNED_TO_NAME");
			GridColumns(sGRID_NAME, arrSelectFields, arrSkippedFields);
			// 10/03/2018 Paul.  Remove an empty field.  This can occur if Hover field is used in Search layout. 
			arrSelectFields.Remove(String.Empty);
		}

		// 04/20/2011 Paul.  Create a new method so that we can get export field. 
		public void GridColumns(string sGRID_NAME, UniqueStringCollection arrSelectFields, StringCollection arrSkippedFields)
		{
			// 05/10/2016 Paul.  The User Primary Role is used with role-based views. 
			DataTable dt = SplendidCache.GridViewColumns(sGRID_NAME, Security.PRIMARY_ROLE_NAME);
			if ( dt != null )
			{
				// 01/18/2010 Paul.  To apply ACL Field Security, we need to know if the Module Name, which we will extract from the EditView Name. 
				string sMODULE_NAME = String.Empty;
				string[] arrGRID_NAME = sGRID_NAME.Split('.');
				if ( arrGRID_NAME.Length > 0 )
				{
					if ( arrGRID_NAME[0] == "ListView" || arrGRID_NAME[0] == "PopupView" || arrGRID_NAME[0] == "Activities" )
						sMODULE_NAME = arrGRID_NAME[0];
					// 01/18/2010 Paul.  A sub-panel should apply the access rules of the related module. 
					else if ( Sql.ToBoolean(Application["Modules." + arrGRID_NAME[1] + ".Valid"]) )
						sMODULE_NAME = arrGRID_NAME[1];
					else
						sMODULE_NAME = arrGRID_NAME[0];
				}
				foreach(DataRow row in dt.Rows)
				{
					string sCOLUMN_TYPE = Sql.ToString (row["COLUMN_TYPE"]);
					string sDATA_FIELD  = Sql.ToString (row["DATA_FIELD" ]);
					string sDATA_FORMAT = Sql.ToString (row["DATA_FORMAT"]);
					string sMODULE_TYPE = Sql.ToString (row["MODULE_TYPE"]);
					
					// 04/20/2011 Paul.  Export requests will not exclude any fields. 
					if ( arrSkippedFields != null )
					{
						if ( arrSkippedFields.Contains(sDATA_FIELD) || sDATA_FIELD.EndsWith("_ID") || sDATA_FIELD.EndsWith("_CURRENCY") )
							continue;
					}
					
					// 01/18/2010 Paul.  A field is either visible or not.  At this time, we will not only show a field to its owner. 
					bool bIsReadable  = true;
					if ( SplendidInit.bEnableACLFieldSecurity && !Sql.IsEmptyString(sDATA_FIELD) )
					{
						Security.ACL_FIELD_ACCESS acl = Security.GetUserFieldSecurity(sMODULE_NAME, sDATA_FIELD, Guid.Empty);
						bIsReadable  = acl.IsReadable();
					}
					
					if ( bIsReadable )
					{
						if ( String.Compare(sCOLUMN_TYPE, "TemplateColumn", true) == 0 )
						{
							if ( String.Compare(sDATA_FORMAT, "HyperLink", true) == 0 )
							{
								if ( !Sql.IsEmptyString(sDATA_FIELD) )
								{
									// 02/26/2018 Paul.  There is a special case where we have a custom field module lookup. 
									// 07/12/2018 Paul.  Use Contains instead of ends with. 
									if ( sGRID_NAME.Contains(".Export") && !Sql.IsEmptyString(sMODULE_TYPE) && sDATA_FIELD.EndsWith("_ID_C") )
									{
										string sSubQueryTable = Modules.TableName(sMODULE_TYPE);
										// 02/26/2018 Paul.  Top 1 will not work in Oracle, but this will have to be a known limitation. 
										string sSubQueryField = "(select top 1 NAME from vw" + sSubQueryTable + " where vw" + sSubQueryTable + ".ID = " + sDATA_FIELD + ") as " + sDATA_FIELD;
										if ( arrSelectFields.Contains(sDATA_FIELD) )
										{
											arrSelectFields.Remove(sDATA_FIELD);
											arrSelectFields.Add(sSubQueryField);
										}
										else
										{
											arrSelectFields.Add(sSubQueryField);
										}
									}
									else
									{
										arrSelectFields.Add(sDATA_FIELD);
									}
								}
							}
							// 05/05/2017 Paul.  Include Date, DateTime and Currency in case they were configured for export as template fields. 
							else if ( String.Compare(sDATA_FORMAT, "Date", true) == 0 || String.Compare(sDATA_FORMAT, "DateTime", true) == 0 || String.Compare(sDATA_FORMAT, "Currency", true) == 0 )
							{
								if ( !Sql.IsEmptyString(sDATA_FIELD) )
									arrSelectFields.Add(sDATA_FIELD);
							}
							// 02/11/2016 Paul.  Allow searching of hover fields. 
							else if ( String.Compare(sDATA_FORMAT, "Hover", true) == 0 )
							{
								string sURL_FIELD = Sql.ToString (row["URL_FIELD"]);
								if ( SplendidInit.bEnableACLFieldSecurity && !Sql.IsEmptyString(sURL_FIELD) )
								{
									string[] arrURL_FIELD = sURL_FIELD.Split(' ');
									for ( int i=0; i < arrURL_FIELD.Length; i++ )
									{
										if ( !arrURL_FIELD[i].Contains(".") )
										{
											Security.ACL_FIELD_ACCESS acl = Security.GetUserFieldSecurity(sMODULE_NAME, arrURL_FIELD[i], Guid.Empty);
											if ( acl.IsReadable() )
												arrSelectFields.Add(sDATA_FIELD);
										}
									}
								}
							}
							// 05/16/2016 Paul.  Include Tags in list of valid columns. 
							else if ( String.Compare(sDATA_FORMAT, "Tags", true) == 0 )
							{
								if ( !Sql.IsEmptyString(sDATA_FIELD) )
									arrSelectFields.Add(sDATA_FIELD);
							}
						}
						else if ( String.Compare(sCOLUMN_TYPE, "BoundColumn", true) == 0 )
						{
							// 09/23/2010 Paul.  Add the bound field. 
							if ( !Sql.IsEmptyString(sDATA_FIELD) )
								arrSelectFields.Add(sDATA_FIELD);
						}
						// 02/11/2016 Paul.  Allow searching of hidden field. 
						else if ( String.Compare(sCOLUMN_TYPE, "Hidden", true) == 0 )
						{
							if ( !Sql.IsEmptyString(sDATA_FIELD) )
								arrSelectFields.Add(sDATA_FIELD);
						}
					}
				}
			}
		}

		// 12/04/2010 Paul.  Add support for Business Rules Framework to Reports. 
		public void ApplyReportRules(L10N L10n, Guid gPRE_LOAD_EVENT_ID, Guid gPOST_LOAD_EVENT_ID, DataTable dt)
		{
			if ( !Sql.IsEmptyGuid(gPRE_LOAD_EVENT_ID) )
			{
				DataTable dtFields = SplendidCache.ReportRules(gPRE_LOAD_EVENT_ID);
				if ( dtFields.Rows.Count > 0 )
				{
					string sMODULE_NAME = Sql.ToString(dtFields.Rows[0]["MODULE_NAME"]);
					string sXOML        = Sql.ToString(dtFields.Rows[0]["XOML"       ]);
					if ( !Sql.IsEmptyString(sXOML) )
					{
						RuleSet rules = RulesUtil.Deserialize(sXOML);
						RuleValidation validation = new RuleValidation(typeof(SplendidReportThis), null);
						// 11/11/2010 Paul.  Validate so that we can get more information on a runtime error. 
						rules.Validate(validation);
						if ( validation.Errors.Count > 0 )
						{
							throw(new Exception(RulesUtil.GetValidationErrors(validation)));
						}
						SplendidReportThis swThis = new SplendidReportThis(Security, Container, sMODULE_NAME, dt);
						RuleExecution exec = new RuleExecution(validation, swThis);
						rules.Execute(exec);
					}
				}
			}
			if ( !Sql.IsEmptyGuid(gPOST_LOAD_EVENT_ID) )
			{
				DataTable dtFields = SplendidCache.ReportRules(gPOST_LOAD_EVENT_ID);
				if ( dtFields.Rows.Count > 0 )
				{
					string sMODULE_NAME = Sql.ToString(dtFields.Rows[0]["MODULE_NAME"]);
					string sXOML        = Sql.ToString(dtFields.Rows[0]["XOML"       ]);
					if ( !Sql.IsEmptyString(sXOML) )
					{
						RuleSet rules = RulesUtil.Deserialize(sXOML);
						RuleValidation validation = new RuleValidation(typeof(SplendidReportThis), null);
						// 11/11/2010 Paul.  Validate so that we can get more information on a runtime error. 
						rules.Validate(validation);
						if ( validation.Errors.Count > 0 )
						{
							throw(new Exception(RulesUtil.GetValidationErrors(validation)));
						}
						foreach ( DataRow row in dt.Rows )
						{
							SplendidReportThis swThis = new SplendidReportThis(Security, Container, sMODULE_NAME, row);
							RuleExecution exec = new RuleExecution(validation, swThis);
							rules.Execute(exec);
						}
					}
				}
			}
		}

// 11/03/2021 Paul.  ASP.Net components are not needed. 
#if !ReactOnlyUI
		public static void AppendGridColumns(string sGRID_NAME, DataGrid grd, UniqueStringCollection arrSelectFields)
		{
			AppendGridColumns(sGRID_NAME, grd, arrSelectFields, null);
		}

		// 02/08/2008 Paul.  We need to build a list of the fields used by the dynamic grid. 
		// 03/01/2014 Paul.  Add Preview button. 
		public static void AppendGridColumns(string sGRID_NAME, DataGrid grd, UniqueStringCollection arrSelectFields, CommandEventHandler Page_Command)
		{
			if ( grd == null )
			{
				SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), "DataGrid is not defined for " + sGRID_NAME);
				return;
			}
			// 05/10/2016 Paul.  The User Primary Role is used with role-based views. 
			DataTable dt = SplendidCache.GridViewColumns(sGRID_NAME, Security.PRIMARY_ROLE_NAME);
			if ( dt != null )
			{
				// 01/01/2008 Paul.  Pull config flag outside the loop. 
				bool bEnableTeamManagement = Crm.Config.enable_team_management();
				// 08/28/2009 Paul.  Allow dynamic teams to be turned off. 
				bool bEnableDynamicTeams   = Crm.Config.enable_dynamic_teams();
				// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
				bool bEnableDynamicAssignment = Crm.Config.enable_dynamic_assignment();
				// 09/16/2018 Paul.  Create a multi-tenant system. 
				if ( Crm.Config.enable_multi_tenant_teams() )
				{
					bEnableTeamManagement    = false;
					bEnableDynamicTeams      = false;
					bEnableDynamicAssignment = false;
				}
				foreach(DataRow row in dt.Rows)
				{
					int    nCOLUMN_INDEX               = Sql.ToInteger(row["COLUMN_INDEX"              ]);
					string sCOLUMN_TYPE                = Sql.ToString (row["COLUMN_TYPE"               ]);
					string sHEADER_TEXT                = Sql.ToString (row["HEADER_TEXT"               ]);
					string sSORT_EXPRESSION            = Sql.ToString (row["SORT_EXPRESSION"           ]);
					string sITEMSTYLE_WIDTH            = Sql.ToString (row["ITEMSTYLE_WIDTH"           ]);
					string sITEMSTYLE_CSSCLASS         = Sql.ToString (row["ITEMSTYLE_CSSCLASS"        ]);
					string sITEMSTYLE_HORIZONTAL_ALIGN = Sql.ToString (row["ITEMSTYLE_HORIZONTAL_ALIGN"]);
					string sITEMSTYLE_VERTICAL_ALIGN   = Sql.ToString (row["ITEMSTYLE_VERTICAL_ALIGN"  ]);
					bool   bITEMSTYLE_WRAP             = Sql.ToBoolean(row["ITEMSTYLE_WRAP"            ]);
					string sDATA_FIELD                 = Sql.ToString (row["DATA_FIELD"                ]);
					string sDATA_FORMAT                = Sql.ToString (row["DATA_FORMAT"               ]);
					string sURL_FIELD                  = Sql.ToString (row["URL_FIELD"                 ]);
					string sURL_FORMAT                 = Sql.ToString (row["URL_FORMAT"                ]);
					string sURL_TARGET                 = Sql.ToString (row["URL_TARGET"                ]);
					string sLIST_NAME                  = Sql.ToString (row["LIST_NAME"                 ]);
					// 04/28/2006 Paul.  The module is necessary in order to determine if a user has access. 
					string sURL_MODULE                 = Sql.ToString (row["URL_MODULE"                ]);
					// 05/02/2006 Paul.  The assigned user id is necessary if the user only has Owner access. 
					string sURL_ASSIGNED_FIELD         = Sql.ToString (row["URL_ASSIGNED_FIELD"        ]);
					// 02/16/2010 Paul.  Add MODULE_TYPE so that we can lookup custom field IDs. 
					string sMODULE_TYPE = String.Empty;
					try
					{
						sMODULE_TYPE = Sql.ToString (row["MODULE_TYPE"]);
					}
					catch(Exception ex)
					{
						// 06/16/2010 Paul.  The MODULE_TYPE is not in the view, then log the error and continue. 
						SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
					}
					// 10/09/2010 Paul.  Add PARENT_FIELD so that we can establish dependent listboxes. 
					string sPARENT_FIELD = String.Empty;
					try
					{
						sPARENT_FIELD = Sql.ToString (row["PARENT_FIELD"]);
					}
					catch(Exception ex)
					{
						// 10/09/2010 Paul.  The PARENT_FIELD is not in the view, then log the error and continue. 
						SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
					}

					if ( (sDATA_FIELD == "TEAM_NAME" || sDATA_FIELD == "TEAM_SET_NAME") )
					{
						// 05/06/2018 Paul.  Change to single instead of 1 to prevent auto-postback. 
						if ( bEnableTeamManagement && bEnableDynamicTeams && sDATA_FORMAT != "1" && !sDATA_FORMAT.ToLower().Contains("single") )
						{
							sHEADER_TEXT = ".LBL_LIST_TEAM_SET_NAME";
							sDATA_FIELD  = "TEAM_SET_NAME";
						}
						else
						{
							sHEADER_TEXT = ".LBL_LIST_TEAM_NAME";
							sDATA_FIELD  = "TEAM_NAME";
						}
					}
					// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
					else if ( sDATA_FIELD == "ASSIGNED_TO" || sDATA_FIELD == "ASSIGNED_TO_NAME" || sDATA_FIELD == "ASSIGNED_SET_NAME" )
					{
						// 12/17/2017 Paul.  Allow a layout to remain singular with DATA_FORMAT = 1. 
						// 05/06/2018 Paul.  Change to single instead of 1 to prevent auto-postback. 
						if ( bEnableDynamicAssignment && !sDATA_FORMAT.ToLower().Contains("single") )
						{
							sHEADER_TEXT = ".LBL_LIST_ASSIGNED_SET_NAME";
							sDATA_FIELD  = "ASSIGNED_SET_NAME";
						}
						else if ( sDATA_FIELD == "ASSIGNED_SET_NAME" )
						{
							sHEADER_TEXT = ".LBL_LIST_ASSIGNED_USER";
							sDATA_FIELD  = "ASSIGNED_TO_NAME";
						}
					}
					// 02/08/2008 Paul.  We need to build a list of the fields used by the dynamic grid. 
					if ( arrSelectFields != null )
					{
						// 08/02/2010 Paul.  The JavaScript and Hover fields will not have a data field. 
						if ( !Sql.IsEmptyString(sDATA_FIELD) )
							arrSelectFields.Add(sDATA_FIELD);
						if ( !Sql.IsEmptyString(sSORT_EXPRESSION) )
							arrSelectFields.Add(sSORT_EXPRESSION);
						if ( !Sql.IsEmptyString(sURL_FIELD) )
						{
							// 08/02/2010 Paul.  We want to allow Terminology fields, so exclude anything with a "."
							if ( sURL_FIELD.IndexOf(' ') >= 0 )
							{
								string[] arrURL_FIELD = sURL_FIELD.Split(' ');
								foreach ( string s in arrURL_FIELD )
								{
									if ( !s.Contains(".") && !Sql.IsEmptyString(s) )
										arrSelectFields.Add(s);
								}
							}
							else if ( !sURL_FIELD.Contains(".") )
								arrSelectFields.Add(sURL_FIELD);
							if ( !Sql.IsEmptyString(sURL_ASSIGNED_FIELD) )
								arrSelectFields.Add(sURL_ASSIGNED_FIELD);
						}
						// 10/09/2010 Paul.  Add PARENT_FIELD so that we can establish dependent listboxes. 
						if ( !Sql.IsEmptyString(sPARENT_FIELD) )
							arrSelectFields.Add(sPARENT_FIELD);
					}
					
					HorizontalAlign eHorizontalAlign = HorizontalAlign.NotSet;
					switch ( sITEMSTYLE_HORIZONTAL_ALIGN.ToLower() )
					{
						case "left" :  eHorizontalAlign = HorizontalAlign.Left ;  break;
						case "right":  eHorizontalAlign = HorizontalAlign.Right;  break;
					}
					VerticalAlign eVerticalAlign = VerticalAlign.NotSet;
					switch ( sITEMSTYLE_VERTICAL_ALIGN.ToLower() )
					{
						case "top"   :  eVerticalAlign = VerticalAlign.Top   ;  break;
						case "middle":  eVerticalAlign = VerticalAlign.Middle;  break;
						case "bottom":  eVerticalAlign = VerticalAlign.Bottom;  break;
					}
					// 11/28/2005 Paul.  Wrap defaults to true. 
					if ( row["ITEMSTYLE_WRAP"] == DBNull.Value )
						bITEMSTYLE_WRAP = true;

					// 01/18/2010 Paul.  To apply ACL Field Security, we need to know if the Module Name, which we will extract from the EditView Name. 
					string sMODULE_NAME = String.Empty;
					string[] arrGRID_NAME = sGRID_NAME.Split('.');
					if ( arrGRID_NAME.Length > 0 )
					{
						if ( arrGRID_NAME[0] == "ListView" || arrGRID_NAME[0] == "PopupView" || arrGRID_NAME[0] == "Activities" )
							sMODULE_NAME = arrGRID_NAME[0];
						// 01/18/2010 Paul.  A sub-panel should apply the access rules of the related module. 
						else if ( Sql.ToBoolean(HttpContext.Current.Application["Modules." + arrGRID_NAME[1] + ".Valid"]) )
							sMODULE_NAME = arrGRID_NAME[1];
						else
							sMODULE_NAME = arrGRID_NAME[0];
					}
					// 01/18/2010 Paul.  A field is either visible or not.  At this time, we will not only show a field to its owner. 
					bool bIsReadable  = true;
					// 08/02/2010 Paul.  The JavaScript and Hover fields will not have a data field. 
					if ( SplendidInit.bEnableACLFieldSecurity && !Sql.IsEmptyString(sDATA_FIELD) )
					{
						Security.ACL_FIELD_ACCESS acl = Security.GetUserFieldSecurity(sMODULE_NAME, sDATA_FIELD, Guid.Empty);
						bIsReadable  = acl.IsReadable();
					}

					DataGridColumn col = null;
					// 02/03/2006 Paul.  Date and Currency must always be handled by CreateItemTemplateLiteral. 
					// Otherwise, the date or time will not get properly translated to the correct timezone. 
					// This bug was reported by David Williams. 
					// 05/20/2009 Paul.  We need a way to preserve CRLF in description fields. 
					if (     String.Compare(sCOLUMN_TYPE, "BoundColumn", true) == 0 
					  && (   String.Compare(sDATA_FORMAT, "Date"       , true) == 0 
					      || String.Compare(sDATA_FORMAT, "DateTime"   , true) == 0 
					      || String.Compare(sDATA_FORMAT, "Currency"   , true) == 0
					      || String.Compare(sDATA_FORMAT, "Image"      , true) == 0
					      || String.Compare(sDATA_FORMAT, "MultiLine"  , true) == 0
					     )
					   )
					{
						sCOLUMN_TYPE = "TemplateColumn";
					}
					// 03/14/2014 Paul.  A hidden field does not render.  It is primarily used to add a field to the SQL select list for Business Rules management. 
					// 08/20/2016 Paul.  The hidden field is a DATA_FORMAT, not a COLUMN_TYPE, but keep COLUMN_TYPE just in case anyone used it. 
					if ( String.Compare(sCOLUMN_TYPE, "Hidden", true) == 0 || String.Compare(sDATA_FORMAT, "Hidden", true) == 0 )
					{
						continue;
					}
					if ( String.Compare(sCOLUMN_TYPE, "BoundColumn", true) == 0 )
					{
						if ( Sql.IsEmptyString(sLIST_NAME) )
						{
							// GRID_NAME, COLUMN_ORDER, COLUMN_TYPE, HEADER_TEXT, DATA_FIELD, SORT_EXPRESSION, ITEMSTYLE_WIDTH
							// 09/15/2014 Paul.  Prevent Cross-Site Scripting by HTML encoding the data. 
							TemplateColumn bnd = new TemplateColumn();
							bnd.HeaderText                  = sHEADER_TEXT       ;
							//bnd.DataField                   = sDATA_FIELD        ;
							bnd.SortExpression              = sSORT_EXPRESSION   ;
							bnd.ItemStyle.Width             = new Unit(sITEMSTYLE_WIDTH);
							bnd.ItemStyle.CssClass          = sITEMSTYLE_CSSCLASS;
							bnd.ItemStyle.HorizontalAlign   = eHorizontalAlign   ;
							bnd.ItemStyle.VerticalAlign     = eVerticalAlign     ;
							bnd.ItemStyle.Wrap              = bITEMSTYLE_WRAP    ;
							// 04/13/2007 Paul.  Align the headers to match the data. 
							bnd.HeaderStyle.HorizontalAlign = eHorizontalAlign   ;
							col = bnd;
							// 01/18/2010 Paul.  Apply ACL Field Security. 
							col.Visible = bIsReadable;
							// 10/23/2012 Kevin.  Allow me to pass data format for gridview bound columns. 
							// 09/15/2014 Paul.  Prevent Cross-Site Scripting by HTML encoding the data. 
							//if ( !Sql.IsEmptyString(sDATA_FORMAT) )
							//{
							//	bnd.DataFormatString = sDATA_FORMAT;
							//}
							bnd.ItemTemplate = new CreateItemTemplateLiteral(sDATA_FIELD, sDATA_FORMAT, sMODULE_TYPE);
						}
						else
						{
							// GRID_NAME, COLUMN_ORDER, COLUMN_TYPE, HEADER_TEXT, DATA_FIELD, SORT_EXPRESSION, ITEMSTYLE_WIDTH
							TemplateColumn tpl = new TemplateColumn();
							tpl.HeaderText                  = sHEADER_TEXT       ;
							tpl.SortExpression              = sSORT_EXPRESSION   ;
							tpl.ItemStyle.Width             = new Unit(sITEMSTYLE_WIDTH);
							tpl.ItemStyle.CssClass          = sITEMSTYLE_CSSCLASS;
							tpl.ItemStyle.HorizontalAlign   = eHorizontalAlign   ;
							tpl.ItemStyle.VerticalAlign     = eVerticalAlign     ;
							tpl.ItemStyle.Wrap              = bITEMSTYLE_WRAP    ;
							// 04/13/2007 Paul.  Align the headers to match the data. 
							tpl.HeaderStyle.HorizontalAlign = eHorizontalAlign   ;
							// 10/09/2010 Paul.  Add PARENT_FIELD so that we can establish dependent listboxes. 
							tpl.ItemTemplate = new CreateItemTemplateLiteralList(sDATA_FIELD, sLIST_NAME, sPARENT_FIELD);
							col = tpl;
							// 01/18/2010 Paul.  Apply ACL Field Security. 
							col.Visible = bIsReadable;
						}
					}
					else if ( String.Compare(sCOLUMN_TYPE, "TemplateColumn", true) == 0 )
					{
						// GRID_NAME, COLUMN_ORDER, COLUMN_TYPE, HEADER_TEXT, DATA_FIELD, SORT_EXPRESSION, ITEMSTYLE_WIDTH
						TemplateColumn tpl = new TemplateColumn();
						tpl.HeaderText                  = sHEADER_TEXT       ;
						tpl.SortExpression              = sSORT_EXPRESSION   ;
						tpl.ItemStyle.Width             = new Unit(sITEMSTYLE_WIDTH);
						tpl.ItemStyle.CssClass          = sITEMSTYLE_CSSCLASS;
						tpl.ItemStyle.HorizontalAlign   = eHorizontalAlign   ;
						tpl.ItemStyle.VerticalAlign     = eVerticalAlign     ;
						tpl.ItemStyle.Wrap              = bITEMSTYLE_WRAP    ;
						// 04/13/2007 Paul.  Align the headers to match the data. 
						tpl.HeaderStyle.HorizontalAlign = eHorizontalAlign   ;
						if ( String.Compare(sDATA_FORMAT, "JavaScript", true) == 0 )
						{
							// 08/02/2010 Paul.  In our application of Field Level Security, we will hide fields by replacing with "."
							if ( SplendidInit.bEnableACLFieldSecurity && !Sql.IsEmptyString(sURL_FIELD) )
							{
								string[] arrURL_FIELD = sURL_FIELD.Split(' ');
								for ( int i=0; i < arrURL_FIELD.Length; i++ )
								{
									Security.ACL_FIELD_ACCESS acl = Security.GetUserFieldSecurity(sMODULE_NAME, sDATA_FIELD, Guid.Empty);
									if ( !acl.IsReadable() )
										arrURL_FIELD[i] = ".";
								}
								sURL_FIELD = String.Join(" ", arrURL_FIELD);
							}
							tpl.ItemTemplate = new CreateItemTemplateJavaScript(sDATA_FIELD, sURL_FIELD, sURL_FORMAT, sURL_TARGET);
						}
						// 02/26/2014 Paul.  Add Preview button. 
						else if ( String.Compare(sDATA_FORMAT, "JavaImage", true) == 0 )
						{
							if ( SplendidInit.bEnableACLFieldSecurity && !Sql.IsEmptyString(sURL_FIELD) )
							{
								string[] arrURL_FIELD = sURL_FIELD.Split(' ');
								for ( int i=0; i < arrURL_FIELD.Length; i++ )
								{
									Security.ACL_FIELD_ACCESS acl = Security.GetUserFieldSecurity(sMODULE_NAME, sDATA_FIELD, Guid.Empty);
									if ( !acl.IsReadable() )
										arrURL_FIELD[i] = ".";
								}
								sURL_FIELD = String.Join(" ", arrURL_FIELD);
							}
							tpl.ItemTemplate = new CreateItemTemplateJavaScriptImage(sURL_FIELD, sURL_FORMAT, sURL_TARGET);
						}
						// 03/01/2014 Paul.  Add Preview button. 
						else if ( String.Compare(sDATA_FORMAT, "ImageButton", true) == 0 )
						{
							// 03/01/2014 Paul.  sURL_FIELD is an internal value, so there is no need to apply ACL rules. 
							tpl.ItemTemplate = new CreateItemTemplateImageButton(sURL_FIELD, sURL_FORMAT, sURL_TARGET, sITEMSTYLE_CSSCLASS, Page_Command);
							// 06/07/2015 Paul.  Only show the preview button on the Seven theme. 
							if ( sURL_FORMAT == "Preview" )
								bIsReadable &= SplendidDynamic.StackedLayout(grd.Page.Theme);
						}
						else if ( String.Compare(sDATA_FORMAT, "Hover", true) == 0 )
						{
							string sIMAGE_SKIN = sURL_TARGET;
							if ( Sql.IsEmptyString(sIMAGE_SKIN) )
								sIMAGE_SKIN = "info_inline";
							// 08/02/2010 Paul.  In our application of Field Level Security, we will hide fields by replacing with "."
							if ( SplendidInit.bEnableACLFieldSecurity && !Sql.IsEmptyString(sURL_FIELD) )
							{
								string[] arrURL_FIELD = sURL_FIELD.Split(' ');
								for ( int i=0; i < arrURL_FIELD.Length; i++ )
								{
									// 02/11/2016 Paul.  Exclude terminology. 
									if ( !arrURL_FIELD[i].Contains(".") )
									{
										// 02/11/2016 Paul.  Fix cut-and-paste error.  We were testing sDATA_FIELD and not arrURL_FIELD[i]. 
										Security.ACL_FIELD_ACCESS acl = Security.GetUserFieldSecurity(sMODULE_NAME, arrURL_FIELD[i], Guid.Empty);
										if ( !acl.IsReadable() )
											arrURL_FIELD[i] = ".";
									}
								}
								sURL_FIELD = String.Join(" ", arrURL_FIELD);
							}
							tpl.ItemTemplate = new CreateItemTemplateHover(sDATA_FIELD, sURL_FIELD, sURL_FORMAT, sIMAGE_SKIN);
						}
						else if ( String.Compare(sDATA_FORMAT, "HyperLink", true) == 0 )
						{
							// 07/26/2007 Paul.  PopupViews have special requirements.  They need an OnClick action that takes more than one parameter. 
							if ( sURL_FIELD.IndexOf(' ') >= 0 )
								tpl.ItemTemplate = new CreateItemTemplateHyperLinkOnClick(sDATA_FIELD, sURL_FIELD, sURL_FORMAT, sURL_TARGET, sITEMSTYLE_CSSCLASS, sURL_MODULE, sURL_ASSIGNED_FIELD, sMODULE_TYPE);
							else
								tpl.ItemTemplate = new CreateItemTemplateHyperLink(sDATA_FIELD, sURL_FIELD, sURL_FORMAT, sURL_TARGET, sITEMSTYLE_CSSCLASS, sURL_MODULE, sURL_ASSIGNED_FIELD, sMODULE_TYPE);
						}
						else if ( String.Compare(sDATA_FORMAT, "Image", true) == 0 )
						{
							// 08/15/2014 Paul.  Show the URL_FORMAT for Images so that we can point to the EmailImages URL. 
							tpl.ItemTemplate = new CreateItemTemplateImage(sDATA_FIELD, sURL_FORMAT, sITEMSTYLE_CSSCLASS);
						}
						else
						{
							tpl.ItemStyle.CssClass = sITEMSTYLE_CSSCLASS;
							tpl.ItemTemplate = new CreateItemTemplateLiteral(sDATA_FIELD, sDATA_FORMAT, sMODULE_TYPE);
						}
						col = tpl;
						// 01/18/2010 Paul.  Apply ACL Field Security. 
						col.Visible = bIsReadable;
					}
					else if ( String.Compare(sCOLUMN_TYPE, "HyperLinkColumn", true) == 0 )
					{
						// GRID_NAME, COLUMN_ORDER, COLUMN_TYPE, HEADER_TEXT, DATA_FIELD, SORT_EXPRESSION, ITEMSTYLE_WIDTH, ITEMSTYLE-CSSCLASS, URL_FIELD, URL_FORMAT
						HyperLinkColumn lnk = new HyperLinkColumn();
						lnk.HeaderText                  = sHEADER_TEXT       ;
						lnk.DataTextField               = sDATA_FIELD        ;
						lnk.SortExpression              = sSORT_EXPRESSION   ;
						lnk.DataNavigateUrlField        = sURL_FIELD         ;
						lnk.DataNavigateUrlFormatString = sURL_FORMAT        ;
						lnk.Target                      = sURL_TARGET        ;
						lnk.ItemStyle.Width             = new Unit(sITEMSTYLE_WIDTH);
						lnk.ItemStyle.CssClass          = sITEMSTYLE_CSSCLASS;
						lnk.ItemStyle.HorizontalAlign   = eHorizontalAlign   ;
						lnk.ItemStyle.VerticalAlign     = eVerticalAlign     ;
						lnk.ItemStyle.Wrap              = bITEMSTYLE_WRAP    ;
						// 04/13/2007 Paul.  Align the headers to match the data. 
						lnk.HeaderStyle.HorizontalAlign = eHorizontalAlign   ;
						col = lnk;
						// 01/18/2010 Paul.  Apply ACL Field Security. 
						col.Visible = bIsReadable;
					}
					if ( col != null )
					{
						// 11/25/2006 Paul.  If Team Management has been disabled, then hide the column. 
						// Keep the column, but hide it so that the remaining column positions will still be valid. 
						// 10/27/2007 Paul.  The data field was changed to TEAM_NAME on 11/25/2006. It should have been changed here as well. 
						// 08/24/2009 Paul.  Add support for dynamic teams. 
						if ( (sDATA_FIELD == "TEAM_NAME" || sDATA_FIELD == "TEAM_SET_NAME") && !bEnableTeamManagement )
						{
							col.Visible = false;
						}
						// 11/28/2005 Paul.  In case the column specified is too high, just append column. 
						if ( nCOLUMN_INDEX >= grd.Columns.Count )
							grd.Columns.Add(col);
						else
							grd.Columns.AddAt(nCOLUMN_INDEX, col);
					}
				}
			}
			// 09/20/2012 Paul.  We need a SCRIPT field that is form specific. 
			if ( dt.Rows.Count > 0 )
			{
				try
				{
					string sFORM_SCRIPT = Sql.ToString(dt.Rows[0]["SCRIPT"]);
					if ( !Sql.IsEmptyString(sFORM_SCRIPT) )
					{
						// 09/20/2012 Paul.  The base ID is not the ID of the parent, but the ID of the TemplateControl. 
						sFORM_SCRIPT = sFORM_SCRIPT.Replace("SPLENDID_GRIDVIEW_LAYOUT_ID", grd.TemplateControl.ClientID);
						ScriptManager.RegisterStartupScript(grd, typeof(System.String), sGRID_NAME.Replace(".", "_") + "_SCRIPT", sFORM_SCRIPT, true);
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
				}
			}
		}

		// 04/11/2011 Paul.  Add the layout flag so that we can provide a preview mode. 
		public static void AppendGridColumns(DataView dvFields, HtmlTable tbl, IDataReader rdr, L10N L10n, TimeZone T10n, CommandEventHandler Page_Command, bool bLayoutMode)
		{
			if ( tbl == null )
			{
				SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), "HtmlTable is not defined");
				return;
			}
			// 01/07/2006 Paul.  Show table borders in layout mode. This will help distinguish blank lines from wrapped lines. 
			if ( bLayoutMode )
				tbl.Border = 1;

			HtmlTableRow trAction = new HtmlTableRow();
			HtmlTableRow trHeader = new HtmlTableRow();
			HtmlTableRow trField  = new HtmlTableRow();
			tbl.Rows.Insert(0, trAction);
			tbl.Rows.Insert(1, trHeader);
			tbl.Rows.Insert(2, trField );
			trAction.Attributes.Add("class", "listViewThS1");
			trHeader.Attributes.Add("class", "listViewThS1");
			trField .Attributes.Add("class", "oddListRowS1");
			trAction.Visible = bLayoutMode;

			HttpSessionState Session = HttpContext.Current.Session;
			bool bSupportsDraggable = Sql.ToBoolean(Session["SupportsDraggable"]);
			foreach(DataRowView row in dvFields)
			{
				Guid   gID                         = Sql.ToGuid   (row["ID"                        ]);
				int    nCOLUMN_INDEX               = Sql.ToInteger(row["COLUMN_INDEX"              ]);
				string sCOLUMN_TYPE                = Sql.ToString (row["COLUMN_TYPE"               ]);
				string sHEADER_TEXT                = Sql.ToString (row["HEADER_TEXT"               ]);
				string sSORT_EXPRESSION            = Sql.ToString (row["SORT_EXPRESSION"           ]);
				string sITEMSTYLE_WIDTH            = Sql.ToString (row["ITEMSTYLE_WIDTH"           ]);
				string sITEMSTYLE_CSSCLASS         = Sql.ToString (row["ITEMSTYLE_CSSCLASS"        ]);
				string sITEMSTYLE_HORIZONTAL_ALIGN = Sql.ToString (row["ITEMSTYLE_HORIZONTAL_ALIGN"]);
				string sITEMSTYLE_VERTICAL_ALIGN   = Sql.ToString (row["ITEMSTYLE_VERTICAL_ALIGN"  ]);
				bool   bITEMSTYLE_WRAP             = Sql.ToBoolean(row["ITEMSTYLE_WRAP"            ]);
				string sDATA_FIELD                 = Sql.ToString (row["DATA_FIELD"                ]);
				string sDATA_FORMAT                = Sql.ToString (row["DATA_FORMAT"               ]);
				string sURL_FIELD                  = Sql.ToString (row["URL_FIELD"                 ]);
				string sURL_FORMAT                 = Sql.ToString (row["URL_FORMAT"                ]);
				string sURL_TARGET                 = Sql.ToString (row["URL_TARGET"                ]);
				string sLIST_NAME                  = Sql.ToString (row["LIST_NAME"                 ]);
				
				HtmlTableCell tdAction = new HtmlTableCell();
				trAction.Cells.Add(tdAction);
				tdAction.NoWrap = true;

				Literal litIndex = new Literal();
				tdAction.Controls.Add(litIndex);
				litIndex.Text = " " + nCOLUMN_INDEX.ToString() + " ";

				// 05/18/2013 Paul.  Add drag handle. 
				if ( bSupportsDraggable )
				{
					Image imgDragIcon = new Image();
					imgDragIcon.SkinID = "draghandle_horz";
					imgDragIcon.Attributes.Add("draggable"  , "true");
					imgDragIcon.Attributes.Add("ondragstart", "event.dataTransfer.setData('Text', '" + nCOLUMN_INDEX.ToString() + "');");
					tdAction.Controls.Add(imgDragIcon);
					// 08/08/2013 Paul.  IE does not support preventDefault. 
					// http://stackoverflow.com/questions/1000597/event-preventdefault-function-not-working-in-ie
					tdAction.Attributes.Add("ondragover", "LayoutDragOver(event, '" + nCOLUMN_INDEX.ToString() + "')");
					tdAction.Attributes.Add("ondrop"    , "LayoutDropIndex(event, '" + nCOLUMN_INDEX.ToString() + "')");
				}
				else
				{
					ImageButton btnMoveUp   = CreateLayoutImageButtonSkin(gID, "Layout.MoveUp"  , nCOLUMN_INDEX, L10n.Term(".LNK_LEFT"  ), "leftarrow_inline" , Page_Command);
					ImageButton btnMoveDown = CreateLayoutImageButtonSkin(gID, "Layout.MoveDown", nCOLUMN_INDEX, L10n.Term(".LNK_RIGHT" ), "rightarrow_inline", Page_Command);
					tdAction.Controls.Add(btnMoveUp  );
					tdAction.Controls.Add(btnMoveDown);
				}
				ImageButton btnInsert   = CreateLayoutImageButtonSkin(gID, "Layout.Insert"  , nCOLUMN_INDEX, L10n.Term(".LNK_INS"   ), "plus_inline"      , Page_Command);
				ImageButton btnEdit     = CreateLayoutImageButtonSkin(gID, "Layout.Edit"    , nCOLUMN_INDEX, L10n.Term(".LNK_EDIT"  ), "edit_inline"      , Page_Command);
				ImageButton btnDelete   = CreateLayoutImageButtonSkin(gID, "Layout.Delete"  , nCOLUMN_INDEX, L10n.Term(".LNK_DELETE"), "delete_inline"    , Page_Command);
				tdAction.Controls.Add(btnInsert  );
				tdAction.Controls.Add(btnEdit    );
				tdAction.Controls.Add(btnDelete  );
				
				HtmlTableCell tdHeader = new HtmlTableCell();
				trHeader.Cells.Add(tdHeader);
				tdHeader.NoWrap = true;
				
				HtmlTableCell tdField = new HtmlTableCell();
				trField.Cells.Add(tdField);
				tdField.NoWrap = true;

				Literal litHeader = new Literal();
				tdHeader.Controls.Add(litHeader);
				if ( bLayoutMode )
					litHeader.Text = sHEADER_TEXT;
				else
					litHeader.Text = L10n.Term(sHEADER_TEXT);

				Literal litField = new Literal();
				tdField.Controls.Add(litField);
				litField.Text = sDATA_FIELD;
				litField.Visible = bLayoutMode;
			}
		}

		public static void AppendButtons(string sVIEW_NAME, Guid gASSIGNED_USER_ID, Control ctl, bool bIsMobile, DataRow rdr, L10N L10n, CommandEventHandler Page_Command)
		{
			AppendButtons(sVIEW_NAME, gASSIGNED_USER_ID, ctl, null, String.Empty, bIsMobile, rdr, L10n, Page_Command);
		}

		// 05/24/2015 Paul.  Seven theme has a hover popdown. 
		public static int AppendButtons(string sVIEW_NAME, Guid gASSIGNED_USER_ID, Control ctl, Control ctlHover, string sButtonStyle, bool bIsMobile, DataRow rdr, L10N L10n, CommandEventHandler Page_Command)
		{
			// 06/03/2015 Paul.  The button count is used by SubPanelButtons in the Seven theme. 
			int nButtonCount = 0;
			if ( ctl == null )
			{
				SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), "AppendButtons ctl is not defined");
				return nButtonCount;
			}
			//ctl.Controls.Clear();

			Hashtable hashIDs = new Hashtable();
			// 05/06/2010 Paul.  Use a special Page flag to override the default IsPostBack behavior. 
			// 06/05/2015 Paul.  ctl.Page is not available when creating MassUpdate buttons on DataGrid. 
			bool bIsPostBack = false;
			if ( ctl.Page != null )
				bIsPostBack = ctl.Page.IsPostBack;
			bool bNotPostBack = false;
			if ( ctl.TemplateControl is SplendidControl )
			{
				bNotPostBack = (ctl.TemplateControl as SplendidControl).NotPostBack;
				bIsPostBack = ctl.Page.IsPostBack && !bNotPostBack;
			}
			bool bShowUnassigned = Crm.Config.show_unassigned();
			DataTable dt = SplendidCache.DynamicButtons(sVIEW_NAME);
			if ( dt != null )
			{
				nButtonCount = dt.Rows.Count;
				for ( int iButton = 0; iButton < dt.Rows.Count; iButton++ )
				{
					DataRow row = dt.Rows[iButton];
					Guid   gID                 = Sql.ToGuid   (row["ID"                ]);
					int    nCONTROL_INDEX      = Sql.ToInteger(row["CONTROL_INDEX"     ]);
					string sCONTROL_TYPE       = Sql.ToString (row["CONTROL_TYPE"      ]);
					string sMODULE_NAME        = Sql.ToString (row["MODULE_NAME"       ]);
					string sMODULE_ACCESS_TYPE = Sql.ToString (row["MODULE_ACCESS_TYPE"]);
					string sTARGET_NAME        = Sql.ToString (row["TARGET_NAME"       ]);
					string sTARGET_ACCESS_TYPE = Sql.ToString (row["TARGET_ACCESS_TYPE"]);
					bool   bMOBILE_ONLY        = Sql.ToBoolean(row["MOBILE_ONLY"       ]);
					bool   bADMIN_ONLY         = Sql.ToBoolean(row["ADMIN_ONLY"        ]);
					string sCONTROL_TEXT       = Sql.ToString (row["CONTROL_TEXT"      ]);
					string sCONTROL_TOOLTIP    = Sql.ToString (row["CONTROL_TOOLTIP"   ]);
					string sCONTROL_ACCESSKEY  = Sql.ToString (row["CONTROL_ACCESSKEY" ]);
					string sCONTROL_CSSCLASS   = Sql.ToString (row["CONTROL_CSSCLASS"  ]);
					string sTEXT_FIELD         = Sql.ToString (row["TEXT_FIELD"        ]);
					string sARGUMENT_FIELD     = Sql.ToString (row["ARGUMENT_FIELD"    ]);
					string sCOMMAND_NAME       = Sql.ToString (row["COMMAND_NAME"      ]);
					string sURL_FORMAT         = Sql.ToString (row["URL_FORMAT"        ]);
					string sURL_TARGET         = Sql.ToString (row["URL_TARGET"        ]);
					string sONCLICK_SCRIPT     = Sql.ToString (row["ONCLICK_SCRIPT"    ]);
					// 07/28/2010 Paul.  We need a flag to exclude a button from a mobile device. 
					bool   bEXCLUDE_MOBILE     = false;
					try
					{
						bEXCLUDE_MOBILE = Sql.ToBoolean(row["EXCLUDE_MOBILE"]);
					}
					catch
					{
					}
					// 03/14/2014 Paul.  Allow hidden buttons to be created. 
					bool   bHIDDEN             = false;
					try
					{
						bHIDDEN = Sql.ToBoolean(row["HIDDEN"]);
					}
					catch
					{
					}
					// 08/16/2017 Paul.  Add ability to apply a business rule to a button. 
					string sBUSINESS_RULE      = String.Empty;
					try
					{
						sBUSINESS_RULE = Sql.ToString (row["BUSINESS_RULE"     ]);
					}
					catch
					{
					}
					// 05/25/2015 Paul.  Reuse as much existing code as possible by allowing ctlHover to be NULL. 
					if ( ctlHover != null )
					{
						if ( (iButton == 1 && (sButtonStyle == "ModuleHeader" || sButtonStyle == "ListHeader" || sButtonStyle == "MassUpdateHeader")) || (iButton == 0 && sButtonStyle == "DataGrid") )
						{
							ImageButton btnMore = new ImageButton();
							btnMore.SkinID        = sButtonStyle + "MoreButton";
							btnMore.CssClass      = sButtonStyle + "MoreButton";
							btnMore.OnClientClick = "void(0); return false;";
							btnMore.Attributes.Add("style", "vertical-align: top;");
							ctl.Controls.Add(btnMore);
							ctl = ctlHover;
						}
						// 06/06/2015 Paul.  Change standard MassUpdate command to a command to toggle visibility. 
						if ( sButtonStyle == "DataGrid" && sCOMMAND_NAME == "MassUpdate" )
						{
							sCONTROL_TEXT       = L10n.Term(".LBL_MASS_UPDATE_TITLE");
							sCONTROL_TOOLTIP    = L10n.Term(".LBL_MASS_UPDATE_TITLE");
							sCOMMAND_NAME       = "ToggleMassUpdate";
							sONCLICK_SCRIPT     = String.Empty;
							sMODULE_ACCESS_TYPE = null;
							// 05/07/2017 Paul.  Don't display MassUpdate toggle if it is disabled for the module. 
							string sMODULE = sVIEW_NAME.Split('.')[0];
							if ( !Sql.IsEmptyString(sMODULE) && !(!bIsMobile && SplendidCRM.Crm.Modules.MassUpdate(sMODULE)) )
								bHIDDEN = true;
						}
						if ( iButton == 0 && sButtonStyle == "ListHeader" && sCOMMAND_NAME.EndsWith("Create") )
						{
							sCONTROL_TEXT = "+";
						}
					}

					// 09/01/2008 Paul.  Give each button an ID to simplify validation. 
					// Attempt to name the control after the command name.  If no command name, then use the control text. 
					string sCONTROL_ID = String.Empty;
					if ( !Sql.IsEmptyString(sCOMMAND_NAME) )
					{
						sCONTROL_ID = sCOMMAND_NAME;
					}
					else if ( !Sql.IsEmptyString(sCONTROL_TEXT) )
					{
						sCONTROL_ID = sCONTROL_TEXT;
						if ( sCONTROL_TEXT.IndexOf('.') >= 0 )
						{
							sCONTROL_ID = sCONTROL_TEXT.Split('.')[1];
							sCONTROL_ID = sCONTROL_ID.Replace("LBL_", "");
							sCONTROL_ID = sCONTROL_ID.Replace("_BUTTON_LABEL", "");
						}
					}
					if ( !Sql.IsEmptyString(sCONTROL_ID) )
					{
						// 09/01/2008 Paul.  Cleanup the ID. 
						sCONTROL_ID = sCONTROL_ID.Trim();
						sCONTROL_ID = sCONTROL_ID.Replace(' ', '_');
						sCONTROL_ID = sCONTROL_ID.Replace('.', '_');
						sCONTROL_ID = "btn" + sCONTROL_ID.ToUpper();
						// 12/16/2008 Paul.  Add to hash after cleaning the ID. 
						if ( hashIDs.Contains(sCONTROL_ID) )
							sCONTROL_ID = sVIEW_NAME.Replace('.', '_') + "_" + nCONTROL_INDEX.ToString();
						if ( !hashIDs.Contains(sCONTROL_ID) )
							hashIDs.Add(sCONTROL_ID, null);
						else
							sCONTROL_ID = String.Empty;  // If ID still exists, then don't set the ID. 
					}

					// 03/21/2008 Paul.  We need to use a view to search for the rows for the ColumnName. 
					// 11/22/2010 Paul.  Convert data reader to data table for Rules Wizard. 
					//DataView vwSchema = null;
					//if ( rdr != null )
					//	vwSchema = new DataView(rdr.GetSchemaTable());

					string[] arrTEXT_FIELD = sTEXT_FIELD.Split(' ');
					object[] objTEXT_FIELD = new object[arrTEXT_FIELD.Length];
					for ( int i=0 ; i < arrTEXT_FIELD.Length; i++ )
					{
						if ( !Sql.IsEmptyString(arrTEXT_FIELD[i]) )
						{
							objTEXT_FIELD[i] = String.Empty;
							if ( rdr != null ) // && vwSchema != null
							{
								//vwSchema.RowFilter = "ColumnName = '" + Sql.EscapeSQL(arrTEXT_FIELD[i]) + "'";
								//if ( vwSchema.Count > 0 )
								// 11/22/2010 Paul.  Convert data reader to data table for Rules Wizard. 
								if ( rdr.Table.Columns.Contains(arrTEXT_FIELD[i]) )
									objTEXT_FIELD[i] = Sql.ToString(rdr[arrTEXT_FIELD[i]]);
							}
						}
					}
					// 11/08/2020 Paul.  An Admin Only control should not be added as it would allow the visible/hidden flag to ignore the admin only rule. 
					// This is happening in DetailView with the Archive.MoveData command. 
					bool bVisible = (bADMIN_ONLY && Security.IS_ADMIN || !bADMIN_ONLY);
					if ( String.Compare(sCONTROL_TYPE, "Button", true) == 0 )
					{
						Button btn = new Button();
						// 11/08/2020 Paul.  An Admin Only control should not be added as it would allow the visible/hidden flag to ignore the admin only rule. 
						if ( bVisible )
							ctl.Controls.Add(btn);
						if ( !Sql.IsEmptyString(sCONTROL_ID) )
							btn.ID = sCONTROL_ID;
						// 01/05/2016 Paul.  Overload the URL_FORMAT field to set the command argument. 
						if ( !Sql.IsEmptyString(sURL_FORMAT) )
						{
							btn.CommandArgument = sURL_FORMAT;
						}
						if ( !Sql.IsEmptyString(sARGUMENT_FIELD) )
						{
							if ( rdr != null ) // && vwSchema != null )
							{
								//vwSchema.RowFilter = "ColumnName = '" + Sql.EscapeSQL(sARGUMENT_FIELD) + "'";
								//if ( vwSchema.Count > 0 )
								if ( rdr.Table.Columns.Contains(sARGUMENT_FIELD) )
									btn.CommandArgument = Sql.ToString(rdr[sARGUMENT_FIELD]);
							}
						}

						btn.Text            = "  " + L10n.Term(sCONTROL_TEXT) + "  ";
						btn.CssClass        = sCONTROL_CSSCLASS;
						// 05/25/2015 Paul.  New style for Seven theme buttons. 
						if ( ctlHover != null )
						{
							btn.CssClass = (iButton == 0 ? sButtonStyle + "FirstButton" : sButtonStyle + "OtherButton");
						}
						btn.Command        += Page_Command;
						btn.CommandName     = sCOMMAND_NAME;
						btn.OnClientClick   = sONCLICK_SCRIPT;
						// 11/21/2008 Paul.  On post back, we need to re-create the buttons, but don't change the visiblity flag. 
						// The problem is that we don't have the record at this early stage, so we cannot properly evaluate gASSIGNED_USER_ID. 
						// This is not an issue because .NET will restore the previous visibility state on post back. 
						if ( !bIsPostBack )
						{
							// 07/28/2010 Paul.  We need a flag to exclude a button from a mobile device. 
							// 03/14/2014 Paul.  Allow hidden buttons to be created. 
							btn.Visible         = (!bEXCLUDE_MOBILE || !bIsMobile) && (bMOBILE_ONLY && bIsMobile || !bMOBILE_ONLY) && (bADMIN_ONLY && Security.IS_ADMIN || !bADMIN_ONLY) && !bHIDDEN;
							if ( btn.Visible && !Sql.IsEmptyString(sMODULE_NAME) && !Sql.IsEmptyString(sMODULE_ACCESS_TYPE) )
							{
								int nACLACCESS = SplendidCRM.Security.GetUserAccess(sMODULE_NAME, sMODULE_ACCESS_TYPE);
								// 08/11/2008 John.  Fix owner access rights. 
								// 10/27/2008 Brian.  Only show button if show_unassigned is enabled.
								// 11/21/2008 Paul.  We need to make sure that an owner can create a new record. 
								btn.Visible = (nACLACCESS > ACL_ACCESS.OWNER) || (nACLACCESS == ACL_ACCESS.OWNER && ((Security.USER_ID == gASSIGNED_USER_ID) || (!bIsPostBack && rdr == null) || (rdr != null && bShowUnassigned && Sql.IsEmptyGuid(gASSIGNED_USER_ID))));
								if ( btn.Visible && !Sql.IsEmptyString(sTARGET_NAME) && !Sql.IsEmptyString(sTARGET_ACCESS_TYPE) )
								{
									// 08/11/2008 John.  Fix owner access rights.
									nACLACCESS = SplendidCRM.Security.GetUserAccess(sTARGET_NAME, sTARGET_ACCESS_TYPE);
									// 11/21/2008 Paul.  We need to make sure that an owner can create a new record. 
									btn.Visible = (nACLACCESS > ACL_ACCESS.OWNER) || (nACLACCESS == ACL_ACCESS.OWNER && ((Security.USER_ID == gASSIGNED_USER_ID) || (!bIsPostBack && rdr == null) || (rdr != null && bShowUnassigned && Sql.IsEmptyGuid(gASSIGNED_USER_ID))));
								}
							}
						}
						if ( !Sql.IsEmptyString(sCONTROL_ACCESSKEY) )
						{
							btn.AccessKey = L10n.AccessKey(sCONTROL_ACCESSKEY);
						}
						if ( !Sql.IsEmptyString(sCONTROL_TOOLTIP) )
						{
							btn.ToolTip = L10n.Term (sCONTROL_TOOLTIP);
							if ( btn.ToolTip.Contains("[Alt]") )
							{
								if ( btn.AccessKey.Length > 0 )
									btn.ToolTip = btn.ToolTip.Replace("[Alt]", "[Alt+" + btn.AccessKey + "]");
								else
									btn.ToolTip = btn.ToolTip.Replace("[Alt]", String.Empty);
							}
						}
						// 05/25/2015 Paul.  We don't want the spacer in the Seven module header. 
						if ( !(iButton == 0 && ctlHover != null) )
							btn.Attributes.Add("style", "margin-right: 3px;");
						// 08/16/2017 Paul.  Add ability to apply a business rule to a button. 
						if ( !Sql.IsEmptyString(sBUSINESS_RULE) )
						{
							RuleValidation validation = new RuleValidation(typeof(DynamicButtonThis), null);
							RuleSet rules = RulesUtil.BuildRuleSet(sBUSINESS_RULE, validation);
							rules.Validate(validation);
							if ( validation.Errors.HasErrors )
							{
								SplendidError.SystemError(new StackTrace(true).GetFrame(0), RulesUtil.GetValidationErrors(validation));
							}
							else
							{
								DynamicButtonThis swThis = new DynamicButtonThis(btn, L10n);
								RuleExecution exec = new RuleExecution(validation, swThis);
								rules.Execute(exec);
							}
						}
					}
					else if ( String.Compare(sCONTROL_TYPE, "HyperLink", true) == 0 )
					{
						HyperLink lnk = new HyperLink();
						// 11/08/2020 Paul.  An Admin Only control should not be added as it would allow the visible/hidden flag to ignore the admin only rule. 
						if ( bVisible )
							ctl.Controls.Add(lnk);
						if ( !Sql.IsEmptyString(sCONTROL_ID) )
							lnk.ID          = sCONTROL_ID;
						lnk.Text        = L10n.Term(sCONTROL_TEXT);
						lnk.NavigateUrl = String.Format(sURL_FORMAT, objTEXT_FIELD);
						lnk.Target      = sURL_TARGET;
						lnk.CssClass    = sCONTROL_CSSCLASS;
						// 11/21/2008 Paul.  On post back, we need to re-create the buttons, but don't change the visiblity flag. 
						// The problem is that we don't have the record at this early stage, so we cannot properly evaluate gASSIGNED_USER_ID. 
						// Not setting the visibility flag is not an issue because .NET will restore the previous visibility state on post back. 
						if ( !bIsPostBack )
						{
							// 07/28/2010 Paul.  We need a flag to exclude a button from a mobile device. 
							// 03/14/2014 Paul.  Allow hidden buttons to be created. 
							lnk.Visible     = (!bEXCLUDE_MOBILE || !bIsMobile) && (bMOBILE_ONLY && bIsMobile || !bMOBILE_ONLY) && (bADMIN_ONLY && Security.IS_ADMIN || !bADMIN_ONLY) && !bHIDDEN;
							if ( lnk.Visible && !Sql.IsEmptyString(sMODULE_NAME) && !Sql.IsEmptyString(sMODULE_ACCESS_TYPE) )
							{
								int nACLACCESS = SplendidCRM.Security.GetUserAccess(sMODULE_NAME, sMODULE_ACCESS_TYPE);
								// 08/11/2008 John.  Fix owner access rights.
								// 10/27/2008 Brian.  Only show button if show_unassigned is enabled.
								// 11/21/2008 Paul.  We need to make sure that an owner can create a new record. 
								lnk.Visible = (nACLACCESS > ACL_ACCESS.OWNER) || (nACLACCESS == ACL_ACCESS.OWNER && ((Security.USER_ID == gASSIGNED_USER_ID) || (!bIsPostBack && rdr == null) || (rdr != null && bShowUnassigned && Sql.IsEmptyGuid(gASSIGNED_USER_ID))));
								if ( lnk.Visible && !Sql.IsEmptyString(sTARGET_NAME) && !Sql.IsEmptyString(sTARGET_ACCESS_TYPE) )
								{
									// 08/11/2008 John.  Fix owner access rights.
									nACLACCESS = SplendidCRM.Security.GetUserAccess(sTARGET_NAME, sTARGET_ACCESS_TYPE);
									// 10/27/2008 Brian.  Only show button if show_unassigned is enabled.
									// 11/21/2008 Paul.  We need to make sure that an owner can create a new record. 
									lnk.Visible = (nACLACCESS > ACL_ACCESS.OWNER) || (nACLACCESS == ACL_ACCESS.OWNER && ((Security.USER_ID == gASSIGNED_USER_ID) || (!bIsPostBack && rdr == null) || (rdr != null && bShowUnassigned && Sql.IsEmptyGuid(gASSIGNED_USER_ID))));
								}
							}
						}
						if ( !Sql.IsEmptyString(sONCLICK_SCRIPT) )
						{
							lnk.Attributes.Add("onclick", sONCLICK_SCRIPT);
						}
						if ( !Sql.IsEmptyString(sCONTROL_ACCESSKEY) )
						{
							lnk.AccessKey = L10n.AccessKey(sCONTROL_ACCESSKEY);
						}
						if ( !Sql.IsEmptyString(sCONTROL_TOOLTIP) )
						{
							lnk.ToolTip = L10n.Term(sCONTROL_TOOLTIP);
							if ( lnk.ToolTip.Contains("[Alt]") )
							{
								if ( lnk.AccessKey.Length > 0 )
									lnk.ToolTip = lnk.ToolTip.Replace("[Alt]", "[Alt+" + lnk.AccessKey + "]");
								else
									lnk.ToolTip = lnk.ToolTip.Replace("[Alt]", String.Empty);
							}
						}
						// 04/04/2008 Paul.  Links need additional spacing.
						lnk.Attributes.Add("style", "margin-right: 3px; margin-left: 3px;");
						// 08/16/2017 Paul.  Add ability to apply a business rule to a button. 
						if ( !Sql.IsEmptyString(sBUSINESS_RULE) )
						{
							RuleValidation validation = new RuleValidation(typeof(DynamicButtonThis), null);
							RuleSet rules = RulesUtil.BuildRuleSet(sBUSINESS_RULE, validation);
							rules.Validate(validation);
							if ( validation.Errors.HasErrors )
							{
								SplendidError.SystemError(new StackTrace(true).GetFrame(0), RulesUtil.GetValidationErrors(validation));
							}
							else
							{
								DynamicButtonThis swThis = new DynamicButtonThis(lnk, L10n);
								RuleExecution exec = new RuleExecution(validation, swThis);
								rules.Execute(exec);
							}
						}
					}
					else if ( String.Compare(sCONTROL_TYPE, "ButtonLink", true) == 0 )
					{
						Button btn = new Button();
						// 11/08/2020 Paul.  An Admin Only control should not be added as it would allow the visible/hidden flag to ignore the admin only rule. 
						if ( bVisible )
							ctl.Controls.Add(btn);
						if ( !Sql.IsEmptyString(sCONTROL_ID) )
							btn.ID              = sCONTROL_ID;
						btn.Text            = "  " + L10n.Term(sCONTROL_TEXT) + "  ";
						btn.CssClass        = sCONTROL_CSSCLASS;
						// 05/25/2015 Paul.  New style for Seven theme buttons. 
						if ( ctlHover != null )
						{
							btn.CssClass = iButton == 0 ? sButtonStyle + "FirstButton" : sButtonStyle + "OtherButton";
						}
						// 03/21/2008 Paul.  Keep the command just in case we are in a browser that does not support javascript. 
						btn.Command        += Page_Command;
						btn.CommandName     = sCOMMAND_NAME;
						// 04/04/2016 Paul.  We want the ability to use the ~/ root instead of just ../. 
						sURL_FORMAT = sURL_FORMAT.Replace("~/", Sql.ToString(HttpContext.Current.Application["rootURL"]));
						// 08/22/2010 Paul.  Provide a way to override the default URL behavior and run javascript. 
						if ( !Sql.IsEmptyString(sONCLICK_SCRIPT) )
							btn.OnClientClick   = String.Format(sONCLICK_SCRIPT, objTEXT_FIELD);
						// 03/24/2016 Paul.  Receive signature from jQuery Signature popup. 
						else if ( sURL_TARGET.EndsWith("PDF") || sURL_TARGET == "vCard" || Sql.IsEmptyString(sURL_TARGET) )
							btn.OnClientClick   = "window.location.href='" + Sql.EscapeJavaScript(String.Format(sURL_FORMAT, objTEXT_FIELD)) + "'; return false;";
						else
							btn.OnClientClick   = "window.open('" + Sql.EscapeJavaScript(String.Format(sURL_FORMAT, objTEXT_FIELD)) + "', '" + sURL_TARGET + "', '" +  SplendidCRM.Crm.Config.PopupWindowOptions() + "'); return false;";
						// 11/21/2008 Paul.  On post back, we need to re-create the buttons, but don't change the visiblity flag. 
						// The problem is that we don't have the record at this early stage, so we cannot properly evaluate gASSIGNED_USER_ID. 
						// Not setting the visibility flag is not an issue because .NET will restore the previous visibility state on post back. 
						if ( !bIsPostBack )
						{
							// 07/28/2010 Paul.  We need a flag to exclude a button from a mobile device. 
							// 03/14/2014 Paul.  Allow hidden buttons to be created. 
							btn.Visible     = (!bEXCLUDE_MOBILE || !bIsMobile) && (bMOBILE_ONLY && bIsMobile || !bMOBILE_ONLY) && (bADMIN_ONLY && Security.IS_ADMIN || !bADMIN_ONLY) && !bHIDDEN;
							if ( btn.Visible && !Sql.IsEmptyString(sMODULE_NAME) && !Sql.IsEmptyString(sMODULE_ACCESS_TYPE) )
							{
								int nACLACCESS = SplendidCRM.Security.GetUserAccess(sMODULE_NAME, sMODULE_ACCESS_TYPE);
								// 08/11/2008 John.  Fix owner access rights.
								// 10/27/2008 Brian.  Only show button if show_unassigned is enabled.
								// 11/21/2008 Paul.  We need to make sure that an owner can create a new record. 
								btn.Visible = (nACLACCESS > ACL_ACCESS.OWNER) || (nACLACCESS == ACL_ACCESS.OWNER && ((Security.USER_ID == gASSIGNED_USER_ID) || (!bIsPostBack && rdr == null) || (rdr != null && bShowUnassigned && Sql.IsEmptyGuid(gASSIGNED_USER_ID))));
								if ( btn.Visible && !Sql.IsEmptyString(sTARGET_NAME) && !Sql.IsEmptyString(sTARGET_ACCESS_TYPE) )
								{
									// 08/11/2008 John.  Fix owner access rights.
									nACLACCESS = SplendidCRM.Security.GetUserAccess(sTARGET_NAME, sTARGET_ACCESS_TYPE);
									// 10/27/2008 Brian.  Only show button if show_unassigned is enabled.
									// 11/21/2008 Paul.  We need to make sure that an owner can create a new record. 
									btn.Visible = (nACLACCESS > ACL_ACCESS.OWNER) || (nACLACCESS == ACL_ACCESS.OWNER && ((Security.USER_ID == gASSIGNED_USER_ID) || (!bIsPostBack && rdr == null) || (rdr != null && bShowUnassigned && Sql.IsEmptyGuid(gASSIGNED_USER_ID))));
								}
							}
						}
						if ( !Sql.IsEmptyString(sCONTROL_ACCESSKEY) )
						{
							btn.AccessKey = L10n.AccessKey(sCONTROL_ACCESSKEY);
						}
						if ( !Sql.IsEmptyString(sCONTROL_TOOLTIP) )
						{
							btn.ToolTip = L10n.Term (sCONTROL_TOOLTIP);
							if ( btn.ToolTip.Contains("[Alt]") )
							{
								if ( btn.AccessKey.Length > 0 )
									btn.ToolTip = btn.ToolTip.Replace("[Alt]", "[Alt+" + btn.AccessKey + "]");
								else
									btn.ToolTip = btn.ToolTip.Replace("[Alt]", String.Empty);
							}
						}
						// 05/25/2015 Paul.  We don't want the spacer in the Seven module header. 
						if ( !(iButton == 0 && ctlHover != null) )
							btn.Attributes.Add("style", "margin-right: 3px;");
						// 08/16/2017 Paul.  Add ability to apply a business rule to a button. 
						if ( !Sql.IsEmptyString(sBUSINESS_RULE) )
						{
							RuleValidation validation = new RuleValidation(typeof(DynamicButtonThis), null);
							RuleSet rules = RulesUtil.BuildRuleSet(sBUSINESS_RULE, validation);
							rules.Validate(validation);
							if ( validation.Errors.HasErrors )
							{
								SplendidError.SystemError(new StackTrace(true).GetFrame(0), RulesUtil.GetValidationErrors(validation));
							}
							else
							{
								DynamicButtonThis swThis = new DynamicButtonThis(btn, L10n);
								RuleExecution exec = new RuleExecution(validation, swThis);
								rules.Execute(exec);
							}
						}
					}
				}
			}
			return nButtonCount;
		}

		/*
		private static ImageButton CreateLayoutImageButton(Guid gID, string sCommandName, int nFIELD_INDEX, string sAlternateText, string sImageUrl, CommandEventHandler Page_Command)
		{
			ImageButton btnDelete = new ImageButton();
			// 01/07/2006 Paul.  The problem with the ImageButton Delete event was that the dynamically rendered ID 
			// was not being found on every other page request.  The solution was to manually name and number the ImageButton IDs.
			// Make sure not to use ":" in the name, otherwise it will confuse the FindControl function. 
			btnDelete.ID              = sCommandName + "." + gID.ToString();
			btnDelete.CommandName     = sCommandName        ;
			btnDelete.CommandArgument = nFIELD_INDEX.ToString();
			btnDelete.CssClass        = "listViewTdToolsS1" ;
			// 08/18/2010 Paul.  IE8 does not support alt any more, so we need to use ToolTip instead. 
			btnDelete.ToolTip         = sAlternateText      ;
			btnDelete.ImageUrl        = sImageUrl           ;
			btnDelete.BorderWidth     = 0                   ;
			btnDelete.Width           = 12                  ;
			btnDelete.Height          = 12                  ;
			btnDelete.ImageAlign      = ImageAlign.AbsMiddle;
			if ( Page_Command != null )
				btnDelete.Command += Page_Command;
			return btnDelete;
		}
		*/

		// 04/09/2008 Paul.  Use SkinID to define the image. 
		// 12/24/2008 Paul.  We need access to this function for the merge module. 
		public static ImageButton CreateLayoutImageButtonSkin(Guid gID, string sCommandName, int nFIELD_INDEX, string sAlternateText, string sSkinID, CommandEventHandler Page_Command)
		{
			ImageButton btnDelete = new ImageButton();
			// 01/07/2006 Paul.  The problem with the ImageButton Delete event was that the dynamically rendered ID 
			// was not being found on every other page request.  The solution was to manually name and number the ImageButton IDs.
			// Make sure not to use ":" in the name, otherwise it will confuse the FindControl function. 
			btnDelete.ID              = sCommandName + "." + gID.ToString();
			btnDelete.CommandName     = sCommandName        ;
			btnDelete.CommandArgument = nFIELD_INDEX.ToString();
			btnDelete.CssClass        = "listViewTdToolsS1" ;
			// 08/18/2010 Paul.  IE8 does not support alt any more, so we need to use ToolTip instead. 
			btnDelete.ToolTip         = sAlternateText      ;
			btnDelete.SkinID          = sSkinID             ;
			if ( Page_Command != null )
				btnDelete.Command += Page_Command;
			return btnDelete;
		}

		public static void AppendDetailViewFields(string sDETAIL_NAME, HtmlTable tbl, DataRow rdr, L10N L10n, TimeZone T10n, CommandEventHandler Page_Command)
		{
			if ( tbl == null )
			{
				SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), "HtmlTable is not defined for " + sDETAIL_NAME);
				return;
			}
			// 05/05/2016 Paul.  The User Primary Role is used with role-based views. 
			DataTable dtFields = SplendidCache.DetailViewFields(sDETAIL_NAME, Security.PRIMARY_ROLE_NAME);
			AppendDetailViewFields(dtFields.DefaultView, tbl, rdr, L10n, T10n, Page_Command, false);
		}

		public static void AppendDetailViewFields(DataView dvFields, HtmlTable tbl, DataRow rdr, L10N L10n, TimeZone T10n, CommandEventHandler Page_Command, bool bLayoutMode)
		{
			bool bIsMobile = false;
			SplendidPage Page = tbl.Page as SplendidPage;
			if ( Page != null )
				bIsMobile = Page.IsMobile;
			// 11/23/2009 Paul.  We need to make sure that AJAX is available before we use it. 
			ScriptManager mgrAjax = ScriptManager.GetCurrent(tbl.Page);

			HtmlTableRow tr = null;
			// 11/28/2005 Paul.  Start row index using the existing count so that headers can be specified. 
			int nRowIndex = tbl.Rows.Count - 1;
			int nColIndex = 0;
			// 01/07/2006 Paul.  Show table borders in layout mode. This will help distinguish blank lines from wrapped lines. 
			if ( bLayoutMode )
				tbl.Border = 1;
			// 03/30/2007 Paul.  Convert the currency values before displaying. 
			// The UI culture should already be set to format the currency. 
			Currency C10n = HttpContext.Current.Items["C10n"] as Currency;
			HttpSessionState Session = HttpContext.Current.Session;
			// 11/15/2007 Paul.  If there are no fields in the detail view, then hide the entire table. 
			// This allows us to hide the table by removing all detail view fields. 
			// 09/12/2009 Paul.  There is no reason to hide the table when in layout mode. 
			if ( dvFields.Count == 0 && tbl.Rows.Count <= 1 && !bLayoutMode )
				tbl.Visible = false;
			
			// 01/27/2008 Paul.  We need the schema table to determine if the data label is free-form text. 
			// 03/21/2008 Paul.  We need to use a view to search for the rows for the ColumnName. 
			// 01/18/2010 Paul.  To apply ACL Field Security, we need to know if the current record has an ASSIGNED_USER_ID field, and its value. 
			// 06/30/2018 Paul.  Preprocess the erased fields for performance. 
			Guid gASSIGNED_USER_ID = Guid.Empty;
			List<string> arrERASED_FIELDS = new List<string>();
			//DataView vwSchema = null;
			if ( rdr != null )
			{
				// 11/22/2010 Paul.  Convert data reader to data table for Rules Wizard. 
				//vwSchema = new DataView(rdr.GetSchemaTable());
				//vwSchema.RowFilter = "ColumnName = 'ASSIGNED_USER_ID'";
				//if ( vwSchema.Count > 0 )
				if ( rdr.Table.Columns.Contains("ASSIGNED_USER_ID") )
				{
					gASSIGNED_USER_ID = Sql.ToGuid(rdr["ASSIGNED_USER_ID"]);
				}
				if ( Crm.Config.enable_data_privacy() )
				{
					if ( rdr.Table.Columns.Contains("ERASED_FIELDS") )
					{
						string sERASED_FIELDS = Sql.ToString(rdr["ERASED_FIELDS"]);
						if ( !Sql.IsEmptyString(sERASED_FIELDS) )
						{
							arrERASED_FIELDS.AddRange(sERASED_FIELDS.Split(','));
						}
					}
				}
			}

			// 08/02/2010 Paul.  The cell fields need to be outside the loop in order to support COLSPAN=-1. 
			HtmlTableCell tdLabel = null;
			HtmlTableCell tdField = null;
			// 01/01/2008 Paul.  Pull config flag outside the loop. 
			bool bEnableTeamManagement = Crm.Config.enable_team_management();
			// 08/28/2009 Paul.  Allow dynamic teams to be turned off. 
			bool bEnableDynamicTeams   = Crm.Config.enable_dynamic_teams();
			// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
			bool bEnableDynamicAssignment = Crm.Config.enable_dynamic_assignment();
			// 09/16/2018 Paul.  Create a multi-tenant system. 
			if ( Crm.Config.enable_multi_tenant_teams() )
			{
				bEnableTeamManagement    = false;
				bEnableDynamicTeams      = false;
				bEnableDynamicAssignment = false;
			}
			
			HttpApplicationState Application = HttpContext.Current.Application;
			// 08/22/2012 Paul.  We need to prevent duplicate names. 
			Hashtable hashLABEL_IDs = new Hashtable();
			bool bSupportsDraggable = Sql.ToBoolean(Session["SupportsDraggable"]);
			// 12/13/2013 Paul.  Allow each line item to have a separate tax rate. 
			bool bEnableTaxLineItems = Sql.ToBoolean(HttpContext.Current.Application["CONFIG.Orders.TaxLineItems"]);
			foreach(DataRowView row in dvFields)
			{
				string sDETAIL_NAME = Sql.ToString (row["DETAIL_NAME"]);
				Guid   gID          = Sql.ToGuid   (row["ID"         ]);
				int    nFIELD_INDEX = Sql.ToInteger(row["FIELD_INDEX"]);
				string sFIELD_TYPE  = Sql.ToString (row["FIELD_TYPE" ]);
				string sDATA_LABEL  = Sql.ToString (row["DATA_LABEL" ]);
				string sDATA_FIELD  = Sql.ToString (row["DATA_FIELD" ]);
				string sDATA_FORMAT = Sql.ToString (row["DATA_FORMAT"]);
				string sLIST_NAME   = Sql.ToString (row["LIST_NAME"  ]);
				int    nCOLSPAN     = Sql.ToInteger(row["COLSPAN"    ]);
				string LABEL_WIDTH = Sql.ToString (row["LABEL_WIDTH"]);
				string sFIELD_WIDTH = Sql.ToString (row["FIELD_WIDTH"]);
				int    nDATA_COLUMNS= Sql.ToInteger(row["DATA_COLUMNS"]);
				// 08/02/2010 Paul.  Move URL fields to the top of the loop. 
				string sURL_FIELD   = Sql.ToString (row["URL_FIELD"  ]);
				string sURL_FORMAT  = Sql.ToString (row["URL_FORMAT" ]);
				string sURL_TARGET  = Sql.ToString (row["URL_TARGET" ]);
				// 06/12/2009 Paul.  Add TOOL_TIP for help hover.
				string sTOOL_TIP    = String.Empty;
				try
				{
					sTOOL_TIP = Sql.ToString (row["TOOL_TIP"]);
				}
				catch(Exception ex)
				{
					// 06/12/2009 Paul.  The TOOL_TIP is not in the view, then log the error and continue. 
					SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
				}
				// 02/16/2010 Paul.  Add MODULE_TYPE so that we can lookup custom field IDs. 
				string sMODULE_TYPE = String.Empty;
				try
				{
					sMODULE_TYPE = Sql.ToString (row["MODULE_TYPE"]);
				}
				catch(Exception ex)
				{
					// 06/16/2010 Paul.  The MODULE_TYPE is not in the view, then log the error and continue. 
					SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
				}
				// 10/09/2010 Paul.  Add PARENT_FIELD so that we can establish dependent listboxes. 
				string sPARENT_FIELD = String.Empty;
				try
				{
					sPARENT_FIELD = Sql.ToString (row["PARENT_FIELD"]);
				}
				catch(Exception ex)
				{
					// 10/09/2010 Paul.  The PARENT_FIELD is not in the view, then log the error and continue. 
					SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
				}
				// 12/02/2007 Paul.  Each view can now have its own number of data columns. 
				// This was needed so that search forms can have 4 data columns. The default is 2 columns. 
				if ( nDATA_COLUMNS == 0 )
					nDATA_COLUMNS = 2;

				// 01/18/2010 Paul.  To apply ACL Field Security, we need to know if the Module Name, which we will extract from the EditView Name. 
				string sMODULE_NAME = String.Empty;
				string[] arrDETAIL_NAME = sDETAIL_NAME.Split('.');
				if ( arrDETAIL_NAME.Length > 0 )
					sMODULE_NAME = arrDETAIL_NAME[0];
				bool bIsReadable  = true;
				// 06/16/2010 Paul.  sDATA_FIELD may be empty. 
				if ( SplendidInit.bEnableACLFieldSecurity && !Sql.IsEmptyString(sDATA_FIELD) )
				{
					Security.ACL_FIELD_ACCESS acl = Security.GetUserFieldSecurity(sMODULE_NAME, sDATA_FIELD, gASSIGNED_USER_ID);
					bIsReadable  = acl.IsReadable();
				}

				// 11/25/2006 Paul.  If Team Management has been disabled, then convert the field to a blank. 
				// Keep the field, but treat it as blank so that field indexes will still be valid. 
				// 12/03/2006 Paul.  Allow the team field to be visible during layout. 
				// 12/03/2006 Paul.  The correct field is TEAM_NAME.  We don't use TEAM_ID in the detail view. 
				// 08/24/2009 Paul.  Add support for dynamic teams. 
				if ( !bLayoutMode && (sDATA_FIELD == "TEAM_NAME" || sDATA_FIELD == "TEAM_SET_NAME") )
				{
					if ( !bEnableTeamManagement )
					{
						sFIELD_TYPE = "Blank";
					}
					else if ( bEnableDynamicTeams )
					{
						// 05/06/2018 Paul.  Change to single instead of 1 to prevent auto-postback. 
						if ( sDATA_FORMAT != "1" && !sDATA_FORMAT.ToLower().Contains("single") )
						{
							// 08/28/2009 Paul.  If dynamic teams are enabled, then always use the set name. 
							sDATA_LABEL = ".LBL_TEAM_SET_NAME";
							sDATA_FIELD = "TEAM_SET_NAME";
						}
						else
						{
							sDATA_LABEL = ".LBL_TEAM_NAME";
							sDATA_FIELD = "TEAM_NAME";
						}
					}
				}
				// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
				if ( !bLayoutMode && (sDATA_FIELD == "ASSIGNED_TO" || sDATA_FIELD == "ASSIGNED_TO_NAME" || sDATA_FIELD == "ASSIGNED_SET_NAME") )
				{
					// 12/17/2017 Paul.  Allow a layout to remain singular with DATA_FORMAT = 1. 
					// 05/06/2018 Paul.  Change to single instead of 1 to prevent auto-postback. 
					if ( bEnableDynamicAssignment && !sDATA_FORMAT.ToLower().Contains("single") )
					{
						sDATA_LABEL = ".LBL_ASSIGNED_SET_NAME";
						sDATA_FIELD = "ASSIGNED_SET_NAME";
					}
					else if ( sDATA_FIELD == "ASSIGNED_SET_NAME" )
					{
						sDATA_LABEL = ".LBL_ASSIGNED_TO";
						sDATA_FIELD = "ASSIGNED_TO_NAME";
					}
				}
				// 12/13/2013 Paul.  Allow each product to have a default tax rate. 
				if ( !bLayoutMode && sDATA_FIELD == "TAX_CLASS" )
				{
					if ( bEnableTaxLineItems )
					{
						// 08/28/2009 Paul.  If dynamic teams are enabled, then always use the set name. 
						sDATA_LABEL = "ProductTemplates.LBL_TAXRATE_ID";
						sDATA_FIELD = "TAXRATE_ID";
						sLIST_NAME  = "TaxRates";
					}
				}

				// 04/04/2010 Paul.  Hide the Exchange Folder field if disabled for this module or user. 
				if ( !bLayoutMode && sDATA_FIELD == "EXCHANGE_FOLDER" )
				{
					if ( !Crm.Modules.ExchangeFolders(sMODULE_NAME) || !Security.HasExchangeAlias() )
					{
						sFIELD_TYPE = "Blank";
					}
				}
				// 09/02/2012 Paul.  A separator will create a new table. We need to match the outer and inner layout. 
				if ( String.Compare(sFIELD_TYPE, "Separator", true) == 0 )
				{
					System.Web.UI.HtmlControls.HtmlTable tblNew = new System.Web.UI.HtmlControls.HtmlTable();
					tblNew.Attributes.Add("class", "tabDetailView");
					tblNew.Style.Add(HtmlTextWriterStyle.MarginTop, "5px");
					// 09/27/2012 Paul.  Separator can have an ID and can have a style so that it can be hidden. 
					if ( !Sql.IsEmptyString(sDATA_FIELD) )
						tblNew.ID = sDATA_FIELD;
					if ( !Sql.IsEmptyString(sDATA_FORMAT) && !bLayoutMode )
						tblNew.Style.Add(HtmlTextWriterStyle.Display, sDATA_FORMAT);
					int nParentIndex = tbl.Parent.Controls.IndexOf(tbl);
					tbl.Parent.Controls.AddAt(nParentIndex + 1, tblNew);
					tbl = tblNew;
					
					nRowIndex = -1;
					nColIndex = 0;
					tdLabel = null;
					tdField = null;
					if ( bLayoutMode )
						tbl.Border = 1;
					else
						continue;
				}
				// 11/17/2007 Paul.  On a mobile device, each new field is on a new row. 
				// 08/02/2010 Paul. COLSPAN == -1 means that a new column should not be created. 
				if ( (nCOLSPAN >= 0 && nColIndex == 0) || tr == null || bIsMobile )
				{
					// 11/25/2005 Paul.  Don't pre-create a row as we don't want a blank
					// row at the bottom.  Add rows just before they are needed. 
					nRowIndex++;
					tr = new HtmlTableRow();
					tbl.Rows.Insert(nRowIndex, tr);
				}
				if ( bLayoutMode )
				{
					HtmlTableCell tdAction = new HtmlTableCell();
					tr.Cells.Add(tdAction);
					tdAction.Attributes.Add("class", "tabDetailViewDL");
					tdAction.NoWrap = true;

					Literal litIndex = new Literal();
					litIndex.Text = "&nbsp;" + nFIELD_INDEX.ToString() + "&nbsp;";
					tdAction.Controls.Add(litIndex   );

					// 05/26/2007 Paul.  Fix the terms. The are in the Dropdown module. 
					// 05/18/2013 Paul.  Add drag handle. 
					if ( bSupportsDraggable )
					{
						Image imgDragIcon = new Image();
						imgDragIcon.SkinID = "draghandle_table";
						imgDragIcon.Attributes.Add("draggable"  , "true");
						imgDragIcon.Attributes.Add("ondragstart", "event.dataTransfer.setData('Text', '" + nFIELD_INDEX.ToString() + "');");
						tdAction.Controls.Add(imgDragIcon);
						// 08/08/2013 Paul.  IE does not support preventDefault. 
						// http://stackoverflow.com/questions/1000597/event-preventdefault-function-not-working-in-ie
						tdAction.Attributes.Add("ondragover", "LayoutDragOver(event, '" + nFIELD_INDEX.ToString() + "')");
						tdAction.Attributes.Add("ondrop"    , "LayoutDropIndex(event, '" + nFIELD_INDEX.ToString() + "')");
					}
					else
					{
						ImageButton btnMoveUp   = CreateLayoutImageButtonSkin(gID, "Layout.MoveUp"  , nFIELD_INDEX, L10n.Term("Dropdown.LNK_UP"    ), "uparrow_inline"  , Page_Command);
						ImageButton btnMoveDown = CreateLayoutImageButtonSkin(gID, "Layout.MoveDown", nFIELD_INDEX, L10n.Term("Dropdown.LNK_DOWN"  ), "downarrow_inline", Page_Command);
						tdAction.Controls.Add(btnMoveUp  );
						tdAction.Controls.Add(btnMoveDown);
					}
					ImageButton btnInsert   = CreateLayoutImageButtonSkin(gID, "Layout.Insert"  , nFIELD_INDEX, L10n.Term("Dropdown.LNK_INS"   ), "plus_inline"     , Page_Command);
					ImageButton btnEdit     = CreateLayoutImageButtonSkin(gID, "Layout.Edit"    , nFIELD_INDEX, L10n.Term("Dropdown.LNK_EDIT"  ), "edit_inline"     , Page_Command);
					ImageButton btnDelete   = CreateLayoutImageButtonSkin(gID, "Layout.Delete"  , nFIELD_INDEX, L10n.Term("Dropdown.LNK_DELETE"), "delete_inline"   , Page_Command);
					tdAction.Controls.Add(btnInsert  );
					tdAction.Controls.Add(btnEdit    );
					tdAction.Controls.Add(btnDelete  );
				}
				// 08/02/2010 Paul.  Move literal label up so that it can be accessed when processing a blank. 
				Literal   litLabel = new Literal();
				// 08/22/2012 Paul.  Try and create a safe label ID so that it can be accessed using FindControl(). 
				// We are using the DATA_FIELD to match the logic in the EditView area. 
				if ( !Sql.IsEmptyString(sDATA_FIELD) && !hashLABEL_IDs.Contains(sDATA_FIELD) )
				{
					litLabel.ID = sDATA_FIELD.Replace(" ", "_").Replace(".", "_") + "_LABEL";
					hashLABEL_IDs.Add(sDATA_FIELD, null);
				}
				HyperLink lnkField = null;
				if ( nCOLSPAN >= 0 || tdLabel == null || tdField == null )
				{
					// 05/28/2015 Paul.  The Seven theme has labels stacked above values. 
					if ( SplendidDynamic.StackedLayout(Page.Theme, sDETAIL_NAME) )
					{
						tdLabel = new HtmlTableCell();
						tdField = tdLabel;
						//tdLabel.Attributes.Add("class", "tabDetailViewDL");
						//tdLabel.VAlign = "top";
						//tdLabel.Width  = LABEL_WIDTH;
						tdField.Attributes.Add("class", "tabStackedDetailViewDF");
						tdField.VAlign = "top";
						//tr.Cells.Add(tdLabel);
						tr.Cells.Add(tdField);
						if ( nCOLSPAN > 0 )
						{
							tdField.ColSpan = (nCOLSPAN + 1) / 2;
							if ( bLayoutMode )
								tdField.ColSpan++;
						}
						tdField.Width  = (100 / nDATA_COLUMNS).ToString() + "%";
						// 05/28/2015 Paul.  Wrap the label in a div. 
						HtmlGenericControl span = new HtmlGenericControl("span");
						span.Attributes.Add("class", "tabStackedDetailViewDL");
						tdLabel.Controls.Add(span);
						span.Controls.Add(litLabel);
					}
					else
					{
						tdLabel = new HtmlTableCell();
						tdField = new HtmlTableCell();
						tdLabel.Attributes.Add("class", "tabDetailViewDL");
						tdLabel.VAlign = "top";
						tdLabel.Width  = LABEL_WIDTH;
						tdField.Attributes.Add("class", "tabDetailViewDF");
						tdField.VAlign = "top";
						tr.Cells.Add(tdLabel);
						tr.Cells.Add(tdField);
						if ( nCOLSPAN > 0 )
						{
							tdField.ColSpan = nCOLSPAN;
							if ( bLayoutMode )
								tdField.ColSpan++;
						}
						// 11/28/2005 Paul.  Don't use the field width if COLSPAN is specified as we want it to take the rest of the table.  The label width will be sufficient. 
						if ( nCOLSPAN == 0 )
							tdField.Width  = sFIELD_WIDTH;
					
						// 08/02/2010 Paul.  The label will get skipped if we are processing COLSPAN=-1. 
						tdLabel.Controls.Add(litLabel);
					}
					// 01/18/2010 Paul.  Apply ACL Field Security. 
					litLabel.Visible = bLayoutMode || bIsReadable;
					//litLabel.Text = nFIELD_INDEX.ToString() + " (" + nRowIndex.ToString() + "," + nColIndex.ToString() + ")";
					try
					{
						if ( bLayoutMode )
							litLabel.Text = sDATA_LABEL;
						else if ( sDATA_LABEL.IndexOf(".") >= 0 )
							litLabel.Text = L10n.Term(sDATA_LABEL);
						else if ( !Sql.IsEmptyString(sDATA_LABEL) && rdr != null )
						{
							// 01/27/2008 Paul.  If the data label is not in the schema table, then it must be free-form text. 
							// It is not used often, but we allow the label to come from the result set.  For example,
							// when the parent is stored in the record, we need to pull the module name from the record. 
							litLabel.Text = sDATA_LABEL;
							// 11/22/2010 Paul.  Convert data reader to data table for Rules Wizard. 
							//if ( vwSchema != null )
							if ( rdr != null )
							{
								//vwSchema.RowFilter = "ColumnName = '" + Sql.EscapeSQL(sDATA_LABEL) + "'";
								//if ( vwSchema.Count > 0 )
								if ( rdr.Table.Columns.Contains(sDATA_LABEL) )
									litLabel.Text = Sql.ToString(rdr[sDATA_LABEL]) + L10n.Term("Calls.LBL_COLON");
							}
						}
						// 07/15/2006 Paul.  Always put something for the label so that table borders will look right. 
						else
							litLabel.Text = "&nbsp;";
						// 05/28/2015 Paul.  The Seven theme has labels stacked above values. 
						if ( SplendidDynamic.StackedLayout(Page.Theme, sDETAIL_NAME) && litLabel.Text.EndsWith(":") )
							litLabel.Text = litLabel.Text.Substring(0, litLabel.Text.Length - 1);

						// 06/12/2009 Paul.  Add Tool Tip hover. 
						// 11/23/2009 Paul.  Only add tool tip if AJAX is available and this is not a mobile device. 
						// 01/18/2010 Paul.  Only add tool tip if the label is visible. 
						if ( !bIsMobile && mgrAjax != null && !Sql.IsEmptyString(sTOOL_TIP) && !Sql.IsEmptyString(sDATA_FIELD) && litLabel.Visible )
						{
							Image imgToolTip = new Image();
							imgToolTip.SkinID = "tooltip_inline";
							// 07/06/2017 Paul.  IDs should not have spaces, but we do allow multiple data fields. 
							imgToolTip.ID     = sDATA_FIELD.Replace(" ", "_") + "_TOOLTIP_IMAGE";
							tdLabel.Controls.Add(imgToolTip);
							
							Panel pnlToolTip = new Panel();
							pnlToolTip.ID       = sDATA_FIELD.Replace(" ", "_") + "_TOOLTIP_PANEL";
							pnlToolTip.CssClass = "tooltip";
							tdLabel.Controls.Add(pnlToolTip);

							Literal litToolTip = new Literal();
							litToolTip.Text = sDATA_FIELD.Replace(" ", "_");
							pnlToolTip.Controls.Add(litToolTip);
							if ( bLayoutMode )
								litToolTip.Text = sTOOL_TIP;
							else if ( sTOOL_TIP.IndexOf(".") >= 0 )
								litToolTip.Text = L10n.Term(sTOOL_TIP);
							else
								litToolTip.Text = sTOOL_TIP;
							
							AjaxControlToolkit.HoverMenuExtender hovToolTip = new AjaxControlToolkit.HoverMenuExtender();
							hovToolTip.TargetControlID = imgToolTip.ID;
							hovToolTip.PopupControlID  = pnlToolTip.ID;
							hovToolTip.PopupPosition   = AjaxControlToolkit.HoverMenuPopupPosition.Right;
							hovToolTip.PopDelay        = 50;
							hovToolTip.OffsetX         = 0;
							hovToolTip.OffsetY         = 0;
							tdLabel.Controls.Add(hovToolTip);
						}
					}
					catch(Exception ex)
					{
						SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
						litLabel.Text = ex.Message;
					}
				}
				
				if ( String.Compare(sFIELD_TYPE, "Blank", true) == 0 )
				{
					Literal litField = new Literal();
					tdField.Controls.Add(litField);
					if ( bLayoutMode )
					{
						litLabel.Text = "*** BLANK ***";
						litField.Text = "*** BLANK ***";
					}
					else
					{
						// 12/03/2006 Paul.  Make sure to clear the label.  This is necessary to convert a TEAM to blank when disabled. 
						litLabel.Text = "&nbsp;";
						litField.Text = "&nbsp;";
					}
				}
				// 09/03/2012 Paul.  A separator does nothing in Layout mode. 
				else if ( String.Compare(sFIELD_TYPE, "Separator", true) == 0 )
				{
					if ( bLayoutMode )
					{
						litLabel.Text = "*** SEPARATOR ***";
						nColIndex = nDATA_COLUMNS;
						tdField.ColSpan = 2 * nDATA_COLUMNS - 1;
						// 09/03/2012 Paul.  When in layout mode, we need to add a column for arrangement. 
						tdField.ColSpan++;
					}
				}
				// 09/03/2012 Paul.  A header is similar to a label, but without the data field. 
				else if ( String.Compare(sFIELD_TYPE, "Header", true) == 0 )
				{
					// 06/05/2015 Paul.  Adding space in Seven theme is creating a blank line. 
					if ( tdField != tdLabel )
					{
						Literal litField = new Literal();
						tdField.Controls.Add(litField);
						litField.Text = "&nbsp;";
					}
					if ( !bLayoutMode )
					{
						litLabel.Text = "<h4>" + litLabel.Text + "</h4>";
					}
				}
				else if ( String.Compare(sFIELD_TYPE, "Line", true) == 0 )
				{
					if ( bLayoutMode )
					{
						Literal litField = new Literal();
						tdField.Controls.Add(litField);
						litLabel.Text = "*** LINE ***";
						litField.Text = "*** LINE ***";
					}
					else
					{
						tr.Cells.Clear();
					}
				}
				else if ( String.Compare(sFIELD_TYPE, "String", true) == 0 )
				{
					if ( bLayoutMode )
					{
						Literal litField = new Literal();
						litField.Text = sDATA_FIELD;
						tdField.Controls.Add(litField);
					}
					else if ( !Sql.IsEmptyString(sDATA_FIELD) )
					{
						// 12/06/2005 Paul.  Wrap all string fields in a SPAN tag to simplify regression testing. 
						HtmlGenericControl spnField = new HtmlGenericControl("span");
						tdField.Controls.Add(spnField);
						spnField.ID = sDATA_FIELD;

						Literal litField = new Literal();
						spnField.Controls.Add(litField);
						// 01/18/2010 Paul.  Apply ACL Field Security. 
						litField.Visible = bLayoutMode || bIsReadable;
						try
						{
							string[] arrLIST_NAME  = sLIST_NAME .Split(' ');
							string[] arrDATA_FIELD = sDATA_FIELD.Split(' ');
							// 10/09/2010 Paul.  Add PARENT_FIELD so that we can establish dependent listboxes. 
							string[] arrPARENT_FIELD = sPARENT_FIELD.Split(' ');
							object[] objDATA_FIELD = new object[arrDATA_FIELD.Length];
							for ( int i=0 ; i < arrDATA_FIELD.Length; i++ )
							{
								if ( arrDATA_FIELD[i].IndexOf(".") >= 0 )
								{
									objDATA_FIELD[i] = L10n.Term(arrDATA_FIELD[i]);
								}
								// 10/09/2010 Paul.  Add PARENT_FIELD so that we can establish dependent listboxes. 
								else if ( !Sql.IsEmptyString(sPARENT_FIELD) && !Sql.IsEmptyString(sLIST_NAME) )
								{
									if ( arrPARENT_FIELD.Length == arrLIST_NAME.Length && arrLIST_NAME.Length == arrDATA_FIELD.Length )
									{
										if ( rdr != null )
										{
											string sPARENT_LIST_NAME = Sql.ToString(rdr[arrPARENT_FIELD[i]]);
											if ( !Sql.IsEmptyString(sPARENT_LIST_NAME) )
											{
												bool bCustomCache = false;
												objDATA_FIELD[i] = SplendidCache.CustomList(sPARENT_LIST_NAME, Sql.ToString(rdr[arrDATA_FIELD[i]]), ref bCustomCache);
												if ( bCustomCache )
													continue;
												if ( Sql.ToString(rdr[arrDATA_FIELD[i]]).StartsWith("<?xml") )
												{
													StringBuilder sb = new StringBuilder();
													XmlDocument xml = new XmlDocument();
													// 01/20/2015 Paul.  Disable XmlResolver to prevent XML XXE. 
													// https://www.owasp.org/index.php/XML_External_Entity_(XXE)_Processing
													// http://stackoverflow.com/questions/14230988/how-to-prevent-xxe-attack-xmldocument-in-net
													xml.XmlResolver = null;
													xml.LoadXml(Sql.ToString(rdr[arrDATA_FIELD[i]]));
													XmlNodeList nlValues = xml.DocumentElement.SelectNodes("Value");
													foreach ( XmlNode xValue in nlValues )
													{
														if ( sb.Length > 0 )
															sb.Append(", ");
														sb.Append(L10n.Term("." + sPARENT_LIST_NAME + ".", xValue.InnerText));
													}
													objDATA_FIELD[i] = sb.ToString();
												}
												else
												{
													objDATA_FIELD[i] = L10n.Term("." + sPARENT_LIST_NAME + ".", rdr[arrDATA_FIELD[i]]);
												}
											}
											else
												objDATA_FIELD[i] = String.Empty;
										}
										else
											objDATA_FIELD[i] = String.Empty;
									}
								}
								// 06/30/2018 Paul.  The data field cannot be empty. 
								else if ( !Sql.IsEmptyString(sLIST_NAME) && !Sql.IsEmptyString(arrDATA_FIELD[i]) )
								{
									if ( arrLIST_NAME.Length == arrDATA_FIELD.Length )
									{
										if ( rdr != null )
										{
											// 06/30/2018 Paul.  Value may have been erased. If so, replace with Erased Value message. 
											if ( rdr[arrDATA_FIELD[i]] == DBNull.Value )
											{
												if ( arrERASED_FIELDS.Contains(arrDATA_FIELD[i]) )
												{
													objDATA_FIELD[i] = Sql.DataPrivacyErasedPill(L10n);
													continue;
												}
											}
											// 08/06/2008 Paul.  Use an array to define the custom caches so that list is in the Cache module. 
											// This should reduce the number of times that we have to edit the SplendidDynamic module. 
											bool bCustomCache = false;
											// 08/10/2008 Paul.  Use a shared function to simplify access to Custom Cache.
											objDATA_FIELD[i] = SplendidCache.CustomList(arrLIST_NAME[i], Sql.ToString(rdr[arrDATA_FIELD[i]]), ref bCustomCache);
											if ( bCustomCache )
											{
												// 06/27/2018 Paul.  csv and custom list requires exception. 
												if ( sDATA_FORMAT.ToLower() == "csv" )
												{
													string[] arrValues = Sql.ToString(rdr[arrDATA_FIELD[i]]).Split(',');
													objDATA_FIELD[i] = SplendidCache.CustomListValues(arrLIST_NAME[i], arrValues);
												}
												continue;
											}
											// 02/12/2008 Paul.  If the list contains XML, then treat as a multi-selection. 
											if ( Sql.ToString(rdr[arrDATA_FIELD[i]]).StartsWith("<?xml") )
											{
												StringBuilder sb = new StringBuilder();
												XmlDocument xml = new XmlDocument();
												// 01/20/2015 Paul.  Disable XmlResolver to prevent XML XXE. 
												// https://www.owasp.org/index.php/XML_External_Entity_(XXE)_Processing
												// http://stackoverflow.com/questions/14230988/how-to-prevent-xxe-attack-xmldocument-in-net
												xml.XmlResolver = null;
												xml.LoadXml(Sql.ToString(rdr[arrDATA_FIELD[i]]));
												XmlNodeList nlValues = xml.DocumentElement.SelectNodes("Value");
												foreach ( XmlNode xValue in nlValues )
												{
													if ( sb.Length > 0 )
														sb.Append(", ");
													sb.Append(L10n.Term("." + arrLIST_NAME[i] + ".", xValue.InnerText));
												}
												objDATA_FIELD[i] = sb.ToString();
											}
											// 06/27/2018 Paul.  csv and custom list requires exception. 
											else if ( sDATA_FORMAT.ToLower() == "csv" )
											{
												StringBuilder sb = new StringBuilder();
												string[] arrValues = Sql.ToString(rdr[arrDATA_FIELD[i]]).Split(',');
												foreach ( string sValue in arrValues )
												{
													if ( sb.Length > 0 )
														sb.Append(", ");
													sb.Append(L10n.Term("." + arrLIST_NAME[i] + ".", sValue));
												}
												objDATA_FIELD[i] = sb.ToString();
											}
											else
											{
												objDATA_FIELD[i] = L10n.Term("." + arrLIST_NAME[i] + ".", rdr[arrDATA_FIELD[i]]);
											}
										}
										else
											objDATA_FIELD[i] = String.Empty;
									}
								}
								else if ( !Sql.IsEmptyString(arrDATA_FIELD[i]) )
								{
									if ( rdr != null && rdr[arrDATA_FIELD[i]] != DBNull.Value)
									{
										// 12/05/2005 Paul.  If the data is a DateTime field, then make sure to perform the timezone conversion. 
										if ( rdr[arrDATA_FIELD[i]].GetType() == Type.GetType("System.DateTime") )
											objDATA_FIELD[i] = T10n.FromServerTime(rdr[arrDATA_FIELD[i]]);
										// 02/16/2010 Paul.  Add MODULE_TYPE so that we can lookup custom field IDs. 
										// 02/16/2010 Paul.  Move ToGuid to the function so that it can be captured if invalid. 
										// 09/15/2014 Paul.  Prevent Cross-Site Scripting by HTML encoding the data. 
										else if ( !Sql.IsEmptyString(sMODULE_TYPE) )
											objDATA_FIELD[i] = HttpUtility.HtmlEncode(Crm.Modules.ItemName(Application, sMODULE_TYPE, rdr[arrDATA_FIELD[i]]));
										else if ( rdr[arrDATA_FIELD[i]].GetType() == typeof(System.String) )
										{
											// 09/15/2014 Paul.  Special case where we format address fields in HTML in the SQL View. 
											// We need to un-enode <br>. 
											// 12/09/2104 Paul.  Encoding HTML fields makes it difficult to see the Email Template for campaigns. 
											if ( sDATA_FIELD.EndsWith("_HTML") )
												objDATA_FIELD[i] = Sql.ToString(rdr[arrDATA_FIELD[i]]);  // HttpUtility.HtmlEncode(Sql.ToString(rdr[arrDATA_FIELD[i]])).Replace("&lt;br&gt;", "<br />").Replace("&amp;nbsp;", "&nbsp;");
											else
												objDATA_FIELD[i] = HttpUtility.HtmlEncode(Sql.ToString(rdr[arrDATA_FIELD[i]]));
										}
										else
											objDATA_FIELD[i] = rdr[arrDATA_FIELD[i]];
									}
									else
									{
										// 06/30/2018 Paul.  Value may have been erased. If so, replace with Erased Value message. 
										if ( arrERASED_FIELDS.Contains(arrDATA_FIELD[i]) )
										{
											objDATA_FIELD[i] = Sql.DataPrivacyErasedPill(L10n);
										}
									}
								}
							}
							if ( rdr != null )
							{
								// 01/09/2006 Paul.  Allow DATA_FORMAT to be optional.   If missing, write data directly. 
								if ( sDATA_FORMAT == String.Empty )
								{
									for ( int i=0; i < arrDATA_FIELD.Length; i++ )
										arrDATA_FIELD[i] = Sql.ToString(objDATA_FIELD[i]);
									litField.Text = String.Join(" ", arrDATA_FIELD);
								}
								else if ( sDATA_FORMAT == "{0:c}" && C10n != null )
								{
									// 03/30/2007 Paul.  Convert DetailView currencies on the fly. 
									// 05/05/2007 Paul.  In an earlier step, we convert NULLs to empty strings. 
									// Attempts to convert to decimal will generate an error: Input string was not in a correct format.
									// 04/19/2020 Paul.  Null value is no longer converted to an empty string. 
									if ( objDATA_FIELD[0] != null && !(objDATA_FIELD[0] is string) )
									{
										Decimal d = C10n.ToCurrency(Convert.ToDecimal(objDATA_FIELD[0]));
										litField.Text = d.ToString("c");
									}
								}
								// 06/27/2018 Paul.  csv and custom list requires exception. 
								else if ( sDATA_FORMAT.ToLower() == "csv" )
								{
									litField.Text = Sql.ToString(objDATA_FIELD[0]);
								}
								else
									litField.Text = String.Format(sDATA_FORMAT, objDATA_FIELD);
								/*
								// 08/02/2010 Paul.  Add javascript to the output. 
								// 08/02/2010 Paul.  The javascript will be moved to a separate record. 
								if ( !Sql.IsEmptyString(sURL_FIELD) && !Sql.IsEmptyString(sURL_FORMAT) )
								{
									Literal litUrlField = new Literal();
									tdField.Controls.Add(litUrlField);
									string[] arrURL_FIELD = sURL_FIELD.Split(' ');
									object[] objURL_FIELD = new object[arrURL_FIELD.Length];
									for ( int i=0 ; i < arrURL_FIELD.Length; i++ )
									{
										if ( !Sql.IsEmptyString(arrURL_FIELD[i]) )
										{
											// 07/26/2007 Paul.  Make sure to escape the javascript string. 
											if ( row[arrURL_FIELD[i]] != DBNull.Value )
												objURL_FIELD[i] = Sql.EscapeJavaScript(Sql.ToString(rdr[arrURL_FIELD[i]]));
											else
												objURL_FIELD[i] = String.Empty;
										}
									}
									// 12/03/2009 Paul.  LinkedIn Company Profile requires a span tag to insert the link.
									litUrlField.Text = "&nbsp;<span id=\"" + String.Format(sURL_TARGET, objURL_FIELD) + "\"></span>";
									litUrlField.Text += "<script type=\"text/javascript\"> " + String.Format(sURL_FORMAT, objURL_FIELD) + "</script>";
								}
								*/
							}
						}
						catch(Exception ex)
						{
							SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
							litField.Text = ex.Message;
						}
					}
				}
				else if ( String.Compare(sFIELD_TYPE, "CheckBox", true) == 0 )
				{
					if ( !Sql.IsEmptyString(sDATA_FIELD) )
					{
						CheckBox chkField = new CheckBox();
						tdField.Controls.Add(chkField);
						chkField.Enabled  = false     ;
						chkField.CssClass = "checkbox";
						// 03/16/2006 Paul.  Give the checkbox a name so that it can be validated with SplendidTest. 
						chkField.ID       = sDATA_FIELD;
						// 01/18/2010 Paul.  Apply ACL Field Security. 
						chkField.Visible  = bLayoutMode || bIsReadable;
						try
						{
							if ( rdr != null )
								chkField.Checked = Sql.ToBoolean(rdr[sDATA_FIELD]);
						}
						catch(Exception ex)
						{
							SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
						}
						if ( bLayoutMode )
						{
							Literal litField = new Literal();
							litField.Text = sDATA_FIELD;
							tdField.Controls.Add(litField);
						}
					}
				}
				else if ( String.Compare(sFIELD_TYPE, "Button", true) == 0 )
				{
					if ( !Sql.IsEmptyString(sDATA_FIELD) )
					{
						Button btnField = new Button();
						tdField.Controls.Add(btnField);
						btnField.CssClass = "button";
						// 03/16/2006 Paul.  Give the button a name so that it can be validated with SplendidTest. 
						btnField.ID       = sDATA_FIELD;
						// 01/18/2010 Paul.  Apply ACL Field Security. 
						btnField.Visible  = bLayoutMode || bIsReadable;
						if ( Page_Command != null )
						{
							btnField.Command    += Page_Command;
							btnField.CommandName = sDATA_FORMAT  ;
						}
						try
						{
							if ( bLayoutMode )
							{
								btnField.Text    = sDATA_FIELD;
								btnField.Enabled = false      ;
							}
							else if ( sDATA_FIELD.IndexOf(".") >= 0 )
							{
								btnField.Text = L10n.Term(sDATA_FIELD);
							}
							else if ( !Sql.IsEmptyString(sDATA_FIELD) && rdr != null )
							{
								btnField.Text = Sql.ToString(rdr[sDATA_FIELD]);
							}
							btnField.Attributes.Add("title", btnField.Text);
						}
						catch(Exception ex)
						{
							SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
							btnField.Text = ex.Message;
						}
					}
				}
				else if ( String.Compare(sFIELD_TYPE, "Textbox", true) == 0 )
				{
					/*
					if ( !Sql.IsEmptyString(sDATA_FIELD) )
					{
						TextBox txtField = new TextBox();
						tdField.Controls.Add(txtField);
						txtField.ReadOnly = true;
						txtField.TextMode = TextBoxMode.MultiLine;
						// 03/16/2006 Paul.  Give the textbox a name so that it can be validated with SplendidTest. 
						txtField.ID       = sDATA_FIELD;
						try
						{
							string[] arrDATA_FORMAT = sDATA_FORMAT.Split(',');
							if ( arrDATA_FORMAT.Length == 2 )
							{
								txtField.Rows    = Sql.ToInteger(arrDATA_FORMAT[0]);
								txtField.Columns = Sql.ToInteger(arrDATA_FORMAT[1]);
							}
							if ( bLayoutMode )
							{
								txtField.Text = sDATA_FIELD;
							}
							else if ( !Sql.IsEmptyString(sDATA_FIELD) && rdr != null )
							{
								txtField.Text = Sql.ToString(rdr[sDATA_FIELD]);
							}
						}
						catch(Exception ex)
						{
							SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
							txtField.Text = ex.Message;
						}
					}
					*/
					// 07/07/2007 Paul.  Instead of using a real textbox, just replace new lines with <br />. 
					// This will perserve a majority of the HTML formating if it exists. 
					if ( bLayoutMode )
					{
						Literal litField = new Literal();
						litField.Text = sDATA_FIELD;
						tdField.Controls.Add(litField);
					}
					else if ( !Sql.IsEmptyString(sDATA_FIELD) )
					{
						// 12/06/2005 Paul.  Wrap all string fields in a SPAN tag to simplify regression testing. 
						HtmlGenericControl spnField = new HtmlGenericControl("span");
						tdField.Controls.Add(spnField);
						spnField.ID = sDATA_FIELD;

						Literal litField = new Literal();
						spnField.Controls.Add(litField);
						// 01/18/2010 Paul.  Apply ACL Field Security. 
						litField.Visible = bLayoutMode || bIsReadable;
						try
						{
							if ( rdr != null )
							{
								// 06/30/2018 Paul.  Value may have been erased. If so, replace with Erased Value message. 
								if ( rdr[sDATA_FIELD] == DBNull.Value )
								{
									if ( arrERASED_FIELDS.Contains(sDATA_FIELD) )
									{
										litField.Text = Sql.DataPrivacyErasedPill(L10n);
									}
								}
								else
								{
									string sDATA = Sql.ToString(rdr[sDATA_FIELD]);
									// 07/07/2007 Paul.  Emails may not have the proper \r\n terminators, so perform a few extra steps to ensure clean data. 
									// 06/04/2010 Paul.  Try and prevent excess blank lines. 
									sDATA = EmailUtils.NormalizeDescription(sDATA);
									litField.Text = sDATA;
								}
							}
						}
						catch(Exception ex)
						{
							SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
							litField.Text = ex.Message;
						}
					}
				}
				else if ( String.Compare(sFIELD_TYPE, "HyperLink", true) == 0 || String.Compare(sFIELD_TYPE, "ModuleLink", true) == 0 )
				{
					if ( !Sql.IsEmptyString(sDATA_FIELD) && (!Sql.IsEmptyString(sURL_FIELD) || String.Compare(sFIELD_TYPE, "ModuleLink", true) == 0) )
					{
						lnkField = new HyperLink();
						tdField.Controls.Add(lnkField);
						lnkField.Target   = sURL_TARGET;
						lnkField.CssClass = "tabDetailViewDFLink";
						// 03/16/2006 Paul.  Give the hyperlink a name so that it can be validated with SplendidTest. 
						lnkField.ID       = sDATA_FIELD;
						// 01/18/2010 Paul.  Apply ACL Field Security. 
						lnkField.Visible = bLayoutMode || bIsReadable;
						try
						{
							// 09/13/2018 Paul.  The literal must always be created, otherwise the postback with rdr == null will prevent any events from firing due to mismatched layouts. 
							Literal litField = new Literal();
							tdField.Controls.Add(litField);
							if ( bLayoutMode )
							{
								lnkField.Text    = sDATA_FIELD;
								lnkField.Enabled = false      ;
							}
							else if ( rdr != null )
							{
								if ( rdr[sDATA_FIELD] != DBNull.Value )
								{
									// 01/09/2006 Paul.  Allow DATA_FORMAT to be optional.   If missing, write data directly. 
									if ( Sql.IsEmptyString(sDATA_FORMAT) )
									{
										// 02/16/2010 Paul.  Add MODULE_TYPE so that we can lookup custom field IDs. 
										// 02/16/2010 Paul.  Move ToGuid to the function so that it can be captured if invalid. 
										// 09/15/2014 Paul.  Prevent Cross-Site Scripting by HTML encoding the data. 
										if ( !Sql.IsEmptyString(sMODULE_TYPE) )
											lnkField.Text = HttpUtility.HtmlEncode(Crm.Modules.ItemName(Application, sMODULE_TYPE, rdr[sDATA_FIELD]));
										else
											lnkField.Text = HttpUtility.HtmlEncode(Sql.ToString(rdr[sDATA_FIELD]));
									}
									else
									{
										// 02/16/2010 Paul.  Add MODULE_TYPE so that we can lookup custom field IDs. 
										// 02/16/2010 Paul.  Move ToGuid to the function so that it can be captured if invalid. 
										// 09/15/2014 Paul.  Prevent Cross-Site Scripting by HTML encoding the data. 
										if ( !Sql.IsEmptyString(sMODULE_TYPE) )
											lnkField.Text = String.Format(sDATA_FORMAT, HttpUtility.HtmlEncode(Crm.Modules.ItemName(Application, sMODULE_TYPE, rdr[sDATA_FIELD])));
										else
											lnkField.Text = String.Format(sDATA_FORMAT, HttpUtility.HtmlEncode(Sql.ToString(rdr[sDATA_FIELD])));
									}
								}
								else
								{
									// 06/30/2018 Paul.  Value may have been erased. If so, replace with Erased Value message. 
									lnkField.Visible = false;
									if ( arrERASED_FIELDS.Contains(sDATA_FIELD) )
									{
										litField.Text = Sql.DataPrivacyErasedPill(L10n);
									}
								}
							}
						}
						catch(Exception ex)
						{
							SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
							lnkField.Text = ex.Message;
						}
						try
						{
							if ( bLayoutMode )
							{
								lnkField.NavigateUrl = sURL_FIELD;
							}
							else if ( rdr != null )
							{
								// 03/19/2013 Paul.  URL_FIELD should support multiple fields. 
								string[] arrURL_FIELD = sURL_FIELD.Split(' ');
								object[] objURL_FIELD = new object[arrURL_FIELD.Length];
								for ( int i=0 ; i < arrURL_FIELD.Length; i++ )
								{
									if ( !Sql.IsEmptyString(arrURL_FIELD[i]) )
									{
										if ( rdr != null && rdr[arrURL_FIELD[i]] != DBNull.Value )
										{
											if ( rdr[arrURL_FIELD[i]].GetType() == Type.GetType("System.DateTime") )
												objURL_FIELD[i] = HttpUtility.UrlEncode(Sql.ToString(T10n.FromServerTime(rdr[arrURL_FIELD[i]])));
											else
											{
												// 04/08/2013 Paul.  Web site URLs should not be UrlEncoded as it will not be clickable. 
												string sURL_VALUE = Sql.ToString(rdr[arrURL_FIELD[i]]);
												if ( sURL_VALUE.Contains("://") )
													objURL_FIELD[i] = sURL_VALUE;
												else
													objURL_FIELD[i] = HttpUtility.UrlEncode(sURL_VALUE);
											}
										}
										else
											objURL_FIELD[i] = String.Empty;
									}
								}
								// 09/04/2010 Paul.  sURL_FIELD will be empty when used with a custom field. 
								if ( !Sql.IsEmptyString(sURL_FIELD) )
								{
									// 01/09/2006 Paul.  Allow DATA_FORMAT to be optional.   If missing, write data directly. 
									// 06/08/2012 Paul.  Check sURL_FORMAT instead of DATA_FORMAT. 
									if ( Sql.IsEmptyString(sURL_FORMAT) )
										lnkField.NavigateUrl = Sql.ToString(objURL_FIELD[0]);
									else
									{
										// 01/24/2019 Paul.  ~/Teams is not valid. 
										if ( sURL_FORMAT.Contains("~/Teams") )
										{
											sURL_FORMAT = sURL_FORMAT.Replace("~/Teams", "~/Administration/Teams");
										}
										lnkField.NavigateUrl = String.Format(sURL_FORMAT, objURL_FIELD);
									}
								}
								// 02/16/2010 Paul.  Add MODULE_TYPE so that we can lookup custom field IDs. 
								else if ( !Sql.IsEmptyString(sMODULE_TYPE) )
								{
									// 09/04/2010 Paul.  This should be a URL_FORMAT test. 
									if ( Sql.IsEmptyString(sURL_FORMAT) )
									{
										// 02/18/2010 Paul.  Get the Module Relative Path so that Project and Project Task will be properly handled. 
										// In these cases, the type is singular and the path is plural. 
										string sRELATIVE_PATH = Sql.ToString(Application["Modules." + sMODULE_TYPE + ".RelativePath"]);
										if ( Sql.IsEmptyString(sRELATIVE_PATH) )
											sRELATIVE_PATH = "~/" + sMODULE_TYPE + "/";
										// 08/30/2015 Paul.  sURL_FIELD will be empty for ModuleLink.  In that case we use sDATA_FIELD. 
										if ( String.Compare(sFIELD_TYPE, "ModuleLink", true) == 0 && Sql.IsEmptyString(sURL_FIELD) )
											lnkField.NavigateUrl = sRELATIVE_PATH + "view.aspx?ID=" + Sql.ToString(rdr[sDATA_FIELD]);
										else
											lnkField.NavigateUrl = sRELATIVE_PATH + "view.aspx?ID=" + Sql.ToString(objURL_FIELD[0]);
									}
									else
									{
										// 01/24/2019 Paul.  ~/Teams is not valid. 
										if ( sURL_FORMAT.Contains("~/Teams") )
										{
											sURL_FORMAT = sURL_FORMAT.Replace("~/Teams", "~/Administration/Teams");
										}
										lnkField.NavigateUrl = String.Format(sURL_FORMAT, objURL_FIELD);
									}
								}
							}
						}
						catch(Exception ex)
						{
							SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
							// 06/30/2018 Paul.  The error needs to be placed in the text field in order for it to be displayed. 
							lnkField.Text += " " + ex.Message;
						}
					}
				}
				// 11/23/2010 Paul.  Provide a link to the file. 
				else if ( String.Compare(sFIELD_TYPE, "File", true) == 0 )
				{
					if ( !Sql.IsEmptyString(sDATA_FIELD) )
					{
						lnkField = new HyperLink();
						lnkField.ID = sDATA_FIELD;
						lnkField.Visible = bLayoutMode || bIsReadable;
						try
						{
							if ( bLayoutMode )
							{
								Literal litField = new Literal();
								litField.Text = sDATA_FIELD;
								tdField.Controls.Add(litField);
							}
							else if ( rdr != null )
							{
								if ( !Sql.IsEmptyString(rdr[sDATA_FIELD]) )
								{
									lnkField.NavigateUrl = "~/Images/Image.aspx?ID=" + Sql.ToString(rdr[sDATA_FIELD]);
									// 09/15/2014 Paul.  Prevent Cross-Site Scripting by HTML encoding the data. 
									lnkField.Text = HttpUtility.HtmlEncode(Crm.Modules.ItemName(Application, "Images", rdr[sDATA_FIELD]));
									tdField.Controls.Add(lnkField);
								}
							}
						}
						catch(Exception ex)
						{
							SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
							lnkField.Text = ex.Message;
						}
					}
				}
				else if ( String.Compare(sFIELD_TYPE, "Image", true) == 0 )
				{
					if ( !Sql.IsEmptyString(sDATA_FIELD) )
					{
						Image imgField = new Image();
						// 04/13/2006 Paul.  Give the image a name so that it can be validated with SplendidTest. 
						imgField.ID = sDATA_FIELD;
						// 01/18/2010 Paul.  Apply ACL Field Security. 
						imgField.Visible = bLayoutMode || bIsReadable;
						try
						{
							if ( bLayoutMode )
							{
								Literal litField = new Literal();
								litField.Text = sDATA_FIELD;
								tdField.Controls.Add(litField);
							}
							else if ( rdr != null )
							{
								if ( !Sql.IsEmptyString(rdr[sDATA_FIELD]) )
								{
									imgField.ImageUrl = "~/Images/Image.aspx?ID=" + Sql.ToString(rdr[sDATA_FIELD]);
									// 04/13/2006 Paul.  Only add the image if it exists. 
									tdField.Controls.Add(imgField);
								}
							}
						}
						catch(Exception ex)
						{
							SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
							// 07/03/2014 Paul.  Add label to display error. 
							Label lblError = new Label();
							tdField.Controls.Add(lblError);
							lblError.Text = ex.Message;
						}
					}
				}
				else if ( String.Compare(sFIELD_TYPE, "IFrame", true) == 0 )
				{
					Literal litField = new Literal();
					litField.Visible = bLayoutMode || bIsReadable;
					tdField.Controls.Add(litField);
					try
					{
						string sIFRAME_SRC    = String.Empty;
						// 08/02/2010 Paul.  The iFrame height is stored in the URL Target field. 
						string sIFRAME_HEIGHT = sURL_TARGET;
						if ( Sql.IsEmptyString(sIFRAME_HEIGHT) )
							sIFRAME_HEIGHT = "200";
						if ( !Sql.IsEmptyString(sURL_FIELD) )
						{
							if ( bLayoutMode )
							{
								litField.Text = sURL_FIELD;
							}
							else if ( rdr != null )
							{
								string[] arrURL_FIELD = sURL_FIELD.Split(' ');
								object[] objURL_FIELD = new object[arrURL_FIELD.Length];
								for ( int i=0 ; i < arrURL_FIELD.Length; i++ )
								{
									if ( !Sql.IsEmptyString(arrURL_FIELD[i]) )
									{
										if ( rdr != null && rdr[arrURL_FIELD[i]] != DBNull.Value)
										{
											if ( rdr[arrURL_FIELD[i]].GetType() == Type.GetType("System.DateTime") )
												objURL_FIELD[i] = HttpUtility.UrlEncode(Sql.ToString(T10n.FromServerTime(rdr[arrURL_FIELD[i]])));
											else
												objURL_FIELD[i] = HttpUtility.UrlEncode(Sql.ToString(rdr[arrURL_FIELD[i]]));
										}
										else
											objURL_FIELD[i] = String.Empty;
									}
								}
								sIFRAME_SRC = String.Format(sURL_FORMAT, objURL_FIELD);
							}
						}
						else if ( !Sql.IsEmptyString(sDATA_FIELD) )
						{
							if ( bLayoutMode )
							{
								litField.Text = sDATA_FIELD;
							}
							else if ( rdr != null )
							{
								sIFRAME_SRC = Sql.ToString(rdr[sDATA_FIELD]);
							}
						}
						if ( !Sql.IsEmptyString(sIFRAME_SRC) )
							litField.Text = "<iframe src=\"" + sIFRAME_SRC + "\" height=\"" + sIFRAME_HEIGHT + "\" width=\"100%\"/></iframe>";
					}
					catch(Exception ex)
					{
						SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
						litField.Text = ex.Message;
					}
				}
				// 08/02/2010 Paul.  Create a seprate JavaScript field type. 
				else if ( String.Compare(sFIELD_TYPE, "JavaScript", true) == 0 )
				{
					Literal litField = new Literal();
					litField.Visible = bLayoutMode || bIsReadable;
					tdField.Controls.Add(litField);
					try
					{
						if ( bLayoutMode )
						{
							litField.Text = sURL_FIELD;
						}
						else if ( !Sql.IsEmptyString(sURL_FIELD) && !Sql.IsEmptyString(sURL_FORMAT) )
						{
							string[] arrURL_FIELD = sURL_FIELD.Split(' ');
							object[] objURL_FIELD = new object[arrURL_FIELD.Length];
							for ( int i=0 ; i < arrURL_FIELD.Length; i++ )
							{
								if ( !Sql.IsEmptyString(arrURL_FIELD[i]) )
								{
									// 07/26/2007 Paul.  Make sure to escape the javascript string. 
									if ( rdr[arrURL_FIELD[i]] != DBNull.Value )
										objURL_FIELD[i] = Sql.EscapeJavaScript(Sql.ToString(rdr[arrURL_FIELD[i]]));
									else
										objURL_FIELD[i] = String.Empty;
								}
							}
							// 12/03/2009 Paul.  LinkedIn Company Profile requires a span tag to insert the link.
							litField.Text = "&nbsp;<span id=\"" + String.Format(sURL_TARGET, objURL_FIELD) + "\"></span>";
							litField.Text += "<script type=\"text/javascript\"> " + String.Format(sURL_FORMAT, objURL_FIELD) + "</script>";
						}
					}
					catch(Exception ex)
					{
						SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
						litField.Text = ex.Message;
					}
				}
				// 05/14/2016 Paul.  Add Tags module. 
				else if ( String.Compare(sFIELD_TYPE, "Tags", true) == 0 )
				{
					if ( bLayoutMode )
					{
						Literal litField = new Literal();
						litField.Text = sDATA_FIELD;
						tdField.Controls.Add(litField);
					}
					else if ( !Sql.IsEmptyString(sDATA_FIELD) )
					{
						HtmlGenericControl spnField = new HtmlGenericControl("span");
						tdField.Controls.Add(spnField);
						spnField.ID = sDATA_FIELD;

						Literal litField = new Literal();
						spnField.Controls.Add(litField);
						litField.Visible = bLayoutMode || bIsReadable;
						try
						{
							if ( rdr != null )
							{
								string sDATA = Sql.ToString(rdr[sDATA_FIELD]);
								if ( !Sql.IsEmptyString(sDATA) )
								{
									sDATA = "<span class='Tags'>" + sDATA.Replace(",", "</span> <span class='Tags'>") + "</span>";
									litField.Text = sDATA;
								}
							}
						}
						catch(Exception ex)
						{
							SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
							litField.Text = ex.Message;
						}
					}
				}
				else
				{
					Literal litField = new Literal();
					tdField.Controls.Add(litField);
					litField.Text = "Unknown field type " + sFIELD_TYPE;
					// 01/07/2006 Paul.  Don't report the error in layout mode. 
					if ( !bLayoutMode )
						SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), "Unknown field type " + sFIELD_TYPE);
				}
				// 12/02/2007 Paul.  Each view can now have its own number of data columns. 
				// This was needed so that search forms can have 4 data columns. The default is 2 columns. 
				if ( nCOLSPAN > 0 )
					nColIndex += nCOLSPAN;
				else if ( nCOLSPAN == 0 )
					nColIndex++;
				if ( nColIndex >= nDATA_COLUMNS )
					nColIndex = 0;
			}
			// 09/20/2012 Paul.  We need a SCRIPT field that is form specific. 
			if ( dvFields.Count > 0 && !bLayoutMode )
			{
				try
				{
					string sDETAIL_NAME = Sql.ToString(dvFields[0]["DETAIL_NAME"]);
					string sFORM_SCRIPT = Sql.ToString(dvFields[0]["SCRIPT"     ]);
					if ( !Sql.IsEmptyString(sFORM_SCRIPT) )
					{
						// 09/20/2012 Paul.  The base ID is not the ID of the parent, but the ID of the TemplateControl. 
						sFORM_SCRIPT = sFORM_SCRIPT.Replace("SPLENDID_DETAILVIEW_LAYOUT_ID", tbl.TemplateControl.ClientID);
						sFORM_SCRIPT = sFORM_SCRIPT.Trim();
						// 01/18/2018 Paul.  If wrapped, then treat FORM_SCRIPT as a function. 
						if ( sFORM_SCRIPT.StartsWith("(") && sFORM_SCRIPT.EndsWith(")") )
						{
							string sFormVar = tbl.TemplateControl.ClientID + "_FORM_SCRIPT";
							sFORM_SCRIPT = "var " + sFormVar + " = " + sFORM_SCRIPT + ";" + ControlChars.CrLf
							             + "if ( typeof(" + sFormVar + ") == 'function' )" + ControlChars.CrLf
							             + "{" + ControlChars.CrLf
							             + "	var fnFORM_SCRIPT = " + sFormVar + "();" + ControlChars.CrLf
							             + "	if ( fnFORM_SCRIPT !== undefined && typeof(fnFORM_SCRIPT.Initialize) == 'function' ) " + ControlChars.CrLf
							             + "	{" + ControlChars.CrLf
							//             + "		console.log('Executing form script Initialize function.');" + ControlChars.CrLf
							             + "		fnFORM_SCRIPT.Initialize();" + ControlChars.CrLf
							             + "	}" + ControlChars.CrLf
							             + "	else" + ControlChars.CrLf
							             + "	{" + ControlChars.CrLf
							//             + "		console.log('Executed form script as function.');" + ControlChars.CrLf
							             + "	}" + ControlChars.CrLf
							             + "}" + ControlChars.CrLf
							             + "else" + ControlChars.CrLf
							             + "{" + ControlChars.CrLf
							             + "	console.log('Form script not a function and will not be executed.');" + ControlChars.CrLf
							             + "}" + ControlChars.CrLf
							;
						}
						else
						{
							//sFORM_SCRIPT += ControlChars.CrLf + "console.log('Executing form script as raw script.');";
						}
						ScriptManager.RegisterStartupScript(tbl, typeof(System.String), sDETAIL_NAME.Replace(".", "_") + "_SCRIPT", sFORM_SCRIPT, true);
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
				}
			}
		}

		// 06/21/2009 Paul.  Automatically associate the TextBox with a Submit button. 
		public static void AppendEditViewFields(string sEDIT_NAME, HtmlTable tbl, DataRow rdr, L10N L10n, TimeZone T10n, string sSubmitClientID)
		{
			if ( tbl == null )
			{
				SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), "HtmlTable is not defined for " + sEDIT_NAME);
				return;
			}
			// 05/05/2016 Paul.  The User Primary Role is used with role-based views. 
			DataTable dtFields = SplendidCache.EditViewFields(sEDIT_NAME, Security.PRIMARY_ROLE_NAME);
			AppendEditViewFields(dtFields.DefaultView, tbl, rdr, L10n, T10n, null, false, sSubmitClientID);
		}

		public static void AppendEditViewFields(string sEDIT_NAME, HtmlTable tbl, DataRow rdr, L10N L10n, TimeZone T10n)
		{
			if ( tbl == null )
			{
				SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), "HtmlTable is not defined for " + sEDIT_NAME);
				return;
			}
			// 05/05/2016 Paul.  The User Primary Role is used with role-based views. 
			DataTable dtFields = SplendidCache.EditViewFields(sEDIT_NAME, Security.PRIMARY_ROLE_NAME);
			AppendEditViewFields(dtFields.DefaultView, tbl, rdr, L10n, T10n, null, false, String.Empty);
		}

		// 11/10/2010 Paul.  Apply Business Rules. 
		public static void ApplyEditViewRules(string sEDIT_NAME, SplendidControl parent, string sXOML_FIELD_NAME, DataRow row)
		{
			try
			{
				string sMODULE_NAME = sEDIT_NAME.Split('.')[0];
				// 05/10/2016 Paul.  The User Primary Role is used with role-based views. 
				DataTable dtFields = SplendidCache.EditViewRules(sEDIT_NAME, Security.PRIMARY_ROLE_NAME);
				if ( dtFields.Rows.Count > 0 )
				{
					string sXOML = Sql.ToString(dtFields.Rows[0][sXOML_FIELD_NAME]);
					if ( !Sql.IsEmptyString(sXOML) )
					{
						RuleSet rules = RulesUtil.Deserialize(sXOML);
						RuleValidation validation = new RuleValidation(typeof(SplendidControlThis), null);
						// 11/11/2010 Paul.  Validate so that we can get more information on a runtime error. 
						rules.Validate(validation);
						if ( validation.Errors.HasErrors )
						{
							throw(new Exception(RulesUtil.GetValidationErrors(validation)));
						}
						SplendidControlThis swThis = new SplendidControlThis(parent, sMODULE_NAME, row);
						RuleExecution exec = new RuleExecution(validation, swThis);
						rules.Execute(exec);
					}
				}
			}
			catch(Exception ex)
			{
				//SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				// 11/10/2010 Paul.  Throwing an exception will be the preferred method of displaying an error. 
				// We want to skip the filler message "The following error was encountered while executing method SplendidCRM.SplendidControlThis.Throw". 
				if ( ex.InnerException != null )
					throw(new Exception(ex.InnerException.Message));
				else
					throw(new Exception(ex.Message));
			}
		}

		public static void ApplyDetailViewRules(string sDETAIL_NAME, SplendidControl parent, string sXOML_FIELD_NAME, DataRow row)
		{
			try
			{
				string sMODULE_NAME = sDETAIL_NAME.Split('.')[0];
				// 05/10/2016 Paul.  The User Primary Role is used with role-based views. 
				DataTable dtFields = SplendidCache.DetailViewRules(sDETAIL_NAME, Security.PRIMARY_ROLE_NAME);
				if ( dtFields.Rows.Count > 0 )
				{
					string sXOML = Sql.ToString(dtFields.Rows[0][sXOML_FIELD_NAME]);
					if ( !Sql.IsEmptyString(sXOML) )
					{
						RuleSet rules = RulesUtil.Deserialize(sXOML);
						RuleValidation validation = new RuleValidation(typeof(SplendidControlThis), null);
						// 11/11/2010 Paul.  Validate so that we can get more information on a runtime error. 
						rules.Validate(validation);
						if ( validation.Errors.HasErrors )
						{
							throw(new Exception(RulesUtil.GetValidationErrors(validation)));
						}
						SplendidControlThis swThis = new SplendidControlThis(parent, sMODULE_NAME, row);
						RuleExecution exec = new RuleExecution(validation, swThis);
						rules.Execute(exec);
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
			}
		}
#endif

		// 05/25/2008 Paul.  We need a version of UpdateCustomFields that pulls data from a DataRow as this is how 
		// Quotes, Orders and Invoices manage their line items. 
		// 09/09/2009 Paul.  Change parameter name to be more logical.
		public void UpdateCustomFields(DataRow rowForm, IDbTransaction trn, Guid gID, string sTABLE_NAME, DataTable dtCustomFields)
		{
			if ( dtCustomFields.Rows.Count > 0 )
			{
				IDbConnection con = trn.Connection;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.Transaction = trn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "update " + sTABLE_NAME + "_CSTM" + ControlChars.CrLf;
					int nFieldIndex = 0;
					foreach(DataRow row in dtCustomFields.Rows)
					{
						// 01/11/2006 Paul.  Uppercase looks better. 
						string sNAME   = Sql.ToString(row["NAME"  ]).ToUpper();
						string sCsType = Sql.ToString(row["CsType"]);
						// 06/02/2016 Paul.  We need to be able to define the currency format. 
						// 10/11/2016 Paul.  vwFIELDS_META_DATA_Unvalidated does not return DATA_TYPE. 
						string sDATA_TYPE = String.Empty;
						if ( row.Table.Columns.Contains("DATA_TYPE") )
							sDATA_TYPE = Sql.ToString(row["DATA_TYPE"]);
						// 01/13/2007 Paul.  We need to truncate any long strings to prevent SQL error. 
						// String or binary data would be truncated. The statement has been terminated. 
						int    nMAX_SIZE = Sql.ToInteger(row["MAX_SIZE"]);
						if ( rowForm.Table.Columns.Contains(sNAME) )
						{
							if ( nFieldIndex == 0 )
								cmd.CommandText += "   set ";
							else
								cmd.CommandText += "     , ";
							// 01/10/2006 Paul.  We can't use a StringBuilder because the Sql.AddParameter function
							// needs to be able to replace the @ with the appropriate database specific token. 
							cmd.CommandText += sNAME + " = @" + sNAME + ControlChars.CrLf;
							
							switch ( sCsType )
							{
								case "Guid"    :  Sql.AddParameter(cmd, "@" + sNAME, Sql.ToGuid    (rowForm[sNAME]));  break;
								case "short"   :  Sql.AddParameter(cmd, "@" + sNAME, Sql.ToInteger (rowForm[sNAME]));  break;
								case "Int32"   :  Sql.AddParameter(cmd, "@" + sNAME, Sql.ToInteger (rowForm[sNAME]));  break;
								case "Int64"   :  Sql.AddParameter(cmd, "@" + sNAME, Sql.ToInteger (rowForm[sNAME]));  break;
								case "float"   :  Sql.AddParameter(cmd, "@" + sNAME, Sql.ToFloat   (rowForm[sNAME]));  break;
								// 06/02/2016 Paul.  We need to be able to define the currency format. 
								case "decimal" :
									if ( sDATA_TYPE == "money" )
									{
										Decimal d = Sql.ToDecimal(rowForm[sNAME]);
										if ( C10n != null )
											d = C10n.FromCurrency(d);
										Sql.AddParameter(cmd, "@" + sNAME, d);
									}
									else
									{
										Sql.AddParameter(cmd, "@" + sNAME, Sql.ToDecimal (rowForm[sNAME]));
									}
									break;
								case "bool"    :  Sql.AddParameter(cmd, "@" + sNAME, Sql.ToBoolean (rowForm[sNAME]));  break;
								case "DateTime":
									// 07/30/2020 Paul.  Date may be in json format. 
									Sql.AddParameter(cmd, "@" + sNAME, RestUtil.FromJsonDate(Sql.ToString(rowForm[sNAME])));
									break;
								default        :  Sql.AddParameter(cmd, "@" + sNAME, Sql.ToString  (rowForm[sNAME]), nMAX_SIZE);  break;
							}
							nFieldIndex++;
						}
					}
					if ( nFieldIndex > 0 )
					{
						cmd.CommandText += " where ID_C = @ID_C" + ControlChars.CrLf;
						Sql.AddParameter(cmd, "@ID_C", gID);
						cmd.ExecuteNonQuery();
					}
					// 02/08/2021 Paul.  We need to update the custom field table even if no data has changed so that an audit record gets created.  This is a very old bug. 
					else
					{
						cmd.CommandText += "   set ID_C = ID_C" + ControlChars.CrLf;
						cmd.CommandText += " where ID_C = @ID_C" + ControlChars.CrLf;
						Sql.AddParameter(cmd, "@ID_C", gID);
						cmd.ExecuteNonQuery();
					}
				}
			}
		}

		// 12/29/2007 Paul.  TEAM_ID is now in the stored procedure. 
		/*
		public static void UpdateTeam(SplendidControl ctlPARENT, IDbTransaction trn, Guid gID, string sMODULE)
		{
			DynamicControl ctlCustomField = new DynamicControl(ctlPARENT, "TEAM_ID");
			if ( ctlCustomField.Exists )
			{
				UpdateTeam(trn, gID, sMODULE, ctlCustomField.ID);
			}
		}

		// 11/30/2006 Paul.  We need to be able to update the team when importing data. 
		public static void UpdateTeam(IDbTransaction trn, Guid gID, string sMODULE, Guid gTEAM_ID)
		{
			// 11/22/2006 Paul.  Team management is optional. 
			if ( Crm.Config.enable_team_management() )
			{
				IDbConnection con = trn.Connection;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.Transaction = trn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText  = "update " + sMODULE.ToUpper() + ControlChars.CrLf;
					cmd.CommandText += "   set TEAM_ID = @TEAM_ID" + ControlChars.CrLf;
					cmd.CommandText += " where ID      = @ID     " + ControlChars.CrLf;
					Sql.AddParameter(cmd, "@TEAM_ID", gTEAM_ID);
					Sql.AddParameter(cmd, "@ID"     , gID);
					cmd.ExecuteNonQuery();
				}
			}
		}
		*/

	}
}

