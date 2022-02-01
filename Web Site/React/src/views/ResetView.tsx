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
import * as React from 'react';
import { RouteComponentProps, withRouter } from 'react-router-dom'              ;
// 2. Store and Types. 
// 3. Scripts. 
import Credentials                         from '../scripts/Credentials'        ;
import { StartsWith }                      from '../scripts/utility'            ;
// 4. Components and Views. 

interface IResetViewProps extends RouteComponentProps<any>
{
}

class ResetView extends React.Component<IResetViewProps>
{
	async componentDidMount()
	{
		const { history, location } = this.props;
		//console.log((new Date()).toISOString() + ' ' + this.constructor.name + '.componentDidMount', location.pathname + location.search);
		let sRedirectUrl: string = '';
		// 06/25/2019 Paul.  Remove the /Reset and continue along the path. 
		if ( location.pathname.length >= 6 )
		{
			// 10/11/2019 Paul.  Include the query parameters. 
			sRedirectUrl = location.pathname.substring(6) + location.search;
			if ( Credentials.bMOBILE_CLIENT )
			{
				if ( StartsWith(sRedirectUrl, '/android_asset/www') )
				{
					sRedirectUrl = sRedirectUrl.substring(18);
				}
				if ( sRedirectUrl == '/index.html' )
				{
					sRedirectUrl = '';
				}
			}
		}
		if ( sRedirectUrl == '' )
		{
			sRedirectUrl = '/Home';
		}
		// 08/05/2019 Paul.  Try and replace the /Reset so that the back button will work properly. 
		history.replace(sRedirectUrl);
	}

	public render()
	{
		const { history, location } = this.props;
		return (<div>{ location.pathname + location.search }</div>);
	}
}

export default withRouter(ResetView);

