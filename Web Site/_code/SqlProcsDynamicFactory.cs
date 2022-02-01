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

namespace SplendidCRM
{
	public partial class SqlProcs
	{
		// 11/26/2021 Paul.  In order to support dynamically created modules in the React client, we need to load the procedures dynamically. 
		public IDbCommand DynamicFactory(IDbConnection con, string sProcedureName)
		{
			// 11/26/2021 Paul.  Store the data table of rows instead of the command so that connection does not stay referenced. 
			DataTable dt = Application["SqlProcs." + sProcedureName] as DataTable;
			if ( dt == null )
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				// 11/26/2021 Paul.  We can't use the same connection as provided as it may already be inside a transaction. 
				using ( IDbConnection con2 = dbf.CreateConnection() )
				{
					con2.Open();
					using ( IDbCommand cmd = con2.CreateCommand() )
					{
						string sSQL;
						sSQL = "select count(*)       " + ControlChars.CrLf
						     + "  from vwSqlProcedures" + ControlChars.CrLf
						     + " where name = @NAME   " + ControlChars.CrLf;
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@NAME", Sql.MetadataName(cmd, Sql.ToString(sProcedureName)));
						int nExists = Sql.ToInteger(cmd.ExecuteScalar());
						if ( nExists == 0 )
						{
							throw(new Exception("Unknown stored procedure " + sProcedureName));
						}
					}
					using ( IDbCommand cmd = con2.CreateCommand() )
					{
						string sSQL;
						sSQL = "select *                       " + ControlChars.CrLf
						     + "  from vwSqlColumns            " + ControlChars.CrLf
						     + " where ObjectName = @OBJECTNAME" + ControlChars.CrLf
						     + "   and ObjectType = 'P'        " + ControlChars.CrLf
						     + " order by colid                " + ControlChars.CrLf;
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@OBJECTNAME", Sql.MetadataName(cmd, Sql.ToString(sProcedureName)));
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							dt = new DataTable();
							da.Fill(dt);
							Application["SqlProcs." + sProcedureName] = dt;
						}
					}
				}
			}
			
			IDbCommand cmdDynamicProcedure = null;
			cmdDynamicProcedure = con.CreateCommand();
			cmdDynamicProcedure.CommandType = CommandType.StoredProcedure;
			cmdDynamicProcedure.CommandText = Sql.MetadataName(con, Sql.ToString(sProcedureName));
			for ( int j = 0 ; j < dt.Rows.Count; j++ )
			{
				DataRow row = dt.Rows[j];
				string sName      = Sql.ToString (row["ColumnName"]);
				string sCsType    = Sql.ToString (row["CsType"    ]);
				int    nLength    = Sql.ToInteger(row["length"    ]);
				bool   bIsOutput  = Sql.ToBoolean(row["isoutparam"]);
				string sBareName  = sName.Replace("@", "");
				IDbDataParameter par = Sql.CreateParameter(cmdDynamicProcedure, sName, sCsType, nLength);
				if ( bIsOutput )
					par.Direction = ParameterDirection.InputOutput;
			}
			return cmdDynamicProcedure;
		}
	}
}


