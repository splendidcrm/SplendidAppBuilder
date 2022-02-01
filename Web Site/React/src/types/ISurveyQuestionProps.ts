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
import SURVEY_PAGE_QUESTION  from '../types/SURVEY_PAGE_QUESTION';

export default interface ISurveyQuestionProps
{
	row                 : SURVEY_PAGE_QUESTION;
	displayMode         : string;
	rowQUESTION_RESULTS?: any;
	onChanged?          : (DATA_FIELD: string, DATA_VALUE: any, DISPLAY_FIELD?: string, DISPLAY_VALUE?: any) => void;
	onSubmit?           : () => void;
	onUpdate?           : (PARENT_FIELD: string, DATA_VALUE: any, item?: any) => void;
	createDependency?   : (DATA_FIELD: string, PARENT_FIELD: string, PROPERTY_NAME?: string) => void;
	onFocusNextQuestion?: (ID: string) => void;
	isPageFocused?      : () => boolean;
}


