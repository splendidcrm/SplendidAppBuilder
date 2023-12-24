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
	public class RuleAction
	{
		public string code { get; set; }

		public RuleAction(string code)
		{
			this.code = code;
		}

		public bool Validate(RuleValidation validator)
		{
			// 08/12/2023 Paul.  Rosyln expects a semi-colon terminator. 
			if ( !code.Trim().EndsWith(";") )
				code += ";";
			if ( !String.IsNullOrEmpty(code) )
			{
				SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
				IEnumerable<Diagnostic> diags = tree.GetDiagnostics();
				foreach (Diagnostic diag in diags)
				{
					if ( diag.Severity == DiagnosticSeverity.Error )
					{
						ValidationError error = new ValidationError(diag.GetMessage());
						validator.Errors.Add(error);
					}
				}
			}
			return validator.Errors.Count == 0;
		}

		public void Execute(RuleExecution exec)
		{
			if ( !Sql.IsEmptyString(code) )
			{
				string sActionCode = code.Replace("this.", "THIS.").Replace("this[", "THIS[");
				ScriptState<object> scriptState = CSharpScript.RunAsync(sActionCode, exec.ScriptOptions, exec.Globals).Result;
				//scriptState.ContinueWithAsync(code).Result;
			}
		}
	}
}
