/*
 * Copyright (C) 2005-2022 SplendidCRM Software, Inc. 
 * MIT License
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
 * files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
 * modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
 * is furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 */

import * as React from 'react';

export interface IEditComponentProps
{
	baseId                        : string;
	key?                          : string;
	row                           : any;
	layout                        : any;
	onChanged?                    : (DATA_FIELD: string, DATA_VALUE: any, DISPLAY_FIELD?: string, DISPLAY_VALUE?: any) => void;
	onSubmit?                     : () => void;
	onUpdate?                     : (PARENT_FIELD: string, DATA_VALUE: any, item?: any) => void;
	createDependency?             : (DATA_FIELD: string, PARENT_FIELD: string, PROPERTY_NAME?: string) => void;
	fieldDidMount?                : (DATA_FIELD: string, component: any) => void;
	bIsWriteable?                 : boolean;
	// 11/02/2019 Paul.  Hidden property is used to dynamically hide and show layout fields. 
	bIsHidden?                    : boolean;
	// 11/04/2019 Paul.  Some layouts have a test button next to a URL. 
	Page_Command?                 : Function;
	// 09/27/2020 Paul.  We need to be able to disable the default grow on TextBox. 
	bDisableFlexGrow?             : boolean;
}

export abstract class EditComponent<P extends IEditComponentProps, S> extends React.Component<P, S>
{
	public abstract get data(): any;
	public abstract validate(): boolean;
	public abstract updateDependancy(PARENT_FIELD: string, DATA_VALUE: any, PROPERTY_NAME?: string, item?: any): void;
	public abstract clear(): void;
}


