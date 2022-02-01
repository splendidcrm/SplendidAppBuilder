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

export default interface SURVEY_PAGE_QUESTION
{ SURVEY_PAGE_ID                : string  // uniqueidentifier
, SURVEY_ID                     : string  // uniqueidentifier
, QUESTION_NUMBER               : number  // int
, ID                            : string  // uniqueidentifier
, NAME                          : string  // nvarchar
, DESCRIPTION                   : string  // nvarchar
, QUESTION_TYPE                 : string  // nvarchar
, DISPLAY_FORMAT                : string  // nvarchar
, ANSWER_CHOICES                : string  // nvarchar
, COLUMN_CHOICES                : string  // nvarchar
, FORCED_RANKING                : boolean // bit
, INVALID_DATE_MESSAGE          : string  // nvarchar
, INVALID_NUMBER_MESSAGE        : string  // nvarchar
, NA_ENABLED                    : boolean // bit
, NA_LABEL                      : string  // nvarchar
, OTHER_ENABLED                 : boolean // bit
, OTHER_LABEL                   : string  // nvarchar
, OTHER_HEIGHT                  : number  // int
, OTHER_WIDTH                   : number  // int
, OTHER_AS_CHOICE               : boolean // bit
, OTHER_ONE_PER_ROW             : boolean // bit
, OTHER_REQUIRED_MESSAGE        : string  // nvarchar
, OTHER_VALIDATION_TYPE         : string  // nvarchar
, OTHER_VALIDATION_MIN          : string  // nvarchar
, OTHER_VALIDATION_MAX          : string  // nvarchar
, OTHER_VALIDATION_MESSAGE      : string  // nvarchar
, REQUIRED                      : boolean // bit
, REQUIRED_TYPE                 : string  // nvarchar
, REQUIRED_RESPONSES_MIN        : number  // int
, REQUIRED_RESPONSES_MAX        : number  // int
, REQUIRED_MESSAGE              : string  // nvarchar
, VALIDATION_TYPE               : string  // nvarchar
, VALIDATION_MIN                : string  // nvarchar
, VALIDATION_MAX                : string  // nvarchar
, VALIDATION_MESSAGE            : string  // nvarchar
, VALIDATION_SUM_ENABLED        : boolean // bit
, VALIDATION_NUMERIC_SUM        : number  // int
, VALIDATION_SUM_MESSAGE        : string  // nvarchar
, RANDOMIZE_TYPE                : string  // nvarchar
, RANDOMIZE_NOT_LAST            : boolean // bit
, SIZE_WIDTH                    : string  // nvarchar
, SIZE_HEIGHT                   : string  // nvarchar
, BOX_WIDTH                     : string  // nvarchar
, BOX_HEIGHT                    : string  // nvarchar
, COLUMN_WIDTH                  : string  // nvarchar
, PLACEMENT                     : string  // nvarchar
, SPACING_LEFT                  : number  // int
, SPACING_TOP                   : number  // int
, SPACING_RIGHT                 : number  // int
, SPACING_BOTTOM                : number  // int
, IMAGE_URL                     : string  // nvarchar
, IMAGE_MIME_TYPE               : string  // nvarchar
, IMAGE_CONTENT                 : string  // nvarchar
, VERTICAL_FILL                 : boolean // bit
}


