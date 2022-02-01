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
import { uuidFast }                           from '../../scripts/utility'            ;

const style: React.CSSProperties =
{
	border         : '1px dashed grey',
	backgroundColor: '#eeeeee',
	padding        : '2px',
	margin         : '2px',
	borderRadius   : '2px',
	width          : '200px',
};

interface ISourceRowProps
{
	TITLE: string;
	removeRow: (index: number) => void;
	isDragging?: boolean;
	connectDragSource?: Function;  // ConnectDragSource;
}

const source =
{
	beginDrag(props: ISourceRowProps)
	{
		//console.log((new Date()).toISOString() + ' ' + 'SourceRow' + '.beginDrag', props);
		return {
			id   : uuidFast(),
			index: -1
		};
	},
	endDrag(props: ISourceRowProps, monitor: DragSourceMonitor)
	{
		//console.log((new Date()).toISOString() + ' ' + 'SourceRow' + '.endDrag', props, monitor);
		if ( !monitor.didDrop() )
		{
			props.removeRow(monitor.getItem().index);
		}
	}
}

function collect(connect: DragSourceConnector, monitor: DragSourceMonitor)
{
	//console.log((new Date()).toISOString() + ' ' + 'SourceRow' + '.collect', connect, monitor);
	return {
		connectDragSource: connect.dragSource()
	};
}

class SourceRow extends React.Component<ISourceRowProps>
{
	constructor(props: ISourceRowProps)
	{
		super(props);
		//console.log((new Date()).toISOString() + ' ' + this.constructor.name + '.constructor');
	}

	public render()
	{
		const { TITLE } = this.props;
		return (
			this.props.connectDragSource &&
			this.props.connectDragSource(
				<div
					style={ { ...style } }>
					{ TITLE }
				</div>
			)
		);
	}
}

export default DragSource('ROW', source, collect)(SourceRow);

