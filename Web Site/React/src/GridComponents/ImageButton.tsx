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
import { FontAwesomeIcon }  from '@fortawesome/react-fontawesome';
// 2. Store and Types. 
// 3. Scripts. 
import Sql                  from '../scripts/Sql'          ;
import L10n                 from '../scripts/L10n'         ;
import Credentials          from '../scripts/Credentials'  ;
import SplendidCache        from '../scripts/SplendidCache';
// 4. Components and Views. 

interface IImageButtonProps
{
	row          : any;
	layout       : any;
	Page_Command?: Function;
}

interface IImageButtonState
{
	DISPLAY_NAME: string;
}

class ImageButton extends React.PureComponent<IImageButtonProps, IImageButtonState>
{
	constructor(props: IImageButtonProps)
	{
		super(props);
		const { layout, row } = this.props;
	}

	private _onClick = (e) =>
	{
		const { row, layout, Page_Command } = this.props;
		let URL_FORMAT: string = Sql.ToString(layout.URL_FORMAT);
		//console.log((new Date()).toISOString() + ' ' + this.constructor.name + '._onClick ' + URL_FORMAT, row.ID);
		if ( Page_Command != null )
		{
			Page_Command(URL_FORMAT, row.ID);
		}
	}

	public render()
	{
		const { layout, row } = this.props;
		let URL_FIELD : string = Sql.ToString(layout.URL_FIELD );
		let URL_FORMAT: string = Sql.ToString(layout.URL_FORMAT);
		let TITLE     : string = L10n.Term('.LBL_' + URL_FORMAT.toUpperCase());
		let themeURL  : string = Credentials.RemoteServer + 'App_Themes/' + SplendidCache.UserTheme + '/';
		if ( layout == null )
		{
			return (<div>layout prop is null</div>);
		}
		else if ( Sql.IsEmptyString(URL_FIELD) )
		{
			return (<div>URL_FIELD is empty for FIELD_INDEX {layout.FIELD_INDEX}</div>);
		}
		else
		{
			return (<span style={ {cursor: 'pointer'} } onClick={ this._onClick }>
				{ URL_FORMAT == 'Preview'
				? <FontAwesomeIcon icon={ {prefix: 'far', iconName: 'eye'} } title={ TITLE } />
				: <img src={ themeURL + 'images/' + URL_FORMAT + '.gif' } alt={ TITLE } style={ {height: '16px', width: '16px', borderWidth: '0px'} } />
				}
			</span>);
		}
	}
}

export default ImageButton;

