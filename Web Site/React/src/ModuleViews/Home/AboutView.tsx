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
import { Link, RouteComponentProps, withRouter }     from 'react-router-dom'           ;
// 2. Store and Types. 
// 3. Scripts. 
import Sql                                           from '../../scripts/Sql'          ;
import L10n                                          from '../../scripts/L10n'         ;
import Credentials                                   from '../../scripts/Credentials'  ;
import SplendidCache                                 from '../../scripts/SplendidCache';
import { Crm_Config }                                from '../../scripts/Crm'          ;
import { Version }                                   from '../../scripts/Login'        ;
import { AppName, AppVersion }                       from '../../AppVersion'           ;
import { isMobileDevice, isTouchDevice, screenWidth, screenHeight } from '../../scripts/utility'      ;
// 4. Components and Views. 

interface IAboutViewProps extends RouteComponentProps<any>
{
}

interface IAboutViewState
{
	lblBuildNumber  : string;
	lblVersionNumber: string;
	lblServiceLevel : string;
	lblLicense      : string;
	error?          : any;
	height          : number;
	width           : number;
}

class AboutView extends React.Component<IAboutViewProps, IAboutViewState>
{
	constructor(props: IAboutViewProps)
	{
		super(props);

		let lblBuildNumber  : string = '';
		let lblVersionNumber: string = Crm_Config.ToString('sugar_version');
		let lblServiceLevel : string = Crm_Config.ToString('service_level');
		let lblLicense      : string = '';
		if ( !Sql.IsEmptyString(lblServiceLevel) )
		{
			if ( lblServiceLevel == 'Community' )
			{
				lblLicense = Crm_Config.ToString('gnu_license');
			}
			else
			{
				lblLicense = Crm_Config.ToString('license');
			}
		}
		Credentials.SetViewMode('AboutView');
		let width : number = screenWidth();
		let height: number = screenHeight();
		this.state =
		{
			lblBuildNumber  ,
			lblVersionNumber,
			lblServiceLevel ,
			lblLicense      ,
			height          ,
			width           ,
		}
		//this.updateDimensions = this.updateDimensions.bind(this);
	}

	async componentDidMount()
	{
		try
		{
			document.title = L10n.Term('.LBL_BROWSER_TITLE');
			let lblBuildNumber: string = await Version();
			this.setState({ lblBuildNumber });
			window.addEventListener("resize", this.updateDimensions);
		}
		catch(error)
		{
			this.setState({ error });
		}
	}

	componentWillUnmount()
	{
		window.removeEventListener("resize", this.updateDimensions);
	}

	private updateDimensions = () =>
	{
		let width : number = screenWidth();
		let height: number = screenHeight();
		//console.error((new Date()).toISOString() + ' ' + this.constructor.name + '.updateDimensions (' +  width + ', ' + height + ')');
		this.setState(
		{
			height,
			width ,
		});
	}

