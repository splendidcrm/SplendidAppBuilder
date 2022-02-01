/*
 * Copyright (C) 2005-2022 SplendidCRM Software, Inc. 
 * MIT License
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
 * files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
 * modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
 * is furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 */

import React from 'react';
import { FontAwesomeIcon }                       from '@fortawesome/react-fontawesome';
import { DropTarget, DropTargetConnector, DropTargetMonitor, ConnectDropTarget } from 'react-dnd';

interface IDraggableRemoveProps
{
	isOver?           : boolean
	connectDropTarget?: Function;  // ConnectDropTarget;
	remove            : (item, type) => void;
}

interface IDraggableRemoveState
{
	isOver            : boolean;
}

const boxTarget =
{
	drop(props: IDraggableRemoveProps, monitor: DropTargetMonitor)
	{
		//console.log((new Date()).toISOString() + ' ' + 'DraggableRemove' + '.drop', props);
		props.remove(monitor.getItem(), monitor.getItemType());
	}
};

function collect(connect: DropTargetConnector, monitor: DropTargetMonitor)
{
	//console.log((new Date()).toISOString() + ' ' + 'DraggableRemove' + '.collect', connect, monitor);
	return {
		connectDropTarget: connect.dropTarget(),
		isOver           : monitor.isOver(),
	};
}

class DraggableRemove extends React.Component<IDraggableRemoveProps, IDraggableRemoveState>
{
	constructor(props: IDraggableRemoveProps)
	{
		super(props);
		//console.log((new Date()).toISOString() + ' ' + this.constructor.name + '.constructor', props);
		this.state =
		{
			isOver: false
		};
	}

	public render()
	{
		const { isOver } = this.state;
		return (
			this.props.connectDropTarget &&
			this.props.connectDropTarget(
				<div
					style={{ padding: '1em 0', display: 'inline-block' }}>
					<FontAwesomeIcon icon='trash-alt' size='4x' />
				</div>
			)
		);
	}
}

export default DropTarget(['ITEM', 'ROW'], boxTarget, collect)(DraggableRemove);

