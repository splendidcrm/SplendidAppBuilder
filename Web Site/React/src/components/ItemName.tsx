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
import { Crm_Modules } from '../scripts/Crm';
import Sql from '../scripts/Sql';
// 4. Components and Views. 

interface IItemNameProps extends RouteComponentProps<any>
{
	MODULE_NAME : string;
	ID          : string;
	DATA_FORMAT?: string;
};

interface IItemNameState
{
	NAME        : string;
}
@observer
class ItemName extends React.Component<IItemNameProps, IItemNameState>
{
	constructor(props: IItemNameProps)
	{
		super(props);
		this.state =
		{
			NAME: props.MODULE_NAME + ':' + props.ID
		};
	}

	async componentDidMount()
	{
		const { MODULE_NAME, ID } = this.props;
		try
		{
			let value = await Crm_Modules.ItemName(MODULE_NAME, ID);
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
		const { ID, DATA_FORMAT } = this.props;
		const { NAME } = this.state;
		var sDISPLAY_NAME = NAME;
		if (!Sql.IsEmptyString(DATA_FORMAT))
			sDISPLAY_NAME = DATA_FORMAT.replace('{0}', NAME);
		return (
			<span>{sDISPLAY_NAME}</span>
		)
	}
}

export default withRouter(ItemName);

