using System;
using System.IO;
using System.Linq;
using Roslyn.Compilers.CSharp;

namespace RoslynSandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            NotifyPropertyChangedExample();
        }

        private static void NotifyPropertyChangedExample()
        {
            var tree = SyntaxTree.ParseFile(@"..\..\..\ExampleCode\NotifyPropertyChanged.cs");

            NotifyPropertyChangedRewriter rewriter = new NotifyPropertyChangedRewriter();

            File.WriteAllText(@"..\..\..\ExampleCode\NotifyPropertyChanged.out.cs", rewriter.Visit(tree.GetRoot()).GetText().ToString());
        }
    }
}