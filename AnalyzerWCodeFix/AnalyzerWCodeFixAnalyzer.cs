using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace AnalyzerWCodeFix
{
    

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AnalyzerWCodeFixAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "PPI001";
        static LocalizableString Title = "Type name contains lowercase letters";
        static LocalizableString MessageFormat = "{0}";
        static LocalizableString Description = "Type names should be all uppercase.";
        const string Category = "Analyzers";
        static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, true, Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            //Debugger.Launch();

            context.RegisterOperationAction(AnalyzeSymbol, OperationKind.ConditionalAccess);
        }

        private static void AnalyzeSymbol(OperationAnalysisContext context)
        {
            var parent = (IConditionalAccessOperation)context.Operation;

            if (!(parent.WhenNotNull is IInvocationOperation expectedOperation))
            {
                return;
            }

            // check to see if the method is from System.Linq.Enumerable
            var expectedInvocationClass = expectedOperation.TargetMethod.ContainingType.Name;
            var expectedInvocationTargetNamespace = expectedOperation.TargetMethod.ContainingNamespace.ToString();
            var invocationName = expectedOperation.TargetMethod.Name;

            // Check if the method's return type isn't nullable
            if (expectedOperation.TargetMethod.ReturnType.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
            {
                return;
            }

            // Check if the operation is in a nullable context
            if (!context.Compilation.Options.NullableContextOptions.HasFlag(NullableContextOptions.Enable))
            {
                return;
            }

            if (expectedInvocationTargetNamespace == "System.Linq" && expectedInvocationClass == "Enumerable")
            {
                var diagnostic = Diagnostic.Create(Rule, parent.Syntax.GetLocation(), $"'System.Linq.Enumerable.{invocationName}' does not return null. Avoid using '?.' with non nullable types");
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
