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
// 2. Store and Types. 
import { IEditComponentProps, EditComponent } from '../types/EditComponent';
// 3. Scripts. 
import Sql                                    from '../scripts/Sql'        ;
import L10n                                   from '../scripts/L10n'       ;
// 4. Components and Views. 

interface IHeaderState
{
	ID               : string;
	FIELD_INDEX      : number;
	DATA_LABEL       : string;
	DATA_VALUE       : string;
	CSS_CLASS?       : string;
}

export default class Header extends React.PureComponent<IEditComponentProps, IHeaderState>
{
	public get data(): any
	{
		return null;
	}

	public validate(): boolean
	{
		return true;
	}

	public updateDependancy(PARENT_FIELD: string, DATA_VALUE: any, PROPERTY_NAME?: string, item?: any): void
	{
		if ( PROPERTY_NAME == 'class' )
		{
			this.setState({ CSS_CLASS: DATA_VALUE });
		}
	}

	public clear(): void
	{
	}

	constructor(props: IEditComponentProps)
	{
		super(props);
		let FIELD_INDEX      : number = 0;
		let DATA_LABEL       : string = '';
		let DATA_VALUE       : string = '';

		let ID: string = null;
		try
		{
			const { baseId, layout, row, onChanged } = this.props;
			if ( layout != null )
			{
				FIELD_INDEX       = Sql.ToInteger(layout.FIELD_INDEX);
				DATA_LABEL        = Sql.ToString (layout.DATA_LABEL );
				ID = baseId + '_' + layout.FIELD_TYPE + '_' + layout.FIELD_INDEX;

				if ( DATA_LABEL.indexOf('.') >= 0 )
				{
					DATA_VALUE = L10n.Term(DATA_LABEL);
				}
				else if ( !Sql.IsEmptyString(DATA_LABEL) )
				{
					// 06/21/2015 Paul.  Label can contain raw text. 
					DATA_VALUE = DATA_LABEL;
				}
				// 05/28/2018 Paul.  HTML entities will not get escaped in React.  Do a couple of them manually. 
				DATA_VALUE = Sql.ReplaceEntities(DATA_VALUE);
			}
		}
		catch(error)
		{
			console.error((new Date()).toISOString() + ' ' + this.constructor.name + '.constructor', error);
		}
		this.state =
		{
			ID         ,
			FIELD_INDEX,
			DATA_LABEL ,
			DATA_VALUE ,
		};
	}

	shouldComponentUpdate(nextProps: IEditComponentProps, nextState: IHeaderState)
	{
		if ( nextState.CSS_CLASS != this.state.CSS_CLASS )
		{
			//console.log((new Date()).toISOString() + ' ' + this.constructor.name + '.shouldComponentUpdate ' + DATA_FIELD, CSS_CLASS, nextProps, nextState);
			return true;
		}
		return false;
	}

	public render()
	{
		const { baseId, layout, row, onChanged } = this.props;
		const { ID, FIELD_INDEX, DATA_LABEL, DATA_VALUE, CSS_CLASS } = this.state;
		//console.log((new Date()).toISOString() + ' ' + this.constructor.name + '.render ' + DATA_FIELD);
		try
		{
			if ( layout == null )
			{
				return (<span>layout is null</span>);
			}
			else if (Sql.IsEmptyString(DATA_LABEL))
			{
				return (<h4>DATA_LABEL is empty for FIELD_INDEX { FIELD_INDEX }</h4>);
			}
			else
			{
				return (
					<h4 id={ ID } key={ ID } className={ CSS_CLASS }>{ DATA_VALUE }</h4>
				);
			}
		}
		catch(error)
		{
			console.error((new Date()).toISOString() + ' ' + this.constructor.name + '.render', error);
			return (<span>{ error.message }</span>);
		}
	}
}


