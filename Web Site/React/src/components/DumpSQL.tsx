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
import { Crm_Config }          from '../scripts/Crm'  ;
// 4. Components and Views. 

interface IDumpSQLProps
{
	SQL: string;
}

interface IDumpSQLState
{
	show_sql           : boolean;
	expand_sql         : boolean;
}

export default class DumpSQL extends React.Component<IDumpSQLProps, IDumpSQLState>
{
	constructor(props: IDumpSQLProps)
	{
		super(props);
		this.state =
		{
			show_sql          : Crm_Config.ToBoolean('show_sql'),
			expand_sql        : false,
		};
	}

	private onToggleSql = () =>
	{
		this.setState({ expand_sql: !this.state.expand_sql });
	}

	public render()
	{
		const { SQL } = this.props;
		const { show_sql, expand_sql } = this.state;
		// 04/19/2021 Paul.  Turn overflow off. 
		let cssSql: any = { height: '1em', cursor: 'pointer', marginBottom: 0, overflowX: 'hidden' };
		if ( expand_sql )
		{
			cssSql = { cursor: 'pointer', marginBottom: 0 };
		}
		// 04/14/2022 Paul.  Don't show if SQL is null, such as during new record creation. 
		if ( show_sql && SQL != null )
		{
			return (<pre onClick={ this.onToggleSql } style={ cssSql }>{ SQL }</pre>);
		}
		else
		{
			return null;
		}
	}
}

