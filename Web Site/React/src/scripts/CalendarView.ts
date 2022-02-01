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
import Sql                                          from '../scripts/Sql'            ;
import { formatDate, FromJsonDate }                 from '../scripts/Formatting'     ;
import { CreateSplendidRequest, GetSplendidResult } from '../scripts/SplendidRequest';

export async function CalendarView_GetCalendar(sDATE_START, sDATE_END, gASSIGNED_USER_ID)
{
	let res = await CreateSplendidRequest('Rest.svc/GetCalendar?DATE_START=' + encodeURIComponent(sDATE_START) + '&DATE_END=' + encodeURIComponent(sDATE_END) + '&ASSIGNED_USER_ID=' + encodeURIComponent(gASSIGNED_USER_ID), 'GET');
	let json = await GetSplendidResult(res);
	return json.d;
}

// 06/05/2020 Paul.  The dates are in json format as this allows us to use the value that is returned from the REST API. 
export async function GetInviteesActivities(sDATE_START: string, sDATE_END: string, sINVITEE_LIST: string)
{
	let res = await CreateSplendidRequest('Rest.svc/GetInviteesActivities?DATE_START=' + encodeURIComponent(Sql.ToString(sDATE_START)) + '&DATE_END=' + encodeURIComponent(Sql.ToString(sDATE_END)) + '&INVITEE_LIST=' + encodeURIComponent(Sql.ToString(sINVITEE_LIST)), 'GET');
	let json = await GetSplendidResult(res);
	// 06/06/2020 Paul.  Pre-convert the dates to a date object so that it does not needed to be done on render. 
	if ( json.d && json.d.results )
	{
		let InviteesActivities: any[] = json.d.results;
		for ( let j: number = 0; j < InviteesActivities.length; j++ )
		{
			let invitee: any = InviteesActivities[j];
			for ( let j: number = 0; j < invitee.Activities.length; j++ )
			{
				let activity: any = invitee.Activities[j];
				activity.dtDATE_START = (activity.DATE_START ? FromJsonDate(activity.DATE_START) : null);
				activity.dtDATE_END   = (activity.DATE_END   ? FromJsonDate(activity.DATE_END  ) : null);
			}
		}
	}
	json.d.__total = json.__total;
	json.d.__sql = json.__sql;
	return json.d;
}

// 06/06/2020 Paul.  The dates are in json format as this allows us to use the value that is returned from the REST API. 
export async function GetInviteesList(sFIRST_NAME: string, sLAST_NAME: string, sEMAIL: string, sDATE_START: string, sDATE_END: string, nTOP: number, nSKIP: number, sORDER_BY: string)
{
	let res = await CreateSplendidRequest('Rest.svc/GetInviteesList?DATE_START=' + encodeURIComponent(Sql.ToString(sDATE_START)) + '&DATE_END=' + encodeURIComponent(Sql.ToString(sDATE_END)) + '&FIRST_NAME=' + encodeURIComponent(Sql.ToString(sFIRST_NAME)) + '&LAST_NAME=' + encodeURIComponent(Sql.ToString(sLAST_NAME)) + '&EMAIL=' + encodeURIComponent(Sql.ToString(sEMAIL)) + '&$top=' + nTOP + '&$skip=' + nSKIP + '&$orderby=' + encodeURIComponent(sORDER_BY), 'GET');
	let json = await GetSplendidResult(res);
	// 06/06/2020 Paul.  Pre-convert the dates to a date object so that it does not needed to be done on render. 
	if ( json.d && json.d.results )
	{
		let InviteesActivities: any[] = json.d.results;
		for ( let j: number = 0; j < InviteesActivities.length; j++ )
		{
			let invitee: any = InviteesActivities[j];
			for ( let j: number = 0; j < invitee.Activities.length; j++ )
			{
				let activity: any = invitee.Activities[j];
				activity.dtDATE_START = (activity.DATE_START ? FromJsonDate(activity.DATE_START) : null);
				activity.dtDATE_END   = (activity.DATE_END   ? FromJsonDate(activity.DATE_END  ) : null);
			}
		}
	}
	json.d.__total = json.__total;
	json.d.__sql = json.__sql;
	return json.d;
}


