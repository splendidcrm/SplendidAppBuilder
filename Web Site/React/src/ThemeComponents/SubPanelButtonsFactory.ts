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
// 2. Store and Types. 
// 3. Scripts. 
// 4. Components and Views. 

import ArcticSubPanelHeaderButtons    from './Arctic/SubPanelHeaderButtons'   ;

export default function SubPanelButtonsFactory(sTHEME: string)
{
	let ctl: any = null;
	switch ( sTHEME )
	{
		case 'Arctic'   :  ctl = ArcticSubPanelHeaderButtons   ;  break;
	}
	if ( ctl )
	{
		//console.log((new Date()).toISOString() + ' ' + 'SubPanelButtonsFactory found ' + sTHEME);
	}
	else
	{
		ctl = ArcticSubPanelHeaderButtons;
		//console.log((new Date()).toISOString() + ' ' + 'SubPanelButtonsFactory not found ' + sTHEME + ', using Arctic');
	}
	return ctl;
}


