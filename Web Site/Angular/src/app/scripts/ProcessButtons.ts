/*
 * Copyright (C) 2005-2022 SplendidCRM Software, Inc. 
 * MIT License
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
 * files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
 * modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
 * is furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 */
import { Injectable             } from '@angular/core'                      ;
import { SplendidRequestService } from '../scripts/SplendidRequest'         ;
import { SplendidCacheService   } from '../scripts/SplendidCache'           ;
import { CredentialsService     } from '../scripts/Credentials'             ;
import { L10nService            } from '../scripts/L10n'                    ;
import Sql                        from '../scripts/Sql'                     ;
import EDITVIEWS_FIELD            from '../types/EDITVIEWS_FIELD'           ;

@Injectable({
	providedIn: 'root'
})
export class ProcessButtonsService
{
	constructor(private SplendidRequest: SplendidRequestService, protected SplendidCache: SplendidCacheService, protected Credentials: CredentialsService, protected L10n: L10nService)
	{
	}

	public async GetProcessStatus(gPENDING_PROCESS_ID: string): Promise<any>
	{
		let json: any = await this.SplendidRequest.CreateSplendidRequest('Processes/Rest.svc/GetProcessStatus?ID=' + gPENDING_PROCESS_ID, 'GET');
		// 10/04/2011 Paul.  DetailViewUI.LoadItem returns the row. 
		return (json.d.results);
	}

	public async GetProcessHistory(gPENDING_PROCESS_ID: string): Promise<any>
	{
		let json: any = await this.SplendidRequest.CreateSplendidRequest('Processes/Rest.svc/GetProcessHistory?ID=' + gPENDING_PROCESS_ID, 'GET');
		json.d.__title = json.__title;
		return (json.d);
	}

	public async GetProcessNotes(gPENDING_PROCESS_ID: string): Promise<any>
	{
		let json: any = await this.SplendidRequest.CreateSplendidRequest('Processes/Rest.svc/GetProcessNotes?ID=' + gPENDING_PROCESS_ID, 'GET');
		json.d.__title = json.__title;
		return (json.d);
	}

	public async DeleteProcessNote(gPROCESS_NOTE_ID: string): Promise<any>
	{
		let json: any = await this.SplendidRequest.CreateSplendidRequest('Processes/Rest.svc/DeleteProcessNote?ID=' + gPROCESS_NOTE_ID, 'POST', 'application/octet-stream', null);
		return null;
	}

	public async AddProcessNote(gPROCESS_NOTE_ID: string, sPROCESS_NOTE: string): Promise<any>
	{
		let json: any = await this.SplendidRequest.CreateSplendidRequest('Processes/Rest.svc/AddProcessNote?ID=' + gPROCESS_NOTE_ID, 'POST', 'application/octet-stream', sPROCESS_NOTE);
		return null;
	}

	public async ProcessAction(sACTION: string, gPENDING_PROCESS_ID: string, gPROCESS_USER_ID: string, sPROCESS_NOTES: string): Promise<any>
	{
		if (! this.Credentials.ValidateCredentials )
		{
			throw new Error('Invalid connection information.');
		}
		else
		{
			let obj: any = new Object();
			obj['ACTION'            ] = sACTION            ;
			obj['PENDING_PROCESS_ID'] = gPENDING_PROCESS_ID;
			obj['PROCESS_USER_ID'   ] = gPROCESS_USER_ID   ;
			obj['PROCESS_NOTES'     ] = sPROCESS_NOTES     ;
			let sBody = JSON.stringify(obj);
			let json: any = await this.SplendidRequest.CreateSplendidRequest('Processes/Rest.svc/ProcessAction', 'POST', 'application/octet-stream', sBody);
			return 1;
		}
	}

	public async ProcessUsers(gTEAM_ID: string, sSORT_FIELD: string, sSORT_DIRECTION: string, sSELECT: string, sFILTER: string): Promise<any>
	{
		// 03/01/2013 Paul.  If sSORT_FIELD is not provided, then clear sSORT_DIRECTION. 
		if ( sSORT_FIELD === undefined || sSORT_FIELD == null || sSORT_FIELD == '' )
		{
			sSORT_FIELD = '';
			sSORT_DIRECTION = '';
		}
		let json: any = await this.SplendidRequest.CreateSplendidRequest('Processes/Rest.svc/ProcessUsers?TEAM_ID=' + gTEAM_ID + '&$orderby=' + encodeURIComponent(sSORT_FIELD + ' ' + sSORT_DIRECTION) + '&$select=' + escape(sSELECT) + '&$filter=' + escape(sFILTER), 'GET');
		// 10/04/2011 Paul.  ListView_LoadModule returns the rows. 
		return (json.d.results);
	}

	public async LoadProcessPaginated(sMODULE_NAME: string, sSORT_FIELD: string, sSORT_DIRECTION: string, sSELECT: string, sFILTER: string, rowSEARCH_VALUES: any, nTOP: number, nSKIP: number, bMyList: boolean): Promise<any>
	{
		// 03/01/2013 Paul.  If sSORT_FIELD is not provided, then clear sSORT_DIRECTION. 
		if (sSORT_FIELD === undefined || sSORT_FIELD == null || sSORT_FIELD == '')
		{
			sSORT_FIELD = '';
			sSORT_DIRECTION = '';
		}
		//console.log('ListView_LoadModulePaginated ADMIN_MODE', bADMIN_MODE);
		// 08/17/2019 Paul.  Post version so that we can support large filter requests.  This is common with the UnifiedSearch. 
		let obj: any = new Object();
		obj['$top'         ] = nTOP       ;
		obj['$skip'        ] = nSKIP      ;
		obj['$orderby'     ] = sSORT_FIELD + ' ' + sSORT_DIRECTION;
		obj['$select'      ] = sSELECT    ;
		obj['$filter'      ] = sFILTER    ;
		obj['MyList'       ] = bMyList    ;
		// 11/16/2019 Paul.  We will be ignoring the $filter as we transition to $searchvalues to avoid the security issue of passing full SQL query as a paramter. 
		if ( rowSEARCH_VALUES != null )
		{
			obj['$searchvalues'] = rowSEARCH_VALUES;
		}
		obj['ModuleName'] = sMODULE_NAME;
		let sBody: string = JSON.stringify(obj);
		let json: any = await this.SplendidRequest.CreateSplendidRequest('Processes/Rest.svc/PostModuleList', 'POST', 'application/octet-stream', sBody);
	
		//xhr.SearchValues = rowSEARCH_VALUES;
		// 10/04/2011 Paul.  ListView_LoadModule returns the rows. 
		// 04/21/2017 Paul.  We need to return the total when using nTOP. 
		// 05/13/2018 Paul.  Instead of returning just the results, we will need to return everything to get the total. 
		// 05/17/2018 Paul.  Copy total to d to simplfy the return value. 
		json.d.__total = json.__total;
		json.d.__sql = json.__sql;
		return (json.d);
	}

	public async LoadItem(ID: string)
	{
		let json: any = await this.SplendidRequest.CreateSplendidRequest('Processes/Rest.svc/GetModuleItem?ID=' + ID + '&$accessMode=view', 'GET');
		// 11/19/2019 Paul.  Change to allow return of SQL. 
		json.d.__sql = json.__sql;
		return json.d;
	}

}
