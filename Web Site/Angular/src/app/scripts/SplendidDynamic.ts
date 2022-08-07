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
import { EndsWith } from '../scripts/utility';

export default class SplendidDynamic
{
	// 06/18/2015 Paul.  Add support for Seven theme. 
	static StackedLayout(sTheme: string, sViewName?: string): boolean
	{
		if (sViewName === undefined || sViewName == null)
			sViewName = '';
		// 04/02/2022 Paul.  Pacific uses stacked action menus. 
		return (sTheme === 'Seven' || sTheme === 'Pacific') && !EndsWith(sViewName, '.Preview');
	}

	// 04/08/2017 Paul.  Use Bootstrap for responsive design.
	static BootstrapLayout(): boolean
	{
		// 06/24/2017 Paul.  We need a way to turn off bootstrap for BPMN, ReportDesigner and ChatDashboard. 
		//return !bDESKTOP_LAYOUT && sPLATFORM_LAYOUT != '.OfficeAddin';
		return true;
	}
}

