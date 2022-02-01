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

export default interface FIELDS_META_DATA
{
	ID                            : string ;  // uniqueidentifier
	NAME                          : string ;  // nvarchar
	LABEL                         : string ;  // nvarchar
	CUSTOM_MODULE                 : string ;  // nvarchar
	DATA_TYPE                     : string ;  // nvarchar
	MAX_SIZE                      : number ;  // int
	REQUIRED_OPTION               : string ;  // nvarchar
	AUDITED                       : boolean;  // bit
	DEFAULT_VALUE                 : string ;  // nvarchar
	EXT1                          : string ;  // nvarchar
	EXT2                          : string ;  // nvarchar
	EXT3                          : string ;  // nvarchar
	MASS_UPDATE                   : boolean;  // bit
}


