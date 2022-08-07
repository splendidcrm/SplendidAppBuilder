/*
 * Copyright (C) 2005-2022 SplendidCRM Software, Inc. 
 * MIT License
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
 * files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
 * modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
 * is furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 */

// 05/01/2019 Paul.  We need a flag so that the React client can determine if the module is Process enabled. 
// 07/31/2019 Paul.  DEFAULT_SORT is a new field for the React Client. 
// 08/12/2019 Paul.  ARCHIVED_ENBLED is needed for the dynamic buttons. 
// 12/03/2019 Paul.  Separate Archive View exists flag so that we can display information on DetailView. 
// 06/26/2021 Paul.  IS_ASSIGNED is available in vwMODULES_AppVars. 
export default interface MODULE
{
	ID                            : string ;  // uniqueidentifier
	NAME                          : string ;  // nvarchar
	MODULE_NAME                   : string ;  // nvarchar
	DISPLAY_NAME                  : string ;  // nvarchar
	RELATIVE_PATH                 : string ;  // nvarchar
	MODULE_ENABLED                : boolean;  // bit
	TAB_ENABLED                   : boolean;  // bit
	TAB_ORDER                     : number;   // int
	PORTAL_ENABLED                : boolean;  // bit
	CUSTOM_ENABLED                : boolean;  // bit
	IS_ADMIN                      : boolean;  // bit
	TABLE_NAME                    : string ;  // nvarchar
	REPORT_ENABLED                : boolean;  // bit
	IMPORT_ENABLED                : boolean;  // bit
	SYNC_ENABLED                  : boolean;  // bit
	MOBILE_ENABLED                : boolean;  // bit
	CUSTOM_PAGING                 : boolean;  // bit
	DATE_MODIFIED                 : Date   ;  // datetime
	DATE_MODIFIED_UTC             : Date   ;  // datetime
	MASS_UPDATE_ENABLED           : boolean;  // bit
	DEFAULT_SEARCH_ENABLED        : boolean;  // bit
	EXCHANGE_SYNC                 : boolean;  // bit
	EXCHANGE_FOLDERS              : boolean;  // bit
	EXCHANGE_CREATE_PARENT        : boolean;  // bit
	REST_ENABLED                  : boolean;  // bit
	DUPLICATE_CHECHING_ENABLED    : boolean;  // bit
	RECORD_LEVEL_SECURITY_ENABLED : boolean;  // bit
	PROCESS_ENABLED               : boolean;  // bit
	DEFAULT_SORT                  : string ;  // nvarchar
	ARCHIVED_ENBLED               : boolean;  // bit
	ARCHIVED_VIEW_EXISTS          : boolean;  // bit
	STREAM_ENBLED                 : boolean;  // bit
	IS_ASSIGNED                   : boolean;  // bit
}

