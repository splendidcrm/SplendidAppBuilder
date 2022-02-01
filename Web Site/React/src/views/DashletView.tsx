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
import { RouteComponentProps, withRouter }    from 'react-router-dom'            ;
import { observer }                           from 'mobx-react'                  ;
// 2. Store and Types. 
import IDashletProps                          from '../types/IDashletProps'      ;
// 3. Scripts. 
import { AuthenticatedMethod, LoginRedirect } from '../scripts/Login'            ;
import { Dashboards_Dashlet }                 from '../scripts/Dashboard'        ;
// 4. Components and Views. 
import ErrorComponent                         from '../components/ErrorComponent';

interface IDashletViewProps extends IDashletProps
{
	DASHLET_NAME     : string;
}

interface IDashletViewState
{
	dashlet          : any;
	error?           : any;
}

@observer
class DashletView extends React.Component<IDashletViewProps, IDashletViewState>
{
	private _isMounted = false;

	constructor(props: IDashletViewProps)
	{
		super(props);
		this.state =
		{
			dashlet: null,
		};
	}

	componentDidCatch(error, info)
	{
		console.error((new Date()).toISOString() + ' ' + this.constructor.name + '.componentDidCatch', error, info);
	}

	async componentDidMount()
	{
		//console.log((new Date()).toISOString() + ' ' + this.constructor.name + '.componentDidMount ' + this.props.MODULE_NAME + ' ' + this.props.ID);
		this._isMounted = true;
		try
		{
			let status = await AuthenticatedMethod(this.props, this.constructor.name + '.componentDidMount');
			if ( status == 1 )
			{
				await this.init();
			}
			else
			{
				LoginRedirect(this.props.history, this.constructor.name + '.componentDidMount');
			}
		}
		catch(error)
		{
			console.error((new Date()).toISOString() + ' ' + this.constructor.name + '.componentDidMount', error);
			this.setState({ error });
		}
	}

	componentDidUpdate(prevProps: IDashletViewProps)
	{
		// 04/28/2019 Paul.  Include pathname in filter to prevent double-bounce when state changes. 
		if ( this.props.location.pathname != prevProps.location.pathname )
		{
			this.init();
		}
	}

	componentWillUnmount()
	{
		this._isMounted = false;
	}

	private init = async () =>
	{
		await this.Load();
	}

	private Load = async () =>
	{
		const { DASHLET_NAME } = this.props;
		try
		{
			//console.log((new Date()).toISOString() + ' ' + this.constructor.name + '.Load() Begin', DASHLET_NAME);
			let dashlet = await Dashboards_Dashlet(DASHLET_NAME);
			//console.log((new Date()).toISOString() + ' ' + this.constructor.name + '.Load() End', dashlet);
			// 05/30/2019 Paul.  The component may be unmounted by the time the custom view is generated. 
			if ( this._isMounted )
			{
				this.setState({ dashlet: dashlet });
			}
		}
		catch(error)
		{
			console.error((new Date()).toISOString() + ' ' + this.constructor.name + '.componentDidMount', error);
			this.setState({ error });
		}
	}

	public render()
	{
		const { DASHLET_NAME, ID, TITLE, SETTINGS_EDITVIEW, DEFAULT_SETTINGS } = this.props;
		const { dashlet: Dashlet, error } = this.state;
		if ( error )
		{
			return <ErrorComponent error={error} />;
		}
		else
		{
			// 06/15/2018 Paul.  The overflowX style is forcing the ListBoxes to not expand so much. 
			return (<React.Fragment>
				{ Dashlet
				? <Dashlet {...this.props}
					history={ this.props.history }
					location={ this.props.location }
					match={ this.props.match }
				/>
				: DASHLET_NAME + ' Dashlet does not exist!'
				}
			</React.Fragment>);
		}
	}
}

export default withRouter(DashletView);

