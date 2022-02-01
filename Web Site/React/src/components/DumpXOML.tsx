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
import Sql        from '../scripts/Sql';
// 4. Components and Views. 

interface IDumpXOMLProps
{
	XOML        : string;
	default_xoml: boolean;
}

interface IDumpXOMLState
{
	expand_xmol: boolean;
}

export default class DumpXOML extends React.Component<IDumpXOMLProps, IDumpXOMLState>
{
	constructor(props: IDumpXOMLProps)
	{
		super(props);
		this.state =
		{
			expand_xmol: props.default_xoml,
		};
	}

	private onToggleXoml = () =>
	{
		this.setState({ expand_xmol: !this.state.expand_xmol });
	}

	private HtmlEncode = (s) =>
	{
		return s.replace(/&/g, '&amp;')
		.replace(/</g, '&lt;')
		.replace(/>/g, '&gt;')
		.replace(/'/g, '&#39;')
		.replace(/"/g, '&#34;');
	}

	private XmolEncode = (sXOML) =>
	{
		sXOML = this.HtmlEncode(sXOML);
		sXOML = sXOML.replace(/\n/g, '<br />\n');
		sXOML = sXOML.replace(/\t/g, '&nbsp;&nbsp;&nbsp;');
		return sXOML;
	}

	public render()
	{
		const { expand_xmol } = this.state;
		let sXOML: string = Sql.ToString(this.props.XOML);
		sXOML = this.XmolEncode(sXOML);
		let cssSql: any = { height: '2em', cursor: 'pointer', marginBottom: 0, overflowX: 'hidden', width: '100%', border: '1px solid black', fontFamily: 'courier new', padding: '1px' };
		if ( expand_xmol )
		{
			cssSql = { cursor: 'pointer', marginBottom: 0, width: '100%', border: '1px solid black', fontFamily: 'courier new', padding: '1px' };
		}
		return (<div style={ cssSql } onClick={ this.onToggleXoml } >
			<div dangerouslySetInnerHTML={ { __html: sXOML } }></div>
		</div>);
	}
}


