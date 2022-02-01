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
import React, { useState } from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import posed from 'react-pose';

const Toggle = posed.i(
{
	pressable: true,
	open:
	{
		rotate: '180deg',
	},
	closed:
	{
		rotate: '0deg'
	}
});

const Content = posed.div(
{
	open:
	{
		height: '100%'
	},
	closed:
	{
		height: 0
	}
});

interface ICollapsableProps
{
	name        : string;
	onToggle?   : (open: boolean) => void;
	initialOpen?: boolean;
}

interface ICollapsableState
{
	open: boolean;
}

export default class Collapsable extends React.Component<ICollapsableProps, ICollapsableState>
{
	constructor(props: ICollapsableProps)
	{
		super(props);
		let open = false;
		if ( props.initialOpen !== null && props.initialOpen !== undefined )
		{
			open = props.initialOpen;
		}
		this.state =
		{
			open
		};
	}

	private toggle = () =>
	{
		this.setState({ open: !this.state.open }, () =>
		{
			if (this.props.onToggle)
			{
				this.props.onToggle(this.state.open);
			}
		});
	}
	public render()
	{
		const { open } = this.state;
		// 06/29/2019 Paul.  Only include the children when open so that internal query is not performed unless open. 
		return (
			<React.Fragment>
				<div className='h3Row' style={ {fontSize: '1.5em'} }>
					<h3>
						<Toggle onClick={ this.toggle } pose={ open ? 'open' : 'closed' } style={ {marginRight: '0.5em', cursor: 'pointer'} }>
							<FontAwesomeIcon icon={ open ? 'chevron-up' : 'chevron-down' } />
						</Toggle>
						{this.props.name}
					</h3>
				</div>
				<Content pose={ open ? 'open' : 'closed' } style={ {overflow: (open ? 'visible' : 'hidden')} }>
					{ open
					? this.props.children
					: null
					}
				</Content>
			</React.Fragment>
		);
	}
}


