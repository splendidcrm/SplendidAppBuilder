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
// 2. Types
// 3. Scripts
import Sql                       from './Sql'                  ;
import L10n                      from './L10n'                 ;
import Credentials               from './Credentials'          ;
import SplendidCache             from './SplendidCache'        ;
import ModuleViewFactory         from '../ModuleViews'         ;
import { EditView_LoadLayout }   from './EditView'             ;
import { DynamicLayout_Compile } from './DynamicLayout_Compile';
// 4. Components and Views. 

export async function DynamicLayout_Module(sMODULE_NAME:string, sLAYOUT_TYPE:string, sVIEW_NAME:string)
{
	try
	{
		let view = null;
		let sLAYOUT_NAME: string = sMODULE_NAME + '.' + sVIEW_NAME;
		if ( sLAYOUT_TYPE != null )
		{
			let responseText: string = SplendidCache.ReactCustomViews(sMODULE_NAME, sLAYOUT_TYPE, sVIEW_NAME);
			if ( responseText != null )
			{
				// 02/02/2020 Paul.  Comment out includes so that we don't need to wrap code in a function. 
				responseText = responseText.replace(/\r\nimport/g, '\r\n//import');
				// 01/22/2021 Paul.  We are getting an error ever since we upgraded mobx. 
				// Cannot read property 'componentWillReact' of undefined
				responseText = responseText.replace('\r\n@observer', '\r\n/*@observer*/');
				view = SplendidCache.CompiledCustomViews(sMODULE_NAME, sLAYOUT_TYPE, sVIEW_NAME);
				if ( view == null )
				{
					//console.log((new Date()).toISOString() + ' ' + 'DynamicLayout_Module', sLAYOUT_NAME);
					// 04/19/2020 Paul.  Move Babel transform to a separate file. 
					view = await DynamicLayout_Compile(responseText);
					SplendidCache.SetCompiledCustomView(sMODULE_NAME, sLAYOUT_TYPE, sVIEW_NAME, view);
				}
				if ( sLAYOUT_TYPE == 'EditViews' )
				{
					await DynamicLayout_PrecompilePopupModules(sLAYOUT_NAME);
				}
				return view;
			}
		}
		else
		{
			// 08/03/2019 Paul.  The DynamicLayoutView will not know the layout type in advance, so we need to try all the types. 
			let arrLAYOUT_TYPES: string[] = ['DetailViews', 'EditViews', 'ListViews', 'SubPanels'];
			for ( let i = 0; i < arrLAYOUT_TYPES.length; i++ )
			{
				let responseText: string = SplendidCache.ReactCustomViews(sMODULE_NAME, arrLAYOUT_TYPES[i], sVIEW_NAME);
				if ( responseText != null )
				{
					// 02/02/2020 Paul.  Comment out includes so that we don't need to wrap code in a function. 
					responseText = responseText.replace(/\r\nimport/g, '\r\n//import');
					// 01/22/2021 Paul.  We are getting an error ever since we upgraded mobx. 
					// Cannot read property 'componentWillReact' of undefined
					responseText = responseText.replace('\r\n@observer', '\r\n/*@observer*/');
					view = SplendidCache.CompiledCustomViews(sMODULE_NAME, arrLAYOUT_TYPES[i], sVIEW_NAME);
					if ( view == null )
					{
						//console.log((new Date()).toISOString() + ' ' + 'DynamicLayout_Module', sLAYOUT_NAME);
						// 04/19/2020 Paul.  Move Babel transform to a separate file. 
						view = await DynamicLayout_Compile(responseText);
						SplendidCache.SetCompiledCustomView(sMODULE_NAME, arrLAYOUT_TYPES[i], sVIEW_NAME, view);
					}
					if ( arrLAYOUT_TYPES[i] != 'SubPanels' )
					{
						// 01/19/2020 Paul.  Remove the 's at the end of the type. 
						Credentials.SetViewMode(arrLAYOUT_TYPES[i].substring(0, arrLAYOUT_TYPES[i].length-1));
					}
					if ( arrLAYOUT_TYPES[i] == 'EditViews' )
					{
						await DynamicLayout_PrecompilePopupModules(sLAYOUT_NAME);
					}
					return view;
				}
			}
		}
		view = ModuleViewFactory(sLAYOUT_NAME);
		// 08/11/2020 Paul.  If inline does not exist, then use the base edit view. 
		if ( view == null && sVIEW_NAME == 'EditView.Inline' )
		{
			view = ModuleViewFactory(sMODULE_NAME + '.EditView');
		}
		if ( sLAYOUT_TYPE == 'EditViews' || sVIEW_NAME == 'EditView' || sVIEW_NAME == 'EditView.Inline' )
		{
			await DynamicLayout_PrecompilePopupModules(sLAYOUT_NAME);
		}
		return view;
	}
	catch(error)
	{
		console.error((new Date()).toISOString() + ' ' + 'DynamicLayout_Module ' + sLAYOUT_TYPE + ' ' + sMODULE_NAME + '.' + sVIEW_NAME, error);
		//throw('DynamicLayout_Module ' + sMODULE_NAME + '.' + sLAYOUT_NAME + ': ' + e.message);
	}
	return null;
}

