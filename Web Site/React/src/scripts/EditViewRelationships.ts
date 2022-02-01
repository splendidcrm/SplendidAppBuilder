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

// 05/26/2019 Paul.  EditViewRelationships are retrieved in Application_GetAllLayouts. 
/*
export async function EditViewRelationships_LoadAllLayouts(): Promise<any>
{
	let res = await SystemCacheRequestAll('GetAllEditViewsRelationships');
	let json = await GetSplendidResult(res);
	SplendidCache.SetEDITVIEWS_RELATIONSHIPS(json.d.results);
	return (json.d.results);
}
*/

export async function EditViewRelationships_LoadLayout(sEDIT_NAME): Promise<any>
{
	let layout = SplendidCache.EditViewRelationships(sEDIT_NAME);
	if ( layout == null )
	{
		// 05/26/2019 Paul.  We will no longer lookup missing layout values if not in the cache. 
		//console.log((new Date()).toISOString() + ' ' + sEDIT_NAME + ' not found in EditViewRelationships');
		/*
		// 06/11/2012 Paul.  Wrap System Cache requests for Cordova. 
		let res = await SystemCacheRequest('EDITVIEWS_RELATIONSHIPS', 'RELATIONSHIP_ORDER asc', null, 'EDIT_NAME', sEDIT_NAME);
		//var xhr = await CreateSplendidRequest('Rest.svc/GetModuleTable?TableName=EDITVIEWS_RELATIONSHIPS&$orderby=RELATIONSHIP_ORDER asc&$filter=' + encodeURIComponent('EDIT_NAME eq \'' + sEDIT_NAME + '\''), 'GET');
		let json = await GetSplendidResult(res);
		SplendidCache.SetEditViewRelationships(sEDIT_NAME, json.d.results);
		// 10/04/2011 Paul.  EditViewRelationships_LoadLayout returns the layout. 
		layout = SplendidCache.EditViewRelationships(sEDIT_NAME);
		return (json.d.results);
		*/
	}
	return layout;
}


