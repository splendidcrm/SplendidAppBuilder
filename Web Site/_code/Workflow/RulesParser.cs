/**********************************************************************************************************************
 * Copyright (C) 2005-2022 SplendidCRM Software, Inc. 
 * MIT License
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
 * files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
 * modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
 * is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
 * LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR 
 * IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 *********************************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace SplendidCRM
{
	public class RulesParser
	{
		private RuleValidation Validation;

		public RulesParser(RuleValidation validation)
		{
			this.Validation = validation;
		}

		public RuleExpressionCondition ParseCondition(string code)
		{
			RuleExpressionCondition condition = new RuleExpressionCondition(code);
			// 08/12/2023 Paul.  Actual validation will occur in Rule class. 
			/*
			try
			{
				condition.Validate(this.Validation);
			}
			catch(Exception ex)
			{
				ValidationError error = new ValidationError(ex.Message);
				Validation.Errors.Add(error);
			}
			*/
			return condition;
		}

		public List<RuleAction> ParseStatementList(string code)
		{
			List<RuleAction> rules = new List<RuleAction>();
			RuleAction action = new RuleAction(code);
			rules.Add(action);
			// 08/12/2023 Paul.  Actual validation will occur in Rule class. 
			/*
			try
			{
				action.Validate(this.Validation);
			}
			catch(Exception ex)
			{
				ValidationError error = new ValidationError(ex.Message);
				Validation.Errors.Add(error);
			}
			*/
			return rules;
		}
	}
}
