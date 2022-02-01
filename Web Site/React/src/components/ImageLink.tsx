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
import { observer } from 'mobx-react';
// 2. Store and Types. 
// 3. Scripts. 
import Credentials from '../scripts/Credentials';
import { Crm_Modules } from '../scripts/Crm';
// 4. Components and Views. 

interface IImageLinkProps extends RouteComponentProps<any>
{
	ID  : string;
}

interface IImageLinkState
{
	NAME: string;
}

@observer
class ImageLink extends React.Component<IImageLinkProps, IImageLinkState>
{
	constructor(props: IImageLinkProps)
	{
		super(props);
		this.state =
		{
			NAME: ''
		}
	}

	async componentDidMount()
	{
		const { ID } = this.props;
		try
		{
			let value = await Crm_Modules.ItemName('Images', ID);
			this.setState({ NAME: value });
		}
		catch(error)
		{
			// 05/20/2018 Paul.  When an error is encountered, we display the error in the name. 
			console.error((new Date()).toISOString() + ' ' + this.constructor.name + '.componentDidMount', error);
			this.setState({ NAME: error });
		}
	}

	public render()
	{
		const { ID } = this.props;
		const { NAME } = this.state;
		// 06/23/2019 Paul.  The server should always end with a slash. 
		let sURL = Credentials.RemoteServer + 'Images/Image.aspx?ID=' + ID;
		return (
			<div>
				<a href={sURL}>{NAME}</a>
			</div>
		)
	}
}

export default withRouter(ImageLink);

