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
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace SplendidCRM
{
	public class HttpApplicationState
	{
		private static Dictionary<string, object> Application = null;

		public HttpApplicationState()
		{
			if ( Application == null )
			{
				Application = new Dictionary<string, object>();
			}
		}
		public HttpApplicationState(IConfiguration Configuration)
		{
			if ( Application == null )
			{
				Application = new Dictionary<string, object>();
				this["CONFIG.Azure.SingleSignOn.Enabled"           ] = Configuration["Azure.SingleSignOn:Enabled"           ];
				this["CONFIG.Azure.SingleSignOn.AadTenantDomain"   ] = Configuration["Azure.SingleSignOn:AadTenantDomain"   ];
				this["CONFIG.Azure.SingleSignOn.ValidIssuer"       ] = Configuration["Azure.SingleSignOn:ValidIssuer"       ];
				this["CONFIG.Azure.SingleSignOn.AadTenantId"       ] = Configuration["Azure.SingleSignOn:AadTenantId"       ];
				this["CONFIG.Azure.SingleSignOn.AadClientId"       ] = Configuration["Azure.SingleSignOn:AadClientId"       ];
				this["CONFIG.Azure.SingleSignOn.AadSecretId"       ] = Configuration["Azure.SingleSignOn:AadSecretId"       ];
				this["CONFIG.Azure.SingleSignOn.MobileClientId"    ] = Configuration["Azure.SingleSignOn:MobileClientId"    ];
				this["CONFIG.Azure.SingleSignOn.MobileRedirectUrl" ] = Configuration["Azure.SingleSignOn:MobileRedirectUrl" ];
				this["CONFIG.Azure.SingleSignOn.Realm"             ] = Configuration["Azure.SingleSignOn:Realm"             ];
				this["CONFIG.Azure.SingleSignOn.FederationMetadata"] = Configuration["Azure.SingleSignOn:FederationMetadata"];

				this["CONFIG.ADFS.SingleSignOn.Enabled"            ] = Configuration["ADFS.SingleSignOn:Enabled"            ];
				this["CONFIG.ADFS.SingleSignOn.Authority"          ] = Configuration["ADFS.SingleSignOn:Authority"          ];
				this["CONFIG.ADFS.SingleSignOn.ClientId"           ] = Configuration["ADFS.SingleSignOn:ClientId"           ];
				this["CONFIG.ADFS.SingleSignOn.MobileClientId"     ] = Configuration["ADFS.SingleSignOn:MobileClientId"     ];
				this["CONFIG.ADFS.SingleSignOn.MobileRedirectUrl"  ] = Configuration["ADFS.SingleSignOn:MobileRedirectUrl"  ];
				this["CONFIG.ADFS.SingleSignOn.Realm"              ] = Configuration["ADFS.SingleSignOn:Realm"              ];
				this["CONFIG.ADFS.SingleSignOn.Thumbprint"         ] = Configuration["ADFS.SingleSignOn:Thumbprint"         ];
			}
		}

		public object this[string key]
		{
			get
			{
				if ( Application.ContainsKey(key) )
					return Application[key];
				return null;
			}
			set
			{
				Application[key] = value;
			}
		}

		public int Count
		{
			get
			{
				return Application.Count;
			}
		}

		public Dictionary<string, object>.KeyCollection Keys
		{
			get
			{
				return Application.Keys;
			}
		}

		public void Remove(string key)
		{
			Application.Remove(key);
		}
	}
}

