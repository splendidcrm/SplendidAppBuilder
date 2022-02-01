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
import Sql    from './Sql';

// 02/21/2021 Paul.  DiscountPrice() will return dDISCOUNT_PRICE unmodified if formula is blank or fixed. 
export function DiscountPrice(sPRICING_FORMULA: string, fPRICING_FACTOR: number, dCOST_PRICE: number, dLIST_PRICE: number, dDISCOUNT_PRICE: number)
{
	if ( fPRICING_FACTOR > 0 )
	{
		switch ( sPRICING_FORMULA )
		{
			case "Fixed"             :
				break;
			case "ProfitMargin"      :
				dDISCOUNT_PRICE = dCOST_PRICE * 100 / (100 - fPRICING_FACTOR);
				break;
			case "PercentageMarkup"  :
				dDISCOUNT_PRICE = dCOST_PRICE * (1 + (fPRICING_FACTOR /100));
				break;
			case "PercentageDiscount":
				dDISCOUNT_PRICE = (dLIST_PRICE * (1 - (fPRICING_FACTOR /100))*100)/100;
				break;
			case "FixedDiscount":
				dDISCOUNT_PRICE = dLIST_PRICE - fPRICING_FACTOR;
				break;
			case "IsList"            :
				dDISCOUNT_PRICE = dLIST_PRICE;
				break;
		}
	}
	return dDISCOUNT_PRICE;
}

export function DiscountValue(sPRICING_FORMULA, fPRICING_FACTOR, dLIST_PRICE)
{
	let dDISCOUNT_VALUE = 0.0;
	if ( fPRICING_FACTOR > 0 )
	{
		switch ( sPRICING_FORMULA )
		{
			case 'PercentageDiscount':
				dDISCOUNT_VALUE = (dLIST_PRICE * (Sql.ToDecimal(fPRICING_FACTOR) /100)*100)/100;
				break;
			case 'FixedDiscount'     :
				dDISCOUNT_VALUE = Sql.ToDecimal(fPRICING_FACTOR);
				break;
		}
	}
	return dDISCOUNT_VALUE;
}


