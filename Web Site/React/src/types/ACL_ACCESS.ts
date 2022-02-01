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

export default class ACL_ACCESS
{
	// 09/26/2017 Paul.  Add Archive access right. 
	public static FULL_ACCESS: number = 100;
	public static ARCHIVE    : number = 91;
	public static VIEW       : number = 90;
	public static ALL        : number = 90;
	public static ENABLED    : number =  89;
	public static OWNER      : number =  75;
	public static DISABLED   : number = -98;
	public static NONE       : number = -99;

	public static GetName(access: string, value: number)
	{
		let name: string = 'NONE';
		switch ( value )
		{
			case ACL_ACCESS.FULL_ACCESS:  name = 'FULL_ACCESS';  break;
			case ACL_ACCESS.ARCHIVE    :  name = 'ARCHIVE'    ;  break;
			case ACL_ACCESS.VIEW       :  (access == 'archive' ? name = 'VIEW' : name = 'ALL');  break;
			case ACL_ACCESS.ENABLED    :  name = 'ENABLED'    ;  break;
			case ACL_ACCESS.OWNER      :  name = 'OWNER'      ;  break;
			case ACL_ACCESS.DISABLED   :  name = 'DISABLED'   ;  break;
			case ACL_ACCESS.NONE       :  name = 'NONE'       ;  break;
		}
		return name;
	}
}


