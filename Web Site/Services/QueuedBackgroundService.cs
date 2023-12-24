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
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace SplendidCRM
{
	// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-7.0&tabs=visual-studio
	public class QueuedBackgroundService : BackgroundService
	{
		private HttpApplicationState Application             = new HttpApplicationState();
		private readonly   IServiceProvider                  _serviceProvider;
		private readonly   ILogger<QueuedBackgroundService>  _logger         ;
		public             IBackgroundTaskQueue              TaskQueue { get; }

		public QueuedBackgroundService(IServiceProvider serviceProvider, ILogger<QueuedBackgroundService> logger, IBackgroundTaskQueue taskQueue)
		{
			_serviceProvider = serviceProvider;
			_logger          =  logger        ;
			TaskQueue        = taskQueue      ;
		}

		public override async Task StartAsync(CancellationToken stoppingToken)
		{
			using ( IServiceScope scope = _serviceProvider.CreateScope() )
			{
				SplendidError SplendidError = scope.ServiceProvider.GetRequiredService<SplendidError>();
				SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), "The Queued Hosted Service has been activated.");
			}
			await base.StartAsync(stoppingToken);
		}

		public override async Task StopAsync(CancellationToken stoppingToken)
		{
			using ( IServiceScope scope = _serviceProvider.CreateScope() )
			{
				SplendidError SplendidError = scope.ServiceProvider.GetRequiredService<SplendidError>();
				SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), "The Queued Hosted Service is stopping.");
			}
			await base.StopAsync(stoppingToken);
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while ( !stoppingToken.IsCancellationRequested )
			{
				// 12/20/203 Paul.  Service can start before database initialized. 
				if ( Sql.ToBoolean(Application["SplendidInit.InitApp"]) )
				{
					using ( IServiceScope scope = _serviceProvider.CreateScope() )
					{
						SplendidError SplendidError = scope.ServiceProvider.GetRequiredService<SplendidError>();
						var workItem = await TaskQueue.DequeueAsync(stoppingToken);
						try
						{
							string sName = nameof(workItem);
							Debug.WriteLine($"Queued Hosted Service Processing {sName}.");
							SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), $"Queued Hosted Service Processing {sName}.");
#pragma warning disable CS4014
							// 05/16/2023 Paul.  We don't want to block other work items, so don't await. 
							workItem(stoppingToken);
#pragma warning restore CS4014
						}
						catch (Exception ex)
						{
							_logger.LogError(ex, "Error occurred executing {WorkItem}.", nameof(workItem));
							SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						}
					}
				}
				// 05/23/2023 Paul.  Without delay, loop consumes 100% of resources. 
				await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
			}
		}

	}
}
