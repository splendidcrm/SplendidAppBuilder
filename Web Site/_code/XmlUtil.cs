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
using System.Web;
using System.Xml;
using System.Text;
using System.Globalization;
using System.Diagnostics;

namespace SplendidCRM
{
	/// <summary>
	/// Summary description for XmlUtil.
	/// </summary>
	public class XmlUtil
	{
		private SplendidError SplendidError;

		public XmlUtil(SplendidError SplendidError)
		{
			this.SplendidError = SplendidError;
		}

		public DataTable CreateDataTable(XmlNode parent, string sTableName, string sPrimaryKey, string[] asColumns)
		{
			DataTable dt = new DataTable(sTableName);
			dt.Columns.Add(sPrimaryKey);
			foreach(string sColumn in asColumns)
			{
				dt.Columns.Add(sColumn);
			}
			if ( parent != null )
			{
				XmlNodeList nl = parent.SelectNodes(sTableName);
				if ( nl != null )
				{
					foreach(XmlNode node in nl)
					{
						DataRow row = dt.NewRow();
						dt.Rows.Add(row);
						try
						{
							row[sPrimaryKey] = node.Attributes.GetNamedItem(sPrimaryKey).Value;
						}
						catch (Exception ex)
						{
							SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						}
						foreach(string sColumn in asColumns)
						{
							row[sColumn] = this.SelectSingleNode(node, sColumn);
						}
					}
				}
			}
			return dt;
		}

		public DataTable CreateDataTable(XmlNode parent, string sTableName, string[] asColumns)
		{
			DataTable dt = new DataTable(sTableName);
			foreach(string sColumn in asColumns)
			{
				dt.Columns.Add(sColumn);
			}
			if ( parent != null )
			{
				XmlNodeList nl = parent.SelectNodes(sTableName);
				if ( nl != null )
				{
					foreach(XmlNode node in nl)
					{
						DataRow row = dt.NewRow();
						dt.Rows.Add(row);
						foreach(string sColumn in asColumns)
						{
							row[sColumn] = this.SelectSingleNode(node, sColumn);
						}
					}
				}
			}
			return dt;
		}

		public void RemoveAllChildren(XmlDocument xml, string sNode)
		{
			try
			{
				XmlNode node   = null;
				XmlNode parent = xml.DocumentElement;
				string[] aNode = sNode.Split('/');
				foreach ( string sNodePart in aNode )
				{
					node = parent.SelectSingleNode(sNodePart);
					if ( node == null )
					{
						return ;
					}
					parent = node;
				}
				if ( node != null )
				{
					node.RemoveAll();
				}
			}
			catch(Exception /* ex */)
			{
			}
		}

		public string SelectSingleNode(XmlDocument xml, string sNode)
		{
			try
			{
				if ( xml.DocumentElement != null )
				{
					XmlNode node = xml.DocumentElement.SelectSingleNode(sNode);
					if ( node != null )
					{
						return node.InnerText;
					}
				}
			}
			catch(Exception /* ex */)
			{
			}
			return String.Empty;
		}

		public string GetNamedItem(XmlNode xNode, string sAttribute)
		{
			string sValue = String.Empty;
			XmlNode xValue = xNode.Attributes.GetNamedItem(sAttribute);
			if ( xValue != null )
				sValue = xValue.Value;
			return sValue;
		}

		public string SelectSingleNode(XmlDocument xml, string sNode, XmlNamespaceManager nsmgr)
		{
			try
			{
				if ( xml.DocumentElement != null )
				{
					// 06/20/2006 Paul.  The default namespace cannot be selected, so create an alias and reference the alias. 
					if ( sNode.IndexOf(':') < 0 )
						sNode = "defaultns:" + sNode;
					XmlNode node = xml.DocumentElement.SelectSingleNode(sNode, nsmgr);
					if ( node != null )
					{
						return node.InnerText;
					}
				}
			}
			catch(Exception /* ex */)
			{
			}
			return String.Empty;
		}

		public string SelectSingleNode(XmlDocument xml, string sNode, string sDefault)
		{
			try
			{
				if ( xml.DocumentElement != null )
				{
					XmlNode node = xml.DocumentElement.SelectSingleNode(sNode);
					if ( node != null )
					{
						if ( !Sql.IsEmptyString(node.InnerText) )
							return node.InnerText;
					}
				}
			}
			catch(Exception /* ex */)
			{
			}
			return sDefault;
		}

