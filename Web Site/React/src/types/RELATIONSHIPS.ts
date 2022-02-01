/*
 * Copyright (C) 2005-2022 SplendidCRM Software, Inc. 
 * MIT License
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
 * files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
 * modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
 * is furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 */

export default interface RELATIONSHIPS
{
	ID                            : string  ; // uniqueidentifier
	RELATIONSHIP_NAME             : string  ; // nvarchar
	LHS_MODULE                    : string  ; // nvarchar
	LHS_TABLE                     : string  ; // nvarchar
	LHS_KEY                       : string  ; // nvarchar
	RHS_MODULE                    : string  ; // nvarchar
	RHS_TABLE                     : string  ; // nvarchar
	RHS_KEY                       : string  ; // nvarchar
	JOIN_TABLE                    : string  ; // nvarchar
	JOIN_KEY_LHS                  : string  ; // nvarchar
	JOIN_KEY_RHS                  : string  ; // nvarchar
	RELATIONSHIP_TYPE             : string  ; // nvarchar
	RELATIONSHIP_ROLE_COLUMN      : string  ; // nvarchar
	RELATIONSHIP_ROLE_COLUMN_VALUE: string  ; // nvarchar
	REVERSE                       : boolean ; // bit
}


