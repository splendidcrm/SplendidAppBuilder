/*
 * Copyright (C) 2005-2022 SplendidCRM Software, Inc. 
 * MIT License
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
 * files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
 * modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
 * is furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 */

export default interface DYNAMIC_BUTTON
{
	ID                            : string;  // uniqueidentifier
	VIEW_NAME                     : string;  // nvarchar
	CONTROL_INDEX                 : number;  // int
	CONTROL_TYPE                  : string;  // nvarchar
	DEFAULT_VIEW                  : boolean; // bit
	MODULE_NAME                   : string;  // nvarchar
	MODULE_ACCESS_TYPE            : string;  // nvarchar
	TARGET_NAME                   : string;  // nvarchar
	TARGET_ACCESS_TYPE            : string;  // nvarchar
	MOBILE_ONLY                   : boolean; // bit
	ADMIN_ONLY                    : boolean; // bit
	EXCLUDE_MOBILE                : boolean; // bit
	CONTROL_TEXT                  : string;  // nvarchar
	CONTROL_TOOLTIP               : string;  // nvarchar
	CONTROL_ACCESSKEY             : string;  // nvarchar
	CONTROL_CSSCLASS              : string;  // nvarchar
	TEXT_FIELD                    : string;  // nvarchar
	ARGUMENT_FIELD                : string;  // nvarchar
	COMMAND_NAME                  : string;  // nvarchar
	URL_FORMAT                    : string;  // nvarchar
	URL_TARGET                    : string;  // nvarchar
	ONCLICK_SCRIPT                : string;  // nvarchar
	HIDDEN                        : boolean; // bit
	BUSINESS_RULE                 : string;  // nvarchar
	BUSINESS_SCRIPT               : string;  // nvarchar
	MODULE_ACLACCESS              : string;
	TARGET_ACLACCESS              : string;
}


