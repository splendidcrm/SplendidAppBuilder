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
using System.Web;
using System.Data;
using System.Data.Common;
using Microsoft.Win32;

namespace SplendidCRM
{
	/// <summary>
	/// Summary description for DbProviderFactories.
	/// </summary>
	public class DbProviderFactories
	{
		private HttpApplicationState Application = new HttpApplicationState();
		private AppSettings          AppSettings = new AppSettings();

		public DbProviderFactories()
		{
		}

		// 12/22/2007 Paul.  Inside the timer event, there is no current context, so we need to pass the application. 
		public DbProviderFactory GetFactory()
		{
			// 11/14/2005 Paul.  Cache the connection string in the application as config and registry access is expected to be slower. 
			string sSplendidProvider = Sql.ToString(Application["SplendidProvider"]);
			string sConnectionString = Sql.ToString(Application["ConnectionString"]);
#if DEBUG
//			sSplendidProvider = String.Empty;
#endif
			if ( Sql.IsEmptyString(sSplendidProvider) || Sql.IsEmptyString(sConnectionString) )
			{
				sSplendidProvider = AppSettings["SplendidProvider"];
				switch ( sSplendidProvider )
				{
					case "System.Data.SqlClient":
						sConnectionString = AppSettings["SplendidSQLServer"];
						break;
					case "System.Data.OracleClient":
						sConnectionString = AppSettings["SplendidSystemOracle"];
						break;
					case "Oracle.DataAccess.Client":
						sConnectionString = AppSettings["SplendidOracle"];
						break;
					case "MySql.Data":
						sConnectionString = AppSettings["SplendidMySql"];
						break;
					case "IBM.Data.DB2":
						sConnectionString = AppSettings["SplendidDB2"];
						break;
					case "Sybase.Data.AseClient":
						sConnectionString = AppSettings["SplendidSybase"];
						break;
					case "iAnywhere.Data.AsaClient":
						sConnectionString = AppSettings["SplendidSQLAnywhere"];
						break;
					case "Npgsql":
						sConnectionString = AppSettings["SplendidNpgsql"];
						break;
					case "Registry":
					{
						string sSplendidRegistry = AppSettings["SplendidRegistry"];
						if ( Sql.IsEmptyString(sSplendidRegistry) )
						{
							// 11/14/2005 Paul.  If registry key is not provided, then compute it using the server and the application path. 
							// This will allow a single installation to support multiple databases. 
							// 12/22/2007 Paul.  We can no longer rely upon the Request object being valid as we might be inside the timer event. 
							string sServerName      = Sql.ToString(Application["ServerName"     ]);
							string sApplicationPath = Sql.ToString(Application["ApplicationPath"]);
							// 09/24/2010 Paul.  Remove trailing . so that it will be easier to debug http://localhost./SplendidCRM using Fiddler2. 
							if ( sServerName.EndsWith(".") )
								sServerName = sServerName.Substring(0, sServerName.Length - 1);
							sSplendidRegistry  = "SOFTWARE\\SplendidCRM Software\\" ;
							sSplendidRegistry += sServerName;
							if ( sApplicationPath != "/" )
								sSplendidRegistry += sApplicationPath.Replace("/", "\\");
						}
#pragma warning disable CA1416
						using (RegistryKey keySplendidCRM = Registry.LocalMachine.OpenSubKey(sSplendidRegistry))
						{
							if ( keySplendidCRM != null )
							{
								sSplendidProvider = Sql.ToString(keySplendidCRM.GetValue("SplendidProvider"));
								sConnectionString = Sql.ToString(keySplendidCRM.GetValue("ConnectionString"));
								// 01/17/2008 Paul.  99.999% percent of the time, we will be hosting on SQL Server. 
								// If the provider is not specified, then just assume SQL Server. 
								if ( Sql.IsEmptyString(sSplendidProvider) )
									sSplendidProvider = "System.Data.SqlClient";
							}
							else
							{
								throw(new Exception("Database connection information was not found in the registry " + sSplendidRegistry));
							}
						}
#pragma warning restore CA1416
							break;
					}
// 12/16/2021 TODO.  HostingDatabase
#if false
					case "HostingDatabase":
					{
						// 09/27/2006 Paul.  Allow a Hosting Database to contain connection strings. 
						/*
						<appSettings>
							<add key="SplendidProvider"          value="HostingDatabase" />
							<add key="SplendidHostingProvider"   value="System.Data.SqlClient" />
							<add key="SplendidHostingConnection" value="data source=(local)\SplendidCRM;initial catalog=SplendidCRM;user id=sa;password=" />
						</appSettings>
						*/
						string sSplendidHostingProvider   = AppSettings["SplendidHostingProvider"  ];
						string sSplendidHostingConnection = AppSettings["SplendidHostingConnection"];
						if ( Sql.IsEmptyString(sSplendidHostingProvider) || Sql.IsEmptyString(sSplendidHostingConnection) )
						{
							throw(new Exception("SplendidHostingProvider and SplendidHostingConnection are both required in order to pull the connection from a hosting server. "));
						}
						else
						{
							// 12/22/2007 Paul.  We can no longer rely upon the Request object being valid as we might be inside the timer event. 
							string sSplendidHostingSite = Sql.ToString(Application["ServerName"     ]);
							string sApplicationPath     = Sql.ToString(Application["ApplicationPath"]);
							if ( sApplicationPath != "/" )
								sSplendidHostingSite += sApplicationPath;
							
							DbProviderFactory dbf = GetFactory(sSplendidHostingProvider, sSplendidHostingConnection);
							using ( IDbConnection con = dbf.CreateConnection() )
							{
								con.Open();
								string sSQL ;
								sSQL = "select SPLENDID_PROVIDER           " + ControlChars.CrLf
								     + "     , CONNECTION_STRING           " + ControlChars.CrLf
								     + "     , EXPIRATION_DATE             " + ControlChars.CrLf
								     + "  from vwSPLENDID_HOSTING_SITES    " + ControlChars.CrLf
								     + " where HOSTING_SITE = @HOSTING_SITE" + ControlChars.CrLf;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.CommandText = sSQL;
									Sql.AddParameter(cmd, "@HOSTING_SITE", sSplendidHostingSite);
									using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
									{
										if ( rdr.Read() )
										{
											sSplendidProvider = Sql.ToString(rdr["SPLENDID_PROVIDER"]);
											sConnectionString = Sql.ToString(rdr["CONNECTION_STRING"]);
											// 01/17/2008 Paul.  99.999% percent of the time, we will be hosting on SQL Server. 
											// If the provider is not specified, then just assume SQL Server. 
											if ( Sql.IsEmptyString(sSplendidProvider) )
												sSplendidProvider = "System.Data.SqlClient";
											if ( rdr["EXPIRATION_DATE"] != DBNull.Value )
											{
												DateTime dtEXPIRATION_DATE = Sql.ToDateTime(rdr["EXPIRATION_DATE"]);
												if ( dtEXPIRATION_DATE < DateTime.Today )
													throw(new Exception("The hosting site " + sSplendidHostingSite + " expired on " + dtEXPIRATION_DATE.ToShortDateString()));
											}
											if ( Sql.IsEmptyString(sSplendidProvider) || Sql.IsEmptyString(sSplendidProvider) )
												throw(new Exception("Incomplete database connection information was found on the hosting server for site " + sSplendidHostingSite));
										}
										else
										{
											throw(new Exception("Database connection information was not found on the hosting server for site " + sSplendidHostingSite));
										}
									}
								}
							}
						}
						break;
					}
#endif
				}
				Application["SplendidProvider"] = sSplendidProvider;
				Application["ConnectionString"] = sConnectionString;
			}
			return GetFactory(sSplendidProvider, sConnectionString);
		}

		public DbProviderFactory GetFactory(string sSplendidProvider, string sConnectionString)
		{
			switch ( sSplendidProvider )
			{
				case "System.Data.SqlClient":
				{
					return new SqlClientFactory(sConnectionString);
				}
// 12/16/2021 TODO.  System.Data.OracleClient, MySql.Data, etc. 
#if false
				case "System.Data.OracleClient":
				{
					return new OracleSystemDataFactory(sConnectionString);
				}
				case "Oracle.DataAccess.Client":
				{
					return new OracleClientFactory(sConnectionString);
				}
				case "MySql.Data":
				{
					return new MySQLClientFactory(sConnectionString);
				}
				case "IBM.Data.DB2":
				{
					return new DB2ClientFactory(sConnectionString);
				}
				case "Sybase.Data.AseClient":
				{
					return new SybaseClientFactory(sConnectionString);
				}
				case "iAnywhere.Data.AsaClient":
				{
					return new SQLAnywhereClientFactory(sConnectionString);
				}
				case "Npgsql":
				{
					return new NpgsqlClientFactory(sConnectionString);
				}
#endif
				default:
					throw(new Exception("Unsupported factory " + sSplendidProvider));
			}
		}
	}
}

