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
import * as React                from 'react';
import { Tabs, Tab }             from 'react-bootstrap'         ;
// 2. Store and Types. 
// 3. Scripts. 
import Sql                       from '../scripts/Sql'          ;
import L10n                      from '../scripts/L10n'         ;
import Security                  from '../scripts/Security'     ;
import { EditView_GetTabList }   from '../scripts/EditView'     ;
import { DetailView_GetTabList } from '../scripts/DetailView'   ;
// 4. Components and Views. 

interface ILayoutTabsProps
{
	layout      : any[];
	onTabChange?: Function;
}

interface ILayoutTabsState
{
	VIEW_NAME  : string;
	tabsEnabled: boolean;
	arrTabs    : any[];
	activeKey  : number;
	defaultKey : number;
}

export default class LayoutTabs extends React.Component<ILayoutTabsProps, ILayoutTabsState>
{
	constructor(props: ILayoutTabsProps)
	{
		super(props);
		const { layout } = props;
		let sTheme: string  = Security.USER_THEME();
		let VIEW_NAME  : string  = '';
		let tabsEnabled: boolean = false;
		let arrTabs    : any[] = [];
		let activeKey  : number = 0;
		if ( sTheme == 'Pacific' )
		{
			if ( layout && layout.length > 0 )
			{
				if ( layout[0].EDIT_NAME !== undefined )
				{
					VIEW_NAME = Sql.ToString(layout[0].EDIT_NAME);
					arrTabs = EditView_GetTabList(layout);
					if ( arrTabs != null && arrTabs.length > 0 )
					{
						tabsEnabled = true;
					}
				}
				else if ( layout[0].DETAIL_NAME !== undefined )
				{
					VIEW_NAME = Sql.ToString(layout[0].DETAIL_NAME);
					arrTabs = DetailView_GetTabList(layout);
					if ( arrTabs != null && arrTabs.length > 0 )
					{
						tabsEnabled = true;
					}
				}
			}
			if ( tabsEnabled )
			{
				if ( arrTabs[0].nLayoutIndex != 0 )
				{
					let DATA_LABEL: string = L10n.Term('.LBL_LAYOUT_TAB_OVERVIEW');
					arrTabs.unshift({ nLayoutIndex: 0, DATA_LABEL });
				}
			}
		}
		this.state = 
		{
			VIEW_NAME  ,
			tabsEnabled,
			arrTabs    ,
			activeKey  ,
			defaultKey : activeKey,
		};
	}

	private _onLayoutTabChange = (key) =>
	{
		const { onTabChange } = this.props;
		//console.log((new Date()).toISOString() + ' ' + this.constructor.name + '._onLayoutTabChange', key);
		this.setState({ activeKey: key });
		if ( onTabChange != null )
		{
			try
			{
				onTabChange(key);
			}
			catch(error)
			{
				console.error((new Date()).toISOString() + ' ' + this.constructor.name + '._onLayoutTabChange', error);
			}
		}
		return false;
	}

	public render()
	{
		const { VIEW_NAME, tabsEnabled, arrTabs, activeKey, defaultKey } = this.state;
		if ( tabsEnabled )
		{
			return (
				<div id={ 'divLayoutTabs' + VIEW_NAME }>
					<Tabs activeKey={ activeKey } defaultActiveKey={ defaultKey } onSelect={ this._onLayoutTabChange }>
					{
						arrTabs.map((tab) =>
						{
							return (<Tab eventKey={ tab.nLayoutIndex } tabClassName='LayoutTab' title={ tab.DATA_LABEL } />);
						})
					}
					</Tabs>
				</div>
				);
		}
		else
		{
			return null;
		}
	}
}

