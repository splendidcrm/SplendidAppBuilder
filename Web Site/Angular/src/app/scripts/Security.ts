/*
 * Copyright (C) 2005-2022 SplendidCRM Software, Inc. 
 * MIT License
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
 * files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
 * modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
 * is furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 */
import { Injectable             } from '@angular/core'                      ;
import { SplendidCacheService   } from '../scripts/SplendidCache'           ;
import { CredentialsService     } from '../scripts/Credentials'             ;
import Sql                        from '../scripts/Sql'                     ;

@Injectable({
	providedIn: 'root'
})
export class SecurityService
{
	constructor(protected SplendidCache: SplendidCacheService, protected Credentials: CredentialsService)
	{
	}

	public IS_ADMIN()
	{
		return this.Credentials.bIS_ADMIN || this.Credentials.bIS_ADMIN_DELEGATE;
	}
	
	public IS_ADMIN_DELEGATE()
	{
		return this.Credentials.bIS_ADMIN_DELEGATE;
	}
	
	public USER_ID()
	{
		return this.SplendidCache.UserID;
	}
	
	public USER_NAME()
	{
		return this.SplendidCache.UserName;
	}
	
	public FULL_NAME()
	{
		return this.SplendidCache.FullName;
	}
	
	// 11/25/2014 Paul.  sPICTURE is used by the ChatDashboard. 
	public PICTURE()
	{
		return this.SplendidCache.Picture;
	}
	
	public USER_LANG()
	{
		return this.SplendidCache.UserLang;
	}
	
	// 04/23/2013 Paul.  The HTML5 Offline Client now supports Atlantic theme. 
	public USER_THEME()
	{
		return this.SplendidCache.UserTheme;
	}
	
	public USER_DATE_FORMAT()
	{
		return this.SplendidCache.UserDateFormat;
	}
	
	public USER_TIME_FORMAT()
	{
		return this.SplendidCache.UserTimeFormat;
	}
	
	public TEAM_ID()
	{
		return this.SplendidCache.TeamID;
	}
	
	public TEAM_NAME()
	{
		return this.SplendidCache.TeamName;
	}
	
	// 02/26/2016 Paul.  Use values from C# NumberFormatInfo. 
	public NumberFormatInfo()
	{
		// 10/29/2020 Paul.  Clone so that the decimal digits can be modified safely. 
		return Object.assign({}, this.SplendidCache.NumberFormatInfo);
	}

	public HasExchangeAlias()
	{
		return !Sql.IsEmptyString(this.Credentials.sEXCHANGE_ALIAS);
	}

	// 01/22/2021 Paul.  Customizations may be based on the PRIMARY_ROLE_ID and not the name. 
	public PRIMARY_ROLE_ID(): string
	{
		return this.Credentials.sPRIMARY_ROLE_ID;
	}
	
	public PRIMARY_ROLE_NAME(): string
	{
		return this.Credentials.sPRIMARY_ROLE_NAME;
	}
	
	// 01/22/2021 Paul.  Some customizations may be dependent on role name. 
	public GetACLRoleAccess(sNAME: string) : boolean
	{
		return this.SplendidCache.GetACLRoleAccess(sNAME);
	}

	// 03/29/2021 Paul.  Allow display of impersonation state. 
	public IsImpersonating(): boolean
	{
		return this.Credentials.USER_IMPERSONATION;
	}
}
