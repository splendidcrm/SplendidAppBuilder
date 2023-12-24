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
using System.Diagnostics;

namespace SplendidCRM.Crm
{
	public class Users
	{
		private DbProviderFactories  DbProviderFactories = new DbProviderFactories();
		private HttpApplicationState Application = new HttpApplicationState();

		public Users()
		{
		}

		public string USER_NAME(Guid gID)
		{
			string sUSER_NAME = String.Empty;
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				con.Open();
				string sSQL;
				sSQL = "select USER_NAME" + ControlChars.CrLf
				     + "  from vwUSERS  " + ControlChars.CrLf
				     + " where ID = @ID " + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Sql.AddParameter(cmd, "@ID", gID);
					using ( IDataReader rdr = cmd.ExecuteReader() )
					{
						if ( rdr.Read() )
						{
							sUSER_NAME = Sql.ToString(rdr["USER_NAME"]);
						}
					}
				}
			}
			return sUSER_NAME;
		}

		// 04/07/2014 Paul.  When adding or removing a user to a call or meeting, we also need to add the private team to the dynamic teams. 
		public Guid PRIVATE_TEAM_ID(Guid gID)
		{
			Guid gPRIVATE_TEAM_ID = Guid.Empty;
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				con.Open();
				string sSQL;
				sSQL = "select PRIVATE_TEAM_ID" + ControlChars.CrLf
				     + "  from vwUSERS_Login  " + ControlChars.CrLf
				     + " where ID = @ID       " + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Sql.AddParameter(cmd, "@ID", gID);
					using ( IDataReader rdr = cmd.ExecuteReader() )
					{
						if ( rdr.Read() )
						{
							gPRIVATE_TEAM_ID = Sql.ToGuid(rdr["PRIVATE_TEAM_ID"]);
						}
					}
				}
			}
			return gPRIVATE_TEAM_ID;
		}

		public void GetUserByExtension(string sEXTENSION, ref Guid gUSER_ID, ref Guid gTEAM_ID)
		{
			// 09/05/2013 Paul.  Use the Application as a cache for the Asterisk extension as we can correct by editing a user. 
			// 09/20/2013 Paul.  Move EXTENSION to the main table. 
			if ( Application["Users.EXTENSION." + sEXTENSION + ".USER_ID"] == null || Application["Users.EXTENSION." + sEXTENSION + ".TEAM_ID"] == null )
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					string sSQL;
					// 09/06/2013 Paul.  We need to use vwUSERS_Login so that we can get either the default time or the private team. 
					// 09/20/2013 Paul.  Move EXTENSION to the main table. 
					sSQL = "select ID                    " + ControlChars.CrLf
					     + "     , TEAM_ID               " + ControlChars.CrLf
					     + "  from vwUSERS_Login         " + ControlChars.CrLf
					     + " where EXTENSION = @EXTENSION" + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@EXTENSION", sEXTENSION);
						using ( IDataReader rdr = cmd.ExecuteReader() )
						{
							if ( rdr.Read() )
							{
								gUSER_ID = Sql.ToGuid(rdr["ID"     ]);
								gTEAM_ID = Sql.ToGuid(rdr["TEAM_ID"]);
								Application["Users.EXTENSION." + sEXTENSION + ".USER_ID"] = gUSER_ID;
								Application["Users.EXTENSION." + sEXTENSION + ".TEAM_ID"] = gTEAM_ID;
							}
						}
					}
				}
			}
			else
			{
				gUSER_ID = Sql.ToGuid(Application["Users.EXTENSION." + sEXTENSION + ".USER_ID"]);
				gTEAM_ID = Sql.ToGuid(Application["Users.EXTENSION." + sEXTENSION + ".TEAM_ID"]);
			}
		}

		public int ActiveUsers()
		{
			int nActiveUsers = 0;
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				con.Open();
				string sSQL;
				// 04/07/2015 Paul.  Change active user logic to use same as stored procedure. 
				// 05/04/2015 Paul.  We have new users for HubSpot, iContact and ConstantContact, so make more room. 
				sSQL = "select count(*)          " + ControlChars.CrLf
				     + "  from vwUSERS_Login     " + ControlChars.CrLf
				     + " where ID > '00000000-0000-0000-0000-00000000000F'" + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					nActiveUsers = Sql.ToInteger(cmd.ExecuteScalar());
				}
			}
			return nActiveUsers;
		}
	}

	public class Modules
	{
		private DbProviderFactories  DbProviderFactories = new DbProviderFactories();
		private HttpApplicationState Application = new HttpApplicationState();
		private Security             Security           ;
		private SplendidError        SplendidError      ;

		public Modules(Security Security, SplendidError SplendidError)
		{
			this.Security            = Security           ;
			this.SplendidError       = SplendidError      ;
		}

		// 09/07/2009 Paul.  We need a more consistent way to get the table name from the module name. 
		public string TableName(string sMODULE)
		{
			// 01/07/2009 Paul.  For old databases, if the table name is not known, then assume that it matches the module name. 
			string sTABLE_NAME = Sql.ToString(Application["Modules." + sMODULE + ".TableName"]);
			if ( Sql.IsEmptyString(sTABLE_NAME) )
			{
				sTABLE_NAME = sMODULE.ToUpper();
				// 10/30/2014 Paul.  Some common modules that are disabled, but generate Precompile errors. 
				if ( sTABLE_NAME == "PROJECTTASK" )
					sTABLE_NAME = "PROJECT_TASK";
				else if ( sTABLE_NAME == "PROSPECTLISTS" )
					sTABLE_NAME = "'PROSPECT_LISTS";
				else if ( sTABLE_NAME == "SMSMESSAGES" )
					sTABLE_NAME = "SMS_MESSAGES";
				else if ( sTABLE_NAME == "TWITTERMESSAGES" )
					sTABLE_NAME = "TWITTER_MESSAGES";
				else if ( sTABLE_NAME == "TWITTERTRACKS" )
					sTABLE_NAME = "TWITTER_TRACKS";
				// 03/14/2016 Paul.  Add Chat tables in case they are disabled.
				else if ( sTABLE_NAME == "CHATCHANNELS" )
					sTABLE_NAME = "CHAT_CHANNELS";
				else if ( sTABLE_NAME == "CHATMESSAGES" )
					sTABLE_NAME = "CHAT_MESSAGES";
				else if ( sTABLE_NAME == "SURVEYQUESTIONS" )
				// 03/21/2016 Paul.  Add Survey tables in case they are disabled.
					sTABLE_NAME = "SURVEY_QUESTIONS";
				else if ( sTABLE_NAME == "SURVEYRESULTS" )
					sTABLE_NAME = "SURVEY_RESULTS";
				// 03/31/2017 Paul.  Add Product Catalog. 
				else if ( sTABLE_NAME == "PRODUCTTEMPLATES" )
					sTABLE_NAME = "PRODUCT_TEMPLATES";
				else if ( sTABLE_NAME == "PRODUCTCATALOG" )
					sTABLE_NAME = "PRODUCT_TEMPLATES";
				else if ( sTABLE_NAME == "PRODUCTTYPES" )
					sTABLE_NAME = "PRODUCT_TYPES";
				// 11/02/2017 Paul.  Add more tables based on record ACL. 
				else if ( sTABLE_NAME == "CAMPAIGNTRACKERS" )
					sTABLE_NAME = "CAMPAIGN_TRKRS";
			}
			return sTABLE_NAME;
		}

		// 11/06/2011 Paul.  Make accessing the module name easier. 
		public string ModuleName(string sTABLE_NAME)
		{
			string sMODULE_NAME = Sql.ToString(Application["Modules." + sTABLE_NAME + ".ModuleName"]);
			return sMODULE_NAME;
		}

		public static string SingularTableName(string sTABLE_NAME)
		{
			if ( sTABLE_NAME.EndsWith("IES") )
				sTABLE_NAME = sTABLE_NAME.Substring(0, sTABLE_NAME.Length-3) + "Y";
			else if ( sTABLE_NAME.EndsWith("S") )
				sTABLE_NAME = sTABLE_NAME.Substring(0, sTABLE_NAME.Length-1);
			return sTABLE_NAME;
		}

		public static string SingularModuleName(string sTABLE_NAME)
		{
			if ( sTABLE_NAME.EndsWith("ies") )
				sTABLE_NAME = sTABLE_NAME.Substring(0, sTABLE_NAME.Length-3) + "y";
			else if ( sTABLE_NAME.EndsWith("s") )
				sTABLE_NAME = sTABLE_NAME.Substring(0, sTABLE_NAME.Length-1);
			return sTABLE_NAME;
		}

		public bool CustomPaging(string sMODULE)
		{
			return Sql.ToBoolean(Application["Modules." + sMODULE + ".CustomPaging"]);
		}

		// 04/04/2010 Paul.  Add EXCHANGE_SYNC so that we can enable/disable the sync buttons on the MassUpdate panels. 
		public bool ExchangeFolders(string sMODULE)
		{
			return Sql.ToBoolean(Application["Modules." + sMODULE + ".ExchangeSync" ]) && Sql.ToBoolean(Application["Modules." + sMODULE + ".ExchangeFolders"]);
		}

		// 12/02/2009 Paul.  Add the ability to disable Mass Updates. 
		public bool MassUpdate(string sMODULE)
		{
			return Sql.ToBoolean(Application["Modules." + sMODULE + ".MassUpdate"]);
		}

		// 01/13/2010 Paul.  Some customers want the ability to disable the deafult search. 
		public bool DefaultSearch(string sMODULE)
		{
			// 01/13/2010 Paul.  If the value is not set, we want to assume true. 
			object oDefaultSearch = Application["Modules." + sMODULE + ".DefaultSearch"];
			if ( oDefaultSearch == null )
				return true;
			return Sql.ToBoolean(oDefaultSearch);
		}

		// 12/22/2007 Paul.  Inside the timer event, there is no current context, so we need to pass the application. 
		public DataTable Parent(string sPARENT_TYPE, Guid gPARENT_ID)
		{
			DataTable dt = new DataTable();
			// 09/07/2009 Paul.  Use the new TableName function. 
			string sTABLE_NAME = TableName(sPARENT_TYPE);
			if ( !Sql.IsEmptyString(sTABLE_NAME) )
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					string sSQL;
					// 06/09/2008 Paul.  Use the Edit view so that description fields will be available. 
					sSQL = "select *"                          + ControlChars.CrLf
					     + "  from vw" + sTABLE_NAME + "_Edit" + ControlChars.CrLf
					     + " where ID = @ID"                   + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@ID", gPARENT_ID);
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							da.Fill(dt);
						}
					}
				}
			}
			return dt;
		}

		// 02/16/2010 Paul.  Move ToGuid to the function so that it can be captured if invalid. 
		public string ItemName(string sMODULE_NAME, object oID)
		{
			string sName = String.Empty;
			try
			{
				Guid gID = Sql.ToGuid(oID);
				sName = ItemName(sMODULE_NAME, gID);
			}
			catch(Exception ex)
			{
				sName = Sql.ToString(oID);
				SplendidError.SystemMessage("Error", new StackTrace(true).GetFrame(0), Utils.ExpandException(ex) + ControlChars.CrLf + sName);
			}
			return sName;
		}

		public string ItemName(string sMODULE_NAME, Guid gID)
		{
			string sNAME = String.Empty;
			string sTABLE_NAME = TableName(sMODULE_NAME);
			if ( !Sql.IsEmptyString(sTABLE_NAME) )
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					string sSQL;
					// 12/03/2009 Paul.  The Users table is special in that we want to use the USER_NAME instead of the NAME. 
					// The primary reason for this is to allow it to be used by the EditView Assigned User ID field. 
					// 02/03/2011 Paul.  Employees returns the USERS table, which does not define USER_NAME for an employee. 
					if ( String.Compare(sMODULE_NAME, "Employees", true) == 0 )
					{
						sSQL = "select NAME       " + ControlChars.CrLf
						     + "  from vwEMPLOYEES" + ControlChars.CrLf
						     + " where ID = @ID   " + ControlChars.CrLf;
					}
					// 09/05/2016 Paul.  vwTEAM_SETS does not have a name field. 
					else if ( String.Compare(sMODULE_NAME, "TeamSets", true) == 0 )
					{
						sSQL = "select TEAM_SET_NAME" + ControlChars.CrLf
						     + "  from vwTEAM_SETS  " + ControlChars.CrLf
						     + " where ID = @ID     " + ControlChars.CrLf;
					}
					else if ( String.Compare(sTABLE_NAME, "USERS", true) == 0 )
					{
						sSQL = "select USER_NAME as NAME"+ ControlChars.CrLf
						     + "  from vw" + sTABLE_NAME + ControlChars.CrLf
						     + " where ID = @ID"         + ControlChars.CrLf;
					}
					else
					{
						sSQL = "select NAME"             + ControlChars.CrLf
						     + "  from vw" + sTABLE_NAME + ControlChars.CrLf
						     + " where ID = @ID"         + ControlChars.CrLf;
					}
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@ID", gID);
						sNAME = Sql.ToString(cmd.ExecuteScalar());
					}
				}
			}
			return sNAME;
		}

		// 06/07/2015 Paul.  Add support for Preview button. 
		public string ActivityType(Guid gID)
		{
			string sACTIVITY_TYPE = String.Empty;
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				con.Open();
				string sSQL;
				sSQL = "select ACTIVITY_TYPE" + ControlChars.CrLf
				     + "  from vwACTIVITIES"  + ControlChars.CrLf
				     + " where ID = @ID"      + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Sql.AddParameter(cmd, "@ID", gID);
					sACTIVITY_TYPE = Sql.ToString(cmd.ExecuteScalar());
				}
			}
			return sACTIVITY_TYPE;
		}

		public DataRow ItemEdit(string sMODULE_NAME, Guid gID)
		{
			DataRow row = null;
			string sTABLE_NAME = TableName(sMODULE_NAME);
			if ( !Sql.IsEmptyString(sTABLE_NAME) )
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					string sSQL;
					sSQL = "select *"                          + ControlChars.CrLf
					     + "  from vw" + sTABLE_NAME + "_Edit" + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Security.Filter(cmd, sMODULE_NAME, "edit");
						Sql.AppendParameter(cmd, gID, "ID", false);
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								if ( dt.Rows.Count > 0 )
								{
									row = dt.Rows[0];
								}
							}
						}
					}
				}
			}
			return row;
		}

		public DataTable Items(string sMODULE)
		{
			DataTable dt = new DataTable();
			string sTABLE_NAME = this.TableName(sMODULE);
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				con.Open();
				string sSQL;
				// 12/07/2009 Paul.  The Users table is special in that we want to use the USER_NAME instead of the NAME. 
				// The primary reason for this is to allow it to be used by the EditView Assigned User ID field. 
				if ( String.Compare(sTABLE_NAME, "USERS", true) == 0 )
				{
					sSQL = "select ID               " + ControlChars.CrLf
					     + "     , USER_NAME as NAME" + ControlChars.CrLf;
				}
				else
				{
					sSQL = "select ID  " + ControlChars.CrLf
					     + "     , NAME" + ControlChars.CrLf;
				}

				sSQL += "  from vw" + sTABLE_NAME + "_List" + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Security.Filter(cmd, sMODULE, "list");
					cmd.CommandText += " order by NAME";
					
					using ( DbDataAdapter da = dbf.CreateDataAdapter() )
					{
						((IDbDataAdapter)da).SelectCommand = cmd;
						da.Fill(dt);
					}
				}
			}
			return dt;
		}

	}

	public class Emails
	{
		private HttpApplicationState Application = new HttpApplicationState();
		private Security             Security   ;

		public Emails(Security Security)
		{
			this.Security    = Security   ;
		}

		// 08/30/2010 Paul.  Exchange, Imap and Pop3 utils will all use this method to lookup a recipient by email. 
		// 08/30/2010 Paul.  The previous method only returned Contacts, where as this new method returns Contacts, Leads and Prospects. 
		public Guid RecipientByEmail(IDbConnection con, string sEMAIL)
		{
			Guid gRECIPIENT_ID = Guid.Empty;
			// 04/26/2018 Paul.  We need to apply each module security rule separately. 
			//using ( IDbCommand cmd = con.CreateCommand() )
			//{
			//	string sSQL = String.Empty;
			//	// 11/06/2010 Paul.  Fix query.  There is no ID in the view vwPARENTS_EMAIL_ADDRESS. 
			//	sSQL = "select PARENT_ID              " + ControlChars.CrLf
			//	     + "     , PARENT_TYPE            " + ControlChars.CrLf
			//	     + "  from vwPARENTS_EMAIL_ADDRESS" + ControlChars.CrLf
			//	     + " where EMAIL1 = @EMAIL1       " + ControlChars.CrLf
			//	     + "   and PARENT_TYPE in ('Contacts', 'Leads', 'Prospects')" + ControlChars.CrLf
			//	     + " order by PARENT_TYPE         " + ControlChars.CrLf;
			//	cmd.CommandText = sSQL;
			//	Sql.AddParameter(cmd, "@EMAIL1", sEMAIL);
			//	// 08/30/2010 Paul.  Use are reader just in case there are multiple results. 
			//	using ( IDataReader rdr = cmd.ExecuteReader() )
			//	{
			//		if ( rdr.Read() )
			//		{
			//			gRECIPIENT_ID = Sql.ToGuid(rdr["PARENT_ID"]);
			//		}
			//	}
			//}
			string sSQL = String.Empty;
			string sMODULE_NAME = "Contacts";
			if ( Sql.ToBoolean(Application["Modules." + sMODULE_NAME + ".Valid"]) && Sql.IsEmptyGuid(gRECIPIENT_ID) )
			{
				string sTABLE_NAME = Sql.ToString(Application["Modules." + sMODULE_NAME + ".TableName"]);
				sSQL = "select ID              " + ControlChars.CrLf
				     + "     , EMAIL1          " + ControlChars.CrLf
				     + "  from vw" + sTABLE_NAME + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					// 04/26/2018 Paul.  Exchange Sync needs to follow team hierarchy rules. 
					Security.Filter(cmd, sMODULE_NAME, "view");
					Sql.AppendParameter(cmd, sEMAIL, "EMAIL1");
					using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
					{
						if ( rdr.Read() )
						{
							gRECIPIENT_ID = Sql.ToGuid(rdr["ID"]);
						}
					}
				}
			}
			sMODULE_NAME = "Leads";
			if ( Sql.ToBoolean(Application["Modules." + sMODULE_NAME + ".Valid"]) && Sql.IsEmptyGuid(gRECIPIENT_ID) )
			{
				string sTABLE_NAME = Sql.ToString(Application["Modules." + sMODULE_NAME + ".TableName"]);
				sSQL = "select ID              " + ControlChars.CrLf
				     + "     , EMAIL1          " + ControlChars.CrLf
				     + "  from vw" + sTABLE_NAME + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					// 04/26/2018 Paul.  Exchange Sync needs to follow team hierarchy rules. 
					Security.Filter(cmd, sMODULE_NAME, "view");
					Sql.AppendParameter(cmd, sEMAIL, "EMAIL1");
					using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
					{
						if ( rdr.Read() )
						{
							gRECIPIENT_ID = Sql.ToGuid(rdr["ID"]);
						}
					}
				}
			}
			sMODULE_NAME = "Prospects";
			if ( Sql.ToBoolean(Application["Modules." + sMODULE_NAME + ".Valid"]) && Sql.IsEmptyGuid(gRECIPIENT_ID) )
			{
				string sTABLE_NAME = Sql.ToString(Application["Modules." + sMODULE_NAME + ".TableName"]);
				sSQL = "select ID              " + ControlChars.CrLf
				     + "     , EMAIL1          " + ControlChars.CrLf
				     + "  from vw" + sTABLE_NAME + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					// 04/26/2018 Paul.  Exchange Sync needs to follow team hierarchy rules. 
					Security.Filter(cmd, sMODULE_NAME, "view");
					Sql.AppendParameter(cmd, sEMAIL, "EMAIL1");
					using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
					{
						if ( rdr.Read() )
						{
							gRECIPIENT_ID = Sql.ToGuid(rdr["ID"]);
						}
					}
				}
			}
			return gRECIPIENT_ID;
		}
	}

	// 05/27/2016 Paul.  Move LoadFile to Crm.Images class. 
	public class Images
	{
		private HttpApplicationState Application = new HttpApplicationState();
		private SqlProcs             SqlProcs   ;

		public Images(SqlProcs SqlProcs)
		{
			this.SqlProcs    = SqlProcs;
		}

		public void LoadFile(Guid gID, Stream stm, IDbTransaction trn)
		{
			if ( Sql.StreamBlobs(trn.Connection) )
			{
				const int BUFFER_LENGTH = 4*1024;
				byte[] binFILE_POINTER = new byte[16];
				// 01/20/2006 Paul.  Must include in transaction
				SqlProcs.spIMAGE_InitPointer(gID, ref binFILE_POINTER, trn);
				using ( BinaryReader reader = new BinaryReader(stm) )
				{
					int nFILE_OFFSET = 0 ;
					byte[] binBYTES = reader.ReadBytes(BUFFER_LENGTH);
					while ( binBYTES.Length > 0 )
					{
						// 08/14/2005 Paul.  gID is used by Oracle, binFILE_POINTER is used by SQL Server. 
						// 01/20/2006 Paul.  Must include in transaction
						SqlProcs.spIMAGE_WriteOffset(gID, binFILE_POINTER, nFILE_OFFSET, binBYTES, trn);
						nFILE_OFFSET += binBYTES.Length;
						binBYTES = reader.ReadBytes(BUFFER_LENGTH);
					}
				}
			}
			else
			{
				using ( BinaryReader reader = new BinaryReader(stm) )
				{
					byte[] binBYTES = reader.ReadBytes((int) stm.Length);
					SqlProcs.spIMAGES_CONTENT_Update(gID, binBYTES, trn);
				}
			}
		}

		// 05/27/2016 Paul.  We need a version that accepts a byte array. 
		public void LoadFile(Guid gID, byte[] binDATA, IDbTransaction trn)
		{
			if ( Sql.StreamBlobs(trn.Connection) )
			{
				const int BUFFER_LENGTH = 4*1024;
				byte[] binFILE_POINTER = new byte[16];
				SqlProcs.spIMAGE_InitPointer(gID, ref binFILE_POINTER, trn);
				using ( MemoryStream stm = new MemoryStream(binDATA) )
				{
					using ( BinaryReader reader = new BinaryReader(stm) )
					{
						int nFILE_OFFSET = 0 ;
						byte[] binBYTES = reader.ReadBytes(BUFFER_LENGTH);
						while ( binBYTES.Length > 0 )
						{
							SqlProcs.spIMAGE_WriteOffset(gID, binFILE_POINTER, nFILE_OFFSET, binBYTES, trn);
							nFILE_OFFSET += binBYTES.Length;
							binBYTES = reader.ReadBytes(BUFFER_LENGTH);
						}
					}
				}
			}
			else
			{
				SqlProcs.spIMAGES_CONTENT_Update(gID, binDATA, trn);
			}
		}

	}

	public class Config
	{
		private DbProviderFactories  DbProviderFactories = new DbProviderFactories();
		private HttpApplicationState Application = new HttpApplicationState();

		public Config()
		{
		}

		// 12/09/2010 Paul.  Provide a way to customize the AutoComplete.CompletionSetCount. 
		public int CompletionSetCount()
		{
			int nCompletionSetCount = Sql.ToInteger(Application["CONFIG.AutoComplete.CompletionSetCount"]);
			if ( nCompletionSetCount <= 0 )
				nCompletionSetCount = 12;
			return nCompletionSetCount;
		}

		// 09/08/2009 Paul.  Allow custom paging to be turned on and off. 
		public bool allow_custom_paging()
		{
			return Sql.ToBoolean(Application["CONFIG.allow_custom_paging"]);
		}
		// 09/16/2018 Paul.  Create a multi-tenant system. 
		public bool enable_multi_tenant_teams()
		{
			return Sql.ToBoolean(Application["CONFIG.enable_multi_tenant_teams"]);
		}
		public bool enable_team_management()
		{
			// 09/16/2018 Paul.  Create a multi-tenant system. 
			return Sql.ToBoolean(Application["CONFIG.enable_team_management"]) || Sql.ToBoolean(Application["CONFIG.enable_multi_tenant_teams"]);
		}
		public bool require_team_management()
		{
			// 09/16/2018 Paul.  Create a multi-tenant system. 
			return Sql.ToBoolean(Application["CONFIG.require_team_management"]) || Sql.ToBoolean(Application["CONFIG.enable_multi_tenant_teams"]);
		}
		// 08/28/2009 Paul.  Allow dynamic teams to be turned off. 
		public bool enable_dynamic_teams()
		{
			return Sql.ToBoolean(Application["CONFIG.enable_dynamic_teams"]);
		}
		// 11/30/2017 Paul. Add ASSIGNED_SET_ID for Dynamic User Assignment. 
		public bool enable_dynamic_assignment()
		{
			return Sql.ToBoolean(Application["CONFIG.enable_dynamic_assignment"]);
		}
		// 04/02/2018 Paul.  Enable Dynamic Mass Update.
		public bool enable_dynamic_mass_update()
		{
			return Sql.ToBoolean(Application["CONFIG.enable_dynamic_mass_update"]);
		}
		// 04/28/2016 Paul.  Allow team hierarchy. 
		public bool enable_team_hierarchy()
		{
			return Sql.ToBoolean(Application["CONFIG.enable_team_hierarchy"]);
		}
		// 01/01/2008 Paul.  We need a quick way to require user assignments across the system. 
		public bool require_user_assignment()
		{
			return Sql.ToBoolean(Application["CONFIG.require_user_assignment"]);
		}
		// 06/26/2018 Paul.  Data Privacy uses the module enabled flag. 
		public bool enable_data_privacy()
		{
			return Sql.ToBoolean(Application["Modules.DataPrivacy.Valid"]);
		}
		// 01/27/2011 Paul.  Need to be able to call show_unassigned from the ExchangeSync service. 
		public bool show_unassigned()
		{
			// 01/22/2007 Paul.  If ASSIGNED_USER_ID is null, then let everybody see it. 
			// This was added to work around a bug whereby the ASSIGNED_USER_ID was not automatically assigned to the creating user. 
			return Sql.ToBoolean(Application["CONFIG.show_unassigned"]);
		}
		public string inbound_email_case_subject_macro()
		{
			string sMacro = Sql.ToString(Application["CONFIG.inbound_email_case_subject_macro"]);
			if ( Sql.IsEmptyString(sMacro) )
				sMacro = "[CASE:%1]";
			return sMacro;
		}
		// 03/30/2008 Paul.  Provide a way to disable silverlight graphs. 
		public bool enable_silverlight()
		{
			return Sql.ToBoolean(Application["CONFIG.enable_silverlight"]);
		}
		// 03/30/2008 Paul.  Provide a way to disable flash graphs. 
		public bool enable_flash()
		{
			return Sql.ToBoolean(Application["CONFIG.enable_flash"]);
		}
		// 01/13/2010 Paul.  Provide a way for the popup window options to be specified. 
		public string default_popup_width()
		{
			string sWidth = Sql.ToString(Application["CONFIG.default_popup_width"]);
			if ( Sql.IsEmptyString(sWidth) )
				sWidth = "900";  // 04/04/2016 Paul.  Increase to 900. 
			return sWidth;
		}
		public string default_popup_height()
		{
			string sHeight = Sql.ToString(Application["CONFIG.default_popup_height"]);
			if ( Sql.IsEmptyString(sHeight) )
				sHeight = "900";  // 04/04/2016 Paul.  Increase to 900. 
			return sHeight;
		}
		// 02/10/2010 Paul.  Provide a way for the popup window position to be specified. 
		public string default_popup_left()
		{
			string sLeft = Sql.ToString(Application["CONFIG.default_popup_left"]);
			return sLeft;
		}
		public string default_popup_top()
		{
			string sTop = Sql.ToString(Application["CONFIG.default_popup_top"]);
			return sTop;
		}
		public string PopupWindowOptions()
		{
			string sOptions = "width=" + default_popup_width() + ",height=" + default_popup_height() + ",resizable=1,scrollbars=1";
			// 02/10/2010 Paul.  Include left and top, if provided.  Otherwise, use default location. 
			string sLeft = default_popup_left();
			string sTop  = default_popup_top();
			if ( !Sql.IsEmptyString(sLeft) )
				sOptions += ",left=" + sLeft;
			if ( !Sql.IsEmptyString(sTop) )
				sOptions += ",top=" + sTop;
			return sOptions;
		}

		public string SiteURL()
		{
			string sSiteURL = Sql.ToString(Application["CONFIG.site_url"]);
			if ( Sql.IsEmptyString(sSiteURL) )
			{
				// 12/15/2007 Paul.  Use the environment as it is always available. 
				// The Request object is not always available, such as when inside a timer event. 
				// 12/22/2007 Paul.  We are now storing the server name in an application variable. 
				// 12/27/2020 Paul.  We need the initial scheme when creating the default site_url. 
				string sServerScheme    = Sql.ToString(Application["ServerScheme"   ]);
				string sServerName      = Sql.ToString(Application["ServerName"     ]);
				string sApplicationPath = Sql.ToString(Application["ApplicationPath"]);
				sSiteURL = sServerScheme + "://" + sServerName + sApplicationPath;
			}
			if ( !sSiteURL.StartsWith("http") )
				sSiteURL = "http://" + sSiteURL;
			if ( !sSiteURL.EndsWith("/") )
				sSiteURL += "/";
			return sSiteURL;
		}

		public string Value(string sNAME)
		{
			string sVALUE = String.Empty;
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				con.Open();
				string sSQL;
				sSQL = "select VALUE       " + ControlChars.CrLf
				     + "  from vwCONFIG    " + ControlChars.CrLf
				     + " where NAME = @NAME" + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Sql.AddParameter(cmd, "@NAME", sNAME);
					sVALUE = Sql.ToString(cmd.ExecuteScalar());
				}
			}
			return sVALUE;
		}

		// 08/07/2015 Paul.  Revenue Line Items. 
		public string OpportunitiesMode()
		{
			string sOpportunitiesMode = Sql.ToString(Application["CONFIG.OpportunitiesMode"]);
			if ( String.Compare(sOpportunitiesMode, "Revenue", true) == 0 )
				sOpportunitiesMode = "Revenue";
			else
				sOpportunitiesMode = "Opportunities";
			return sOpportunitiesMode;
		}

		// 03/24/2020 Paul.  Reports require an additional scheduler join. 
		public Boolean WorkflowExists()
		{
			bool bWorkflowExists = Sql.ToBoolean(Application["Exists.WORKFLOW"]);
			if ( !bWorkflowExists && Application["Exists.WORKFLOW"] == null )
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					string sSQL;
					sSQL = "select count(*)               " + ControlChars.CrLf
					     + "  from vwSqlTables            " + ControlChars.CrLf
					     + " where TABLE_NAME = 'WORKFLOW'" + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						bWorkflowExists = (Sql.ToInteger(cmd.ExecuteScalar()) > 0);
						Application["Exists.WORKFLOW"] = bWorkflowExists;
					}
				}
			}
			return bWorkflowExists;
		}
	}

	public class Password
	{
		private HttpApplicationState Application = new HttpApplicationState();

		public Password()
		{
		}

		public int PreferredPasswordLength
		{
			get
			{
				string sValue = Sql.ToString(Application["CONFIG.Password.PreferredPasswordLength"]);
				if ( Sql.IsEmptyString(sValue) )
					sValue = "6";
				return Sql.ToInteger(sValue);
			}
		}

		public int MinimumLowerCaseCharacters
		{
			get
			{
				string sValue = Sql.ToString(Application["CONFIG.Password.MinimumLowerCaseCharacters"]);
				if ( Sql.IsEmptyString(sValue) )
					sValue = "1";
				return Sql.ToInteger(sValue);
			}
		}

		public int MinimumUpperCaseCharacters
		{
			get
			{
				string sValue = Sql.ToString(Application["CONFIG.Password.MinimumUpperCaseCharacters"]);
				if ( Sql.IsEmptyString(sValue) )
					sValue = "0";
				return Sql.ToInteger(sValue);
			}
		}

		public int MinimumNumericCharacters
		{
			get
			{
				string sValue = Sql.ToString(Application["CONFIG.Password.MinimumNumericCharacters"]);
				if ( Sql.IsEmptyString(sValue) )
					sValue = "1";
				return Sql.ToInteger(sValue);
			}
		}

		public int MinimumSymbolCharacters
		{
			get
			{
				string sValue = Sql.ToString(Application["CONFIG.Password.MinimumSymbolCharacters"]);
				if ( Sql.IsEmptyString(sValue) )
					sValue = "0";
				return Sql.ToInteger(sValue);
			}
		}

		public string PrefixText
		{
			get
			{
				string sValue = Sql.ToString(Application["CONFIG.Password.PrefixText"]);
				// 02/19/2011 Paul.  The default is a blank string. 
				return sValue;
			}
		}

		public string TextStrengthDescriptions
		{
			get
			{
				string sValue = Sql.ToString(Application["CONFIG.Password.TextStrengthDescriptions"]);
				// 02/19/2011 Paul.  The default is not to display strength descriptions. 
				if ( Sql.IsEmptyString(sValue) )
					sValue = ";;;;;;";
				return sValue;
			}
		}

		public string SymbolCharacters
		{
			get
			{
				string sValue = Sql.ToString(Application["CONFIG.Password.SymbolCharacters"]);
				if ( Sql.IsEmptyString(sValue) )
					sValue = "!@#$%^&*()<>?~.";
				return sValue;
			}
		}

		public int ComplexityNumber
		{
			get
			{
				string sValue = Sql.ToString(Application["CONFIG.Password.ComplexityNumber"]);
				if ( Sql.IsEmptyString(sValue) )
					sValue = "2";
				return Sql.ToInteger(sValue);
			}
		}

		public int HistoryMaximum
		{
			get
			{
				string sValue = Sql.ToString(Application["CONFIG.Password.HistoryMaximum"]);
				if ( Sql.IsEmptyString(sValue) )
					sValue = "0";
				return Sql.ToInteger(sValue);
			}
		}

		public int LoginLockoutCount()
		{
			int nValue = Sql.ToInteger(Application["CONFIG.Password.LoginLockoutCount"]);
			// 03/04/2011 Paul.  We cannot allow a lockout count of zero as it would prevent all logins. 
			if ( nValue <= 0 )
			{
				nValue = 5;
				// 03/05/2011 Paul.  Save the default value so as to reduce the conversion for each login. 
				Application["CONFIG.Password.LoginLockoutCount"] = nValue;
			}
			return nValue;
		}

		public int ExpirationDays()
		{
			int nValue = Sql.ToInteger(Application["CONFIG.Password.ExpirationDays"]);
			if ( nValue < 0 )
			{
				nValue = 0;
				// 03/05/2011 Paul.  Save the default value so as to reduce the conversion for each login. 
				Application["CONFIG.Password.ExpirationDays"] = nValue;
			}
			return nValue;
		}
	}
}

