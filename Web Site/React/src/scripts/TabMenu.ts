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

export async function TabMenu_Load(): Promise<any>
{
	let layout = SplendidCache.TAB_MENU;
	if ( layout == null )
	{
		// 05/26/2019 Paul.  We will no longer lookup missing layout values if not in the cache. 
		//console.log((new Date()).toISOString() + ' Tab Menu is null');
		/*
		// 06/11/2012 Paul.  Wrap System Cache requests for Cordova. 
		let res = await SystemCacheRequest('TAB_MENUS', 'TAB_ORDER asc');
		//var xhr = await CreateSplendidRequest('Rest.svc/GetModuleTable?TableName=TAB_MENUS&$orderby=TAB_ORDER asc', 'GET');
		let json = await GetSplendidResult(res);
		SplendidCache.SetTAB_MENU(json.d.results);
		return (json.d.results);
		*/
	}
	return layout;
}


