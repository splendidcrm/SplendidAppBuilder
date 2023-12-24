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
using System.Collections.Generic;

namespace SplendidCRM
{
	public class Rule
	{
		public string                   Name                 { get; set; }
		public string                   Description          { get; set; }
		public int                      Priority             { get; set; }
		public RuleReevaluationBehavior ReevaluationBehavior { get; set; }
		public bool                     Active               { get; set; }
		public RuleExpressionCondition  Condition            { get; set; }
		public IList<RuleAction>        ThenActions          { get; set; }
		public IList<RuleAction>        ElseActions          { get; set; }

		public Rule(string name, RuleExpressionCondition condition, List<RuleAction> thenActions, List<RuleAction> elseActions)
		{
			this.Name                 = name          ;
			this.Description          = string.Empty  ;
			this.Priority             = 0             ;
			this.ReevaluationBehavior = RuleReevaluationBehavior.Always;
			this.Active               = true          ;
			this.Condition            = condition     ;
			this.ThenActions          = thenActions   ;
			this.ElseActions          = elseActions   ;
		}

		public void Validate(RuleValidation validation)
		{
			// check the condition
			if ( this.Condition == null )
				validation.Errors.Add(new ValidationError("Messages.MissingRuleCondition"));
			else
				this.Condition.Validate(validation);
 
			// check the optional then actions
			if ( this.ThenActions != null)
				ValidateRuleActions(this.ThenActions, validation);
 
			// check the optional else actions
			if (this.ElseActions != null)
				ValidateRuleActions(this.ElseActions, validation);
		}
 
		private static void ValidateRuleActions(ICollection<RuleAction> ruleActions, RuleValidation validator)
		{
			foreach (RuleAction action in ruleActions)
			{
				action.Validate(validator);
			}
		}
 	}
}
