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
import { RouteComponentProps, withRouter }            from 'react-router-dom'              ;
import { FontAwesomeIcon }                            from '@fortawesome/react-fontawesome';
import { observer }                                   from 'mobx-react'                    ;
// 2. Store and Types. 
// 3. Scripts. 
import SplendidCache                                  from '../scripts/SplendidCache'      ;
import Credentials                                    from '../scripts/Credentials'        ;
import { DynamicLayout_Module }                       from '../scripts/DynamicLayout'      ;
import { AuthenticatedMethod, LoginRedirect }         from '../scripts/Login'              ;
import { DetailView_LoadItem, DetailView_LoadLayout } from '../scripts/DetailView'         ;
import { EditView_LoadItem, EditView_LoadLayout }     from '../scripts/EditView'           ;
import { ListView_LoadLayout }                        from '../scripts/ListView'           ;
// 4. Components and Views. 
import ErrorComponent                                 from '../components/ErrorComponent'  ;
import DetailView                                     from './DetailView'                  ;
import EditView                                       from './EditView'                    ;
import ListView                                       from './ListView'                    ;

interface IDynamicLayoutViewProps extends RouteComponentProps<any>
{
	MODULE_NAME        : string;
	ID                 : string;
	VIEW_NAME          : string;  // The view name does not include the module. 
	// 01/26/2021 Paul.  Now that the search view can be customized, we need to make sure it does not set the mode. 
	isSearchView?      : boolean;
	isUpdatePanel?     : boolean;
	isQuickCreate?     : boolean;
	// 04/10/2021 Paul.  Create framework to allow pre-compile of all modules. 
	isPrecompile?       : boolean;
	onComponentComplete?: (MODULE_NAME, RELATED_MODULE, LAYOUT_NAME, data) => void;
}

interface IDynamicLayoutViewState
{
	LAYOUT_NAME: string;
	customView?: any;
	error?     : any;
}

@observer
class DynamicLayoutView extends React.Component<IDynamicLayoutViewProps, IDynamicLayoutViewState>
{
	private _isMounted = false;

	constructor(props: IDynamicLayoutViewProps)
	{
		super(props);
		this.state = 
		{
			LAYOUT_NAME: (props.MODULE_NAME + '.' + props.VIEW_NAME)
		};
	}

	componentDidCatch(error, info)
	{
		console.error((new Date()).toISOString() + ' ' + this.constructor.name + '.componentDidCatch', error, info);
	}

	async componentDidMount()
	{
		const { MODULE_NAME, ID, VIEW_NAME } = this.props;
		const { LAYOUT_NAME } = this.state;
		//console.log((new Date()).toISOString() + ' ' + this.constructor.name + '.componentDidMount ' + this.props.MODULE_NAME + ' ' + this.props.ID);
		this._isMounted = true;
		try
		{
			let status = await AuthenticatedMethod(this.props, this.constructor.name + '.componentDidMount');
			if ( status == 1 )
			{
				let customView = await DynamicLayout_Module(MODULE_NAME, null, VIEW_NAME);
				if ( customView == null )
				{
					// 08/16/2019 Paul.  Need to check for a valid ID as ArchiveView exists for ListViews and DetailViews. 
					// We should try not to do that again.  Unique view names make things easier. 
					if ( ID !== undefined )
					{
						// 02/02/2020 Paul.  Ignore missing during DynamicLayout. 
						let layout = DetailView_LoadLayout(LAYOUT_NAME, true);
						if ( layout != null )
						{
							customView = DetailView;
								Credentials.SetViewMode('DetailView');
						}
						else
						{
							// 02/02/2020 Paul.  Ignore missing during DynamicLayout. 
							layout = EditView_LoadLayout(LAYOUT_NAME, true);
							if ( layout != null )
							{
								customView = EditView;
								// 01/26/2021 Paul.  Now that the search view can be customized, we need to make sure it does not set the mode. 
								if ( !this.props.isSearchView && !this.props.isUpdatePanel && !this.props.isQuickCreate )
								{
									Credentials.SetViewMode('EditView');
								}
							}
						}
					}
					else
					{
						// 02/02/2020 Paul.  Ignore missing during DynamicLayout. 
						let layout = EditView_LoadLayout(LAYOUT_NAME, true);
						if ( layout != null )
						{
							customView = EditView;
							// 01/26/2021 Paul.  Now that the search view can be customized, we need to make sure it does not set the mode. 
							if ( !this.props.isSearchView && !this.props.isUpdatePanel && !this.props.isQuickCreate )
							{
								Credentials.SetViewMode('EditView');
							}
						}
						else
						{
							// 02/02/2020 Paul.  Ignore missing during DynamicLayout. 
							layout = ListView_LoadLayout(LAYOUT_NAME, true);
							if ( layout != null )
							{
								customView = ListView;
								Credentials.SetViewMode('ListView');
							}
						}
					}
				}
				// 05/26/2019 Paul.  The component may be unmounted by the time the custom view is generated. 
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

	componentDidUpdate(prevProps: IDynamicLayoutViewProps)
	{
		//console.log((new Date()).toISOString() + ' ' + this.constructor.name + '.componentDidUpdate', prevProps);
		// 01/19/2021 Paul.  A user may click the browser back button from one detail view to another.  Detect and reset so that the correct custom view is loaded. 
		if ( this.props.location.pathname != prevProps.location.pathname )
		{
			this.props.history.push('/Reset' + this.props.location.pathname + this.props.location.search);
		}
	}

	componentWillUnmount()
	{
		this._isMounted = false;
	}

	public render()
	{
		const { MODULE_NAME } = this.props;
		const { LAYOUT_NAME, customView, error } = this.state;
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
			return React.createElement(customView, { LAYOUT_NAME, ...this.props });
		}
		else if ( SplendidCache.IsInitialized && Credentials.bIsAuthenticated )
		{
			return <span>Layout not found for { LAYOUT_NAME }</span>;
		}
		else
		{
			return null;
		}
	}
}

export default withRouter(DynamicLayoutView);

