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

// 04/01/2022 Paul.  Add Pacific theme, derived from Arctic.
import PacificHeaderButtons           from './Pacific/HeaderButtons'          ;
import ArcticHeaderButtons            from './Arctic/HeaderButtons'           ;

export default function HeaderButtonsFactory(sTHEME: string)
{
	let ctl: any = null;
	switch ( sTHEME )
	{
		// 04/01/2022 Paul.  Add Pacific theme, derived from Arctic.
		case 'Pacific'  :  ctl = PacificHeaderButtons  ;  break;
		case 'Arctic'   :  ctl = ArcticHeaderButtons   ;  break;
	}
	if ( ctl )
	{
		//console.log((new Date()).toISOString() + ' ' + 'HeaderButtonsFactory found ' + sTHEME);
	}
	else
	{
		ctl = ArcticHeaderButtons;
		//console.log((new Date()).toISOString() + ' ' + 'HeaderButtonsFactory not found ' + sTHEME + ', using Arctic');
	}
	return ctl;
}

