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
import { RouteComponentProps, withRouter, Link }    from 'react-router-dom'              ;
import { FontAwesomeIcon }                          from '@fortawesome/react-fontawesome';
// 2. Store and Types. 
// 3. Scripts. 
import L10n                                         from '../scripts/L10n'               ;
import Credentials                                  from '../scripts/Credentials'        ;
import SplendidCache                                from '../scripts/SplendidCache'      ;
// 4. Components and Views. 

interface IListHeaderProps extends RouteComponentProps<any>
{
	MODULE_NAME?: string;
	TITLE?      : string;
}

class ListHeader extends React.Component<IListHeaderProps>
{
	constructor(props: IListHeaderProps)
	{
		super(props);
	}

	public render()
	{
		const { MODULE_NAME, TITLE } = this.props;
		let sMODULE_TITLE = L10n.Term(TITLE ? TITLE : MODULE_NAME + '.LBL_LIST_FORM_TITLE');
		// 10/29/2020 Paul.  Add the header arrow. 
		let themeURL = Credentials.RemoteServer + 'App_Themes/' + SplendidCache.UserTheme + '/';
		return (
			<table className='h3Row' cellSpacing={ 1 } cellPadding={ 0 } style={ {width: '100%', border: 'none', marginBottom: '2px'} }>
				<tr>
					<td style={ {whiteSpace: 'nowrap'} }>
						<h3>
							<FontAwesomeIcon icon='arrow-right' size='lg' style={ {marginRight: '.5em'} } transform={ {rotate: 45} } />
							&nbsp;<span>{ sMODULE_TITLE }</span>
						</h3>
					</td>
				</tr>
			</table>
		);
	}
}

export default withRouter(ListHeader);

