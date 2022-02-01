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

export interface ISubPanelHeaderButtonsProps extends RouteComponentProps<any>
{
	MODULE_NAME      : string;
	MODULE_TITLE?    : string;
	SUB_TITLE?       : string;
	ID?              : string;
	LINK_NAME?       : string;
	CONTROL_VIEW_NAME: string;
	error            : any;
	// Button properties
	ButtonStyle      : string;
	FrameStyle?      : any;
	ContentStyle?    : any;
	VIEW_NAME        : string;
	row              : object;
	Page_Command     : (sCommandName, sCommandArguments) => void;
	onLayoutLoaded?  : () => void;
	showButtons      : boolean;
	onToggle         : (open: boolean) => void;
	// 04/10/2021 Paul.  Create framework to allow pre-compile of all modules. 
	isPrecompile?       : boolean;
}

interface ISubPanelHeaderButtonsState
{
	helpText        : string
	archiveView     : boolean;
	streamEnabled   : boolean;
	headerError     : any;
	localKey        : string;
	open            : boolean;
}

export abstract class SubPanelHeaderButtons extends React.Component<ISubPanelHeaderButtonsProps, ISubPanelHeaderButtonsState>
{
	// 10/30/2020 Paul.  We need a busy indicator for long-running tasks such as Archive. 
	public abstract Busy         (): void;
	public abstract NotBusy      (): void;
	public abstract DisableAll   (): void;
	public abstract EnableAll    (): void;
	public abstract HideAll      (): void;
	public abstract ShowAll      (): void;
	public abstract EnableButton (COMMAND_NAME: string, enabled: boolean): void;
	public abstract ShowButton   (COMMAND_NAME: string, visible: boolean): void;
	public abstract ShowHyperLink(URL         : string, visible: boolean): void;
}


