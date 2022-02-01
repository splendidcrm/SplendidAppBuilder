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

interface ISourceTemplateColumnProps
{
	TITLE: string;
	removeRow: (index: number) => void;
	isDragging?: boolean;
	connectDragSource?: ConnectDragSource;
}

const source =
{
	beginDrag(props: ISourceTemplateColumnProps)
	{
		//console.log((new Date()).toISOString() + ' ' + 'SourceTemplateColumn' + '.beginDrag', props);
		return {
			id         : uuidFast(),
			index      : -1,
			COLUMN_TYPE: 'NewTemplateColumn'
		};
	},
	endDrag(props: ISourceTemplateColumnProps, monitor: DragSourceMonitor)
	{
		//console.log((new Date()).toISOString() + ' ' + 'SourceTemplateColumn' + '.endDrag', props, monitor);
		if ( !monitor.didDrop() )
		{
			props.removeRow(monitor.getItem().index);
		}
	}
}

function collect(connect: DragSourceConnector, monitor: DragSourceMonitor)
{
	//console.log((new Date()).toISOString() + ' ' + 'SourceTemplateColumn' + '.collect', connect, monitor);
	return {
		connectDragSource: connect.dragSource()
	};
}

class SourceTemplateColumn extends React.Component<ISourceTemplateColumnProps>
{
	constructor(props: ISourceTemplateColumnProps)
	{
		super(props);
		//console.log((new Date()).toISOString() + ' ' + this.constructor.name + '.constructor');
	}

	public render()
	{
		const { TITLE, connectDragSource } = this.props;
		return (
			connectDragSource &&
			connectDragSource(
				<div
					style={ { ...style } }>
					{ TITLE }
				</div>
			)
		);
	}
}

export default DragSource('ROW', source, collect)(SourceTemplateColumn);

