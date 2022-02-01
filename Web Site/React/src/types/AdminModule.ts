/*
 * Copyright (C) 2005-2022 SplendidCRM Software, Inc. 
 * MIT License
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
 * files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
 * modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
 * is furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 */

export default interface AdminModule
{
	MODULE_NAME                   : string;
	DISPLAY_NAME                  : string;
	DESCRIPTION                   : string;
	EDIT_LABEL                    : string;
	MENU_ENABLED                  : boolean;
	TAB_ORDER                     : number;
	ADMIN_ROUTE                   : string;
	ICON_NAME                     : string;
}


