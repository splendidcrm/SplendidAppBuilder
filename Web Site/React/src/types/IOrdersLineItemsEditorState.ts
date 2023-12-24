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

export default interface IOrdersLineItemsEditorState
{
	bEnableTaxLineItems?  : boolean;
	bEnableTaxShipping?   : boolean;
	bShowTax?             : boolean;
	bEnableSalesTax?      : boolean;
	bDisableExchangeRate? : boolean;
	oNumberFormat?        : any;

	CURRENCY_ID?          : string;
	// 11/12/2022 Paul.  We can't dynamically convert to a number as it will prevent editing. 
	EXCHANGE_RATE?        : string;
	TAXRATE_ID?           : string;
	SHIPPER_ID?           : string;

	CURRENCY_ID_LIST?     : any[];
	TAXRATE_ID_LIST?      : any[];
	SHIPPER_ID_LIST?      : any[];

	SUBTOTAL?             : number;
	DISCOUNT?             : number;
	// 11/12/2022 Paul.  We can't dynamically convert to a number as it will prevent editing. 
	SHIPPING?             : string;
	TAX?                  : number;
	TOTAL?                : number;
}

