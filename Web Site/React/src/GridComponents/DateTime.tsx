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
// 3. Scripts. 
import Sql              from '../scripts/Sql';
import Security         from '../scripts/Security';
import { FromJsonDate } from '../scripts/Formatting';
// 4. Components and Views. 

interface IDateTimeProps
{
	row     : any;
	layout  : any;
	dateOnly: boolean;
}

class DateTime extends React.PureComponent<IDateTimeProps>
{
	public render()
	{
		const { layout, row, dateOnly } = this.props;
		let DATA_VALUE = '';
		let DATA_FIELD = Sql.ToString(layout.DATA_FIELD);
		if ( dateOnly )
		{
			//console.log((new Date()).toISOString() + ' ' + this.constructor.name + '.Render Date ' + DATA_FIELD, row);
		}
		else
		{
			//console.log((new Date()).toISOString() + ' ' + this.constructor.name + '.Render DateTime ' + DATA_FIELD, row);
		}
		if ( layout == null )
		{
			return (<div>layout prop is null</div>);
		}
		else if ( Sql.IsEmptyString(DATA_FIELD) )
		{
			return (<div>DATA_FIELD is empty for FIELD_INDEX { layout.FIELD_INDEX }</div>);
		}
		else
		{
			let sVALUE = (row ? Sql.ToString(row[DATA_FIELD]) : '');
			if ( row )
			{
				DATA_VALUE = row[DATA_FIELD];
				if ( dateOnly )
				{
					DATA_VALUE = FromJsonDate(DATA_VALUE, Security.USER_DATE_FORMAT());
				}
				else
				{
					DATA_VALUE = FromJsonDate(DATA_VALUE, Security.USER_DATE_FORMAT() + ' ' + Security.USER_TIME_FORMAT());
				}
			}
			return (<div>{ DATA_VALUE }</div>);
		}
	}
}

export default DateTime;

