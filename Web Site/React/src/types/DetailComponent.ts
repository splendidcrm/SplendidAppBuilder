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
import { RouteComponentProps } from 'react-router-dom';

export interface IDetailViewProps extends RouteComponentProps<any>
{
	MODULE_NAME  : string;
	ID           : string;
	LAYOUT_NAME? : string;
	// 04/10/2021 Paul.  Create framework to allow pre-compile of all modules. 
	isPrecompile?       : boolean;
	onComponentComplete?: (MODULE_NAME, RELATED_MODULE, LAYOUT_NAME, vwMain) => void;
}

export interface IDetailComponentProps
{
	baseId        : string;
	row           : any;
	layout        : any;
	ERASED_FIELDS : string[];
	Page_Command? : Function;
	fieldDidMount?: (DATA_FIELD: string, component: any) => void;
	// 11/02/2019 Paul.  Hidden property is used to dynamically hide and show layout fields. 
	bIsHidden?    : boolean;
}

export interface IDetailComponentState
{
	ID          : string;
	FIELD_INDEX : number;
	DATA_FIELD? : string;
	DATA_VALUE? : string;
	DATA_FORMAT?: string;
	CSS_CLASS?  : string;
}

export abstract class DetailComponent<P extends IDetailComponentProps, S> extends React.Component<P, S>
{
	public abstract updateDependancy(PARENT_FIELD: string, DATA_VALUE: any, PROPERTY_NAME?: string, item?: any): void;
}


