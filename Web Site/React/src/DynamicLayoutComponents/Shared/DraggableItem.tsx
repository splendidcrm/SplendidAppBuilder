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
import { FontAwesomeIcon }                 from '@fortawesome/react-fontawesome';
import { DragSource, DropTarget, ConnectDropTarget, ConnectDragSource, DropTargetMonitor, DropTargetConnector, DragSourceConnector, DragSourceMonitor } from 'react-dnd';
// 2. Store and Types. 
// 3. Scripts. 
import Sql                                 from '../../scripts/Sql';
import L10n                                from '../../scripts/L10n';

const style: React.CSSProperties =
{
	border         : '1px solid grey',
	backgroundColor: '#eeeeee',
	padding        : '2px',
	margin         : '2px',
	borderRadius   : '2px',
	width          : '200px',
	overflowX      : 'hidden',
	transition     : 'flex-basis 0.2s ease, order 0.2s ease',
	display        : 'flex',
	alignItems     : 'center',
	justifyContent : 'space-between',
};

interface IDraggableItemProps
{
	id                : string;
	item              : any;
	onEditClick       : (id: string) => void;
	fieldIndex        : number;
	colIndex          : number;
	rowIndex          : number;
	isDragging?       : boolean;
	didDrop?          : boolean;
	rowTotal?         : number;
	setDragging       : (id: string) => void;
	draggingId        : string;
	connectDragSource?: Function; // ConnectDragSource;
	moveDraggableItem : (id: string, hoverColIndex: number, hoverRowIndex: number, didDrop: boolean) => void;
	remove            : (item, type) => void;
}

interface IDraggableItemState
{
	isOver            : boolean;
	isDragging        : boolean;
}

const source =
{
	beginDrag(props: IDraggableItemProps)
	{
		//console.log((new Date()).toISOString() + ' ' + 'DraggableItem' + '.beginDrag', props);
		return {
			id        : props.id        ,
			fieldIndex: props.fieldIndex,
			colIndex  : props.colIndex  ,
			rowIndex  : props.rowIndex  ,
			origId    : props.id        ,
		};
	},
	endDrag(props: IDraggableItemProps, monitor: DragSourceMonitor)
	{
		const { fieldIndex, colIndex, rowIndex } = props;
		//console.log((new Date()).toISOString() + ' ' + 'DraggableItem' + '.endDrag', props, monitor.getItem(), monitor);
		if ( monitor.didDrop() )
		{
			const id            : string = monitor.getItem().id        ;
			const hoverColIndex : number = monitor.getItem().colIndex  ;
			const hoverRowIndex : number = monitor.getItem().rowIndex  ;
			props.moveDraggableItem(id, hoverColIndex, hoverRowIndex, true);
		}
		else
		{
			props.remove(monitor.getItem(), monitor.getItemType());
		}
	}
};

function collect(connect: DragSourceConnector, monitor: DragSourceMonitor)
{
	//console.log((new Date()).toISOString() + ' ' + 'DraggableItem' + '.collect', connect, monitor);
	return {
		connectDragSource: connect.dragSource(),
		isDragging       : monitor.isDragging(),
		didDrop          : monitor.didDrop(),
	};
}

class DraggableItem extends React.Component<IDraggableItemProps, IDraggableItemState>
{
	constructor(props: IDraggableItemProps)
	{
		super(props);
		//console.log((new Date()).toISOString() + ' ' + this.constructor.name + '.constructor', props);
		this.state =
		{
			isOver    : false,
			isDragging: false,
		};
	}

	shouldComponentUpdate(nextProps: IDraggableItemProps)
	{
		if ( this.props.draggingId !== nextProps.draggingId && nextProps.draggingId == nextProps.id )
		{
			return true;
		}
		else if ( this.props.isDragging != nextProps.isDragging || this.props.didDrop != nextProps.didDrop )
		{
			return true;
		}
		else if ( this.props.fieldIndex != nextProps.fieldIndex || this.props.colIndex != nextProps.colIndex || this.props.rowIndex != nextProps.rowIndex )
		{
			return true;
		}
		else if ( this.props.id != nextProps.id || JSON.stringify(this.props.item) != JSON.stringify(nextProps.item) )
		{
			return true;
		}
		return false;
	}

	componentWillUpdate(nextProps)
	{
		if ( nextProps.isDragging )
		{
			//console.log((new Date()).toISOString() + ' ' + this.constructor.name + '.componentWillUpdate', nextProps);
			nextProps.setDragging(nextProps.id);
		}
		if ( nextProps.didDrop && nextProps.id == nextProps.draggingId )
		{
			//console.log((new Date()).toISOString() + ' ' + this.constructor.name + '.componentWillUpdate', nextProps);
			nextProps.setDragging('');
		}
	}

	public render()
	{
		const { item, id, draggingId, rowTotal, colIndex, onEditClick } = this.props;
		const opacity = id == draggingId ? 0 : 1;
		let DATA_FIELD: string = null;
		if ( item )
		{
			if ( item.FIELD_TYPE == 'Header' )
			{
				DATA_FIELD = L10n.Term('DynamicLayout.LBL_HEADER_TYPE');
			}
			else if ( item.FIELD_TYPE == 'Blank' )
			{
				DATA_FIELD = L10n.Term('DynamicLayout.LBL_BLANK_TYPE');
			}
			else if ( item.FIELD_TYPE == 'Separator' )
			{
				DATA_FIELD = L10n.Term('DynamicLayout.LBL_SEPARATOR_TYPE');
			}
			else if ( item.FIELD_TYPE == 'Hidden' )
			{
				DATA_FIELD = L10n.Term('DynamicLayout.LBL_HIDDEN_TYPE');
			}
			else
			{
				DATA_FIELD = item.DATA_FIELD;
			}
		}
		return (
			this.props.connectDragSource &&
			this.props.connectDragSource(
				<div
					className='grab'
					style={{ ...style, opacity, flexBasis: `${100 / rowTotal}%` }}
					id={ 'ctlDynamicLayout_' + id }
				>
					{ item
					? DATA_FIELD
					: null
					}
					<br />
					{ item && !Sql.IsEmptyString(item.DATA_LABEL)
					? L10n.Term(item.DATA_LABEL)
					: null
					}
					{ item && (item.FIELD_TYPE != 'Blank' && item.FIELD_TYPE != 'Separator')
					? <span style={ {cursor: 'pointer'} } onClick={ () => onEditClick(id) }>
						<FontAwesomeIcon icon="edit" size="lg" />
					</span>
					: null
					}
				</div>
			)
		);
	}
}

export default DragSource('ITEM', source, collect)(DraggableItem);

