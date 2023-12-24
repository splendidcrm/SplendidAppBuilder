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
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SplendidCRM
{
	// https://weblog.west-wind.com/posts/2017/Sep/14/Accepting-Raw-Request-Body-Content-in-ASPNET-Core-API-Controllers?Page=2#Raw-String
	// 12/25/2021 Paul.  The React Client posts data using application/octet-stream as that is what ASP.NET 4.8 requires. 
	public class RawRequestBodyFormatter : InputFormatter
	{
		public RawRequestBodyFormatter()
		{
			SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/octet-stream"));
		}

		public override Boolean CanRead(InputFormatterContext context)
		{
			if ( context == null )
				throw new ArgumentNullException(nameof(context));

			var contentType = context.HttpContext.Request.ContentType;
			if ( contentType == "application/octet-stream" )
				return true;

			return false;
		}

		private System.Collections.ArrayList ConvertToArray(JsonElement json)
		{
			System.Collections.ArrayList array = new System.Collections.ArrayList();
			JsonElement.ArrayEnumerator arrenum = json.EnumerateArray();
			while ( arrenum.MoveNext() )
			{
				switch ( arrenum.Current.ValueKind )
				{
					case JsonValueKind.Null  :  array.Add(null );  break;
					case JsonValueKind.False :  array.Add(false);  break;
					case JsonValueKind.True  :  array.Add(true );  break;
					case JsonValueKind.String:  array.Add(SplendidCRM.Sql.ToString(arrenum.Current.GetString()));  break;
					// 02/07/2022 Paul.  Convert number to string so that we don't have issues converting float to int. 
					case JsonValueKind.Number:  array.Add(SplendidCRM.Sql.ToString(arrenum.Current.GetString()));  break;
					case JsonValueKind.Object:
					{
						array.Add(ConvertToDictionary(arrenum.Current));
						break;
					}
					case JsonValueKind.Array :
					{
						// 02/07/2022 Paul.  Convert JsonElement to ArrayList to leverage existing SplendidCRM code. 
						array.Add(ConvertToArray(arrenum.Current));
						break;
					}
				}
			}
			return array;
		}

		private Dictionary<string, object> ConvertToDictionary(JsonElement json)
		{
			Dictionary<string, object> dict = new Dictionary<string, object>();
			JsonElement.ObjectEnumerator objenum = json.EnumerateObject();
			while ( objenum.MoveNext() )
			{
				string sName = objenum.Current.Name;
				switch ( objenum.Current.Value.ValueKind )
				{
					case JsonValueKind.Null  :  dict[sName] = null ;  break;
					case JsonValueKind.False :  dict[sName] = false;  break;
					case JsonValueKind.True  :  dict[sName] = true ;  break;
					case JsonValueKind.String:  dict[sName] = SplendidCRM.Sql.ToString(objenum.Current.Value);  break;
					// 02/07/2022 Paul.  Convert number to string so that we don't have issues converting float to int. 
					case JsonValueKind.Number:  dict[sName] = SplendidCRM.Sql.ToString(objenum.Current.Value);  break;
					case JsonValueKind.Object:
					{
						dict[sName] = ConvertToDictionary(objenum.Current.Value);
						break;
					}
					case JsonValueKind.Array :
					{
						// 02/07/2022 Paul.  Convert JsonElement to ArrayList to leverage existing SplendidCRM code. 
						dict[sName] = ConvertToArray(objenum.Current.Value);
						break;
					}
				}
			}
			return dict;
		}

		public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
		{
			HttpRequest request = context.HttpContext.Request;
			string contentType = context.HttpContext.Request.ContentType;

			if ( contentType == "application/octet-stream" )
			{
				using ( var ms = new MemoryStream(2048) )
				{
					await request.Body.CopyToAsync(ms);
					byte[] content = ms.ToArray();
					JsonDocument jDoc = System.Text.Json.JsonDocument.Parse(content);
					// 02/07/2022 Paul.  Convert JsonDocument to Dictionary to leverage existing SplendidCRM code. 
					Dictionary<string, object> obj = ConvertToDictionary(jDoc.RootElement);
					return await InputFormatterResult.SuccessAsync(obj);
				}
			}
			return await InputFormatterResult.FailureAsync();
		}
	}
}

