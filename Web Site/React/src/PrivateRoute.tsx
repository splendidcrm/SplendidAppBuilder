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
import { Redirect, Route, RouteComponentProps, RouteProps, withRouter } from 'react-router-dom';
import { observer }                           from 'mobx-react'             ;
// 2. Store and Types. 
// 3. Scripts. 
import { AuthenticatedMethod, LoginRedirect } from './scripts/Login'        ;
import { StartsWith }                         from './scripts/utility'      ;
// 4. Components and Views. 

type Props =
{
	computedMatch?: any
} & RouteProps & RouteComponentProps<any>;

@observer
class PrivateRoute extends React.Component<Props>
{
	constructor(props: Props)
	{
		super(props);
	}

	async componentDidMount()
	{
		//console.log((new Date()).toISOString() + ' ' + this.constructor.name + '.componentDidMount', this.props.location.pathname + this.props.location.search);
		try
		{
			let status = await AuthenticatedMethod(this.props, this.constructor.name + '.componentDidMount');
			if ( status == 0 && !StartsWith(this.props.location.pathname, '/Reload') )
			{
				LoginRedirect(this.props.history, this.constructor.name + '.componentDidMount');
			}
		}
		catch(error)
		{
			console.error((new Date()).toISOString() + ' ' + this.constructor.name + '.componentDidMount', error);
		}
	}

	public render()
	{
		const { component: Component, ...rest } = this.props;
		const match = this.props.computedMatch
		/*
		if ( match && match.params['MODULE_NAME'] )
		{
			if ( match.params['MODULE_NAME'] != 'Reload' && match.params['MODULE_NAME'] != 'Reset' )
				localStorage.setItem('ReactLastActiveModule', match.params['MODULE_NAME']);
		}
		else
		{
			localStorage.removeItem('ReactLastActiveModule');
		}
		*/
		// 05/30/2019 Paul.  Experiment with returning to the same location, no matter how deep. 
		localStorage.setItem('ReactLastActiveModule', this.props.location.pathname);
		return <Route {...rest} render={() => <Component {...this.props} {...match.params} />} />
	}
}

export default withRouter(PrivateRoute);

