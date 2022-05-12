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
import { RouteComponentProps, withRouter }                        from 'react-router-dom'              ;
import { FontAwesomeIcon }                                        from '@fortawesome/react-fontawesome';
import { Tabs, Tab }                                              from 'react-bootstrap'               ;
import { observer }                                               from 'mobx-react'                    ;
// 2. Store and Types. 
import DASHBOARDS_PANELS                                          from '../types/DASHBOARDS_PANELS'    ;
// 3. Scripts. 
import Sql                                                        from '../scripts/Sql'                ;
import L10n                                                       from '../scripts/L10n'               ;
import Credentials                                                from '../scripts/Credentials'        ;
import SplendidCache                                              from '../scripts/SplendidCache'      ;
import { Crm_Config }                                             from '../scripts/Crm'                ;
import { AuthenticatedMethod, LoginRedirect }                     from '../scripts/Login'              ;
import { Dashboards, Dashboards_LoadItem, Dashboards_LoadPanels } from '../scripts/Dashboard'          ;
// 4. Components and Views. 
import DashletView                                                from './DashletView'                 ;
import ErrorComponent                                             from '../components/ErrorComponent'  ;

const CATEGORY = 'Home';

interface IHomeViewProps extends RouteComponentProps<any>
{
	ID?           : string;
}

interface IHomeViewState
{
	ID            : string;
	DASHBOARD_NAME: string;
	dashboards    : Array<any>;
	panels        : DASHBOARDS_PANELS[];
	rows          : Array<Array<DASHBOARDS_PANELS>>;
	error?        : any;
}

@observer
class HomeView extends React.Component<IHomeViewProps, IHomeViewState>
{
	private _isMounted = false;

	constructor(props: IHomeViewProps)
	{
		super(props);
		let sCURRENT_DASHBOARD_ID = '';
		if ( !Sql.IsEmptyString(props.ID) )
		{
			sCURRENT_DASHBOARD_ID = Sql.ToString(props.ID);
			localStorage.setItem('ReactLast' + CATEGORY, sCURRENT_DASHBOARD_ID);
		}
		Credentials.SetViewMode('DashboardView');
		this.state =
		{
			ID: props.ID,
			DASHBOARD_NAME: '',
			dashboards: [],
			panels: null,
			rows: [],
		};
	}

	componentDidCatch(error, info)
	{
		console.error((new Date()).toISOString() + ' ' + this.constructor.name + '.componentDidCatch', error, info);
	}

	async componentDidMount()
	{
		const { ID } = this.state;
		//console.log((new Date()).toISOString() + ' ' + this.constructor.name + '.componentDidMount');
		this._isMounted = true;
		try
		{
			let status = await AuthenticatedMethod(this.props, this.constructor.name + '.componentDidMount');
			if ( status == 1 )
			{
				document.title = L10n.Term('Home.LBL_LIST_FORM_TITLE');
				// 04/26/2020 Paul.  Reset scroll every time we set the title. 
				window.scroll(0, 0);
				await this.preload(ID);
			}
			else
			{
				//console.log((new Date()).toISOString() + ' ' + this.constructor.name + '.componentDidMount() failed authentication');
				LoginRedirect(this.props.history, this.constructor.name + '.componentDidMount');
			}
		}
		catch(error)
		{
			console.error((new Date()).toISOString() + ' ' + this.constructor.name + '.componentDidMount', error);
			this.setState({ error });
		}
	}

	async componentDidUpdate(prevProps: IHomeViewProps)
	{
		// 04/28/2019 Paul.  Include pathname in filter to prevent double-bounce when state changes. 
		if ( this.props.location.pathname != prevProps.location.pathname )
		{
			// 04/26/2019 Paul.  Bounce through ResetView so that layout gets completely reloaded. 
			//console.log((new Date()).toISOString() + ' ' + this.constructor.name + '.componentDidUpdate Reset', this.props.location,  prevProps.location);
			// 11/20/2019 Paul.  Include search parameters. 
			this.props.history.push('/Reset' + this.props.location.pathname + this.props.location.search);
		}
	}

