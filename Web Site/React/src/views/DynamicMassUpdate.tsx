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
import { RouteComponentProps, withRouter }    from 'react-router-dom'              ;
import { FontAwesomeIcon }                    from '@fortawesome/react-fontawesome';
import { observer }                           from 'mobx-react'                    ;
// 2. Store and Types. 
// 3. Scripts. 
import Credentials                            from '../scripts/Credentials'        ;
import SplendidCache                          from '../scripts/SplendidCache'      ;
import { DynamicLayout_Module }               from '../scripts/DynamicLayout'      ;
import { AuthenticatedMethod, LoginRedirect } from '../scripts/Login'              ;
// 4. Components and Views. 
import ErrorComponent                         from '../components/ErrorComponent'  ;
import MassUpdate                             from './MassUpdate'                  ;

interface IDynamicMassUpdateProps extends RouteComponentProps<any>
{
	MODULE_NAME      : string;
	onUpdateComplete?: Function;
	archiveView?     : boolean;
}

interface IDynamicMassUpdateState
{
	customView?      : any;
	error?           : any;
}

@observer
export default class DynamicMassUpdate extends React.Component<IDynamicMassUpdateProps, IDynamicMassUpdateState>
{
	private _isMounted = false;
	private updatePanel = React.createRef<MassUpdate>();

	constructor(props: IDynamicMassUpdateProps)
	{
		super(props);
		this.state = {};
	}

	componentDidCatch(error, info)
	{
		console.error((new Date()).toISOString() + ' ' + this.constructor.name + '.componentDidCatch', error, info);
	}

	async componentDidMount()
	{
		const { MODULE_NAME } = this.props;
		this._isMounted = true;
		try
		{
			let status = await AuthenticatedMethod(this.props, this.constructor.name + '.componentDidMount');
			if ( status == 1 )
			{
				// 04/19/2020 Paul.  We cannot call DynamicLayout_Module from within another DynamicLayout_Module. 
				let customView = await DynamicLayout_Module(MODULE_NAME, 'EditViews', 'MassUpdate');
				if ( this._isMounted )
				{
					this.setState({ customView });
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

	componentDidUpdate(prevProps: IDynamicMassUpdateProps)
	{
		const { MODULE_NAME } = this.props;
		//console.log((new Date()).toISOString() + ' ' + this.constructor.name + '.componentDidUpdate', MODULE_NAME);
	}

	componentWillUnmount()
	{
		this._isMounted = false;
	}

	public SelectionChanged = (value: any) =>
	{
		//console.log((new Date()).toISOString() + ' ' + this.constructor.name + '._onSelectionChanged: ' + MODULE_NAME, value);
		if ( this.updatePanel.current != null )
		{
			this.updatePanel.current.SelectionChanged(value);
		}
	}

	public Page_Command = async (sCommandName, sCommandArguments) =>
	{
		//console.log((new Date()).toISOString() + ' ' + this.constructor.name + '.Page_Command: ' + sCommandName, sCommandArguments);
		if ( this.updatePanel.current != null )
		{
			this.updatePanel.current.Page_Command(sCommandName, sCommandArguments);
		}
	}

	public render()
	{
		const { MODULE_NAME } = this.props;
		const { customView, error } = this.state;
		// 06/27/2019 Paul.  Reference obserable IsInitialized so that terminology update will cause refresh. 
		if ( error )
		{
			return <ErrorComponent error={error} />;
		}
		else if ( customView === undefined )
		{
			return (
			<div id={ this.constructor.name + '_spinner' } style={ {textAlign: 'center'} }>
				<FontAwesomeIcon icon="spinner" spin={ true } size="5x" />
			</div>);
		}
		else if ( SplendidCache.IsInitialized && customView )
		{
			return React.createElement(customView, { key: this.props.MODULE_NAME, ref: this.updatePanel, ...this.props });
		}
		else if ( SplendidCache.IsInitialized && Credentials.bIsAuthenticated )
		{
			return <MassUpdate key={ MODULE_NAME + '.MassUpdate' } ref={ this.updatePanel } { ...this.props } />;
		}
		else
		{
			return null;
		}
	}
}


