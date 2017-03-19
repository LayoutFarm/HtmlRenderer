using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Test3
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //load and init v8 engine***
            string libespr = @"D:\projects\CompilerKit\Espresso\build\Release\libespr.dll";             
          
            Espresso.JsBridge.LoadV8(libespr);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            //------------------
            Espresso.JsBridge.UnloadV8();

        }
    }


    class TestCaseInstance
    {
        Type testCaseType;
        System.Reflection.MethodInfo setupMethod;
        System.Reflection.MethodInfo teardownMethod;
        internal List<TestCaseMethod> testMethods = new List<TestCaseMethod>();
        public TestCaseInstance(Type testCaseType)
        {
            this.testCaseType = testCaseType;

        }
        public void SetSetupMethod(System.Reflection.MethodInfo met)
        {
            this.setupMethod = met;
        }
        public void SetTeardownMethod(System.Reflection.MethodInfo met)
        {
            this.teardownMethod = met;
        }
        public TestCaseMethod AddTestMethod(System.Reflection.MethodInfo met)
        {
            TestCaseMethod testcaseMethod = new TestCaseMethod(this, met);
            testMethods.Add(testcaseMethod);
            return testcaseMethod;
        }
        public override string ToString()
        {
            return this.testCaseType.Name;
        }

        internal void RunTestCase(System.Reflection.MethodInfo met)
        {
            var testInstance = Activator.CreateInstance(this.testCaseType);
            if (this.setupMethod != null)
            {
                this.setupMethod.Invoke(testInstance, null);
            }
            met.Invoke(testInstance, null);

        }
    }

    class TestCaseMethod
    {
        TestCaseInstance owner;
        System.Reflection.MethodInfo met;
        public TestCaseMethod(TestCaseInstance owner, System.Reflection.MethodInfo met)
        {
            this.owner = owner;
            this.met = met;
        }
        public override string ToString()
        {
            return this.owner.ToString() + "::" + this.met.Name;
        }
        public void RunTest()
        {
            this.owner.RunTestCase(this.met);

        }
    }
}
