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
import SplendidCache from '../scripts/SplendidCache';
 
// 05/26/2019 Paul.  DynamicButtons are retrieved in Application_GetAllLayouts. 
/*
export async function DynamicButtons_LoadAllLayouts()
{
	let res = await SystemCacheRequestAll('GetAllDynamicButtons');
	let json = await GetSplendidResult(res);
	SplendidCache.SetDYNAMIC_BUTTONS(json.d.results);
	return (json.d.results);
}
*/

export function DynamicButtons_LoadLayout(VIEW_NAME)
{
	// 10/03/2011 Paul.  DynamicButtons_LoadLayout returns the layout. 
	let layout = SplendidCache.DynamicButtons(VIEW_NAME);
	if ( layout == null )
	{
		// 05/26/2019 Paul.  We will no longer lookup missing layout values if not in the cache. 
		// 05/21/2019 Paul.  It is common for a sub-panel not to have dynamic buttons, so don't log the event. 
		//console.log((new Date()).toISOString() + ' ' + VIEW_NAME + ' not found in DynamicButtons');
		/*
		// 06/11/2012 Paul.  Wrap System Cache requests for Cordova. 
		let res = await SystemCacheRequest('DYNAMIC_BUTTONS', 'CONTROL_INDEX asc', null, 'VIEW_NAME', VIEW_NAME, true);
		//var xhr = await CreateSplendidRequest('Rest.svc/GetModuleTable?TableName=DYNAMIC_BUTTONS&$orderby=CONTROL_INDEX asc&$filter=' + encodeURIComponent('(VIEW_NAME eq \'' + VIEW_NAME + '\' and DEFAULT_VIEW eq 0)'), 'GET');
		let json = await GetSplendidResult(res);
		SplendidCache.SetDynamicButtons(VIEW_NAME, json.d.results);
		// 10/03/2011 Paul.  DynamicButtons_LoadLayout returns the layout. 
		layout = SplendidCache.DynamicButtons(VIEW_NAME);
		*/
	}
	else
	{
		// 04/05/2021 Paul.  Return a clone of the layout so that we can dynamically modify the layout. 
		let newArray: any[] = [];
		layout.forEach((item) =>
		{
			newArray.push(Object.assign({}, item));
		});
		layout = newArray;
	}
	return layout;
}


