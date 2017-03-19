using System;
using System.IO;
using Espresso;

namespace TestEspressoCore
{
    class TestInfo
    {

        public TestInfo()
        {

        }
        public string Name { get; set; }
        public string Choice { get; set; }
        public System.Reflection.MethodInfo TestMethod { get; set; }
    }
}