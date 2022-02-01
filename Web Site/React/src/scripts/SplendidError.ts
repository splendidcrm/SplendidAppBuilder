/*
 * Copyright (C) 2005-2022 SplendidCRM Software, Inc. 
 * MIT License
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
 * files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
 * modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
 * is furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 */
import { dumpObj } from '../scripts/utility';
import { formatDate } from '../scripts/Formatting';

export class SplendidErrorStore
{
	arrErrorLog = new Array();
	sLastError = '';

	public ClearAllErrors()
	{
		this.arrErrorLog = new Array();
		this.sLastError = '';
	}

	public SystemError(e, method)
	{
		let message = this.FormatError(e, method);
		this.arrErrorLog.push(message);
		this.sLastError = message;
	}

	public SystemMessage(message)
	{
		if ( message != null && message != '' )
		{
			// 06/27/2017 Paul.  Prepend timestamp. 
			this.arrErrorLog.push(formatDate((new Date()), 'YYYY/MM/DD HH:mm:ss') + ' ' + message);
		}
		this.sLastError = message;
	}

	public SystemLog(message)
	{
		if (message != null && message != '')
		{
			this.arrErrorLog.push(formatDate((new Date()), 'YYYY/MM/DD HH:mm:ss') + ' ' + message);
		}
	}

	public SystemAlert(e, method)
	{
		let message = this.FormatError(e, method);
		this.arrErrorLog.push(message);
		alert(message);
	}

	public FormatError(e, method)
	{
		if ( typeof(e) == 'object' )
		{
			return method + ': ' + e.message + '<br>\n' + dumpObj(e, method);
		}
		else if ( typeof(e) == 'string' )
		{
			return method + ': ' + e + '<br>\n' + dumpObj(e, method);
		}
		else if ( typeof(e) != null )
		{
			return method + ': ' + e.toString() + '<br>\n' + dumpObj(e, method);
		}
		else
		{
			return method + ': ' + 'Unknown error' + '<br>\n' + dumpObj(e, method);
		}
	}
}

const splendidErrors = new SplendidErrorStore();
export default splendidErrors;

