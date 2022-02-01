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

export async function AutoComplete_ModuleMethod(sMODULE_NAME, sMETHOD, sREQUEST)
{
	if ( !Credentials.ValidateCredentials )
	{
		throw new Error('Invalid connection information.');
	}
	else
	{
		if ( sMODULE_NAME == 'Teams' )
		{
			sMODULE_NAME = 'Administration/Teams';
		}
		else if (sMODULE_NAME == 'Tags')
		{
			sMODULE_NAME = 'Administration/Tags';
		}
		// 06/07/2017 Paul.  Add NAICSCodes module. 
		else if (sMODULE_NAME == 'NAICSCodes')
		{
			sMODULE_NAME = 'Administration/NAICSCodes';
		}
		// 06/05/2018 Paul.  sREQUEST has already been stringified. 
		var sBody = sREQUEST;
		let res = await CreateSplendidRequest(sMODULE_NAME + '/AutoComplete.asmx/' + sMETHOD, 'POST', 'application/json; charset=utf-8', sBody);
		let json = await GetSplendidResult(res);
		return json.d;
	}
}


