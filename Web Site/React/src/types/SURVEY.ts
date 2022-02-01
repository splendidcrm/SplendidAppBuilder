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
import SURVEY_PAGE from './SURVEY_PAGE';

export default interface SURVEY
{ ID                            : string  // uniqueidentifier
, DATE_MODIFIED_UTC             : Date
, NAME                          : string  // nvarchar
, STATUS                        : string  // nvarchar
// 10/01/2018 Paul.  Include SURVEY_TARGET_MODULE. 
, SURVEY_TARGET_MODULE          : string  // nvarchar
, SURVEY_STYLE                  : string  // nvarchar
, PAGE_RANDOMIZATION            : string  // nvarchar
, DESCRIPTION                   : string  // nvarchar
, SURVEY_THEME_ID               : string  // uniqueidentifier
, RANDOMIZE_COUNT               : number  // int
, RANDOMIZE_APPLIED             : boolean // 07/16/2018 Paul.  computed
, RENUMBER_PAGES                : boolean // 07/16/2018 Paul.  computed
, SURVEY_PAGES                  : SURVEY_PAGE[]
, LOOP_SURVEY                   : boolean // bit
, TIMEOUT                       : number  // seconds
, RESULTS_COUNT                 : number  // return with cached list. 
, SURVEY_THEME                  : any     // the entire theme is included with the survey. 
}


