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
using System.Threading.Channels;

namespace SplendidCRM
{
	// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-7.0&tabs=visual-studio
	public interface IBackgroundTaskQueue
	{
		ValueTask QueueBackgroundWorkItemAsync(Func<CancellationToken, ValueTask> workItem);

		ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken);
	}

	public class BackgroundTaskQueue : IBackgroundTaskQueue
	{
		private readonly Channel<Func<CancellationToken, ValueTask>> _queue;

		public BackgroundTaskQueue()
		{
			// Capacity should be set based on the expected application load and
			// number of concurrent threads accessing the queue.            
			// BoundedChannelFullMode.Wait will cause calls to WriteAsync() to return a task,
			// which completes only when space became available. This leads to backpressure,
			// in case too many publishers/calls start accumulating.
			HttpApplicationState Application = new HttpApplicationState();
			int capacity  = Sql.ToInteger(Application["CONFIG.backgroundtask_capacity"]);
			if ( capacity == 0 )
				capacity = 100;
			BoundedChannelOptions options = new BoundedChannelOptions(capacity)
			{
				FullMode = BoundedChannelFullMode.Wait
			};
			_queue = Channel.CreateBounded<Func<CancellationToken, ValueTask>>(options);
		}

		public async ValueTask QueueBackgroundWorkItemAsync(Func<CancellationToken, ValueTask> workItem)
		{
			if ( workItem == null )
			{
				throw new ArgumentNullException(nameof(workItem));
			}
			await _queue.Writer.WriteAsync(workItem);
		}

		public async ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken)
		{
			var workItem = await _queue.Reader.ReadAsync(cancellationToken);
			return workItem;
		}
	}
}
