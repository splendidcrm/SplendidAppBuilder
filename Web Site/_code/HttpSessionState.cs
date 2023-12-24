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
using System.Text.Json;

using Microsoft.AspNetCore.Http;

namespace SplendidCRM
{
	public class HttpSessionState
	{
		public static int Timeout = 20;
		private HttpContext          Context    ;

		public HttpSessionState(IHttpContextAccessor httpContextAccessor)
		{
			this.Context     = httpContextAccessor.HttpContext;
		}

		// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/app-state?view=aspnetcore-6.0
		public object this[string key]
		{
			get
			{
				object obj = null;
				try
				{
					// 06/18/2023 Paul.  Debugging with IIS express fails with Context == null. 
					if ( this.Context != null )
					{
						string value = this.Context.Session.GetString(key);
						if ( value != null )
						{
							// 12/26/2021 Paul.  JsonSerializer.Deserialize is returning JsonElement, which does not convert well to boolean. 
							if ( value == "true" )
								obj = true;
							else if ( value == "false" )
								obj = false;
							else
								obj = JsonSerializer.Deserialize<object>(value);
						}
					}
				}
				catch
				{
				}
				return obj;
			}
			set
			{
				if ( value != null )
				{
					if ( value.GetType() == typeof(DataTable) )
						throw(new Exception("HttpSessionState: Use Get/Set to serialize DataTable"));
					this.Context.Session.SetString(key, JsonSerializer.Serialize(value));
				}
				else
				{
					this.Context.Session.SetString(key, null);
				}
			}
		}

		// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state?view=aspnetcore-7.0
		public void SetTable(string key, DataTable value)
		{
			if ( value != null )
			{
				// 06/09/2023 Paul.  Must have table name to serialize. 
				if ( Sql.IsEmptyString(value.TableName) )
				{
					value.TableName = "[" + key + "]";
				}
				StringBuilder sb = new StringBuilder();
				using ( StringWriter wtr = new StringWriter(sb, System.Globalization.CultureInfo.InvariantCulture) )
				{
					(value as DataTable).WriteXml(wtr, XmlWriteMode.WriteSchema, false);
				}
				this.Context.Session.SetString(key, sb.ToString());
			}
			else
			{
				this.Context.Session.SetString(key, null);
			}
		}

		public DataTable GetTable(string key)
		{
			DataTable dt = null;
			string value = this.Context.Session.GetString(key);
			if ( value != null )
			{
				dt = new DataTable();
				using ( StringReader srdr = new StringReader(value) )
				{
					dt.ReadXml(srdr);
				}
			}
			return dt;
		}

		public void Remove(string key)
		{
			this.Context.Session.Remove(key);
		}

		public void Clear()
		{
			this.Context.Session.Clear();
		}

		public string Id
		{
			get { return this.Context.Session.Id; }
		}
	}
}
