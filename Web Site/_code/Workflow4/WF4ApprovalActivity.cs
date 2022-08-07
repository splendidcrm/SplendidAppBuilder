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
using System.Text;
using System.Data;
using System.Data.Common;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Xml;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.ComponentModel;
using System.Diagnostics;

namespace SplendidCRM
{
	public class WF4ApprovalActivityService
	{
		private DbProviderFactories  DbProviderFactories = new DbProviderFactories();
		private HttpApplicationState Application = new HttpApplicationState();
		private HttpSessionState     Session            ;
		private Security             Security           ;
		private Sql                  Sql                ;
		private L10N                 L10n               ;
		private SqlProcs             SqlProcs           ;
		private SplendidError        SplendidError      ;
		private SplendidCRM.Crm.Modules          Modules          ;

		public WF4ApprovalActivityService(HttpSessionState Session, Security Security, SplendidError SplendidError, Sql Sql, SplendidCRM.Crm.Modules Modules)
		{
			this.Session             = Session            ;
			this.Security            = Security           ;
			this.L10n                = new L10N(Sql.ToString(Session["USER_LANG"]));
			this.L10n                = new L10N(Sql.ToString(Session["USER_LANG"]));
			this.Sql                 = new Sql(Session, Security);
			this.SqlProcs            = new SqlProcs(Security, Sql);
			this.SplendidError       = SplendidError      ;
			this.Modules             = Modules            ;
		}

		public void spPROCESSES_UpdateApproval(HttpApplicationState Application, Guid gID, Guid gUSER_ID, string sAPPROVAL_RESPONSE)
		{
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				con.Open();
				using ( IDbTransaction trn = Sql.BeginTransaction(con) )
				{
					try
					{
						SqlProcs.spPROCESSES_UpdateApproval(gID, gUSER_ID, sAPPROVAL_RESPONSE, trn);
						trn.Commit();
					}
					catch
					{
						trn.Rollback();
						throw;
					}
				}
			}
		}

		public void Approve(HttpApplicationState Application, L10N L10n, Guid gID, Guid gUSER_ID)
		{
			ValidateRequiredFields(Application, L10n, gID);
			spPROCESSES_UpdateApproval(Application, gID, gUSER_ID, "Approve");
		}

		public void Reject(HttpApplicationState Application, Guid gID, Guid gUSER_ID)
		{
			spPROCESSES_UpdateApproval(Application, gID, gUSER_ID, "Reject");
		}

		public void Route(HttpApplicationState Application, L10N L10n, Guid gID, Guid gUSER_ID)
		{
			ValidateRequiredFields(Application, L10n, gID);
			spPROCESSES_UpdateApproval(Application, gID, gUSER_ID, "Route");
		}

		public void Claim(HttpApplicationState Application, Guid gID, Guid gUSER_ID)
		{
			spPROCESSES_UpdateApproval(Application, gID, gUSER_ID, "Claim");
		}

		public void Cancel(HttpApplicationState Application, Guid gID, Guid gUSER_ID)
		{
			spPROCESSES_UpdateApproval(Application, gID, gUSER_ID, "Cancel");
		}

