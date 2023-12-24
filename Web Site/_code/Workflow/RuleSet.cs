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
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace SplendidCRM
{
	public class RuleSet
	{
		public string               Name        { get; set; }
		public string               Description { get; set; }
		public RuleChainingBehavior Behavior    { get; set; }
		public List<Rule>           Rules       { get; set; }

		public RuleSet(string name)
		{
			this.Behavior = RuleChainingBehavior.Full;
			this.Rules = new List<Rule>();
		}

		public bool Validate(RuleValidation validation)
		{
			if ( validation == null )
				throw new ArgumentNullException("validation");
 
			foreach ( Rule r in this.Rules )
			{
				r.Validate(validation);
			}
 
			if ( validation.Errors == null || validation.Errors.Count == 0 )
				return true;
 
			return false;
		}

		public void Execute(RuleExecution exec)
		{
			try
			{
				foreach ( Rule r in this.Rules )
				{
					bool bCondition = r.Condition.Evaluate(exec);
					if ( bCondition )
					{
						foreach ( RuleAction action in r.ThenActions )
						{
							action.Execute(exec);
						}
					}
					else
					{
						foreach ( RuleAction action in r.ElseActions )
						{
							action.Execute(exec);
						}
					}
				}
			}
			catch(Exception ex)
			{
				if ( exec.ThisObject is SqlObj )
				{
					(exec.ThisObject as SqlObj).ErrorMessage = ex.Message;
				}
				ValidationError error = new ValidationError(ex.Message);
				exec.Validation.Errors.Add(error);
			}
		}
	}
}