	componentWillUnmount()
	{
		this._isMounted = false;
	}

	private preload = async (sID: string) =>
	{
		const { DASHBOARD_NAME } = this.state;
		let dashboards = await Dashboards('Home');
		if ( this._isMounted && dashboards != null )
		{
			this.setState({ dashboards: dashboards });
		}
		if ( Sql.IsEmptyString(sID) )
		{
			if ( dashboards != null )
			{
				sID = Sql.ToGuid(localStorage.getItem('ReactLast' + CATEGORY));
				if ( Sql.IsEmptyString(sID) && dashboards.length > 0 )
				{
					sID = Sql.ToString(dashboards[0]['ID']);
					localStorage.setItem('ReactLast' + CATEGORY, sID);
				}
			}
			if ( this._isMounted && !Sql.IsEmptyString(sID) )
			{
				this.setState({ ID: sID });
			}
		}
		if ( Sql.IsEmptyString(DASHBOARD_NAME) && !Sql.IsEmptyString(sID) )
		{
			try
			{
				let item = await Dashboards_LoadItem(sID);
				if ( item != null )
				{
					// 05/31/2019 Paul.  The component may be unmounted by the time the custom view is generated. 
					if ( this._isMounted )
					{
						this.setState({ DASHBOARD_NAME: item.NAME });
					}
					await this.Load(sID);
				}
			}
			catch(error)
			{
				console.error((new Date()).toISOString() + ' ' + this.constructor.name + '.preload', error);
			}
		}
		// 10/20/2020 Paul.  Make sure that the ID is valid. 
		else if ( !Sql.IsEmptyString(sID) )
		{
			await this.Load(sID);
		}
	}

	private Load = async (sID: string) =>
	{
		try
		{
			let results = await Dashboards_LoadPanels(sID);
			//console.log((new Date()).toISOString() + ' ' + this.constructor.name + '.Load', results);
			let arrDASHBOARDS_PANELS: DASHBOARDS_PANELS[] = results;
			let rows = [];
			if (arrDASHBOARDS_PANELS != null && arrDASHBOARDS_PANELS.length > 0)
			{
				for (let nLayoutIndex = 0; nLayoutIndex < arrDASHBOARDS_PANELS.length; nLayoutIndex++)
				{
					let lay = arrDASHBOARDS_PANELS[nLayoutIndex];
					if ( Sql.IsEmptyGuid(lay.DASHBOARD_APP_ID) )
						lay.PANEL_TYPE = 'Blank';
					else
						lay.PANEL_TYPE = 'Panel';
					if ( !rows[lay.ROW_INDEX] )
					{
						rows[lay.ROW_INDEX] = [];
					}
					// 02/22/2020 Paul.  Panels are returned in the corret order, so just append. 
					rows[lay.ROW_INDEX].push(lay);
				}
			}
			//console.log((new Date()).toISOString() + ' ' + this.constructor.name + '.Load', rows);
			if ( this._isMounted )
			{
				this.setState({ panels: arrDASHBOARDS_PANELS, rows: rows });
			}
		}
		catch(error)
		{
			console.error((new Date()).toISOString() + ' ' + this.constructor.name + '.load', error);
		}
	}

	private _onTabChange = (key) =>
	{
		let arrKey = key.split('/');
		let sCURRENT_DASHBOARD_ID = arrKey[arrKey.length - 1];
		if ( sCURRENT_DASHBOARD_ID.length == 36 )
		{
			if ( this._isMounted )
			{
				localStorage.setItem('ReactLast' + CATEGORY, sCURRENT_DASHBOARD_ID);
			}
		}
		this.props.history.push(key);
	}

