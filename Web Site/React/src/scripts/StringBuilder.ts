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

const ControlChars = { CrLf: '\r\n', Cr: '\r', Lf: '\n', Tab: '\t' };

export default class StringBuilder
{
	private value : string;
	public  length: number;

	constructor()
	{
		this.value  = '';
		this.length = this.value.length;
	}

	public Append(s: string)
	{
		this.value  = this.value + s;
		this.length = this.value.length;
	}

	public AppendLine(s?: string)
	{
		if ( s === undefined )
		{
			this.value  = this.value + ControlChars;
			this.length = this.value.length;
		}
		else
		{
			this.value  = this.value + s;
			this.length = this.value.length;
		}
	}

	public toString()
	{
		return this.value;
	}
}


