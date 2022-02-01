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
import { Route, Switch, Redirect }     from 'react-router-dom'                                                     ;
// 4. Components and Views. 
import PrivateRoute                   from './PrivateRoute'                                                        ;
import App                            from './App'                                                                 ;
import DashboardView                  from './views/DashboardView'                                                 ;
import DashboardEditView              from './views/DashboardEditView'                                             ;
import HomeDashboardEditView          from './views/HomeDashboardEditView'                                         ;
import DynamicDetailView              from './views/DynamicDetailView'                                             ;
import DynamicEditView                from './views/DynamicEditView'                                               ;
import DynamicListView                from './views/DynamicListView'                                               ;
import DynamicLayoutView              from './views/DynamicLayoutView'                                             ;
import AdministrationView             from './views/AdministrationView'                                            ;
import AdminDynamicDetailView         from './views/DynamicAdminDetailView'                                        ;
import AdminDynamicEditView           from './views/DynamicAdminEditView'                                          ;
import AdminDynamicListView           from './views/DynamicAdminListView'                                          ;
import AdminReadOnlyListView          from './views/AdminReadOnlyListView'                                         ;
import AdminReadOnlyConfigView        from './views/AdminReadOnlyConfigView'                                       ;
import AdminConfigView                from './views/AdminConfigView'                                               ;
import LoginView                      from './views/LoginView'                                                     ;
import HomeView                       from './views/HomeView'                                                      ;
import RootView                       from './views/RootView'                                                      ;
import ResetView                      from './views/ResetView'                                                     ;
import ReloadView                     from './views/ReloadView'                                                    ;
import UnifiedSearch                  from './views/UnifiedSearch'                                                 ;
import ImportView                     from './views/ImportView'                                                    ;
import PlaceholderView                from './views/PlaceholderView'                                               ;

import AboutView                      from './ModuleViews/Home/AboutView'                                          ;
import MyAccountView                  from './ModuleViews/Users/MyAccountView'                                     ;
import MyAccountEdit                  from './ModuleViews/Users/MyAccountEdit'                                     ;
import UserWizard                     from './ModuleViews/Users/Wizard'                                            ;

import Precompile                     from './ModuleViews/Administration/_devtools/Precompile'                     ;
import AdminPasswordManager           from './ModuleViews/Administration/PasswordManager/ConfigView'               ;
import ACLRolesByUser                 from './ModuleViews/Administration/ACLRoles/ByUser'                          ;
import AdminRenameTabs                from './ModuleViews/Administration/RenameTabs/ListView'                      ;
import AdminConfigureTabs             from './ModuleViews/Administration/ConfigureTabs/ListView'                   ;
import SystemLogListView              from './ModuleViews/Administration/SystemLog/ListView'                       ;
import UserLoginsListView             from './ModuleViews/Administration/UserLogins/ListView'                      ;
import ConfiguratorAdminWizard        from './ModuleViews/Administration/Configurator/AdminWizard'                 ;

import AdminDynamicLayout             from './DynamicLayoutComponents/DynamicLayoutEditor'                         ;
import GoogleOAuth                    from './views/GoogleOAuth'                                                   ;
import Office365OAuth                 from './views/Office365OAuth'                                                ;
import TerminologyImportView          from './ModuleViews/Administration/Terminology/ImportView'                   ;
import DatabaseImportView             from './ModuleViews/Administration/Import/ImportView'                        ;
import ModuleBuilderWizardView        from './ModuleBuilder/WizardView'                                            ;

