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
import { RouteComponentProps, withRouter } from 'react-router-dom';
import { Modal, Alert }                    from 'react-bootstrap';
import { observer } from 'mobx-react';
// 2. Store and Types. 
// 3. Scripts. 
import Credentials from '../scripts/Credentials';
import SplendidCache from '../scripts/SplendidCache';
import { Logout } from '../scripts/Login';
import L10n from '../scripts/L10n'
// 4. Components and Views. 

const icon = require('../assets/img/SplendidCRM_Icon.gif');

interface IUserDropdownProps extends RouteComponentProps<any>
{
}

type State =
{
}

@observer
class UserDropdown extends React.Component<IUserDropdownProps, State>
{
	constructor(props: IUserDropdownProps)
	{
		super(props);
		this.state =
		{
		};
	}
	
	private AdminMode = () =>
	{
		this.props.history.push(`/Reset/Administration`);
	}

	private UserMode = () =>
	{
		Credentials.SetADMIN_MODE(false);
		this.props.history.push('/Home');
	}

	public render()
	{
		if ( SplendidCache.IsInitialized )
		{
			const menuIconProps =
			{
				className: "fas fas-image",
				src: Credentials.sPICTURE || icon
			};
			
			let menuProps =
			{
				shouldFocusOnMount: true,
				items:
				[
				]
			};
			if ( Credentials.ADMIN_MODE )
			{
				menuProps.items[menuProps.items.length] = 
				{
					key: 'usernmode',
					name: L10n.Term('Home.LBL_LIST_FORM_TITLE'),
					onClick: () => { this.UserMode(); }
				};
			}
			if ( Credentials.bIS_ADMIN || Credentials.bIS_ADMIN_DELEGATE )
			{
				menuProps.items[menuProps.items.length] = 
				{
					key: 'adminmode',
					name: L10n.Term('.LBL_ADMIN'),
					onClick: () => { this.AdminMode(); }
				};
			}
			menuProps.items[menuProps.items.length] = 
			{
				key: 'logout',
				name: 'logout',
				onClick: Logout
			};

			return (
				<div>
					<div style={{ flexGrow: 1 }} />
				</div>
			);
		}
		return null;
	}
}

export default withRouter(UserDropdown);

