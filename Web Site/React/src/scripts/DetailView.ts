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
import DETAILVIEWS_FIELD                            from '../types/DETAILVIEWS_FIELD';
// 3. Scripts. 
import Sql                                          from '../scripts/Sql'            ;
import Credentials                                  from '../scripts/Credentials'    ;
import SplendidCache                                from '../scripts/SplendidCache'  ;
import { CreateSplendidRequest, GetSplendidResult } from '../scripts/SplendidRequest';

export async function DetailView_LoadItem(MODULE_NAME: string, ID: string, ADMIN_MODE: boolean, archiveView: boolean)
{
	let admin: string = '';
	if ( ADMIN_MODE )
		admin = 'Administration/';
	let res = await CreateSplendidRequest(admin + 'Rest.svc/GetModuleItem?ModuleName=' + MODULE_NAME + '&ID=' + ID + '&$accessMode=view' + (archiveView ? '&$archiveView=1' : ''), 'GET');
	let json = await GetSplendidResult(res);
	// 11/19/2019 Paul.  Change to allow return of SQL. 
	json.d.__sql = json.__sql;
	return json.d;
}

// 05/05/2016 Paul.  The User Primary Role is used with role-based views. 
// 02/02/2020 Paul.  Ignore missing during DynamicLayout. 
export function DetailView_LoadLayout(DETAIL_NAME: string, ignoreMissing?: boolean)
{
	let layout: any[] = null;
	if ( Sql.IsEmptyString(Credentials.sPRIMARY_ROLE_NAME) )
	{
		layout = SplendidCache.DetailViewFields(DETAIL_NAME, ignoreMissing);
	}
	else
	{
		// 07/07/2020 Paul.  Ignore missing when looking for primary role. 
		layout = SplendidCache.DetailViewFields(DETAIL_NAME + '.' + Credentials.sPRIMARY_ROLE_NAME, true);
		if ( layout === undefined || layout == null || layout.length == 0 )
		{
			layout = SplendidCache.DetailViewFields(DETAIL_NAME, ignoreMissing);
		}
	}
	// 05/26/2019 Paul.  We will no longer lookup missing layout values if not in the cache. 
	if ( layout == null )
	{
		// 02/02/2020 Paul.  Ignore missing inline as there are too many. 
		if ( !ignoreMissing )
		{
			// 01/08/2021 Paul.  No lnoger needed. 
			//console.warn((new Date()).toISOString() + ' ' + DETAIL_NAME + ' not found in DetailViews');
		}
	}
	else
	{
		// 11/02/2019 Paul.  Return a clone of the layout so that we can dynamically modify the layout. 
		// 11/02/2019 Paul.  Hidden property is used to dynamically hide and show layout fields. 
		let newArray: any[] = [];
		layout.forEach((item) =>
		{
			newArray.push(Object.assign({hidden: false}, item));
		});
		layout = newArray;
	}
	return layout;
}

export async function DetailView_LoadAudit(MODULE_NAME: string, ID: string)
{
	let res = await CreateSplendidRequest('Rest.svc/GetModuleAudit?ModuleName=' + MODULE_NAME + '&ID=' + ID, 'GET');
	let json = await GetSplendidResult(res);
	return json.d;
}

export async function DetailView_GetByAudit(MODULE_NAME: string, AUDIT_ID: string)
{
	let res = await CreateSplendidRequest('Rest.svc/GetModuleItemByAudit?ModuleName=' + MODULE_NAME + '&AUDIT_ID=' + AUDIT_ID, 'GET');
	let json = await GetSplendidResult(res);
	return json.d;
}

export async function DetailView_LoadPersonalInfo(MODULE_NAME: string, ID: string)
{
	let res = await CreateSplendidRequest('Rest.svc/GetModulePersonal?ModuleName=' + MODULE_NAME + '&ID=' + ID, 'GET');
	let json = await GetSplendidResult(res);
	return json.d;
}

export function DetailView_RemoveField(layout: DETAILVIEWS_FIELD[], DATA_FIELD: string)
{
	// 02/08/2021 Paul.  Make sure layout is not null. 
	if ( layout && layout.length > 0 )
	{
		for ( let nLayoutIndex = 0; nLayoutIndex < layout.length; nLayoutIndex++ )
		{
			let lay = layout[nLayoutIndex];
			if ( DATA_FIELD == lay.DATA_FIELD )
			{
				layout.splice(nLayoutIndex, 1);
				break;
			}
		}
	}
}

export function DetailView_HideField(layout: DETAILVIEWS_FIELD[], DATA_FIELD: string, hidden: boolean)
{
	// 02/08/2021 Paul.  Make sure layout is not null. 
	if ( layout && layout.length > 0 )
	{
		for ( let nLayoutIndex = 0; nLayoutIndex < layout.length; nLayoutIndex++ )
		{
			let lay = layout[nLayoutIndex];
			if ( DATA_FIELD == lay.DATA_FIELD )
			{
				lay.hidden = hidden;
				break;
			}
		}
	}
}

export function DetailView_FindField(layout: DETAILVIEWS_FIELD[], DATA_FIELD: string)
{
	// 02/08/2021 Paul.  Make sure layout is not null. 
	if ( layout && layout.length > 0 )
	{
		for ( let nLayoutIndex = 0; nLayoutIndex < layout.length; nLayoutIndex++ )
		{
			let lay = layout[nLayoutIndex];
			if ( DATA_FIELD == lay.DATA_FIELD )
			{
				return lay;
			}
		}
	}
	return null;
}


