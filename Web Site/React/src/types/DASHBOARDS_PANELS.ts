/*
 * Copyright (C) 2005-2022 SplendidCRM Software, Inc. 
 * MIT License
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
 * files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
 * modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
 * is furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 */

export default interface DASHBOARDS_PANELS
{
	ID                            : string  ; // uniqueidentifier
	PANEL_ORDER                   : number  ; // int
	ROW_INDEX                     : number  ; // int
	COLUMN_WIDTH                  : number  ; // int
	DASHBOARD_ID                  : string  ; // uniqueidentifier
	DASHBOARD_APP_ID              : string  ; // uniqueidentifier
	NAME                          : string  ; // nvarchar
	CATEGORY                      : string  ; // nvarchar
	MODULE_NAME                   : string  ; // nvarchar
	TITLE                         : string  ; // nvarchar
	SETTINGS_EDITVIEW             : string  ; // nvarchar
	IS_ADMIN                      : boolean ; // bit
	APP_ENABLED                   : boolean ; // bit
	SCRIPT_URL                    : string  ; // nvarchar
	DEFAULT_SETTINGS              : string  ; // nvarchar
	PANEL_TYPE?                   : string  ; // nvarchar
}


