/*
 * Copyright (C) 2005-2022 SplendidCRM Software, Inc. 
 * MIT License
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
 * files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
 * modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
 * is furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 */
mport { Injectable             } from '@angular/core'                      ;
import { CredentialsService     } from '../scripts/Credentials'             ;
import Sql                        from '../scripts/Sql'                     ;

@Injectable({
	providedIn: 'root'
})
export class C10nService
{
	constructor(protected Credentials: CredentialsService)
	{
	}

	public ToCurrency(f: number): number
	{
		// 05/10/2006 Paul.  Short-circuit the math if USD. 
		// This is more to prevent bugs than to speed calculations. 
		if ( this.Credentials.bUSER_CurrencyUSDollars || this.Credentials.dUSER_CurrencyCONVERSION_RATE <= 0 )
			return f;
		return f * this.Credentials.dUSER_CurrencyCONVERSION_RATE;
	}

	public FromCurrency(f: number): number
	{
		// 05/10/2006 Paul.  Short-circuit the math if USD. 
		// This is more to prevent bugs than to speed calculations. 
		if ( this.Credentials.bUSER_CurrencyUSDollars || this.Credentials.dUSER_CurrencyCONVERSION_RATE <= 0 )
			return f;
		return f / this.Credentials.dUSER_CurrencyCONVERSION_RATE;
	}
}
