/*
 * Copyright (C) 2005-2022 SplendidCRM Software, Inc. 
 * MIT License
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
 * files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
 * modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
 * is furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 */

export default interface EDITVIEWS_RELATIONSHIPS
{
	ID?                           : string  // uniqueidentifier
	EDIT_NAME?                    : string  // nvarchar
	MODULE_NAME                   : string  // nvarchar
	TITLE                         : string  // nvarchar
	CONTROL_NAME                  : string  // nvarchar
	RELATIONSHIP_ORDER?           : number  // int
	NEW_RECORD_ENABLED            : boolean // bit
	EXISTING_RECORD_ENABLED       : boolean // bit
	ALTERNATE_VIEW                : string  // nvarchar
	initialOpen?                  : boolean;
}


