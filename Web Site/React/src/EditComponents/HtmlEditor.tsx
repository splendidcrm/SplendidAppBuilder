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
import * as React      from 'react';
import CKEditor        from '../components/CKEditor';
//import ClassicEditorOld   from '@ckeditor/ckeditor5-build-classic';
import ClassicEditor   from 'ckeditor5-custom-build';
// 2. Store and Types. 
import { IEditComponentProps, EditComponent } from '../types/EditComponent';
// 3. Scripts. 
import Sql                                    from '../scripts/Sql'        ;
import L10n                                   from '../scripts/L10n'       ;
import Security                               from '../scripts/Security'   ;
// 4. Components and Views. 


// https://ckeditor.com/docs/ckeditor5/latest/builds/guides/integration/configuration.html
// https://stackoverflow.com/questions/46559354/how-to-set-the-height-of-ckeditor-5-classic-editor

class MinSizePlugin
{
	private editor;

	constructor( editor )
	{
		this.editor = editor;
	}

	init()
	{
		const minHeight = this.editor.config.get('minHeight');
		const minWidth  = this.editor.config.get('minWidth' );
		//console.log((new Date()).toISOString() + ' ' + 'MinSizePlugin.init', minHeight);
		//console.log((new Date()).toISOString() + ' ' + 'MinSizePlugin.init', minWidth );
		if ( minHeight && minWidth )
		{
			this.editor.ui.view.editable.extendTemplate(
			{
				attributes:
				{
					style:
					{
						minHeight: minHeight,
						minWidth : minWidth,
					}
				}
			});
		}
	}
}


interface IHtmlEditorState
{
	ID               : string;
	FIELD_INDEX      : number;
	DATA_VALUE       : string;
	DATA_FIELD       : string;
	UI_REQUIRED      : boolean;
	FORMAT_TAB_INDEX : number;
	FORMAT_MAX_LENGTH: number;
	FORMAT_ROWS      : number;
	FORMAT_COLUMNS   : number;
	VALUE_MISSING    : boolean;
	ENABLED          : boolean;
	CSS_CLASS?       : string;
}

export default class HtmlEditor extends EditComponent<IEditComponentProps, IHtmlEditorState>
{
	private editor = React.createRef<CKEditor>();

	public get data(): any
	{
		const { DATA_FIELD, DATA_VALUE } = this.state;
		// 06/30/2019 Paul.  Return null instead of empty string. 
		let key   = DATA_FIELD;
		let value = DATA_VALUE;
		if ( Sql.IsEmptyString(value) )
		{
			value = null;
		}
		return { key, value };
	}

	public validate(): boolean
	{
		const { DATA_FIELD, DATA_VALUE, UI_REQUIRED, VALUE_MISSING, ENABLED } = this.state;
		let bVALUE_MISSING: boolean = false;
		// 08/06/2020 Paul.  Hidden fields cannot be required. 
		if ( UI_REQUIRED && !this.props.bIsHidden )
		{
			bVALUE_MISSING = Sql.IsEmptyString(DATA_VALUE);
			if ( bVALUE_MISSING != VALUE_MISSING )
			{
				this.setState({VALUE_MISSING: bVALUE_MISSING});
			}
			if ( bVALUE_MISSING && UI_REQUIRED )
			{
				console.warn((new Date()).toISOString() + ' ' + this.constructor.name + '.validate ' + DATA_FIELD);
			}
		}
		return !bVALUE_MISSING;
	}

	public updateDependancy(PARENT_FIELD: string, DATA_VALUE: any, PROPERTY_NAME?: string, item?: any): void
	{
		if ( Sql.IsEmptyString(PROPERTY_NAME) || PROPERTY_NAME == 'value' )
		{
			this.setState({ DATA_VALUE });
		}
		else if ( PROPERTY_NAME == 'enabled' )
		{
			this.setState(
			{
				ENABLED: Sql.ToBoolean(DATA_VALUE)
			});
		}
		else if ( PROPERTY_NAME == 'class' )
		{
			this.setState({ CSS_CLASS: DATA_VALUE });
		}
		else if ( PROPERTY_NAME == 'insert' )
		{
			if ( this.editor.current != null )
			{
				this.editor.current.insertText(DATA_VALUE);
			}
		}
	}

	public clear(): void
	{
		const { ENABLED } = this.state;
		// 01/11/2020.  Apply Field Level Security. 
		if ( ENABLED )
		{
			// 02/02/2020 Paul.  input does not update when DATA_VALUE is set to null. 
			this.setState(
			{
				DATA_VALUE: ''
			});
		}
	}