async function DynamicLayout_PrecompilePopupModules(EDIT_NAME:string)
{
	// 02/02/2020 Paul.  Ignore missing during DynamicLayout. 
	let layout = EditView_LoadLayout(EDIT_NAME, true);
	if ( layout != null )
	{
		//console.log((new Date()).toISOString() + ' ' + 'DynamicLayout_PrecompilePopupModules Inspecting ' + EDIT_NAME);
		for ( let nLayoutIndex = 0; nLayoutIndex < layout.length; nLayoutIndex++ )
		{
			let lay = layout[nLayoutIndex];
			let FIELD_TYPE : string = lay.FIELD_TYPE ;
			let DATA_LABEL : string = lay.DATA_LABEL ;
			let MODULE_TYPE: string = lay.MODULE_TYPE;
			let sVIEW_NAME : string = 'PopupView.Inline';
			if ( FIELD_TYPE == 'ModulePopup' )
			{
				if ( !Sql.IsEmptyString(MODULE_TYPE) )
				{
					let sMODULE_NAME: string = MODULE_TYPE;
					let sLAYOUT_NAME: string = sMODULE_NAME + '.' + sVIEW_NAME;
					//console.log((new Date()).toISOString() + ' ' + 'DynamicLayout_PrecompilePopupModules Checking for ' + sLAYOUT_NAME);
					let responseText: string = SplendidCache.ReactCustomViews(sMODULE_NAME, 'EditViews', sVIEW_NAME);
					if ( responseText != null )
					{
						// 02/02/2020 Paul.  Comment out includes so that we don't need to wrap code in a function. 
						responseText = responseText.replace(/\r\nimport/g, '\r\n//import');
						let view = SplendidCache.CompiledCustomViews(sMODULE_NAME, 'EditViews', sVIEW_NAME);
						if ( view == null )
						{
							//console.log((new Date()).toISOString() + ' ' + 'DynamicLayout_PrecompilePopupModules Compiling ' + sLAYOUT_NAME);
							try
							{
								// 04/19/2020 Paul.  Move Babel transform to a separate file. 
								view = await DynamicLayout_Compile(responseText);
								SplendidCache.SetCompiledCustomView(sMODULE_NAME, 'EditViews', sVIEW_NAME, view);
							}
							catch(error)
							{
								console.error((new Date()).toISOString() + ' ' + 'DynamicLayout_PrecompilePopupModules ' + sLAYOUT_NAME, error);
							}
						}
					}
				}
			}
			else if ( FIELD_TYPE == 'ChangeButton' )
			{
				if ( DATA_LABEL == 'PARENT_TYPE' )
				{
					let LIST_NAME: string = 'record_type_display';
					let arrLIST: string[] = L10n.GetList(LIST_NAME);
					if ( arrLIST != null )
					{
						for ( let i = 0; i < arrLIST.length; i++ )
						{
							let sMODULE_NAME: string = arrLIST[i];
							let sLAYOUT_NAME: string = sMODULE_NAME + '.PopupView.Inline';
							//console.log((new Date()).toISOString() + ' ' + 'DynamicLayout_PrecompilePopupModules Checking for ' + sLAYOUT_NAME);
							let responseText: string = SplendidCache.ReactCustomViews(sMODULE_NAME, 'EditViews', sVIEW_NAME);
							if ( responseText != null )
							{
								// 02/02/2020 Paul.  Comment out includes so that we don't need to wrap code in a function. 
								responseText = responseText.replace(/\r\nimport/g, '\r\n//import');
								let view = SplendidCache.CompiledCustomViews(sMODULE_NAME, 'EditViews', sVIEW_NAME);
								if ( view == null )
								{
									//console.log((new Date()).toISOString() + ' ' + 'DynamicLayout_PrecompilePopupModules Compiling ' + sLAYOUT_NAME);
									try
									{
										// 04/19/2020 Paul.  Move Babel transform to a separate file. 
										view = await DynamicLayout_Compile(responseText);
										SplendidCache.SetCompiledCustomView(sMODULE_NAME, 'EditViews', sVIEW_NAME, view);
									}
									catch(error)
									{
										console.error((new Date()).toISOString() + ' ' + 'DynamicLayout_PrecompilePopupModules ' + sLAYOUT_NAME, error);
									}
								}
							}
						}
					}
				}
				else if ( !Sql.IsEmptyString(MODULE_TYPE) )
				{
					let sMODULE_NAME: string = MODULE_TYPE;
					let sLAYOUT_NAME: string = sMODULE_NAME + '.' + sVIEW_NAME;
					//console.log((new Date()).toISOString() + ' ' + 'DynamicLayout_PrecompilePopupModules Checking for ' + sLAYOUT_NAME);
					let responseText: string = SplendidCache.ReactCustomViews(sMODULE_NAME, 'EditViews', sVIEW_NAME);
					if ( responseText != null )
					{
						// 02/02/2020 Paul.  Comment out includes so that we don't need to wrap code in a function. 
						responseText = responseText.replace(/\r\nimport/g, '\r\n//import');
						let view = SplendidCache.CompiledCustomViews(sMODULE_NAME, 'EditViews', sVIEW_NAME);
						if ( view == null )
						{
							//console.log((new Date()).toISOString() + ' ' + 'DynamicLayout_PrecompilePopupModules Compiling ' + sLAYOUT_NAME);
							try
							{
								// 04/19/2020 Paul.  Move Babel transform to a separate file. 
								view = await DynamicLayout_Compile(responseText);
								SplendidCache.SetCompiledCustomView(sMODULE_NAME, 'EditViews', sVIEW_NAME, view);
							}
							catch(error)
							{
								console.error((new Date()).toISOString() + ' ' + 'DynamicLayout_PrecompilePopupModules ' + sLAYOUT_NAME, error);
							}
						}
					}
				}
			}
		}
	}
}


