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
import { Modal }                              from 'react-bootstrap'             ;
import { FontAwesomeIcon }                    from '@fortawesome/react-fontawesome';
// 2. Store and Types. 
// 3. Scripts. 
import Sql                                    from '../scripts/Sql'              ;
import L10n                                   from '../scripts/L10n'             ;
import Credentials                            from '../scripts/Credentials'      ;
import SplendidCache                          from '../scripts/SplendidCache'    ;
import { AuthenticatedMethod, LoginRedirect } from '../scripts/Login'            ;
// 4. Components and Views. 
import ErrorComponent                         from '../components/ErrorComponent';
import SplendidGrid                           from '../components/SplendidGrid'  ;
import SearchView                             from '../views/SearchView'         ;

interface ITemplatePopupViewProps extends RouteComponentProps<any>
{
	callback           : Function;
	isOpen             : boolean;
	isPrecompile?       : boolean;
	onComponentComplete?: (MODULE_NAME, RELATED_MODULE, LAYOUT_NAME, vwMain) => void;
}

interface ITemplatePopupViewState
{
	defaultSearch      : any;
	error?             : any;
}

class TemplatePopupView extends React.Component<ITemplatePopupViewProps, ITemplatePopupViewState>
{
	private _isMounted   = false;
	private searchView     = React.createRef<SearchView>();
	private splendidGrid   = React.createRef<SplendidGrid>();

	constructor(props: ITemplatePopupViewProps)
	{
		super(props);
		let defaultSearch: any = null;
		this.state =
		{
			defaultSearch     ,
		};
	}

	async componentDidMount()
	{
		this._isMounted = true;
		try
		{
			let status = await AuthenticatedMethod(this.props, this.constructor.name + '.componentDidMount');
			if ( status == 1 )
			{
				if ( Credentials.ADMIN_MODE )
				{
					Credentials.SetADMIN_MODE(false);
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

	componentDidCatch(error, info)
	{
		console.error((new Date()).toISOString() + ' ' + this.constructor.name + '.componentDidCatch', error, info);
	}

	componentWillUnmount()
	{
		this._isMounted = false;
	}

	private _onComponentComplete = (MODULE_NAME, RELATED_MODULE, LAYOUT_NAME, data): void => 
	{
		const { error } = this.state;
		//console.log((new Date()).toISOString() + ' ' + this.constructor.name + '._onComponentComplete ' + LAYOUT_NAME, data);
		if ( this.props.onComponentComplete )
		{
			if ( error == null )
			{
				let vwMain = null;
				this.props.onComponentComplete(MODULE_NAME, RELATED_MODULE, LAYOUT_NAME, data);
			}
		}
	}

	private _onClose = () =>
	{
		const { callback } = this.props;
		callback({ Action: 'Close' });
	}

	private _onHyperLinkCallback = (MODULE_NAME: string, ID: string, NAME: string, URL: string, row?: any) =>
	{
		const { callback } = this.props;
		//console.log((new Date()).toISOString() + ' ' + this.constructor.name + '._onHyperLinkCallback', ID, NAME, URL);
		callback({ Action: 'SingleSelect', ID, NAME });
	}

	private _onSearchViewCallback = (sFILTER: string, row: any, oSORT?: any) =>
	{
		//console.log((new Date()).toISOString() + ' ' + this.constructor.name + '._onSearchViewCallback');
		if ( this.splendidGrid.current != null )
		{
			this.splendidGrid.current.Search(sFILTER, row, oSORT);
		}
	}

	private _onGridLayoutLoaded = () =>
	{
		//console.log((new Date()).toISOString() + ' ' + this.constructor.name + '._onGridLayoutLoaded');
		if ( this.searchView.current != null )
		{
			this.searchView.current.SubmitSearch();
		}
	}

	private Load = async (sMODULE_NAME, sSORT_FIELD, sSORT_DIRECTION, sSELECT, sFILTER, rowSEARCH_VALUES, nTOP, nSKIP, bADMIN_MODE?, archiveView?) =>
	{
		//console.log((new Date()).toISOString() + ' ' + this.constructor.name + '.Load');
		let d: any = {};
		d.results = [];
		d.results.push({ MODULE_NAME: 'Accounts'      });
		d.results.push({ MODULE_NAME: 'Contacts'      });
		d.results.push({ MODULE_NAME: 'Leads'         });
		d.results.push({ MODULE_NAME: 'Opportunities' });
		return d;
	}

	public renderBody = () =>
	{
		const { isOpen, callback } = this.props;
		const { defaultSearch, error } = this.state;
		let EDIT_NAME: string = 'ModuleBuilder.SearchPopup';
		let GRID_NAME: string = 'ModuleBuilder.PopupView'  ;
		return (<React.Fragment>
					<ErrorComponent error={error} />
					<SearchView
						key={ EDIT_NAME }
						EDIT_NAME={ EDIT_NAME }
						IsPopupSearch={ true }
						rowDefaultSearch={ defaultSearch }
						cbSearch={ this._onSearchViewCallback }
						history={ this.props.history }
						location={ this.props.location }
						match={ this.props.match }
						ref={ this.searchView }
					/>
					<div>
						<button key={ 'btnCancel_' + EDIT_NAME }
							className='button'
							onClick={ this._onClose }
							style={ {marginBottom: '.2em', marginLeft: '.5em'} }>
							{ L10n.Term('.LBL_CLOSE_BUTTON_LABEL') }
						</button>
					</div>
					<SplendidGrid
						onLayoutLoaded={ this._onGridLayoutLoaded }
						MODULE_NAME="ModuleBuilder"
						GRID_NAME={ GRID_NAME }
						ADMIN_MODE={ false }
						hyperLinkCallback={ this._onHyperLinkCallback }
						isPopupView={ true }
						deferLoad={ true }
						enableSelection={ false }
						onComponentComplete={ this._onComponentComplete }
						scrollable
						cbCustomLoad={ this.Load }
						history={ this.props.history }
						location={ this.props.location }
						match={ this.props.match }
						ref={ this.splendidGrid }
					/>
		</React.Fragment>);
	}

	public render()
	{
		const { isOpen, isPrecompile } = this.props;
		if ( SplendidCache.IsInitialized )
		{
			// 04/12/2021 Paul.  Move the rendering to a separate function so that we can skip the modal during Precompile. 
			if ( isPrecompile )
			{
				return this.renderBody();
			}
			else
			{
				return (
					<Modal show={ isOpen } onHide={ this._onClose }>
						<Modal.Body style={{ minHeight: '80vh', minWidth: '80vw' }}>
							{ this.renderBody() }
						</Modal.Body>
						<Modal.Footer>
							<button className='button' onClick={ this._onClose }>{ L10n.Term('.LBL_CLOSE_BUTTON_LABEL') }</button>
						</Modal.Footer>
					</Modal>
				);
			}
		}
		else
		{
			return (
			<div id={ this.constructor.name + '_spinner' } style={ {textAlign: 'center'} }>
				<FontAwesomeIcon icon="spinner" spin={ true } size="5x" />
			</div>);
		}
	}
}

export default TemplatePopupView;

