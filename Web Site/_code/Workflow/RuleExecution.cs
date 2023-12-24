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
using Microsoft.CodeAnalysis.Scripting;

namespace SplendidCRM
{
	public class RuleExecution
	{
		public class SplendidControlThisGlobals
		{
			public SplendidControlThis THIS;
		}

		public class SplendidWizardThisGlobals
		{
			public SplendidWizardThis THIS;
		}

		public class SplendidImportThisGlobals
		{
			public SplendidImportThis THIS;
		}

		public class SplendidReportThisGlobals
		{
			public SplendidReportThis THIS;
		}

		public bool                     Halted                   { get; set; }
		public object                   ThisObject               { get; set; }
		public RuleValidation           Validation               { get; set; }
		public ActivityExecutionContext ActivityExecutionContext { get; set; }
		public RuleLiteralResult        ThisLiteralResult        { get; set; }
		public ScriptOptions            ScriptOptions            { get; set; }
		public object                   Globals                  { get; set; }

		public RuleExecution(RuleValidation validation, object swThis)
		{
			this.Validation = validation;
			this.ThisObject = swThis    ;
			this.ScriptOptions = ScriptOptions.Default.AddReferences("SplendidCRM");
			this.ScriptOptions.AddImports("System");

			if      ( this.ThisObject is SplendidControlThis ) this.Globals = new SplendidControlThisGlobals { THIS = (SplendidControlThis) this.ThisObject };
			else if ( this.ThisObject is SplendidWizardThis  ) this.Globals = new SplendidWizardThisGlobals  { THIS = (SplendidWizardThis ) this.ThisObject };
			else if ( this.ThisObject is SplendidImportThis  ) this.Globals = new SplendidImportThisGlobals  { THIS = (SplendidImportThis ) this.ThisObject };
			else if ( this.ThisObject is SplendidReportThis  ) this.Globals = new SplendidReportThisGlobals  { THIS = (SplendidReportThis ) this.ThisObject };
		}
	}
}