	constructor(props: IEditComponentProps)
	{
		super(props);
		let FIELD_INDEX      : number = 0;
		let DATA_VALUE       : string = '';
		let DATA_FIELD       : string = '';
		let UI_REQUIRED      : boolean = null;
		let FORMAT_TAB_INDEX : number = null;
		let FORMAT_MAX_LENGTH: number = null;
		let FORMAT_ROWS      : number = null;
		let FORMAT_COLUMNS   : number = null;
		let ENABLED          : boolean = props.bIsWriteable;

		let ID: string = null;
		try
		{
			const { baseId, layout, row, onChanged } = this.props;
			if (layout != null)
			{
				FIELD_INDEX       = Sql.ToInteger(layout.FIELD_INDEX      );
				DATA_FIELD        = Sql.ToString (layout.DATA_FIELD       );
				UI_REQUIRED       = Sql.ToBoolean(layout.UI_REQUIRED      ) || Sql.ToBoolean(layout.DATA_REQUIRED);
				FORMAT_TAB_INDEX  = Sql.ToInteger(layout.FORMAT_TAB_INDEX );
				FORMAT_MAX_LENGTH = Sql.ToInteger(layout.FORMAT_MAX_LENGTH);
				FORMAT_ROWS       = Sql.ToInteger(layout.FORMAT_ROWS      );
				FORMAT_COLUMNS    = Sql.ToInteger(layout.FORMAT_COLUMNS   );
				ID = baseId + '_' + DATA_FIELD;
				
				if ( FORMAT_ROWS == 0 )
				{
					FORMAT_ROWS = 120;
				}
				if ( FORMAT_COLUMNS == 0 )
				{
					FORMAT_COLUMNS = 900;
				}
				if ( row != null )
				{
					if ( row != null && row[DATA_FIELD] != null )
					{
						DATA_VALUE = Sql.ToString(row[DATA_FIELD]);
					}
				}
			}
		}
		catch(error)
		{
			console.error((new Date()).toISOString() + ' ' + this.constructor.name + '.constructor', error);
		}
		this.state =
		{
			ID               ,
			FIELD_INDEX      ,
			DATA_VALUE       ,
			DATA_FIELD       ,
			UI_REQUIRED      ,
			FORMAT_TAB_INDEX ,
			FORMAT_MAX_LENGTH,
			FORMAT_ROWS      ,
			FORMAT_COLUMNS   ,
			VALUE_MISSING    : false,
			ENABLED          ,
		};
		//document.components[sID] = this;
	}
	
	async componentDidMount()
	{
		const { DATA_FIELD } = this.state;
		if ( this.props.fieldDidMount )
		{
			this.props.fieldDidMount(DATA_FIELD, this);
		}
	}

	shouldComponentUpdate(nextProps: IEditComponentProps, nextState: IHtmlEditorState)
	{
		const { DATA_FIELD, DATA_VALUE, VALUE_MISSING, ENABLED } = this.state;
		if ( nextProps.row != this.props.row )
		{
			//console.log((new Date()).toISOString() + ' ' + this.constructor.name + '.shouldComponentUpdate ' + DATA_FIELD, DATA_VALUE, nextProps, nextState);
			return true;
		}
		else if ( nextProps.layout != this.props.layout )
		{
			//console.log((new Date()).toISOString() + ' ' + this.constructor.name + '.shouldComponentUpdate ' + DATA_FIELD, DATA_VALUE, nextProps, nextState);
			return true;
		}
		// 11/02/2019 Paul.  Hidden property is used to dynamically hide and show layout fields. 
		else if ( nextProps.bIsHidden != this.props.bIsHidden )
		{
			//console.log((new Date()).toISOString() + ' ' + this.constructor.name + '.shouldComponentUpdate ' + DATA_FIELD, DATA_VALUE, nextProps, nextState);
			return true;
		}
		else if ( nextState.DATA_VALUE != this.state.DATA_VALUE )
		{
			//console.log((new Date()).toISOString() + ' ' + this.constructor.name + '.shouldComponentUpdate ' + DATA_FIELD, DATA_VALUE, nextProps, nextState);
			return true;
		}
		else if ( nextState.VALUE_MISSING != this.state.VALUE_MISSING || nextState.ENABLED != this.state.ENABLED )
		{
			//console.log((new Date()).toISOString() + ' ' + this.constructor.name + '.shouldComponentUpdate ' + DATA_FIELD, DATA_VALUE, VALUE_MISSING, nextProps, nextState);
			return true;
		}
		else if ( nextState.CSS_CLASS != this.state.CSS_CLASS )
		{
			//console.log((new Date()).toISOString() + ' ' + this.constructor.name + '.shouldComponentUpdate ' + DATA_FIELD, CSS_CLASS, nextProps, nextState);
			return true;
		}
		return false;
	}

