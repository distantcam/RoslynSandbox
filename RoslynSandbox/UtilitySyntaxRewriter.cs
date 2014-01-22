using System;
using System.Linq;
using Roslyn.Compilers.CSharp;

namespace RoslynSandbox
{
    public class UtilitySyntaxRewriter : SyntaxRewriter
    {
        protected SyntaxTrivia Space { get { return Syntax.Whitespace(" "); } }
        protected SyntaxTrivia NewLine { get { return Syntax.Whitespace(Environment.NewLine); } }
    }
}