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
import Sql from '../scripts/Sql';
import { escapeHTML } from '../scripts/utility';
// 4. Components and Views. 

interface ITagsProps
{
	row   : any;
	layout: any;
}

class Tags extends React.PureComponent<ITagsProps>
{
	public render()
	{
		const { layout, row } = this.props;
		let DATA_FIELD = Sql.ToString(layout.DATA_FIELD);
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
			let DATA_VALUE = '';
			if ( row )
			{
				DATA_VALUE = '';
				let sDATA = row[DATA_FIELD];
				if ( !Sql.IsEmptyString(sDATA) )
				{
					let divTagsChildren = [];
					let divTags = React.createElement('div', { }, divTagsChildren);
					let arrTAGS = sDATA.split(',');
					for ( let iTag = 0; iTag < arrTAGS.length; iTag++ )
					{
						// 11/03/2018 Paul.  Keys only need to be unique within siblings.  Not globally. 
						// https://reactjs.org/docs/lists-and-keys.html#keys
						let spnTag = React.createElement('span', { key: arrTAGS[iTag], className: 'Tags' }, escapeHTML(arrTAGS[iTag]));
						divTagsChildren.push(spnTag);
					}
					return divTags;
				}
			}
			return (<div>{ DATA_VALUE }</div>);
		}
	}
}

export default Tags;