// 02/15/2020 Paul.  To debug the router, use DebugRouter in index.tsx. 
export const routes = (
	<App>
		<Switch>
			<Route        exact path="/login"                                                    component={LoginView} />
			<Redirect     exact from="/Administration/Users/EditMyAccount"                       to="/Users/EditMyAccount" />

			<PrivateRoute       path="/Administration/_devtools/Precompile"                      component={Precompile} />

			<PrivateRoute exact path="/GoogleOAuth"                                              component={GoogleOAuth} />
			<PrivateRoute exact path="/Office365OAuth"                                           component={Office365OAuth} />
			<PrivateRoute exact path="/Home/About"                                               component={AboutView} />
			<PrivateRoute exact path="/Home/DashboardEdit/:ID"                                   component={HomeDashboardEditView} />
			<PrivateRoute exact path="/Home/DashboardEdit"                                       component={HomeDashboardEditView} />
			<PrivateRoute exact path="/Home/:ID"                                                 component={HomeView} />
			<PrivateRoute exact path="/Home"                                                     component={HomeView} />
			<PrivateRoute exact path="/Reset/*"                                                  component={ResetView } />
			<PrivateRoute exact path="/Reload"                                                   component={ReloadView} />
			<PrivateRoute exact path="/Reload/*"                                                 component={ReloadView} />
			<PrivateRoute exact path="/Dashboard/DashboardEdit/:ID"                              component={DashboardEditView} />
			<PrivateRoute exact path="/Dashboard/DashboardEdit"                                  component={DashboardEditView} />
			<PrivateRoute exact path="/Dashboard/:ID"                                            component={DashboardView} />
			<PrivateRoute exact path="/Dashboard"                                                component={DashboardView} />
			<PrivateRoute exact path="/UnifiedSearch/:search"                                    component={UnifiedSearch} />

			<PrivateRoute exact path="/Users/MyAccount"                                          component={MyAccountView} />
			<PrivateRoute exact path="/Users/EditMyAccount"                                      component={MyAccountEdit} />
			<PrivateRoute exact path="/Users/Wizard"                                             component={UserWizard} />

			<PrivateRoute       path="/Administration/Config/PasswordManager"                    component={AdminPasswordManager} />
			<PrivateRoute       path="/Administration/Terminology/TerminologyImport"             component={TerminologyImportView} />
			<PrivateRoute       path="/Administration/ModuleBuilder"                             component={ModuleBuilderWizardView} />

			<PrivateRoute       path="/Administration/Configurator"                              component={ConfiguratorAdminWizard} />

			<PrivateRoute exact path="/Administration/DynamicLayout/AdminDynamicLayout"          component={AdminDynamicLayout} />
			<PrivateRoute exact path="/Administration/Terminology/RenameTabs"                    component={AdminRenameTabs} />
			<PrivateRoute exact path="/Administration/Terminology/ConfigureTabs"                 component={AdminConfigureTabs} />

			<PrivateRoute       path="/Administration/SystemLog"                                 component={SystemLogListView} />
			<PrivateRoute       path="/Administration/UserLogins"                                component={UserLoginsListView} />
			<PrivateRoute       path="/Administration/AuditEvents"                               component={AdminReadOnlyListView} />

			<PrivateRoute exact path="/Administration/ACLRoles/ByUser"                           component={ACLRolesByUser} />
			<PrivateRoute exact path="/Administration/:MODULE_NAME/ReadOnlyListView"             component={AdminReadOnlyListView} />
			<PrivateRoute exact path="/Administration/:MODULE_NAME/DetailView"                   component={AdminReadOnlyConfigView} />
			<PrivateRoute exact path="/Administration/:MODULE_NAME/ConfigView"                   component={AdminConfigView} />
			<PrivateRoute exact path="/Administration/:MODULE_NAME/Config"                       component={AdminConfigView} />
			<PrivateRoute exact path="/Administration/:MODULE_NAME/List"                         component={AdminDynamicListView} />
			<PrivateRoute exact path="/Administration/:MODULE_NAME/View/:ID"                     component={AdminDynamicDetailView} />
			<PrivateRoute exact path="/Administration/:MODULE_NAME/Duplicate/:DuplicateID"       component={AdminDynamicEditView} />
			<PrivateRoute exact path="/Administration/:MODULE_NAME/Edit/:ID"                     component={AdminDynamicEditView} />
			<PrivateRoute exact path="/Administration/:MODULE_NAME/Edit"                         component={AdminDynamicEditView} />
			<PrivateRoute exact path="/Administration/:MODULE_NAME/Import"                       component={ImportView} />

			<PrivateRoute exact path="/Administration/:MODULE_NAME/"                             component={AdminDynamicListView} />
			<PrivateRoute exact path="/Administration"                                           component={AdministrationView} />

			<PrivateRoute exact path="/:MODULE_NAME/List"                                        component={DynamicListView} />
			<PrivateRoute exact path="/:MODULE_NAME/View/:ID"                                    component={DynamicDetailView} />
			<PrivateRoute exact path="/:MODULE_NAME/Duplicate/:DuplicateID"                      component={DynamicEditView} />
			<PrivateRoute exact path="/:MODULE_NAME/Convert/:ConvertModule/:ConvertID"           component={DynamicEditView} />
			<PrivateRoute exact path="/:MODULE_NAME/Edit/:ID"                                    component={DynamicEditView} />
			<PrivateRoute exact path="/:MODULE_NAME/Edit"                                        component={DynamicEditView} />
			<PrivateRoute exact path="/:MODULE_NAME/Import"                                      component={ImportView} />

			<PrivateRoute exact path="/:MODULE_NAME/:VIEW_NAME/:ID"                              component={DynamicLayoutView} />
			<PrivateRoute exact path="/:MODULE_NAME/:VIEW_NAME"                                  component={DynamicLayoutView} />

			<PrivateRoute exact path="/:MODULE_NAME/"                                            component={DynamicListView}   />
			<Route exact path="/" component={RootView} />
			<Route render={(props) => <div>{JSON.stringify(props)}</div>} />
		</Switch>
	</App>
);

