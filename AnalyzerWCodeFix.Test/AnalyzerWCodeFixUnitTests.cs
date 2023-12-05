using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using VerifyCS = AnalyzerWCodeFix.Test.CSharpCodeFixVerifier<
    AnalyzerWCodeFix.AnalyzerWCodeFixAnalyzer,
    AnalyzerWCodeFix.AnalyzerWCodeFixCodeFixProvider>;

namespace AnalyzerWCodeFix.Test
{
    [TestClass]
    public class AnalyzerWCodeFixUnitTest
    {
        //No diagnostics expected to show up
        [TestMethod]
        public async Task TestMethod1()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        public async Task TestMethod2()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class {|#0:TypeName|}
        {   
                        public void Test()
            {
                var list = new List<int>();
                var any = list.Where(x => x > 0)?.Any();
            }
        }
    }";

            var fixtest = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TYPENAME
        {   
            public void Test()
            {
                var list = new List<int>();
                var any = list.Where(x => x > 0)?.Any();
            }
        }
    }";

            var expected = VerifyCS.Diagnostic("AnalyzerWCodeFix").WithLocation(0).WithArguments("TypeName");
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }
    }
}
