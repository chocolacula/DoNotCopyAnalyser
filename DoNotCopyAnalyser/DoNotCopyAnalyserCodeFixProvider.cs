using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Editing;
using System;

namespace DoNotCopyAnalyser
{
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DoNotCopyAnalyserCodeFixProvider)), Shared]
	public class DoNotCopyAnalyserCodeFixProvider : CodeFixProvider
	{
		private const string TITLE = "Delete assignment";

		public sealed override ImmutableArray<string> FixableDiagnosticIds
		{
			get { return ImmutableArray.Create(DoNotCopyAnalyser.DIAGNOSTIC_ID); }
		}

		public sealed override FixAllProvider GetFixAllProvider()
		{	
			return WellKnownFixAllProviders.BatchFixer;
		}

		public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

			var diagnostic = context.Diagnostics.First();
			var diagnosticSpan = diagnostic.Location.SourceSpan;

			var assignment = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<AssignmentExpressionSyntax>().First();

			context.RegisterCodeFix(
				CodeAction.Create(
					title: TITLE,
					createChangedDocument: c => DeleteAsync(context.Document, root, assignment, c),
					equivalenceKey: TITLE),
				diagnostic);
		}

		private Task<Document> DeleteAsync(Document document, SyntaxNode root, AssignmentExpressionSyntax assignment, CancellationToken cancellationToken)
		{
			var task = Task.Run(() =>
			{
				var newRoot = root.RemoveNode(assignment.Parent, SyntaxRemoveOptions.KeepNoTrivia);

				return document.WithSyntaxRoot(newRoot);
			});

			return task;
		}
	}
}
