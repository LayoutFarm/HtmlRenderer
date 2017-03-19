using System;
using System.IO; 
using Espresso;

namespace TestNode01
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //-----------------------------------
            //1.
            //after we build nodejs in dll version
            //we will get node.dll
            //then just copy it to another name 'libespr'   
            string libEspr = @"C:\projects\node-v6.7.0\Release\libespr.dll";
            if (File.Exists(libEspr))
            {
                //delete the old one
                File.Delete(libEspr);
            }
            File.Copy(
               @"C:\projects\node-v6.7.0\Release\node.dll", //from
               libEspr);
            //-----------------------------------
            //2. load libespr.dll (node.dll)
            IntPtr intptr = LoadLibrary(libEspr);
            int errCode = GetLastError();
#if DEBUG
            JsBridge.dbugTestCallbacks();
#endif
            //------------ 
            JsEngine.RunJsEngine((IntPtr nativeEngine, IntPtr nativeContext) =>
            {

                JsEngine eng = new JsEngine(nativeEngine);
                JsContext ctx = eng.CreateContext(nativeContext);
                //-------------
                //this LibEspressoClass object is need,
                //so node can talk with us,
                //-------------
                JsTypeDefinition jstypedef = new JsTypeDefinition("LibEspressoClass");
                jstypedef.AddMember(new JsMethodDefinition("LoadMainSrcFile", args =>
                {
                    string filedata = @"var http = require('http');
                                                (function t(){
	                                                console.log('hello from EspressoCup');
	                                                var server = http.createServer(function(req, res) {
                                                    res.writeHead(200);
                                                    res.end('Hello! from EspressoCup');
                                                    });
                                                    server.listen(8080,'localhost');
                                                })();";
                    args.SetResult(filedata);
                }));
                jstypedef.AddMember(new JsMethodDefinition("C", args =>
                {

                    args.SetResult(true);
                }));
                jstypedef.AddMember(new JsMethodDefinition("E", args =>
                {
                    args.SetResult(true);
                }));
                if (!jstypedef.IsRegisterd)
                {
                    ctx.RegisterTypeDefinition(jstypedef);
                }
                //----------
                //then register this as x***       
                //this object is just an instance for reference        
                ctx.SetVariableFromAny("LibEspresso",
                      ctx.CreateWrapper(new object(), jstypedef));
            });
        }


        private static void Proc_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {

        }

        private static void Proc_ErrorDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {

        }

        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        static extern IntPtr LoadLibrary(string dllname);
        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        static extern int GetLastError();
    }
}
