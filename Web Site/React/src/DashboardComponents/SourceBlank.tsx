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
import { DragSource, DropTarget, ConnectDropTarget, ConnectDragSource, DropTargetMonitor, DropTargetConnector, DragSourceConnector, DragSourceMonitor } from 'react-dnd';
import { uuidFast }                           from '../scripts/utility'            ;

const style: React.CSSProperties =
{
	border         : '1px dashed grey',
	padding        : '0.5rem 1rem',
	backgroundColor: 'white',
	margin         : '0 .25em',
};

interface ISourceBlankProps
{
	TITLE               : string;
	createItemFromSource: (item: any) => any;
	connectDragSource?  : ConnectDragSource;
}

const source =
{
	beginDrag(props: ISourceBlankProps)
	{
		//console.log((new Date()).toISOString() + ' ' + 'SourceBlank' + '.beginDrag', props);
		return props.createItemFromSource(
		{
			id               : uuidFast(),
			index            : -1,
			NAME             : '(blank)',
			CATEGORY         : null,
			MODULE_NAME      : null,
			TITLE            : '(blank)',
			SETTINGS_EDITVIEW: null,
			IS_ADMIN         : false,
			APP_ENABLED      : true,
			SCRIPT_URL       : null,
			DEFAULT_SETTINGS : null,
		});
	},
	endDrag(props: ISourceBlankProps, monitor: DragSourceMonitor)
	{
		//console.log((new Date()).toISOString() + ' ' + 'SourceBlank' + '.endDrag', props, monitor);
	}
};

function collect(connect: DragSourceConnector, monitor: DragSourceMonitor)
{
	//console.log((new Date()).toISOString() + ' ' + 'SourceBlank' + '.collect', connect, monitor);
	return {
		connectDragSource: connect.dragSource()
	};
}

class SourceBlank extends React.Component<ISourceBlankProps>
{
	constructor(props: ISourceBlankProps)
	{
		super(props);
		//console.log((new Date()).toISOString() + ' ' + this.constructor.name + '.constructor', props);
	}

	public render()
	{
		const{ TITLE, connectDragSource } = this.props;
		return (
			connectDragSource &&
			connectDragSource(
				<div style={ { ...style } } className="grab">
					{ TITLE }
				</div>
			)
		);
	}
}

export default DragSource('ITEM', source, collect)(SourceBlank);