		public string SelectSingleNode(XmlNode node, string sNode)
		{
			try
			{
				if ( node != null )
				{
					node = node.SelectSingleNode(sNode);
					if ( node != null )
					{
						return node.InnerText;
					}
				}
			}
			catch(Exception /* ex */)
			{
			}
			return String.Empty;
		}

		// 03/29/2008 Paul.  We need a safe way to get the attribute. 
		public string SelectAttribute(XmlNode parent, string sNode, string sAttribute)
		{
			try
			{
				if ( parent != null )
				{
					XmlNode node = parent.SelectSingleNode(sNode);
					if ( node != null )
					{
						XmlNode attr = node.Attributes.GetNamedItem(sAttribute);
						if ( attr != null )
							return attr.Value;
					}
				}
			}
			catch(Exception /* ex */)
			{
			}
			return String.Empty;
		}

		public string SelectAttribute(XmlNode node, string sAttribute)
		{
			try
			{
				if ( node != null )
				{
					XmlNode attr = node.Attributes.GetNamedItem(sAttribute);
					if ( attr != null )
						return attr.Value;
				}
			}
			catch(Exception /* ex */)
			{
			}
			return String.Empty;
		}

		public string SelectSingleNode(XmlNode parent, string sNode, XmlNamespaceManager nsmgr)
		{
			try
			{
				if ( parent != null )
				{
					XmlNode node = null;
					// 10/24/2007 Paul.  We need to support multiple tags. 
					string[] aNode = sNode.Split('/');
					int i = 0;
					for ( i=0; i < aNode.Length; i++ )
					{
						string sNodeNS = aNode[i];
						if ( sNodeNS.IndexOf(':') < 0 )
							sNodeNS = "defaultns:" + sNodeNS;
						node = parent.SelectSingleNode(sNodeNS, nsmgr);
						if ( node == null )
						{
							return null;
						}
						parent = node;
					}
					if ( node != null )
					{
						return node.InnerText;
					}
				}
			}
			catch(Exception /* ex */)
			{
			}
			return String.Empty;
		}

