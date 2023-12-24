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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
//using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Diagnostics;

using SplendidCRM;

namespace SplendidApp
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			//Console.WriteLine("Startup.constructor");
			Configuration = configuration;
		}

		public IConfiguration Configuration
		{
			get;
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			//Console.WriteLine("Startup.ConfigureServices");
			HttpApplicationState Application = new HttpApplicationState(Configuration);
			// https://docs.microsoft.com/en-us/aspnet/core/security/authentication/windowsauth?view=aspnetcore-5.0&tabs=visual-studio
			services.AddAuthentication(IISDefaults.AuthenticationScheme);
			/*
			if ( Sql.ToBoolean(Configuration["Azure.SingleSignOn:Enabled"]) )
			{
				services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
					//.AddMicrosoftIdentityWebApp(Configuration.GetSection("AzureAd"));
					// https://docs.microsoft.com/en-us/answers/questions/567445/azure-ad-openidconnect-federation-using-addmicroso.html
					.AddMicrosoftIdentityWebApp(options =>
					{
						options.Instance = "https://login.microsoftonline.com/";
						options.Domain   = Configuration["Azure.SingleSignOn:AadTenantDomain"];
						options.TenantId = Configuration["Azure.SingleSignOn:AadTenantId"];
						options.ClientId = Configuration["Azure.SingleSignOn:AadClientId"];
						options.CallbackPath = "/signin-oidc";
						options.SignedOutCallbackPath = "/signout-callback-oidc";
						options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
					});
			}
			else
			*/
			{
				services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
				services.AddAuthentication(options =>
				{
					options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				});
			}

			// 01/25/2022 Paul.  We are now using the Identity to take advantage of [Authorize] attribute. 
			// https://blog.johnnyreilly.com/2020/12/21/how-to-make-azure-ad-403/
			// https://docs.microsoft.com/en-us/aspnet/core/security/authentication/azure-ad-b2c?view=aspnetcore-3.1#configure-the-underlying-openidconnectoptionsjwtbearercookie-options
			services.Configure<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme, options =>
			{
				options.Events.OnRedirectToAccessDenied = new Func<RedirectContext<CookieAuthenticationOptions>, Task>(context =>
				{
					L10N L10n = new L10N(context.HttpContext);
					ExceptionDetail detail = new ExceptionDetail(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS"));
					Dictionary<string, object> error = new Dictionary<string, object>();
					error.Add("ExceptionDetail", detail);
					context.Response.StatusCode = StatusCodes.Status401Unauthorized;
					context.Response.WriteAsync(JsonSerializer.Serialize(error));
					return context.Response.CompleteAsync();
				});
				options.Events.OnRedirectToLogin = context =>
				{
					L10N L10n = new L10N(context.HttpContext);
					ExceptionDetail detail = new ExceptionDetail(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS"));
					Dictionary<string, object> error = new Dictionary<string, object>();
					error.Add("ExceptionDetail", detail);
					context.Response.StatusCode = StatusCodes.Status401Unauthorized;
					context.Response.WriteAsync(JsonSerializer.Serialize(error));
					return context.Response.CompleteAsync();
				};
			});
			// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/app-state?view=aspnetcore-5.0
			services.AddHttpContextAccessor();
			services.AddMemoryCache();
			services.AddDistributedMemoryCache();
			services.AddSession(options =>
			{
				options.IdleTimeout        = TimeSpan.FromMinutes(HttpSessionState.Timeout);
				options.Cookie.HttpOnly    = true;
				options.Cookie.IsEssential = true;
			});
			services.AddScoped<HttpSessionState>();
			services.AddScoped<Security>();
			services.AddScoped<Sql>();
			services.AddScoped<SqlProcs>();
			services.AddScoped<SplendidError>();
			services.AddScoped<XmlUtil>();
			services.AddScoped<Utils>();
			services.AddScoped<CurrencyUtils>();
			services.AddScoped<SplendidInit>();
			services.AddScoped<SplendidCache>();
			services.AddScoped<RestUtil>();
			services.AddScoped<SplendidControl>();
			services.AddScoped<SplendidDynamic>();
			services.AddScoped<ModuleUtils.Audit>();
			services.AddScoped<ModuleUtils.AuditPersonalInfo>();
			services.AddScoped<ModuleUtils.EditCustomFields>();
			services.AddScoped<SplendidCRM.Crm.Users>();
			services.AddScoped<SplendidCRM.Crm.Modules>();
			services.AddScoped<SplendidCRM.Crm.Images>();
			services.AddScoped<ActiveDirectory>();
			services.AddScoped<ExchangeSync>();
			services.AddScoped<GoogleApps>();
			services.AddScoped<Spring.Social.Office365.Office365Sync>();
			services.AddScoped<ModuleUtils.Login>();
			services.AddScoped<ArchiveUtils>();

			services.AddControllersWithViews();
			// http://www.binaryintellect.net/articles/a1e0e49e-d4d0-4b7c-b758-84234f14047b.aspx
			// 12/23/2021 Paul.  We need to prevent UserProfile from getting converted to camel case. 
			services.AddControllers().AddJsonOptions(options =>
			{
				options.JsonSerializerOptions.PropertyNamingPolicy = null;
				// 12/31/2021 Paul.  We need to make sure that DBNull is treated as null instead of an empty object. 
				// https://github.com/dotnet/runtime/issues/418
				options.JsonSerializerOptions.Converters.Add(new DBNullConverter());
			});
			services.AddControllers(options =>
			{
				options.InputFormatters.Insert(0, new RawRequestBodyFormatter());
			});
			// In production, the React files will be served from this directory
			services.AddSpaStaticFiles(configuration =>
			{
				configuration.RootPath = "React/dist"; // "ClientApp/build";
			});
			services.AddRazorPages();
			
			services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
			services.AddHostedService<QueuedBackgroundService>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			Console.WriteLine("Startup.Configure");
			Debug.WriteLine("Startup.Configure");
			app.UseExceptionHandler(appError =>
			{
				appError.Run(async context =>
				{
					context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
					context.Response.ContentType = "application/json";
					IExceptionHandlerFeature contextFeature = context.Features.Get<IExceptionHandlerFeature>();
					if ( contextFeature != null )
					{
						ExceptionDetail detail = new ExceptionDetail(contextFeature.Error);
						Dictionary<string, object> error = new Dictionary<string, object>();
						error.Add("ExceptionDetail", detail);
						await context.Response.WriteAsync(JsonSerializer.Serialize(error));
					}
					else
					{
						ExceptionDetail detail = new ExceptionDetail("Startup.UseExceptionHandler contextFeature is null");
						Dictionary<string, object> error = new Dictionary<string, object>();
						error.Add("ExceptionDetail", detail);
						await context.Response.WriteAsync(JsonSerializer.Serialize(error));
					}
				});
			});
			app.UseDefaultFiles(new DefaultFilesOptions
			{
				DefaultFileNames = new List<string>
				{
					"index.cshtml"
				}
			});
			// 12/30/2021 Paul.  We must rewrite the URL very early, otherwise it is ignored. 
			app.Use(async (context, next) =>
			{
				string sRequestPath = context.Request.Path.Value;
				Console.WriteLine("Request: " + sRequestPath);
				Debug.WriteLine("Request: " + sRequestPath);
				if ( !sRequestPath.Contains(".") )
				{
					// 12/29/2021 Paul.  SpaDefaultPageMiddleware.cs: Rewrite all requests to the default page
					// This is not working.  Still get page not found for /Home
					// https://weblog.west-wind.com/posts/2020/Mar/13/Back-to-Basics-Rewriting-a-URL-in-ASPNET-Core
					// 07/31/2022 Paul.  Enable Angular UI. 
					if ( sRequestPath.Contains("/Angular") )
					{
						context.Request.Path = "/Angular";
					}
					// 06/20/2023 Paul.  Must not chagne SignalR requests. 
					else if ( !sRequestPath.StartsWith("/signalr_") )
					{
						context.Request.Path = "/";
					}
				}
				await next.Invoke();
				//return next();
			});
			//app.UseHttpsRedirection();
			// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/static-files?view=aspnetcore-5.0
			app.UseStaticFiles();
			app.UseStaticFiles(new StaticFileOptions
			{
				FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "Include")),
				RequestPath  = "/Include"
			});  // Include
			app.UseStaticFiles(new StaticFileOptions
			{
				FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "App_Themes")),
				RequestPath  = "/App_Themes"
			});  // App_Themes
			// 06/10/2023 Paul.  Not working on Angular at this time. 
			/*
			app.UseStaticFiles(new StaticFileOptions
			{
				FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "Angular/dist")),
				RequestPath  = "/Angular/dist"
			});
			*/
			/*
			app.UseStaticFiles(new StaticFileOptions
			{
				FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "React")),
				RequestPath  = "/React"
			});  // React
			*/
			app.UseSpaStaticFiles();
			app.UseRouting();
			app.UseAuthentication();
			app.UseAuthorization();
			// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/app-state?view=aspnetcore-5.0
			app.UseSession();

			// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-5.0
			// Context is null at this point.  Must go inside Use() to get valid context. 
			app.Use(async (context, next) =>
			{
				string sRequestPath = context.Request.Path.Value;
				//Console.WriteLine("Request: " + sRequestPath);
				//Debug.WriteLine("Request: " + sRequestPath);
				if ( context.User != null )
				{
					//Console.WriteLine("User: " + context.User.Identity.Name);
					//Debug.WriteLine("User: " + context.User.Identity.Name);
				}
				if ( sRequestPath.Contains(".svc") )
				{
					HttpApplicationState Application = new HttpApplicationState();
					if ( !Sql.ToBoolean(Application["SplendidInit.InitApp"]) && !MaintenanceMiddleware.MaintenanceMode )
					{
						// https://www.thecodebuzz.com/cannot-resolve-scoped-service-from-root-provider-asp-net-core/
						IServiceScope scope = app.ApplicationServices.CreateScope();
						SplendidInit SplendidInit = scope.ServiceProvider.GetService<SplendidInit>();
						await SplendidInit.InitDatabase();
					}
					ISession Session = context.Session;
					if ( Session != null && !MaintenanceMiddleware.MaintenanceMode )
					{
						//Console.WriteLine("Session: " + Session.Id);
						//Debug.WriteLine("Session: " + Session.Id);
						//Console.WriteLine("Session Items: " + Session.Keys.Count<string>().ToString());
						//Debug.WriteLine("Session Items: " + Session.Keys.Count<string>().ToString());
						if ( Session.Keys.Count<string>() == 0 )
						{
							IServiceScope scope = app.ApplicationServices.CreateScope();
							SplendidInit SplendidInit = scope.ServiceProvider.GetService<SplendidInit>();
							SplendidInit.InitSession();
						}
					}
				}
				await next.Invoke();
			});
			app.UseMiddleware<MaintenanceMiddleware>();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(name: "default", pattern: "{controller}/{action=Index}/{id?}");
				endpoints.MapRazorPages();
			});
			// 12/30/2021 Paul.  Using the standard html file does not allow for dynamic base href. 
			/*
			app.UseSpa(spa =>
			{
				spa.Options.SourcePath = "React/dist";  // "ClientApp";
				if ( env.IsDevelopment() )
				{
					//spa.UseReactDevelopmentServer(npmScript: "start");
				}
			});
			*/
		}
	}
}
