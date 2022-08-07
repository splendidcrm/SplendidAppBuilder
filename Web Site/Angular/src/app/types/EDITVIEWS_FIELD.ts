/*
 * Copyright (C) 2005-2022 SplendidCRM Software, Inc. 
 * MIT License
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
 * files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
 * modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
 * is furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 */

export default interface EDITVIEWS_FIELD
{
	ID                            : string;  // uniqueidentifier
	DELETED                       : boolean; // bit
	EDIT_NAME                     : string;  // nvarchar
	FIELD_INDEX                   : number;  // int
	FIELD_TYPE                    : string;  // nvarchar
	DEFAULT_VIEW                  : boolean; // bit
	DATA_LABEL                    : string;  // nvarchar
	DATA_FIELD                    : string;  // nvarchar
	DATA_FORMAT                   : string;  // nvarchar
	DISPLAY_FIELD                 : string;  // nvarchar
	CACHE_NAME                    : string;  // nvarchar
	LIST_NAME                     : string;  // nvarchar
	DATA_REQUIRED                 : boolean; // bit
	UI_REQUIRED                   : boolean; // bit
	ONCLICK_SCRIPT                : string;  // nvarchar
	FORMAT_SCRIPT                 : string;  // nvarchar
	FORMAT_TAB_INDEX              : number;  // int
	FORMAT_MAX_LENGTH             : number;  // int
	FORMAT_SIZE                   : number;  // int
	FORMAT_ROWS                   : number;  // int
	FORMAT_COLUMNS                : number;  // int
	COLSPAN                       : number;  // int
	ROWSPAN                       : number;  // int
	LABEL_WIDTH                   : string;  // nvarchar
	FIELD_WIDTH                   : string;  // nvarchar
	DATA_COLUMNS                  : number;  // int
	VIEW_NAME                     : string;  // nvarchar
	FIELD_VALIDATOR_ID            : string;  // uniqueidentifier
	FIELD_VALIDATOR_MESSAGE       : string;  // nvarchar
	UI_VALIDATOR                  : number;  // int
	VALIDATION_TYPE               : string;  // nvarchar
	REGULAR_EXPRESSION            : string;  // nvarchar
	DATA_TYPE                     : string;  // nvarchar
	MININUM_VALUE                 : string;  // nvarchar
	MAXIMUM_VALUE                 : string;  // nvarchar
	COMPARE_OPERATOR              : string;  // nvarchar
	MODULE_TYPE                   : string;  // nvarchar
	MODULE_NAME                   : string;  // nvarchar
	FIELD_VALIDATOR_NAME          : string;  // nvarchar
	TOOL_TIP                      : string;  // nvarchar
	RELATED_SOURCE_MODULE_NAME    : string;  // nvarchar
	RELATED_SOURCE_VIEW_NAME      : string;  // nvarchar
	RELATED_SOURCE_ID_FIELD       : string;  // nvarchar
	RELATED_SOURCE_NAME_FIELD     : string;  // nvarchar
	RELATED_VIEW_NAME             : string;  // nvarchar
	RELATED_ID_FIELD              : string;  // nvarchar
	RELATED_NAME_FIELD            : string;  // nvarchar
	RELATED_JOIN_FIELD            : string;  // nvarchar
	PARENT_FIELD                  : string;  // nvarchar
	SCRIPT                        : string;  // nvarchar
	hidden                        : boolean;
	ActiveTab                     : boolean;
}