		public XmlNode SelectNode(XmlDocument xml, string sNode, XmlNamespaceManager nsmgr)
		{
			try
			{
				XmlNode node = xml.SelectSingleNode(sNode, nsmgr);
				if ( node == null )
				{
					XmlNode parent = xml.DocumentElement;
					string[] aNode = sNode.Split('/');
					int i = 0;
					for ( i=0; i < aNode.Length; i++ )
					{
						// 06/20/2006 Paul.  The default namespace cannot be selected, so create an alias and reference the alias. 
						string sNodeNS = aNode[i];
						if ( sNodeNS.IndexOf(':') < 0 )
							sNodeNS = "defaultns:" + sNodeNS;
						node = parent.SelectSingleNode(sNodeNS, nsmgr);
						if ( node == null )
						{
							return null;
						}
						parent = node;
					}
					if ( i == aNode.Length )
						return parent;
				}
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
			return null;
		}

		public XmlNode SelectNode(XmlNode parent, string sNode, XmlNamespaceManager nsmgr)
		{
			try
			{
				XmlNode node = null;
				string[] aNode = sNode.Split('/');
				int i = 0;
				for ( i=0; i < aNode.Length; i++ )
				{
					// 06/20/2006 Paul.  The default namespace cannot be selected, so create an alias and reference the alias. 
					string sNodeNS = aNode[i];
					if ( sNodeNS.IndexOf(':') < 0 )
						sNodeNS = "defaultns:" + sNodeNS;
					node = parent.SelectSingleNode(sNodeNS, nsmgr);
					if ( node == null )
					{
						return null;
					}
					parent = node;
				}
				if ( i == aNode.Length )
					return node;
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
			return null;
		}

		public void SetSingleNode(XmlDocument xml, string sNode, string sValue)
		{
			try
			{
				XmlNode node = xml.SelectSingleNode(sNode);
				if ( node == null )
				{
					XmlNode parent = xml.DocumentElement;
					string[] aNode = sNode.Split('/');
					foreach ( string sNodePart in aNode )
					{
						node = parent.SelectSingleNode(sNodePart);
						if ( node == null )
						{
							node = xml.CreateElement(sNodePart);
							parent.AppendChild(node);
						}
						parent = node;
					}
				}
				node.InnerText = sValue;
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}

		public void SetSingleNode(XmlDocument xml, string sNode, string sValue, XmlNamespaceManager nsmgr, string sNamespaceURI)
		{
			try
			{
				XmlNode node = xml.SelectSingleNode(sNode, nsmgr);
				if ( node == null )
				{
					XmlNode parent = xml.DocumentElement;
					string[] aNode = sNode.Split('/');
					foreach ( string sNodePart in aNode )
					{
						string sNodeNS = sNodePart;
						// 06/20/2006 Paul.  The default namespace cannot be selected, so create an alias and reference the alias. 
						if ( sNodeNS.IndexOf(':') < 0 )
							sNodeNS = "defaultns:" + sNodeNS;
						node = parent.SelectSingleNode(sNodeNS, nsmgr);
						if ( node == null )
						{
							node = xml.CreateElement(sNodePart, sNamespaceURI);
							parent.AppendChild(node);
						}
						parent = node;
					}
				}
				node.InnerText = sValue;
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}

		// 11/20/2011 Paul.  Charting needs a way to skip updating if value exists. 
		public void SetSingleNode_InsertOnly(XmlDocument xml, string sNode, string sValue, XmlNamespaceManager nsmgr, string sNamespaceURI)
		{
			try
			{
				XmlNode node = xml.SelectSingleNode(sNode, nsmgr);
				if ( node == null )
				{
					XmlNode parent = xml.DocumentElement;
					string[] aNode = sNode.Split('/');
					foreach ( string sNodePart in aNode )
					{
						string sNodeNS = sNodePart;
						// 06/20/2006 Paul.  The default namespace cannot be selected, so create an alias and reference the alias. 
						if ( sNodeNS.IndexOf(':') < 0 )
							sNodeNS = "defaultns:" + sNodeNS;
						node = parent.SelectSingleNode(sNodeNS, nsmgr);
						if ( node == null )
						{
							node = xml.CreateElement(sNodePart, sNamespaceURI);
							parent.AppendChild(node);
						}
						parent = node;
					}
				}
				if ( Sql.IsEmptyString(node.InnerText) )
					node.InnerText = sValue;
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}

		public void SetSingleNodeAttribute(XmlDocument xml, string sNode, string sAttribute, string sValue)
		{
			try
			{
				XmlNode node = xml.SelectSingleNode(sNode);
				if ( node == null )
				{
					XmlNode parent = xml.DocumentElement;
					string[] aNode = sNode.Split('/');
					foreach ( string sNodePart in aNode )
					{
						node = parent.SelectSingleNode(sNodePart);
						if ( node == null )
						{
							node = xml.CreateElement(sNodePart);
							parent.AppendChild(node);
						}
						parent = node;
					}
				}
				XmlAttribute attr = xml.CreateAttribute(sAttribute);
				attr.Value = sValue;
				node.Attributes.SetNamedItem(attr);
			}
			catch(Exception /* ex */)
			{
			}
		}

		public void SetSingleNodeAttribute(XmlDocument xml, string sNode, string sAttribute, string sValue, XmlNamespaceManager nsmgr, string sNamespaceURI)
		{
			try
			{
				XmlNode node = xml.SelectSingleNode(sNode, nsmgr);
				if ( node == null )
				{
					XmlNode parent = xml.DocumentElement;
					string[] aNode = sNode.Split('/');
					foreach ( string sNodePart in aNode )
					{
						string sNodeNS = sNodePart;
						// 06/20/2006 Paul.  The default namespace cannot be selected, so create an alias and reference the alias. 
						if ( sNodeNS.IndexOf(':') < 0 )
							sNodeNS = "defaultns:" + sNodeNS;
						node = parent.SelectSingleNode(sNodeNS, nsmgr);
						if ( node == null )
						{
							node = xml.CreateElement(sNodePart, sNamespaceURI);
							parent.AppendChild(node);
						}
						parent = node;
					}
				}
				XmlAttribute attr = xml.CreateAttribute(sAttribute);
				attr.Value = sValue;
				node.Attributes.SetNamedItem(attr);
			}
			catch(Exception /* ex */)
			{
			}
		}

		public void SetSingleNodeAttribute(XmlDocument xml, XmlNode parent, string sAttribute, string sValue)
		{
			try
			{
				XmlAttribute attr = xml.CreateAttribute(sAttribute);
				attr.Value = sValue;
				parent.Attributes.SetNamedItem(attr);
			}
			catch(Exception /* ex */)
			{
			}
		}

		public void SetSingleNodeAttribute(XmlDocument xml, XmlNode parent, string sAttribute, string sValue, XmlNamespaceManager nsmgr, string sNamespaceURI)
		{
			try
			{
				XmlAttribute attr = xml.CreateAttribute(sAttribute);
				attr.Value = sValue;
				parent.Attributes.SetNamedItem(attr);
			}
			catch(Exception /* ex */)
			{
			}
		}

		// 12/10/2009 Paul.  We need to be able to set the attribute with a prefix. 
		public void SetSingleNodeAttribute(XmlDocument xml, XmlNode parent, string prefix, string sAttribute, string sValue, XmlNamespaceManager nsmgr, string sNamespaceURI)
		{
			try
			{
				XmlAttribute attr = xml.CreateAttribute(prefix, sAttribute, sNamespaceURI);
				attr.Value = sValue;
				parent.Attributes.SetNamedItem(attr);
			}
			catch(Exception /* ex */)
			{
			}
		}

		public void SetSingleNode(XmlDocument xml, XmlNode parent, string sNode, string sValue)
		{
			try
			{
				XmlNode node = xml.SelectSingleNode(sNode);
				if ( node == null )
				{
					string[] aNode = sNode.Split('/');
					foreach ( string sNodePart in aNode )
					{
						node = parent.SelectSingleNode(sNodePart);
						if ( node == null )
						{
							node = xml.CreateElement(sNodePart);
							parent.AppendChild(node);
						}
						parent = node;
					}
				}
				node.InnerText = sValue;
			}
			catch(Exception /* ex */)
			{
			}
		}

		public void SetSingleNode(XmlDocument xml, XmlNode parent, string sNode, string sValue, XmlNamespaceManager nsmgr, string sNamespaceURI)
		{
			try
			{
				XmlNode node = xml.SelectSingleNode(sNode, nsmgr);
				if ( node == null )
				{
					string[] aNode = sNode.Split('/');
					foreach ( string sNodePart in aNode )
					{
						string sNodeNS = sNodePart;
						// 06/20/2006 Paul.  The default namespace cannot be selected, so create an alias and reference the alias. 
						if ( sNodeNS.IndexOf(':') < 0 )
							sNodeNS = "defaultns:" + sNodeNS;
						node = parent.SelectSingleNode(sNodeNS, nsmgr);
						if ( node == null )
						{
							node = xml.CreateElement(sNodePart, sNamespaceURI);
							parent.AppendChild(node);
						}
						parent = node;
					}
				}
				node.InnerText = sValue;
			}
			catch(Exception /* ex */)
			{
			}
		}

		public void AppendNode(XmlDocument xml, XmlNode parent, string sNode, string sValue, XmlNamespaceManager nsmgr, string sNamespaceURI)
		{
			try
			{
				XmlNode node = null;
				string[] aNode = sNode.Split('/');
				foreach ( string sNodePart in aNode )
				{
					node = xml.CreateElement(sNodePart, sNamespaceURI);
					parent.AppendChild(node);
					parent = node;
				}
				node.InnerText = sValue;
			}
			catch(Exception /* ex */)
			{
			}
		}

		public static void Dump(ref StringBuilder sb, string sIndent, XmlNode parent)
		{
			if ( parent == null )
				return;
			sb.Append(sIndent + "<" + parent.Name);
			if ( parent.Attributes != null )
			{
				foreach ( XmlAttribute attr in parent.Attributes )
				{
					sb.Append(" "  + attr.Name  + "=");
					sb.Append("\"" + attr.Value + "\""); // TODO: encode the value.  
				}
			}
			if ( parent.HasChildNodes )
			{
				if ( parent.ChildNodes.Count == 1 && parent.ChildNodes[0].NodeType == XmlNodeType.Text )
				{
					XmlNode child = parent.ChildNodes[0];
					if ( child.Value != String.Empty )
					{
						// 10/12/2006 Paul.  Reduce the XML dump. 
						if ( child.Value.IndexOf(' ') > 0 )
						{
							sb.AppendLine(">");
							// 07/15/2010 Paul.  Use tab to make it easier to format. 
							sb.AppendLine(sIndent + "\t" + child.Value);
							sb.AppendLine(sIndent + "</" + parent.Name + ">");
						}
						else
						{
							sb.AppendLine(">" + child.Value + "</" + parent.Name + ">");
						}
					}
					else
					{
						sb.AppendLine(" />");
					}
				}
				else
				{
					sb.AppendLine(">");
					foreach ( XmlNode child in parent.ChildNodes )
					{
						if ( child.NodeType == XmlNodeType.Text )
						{
							if ( child.Value != String.Empty )
							{
								// 07/15/2010 Paul.  Use tab to make it easier to format. 
								sb.AppendLine(sIndent + "\t" + child.Value);
							}
						}
						else
						{
							// 07/15/2010 Paul.  Use tab to make it easier to format. 
							Dump(ref sb, sIndent + "\t", child);
						}
					}
					sb.AppendLine(sIndent + "</" + parent.Name + ">");
				}
			}
			else
			{
				sb.AppendLine(" />");
			}
		}

		public string BaseTypeXPath(object o)
		{
			return o.GetType().BaseType.ToString().Replace(".", "/");
		}

		private static string PHPString(MemoryStream mem)
		{
			string sSize   = String.Empty;
			string sString = String.Empty;
			int nMode = 0;
			int nChar = mem.ReadByte();
			while ( nChar != -1 )
			{
				char ch = Convert.ToChar(nChar);
				switch ( nMode )
				{
					case 0:  // Looking for ':'
						if ( ch == ':' )
							nMode = 1;
						break;
					case 1:  // Looking for a number
						if ( Char.IsDigit(ch) )
							sSize += ch;
						else if ( ch == ':' )
							nMode = 2;
						break;
					case 2: // Read string
					{
						int nSize = Int32.Parse(sSize);
						for ( int i = 0 ; i < (nSize+2) && nChar != -1 ; i++ )
						{
							if ( !(ch == '\"' && (i == 0 || i == nSize + 1)) )
								sString += ch;
							nChar = mem.ReadByte();
							if ( nChar != -1 )
								ch = Convert.ToChar(nChar);
						}
						if ( nChar != -1 && ch == ';' )
							return sString;
						nMode = 3;
						break;
					}
					case 3: // Expecting ';'
						if ( ch == ';' )
							return sString;
						break;
				}
				nChar = mem.ReadByte();
			}
			return sString;
		}

		private static string PHPInteger(MemoryStream mem)
		{
			string sNumber = String.Empty;
			int nMode = 0;
			int nChar = mem.ReadByte();
			while ( nChar != -1 )
			{
				char ch = Convert.ToChar(nChar);
				switch ( nMode )
				{
					case 0:  // Looking for ':'
						if ( ch == ':' )
							nMode = 1;
						break;
					case 1:  // Looking for a number
						if ( Char.IsDigit(ch) )
							sNumber += ch;
						else if ( ch == ';' )
						{
							return sNumber;
						}
						break;
				}
				nChar = mem.ReadByte();
			}
			return sNumber;
		}

		private void PHPArray(XmlDocument xml, XmlElement parent, MemoryStream mem)
		{
			string sSize = String.Empty;
			string sNAME  = String.Empty;
			string sVALUE = String.Empty;
			int nChar = mem.ReadByte();
			// Skip past size and get to the begging of the array. 
			while ( nChar != -1 && Convert.ToChar(nChar) != '{' )
			{
				nChar = mem.ReadByte();
			}
			if ( nChar == -1 )
				return ;

			int nMode = 0;
			nChar = mem.ReadByte();
			while ( nChar != -1 )
			{
				char ch = Convert.ToChar(nChar);
				switch ( nMode )
				{
					case 0:  // Looking for "s" at the start of the variable. 
						if ( ch == 's' )
						{
							sNAME = PHPString(mem);
							nMode = 1;
						}
						else if ( ch == 'i' )
						{
							sNAME = PHPInteger(mem);
							XmlAttribute attr = xml.CreateAttribute("index_array");
							attr.Value = "true";
							parent.Attributes.SetNamedItem(attr);
							nMode = 2;
						}
						else if ( ch == '}' )
						{
							// End of the array was reached. 
							return;
						}
						break;
					case 1: // Read variable data type
						if ( ch == 's' )
						{
							sVALUE = PHPString(mem);
							this.SetSingleNode(xml, parent, sNAME, sVALUE);
							nMode = 0;
						}
						else if ( ch == 'i' )
						{
							sVALUE = PHPInteger(mem);
							this.SetSingleNode(xml, parent, sNAME, sVALUE);
							nMode = 0;
						}
						else if ( ch == 'a' )
						{
							XmlElement node = xml.CreateElement(sNAME);
							parent.AppendChild(node);
							PHPArray(xml, node, mem);
							nMode = 0;
						}
						break;
					case 2: // Index array values. 
						if ( ch == 's' )
						{
							sVALUE = PHPString(mem);
							this.SetSingleNode(xml, parent, "index_" + sNAME, sVALUE);
							nMode = 0;
						}
						else if ( ch == 'i' )
						{
							sVALUE = PHPInteger(mem);
							this.SetSingleNode(xml, parent, "index_" + sNAME, sVALUE);
							nMode = 0;
						}
						break;
				}
				nChar = mem.ReadByte();
			}
		}

		public string ConvertToPHP(XmlElement parent)
		{
			StringBuilder sb = new StringBuilder();
			if ( parent.ChildNodes.Count > 1 )
			{
				sb.Append("s:" + parent.Name.Length.ToString() + ":\"" + parent.Name + "\";");
				sb.Append("a:" + parent.ChildNodes.Count.ToString() + "{");
				if ( Sql.ToBoolean(parent.GetAttribute("index_array")) )
				{
					int i = 0;
					foreach(XmlElement node in parent.ChildNodes)
					{
						sb.Append("i:" + i.ToString() + ";");
						sb.Append("s:" + node.InnerText.Length.ToString() + ":\"" + node.InnerText + "\";");
						i++;
					}
				}
				else
				{
					foreach(XmlElement node in parent.ChildNodes)
					{
						sb.Append(ConvertToPHP(node));
					}
				}
				sb.Append("}");
			}
			else
			{
				sb.Append("s:" + parent.Name.Length.ToString() + ":\"" + parent.Name + "\";");
				sb.Append("s:" + parent.InnerText.Length.ToString() + ":\"" + parent.InnerText + "\";");
			}
			return ToBase64String(sb.ToString());
		}

		public string ToBase64String(string s)
		{
			byte[] aby = UTF8Encoding.UTF8.GetBytes(s);
			return Convert.ToBase64String(aby);
		}

		public string FromBase64String(string s)
		{
			byte[] aby = Convert.FromBase64String(s);
			return UTF8Encoding.UTF8.GetString(aby);
		}

		// http://stackoverflow.com/questions/642125/encoding-xpath-expressions-with-both-single-and-double-quotes
		public string EncaseXpathString(string input)
		{
			// If we don't have any " then encase string in "
			if ( !input.Contains("\"") )
				return String.Format("\"{0}\"", input);
			
			// If we have some " but no ' then encase in '
			if ( !input.Contains("'") )
				return String.Format("'{0}'", input);
			
			// If we get here we have both " and ' in the string so must use Concat 
			StringBuilder sb = new StringBuilder("concat(");
			
			// Going to look for " as they are LESS likely than ' in our data so will minimise
			// number of arguments to concat.
			int lastPos = 0;
			int nextPos = input.IndexOf("\"");
			while ( nextPos != -1 )
			{
				// If this is not the first time through the loop then seperate arguments with ,
				if ( lastPos != 0 )
					sb.Append(",");
			
				sb.AppendFormat("\"{0}\",'\"'", input.Substring(lastPos, nextPos - lastPos));
				lastPos = ++nextPos;
			
				// Find next occurance 
				nextPos = input.IndexOf("\"", lastPos);
			}
			// 01/31/2010 Paul.  Original code did not add the last part. 
			if ( lastPos < input.Length )
				sb.AppendFormat(",\"{0}\"", input.Substring(lastPos));
			
			sb.Append(")");
			return sb.ToString();
		}
	}
}

