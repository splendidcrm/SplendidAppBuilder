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
using System.Data;
using System.Data.Common;
using System.Web;
using System.Diagnostics;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace SplendidCRM
{
	/// <summary>
	/// Summary description for SyncError.
	/// </summary>
	public class SyncError
	{
		private IWebHostEnvironment  hostingEnvironment ;
		private DbProviderFactories  DbProviderFactories = new DbProviderFactories();
		private HttpApplicationState Application         = new HttpApplicationState();
		private HttpContext          Context            ;
		private HttpSessionState     Session            ;
		private Security             Security           ;
		private Sql                  Sql                ;
		private SqlProcs             SqlProcs           ;

		public SyncError(IWebHostEnvironment hostingEnvironment, IHttpContextAccessor httpContextAccessor, HttpSessionState Session, Security Security, Sql Sql, SqlProcs SqlProcs)
		{
			this.hostingEnvironment  = hostingEnvironment  ;
			this.Context             = httpContextAccessor.HttpContext;
			this.Session             = Session            ;
			this.Security            = Security           ;
			this.Sql                 = Sql                ;
			this.SqlProcs            = SqlProcs           ;
		}

		public void SystemWarning(StackFrame stack, string sMESSAGE)
		{
			SystemMessage("Warning", stack, sMESSAGE);
		}
		
		public void SystemError(StackFrame stack, string sMESSAGE)
		{
			SystemMessage("Error", stack, sMESSAGE);
		}
		
		public void SystemWarning(StackFrame stack, Exception ex)
		{
			// 08/13/2007 Paul.  Instead of ignoring the the english abort message, ignore the abort exception. 
			if ( ex.GetType() != Type.GetType("System.Threading.ThreadAbortException") )
			{
				string sMESSAGE = Utils.ExpandException(ex);
				SystemMessage("Warning", stack, sMESSAGE);
			}
		}
		
		public void SystemError(StackFrame stack, Exception ex)
		{
			// 08/13/2007 Paul.  Instead of ignoring the the english abort message, ignore the abort exception. 
			if ( ex.GetType() != Type.GetType("System.Threading.ThreadAbortException") )
			{
				string sMESSAGE = Utils.ExpandException(ex);
				// 01/14/2009 Paul.  Save the stack trace to help locate the source of a bug. 
				if ( ex.StackTrace != null )
					sMESSAGE += "<br />\r\n" + ex.StackTrace.Replace(ControlChars.CrLf, "<br />\r\n");
				SystemMessage("Error", stack, sMESSAGE);
			}
		}
		
		public void SystemMessage(string sERROR_TYPE, StackFrame stack, Exception ex)
		{
			string sMESSAGE = Utils.ExpandException(ex);
			if ( sERROR_TYPE == "Error" )
			{
				// 01/14/2009 Paul.  Save the stack trace to help locate the source of a bug. 
				if ( ex.StackTrace != null )
					sMESSAGE += "<br />\r\n" + ex.StackTrace.Replace(ControlChars.CrLf, "<br />\r\n");
			}
			SystemMessage(sERROR_TYPE, stack, sMESSAGE);
		}
		
		public void SystemMessage(string sERROR_TYPE, StackFrame stack, string sMESSAGE)
		{
			if ( Application == null )
				return;
			try
			{
				// 11/29/2009 Paul.  Use a global status value that can be polled. 
				Application["SystemSync.Status"] = sMESSAGE;
				DataTable dt = Application["SystemSync.Errors"] as DataTable;
				if ( dt == null )
				{
					dt = new DataTable();
					DataColumn colDATE_ENTERED = new DataColumn("DATE_ENTERED", Type.GetType("System.DateTime"));
					DataColumn colERROR_TYPE   = new DataColumn("ERROR_TYPE"  , Type.GetType("System.String"  ));
					DataColumn colFILE_NAME    = new DataColumn("FILE_NAME"   , Type.GetType("System.String"  ));
					DataColumn colMETHOD       = new DataColumn("METHOD"      , Type.GetType("System.String"  ));
					DataColumn colLINE_NUMBER  = new DataColumn("LINE_NUMBER" , Type.GetType("System.String"  ));
					DataColumn colMESSAGE      = new DataColumn("MESSAGE"     , Type.GetType("System.String"  ));
					dt.Columns.Add(colDATE_ENTERED);
					dt.Columns.Add(colERROR_TYPE  );
					dt.Columns.Add(colMESSAGE     );
					dt.Columns.Add(colFILE_NAME   );
					dt.Columns.Add(colMETHOD      );
					dt.Columns.Add(colLINE_NUMBER );
					Application["SystemSync.Errors"] = dt;
				}

				Guid   gUSER_ID          = Guid.Empty;
				string sUSER_NAME        = String.Empty;
				string sMACHINE          = String.Empty;
				string sREMOTE_URL       = Sql.ToString(Application["SplendidCRM_REMOTE_URL"]);
				string sFILE_NAME        = String.Empty;
				string sMETHOD           = String.Empty;
				Int32  nLINE_NUMBER      = 0;

				try
				{
					// 09/17/2009 Paul.  Azure does not support MachineName.  Just ignore the error. 
					sMACHINE = System.Environment.MachineName;
				}
				catch
				{
				}
				DataRow row = dt.NewRow();
				dt.Rows.Add(row);
				try
				{
					// 12/22/2007 Paul.  The current context will be null when inside a timer. 
					if ( Context != null && Context.Session != null )
					{
						gUSER_ID          = Security.USER_ID  ;
						sUSER_NAME        = Security.USER_NAME;
					}
				}
				catch
				{
				}
				row["DATE_ENTERED"] = DateTime.Now;
				row["ERROR_TYPE"  ] = sERROR_TYPE ;
				row["MESSAGE"     ] = sMESSAGE    ;
				if ( stack != null )
				{
					sFILE_NAME   = stack.GetFileName();
					sMETHOD      = stack.GetMethod().ToString();
					nLINE_NUMBER = stack.GetFileLineNumber();
					try
					{
						if ( Context != null && Context.Request != null )
						{
							if ( !Sql.IsEmptyString(sFILE_NAME) )
							{
								// 04/16/2006 Paul.  Use native function to get file name. 
								// 08/01/2007 Paul.  Include part of the path in the file name. Remove the physical root as it is not useful. 
								sFILE_NAME = sFILE_NAME.Replace(hostingEnvironment.ContentRootPath, "~" + Path.DirectorySeparatorChar);
								sFILE_NAME = sFILE_NAME.Replace(Path.DirectorySeparatorChar, '/');
							}
						}
					}
					catch
					{
					}
					row["FILE_NAME"   ] = sFILE_NAME;
					row["METHOD"      ] = sMETHOD;
					row["LINE_NUMBER" ] = nLINE_NUMBER;
				}

				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						// 10/07/2009 Paul.  We need to create our own global transaction ID to support auditing and workflow on SQL Azure, PostgreSQL, Oracle, DB2 and MySQL. 
						using ( IDbTransaction trn = Sql.BeginTransaction(con) )
						{
							try
							{
								SqlProcs.spSYSTEM_SYNC_LOG_InsertOnly(gUSER_ID, sMACHINE, sREMOTE_URL, sERROR_TYPE, sFILE_NAME, sMETHOD, nLINE_NUMBER, sMESSAGE, trn);
								trn.Commit();
							}
							catch //(Exception ex)
							{
								trn.Rollback();
								// 10/26/2008 Paul.  Can't throw an exception here as it could create an endless loop. 
								//SplendidError.SystemMessage("Error", new StackTrace(true).GetFrame(0), Utils.ExpandException(ex));
							}
						}
					}
				}
				catch
				{
				}
				// 02/11/2012 Paul.  Dumping the error message will help when debugging. 
#if DEBUG
				try
				{
					if ( sERROR_TYPE == "Error" )
						Debug.WriteLine(sMESSAGE);
				}
				catch
				{
				}
#endif
			}
			finally
			{
			}
		}
	}
}

