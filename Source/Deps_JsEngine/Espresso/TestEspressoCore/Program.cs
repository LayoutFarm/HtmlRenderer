using System;
using System.Collections.Generic;
using System.Reflection;
namespace TestEspressoCore
{
    public class Program
    {
        public static void Main(string[] args)
        {


            Espresso.JsBridge.V8Init(); 
            //----------------------------
            //prepare test cases 
            var testList = GetTestInfoList();
            //----------------------------
            testDictionary = new Dictionary<string, TestInfo>();
            foreach (TestInfo t in testList)
            {
                testDictionary.Add(t.Choice, t);
            }

            AGAIN:
            //----------------------------------------------
            Menu(testList);
            Console.WriteLine("");
            Console.WriteLine("type 0 to exit");
            //----------------------------------------------
            string num = Console.ReadLine();
            Console.WriteLine("running ...");
            Console.WriteLine("");
            if (num == "0")
            {
                return;
            }
            TestInfo selectedTest;
            if (testDictionary.TryGetValue(num, out selectedTest))
            {
                //found test
                selectedTest.TestMethod.Invoke(num, new object[0]);
            }
            else
            {
                Console.WriteLine("---[not found, Please press choice (1,2 etc)]---");
            }
            Console.WriteLine("------");
            Console.WriteLine("");
            goto AGAIN;
        }


        static List<TestInfo> GetTestInfoList()
        {
            List<TestInfo> testList = new List<TestInfo>();
            Type t = typeof(Test1);
            var mets = t.GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            foreach (var met in mets)
            {
                var found = met.GetCustomAttributes(typeof(TestAttribute), false) as TestAttribute[];
                if (found != null && found.Length > 0)
                {
                    TestInfo testInfo = new TestInfo()
                    {
                        Choice = found[0].Choice,
                        Name = found[0].Name,
                        TestMethod = met
                    };
                    testList.Add(testInfo);
                }
            }
            return testList;
        }

        static Dictionary<string, TestInfo> testDictionary;
        static void Menu(List<TestInfo> testList)
        {
            Console.WriteLine("Espresso");
            Console.WriteLine("Select test case, and press Enter");
            Console.WriteLine("[0] Exit");
            foreach (TestInfo t in testList)
            {
                Console.WriteLine("[" + t.Choice + "] " + t.Name);
            }
        }
    }
}
