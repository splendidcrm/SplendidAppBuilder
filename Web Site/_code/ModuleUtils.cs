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
using System.Web;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Net.Mail;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;

using Spring.Social.Office365;

namespace SplendidCRM
{
	public class ModuleUtils
	{
		// 10/30/2021 Paul.  Moved from LoginView. 
		public class Login
		{
			private IMemoryCache                     memoryCache         ;
			private SplendidCRM.DbProviderFactories  DbProviderFactories = new SplendidCRM.DbProviderFactories();
			private HttpApplicationState             Application         = new HttpApplicationState();
			private Security                         Security            ;
			private SqlProcs                         SqlProcs            ;
			private SplendidError                    SplendidError       ;
			private Crm.Config                       Config              = new SplendidCRM.Crm.Config();
			private GoogleApps                       GoogleApps          ;
			private Office365Sync                    Office365Sync       ;

			public Login(IMemoryCache memoryCache, Security Security, SplendidError SplendidError, SqlProcs SqlProcs, GoogleApps GoogleApps, Office365Sync Office365Sync)
			{
				this.memoryCache         = memoryCache        ;
				this.Security            = Security           ;
				this.SqlProcs            = SqlProcs           ;
				this.SplendidError       = SplendidError      ;
				this.GoogleApps          = GoogleApps         ;
				this.Office365Sync       = Office365Sync      ;
			}

