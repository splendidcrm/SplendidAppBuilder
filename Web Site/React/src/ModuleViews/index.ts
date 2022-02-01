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
// 2. Store and Types. 
// 3. Scripts. 
// 4. Components and Views. 
// 5. Dashlets
import UsersDetailView           from './Users/DetailView'          ;
import UsersEditView             from './Users/EditView'            ;
import UsersPopupView            from './Users/PopupView'           ;
import UsersLogins               from './Users/UsersLogins'         ;
import UsersACLRoles             from './Users/UsersACLRoles'       ;
import UsersTeams                from './Users/UsersTeams'          ;
// Administration 
import AdminConfigView           from '../views/AdminConfigView'                  ;
import AdminReadOnlyConfigView   from '../views/AdminReadOnlyConfigView'          ;
import AdminReadOnlyListView     from '../views/AdminReadOnlyListView'            ;
import DropdownListView          from './Administration/Dropdown/ListView'        ;
import DropdownEditView          from './Administration/Dropdown/EditView'        ;
import TeamsUsers                from './Administration/Teams/TeamsUsers'         ;
import ACLRolesUsers             from './Administration/ACLRoles/ACLRolesUsers'   ;
import ACLRolesListView          from './Administration/ACLRoles/ListView'        ;
import ACLRolesEditView          from './Administration/ACLRoles/EditView'        ;
import ACLRolesDetailView        from './Administration/ACLRoles/DetailView'      ;
import ACLRolesByUser            from './Administration/ACLRoles/ByUser'          ;
import EditCustomFieldsListView  from './Administration/EditCustomFields/ListView';
import EditCustomFieldsEditView  from './Administration/EditCustomFields/EditView';
import DynamicButtonsListView    from './Administration/DynamicButtons/ListView'  ;
import FieldValidatorsListView   from './Administration/FieldValidators/ListView' ;
import LanguagesListView         from './Administration/Languages/ListView'       ;
import ModulesDetailView         from './Administration/Modules/DetailView'       ;
import CurrenciesListView        from './Administration/Currencies/ListView'      ;
import CurrenciesDetailView      from './Administration/Currencies/DetailView'    ;
import CurrenciesPopupView       from './Administration/Currencies/PopupView'     ;
import ConfigListView            from './Administration/Config/ListView'          ;
import ConfigEditView            from './Administration/Config/EditView'          ;
import SchedulersDetailView      from './Administration/Schedulers/DetailView'    ;
import UndeleteListView                 from './Administration/Undelete/ListView'                  ;
// Admin logs
import SystemLogListView                 from './Administration/SystemLog/ListView'                ;
import UserLoginsListView                from './Administration/UserLogins/ListView'               ;

export default function ModuleViewFactory(sLAYOUT_NAME: string)
{
	let view = null;
	switch ( sLAYOUT_NAME )
	{
		case 'Users.DetailView'          :  view = UsersDetailView          ;  break;
		case 'Users.EditView'            :  view = UsersEditView            ;  break;
		case 'Users.PopupView'           :  view = UsersPopupView           ;  break;
		case 'Users.Logins'              :  view = UsersLogins              ;  break;
		case 'Users.ACLRoles'            :  view = UsersACLRoles            ;  break;
		case 'Users.Teams'               :  view = UsersTeams               ;  break;
		// Administration 
		case 'Dropdown.ListView'                :  view = DropdownListView         ;  break;
		case 'Dropdown.EditView'                :  view = DropdownEditView         ;  break;
		case 'Teams.Users'                      :  view = TeamsUsers               ;  break;
		case 'ACLRoles.Users'                   :  view = ACLRolesUsers            ;  break;
		case 'ACLRoles.ListView'                :  view = ACLRolesListView         ;  break;
		case 'ACLRoles.EditView'                :  view = ACLRolesEditView         ;  break;
		case 'ACLRoles.DetailView'              :  view = ACLRolesDetailView       ;  break;
		case 'ACLRoles.ByUser'                  :  view = ACLRolesByUser           ;  break;
		case 'EditCustomFields.ListView'        :  view = EditCustomFieldsListView ;  break;
		case 'EditCustomFields.EditView'        :  view = EditCustomFieldsEditView ;  break;
		case 'DynamicButtons.ListView'          :  view = DynamicButtonsListView   ;  break;
		case 'FieldValidators.ListView'         :  view = FieldValidatorsListView  ;  break;
		case 'Languages.ListView'               :  view = LanguagesListView        ;  break;
		case 'Modules.DetailView'               :  view = ModulesDetailView        ;  break;
		case 'Currencies.ListView'              :  view = CurrenciesListView       ;  break;
		case 'Currencies.DetailView'            :  view = CurrenciesDetailView     ;  break;
		case 'Currencies.PopupView'             :  view = CurrenciesPopupView      ;  break;
		case 'Config.ListView'                  :  view = ConfigListView           ;  break;
		case 'Config.EditView'                  :  view = ConfigEditView           ;  break;
		case 'Schedulers.DetailView'            :  view = SchedulersDetailView     ;  break;
		case 'Undelete.ListView'                :  view = UndeleteListView                ;  break;
		// Admin Logs
		case 'SystemLog.ListView'               :  view = SystemLogListView               ;  break;
		case 'UserLogins.ListView'              :  view = UserLoginsListView              ;  break;
		case 'AuditEvents.ListView'             :  view = AdminReadOnlyListView           ;  break;
	}
	if ( view )
	{
		//console.log((new Date()).toISOString() + ' ' + 'ModuleViewFactory found ' + sLAYOUT_NAME);
	}
	else
	{
		//console.log((new Date()).toISOString() + ' ' + 'ModuleViewFactory NOT found ' + sLAYOUT_NAME);
	}
	return view;
}