		public void ChangeProcessUser(HttpApplicationState Application, Guid gID, Guid gPROCESS_USER_ID, string sPROCESS_NOTES)
		{
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				con.Open();
				using ( IDbTransaction trn = Sql.BeginTransaction(con) )
				{
					try
					{
						SqlProcs.spPROCESSES_ChangeProcessUser(gID, gPROCESS_USER_ID, sPROCESS_NOTES, trn);
						trn.Commit();
					}
					catch
					{
						trn.Rollback();
						throw;
					}
				}
			}
		}

		public void ChangeAssignedUser(HttpApplicationState Application, Guid gID, Guid gASSIGNED_USER_ID, string sPROCESS_NOTES)
		{
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				con.Open();
				using ( IDbTransaction trn = Sql.BeginTransaction(con) )
				{
					try
					{
						SqlProcs.spPROCESSES_ChangeAssignedUser(gID, gASSIGNED_USER_ID, sPROCESS_NOTES, trn);
						trn.Commit();
					}
					catch
					{
						trn.Rollback();
						throw;
					}
				}
			}
		}

		public void Filter(HttpApplicationState Application, IDbCommand cmd, Guid gUSER_ID)
		{
			string sSQL = String.Empty;
			bool bEnableTeamHierarchy = Sql.ToBoolean(Application["CONFIG.enable_team_hierarchy"]);
			sSQL =  " where PROCESS_USER_ID = @PROCESS_USER_ID                 " + ControlChars.CrLf
			     +  "    or (     PROCESS_USER_ID is null                      " + ControlChars.CrLf
			     +  "         and USER_ASSIGNMENT_METHOD = N'Self-Service Team'" + ControlChars.CrLf;
			if ( !bEnableTeamHierarchy )
			{
				sSQL += "         and exists(select *                                           " + ControlChars.CrLf
				     +  "                      from vwTEAM_MEMBERSHIPS                          " + ControlChars.CrLf
				     +  "                     where MEMBERSHIP_TEAM_ID = DYNAMIC_PROCESS_TEAM_ID" + ControlChars.CrLf
				     +  "                       and MEMBERSHIP_USER_ID = @TEAM_USER_ID          " + ControlChars.CrLf
				     +  "                   )                                                   " + ControlChars.CrLf;
			}
			else if ( Sql.IsOracle(cmd) )
			{
				sSQL += "         and exists(select *                                                                   " + ControlChars.CrLf
				     +  "                      table(fnTEAM_HIERARCHY_USERS(DYNAMIC_PROCESS_TEAM_ID)) vwTEAM_MEMBERSHIPS" + ControlChars.CrLf
				     +  "                     where MEMBERSHIP_USER_ID = @TEAM_USER_ID                                  " + ControlChars.CrLf
				     +  "                   )                                                                           " + ControlChars.CrLf;
			}
			else
			{
				string fnPrefix = (Sql.IsSQLServer(cmd) ? "dbo." : String.Empty);
				sSQL += "         and exists(select *                                                                                       " + ControlChars.CrLf
				     +  "                      from " + fnPrefix + "fnTEAM_HIERARCHY_USERS(DYNAMIC_PROCESS_TEAM_ID) vwTEAM_MEMBERSHIPS" + ControlChars.CrLf
				     +  "                     where MEMBERSHIP_USER_ID = @TEAM_USER_ID                                                      " + ControlChars.CrLf
				     +  "                   )                                                                                               " + ControlChars.CrLf;
			}
			sSQL += "       )                                                                          " + ControlChars.CrLf
			     +  "    or (     PROCESS_USER_ID is null                                              " + ControlChars.CrLf
			     +  "         and USER_ASSIGNMENT_METHOD = N'Self-Service Role'                        " + ControlChars.CrLf
			     +  "         and exists(select *                                                      " + ControlChars.CrLf
			     +  "                      from vwUSERS_ACL_ROLES                                      " + ControlChars.CrLf
			     +  "                     inner join vwUSERS_ASSIGNED_TO                               " + ControlChars.CrLf
			     +  "                             on vwUSERS_ASSIGNED_TO.ID = vwUSERS_ACL_ROLES.USER_ID" + ControlChars.CrLf
			     +  "                     where vwUSERS_ACL_ROLES.ROLE_ID = DYNAMIC_PROCESS_ROLE_ID    " + ControlChars.CrLf
			     +  "                       and vwUSERS_ACL_ROLES.USER_ID = @ROLE_USER_ID              " + ControlChars.CrLf
			     +  "                   )                                                              " + ControlChars.CrLf
			     +  "       )                                                                          " + ControlChars.CrLf;
			cmd.CommandText += sSQL;
			Sql.AddParameter(cmd, "@PROCESS_USER_ID", Security.USER_ID);
			Sql.AddParameter(cmd, "@TEAM_USER_ID"   , Security.USER_ID);
			Sql.AddParameter(cmd, "@ROLE_USER_ID"   , Security.USER_ID);
		}

		public  bool GetProcessStatus(HttpApplicationState Application, L10N L10n, Guid gPENDING_PROCESS_ID, ref string sProcessStatus, ref bool bShowApprove, ref bool bShowReject, ref bool bShowRoute, ref bool bShowClaim, ref string sUSER_TASK_TYPE, ref Guid gPROCESS_USER_ID, ref Guid gASSIGNED_TEAM_ID, ref Guid gPROCESS_TEAM_ID)
		{
			bool bFound = false;
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				con.Open();
				string sSQL = String.Empty;
				sSQL = "select *                        " + ControlChars.CrLf
				     + "  from vwPROCESSES_PendingStatus" + ControlChars.CrLf
				     + " where ID = @ID                 " + ControlChars.CrLf;
				using ( DataTable dtProcess = new DataTable() )
				{
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@ID", gPENDING_PROCESS_ID);
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							da.Fill(dtProcess);
						}
					}
					if ( dtProcess.Rows.Count > 0 )
					{
						DataRow rdrProcess = dtProcess.Rows[0];
						int      nPROCESS_NUMBER               = Sql.ToInteger (rdrProcess["PROCESS_NUMBER"              ]);
						Guid     gBUSINESS_PROCESS_INSTANCE_ID = Sql.ToGuid    (rdrProcess["BUSINESS_PROCESS_INSTANCE_ID"]);
						string   sACTIVITY_INSTANCE            = Sql.ToString  (rdrProcess["ACTIVITY_INSTANCE"           ]);
						string   sACTIVITY_NAME                = Sql.ToString  (rdrProcess["ACTIVITY_NAME"               ]);
						Guid     gBUSINESS_PROCESS_ID          = Sql.ToGuid    (rdrProcess["BUSINESS_PROCESS_ID"         ]);
						string   sBUSINESS_PROCESS_NAME        = Sql.ToString  (rdrProcess["BUSINESS_PROCESS_NAME"       ]);
						// 08/06/2016 Paul.  We will return gPROCESS_USER_ID. 
						         gPROCESS_USER_ID              = Sql.ToGuid    (rdrProcess["PROCESS_USER_ID"             ]);
						string   sBOOKMARK_NAME                = Sql.ToString  (rdrProcess["BOOKMARK_NAME"               ]);
						string   sPARENT_TYPE                  = Sql.ToString  (rdrProcess["PARENT_TYPE"                 ]);
						Guid     gPARENT_ID                    = Sql.ToGuid    (rdrProcess["PARENT_ID"                   ]);
						// 08/06/2016 Paul.  We will return sUSER_TASK_TYPE. 
						         sUSER_TASK_TYPE               = Sql.ToString  (rdrProcess["USER_TASK_TYPE"              ]);
						bool     bCHANGE_ASSIGNED_USER         = Sql.ToBoolean (rdrProcess["CHANGE_ASSIGNED_USER"        ]);
						Guid     gCHANGE_ASSIGNED_TEAM_ID      = Sql.ToGuid    (rdrProcess["CHANGE_ASSIGNED_TEAM_ID"     ]);
						bool     bCHANGE_PROCESS_USER          = Sql.ToBoolean (rdrProcess["CHANGE_PROCESS_USER"         ]);
						Guid     gCHANGE_PROCESS_TEAM_ID       = Sql.ToGuid    (rdrProcess["CHANGE_PROCESS_TEAM_ID"      ]);
						string   sUSER_ASSIGNMENT_METHOD       = Sql.ToString  (rdrProcess["USER_ASSIGNMENT_METHOD"      ]);
						Guid     gSTATIC_ASSIGNED_USER_ID      = Sql.ToGuid    (rdrProcess["STATIC_ASSIGNED_USER_ID"     ]);
						Guid     gDYNAMIC_PROCESS_TEAM_ID      = Sql.ToGuid    (rdrProcess["DYNAMIC_PROCESS_TEAM_ID"     ]);
						Guid     gDYNAMIC_PROCESS_ROLE_ID      = Sql.ToGuid    (rdrProcess["DYNAMIC_PROCESS_ROLE_ID"     ]);
						string   sREAD_ONLY_FIELDS             = Sql.ToString  (rdrProcess["READ_ONLY_FIELDS"            ]);
						string   sREQUIRED_FIELDS              = Sql.ToString  (rdrProcess["REQUIRED_FIELDS"             ]);
						string   sDURATION_UNITS               = Sql.ToString  (rdrProcess["DURATION_UNITS"              ]);
						int      nDURATION_VALUE               = Sql.ToInteger (rdrProcess["DURATION_VALUE"              ]);
						string   sSTATUS                       = Sql.ToString  (rdrProcess["STATUS"                      ]);
						Guid     gAPPROVAL_USER_ID             = Sql.ToGuid    (rdrProcess["APPROVAL_USER_ID"            ]);
						string   sAPPROVAL_DATE                = Sql.ToString  (rdrProcess["APPROVAL_DATE"               ]);
						string   sAPPROVAL_RESPONSE            = Sql.ToString  (rdrProcess["APPROVAL_RESPONSE"           ]);
						DateTime dtDATE_ENTERED                = Sql.ToDateTime(rdrProcess["DATE_ENTERED"                ]);
						DateTime dtDATE_MODIFIED               = Sql.ToDateTime(rdrProcess["DATE_MODIFIED"               ]);
						
						bFound = true;
						sProcessStatus = sBUSINESS_PROCESS_NAME;
						if ( !Sql.IsEmptyString(sACTIVITY_NAME) )
						{
							sProcessStatus += " | " + sACTIVITY_NAME;
						}
						if ( !Sql.IsEmptyString(sDURATION_UNITS) && nDURATION_VALUE > 0 )
						{
							DateTime dtDUE_DATE = dtDATE_ENTERED;
							switch ( sDURATION_UNITS )
							{
								case "hour"   :  dtDUE_DATE = dtDUE_DATE.AddHours (    nDURATION_VALUE);  break;
								case "day"    :  dtDUE_DATE = dtDUE_DATE.AddDays  (    nDURATION_VALUE);  break;
								case "week"   :  dtDUE_DATE = dtDUE_DATE.AddDays  (7 * nDURATION_VALUE);  break;
								case "month"  :  dtDUE_DATE = dtDUE_DATE.AddMonths(    nDURATION_VALUE);  break;
								case "quarter":  dtDUE_DATE = dtDUE_DATE.AddMonths(3 * nDURATION_VALUE);  break;
								case "year"   :  dtDUE_DATE = dtDUE_DATE.AddYears (    nDURATION_VALUE);  break;
							}
							string sDUE_DATE = dtDUE_DATE.ToString();
							if ( dtDUE_DATE > DateTime.Now )
							{
								sProcessStatus += " [ " + String.Format(L10n.Term("Processes.LBL_DUE_DATE_FORMAT"), sDUE_DATE) + " ]";
							}
							else
							{
								sProcessStatus += " [ <span class=ProcessOverdue>" + String.Format(L10n.Term("Processes.LBL_OVERDUE_FORMAT"), sDUE_DATE) + "</span> ]";
							}
						}
						if ( bCHANGE_ASSIGNED_USER )
							gASSIGNED_TEAM_ID = gCHANGE_ASSIGNED_TEAM_ID;
						if ( bCHANGE_PROCESS_USER )
							gPROCESS_TEAM_ID = gCHANGE_PROCESS_TEAM_ID;
						// 08/02/2016 Paul.  Self-service only applies if Process User not set. 
						if ( !Sql.IsEmptyGuid(gPROCESS_USER_ID) )
						{
							if ( sUSER_TASK_TYPE == "Approve/Reject" )
							{
								bShowApprove = true;
								bShowReject  = true;
							}
							else // if ( sUSER_TASK_TYPE == "Route" )
							{
								bShowRoute = true;
							}
						}
						else if ( sUSER_ASSIGNMENT_METHOD == "Self-Service Team" )
						{
							bool bEnableTeamHierarchy = Sql.ToBoolean(Application["CONFIG.enable_team_hierarchy"]);
							if ( !bEnableTeamHierarchy )
							{
								sSQL = "select count(*)                     " + ControlChars.CrLf
								     + "  from vwTEAM_MEMBERSHIPS           " + ControlChars.CrLf
								     + " where MEMBERSHIP_TEAM_ID = @TEAM_ID" + ControlChars.CrLf
								     + "   and MEMBERSHIP_USER_ID = @USER_ID" + ControlChars.CrLf;
							}
							else
							{
								if ( Sql.IsOracle(con) )
								{
									sSQL = "select count(*)                     " + ControlChars.CrLf
									     + "  from table(fnTEAM_HIERARCHY_USERS(@TEAM_ID)) vwTEAM_MEMBERSHIPS" + ControlChars.CrLf
									     + " where MEMBERSHIP_USER_ID = @USER_ID" + ControlChars.CrLf;
								}
								else
								{
									string fnPrefix = (Sql.IsSQLServer(con) ? "dbo." : String.Empty);
									sSQL = "select count(*)                     " + ControlChars.CrLf
									     + "  from " + fnPrefix + "fnTEAM_HIERARCHY_USERS(@TEAM_ID) vwTEAM_MEMBERSHIPS" + ControlChars.CrLf
									     + " where MEMBERSHIP_USER_ID = @USER_ID" + ControlChars.CrLf;
								}
							}
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@TEAM_ID", gDYNAMIC_PROCESS_TEAM_ID);
								Sql.AddParameter(cmd, "@USER_ID", Security.USER_ID        );
								int nUserIncluded = Sql.ToInteger(cmd.ExecuteScalar());
								if ( nUserIncluded > 0 )
								{
									bShowClaim = true;
								}
							}
						}
						else if ( sUSER_ASSIGNMENT_METHOD == "Self-Service Role" )
						{
							sSQL = "select count(*)                                               " + ControlChars.CrLf
							     + "  from vwUSERS_ACL_ROLES                                      " + ControlChars.CrLf
							     + " inner join vwUSERS_ASSIGNED_TO                               " + ControlChars.CrLf
							     + "         on vwUSERS_ASSIGNED_TO.ID = vwUSERS_ACL_ROLES.USER_ID" + ControlChars.CrLf
							     + " where vwUSERS_ACL_ROLES.ROLE_ID = @ROLE_ID                   " + ControlChars.CrLf
							     + "   and vwUSERS_ACL_ROLES.USER_ID = @USER_ID                   " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@ROLE_ID", gDYNAMIC_PROCESS_ROLE_ID);
								Sql.AddParameter(cmd, "@USER_ID", Security.USER_ID        );
								int nUserIncluded = Sql.ToInteger(cmd.ExecuteScalar());
								if ( nUserIncluded > 0 )
								{
									bShowClaim = true;
								}
							}
						}
					}
				}
			}
			return bFound;
		}

		public void ValidateRequiredFields(HttpApplicationState Application, L10N L10n, Guid gPENDING_PROCESS_ID)
		{
			if ( !Sql.IsEmptyGuid(gPENDING_PROCESS_ID) )
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					string sSQL = String.Empty;
					sSQL = "select *                        " + ControlChars.CrLf
					     + "  from vwPROCESSES_PendingStatus" + ControlChars.CrLf
					     + " where ID = @ID                 " + ControlChars.CrLf;
					using ( DataTable dtProcess = new DataTable() )
					{
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@ID", gPENDING_PROCESS_ID);
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								da.Fill(dtProcess);
							}
						}
						if ( dtProcess.Rows.Count > 0 )
						{
							DataRow rdrProcess = dtProcess.Rows[0];
							int      nPROCESS_NUMBER               = Sql.ToInteger (rdrProcess["PROCESS_NUMBER"              ]);
							Guid     gBUSINESS_PROCESS_INSTANCE_ID = Sql.ToGuid    (rdrProcess["BUSINESS_PROCESS_INSTANCE_ID"]);
							string   sACTIVITY_INSTANCE            = Sql.ToString  (rdrProcess["ACTIVITY_INSTANCE"           ]);
							string   sACTIVITY_NAME                = Sql.ToString  (rdrProcess["ACTIVITY_NAME"               ]);
							Guid     gBUSINESS_PROCESS_ID          = Sql.ToGuid    (rdrProcess["BUSINESS_PROCESS_ID"         ]);
							string   sBUSINESS_PROCESS_NAME        = Sql.ToString  (rdrProcess["BUSINESS_PROCESS_NAME"       ]);
							Guid     gPROCESS_USER_ID              = Sql.ToGuid    (rdrProcess["PROCESS_USER_ID"             ]);
							string   sBOOKMARK_NAME                = Sql.ToString  (rdrProcess["BOOKMARK_NAME"               ]);
							string   sPARENT_TYPE                  = Sql.ToString  (rdrProcess["PARENT_TYPE"                 ]);
							Guid     gPARENT_ID                    = Sql.ToGuid    (rdrProcess["PARENT_ID"                   ]);
							string   sUSER_TASK_TYPE               = Sql.ToString  (rdrProcess["USER_TASK_TYPE"              ]);
							string   sREAD_ONLY_FIELDS             = Sql.ToString  (rdrProcess["READ_ONLY_FIELDS"            ]);
							string   sREQUIRED_FIELDS              = Sql.ToString  (rdrProcess["REQUIRED_FIELDS"             ]);
							string   sSTATUS                       = Sql.ToString  (rdrProcess["STATUS"                      ]);
							
							sREQUIRED_FIELDS = sREQUIRED_FIELDS.Replace(" ", "");
							if ( !Sql.IsEmptyString(sREQUIRED_FIELDS) && (sSTATUS == "In Progress" || sSTATUS == "Unclaimed") )
							{
								using ( DataTable dtCurrent = new DataTable() )
								{
									string sTABLE_NAME = Modules.TableName(sPARENT_TYPE);
									sSQL = "select *                         " + ControlChars.CrLf
									     + "  from vw" + sTABLE_NAME + "_Edit" + ControlChars.CrLf
									     + " where ID = @ID                  " + ControlChars.CrLf;
									using ( IDbCommand cmd = con.CreateCommand() )
									{
										cmd.CommandText = sSQL;
										Sql.AddParameter(cmd, "@ID", gPARENT_ID);
										using ( DbDataAdapter da = dbf.CreateDataAdapter() )
										{
											((IDbDataAdapter)da).SelectCommand = cmd;
											da.Fill(dtCurrent);
										}
									}
									if ( dtCurrent.Rows.Count > 0 )
									{
										StringBuilder sbErrors = new StringBuilder();
										DataRow row = dtCurrent.Rows[0];
										foreach ( string sFIELD_NAME in sREQUIRED_FIELDS.Split(',') )
										{
											if ( dtCurrent.Columns.Contains(sFIELD_NAME) )
											{
												DataColumn col = dtCurrent.Columns[sFIELD_NAME];
												if ( row[sFIELD_NAME] != DBNull.Value )
												{
													switch ( col.DataType.FullName )
													{
														case "System.Single"  :  if ( Sql.ToDouble  (row[sFIELD_NAME]) == 0.0d             ) sbErrors.Append(sFIELD_NAME);  break;
														case "System.Double"  :  if ( Sql.ToDouble  (row[sFIELD_NAME]) == 0.0d             ) sbErrors.Append(sFIELD_NAME);  break;
														case "System.Int16"   :  if ( Sql.ToInteger (row[sFIELD_NAME]) == 0                ) sbErrors.Append(sFIELD_NAME);  break;
														case "System.Int32"   :  if ( Sql.ToInteger (row[sFIELD_NAME]) == 0                ) sbErrors.Append(sFIELD_NAME);  break;
														case "System.Int64"   :  if ( Sql.ToInteger (row[sFIELD_NAME]) == 0                ) sbErrors.Append(sFIELD_NAME);  break;
														case "System.Decimal" :  if ( Sql.ToDecimal (row[sFIELD_NAME]) == Decimal.Zero     ) sbErrors.Append(sFIELD_NAME);  break;
														case "System.DateTime":  if ( Sql.ToDateTime(row[sFIELD_NAME]) == DateTime.MinValue) sbErrors.Append(sFIELD_NAME);  break;
														case "System.Guid"    :  if ( Sql.ToGuid    (row[sFIELD_NAME]) == Guid.Empty       ) sbErrors.Append(sFIELD_NAME);  break;
														case "System.String"  :  if ( Sql.ToString  (row[sFIELD_NAME]) == String.Empty     ) sbErrors.Append(sFIELD_NAME);  break;
													}
												}
												else
												{
													sbErrors.Append(sFIELD_NAME);
												}
											}
											else
											{
												// We are going to ignore the warning that the required field does not exist. 
											}
										}
										if ( sbErrors.Length > 0 )
											throw(new Exception(L10n.Term(".ERR_MISSING_REQUIRED_FIELDS") + " " + sbErrors.ToString()));
									}
								}
							}
						}
					}
				}
			}
		}
	}

}
