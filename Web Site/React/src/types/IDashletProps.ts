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
import { RouteComponentProps } from 'react-router-dom';
// 2. Store and Types. 

export default interface IDashletProps extends RouteComponentProps<any>
{
	ID               : string;
	TITLE            : string;
	SETTINGS_EDITVIEW: any;
	DEFAULT_SETTINGS : any;
	COLUMN_WIDTH     : number  ; // bootstrap 1 to 12. 
}


