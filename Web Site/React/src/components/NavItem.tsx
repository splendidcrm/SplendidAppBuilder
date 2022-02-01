/*
 * Copyright (C) 2005-2022 SplendidCRM Software, Inc. 
 * MIT License
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
 * files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
 * modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
 * is furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 */

import * as React from 'react';
import { NavDropdown, NavDropdownProps, Dropdown } from 'react-bootstrap';
import { ReplaceProps } from 'react-bootstrap/helpers';

interface INavItemState
{
	show: boolean;
}

class NavItem extends React.Component<ReplaceProps<typeof Dropdown, NavDropdownProps>, INavItemState>
{
	state =
	{
		show: false
	};

	render()
	{
		const { show } = this.state;
		const {  onMouseEnter, onMouseLeave  } = this.props;
		return (
			<NavDropdown {...this.props}
				show={show}
				onMouseEnter={ () => this.setState({ show: true  }) }
				onMouseLeave={ () => this.setState({ show: false }) }
				>
				{ this.props.children }
			</NavDropdown>
		)
	}
}

export default NavItem;