			public string SendForgotPasswordNotice(string sUSER_NAME, string sEMAIL)
			{
				string sStatus = String.Empty;
				L10N L10n = new L10N("en-US");
				if ( Security.IsWindowsAuthentication() )
				{
					sStatus = L10n.Term("Users.LBL_WINDOWS_AUTHENTICATION_REQUIRED");
				}
				else if ( Sql.ToBoolean(Application["CONFIG.ADFS.SingleSignOn.Enabled"]) )
				{
					sStatus = L10n.Term("Users.LBL_ADFS_AUTHENTICATION_REQUIRED");
				}
				else if ( Sql.ToBoolean(Application["CONFIG.Azure.SingleSignOn.Enabled"]) )
				{
					sStatus = L10n.Term("Users.LBL_AZURE_AUTHENTICATION_REQUIRED");
				}
				else
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select *                            " + ControlChars.CrLf
						     + "  from vwUSERS                      " + ControlChars.CrLf
						     + " where lower(USER_NAME) = @USER_NAME" + ControlChars.CrLf
						     + "   and lower(EMAIL1   ) = @EMAIL1   " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@USER_NAME", sUSER_NAME.ToLower());
							Sql.AddParameter(cmd, "@EMAIL1"   , sEMAIL.ToLower());
							using ( IDataReader rdr = cmd.ExecuteReader() )
							{
								//string sApplicationPath = Sql.ToString(Application["rootURL"]);
								Guid gUSER_LOGIN_ID = Guid.Empty;
								if ( rdr.Read() )
								{
									MailMessage mail = new MailMessage();
									string sFromName     = Sql.ToString (Application["CONFIG.fromname"    ]);
									string sFromAddress  = Sql.ToString (Application["CONFIG.fromaddress" ]);
									if ( !Sql.IsEmptyString(sFromAddress) && !Sql.IsEmptyString(sFromName) )
										mail.From = new MailAddress(sFromAddress, sFromName);
									else
										mail.From = new MailAddress(sFromAddress);
									mail.To.Add(new MailAddress(sEMAIL));
									
									Guid gPASSWORD_ID = Guid.Empty;
									SqlProcs.spUSERS_PASSWORD_LINK_InsertOnly(ref gPASSWORD_ID, sUSER_NAME);
									
									string sSiteURL   = Config.SiteURL();
									string sResetURL  = sSiteURL + "Users/ChangePassword.aspx?ID=" + gPASSWORD_ID.ToString();
									string sSubject   = L10n.Term("Users.LBL_RESET_PASSWORD_SUBJECT");
									if ( Sql.IsEmptyString(sSubject) )
										sSubject = "Reset your password";
									string sBodyHtml  = L10n.Term("Users.LBL_RESET_PASSWORD_BODY");
									if ( Sql.IsEmptyString(sBodyHtml) )
									{
										sBodyHtml += "<p>A password reset was requested.</p>\n";
										sBodyHtml += "<p>Please click the following link to reset your password:</p>\n";
										sBodyHtml += "<p><a href=\"{0}\">{0}</a></p>\n";
									}
									if ( sBodyHtml.IndexOf("{0}") < 0 )
									{
										sBodyHtml += "<p><a href=\"{0}\">{0}</a></p>\n";
									}
									sBodyHtml = String.Format(sBodyHtml, sResetURL);
									mail.Subject      = sSubject ;
									mail.Body         = sBodyHtml;
									mail.IsBodyHtml   = true;
									mail.BodyEncoding = System.Text.Encoding.UTF8;
									
									// 01/17/2017 Paul.  New SplendidMailClient object to encapsulate SMTP, Exchange and Google mail. 
									SplendidMailClient client = SplendidMailClient.CreateMailClient(Application, memoryCache, Security, SplendidError, GoogleApps, Office365Sync);
									client.Send(mail);
									sStatus = L10n.Term("Users.LBL_RESET_PASSWORD_STATUS");
								}
								else
								{
									sStatus = L10n.Term("Users.ERR_INVALID_FORGOT_PASSWORD");
								}
							}
						}
					}
				}
				return sStatus;
			}
		}


		// 03/11/2016 Paul.  We are getting timeouts on Azure, so recompile in the background with a status update. 
		// 10/31/2021 Paul.  Moved from EditCustomFields/NewRecord. 
		public class EditCustomFields
		{
			private SplendidCRM.DbProviderFactories  DbProviderFactories = new SplendidCRM.DbProviderFactories();
			private HttpApplicationState Application        = new HttpApplicationState();
			private SplendidError        SplendidError      ;
			private readonly ILogger<EditCustomFields> _logger;

			public EditCustomFields(SplendidError SplendidError, ILogger<EditCustomFields> logger)
			{
				this.SplendidError = SplendidError;
				this._logger       = logger;
			}

#pragma warning disable CS1998
			public async ValueTask RecompileViews(CancellationToken token)
			{
				RecompileViews();
			}
#pragma warning restore CS1998

			private void RecompileViews()
			{
				_logger.LogInformation("EditCustomFields.RecompileViews Begin");
				try
				{
					Application["System.Recompile.StartDate"      ] = DateTime.Now;
					Application["System.Recompile.CurrentPass"    ] = 0;
					Application["System.Recompile.TotalPasses"    ] = 9;
					Application["System.Recompile.CurrentView"    ] = 0;
					Application["System.Recompile.TotalViews"     ] = 0;
					Application["System.Recompile.CurrentViewName"] = String.Empty;
					
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						try
						{
							int nPassMax = 9;
							// 03/11/2016 Paul.  We are always going to make 9 passes. 
							// if ( !Sql.IsEmptyString(Application["refreshallviews_maxpass"]) )
							// {
							// 	nPassMax = Sql.ToInteger(Application["refreshallviews_maxpass"]);
							// }
							// 03/11/2016 Paul.  We only want to manage separately for SQL Server. 
							if ( Sql.IsSQLServer(con) )
							{
								string sSQL;
								sSQL = "select TABLE_NAME              " + ControlChars.CrLf
								     + "  from INFORMATION_SCHEMA.VIEWS" + ControlChars.CrLf
								     + " where TABLE_SCHEMA = 'dbo'    " + ControlChars.CrLf
								     + "   and TABLE_NAME like 'vw%'   " + ControlChars.CrLf
								     + " order by len(TABLE_NAME)      " + ControlChars.CrLf;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.CommandText = sSQL;
									using ( DbDataAdapter da = dbf.CreateDataAdapter() )
									{
										((IDbDataAdapter)da).SelectCommand = cmd;
										using ( DataTable dt = new DataTable() )
										{
											da.Fill(dt);
											IDbCommand cmdRefreshView = con.CreateCommand();
											cmdRefreshView.CommandType = CommandType.StoredProcedure;
											cmdRefreshView.CommandText = "sp_refreshview";
											cmdRefreshView.CommandTimeout = 0;
											IDbDataParameter parViewName = cmdRefreshView.CreateParameter();
											parViewName.ParameterName = "@viewname";
											parViewName.DbType        = DbType.String;
											parViewName.Size          = 100;
											cmdRefreshView.Parameters.Add(parViewName);
											
											bool bRestart = true;
											while ( bRestart )
											{
												bRestart = false;
												Application["System.Recompile.TotalPasses"] = nPassMax;
												for ( int iPass = 0; iPass < nPassMax && !bRestart; iPass++ )
												{
													Application["System.Recompile.CurrentPass"] = iPass + 1;
													Application["System.Recompile.TotalViews" ] = dt.Rows.Count;
													for ( int iView = 0; iView < dt.Rows.Count && !bRestart; iView++ )
													{
														DataRow row = dt.Rows[iView];
														string sTABLE_NAME = Sql.ToString(row["TABLE_NAME"]);
														Application["System.Recompile.CurrentView"    ] = iView + 1  ;
														Application["System.Recompile.CurrentViewName"] = sTABLE_NAME;
														parViewName.Value = sTABLE_NAME;
														cmdRefreshView.ExecuteNonQuery();
														// 03/11/2016 Paul.  Allow recompile to be restarted. 
														bRestart = Sql.ToBoolean(Application["System.Recompile.Restart"]);
													}
												}
											}
										}
									}
								}
							}
							else
							{
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.CommandType = CommandType.StoredProcedure;
									cmd.CommandText = "spSqlRefreshAllViews";
									cmd.CommandTimeout = 0;
									cmd.ExecuteNonQuery();
								}
							}
						}
						catch(Exception ex)
						{
							SplendidError.SystemMessage("Error", new StackTrace(true).GetFrame(0), ex);
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemMessage("Error", new StackTrace(true).GetFrame(0), ex);
				}
				finally
				{
					Application.Remove("System.Recompile.StartDate"      );
					Application.Remove("System.Recompile.Restart"        );
					Application.Remove("System.Recompile.CurrentPass"    );
					Application.Remove("System.Recompile.TotalPasses"    );
					Application.Remove("System.Recompile.CurrentView"    );
					Application.Remove("System.Recompile.TotalViews"     );
					Application.Remove("System.Recompile.CurrentViewName");
				}
				_logger.LogInformation("EditCustomFields.RecompileViews End");
			}
		}


		// 10/31/2021 Paul.  Moved GetAuditData to ModuleUtils from Audit/PopupView. 
		public class Audit
		{
			private SplendidCRM.DbProviderFactories  DbProviderFactories = new SplendidCRM.DbProviderFactories();
			private HttpApplicationState Application        = new HttpApplicationState();
			private Security             Security           ;
			private L10N                 L10n               ;
			private SplendidCRM.TimeZone T10n               = new SplendidCRM.TimeZone();
			private SplendidError        SplendidError      ;
			private SplendidCache        SplendidCache      ;
			private Crm.Modules          Modules            ;
			
			public Audit(HttpSessionState Session, Security Security, SplendidError SplendidError, SplendidCache SplendidCache, Crm.Modules Modules)
			{
				this.Security            = Security           ;
				this.L10n                = new L10N(Sql.ToString(Session["USER_SETTINGS/CULTURE"]));
				this.SplendidError       = SplendidError      ;
				this.SplendidCache       = SplendidCache      ;
				this.Modules             = Modules            ;
			}

			// 02/05/2018 Paul.  Provide a way to convert ID to NAME for custom fields. 
			public DataTable BuildChangesTable(string sModule, DataTable dtAudit, DataTable dtLayoutFields)
			{
				DataTable dtChanges = new DataTable();
				DataColumn colFIELD_NAME   = new DataColumn("FIELD_NAME"  , typeof(System.String  ));
				DataColumn colBEFORE_VALUE = new DataColumn("BEFORE_VALUE", typeof(System.String  ));
				DataColumn colAFTER_VALUE  = new DataColumn("AFTER_VALUE" , typeof(System.String  ));
				DataColumn colCREATED_BY   = new DataColumn("CREATED_BY"  , typeof(System.String  ));
				DataColumn colDATE_CREATED = new DataColumn("DATE_CREATED", typeof(System.DateTime));
				dtChanges.Columns.Add(colFIELD_NAME  );
				dtChanges.Columns.Add(colBEFORE_VALUE);
				dtChanges.Columns.Add(colAFTER_VALUE );
				dtChanges.Columns.Add(colCREATED_BY  );
				dtChanges.Columns.Add(colDATE_CREATED);
				if ( dtAudit.Rows.Count > 0 )
				{
					StringDictionary dict = new StringDictionary();
					dict.Add("AUDIT_ACTION"      , String.Empty);
					dict.Add("AUDIT_DATE"        , String.Empty);
					dict.Add("AUDIT_COLUMNS"     , String.Empty);
					dict.Add("CSTM_AUDIT_COLUMNS", String.Empty);
					dict.Add("ID"                , String.Empty);
					dict.Add("ID_C"              , String.Empty);
					dict.Add("DELETED"           , String.Empty);
					dict.Add("CREATED_BY"        , String.Empty);
					dict.Add("DATE_ENTERED"      , String.Empty);
					dict.Add("MODIFIED_USER_ID"  , String.Empty);
					dict.Add("DATE_MODIFIED"     , String.Empty);
					// 09/17/2009 Paul.  No need to audit the UTC date. 
					dict.Add("DATE_MODIFIED_UTC" , String.Empty);

					DataView vwLayoutFields = new DataView(dtLayoutFields);
					DataRow rowLast = dtAudit.Rows[0];
					for ( int i = 1; i < dtAudit.Rows.Count; i++ )
					{
						DataRow row = dtAudit.Rows[i];
						foreach ( DataColumn col in row.Table.Columns )
						{
							if ( !dict.ContainsKey(col.ColumnName) )
							{
								if ( Sql.ToString(rowLast[col.ColumnName]) != Sql.ToString(row[col.ColumnName]) )
								{
									DataRow rowChange = dtChanges.NewRow();
									dtChanges.Rows.Add(rowChange);
									// 09/16/2009 Paul.  Localize the field name. 
									rowChange["FIELD_NAME"  ] = Utils.TableColumnName(L10n, sModule, col.ColumnName);
									rowChange["CREATED_BY"  ] = SplendidCache.AssignedUser(Sql.ToGuid(row["MODIFIED_USER_ID"]));
									// 06/15/2009 Van.  The change date was not being converted to the time zone of the current user. 
									rowChange["DATE_CREATED"] = T10n.FromServerTime(row["AUDIT_DATE"]);
									rowChange["BEFORE_VALUE"] = rowLast[col.ColumnName];
									rowChange["AFTER_VALUE" ] = row    [col.ColumnName];
									// 09/05/2016 Paul.  Convert the Guid to a display name. 
									if ( col.ColumnName.EndsWith("_ID") )
									{
										string sDATA_FIELD = col.ColumnName;
										string sTABLE_NAME = sDATA_FIELD.Substring(0, sDATA_FIELD.Length - 3);
										if ( sDATA_FIELD == "ASSIGNED_USER_ID" )
											sTABLE_NAME = "USERS";
										else if ( sTABLE_NAME.EndsWith("Y") )
											sTABLE_NAME = sDATA_FIELD.Substring(0, sDATA_FIELD.Length - 4) + "IES";
										else if ( sTABLE_NAME != "PROJECT" && sTABLE_NAME != "PROJECT_TASK" )
											sTABLE_NAME += "S";
										string sMODULE_NAME = Modules.ModuleName(sTABLE_NAME);
										if ( sTABLE_NAME == "TEAM_SETS" )
											sMODULE_NAME = "TeamSets";
										rowChange["BEFORE_VALUE"] = Modules.ItemName(sMODULE_NAME, Sql.ToGuid(rowLast[col.ColumnName]));
										rowChange["AFTER_VALUE" ] = Modules.ItemName(sMODULE_NAME, Sql.ToGuid(row    [col.ColumnName]));
									}
									// 02/05/2018 Paul.  Provide a way to convert ID to NAME for custom fields. 
									else if ( col.ColumnName.EndsWith("_ID_C") )
									{
										vwLayoutFields.RowFilter = "DATA_FIELD = '" + col.ColumnName + " '";
										if ( vwLayoutFields.Count > 0 )
										{
											string sMODULE_TYPE = Sql.ToString(vwLayoutFields[0]["MODULE_TYPE"]);
											if ( !Sql.IsEmptyString(sMODULE_TYPE) )
											{
												rowChange["BEFORE_VALUE"] = Modules.ItemName(sMODULE_TYPE, Sql.ToGuid(rowLast[col.ColumnName]));
												rowChange["AFTER_VALUE" ] = Modules.ItemName(sMODULE_TYPE, Sql.ToGuid(row    [col.ColumnName]));
											}
										}
									}
									// 09/15/2014 Paul.  Prevent Cross-Site Scripting by HTML encoding the data. 
									if ( rowChange["BEFORE_VALUE"] != DBNull.Value )
									{
										if ( rowChange["BEFORE_VALUE"].GetType() == typeof(System.String) )
											rowChange["BEFORE_VALUE"] = HttpUtility.HtmlEncode(Sql.ToString(rowChange["BEFORE_VALUE"]));
									}
									if ( rowChange["AFTER_VALUE"] != DBNull.Value )
									{
										if ( rowChange["AFTER_VALUE"].GetType() == typeof(System.String) )
											rowChange["AFTER_VALUE"] = HttpUtility.HtmlEncode(Sql.ToString(rowChange["AFTER_VALUE"]));
									}
								}
							}
						}
						rowLast = row;
					}
				}
				return dtChanges;
			}

			public void GetAuditData(string sModule, Guid gAUDIT_ID, ref Guid gID, ref bool bArchiveView, StringBuilder sbSQLCode)
			{
				string sTableName = Sql.ToString(Application["Modules." + sModule + ".TableName"]);
				// 05/04/2008 Paul.  Protect against SQL Injection. A table name will never have a space character.
				sTableName = sTableName.Replace(" ", "");
				if ( !Sql.IsEmptyGuid(gAUDIT_ID) && !Sql.IsEmptyString(sModule) && !Sql.IsEmptyString(sTableName) )
				{
					bool bAccessAllowed = false;
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL ;
						sSQL = "select ID                        " + ControlChars.CrLf
						     + "  from vw" + sTableName + "_AUDIT" + ControlChars.CrLf
						     + " where AUDIT_ID = @AUDIT_ID      " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@AUDIT_ID", gAUDIT_ID);
							sbSQLCode.AppendLine(Sql.ExpandParameters(cmd) + ";");

							using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
							{
								if ( rdr.Read() )
								{
									bAccessAllowed = true;
									try
									{
										// 12/30/2007 Paul.  The name field might not be called NAME.
										// For now, just ignore the issue. 
										gID   = Sql.ToGuid  (rdr["ID"  ]);
										bArchiveView = false;
									}
									catch
									{
									}
								}
							}
						}
						// 10/25/2018 Paul.  Data might be archived. 
						if ( !bAccessAllowed && SplendidCache.ArchiveViewExists("vw" + sTableName + "_AUDIT") )
						{
							sSQL = "select ID                                " + ControlChars.CrLf
							     + "  from vw" + sTableName + "_AUDIT_ARCHIVE" + ControlChars.CrLf
							     + " where AUDIT_ID = @AUDIT_ID              " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@AUDIT_ID", gAUDIT_ID);
								sbSQLCode.AppendLine(Sql.ExpandParameters(cmd) + ";");

								using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
								{
									if ( rdr.Read() )
									{
										bAccessAllowed = true;
										try
										{
											// 12/30/2007 Paul.  The name field might not be called NAME.
											// For now, just ignore the issue. 
											gID   = Sql.ToGuid  (rdr["ID"  ]);
											bArchiveView = true;
										}
										catch
										{
										}
									}
								}
							}
						}
					}
				}
			}

			public DataTable GetAuditData(string sModule, Guid gID, ref string sNAME, StringBuilder sbSQLCode)
			{
				DataTable dtChanges = null;
				string sTableName = Sql.ToString(Application["Modules." + sModule + ".TableName"]);
				// 05/04/2008 Paul.  Protect against SQL Injection. A table name will never have a space character.
				sTableName = sTableName.Replace(" ", "");
				if ( !Sql.IsEmptyGuid(gID) && !Sql.IsEmptyString(sModule) && !Sql.IsEmptyString(sTableName) )
				{
					// 12/30/2007 Paul.  The first query should be used just to determine if access is allowed. 
					bool bAccessAllowed = false;
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL ;
						sSQL = "select NAME           " + ControlChars.CrLf
						     + "  from vw" + sTableName + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Security.Filter(cmd, sModule, "view");
							Sql.AppendParameter(cmd, gID, "ID", false);
							sbSQLCode.AppendLine(Sql.ExpandParameters(cmd) + ";");

							using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
							{
								if ( rdr.Read() )
								{
									bAccessAllowed = true;
									try
									{
										// 12/30/2007 Paul.  The name field might not be called NAME.
										// For now, just ignore the issue. 
										sNAME = Sql.ToString(rdr["NAME"]);
									}
									catch
									{
									}
								}
							}
						}
						// 10/25/2018 Paul.  Data might be archived. 
						if ( !bAccessAllowed && SplendidCache.ArchiveViewExists("vw" + sTableName) )
						{
							sSQL = "select NAME                        " + ControlChars.CrLf
							     + "  from vw" + sTableName + "_ARCHIVE" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Security.Filter(cmd, sModule, "view");
								Sql.AppendParameter(cmd, gID, "ID", false);
								sbSQLCode.AppendLine(Sql.ExpandParameters(cmd) + ";");

								using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
								{
									if ( rdr.Read() )
									{
										bAccessAllowed = true;
										try
										{
											// 12/30/2007 Paul.  The name field might not be called NAME.
											// For now, just ignore the issue. 
											sNAME = Sql.ToString(rdr["NAME"]);
										}
										catch
										{
										}
									}
								}
							}
						}
						if ( bAccessAllowed )
						{
							StringBuilder sb = new StringBuilder();
							DataTable dtTableColumns  = new DataTable();
							DataTable dtCustomColumns = new DataTable();
							// 02/29/2008 Niall.  Some SQL Server 2005 installations require matching case for the parameters. 
							// Since we force the parameter to be uppercase, we must also make it uppercase in the command text. 
							sSQL = "select ColumnName              " + ControlChars.CrLf
							     + "  from vwSqlColumns            " + ControlChars.CrLf
							     + " where ObjectName = @OBJECTNAME" + ControlChars.CrLf
							     + " order by colid                " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								// 09/02/2008 Paul.  Standardize the case of metadata tables to uppercase.  PostgreSQL defaults to lowercase. 
								Sql.AddParameter(cmd, "@OBJECTNAME", Sql.MetadataName(cmd, sTableName));
								
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dtTableColumns);
								}
							}
							// 02/29/2008 Niall.  Some SQL Server 2005 installations require matching case for the parameters. 
							// Since we force the parameter to be uppercase, we must also make it uppercase in the command text. 
							sSQL = "select ColumnName              " + ControlChars.CrLf
							     + "  from vwSqlColumns            " + ControlChars.CrLf
							     + " where ObjectName = @OBJECTNAME" + ControlChars.CrLf
							     + " order by colid                " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								// 09/02/2008 Paul.  Standardize the case of metadata tables to uppercase.  PostgreSQL defaults to lowercase. 
								Sql.AddParameter(cmd, "@OBJECTNAME", Sql.MetadataName(cmd, sTableName + "_CSTM"));
								
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dtCustomColumns);
								}
							}
							string sAuditName       = sTableName + "_AUDIT";
							string sCustomAuditName = sTableName + "_CSTM_AUDIT";
							sb.AppendLine("select " + sAuditName       + ".AUDIT_ACTION  as AUDIT_ACTION      ");
							sb.AppendLine("     , " + sAuditName       + ".AUDIT_DATE    as AUDIT_DATE        ");
							sb.AppendLine("     , " + sAuditName       + ".AUDIT_COLUMNS as AUDIT_COLUMNS     ");
							sb.AppendLine("     , " + sCustomAuditName + ".AUDIT_COLUMNS as CSTM_AUDIT_COLUMNS");
							foreach ( DataRow row in dtTableColumns.Rows )
							{
								sb.AppendLine("     , " + sAuditName + "." + Sql.ToString(row["ColumnName"]));
							}
							foreach ( DataRow row in dtCustomColumns.Rows )
							{
								sb.AppendLine("     , " + sCustomAuditName + "." + Sql.ToString(row["ColumnName"]));
							}
							sb.AppendLine("  from            " + sAuditName);
							// 05/12/2017 Paul.  Don't join to custom audit table if custom table does not have fields. 
							if ( dtCustomColumns.Rows.Count > 0 )
							{
								sb.AppendLine("  left outer join " + sCustomAuditName);
								sb.AppendLine("               on " + sCustomAuditName + ".ID_C        = " + sAuditName + ".ID         ");
								sb.AppendLine("              and " + sCustomAuditName + ".AUDIT_TOKEN = " + sAuditName + ".AUDIT_TOKEN");
							}
							sb.AppendLine(" where " + sAuditName + ".ID = @ID");
							// 02/08/2020 Paul.  Audited tables are now archived, but the AUDIT_VERSION field is not restored, so first sort by date, then version. 
							sb.AppendLine(" order by " + sAuditName + ".AUDIT_DATE asc, " + sAuditName + ".AUDIT_VERSION asc");
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sb.ToString();
								Sql.AddParameter(cmd, "@ID", gID);
								sbSQLCode.Append(Sql.ExpandParameters(cmd));
								
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dt = new DataTable() )
									{
										da.Fill(dt);
										// 02/05/2018 Paul.  Provide a way to convert ID to NAME for custom fields. 
										DataTable dtLayoutFields = SplendidCache.EditViewFields(sModule + ".EditView", Security.PRIMARY_ROLE_NAME);
										dtChanges = BuildChangesTable(sModule, dt, dtLayoutFields);
										// 06/03/2009 Paul.  We will not import the SugarCRM history, but we should still display it. 
										if ( Sql.ToBoolean(Application["CONFIG.append_sugarcrm_history"]) )
										{
											try
											{
												cmd.Parameters.Clear();
												using ( DataTable dtSugarCRM = new DataTable() )
												{
													string sSugarAuditName = sAuditName.ToUpper() + "_SUGARCRM";
													sSQL = "select " + sSugarAuditName + ".DATE_CREATED       " + ControlChars.CrLf
													     + "     , USERS.USER_NAME      as CREATED_BY         " + ControlChars.CrLf
													     + "     , " + sSugarAuditName + ".FIELD_NAME         " + ControlChars.CrLf
													     + "     , " + sSugarAuditName + ".BEFORE_VALUE_STRING" + ControlChars.CrLf
													     + "     , " + sSugarAuditName + ".AFTER_VALUE_STRING " + ControlChars.CrLf
													     + "     , " + sSugarAuditName + ".BEFORE_VALUE_TEXT  " + ControlChars.CrLf
													     + "     , " + sSugarAuditName + ".AFTER_VALUE_TEXT   " + ControlChars.CrLf
													     + "  from      " + sSugarAuditName                     + ControlChars.CrLf
													     + " inner join USERS                                 " + ControlChars.CrLf
													     + "         on USERS.ID      = " + sSugarAuditName + ".CREATED_BY" + ControlChars.CrLf
													     + "        and USERS.DELETED = 0                     " + ControlChars.CrLf
													     + " where " + sSugarAuditName + ".PARENT_ID = @ID    " + ControlChars.CrLf
													     + " order by " + sSugarAuditName + ".DATE_CREATED    " + ControlChars.CrLf;
													cmd.CommandText = sSQL;
													Sql.AddParameter(cmd, "@ID", gID);
													sbSQLCode.Append(Sql.ExpandParameters(cmd));
													
													da.Fill(dtSugarCRM);
													foreach ( DataRow rowSugar in dtSugarCRM.Rows )
													{
														DataRow rowMerge = dtChanges.NewRow();
														rowMerge["DATE_CREATED"] = Sql.ToString(rowSugar["DATE_CREATED"       ]);
														rowMerge["CREATED_BY"  ] = Sql.ToString(rowSugar["CREATED_BY"         ]);
														rowMerge["FIELD_NAME"  ] = Sql.ToString(rowSugar["FIELD_NAME"         ]);
														rowMerge["BEFORE_VALUE"] = Sql.ToString(rowSugar["BEFORE_VALUE_STRING"]) + Sql.ToString(rowSugar["BEFORE_VALUE_TEXT"]);
														rowMerge["AFTER_VALUE" ] = Sql.ToString(rowSugar["AFTER_VALUE_STRING" ]) + Sql.ToString(rowSugar["AFTER_VALUE_TEXT" ]);
														dtChanges.Rows.Add(rowMerge);
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
							}
						}
					}
				}
				return dtChanges;
			}
		}

		// 10/31/2021 Paul.  Moved GetAuditData to ModuleUtils from Audit/PopupPersonalInfo. 
		public class AuditPersonalInfo
		{
			private SplendidCRM.DbProviderFactories  DbProviderFactories = new SplendidCRM.DbProviderFactories();
			private HttpApplicationState Application        = new HttpApplicationState();
			private Security             Security           ;
			private L10N                 L10n               ;
			
			public AuditPersonalInfo(HttpSessionState Session, Security Security)
			{
				this.Security            = Security           ;
				this.L10n                = new L10N(Sql.ToString(Session["USER_SETTINGS/CULTURE"]));
			}

			// 02/05/2018 Paul.  Provide a way to convert ID to NAME for custom fields. 
			private DataTable BuildChangesTable(string sModule, DataTable dtAudit, DataTable dtDATA_PRIVACY_FIELDS)
			{
				DataTable dtChanges = new DataTable();
				DataColumn colFIELD_NAME   = new DataColumn("FIELD_NAME"  , typeof(System.String  ));
				DataColumn colVALUE        = new DataColumn("VALUE"       , typeof(System.String  ));
				DataColumn colMODIFIED_BY  = new DataColumn("MODIFIED_BY" , typeof(System.String  ));
				DataColumn colLEAD_SOURCE  = new DataColumn("LEAD_SOURCE" , typeof(System.String  ));
				DataColumn colLAST_UPDATED = new DataColumn("LAST_UPDATED", typeof(System.DateTime));
				dtChanges.Columns.Add(colFIELD_NAME  );
				dtChanges.Columns.Add(colVALUE       );
				dtChanges.Columns.Add(colMODIFIED_BY );
				dtChanges.Columns.Add(colLEAD_SOURCE );
				dtChanges.Columns.Add(colLAST_UPDATED);
				if ( dtAudit.Rows.Count > 0 )
				{
					foreach ( DataRow rowPrivacyField in dtDATA_PRIVACY_FIELDS.Rows )
					{
						string sPRIVACY_FIELD = Sql.ToString(rowPrivacyField["FIELD_NAME"]);
						if ( dtAudit.Columns.Contains(sPRIVACY_FIELD) )
						{
							DataRow rowChange = dtChanges.NewRow();
							dtChanges.Rows.Add(rowChange);
							rowChange["FIELD_NAME"] = Utils.TableColumnName(L10n, sModule, sPRIVACY_FIELD);
							DataRow row = dtAudit.Rows[0];
							rowChange["VALUE"       ] = Sql.ToString(row[sPRIVACY_FIELD]);
							rowChange["LAST_UPDATED"] = row["AUDIT_DATE"];
							rowChange["MODIFIED_BY"  ] = Sql.ToString(row["MODIFIED_BY"]);
							if ( dtAudit.Columns.Contains("LEAD_SOURCE") )
								rowChange["LEAD_SOURCE"] = Sql.ToString(row["LEAD_SOURCE"]);
							for ( int i = 1; i < dtAudit.Rows.Count; i++ )
							{
								row = dtAudit.Rows[i];
								if ( Sql.ToString(row[sPRIVACY_FIELD]) != Sql.ToString(rowChange["VALUE"]) )
									break;
								rowChange["LAST_UPDATED"] = row["AUDIT_DATE"];
								rowChange["MODIFIED_BY"  ] = Sql.ToString(row["MODIFIED_BY"]);
								if ( dtAudit.Columns.Contains("LEAD_SOURCE") )
									rowChange["LEAD_SOURCE"] = Sql.ToString(row["LEAD_SOURCE"]);
							}
						}
					}
				}
				return dtChanges;
			}

			public DataTable GetAuditData(string sModule, Guid gID, ref string sNAME, StringBuilder sbSQLCode)
			{
				DataTable dtChanges = null;
				string sTableName = Sql.ToString(Application["Modules." + sModule + ".TableName"]);
				// 05/04/2008 Paul.  Protect against SQL Injection. A table name will never have a space character.
				sTableName = sTableName.Replace(" ", "");
				if ( !Sql.IsEmptyGuid(gID) && !Sql.IsEmptyString(sModule) && !Sql.IsEmptyString(sTableName) )
				{
					// 12/30/2007 Paul.  The first query should be used just to determine if access is allowed. 
					bool bAccessAllowed = false;
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL ;
						sSQL = "select *              " + ControlChars.CrLf
						     + "  from vw" + sTableName + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Security.Filter(cmd, sModule, "view");
							Sql.AppendParameter(cmd, gID, "ID", false);

							using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
							{
								if ( rdr.Read() )
								{
									bAccessAllowed = true;
									try
									{
										// 12/30/2007 Paul.  The name field might not be called NAME.
										// For now, just ignore the issue. 
										sNAME = Sql.ToString(rdr["NAME"]);
									}
									catch
									{
									}
								}
							}
						}
						if ( bAccessAllowed )
						{
							DataTable dtDATA_PRIVACY_FIELDS = new DataTable();
							sSQL = "select FIELD_NAME                " + ControlChars.CrLf
							     + "  from vwDATA_PRIVACY_FIELDS     " + ControlChars.CrLf
							     + " where MODULE_NAME = @MODULE_NAME" + ControlChars.CrLf
							     + " order by FIELD_NAME             " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@MODULE_NAME", sModule);
							
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dtDATA_PRIVACY_FIELDS);
								}
							}
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								// 02/08/2020 Paul.  Audited tables are now archived, but the AUDIT_VERSION field is not restored, so first sort by date, then version. 
								sSQL = "select *                    " + ControlChars.CrLf
								     + "  from vw" + sTableName + "_AUDIT" + ControlChars.CrLf
								     + " where ID = @ID             " + ControlChars.CrLf
								     + " order by AUDIT_DATE, AUDIT_VERSION desc" + ControlChars.CrLf;
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@ID", gID);
								sbSQLCode.Append(Sql.ExpandParameters(cmd));

								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dt = new DataTable() )
									{
										da.Fill(dt);
										dtChanges = BuildChangesTable(sModule, dt, dtDATA_PRIVACY_FIELDS);
									}
								}
							}
						}
					}
				}
				return dtChanges;
			}

		}
	}

}

