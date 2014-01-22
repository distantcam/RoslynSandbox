using System;
using System.Collections.Generic;
using System.Linq;
using Roslyn.Compilers.CSharp;

namespace RoslynSandbox
{
    public class NotifyPropertyChangedRewriter : UtilitySyntaxRewriter
    {
        private readonly List<FieldDeclarationSyntax> _fields = new List<FieldDeclarationSyntax>();

        public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            var propName = node.Identifier.ValueText;
            var fieldName = "k_" + propName.ToCamelCase();
            var indent = Syntax.Whitespace(node.GetLeadingTrivia().ToFullString().Replace(Environment.NewLine, ""));

            // Add the field
            var fieldString = String.Format("private {0} {1};", node.Type, fieldName);
            var parse = Syntax.ParseCompilationUnit(fieldString);

            var newField = ((FieldDeclarationSyntax)parse.Members[0])
                .WithLeadingTrivia(indent)
                .WithTrailingTrivia(NewLine);

            _fields.Add(newField);

            // Add the property
            var property = String.Format(@"public {0} {1}
{{
    get {{ return {2}; }}
    set
    {{
        if ({3})
            return;
        {2} = value;
        OnPropertyChanged(""{1}"");
    }}
}}", node.Type, propName, fieldName, GetEqualsCodeForType(node.Type, fieldName))
                .Replace(Environment.NewLine, Environment.NewLine + indent);
            parse = Syntax.ParseCompilationUnit(property);

            var newProperty = ((PropertyDeclarationSyntax)parse.Members[0])
                .WithLeadingTrivia(indent)
                .WithTrailingTrivia(NewLine);

            return newProperty;
        }

        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            var newTypeDeclaration = (TypeDeclarationSyntax)base.VisitClassDeclaration(node);

            if (_fields.Any())
            {
                var members = new List<MemberDeclarationSyntax>(newTypeDeclaration.Members);
                members.InsertRange(0, _fields);

                return ((ClassDeclarationSyntax)newTypeDeclaration).Update(
                    newTypeDeclaration.AttributeLists,
                    newTypeDeclaration.Modifiers,
                    newTypeDeclaration.Keyword,
                    newTypeDeclaration.Identifier,
                    newTypeDeclaration.TypeParameterList,
                    newTypeDeclaration.BaseList,
                    newTypeDeclaration.ConstraintClauses,
                    newTypeDeclaration.OpenBraceToken,
                    Syntax.List(members.AsEnumerable()),
                    newTypeDeclaration.CloseBraceToken,
                    newTypeDeclaration.SemicolonToken);
            }

            return newTypeDeclaration;
        }

        private string GetEqualsCodeForType(TypeSyntax type, string fieldName)
        {
            if (String.Equals(type.ToString(), "String", StringComparison.CurrentCultureIgnoreCase))
                return String.Format("String.Equals({0}, value, StringComparison.CurrentCultureIgnoreCase)", fieldName);

            if (String.Equals(type.ToString(), "int") || String.Equals(type.ToString(), "Int32"))
                return String.Format("{0} == value", fieldName);

            return "false";
        }
    }
}