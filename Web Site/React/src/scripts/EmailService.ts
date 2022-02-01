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
// 2. Store and Types. 
// 3. Scripts. 
import Credentials from '../scripts/Credentials';
import { CreateSplendidRequest, GetSplendidResult } from '../scripts/SplendidRequest';

export async function EmailService_ParseEmail(request): Promise<any>
{
	if (!Credentials.ValidateCredentials)
	{
		throw new Error('Invalid connection information.');
	}
	else
	{
		var sBody = '{"EmailHeaders": ' + JSON.stringify(request) + '}';
		let res = await CreateSplendidRequest('BrowserExtensions/EmailService.svc/ParseEmail', 'POST', 'application/json; charset=utf-8', sBody);
		let json = await GetSplendidResult(res);
		return json.d;
	}
}

export async function EmailService_ArchiveEmail(request): Promise<any>
{
	if (!Credentials.ValidateCredentials)
	{
		throw new Error('Invalid connection information.');
	}
	else
	{
		let res = await CreateSplendidRequest('BrowserExtensions/EmailService.svc/ArchiveEmail', 'POST', 'application/octet-stream', JSON.stringify(request));
		let json = await GetSplendidResult(res);
		return json.d;
	}
}

export async function EmailService_SetEmailRelationships(sID, arrSelection): Promise<any>
{
	if (!Credentials.ValidateCredentials)
	{
		throw new Error('Invalid connection information.');
	}
	else
	{
		var sBody = '{"ID": ' + JSON.stringify(sID) + ', "Selection": ' + JSON.stringify(arrSelection) + '}';
		let res = await CreateSplendidRequest('BrowserExtensions/EmailService.svc/SetEmailRelationships', 'POST', 'application/json; charset=utf-8', sBody);
		let json = await GetSplendidResult(res);
		return json.d;
	}
}


