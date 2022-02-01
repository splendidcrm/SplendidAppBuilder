/*
 * Copyright (C) 2005-2022 SplendidCRM Software, Inc. 
 * MIT License
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
 * files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
 * modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
 * is furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 */

export default interface GRIDVIEWS_COLUMN
{
	ID                            : string;  // uniqueidentifier
	DELETED                       : boolean; // bit
	GRID_NAME                     : string;  // nvarchar
	COLUMN_INDEX                  : number;  // int
	COLUMN_TYPE                   : string;  // nvarchar
	DEFAULT_VIEW                  : boolean; // bit
	HEADER_TEXT                   : string;  // nvarchar
	SORT_EXPRESSION               : string;  // nvarchar
	ITEMSTYLE_WIDTH               : string;  // nvarchar
	ITEMSTYLE_CSSCLASS            : string;  // nvarchar
	ITEMSTYLE_HORIZONTAL_ALIGN    : string;  // nvarchar
	ITEMSTYLE_VERTICAL_ALIGN      : string;  // nvarchar
	ITEMSTYLE_WRAP                : boolean; // bit
	DATA_FIELD                    : string;  // nvarchar
	DATA_FORMAT                   : string;  // nvarchar
	URL_FIELD                     : string;  // nvarchar
	URL_FORMAT                    : string;  // nvarchar
	URL_TARGET                    : string;  // nvarchar
	LIST_NAME                     : string;  // nvarchar
	URL_MODULE                    : string;  // nvarchar
	URL_ASSIGNED_FIELD            : string;  // nvarchar
	VIEW_NAME                     : string;  // nvarchar
	MODULE_NAME                   : string;  // nvarchar
	MODULE_TYPE                   : string;  // nvarchar
	PARENT_FIELD                  : string;  // nvarchar
	SCRIPT                        : string;  // nvarchar

}


