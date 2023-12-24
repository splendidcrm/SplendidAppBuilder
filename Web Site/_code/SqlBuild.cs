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
using System.Threading.Tasks;
using System.Diagnostics;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace SplendidCRM
{
	// 08/01/2015 Paul.  The Microsoft Web Platform Installer is unable to deploy due to a timeout when applying the Build.sql file. 
	// Increasing the timeout in the Manifest.xml does not solve the problem. 
	// 06/30/2018 Paul.  Move SqlBuild to separate file. 
	public class SqlBuild
	{
		// http://stackoverflow.com/questions/3773857/escape-curly-brace-in-string-format
		protected const string sProgressTemplate = @"
<html>
<head>
<style type=""text/css"">
.ProgressBarFrame {{ padding: 2px; border: 1px solid #cccccc; width: 60%; background-color: #ffffff; }}
.ProgressBar      {{ background-color: #000000; }}
.ProgressBar td   {{ color: #ffffff; font-size: 12px; font-style: normal; font-weight: normal; text-decoration: none; }}
.QuestionError    {{ color: #e00000; font-size: 11px; font-style: normal; font-weight: bold; text-decoration: none; background-color: inherit; }}
</style>
</head>
<script type=""text/javascript"">
setTimeout(function()
{{
	location.reload();
}}, 3000);
</script>
<body>
The SplendidCRM database is being built.  {4}
<div class=""ProgressBarFrame"" align=""left"">
	<table cellspacing=""0"" width=""100%"" class=""ProgressBar"" style=""width: {0}%;"">
		<tbody class=""ProgressBar"">
			<tr>
				<td align=""center"" style=""padding: 2px;"">{1}%</td>
			</tr>
		</tbody>
	</table>
</div>
<div class=""QuestionError"">{2}</div>
<pre>{3}</pre>
</body>
</html>";
		protected const string sErrorTemplate = @"<html>
<head>
<style type=""text/css"">
</style>
</head>
<body>
There were errors during the SplendidCRM database build process. 
To manually enable SplendidCRM, you will need to delete the app_offline.htm file at the root of the web site. 
<pre>%0</pre>
</body>
</html>";

		private IWebHostEnvironment  hostingEnvironment ;
		private SplendidError        SplendidError      ;
		private SplendidInit         SplendidInit       ;
		private DbProviderFactories  DbProviderFactories = new DbProviderFactories();
		private HttpApplicationState Application = new HttpApplicationState();

		public SqlBuild(IWebHostEnvironment hostingEnvironment, SplendidError SplendidError, SplendidInit SplendidInit)
		{
			this.hostingEnvironment  = hostingEnvironment ;
			this.SplendidError       = SplendidError      ;
			this.SplendidInit        = SplendidInit       ;
		}

		public class BuildState
		{
			private IWebHostEnvironment  hostingEnvironment ;
			private DbProviderFactories  DbProviderFactories = new DbProviderFactories();
			private HttpApplicationState Application = new HttpApplicationState();
			private SplendidError        SplendidError      ;
			private SplendidInit         SplendidInit       ;
			private string[]             arrSQL             ;
			
			public BuildState(IWebHostEnvironment hostingEnvironment, SplendidError SplendidError, SplendidInit SplendidInit, string[] arrSQL)
			{
				this.hostingEnvironment  = hostingEnvironment ;
				this.SplendidError       = SplendidError      ;
				this.SplendidInit        = SplendidInit       ;
				this.arrSQL              = arrSQL             ;
			}
			
			public async Task Start()
			{
				Console.WriteLine("SqlBuild.Start");
				Debug.WriteLine("SqlBuild.Start");
				string sBuildLogPath = "~/App_Data/Build.log".Replace("~", hostingEnvironment.ContentRootPath);
				if ( Path.DirectorySeparatorChar == '\\' )
					sBuildLogPath = sBuildLogPath.Replace("/", "\\");
				try
				{
					DateTime dtStart = DateTime.Now;
					MaintenanceMiddleware.MaintenanceMode = true;
					Debug.WriteLine(DateTime.Now.ToString() + " Begin");
					await Task.Delay(TimeSpan.FromMilliseconds(1));
					
					int nErrors = 0;
					StringBuilder sbLogText = new StringBuilder();
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						for ( int i = 0; i < arrSQL.Length; i++ )
						{
							string sSQL = arrSQL[i].Trim();
							if ( !String.IsNullOrEmpty(sSQL) )
							{
								int nProgress = (100 * i) / arrSQL.Length;
								try
								{
									// 08/02/2015 Paul.  Do not include the SQL as it would confuse users. 
									// 06/03/2023 Paul.  Include SQL until code stablizes. 
									TimeSpan ts = DateTime.Now - dtStart;
									string sOfflineHtml = String.Format(sProgressTemplate, nProgress, nProgress, sbLogText.ToString(), sSQL, "Elapse time " + ts.ToString(@"h\:mm\:ss"));
									MaintenanceMiddleware.OfflineText = sOfflineHtml;
#if DEBUG
									int nEndOfLine = sSQL.IndexOf(ControlChars.CrLf);
									string sFirstLine = (nEndOfLine > 0) ? sSQL.Substring(0, nEndOfLine) : sSQL;
									Debug.WriteLine(sFirstLine);
									Console.WriteLine(sFirstLine);
#endif
									using ( IDbCommand cmd = con.CreateCommand() )
									{
										cmd.CommandTimeout = 0;
										cmd.CommandText = sSQL;
										cmd.ExecuteNonQuery();
									}
								}
								catch(Exception ex)
								{
									nErrors++;
									string sThisError = i.ToString() + ": " + ex.Message + ControlChars.CrLf;
									sbLogText.Append(sThisError);
									try
									{
										File.AppendAllText(sBuildLogPath, DateTime.Now.ToString() + " - " + sThisError + sSQL + ControlChars.CrLf + ControlChars.CrLf);
									}
									catch
									{
										// The App_Data folder may be read-only, so protect against exception. 
									}
								}
							}
						}
						TimeSpan tsEnd = DateTime.Now - dtStart;
						SplendidError.SystemMessage("Information", new StackTrace(true).GetFrame(0), "Database build elabase time " + tsEnd.ToString(@"h\:mm\:ss"));
					}
					try
					{
						Debug.WriteLine(DateTime.Now.ToString() + " End");
						File.AppendAllText(sBuildLogPath, DateTime.Now.ToString() + " End" + ControlChars.CrLf);
					}
					catch
					{
						// The App_Data folder may be read-only, so protect against exception. 
					}
					if ( nErrors > 0 )
					{
						SplendidError.SystemMessage("Error", new StackTrace(true).GetFrame(0), sbLogText.ToString());
						string sOfflineHtml = String.Format(sErrorTemplate, sbLogText.ToString());
						MaintenanceMiddleware.OfflineText = sOfflineHtml;
					}
					else
					{
						SplendidInit.InitApp();
						Application["SplendidInit.InitApp"] = true;
						MaintenanceMiddleware.MaintenanceMode = false;
						MaintenanceMiddleware.OfflineText     = "Build Completed at " + DateTime.Now.ToString();

						// 06/10/2023 Paul.  ZipCode table will be populated separately as it takes a very long time, roughly equally as long as rest of build file. 
						string sBuildSqlPath = "~/App_Data/ZIPCODES.5.sql".Replace("~", hostingEnvironment.ContentRootPath);
						if ( Path.DirectorySeparatorChar == '\\' )
							sBuildSqlPath = sBuildSqlPath.Replace("/", "\\");

						if ( File.Exists(sBuildSqlPath) )
						{
							string sBuildSQL = File.ReadAllText(sBuildSqlPath);
							if ( !String.IsNullOrEmpty(sBuildSQL) )
							{
								SplendidError.SystemMessage("Information", new StackTrace(true).GetFrame(0), "ZipCode build start");
								dtStart = DateTime.Now;
								sBuildSQL = sBuildSQL.Replace(ControlChars.CrLf + "go" + ControlChars.CrLf, ControlChars.CrLf + "GO" + ControlChars.CrLf);
								sBuildSQL = sBuildSQL.Replace(ControlChars.CrLf + "Go" + ControlChars.CrLf, ControlChars.CrLf + "GO" + ControlChars.CrLf);
								string[] arrZipcodesSQL = Microsoft.VisualBasic.Strings.Split(sBuildSQL, ControlChars.CrLf + "GO" + ControlChars.CrLf, -1, Microsoft.VisualBasic.CompareMethod.Text);
								using ( IDbConnection con = dbf.CreateConnection() )
								{
									con.Open();
									for ( int i = 0; i < arrZipcodesSQL.Length; i++ )
									{
										string sSQL = arrZipcodesSQL[i].Trim();
										if ( !String.IsNullOrEmpty(sSQL) )
										{
											try
											{
												using ( IDbCommand cmd = con.CreateCommand() )
												{
													cmd.CommandTimeout = 0;
													cmd.CommandText = sSQL;
													cmd.ExecuteNonQuery();
												}
											}
											catch(Exception ex)
											{
												nErrors++;
												string sThisError = i.ToString() + ": " + ex.Message + ControlChars.CrLf;
												sbLogText.Append(sThisError);
												try
												{
													SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
													File.AppendAllText(sBuildLogPath, DateTime.Now.ToString() + " - " + sThisError + sSQL + ControlChars.CrLf + ControlChars.CrLf);
												}
												catch
												{
													// The App_Data folder may be read-only, so protect against exception. 
												}
												// 06/10/2023 Paul.  No need to continue if we encounter an error. 
												break;
											}
										}
									}
									TimeSpan tsEnd = DateTime.Now - dtStart;
									SplendidError.SystemMessage("Information", new StackTrace(true).GetFrame(0), "ZipCode build elabase time " + tsEnd.ToString(@"h\:mm\:ss"));
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemMessage("Error", new StackTrace(true).GetFrame(0), ex);
					MaintenanceMiddleware.OfflineText = "Build Failed: " + ex.Message;
					try
					{
						File.AppendAllText(sBuildLogPath, DateTime.Now.ToString() + " - " + ex.Message + ControlChars.CrLf + ControlChars.CrLf);
					}
					catch
					{
						// The App_Data folder may be read-only, so protect against exception. 
					}
				}
			}
		}

		private async Task CreateDatabase()
		{
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			try
			{
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						string sSQL = "select count(*) from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'CONFIG'";
						cmd.CommandTimeout = 0;
						cmd.CommandText = sSQL;
						int nTables = Sql.ToInteger(cmd.ExecuteScalar());
					}
				}
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex.Message);
				// 06/01/20203 Paul.  If we cannot access list of tables, then database may not exist. 
				string sSplendidProvider = Sql.ToString(Application["SplendidProvider"]);
				string sConnectionString = Sql.ToString(Application["ConnectionString"]);
				string sDatabaseName     = String.Empty;
				StringBuilder sbMasterConnectionString = new StringBuilder();
				if ( sSplendidProvider == "System.Data.SqlClient" )
				{
					string[] arrConnectionString = sConnectionString.Split(";");
					foreach ( string keyvalue in arrConnectionString )
					{
						string[] arrKeyValue = keyvalue.Trim().Split("=");
						if ( arrKeyValue[0].ToLower() == "initial catalog" && arrKeyValue.Length == 2 )
						{
							sDatabaseName = arrKeyValue[1];
							sbMasterConnectionString.Append("initial catalog=master;");
						}
						else
						{
							sbMasterConnectionString.Append(keyvalue + ";");
						}
					}
					if ( !Sql.IsEmptyString(sDatabaseName) )
					{
						string sMasterConnectionString = sbMasterConnectionString.ToString();
						Debug.WriteLine(sMasterConnectionString);
						DbProviderFactory dbfMaster = DbProviderFactories.GetFactory(sSplendidProvider, sMasterConnectionString);
						using ( IDbConnection conMaster = dbfMaster.CreateConnection() )
						{
							conMaster.Open();
							using ( IDbCommand cmd = conMaster.CreateCommand() )
							{
								string sSQL = "select count(*) from sys.sysdatabases where name = '" + sDatabaseName + "'";
								cmd.CommandTimeout = 0;
								cmd.CommandText = sSQL;
								int nDatabases = Sql.ToInteger(cmd.ExecuteScalar());
								if ( nDatabases == 0 )
								{
									Console.WriteLine("SqlBuild.CreateDatabase");
									Debug.WriteLine("SqlBuild.CreateDatabase");
									sSQL = "create database " + sDatabaseName;
									cmd.CommandText = sSQL;
									cmd.ExecuteNonQuery();
									for ( int i = 0; i < 10; i++ )
									{
										// 06/08/2023 Paul.  For some reason, the database is not immediately openable. 
										await Task.Delay(TimeSpan.FromSeconds(1));
										try
										{
											using ( IDbConnection con = dbf.CreateConnection() )
											{
												con.Open();
												return;
											}
										}
										catch
										{
											Debug.WriteLine("Failed to connect to new database, pass #" + i.ToString());
										}
									}
								}
							}
						}
					}
				}
			}
		}

		private void AppDirectoryTree(string strDirectory)
		{
			FileInfo objInfo ;

			string[] arrFiles = Directory.GetFiles(strDirectory);
			for ( int i = 0 ; i < arrFiles.Length ; i++ )
			{
				objInfo = new FileInfo(arrFiles[i]);
				Debug.WriteLine(objInfo.FullName);
			}

			string[] arrDirectories = Directory.GetDirectories(strDirectory);
			for ( int i = 0 ; i < arrDirectories.Length ; i++ )
			{
				objInfo = new FileInfo(arrDirectories[i]);
				AppDirectoryTree(objInfo.FullName);
			}
		}

		public async Task BuildDatabase()
		{
			await Task.Delay(TimeSpan.FromMilliseconds(1));
			string sBuildSqlPath = "~/App_Data/Build.sql".Replace("~", hostingEnvironment.ContentRootPath);
			if ( Path.DirectorySeparatorChar == '\\' )
				sBuildSqlPath = sBuildSqlPath.Replace("/", "\\");
			try
			{
				//AppDirectoryTree(hostingEnvironment.ContentRootPath);
				// 08/01/2015 Paul.  If Build.log exists, then we have already processed the build.sql file, so skip. 
				if ( File.Exists(sBuildSqlPath) )
				{
					await CreateDatabase();
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							string sSQL = "select count(*) from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'CONFIG'";
							cmd.CommandTimeout = 1;
							cmd.CommandText = sSQL;
							int nTables = Sql.ToInteger(cmd.ExecuteScalar());
							if ( nTables == 0 )
							{
								Console.WriteLine("SqlBuild.BuildDatabase");
								Debug.WriteLine("SqlBuild.BuildDatabase");
								// 08/12/2015 Paul.  Read the file after checking for a valid database. 
								string sBuildSQL = File.ReadAllText(sBuildSqlPath);
								if ( !String.IsNullOrEmpty(sBuildSQL) )
								{
									sBuildSQL = sBuildSQL.Replace(ControlChars.CrLf + "go" + ControlChars.CrLf, ControlChars.CrLf + "GO" + ControlChars.CrLf);
									sBuildSQL = sBuildSQL.Replace(ControlChars.CrLf + "Go" + ControlChars.CrLf, ControlChars.CrLf + "GO" + ControlChars.CrLf);
									string[] arrSQL = Microsoft.VisualBasic.Strings.Split(sBuildSQL, ControlChars.CrLf + "GO" + ControlChars.CrLf, -1, Microsoft.VisualBasic.CompareMethod.Text);
									if ( arrSQL.Length > 1 )
									{
										string sOfflineHtml = String.Format(sProgressTemplate, 0, 0, String.Empty, String.Empty, String.Empty);
										MaintenanceMiddleware.OfflineText     = sOfflineHtml;
										
										if ( !MaintenanceMiddleware.MaintenanceMode )
										{
											BuildState build = new BuildState(hostingEnvironment, SplendidError, SplendidInit, arrSQL);
											// 06/03/2023 Paul.  Start async but don't wait. 
#pragma warning disable CS4014
											build.Start();
#pragma warning restore CS4014
										}
									}
								}
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex.Message);
				SplendidError.SystemMessage("Error", new StackTrace(true).GetFrame(0), ex);
			}
		}
	}
}
