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

export class SignalRCoreStore
{
	private history           : H.History<H.LocationState> = null;

	// 06/19/2023 Paul.  Call from main App to provide access to history. 
	public SetHistory(history)
	{
		this.history = history;
	}

	public Startup()
	{
		console.log((new Date()).toISOString() + ' ' + this.constructor.name + '.Startup', this.history);
		
		let sRemoteServer                 : string = Credentials.RemoteServer                  ;
		let sUSER_ID                      : string = Credentials.sUSER_ID                      ;
		let sUSER_EXTENSION               : string = Credentials.sUSER_EXTENSION               ;
		let sUSER_SMS_OPT_IN              : string = Credentials.sUSER_SMS_OPT_IN              ;
		let sUSER_PHONE_MOBILE            : string = Credentials.sUSER_PHONE_MOBILE            ;
		let sUSER_TWITTER_TRACKS          : string = Credentials.sUSER_TWITTER_TRACKS          ;
		let sUSER_CHAT_CHANNELS           : string = Credentials.sUSER_CHAT_CHANNELS           ;
		let dtPHONEBURNER_TOKEN_EXPIRES_AT: Date   = Credentials.dtPHONEBURNER_TOKEN_EXPIRES_AT;
	}

	public Shutdown()
	{
		//console.log((new Date()).toISOString() + ' ' + this.constructor.name + '.Shutdown');
	}
}

const signalrStore = new SignalRCoreStore();
export default signalrStore;

