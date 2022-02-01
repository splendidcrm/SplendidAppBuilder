/*
 * Copyright (C) 2005-2022 SplendidCRM Software, Inc. 
 * MIT License
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
 * files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
 * modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
 * is furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 */
// 09/15/2019 Paul.  Adapted from @ckeditor/@ckeditor5-react

import * as React from 'react';

interface ICKEditorProps
{
	editor   : any;
	data?    : string;
	config?  : any;
	onChange?: Function;
	onInit?  : Function;
	onFocus? : Function;
	onBlur?  : Function;
	disabled?: boolean;
}

interface ICKEditorState
{
}

export default class CKEditor extends React.Component<ICKEditorProps, ICKEditorState>
{
	private editor       = null;
	private domContainer = React.createRef<HTMLDivElement>();

	constructor( props: ICKEditorProps )
	{
		super( props );
		// After mounting the editor, the variable will contain a reference to the created editor.
		// @see: https://ckeditor.com/docs/ckeditor5/latest/api/module_core_editor_editor-Editor.html
		//this.editor = null;
		//this.domContainer = React.createRef();
		this.state = 
		{
		};
	}

	// This component should never be updated by React itself.
	shouldComponentUpdate( nextProps: ICKEditorProps )
	{
		if ( !this.editor )
		{
			return false;
		}
		if ( this._shouldUpdateContent( nextProps ) )
		{
			this.editor.setData( nextProps.data );
		}
		if ( 'disabled' in nextProps )
		{
			this.editor.isReadOnly = nextProps.disabled;
		}
		return false;
	}

	// Initialize the editor when the component is mounted.
	componentDidMount()
	{
		this._initializeEditor();
	}

	// Destroy the editor before unmouting the component.
	componentWillUnmount()
	{
		this._destroyEditor();
	}

	public insertText(txt)
	{
		if ( this.editor )
		{
			// https://ckeditor.com/docs/ckeditor5/latest/builds/guides/faq.html#where-are-the-editorinserthtml-and-editorinserttext-methods-how-to-insert-some-content
			this.editor.model.change( writer =>
			{
				writer.insertText(txt, this.editor.model.document.selection.getFirstPosition() );
			});
		}
	}

	// Render a <div> element which will be replaced by CKEditor.
	render()
	{
		// We need to inject initial data to the container where the editable will be enabled. Using `editor.setData()`
		// is a bad practice because it initializes the data after every new connection (in case of collaboration usage).
		// It leads to reset the entire content. See: #68
		return (
			<div ref={ this.domContainer } dangerouslySetInnerHTML={ { __html: this.props.data || '' } }></div>
		);
	}

	_initializeEditor()
	{
		this.props.editor
		.create( this.domContainer.current , this.props.config )
		.then( editor =>
		{
			this.editor = editor;
			if ( 'disabled' in this.props )
			{
				editor.isReadOnly = this.props.disabled;
			}
			if ( this.props.onInit )
			{
				this.props.onInit( editor );
			}
			const modelDocument = editor.model.document;
			const viewDocument = editor.editing.view.document;
			modelDocument.on( 'change:data', event =>
			{
				/* istanbul ignore else */
				if ( this.props.onChange )
				{
					this.props.onChange( event, editor );
				}
			});
			viewDocument.on( 'focus', event =>
			{
				/* istanbul ignore else */
				if ( this.props.onFocus ) {
					this.props.onFocus( event, editor );
				}
			});
			viewDocument.on( 'blur', event =>
			{
				/* istanbul ignore else */
				if ( this.props.onBlur )
				{
					this.props.onBlur( event, editor );
				}
			});
		} )
		.catch( error =>
		{
			console.error( error );
		});
	}

	_destroyEditor()
	{
		if ( this.editor )
		{
			this.editor.destroy()
				.then( () =>
				{
					this.editor = null;
				});
		}
	}

	_shouldUpdateContent( nextProps ) {
		// Check whether `nextProps.data` is equal to `this.props.data` is required if somebody defined the `#data`
		// property as a static string and updated a state of component when the editor's content has been changed.
		// If we avoid checking those properties, the editor's content will back to the initial value because
		// the state has been changed and React will call this method.
		if ( this.props.data === nextProps.data )
		{
			return false;
		}
		// We should not change data if the editor's content is equal to the `#data` property.
		if ( this.editor.getData() === nextProps.data )
		{
			return false;
		}
		return true;
	}
}


