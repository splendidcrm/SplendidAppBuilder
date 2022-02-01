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
import L10n from '../scripts/L10n';
// 4. Components and Views. 

interface ISearchTabsProps
{
	searchMode            : string;
	duplicateSearchEnabled: boolean;
	onTabChange           : Function;
}

export default class SearchTabs extends React.Component<ISearchTabsProps>
{
	constructor(props: ISearchTabsProps)
	{
		super(props);
	}

	private _onSearchTabChange = (key) =>
	{
		const { onTabChange } = this.props;
		if ( onTabChange != null )
		{
			onTabChange(key);
		}
		return false;
	}

	public render()
	{
		const { searchMode, duplicateSearchEnabled } = this.props;
		return (
			<ul id='pnlSearchTabs' className='tablist' onSelect={ this._onSearchTabChange }>
				<li>
					<a id='lnkBasicSearch' onClick={ (e) => { e.preventDefault(); return this._onSearchTabChange('Basic'); } } href='#' className={ searchMode == 'Basic' ? 'current' : null }>{ L10n.Term('.LNK_BASIC_SEARCH') }</a>
				</li>
				<li>
					<a id='lnkAdvancedSearch' onClick={ (e) => { e.preventDefault(); return this._onSearchTabChange('Advanced'); } } href='#' className={ searchMode == 'Advanced' ? 'current' : null }>{ L10n.Term('.LNK_ADVANCED_SEARCH') }</a>
				</li>
				{ duplicateSearchEnabled
				? <li>
					<a id='lnkDuplicateSearch' onClick={ (e) => { e.preventDefault(); return this._onSearchTabChange('Duplicates'); } } href='#' className={ searchMode == 'Duplicates' ? 'current' : null }>{ L10n.Term('.LNK_DUPLICATE_SEARCH') }</a>
				</li>
				: null
				}
			</ul>
		);
	}
}