	public render()
	{
		const { ID, dashboards, panels, rows, error } = this.state;
		// 06/16/2019 Paul.  Watch for cache changes. 
		SplendidCache.NAV_MENU_CHANGE;
		// 10/26/2020 Paul.  We are having an issue with the dashboard loading when not ready. 
		if ( SplendidCache.IsInitialized  )
		{
			if ( error )
			{
				return <ErrorComponent error={ error } />;
			}
			else
			{
				// 01/17/2020 Paul.  Some customers want to disable end-user editing of the dashboards. 
				// 06/09/2021 Paul.  Add hover titles to icons. 
				let bDisableEdit: boolean = Crm_Config.ToBoolean('DashboardView.DisableHomeEdit');
				let pnlDashboardTabs = null;
				if ( SplendidCache.UserTheme == 'Pacific' )
				{
					let tabs = [];
					dashboards.map((dashboard) =>
					{
						let sClassName: string = 'DashboardTabButton';
						if ( dashboard.ID == ID )
							sClassName += ' DashboardTabButtonActive';
						let tab = <div className={ sClassName }>
							<button onClick={ (e) => this.props.history.push('/Reset/Home/' + dashboard.ID) }>{ dashboard.NAME }</button>
							{ !bDisableEdit
							? <button onClick={ (e) => this.props.history.push('/Reset/Home/DashboardEdit/' + dashboard.ID) }>
								<FontAwesomeIcon icon='edit' size='lg' title={ L10n.Term('Dashboard.LBL_DASHBOARD_TAB_EDIT') } />
							</button>
							: null
							}
						</div>;
						tabs.push(tab);
					});
					if ( !bDisableEdit )
					{
						tabs.push(<div className='DashboardTabButton'><button id="pnlDashboardCreate" onClick={ (e) => this.props.history.push('/Reset/Home/DashboardEdit/') }><FontAwesomeIcon icon='asterisk' size='lg' title={ L10n.Term('Dashboard.LBL_DASHBOARD_TAB_CREATE') } /></button></div>);
					}
					pnlDashboardTabs = <div style={ {display: 'flex', flexDirection: 'row'} }>{ tabs }</div>;
				}
				else
				{
					let tabs = [];
					dashboards.map((dashboard) =>
					{
						let edit = <FontAwesomeIcon icon="edit" size="lg" title={ L10n.Term('Dashboard.LBL_DASHBOARD_TAB_EDIT') } />;
						tabs.push(<Tab key={ dashboard.ID + '_view' } eventKey={ '/Reset/Home/' + dashboard.ID } title={ dashboard.NAME }></Tab>);
						if ( !bDisableEdit )
						{
							tabs.push(<Tab key={ dashboard.ID + '_edit' } eventKey={ '/Reset/Home/DashboardEdit/' + dashboard.ID } title={ edit }></Tab>);
						}
					});
					if ( !bDisableEdit )
					{
						let asterisk = <FontAwesomeIcon icon="asterisk" size="lg" title={ L10n.Term('Dashboard.LBL_DASHBOARD_TAB_CREATE') } />;
						tabs.push(<Tab key="pnlDashboardCreate" id="pnlDashboardCreate"  eventKey="/Reset/Home/DashboardEdit/"  title={ asterisk }></Tab>);
					}
					pnlDashboardTabs = 
						<Tabs key="pnlDashboardTabs" id="pnlDashboardTabs" activeKey={ '/Reset/Home/' + ID } onSelect={ this._onTabChange }>
							{ tabs }
						</Tabs>;
				}
				return (
					<div>
						{ pnlDashboardTabs }
						<div style={{ display: 'flex', flexDirection: 'column' }}>
							{ rows.map((row, index) => (
								<div style={{ display: 'flex' }} key={'row' + index}>
									{ row.map((panel) =>
									{
										let dashletName = panel.SCRIPT_URL.slice(panel.SCRIPT_URL.lastIndexOf('/') + 1, panel.SCRIPT_URL.lastIndexOf('.'));
										return (
												<DashletView
													key={ panel.ID }
													ID={ panel.ID }
													DASHLET_NAME={ dashletName }
													TITLE={ panel.TITLE }
													SETTINGS_EDITVIEW={ panel.SETTINGS_EDITVIEW }
													DEFAULT_SETTINGS={ panel.DEFAULT_SETTINGS }
													COLUMN_WIDTH={ panel.COLUMN_WIDTH }
												/>
										);
									})}
								</div>
							))}
						</div>
					</div>
				);
			}
		}
		else
		{
			return null;
		}
	}
}

export default withRouter(HomeView);
