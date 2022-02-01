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
import * as React from 'react';
import { DragSource, DropTarget, ConnectDropTarget, ConnectDragSource, DropTargetMonitor, DropTargetConnector, DragSourceConnector, DragSourceMonitor } from 'react-dnd';
// 2. Store and Types. 
// 3. Scripts. 
import L10n from '../../scripts/L10n';

const style: React.CSSProperties =
{
	border         : '1px solid grey',
	backgroundColor: '#eeeeee',
	padding        : '2px',
	margin         : '2px',
	borderRadius   : '2px',
	width          : '200px',
	overflowX      : 'hidden',
};

interface ISouceItemProps
{
	ModuleName          : string;
	item                : any;
	isFieldInUse        : boolean;
	connectDragSource?  : Function;  // ConnectDragSource;
	createItemFromSource: (item: any) => any;
	moveDraggableItem   : (id: string, hoverColIndex: number, hoverRowIndex: number, didDrop: boolean) => void;
	remove              : (item, type) => void;
}

const source =
{
	beginDrag(props: ISouceItemProps)
	{
		//console.log((new Date()).toISOString() + ' ' + 'SouceItem' + '.beginDrag', props);
		let obj: any = Object.assign(props.item, { id: props.item.DATA_FIELD} );
		return props.createItemFromSource(obj);
	},
	endDrag(props: ISouceItemProps, monitor: DragSourceMonitor)
	{
		//console.log((new Date()).toISOString() + ' ' + 'SouceItem' + '.endDrag', props, monitor.getItem());
		if ( monitor.didDrop() )
		{
			const id           : string = monitor.getItem().id        ;
			const hoverColIndex: number = monitor.getItem().colIndex  ;
			const hoverRowIndex: number = monitor.getItem().rowIndex  ;
			props.moveDraggableItem(id, hoverColIndex, hoverRowIndex, true);
		}
		else
		{
			// 03/14/2020 Paul.  We need to remove the ghost item created above. 
			props.remove(monitor.getItem(), 'ITEM');
		}
	}
};

function collect(connect: DragSourceConnector, monitor: DragSourceMonitor)
{
	//console.log((new Date()).toISOString() + ' ' + 'SouceItem' + '.collect', connect, monitor);
	return {
		connectDragSource: connect.dragSource()
	};
}

class SouceItem extends React.Component<ISouceItemProps>
{
	constructor(props: ISouceItemProps)
	{
		super(props);
		//console.log((new Date()).toISOString() + ' ' + this.constructor.name + '.constructor', props);
	}

	public render()
	{
		const{ ModuleName, item, isFieldInUse } = this.props;
		// 03/14/2020 Paul.  When field is in use, we must hide it instead of not creating it as failure to create would prefent endDrag from firing. 
		return (
			this.props.connectDragSource &&
			this.props.connectDragSource(
				<div
					className='grab'
					style={ { ...style, display: (isFieldInUse ? 'none' : null) } }
					>
					{ item.ColumnName }
					<br />
					{ L10n.TableColumnName(ModuleName, item.ColumnName) }
				</div>
			)
		);
	}
}

export default DragSource('ITEM', source, collect)(SouceItem);