	private _onChange = (event, editor): void =>
	{
		const { onChanged, onUpdate } = this.props;
		const { DATA_FIELD, ENABLED } = this.state;
		let DATA_VALUE = editor.getData();
		//console.log((new Date()).toISOString() + ' ' + this.constructor.name + '._onChange ' + DATA_FIELD + ' = ' + DATA_VALUE);
		try
		{
			// 07/23/2019.  Apply Field Level Security. 
			if ( ENABLED )
			{
				this.setState({ DATA_VALUE }, this.validate);
				onChanged(DATA_FIELD, DATA_VALUE);
				onUpdate (DATA_FIELD, DATA_VALUE);
			}
		}
		catch(error)
		{
			console.error((new Date()).toISOString() + ' ' + this.constructor.name + '._onChange', error);
		}
	}

	public render()
	{
		const { baseId, layout, row, onChanged } = this.props;
		const { ID, DATA_VALUE, DATA_FIELD, UI_REQUIRED, FORMAT_TAB_INDEX, FORMAT_MAX_LENGTH, FORMAT_ROWS, FORMAT_COLUMNS, VALUE_MISSING, ENABLED, CSS_CLASS } = this.state;
		//console.log((new Date()).toISOString() + ' ' + this.constructor.name + '.render ' + DATA_FIELD);
		try
		{
			if ( layout == null )
			{
				return (<span>layout is null</span>);
			}
			else if ( Sql.IsEmptyString(DATA_FIELD) )
			{
				return (<div>DATA_FIELD is empty for FIELD_INDEX {layout.FIELD_INDEX}</div>);
			}
			else if ( onChanged == null )
			{
				return (<span>onChanged is null for DATA_FIELD { DATA_FIELD }</span>);
			}
			// 11/02/2019 Paul.  Hidden property is used to dynamically hide and show layout fields. 
			else if ( layout.hidden )
			{
				return (<span></span>);
			}
			else
			{
				let cssRequired = { paddingLeft: '4px', display: (VALUE_MISSING ? 'inline' : 'none') };
				// 08/31/2012 Paul.  Add support for speech. 
				// 04/28/2019 Paul.  Speech as been deprecated. 
				// https://ckeditor.com/docs/ckeditor5/latest/builds/guides/integration/frameworks/react.html
				// 05/19/2023 Paul.  Custom toolbar as default excludes sourceEditing. 
				let config: any =
				{
					language    : Security.USER_LANG().substring(0, 2),
					extraPlugins: [ MinSizePlugin ],
					minWidth    : FORMAT_COLUMNS + 'px',
					minHeight   : FORMAT_ROWS    + 'px',
					toolbar     :
					{
						items:
						[
							'sourceEditing',
							'heading',
							'|',
							'bold',
							'italic',
							'strikethrough',
							'underline',
							'|',
							'fontBackgroundColor',
							'fontColor',
							'fontSize',
							'fontFamily',
							'bulletedList',
							'numberedList',
							'|',
							'outdent',
							'indent',
							'alignment',
							'|',
							'link',
							'imageUpload',
							'blockQuote',
							'insertTable',
							'mediaEmbed',
							'undo',
							'redo'
						]
					},
					image:
					{
						toolbar:
						[
							'imageTextAlternative',
							'toggleImageCaption',
							'imageStyle:inline',
							'imageStyle:block',
							'imageStyle:side',
							'linkImage'
						]
					},
					table:
					{
						contentToolbar:
						[
							'tableColumn',
							'tableRow',
							'mergeTableCells'
						]
					}
				};

				// https://ckeditor.com/docs/ckeditor5/latest/api/module_core_editor_editorconfig-EditorConfig.html
				return (
					<span className={ CSS_CLASS }>
						<CKEditor
							key={ ID }
							editor={ ClassicEditor }
							data={ DATA_VALUE }
							config={ config }
							onChange={ this._onChange }
							disabled={ !ENABLED }
							ref={ this.editor }
						/>
						{ UI_REQUIRED ? <span id={ ID + '_REQUIRED' } key={ ID + '_REQUIRED' } className="required" style={ cssRequired } >{ L10n.Term('.ERR_REQUIRED_FIELD') }</span> : null}
					</span>
				);
			}
		}
		catch(error)
		{
			console.error((new Date()).toISOString() + ' ' + this.constructor.name + '.render', error);
			return (<span>{ error.message }</span>);
		}
	}
}

