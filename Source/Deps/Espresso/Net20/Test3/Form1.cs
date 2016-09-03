using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Drawing;

using System.Text;
using System.Windows.Forms;
using Espresso;
using NUnit.Framework;

namespace Test3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            ////-----------------------------------------------------------------------            
            //var asm = typeof(VroomJs.Tests.TestClass).Assembly;
            //var testFixtureAttr = typeof(NUnit.Framework.TestFixtureAttribute);
            //var testAttr = typeof(NUnit.Framework.TestAttribute);
            //var setupAttr = typeof(NUnit.Framework.SetUpAttribute);
            //var tearDownAttr = typeof(NUnit.Framework.TearDownAttribute);
            //var testCaseAttr = typeof(NUnit.Framework.TestCaseAttribute);

            ////-----------------------------------------------------------------------            
            //List<TestCaseInstance> testCaseList = new List<TestCaseInstance>();
            //foreach (var t in asm.GetTypes())
            //{
            //    var founds = t.GetCustomAttributes(testFixtureAttr, false);
            //    if (founds != null && founds.Length > 0)
            //    {
            //        //test 
            //        var testCaseInstance = new TestCaseInstance(t);
            //        //find setup/teardown method
            //        foreach (var met in t.GetMethods())
            //        {
            //            var someSetUpAttrs = met.GetCustomAttributes(setupAttr, false);
            //            if (someSetUpAttrs != null && someSetUpAttrs.Length > 0)
            //            {
            //                testCaseInstance.SetSetupMethod(met);
            //                continue;
            //            }
            //            var someTeardownAttrs = met.GetCustomAttributes(tearDownAttr, false);
            //            if (someTeardownAttrs != null && someTeardownAttrs.Length > 0)
            //            {
            //                testCaseInstance.SetTeardownMethod(met);
            //                continue;
            //            }
            //            var someTestAttrs = met.GetCustomAttributes(testAttr, false);
            //            if (someTestAttrs != null && someTestAttrs.Length > 0)
            //            {

            //                var testMethod = testCaseInstance.AddTestMethod(met);
            //                this.listBox1.Items.Add(testMethod);
            //                continue;
            //            }
            //            someTestAttrs = met.GetCustomAttributes(testCaseAttr, false);
            //            if (someTestAttrs != null && someTestAttrs.Length > 0)
            //            {
            //                var testMethod = testCaseInstance.AddTestMethod(met);
            //                this.listBox1.Items.Add(testMethod);
            //            }
            //        }

            //        testCaseList.Add(testCaseInstance);
            //    }
            //}
            ////---------------------------------------------------------------------
            //this.listBox1.DoubleClick += new EventHandler(listBox1_DoubleClick);
        }

        void listBox1_DoubleClick(object sender, EventArgs e)
        {
            var testCaseMethod = listBox1.SelectedItem as TestCaseMethod;
            if (testCaseMethod != null)
            {
                //run test
                testCaseMethod.RunTest();
            }
        }

        class TestMe1
        {
            public int B()
            {
                return 100;
            }
            public bool C()
            {
                return true;
            }
        }

        delegate object AboutMeEventHandler(object[] args);

        [JsType]
        class AboutMe
        {
            AboutMeEventHandler mousedownEventHandler;

            [JsMethod]
            public int Test1()
            {
                return 123;
            }

            [JsMethod]
            public string Test2(string text)
            {
                return "hello " + text;
            }
            [JsProperty]
            public bool IsOK
            {
                get
                {
                    return true;
                }
            }
            [JsMethod]
            public AboutMe NewAboutMe()
            {
                return new AboutMe();
            }
            [JsMethod]
            public void AttachEvent(string eventName, AboutMeEventHandler evHandler)
            {
                this.mousedownEventHandler = evHandler;
            }
            [JsMethod]
            public void FireEventMouseDown(object eventArg)
            {
                if (mousedownEventHandler != null)
                {
                    //JsFunction func = mousedownEventHandler.Target as JsFunction;
                    //func.Invoke(eventArg);
                    //SimpleDelegate simpleDel = mousedownEventHandler as SimpleDelegate;
                    //simpleDel(new object[] { eventArg }); 
                    //mousedownEventHandler(new object[] { eventArg });
                    mousedownEventHandler(new object[] { null, eventArg });
                    //mousedownEventHandler(this, eventArg);
                }
            }
            [JsMethod]
            public void SetResult(object result1)
            {
                Console.WriteLine(result1.ToString());
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
#if DEBUG
            JsBridge.dbugTestCallbacks();
#endif

            JsTypeDefinition jstypedef = new JsTypeDefinition("AA");
            jstypedef.AddMember(new JsMethodDefinition("B", args =>
            {
                args.SetResult(100);
            }));
            jstypedef.AddMember(new JsMethodDefinition("C", args =>
            {
                args.SetResult(true);
            }));
            //===============================================================
            //create js engine and context
            using (JsEngine engine = new JsEngine())
            using (JsContext ctx = engine.CreateContext())
            {

                if (!jstypedef.IsRegisterd)
                {
                    ctx.RegisterTypeDefinition(jstypedef);
                }
                GC.Collect();
                System.Diagnostics.Stopwatch stwatch = new System.Diagnostics.Stopwatch();
                stwatch.Start();

                TestMe1 t1 = new TestMe1();
                var proxy = ctx.CreateWrapper(t1, jstypedef);

                for (int i = 2000; i >= 0; --i)
                {
                    ctx.SetVariableFromAny("x", proxy);
                    object result = ctx.Execute("(function(){if(x.C()){return x.B();}else{return 0;}})()");
                }
                //for (int i = 1; i >= 0; --i)
                //{
                //    ctx.SetVariableFromAny("x", proxy);
                //    object result = ctx.Execute(@"
                //    var http = require('http'); 
                //    var server = http.createServer(function(req, res) {
                //    res.writeHead(200);
                //    res.end('Hello Http');
                //    });
                //    server.listen(8080);
                //    ");
                //}
                stwatch.Stop();

                Console.WriteLine("met1 template:" + stwatch.ElapsedMilliseconds.ToString());
                //Assert.That(result, Is.EqualTo(100));
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
#if DEBUG
            JsBridge.dbugTestCallbacks();
#endif
            //create js engine and context

            using (JsEngine engine = new JsEngine())
            using (JsContext ctx = engine.CreateContext())
            {
                GC.Collect();
                System.Diagnostics.Stopwatch stwatch = new System.Diagnostics.Stopwatch();
                stwatch.Start();

                TestMe1 t1 = new TestMe1();

                for (int i = 2000; i >= 0; --i)
                {
                    ctx.SetVariableFromAny("x", t1);
                    object result = ctx.Execute("(function(){if(x.C()){return  x.B();}else{return 0;}})()");
                }
                stwatch.Stop();
                Console.WriteLine("met2 managed reflection:" + stwatch.ElapsedMilliseconds.ToString());
                //Assert.That(result, Is.EqualTo(100)); 
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

#if DEBUG
            JsBridge.dbugTestCallbacks();
#endif
            JsTypeDefinition jstypedef = new JsTypeDefinition("AA");
            jstypedef.AddMember(new JsMethodDefinition("B", args =>
            {
                args.SetResult(100);
            }));
            jstypedef.AddMember(new JsMethodDefinition("C", args =>
            {
                args.SetResult(true);
            }));

            jstypedef.AddMember(new JsPropertyDefinition("D",
                args =>
                {   //getter
                    args.SetResult(true);
                },
                args =>
                {
                    //setter 
                }));
            jstypedef.AddMember(new JsPropertyDefinition("E",
                args =>
                {   //getter
                    args.SetResult(250);
                },
                args =>
                {
                    //setter 
                }));

            //===============================================================
            //create js engine and context
            using (JsEngine engine = new JsEngine())
            using (JsContext ctx = engine.CreateContext())
            {

                ctx.RegisterTypeDefinition(jstypedef);

                GC.Collect();
                System.Diagnostics.Stopwatch stwatch = new System.Diagnostics.Stopwatch();
                stwatch.Reset();
                stwatch.Start();

                TestMe1 t1 = new TestMe1();
                var proxy = ctx.CreateWrapper(t1, jstypedef);

                //for (int i = 2000; i >= 0; --i)
                //{
                ctx.SetVariableFromAny("x", proxy);
                //object result = ctx.Execute("(function(){if(x.C()){return  x.B();}else{return 0;}})()");
                object result = ctx.Execute("(function(){if(x.D){ x.E=300; return  x.B();}else{return 0;}})()");

                //}
                stwatch.Stop();

                Console.WriteLine("met1 template:" + stwatch.ElapsedMilliseconds.ToString());
                //Assert.That(result, Is.EqualTo(100));
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
#if DEBUG
            JsBridge.dbugTestCallbacks();
#endif
            JsTypeDefinition jstypedef = new JsTypeDefinition("AA");
            jstypedef.AddMember(new JsMethodDefinition("B", args =>
            {
                var argCount = args.ArgCount;
                var thisArg = args.GetThisArg();
                var arg0 = args.GetArgAsObject(0);

                args.SetResult(100);
            }));
            jstypedef.AddMember(new JsMethodDefinition("C", args =>
            {
                args.SetResult(true);
            }));

            //----------------------------------------------------- 
            jstypedef.AddMember(new JsPropertyDefinition("D",
                args =>
                {
                    //getter
                    TestMe1 t2 = new TestMe1();
                    args.SetResultObj(t2, jstypedef);

                },
                args =>
                {
                    //setter 
                }));
            jstypedef.AddMember(new JsPropertyDefinition("E",
                args =>
                {   //getter
                    args.SetResult(250);
                },
                args =>
                {
                    //setter 
                }));

            //===============================================================
            //create js engine and context
            using (JsEngine engine = new JsEngine())
            using (JsContext ctx = engine.CreateContext())
            {

                ctx.RegisterTypeDefinition(jstypedef);

                GC.Collect();
                System.Diagnostics.Stopwatch stwatch = new System.Diagnostics.Stopwatch();
                stwatch.Reset();
                stwatch.Start();

                TestMe1 t1 = new TestMe1();
                INativeScriptable proxy = ctx.CreateWrapper(t1, jstypedef);

                ctx.SetVariable("x", proxy);

                //string testsrc = "(function(){if(x.C()){return  x.B();}else{return 0;}})()";
                //string testsrc = "(function(){if(x.D != null){ x.E=300; return  x.B();}else{return 0;}})()";
                string testsrc = "x.B(x.D,15);";

                object result = ctx.Execute(testsrc);
                stwatch.Stop();

                Console.WriteLine("met1 template:" + stwatch.ElapsedMilliseconds.ToString());

            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
#if DEBUG
            JsBridge.dbugTestCallbacks();
#endif
            JsTypeDefinition jstypedef = new JsTypeDefinition("AA");
            jstypedef.AddMember(new JsMethodDefinition("B", args =>
            {
                var argCount = args.ArgCount;
                var thisArg = args.GetThisArg();
                var arg0 = args.GetArgAsObject(0);
                args.SetResult((bool)arg0);

            }));
            jstypedef.AddMember(new JsMethodDefinition("C", args =>
            {
                args.SetResult(true);
            }));

            //----------------------------------------------------- 
            jstypedef.AddMember(new JsPropertyDefinition("D",
                args =>
                {
                    var ab = new AboutMe();
                    args.SetResultAutoWrap(ab);
                },
                args =>
                {
                    //setter 
                }));
            jstypedef.AddMember(new JsPropertyDefinition("E",
                args =>
                {   //getter
                    args.SetResult(250);
                },
                args =>
                {
                    //setter 
                }));
            //===============================================================
            //create js engine and context 
            using (JsEngine engine = new JsEngine())
            using (JsContext ctx = engine.CreateContext())
            {

                ctx.RegisterTypeDefinition(jstypedef);

                GC.Collect();
                System.Diagnostics.Stopwatch stwatch = new System.Diagnostics.Stopwatch();
                stwatch.Reset();
                stwatch.Start();


                TestMe1 t1 = new TestMe1();

                INativeScriptable proxy = ctx.CreateWrapper(t1, jstypedef);
                ctx.SetVariable("x", proxy);


                string testsrc = "x.B(x.D.IsOK);";
                object result = ctx.Execute(testsrc);
                stwatch.Stop();

                Console.WriteLine("met1 template:" + stwatch.ElapsedMilliseconds.ToString());

            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
#if DEBUG
            JsBridge.dbugTestCallbacks();
#endif
            using (JsEngine engine = new JsEngine())
            using (JsContext ctx = engine.CreateContext(new MyJsTypeDefinitionBuilder()))
            {

                GC.Collect();
                System.Diagnostics.Stopwatch stwatch = new System.Diagnostics.Stopwatch();
                stwatch.Reset();
                stwatch.Start();

                var ab = new AboutMe();
                ctx.SetVariableAutoWrap("x", ab);

                //string testsrc = "x.IsOK;";
                string testsrc = "x.Test2('AAA');";
                object result = ctx.Execute(testsrc);
                stwatch.Stop();

                Console.WriteLine("met1 template:" + stwatch.ElapsedMilliseconds.ToString());

            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
#if DEBUG
            JsBridge.dbugTestCallbacks();
#endif
            using (JsEngine engine = new JsEngine())
            using (JsContext ctx = engine.CreateContext(new MyJsTypeDefinitionBuilder()))
            {

                GC.Collect();
                System.Diagnostics.Stopwatch stwatch = new System.Diagnostics.Stopwatch();
                stwatch.Reset();
                stwatch.Start();

                var ab = new AboutMe();
                ctx.SetVariableAutoWrap("x", ab);

                //string testsrc = "x.IsOK;";
                string testsrc = "x.NewAboutMe();";
                object result = ctx.Execute(testsrc);
                stwatch.Stop();

                Console.WriteLine("met1 template:" + stwatch.ElapsedMilliseconds.ToString());

            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            int version = JsBridge.LibVersion;
            JsBridge.dbugTestCallbacks();
            using (JsEngine engine = new JsEngine())
            using (JsContext ctx = engine.CreateContext(new MyJsTypeDefinitionBuilder()))
            {

                GC.Collect();
                System.Diagnostics.Stopwatch stwatch = new System.Diagnostics.Stopwatch();
                stwatch.Reset();
                stwatch.Start();

                var ab = new AboutMe();
                ctx.SetVariableAutoWrap("x", ab);

                string testsrc = @"(function(){
                    x.AttachEvent('mousedown',function(evArgs){});
                    x.FireEventMouseDown({});
                })()";
                object result = ctx.Execute(testsrc);
                stwatch.Stop();

                Console.WriteLine("met1 template:" + stwatch.ElapsedMilliseconds.ToString());
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            //------------------------
            //test esprima package
            //------------------------

            string esprima_code = File.ReadAllText("d:\\projects\\Espresso\\js_tools\\esprima\\esprima.js");
            StringBuilder stbuilder = new StringBuilder();
            stbuilder.Append(esprima_code);


            int version = JsBridge.LibVersion;
            JsBridge.dbugTestCallbacks();
            using (JsEngine engine = new JsEngine())
            using (JsContext ctx = engine.CreateContext(new MyJsTypeDefinitionBuilder()))
            {

                GC.Collect();
                System.Diagnostics.Stopwatch stwatch = new System.Diagnostics.Stopwatch();
                stwatch.Reset();
                stwatch.Start();

                var ab = new AboutMe();
                ctx.SetVariableAutoWrap("aboutme1", ab);

                string testsrc = @"(function(){
                            var syntax= esprima.parse('var answer = 42');
                            //convert to json format and send to managed side
                            aboutme1.SetResult(JSON.stringify(syntax, null, 4));         
                    })()";
                stbuilder.Append(testsrc);
                ctx.Execute(stbuilder.ToString());

                stwatch.Stop();
                Console.WriteLine("met1 template:" + stwatch.ElapsedMilliseconds.ToString());
            }

        }

        private void button10_Click(object sender, EventArgs e)
        {
            //very basic ***
            //-----------------
            //test tsc.js
            //this needs EspressoHostForTsc 
            //-----------------
            string esprima_code = File.ReadAllText("d:\\projects\\Espresso\\js_tools\\tsc\\tsc_espr.js");
            StringBuilder stbuilder = new StringBuilder();
            stbuilder.Append(esprima_code);
            //-----------------

            int version = JsBridge.LibVersion;
            JsBridge.dbugTestCallbacks();
            using (JsEngine engine = new JsEngine())
            using (JsContext ctx = engine.CreateContext(new MyJsTypeDefinitionBuilder()))
            {

                GC.Collect();
                System.Diagnostics.Stopwatch stwatch = new System.Diagnostics.Stopwatch();
                stwatch.Reset();
                stwatch.Start();

                var my_expr_ext = new EspressoHostForTsc();
                ctx.SetVariableAutoWrap("my_expr_ext", my_expr_ext);

                string testsrc = @"(function(){
                       
                        // test1: general  compile through commamd line
                        // ts.executeCommandLine(['greeter.ts']);
                        //-------------------------------------------------
                        // test 2: generate ast 
                        var filename=""greeter.ts"";  //example only
                        //parse
                        const sourceFile = ts.createSourceFile(filename,
                        my_expr_ext.ReadFile(filename),2, false);
                        //send output as json to managed host
                        my_expr_ext.ConsoleLog(JSON.stringify( sourceFile));
                    })()";
                stbuilder.Append(testsrc);
                ctx.Execute(stbuilder.ToString());

                stwatch.Stop();
                Console.WriteLine("met1 template:" + stwatch.ElapsedMilliseconds.ToString());
            }
        }


        [JsType]
        class EspressoHostForTsc
        {
            //-------------------------------------
            //function getEspressoSystem()
            //{
            //    var args = JSON.parse(my_expr_ext.GetArgs());//array of args
            //    function readFile(fileName, encoding) {
            //        return my_expr_ext.ReadFile(fileName);
            //    } 
            //    function writeFile(fileName, data, writeByteOrderMark) {
            //        my_expr_ext.WriteFile(fileName, data);
            //    }
            //    function getDirectories(path) {
            //        //return array of sub dirs
            //        return JSON.parse(my_expr_ext.GetDirectories(path));
            //    }
            //    function getAccessibleFileSystemEntries(path) {
            //        //return { files: files, directories: directories };
            //        return JSON.parse(my_expr_ext.GetAccessibleFileSystemEntries(path));
            //    }
            //    function readDirectory(path, extensions, excludes, includes) {
            //        return ts.matchFiles(path, extensions, excludes, includes, false, my_expr_ext.GetCurrentDir(), getAccessibleFileSystemEntries);
            //    }

            //    var wscriptSystem = {
            //        args: args,
            //    newLine: "\r\n",
            //    useCaseSensitiveFileNames: false,
            //    write: function(s) {
            //            //WScript.StdOut.Write(s);
            //            my_expr_ext.ConsoleWrite(s);
            //        },
            //    readFile: readFile,
            //    writeFile: writeFile,
            //    resolvePath: function(path) {
            //            return my_expr_ext.GetAbsolutePathName(path);
            //        },
            //    fileExists: function(path) {
            //            return my_expr_ext.FileExists(path);
            //        },
            //    directoryExists: function(path) {
            //            return my_expr_ext.FolderExists(path);
            //        },
            //    createDirectory: function(directoryName) {
            //            if (!wscriptSystem.directoryExists(directoryName))
            //            {
            //                my_expr_ext.CreateFolder(directoryName);
            //            }
            //        },
            //    getExecutingFilePath: function() {
            //            //return WScript.ScriptFullName;
            //            return my_expr_ext.GetScriptFullName();
            //        },
            //    getCurrentDirectory: function() {
            //            return my_expr_ext.GetCurrentDir();
            //        },
            //    getDirectories: getDirectories,
            //    readDirectory: readDirectory,
            //    exit: function(exitCode) {
            //            try
            //            {
            //                //WScript.Quit(exitCode);
            //                my_expr_ext.Quit(exitCode);
            //            }
            //            catch (e)
            //            {
            //            }
            //        }
            //    };
            //    return wscriptSystem;
            //}
            //-------------------------------------

            [JsMethod]
            public string GetArgs()
            {
                //return json string
                return "[]";
            }
            [JsMethod]
            public void RemoveFile(string filename)
            {
                //danger !!!!
            }
            [JsMethod]
            public string ReadFile(string filename)
            {
                //string sampleFileContent1 = @"function greeter(person) {
                //                           return ""Hello, "" + person;
                //                     }
                //                     var user = ""Jane User"";
                //                     document.body.innerHTML = greeter(user);";

                if (filename == "greeter.ts")
                {
                    string sampleFileContent2 =
                        @"
                        class Student {
                            fullName: string;
                            constructor(public firstName, public middleInitial, public lastName) {
                                this.fullName = firstName + "" "" + middleInitial + "" "" + lastName;
                            }
                         }

                        interface Person
                        {
                            firstName: string;
                            lastName: string;
                        }

                        function greeter(person : Person)
                        {
                            return ""Hello, "" + person.firstName + "" "" + person.lastName;
                        }

                        var user = new Student(""Jane"", ""M."",""User"");
                        document.body.innerHTML = greeter(user);
                    ";
                    return sampleFileContent2;
                }
                else
                {
                    return "";
                }
            }
            [JsMethod]
            public void WriteFile(string filename, string data)
            {
                Console.WriteLine("req: write " + filename);
                Console.WriteLine("");
                Console.WriteLine("=== compiler output===");
                Console.WriteLine("");
                Console.WriteLine(data);
                Console.WriteLine("");
                Console.WriteLine("======");
                Console.WriteLine("");
            }
            [JsMethod]
            public string GetDirectories(string path)
            {
                //get directory
                return "";
            }
            [JsMethod]
            public string GetAccessibleFileSystemEntries(string path)
            {
                //return { files: files, directories: directories };
                //        return JSON.parse(my_expr_ext.GetAccessibleFileSystemEntries(path));
                //get directory

                return "";
            }
            [JsMethod]
            public string GetCurrentDir()
            {
                return ".";
            }
            [JsMethod]
            public bool FileExists(string filename)
            {
                return false;
            }
            [JsMethod]
            public bool FolderExists(string folderName)
            {
                return false;
            }
            [JsMethod]
            public string GetScriptFullName()
            {
                return "hello1.ts";
            }
            [JsMethod]
            public void CreateFolder(string directoryName)
            {

            }
            [JsMethod]
            public void Quit(int exitCode)
            {
                Console.WriteLine("quit:" + exitCode);
            }
            [JsMethod]
            public void ConsoleWrite(string msg)
            {
                Console.WriteLine("console write :" + msg);
            }
            [JsMethod]
            public void ConsoleLog(object o)
            {
                Console.WriteLine("console write :" + o.ToString());
            }
            [JsMethod]
            public object Require(string module)
            {
                //for require()
                if (module == "fs")
                {
                    return this;
                }
                else
                {
                    return null;
                }
            }

        }

        [JsType]
        class EspressoHostForLoki
        {
            ////------------------------------------------------------------
            ////my extension
            //function LokiEsprAdapter()
            //{
            //} 
            //LokiEsprAdapter.prototype.loadDatabase = function(dbname, callback)
            //{
            //    //Read from filesystem
            //    var content = my_expr_ext.ReadFile(dbname);
            //    if (content === "")
            //    {
            //        callback(new Error("DB file does not exist"));
            //    }
            //    else
            //    {
            //        callback(content);
            //    }
            //}; 
            //LokiEsprAdapter.prototype.saveDatabase = function(dbname, serialized, callback)
            //{
            //    my_expr_ext.WriteFile(dbname, serialized);
            //    callback();
            //};

            //LokiEsprAdapter.prototype.deleteDatabase = function deleteDatabase(dbname, callback)
            //{
            //    my_expr_ext.RemoveFile(dbname);
            //    callback();
            //};
            ////------------------------------------------------------------
            public string ReadFile(string filename)
            {
                return "";
            }
            [JsMethod]
            public void WriteFile(string filename, string data)
            {
                Console.WriteLine("loki: write " + filename);
                Console.WriteLine("");
                Console.WriteLine("=== output===");
                Console.WriteLine("");
                Console.WriteLine(data);
                Console.WriteLine("");
                Console.WriteLine("======");
                Console.WriteLine("");
            }
            [JsMethod]
            public void ConsoleLog(object o)
            {
                Console.WriteLine(o.ToString());
            }
            [JsMethod]
            public object Require(string module)
            {
                //for require()
                if (module == "fs")
                {
                    return this;
                }
                else
                {
                    return null;
                }
            }
        }
        private void button11_Click(object sender, EventArgs e)
        {
            //very basic ***
            //-----------------
            //test loki.js
            //this needs EspressoHostForTsc 
            //-----------------
            string esprima_code = File.ReadAllText("d:\\projects\\Espresso\\js_tools\\lokijs\\lokijs.js");
            StringBuilder stbuilder = new StringBuilder();
            stbuilder.Append(esprima_code);
            //-----------------

            int version = JsBridge.LibVersion;
            JsBridge.dbugTestCallbacks();
            using (JsEngine engine = new JsEngine())
            using (JsContext ctx = engine.CreateContext(new MyJsTypeDefinitionBuilder()))
            {

                GC.Collect();
                System.Diagnostics.Stopwatch stwatch = new System.Diagnostics.Stopwatch();
                stwatch.Reset();
                stwatch.Start();

                var my_expr_ext = new EspressoHostForLoki();
                ctx.SetVariableAutoWrap("my_expr_ext", my_expr_ext);
                string testsrc = @"
                    function require(file){
                        my_expr_ext.Require(file);
                    }
                    (function(){
                       var db = new loki('loki.json');
                       //test log
                       my_expr_ext.ConsoleLog(db);
                       db.save();
                    })()";
                stbuilder.Append(testsrc);
                ctx.Execute(stbuilder.ToString());

                stwatch.Stop();
                Console.WriteLine("met1 template:" + stwatch.ElapsedMilliseconds.ToString());
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {

        }
    }
}
