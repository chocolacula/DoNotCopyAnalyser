using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DoNotCopyAnalyser
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class DoNotCopyAnalyser : DiagnosticAnalyzer
	{
		public const string DIAGNOSTIC_ID = "DNC0001";

		private const string CATEGORY = "Assignment";

		private static readonly LocalizableString _title = "Illegal copying";
		private static readonly LocalizableString _messageFormat = "Copying of objects with [DoNOtCopy] Attribute is forbidden";
		private static readonly LocalizableString _description = "Copying is forbidden";

		private static readonly DiagnosticDescriptor _rule = new DiagnosticDescriptor(
			DIAGNOSTIC_ID,
			_title,
			_messageFormat,
			CATEGORY,
			DiagnosticSeverity.Error,
			isEnabledByDefault: true,
			description: _description
		);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics 
		{
			get { return ImmutableArray.Create(_rule); }
		}

		public override void Initialize(AnalysisContext context)
		{
			context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.SimpleAssignmentExpression);
		}

		private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
		{
			var nodes = context.Node.ChildNodes();

			MemberAccessExpressionSyntax access = null;

			foreach (var node in nodes)
			{
				if (node is MemberAccessExpressionSyntax)
				{
					access = node as MemberAccessExpressionSyntax;
					break;
				}
			}

			if (access == null)
				return;

			nodes = access.ChildNodes();

			IdentifierNameSyntax identifier = null;

			foreach (var node in nodes)
			{
				if (node is IdentifierNameSyntax)
				{
					// the last one
					identifier = node as IdentifierNameSyntax;
				}
			}

			var symbol = context.SemanticModel.GetSymbolInfo(identifier);

			var fieldSymbol = (IFieldSymbol)symbol.Symbol;
			var attributes = fieldSymbol.GetAttributes();

			foreach (var attribute in attributes)
			{
				if (attribute.AttributeClass.Name == "DoNotCopyAttribute")
				{
					var diagnostic = Diagnostic.Create(_rule, context.Node.GetLocation());
					context.ReportDiagnostic(diagnostic);
				}
			}
		}
	}
}
