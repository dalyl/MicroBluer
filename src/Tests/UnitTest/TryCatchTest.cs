using System;
using MicroBluer.AndroidMobile;
using MicroBluer.AndroidUtils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
    public class TryCatchTest
    {
        [TestMethod]
        public void TestOrder()
        {
            Action<string> show = msg => Console.WriteLine($"show:{msg}\n");
            Action begin = () => Console.WriteLine("begin\n");
            Action end = () => Console.WriteLine("end\n");
            var test = new TryCatch(show);
            test.Invoke(() => Console.WriteLine("test\n"));
            Console.WriteLine(test.Invoke("default",()=> "test"));
            Console.WriteLine(test.Invoke("default", () => { Console.WriteLine("1 \n"); return "test"; }));
            Console.WriteLine(test.Invoke("default", WriteTest));
        }

        string WriteTest()
        {
            Console.WriteLine("1 \n");
            return "test";
        }
    }
}
