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

// 05/26/2019 Paul.  DetailViewRelationships are retrieved in Application_GetAllLayouts. 
/*
export async function DetailViewRelationships_LoadAllLayouts()
{
	let res = await SystemCacheRequestAll('GetAllDetailViewsRelationships');
	let json = await GetSplendidResult(res);
	SplendidCache.SetDETAILVIEWS_RELATIONSHIPS(json.d.results);
	return (json.d.results);
}
*/

export async function DetailViewRelationships_LoadLayout(sDETAIL_NAME): Promise<any>
{
	let layout = SplendidCache.DetailViewRelationships(sDETAIL_NAME);
	if ( layout == null )
	{
		// 05/26/2019 Paul.  We will no longer lookup missing layout values if not in the cache. 
		//console.log((new Date()).toISOString() + ' ' + sDETAIL_NAME + ' not found in DetailViewRelationships');
		/*
		// 06/11/2012 Paul.  Wrap System Cache requests for Cordova. 
		let res = await SystemCacheRequest('DETAILVIEWS_RELATIONSHIPS', 'RELATIONSHIP_ORDER asc', null, 'DETAIL_NAME', sDETAIL_NAME);
		//var xhr = await CreateSplendidRequest('Rest.svc/GetModuleTable?TableName=DETAILVIEWS_RELATIONSHIPS&$orderby=RELATIONSHIP_ORDER asc&$filter=' + encodeURIComponent('DETAIL_NAME eq \'' + sDETAIL_NAME + '\''), 'GET');
		let json = await GetSplendidResult(res);
		SplendidCache.SetDetailViewRelationships(sDETAIL_NAME, json.d.results);
		// 10/03/2011 Paul.  DetailView_LoadLayout returns the layout. 
		layout = SplendidCache.DetailViewRelationships(sDETAIL_NAME);
		return (json.d.results);
		*/
	}
	return layout;
}


