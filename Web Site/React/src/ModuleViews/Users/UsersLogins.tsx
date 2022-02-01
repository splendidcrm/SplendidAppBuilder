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
import { RouteComponentProps, withRouter } from 'react-router-dom'                    ;
// 2. Store and Types. 
import DETAILVIEWS_RELATIONSHIP            from '../../types/DETAILVIEWS_RELATIONSHIP';
// 3. Scripts. 
// 4. Components and Views. 
import SubPanelView                        from '../../views/SubPanelView'            ;

interface IUsersLoginsProps extends RouteComponentProps<any>
{
	PARENT_TYPE      : string;
	row              : any;
	layout           : DETAILVIEWS_RELATIONSHIP;
}

class UsersLogins extends React.Component<IUsersLoginsProps>
{
	constructor(props: IUsersLoginsProps)
	{
		super(props);
		//console.log((new Date()).toISOString() + ' ' + this.constructor.name + '.constructor ' + props.PARENT_TYPE, props.layout);
	}

	public render()
	{
		return <SubPanelView { ...this.props } disableView={ true } disableEdit={ true } disableRemove={ true } CONTROL_VIEW_NAME='Users.Logins' />;
	}
}

export default withRouter(UsersLogins);

