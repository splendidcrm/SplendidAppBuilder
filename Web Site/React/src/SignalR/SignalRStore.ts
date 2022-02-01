/*
 * Copyright (C) 2005-2022 SplendidCRM Software, Inc. 
 * MIT License
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
 * files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
 * modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
 * is furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 */

// 1. React and fabric. 
import * as H                                      from 'history'               ;
import $                                           from 'jquery'                ;
window.$ = $;
window.jQuery = $;
import 'signalr';
// 2. Store and Types. 
// 3. Scripts. 
import Credentials                                 from '../scripts/Credentials';
// 4. SignalR hubs.

function makeProxyCallback(hub, callback)
{
	return function ()
	{
		// Call the client hub method
		callback.apply(hub, $.makeArray(arguments));
	};
}

function registerHubProxies(instance, shouldSubscribe)
{
	var key, hub, memberKey, memberValue, subscriptionMethod;

	for ( key in instance )
	{
		if ( instance.hasOwnProperty(key) )
		{
			hub = instance[key];

			if ( !(hub.hubName) )
			{
				// Not a client hub
				continue;
			}

			if ( shouldSubscribe )
			{
				// We want to subscribe to the hub events
				subscriptionMethod = hub.on;
			}
			else
			{
				// We want to unsubscribe from the hub events
				subscriptionMethod = hub.off;
			}

			// Loop through all members on the hub and find client hub functions to subscribe/unsubscribe
			for ( memberKey in hub.client )
			{
				if ( hub.client.hasOwnProperty(memberKey) )
				{
					memberValue = hub.client[memberKey];

					if ( !$.isFunction(memberValue) )
					{
						// Not a client hub function
						continue;
					}

					subscriptionMethod.call(hub, memberKey, makeProxyCallback(hub, memberValue));
				}
			}
		}
	}
}

export class SignalRStore
{
	private bSignalRStarted   : boolean = false;
	private history           : H.History<H.LocationState> = null;
	private SignalR_Command   : (sHubName: string, sCommandName: string, oCommandArguments: any) => void = null;

	public SetHistory(history)
	{
		this.history = history;
	}

	public Startup()
	{
		//console.log((new Date()).toISOString() + ' ' + this.constructor.name + '.Startup', this.history);
		/*
		let sRemoteServer                 : string = Credentials.RemoteServer                  ;
		let sUSER_ID                      : string = Credentials.sUSER_ID                      ;
		let sUSER_EXTENSION               : string = Credentials.sUSER_EXTENSION               ;

		let signalR: SignalR = $.signalR;
		let proxies: any = {};
		signalR.hub = $.hubConnection(sRemoteServer + "signalr", { useDefaultPath: false, logging: true });
		signalR.hub.logging = true;
		signalR.hub.starting(function()
		{
			//console.log((new Date()).toISOString() + ' SignalRStore.Startup signalR.hub.starting');
			// Register the hub proxies as subscribed (instance, shouldSubscribe)
			registerHubProxies(proxies, true);

			this._registerSubscribedHubs();
		}).disconnected(function()
		{
			//console.log((new Date()).toISOString() + ' SignalRStore.Startup signalR.hub.disconnected');
			// Unsubscribe all hub proxies when we "disconnect".  This is to ensure that we do not re-add functional call backs. (instance, shouldSubscribe)
			registerHubProxies(proxies, false);
		});
		$.extend(signalR, proxies);

		//console.log('SignalR_Connection_Start');
		{
			let _this = this;
			// Start Connection
			// { transport: ['webSockets'] }
			// 09/17/2020 Paul.  Make sure to disable hub logging in the release build. 
			$.connection.hub.logging = true;
			$.connection.hub.start().done(function()
			{
				_this.bSignalRStarted = true;
				try
				{
				}
				catch(e)
				{
					console.error((new Date()).toISOString() + ' SignalRStore.Startup', e);
				}
			});
	}
		*/
	}

	public Shutdown()
	{
		/*
		//console.log((new Date()).toISOString() + ' ' + this.constructor.name + '.Shutdown');
		if ( this.bSignalRStarted )
		{
			try
			{
				$.connection.hub.stop();
			}
			catch(e)
			{
				console.error((new Date()).toISOString() + ' SignalRStore.Shutdown', e);
			}
		}
		*/
	}
}

const signalrStore = new SignalRStore();
export default signalrStore;


