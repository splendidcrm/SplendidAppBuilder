/*
 * Copyright (C) 2005-2022 SplendidCRM Software, Inc. 
 * MIT License
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
 * files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
 * modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
 * is furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 */

export default interface DETAILVIEWS_RELATIONSHIP
{
	ID?                           : string;  // uniqueidentifier
	DETAIL_NAME?                  : string;  // nvarchar
	MODULE_NAME                   : string;  // nvarchar
	TITLE                         : string;  // nvarchar
	CONTROL_NAME                  : string;  // nvarchar
	RELATIONSHIP_ORDER?           : number;  // int
	TABLE_NAME                    : string;  // nvarchar
	PRIMARY_FIELD                 : string;  // nvarchar
	SORT_FIELD                    : string;  // nvarchar
	SORT_DIRECTION                : string;  // nvarchar
	initialOpen?                  : boolean;
}


