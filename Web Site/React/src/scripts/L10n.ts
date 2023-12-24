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
// 2. Store and Types. 
// 3. Scripts. 
import SplendidCache from '../scripts/SplendidCache';
import Sql from '../scripts/Sql';

export default class L10n
{
	static GetList(sLIST_NAME: string)
	{
		return SplendidCache.TerminologyList(sLIST_NAME);
	}

	// 02/22/2013 Paul.  We need a way to get the list values, such as month names. 
	static GetListTerms(sLIST_NAME)
	{
		var arrTerms = new Array();
		var arrList  = SplendidCache.TerminologyList(sLIST_NAME);
		if ( arrList != null )
		{
			for ( var i = 0; i < arrList.length; i++ )
			{
				var sEntryName = '.' + sLIST_NAME + '.' + arrList[i];
				var sTerm = SplendidCache.Terminology(sEntryName);
				if ( sTerm == null )
					sTerm = '';
				arrTerms.push(sTerm);
			}
		}
		return arrTerms;
	}

	static Term(sEntryName: string)
	{
		try
		{
			var sTerm = SplendidCache.Terminology(sEntryName);
			if ( sTerm == null )
			{
				if ( sEntryName != '+' && SplendidCache.IsInitialized )
				{
					console.log('Term not found: ' + sEntryName);
				}
				return sEntryName;
			}
			return sTerm;
		}
		catch(error)
		{
			// 12/31/2017 Paul.  Change from alert to error. 
			console.error((new Date()).toISOString() + ' ' + this.constructor.name + '.Term ' + sEntryName, error);
		}
		return sEntryName;
	}

	// 10/27/2012 Paul.  It is normal for a list term to return an empty string. 
	static ListTerm(sLIST_NAME: string, sNAME: string)
	{
		// 06/19/2020 Paul.  L10n.Term() function to return empty string if name is empty. 
		// 07/12/2021 Paul.  There are some lists that have NULL as the first item. (survey_question_validation, bpmn_duration_units, dom_sms_opt_in_search, saved_reports_dom). 
		// 07/12/2021 Paul.  Not ready to make a breaking change at this time.  Instead, just correct at location of use. 
		if ( Sql.IsEmptyString(sNAME) )
			return '';
		let sEntryName: string = '.' + sLIST_NAME + '.' + Sql.ToString(sNAME);
		// 07/14/2019 Paul.  SQL Server returns booleans as 1 and 0, but the web server is returning true/false.  Convert back. 
		if ( typeof sNAME == 'boolean' )
		{
			sEntryName = '.' + sLIST_NAME + '.' + (sNAME ? '1' : '0');
		}
		try
		{
			var sTerm = SplendidCache.Terminology(sEntryName);
			if ( sTerm == null )
			{
				if ( !Sql.IsEmptyString(sNAME) )
				{
					if ( SplendidCache.IsInitialized )
					{
						//console.log('Term not found: ' + sEntryName);
					}
					return sEntryName;
				}
				else
				{
					sTerm = '';
				}
			}
			return sTerm;
		}
		catch(error)
		{
			console.error((new Date()).toISOString() + ' ' + this.constructor.name + '.ListTerm ' + sLIST_NAME + ' ' + sNAME, error);
		}
		return sEntryName;
	}

	static BuildTermName(sModule: string, sColumnName: string)
	{
		// 05/16/2016 Paul.  Add Tags module. 
		// 08/20/2016 Paul.  PENDING_PROCESS_ID should be a global term. 
		// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
		// 04/19/2018 Paul.  MODIFIED_BY_ID is not the correct name, use MODIFIED_USER_ID instead. 
		// 07/18/2018 Paul.  Add LBL_ARCHIVE_BY. 
		// 10/08/2022 Paul.  LBL_EXCHANGE_FOLDER is global. 
		let sTERM_NAME: string = '';
		if (  sColumnName == 'ID'              
		   || sColumnName == 'DELETED'         
		   || sColumnName == 'CREATED_BY'      
		   || sColumnName == 'CREATED_BY_ID'   
		   || sColumnName == 'CREATED_BY_NAME' 
		   || sColumnName == 'DATE_ENTERED'    
		   || sColumnName == 'MODIFIED_USER_ID'
		   || sColumnName == 'DATE_MODIFIED'   
		   || sColumnName == 'DATE_MODIFIED_UTC'
		   || sColumnName == 'MODIFIED_BY'     
		   || sColumnName == 'MODIFIED_USER_ID'  
		   || sColumnName == 'MODIFIED_BY_NAME'
		   || sColumnName == 'ASSIGNED_USER_ID'
		   || sColumnName == 'ASSIGNED_TO'     
		   || sColumnName == 'ASSIGNED_TO_NAME'
		   || sColumnName == 'TEAM_ID'         
		   || sColumnName == 'TEAM_NAME'       
		   || sColumnName == 'TEAM_SET_ID'     
		   || sColumnName == 'TEAM_SET_NAME'   
		   || sColumnName == 'TEAM_SET_LIST'   
		   || sColumnName == 'ASSIGNED_SET_ID'  
		   || sColumnName == 'ASSIGNED_SET_NAME'
		   || sColumnName == 'ASSIGNED_SET_LIST'
		   || sColumnName == 'ID_C'            
		   || sColumnName == 'AUDIT_ID'        
		   || sColumnName == 'AUDIT_ACTION'    
		   || sColumnName == 'AUDIT_DATE'      
		   || sColumnName == 'AUDIT_COLUMNS'   
		   || sColumnName == 'AUDIT_TABLE'     
		   || sColumnName == 'AUDIT_TOKEN'     
		   || sColumnName == 'LAST_ACTIVITY_DATE'
		   || sColumnName == 'TAG_SET_NAME'    
		   || sColumnName == 'PENDING_PROCESS_ID'
		   || sColumnName == 'ARCHIVE_BY'      
		   || sColumnName == 'ARCHIVE_BY_NAME' 
		   || sColumnName == 'ARCHIVE_DATE_UTC'
		   || sColumnName == 'ARCHIVE_USER_ID' 
		   || sColumnName == 'ARCHIVE_VIEW'    
		   || sColumnName == 'EXCHANGE_FOLDER'
			)
		{
			sTERM_NAME = '.LBL_' + sColumnName;
		}
		else
		{
			sTERM_NAME = sModule + '.LBL_' + sColumnName;
		}
		return sTERM_NAME;
	}

	static TableColumnName(sModule: string, sColumnName: string)
	{
		try
		{
			let sTERM_NAME = L10n.BuildTermName(sModule, sColumnName);
			if ( SplendidCache.Terminology(sTERM_NAME) != null )
				sColumnName = SplendidCache.Terminology(sTERM_NAME);
			return sColumnName;
		}
		catch(error)
		{
			console.error((new Date()).toISOString() + ' ' + this.constructor.name + '.TableColumnName ' + sModule + ' ' + sColumnName, error);
		}
		return sColumnName;
	}
}

