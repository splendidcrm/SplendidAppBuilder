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
import { RouteComponentProps, withRouter } from 'react-router-dom'                 ;
// 2. Store and Types. 
// 3. Scripts. 
import Credentials                         from '../../scripts/Credentials'        ;
import SplendidCache                       from '../../scripts/SplendidCache'      ;
// 4. Components and Views. 
import ArcticActions                       from './Actions'                        ;
import ArcticFavorites                     from './Favorites'                      ;
import ArcticLastViewed                    from './LastViewed'                     ;

interface ISideBarProps extends RouteComponentProps<any>
{
}

interface ISideBarState
{
	showLeftCol: boolean;
}

class ArcticSideBar extends React.Component<ISideBarProps, ISideBarState>
{
	constructor(props: ISideBarProps)
	{
		super(props);
		this.state =
		{
			showLeftCol: Credentials.showLeftCol,
		};
	}

	private toggleSideBar = (e) =>
	{
		Credentials.showLeftCol = !Credentials.showLeftCol;
		// 01/12/2020 Paul.  Save the state. 
		localStorage.setItem('showLeftCol', Credentials.showLeftCol.toString());
		this.setState({ showLeftCol: Credentials.showLeftCol });
	}

	public render()
	{
		const { showLeftCol } = this.state;
		//console.log((new Date()).toISOString() + ' ' + 'ArcticSideBar.render');
		// 08/08/2021 Paul.  height 100% is not working, but 100vh does work. 
		let themeUrl: string = Credentials.RemoteServer + 'App_Themes/' + SplendidCache.UserTheme + '/images/';
		return (
			<table cellPadding='0' cellSpacing='0' style={ {height: '100vh', paddingTop: '10px', paddingLeft: '10px'} }>
				<tr>
					{ showLeftCol
					? <td className='lastViewPanel' style={ {paddingTop: '6px', verticalAlign: 'top'} }>
						<ArcticActions    />
						<ArcticLastViewed />
						<ArcticFavorites  />
					</td>
					: null
					}
					<td style={ {width: '24px', paddingTop: '6px', verticalAlign: 'top'} }>
						<img onClick={ this.toggleSideBar} style={ {cursor: 'pointer', width: '24px', height: '24px'} } src={ themeUrl + (showLeftCol ? 'hide.gif' : 'show.gif') } />
					</td>
				</tr>
			</table>
		);
	}
}

export default withRouter(ArcticSideBar);

