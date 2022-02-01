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
import SURVEY_PAGE_QUESTION from './SURVEY_PAGE_QUESTION';

export default interface SURVEY_PAGE
{ ID                            : string  // uniqueidentifier
, NAME                          : string  // nvarchar
, PAGE_NUMBER                   : number  // int
, QUESTION_RANDOMIZATION        : string  // nvarchar
, DESCRIPTION                   : string  // nvarchar
, SURVEY_ID                     : string  // uniqueidentifier
, RANDOMIZE_COUNT               : number  // int
, RANDOMIZE_APPLIED             : boolean  // 07/16/2018 Paul.  computed
, RENUMBER_QUESTIONS            : boolean  // 07/16/2018 Paul.  computed
, QUESTION_OFFSET               : number  // 07/16/2018 Paul.  computed
, MOBILE_ID                     : string  // 07/16/2018 Paul.  computed
, SURVEY_QUESTIONS              : SURVEY_PAGE_QUESTION[]
}