	public render()
	{
		const { lblBuildNumber, lblServiceLevel, lblLicense, height, width } = this.state;
		let bMOBILE_CLIENT        : boolean = Credentials.bMOBILE_CLIENT;
		// 04/20/2021 Paul.  Reference obserable IsInitialized so that terminology update will cause refresh. 
		SplendidCache.IsInitialized;
		return (<div className='body'>
	<div style={ {marginTop: '15px', marginBottom: '2px'} }><b>{ AppName } { lblServiceLevel } Version { lblBuildNumber }</b></div>
	{ bMOBILE_CLIENT
	? <div><b>{ AppName } { L10n.Term('.LNK_MOBILE_CLIENT') } Build { AppVersion }</b></div>
	: null
	}
	<div style={ {marginTop: '2px', marginBottom: '4px'} }>Copyright &copy; 2005 -{ (new Date()).getFullYear() } <a href='http://www.splendidcrm.com' target='_blank' className='body'>SplendidCRM Software, Inc.</a> All Rights Reserved.</div>
	<div dangerouslySetInnerHTML={ { __html: lblLicense } } />
	
	<div style={ {marginTop: '15px'} }><b>SplendidCRM Software, Inc.</b></div>
	<div>Web:     <a href='http://www.splendidcrm.com' target='_blank' className='body'>http://www.splendidcrm.com</a></div>
	<div>Sales:   <a href='mailto:sales@splendidcrm.com'               className='body'>sales@splendidcrm.com</a></div>
	<div>Support: <a href='mailto:support@splendidcrm.com'             className='body'>support@splendidcrm.com</a></div>
	
	<div style={ {marginTop: '15px'} }><b>Open-Source libraries</b></div>
	<ul style={ {marginTop: '0px'} }>
		<li><a href='https://reactjs.org/'                target='_blank'>React</a> - A JavaScript library for building user intefaces.</li>

		<li><a href='http://www.mimekit.net/'             target='_blank'>MimeKit</a> - MimeKit is a C# library which may be used for the creation and parsing of messages using the Multipurpose Internet Mail Extension (MIME).</li>
		<li><a href='http://www.asp.net/signalr'          target='_blank'>ASP.NET SignalR</a> - ASP.NET SignalR is a new library for ASP.NET developers that makes developing real-time web functionality easy.</li>
		<li><a href='http://ckeditor.com/'                target='_blank'>CKEditor</a> - CKEditor is a text editor to be used inside web pages. It's a WYSIWYG editor, which means that the text being edited on it looks as similar as possible to the results users have when publishing it.</li>
		<li><a href='http://www.jqplot.com/'              target='_blank'>jqPlot</a> - jqPlot is a plotting and charting plugin for the jQuery Javascript framework.</li>
		<li><a href='http://jqueryui.com/'                target='_blank'>jQuery UI</a> - jQuery UI provides abstractions for low-level interaction and animation, advanced effects and high-level, themeable widgets, built on top of the jQuery JavaScript Library, that you can use to build highly interactive web applications.</li>
		<li><a href='http://jquery.com/'                  target='_blank'>jQuery</a> - jQuery is a fast and concise JavaScript Library that simplifies HTML document traversing, event handling, animating, and Ajax interactions for rapid web development.</li>
		<li><a href='http://getbootstrap.com/'            target='_blank'>Bootstrap</a> - Bootstrap is the most popular HTML, CSS, and JS framework for developing responsive, mobile first projects on the web.</li>
		<li><a href='http://datatables.net/'              target='_blank'>DataTables</a> - DataTables is a highly flexible tool, based upon the foundations of progressive enhancement, and will add advanced interaction controls to any HTML table.</li>
		<li><a href='http://imapnet.codeplex.com/'        target='_blank'>Koolwired.IMAP</a> - Koolwired.IMAP is a C# implementation of IMAP.</li>
	</ul>
	<div style={ {marginTop: '15px'} }><b>Device Information</b></div>
	<table cellPadding={ 4 } cellSpacing={ 0 }>
		<tr>
			<td style={ {marginTop: '2px', marginBottom: '2px'} }>React App Build:</td><td>{ AppVersion }</td>
		</tr>
		<tr>
			<td style={ {marginTop: '2px', marginBottom: '2px'} }>User Theme:</td><td>{ SplendidCache.UserTheme }</td>
		</tr>
		<tr>
			<td style={ {marginTop: '2px', marginBottom: '2px'} }>Screen Width:</td><td>{ screen.width } x { screen.height }</td>
		</tr>
		<tr>
			<td style={ {marginTop: '2px', marginBottom: '2px'} }>Device Pixel Ratio:</td><td>{ window.devicePixelRatio }</td>
		</tr>
		<tr>
			<td style={ {marginTop: '2px', marginBottom: '2px'} }>Window Inner Size:</td><td>{ window.innerWidth } x { window.innerHeight }</td>
		</tr>
		<tr>
			<td style={ {marginTop: '2px', marginBottom: '2px'} }>Effective Window Size:</td><td>{ width } x { height }</td>
		</tr>
		<tr>
			<td style={ {marginTop: '2px', marginBottom: '2px'} }>Is Mobile:</td><td>{ isMobileDevice() ? 'Yes' : 'No' }</td>
		</tr>
		<tr>
			<td style={ {marginTop: '2px', marginBottom: '2px'} }>Is Touch:</td><td>{ isTouchDevice() ? 'Yes' : 'No' }</td>
		</tr>
		<tr>
			<td style={ {marginTop: '2px', marginBottom: '2px'} }>User Agent:</td><td>{ navigator.userAgent }</td>
		</tr>
	</table>
</div>
		);
	}
}

export default withRouter(AboutView);

