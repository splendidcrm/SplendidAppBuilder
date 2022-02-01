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
import { Alert } from 'react-bootstrap';
// 2. Store and Types. 
// 3. Scripts. 
// 4. Components and Views. 
interface IErrorComponentProps
{
	error?: any;
}

class ErrorComponent extends React.Component<IErrorComponentProps>
{
	constructor(props: IErrorComponentProps)
	{
		super(props);
	}

	public render()
	{
		const { error } = this.props;
		if ( error != undefined && error != null )
		{
			//console.error((new Date()).toISOString() + ' ' + this.constructor.name + '.render', error);
			if (error)
			{
				let sError = error;
				if ( error.message !== undefined )
				{
					sError = error.message;
				}
				else if ( typeof(error) == 'string' )
				{
					sError = error;
				}
				else if ( typeof(error) == 'object' )
				{
					sError = JSON.stringify(error);
				}
				return <Alert variant='danger'>{sError}</Alert>;
			}
			return null;
		}
		else
		{
			return null;
		}
	}
}

export default ErrorComponent;

