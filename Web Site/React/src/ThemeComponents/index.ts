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
import { isMobileDevice, isTouchDevice, screenWidth } from '../scripts/utility'               ;
// 4. Components and Views. 
// 04/01/2022 Paul.  Add Pacific theme, derived from Arctic.
import PacificTopNav                  from './Pacific/TopNav'                 ;
import ArcticTopNav                   from './Arctic/TopNav'                  ;
// 05/10/2021 Paul.  The new menu system leaves the menu up or pops up in the wrong location, so only use on mobile devices. 
import ArcticTopNav_Desktop           from './Arctic/TopNav_Desktop'          ;

import ArcticSideBar                  from './Arctic/SideBar'                 ;

export function TopNavFactory(sTHEME: string)
{
	let ctl: any = null;
	// 05/10/2021 Paul.  The new menu system leaves the menu up or pops up in the wrong location, so only use on mobile devices. 
	let width : number = screenWidth();
	// 05/11/2021 Paul.  We are having issues with the more dropdown disappearing, so don't treat touch as mobile. 
	// 07/14/2021 Paul.  Now that we have time to test, always enable new menus. 
	if ( true || isMobileDevice() || width < 800 )
	{
		//console.log((new Date()).toISOString() + ' ' + 'TopNavFactory mobile ' + sTHEME);
		switch ( sTHEME )
		{
			// 04/01/2022 Paul.  Add Pacific theme, derived from Arctic.
			case 'Pacific'  :  ctl = PacificTopNav  ;  break;
			case 'Arctic'   :  ctl = ArcticTopNav   ;  break;
		}
	}
	else
	{
		//console.log((new Date()).toISOString() + ' ' + 'TopNavFactory desktop ' + sTHEME);
		switch ( sTHEME )
		{
			case 'Arctic'   :  ctl = ArcticTopNav_Desktop   ;  break;
		}
	}
	if ( ctl )
	{
		//console.log((new Date()).toISOString() + ' ' + 'TopNavFactory found ' + sTHEME);
	}
	else
	{
		// 04/01/2022 Paul.  Add Pacific theme, derived from Arctic.
		ctl = PacificTopNav;
		//console.log((new Date()).toISOString() + ' ' + 'TopNavFactory not found ' + sTHEME + ', using Arctic');
	}
	return ctl;
}

// 01/19/2020 Paul.  Moved HeaderButtonsFactory to separate file due to problem with DynamicLayout.ts not being able to find it. 
// 01/19/2020 Paul.  Moved SubPanelButtonsFactory to separate file due to problem with DynamicLayout.ts not being able to find it. 

export function SideBarFactory(sTHEME: string)
{
	let ctl: any = null;
	switch ( sTHEME )
	{
		case 'Arctic'   :  ctl = ArcticSideBar   ;  break;
	}
	if ( ctl )
	{
		//console.log((new Date()).toISOString() + ' ' + 'SideBarFactory found ' + sTHEME);
	}
	return ctl;
}
