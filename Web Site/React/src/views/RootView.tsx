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
// 2. Store and Types. 
// 3. Scripts. 
import Sql                                    from '../scripts/Sql'              ;
import { Crm_Config }                         from '../scripts/Crm'              ;
import { AuthenticatedMethod, LoginRedirect } from '../scripts/Login'            ;
// 4. Components and Views. 
import ErrorComponent                         from '../components/ErrorComponent';

interface IRootViewProps extends RouteComponentProps<any>
{
}

interface IRootViewState
{
	error?: any;
}

class RootView extends React.Component<IRootViewProps, IRootViewState>
{
	constructor(props: IRootViewProps)
	{
		super(props);
		this.state = {};
	}

	async componentDidMount()
	{
		try
		{
			let status = await AuthenticatedMethod(this.props, this.constructor.name + '.componentDidMount');
			if ( status == 1 )
			{
				const { history } = this.props;
				let home: string = Crm_Config.ToString('default_module');
				if ( !Sql.IsEmptyString(home) && home != 'Home')
				{
					// 02/08/2021 Paul.  Should have a leading slash. 
					home = home.replace('~/', '');
					history.push('/' + home);
				}
				else
				{
					history.push('/Home');
				}
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

	public render()
	{
		const { error } = this.state;
		if ( error )
		{
			return <ErrorComponent error={error} />;
		}
		else
		{
			return (<div>
			</div>);
		}
	}
}

export default withRouter(RootView);

