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

namespace SplendidCRM
{
	public class RuleValidation
	{
		public Type                                           ThisType            { get; set; }
		public ITypeProvider                                  TypeProvider        { get; set; }
		public List<ValidationError>                          Errors              { get; set; }
		public Dictionary<string, Type>                       TypesUsed           { get; set; }
		public Dictionary<string, Type>                       TypesUsedAuthorized { get; set; }
		//public Stack<CodeExpression>                          ActiveParentNodes   { get; set; }
		//public Dictionary<CodeExpression, RuleExpressionInfo> ExpressionInfoMap   { get; set; }
		//public Dictionary<CodeTypeReference, Type>            TypeRefMap          { get; set; }
		public bool                                           CheckStaticType     { get; set; }
		public IList<AuthorizedType>                          AuthorizedTypes     { get; set; }
 
		public RuleValidation(Type thisType, SplendidRulesTypeProvider typeProvider)
		{
			this.Errors = new List<ValidationError>();
		}

		public bool ValidateConditionExpression(string expression)
		{
			if ( String.IsNullOrEmpty(expression) )
				throw(new ArgumentNullException("expression"));

			// 08/12/2023 Paul.  Rosyln expects a condition. 
			string code = "if (" + expression + ") {}";
			SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
			IEnumerable<Diagnostic> diags = tree.GetDiagnostics();
			foreach (Diagnostic diag in diags)
			{
				if ( diag.Severity == DiagnosticSeverity.Error )
				{
					ValidationError error = new ValidationError(diag.GetMessage());
					this.Errors.Add(error);
				}
			}
			return Errors.Count == 0;
		}
	}
}
